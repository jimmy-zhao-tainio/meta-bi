using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaSql;

internal static class Program
{
    private const string AppName = "meta-sql";
    private const string ApplicationId = "app-meta-sql";
    private const string CommandWorkspaceDirectoryName = "meta-sql.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    public static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaSqlCommandHandlers(Presenter, AppName);

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaSqlModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind(
                "exec-extract-sqlserver",
                [MetaCliWorkspace.Create("output", "output-xml", "output-csharp", "output-sql", "output-connection-env")],
                handlers.RunExtractSqlServerAsync)
            .Bind("exec-deploy-plan", invocation => CompleteAsync(() => handlers.RunDeployPlanAsync(invocation)))
            .Bind("exec-deploy", invocation => CompleteAsync(() => handlers.RunDeployAsync(invocation)))
            .Bind("exec-execute", invocation => CompleteAsync(() => handlers.RunExecuteAsync(invocation)));

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);

    private static void CompleteAsync(Func<Task<int>> action)
    {
        var exitCode = action().GetAwaiter().GetResult();
        if (exitCode != 0)
        {
            throw new MetaCliExitException(exitCode);
        }
    }
}
