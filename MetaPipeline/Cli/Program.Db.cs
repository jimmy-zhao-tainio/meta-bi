using Meta.Core.Presentation.Cli;
using MetaCli.Core;

internal static partial class Program
{
    private static readonly MetaPipeline.MetaPipelineOperationalDbAdminService OperationalDbAdminService = new();

    private static async Task<int> RunCreatePipelineDbAsync(MetaCliInvocation invocation)
    {
        var parse = ReadCreatePipelineDbArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("create-pipeline-db"));
        }

        try
        {
            using (var activity = CliActivityLine.Start("Creating"))
            {
                await OperationalDbAdminService
                    .CreateDatabaseAndBootstrapAsync(
                        parse.PipelineDbConnectionEnvironmentVariableName,
                        parse.PipelineDbName)
                    .ConfigureAwait(false);

                activity.Succeed();
            }

            return 0;
        }
        catch (Meta.Core.Connections.ConnectionEnvironmentVariableException ex)
        {
            return Fail(
                "Cannot create MetaPipeline operational DB.",
                "set the named connection environment variable and retry.",
                4,
                [$"  {ex.Message}"]);
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot create MetaPipeline operational DB.",
                "check the connection environment variable, database name, and SQL Server permissions, then retry.",
                4,
                new[]
                {
                    $"  ConnectionEnv: {parse.PipelineDbConnectionEnvironmentVariableName}",
                    $"  Database: {parse.PipelineDbName}",
                    $"  {ex.Message}",
                });
        }
    }

    private static async Task<int> RunPrunePipelineDbAsync(MetaCliInvocation invocation)
    {
        var parse = ReadPrunePipelineDbArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("prune-pipeline-db"));
        }

        try
        {
            using (var activity = CliActivityLine.Start(parse.DryRun ? "Checking" : "Pruning"))
            {
                await OperationalDbAdminService
                    .PruneAsync(
                        parse.PipelineDbConnectionEnvironmentVariableName,
                        parse.RetentionDays,
                        parse.DryRun)
                    .ConfigureAwait(false);

                activity.Succeed();
            }

            return 0;
        }
        catch (Meta.Core.Connections.ConnectionEnvironmentVariableException ex)
        {
            return Fail(
                "Cannot prune MetaPipeline operational DB.",
                "set the named pipeline DB connection environment variable and retry.",
                4,
                [$"  {ex.Message}"]);
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot prune MetaPipeline operational DB.",
                "check the pipeline DB connection environment variable, schema, and SQL Server permissions, then retry.",
                4,
                new[]
                {
                    $"  ConnectionEnv: {parse.PipelineDbConnectionEnvironmentVariableName}",
                    $"  RetentionDays: {parse.RetentionDays}",
                    $"  {ex.Message}",
                });
        }
    }
}
