using Meta.Core.Domain;
using Meta.Core.Serialization;

namespace MetaSql.Extractors.SqlServer;

public static class SqlServerMetaSqlWorkspaceFactory
{
    public const string DefaultSchemaName = "dbo";

    public static InMemoryWorkspace CreateEmptyWorkspace(
        string databaseName,
        IEnumerable<string>? schemaNames = null)
    {
        var model = CreateEmptyModel(databaseName, schemaNames);
        return TypedWorkspaceModelMapper.ToInMemoryWorkspace(model);
    }

    public static MetaSqlModel CreateEmptyModel(
        string databaseName,
        IEnumerable<string>? schemaNames = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var model = MetaSqlModel.CreateEmpty();
        var database = new Database
        {
            Id = databaseName,
            Name = databaseName,
        };
        model.DatabaseList.Add(database);

        var normalizedSchemaNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            DefaultSchemaName,
        };
        foreach (var schemaName in schemaNames ?? Array.Empty<string>())
        {
            if (string.IsNullOrWhiteSpace(schemaName))
            {
                continue;
            }

            normalizedSchemaNames.Add(schemaName.Trim());
        }

        foreach (var schemaName in normalizedSchemaNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
        {
            model.SchemaList.Add(new Schema
            {
                Id = $"{database.Id}.{schemaName}",
                Name = schemaName,
                Database = database,
            });
        }

        return model;
    }
}
