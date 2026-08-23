using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaTransformScript;
using MetaTransformScript.Sql;

internal static class Program
{
    private const string AppName = "meta-transform-script";
    private const string ApplicationId = "app-meta-transform-script";
    private const string CommandWorkspaceDirectoryName = "meta-transform-script.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    public static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaTransformScriptCommandHandlers(
            Presenter,
            new MetaTransformScriptSqlService(),
            AppName);

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaTransformScriptModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .BindTarget("exec-from-sql-file", OutputWorkspace(), handlers.RunFromSqlFileAsync)
            .BindTarget("exec-from-sql-files", OutputWorkspace(), handlers.RunFromSqlFilesAsync)
            .BindTarget("exec-from-sql-code", OutputWorkspace(), handlers.RunFromSqlCodeAsync)
            .BindTarget("exec-from-sql-script-workspace", OutputWorkspace(), handlers.RunFromSqlScriptWorkspaceAsync)
            .Bind("exec-to-sql-path", (invocation, model) => RunAsync(() => handlers.RunToSqlPathAsync(invocation, model)))
            .Bind("exec-to-sql-code", (invocation, model) => handlers.RunToSqlCode(invocation, model))
            .Bind("exec-target-identifiers-from-pattern", handlers.RunTargetIdentifiersFromPattern)
            .Bind("exec-stored-procedure-view-contract", handlers.RunStoredProcedureViewContract)
            .Bind("exec-stored-procedure-add-contract", handlers.RunStoredProcedureAddContract)
            .Bind("exec-stored-procedure-remove-contract", handlers.RunStoredProcedureRemoveContract);

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);

    private static MetaCliWorkspaceParameter[] OutputWorkspace() =>
    [
        MetaCliWorkspace.Create(
            "output",
            "output-xml",
            "output-csharp",
            "output-sql",
            "output-connection-env")
    ];

    private static void RunAsync(Func<Task> action) =>
        action().GetAwaiter().GetResult();
}
