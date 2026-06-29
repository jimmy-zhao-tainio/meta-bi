using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataVault.Core;
using MetaRawDataVault;

internal static class Program
{
    private const string AppName = "meta-datavault-raw";
    private const string ApplicationId = "app-meta-datavault-raw";
    private const string CommandWorkspaceDirectoryName = "meta-datavault-raw.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    private static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaRawDataVaultCommandHandlers(
            Presenter,
            AppName,
            new RawDataVaultAuthoringService());

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaRawDataVaultModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-new-workspace", handlers.RunNewWorkspace);

        foreach (var executableCommandId in MetaRawDataVaultCommandHandlers.AuthoringExecutableCommandIds)
        {
            runtime.Bind(executableCommandId, handlers.RunAddRecord);
        }

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
}
