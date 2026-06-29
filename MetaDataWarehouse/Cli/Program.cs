using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataWarehouse.Core;
using MetaDataWarehouseModel = MetaDataWarehouse.MetaDataWarehouseModel;

internal static class Program
{
    private const string AppName = "meta-data-warehouse";
    private const string ApplicationId = "app-meta-data-warehouse";
    private const string CommandWorkspaceDirectoryName = "meta-data-warehouse.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    private static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaDataWarehouseCommandHandlers(
            Presenter,
            AppName,
            new DataWarehouseAuthoringService());

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaDataWarehouseModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-new-workspace", handlers.RunNewWorkspace);

        foreach (var executableCommandId in MetaDataWarehouseCommandHandlers.AuthoringExecutableCommandIds)
        {
            runtime.Bind(executableCommandId, handlers.RunAddRecord);
        }

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
}
