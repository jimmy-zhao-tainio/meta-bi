using Meta.Operations.Domain;

namespace MetaSql;

// Workspace IDs locate records; SQL names within their owning scope identify objects.
// Length-prefixed components preserve names containing punctuation without collisions.
internal sealed class SqlObjectIdentity
{
    private readonly Dictionary<string, string> schemas;
    private readonly Dictionary<string, string> tables;
    private readonly Dictionary<string, string> columns;
    private readonly HashSet<string> tableKeys;
    private readonly HashSet<string> columnKeys;

    public SqlObjectIdentity(InMemoryWorkspace workspace)
    {
        schemas = Records(workspace, "Schema").ToDictionary(row => row.Id, row => Key(row.Values["Name"]), StringComparer.Ordinal);
        tables = Records(workspace, "Table").ToDictionary(row => row.Id,
            row => Key(schemas[row.RelationshipIds["SchemaId"]], row.Values["Name"]), StringComparer.Ordinal);
        columns = Records(workspace, "TableColumn").ToDictionary(row => row.Id,
            row => Key(tables[row.RelationshipIds["TableId"]], row.Values["Name"]), StringComparer.Ordinal);
        EnsureUnique(schemas, "schema");
        EnsureUnique(tables, "table");
        EnsureUnique(columns, "column");
        tableKeys = tables.Values.ToHashSet(StringComparer.Ordinal);
        columnKeys = columns.Values.ToHashSet(StringComparer.Ordinal);
    }

    public string Schema(string id) => schemas[id];
    public string Table(string id) => tables[id];
    public string Column(string id) => columns[id];
    public string TableObject(GenericRecord row, string tableRelationship = "TableId") =>
        Key(Table(row.RelationshipIds[tableRelationship]), row.Values["Name"]);
    public bool ContainsTable(string key) => tableKeys.Contains(key);
    public bool ContainsColumn(string key) => columnKeys.Contains(key);
    public bool ContainsSchema(string key) => schemas.ContainsValue(key);
    public static string Key(params string[] parts) => string.Concat(parts.Select(part => $"{part.Length}:{part}"));
    public static bool SameTable(MetaSql.Table source, MetaSql.Table live) =>
        string.Equals(source.Schema.Name, live.Schema.Name, StringComparison.Ordinal) &&
        string.Equals(source.Name, live.Name, StringComparison.Ordinal);

    private static IEnumerable<GenericRecord> Records(InMemoryWorkspace workspace, string entity) =>
        workspace.Instance.RecordsByEntity.TryGetValue(entity, out var records) ? records : [];

    private static void EnsureUnique(Dictionary<string, string> identities, string kind)
    {
        var duplicate = identities.GroupBy(row => row.Value, StringComparer.Ordinal).FirstOrDefault(group => group.Count() > 1);
        if (duplicate is not null)
            throw new InvalidOperationException($"Ambiguous SQL {kind} identity for records {string.Join(", ", duplicate.Select(row => row.Key))}.");
    }
}
