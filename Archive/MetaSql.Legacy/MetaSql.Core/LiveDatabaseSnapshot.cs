namespace MetaSql.Core;

public sealed record LiveDatabaseSnapshot(IReadOnlyDictionary<string, LiveTable> Tables);

public sealed record LiveTable(
    string SchemaName,
    string TableName,
    long RowCount,
    IReadOnlyDictionary<string, LiveColumn> Columns,
    IReadOnlySet<string> ConstraintNames,
    IReadOnlySet<string> IndexNames)
{
    public string ObjectKey => SqlObjectName.Format(SchemaName, TableName);
}

public sealed record LiveColumn(
    string Name,
    string TypeSql,
    bool IsNullable);
