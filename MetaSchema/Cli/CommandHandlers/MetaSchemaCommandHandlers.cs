using System.Globalization;
using Meta.Core.Connections;
using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaSchema.Extractors.SqlServer;

internal sealed class MetaSchemaCommandHandlers
{
    private readonly ConsolePresenter presenter;
    private readonly MetaSchemaSqlServerExtractService sqlServerExtractService;
    private readonly string appName;

    public MetaSchemaCommandHandlers(
        ConsolePresenter presenter,
        MetaSchemaSqlServerExtractService sqlServerExtractService,
        string appName)
    {
        this.presenter = presenter;
        this.sqlServerExtractService = sqlServerExtractService;
        this.appName = appName;
    }

    public async Task RunExtractSqlServerAsync(
        MetaCliInvocation invocation,
        MetaCliWorkspaces workspaces)
    {
        var output = MetaCliWorkspace.OutputLocation(invocation, "output-xml", "output-csharp", "output-sql");

        string connectionString;
        var connectionEnvironmentVariableName = invocation.Required("connection-env");
        try
        {
            connectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                connectionEnvironmentVariableName);
        }
        catch (ConnectionEnvironmentVariableException exception)
        {
            Fail(
                "Cannot extract schema.",
                "set the named connection environment variable and retry.",
                4,
                [$"  {exception.Message}"]);
            throw new InvalidOperationException(exception.Message, exception);
        }

        var request = new SqlServerExtractRequest
        {
            NewWorkspacePath = output,
            ConnectionString = connectionString,
            SystemName = invocation.Required("system"),
            SchemaName = invocation.Optional("schema") ?? string.Empty,
            AllSchemas = invocation.Flag("all-schemas"),
            TableName = invocation.Optional("table") ?? string.Empty,
            AllTables = invocation.Flag("all-tables")
        };

        try
        {
            using var activity = CliActivityLine.Start("Extracting");
            var result = sqlServerExtractService.Extract(request);
            await workspaces.CreateAsync("output", result.Model).ConfigureAwait(false);
            activity.Succeed();

            presenter.WriteKeyValueBlock("MetaSchema", [
                ("Systems", result.SystemCount.ToString(CultureInfo.InvariantCulture)),
                ("Schemas", result.SchemaCount.ToString(CultureInfo.InvariantCulture)),
                ("Objects", result.SchemaObjectCount.ToString(CultureInfo.InvariantCulture)),
                ("Tables", result.TableCount.ToString(CultureInfo.InvariantCulture)),
                ("Views", result.ViewCount.ToString(CultureInfo.InvariantCulture)),
                ("Fields", result.FieldCount.ToString(CultureInfo.InvariantCulture)),
                ("Keys", result.KeyCount.ToString(CultureInfo.InvariantCulture)),
                ("Relationships", result.TableRelationshipCount.ToString(CultureInfo.InvariantCulture)),
                ("Workspace", output)
            ]);
        }
        catch (InvalidOperationException exception)
        {
            Fail(
                "Cannot extract schema.",
                HelpCommand("extract sqlserver"),
                4,
                [$"  {exception.Message}"]);
        }
        catch (Exception exception) when (exception is not MetaCliExitException)
        {
            Fail(
                "Cannot extract schema.",
                "check SQL Server connectivity and extraction scope, then retry.",
                4,
                [$"  {exception.Message}"]);
        }
    }

    private string HelpCommand(string commandName) => $"{appName} help {commandName}";

    private void Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details.Where(static detail => !string.IsNullOrWhiteSpace(detail)));
        }

        renderedDetails.Add($"Next: {next}");
        presenter.WriteFailure(message, renderedDetails);
        throw new MetaCliExitException(exitCode);
    }
}
