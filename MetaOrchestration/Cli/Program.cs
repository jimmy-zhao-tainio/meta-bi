using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaOrchestration;

internal static class Program
{
    private const string AppName = "meta-orchestration";
    private const string ApplicationId = "app-meta-orchestration";
    private const string CommandWorkspaceDirectoryName = "meta-orchestration.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    private static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaOrchestrationCommandHandlers(Presenter, AppName);

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaOrchestrationModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-infer", handlers.RunInfer)
            .Bind("exec-inspect", handlers.RunInspect)
            .Bind("exec-list-issues", handlers.RunListIssues)
            .Bind("exec-explain-issue", handlers.RunExplainIssue)
            .Bind("exec-add-dependency", (invocation, model) => handlers.RunAddOrder(invocation, model, "add-dependency"))
            .Bind("exec-add-order", (invocation, model) => handlers.RunAddOrder(invocation, model, "add-order"))
            .Bind("exec-allow-concurrent-append", handlers.RunAllowConcurrentAppend)
            .Bind("exec-set-lock-policy", handlers.RunSetLockPolicy)
            .Bind("exec-refresh-run-plan", handlers.RunRefreshRunPlan)
            .Bind("exec-inspect-run-plan", handlers.RunInspectRunPlan)
            .Bind("exec-execute", invocation => CompleteAsync(() => handlers.RunExecuteAsync(invocation)));

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);

    private static void CompleteAsync(Func<Task> action) =>
        action().GetAwaiter().GetResult();
}
