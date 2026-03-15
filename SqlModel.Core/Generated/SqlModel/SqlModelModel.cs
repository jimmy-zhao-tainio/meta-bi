using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Meta.Core.Domain;

namespace SqlModel
{
    public sealed partial class SqlModelModel
    {
        internal SqlModelModel(
            IReadOnlyList<Database> databaseList,
            IReadOnlyList<ForeignKey> foreignKeyList,
            IReadOnlyList<ForeignKeyColumn> foreignKeyColumnList,
            IReadOnlyList<Index> indexList,
            IReadOnlyList<IndexColumn> indexColumnList,
            IReadOnlyList<PrimaryKey> primaryKeyList,
            IReadOnlyList<PrimaryKeyColumn> primaryKeyColumnList,
            IReadOnlyList<Schema> schemaList,
            IReadOnlyList<Table> tableList,
            IReadOnlyList<TableColumn> tableColumnList
        )
        {
            DatabaseList = databaseList;
            ForeignKeyList = foreignKeyList;
            ForeignKeyColumnList = foreignKeyColumnList;
            IndexList = indexList;
            IndexColumnList = indexColumnList;
            PrimaryKeyList = primaryKeyList;
            PrimaryKeyColumnList = primaryKeyColumnList;
            SchemaList = schemaList;
            TableList = tableList;
            TableColumnList = tableColumnList;
        }

        public IReadOnlyList<Database> DatabaseList { get; }
        public IReadOnlyList<ForeignKey> ForeignKeyList { get; }
        public IReadOnlyList<ForeignKeyColumn> ForeignKeyColumnList { get; }
        public IReadOnlyList<Index> IndexList { get; }
        public IReadOnlyList<IndexColumn> IndexColumnList { get; }
        public IReadOnlyList<PrimaryKey> PrimaryKeyList { get; }
        public IReadOnlyList<PrimaryKeyColumn> PrimaryKeyColumnList { get; }
        public IReadOnlyList<Schema> SchemaList { get; }
        public IReadOnlyList<Table> TableList { get; }
        public IReadOnlyList<TableColumn> TableColumnList { get; }
    }

    internal static class SqlModelModelFactory
    {
        internal static SqlModelModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var databaseList = new List<Database>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Database", out var databaseListRecords))
            {
                foreach (var record in databaseListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    databaseList.Add(new Database
                    {
                        Id = record.Id ?? string.Empty,
                        Collation = record.Values.TryGetValue("Collation", out var collationValue) ? collationValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Platform = record.Values.TryGetValue("Platform", out var platformValue) ? platformValue ?? string.Empty : string.Empty,
                    });
                }
            }

            var foreignKeyList = new List<ForeignKey>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("ForeignKey", out var foreignKeyListRecords))
            {
                foreach (var record in foreignKeyListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    foreignKeyList.Add(new ForeignKey
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                        TargetTableId = record.RelationshipIds.TryGetValue("TargetTableId", out var targetTableRelationshipId) ? targetTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var foreignKeyColumnList = new List<ForeignKeyColumn>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("ForeignKeyColumn", out var foreignKeyColumnListRecords))
            {
                foreach (var record in foreignKeyColumnListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    foreignKeyColumnList.Add(new ForeignKeyColumn
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        ForeignKeyId = record.RelationshipIds.TryGetValue("ForeignKeyId", out var foreignKeyRelationshipId) ? foreignKeyRelationshipId ?? string.Empty : string.Empty,
                        SourceColumnId = record.RelationshipIds.TryGetValue("SourceColumnId", out var sourceColumnRelationshipId) ? sourceColumnRelationshipId ?? string.Empty : string.Empty,
                        TargetColumnId = record.RelationshipIds.TryGetValue("TargetColumnId", out var targetColumnRelationshipId) ? targetColumnRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var indexList = new List<Index>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Index", out var indexListRecords))
            {
                foreach (var record in indexListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    indexList.Add(new Index
                    {
                        Id = record.Id ?? string.Empty,
                        FilterSql = record.Values.TryGetValue("FilterSql", out var filterSqlValue) ? filterSqlValue ?? string.Empty : string.Empty,
                        IsClustered = record.Values.TryGetValue("IsClustered", out var isClusteredValue) ? isClusteredValue ?? string.Empty : string.Empty,
                        IsUnique = record.Values.TryGetValue("IsUnique", out var isUniqueValue) ? isUniqueValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var indexColumnList = new List<IndexColumn>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("IndexColumn", out var indexColumnListRecords))
            {
                foreach (var record in indexColumnListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    indexColumnList.Add(new IndexColumn
                    {
                        Id = record.Id ?? string.Empty,
                        IsDescending = record.Values.TryGetValue("IsDescending", out var isDescendingValue) ? isDescendingValue ?? string.Empty : string.Empty,
                        IsIncluded = record.Values.TryGetValue("IsIncluded", out var isIncludedValue) ? isIncludedValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        IndexId = record.RelationshipIds.TryGetValue("IndexId", out var indexRelationshipId) ? indexRelationshipId ?? string.Empty : string.Empty,
                        TableColumnId = record.RelationshipIds.TryGetValue("TableColumnId", out var tableColumnRelationshipId) ? tableColumnRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var primaryKeyList = new List<PrimaryKey>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("PrimaryKey", out var primaryKeyListRecords))
            {
                foreach (var record in primaryKeyListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    primaryKeyList.Add(new PrimaryKey
                    {
                        Id = record.Id ?? string.Empty,
                        IsClustered = record.Values.TryGetValue("IsClustered", out var isClusteredValue) ? isClusteredValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var primaryKeyColumnList = new List<PrimaryKeyColumn>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("PrimaryKeyColumn", out var primaryKeyColumnListRecords))
            {
                foreach (var record in primaryKeyColumnListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    primaryKeyColumnList.Add(new PrimaryKeyColumn
                    {
                        Id = record.Id ?? string.Empty,
                        IsDescending = record.Values.TryGetValue("IsDescending", out var isDescendingValue) ? isDescendingValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        PrimaryKeyId = record.RelationshipIds.TryGetValue("PrimaryKeyId", out var primaryKeyRelationshipId) ? primaryKeyRelationshipId ?? string.Empty : string.Empty,
                        TableColumnId = record.RelationshipIds.TryGetValue("TableColumnId", out var tableColumnRelationshipId) ? tableColumnRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var schemaList = new List<Schema>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Schema", out var schemaListRecords))
            {
                foreach (var record in schemaListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    schemaList.Add(new Schema
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        DatabaseId = record.RelationshipIds.TryGetValue("DatabaseId", out var databaseRelationshipId) ? databaseRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableList = new List<Table>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Table", out var tableListRecords))
            {
                foreach (var record in tableListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableList.Add(new Table
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SchemaId = record.RelationshipIds.TryGetValue("SchemaId", out var schemaRelationshipId) ? schemaRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableColumnList = new List<TableColumn>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("TableColumn", out var tableColumnListRecords))
            {
                foreach (var record in tableColumnListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableColumnList.Add(new TableColumn
                    {
                        Id = record.Id ?? string.Empty,
                        ExpressionSql = record.Values.TryGetValue("ExpressionSql", out var expressionSqlValue) ? expressionSqlValue ?? string.Empty : string.Empty,
                        IdentityIncrement = record.Values.TryGetValue("IdentityIncrement", out var identityIncrementValue) ? identityIncrementValue ?? string.Empty : string.Empty,
                        IdentitySeed = record.Values.TryGetValue("IdentitySeed", out var identitySeedValue) ? identitySeedValue ?? string.Empty : string.Empty,
                        IsIdentity = record.Values.TryGetValue("IsIdentity", out var isIdentityValue) ? isIdentityValue ?? string.Empty : string.Empty,
                        IsNullable = record.Values.TryGetValue("IsNullable", out var isNullableValue) ? isNullableValue ?? string.Empty : string.Empty,
                        MetaDataTypeId = record.Values.TryGetValue("MetaDataTypeId", out var metaDataTypeIdValue) ? metaDataTypeIdValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var databaseListById = new Dictionary<string, Database>(global::System.StringComparer.Ordinal);
            foreach (var row in databaseList)
            {
                databaseListById[row.Id] = row;
            }

            var foreignKeyListById = new Dictionary<string, ForeignKey>(global::System.StringComparer.Ordinal);
            foreach (var row in foreignKeyList)
            {
                foreignKeyListById[row.Id] = row;
            }

            var foreignKeyColumnListById = new Dictionary<string, ForeignKeyColumn>(global::System.StringComparer.Ordinal);
            foreach (var row in foreignKeyColumnList)
            {
                foreignKeyColumnListById[row.Id] = row;
            }

            var indexListById = new Dictionary<string, Index>(global::System.StringComparer.Ordinal);
            foreach (var row in indexList)
            {
                indexListById[row.Id] = row;
            }

            var indexColumnListById = new Dictionary<string, IndexColumn>(global::System.StringComparer.Ordinal);
            foreach (var row in indexColumnList)
            {
                indexColumnListById[row.Id] = row;
            }

            var primaryKeyListById = new Dictionary<string, PrimaryKey>(global::System.StringComparer.Ordinal);
            foreach (var row in primaryKeyList)
            {
                primaryKeyListById[row.Id] = row;
            }

            var primaryKeyColumnListById = new Dictionary<string, PrimaryKeyColumn>(global::System.StringComparer.Ordinal);
            foreach (var row in primaryKeyColumnList)
            {
                primaryKeyColumnListById[row.Id] = row;
            }

            var schemaListById = new Dictionary<string, Schema>(global::System.StringComparer.Ordinal);
            foreach (var row in schemaList)
            {
                schemaListById[row.Id] = row;
            }

            var tableListById = new Dictionary<string, Table>(global::System.StringComparer.Ordinal);
            foreach (var row in tableList)
            {
                tableListById[row.Id] = row;
            }

            var tableColumnListById = new Dictionary<string, TableColumn>(global::System.StringComparer.Ordinal);
            foreach (var row in tableColumnList)
            {
                tableColumnListById[row.Id] = row;
            }

            foreach (var row in foreignKeyList)
            {
                row.SourceTable = RequireTarget(
                    tableListById,
                    row.SourceTableId,
                    "ForeignKey",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in foreignKeyList)
            {
                row.TargetTable = RequireTarget(
                    tableListById,
                    row.TargetTableId,
                    "ForeignKey",
                    row.Id,
                    "TargetTableId");
            }

            foreach (var row in foreignKeyColumnList)
            {
                row.ForeignKey = RequireTarget(
                    foreignKeyListById,
                    row.ForeignKeyId,
                    "ForeignKeyColumn",
                    row.Id,
                    "ForeignKeyId");
            }

            foreach (var row in foreignKeyColumnList)
            {
                row.SourceColumn = RequireTarget(
                    tableColumnListById,
                    row.SourceColumnId,
                    "ForeignKeyColumn",
                    row.Id,
                    "SourceColumnId");
            }

            foreach (var row in foreignKeyColumnList)
            {
                row.TargetColumn = RequireTarget(
                    tableColumnListById,
                    row.TargetColumnId,
                    "ForeignKeyColumn",
                    row.Id,
                    "TargetColumnId");
            }

            foreach (var row in indexList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "Index",
                    row.Id,
                    "TableId");
            }

            foreach (var row in indexColumnList)
            {
                row.Index = RequireTarget(
                    indexListById,
                    row.IndexId,
                    "IndexColumn",
                    row.Id,
                    "IndexId");
            }

            foreach (var row in indexColumnList)
            {
                row.TableColumn = RequireTarget(
                    tableColumnListById,
                    row.TableColumnId,
                    "IndexColumn",
                    row.Id,
                    "TableColumnId");
            }

            foreach (var row in primaryKeyList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "PrimaryKey",
                    row.Id,
                    "TableId");
            }

            foreach (var row in primaryKeyColumnList)
            {
                row.PrimaryKey = RequireTarget(
                    primaryKeyListById,
                    row.PrimaryKeyId,
                    "PrimaryKeyColumn",
                    row.Id,
                    "PrimaryKeyId");
            }

            foreach (var row in primaryKeyColumnList)
            {
                row.TableColumn = RequireTarget(
                    tableColumnListById,
                    row.TableColumnId,
                    "PrimaryKeyColumn",
                    row.Id,
                    "TableColumnId");
            }

            foreach (var row in schemaList)
            {
                row.Database = RequireTarget(
                    databaseListById,
                    row.DatabaseId,
                    "Schema",
                    row.Id,
                    "DatabaseId");
            }

            foreach (var row in tableList)
            {
                row.Schema = RequireTarget(
                    schemaListById,
                    row.SchemaId,
                    "Table",
                    row.Id,
                    "SchemaId");
            }

            foreach (var row in tableColumnList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "TableColumn",
                    row.Id,
                    "TableId");
            }

            return new SqlModelModel(
                new ReadOnlyCollection<Database>(databaseList),
                new ReadOnlyCollection<ForeignKey>(foreignKeyList),
                new ReadOnlyCollection<ForeignKeyColumn>(foreignKeyColumnList),
                new ReadOnlyCollection<Index>(indexList),
                new ReadOnlyCollection<IndexColumn>(indexColumnList),
                new ReadOnlyCollection<PrimaryKey>(primaryKeyList),
                new ReadOnlyCollection<PrimaryKeyColumn>(primaryKeyColumnList),
                new ReadOnlyCollection<Schema>(schemaList),
                new ReadOnlyCollection<Table>(tableList),
                new ReadOnlyCollection<TableColumn>(tableColumnList)
            );
        }

        private static T RequireTarget<T>(
            Dictionary<string, T> rowsById,
            string targetId,
            string sourceEntityName,
            string sourceId,
            string relationshipName)
            where T : class
        {
            if (string.IsNullOrEmpty(targetId))
            {
                throw new global::System.InvalidOperationException(
                    $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' is empty."
                );
            }

            if (!rowsById.TryGetValue(targetId, out var target))
            {
                throw new global::System.InvalidOperationException(
                    $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' points to missing Id '{targetId}'."
                );
            }

            return target;
        }
    }
}
