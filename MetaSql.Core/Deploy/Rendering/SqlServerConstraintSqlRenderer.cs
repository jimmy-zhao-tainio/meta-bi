namespace MetaSql;

/// <summary>
/// Renders SQL Server PK/FK/Index add/drop statements.
/// </summary>
internal sealed class SqlServerConstraintSqlRenderer
{
    public string BuildDropForeignKeySql(ForeignKey foreignKey)
    {
        return $"ALTER TABLE {SqlServerRenderingSupport.FormatTableName(foreignKey.SourceTable)} DROP CONSTRAINT {SqlServerRenderingSupport.EscapeSqlIdentifier(foreignKey.Name)};";
    }

    public string BuildDropIndexSql(Index index)
    {
        return $"DROP INDEX {SqlServerRenderingSupport.EscapeSqlIdentifier(index.Name)} ON {SqlServerRenderingSupport.FormatTableName(index.Table)};";
    }

    public string BuildDropPrimaryKeySql(PrimaryKey primaryKey)
    {
        return $"ALTER TABLE {SqlServerRenderingSupport.FormatTableName(primaryKey.Table)} DROP CONSTRAINT {SqlServerRenderingSupport.EscapeSqlIdentifier(primaryKey.Name)};";
    }

    public string BuildAddPrimaryKeySql(PrimaryKey primaryKey, IReadOnlyList<PrimaryKeyColumn> members)
    {
        var clusterClause = SqlServerRenderingSupport.IsTrue(primaryKey.IsClustered)
            ? " CLUSTERED"
            : " NONCLUSTERED";
        var memberList = string.Join(
            ", ",
            members.Select(row =>
            {
                var direction = SqlServerRenderingSupport.IsTrue(row.IsDescending) ? " DESC" : string.Empty;
                return $"{SqlServerRenderingSupport.EscapeSqlIdentifier(row.TableColumn.Name)}{direction}";
            }));
        return $"ALTER TABLE {SqlServerRenderingSupport.FormatTableName(primaryKey.Table)} ADD CONSTRAINT {SqlServerRenderingSupport.EscapeSqlIdentifier(primaryKey.Name)} PRIMARY KEY{clusterClause} ({memberList});";
    }

    public string BuildAddForeignKeySql(ForeignKey foreignKey, IReadOnlyList<ForeignKeyColumn> members)
    {
        var sourceColumns = string.Join(", ", members.Select(row => SqlServerRenderingSupport.EscapeSqlIdentifier(row.SourceColumn.Name)));
        var targetColumns = string.Join(", ", members.Select(row => SqlServerRenderingSupport.EscapeSqlIdentifier(row.TargetColumn.Name)));
        return $"ALTER TABLE {SqlServerRenderingSupport.FormatTableName(foreignKey.SourceTable)} ADD CONSTRAINT {SqlServerRenderingSupport.EscapeSqlIdentifier(foreignKey.Name)} FOREIGN KEY ({sourceColumns}) REFERENCES {SqlServerRenderingSupport.FormatTableName(foreignKey.TargetTable)} ({targetColumns});";
    }

    public string BuildAddIndexSql(Index index, IReadOnlyList<IndexColumn> members)
    {
        var keyMembers = members
            .Where(row => !SqlServerRenderingSupport.IsTrue(row.IsIncluded))
            .ToList();
        if (keyMembers.Count == 0)
        {
            throw new InvalidOperationException($"Cannot add index '{index.Id}' because it has no key members.");
        }

        var includeMembers = members
            .Where(row => SqlServerRenderingSupport.IsTrue(row.IsIncluded))
            .ToList();

        var uniqueClause = SqlServerRenderingSupport.IsTrue(index.IsUnique) ? "UNIQUE " : string.Empty;
        var clusterClause = SqlServerRenderingSupport.IsTrue(index.IsClustered)
            ? "CLUSTERED "
            : SqlServerRenderingSupport.IsFalse(index.IsClustered)
                ? "NONCLUSTERED "
                : string.Empty;
        var keys = string.Join(
            ", ",
            keyMembers.Select(row =>
            {
                var direction = SqlServerRenderingSupport.IsTrue(row.IsDescending) ? " DESC" : string.Empty;
                return $"{SqlServerRenderingSupport.EscapeSqlIdentifier(row.TableColumn.Name)}{direction}";
            }));
        var include = includeMembers.Count == 0
            ? string.Empty
            : $" INCLUDE ({string.Join(", ", includeMembers.Select(row => SqlServerRenderingSupport.EscapeSqlIdentifier(row.TableColumn.Name)))})";
        var filter = string.IsNullOrWhiteSpace(index.FilterSql)
            ? string.Empty
            : $" WHERE {index.FilterSql}";
        return $"CREATE {uniqueClause}{clusterClause}INDEX {SqlServerRenderingSupport.EscapeSqlIdentifier(index.Name)} ON {SqlServerRenderingSupport.FormatTableName(index.Table)} ({keys}){include}{filter};";
    }
}
