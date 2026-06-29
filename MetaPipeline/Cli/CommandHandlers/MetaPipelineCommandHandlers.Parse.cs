using MetaCli.Core;

internal sealed partial class MetaPipelineCommandHandlers
{
    private const string DefaultWorkspacePath = ".";

    private static (
        bool Ok,
        string PipelineWorkspacePath,
        string PipelineName,
        string StepName,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string DataTypeConversionWorkspacePath,
        string PipelineDbConnectionEnvironmentVariableName,
        string ErrorMessage) ReadExecuteStepArgs(MetaCliInvocation invocation)
    {
        var pipelineWorkspacePath = WorkspacePath(invocation);
        var pipelineName = Required(invocation, "pipeline");
        var stepName = Required(invocation, "step-name");
        var transformWorkspacePath = Optional(invocation, "transform-workspace");
        var bindingWorkspacePath = Optional(invocation, "binding-workspace");
        var dataTypeConversionWorkspacePath = Optional(invocation, "data-type-conversion-workspace");
        var pipelineDbConnectionEnvironmentVariableName = Optional(invocation, "pipeline-db-connection-env");

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

        (bool Ok, string PipelineWorkspacePath, string PipelineName, string StepName, string TransformWorkspacePath, string BindingWorkspacePath, string DataTypeConversionWorkspacePath, string PipelineDbConnectionEnvironmentVariableName, string ErrorMessage) FailParse(string message) =>
            (false, pipelineWorkspacePath, pipelineName, stepName, transformWorkspacePath, bindingWorkspacePath, dataTypeConversionWorkspacePath, pipelineDbConnectionEnvironmentVariableName, message);
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
        string ErrorMessage) ReadExecutePipelineArgs(MetaCliInvocation invocation)
    {
        var pipelineWorkspacePath = WorkspacePath(invocation);
        var pipelineName = Required(invocation, "pipeline");
        var transformWorkspacePath = Optional(invocation, "transform-workspace");
        var bindingWorkspacePath = Optional(invocation, "binding-workspace");
        var dataTypeConversionWorkspacePath = Optional(invocation, "data-type-conversion-workspace");
        var pipelineDbConnectionEnvironmentVariableName = Optional(invocation, "pipeline-db-connection-env");
        var workerControlPipeName = Optional(invocation, "control-pipe");
        var controlPipeConnectTimeout = ReadOptionalNonNegativeInt(invocation, "control-pipe-connect-timeout-seconds", "--control-pipe-connect-timeout-seconds");

        if (!controlPipeConnectTimeout.Ok) return FailParse(controlPipeConnectTimeout.ErrorMessage);
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
            controlPipeConnectTimeout.Value,
            string.Empty);

        (bool Ok, string PipelineWorkspacePath, string PipelineName, string TransformWorkspacePath, string BindingWorkspacePath, string DataTypeConversionWorkspacePath, string PipelineDbConnectionEnvironmentVariableName, string WorkerControlPipeName, int? ControlPipeConnectTimeoutSeconds, string ErrorMessage) FailParse(string message) =>
            (false, pipelineWorkspacePath, pipelineName, transformWorkspacePath, bindingWorkspacePath, dataTypeConversionWorkspacePath, pipelineDbConnectionEnvironmentVariableName, workerControlPipeName, controlPipeConnectTimeout.Value, message);
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
        string ErrorMessage) ReadExecuteSqlServerArgs(MetaCliInvocation invocation)
    {
        var transformWorkspacePath = Required(invocation, "transform-workspace");
        var bindingWorkspacePath = Required(invocation, "binding-workspace");
        var executionConnectionEnvironmentVariableName = Required(invocation, "execution-connection-env");
        var targetConnectionEnvironmentVariableName = Optional(invocation, "target-connection-env");
        var script = Required(invocation, "script");
        var binding = Optional(invocation, "binding");
        var targetSqlIdentifier = OptionalNullable(invocation, "target");
        var batchSize = ReadPositiveInt(invocation, "batch-size", "--batch-size");
        var timeoutSeconds = ReadOptionalNonNegativeInt(invocation, "timeout-seconds", "--timeout-seconds");
        var targetDataTypeSystemName = Required(invocation, "target-data-type-system");
        var dataTypeConversionWorkspacePath = Optional(invocation, "data-type-conversion-workspace");
        var pipelineDbConnectionEnvironmentVariableName = Optional(invocation, "pipeline-db-connection-env");

        if (!batchSize.Ok) return FailParse(batchSize.ErrorMessage);
        if (!timeoutSeconds.Ok) return FailParse(timeoutSeconds.ErrorMessage);
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
            targetSqlIdentifier,
            batchSize.Value,
            timeoutSeconds.Value,
            targetDataTypeSystemName,
            dataTypeConversionWorkspacePath,
            pipelineDbConnectionEnvironmentVariableName,
            string.Empty);

        (bool Ok, string TransformWorkspacePath, string BindingWorkspacePath, string ExecutionConnectionEnvironmentVariableName, string TargetConnectionEnvironmentVariableName, string Script, string Binding, string? TargetSqlIdentifier, int BatchSize, int? TimeoutSeconds, string TargetDataTypeSystemName, string DataTypeConversionWorkspacePath, string PipelineDbConnectionEnvironmentVariableName, string ErrorMessage) FailParse(string message) =>
            (false, transformWorkspacePath, bindingWorkspacePath, executionConnectionEnvironmentVariableName, targetConnectionEnvironmentVariableName, script, binding, targetSqlIdentifier, batchSize.Value, timeoutSeconds.Value, targetDataTypeSystemName, dataTypeConversionWorkspacePath, pipelineDbConnectionEnvironmentVariableName, message);
    }

    private static (
        bool Ok,
        string PipelineDbConnectionEnvironmentVariableName,
        string PipelineDbName,
        string ErrorMessage) ReadCreatePipelineDbArgs(MetaCliInvocation invocation)
    {
        var connectionEnv = Required(invocation, "pipeline-db-connection-env");
        var databaseName = Required(invocation, "pipeline-db-name");
        if (string.IsNullOrWhiteSpace(connectionEnv)) return (false, connectionEnv, databaseName, "missing required option --pipeline-db-connection-env <name>.");
        if (string.IsNullOrWhiteSpace(databaseName)) return (false, connectionEnv, databaseName, "missing value for --pipeline-db-name.");
        return (true, connectionEnv, databaseName, string.Empty);
    }

    private static (
        bool Ok,
        string PipelineDbConnectionEnvironmentVariableName,
        int RetentionDays,
        bool DryRun,
        string ErrorMessage) ReadPrunePipelineDbArgs(MetaCliInvocation invocation)
    {
        var connectionEnv = Required(invocation, "pipeline-db-connection-env");
        var retentionDays = ReadPositiveInt(invocation, "retention-days", "--retention-days");
        var dryRun = Flag(invocation, "dry-run");
        if (!retentionDays.Ok) return (false, connectionEnv, retentionDays.Value, dryRun, retentionDays.ErrorMessage);
        if (string.IsNullOrWhiteSpace(connectionEnv)) return (false, connectionEnv, retentionDays.Value, dryRun, "missing required option --pipeline-db-connection-env <name>.");
        return (true, connectionEnv, retentionDays.Value, dryRun, string.Empty);
    }

    private static (
        bool Ok,
        string NewWorkspacePath,
        string ErrorMessage) ReadNewWorkspaceArgs(MetaCliInvocation invocation)
    {
        var newWorkspacePath = Required(invocation, "path");
        return string.IsNullOrWhiteSpace(newWorkspacePath)
            ? (false, newWorkspacePath, "missing required argument <path>.")
            : (true, newWorkspacePath, string.Empty);
    }

    private static (
        bool Ok,
        string WorkspacePath,
        string Name,
        string Description,
        string ErrorMessage) ReadAddPipelineArgs(MetaCliInvocation invocation)
    {
        var workspacePath = WorkspacePath(invocation);
        var name = Required(invocation, "name");
        var description = Optional(invocation, "description");
        if (string.IsNullOrWhiteSpace(name)) return (false, workspacePath, name, description, "missing required option --name <name>.");
        return (true, workspacePath, name, description, string.Empty);
    }

    private static (
        bool Ok,
        string WorkspacePath,
        string ErrorMessage) ReadWorkspaceOnlyArgs(MetaCliInvocation invocation) =>
        (true, WorkspacePath(invocation), string.Empty);

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
        string ErrorMessage) ReadAddExecutableStepArgs(MetaCliInvocation invocation)
    {
        var workspacePath = WorkspacePath(invocation);
        var pipelineName = Required(invocation, "pipeline");
        var stepName = Optional(invocation, "step-name");
        var executablePath = Required(invocation, "executable");
        var arguments = Optional(invocation, "arguments");
        var workingDirectory = Optional(invocation, "working-directory");
        var successExitCode = ReadInt(invocation, "success-exit-code", "--success-exit-code");
        var timeoutSeconds = ReadOptionalNonNegativeInt(invocation, "timeout-seconds", "--timeout-seconds");
        var successExitCodeSpecified = IsSpecified(invocation, "success-exit-code");
        var timeoutSecondsSpecified = IsSpecified(invocation, "timeout-seconds");

        if (!successExitCode.Ok) return FailParse(successExitCode.ErrorMessage);
        if (!timeoutSeconds.Ok) return FailParse(timeoutSeconds.ErrorMessage);
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
            successExitCode.Value,
            successExitCodeSpecified,
            timeoutSeconds.Value,
            timeoutSecondsSpecified,
            string.Empty);

        (bool Ok, string WorkspacePath, string PipelineName, string StepName, string ExecutablePath, string Arguments, string WorkingDirectory, int SuccessExitCode, bool SuccessExitCodeSpecified, int? TimeoutSeconds, bool TimeoutSecondsSpecified, string ErrorMessage) FailParse(string message) =>
            (false, workspacePath, pipelineName, stepName, executablePath, arguments, workingDirectory, successExitCode.Value, successExitCodeSpecified, timeoutSeconds.Value, timeoutSecondsSpecified, message);
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
        string ErrorMessage) ReadAddStepArgs(MetaCliInvocation invocation)
    {
        var workspacePath = WorkspacePath(invocation);
        var pipelineName = Required(invocation, "pipeline");
        var stepName = Optional(invocation, "step-name");
        var transformWorkspacePath = Required(invocation, "transform-workspace");
        var bindingWorkspacePath = Required(invocation, "binding-workspace");
        var script = Required(invocation, "script");
        var binding = Optional(invocation, "binding");
        var executionConnectionEnvironmentVariableName = Required(invocation, "execution-connection-env");
        var targetConnectionEnvironmentVariableName = Optional(invocation, "target-connection-env");
        var targetSqlIdentifier = OptionalNullable(invocation, "target");
        var targetWriteModelName = Required(invocation, "target-write");
        var targetWriteModelSpecified = IsSpecified(invocation, "target-write");
        var batchSize = ReadPositiveInt(invocation, "batch-size", "--batch-size");
        var batchSizeSpecified = IsSpecified(invocation, "batch-size");
        var timeoutSeconds = ReadOptionalNonNegativeInt(invocation, "timeout-seconds", "--timeout-seconds");
        var timeoutSecondsSpecified = IsSpecified(invocation, "timeout-seconds");
        var targetDataTypeSystemName = Required(invocation, "target-data-type-system");
        var targetDataTypeSystemSpecified = IsSpecified(invocation, "target-data-type-system");

        if (!batchSize.Ok) return FailParse(batchSize.ErrorMessage);
        if (!timeoutSeconds.Ok) return FailParse(timeoutSeconds.ErrorMessage);
        if (string.IsNullOrWhiteSpace(pipelineName)) return FailParse("missing required option --pipeline <name>.");
        if (string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("missing required option --binding-workspace <path>.");
        if (string.IsNullOrWhiteSpace(script)) return FailParse("missing required option --script <name-or-id>.");
        if (string.IsNullOrWhiteSpace(executionConnectionEnvironmentVariableName)) return FailParse("missing required option --execution-connection-env <name>.");
        if (string.IsNullOrWhiteSpace(targetDataTypeSystemName)) return FailParse("missing value for --target-data-type-system.");
        if (string.IsNullOrWhiteSpace(targetWriteModelName)) return FailParse("missing value for --target-write.");

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
            batchSize.Value,
            batchSizeSpecified,
            timeoutSeconds.Value,
            timeoutSecondsSpecified,
            targetDataTypeSystemName,
            targetDataTypeSystemSpecified,
            string.Empty);

        (bool Ok, string WorkspacePath, string PipelineName, string StepName, string TransformWorkspacePath, string BindingWorkspacePath, string Script, string Binding, string ExecutionConnectionEnvironmentVariableName, string TargetConnectionEnvironmentVariableName, string? TargetSqlIdentifier, string TargetWriteModelName, bool TargetWriteModelSpecified, int BatchSize, bool BatchSizeSpecified, int? TimeoutSeconds, bool TimeoutSecondsSpecified, string TargetDataTypeSystemName, bool TargetDataTypeSystemSpecified, string ErrorMessage) FailParse(string message) =>
            (false, workspacePath, pipelineName, stepName, transformWorkspacePath, bindingWorkspacePath, script, binding, executionConnectionEnvironmentVariableName, targetConnectionEnvironmentVariableName, targetSqlIdentifier, targetWriteModelName, targetWriteModelSpecified, batchSize.Value, batchSizeSpecified, timeoutSeconds.Value, timeoutSecondsSpecified, targetDataTypeSystemName, targetDataTypeSystemSpecified, message);
    }

    private static string WorkspacePath(MetaCliInvocation invocation) =>
        Optional(invocation, "workspace") is { Length: > 0 } workspacePath
            ? workspacePath
            : DefaultWorkspacePath;

    private static string Required(MetaCliInvocation invocation, string parameter) =>
        invocation.Required(parameter);

    private static string Optional(MetaCliInvocation invocation, string parameter) =>
        OptionalNullable(invocation, parameter) ?? string.Empty;

    private static string? OptionalNullable(MetaCliInvocation invocation, string parameter)
    {
        try
        {
            return invocation.Optional(parameter);
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    private static bool Flag(MetaCliInvocation invocation, string parameter)
    {
        try
        {
            return invocation.Flag(parameter);
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    private static bool IsSpecified(MetaCliInvocation invocation, string parameter)
    {
        try
        {
            return invocation.IsPresent(parameter);
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    private static (bool Ok, int Value, string ErrorMessage) ReadInt(
        MetaCliInvocation invocation,
        string parameter,
        string optionName)
    {
        var raw = Optional(invocation, parameter);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return (true, 0, string.Empty);
        }

        return int.TryParse(raw, out var value)
            ? (true, value, string.Empty)
            : (false, 0, $"invalid value '{raw}' for {optionName}. Expected an integer.");
    }

    private static (bool Ok, int Value, string ErrorMessage) ReadPositiveInt(
        MetaCliInvocation invocation,
        string parameter,
        string optionName)
    {
        var result = ReadInt(invocation, parameter, optionName);
        if (!result.Ok)
        {
            return result;
        }

        return result.Value > 0
            ? result
            : (false, result.Value, $"invalid value '{Optional(invocation, parameter)}' for {optionName}. Expected a positive integer.");
    }

    private static (bool Ok, int? Value, string ErrorMessage) ReadOptionalNonNegativeInt(
        MetaCliInvocation invocation,
        string parameter,
        string optionName)
    {
        var raw = Optional(invocation, parameter);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return (true, null, string.Empty);
        }

        if (!int.TryParse(raw, out var value) || value < 0)
        {
            return (false, null, $"invalid value '{raw}' for {optionName}. Expected a non-negative integer; 0 means no timeout.");
        }

        return (true, value, string.Empty);
    }
}
