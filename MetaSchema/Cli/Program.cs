using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaSchema;
using MetaSchema.Extractors.SqlServer;

internal static class Program
{
    private const string AppName = "meta-schema";
    private const string ApplicationId = "app-meta-schema";
    private const string CommandWorkspaceDirectoryName = "meta-schema.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    public static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaSchemaCommandHandlers(
            Presenter,
            new MetaSchemaSqlServerExtractService(),
            AppName);

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaSchemaModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-extract-sqlserver", invocation => RunAsync(() => handlers.RunExtractSqlServerAsync(invocation)));

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);

    private static void RunAsync(Func<Task> action) =>
        action().GetAwaiter().GetResult();
}
