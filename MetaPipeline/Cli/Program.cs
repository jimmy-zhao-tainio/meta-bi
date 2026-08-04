using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;

internal static class Program
{
    private const string AppName = "meta-pipeline";
    private const string ApplicationId = "app-meta-pipeline";
    private const string CommandWorkspaceDirectoryName = "meta-pipeline.MetaCli";
    private static readonly ConsolePresenter Presenter = new();

    static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaPipelineCommandHandlers(
            Presenter,
            AppName,
            new MetaPipeline.MetaPipelineWorkspaceService(),
            new MetaPipeline.MetaPipelineExecutionCommandService(),
            new MetaPipeline.MetaPipelineOperationalDbAdminService());

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaPipeline.MetaPipelineModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind(
                "exec-create",
                [MetaCliWorkspace.Create("output", "xml", "csharp", "sql")],
                handlers.RunCreate)
            .Bind("exec-add-pipeline", handlers.RunAddPipeline)
            .Bind("exec-inspect", handlers.RunInspect)
            .Bind("exec-add-step", handlers.RunAddStep)
            .Bind("exec-add-executable-step", handlers.RunAddExecutableStep)
            .Bind("exec-execute", handlers.RunExecute)
            .Bind("exec-execute-step", handlers.RunExecuteStep)
            .Bind("exec-execute-worker", handlers.RunExecuteWorker)
            .Bind("exec-execute-sqlserver", handlers.RunExecuteSqlServer)
            .Bind("exec-create-pipeline-db", handlers.RunCreatePipelineDb)
            .Bind("exec-prune-pipeline-db", handlers.RunPrunePipelineDb);

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
}
