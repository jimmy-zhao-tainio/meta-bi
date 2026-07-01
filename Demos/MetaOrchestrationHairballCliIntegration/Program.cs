using System.Globalization;
using System.Text;
using MetaOrchestration;
using MS = MetaSchema;
using SQL = MetaSql;

var options = DemoOptions.Parse(args);
if (!options.Ok)
{
    Console.Error.WriteLine(options.ErrorMessage);
    PrintHelp();
    return 1;
}

if (options.ShowHelp)
{
    PrintHelp();
    return 0;
}

try
{
    switch (options.Command)
    {
        case DemoCommand.Generate:
            var generated = HairballDemo.Generate(options);
            Console.WriteLine($"Generated: {generated.RunRootPath}");
            Console.WriteLine($"SetupScript: {generated.SetupScriptPath}");
            Console.WriteLine($"ExecuteScript: {generated.ExecuteScriptPath}");
            return 0;
        case DemoCommand.Verify:
            var result = HairballDemo.Verify(options);
            PrintSummary(result);
            return result.Passed ? 0 : 4;
        default:
            Console.Error.WriteLine("Unknown command.");
            PrintHelp();
            return 1;
    }
}
catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or IOException or UnauthorizedAccessException)
{
    Console.Error.WriteLine("Cannot run MetaOrchestration hairball demo.");
    Console.Error.WriteLine(ex.Message);
    return 4;
}

static void PrintHelp()
{
    Console.WriteLine("MetaOrchestration hairball demo");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project MetaOrchestrationHairballDemo.csproj -- generate [--out-root <path>] [--seed <number>]");
    Console.WriteLine("  dotnet run --project MetaOrchestrationHairballDemo.csproj -- verify --run-root <path> [--seed <number>]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --out-root <path>  Output folder containing generated run folders. Default: Runs");
    Console.WriteLine("  --run-root <path>  Generated run folder to verify.");
    Console.WriteLine("  --seed <number>    Deterministic generator seed. Default: 20260530");
}

static void PrintSummary(HairballRunResult result)
{
    Console.WriteLine("MetaOrchestration hairball demo");
    Console.WriteLine($"Seed: {result.Seed.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"RunRoot: {result.RunRootPath}");
    Console.WriteLine($"Pipelines: {result.Scenario.PipelineCount.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Transform tasks: {result.Scenario.TransformTaskCount.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Stored procedures: {result.Scenario.StoredProcedureCount.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Result-rowset stored procedures: {result.Scenario.ResultRowsetStoredProcedureCount.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Multi-task pipelines: {result.Scenario.MultiTaskPipelineCount.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Expected pipeline edges: {result.ExpectedPipelineEdgeKeys.Count.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Actual pipeline edges: {result.ActualPipelineEdgeKeys.Count.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Expected data edges: {result.ExpectedDataEdgeKeys.Count.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Actual data edges: {result.ActualDataEdgeKeys.Count.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Expected max dependency depth: {result.Prediction.MaxDependencyDepth.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"Actual max dependency depth: {result.ActualMaxDependencyDepth.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"DagStatus: {result.DagStatus}");
    Console.WriteLine($"DeterminismStatus: {result.DeterminismStatus}");
    Console.WriteLine($"SynchronizationStatus: {result.SynchronizationStatus}");
    Console.WriteLine($"Oracle: {(result.Passed ? "PASS" : "FAIL")}");
    Console.WriteLine($"Summary: {Path.Combine(result.RunRootPath, "summary.txt")}");

    if (result.Failures.Count == 0)
    {
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Failures:");
    foreach (var failure in result.Failures)
    {
        Console.WriteLine($"  {failure}");
    }
}

internal enum DemoCommand
{
    Generate,
    Verify
}

internal sealed record DemoOptions(
    bool Ok,
    bool ShowHelp,
    DemoCommand Command,
    string OutputRootPath,
    string RunRootPath,
    int Seed,
    string ErrorMessage)
{
    public static DemoOptions Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return Fail("missing command: generate or verify.");
        }

        if (IsHelp(args[0]))
        {
            return new DemoOptions(true, true, DemoCommand.Generate, "Runs", string.Empty, HairballDemo.DefaultSeed, string.Empty);
        }

        var command = args[0].ToLowerInvariant() switch
        {
            "generate" => DemoCommand.Generate,
            "verify" => DemoCommand.Verify,
            _ => (DemoCommand?)null
        };
        if (command is null)
        {
            return Fail($"unknown command '{args[0]}'.");
        }

        var outputRootPath = "Runs";
        var runRootPath = string.Empty;
        var seed = HairballDemo.DefaultSeed;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var index = 1; index < args.Length; index++)
        {
            var option = args[index];
            if (IsHelp(option))
            {
                return new DemoOptions(true, true, command.Value, outputRootPath, runRootPath, seed, string.Empty);
            }

            if (!seen.Add(option))
            {
                return Fail($"{option} can only be provided once.");
            }

            if (index + 1 >= args.Length)
            {
                return Fail($"missing value for {option}.");
            }

            var value = args[++index];
            switch (option.ToLowerInvariant())
            {
                case "--out-root":
                    outputRootPath = value;
                    break;
                case "--run-root":
                    runRootPath = value;
                    break;
                case "--seed":
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out seed))
                    {
                        return Fail("--seed must be an integer.");
                    }

                    break;
                default:
                    return Fail($"unknown option '{option}'.");
            }
        }

        if (command.Value == DemoCommand.Generate && string.IsNullOrWhiteSpace(outputRootPath))
        {
            return Fail("--out-root cannot be blank.");
        }

        if (command.Value == DemoCommand.Verify && string.IsNullOrWhiteSpace(runRootPath))
        {
            return Fail("missing required option --run-root <path>.");
        }

        return new DemoOptions(true, false, command.Value, outputRootPath, runRootPath, seed, string.Empty);
    }

    private static bool IsHelp(string value) =>
        string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "help", StringComparison.OrdinalIgnoreCase);

    private static DemoOptions Fail(string errorMessage) =>
        new(false, false, DemoCommand.Generate, string.Empty, string.Empty, HairballDemo.DefaultSeed, errorMessage);
}

internal static class HairballDemo
{
    public const int DefaultSeed = 20260530;
    private const string ExecuteConnectionEnv = "HAIRBALL_EXECUTION_SQL";
    private const string TargetConnectionEnv = "HAIRBALL_TARGET_SQL";
    private const string DatabaseName = "MetaOrchestrationHairball";
    private const string LocalhostConnectionString = "Server=localhost;Database=" + DatabaseName + ";Trusted_Connection=True;TrustServerCertificate=True;";
    private const string ExecuteSystemName = "Hairball";
    private const string RunPlanGraphCapturePath = "orchestration-run-plan-graph.txt";
    private const string ExecuteOutputCapturePath = "orchestration-execute-output.txt";

    public static HairballGeneratedRun Generate(DemoOptions options)
    {
        var runRoot = Path.Combine(
            Path.GetFullPath(options.OutputRootPath),
            $"hairball-seed-{options.Seed.ToString(CultureInfo.InvariantCulture)}");
        if (Directory.Exists(runRoot) && Directory.EnumerateFileSystemEntries(runRoot).Any())
        {
            throw new InvalidOperationException($"Run folder '{runRoot}' already exists and is not empty.");
        }

        Directory.CreateDirectory(runRoot);

        var scenario = HairballScenario.Generate(options.Seed);
        var prediction = scenario.Predict();
        WriteSqlFiles(runRoot, scenario);
        WriteSchemaWorkspace(runRoot, scenario);
        WriteMetaSqlWorkspace(runRoot, scenario);
        WritePredictionFiles(runRoot, prediction);
        var commandScripts = WriteCommandScripts(runRoot, scenario);

        return new HairballGeneratedRun(runRoot, commandScripts.SetupScriptPath, commandScripts.ExecuteScriptPath);
    }

    public static HairballRunResult Verify(DemoOptions options)
    {
        var runRoot = Path.GetFullPath(options.RunRootPath);
        var orchestrationWorkspace = Path.Combine(runRoot, "OrchestrationWS");
        var model = MetaOrchestrationModel.LoadFromXmlWorkspace(orchestrationWorkspace, searchUpward: false);
        var plan = model.OrchestrationPlanList.Single();
        var scenario = HairballScenario.Generate(options.Seed);
        var prediction = scenario.Predict();
        var expectedPipelineEdges = ExpectedPipelineEdgeKeys(prediction).ToArray();
        var actualPipelineEdges = ActualPipelineEdgeKeys(model).ToArray();
        var expectedDataEdges = ExpectedDataEdgeKeys(prediction).ToArray();
        var actualDataEdges = ActualCrossPipelineDataEdgeKeys(model).ToArray();
        var actualMaxDependencyDepth = ComputeDependencyDepth(actualPipelineEdges);
        var failures = BuildFailures(
            model,
            scenario,
            prediction,
            expectedPipelineEdges,
            actualPipelineEdges,
            expectedDataEdges,
            actualDataEdges,
            actualMaxDependencyDepth);

        WriteActualFiles(runRoot, actualPipelineEdges, actualDataEdges);

        var result = new HairballRunResult(
            options.Seed,
            runRoot,
            new HairballScenarioSummary(
                scenario.Pipelines.Count,
                scenario.Transforms.Count,
                scenario.Transforms.Count(static item => item.IsStoredProcedure),
                scenario.Transforms.Count(static item => item.ResultColumns.Count > 0),
                scenario.Pipelines.Count(static item => item.Tasks.Count > 1)),
            prediction,
            expectedPipelineEdges,
            actualPipelineEdges,
            expectedDataEdges,
            actualDataEdges,
            actualMaxDependencyDepth,
            plan.DagStatus,
            plan.DeterminismStatus,
            plan.SynchronizationStatus,
            failures);
        WriteSummary(result);
        return result;
    }

    private static void WriteSqlFiles(string runRoot, HairballScenario scenario)
    {
        var sourceSqlPath = Path.Combine(runRoot, "SourceSql");
        Directory.CreateDirectory(sourceSqlPath);
        foreach (var transform in scenario.Transforms)
        {
            File.WriteAllText(
                Path.Combine(sourceSqlPath, $"{transform.FileStem}.sql"),
                transform.Sql,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }
    }

    private static void WriteSchemaWorkspace(string runRoot, HairballScenario scenario)
    {
        var schemaWorkspace = Path.Combine(runRoot, "SchemaWS");
        var model = MS.MetaSchemaModel.CreateEmpty();
        var system = new MS.System
        {
            Id = "hairball:system",
            Name = ExecuteSystemName,
            Description = "Generated schema workspace for the MetaOrchestration hairball CLI demo."
        };
        model.SystemList.Add(system);

        var schemasByName = new Dictionary<string, MS.Schema>(StringComparer.OrdinalIgnoreCase);
        foreach (var sqlIdentifier in scenario.Pipelines
                     .SelectMany(static pipeline => pipeline.Tasks)
                     .SelectMany(static task => task.ReadObjects.Concat(task.WriteObjects))
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase))
        {
            var parts = sqlIdentifier.Split('.', 2);
            if (parts.Length != 2)
            {
                throw new InvalidOperationException($"Expected two-part SQL identifier, got '{sqlIdentifier}'.");
            }

            if (!schemasByName.TryGetValue(parts[0], out var schema))
            {
                schema = new MS.Schema
                {
                    Id = $"hairball:schema:{parts[0]}",
                    System = system,
                    Name = parts[0],
                };
                schemasByName.Add(parts[0], schema);
                model.SchemaList.Add(schema);
            }

            var table = new MS.Table
            {
                Id = $"hairball:table:{parts[0]}:{parts[1]}",
                Schema = schema,
                Name = parts[1],
                ObjectType = "Table",
            };
            model.TableList.Add(table);
            model.FieldList.Add(new MS.Field
            {
                Id = $"hairball:field:{parts[0]}:{parts[1]}:Value",
                Table = table,
                MetaDataTypeId = "sqlserver:type:int",
                Name = "Value",
                Ordinal = "0",
                IsNullable = "false",
            });
        }

        model.SaveToXmlWorkspace(schemaWorkspace);
    }

    private static void WriteMetaSqlWorkspace(string runRoot, HairballScenario scenario)
    {
        var workspacePath = Path.Combine(runRoot, "CurrentMetaSqlWorkspace");
        var model = SQL.MetaSqlModel.CreateEmpty();
        var database = new SQL.Database
        {
            Id = DatabaseName,
            Name = DatabaseName,
        };
        model.DatabaseList.Add(database);

        var schemasByName = new Dictionary<string, SQL.Schema>(StringComparer.OrdinalIgnoreCase);
        SQL.Schema GetOrAddSchema(string schemaName)
        {
            if (schemasByName.TryGetValue(schemaName, out var existing))
            {
                return existing;
            }

            var schema = new SQL.Schema
            {
                Id = $"{DatabaseName}.{schemaName}",
                Name = schemaName,
                Database = database,
            };
            schemasByName.Add(schemaName, schema);
            model.SchemaList.Add(schema);
            return schema;
        }

        foreach (var sqlIdentifier in scenario.Pipelines
                     .SelectMany(static pipeline => pipeline.Tasks)
                     .SelectMany(static task => task.ReadObjects.Concat(task.WriteObjects))
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase))
        {
            var (schemaName, objectName) = ParseTwoPartIdentifier(sqlIdentifier);
            var schema = GetOrAddSchema(schemaName);
            var table = new SQL.Table
            {
                Id = $"{schema.Id}.{objectName}",
                Name = objectName,
                Schema = schema,
            };
            model.TableList.Add(table);
            model.TableColumnList.Add(new SQL.TableColumn
            {
                Id = $"{table.Id}.Value",
                Table = table,
                Name = "Value",
                Ordinal = "1",
                MetaDataTypeId = "sqlserver:type:int",
                IsNullable = "false",
            });
        }

        for (var index = 0; index < scenario.Transforms.Count; index++)
        {
            var transform = scenario.Transforms[index];
            var (schemaName, moduleName) = ParseTwoPartIdentifier(transform.ScriptName);
            var schema = GetOrAddSchema(schemaName);
            var deployOrdinal = (index + 1).ToString(CultureInfo.InvariantCulture);
            if (transform.IsStoredProcedure)
            {
                model.StoredProcedureList.Add(new SQL.StoredProcedure
                {
                    Id = $"{schema.Id}.procedure.{moduleName}",
                    Schema = schema,
                    Name = moduleName,
                    DefinitionSql = transform.Sql,
                    DeployOrdinal = deployOrdinal,
                });
            }
            else
            {
                model.ViewList.Add(new SQL.View
                {
                    Id = $"{schema.Id}.view.{moduleName}",
                    Schema = schema,
                    Name = moduleName,
                    DefinitionSql = transform.Sql,
                    DeployOrdinal = deployOrdinal,
                });
            }
        }

        model.SaveToXmlWorkspace(workspacePath);
    }

    private static HairballGeneratedScripts WriteCommandScripts(string runRoot, HairballScenario scenario)
    {
        var setupScriptPath = Path.Combine(runRoot, "generated-setup.cmd");
        var executeScriptPath = Path.Combine(runRoot, "generated-execute.cmd");
        var setupCommands = new List<HairballBatchCommand>();

        setupCommands.Add(HairballBatchCommand.Run($"meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-env {ExecuteConnectionEnv} --out MetaSqlDeployManifest"));
        setupCommands.Add(HairballBatchCommand.Run($"meta-sql deploy --manifest-workspace MetaSqlDeployManifest --source-workspace CurrentMetaSqlWorkspace --connection-env {ExecuteConnectionEnv}"));
        setupCommands.Add(HairballBatchCommand.Run($"meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-env {ExecuteConnectionEnv} --out MetaSqlVerifyManifest"));

        for (var index = 0; index < scenario.Transforms.Count; index++)
        {
            var transform = scenario.Transforms[index];
            var workspaceOption = index == 0
                ? "--new-workspace TransformWS"
                : "--workspace TransformWS";
            setupCommands.Add(HairballBatchCommand.Run(
                string.IsNullOrWhiteSpace(transform.TargetSqlIdentifier)
                    ? $"meta-transform-script from sql-file --path {QuoteCmd(Path.Combine("SourceSql", transform.FileStem + ".sql"))} {workspaceOption}"
                    : $"meta-transform-script from sql-file --path {QuoteCmd(Path.Combine("SourceSql", transform.FileStem + ".sql"))} --target {transform.TargetSqlIdentifier} {workspaceOption}"));
        }

        foreach (var transform in scenario.Transforms.Where(static item => item.IsStoredProcedure))
        {
            var command = new StringBuilder();
            command.Append(CultureInfo.InvariantCulture, $"meta-transform-script stored-procedure add-contract --workspace TransformWS --name {transform.ScriptName}");
            foreach (var operation in transform.StoredProcedureOperations)
            {
                command.Append(" --operation ");
                command.Append(QuoteCmd(RenderOperation(operation)));
            }

            if (transform.ResultColumns.Count > 0)
            {
                command.Append(" --result-rowset Result");
                foreach (var column in transform.ResultColumns)
                {
                    command.Append(" --result-column ");
                    command.Append(QuoteCmd($"Result={column}"));
                }
            }

            setupCommands.Add(HairballBatchCommand.Run(command.ToString()));
        }

        setupCommands.Add(HairballBatchCommand.Run("meta-transform-binding bind --transform-workspace TransformWS --source-schema SchemaWS --target-schema SchemaWS --execute-system Hairball --new-workspace BindingWS"));
        setupCommands.Add(HairballBatchCommand.Run("meta-pipeline new-workspace PipelineWS"));

        foreach (var pipeline in scenario.Pipelines)
        {
            setupCommands.Add(HairballBatchCommand.Run($"meta-pipeline add-pipeline --workspace PipelineWS --name {pipeline.PipelineName}"));
            foreach (var task in pipeline.Tasks)
            {
                var command = new StringBuilder();
                command.Append(CultureInfo.InvariantCulture, $"meta-pipeline add-step --workspace PipelineWS --pipeline {pipeline.PipelineName} --step-name {task.TaskName} --script {task.Transform.ScriptName} --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env {ExecuteConnectionEnv}");
                if (!string.IsNullOrWhiteSpace(task.InsertRowsTarget))
                {
                    command.Append(CultureInfo.InvariantCulture, $" --target-connection-env {TargetConnectionEnv} --target {task.InsertRowsTarget}");
                }

                setupCommands.Add(HairballBatchCommand.Run(command.ToString()));
            }
        }

        setupCommands.Add(HairballBatchCommand.Run("meta-pipeline inspect --workspace PipelineWS"));
        setupCommands.Add(HairballBatchCommand.Run("meta-orchestration infer --pipeline-workspace PipelineWS --new-workspace OrchestrationWS --description \"Deterministic hairball CLI demo\""));
        setupCommands.Add(HairballBatchCommand.Run("meta-orchestration inspect --workspace OrchestrationWS"));
        setupCommands.Add(HairballBatchCommand.Run("meta-orchestration refresh-run-plan --workspace OrchestrationWS"));
        setupCommands.Add(HairballBatchCommand.Capture(
            "meta-orchestration inspect-run-plan --workspace OrchestrationWS",
            RunPlanGraphCapturePath));

        WriteBatchScript(
            setupScriptPath,
            setupCommands,
            [
                (ExecuteConnectionEnv, LocalhostConnectionString),
                (TargetConnectionEnv, LocalhostConnectionString)
            ]);
        WriteBatchScript(
            executeScriptPath,
            [
                HairballBatchCommand.Capture(
                    "meta-orchestration execute --workspace OrchestrationWS --pipeline-workspace PipelineWS --max-degree-of-parallelism 12 --run-artifacts-root RunArtifacts",
                    ExecuteOutputCapturePath)
            ],
            [
                (ExecuteConnectionEnv, LocalhostConnectionString),
                (TargetConnectionEnv, LocalhostConnectionString)
            ]);

        return new HairballGeneratedScripts(setupScriptPath, executeScriptPath);
    }

    private static void WriteBatchScript(
        string path,
        IReadOnlyList<HairballBatchCommand> commands,
        IReadOnlyList<(string Name, string Value)>? environmentDefaults = null)
    {
        var lines = new List<string>
        {
            "@echo off",
            "pushd \"%~dp0\" || exit /b 1",
        };

        foreach (var environmentDefault in environmentDefaults ?? [])
        {
            lines.Add($"if not defined {environmentDefault.Name} set \"{environmentDefault.Name}={environmentDefault.Value}\"");
        }

        foreach (var command in commands)
        {
            if (string.IsNullOrWhiteSpace(command.CapturePath))
            {
                lines.Add($"call :run {command.Command} || goto :fail");
                continue;
            }

            lines.Add("echo.");
            lines.Add($"echo {command.Command}");
            lines.Add($"echo Capturing output to {command.CapturePath}");
            lines.Add($"call {command.Command} > {QuoteCmd(command.CapturePath)} 2>&1");
            lines.Add("set \"__hairball_capture_exit=%errorlevel%\"");
            lines.Add($"type {QuoteCmd(command.CapturePath)}");
            lines.Add("if not \"%__hairball_capture_exit%\"==\"0\" (");
            lines.Add("  set \"__hairball_exit=%__hairball_capture_exit%\"");
            lines.Add("  goto :fail");
            lines.Add(")");
        }

        lines.Add("popd");
        lines.Add("exit /b 0");
        lines.Add(":fail");
        lines.Add("if not defined __hairball_exit set \"__hairball_exit=%errorlevel%\"");
        lines.Add("popd");
        lines.Add("exit /b %__hairball_exit%");
        lines.Add(":run");
        lines.Add("echo.");
        lines.Add("echo %*");
        lines.Add("call %*");
        lines.Add("exit /b %errorlevel%");

        File.WriteAllLines(path, lines, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static string RenderOperation(StoredProcedureOperationSeed operation)
    {
        var rendered = $"{operation.Ordinal.ToString(CultureInfo.InvariantCulture)}:{operation.OperationKind}:{operation.SqlIdentifier}";
        return string.IsNullOrWhiteSpace(operation.AccessRole)
            ? rendered
            : $"{rendered}={operation.AccessRole}";
    }

    private static IReadOnlyList<string> BuildFailures(
        MetaOrchestrationModel model,
        HairballScenario scenario,
        HairballPrediction prediction,
        IReadOnlyList<string> expectedPipelineEdges,
        IReadOnlyList<string> actualPipelineEdges,
        IReadOnlyList<string> expectedDataEdges,
        IReadOnlyList<string> actualDataEdges,
        int actualMaxDependencyDepth)
    {
        var plan = model.OrchestrationPlanList.SingleOrDefault();
        var failures = new List<string>();
        if (plan is null)
        {
            failures.Add("Orchestration workspace does not contain exactly one plan.");
            return failures;
        }

        if (!string.Equals(plan.DagStatus, "Complete", StringComparison.Ordinal)) failures.Add($"DAG status was '{plan.DagStatus}', expected Complete.");
        if (!string.Equals(plan.DeterminismStatus, "Deterministic", StringComparison.Ordinal)) failures.Add($"Determinism status was '{plan.DeterminismStatus}', expected Deterministic.");
        if (!string.Equals(plan.SynchronizationStatus, "Complete", StringComparison.Ordinal)) failures.Add($"Synchronization status was '{plan.SynchronizationStatus}', expected Complete.");
        if (model.DependencyIssueList.Count != 0) failures.Add($"Analyzer produced {model.DependencyIssueList.Count.ToString(CultureInfo.InvariantCulture)} issue(s), expected zero.");
        if (model.PipelineReferenceList.Count != scenario.Pipelines.Count) failures.Add($"Pipeline count was {model.PipelineReferenceList.Count.ToString(CultureInfo.InvariantCulture)}, expected {scenario.Pipelines.Count.ToString(CultureInfo.InvariantCulture)}.");
        if (model.TaskAccessProfileList.Count != scenario.Transforms.Count) failures.Add($"Transform task count was {model.TaskAccessProfileList.Count.ToString(CultureInfo.InvariantCulture)}, expected {scenario.Transforms.Count.ToString(CultureInfo.InvariantCulture)}.");
        if (actualMaxDependencyDepth != prediction.MaxDependencyDepth) failures.Add($"Max dependency depth was {actualMaxDependencyDepth.ToString(CultureInfo.InvariantCulture)}, expected {prediction.MaxDependencyDepth.ToString(CultureInfo.InvariantCulture)}.");
        AddSequenceFailure(failures, "Pipeline edges", expectedPipelineEdges, actualPipelineEdges);
        AddSequenceFailure(failures, "Cross-pipeline data edges", expectedDataEdges, actualDataEdges);

        if (!model.TaskDependencyList.Any(static item =>
                string.Equals(item.DependencyKind, "Data", StringComparison.Ordinal) &&
                string.Equals(item.Predecessor.PipelineReference.MetaPipelinePipelineId, "CompositeMart", StringComparison.Ordinal) &&
                string.Equals(item.Successor.PipelineReference.MetaPipelinePipelineId, "CompositeMart", StringComparison.Ordinal) &&
                item.DataObject is not null &&
                string.Equals(item.DataObject.NormalizedKey, "WRK.COMPOSITESTAGE", StringComparison.Ordinal)))
        {
            failures.Add("Missing expected same-pipeline CompositeMart data edge over WRK.COMPOSITESTAGE.");
        }

        if (!model.TaskObjectEffectList.Any(static item =>
                string.Equals(item.TaskAccessProfile.PipelineReference.Name, "CompositeMart", StringComparison.Ordinal) &&
                string.Equals(item.DataObject.SqlIdentifier, "wrk.CompositeCurated", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(item.WriteEffect, "Replace", StringComparison.Ordinal)))
        {
            failures.Add("Missing expected CompositeMart replace effect on wrk.CompositeCurated.");
        }

        if (!model.TaskObjectEffectList.Any(static item =>
                string.Equals(item.TaskAccessProfile.PipelineReference.Name, "AuditHairball", StringComparison.Ordinal) &&
                string.Equals(item.DataObject.SqlIdentifier, "audit.HairballRunLog", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(item.AccessDirection, "ReadWrite", StringComparison.Ordinal) &&
                string.Equals(item.WriteEffect, "Mutation", StringComparison.Ordinal)))
        {
            failures.Add("Missing expected AuditHairball mutation effect on audit.HairballRunLog.");
        }

        return failures;
    }

    private static IEnumerable<string> ExpectedPipelineEdgeKeys(HairballPrediction prediction) =>
        prediction.ExpectedPipelineEdges
            .Select(static item => $"{item.PredecessorPipelineId}->{item.SuccessorPipelineId}")
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static item => item, StringComparer.Ordinal);

    private static IEnumerable<string> ActualPipelineEdgeKeys(MetaOrchestrationModel model) =>
        model.PipelineDependencyList
            .Select(static item => $"{item.Predecessor.MetaPipelinePipelineId}->{item.Successor.MetaPipelinePipelineId}")
            .OrderBy(static item => item, StringComparer.Ordinal);

    private static IEnumerable<string> ExpectedDataEdgeKeys(HairballPrediction prediction) =>
        prediction.ExpectedDataEdges
            .Select(static item => $"{item.PredecessorPipelineId}->{item.SuccessorPipelineId}:{item.ObjectKey}")
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static item => item, StringComparer.Ordinal);

    private static IEnumerable<string> ActualCrossPipelineDataEdgeKeys(MetaOrchestrationModel model) =>
        model.TaskDependencyList
            .Where(static item => string.Equals(item.DependencyKind, "Data", StringComparison.Ordinal))
            .Where(static item => !string.Equals(item.Predecessor.PipelineReference.MetaPipelinePipelineId, item.Successor.PipelineReference.MetaPipelinePipelineId, StringComparison.Ordinal))
            .Where(static item => item.DataObject is not null)
            .Select(static item => $"{item.Predecessor.PipelineReference.MetaPipelinePipelineId}->{item.Successor.PipelineReference.MetaPipelinePipelineId}:{item.DataObject!.NormalizedKey}")
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static item => item, StringComparer.Ordinal);

    private static void AddSequenceFailure(
        ICollection<string> failures,
        string label,
        IReadOnlyList<string> expected,
        IReadOnlyList<string> actual)
    {
        if (expected.SequenceEqual(actual, StringComparer.Ordinal))
        {
            return;
        }

        failures.Add($"{label} mismatch. Expected {expected.Count.ToString(CultureInfo.InvariantCulture)}, actual {actual.Count.ToString(CultureInfo.InvariantCulture)}.");
    }

    private static int ComputeDependencyDepth(IEnumerable<string> pipelineEdgeKeys)
    {
        var edges = pipelineEdgeKeys
            .Select(static item => item.Split("->", 2, StringSplitOptions.None))
            .Where(static item => item.Length == 2)
            .Select(static item => (Predecessor: item[0], Successor: item[1]))
            .ToArray();
        if (edges.Length == 0)
        {
            return 0;
        }

        var predecessorsByPipelineId = edges
            .GroupBy(static item => item.Successor, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group.Select(static item => item.Predecessor).Distinct(StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal);
        var pipelineIds = edges
            .SelectMany(static item => new[] { item.Predecessor, item.Successor })
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        var memo = new Dictionary<string, int>(StringComparer.Ordinal);
        return pipelineIds.Select(Depth).Max();

        int Depth(string pipelineId)
        {
            if (memo.TryGetValue(pipelineId, out var cached))
            {
                return cached;
            }

            var depth = predecessorsByPipelineId.TryGetValue(pipelineId, out var predecessors) && predecessors.Length > 0
                ? predecessors.Select(Depth).Max() + 1
                : 0;
            memo[pipelineId] = depth;
            return depth;
        }
    }

    private static void WritePredictionFiles(string runRoot, HairballPrediction prediction)
    {
        File.WriteAllLines(
            Path.Combine(runRoot, "expected-pipeline-edges.tsv"),
            new[] { "PredecessorPipelineId\tSuccessorPipelineId" }.Concat(
                prediction.ExpectedPipelineEdges.Select(static item => $"{item.PredecessorPipelineId}\t{item.SuccessorPipelineId}")));
        File.WriteAllLines(
            Path.Combine(runRoot, "expected-data-edges.tsv"),
            new[] { "PredecessorPipelineId\tSuccessorPipelineId\tObjectKey" }.Concat(
                prediction.ExpectedDataEdges.Select(static item => $"{item.PredecessorPipelineId}\t{item.SuccessorPipelineId}\t{item.ObjectKey}")));
    }

    private static void WriteActualFiles(
        string runRoot,
        IReadOnlyList<string> actualPipelineEdges,
        IReadOnlyList<string> actualDataEdges)
    {
        File.WriteAllLines(
            Path.Combine(runRoot, "actual-pipeline-edges.tsv"),
            new[] { "Edge" }.Concat(actualPipelineEdges));
        File.WriteAllLines(
            Path.Combine(runRoot, "actual-data-edges.tsv"),
            new[] { "Edge" }.Concat(actualDataEdges));
    }

    private static void WriteSummary(HairballRunResult result)
    {
        var lines = new List<string>
        {
            "MetaOrchestration hairball demo",
            $"Seed: {result.Seed.ToString(CultureInfo.InvariantCulture)}",
            $"RunRoot: {result.RunRootPath}",
            $"Pipelines: {result.Scenario.PipelineCount.ToString(CultureInfo.InvariantCulture)}",
            $"TransformTasks: {result.Scenario.TransformTaskCount.ToString(CultureInfo.InvariantCulture)}",
            $"StoredProcedures: {result.Scenario.StoredProcedureCount.ToString(CultureInfo.InvariantCulture)}",
            $"ResultRowsetStoredProcedures: {result.Scenario.ResultRowsetStoredProcedureCount.ToString(CultureInfo.InvariantCulture)}",
            $"MultiTaskPipelines: {result.Scenario.MultiTaskPipelineCount.ToString(CultureInfo.InvariantCulture)}",
            $"ExpectedPipelineEdges: {result.ExpectedPipelineEdgeKeys.Count.ToString(CultureInfo.InvariantCulture)}",
            $"ActualPipelineEdges: {result.ActualPipelineEdgeKeys.Count.ToString(CultureInfo.InvariantCulture)}",
            $"ExpectedDataEdges: {result.ExpectedDataEdgeKeys.Count.ToString(CultureInfo.InvariantCulture)}",
            $"ActualDataEdges: {result.ActualDataEdgeKeys.Count.ToString(CultureInfo.InvariantCulture)}",
            $"ExpectedMaxDependencyDepth: {result.Prediction.MaxDependencyDepth.ToString(CultureInfo.InvariantCulture)}",
            $"ActualMaxDependencyDepth: {result.ActualMaxDependencyDepth.ToString(CultureInfo.InvariantCulture)}",
            $"DagStatus: {result.DagStatus}",
            $"DeterminismStatus: {result.DeterminismStatus}",
            $"SynchronizationStatus: {result.SynchronizationStatus}",
            $"Oracle: {(result.Passed ? "PASS" : "FAIL")}"
        };
        if (result.Failures.Count > 0)
        {
            lines.Add("Failures:");
            lines.AddRange(result.Failures.Select(static item => $"  {item}"));
        }

        File.WriteAllLines(Path.Combine(result.RunRootPath, "summary.txt"), lines);
    }

    private static string QuoteCmd(string value) => "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";

    private static (string SchemaName, string ObjectName) ParseTwoPartIdentifier(string sqlIdentifier)
    {
        var parts = sqlIdentifier.Split('.', 2);
        if (parts.Length != 2)
        {
            throw new InvalidOperationException($"Expected two-part SQL identifier, got '{sqlIdentifier}'.");
        }

        return (parts[0], parts[1]);
    }

    private static string PipelineId(string pipelineName) => pipelineName;

    private static string ObjectKey(string sqlIdentifier) => sqlIdentifier.ToUpperInvariant();
}

internal sealed record HairballScenario(
    int Seed,
    IReadOnlyList<HairballPipelineSeed> Pipelines)
{
    public IReadOnlyList<HairballTransformSeed> Transforms =>
        Pipelines.SelectMany(static item => item.Tasks)
            .Select(static item => item.Transform)
            .ToArray();

    public HairballPrediction Predict()
    {
        var producerByObjectKey = new Dictionary<string, string>(StringComparer.Ordinal);
        var depthByPipelineId = new Dictionary<string, int>(StringComparer.Ordinal);
        var dataEdges = new HashSet<HairballExpectedEdge>();

        foreach (var pipeline in Pipelines)
        {
            var pipelineId = PipelineId(pipeline.PipelineName);
            var pipelineDepth = 0;
            foreach (var task in pipeline.Tasks)
            {
                foreach (var readObject in task.ReadObjects)
                {
                    var objectKey = ObjectKey(readObject);
                    if (!producerByObjectKey.TryGetValue(objectKey, out var predecessorPipelineId))
                    {
                        continue;
                    }

                    if (!string.Equals(predecessorPipelineId, pipelineId, StringComparison.Ordinal))
                    {
                        dataEdges.Add(new HairballExpectedEdge(predecessorPipelineId, pipelineId, objectKey));
                    }

                    if (depthByPipelineId.TryGetValue(predecessorPipelineId, out var predecessorDepth))
                    {
                        pipelineDepth = Math.Max(pipelineDepth, predecessorDepth + 1);
                    }
                }

                foreach (var writeObject in task.WriteObjects)
                {
                    producerByObjectKey[ObjectKey(writeObject)] = pipelineId;
                }
            }

            depthByPipelineId[pipelineId] = pipelineDepth;
        }

        return new HairballPrediction(
            dataEdges
                .OrderBy(static item => item.PredecessorPipelineId, StringComparer.Ordinal)
                .ThenBy(static item => item.SuccessorPipelineId, StringComparer.Ordinal)
                .ThenBy(static item => item.ObjectKey, StringComparer.Ordinal)
                .ToArray(),
            dataEdges
                .Select(static item => new HairballExpectedPipelineEdge(item.PredecessorPipelineId, item.SuccessorPipelineId))
                .Distinct()
                .OrderBy(static item => item.PredecessorPipelineId, StringComparer.Ordinal)
                .ThenBy(static item => item.SuccessorPipelineId, StringComparer.Ordinal)
                .ToArray(),
            depthByPipelineId.Count == 0 ? 0 : depthByPipelineId.Values.Max());
    }

    public static HairballScenario Generate(int seed)
    {
        var random = new Random(seed);
        var pipelines = new List<HairballPipelineSeed>();
        var availableObjects = new List<string>();

        void AddPipeline(HairballPipelineSeed pipeline)
        {
            pipelines.Add(pipeline);
            availableObjects.AddRange(pipeline.Tasks.SelectMany(static item => item.WriteObjects));
        }

        for (var index = 1; index <= 8; index++)
        {
            var raw = $"raw.Source{index:00}";
            var target = $"stage.Seed{index:00}";
            AddPipeline(new HairballPipelineSeed(
                $"ExtractSeed{index:00}",
                [
                    SelectTask(
                        $"extract-seed-{index:00}",
                        $"demo.ExtractSeed{index:00}",
                        target,
                        [raw])
                ]));
        }

        var layerCount = 8 + random.Next(0, 2);
        for (var layer = 1; layer <= layerCount; layer++)
        {
            var layerObjects = new List<string>();
            var nodeCount = 7 + random.Next(0, 4);
            for (var node = 1; node <= nodeCount; node++)
            {
                var target = ResolveLayerTarget(layer, node);
                var sources = PickObjects(random, availableObjects, 2, 5);
                var kind = (layer + node + random.Next(0, 4)) % 3;
                var pipelineName = $"Layer{layer:00}Node{node:00}";
                var task = kind switch
                {
                    0 => SelectTask(
                        $"select-l{layer:00}-n{node:00}",
                        $"demo.SelectL{layer:00}N{node:00}",
                        target,
                        sources),
                    1 => StoredProcedureRefreshTask(
                        $"refresh-l{layer:00}-n{node:00}",
                        $"etl.RefreshL{layer:00}N{node:00}",
                        target,
                        sources),
                    _ => StoredProcedureResultTask(
                        $"export-l{layer:00}-n{node:00}",
                        $"etl.ExportL{layer:00}N{node:00}",
                        target,
                        sources)
                };

                AddPipeline(new HairballPipelineSeed(pipelineName, [task]));
                layerObjects.Add(target);
            }

            var hubStage = $"hub.Layer{layer:00}Stage";
            var hubCurated = $"hub.Layer{layer:00}Curated";
            var hubPublished = $"hub.Layer{layer:00}Published";
            AddPipeline(new HairballPipelineSeed(
                $"Layer{layer:00}Hub",
                [
                    SelectTask(
                        $"hub-l{layer:00}-stage",
                        $"demo.HubL{layer:00}Stage",
                        hubStage,
                        PickObjects(random, layerObjects, 4, 8)),
                    StoredProcedureRefreshTask(
                        $"hub-l{layer:00}-curate",
                        $"etl.HubL{layer:00}Curate",
                        hubCurated,
                        CombineObjects(
                            [hubStage],
                            PickObjects(random, availableObjects, 2, 4))),
                    StoredProcedureResultTask(
                        $"hub-l{layer:00}-publish",
                        $"etl.HubL{layer:00}Publish",
                        hubPublished,
                        CombineObjects(
                            [hubCurated],
                            PickObjects(random, layerObjects.Concat(availableObjects).ToArray(), 3, 6)))
                ]));

            if (layer > 1 && layer % 2 == 0)
            {
                var bridgeStage = $"bridge.L{layer - 1:00}L{layer:00}Stage";
                var bridgeNorth = $"bridge.L{layer - 1:00}L{layer:00}North";
                var bridgeSouth = $"bridge.L{layer - 1:00}L{layer:00}South";
                AddPipeline(new HairballPipelineSeed(
                    $"BridgeL{layer - 1:00}L{layer:00}",
                    [
                        StoredProcedureResultTask(
                            $"bridge-l{layer - 1:00}-l{layer:00}-collect",
                            $"etl.BridgeL{layer - 1:00}L{layer:00}Collect",
                            bridgeStage,
                            PickObjects(random, availableObjects, 5, 9)),
                        SelectTask(
                            $"bridge-l{layer - 1:00}-l{layer:00}-north",
                            $"demo.BridgeL{layer - 1:00}L{layer:00}North",
                            bridgeNorth,
                            CombineObjects(
                                [bridgeStage],
                                PickObjects(random, availableObjects, 2, 4))),
                        StoredProcedureRefreshTask(
                            $"bridge-l{layer - 1:00}-l{layer:00}-south",
                            $"etl.BridgeL{layer - 1:00}L{layer:00}South",
                            bridgeSouth,
                            CombineObjects(
                                [bridgeStage, bridgeNorth],
                                PickObjects(random, availableObjects, 2, 4)))
                    ]));
            }
        }

        for (var region = 1; region <= 4; region++)
        {
            var regionStage = $"wrk.Region{region:00}Stage";
            var regionCurated = $"wrk.Region{region:00}Curated";
            var regionPublished = $"mart.Region{region:00}Published";
            AddPipeline(new HairballPipelineSeed(
                $"RegionalMart{region:00}",
                [
                    SelectTask(
                        $"regional-{region:00}-stage",
                        $"demo.Regional{region:00}Stage",
                        regionStage,
                        PickObjects(random, availableObjects, 6, 10)),
                    StoredProcedureRefreshTask(
                        $"regional-{region:00}-curate",
                        $"etl.Regional{region:00}Curate",
                        regionCurated,
                        CombineObjects(
                            [regionStage],
                            PickObjects(random, availableObjects, 3, 5))),
                    StoredProcedureResultTask(
                        $"regional-{region:00}-publish",
                        $"etl.Regional{region:00}Publish",
                        regionPublished,
                        CombineObjects(
                            [regionCurated],
                            PickObjects(random, availableObjects, 3, 6))),
                    StoredProcedureMutationTask(
                        $"regional-{region:00}-audit",
                        $"audit.RecordRegional{region:00}",
                        $"audit.Region{region:00}LoadLog",
                        CombineObjects(
                            [regionPublished],
                            PickObjects(random, availableObjects, 1, 2)))
                ]));
        }

        var compositeInputs = PickObjects(random, availableObjects, 6, 9);
        AddPipeline(new HairballPipelineSeed(
            "CompositeMart",
            [
                SelectTask(
                    "composite-stage",
                    "demo.CompositeStage",
                    "wrk.CompositeStage",
                    compositeInputs.Take(4).ToArray()),
                StoredProcedureRefreshTask(
                    "composite-curate",
                    "etl.CompositeCurate",
                    "wrk.CompositeCurated",
                    CombineObjects(
                        ["wrk.CompositeStage"],
                        compositeInputs.Skip(4).Take(2).ToArray())),
                SelectTask(
                    "composite-reconcile",
                    "demo.CompositeReconcile",
                    "wrk.CompositeReconciled",
                    CombineObjects(
                        ["wrk.CompositeCurated"],
                        PickObjects(random, availableObjects, 3, 5))),
                StoredProcedureRefreshTask(
                    "composite-certify",
                    "etl.CompositeCertify",
                    "wrk.CompositeCertified",
                    CombineObjects(
                        ["wrk.CompositeReconciled"],
                        PickObjects(random, availableObjects, 3, 5))),
                StoredProcedureResultTask(
                    "composite-publish",
                    "etl.CompositePublish",
                    "mart.CompositeFinal",
                    CombineObjects(
                        ["wrk.CompositeCertified"],
                        compositeInputs.TakeLast(2).ToArray()))
            ]));

        var auditInputs = PickObjects(random, availableObjects, 4, 7);
        AddPipeline(new HairballPipelineSeed(
            "AuditHairball",
            [
                StoredProcedureMutationTask(
                    "audit-hairball",
                    "audit.RecordHairball",
                    "audit.HairballRunLog",
                    auditInputs)
            ]));

        var finalInputs = PickObjects(random, availableObjects, 10, 14);
        AddPipeline(new HairballPipelineSeed(
            "PublishHairballFinal",
            [
                SelectTask(
                    "publish-hairball-final",
                    "demo.PublishHairballFinal",
                    "mart.HairballFinal",
                    finalInputs)
            ]));

        return new HairballScenario(seed, pipelines);
    }

    private static HairballTaskSeed SelectTask(
        string taskName,
        string scriptName,
        string target,
        IReadOnlyList<string> sources)
    {
        return new HairballTaskSeed(
            taskName,
            new HairballTransformSeed(
                scriptName,
                scriptName,
                CreateSelectSql(scriptName, sources),
                target,
                [],
                []),
            target,
            sources,
            [target]);
    }

    private static HairballTaskSeed StoredProcedureRefreshTask(
        string taskName,
        string scriptName,
        string target,
        IReadOnlyList<string> sources)
    {
        var operations = sources
            .Select((source, index) => Operation(
                10 + (index * 10),
                "Read",
                source,
                index == 0 ? "Source" : "Lookup"))
            .Concat([
                Operation(100, "Reset", target),
                Operation(110, "Append", target),
                Operation(120, "Call", "audit.MarkTransformStep")
            ])
            .ToArray();
        return new HairballTaskSeed(
            taskName,
            new HairballTransformSeed(
                scriptName,
                scriptName,
                CreateStoredProcedureSql(scriptName, returnsValue: false),
                null,
                operations,
                []),
            null,
            sources,
            [target]);
    }

    private static HairballTaskSeed StoredProcedureResultTask(
        string taskName,
        string scriptName,
        string target,
        IReadOnlyList<string> sources)
    {
        var operations = sources
            .Select((source, index) => Operation(
                10 + (index * 10),
                "Read",
                source,
                index == 0 ? "Source" : "Lookup"))
            .ToArray();
        return new HairballTaskSeed(
            taskName,
            new HairballTransformSeed(
                scriptName,
                scriptName,
                CreateStoredProcedureSql(scriptName, returnsValue: true),
                null,
                operations,
                ["Value"]),
            target,
            sources,
            [target]);
    }

    private static HairballTaskSeed StoredProcedureMutationTask(
        string taskName,
        string scriptName,
        string target,
        IReadOnlyList<string> sources)
    {
        var operations = sources
            .Select((source, index) => Operation(
                10 + (index * 10),
                "Read",
                source,
                index == 0 ? "Source" : "Lookup"))
            .Concat([Operation(100, "Mutation", target)])
            .ToArray();
        return new HairballTaskSeed(
            taskName,
            new HairballTransformSeed(
                scriptName,
                scriptName,
                CreateStoredProcedureSql(scriptName, returnsValue: false),
                null,
                operations,
                []),
            null,
            sources,
            [target]);
    }

    private static StoredProcedureOperationSeed Operation(
        int ordinal,
        string operationKind,
        string sqlIdentifier,
        string? accessRole = null) =>
        new(ordinal, operationKind, sqlIdentifier, accessRole);

    private static string ResolveLayerTarget(int layer, int node)
    {
        var schema = layer switch
        {
            1 => "stage",
            2 => "core",
            3 => "dw",
            4 => "dw",
            _ => "mart"
        };
        return $"{schema}.L{layer:00}Node{node:00}";
    }

    private static IReadOnlyList<string> PickObjects(
        Random random,
        IReadOnlyList<string> objects,
        int minCount,
        int maxCount)
    {
        var distinctObjects = objects
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (distinctObjects.Length == 0)
        {
            return [];
        }

        var safeMin = Math.Min(Math.Max(1, minCount), distinctObjects.Length);
        var safeMax = Math.Min(Math.Max(safeMin, maxCount), distinctObjects.Length);
        var count = random.Next(safeMin, safeMax + 1);
        return distinctObjects
            .OrderBy(_ => random.Next())
            .Take(count)
            .OrderBy(static item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<string> CombineObjects(params IReadOnlyList<string>[] objectSets) =>
        objectSets
            .SelectMany(static item => item)
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private static string CreateSelectSql(string scriptName, IReadOnlyList<string> sources)
    {
        var from = new StringBuilder();
        for (var index = 0; index < sources.Count; index++)
        {
            if (index == 0)
            {
                from.Append(CultureInfo.InvariantCulture, $"{sources[index]} AS s{index}");
            }
            else
            {
                from.AppendLine();
                from.Append(CultureInfo.InvariantCulture, $"CROSS JOIN {sources[index]} AS s{index}");
            }
        }

        return $"""
CREATE VIEW {scriptName}
AS
SELECT
    1 AS Value
FROM {from}
""";
    }

    private static string CreateStoredProcedureSql(string scriptName, bool returnsValue)
    {
        var body = returnsValue
            ? "    SELECT CAST(1 AS int) AS Value;"
            : "    RETURN 0;";
        return $"""
CREATE PROCEDURE {scriptName}
AS
BEGIN
    SET NOCOUNT ON;
{body}
END
""";
    }

    private static string PipelineId(string pipelineName) => pipelineName;

    private static string ObjectKey(string sqlIdentifier) => sqlIdentifier.ToUpperInvariant();
}

internal sealed record HairballPipelineSeed(
    string PipelineName,
    IReadOnlyList<HairballTaskSeed> Tasks);

internal sealed record HairballTaskSeed(
    string TaskName,
    HairballTransformSeed Transform,
    string? InsertRowsTarget,
    IReadOnlyList<string> ReadObjects,
    IReadOnlyList<string> WriteObjects);

internal sealed record HairballTransformSeed(
    string ScriptName,
    string FileStem,
    string Sql,
    string? TargetSqlIdentifier,
    IReadOnlyList<StoredProcedureOperationSeed> StoredProcedureOperations,
    IReadOnlyList<string> ResultColumns)
{
    public bool IsStoredProcedure => ScriptName.Contains('.', StringComparison.Ordinal) &&
                                     TargetSqlIdentifier is null &&
                                     Sql.TrimStart().StartsWith("CREATE PROCEDURE", StringComparison.OrdinalIgnoreCase);
}

internal sealed record StoredProcedureOperationSeed(
    int Ordinal,
    string OperationKind,
    string SqlIdentifier,
    string? AccessRole);

internal sealed record HairballPrediction(
    IReadOnlyList<HairballExpectedEdge> ExpectedDataEdges,
    IReadOnlyList<HairballExpectedPipelineEdge> ExpectedPipelineEdges,
    int MaxDependencyDepth);

internal sealed record HairballExpectedEdge(
    string PredecessorPipelineId,
    string SuccessorPipelineId,
    string ObjectKey);

internal sealed record HairballExpectedPipelineEdge(
    string PredecessorPipelineId,
    string SuccessorPipelineId);

internal sealed record HairballGeneratedRun(
    string RunRootPath,
    string SetupScriptPath,
    string ExecuteScriptPath);

internal sealed record HairballGeneratedScripts(
    string SetupScriptPath,
    string ExecuteScriptPath);

internal sealed record HairballBatchCommand(
    string Command,
    string? CapturePath)
{
    public static HairballBatchCommand Run(string command) => new(command, null);

    public static HairballBatchCommand Capture(string command, string capturePath) => new(command, capturePath);
}

internal sealed record HairballRunResult(
    int Seed,
    string RunRootPath,
    HairballScenarioSummary Scenario,
    HairballPrediction Prediction,
    IReadOnlyList<string> ExpectedPipelineEdgeKeys,
    IReadOnlyList<string> ActualPipelineEdgeKeys,
    IReadOnlyList<string> ExpectedDataEdgeKeys,
    IReadOnlyList<string> ActualDataEdgeKeys,
    int ActualMaxDependencyDepth,
    string DagStatus,
    string DeterminismStatus,
    string SynchronizationStatus,
    IReadOnlyList<string> Failures)
{
    public bool Passed => Failures.Count == 0;
}

internal sealed record HairballScenarioSummary(
    int PipelineCount,
    int TransformTaskCount,
    int StoredProcedureCount,
    int ResultRowsetStoredProcedureCount,
    int MultiTaskPipelineCount);
