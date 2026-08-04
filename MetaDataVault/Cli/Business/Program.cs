using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaBusinessDataVault;
using MetaCli.Core;
using MetaDataVault.Core;

internal static class Program
{
    private const string AppName = "meta-datavault-business";
    private const string ApplicationId = "app-meta-datavault-business";
    private const string CommandWorkspaceDirectoryName = "meta-datavault-business.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    private static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaBusinessDataVaultCommandHandlers(
            Presenter,
            AppName,
            new BusinessDataVaultAuthoringService());

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaBusinessDataVaultModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind(
                "exec-create",
                [MetaCliWorkspace.Create("output", "xml", "csharp", "sql")],
                handlers.RunCreate);

        foreach (var executableCommandId in MetaBusinessDataVaultCommandHandlers.AuthoringExecutableCommandIds)
        {
            runtime.Bind(executableCommandId, handlers.RunAddRecord);
        }

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
}
