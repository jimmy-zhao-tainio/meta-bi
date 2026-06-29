using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;

internal sealed partial class MetaPipelineCommandHandlers
{
    private readonly ConsolePresenter presenter;
    private readonly string appName;
    private readonly MetaPipeline.MetaPipelineWorkspaceService workspaceService;
    private readonly MetaPipeline.MetaPipelineExecutionCommandService pipelineExecutionService;
    private readonly MetaPipeline.MetaPipelineOperationalDbAdminService operationalDbAdminService;

    public MetaPipelineCommandHandlers(
        ConsolePresenter presenter,
        string appName,
        MetaPipeline.MetaPipelineWorkspaceService workspaceService,
        MetaPipeline.MetaPipelineExecutionCommandService pipelineExecutionService,
        MetaPipeline.MetaPipelineOperationalDbAdminService operationalDbAdminService)
    {
        this.presenter = presenter;
        this.appName = appName;
        this.workspaceService = workspaceService;
        this.pipelineExecutionService = pipelineExecutionService;
        this.operationalDbAdminService = operationalDbAdminService;
    }

    public void RunNewWorkspace(MetaCliInvocation invocation) =>
        Complete(() => RunNewWorkspaceCore(invocation));

    public void RunAddPipeline(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model) =>
        Complete(() => RunAddPipelineCore(invocation, model));

    public void RunInspect(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model) =>
        Complete(() => RunInspectCore(invocation, model));

    public void RunAddStep(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model) =>
        Complete(() => RunAddStepCore(invocation, model));

    public void RunAddExecutableStep(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model) =>
        Complete(() => RunAddExecutableStepCore(invocation, model));

    public void RunExecute(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model) =>
        CompleteAsync(() => RunExecuteAsync(invocation, model));

    public void RunExecuteStep(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model) =>
        CompleteAsync(() => RunExecuteStepAsync(invocation, model));

    public void RunExecuteWorker(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model) =>
        CompleteAsync(() => RunExecuteWorkerAsync(invocation, model));

    public void RunExecuteSqlServer(MetaCliInvocation invocation) =>
        CompleteAsync(() => RunExecuteSqlServerAsync(invocation));

    public void RunCreatePipelineDb(MetaCliInvocation invocation) =>
        CompleteAsync(() => RunCreatePipelineDbAsync(invocation));

    public void RunPrunePipelineDb(MetaCliInvocation invocation) =>
        CompleteAsync(() => RunPrunePipelineDbAsync(invocation));

    private void Complete(Func<int> action)
    {
        var exitCode = action();
        if (exitCode != 0)
        {
            throw new MetaCliExitException(exitCode);
        }
    }

    private void CompleteAsync(Func<Task<int>> action) =>
        Complete(() => action().GetAwaiter().GetResult());

    private string HelpCommand(string commandName) => $"{appName} help {commandName}";

    private int Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details.Where(static detail => !string.IsNullOrWhiteSpace(detail)));
        }

        renderedDetails.Add($"Next: {next}");
        presenter.WriteFailure(message, renderedDetails);
        return exitCode;
    }
}
