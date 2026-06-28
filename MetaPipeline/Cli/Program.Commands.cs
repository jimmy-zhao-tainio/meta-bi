using Meta.Core.Presentation.Cli;
using MetaCli.Core;

internal static partial class Program
{
    private static readonly MetaPipeline.MetaPipelineWorkspaceService WorkspaceService = new();

    private static int RunNewWorkspace(MetaCliInvocation invocation)
    {
        var parse = ReadNewWorkspaceArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("new-workspace"));
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Fail(
                targetValidation.ErrorMessage,
                "choose a new folder or empty the target directory and retry.",
                4,
                targetValidation.Details);
        }

        WorkspaceService.CreateWorkspace(targetValidation.FullPath);
        Presenter.WriteOk();
        return 0;
    }

    private static int RunAddPipeline(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model)
    {
        var parse = ReadAddPipelineArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("add-pipeline"));
        }

        try
        {
            WorkspaceService.AddPipeline(
                model,
                parse.WorkspacePath,
                parse.Name,
                parse.Description);

            Presenter.WriteOk();
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

    private static int RunInspect(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model)
    {
        var parse = ReadWorkspaceOnlyArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("inspect"));
        }

        try
        {
            var result = WorkspaceService.Inspect(model);

            Presenter.WriteOk("Loaded MetaPipeline workspace");
            Presenter.WriteKeyValueBlock("MetaPipeline", new[]
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
                Presenter.WriteKeyValueBlock($"Pipeline: {pipeline.Name}", new[]
                {
                    ("Id", pipeline.Id),
                    ("Tasks", pipeline.Tasks.Count.ToString()),
                });

                foreach (var task in pipeline.Tasks)
                {
                    Presenter.WriteInfo($"  {task.Name} [{task.Label}]");
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

    private static int RunAddExecutableStep(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model)
    {
        var parse = ReadAddExecutableStepArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("add-executable-step"));
        }

        try
        {
            WorkspaceService.AddExecutableStep(
                model,
                new MetaPipeline.MetaPipelineAddExecutableStepRequest(
                    parse.WorkspacePath,
                    parse.PipelineName,
                    parse.StepName,
                    parse.ExecutablePath,
                    parse.Arguments,
                    parse.WorkingDirectory,
                    parse.SuccessExitCode,
                    parse.SuccessExitCodeSpecified,
                    parse.TimeoutSeconds,
                    parse.TimeoutSecondsSpecified));

            Presenter.WriteOk();
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

    private static int RunAddStep(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model)
    {
        var parse = ReadAddStepArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("add-step"));
        }

        try
        {
            WorkspaceService.AddStep(
                model,
                new MetaPipeline.MetaPipelineAddStepRequest(
                    parse.WorkspacePath,
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

            Presenter.WriteOk();
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
