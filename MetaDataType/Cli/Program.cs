using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataType.Core;
using MetaDataTypeModel = MetaDataType.MetaDataTypeModel;

internal static class Program
{
    private const string AppName = "meta-data-type";
    private const string ApplicationId = "app-meta-data-type";
    private const string CommandWorkspaceDirectoryName = "meta-data-type.MetaCli";
    private static readonly ConsolePresenter Presenter = new();

    static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaDataTypeCommandHandlers(
            Presenter,
            new MetaDataTypeWorkspaceService());

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaDataTypeModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind(
                "exec-create",
                [MetaCliWorkspace.Create("output", "xml", "csharp", "sql")],
                handlers.RunCreate);

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
}
