using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli;
using MetaCli.Core;

internal static class Program
{
    private const string AppName = "meta-convert";
    private const string ApplicationId = "app-meta-convert";
    private const string CommandWorkspaceDirectoryName = "meta-convert.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    public static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        var handlers = new MetaConvertCommandHandlers(Presenter, AppName);

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaCliModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind(
                "exec-schema-to-raw-datavault",
                [
                    MetaCliWorkspace.Open("source-workspace"),
                    MetaCliWorkspace.Create(
                        "output",
                        "output-xml",
                        "output-csharp",
                        "output-sql",
                        "output-connection-env"),
                ],
                (invocation, workspaces) => CompleteAsync(() =>
                    handlers.RunSchemaToRawDataVaultAsync(invocation, workspaces)))
            .Bind("exec-raw-datavault-to-sql", [OutputWorkspace()], (invocation, workspaces) => CompleteAsync(() => handlers.RunRawDataVaultToSqlAsync(invocation, workspaces)))
            .Bind("exec-business-datavault-to-sql", [OutputWorkspace()], (invocation, workspaces) => CompleteAsync(() => handlers.RunBusinessDataVaultToSqlAsync(invocation, workspaces)))
            .Bind("exec-data-quality-to-sql", invocation => CompleteAsync(() => handlers.RunDataQualityToSqlAsync(invocation)))
            .Bind("exec-data-warehouse-to-sql", [OutputWorkspace()], (invocation, workspaces) => CompleteAsync(() => handlers.RunDataWarehouseToSqlAsync(invocation, workspaces)))
            .Bind("exec-transform-pattern-to-sql-script", [OutputWorkspace()], (invocation, workspaces) => CompleteAsync(() => handlers.RunTransformPatternToSqlScriptAsync(invocation, workspaces)))
            .Bind("exec-transform-script-to-sql", [OutputWorkspace()], (invocation, workspaces) => CompleteAsync(() => handlers.RunTransformScriptToSqlAsync(invocation, workspaces)))
            .Bind("exec-sql-to-transform-script", [OutputWorkspace()], (invocation, workspaces) => CompleteAsync(() => handlers.RunSqlToTransformScriptAsync(invocation, workspaces)))
            .Bind("exec-analytics-to-tabular", [OutputWorkspace()], (invocation, workspaces) => CompleteAsync(() => handlers.RunAnalyticsToTabularAsync(invocation, workspaces)))
            .Bind("exec-analytics-to-multi-dimensional", [OutputWorkspace()], (invocation, workspaces) => CompleteAsync(() => handlers.RunAnalyticsToMultiDimensionalAsync(invocation, workspaces)));

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath =>
        Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);

    private static MetaCliWorkspaceOutput OutputWorkspace() =>
        MetaCliWorkspace.Create(
            "output",
            "output-xml",
            "output-csharp",
            "output-sql",
            "output-connection-env");

    private static void CompleteAsync(Func<Task<int>> action)
    {
        var exitCode = action().GetAwaiter().GetResult();
        if (exitCode != 0)
        {
            throw new MetaCliExitException(exitCode);
        }
    }
}
