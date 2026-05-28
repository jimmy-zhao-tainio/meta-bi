using MetaBi.Tests.Common;
using MetaOrchestration.Core;
using MetaPipeline;
using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql;

namespace MetaOrchestration.Tests;

public sealed class MetaOrchestrationAnalysisServiceTests
{
    [Fact]
    public void CliHelp_ShowsRunPlanExecutionCommand()
    {
        var result = RunCli("--help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("execute", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("add-dependency", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CliExecuteHelp_ShowsWorkspaceAndThrottle()
    {
        var result = RunCli("execute --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--schedule-workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--max-degree-of-parallelism", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Options:", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Required. MetaOrchestration workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Refreshes run-plan rows", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("meta-pipeline execute-step", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CliExecute_FailsBeforeWorkspaceLoad_WhenPipelineDbConnectionEnvIsMissing()
    {
        var envName = "__META_ORCHESTRATION_TEST_MISSING_" + Guid.NewGuid().ToString("N");

        var result = RunCli(
            "execute " +
            "--workspace NoSuchOrchestration " +
            "--pipeline-workspace NoSuchPipeline " +
            "--transform-workspace NoSuchTransform " +
            "--binding-workspace NoSuchBinding " +
            $"--pipeline-db-connection-env {envName}");

        Assert.NotEqual(0, result.ExitCode);
        Assert.Contains("Cannot execute orchestration.", result.Output, StringComparison.Ordinal);
        Assert.Contains($"Connection environment variable '{envName}' was not found.", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("RunPlan:", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("FirstFailedTask", result.Output, StringComparison.Ordinal);
        Assert.DoesNotContain("SkippedTask", result.Output, StringComparison.Ordinal);
    }

    [Fact]
    public void CliAddDependencyHelp_ShowsConditionalDependencyShape()
    {
        var result = RunCli("add-dependency --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--condition success|failure", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Options:", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Failure edges", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--from-task", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CliWorkspaceCreationHelp_UsesNewWorkspaceForInferenceOnly()
    {
        var help = RunCli("--help");
        var runPlan = RunCli("refresh-run-plan --help");

        Assert.Equal(0, help.ExitCode);
        Assert.Equal(0, runPlan.ExitCode);
        Assert.Contains("--new-workspace", help.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--new-workspace", runPlan.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--plan", help.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--schedule", runPlan.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--out", help.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("--out", runPlan.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Analyze_InferProducerConsumerDependency_WhenTargetIsReadByAnotherPipeline()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("load-stage", "SELECT CustomerId FROM dbo.RawCustomer", "dbo.StageCustomer"),
                ("load-dim", "SELECT CustomerId FROM dbo.StageCustomer", "dbo.DimCustomer"));
            BuildBindingWorkspace(
                bindingWorkspace,
                (ResolveScript(transformModel, "load-stage"), ["dbo.RawCustomer"], "dbo.StageCustomer"),
                (ResolveScript(transformModel, "load-dim"), ["dbo.StageCustomer"], "dbo.DimCustomer"));
            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "StageCustomer", Script: ResolveScript(transformModel, "load-stage"), InsertRowsTarget: "dbo.StageCustomer"),
                (PipelineName: "DimCustomer", Script: ResolveScript(transformModel, "load-dim"), InsertRowsTarget: "dbo.DimCustomer"));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.True(result.IsCompleteDag);
            var edge = Assert.Single(result.Dependencies);
            Assert.Equal("pipeline:StageCustomer", edge.PredecessorPipelineId);
            Assert.Equal("pipeline:DimCustomer", edge.SuccessorPipelineId);
            Assert.Empty(result.Issues);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_AllowsScalarFunctionDefinitionsInTransformWorkspace_WhenPipelineUsesView()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("dbo.fnCustomerOrderCount", """
CREATE FUNCTION dbo.fnCustomerOrderCount
(
    @CustomerId INT
)
RETURNS BIGINT
AS
BEGIN
    RETURN
    (
        SELECT COUNT_BIG(*)
        FROM dbo.[Order] AS o
        WHERE o.CustomerId = @CustomerId
    );
END
""", null),
                ("load-orders", "SELECT CustomerId FROM dbo.RawOrder", "dbo.Order"),
                ("dbo.v_LoadStage", """
CREATE VIEW dbo.v_LoadStage AS
SELECT
    s.CustomerId,
    dbo.fnCustomerOrderCount(s.CustomerId) AS OrderCount
FROM dbo.Source AS s
""", "dbo.StageCustomer"));

            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspace,
                bindingWorkspace);
            Assert.Equal(0, bindingResult.ErrorCount);
            Assert.Contains(
                bindingResult.Model.TransformBindingList,
                item => string.Equals(item.TransformScriptName, "dbo.fnCustomerOrderCount", StringComparison.OrdinalIgnoreCase));

            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "Orders", Script: ResolveScript(transformModel, "load-orders"), InsertRowsTarget: "dbo.Order"),
                (PipelineName: "StageCustomer", Script: ResolveScript(transformModel, "dbo.v_LoadStage"), InsertRowsTarget: "dbo.StageCustomer"));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.True(result.IsCompleteDag);
            Assert.Empty(result.Issues);
            var dependency = Assert.Single(result.Dependencies);
            Assert.Equal("pipeline:Orders", dependency.PredecessorPipelineId);
            Assert.Equal("pipeline:StageCustomer", dependency.SuccessorPipelineId);

            var pipeline = Assert.Single(result.Pipelines, item => string.Equals(item.PipelineName, "StageCustomer", StringComparison.OrdinalIgnoreCase));
            var transformTask = Assert.Single(pipeline.Tasks, item => string.Equals(item.TransformScriptName, "dbo.v_LoadStage", StringComparison.OrdinalIgnoreCase));
            Assert.Equal(BoundStatementKind.Select.ToString(), transformTask.StatementKind);
            Assert.Contains(transformTask.ObjectAccesses, item =>
                string.Equals(item.SqlIdentifier, "dbo.Source", StringComparison.OrdinalIgnoreCase) &&
                item.AccessKind == OrchestrationObjectAccessKind.Read);
            Assert.Contains(transformTask.ObjectAccesses, item =>
                string.Equals(item.SqlIdentifier, "dbo.Order", StringComparison.OrdinalIgnoreCase) &&
                item.AccessKind == OrchestrationObjectAccessKind.Read);
            Assert.Contains(transformTask.ObjectAccesses, item =>
                string.Equals(item.SqlIdentifier, "dbo.StageCustomer", StringComparison.OrdinalIgnoreCase) &&
                item.AccessKind == OrchestrationObjectAccessKind.Write);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_BlocksScalarFunctionDefinitionsAsPipelineTasks_WithSpecificIssue()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("dbo.fnAddOne", """
CREATE FUNCTION dbo.fnAddOne
(
    @value INT
)
RETURNS INT
AS
BEGIN
    RETURN @value + 1;
END
""", null));
            var bindingResult = new TransformBindingWorkspaceService().BindToWorkspace(
                transformWorkspace,
                bindingWorkspace);
            Assert.Equal(0, bindingResult.ErrorCount);

            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "ScalarFunctionTask", Script: ResolveScript(transformModel, "dbo.fnAddOne"), InsertRowsTarget: null));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.False(result.IsCompleteDag);
            var issue = Assert.Single(result.Issues);
            Assert.Equal(OrchestrationIssueCode.NonExecutableTransformScript, issue.Code);
            Assert.True(issue.BlocksDag);
            Assert.True(issue.BlocksAutomaticRunPlanning);
            Assert.Contains("scalar function transform script 'dbo.fnAddOne'", issue.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("unsupported for binding-driven orchestration", issue.Message, StringComparison.OrdinalIgnoreCase);

            var task = Assert.Single(Assert.Single(result.Pipelines).Tasks);
            Assert.Equal(BoundStatementKind.ScalarFunction.ToString(), task.StatementKind);
            Assert.Empty(task.ObjectAccesses);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_DoesNotBlockPrivateResetWrite()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("truncate-private", "TRUNCATE TABLE dbo.PrivateScratch", null));
            BuildBindingWorkspace(
                bindingWorkspace,
                (ResolveScript(transformModel, "truncate-private"), [], "dbo.PrivateScratch"));
            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "CleanupScratch", Script: ResolveScript(transformModel, "truncate-private"), InsertRowsTarget: null));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.True(result.IsCompleteDag);
            Assert.Empty(result.Dependencies);
            Assert.Empty(result.Issues);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_BlocksResetWrite_WhenAnotherPipelineTouchesSameObject()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");
            var orchestrationWorkspace = Path.Combine(tempRoot, "Orchestration");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("truncate-stage", "TRUNCATE TABLE dbo.StageCustomer", null),
                ("read-stage", "SELECT CustomerId FROM dbo.StageCustomer", "dbo.DimCustomer"));
            BuildBindingWorkspace(
                bindingWorkspace,
                (ResolveScript(transformModel, "truncate-stage"), [], "dbo.StageCustomer"),
                (ResolveScript(transformModel, "read-stage"), ["dbo.StageCustomer"], "dbo.DimCustomer"));
            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "TruncateStage", Script: ResolveScript(transformModel, "truncate-stage"), InsertRowsTarget: null),
                (PipelineName: "ReadStage", Script: ResolveScript(transformModel, "read-stage"), InsertRowsTarget: "dbo.DimCustomer"));

            var service = new MetaOrchestrationAnalysisService();
            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);
            var model = service.CreateModel(result, pipelineWorkspace);
            model.SaveToXmlWorkspace(orchestrationWorkspace);
            var reloaded = MetaOrchestrationModel.LoadFromXmlWorkspace(orchestrationWorkspace, searchUpward: false);

            Assert.False(result.IsCompleteDag);
            var issue = Assert.Single(result.Issues);
            Assert.Equal(OrchestrationIssueCode.UnsafeSharedReset, issue.Code);
            Assert.True(issue.BlocksDag);
            Assert.Equal("Invalid", Assert.Single(reloaded.OrchestrationPlanList).DagStatus);
            Assert.Contains(reloaded.DependencyIssueList, item => item.Code == "UnsafeSharedReset");
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Analyze_SameTargetAppendWriters_KeepCompleteDagWithSynchronizationNote()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("load-stage-a", "SELECT CustomerId FROM dbo.RawCustomerA", "dbo.StageCustomer"),
                ("load-stage-b", "SELECT CustomerId FROM dbo.RawCustomerB", "dbo.StageCustomer"),
                ("read-stage", "SELECT CustomerId FROM dbo.StageCustomer", "dbo.Downstream"));
            BuildBindingWorkspace(
                bindingWorkspace,
                (ResolveScript(transformModel, "load-stage-a"), ["dbo.RawCustomerA"], "dbo.StageCustomer"),
                (ResolveScript(transformModel, "load-stage-b"), ["dbo.RawCustomerB"], "dbo.StageCustomer"),
                (ResolveScript(transformModel, "read-stage"), ["dbo.StageCustomer"], "dbo.Downstream"));
            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "StageA", Script: ResolveScript(transformModel, "load-stage-a"), InsertRowsTarget: "dbo.StageCustomer"),
                (PipelineName: "StageB", Script: ResolveScript(transformModel, "load-stage-b"), InsertRowsTarget: "dbo.StageCustomer"),
                (PipelineName: "ReadStage", Script: ResolveScript(transformModel, "read-stage"), InsertRowsTarget: "dbo.Downstream"));

            var result = Analyze(pipelineWorkspace, transformWorkspace, bindingWorkspace);

            Assert.True(result.IsCompleteDag);
            Assert.Equal("Complete", result.DagStatus);
            Assert.Equal("HasConstraints", result.SynchronizationStatus);
            Assert.Contains(result.Dependencies, item =>
                item.PredecessorPipelineId == "pipeline:StageA" &&
                item.SuccessorPipelineId == "pipeline:ReadStage");
            Assert.Contains(result.Dependencies, item =>
                item.PredecessorPipelineId == "pipeline:StageB" &&
                item.SuccessorPipelineId == "pipeline:ReadStage");
            var issue = Assert.Single(result.Issues);
            Assert.Equal(OrchestrationIssueCode.SharedAppendWritersRequirePolicy, issue.Code);
            Assert.Equal(OrchestrationIssueDomain.Synchronization, issue.Domain);
            Assert.False(issue.BlocksDag);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void Analyze_InferIndependentTableChains_WithReplaceSequence()
    {
        var result = AnalyzeProfiles(
            Profile(
                "WriteA",
                Task("WriteA", 1, "write-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "ReadA",
                Task("ReadA", 1, "read-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source"))),
            Profile(
                "RefreshB",
                Task("RefreshB", 1, "truncate-b", "Truncate", Access("dbo.B", OrchestrationObjectAccessKind.ResetWrite, "Target")),
                Task("RefreshB", 2, "write-b", "Select", Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "ReadB",
                Task("ReadB", 1, "read-b", "Select", Access("dbo.B", OrchestrationObjectAccessKind.Read, "Source"))));

        Assert.True(result.IsCompleteDag);
        Assert.Empty(result.Issues);
        Assert.Contains(result.Dependencies, item =>
            item.PredecessorPipelineId == "pipeline:WriteA" &&
            item.SuccessorPipelineId == "pipeline:ReadA");
        Assert.Contains(result.Dependencies, item =>
            item.PredecessorPipelineId == "pipeline:RefreshB" &&
            item.SuccessorPipelineId == "pipeline:ReadB");
    }

    [Fact]
    public void Analyze_ReplacementMixedWithAppend_RequiresExplicitOrderingWithoutInvalidDag()
    {
        var result = AnalyzeProfiles(
            Profile(
                "RefreshA",
                Task("RefreshA", 1, "truncate-a", "Truncate", Access("dbo.A", OrchestrationObjectAccessKind.ResetWrite, "Target")),
                Task("RefreshA", 2, "write-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "AppendA",
                Task("AppendA", 1, "append-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "ReadA",
                Task("ReadA", 1, "read-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source"))));

        Assert.True(result.IsCompleteDag);
        Assert.Equal("Complete", result.DagStatus);
        Assert.Equal("RequiresExplicitOrdering", result.DeterminismStatus);
        Assert.Contains(result.Dependencies, item =>
            item.PredecessorPipelineId == "pipeline:RefreshA" &&
            item.SuccessorPipelineId == "pipeline:ReadA");
        Assert.Contains(result.Dependencies, item =>
            item.PredecessorPipelineId == "pipeline:AppendA" &&
            item.SuccessorPipelineId == "pipeline:ReadA");
        Assert.Contains(result.Issues, item =>
            item.Code == OrchestrationIssueCode.WriteOrderAmbiguity &&
            item.Domain == OrchestrationIssueDomain.Determinism &&
            !item.BlocksDag &&
            item.BlocksAutomaticRunPlanning);
    }

    [Fact]
    public void Analyze_TwoSameTableMutators_DoNotCreateArtificialDependencyCycle()
    {
        var result = AnalyzeProfiles(
            Profile(
                "UpdateA",
                Task("UpdateA", 1, "update-a", "Update", Access("dbo.A", OrchestrationObjectAccessKind.ReadWrite, "Target"))),
            Profile(
                "MergeA",
                Task("MergeA", 1, "merge-a", "Merge", Access("dbo.A", OrchestrationObjectAccessKind.ReadWrite, "Target"))));

        Assert.True(result.IsCompleteDag);
        Assert.Empty(result.Dependencies);
        Assert.Equal("RequiresExplicitOrdering", result.DeterminismStatus);
        Assert.Contains(result.Issues, item =>
            item.Code == OrchestrationIssueCode.WriteOrderAmbiguity &&
            item.Domain == OrchestrationIssueDomain.Determinism);
        Assert.Contains(result.Issues, item =>
            item.Code == OrchestrationIssueCode.SynchronizationRequired &&
            item.Domain == OrchestrationIssueDomain.Synchronization);
    }

    [Fact]
    public void Analyze_CrossTableProducerConsumerCycle_InvalidatesDag()
    {
        var result = AnalyzeProfiles(
            Profile(
                "P1",
                Task(
                    "P1",
                    1,
                    "load-a-from-b",
                    "Select",
                    Access("dbo.B", OrchestrationObjectAccessKind.Read, "Source"),
                    Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "P2",
                Task(
                    "P2",
                    1,
                    "load-b-from-a",
                    "Select",
                    Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source"),
                    Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))));

        Assert.False(result.IsCompleteDag);
        Assert.Equal("Invalid", result.DagStatus);
        Assert.Contains(result.Issues, item =>
            item.Code == OrchestrationIssueCode.DependencyCycle &&
            item.Domain == OrchestrationIssueDomain.Dependency &&
            item.BlocksDag);
    }

    [Fact]
    public void Analyze_InferredMemberRepair_RequiresSynchronizationWithoutFalseFactPipelineDependency()
    {
        var result = AnalyzeProfiles(
            Profile(
                "LoadInternetSales",
                Task(
                    "LoadInternetSales",
                    1,
                    "load-internet-sales",
                    "Select",
                    Access("dbo.DimCustomer", OrchestrationObjectAccessKind.ReadWrite, "InferredMemberRepair"),
                    Access("dbo.FactInternetSales", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
            Profile(
                "LoadStoreSales",
                Task(
                    "LoadStoreSales",
                    1,
                    "load-store-sales",
                    "Select",
                    Access("dbo.DimCustomer", OrchestrationObjectAccessKind.ReadWrite, "InferredMemberRepair"),
                    Access("dbo.FactStoreSales", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))));

        Assert.True(result.IsCompleteDag);
        Assert.Empty(result.Dependencies);
        Assert.DoesNotContain(result.TaskDependencies, item =>
            item.PredecessorPipelineId != item.SuccessorPipelineId);
        Assert.Contains(result.TaskObjectEffects, item =>
            item.ObjectKey == "DBO.DIMCUSTOMER" &&
            item.WriteEffect == OrchestrationWriteEffect.ConditionalKeyedUpsert &&
            item.AccessPurpose == OrchestrationAccessPurpose.InferredMemberRepair &&
            !item.IsPublishedProducer &&
            item.RequiresSynchronization);
        Assert.Contains(result.Issues, item =>
            item.Code == OrchestrationIssueCode.SynchronizationRequired &&
            item.Domain == OrchestrationIssueDomain.Synchronization &&
            !item.BlocksDag);
    }

    [Fact]
    public void RunPlan_IndependentTasks_AreDependencyOrdered()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "LoadA",
                    Task("LoadA", 1, "load-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "LoadB",
                    Task("LoadB", 1, "load-b", "Select", Access("dbo.RawB", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "LoadC",
                    Task("LoadC", 1, "load-c", "Select", Access("dbo.RawC", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.C", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

        var result = new MetaOrchestrationRunPlanningService().BuildRunPlan(model);

        Assert.Equal("Ready", result.Status);
        Assert.Equal(3, result.PlannedTasks);
        Assert.Equal(3, model.PlannedTaskList.Count);
    }

    [Fact]
    public void CliRefreshRunPlan_WritesRunPlanRowsIntoExistingWorkspace()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var orchestrationWorkspace = Path.Combine(tempRoot, "Orchestration");
            var model = CreateModel(
                AnalyzeProfiles(
                    Profile(
                        "LoadA",
                        Task("LoadA", 1, "load-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                    Profile(
                        "LoadB",
                        Task("LoadB", 1, "load-b", "Select", Access("dbo.RawB", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));
            model.SaveToXmlWorkspace(orchestrationWorkspace);

            var result = RunCli($"refresh-run-plan --workspace \"{orchestrationWorkspace}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("Ok", result.Output, StringComparison.Ordinal);
            var reloaded = MetaOrchestrationModel.LoadFromXmlWorkspace(orchestrationWorkspace, searchUpward: false);
            var runPlan = Assert.Single(reloaded.RunPlanList);
            Assert.Equal("DefaultRunPlan", runPlan.Name);
            Assert.Equal(2, reloaded.PlannedTaskList.Count);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    [Fact]
    public void AddTaskDependency_RecordsFailureConditionAsDagEdge()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "LoadA",
                    Task("LoadA", 1, "load-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "FailureHandler",
                    Task("FailureHandler", 1, "record-failure", "Insert", Access("dbo.OrchestrationFailureLog", OrchestrationObjectAccessKind.Write, "Target")))));
        var service = new MetaOrchestrationRunPlanningService();

        var result = service.AddTaskOrderingResolution(
            model,
            "LoadA.load-a",
            "FailureHandler.record-failure",
            null,
            "Run the modeled failure branch.",
            "failure");

        Assert.Equal("Added", result.Action);
        var resolution = Assert.Single(model.TaskOrderingResolutionList);
        Assert.Equal("OnFailure", resolution.DependencyCondition);
        Assert.Equal("ExplicitTaskDependency", resolution.ResolutionKind);
        Assert.Equal("Active", resolution.Status);
        Assert.Equal("LoadA.load-a", $"{resolution.Predecessor.PipelineReference.Name}.{resolution.Predecessor.TaskName}");
        Assert.Equal("FailureHandler.record-failure", $"{resolution.Successor.PipelineReference.Name}.{resolution.Successor.TaskName}");

        service.BuildRunPlan(model);

        Assert.Equal(["LoadA.load-a", "FailureHandler.record-failure"], PlannedTaskNamesInOrder(model));
    }

    [Fact]
    public void ExecuteContinuity_BlocksOnlyDownstreamDependents_WhenOneTaskFails()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "HRExtract",
                    Task("HRExtract", 1, "extract-hr", "Select", Access("dbo.RawHR", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.HRStage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "HRDimension",
                    Task("HRDimension", 1, "load-hr-dim", "Select", Access("dbo.HRStage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.HRDim", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "FinanceExtract",
                    Task("FinanceExtract", 1, "extract-finance", "Select", Access("dbo.RawFinance", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.FinanceStage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "FinanceBudget",
                    Task("FinanceBudget", 1, "load-budget", "Select", Access("dbo.FinanceStage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Budget", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "HRFailureHandler",
                    Task("HRFailureHandler", 1, "record-failure", "Insert", Access("dbo.HRFailureLog", OrchestrationObjectAccessKind.Write, "Target")))));
        var service = new MetaOrchestrationRunPlanningService();
        service.AddTaskOrderingResolution(model, "HRExtract.extract-hr", "HRFailureHandler.record-failure", null, "Run HR failure branch.", "failure");
        service.BuildRunPlan(model);
        var dependencies = OrchestrationExecutionContinuity.BuildDependencyMap(model);
        var taskOutcomes = new Dictionary<string, string>(StringComparer.Ordinal);
        var plannedTasks = PlannedTaskRows(model);
        var hrExtract = plannedTasks.Single(item => item.PipelineReference.Name == "HRExtract");
        var financeExtract = plannedTasks.Single(item => item.PipelineReference.Name == "FinanceExtract");
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(hrExtract, dependencies, taskOutcomes, out _, out _, out _));
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(financeExtract, dependencies, taskOutcomes, out _, out _, out _));

        var failedHrExtract = hrExtract;
        taskOutcomes[failedHrExtract.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Failed;
        taskOutcomes[financeExtract.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded;

        var hrDimension = plannedTasks.Single(item => item.PipelineReference.Name == "HRDimension");
        var financeBudget = plannedTasks.Single(item => item.PipelineReference.Name == "FinanceBudget");
        var hrFailureHandler = plannedTasks.Single(item => item.PipelineReference.Name == "HRFailureHandler");
        Assert.Equal(OrchestrationTaskReadiness.Skip, OrchestrationExecutionContinuity.EvaluateReadiness(hrDimension, dependencies, taskOutcomes, out var blockedDependency, out var blockedOutcome, out _));
        Assert.Equal(failedHrExtract.TaskAccessProfile.Id, blockedDependency.PredecessorTaskProfileId);
        Assert.Equal("OnSuccess", blockedDependency.Condition);
        Assert.Equal(OrchestrationExecutionContinuity.SkippedBlocked, blockedOutcome);
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(financeBudget, dependencies, taskOutcomes, out _, out _, out _));
        Assert.Equal(OrchestrationTaskReadiness.Ready, OrchestrationExecutionContinuity.EvaluateReadiness(hrFailureHandler, dependencies, taskOutcomes, out _, out _, out _));
    }

    [Fact]
    public void ExecuteContinuity_SkipsFailureBranch_WhenPredecessorSucceeds()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "LoadA",
                    Task("LoadA", 1, "load-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "FailureHandler",
                    Task("FailureHandler", 1, "record-failure", "Insert", Access("dbo.FailureLog", OrchestrationObjectAccessKind.Write, "Target")))));
        var service = new MetaOrchestrationRunPlanningService();
        service.AddTaskOrderingResolution(model, "LoadA.load-a", "FailureHandler.record-failure", null, "Run failure branch.", "failure");
        service.BuildRunPlan(model);
        var dependencies = OrchestrationExecutionContinuity.BuildDependencyMap(model);
        var plannedTasks = PlannedTaskRows(model);
        var loadA = plannedTasks.Single(item => item.PipelineReference.Name == "LoadA");
        var failureHandler = plannedTasks.Single(item => item.PipelineReference.Name == "FailureHandler");
        var taskOutcomes = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [loadA.TaskAccessProfile.Id] = OrchestrationExecutionContinuity.Succeeded
        };

        Assert.Equal(OrchestrationTaskReadiness.Skip, OrchestrationExecutionContinuity.EvaluateReadiness(failureHandler, dependencies, taskOutcomes, out var dependency, out var skipOutcome, out _));
        Assert.Equal("OnFailure", dependency.Condition);
        Assert.Equal(OrchestrationExecutionContinuity.SkippedConditionNotMet, skipOutcome);
    }


    [Fact]
    public void RunPlan_ProducerConsumer_OrdersProducerBeforeConsumer()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "WriteA",
                    Task("WriteA", 1, "write-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "ReadA",
                    Task("ReadA", 1, "read-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

        var result = new MetaOrchestrationRunPlanningService().BuildRunPlan(model);

        Assert.Equal(2, result.PlannedTasks);
        Assert.Equal(["WriteA.write-a", "ReadA.read-a"], PlannedTaskNamesInOrder(model));
    }

    [Fact]
    public void RunPlan_DiamondGraph_KeepsIndependentMiddleTasksTogether()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "Seed",
                    Task("Seed", 1, "seed", "Select", Access("dbo.Raw", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Stage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "Dim",
                    Task("Dim", 1, "dim", "Select", Access("dbo.Stage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Dim", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "Fact",
                    Task("Fact", 1, "fact", "Select", Access("dbo.Stage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Fact", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "Mart",
                    Task("Mart", 1, "mart", "Select", Access("dbo.Dim", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Fact", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Mart", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

        var result = new MetaOrchestrationRunPlanningService().BuildRunPlan(model);

        Assert.Equal(4, result.PlannedTasks);
        var plannedTasks = PlannedTaskNamesInOrder(model);
        Assert.Equal("Seed.seed", plannedTasks[0]);
        Assert.True(Array.IndexOf(plannedTasks, "Dim.dim") > Array.IndexOf(plannedTasks, "Seed.seed"));
        Assert.True(Array.IndexOf(plannedTasks, "Fact.fact") > Array.IndexOf(plannedTasks, "Seed.seed"));
        Assert.Equal("Mart.mart", plannedTasks[^1]);
    }

    [Fact]
    public void RunPlan_ReplacementMixedWithAppend_RequiresOrderingResolution()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "RefreshA",
                    Task("RefreshA", 1, "truncate-a", "Truncate", Access("dbo.A", OrchestrationObjectAccessKind.ResetWrite, "Target")),
                    Task("RefreshA", 2, "write-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "AppendA",
                    Task("AppendA", 1, "append-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "ReadA",
                    Task("ReadA", 1, "read-a", "Select", Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source")))));
        var service = new MetaOrchestrationRunPlanningService();

        var failure = Assert.Throws<InvalidOperationException>(() => service.BuildRunPlan(model));
        Assert.Contains("require explicit run-planning policy", failure.Message, StringComparison.Ordinal);

        service.AddTaskOrderingResolution(model, "RefreshA.write-a", "AppendA.append-a", "dbo.A", "Refresh before append.");
        service.BuildRunPlan(model);

        Assert.Equal(["RefreshA.truncate-a", "RefreshA.write-a", "AppendA.append-a", "ReadA.read-a"], PlannedTaskNamesInOrder(model));
    }

    [Fact]
    public void RunPlan_SameTargetAppendWriters_RecordConcurrentLockPolicy()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "StageA",
                    Task("StageA", 1, "append-a", "Select", Access("dbo.RawA", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Stage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "StageB",
                    Task("StageB", 1, "append-b", "Select", Access("dbo.RawB", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Stage", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "ReadStage",
                    Task("ReadStage", 1, "read-stage", "Select", Access("dbo.Stage", OrchestrationObjectAccessKind.Read, "Source"), Access("dbo.Downstream", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));
        var service = new MetaOrchestrationRunPlanningService();

        service.AddConcurrentAppendPolicy(model, "dbo.Stage", "Stage writes are append-only.");
        service.BuildRunPlan(model);

        var plannedTasks = PlannedTaskNamesInOrder(model);
        Assert.True(Array.IndexOf(plannedTasks, "StageA.append-a") < Array.IndexOf(plannedTasks, "ReadStage.read-stage"));
        Assert.True(Array.IndexOf(plannedTasks, "StageB.append-b") < Array.IndexOf(plannedTasks, "ReadStage.read-stage"));
        Assert.Single(model.LockCompatibilityPolicyList);
        Assert.All(model.PlannedTaskLockList.Where(item => item.DataObject.SqlIdentifier == "dbo.Stage"), item => Assert.NotNull(item.Reason));
    }

    [Fact]
    public void RunPlan_ConditionalKeyedUpsert_RequiresScopedLockPolicy()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "LoadInternetSales",
                    Task(
                        "LoadInternetSales",
                        1,
                        "load-internet-sales",
                        "Select",
                        Access("dbo.DimCustomer", OrchestrationObjectAccessKind.ReadWrite, "InferredMemberRepair"),
                        Access("dbo.FactInternetSales", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "LoadStoreSales",
                    Task(
                        "LoadStoreSales",
                        1,
                        "load-store-sales",
                        "Select",
                        Access("dbo.DimCustomer", OrchestrationObjectAccessKind.ReadWrite, "InferredMemberRepair"),
                        Access("dbo.FactStoreSales", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));
        var service = new MetaOrchestrationRunPlanningService();

        var failure = Assert.Throws<InvalidOperationException>(() => service.BuildRunPlan(model));
        Assert.Contains("require explicit run-planning policy", failure.Message, StringComparison.Ordinal);
        Assert.Equal("Complete", Assert.Single(model.OrchestrationPlanList).DagStatus);

        service.AddLockCompatibilityPolicy(
            model,
            "dbo.DimCustomer",
            "ConditionalKeyedUpsert",
            "ConditionalKeyedUpsert",
            "serialize",
            "No modeled unique-key/atomic upsert proof yet.");
        service.BuildRunPlan(model);

        Assert.Equal(["LoadInternetSales.load-internet-sales", "LoadStoreSales.load-store-sales"], PlannedTaskNamesInOrder(model));
        Assert.All(model.PlannedTaskLockList.Where(item => item.DataObject.SqlIdentifier == "dbo.DimCustomer"), item => Assert.NotNull(item.LockCompatibilityPolicy));
    }

    [Fact]
    public void RunPlan_InvalidDag_Throws()
    {
        var model = CreateModel(
            AnalyzeProfiles(
                Profile(
                    "P1",
                    Task(
                        "P1",
                        1,
                        "load-a-from-b",
                        "Select",
                        Access("dbo.B", OrchestrationObjectAccessKind.Read, "Source"),
                        Access("dbo.A", OrchestrationObjectAccessKind.Write, "InsertRowsTarget"))),
                Profile(
                    "P2",
                    Task(
                        "P2",
                        1,
                        "load-b-from-a",
                        "Select",
                        Access("dbo.A", OrchestrationObjectAccessKind.Read, "Source"),
                        Access("dbo.B", OrchestrationObjectAccessKind.Write, "InsertRowsTarget")))));

        var failure = Assert.Throws<InvalidOperationException>(() => new MetaOrchestrationRunPlanningService().BuildRunPlan(model));

        Assert.Contains("DagStatus is 'Invalid'", failure.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CliInfer_CreatesWorkspaceAndReturnsNonZero_WhenDagIsInvalid()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var transformWorkspace = Path.Combine(tempRoot, "Transform");
            var bindingWorkspace = Path.Combine(tempRoot, "Binding");
            var pipelineWorkspace = Path.Combine(tempRoot, "Pipeline");
            var orchestrationWorkspace = Path.Combine(tempRoot, "Orchestration");

            var transformModel = await BuildTransformWorkspaceAsync(
                transformWorkspace,
                ("truncate-stage", "TRUNCATE TABLE dbo.StageCustomer", null),
                ("read-stage", "SELECT CustomerId FROM dbo.StageCustomer", "dbo.DimCustomer"));
            BuildBindingWorkspace(
                bindingWorkspace,
                (ResolveScript(transformModel, "truncate-stage"), [], "dbo.StageCustomer"),
                (ResolveScript(transformModel, "read-stage"), ["dbo.StageCustomer"], "dbo.DimCustomer"));
            BuildPipelineWorkspace(
                pipelineWorkspace,
                (PipelineName: "TruncateStage", Script: ResolveScript(transformModel, "truncate-stage"), InsertRowsTarget: null),
                (PipelineName: "ReadStage", Script: ResolveScript(transformModel, "read-stage"), InsertRowsTarget: "dbo.DimCustomer"));

            var result = RunCli($"--pipeline-workspace \"{pipelineWorkspace}\" --transform-workspace \"{transformWorkspace}\" --binding-workspace \"{bindingWorkspace}\" --new-workspace \"{orchestrationWorkspace}\"");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("Cannot continue", result.Output, StringComparison.Ordinal);
            var model = MetaOrchestrationModel.LoadFromXmlWorkspace(orchestrationWorkspace, searchUpward: false);
            Assert.Equal("Invalid", Assert.Single(model.OrchestrationPlanList).DagStatus);
            Assert.Single(model.DependencyIssueList);
        }
        finally
        {
            DeleteDirectoryIfExists(tempRoot);
        }
    }

    private static OrchestrationAnalysisResult Analyze(
        string pipelineWorkspace,
        string transformWorkspace,
        string bindingWorkspace)
    {
        return new MetaOrchestrationAnalysisService().Analyze(
            new OrchestrationAnalysisRequest(
                pipelineWorkspace,
                transformWorkspace,
                bindingWorkspace,
                "Default"));
    }

    private static OrchestrationAnalysisResult AnalyzeProfiles(params PipelineDependencyProfile[] profiles)
    {
        return new MetaOrchestrationAnalysisService().AnalyzeProfiles("Default", null, profiles);
    }

    private static MetaOrchestrationModel CreateModel(OrchestrationAnalysisResult result)
    {
        return new MetaOrchestrationAnalysisService().CreateModel(result, Path.Combine(Path.GetTempPath(), "MetaOrchestration.Tests", "Pipeline"));
    }

    private static string[] PlannedTaskNamesInOrder(MetaOrchestrationModel model)
    {
        return PlannedTaskRows(model)
            .Select(static item => $"{item.PipelineReference.Name}.{item.TaskAccessProfile.TaskName}")
            .ToArray();
    }

    private static PlannedTask[] PlannedTaskRows(MetaOrchestrationModel model)
    {
        return model.PlannedTaskList
            .OrderBy(static item => int.Parse(item.Ordinal))
            .ToArray();
    }

    private static PipelineDependencyProfile Profile(string pipelineName, params PipelineTaskAccessProfile[] tasks)
    {
        var objectAccesses = tasks
            .SelectMany(static item => item.ObjectAccesses)
            .GroupBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var first = group.OrderBy(static item => item.SqlIdentifier, StringComparer.OrdinalIgnoreCase).First();
                return new PipelineObjectAccessProfile(
                    first.SqlIdentifier,
                    first.ObjectKey,
                    AggregateAccessKind(group.Select(static item => item.AccessKind)),
                    "Pipeline",
                    string.Join("; ", group.Select(static item => $"{item.AccessRole}:{item.AccessKind}")));
            })
            .OrderBy(static item => item.ObjectKey, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new PipelineDependencyProfile(
            $"pipeline:{pipelineName}",
            pipelineName,
            tasks,
            objectAccesses,
            []);
    }

    private static PipelineTaskAccessProfile Task(
        string pipelineName,
        int ordinal,
        string taskName,
        string statementKind,
        params PipelineObjectAccessProfile[] accesses)
    {
        return new PipelineTaskAccessProfile(
            $"pipeline:{pipelineName}:task:{ordinal}",
            taskName,
            ordinal,
            $"script:{pipelineName}:{taskName}",
            taskName,
            $"binding:{pipelineName}:{taskName}",
            statementKind,
            accesses);
    }

    private static PipelineObjectAccessProfile Access(
        string sqlIdentifier,
        OrchestrationObjectAccessKind accessKind,
        string accessRole)
    {
        return new PipelineObjectAccessProfile(
            sqlIdentifier,
            sqlIdentifier.ToUpperInvariant(),
            accessKind,
            accessRole,
            accessRole);
    }

    private static OrchestrationObjectAccessKind AggregateAccessKind(IEnumerable<OrchestrationObjectAccessKind> kinds)
    {
        var set = kinds.ToHashSet();
        if (set.Contains(OrchestrationObjectAccessKind.ResetWrite)) return OrchestrationObjectAccessKind.ResetWrite;
        if (set.Contains(OrchestrationObjectAccessKind.ReadWrite)) return OrchestrationObjectAccessKind.ReadWrite;
        if (set.Contains(OrchestrationObjectAccessKind.Write)) return OrchestrationObjectAccessKind.Write;
        return OrchestrationObjectAccessKind.Read;
    }

    private static async Task<MetaTransformScriptModel> BuildTransformWorkspaceAsync(
        string transformWorkspace,
        params (string Name, string Sql, string? TargetSqlIdentifier)[] scripts)
    {
        var service = new MetaTransformScriptSqlService();
        for (var index = 0; index < scripts.Length; index++)
        {
            var script = scripts[index];
            if (index == 0)
            {
                await service.ImportFromSqlCodeToWorkspaceAsync(
                    script.Sql,
                    script.TargetSqlIdentifier,
                    transformWorkspace,
                    script.Name);
            }
            else
            {
                await service.AddSqlCodeToWorkspaceAsync(
                    script.Sql,
                    script.TargetSqlIdentifier,
                    transformWorkspace,
                    script.Name);
            }
        }

        return MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspace, searchUpward: false);
    }

    private static void BuildBindingWorkspace(
        string bindingWorkspace,
        params (TransformScript Script, IReadOnlyList<string> Sources, string? Target)[] bindings)
    {
        var model = MetaTransformBindingModel.CreateEmpty();
        foreach (var bindingSeed in bindings)
        {
            var binding = new TransformBinding
            {
                Id = $"{bindingSeed.Script.Id}:binding",
                MetaTransformScriptTransformScriptId = bindingSeed.Script.Id,
                TransformScriptName = bindingSeed.Script.Name,
            };
            model.TransformBindingList.Add(binding);

            foreach (var source in bindingSeed.Sources.Select((value, index) => (value, index)))
            {
                model.RowsetList.Add(new Rowset
                {
                    Id = $"{binding.Id}:source:{source.index + 1}",
                    TransformBinding = binding,
                    Name = source.value,
                    DerivationKind = "Source",
                    SqlIdentifier = source.value,
                });
            }

            if (!string.IsNullOrWhiteSpace(bindingSeed.Target))
            {
                model.TransformBindingTargetList.Add(new TransformBindingTarget
                {
                    Id = $"{binding.Id}:target:1",
                    TransformBinding = binding,
                    SqlIdentifier = bindingSeed.Target,
                });
                var targetRowset = new Rowset
                {
                    Id = $"{binding.Id}:target-rowset",
                    TransformBinding = binding,
                    Name = bindingSeed.Target,
                    DerivationKind = "Target",
                    SqlIdentifier = bindingSeed.Target,
                };
                model.RowsetList.Add(targetRowset);
                model.OutputRowsetList.Add(new OutputRowset
                {
                    Id = $"{binding.Id}:output",
                    TransformBinding = binding,
                    Rowset = targetRowset,
                });
            }
        }

        model.SaveToXmlWorkspace(bindingWorkspace);
    }

    private static void BuildPipelineWorkspace(
        string pipelineWorkspace,
        params (string PipelineName, TransformScript Script, string? InsertRowsTarget)[] pipelines)
    {
        var model = MetaPipelineModel.CreateEmpty();
        foreach (var pipelineSeed in pipelines)
        {
            var pipeline = new Pipeline
            {
                Id = $"pipeline:{pipelineSeed.PipelineName}",
                Name = pipelineSeed.PipelineName,
            };
            model.PipelineList.Add(pipeline);
            var connection = new ConnectionReference
            {
                Id = $"{pipeline.Id}:connection:execution",
                Pipeline = pipeline,
                Name = "Execution",
                EnvironmentVariableName = "EXECUTION_SQL",
            };
            model.ConnectionReferenceList.Add(connection);
            var transformTask = new PipelineTask
            {
                Id = $"{pipeline.Id}:task:transform",
                Pipeline = pipeline,
                Name = "transform",
                Ordinal = "1",
            };
            model.PipelineTaskList.Add(transformTask);
            model.TransformExecutionTaskList.Add(new TransformExecutionTask
            {
                Id = $"{transformTask.Id}:execution",
                PipelineTask = transformTask,
                ExecutionConnectionReference = connection,
                TransformScriptId = pipelineSeed.Script.Id,
                TransformBindingId = $"{pipelineSeed.Script.Id}:binding",
            });

            if (string.IsNullOrWhiteSpace(pipelineSeed.InsertRowsTarget))
            {
                continue;
            }

            var rowStream = new RowStream
            {
                Id = $"{pipeline.Id}:rowstream:1",
                Pipeline = pipeline,
                Name = "transform.rows",
            };
            model.RowStreamList.Add(rowStream);
            model.RowStreamProducerList.Add(new RowStreamProducer
            {
                Id = $"{transformTask.Id}:producer",
                PipelineTask = transformTask,
                RowStream = rowStream,
            });

            var targetWriteTask = new PipelineTask
            {
                Id = $"{pipeline.Id}:task:target-write",
                Pipeline = pipeline,
                Name = "target-write",
                Ordinal = "2",
            };
            model.PipelineTaskList.Add(targetWriteTask);
            var targetWrite = new TargetWriteTask
            {
                Id = $"{targetWriteTask.Id}:target-write",
                PipelineTask = targetWriteTask,
                TargetConnectionReference = connection,
            };
            model.TargetWriteTaskList.Add(targetWrite);
            model.InsertRowsTargetWriteTaskList.Add(new InsertRowsTargetWriteTask
            {
                Id = $"{targetWrite.Id}:insert-rows",
                TargetWriteTask = targetWrite,
                TargetSqlIdentifier = pipelineSeed.InsertRowsTarget,
            });
            model.RowStreamConsumerList.Add(new RowStreamConsumer
            {
                Id = $"{targetWriteTask.Id}:consumer",
                PipelineTask = targetWriteTask,
                RowStream = rowStream,
            });
            model.TaskDependencyList.Add(new MetaPipeline.TaskDependency
            {
                Id = $"{transformTask.Id}:before:{targetWriteTask.Id}",
                Pipeline = pipeline,
                Predecessor = transformTask,
                Successor = targetWriteTask,
            });
        }

        model.SaveToXmlWorkspace(pipelineWorkspace);
    }

    private static TransformScript ResolveScript(MetaTransformScriptModel model, string name) =>
        model.TransformScriptList.Single(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));

    private static (int ExitCode, string Output) RunCli(string arguments, string? pathPrefix = null) =>
        CliTestRunner.RunStandardCli("MetaOrchestration", "meta-orchestration.exe", arguments, pathPrefix);

    private static string CreateTempRoot()
    {
        return Path.Combine(Path.GetTempPath(), "MetaOrchestration.Tests", Guid.NewGuid().ToString("N"));
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
