using Meta.Core.Connections;
using Meta.Core.Operations;
using Meta.Core.Services;
using MetaCli.Core;
using MetaSql;
using MetaSql.Extractors.SqlServer;

internal sealed partial class MetaSqlCommandHandlers
{
    public async Task RunExtractSqlServerAsync(
        MetaCliInvocation invocation,
        MetaCliWorkspaces workspaces)
    {
        var output = MetaCliWorkspace.OutputLocation(
            invocation,
            "output-xml",
            "output-csharp",
            "output-sql");
        var request = BuildSqlServerExtractRequest(invocation);

        var connectionEnvironmentVariableName = invocation.Required("connection-env");
        try
        {
            request.ConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                connectionEnvironmentVariableName);
        }
        catch (ConnectionEnvironmentVariableException exception)
        {
            throw new MetaCliExitException(Fail(
                "Cannot extract SQL database.",
                "set the named connection environment variable and retry.",
                4,
                [$"  {exception.Message}"]));
        }

        MetaSqlModel model;
        try
        {
            model = new SqlServerMetaSqlExtractor().ExtractMetaSqlModel(request);
        }
        catch (InvalidOperationException exception)
        {
            throw new MetaCliExitException(Fail(
                "Cannot extract SQL database.",
                HelpCommand("extract sqlserver"),
                4,
                [$"  {exception.Message}"]));
        }

        await workspaces.CreateAsync("output", model).ConfigureAwait(false);

        Presenter.WriteInfo($"Extracted {Path.GetFileName(output)}");
        Presenter.WriteKeyValueBlock("Summary", new[]
        {
            ("Workspace", output),
            ("Schemas", model.SchemaList.Count.ToString()),
            ("Tables", model.TableList.Count.ToString()),
            ("Views", model.ViewList.Count.ToString()),
            ("Functions", model.FunctionList.Count.ToString()),
            ("StoredProcedures", model.StoredProcedureList.Count.ToString()),
        });
    }

    private SqlServerExtractRequest BuildSqlServerExtractRequest(MetaCliInvocation invocation)
    {
        var request = new SqlServerExtractRequest
        {
            SchemaName = invocation.Optional("schema"),
            TableName = invocation.Optional("table"),
            AllowEmpty = invocation.Flag("allow-empty"),
        };

        var includeSwitchProvided = invocation.Flag("include-tables") ||
                                    invocation.Flag("include-views") ||
                                    invocation.Flag("include-functions") ||
                                    invocation.Flag("include-stored-procedures");
        if (includeSwitchProvided)
        {
            request.ObjectKinds = SqlServerExtractObjectKinds.None;
            if (invocation.Flag("include-tables")) request.ObjectKinds |= SqlServerExtractObjectKinds.Tables;
            if (invocation.Flag("include-views")) request.ObjectKinds |= SqlServerExtractObjectKinds.Views;
            if (invocation.Flag("include-functions")) request.ObjectKinds |= SqlServerExtractObjectKinds.Functions;
            if (invocation.Flag("include-stored-procedures")) request.ObjectKinds |= SqlServerExtractObjectKinds.StoredProcedures;
        }

        if (!string.IsNullOrWhiteSpace(request.TableName))
        {
            if (!includeSwitchProvided)
            {
                request.ObjectKinds = SqlServerExtractObjectKinds.Tables;
            }
            else if (request.ObjectKinds != SqlServerExtractObjectKinds.Tables)
            {
                throw new MetaCliExitException(Fail("--table can only be used with --include-tables.", HelpCommand("extract sqlserver")));
            }
        }

        return request;
    }
}
