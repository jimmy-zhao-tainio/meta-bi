namespace MetaSql.Core;

public sealed record DesiredSqlModel(IReadOnlyList<DesiredTable> Tables);

public sealed record DesiredTable(
    string SchemaName,
    string TableName,
    string CreateTableSql,
    IReadOnlyList<DesiredColumn> Columns,
    IReadOnlyList<DesiredConstraint> InlineConstraints,
    IReadOnlyList<DesiredConstraint> AlterConstraints,
    IReadOnlyList<DesiredIndex> Indexes)
{
    public string ObjectKey => SqlObjectName.Format(SchemaName, TableName);
}

public sealed record DesiredColumn(
    string Name,
    string DefinitionSql,
    string TypeSql,
    bool IsNullable);

public sealed record DesiredConstraint(
    string Name,
    string ConstraintKind,
    string Sql,
    string? ReferencedSchemaName = null,
    string? ReferencedTableName = null);

public sealed record DesiredIndex(
    string Name,
    string Sql);
