internal static partial class Program
{
    private static (
        bool Ok,
        string PipelineWorkspacePath,
        string PipelineName,
        string StepName,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string DataTypeConversionWorkspacePath,
        string PipelineDbConnectionEnvironmentVariableName,
        string ErrorMessage) ParseExecuteStepArgs(
        string[] args,
        int startIndex)
    {
        var pipelineWorkspacePath = string.Empty;
        var pipelineName = string.Empty;
        var stepName = string.Empty;
        var transformWorkspacePath = string.Empty;
        var bindingWorkspacePath = string.Empty;
        var dataTypeConversionWorkspacePath = string.Empty;
        var pipelineDbConnectionEnvironmentVariableName = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(pipelineWorkspacePath)) return FailParse("--workspace can only be provided once.");
                pipelineWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--pipeline", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --pipeline.");
                if (!string.IsNullOrWhiteSpace(pipelineName)) return FailParse("--pipeline can only be provided once.");
                pipelineName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--step-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --step-name.");
                if (!string.IsNullOrWhiteSpace(stepName)) return FailParse("--step-name can only be provided once.");
                stepName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--transform-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --transform-workspace.");
                if (!string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("--transform-workspace can only be provided once.");
                transformWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--binding-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --binding-workspace.");
                if (!string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("--binding-workspace can only be provided once.");
                bindingWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--data-type-conversion-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --data-type-conversion-workspace.");
                if (!string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath)) return FailParse("--data-type-conversion-workspace can only be provided once.");
                dataTypeConversionWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--pipeline-db-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --pipeline-db-connection-env.");
                if (!string.IsNullOrWhiteSpace(pipelineDbConnectionEnvironmentVariableName)) return FailParse("--pipeline-db-connection-env can only be provided once.");
                pipelineDbConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(pipelineWorkspacePath)) return FailParse("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(pipelineName)) return FailParse("missing required option --pipeline <name>.");
        if (string.IsNullOrWhiteSpace(stepName)) return FailParse("missing required option --step-name <name-or-id>.");

        return (
            true,
            pipelineWorkspacePath,
            pipelineName,
            stepName,
            transformWorkspacePath,
            bindingWorkspacePath,
            dataTypeConversionWorkspacePath,
            pipelineDbConnectionEnvironmentVariableName,
            string.Empty);

        (bool Ok, string PipelineWorkspacePath, string PipelineName, string StepName, string TransformWorkspacePath, string BindingWorkspacePath, string DataTypeConversionWorkspacePath, string PipelineDbConnectionEnvironmentVariableName, string ErrorMessage) FailParse(string message)
        {
            return (
                false,
                pipelineWorkspacePath,
                pipelineName,
                stepName,
                transformWorkspacePath,
                bindingWorkspacePath,
                dataTypeConversionWorkspacePath,
                pipelineDbConnectionEnvironmentVariableName,
                message);
        }
    }

    private static (
        bool Ok,
        string PipelineWorkspacePath,
        string PipelineName,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string DataTypeConversionWorkspacePath,
        string PipelineDbConnectionEnvironmentVariableName,
        string WorkerControlPipeName,
        int? ControlPipeConnectTimeoutSeconds,
        string ErrorMessage) ParseExecutePipelineArgs(
        string[] args,
        int startIndex)
    {
        var pipelineWorkspacePath = string.Empty;
        var pipelineName = string.Empty;
        var transformWorkspacePath = string.Empty;
        var bindingWorkspacePath = string.Empty;
        var dataTypeConversionWorkspacePath = string.Empty;
        var pipelineDbConnectionEnvironmentVariableName = string.Empty;
        var workerControlPipeName = string.Empty;
        int? controlPipeConnectTimeoutSeconds = null;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(pipelineWorkspacePath)) return FailParse("--workspace can only be provided once.");
                pipelineWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--pipeline", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --pipeline.");
                if (!string.IsNullOrWhiteSpace(pipelineName)) return FailParse("--pipeline can only be provided once.");
                pipelineName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--transform-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --transform-workspace.");
                if (!string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("--transform-workspace can only be provided once.");
                transformWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--binding-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --binding-workspace.");
                if (!string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("--binding-workspace can only be provided once.");
                bindingWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--data-type-conversion-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --data-type-conversion-workspace.");
                if (!string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath)) return FailParse("--data-type-conversion-workspace can only be provided once.");
                dataTypeConversionWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--pipeline-db-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --pipeline-db-connection-env.");
                if (!string.IsNullOrWhiteSpace(pipelineDbConnectionEnvironmentVariableName)) return FailParse("--pipeline-db-connection-env can only be provided once.");
                pipelineDbConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--control-pipe", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --control-pipe.");
                if (!string.IsNullOrWhiteSpace(workerControlPipeName)) return FailParse("--control-pipe can only be provided once.");
                workerControlPipeName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--control-pipe-connect-timeout-seconds", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --control-pipe-connect-timeout-seconds.");
                if (controlPipeConnectTimeoutSeconds is not null) return FailParse("--control-pipe-connect-timeout-seconds can only be provided once.");
                var raw = args[++i];
                if (!int.TryParse(raw, out var parsedTimeoutSeconds) || parsedTimeoutSeconds < 0)
                {
                    return FailParse($"invalid value '{raw}' for --control-pipe-connect-timeout-seconds. Expected a non-negative integer.");
                }

                controlPipeConnectTimeoutSeconds = parsedTimeoutSeconds;
                continue;
            }

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(pipelineWorkspacePath)) return FailParse("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(pipelineName)) return FailParse("missing required option --pipeline <name>.");

        return (
            true,
            pipelineWorkspacePath,
            pipelineName,
            transformWorkspacePath,
            bindingWorkspacePath,
            dataTypeConversionWorkspacePath,
            pipelineDbConnectionEnvironmentVariableName,
            workerControlPipeName,
            controlPipeConnectTimeoutSeconds,
            string.Empty);

        (bool Ok, string PipelineWorkspacePath, string PipelineName, string TransformWorkspacePath, string BindingWorkspacePath, string DataTypeConversionWorkspacePath, string PipelineDbConnectionEnvironmentVariableName, string WorkerControlPipeName, int? ControlPipeConnectTimeoutSeconds, string ErrorMessage) FailParse(string message)
        {
            return (
                false,
                pipelineWorkspacePath,
                pipelineName,
                transformWorkspacePath,
                bindingWorkspacePath,
                dataTypeConversionWorkspacePath,
                pipelineDbConnectionEnvironmentVariableName,
                workerControlPipeName,
                controlPipeConnectTimeoutSeconds,
                message);
        }
    }

    private static (
        bool Ok,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string ExecutionConnectionEnvironmentVariableName,
        string TargetConnectionEnvironmentVariableName,
        string Script,
        string Binding,
        string? TargetSqlIdentifier,
        int BatchSize,
        int? TimeoutSeconds,
        string TargetDataTypeSystemName,
        string DataTypeConversionWorkspacePath,
        string PipelineDbConnectionEnvironmentVariableName,
        string ErrorMessage) ParseExecuteSqlServerArgs(
        string[] args,
        int startIndex)
    {
        var transformWorkspacePath = string.Empty;
        var bindingWorkspacePath = string.Empty;
        var executionConnectionEnvironmentVariableName = string.Empty;
        var targetConnectionEnvironmentVariableName = string.Empty;
        var script = string.Empty;
        var binding = string.Empty;
        string? targetSqlIdentifier = null;
        var batchSize = 1000;
        var batchSizeSpecified = false;
        int? timeoutSeconds = null;
        var timeoutSecondsSpecified = false;
        var targetDataTypeSystemName = "SqlServer";
        var targetDataTypeSystemSpecified = false;
        var dataTypeConversionWorkspacePath = string.Empty;
        var pipelineDbConnectionEnvironmentVariableName = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--transform-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --transform-workspace.");
                if (!string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("--transform-workspace can only be provided once.");
                transformWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--binding-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --binding-workspace.");
                if (!string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("--binding-workspace can only be provided once.");
                bindingWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--execution-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --execution-connection-env.");
                if (!string.IsNullOrWhiteSpace(executionConnectionEnvironmentVariableName)) return FailParse("--execution-connection-env can only be provided once.");
                executionConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --target-connection-env.");
                if (!string.IsNullOrWhiteSpace(targetConnectionEnvironmentVariableName)) return FailParse("--target-connection-env can only be provided once.");
                targetConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--script", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --script.");
                if (!string.IsNullOrWhiteSpace(script)) return FailParse("--script can only be provided once.");
                script = args[++i];
                continue;
            }

            if (string.Equals(arg, "--binding", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --binding.");
                if (!string.IsNullOrWhiteSpace(binding)) return FailParse("--binding can only be provided once.");
                binding = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --target.");
                if (!string.IsNullOrWhiteSpace(targetSqlIdentifier)) return FailParse("--target can only be provided once.");
                targetSqlIdentifier = args[++i];
                continue;
            }

            if (string.Equals(arg, "--batch-size", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --batch-size.");
                if (batchSizeSpecified) return FailParse("--batch-size can only be provided once.");
                var raw = args[++i];
                if (!int.TryParse(raw, out batchSize) || batchSize <= 0)
                {
                    return FailParse($"invalid value '{raw}' for --batch-size. Expected a positive integer.");
                }

                batchSizeSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--timeout-seconds", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --timeout-seconds.");
                if (timeoutSecondsSpecified) return FailParse("--timeout-seconds can only be provided once.");
                var raw = args[++i];
                if (!int.TryParse(raw, out var parsedTimeoutSeconds) || parsedTimeoutSeconds < 0)
                {
                    return FailParse($"invalid value '{raw}' for --timeout-seconds. Expected a non-negative integer; 0 means no timeout.");
                }

                timeoutSeconds = parsedTimeoutSeconds;
                timeoutSecondsSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--target-data-type-system", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --target-data-type-system.");
                if (targetDataTypeSystemSpecified) return FailParse("--target-data-type-system can only be provided once.");
                targetDataTypeSystemName = args[++i];
                targetDataTypeSystemSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--data-type-conversion-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --data-type-conversion-workspace.");
                if (!string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath)) return FailParse("--data-type-conversion-workspace can only be provided once.");
                dataTypeConversionWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--pipeline-db-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --pipeline-db-connection-env.");
                if (!string.IsNullOrWhiteSpace(pipelineDbConnectionEnvironmentVariableName)) return FailParse("--pipeline-db-connection-env can only be provided once.");
                pipelineDbConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("missing required option --binding-workspace <path>.");
        if (string.IsNullOrWhiteSpace(script)) return FailParse("missing required option --script <name-or-id>.");
        if (string.IsNullOrWhiteSpace(executionConnectionEnvironmentVariableName)) return FailParse("missing required option --execution-connection-env <name>.");
        if (string.IsNullOrWhiteSpace(targetDataTypeSystemName)) return FailParse("missing value for --target-data-type-system.");

        return (
            true,
            transformWorkspacePath,
            bindingWorkspacePath,
            executionConnectionEnvironmentVariableName,
            targetConnectionEnvironmentVariableName,
            script,
            binding,
            string.IsNullOrWhiteSpace(targetSqlIdentifier) ? null : targetSqlIdentifier,
            batchSize,
            timeoutSeconds,
            targetDataTypeSystemName,
            dataTypeConversionWorkspacePath,
            pipelineDbConnectionEnvironmentVariableName,
            string.Empty);

        (bool Ok, string TransformWorkspacePath, string BindingWorkspacePath, string ExecutionConnectionEnvironmentVariableName, string TargetConnectionEnvironmentVariableName, string Script, string Binding, string? TargetSqlIdentifier, int BatchSize, int? TimeoutSeconds, string TargetDataTypeSystemName, string DataTypeConversionWorkspacePath, string PipelineDbConnectionEnvironmentVariableName, string ErrorMessage) FailParse(string message)
        {
            return (
                false,
                transformWorkspacePath,
                bindingWorkspacePath,
                executionConnectionEnvironmentVariableName,
                targetConnectionEnvironmentVariableName,
                script,
                binding,
                targetSqlIdentifier,
                batchSize,
                timeoutSeconds,
                targetDataTypeSystemName,
                dataTypeConversionWorkspacePath,
                pipelineDbConnectionEnvironmentVariableName,
                message);
        }
    }

    private static (
        bool Ok,
        string PipelineDbConnectionEnvironmentVariableName,
        string PipelineDbName,
        string ErrorMessage) ParseCreatePipelineDbArgs(string[] args, int startIndex)
    {
        var pipelineDbConnectionEnvironmentVariableName = string.Empty;
        var pipelineDbName = MetaPipeline.MetaPipelineOperationalDbSchema.DefaultDatabaseName;
        var pipelineDbNameSpecified = false;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--pipeline-db-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse($"missing value for {arg}.");
                if (!string.IsNullOrWhiteSpace(pipelineDbConnectionEnvironmentVariableName)) return FailParse("--pipeline-db-connection-env can only be provided once.");
                pipelineDbConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--pipeline-db-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --pipeline-db-name.");
                if (pipelineDbNameSpecified) return FailParse("--pipeline-db-name can only be provided once.");
                pipelineDbName = args[++i];
                pipelineDbNameSpecified = true;
                continue;
            }

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(pipelineDbConnectionEnvironmentVariableName)) return FailParse("missing required option --pipeline-db-connection-env <name>.");
        if (string.IsNullOrWhiteSpace(pipelineDbName)) return FailParse("missing value for --pipeline-db-name.");

        return (true, pipelineDbConnectionEnvironmentVariableName, pipelineDbName, string.Empty);

        (bool Ok, string PipelineDbConnectionEnvironmentVariableName, string PipelineDbName, string ErrorMessage) FailParse(string message) =>
            (false, pipelineDbConnectionEnvironmentVariableName, pipelineDbName, message);
    }

    private static (
        bool Ok,
        string PipelineDbConnectionEnvironmentVariableName,
        int RetentionDays,
        bool DryRun,
        string ErrorMessage) ParsePrunePipelineDbArgs(string[] args, int startIndex)
    {
        var pipelineDbConnectionEnvironmentVariableName = string.Empty;
        var retentionDays = 0;
        var retentionDaysSpecified = false;
        var dryRun = false;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--pipeline-db-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --pipeline-db-connection-env.");
                if (!string.IsNullOrWhiteSpace(pipelineDbConnectionEnvironmentVariableName)) return FailParse("--pipeline-db-connection-env can only be provided once.");
                pipelineDbConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--retention-days", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --retention-days.");
                if (retentionDaysSpecified) return FailParse("--retention-days can only be provided once.");
                var raw = args[++i];
                if (!int.TryParse(raw, out retentionDays) || retentionDays <= 0)
                {
                    return FailParse($"invalid value '{raw}' for --retention-days. Expected a positive integer.");
                }

                retentionDaysSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--dry-run", StringComparison.OrdinalIgnoreCase))
            {
                if (dryRun) return FailParse("--dry-run can only be provided once.");
                dryRun = true;
                continue;
            }

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(pipelineDbConnectionEnvironmentVariableName)) return FailParse("missing required option --pipeline-db-connection-env <name>.");
        if (!retentionDaysSpecified) return FailParse("missing required option --retention-days <days>.");

        return (true, pipelineDbConnectionEnvironmentVariableName, retentionDays, dryRun, string.Empty);

        (bool Ok, string PipelineDbConnectionEnvironmentVariableName, int RetentionDays, bool DryRun, string ErrorMessage) FailParse(string message) =>
            (false, pipelineDbConnectionEnvironmentVariableName, retentionDays, dryRun, message);
    }

    private static (
        bool Ok,
        string NewWorkspacePath,
        string ErrorMessage) ParseNewWorkspaceArgs(string[] args, int startIndex)
    {
        var newWorkspacePath = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return FailParse("--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(newWorkspacePath)) return FailParse("missing required option --new-workspace <path>.");

        return (true, newWorkspacePath, string.Empty);

        (bool Ok, string NewWorkspacePath, string ErrorMessage) FailParse(string message) =>
            (false, newWorkspacePath, message);
    }

    private static (
        bool Ok,
        string WorkspacePath,
        string Name,
        string Description,
        string ErrorMessage) ParseAddPipelineArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var name = string.Empty;
        var description = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return FailParse("--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --name.");
                if (!string.IsNullOrWhiteSpace(name)) return FailParse("--name can only be provided once.");
                name = args[++i];
                continue;
            }

            if (string.Equals(arg, "--description", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --description.");
                if (!string.IsNullOrWhiteSpace(description)) return FailParse("--description can only be provided once.");
                description = args[++i];
                continue;
            }

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return FailParse("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(name)) return FailParse("missing required option --name <name>.");

        return (true, workspacePath, name, description, string.Empty);

        (bool Ok, string WorkspacePath, string Name, string Description, string ErrorMessage) FailParse(string message) =>
            (false, workspacePath, name, description, message);
    }

    private static (
        bool Ok,
        string WorkspacePath,
        string ErrorMessage) ParseWorkspaceOnlyArgs(
        string[] args,
        int startIndex,
        string _)
    {
        var workspacePath = string.Empty;
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (!string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                return (false, workspacePath, $"unknown option '{arg}'.");
            }

            if (i + 1 >= args.Length)
            {
                return (false, workspacePath, "missing value for --workspace.");
            }

            if (!string.IsNullOrWhiteSpace(workspacePath))
            {
                return (false, workspacePath, "--workspace can only be provided once.");
            }

            workspacePath = args[++i];
        }

        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return (false, workspacePath, "missing required option --workspace <path>.");
        }

        return (true, workspacePath, string.Empty);
    }

    private static (
        bool Ok,
        string WorkspacePath,
        string PipelineName,
        string StepName,
        string ExecutablePath,
        string Arguments,
        string WorkingDirectory,
        int SuccessExitCode,
        bool SuccessExitCodeSpecified,
        int? TimeoutSeconds,
        bool TimeoutSecondsSpecified,
        string ErrorMessage) ParseAddExecutableStepArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var pipelineName = string.Empty;
        var stepName = string.Empty;
        var executablePath = string.Empty;
        var arguments = string.Empty;
        var workingDirectory = string.Empty;
        var successExitCode = 0;
        var successExitCodeSpecified = false;
        int? timeoutSeconds = null;
        var timeoutSecondsSpecified = false;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return FailParse("--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--pipeline", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --pipeline.");
                if (!string.IsNullOrWhiteSpace(pipelineName)) return FailParse("--pipeline can only be provided once.");
                pipelineName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--step-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --step-name.");
                if (!string.IsNullOrWhiteSpace(stepName)) return FailParse("--step-name can only be provided once.");
                stepName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--executable", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --executable.");
                if (!string.IsNullOrWhiteSpace(executablePath)) return FailParse("--executable can only be provided once.");
                executablePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--arguments", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --arguments.");
                if (!string.IsNullOrWhiteSpace(arguments)) return FailParse("--arguments can only be provided once.");
                arguments = args[++i];
                continue;
            }

            if (string.Equals(arg, "--working-directory", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --working-directory.");
                if (!string.IsNullOrWhiteSpace(workingDirectory)) return FailParse("--working-directory can only be provided once.");
                workingDirectory = args[++i];
                continue;
            }

            if (string.Equals(arg, "--success-exit-code", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --success-exit-code.");
                if (successExitCodeSpecified) return FailParse("--success-exit-code can only be provided once.");
                var raw = args[++i];
                if (!int.TryParse(raw, out successExitCode))
                {
                    return FailParse($"invalid value '{raw}' for --success-exit-code. Expected an integer.");
                }

                successExitCodeSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--timeout-seconds", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --timeout-seconds.");
                if (timeoutSecondsSpecified) return FailParse("--timeout-seconds can only be provided once.");
                var raw = args[++i];
                if (!int.TryParse(raw, out var parsedTimeoutSeconds) || parsedTimeoutSeconds < 0)
                {
                    return FailParse($"invalid value '{raw}' for --timeout-seconds. Expected a non-negative integer; 0 means no timeout.");
                }

                timeoutSeconds = parsedTimeoutSeconds;
                timeoutSecondsSpecified = true;
                continue;
            }

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return FailParse("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(pipelineName)) return FailParse("missing required option --pipeline <name>.");
        if (string.IsNullOrWhiteSpace(executablePath)) return FailParse("missing required option --executable <path>.");

        return (
            true,
            workspacePath,
            pipelineName,
            stepName,
            executablePath,
            arguments,
            workingDirectory,
            successExitCode,
            successExitCodeSpecified,
            timeoutSeconds,
            timeoutSecondsSpecified,
            string.Empty);

        (bool Ok, string WorkspacePath, string PipelineName, string StepName, string ExecutablePath, string Arguments, string WorkingDirectory, int SuccessExitCode, bool SuccessExitCodeSpecified, int? TimeoutSeconds, bool TimeoutSecondsSpecified, string ErrorMessage) FailParse(string message)
        {
            return (
                false,
                workspacePath,
                pipelineName,
                stepName,
                executablePath,
                arguments,
                workingDirectory,
                successExitCode,
                successExitCodeSpecified,
                timeoutSeconds,
                timeoutSecondsSpecified,
                message);
        }
    }

    private static (
        bool Ok,
        string WorkspacePath,
        string PipelineName,
        string StepName,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string Script,
        string Binding,
        string ExecutionConnectionEnvironmentVariableName,
        string TargetConnectionEnvironmentVariableName,
        string? TargetSqlIdentifier,
        string TargetWriteModelName,
        bool TargetWriteModelSpecified,
        int BatchSize,
        bool BatchSizeSpecified,
        int? TimeoutSeconds,
        bool TimeoutSecondsSpecified,
        string TargetDataTypeSystemName,
        bool TargetDataTypeSystemSpecified,
        string ErrorMessage) ParseAddStepArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var pipelineName = string.Empty;
        var stepName = string.Empty;
        var transformWorkspacePath = string.Empty;
        var bindingWorkspacePath = string.Empty;
        var script = string.Empty;
        var binding = string.Empty;
        var executionConnectionEnvironmentVariableName = string.Empty;
        var targetConnectionEnvironmentVariableName = string.Empty;
        string? targetSqlIdentifier = null;
        var targetWriteModelName = "InsertRows";
        var targetWriteModelSpecified = false;
        var batchSize = 1000;
        var batchSizeSpecified = false;
        int? timeoutSeconds = null;
        var timeoutSecondsSpecified = false;
        var targetDataTypeSystemName = "SqlServer";
        var targetDataTypeSystemSpecified = false;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];

            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return FailParse("--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--pipeline", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --pipeline.");
                if (!string.IsNullOrWhiteSpace(pipelineName)) return FailParse("--pipeline can only be provided once.");
                pipelineName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--step-name", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --step-name.");
                if (!string.IsNullOrWhiteSpace(stepName)) return FailParse("--step-name can only be provided once.");
                stepName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--transform-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --transform-workspace.");
                if (!string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("--transform-workspace can only be provided once.");
                transformWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--binding-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --binding-workspace.");
                if (!string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("--binding-workspace can only be provided once.");
                bindingWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--script", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --script.");
                if (!string.IsNullOrWhiteSpace(script)) return FailParse("--script can only be provided once.");
                script = args[++i];
                continue;
            }

            if (string.Equals(arg, "--binding", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --binding.");
                if (!string.IsNullOrWhiteSpace(binding)) return FailParse("--binding can only be provided once.");
                binding = args[++i];
                continue;
            }

            if (string.Equals(arg, "--execution-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --execution-connection-env.");
                if (!string.IsNullOrWhiteSpace(executionConnectionEnvironmentVariableName)) return FailParse("--execution-connection-env can only be provided once.");
                executionConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --target-connection-env.");
                if (!string.IsNullOrWhiteSpace(targetConnectionEnvironmentVariableName)) return FailParse("--target-connection-env can only be provided once.");
                targetConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --target.");
                if (!string.IsNullOrWhiteSpace(targetSqlIdentifier)) return FailParse("--target can only be provided once.");
                targetSqlIdentifier = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target-write", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --target-write.");
                if (targetWriteModelSpecified) return FailParse("--target-write can only be provided once.");
                targetWriteModelName = args[++i];
                targetWriteModelSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--batch-size", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --batch-size.");
                if (batchSizeSpecified) return FailParse("--batch-size can only be provided once.");
                var raw = args[++i];
                if (!int.TryParse(raw, out batchSize) || batchSize <= 0)
                {
                    return FailParse($"invalid value '{raw}' for --batch-size. Expected a positive integer.");
                }

                batchSizeSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--timeout-seconds", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --timeout-seconds.");
                if (timeoutSecondsSpecified) return FailParse("--timeout-seconds can only be provided once.");
                var raw = args[++i];
                if (!int.TryParse(raw, out var parsedTimeoutSeconds) || parsedTimeoutSeconds < 0)
                {
                    return FailParse($"invalid value '{raw}' for --timeout-seconds. Expected a non-negative integer; 0 means no timeout.");
                }

                timeoutSeconds = parsedTimeoutSeconds;
                timeoutSecondsSpecified = true;
                continue;
            }

            if (string.Equals(arg, "--target-data-type-system", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --target-data-type-system.");
                if (targetDataTypeSystemSpecified) return FailParse("--target-data-type-system can only be provided once.");
                targetDataTypeSystemName = args[++i];
                targetDataTypeSystemSpecified = true;
                continue;
            }

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return FailParse("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(pipelineName)) return FailParse("missing required option --pipeline <name>.");
        if (string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("missing required option --binding-workspace <path>.");
        if (string.IsNullOrWhiteSpace(script)) return FailParse("missing required option --script <name-or-id>.");
        if (string.IsNullOrWhiteSpace(executionConnectionEnvironmentVariableName)) return FailParse("missing required option --execution-connection-env <name>.");
        if (string.IsNullOrWhiteSpace(targetDataTypeSystemName)) return FailParse("missing value for --target-data-type-system.");

        if (string.IsNullOrWhiteSpace(targetWriteModelName))
        {
            return FailParse("missing value for --target-write.");
        }

        var normalizedTargetWriteModelName = targetWriteModelName.Trim().ToLowerInvariant();
        if (normalizedTargetWriteModelName == "insertrows")
        {
            normalizedTargetWriteModelName = "insert-rows";
        }

        if (normalizedTargetWriteModelName is not "insert-rows")
        {
            return FailParse($"invalid value '{targetWriteModelName}' for --target-write. Expected 'insert-rows'.");
        }

        return (
            true,
            workspacePath,
            pipelineName,
            stepName,
            transformWorkspacePath,
            bindingWorkspacePath,
            script,
            binding,
            executionConnectionEnvironmentVariableName,
            targetConnectionEnvironmentVariableName,
            targetSqlIdentifier,
            "InsertRows",
            targetWriteModelSpecified,
            batchSize,
            batchSizeSpecified,
            timeoutSeconds,
            timeoutSecondsSpecified,
            targetDataTypeSystemName,
            targetDataTypeSystemSpecified,
            string.Empty);

        (bool Ok, string WorkspacePath, string PipelineName, string StepName, string TransformWorkspacePath, string BindingWorkspacePath, string Script, string Binding, string ExecutionConnectionEnvironmentVariableName, string TargetConnectionEnvironmentVariableName, string? TargetSqlIdentifier, string TargetWriteModelName, bool TargetWriteModelSpecified, int BatchSize, bool BatchSizeSpecified, int? TimeoutSeconds, bool TimeoutSecondsSpecified, string TargetDataTypeSystemName, bool TargetDataTypeSystemSpecified, string ErrorMessage) FailParse(string message)
        {
            return (
                false,
                workspacePath,
                pipelineName,
                stepName,
                transformWorkspacePath,
                bindingWorkspacePath,
                script,
                binding,
                executionConnectionEnvironmentVariableName,
                targetConnectionEnvironmentVariableName,
                targetSqlIdentifier,
                targetWriteModelName,
                targetWriteModelSpecified,
                batchSize,
                batchSizeSpecified,
                timeoutSeconds,
                timeoutSecondsSpecified,
                targetDataTypeSystemName,
                targetDataTypeSystemSpecified,
                message);
        }
    }

}
