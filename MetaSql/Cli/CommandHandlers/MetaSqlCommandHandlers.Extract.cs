using Meta.Core.Connections;
using Meta.Core.Domain;
using Meta.Core.Operations;
using Meta.Core.Services;
using MetaCli.Core;
using Meta.Core.Presentation.Cli;
using MetaSql.Extractors.SqlServer;

internal sealed partial class MetaSqlCommandHandlers
{
    public Task<int> RunExtractSqlServerAsync(MetaCliInvocation invocation)
    {
        var request = BuildSqlServerExtractRequest(invocation);
        var targetValidation = CliNewWorkspaceTargetValidator.Validate(request.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Task.FromResult(Fail(targetValidation.ErrorMessage, "choose a new folder or empty the target directory and retry.", 4, targetValidation.Details));
        }

        var connectionEnvironmentVariableName = invocation.Required("connection-env");
        try
        {
            request.ConnectionString = ConnectionEnvironmentVariableResolver.ResolveRequired(
                connectionEnvironmentVariableName);
        }
        catch (ConnectionEnvironmentVariableException exception)
        {
            return Task.FromResult(Fail(
                "Cannot extract SQL database.",
                "set the named connection environment variable and retry.",
                4,
                [$"  {exception.Message}"]));
        }

        request.NewWorkspacePath = targetValidation.FullPath;
        InMemoryWorkspace workspace;
        try
        {
            workspace = new SqlServerMetaSqlExtractor().ExtractMetaSqlWorkspace(request);
        }
        catch (InvalidOperationException exception)
        {
            return Task.FromResult(Fail(
                "Cannot extract SQL database.",
                HelpCommand("extract sqlserver"),
                4,
                [$"  {exception.Message}"]));
        }

        var validation = WorkspaceValidator.Validate(
            workspace.Model,
            workspace.Instance);
        if (validation.HasErrors)
        {
            return Task.FromResult(Fail(
                "extracted MetaSql workspace is invalid.",
                "fix extractor mapping and retry extract.",
                4,
                validation.Issues
                    .Where(item => item.Severity == IssueSeverity.Error)
                    .Select(item => $"  - {item.Code}: {item.Message}")));
        }

        Presenter.WriteInfo($"Extracted {Path.GetFileName(targetValidation.FullPath)}");
        Presenter.WriteKeyValueBlock("Summary", new[]
        {
            ("Workspace", targetValidation.FullPath),
            ("Schemas", CountRecords(workspace, "Schema").ToString()),
            ("Tables", CountRecords(workspace, "Table").ToString()),
            ("Views", CountRecords(workspace, "View").ToString()),
            ("Functions", CountRecords(workspace, "Function").ToString()),
            ("StoredProcedures", CountRecords(workspace, "StoredProcedure").ToString()),
        });
        return Task.FromResult(0);
    }

    private SqlServerExtractRequest BuildSqlServerExtractRequest(MetaCliInvocation invocation)
    {
        var request = new SqlServerExtractRequest
        {
            NewWorkspacePath = invocation.Required("new-workspace"),
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

    private static int CountRecords(InMemoryWorkspace workspace, string entityName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName).Count;
    }
}
