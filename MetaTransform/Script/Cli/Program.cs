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
            .Bind("exec-from-sql-file", invocation => RunAsync(() => handlers.RunFromSqlFileAsync(invocation)))
            .Bind("exec-from-sql-files", invocation => RunAsync(() => handlers.RunFromSqlFilesAsync(invocation)))
            .Bind("exec-from-sql-code", invocation => RunAsync(() => handlers.RunFromSqlCodeAsync(invocation)))
            .Bind("exec-to-sql-path", (invocation, model) => RunAsync(() => handlers.RunToSqlPathAsync(invocation, model)))
            .Bind("exec-to-sql-code", (invocation, model) => handlers.RunToSqlCode(invocation, model))
            .Bind("exec-target-identifiers-from-pattern", invocation => RunAsync(() => handlers.RunTargetIdentifiersFromPatternAsync(invocation)))
            .Bind("exec-stored-procedure-view-contract", invocation => RunAsync(() => handlers.RunStoredProcedureViewContractAsync(invocation)))
            .Bind("exec-stored-procedure-add-contract", invocation => RunAsync(() => handlers.RunStoredProcedureAddContractAsync(invocation)))
            .Bind("exec-stored-procedure-remove-contract", invocation => RunAsync(() => handlers.RunStoredProcedureRemoveContractAsync(invocation)));

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);

    private static void RunAsync(Func<Task> action) =>
        action().GetAwaiter().GetResult();
}
