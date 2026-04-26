internal static partial class Program
{
    private static (
        bool Ok,
        string PipelineWorkspacePath,
        string PipelineName,
        string TaskName,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string ErrorMessage) ParseExecutePipelineArgs(
        string[] args,
        int startIndex)
    {
        var pipelineWorkspacePath = string.Empty;
        var pipelineName = string.Empty;
        var taskName = string.Empty;
        var transformWorkspacePath = string.Empty;
        var bindingWorkspacePath = string.Empty;

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

            if (string.Equals(arg, "--task", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --task.");
                if (!string.IsNullOrWhiteSpace(taskName)) return FailParse("--task can only be provided once.");
                taskName = args[++i];
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

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(pipelineWorkspacePath)) return FailParse("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(pipelineName)) return FailParse("missing required option --pipeline <name>.");
        if (string.IsNullOrWhiteSpace(taskName)) return FailParse("missing required option --task <name>.");
        if (string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("missing required option --binding-workspace <path>.");

        return (
            true,
            pipelineWorkspacePath,
            pipelineName,
            taskName,
            transformWorkspacePath,
            bindingWorkspacePath,
            string.Empty);

        (bool Ok, string PipelineWorkspacePath, string PipelineName, string TaskName, string TransformWorkspacePath, string BindingWorkspacePath, string ErrorMessage) FailParse(string message)
        {
            return (
                false,
                pipelineWorkspacePath,
                pipelineName,
                taskName,
                transformWorkspacePath,
                bindingWorkspacePath,
                message);
        }
    }

    private static (
        bool Ok,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string SourceConnectionEnvironmentVariableName,
        string TargetConnectionEnvironmentVariableName,
        string TransformScriptId,
        string TransformBindingId,
        string? TargetSqlIdentifier,
        int BatchSize,
        string ErrorMessage) ParseExecuteSqlServerArgs(
        string[] args,
        int startIndex)
    {
        var transformWorkspacePath = string.Empty;
        var bindingWorkspacePath = string.Empty;
        var sourceConnectionEnvironmentVariableName = string.Empty;
        var targetConnectionEnvironmentVariableName = string.Empty;
        var transformScriptId = string.Empty;
        var transformBindingId = string.Empty;
        string? targetSqlIdentifier = null;
        var batchSize = 1000;
        var batchSizeSpecified = false;

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

            if (string.Equals(arg, "--source-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --source-connection-env.");
                if (!string.IsNullOrWhiteSpace(sourceConnectionEnvironmentVariableName)) return FailParse("--source-connection-env can only be provided once.");
                sourceConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --target-connection-env.");
                if (!string.IsNullOrWhiteSpace(targetConnectionEnvironmentVariableName)) return FailParse("--target-connection-env can only be provided once.");
                targetConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--transform-script-id", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --transform-script-id.");
                if (!string.IsNullOrWhiteSpace(transformScriptId)) return FailParse("--transform-script-id can only be provided once.");
                transformScriptId = args[++i];
                continue;
            }

            if (string.Equals(arg, "--transform-binding-id", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --transform-binding-id.");
                if (!string.IsNullOrWhiteSpace(transformBindingId)) return FailParse("--transform-binding-id can only be provided once.");
                transformBindingId = args[++i];
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

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("missing required option --binding-workspace <path>.");
        if (string.IsNullOrWhiteSpace(transformScriptId)) return FailParse("missing required option --transform-script-id <id>.");
        if (string.IsNullOrWhiteSpace(transformBindingId)) return FailParse("missing required option --transform-binding-id <id>.");
        if (string.IsNullOrWhiteSpace(sourceConnectionEnvironmentVariableName)) return FailParse("missing required option --source-connection-env <name>.");
        if (string.IsNullOrWhiteSpace(targetConnectionEnvironmentVariableName)) return FailParse("missing required option --target-connection-env <name>.");

        return (
            true,
            transformWorkspacePath,
            bindingWorkspacePath,
            sourceConnectionEnvironmentVariableName,
            targetConnectionEnvironmentVariableName,
            transformScriptId,
            transformBindingId,
            string.IsNullOrWhiteSpace(targetSqlIdentifier) ? null : targetSqlIdentifier,
            batchSize,
            string.Empty);

        (bool Ok, string TransformWorkspacePath, string BindingWorkspacePath, string SourceConnectionEnvironmentVariableName, string TargetConnectionEnvironmentVariableName, string TransformScriptId, string TransformBindingId, string? TargetSqlIdentifier, int BatchSize, string ErrorMessage) FailParse(string message)
        {
            return (
                false,
                transformWorkspacePath,
                bindingWorkspacePath,
                sourceConnectionEnvironmentVariableName,
                targetConnectionEnvironmentVariableName,
                transformScriptId,
                transformBindingId,
                targetSqlIdentifier,
                batchSize,
                message);
        }
    }

    private static (
        bool Ok,
        string NewWorkspacePath,
        string ErrorMessage) ParseInitArgs(string[] args, int startIndex)
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
        string TaskName,
        string TransformWorkspacePath,
        string BindingWorkspacePath,
        string TransformScriptId,
        string TransformBindingId,
        string SourceConnectionReferenceName,
        string SourceConnectionEnvironmentVariableName,
        string TargetConnectionReferenceName,
        string TargetConnectionEnvironmentVariableName,
        string? TargetSqlIdentifier,
        int BatchSize,
        bool BatchSizeSpecified,
        string ErrorMessage) ParseAddTransformArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var pipelineName = string.Empty;
        var taskName = string.Empty;
        var transformWorkspacePath = string.Empty;
        var bindingWorkspacePath = string.Empty;
        var transformScriptId = string.Empty;
        var transformBindingId = string.Empty;
        var sourceConnectionReferenceName = string.Empty;
        var sourceConnectionEnvironmentVariableName = string.Empty;
        var targetConnectionReferenceName = string.Empty;
        var targetConnectionEnvironmentVariableName = string.Empty;
        string? targetSqlIdentifier = null;
        var batchSize = 1000;
        var batchSizeSpecified = false;

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

            if (string.Equals(arg, "--task", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --task.");
                if (!string.IsNullOrWhiteSpace(taskName)) return FailParse("--task can only be provided once.");
                taskName = args[++i];
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

            if (string.Equals(arg, "--transform-script-id", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --transform-script-id.");
                if (!string.IsNullOrWhiteSpace(transformScriptId)) return FailParse("--transform-script-id can only be provided once.");
                transformScriptId = args[++i];
                continue;
            }

            if (string.Equals(arg, "--transform-binding-id", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --transform-binding-id.");
                if (!string.IsNullOrWhiteSpace(transformBindingId)) return FailParse("--transform-binding-id can only be provided once.");
                transformBindingId = args[++i];
                continue;
            }

            if (string.Equals(arg, "--source-connection-ref", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --source-connection-ref.");
                if (!string.IsNullOrWhiteSpace(sourceConnectionReferenceName)) return FailParse("--source-connection-ref can only be provided once.");
                sourceConnectionReferenceName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--source-connection-env", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --source-connection-env.");
                if (!string.IsNullOrWhiteSpace(sourceConnectionEnvironmentVariableName)) return FailParse("--source-connection-env can only be provided once.");
                sourceConnectionEnvironmentVariableName = args[++i];
                continue;
            }

            if (string.Equals(arg, "--target-connection-ref", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return FailParse("missing value for --target-connection-ref.");
                if (!string.IsNullOrWhiteSpace(targetConnectionReferenceName)) return FailParse("--target-connection-ref can only be provided once.");
                targetConnectionReferenceName = args[++i];
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

            return FailParse($"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(workspacePath)) return FailParse("missing required option --workspace <path>.");
        if (string.IsNullOrWhiteSpace(pipelineName)) return FailParse("missing required option --pipeline <name>.");
        if (string.IsNullOrWhiteSpace(taskName)) return FailParse("missing required option --task <name>.");
        if (string.IsNullOrWhiteSpace(transformWorkspacePath)) return FailParse("missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(bindingWorkspacePath)) return FailParse("missing required option --binding-workspace <path>.");
        if (string.IsNullOrWhiteSpace(transformScriptId)) return FailParse("missing required option --transform-script-id <id>.");
        if (string.IsNullOrWhiteSpace(transformBindingId)) return FailParse("missing required option --transform-binding-id <id>.");
        if (string.IsNullOrWhiteSpace(sourceConnectionReferenceName)) return FailParse("missing required option --source-connection-ref <name>.");
        if (string.IsNullOrWhiteSpace(sourceConnectionEnvironmentVariableName)) return FailParse("missing required option --source-connection-env <name>.");
        if (string.IsNullOrWhiteSpace(targetConnectionReferenceName)) return FailParse("missing required option --target-connection-ref <name>.");
        if (string.IsNullOrWhiteSpace(targetConnectionEnvironmentVariableName)) return FailParse("missing required option --target-connection-env <name>.");

        return (
            true,
            workspacePath,
            pipelineName,
            taskName,
            transformWorkspacePath,
            bindingWorkspacePath,
            transformScriptId,
            transformBindingId,
            sourceConnectionReferenceName,
            sourceConnectionEnvironmentVariableName,
            targetConnectionReferenceName,
            targetConnectionEnvironmentVariableName,
            targetSqlIdentifier,
            batchSize,
            batchSizeSpecified,
            string.Empty);

        (bool Ok, string WorkspacePath, string PipelineName, string TaskName, string TransformWorkspacePath, string BindingWorkspacePath, string TransformScriptId, string TransformBindingId, string SourceConnectionReferenceName, string SourceConnectionEnvironmentVariableName, string TargetConnectionReferenceName, string TargetConnectionEnvironmentVariableName, string? TargetSqlIdentifier, int BatchSize, bool BatchSizeSpecified, string ErrorMessage) FailParse(string message)
        {
            return (
                false,
                workspacePath,
                pipelineName,
                taskName,
                transformWorkspacePath,
                bindingWorkspacePath,
                transformScriptId,
                transformBindingId,
                sourceConnectionReferenceName,
                sourceConnectionEnvironmentVariableName,
                targetConnectionReferenceName,
                targetConnectionEnvironmentVariableName,
                targetSqlIdentifier,
                batchSize,
                batchSizeSpecified,
                message);
        }
    }
}
