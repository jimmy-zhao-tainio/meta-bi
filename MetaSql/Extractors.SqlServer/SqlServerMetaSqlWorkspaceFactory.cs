using Meta.Core.Domain;
using Meta.Core.Services;

namespace MetaSql.Extractors.SqlServer;

public static class SqlServerMetaSqlWorkspaceFactory
{
    public static Workspace CreateEmptyWorkspace(
        string newWorkspacePath,
        string databaseName,
        string? schemaName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var model = MetaSqlModel.CreateEmpty();
        var database = new Database
        {
            Id = databaseName,
            Name = databaseName,
            Platform = "sqlserver",
        };
        model.DatabaseList.Add(database);

        if (!string.IsNullOrWhiteSpace(schemaName))
        {
            model.SchemaList.Add(new Schema
            {
                Id = $"{database.Id}.{schemaName}",
                Name = schemaName,
                DatabaseId = database.Id,
                Database = database,
            });
        }

        model.SaveToXmlWorkspace(newWorkspacePath);
        return new WorkspaceService().LoadAsync(newWorkspacePath, searchUpward: false).GetAwaiter().GetResult();
    }
}
