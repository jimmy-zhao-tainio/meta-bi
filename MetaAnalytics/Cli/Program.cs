using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaAnalytics.Core;
using MetaCli.Core;
using MetaAnalyticsModel = MetaAnalytics.MetaAnalyticsModel;

internal static class Program
{
    private const string AppName = "meta-analytics";
    private const string ApplicationId = "app-meta-analytics";
    private const string CommandWorkspaceDirectoryName = "meta-analytics.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    private static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaAnalyticsCommandHandlers(
            Presenter,
            AppName,
            new AnalyticsAuthoringService());

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaAnalyticsModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-new-workspace", handlers.RunNewWorkspace);

        foreach (var executableCommandId in MetaAnalyticsCommandHandlers.AuthoringExecutableCommandIds)
        {
            runtime.Bind(executableCommandId, handlers.RunAddRecord);
        }

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
}
