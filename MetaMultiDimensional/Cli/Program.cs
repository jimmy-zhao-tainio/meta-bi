using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaMultiDimensional.Core;
using MetaMultiDimensional.Core.Deploy;
using MetaMultiDimensionalModel = MetaMultiDimensional.MetaMultiDimensionalModel;

internal static class Program
{
    private const string AppName = "meta-multi-dimensional";
    private const string ApplicationId = "app-meta-multi-dimensional";
    private const string CommandWorkspaceDirectoryName = "meta-multi-dimensional.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    private static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaMultiDimensionalCommandHandlers(
            Presenter,
            AppName,
            new MultiDimensionalAuthoringService(),
            new MetaMultiDimensionalDeployService(),
            new MetaMultiDimensionalRestoreService(),
            new MetaMultiDimensionalDropService());

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaMultiDimensionalModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-new-workspace", handlers.RunNewWorkspace)
            .Bind("exec-deploy", handlers.RunDeploy)
            .Bind("exec-restore", handlers.RunRestore)
            .Bind("exec-drop", handlers.RunDrop);

        foreach (var executableCommandId in MetaMultiDimensionalCommandHandlers.AuthoringExecutableCommandIds)
        {
            runtime.Bind(executableCommandId, handlers.RunAddRecord);
        }

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
}
