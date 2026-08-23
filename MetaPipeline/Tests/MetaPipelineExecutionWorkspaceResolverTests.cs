using MetaTransformBinding;
using MetaTransform.Binding;
using MetaTransformScript;
using MetaTransformScript.Sql;

namespace MetaPipeline.Tests;

public sealed class MetaPipelineExecutionWorkspaceResolverTests
{
    [Fact]
    public async Task ResolveByIds_WhenScriptIdIsBlank_RequiresExplicitScriptId()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            var sqlService = new MetaTransformScriptSqlService();
            await sqlService.ImportFromSqlCodeToXmlWorkspaceAsync(
                "select 1 as CustomerId",
                "dbo.CustomerTarget",
                transformWorkspacePath,
                "dbo.v_customer_one");
            await sqlService.AddSqlCodeToWorkspaceAsync(
                "select 2 as CustomerId",
                "dbo.CustomerTargetTwo",
                transformWorkspacePath,
                "dbo.v_customer_two");

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var first = transformModel.TransformScriptList.Single(item => string.Equals(item.Name, "dbo.v_customer_one", StringComparison.OrdinalIgnoreCase));
            var second = transformModel.TransformScriptList.Single(item => string.Equals(item.Name, "dbo.v_customer_two", StringComparison.OrdinalIgnoreCase));

            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", first, "dbo.CustomerTarget", ["CustomerId"]),
                new BindingSeed("binding:2", second, "dbo.CustomerTargetTwo", ["CustomerId"]));

            var exception = Assert.Throws<ArgumentException>(() =>
                new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(transformWorkspacePath, bindingWorkspacePath, string.Empty, "binding:1"));

            Assert.Contains("transformScriptId", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Resolve_UsesBindingTargetAndOrderedOutputColumns()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "select 1 as CustomerId, 'A' as CustomerName",
                "dbo.CustomerLoad",
                transformWorkspacePath,
                "dbo.v_customer_load");

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);

            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, "warehouse.CustomerLoad", ["CustomerName", "CustomerId"], [1, 0]));

            var result = new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                transformWorkspacePath,
                bindingWorkspacePath,
                script.Id,
                "binding:1");

            Assert.Equal("dbo.v_customer_load", result.TransformScriptName);
            Assert.Equal(script.Id, result.TransformScriptId);
            Assert.Equal("binding:1", result.TransformBindingId);
            Assert.Equal("warehouse.CustomerLoad", result.TargetSqlIdentifier);
            Assert.Collection(
                result.Columns,
                column =>
                {
                    Assert.Equal("CustomerId", column.Name);
                    Assert.Equal(0, column.Ordinal);
                },
                column =>
                {
                    Assert.Equal("CustomerName", column.Name);
                    Assert.Equal(1, column.Ordinal);
                });
            Assert.Contains("select", result.SourceSql, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task ResolveSelection_UsesScriptNameAndSingleBinding()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "select 1 as CustomerId",
                "dbo.CustomerLoad",
                transformWorkspacePath,
                "dbo.v_customer_load");

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);
            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:customer-load", script, "warehouse.CustomerLoad", ["CustomerId"]));

            var selection = new MetaPipelineTransformSelectionResolver().Resolve(
                transformWorkspacePath,
                bindingWorkspacePath,
                "dbo.v_customer_load");

            Assert.Equal(script.Id, selection.TransformScriptId);
            Assert.Equal("dbo.v_customer_load", selection.TransformScriptName);
            Assert.Equal("binding:customer-load", selection.TransformBindingId);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task ResolveSelection_WhenScriptHasMultipleBindingsRequiresBinding()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "select 1 as CustomerId",
                "dbo.CustomerLoad",
                transformWorkspacePath,
                "dbo.v_customer_load");

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);
            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:primary", script, "warehouse.CustomerLoad", ["CustomerId"]),
                new BindingSeed("binding:alternate", script, "warehouse.CustomerLoadReplica", ["CustomerId"]));

            var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
                new MetaPipelineTransformSelectionResolver().Resolve(
                    transformWorkspacePath,
                    bindingWorkspacePath,
                    "dbo.v_customer_load"));

            Assert.Contains("--binding", exception.Message, StringComparison.Ordinal);

            var selection = new MetaPipelineTransformSelectionResolver().Resolve(
                transformWorkspacePath,
                bindingWorkspacePath,
                "dbo.v_customer_load",
                "binding:alternate");

            Assert.Equal("binding:alternate", selection.TransformBindingId);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Resolve_UsesValidationTypeMetadataForOutputColumns()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "select cast(1 as int) as CustomerId",
                "dbo.CustomerLoad",
                transformWorkspacePath,
                "dbo.v_customer_load");

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);

            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed(
                    "binding:1",
                    script,
                    "warehouse.CustomerLoad",
                    ["CustomerId"],
                    [0],
                    [("sqlserver:type:int", "sqlserver:type:nvarchar")]));

            var result = new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                transformWorkspacePath,
                bindingWorkspacePath,
                script.Id,
                "binding:1");

            var column = Assert.Single(result.Columns);
            Assert.Equal("CustomerId", column.Name);
            Assert.Equal("sqlserver:type:int", column.SourceMetaDataTypeId);
            Assert.Equal("sqlserver:type:nvarchar", column.TargetMetaDataTypeId);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Resolve_WhenStoredProcedureDeclaresResultRowset_UsesExplicitTargetAsRowStream()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                CREATE PROCEDURE dq.ExportCustomers
                AS
                BEGIN
                    SELECT CustomerId, CustomerName FROM src.Customer;
                END
                """,
                null,
                transformWorkspacePath);

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);
            AddStoredProcedureContractResultRowset(transformModel, script, ["CustomerId", "CustomerName"]);
            Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(transformModel, transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindStructureToXmlWorkspace(
                transformWorkspacePath,
                bindingWorkspacePath);
            Assert.Equal(0, bindingResult.ErrorCount);
            var binding = Assert.Single(bindingResult.Model.TransformBindingList);
            Assert.Empty(bindingResult.Model.TransformBindingTargetList);

            var result = new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                transformWorkspacePath,
                bindingWorkspacePath,
                script.Id,
                binding.Id,
                "warehouse.CustomerLoad");

            Assert.True(result.IsSelect);
            Assert.Equal("warehouse.CustomerLoad", result.TargetSqlIdentifier);
            Assert.Contains("EXEC dq.ExportCustomers", result.SourceSql, StringComparison.OrdinalIgnoreCase);
            Assert.Collection(
                result.Columns,
                column =>
                {
                    Assert.Equal("CustomerId", column.Name);
                    Assert.Equal(0, column.Ordinal);
                },
                column =>
                {
                    Assert.Equal("CustomerName", column.Name);
                    Assert.Equal(1, column.Ordinal);
                });
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Resolve_WhenStoredProcedureResultRowsetHasNoTarget_FailsClearly()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                CREATE PROCEDURE dq.ExportCustomers
                AS
                BEGIN
                    SELECT CustomerId FROM src.Customer;
                END
                """,
                null,
                transformWorkspacePath);

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);
            AddStoredProcedureContractResultRowset(transformModel, script, ["CustomerId"]);
            Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(transformModel, transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindStructureToXmlWorkspace(
                transformWorkspacePath,
                bindingWorkspacePath);
            Assert.Equal(0, bindingResult.ErrorCount);
            var binding = Assert.Single(bindingResult.Model.TransformBindingList);

            var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
                new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                    transformWorkspacePath,
                    bindingWorkspacePath,
                    script.Id,
                    binding.Id));

            Assert.Contains("returns a rowset", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("explicit target SQL identifier", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task ResolveModeled_WhenStoredProcedureResultFeedsTargetWrite_ReturnsRowStreamPlan()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");
        var pipelineWorkspacePath = Path.Combine(tempRoot, "pipeline");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                CREATE PROCEDURE dq.ExportCustomers
                AS
                BEGIN
                    SELECT CustomerId, CustomerName FROM src.Customer;
                END
                """,
                null,
                transformWorkspacePath);

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);
            AddStoredProcedureContractResultRowset(transformModel, script, ["CustomerId", "CustomerName"]);
            Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(transformModel, transformWorkspacePath);

            var bindingResult = new TransformBindingWorkspaceService().BindStructureToXmlWorkspace(
                transformWorkspacePath,
                bindingWorkspacePath);
            Assert.Equal(0, bindingResult.ErrorCount);
            var binding = Assert.Single(bindingResult.Model.TransformBindingList);
            BuildTransformToTargetWritePipelineWorkspace(
                pipelineWorkspacePath,
                script.Id,
                binding.Id,
                transformWorkspacePath,
                bindingWorkspacePath,
                "warehouse.CustomerLoad",
                ["CustomerId", "CustomerName"]);

            var plan = new MetaPipelineModeledExecutionResolver().Resolve(
                new MetaPipelineModeledExecutionRequest(
                    pipelineWorkspacePath,
                    "CustomerLoad"));

            Assert.True(plan.IsSelect);
            Assert.Equal("warehouse.CustomerLoad", plan.TargetSqlIdentifier);
            Assert.Equal("InsertRows", plan.TargetWriteModelName);
            Assert.Equal(1000, plan.BatchSize);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }


    [Fact]
    public async Task Resolve_WhenTransformScriptHasParameters_FailsForStageOne()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "create function dbo.fn_customer(@CustomerId int) returns table as return (select @CustomerId as CustomerId)",
                null,
                transformWorkspacePath);

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);

            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, "warehouse.CustomerLoad", ["CustomerId"]));

            var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
                new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                    transformWorkspacePath,
                    bindingWorkspacePath,
                    script.Id,
                    "binding:1"));

            Assert.Contains("parameterless transform scripts only", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Resolve_WhenSelectedBindingHasMultipleTargets_RequiresTarget()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "select 1 as CustomerId",
                "dbo.CustomerLoad",
                transformWorkspacePath,
                "dbo.v_customer_load");

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);

            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, ["warehouse.CustomerLoad", "warehouse.CustomerLoadReplica"], ["CustomerId"]));

            var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
                new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                    transformWorkspacePath,
                    bindingWorkspacePath,
                    script.Id,
                    "binding:1"));

            Assert.Contains("Use --target", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task Resolve_WithTarget_SelectsMatchingBindingTarget()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "select 1 as CustomerId",
                "dbo.CustomerLoad",
                transformWorkspacePath,
                "dbo.v_customer_load");

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);

            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, ["warehouse.CustomerLoad", "warehouse.CustomerLoadReplica"], ["CustomerId"]));

            var result = new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                transformWorkspacePath,
                bindingWorkspacePath,
                script.Id,
                "binding:1",
                "warehouse.CustomerLoadReplica");

            Assert.Equal("warehouse.CustomerLoadReplica", result.TargetSqlIdentifier);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task ResolveByIds_WhenScriptIsNotSelect_RequiresBindingButNoTargetWrite()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "UPDATE dbo.Target SET Name = 'A' WHERE Id = 1",
                null,
                transformWorkspacePath,
                "update-target");

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);
            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, "dbo.Target", []));

            var result = new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                transformWorkspacePath,
                bindingWorkspacePath,
                script.Id,
                "binding:1");

            Assert.False(result.IsSelect);
            Assert.Equal("binding:1", result.TransformBindingId);
            Assert.Null(result.TargetSqlIdentifier);
            Assert.Null(result.RowStreamShape);
            Assert.Contains("UPDATE dbo.Target", result.SourceSql, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task ResolveByIds_WhenScriptIsScalarFunction_FailsAsNonExecutableHelperObject()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                """
CREATE FUNCTION dbo.fnAddOne
(
    @value INT
)
RETURNS INT
AS
BEGIN
    RETURN @value + 1;
END
""",
                null,
                transformWorkspacePath,
                "dbo.fnAddOne");

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);
            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, [], []));

            var ex = Assert.Throws<MetaPipelineConfigurationException>(() =>
                new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                    transformWorkspacePath,
                    bindingWorkspacePath,
                    script.Id,
                    "binding:1"));

            Assert.Contains("scalar function definition", ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("cannot be executed as pipeline transform steps", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task ResolveByIds_WhenScriptIsNotSelectRejectsInsertRowsTargetSelection()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "DELETE FROM dbo.Target WHERE Id = 1",
                null,
                transformWorkspacePath,
                "delete-target");

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);
            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, "dbo.Target", []));

            var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
                new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                    transformWorkspacePath,
                    bindingWorkspacePath,
                    script.Id,
                    "binding:1",
                    "dbo.MissingTarget"));

            Assert.Contains("Target 'dbo.MissingTarget' was not found", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task ResolveModeled_WhenNonSelectHasNoTargetWrite_ReturnsScriptOnlyPlan()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");
        var pipelineWorkspacePath = Path.Combine(tempRoot, "pipeline");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "TRUNCATE TABLE dbo.Target",
                null,
                transformWorkspacePath,
                "truncate-target");
            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);
            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, "dbo.Target", []));
            BuildTransformOnlyPipelineWorkspace(pipelineWorkspacePath, script.Id, "binding:1", transformWorkspacePath, bindingWorkspacePath, "45");

            var plan = new MetaPipelineModeledExecutionResolver().Resolve(
                new MetaPipelineModeledExecutionRequest(
                    pipelineWorkspacePath,
                    "CustomerLoad"));

            Assert.False(plan.IsSelect);
            Assert.Equal("binding:1", plan.TransformBindingId);
            Assert.Null(plan.TargetWriteTaskId);
            Assert.Null(plan.TargetWriteTaskName);
            Assert.Equal("None", plan.TargetWriteModelName);
            Assert.Equal(0, plan.BatchSize);
            Assert.Equal(45, plan.TimeoutSeconds);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task ResolveModeled_WhenPipelineHasSerialTransforms_ReturnsOrderedSteps()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");
        var pipelineWorkspacePath = Path.Combine(tempRoot, "pipeline");

        try
        {
            var sqlService = new MetaTransformScriptSqlService();
            await sqlService.ImportFromSqlCodeToXmlWorkspaceAsync(
                "UPDATE dbo.Target SET Name = 'A' WHERE Id = 1",
                null,
                transformWorkspacePath,
                "update-target");
            await sqlService.AddSqlCodeToWorkspaceAsync(
                "DELETE FROM dbo.Target WHERE Id = 2",
                null,
                transformWorkspacePath,
                "delete-target");
            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var updateScript = transformModel.TransformScriptList.Single(item => item.Name == "update-target");
            var deleteScript = transformModel.TransformScriptList.Single(item => item.Name == "delete-target");
            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:update", updateScript, "dbo.Target", []),
                new BindingSeed("binding:delete", deleteScript, "dbo.Target", []));
            BuildSerialTransformPipelineWorkspace(
                pipelineWorkspacePath,
                updateScript.Id,
                "binding:update",
                deleteScript.Id,
                "binding:delete",
                transformWorkspacePath,
                bindingWorkspacePath);

            var plan = new MetaPipelineModeledExecutionResolver().Resolve(
                new MetaPipelineModeledExecutionRequest(
                    pipelineWorkspacePath,
                    "CustomerLoad"));

            Assert.Equal(2, plan.Steps.Count);
            Assert.Equal("update", plan.Steps[0].TransformTaskName);
            Assert.Equal("binding:update", plan.Steps[0].TransformBindingId);
            Assert.Equal("delete", plan.Steps[1].TransformTaskName);
            Assert.Equal("binding:delete", plan.Steps[1].TransformBindingId);
            Assert.All(plan.Steps, step => Assert.False(step.IsSelect));
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task ResolveModeledStep_WhenPipelineHasSerialTransforms_ReturnsSelectedStepOnly()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");
        var pipelineWorkspacePath = Path.Combine(tempRoot, "pipeline");

        try
        {
            var sqlService = new MetaTransformScriptSqlService();
            await sqlService.ImportFromSqlCodeToXmlWorkspaceAsync(
                "UPDATE dbo.Target SET Name = 'A' WHERE Id = 1",
                null,
                transformWorkspacePath,
                "update-target");
            await sqlService.AddSqlCodeToWorkspaceAsync(
                "DELETE FROM dbo.Target WHERE Id = 2",
                null,
                transformWorkspacePath,
                "delete-target");
            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var updateScript = transformModel.TransformScriptList.Single(item => item.Name == "update-target");
            var deleteScript = transformModel.TransformScriptList.Single(item => item.Name == "delete-target");
            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:update", updateScript, "dbo.Target", []),
                new BindingSeed("binding:delete", deleteScript, "dbo.Target", []));
            BuildSerialTransformPipelineWorkspace(
                pipelineWorkspacePath,
                updateScript.Id,
                "binding:update",
                deleteScript.Id,
                "binding:delete",
                transformWorkspacePath,
                bindingWorkspacePath);

            var plan = new MetaPipelineModeledExecutionResolver().ResolveStep(
                new MetaPipelineModeledExecutionStepRequest(
                    pipelineWorkspacePath,
                    "CustomerLoad",
                    "delete"));

            var step = Assert.Single(plan.Steps);
            Assert.Equal("delete", step.TransformTaskName);
            Assert.Equal("binding:delete", step.TransformBindingId);
            Assert.False(step.IsSelect);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public async Task ResolveModeled_WhenSelectHasNoTargetWrite_FailsClearly()
    {
        var tempRoot = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(tempRoot, "transform");
        var bindingWorkspacePath = Path.Combine(tempRoot, "binding");
        var pipelineWorkspacePath = Path.Combine(tempRoot, "pipeline");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "select 1 as CustomerId",
                "dbo.CustomerLoad",
                transformWorkspacePath,
                "dbo.v_customer_load");
            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var script = Assert.Single(transformModel.TransformScriptList);
            BuildBindingWorkspace(
                bindingWorkspacePath,
                new BindingSeed("binding:1", script, "warehouse.CustomerLoad", ["CustomerId"]));
            BuildTransformOnlyPipelineWorkspace(pipelineWorkspacePath, script.Id, "binding:1", transformWorkspacePath, bindingWorkspacePath);

            var exception = Assert.Throws<MetaPipelineConfigurationException>(() =>
                new MetaPipelineModeledExecutionResolver().Resolve(
                    new MetaPipelineModeledExecutionRequest(
                        pipelineWorkspacePath,
                        "CustomerLoad")));

            Assert.Contains("SELECT-kind", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("InsertRows", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    private static void BuildBindingWorkspace(
        string bindingWorkspacePath,
        params BindingSeed[] bindings)
    {
        var model = MetaTransformBindingModel.CreateEmpty();

        foreach (var binding in bindings)
        {
            var transformBinding = new TransformBinding
            {
                Id = binding.BindingId,
                MetaTransformScriptTransformScriptId = binding.Script.Id,
                TransformScriptName = binding.Script.Name,
            };
            model.TransformBindingList.Add(transformBinding);

            var rowsetId = binding.BindingId + ":rowset:1";
            var rowset = new Rowset
            {
                Id = rowsetId,
                TransformBinding = transformBinding,
                DerivationKind = "Output",
                Name = binding.Script.Name,
            };
            model.RowsetList.Add(rowset);

            model.OutputRowsetList.Add(new OutputRowset
            {
                Id = binding.BindingId + ":output:1",
                TransformBinding = transformBinding,
                Rowset = rowset,
            });

            for (var index = 0; index < binding.TargetSqlIdentifiers.Count; index++)
            {
                model.TransformBindingTargetList.Add(new TransformBindingTarget
                {
                    Id = $"{binding.BindingId}:target:{index + 1}",
                    TransformBinding = transformBinding,
                    SqlIdentifier = binding.TargetSqlIdentifiers[index],
                });
            }

            for (var index = 0; index < binding.Columns.Count; index++)
            {
                var ordinal = binding.Ordinals is not null ? binding.Ordinals[index] : index + 1;
                model.ColumnList.Add(new Column
                {
                    Id = $"{binding.BindingId}:column:{index + 1}",
                    Rowset = rowset,
                    Name = binding.Columns[index],
                    Ordinal = ordinal.ToString(),
                });
            }

            if (binding.TypeAssessments is not null)
            {
                if (binding.TypeAssessments.Count != binding.Columns.Count)
                {
                    throw new InvalidOperationException("BindingSeed type assessment count must match column count.");
                }

                var validation = new Validation
                {
                    Id = $"{binding.BindingId}:validation",
                    TransformBinding = transformBinding,
                };
                model.ValidationList.Add(validation);
                var target = model.TransformBindingTargetList.Single(item =>
                    string.Equals(item.Id, $"{binding.BindingId}:target:1", StringComparison.Ordinal));
                var targetRowsetLink = new ValidationTargetRowsetLink
                {
                    Id = $"{binding.BindingId}:validation:target:1",
                    Validation = validation,
                    TransformBindingTarget = target,
                    Rowset = rowset,
                    MetaSchemaTableId = "schema:table:target",
                    ResolvedSqlIdentifier = target.SqlIdentifier,
                };
                model.ValidationTargetRowsetLinkList.Add(targetRowsetLink);

                var orderedColumns = model.ColumnList
                    .Where(item => string.Equals(item.Rowset.Id, rowset.Id, StringComparison.Ordinal))
                    .OrderBy(item => int.Parse(item.Ordinal))
                    .ToArray();
                for (var index = 0; index < orderedColumns.Length; index++)
                {
                    var columnLink = new ValidationTargetColumnLink
                    {
                        Id = $"{targetRowsetLink.Id}:column:{index + 1}",
                        ValidationTargetRowsetLink = targetRowsetLink,
                        Column = orderedColumns[index],
                        MetaSchemaFieldId = $"schema:field:{index + 1}",
                    };
                    model.ValidationTargetColumnLinkList.Add(columnLink);
                    var (sourceMetaDataTypeId, targetMetaDataTypeId) = binding.TypeAssessments[index];
                    if (string.Equals(sourceMetaDataTypeId, targetMetaDataTypeId, StringComparison.Ordinal))
                    {
                        model.ValidationTargetColumnTypeExactList.Add(new ValidationTargetColumnTypeExact
                        {
                            Id = $"{columnLink.Id}:type-exact",
                            ValidationTargetColumnLink = columnLink,
                            SourceMetaDataTypeId = sourceMetaDataTypeId,
                            TargetMetaDataTypeId = targetMetaDataTypeId,
                        });
                    }
                    else
                    {
                        model.ValidationTargetColumnTypeSanctionedConversionList.Add(new ValidationTargetColumnTypeSanctionedConversion
                        {
                            Id = $"{columnLink.Id}:type-sanctioned-conversion",
                            ValidationTargetColumnLink = columnLink,
                            SourceMetaDataTypeId = sourceMetaDataTypeId,
                            TargetMetaDataTypeId = targetMetaDataTypeId,
                        });
                    }
                }
            }
        }

        Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(model, bindingWorkspacePath);
    }

    private static void AddStoredProcedureContractResultRowset(
        MetaTransformScriptModel model,
        TransformScript script,
        IReadOnlyList<string> columnNames)
    {
        var storedProcedure = Assert.Single(model.ScriptObjectStoredProcedureList, item =>
            string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal));
        var contract = new StoredProcedureContract
        {
            Id = $"{script.Id}:stored-procedure-contract",
            ScriptObjectStoredProcedure = storedProcedure,
        };
        model.StoredProcedureContractList.Add(contract);
        var rowset = new StoredProcedureResultRowsetItem
        {
            Id = $"{contract.Id}:result-rowset:1",
            StoredProcedureContract = contract,
            Name = "Result",
            Ordinal = "0",
        };
        model.StoredProcedureResultRowsetItemList.Add(rowset);

        for (var index = 0; index < columnNames.Count; index++)
        {
            model.StoredProcedureResultColumnItemList.Add(new StoredProcedureResultColumnItem
            {
                Id = $"{rowset.Id}:column:{index + 1}",
                StoredProcedureResultRowsetItem = rowset,
                Name = columnNames[index],
                Ordinal = index.ToString(),
            });
        }
    }

    private static void BuildTransformToTargetWritePipelineWorkspace(
        string pipelineWorkspacePath,
        string transformScriptId,
        string transformBindingId,
        string transformWorkspacePath,
        string bindingWorkspacePath,
        string targetSqlIdentifier,
        IReadOnlyList<string> columnNames)
    {
        var model = MetaPipelineModel.CreateEmpty();
        var pipeline = new Pipeline
        {
            Id = "CustomerLoad",
            Name = "CustomerLoad",
        };
        model.PipelineList.Add(pipeline);

        var source = new ConnectionReference
        {
            Id = "CustomerLoad.source",
            Pipeline = pipeline,
            Name = "source",
            EnvironmentVariableName = "SOURCE_ENV",
        };
        var target = new ConnectionReference
        {
            Id = "CustomerLoad.target",
            Pipeline = pipeline,
            Name = "target",
            EnvironmentVariableName = "TARGET_ENV",
        };
        model.ConnectionReferenceList.Add(source);
        model.ConnectionReferenceList.Add(target);

        var transformTask = new PipelineTask
        {
            Id = "CustomerLoad.transform",
            Pipeline = pipeline,
            Name = "transform",
        };
        var targetWriteTask = new PipelineTask
        {
            Id = "CustomerLoad.target-write",
            Pipeline = pipeline,
            Name = "target-write",
        };
        model.PipelineTaskList.Add(transformTask);
        model.PipelineTaskList.Add(targetWriteTask);

        model.TransformExecutionTaskList.Add(new TransformExecutionTask
        {
            Id = "CustomerLoad.transform.TransformExecution",
            PipelineTask = transformTask,
            ExecutionConnectionReference = source,
            TransformScriptId = transformScriptId,
            TransformBindingId = transformBindingId,
            TransformWorkspacePath = transformWorkspacePath,
            BindingWorkspacePath = bindingWorkspacePath,
        });

        var rowStream = new RowStream
        {
            Id = "CustomerLoad.transform.rows",
            Pipeline = pipeline,
            Name = "transform.rows",
        };
        model.RowStreamList.Add(rowStream);
        for (var index = 0; index < columnNames.Count; index++)
        {
            model.RowStreamColumnList.Add(new RowStreamColumn
            {
                Id = $"{rowStream.Id}.column:{index + 1}",
                RowStream = rowStream,
                Name = columnNames[index],
                Ordinal = index.ToString(),
            });
        }

        model.RowStreamProducerList.Add(new RowStreamProducer
        {
            Id = "CustomerLoad.transform.producer",
            PipelineTask = transformTask,
            RowStream = rowStream,
        });

        var targetWrite = new TargetWriteTask
        {
            Id = "CustomerLoad.target-write.TargetWrite",
            PipelineTask = targetWriteTask,
            TargetConnectionReference = target,
        };
        model.TargetWriteTaskList.Add(targetWrite);
        model.InsertRowsTargetWriteTaskList.Add(new InsertRowsTargetWriteTask
        {
            Id = "CustomerLoad.target-write.InsertRows",
            TargetWriteTask = targetWrite,
            TargetSqlIdentifier = targetSqlIdentifier,
        });
        model.RowStreamConsumerList.Add(new RowStreamConsumer
        {
            Id = "CustomerLoad.target-write.consumer",
            PipelineTask = targetWriteTask,
            RowStream = rowStream,
        });
        model.TaskDependencyList.Add(new TaskDependency
        {
            Id = "CustomerLoad.transform.Before.CustomerLoad.target-write",
            Pipeline = pipeline,
            Predecessor = transformTask,
            Successor = targetWriteTask,
        });

        Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(model, pipelineWorkspacePath);
    }

    private static void BuildTransformOnlyPipelineWorkspace(
        string pipelineWorkspacePath,
        string transformScriptId,
        string transformBindingId,
        string transformWorkspacePath,
        string bindingWorkspacePath,
        string? timeoutSeconds = null)
    {
        var model = MetaPipelineModel.CreateEmpty();
        var pipeline = new Pipeline
        {
            Id = "CustomerLoad",
            Name = "CustomerLoad",
        };
        model.PipelineList.Add(pipeline);
        var source = new ConnectionReference
        {
            Id = "CustomerLoad.source",
            Pipeline = pipeline,
            Name = "source",
            EnvironmentVariableName = "SOURCE_ENV",
        };
        model.ConnectionReferenceList.Add(source);
        var task = new PipelineTask
        {
            Id = "CustomerLoad.transform",
            Pipeline = pipeline,
            Name = "transform",
        };
        model.PipelineTaskList.Add(task);
        model.TransformExecutionTaskList.Add(new TransformExecutionTask
        {
            Id = "CustomerLoad.transform.TransformExecution",
            PipelineTask = task,
            ExecutionConnectionReference = source,
            TransformScriptId = transformScriptId,
            TransformBindingId = transformBindingId,
            TransformWorkspacePath = transformWorkspacePath,
            BindingWorkspacePath = bindingWorkspacePath,
            TimeoutSeconds = timeoutSeconds,
        });

        Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(model, pipelineWorkspacePath);
    }

    private static void BuildSerialTransformPipelineWorkspace(
        string pipelineWorkspacePath,
        string firstTransformScriptId,
        string firstTransformBindingId,
        string secondTransformScriptId,
        string secondTransformBindingId,
        string transformWorkspacePath,
        string bindingWorkspacePath)
    {
        var model = MetaPipelineModel.CreateEmpty();
        var pipeline = new Pipeline
        {
            Id = "CustomerLoad",
            Name = "CustomerLoad",
        };
        model.PipelineList.Add(pipeline);
        var source = new ConnectionReference
        {
            Id = "CustomerLoad.source",
            Pipeline = pipeline,
            Name = "source",
            EnvironmentVariableName = "SOURCE_ENV",
        };
        model.ConnectionReferenceList.Add(source);
        var firstTask = new PipelineTask
        {
            Id = "CustomerLoad.update",
            Pipeline = pipeline,
            Name = "update",
        };
        var secondTask = new PipelineTask
        {
            Id = "CustomerLoad.delete",
            Pipeline = pipeline,
            Name = "delete",
        };
        model.PipelineTaskList.Add(firstTask);
        model.PipelineTaskList.Add(secondTask);
        model.TransformExecutionTaskList.Add(new TransformExecutionTask
        {
            Id = "CustomerLoad.update.TransformExecution",
            PipelineTask = firstTask,
            ExecutionConnectionReference = source,
            TransformScriptId = firstTransformScriptId,
            TransformBindingId = firstTransformBindingId,
            TransformWorkspacePath = transformWorkspacePath,
            BindingWorkspacePath = bindingWorkspacePath,
        });
        model.TransformExecutionTaskList.Add(new TransformExecutionTask
        {
            Id = "CustomerLoad.delete.TransformExecution",
            PipelineTask = secondTask,
            ExecutionConnectionReference = source,
            TransformScriptId = secondTransformScriptId,
            TransformBindingId = secondTransformBindingId,
            TransformWorkspacePath = transformWorkspacePath,
            BindingWorkspacePath = bindingWorkspacePath,
        });
        model.TaskDependencyList.Add(new TaskDependency
        {
            Id = "CustomerLoad.update.Before.CustomerLoad.delete",
            Pipeline = pipeline,
            Predecessor = firstTask,
            Successor = secondTask,
        });

        Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(model, pipelineWorkspacePath);
    }

    private sealed record BindingSeed(
        string BindingId,
        TransformScript Script,
        IReadOnlyList<string> TargetSqlIdentifiers,
        IReadOnlyList<string> Columns,
        IReadOnlyList<int>? Ordinals = null,
        IReadOnlyList<(string SourceMetaDataTypeId, string TargetMetaDataTypeId)>? TypeAssessments = null)
    {
        public BindingSeed(
            string bindingId,
            TransformScript script,
            string targetSqlIdentifier,
            IReadOnlyList<string> columns,
            IReadOnlyList<int>? ordinals = null,
            IReadOnlyList<(string SourceMetaDataTypeId, string TargetMetaDataTypeId)>? typeAssessments = null)
            : this(bindingId, script, [targetSqlIdentifier], columns, ordinals, typeAssessments)
        {
        }
    }

    private static string CreateTempRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "MetaPipeline.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void DeleteIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
