using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataQuality;
using MetaDataQuality.Core;

internal static class Program
{
    private const string AppName = "meta-data-quality";
    private const string ApplicationId = "app-meta-data-quality";
    private const string CommandWorkspaceDirectoryName = "meta-data-quality.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaDataQualityCommandHandlers(
            Presenter,
            new MetaDataQualityWorkspaceService(),
            new MetaDataQualityInspectionService(),
            new MetaDataQualityPromotionService());

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaDataQualityModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-from-transform-workspace", handlers.RunFromTransformWorkspace)
            .Bind("exec-inspect", handlers.RunInspect)
            .Bind("exec-promote", handlers.RunPromote);

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
}
