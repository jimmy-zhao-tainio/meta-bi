using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaTabular.Core;
using MetaTabular.Core.Deploy;
using MetaTabularModel = MetaTabular.MetaTabularModel;

internal static class Program
{
    private const string AppName = "meta-tabular";
    private const string ApplicationId = "app-meta-tabular";
    private const string CommandWorkspaceDirectoryName = "meta-tabular.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    private static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaTabularCommandHandlers(
            Presenter,
            AppName,
            new TabularAuthoringService(),
            new MetaTabularDeployService(),
            new MetaTabularProcessService(),
            new MetaTabularRestoreService(),
            new MetaTabularDropService());

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaTabularModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind(
                "exec-create",
                [MetaCliWorkspace.Create("output", "xml", "csharp", "sql")],
                handlers.RunCreate)
            .Bind("exec-deploy", handlers.RunDeploy)
            .Bind("exec-process", handlers.RunProcess)
            .Bind("exec-restore", handlers.RunRestore)
            .Bind("exec-drop", handlers.RunDrop);

        foreach (var executableCommandId in MetaTabularCommandHandlers.AuthoringExecutableCommandIds)
        {
            runtime.Bind(executableCommandId, handlers.RunAddRecord);
        }

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
}
