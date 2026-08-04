using Meta.Core.Presentation.Cli;
using MetaCli.Core;

internal sealed partial class MetaPipelineCommandHandlers
{
    private int RunAddPipelineCore(
        MetaCliInvocation invocation,
        MetaPipeline.MetaPipelineModel model,
        MetaCliCommandCompletion completion)
    {
        var parse = ReadAddPipelineArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("add-pipeline"));
        }

        try
        {
            workspaceService.AddPipeline(
                model,
                parse.Name,
                parse.Description);

            completion.OnSucceeded(() => presenter.WriteOk());
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot update pipeline workspace.",
                "check the workspace path and pipeline name, then retry.",
                4,
                new[] { $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}", $"  {ex.Message}" });
        }
    }

    private int RunInspectCore(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model)
    {
        var parse = ReadWorkspaceOnlyArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("inspect"));
        }

        try
        {
            var result = workspaceService.Inspect(model);

            presenter.WriteOk("Loaded MetaPipeline workspace");
            presenter.WriteKeyValueBlock("MetaPipeline", new[]
            {
                ("Pipelines", result.PipelineCount.ToString()),
                ("Tasks", result.TaskCount.ToString()),
                ("Connections", result.ConnectionCount.ToString()),
                ("RowStreams", result.RowStreamCount.ToString()),
                ("RowStreamColumns", result.RowStreamColumnCount.ToString()),
                ("Dependencies", result.DependencyCount.ToString()),
            });

            foreach (var pipeline in result.Pipelines)
            {
                presenter.WriteKeyValueBlock($"Pipeline: {pipeline.Name}", new[]
                {
                    ("Id", pipeline.Id),
                    ("Tasks", pipeline.Tasks.Count.ToString()),
                });

                foreach (var task in pipeline.Tasks)
                {
                    presenter.WriteInfo($"  {task.Name} [{task.Label}]");
                }
            }

            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot inspect pipeline workspace.",
                "check the workspace path and instance data integrity, then retry.",
                4,
                new[] { $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}", $"  {ex.Message}" });
        }
    }

    private int RunAddExecutableStepCore(
        MetaCliInvocation invocation,
        MetaPipeline.MetaPipelineModel model,
        MetaCliCommandCompletion completion)
    {
        var parse = ReadAddExecutableStepArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("add-executable-step"));
        }

        try
        {
            workspaceService.AddExecutableStep(
                model,
                new MetaPipeline.MetaPipelineAddExecutableStepRequest(
                    parse.PipelineName,
                    parse.StepName,
                    parse.ExecutablePath,
                    parse.Arguments,
                    parse.WorkingDirectory,
                    parse.SuccessExitCode,
                    parse.SuccessExitCodeSpecified,
                    parse.TimeoutSeconds,
                    parse.TimeoutSecondsSpecified));

            completion.OnSucceeded(() => presenter.WriteOk());
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot update pipeline workspace.",
                "check the pipeline name and executable task inputs, then retry.",
                4,
                new[] { $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}", $"  {ex.Message}" });
        }
    }

    private int RunAddStepCore(
        MetaCliInvocation invocation,
        MetaPipeline.MetaPipelineModel model,
        MetaCliCommandCompletion completion)
    {
        var parse = ReadAddStepArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("add-step"));
        }

        try
        {
            workspaceService.AddStep(
                model,
                new MetaPipeline.MetaPipelineAddStepRequest(
                    parse.PipelineName,
                    parse.StepName,
                    parse.TransformWorkspacePath,
                    parse.BindingWorkspacePath,
                    parse.Script,
                    parse.Binding,
                    parse.ExecutionConnectionEnvironmentVariableName,
                    parse.TargetConnectionEnvironmentVariableName,
                    parse.TargetSqlIdentifier,
                    parse.TargetWriteModelName,
                    parse.TargetWriteModelSpecified,
                    parse.BatchSize,
                    parse.BatchSizeSpecified,
                    parse.TimeoutSeconds,
                    parse.TimeoutSecondsSpecified,
                    parse.TargetDataTypeSystemName,
                    parse.TargetDataTypeSystemSpecified));

            completion.OnSucceeded(() => presenter.WriteOk());
            return 0;
        }
        catch (Exception ex)
        {
            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}",
                $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
            };
            if (!string.IsNullOrWhiteSpace(parse.BindingWorkspacePath))
            {
                details.Add($"  BindingWorkspace: {Path.GetFullPath(parse.BindingWorkspacePath)}");
            }

            details.Add($"  {ex.Message}");
            return Fail(
                "Cannot update pipeline workspace.",
                "check the pipeline name and task inputs, then retry.",
                4,
                details);
        }
    }
}
