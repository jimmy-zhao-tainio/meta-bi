namespace MetaSql;

/// <summary>
/// Renders SQL Server schema create statements from explicit manifest actions.
/// </summary>
internal sealed class SqlServerSchemaSqlRenderer
{
    public string BuildAddSchemaSql(Schema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);
        return $"CREATE SCHEMA {SqlServerRenderingSupport.EscapeSqlIdentifier(schema.Name)};";
    }
}
