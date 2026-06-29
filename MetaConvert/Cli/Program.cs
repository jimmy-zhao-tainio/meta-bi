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
            .Bind("exec-schema-to-raw-datavault", invocation => CompleteAsync(() => handlers.RunSchemaToRawDataVaultAsync(invocation)))
            .Bind("exec-raw-datavault-to-sql", invocation => CompleteAsync(() => handlers.RunRawDataVaultToSqlAsync(invocation)))
            .Bind("exec-business-datavault-to-sql", invocation => CompleteAsync(() => handlers.RunBusinessDataVaultToSqlAsync(invocation)))
            .Bind("exec-data-quality-to-sql", invocation => CompleteAsync(() => handlers.RunDataQualityToSqlAsync(invocation)))
            .Bind("exec-data-warehouse-to-sql", invocation => CompleteAsync(() => handlers.RunDataWarehouseToSqlAsync(invocation)))
            .Bind("exec-transform-script-to-sql", invocation => CompleteAsync(() => handlers.RunTransformScriptToSqlAsync(invocation)))
            .Bind("exec-sql-to-transform-script", invocation => CompleteAsync(() => handlers.RunSqlToTransformScriptAsync(invocation)))
            .Bind("exec-analytics-to-tabular", invocation => CompleteAsync(() => handlers.RunAnalyticsToTabularAsync(invocation)))
            .Bind("exec-analytics-to-multi-dimensional", invocation => CompleteAsync(() => handlers.RunAnalyticsToMultiDimensionalAsync(invocation)));

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
