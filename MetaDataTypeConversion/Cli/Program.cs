using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;

internal static class Program
{
    private const string AppName = "meta-data-type-conversion";
    private const string ApplicationId = "app-meta-data-type-conversion";
    private const string CommandWorkspaceDirectoryName = "meta-data-type-conversion.MetaCli";
    private static readonly ConsolePresenter Presenter = new();

    static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaDataTypeConversionCommandHandlers(
            Presenter,
            new MetaDataTypeConversionService(),
            AppName);

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaDataTypeConversionModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-new-workspace", handlers.RunNewWorkspace)
            .Bind("exec-check", handlers.RunCheck)
            .Bind("exec-resolve", handlers.RunResolve);

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
}
