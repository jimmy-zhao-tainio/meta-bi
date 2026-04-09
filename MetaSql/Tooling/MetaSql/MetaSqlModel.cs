using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Meta.Core.Serialization;

namespace MetaSql
{
    [XmlRoot("MetaSql")]
    public sealed partial class MetaSqlModel
    {
        public static MetaSqlModel CreateEmpty() => new();

        [XmlArray("DatabaseList")]
        [XmlArrayItem("Database")]
        public List<Database> DatabaseList { get; set; } = new();
        public bool ShouldSerializeDatabaseList() => DatabaseList.Count > 0;

        [XmlArray("ForeignKeyList")]
        [XmlArrayItem("ForeignKey")]
        public List<ForeignKey> ForeignKeyList { get; set; } = new();
        public bool ShouldSerializeForeignKeyList() => ForeignKeyList.Count > 0;

        [XmlArray("ForeignKeyColumnList")]
        [XmlArrayItem("ForeignKeyColumn")]
        public List<ForeignKeyColumn> ForeignKeyColumnList { get; set; } = new();
        public bool ShouldSerializeForeignKeyColumnList() => ForeignKeyColumnList.Count > 0;

        [XmlArray("IndexList")]
        [XmlArrayItem("Index")]
        public List<Index> IndexList { get; set; } = new();
        public bool ShouldSerializeIndexList() => IndexList.Count > 0;

        [XmlArray("IndexColumnList")]
        [XmlArrayItem("IndexColumn")]
        public List<IndexColumn> IndexColumnList { get; set; } = new();
        public bool ShouldSerializeIndexColumnList() => IndexColumnList.Count > 0;

        [XmlArray("PrimaryKeyList")]
        [XmlArrayItem("PrimaryKey")]
        public List<PrimaryKey> PrimaryKeyList { get; set; } = new();
        public bool ShouldSerializePrimaryKeyList() => PrimaryKeyList.Count > 0;

        [XmlArray("PrimaryKeyColumnList")]
        [XmlArrayItem("PrimaryKeyColumn")]
        public List<PrimaryKeyColumn> PrimaryKeyColumnList { get; set; } = new();
        public bool ShouldSerializePrimaryKeyColumnList() => PrimaryKeyColumnList.Count > 0;

        [XmlArray("SchemaList")]
        [XmlArrayItem("Schema")]
        public List<Schema> SchemaList { get; set; } = new();
        public bool ShouldSerializeSchemaList() => SchemaList.Count > 0;

        [XmlArray("TableList")]
        [XmlArrayItem("Table")]
        public List<Table> TableList { get; set; } = new();
        public bool ShouldSerializeTableList() => TableList.Count > 0;

        [XmlArray("TableColumnList")]
        [XmlArrayItem("TableColumn")]
        public List<TableColumn> TableColumnList { get; set; } = new();
        public bool ShouldSerializeTableColumnList() => TableColumnList.Count > 0;

        [XmlArray("TableColumnDataTypeDetailList")]
        [XmlArrayItem("TableColumnDataTypeDetail")]
        public List<TableColumnDataTypeDetail> TableColumnDataTypeDetailList { get; set; } = new();
        public bool ShouldSerializeTableColumnDataTypeDetailList() => TableColumnDataTypeDetailList.Count > 0;

        public static MetaSqlModel LoadFromXmlWorkspace(
            string workspacePath,
            bool searchUpward = true)
        {
            var model = TypedWorkspaceXmlSerializer.Load<MetaSqlModel>(workspacePath, searchUpward);
            MetaSqlModelFactory.Bind(model);
            return model;
        }

        public static Task<MetaSqlModel> LoadFromXmlWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(LoadFromXmlWorkspace(workspacePath, searchUpward));
        }

        public void SaveToXmlWorkspace(string workspacePath)
        {
            MetaSqlModelFactory.Bind(this);
            TypedWorkspaceXmlSerializer.Save(this, workspacePath, ResolveBundledModelXmlPath());
        }

        public Task SaveToXmlWorkspaceAsync(
            string workspacePath,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SaveToXmlWorkspace(workspacePath);
            return Task.CompletedTask;
        }

        private static string? ResolveBundledModelXmlPath()
        {
            var assemblyDirectory = Path.GetDirectoryName(typeof(MetaSqlModel).Assembly.Location);
            if (string.IsNullOrWhiteSpace(assemblyDirectory))
            {
                return null;
            }

            var namespacedPath = Path.Combine(assemblyDirectory, "MetaSql", "model.xml");
            if (File.Exists(namespacedPath))
            {
                return namespacedPath;
            }

            var directPath = Path.Combine(assemblyDirectory, "model.xml");
            return File.Exists(directPath) ? directPath : null;
        }
    }

    internal static class MetaSqlModelFactory
    {
        internal static void Bind(MetaSqlModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            model.DatabaseList ??= new List<Database>();
            model.ForeignKeyList ??= new List<ForeignKey>();
            model.ForeignKeyColumnList ??= new List<ForeignKeyColumn>();
            model.IndexList ??= new List<Index>();
            model.IndexColumnList ??= new List<IndexColumn>();
            model.PrimaryKeyList ??= new List<PrimaryKey>();
            model.PrimaryKeyColumnList ??= new List<PrimaryKeyColumn>();
            model.SchemaList ??= new List<Schema>();
            model.TableList ??= new List<Table>();
            model.TableColumnList ??= new List<TableColumn>();
            model.TableColumnDataTypeDetailList ??= new List<TableColumnDataTypeDetail>();

            NormalizeDatabaseList(model);
            NormalizeForeignKeyList(model);
            NormalizeForeignKeyColumnList(model);
            NormalizeIndexList(model);
            NormalizeIndexColumnList(model);
            NormalizePrimaryKeyList(model);
            NormalizePrimaryKeyColumnList(model);
            NormalizeSchemaList(model);
            NormalizeTableList(model);
            NormalizeTableColumnList(model);
            NormalizeTableColumnDataTypeDetailList(model);

            var databaseListById = BuildById(model.DatabaseList, row => row.Id, "Database");
            var foreignKeyListById = BuildById(model.ForeignKeyList, row => row.Id, "ForeignKey");
            var foreignKeyColumnListById = BuildById(model.ForeignKeyColumnList, row => row.Id, "ForeignKeyColumn");
            var indexListById = BuildById(model.IndexList, row => row.Id, "Index");
            var indexColumnListById = BuildById(model.IndexColumnList, row => row.Id, "IndexColumn");
            var primaryKeyListById = BuildById(model.PrimaryKeyList, row => row.Id, "PrimaryKey");
            var primaryKeyColumnListById = BuildById(model.PrimaryKeyColumnList, row => row.Id, "PrimaryKeyColumn");
            var schemaListById = BuildById(model.SchemaList, row => row.Id, "Schema");
            var tableListById = BuildById(model.TableList, row => row.Id, "Table");
            var tableColumnListById = BuildById(model.TableColumnList, row => row.Id, "TableColumn");
            var tableColumnDataTypeDetailListById = BuildById(model.TableColumnDataTypeDetailList, row => row.Id, "TableColumnDataTypeDetail");

            foreach (var row in model.ForeignKeyList)
            {
                row.SourceTableId = ResolveRelationshipId(
                    row.SourceTableId,
                    row.SourceTable?.Id,
                    "ForeignKey",
                    row.Id,
                    "SourceTableId");
                row.SourceTable = RequireTarget(
                    tableListById,
                    row.SourceTableId,
                    "ForeignKey",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in model.ForeignKeyList)
            {
                row.TargetTableId = ResolveRelationshipId(
                    row.TargetTableId,
                    row.TargetTable?.Id,
                    "ForeignKey",
                    row.Id,
                    "TargetTableId");
                row.TargetTable = RequireTarget(
                    tableListById,
                    row.TargetTableId,
                    "ForeignKey",
                    row.Id,
                    "TargetTableId");
            }

            foreach (var row in model.ForeignKeyColumnList)
            {
                row.ForeignKeyId = ResolveRelationshipId(
                    row.ForeignKeyId,
                    row.ForeignKey?.Id,
                    "ForeignKeyColumn",
                    row.Id,
                    "ForeignKeyId");
                row.ForeignKey = RequireTarget(
                    foreignKeyListById,
                    row.ForeignKeyId,
                    "ForeignKeyColumn",
                    row.Id,
                    "ForeignKeyId");
            }

            foreach (var row in model.ForeignKeyColumnList)
            {
                row.SourceColumnId = ResolveRelationshipId(
                    row.SourceColumnId,
                    row.SourceColumn?.Id,
                    "ForeignKeyColumn",
                    row.Id,
                    "SourceColumnId");
                row.SourceColumn = RequireTarget(
                    tableColumnListById,
                    row.SourceColumnId,
                    "ForeignKeyColumn",
                    row.Id,
                    "SourceColumnId");
            }

            foreach (var row in model.ForeignKeyColumnList)
            {
                row.TargetColumnId = ResolveRelationshipId(
                    row.TargetColumnId,
                    row.TargetColumn?.Id,
                    "ForeignKeyColumn",
                    row.Id,
                    "TargetColumnId");
                row.TargetColumn = RequireTarget(
                    tableColumnListById,
                    row.TargetColumnId,
                    "ForeignKeyColumn",
                    row.Id,
                    "TargetColumnId");
            }

            foreach (var row in model.IndexList)
            {
                row.TableId = ResolveRelationshipId(
                    row.TableId,
                    row.Table?.Id,
                    "Index",
                    row.Id,
                    "TableId");
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "Index",
                    row.Id,
                    "TableId");
            }

            foreach (var row in model.IndexColumnList)
            {
                row.IndexId = ResolveRelationshipId(
                    row.IndexId,
                    row.Index?.Id,
                    "IndexColumn",
                    row.Id,
                    "IndexId");
                row.Index = RequireTarget(
                    indexListById,
                    row.IndexId,
                    "IndexColumn",
                    row.Id,
                    "IndexId");
            }

            foreach (var row in model.IndexColumnList)
            {
                row.TableColumnId = ResolveRelationshipId(
                    row.TableColumnId,
                    row.TableColumn?.Id,
                    "IndexColumn",
                    row.Id,
                    "TableColumnId");
                row.TableColumn = RequireTarget(
                    tableColumnListById,
                    row.TableColumnId,
                    "IndexColumn",
                    row.Id,
                    "TableColumnId");
            }

            foreach (var row in model.PrimaryKeyList)
            {
                row.TableId = ResolveRelationshipId(
                    row.TableId,
                    row.Table?.Id,
                    "PrimaryKey",
                    row.Id,
                    "TableId");
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "PrimaryKey",
                    row.Id,
                    "TableId");
            }

            foreach (var row in model.PrimaryKeyColumnList)
            {
                row.PrimaryKeyId = ResolveRelationshipId(
                    row.PrimaryKeyId,
                    row.PrimaryKey?.Id,
                    "PrimaryKeyColumn",
                    row.Id,
                    "PrimaryKeyId");
                row.PrimaryKey = RequireTarget(
                    primaryKeyListById,
                    row.PrimaryKeyId,
                    "PrimaryKeyColumn",
                    row.Id,
                    "PrimaryKeyId");
            }

            foreach (var row in model.PrimaryKeyColumnList)
            {
                row.TableColumnId = ResolveRelationshipId(
                    row.TableColumnId,
                    row.TableColumn?.Id,
                    "PrimaryKeyColumn",
                    row.Id,
                    "TableColumnId");
                row.TableColumn = RequireTarget(
                    tableColumnListById,
                    row.TableColumnId,
                    "PrimaryKeyColumn",
                    row.Id,
                    "TableColumnId");
            }

            foreach (var row in model.SchemaList)
            {
                row.DatabaseId = ResolveRelationshipId(
                    row.DatabaseId,
                    row.Database?.Id,
                    "Schema",
                    row.Id,
                    "DatabaseId");
                row.Database = RequireTarget(
                    databaseListById,
                    row.DatabaseId,
                    "Schema",
                    row.Id,
                    "DatabaseId");
            }

            foreach (var row in model.TableList)
            {
                row.SchemaId = ResolveRelationshipId(
                    row.SchemaId,
                    row.Schema?.Id,
                    "Table",
                    row.Id,
                    "SchemaId");
                row.Schema = RequireTarget(
                    schemaListById,
                    row.SchemaId,
                    "Table",
                    row.Id,
                    "SchemaId");
            }

            foreach (var row in model.TableColumnList)
            {
                row.TableId = ResolveRelationshipId(
                    row.TableId,
                    row.Table?.Id,
                    "TableColumn",
                    row.Id,
                    "TableId");
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "TableColumn",
                    row.Id,
                    "TableId");
            }

            foreach (var row in model.TableColumnDataTypeDetailList)
            {
                row.TableColumnId = ResolveRelationshipId(
                    row.TableColumnId,
                    row.TableColumn?.Id,
                    "TableColumnDataTypeDetail",
                    row.Id,
                    "TableColumnId");
                row.TableColumn = RequireTarget(
                    tableColumnListById,
                    row.TableColumnId,
                    "TableColumnDataTypeDetail",
                    row.Id,
                    "TableColumnId");
            }

        }

        private static void NormalizeDatabaseList(MetaSqlModel model)
        {
            foreach (var row in model.DatabaseList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'Database' contains a row with empty Id.");
                row.Collation ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'Database' row '{row.Id}' is missing required property 'Name'.");
            }
        }

        private static void NormalizeForeignKeyList(MetaSqlModel model)
        {
            foreach (var row in model.ForeignKeyList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ForeignKey' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'ForeignKey' row '{row.Id}' is missing required property 'Name'.");
                row.SourceTableId ??= string.Empty;
                row.TargetTableId ??= string.Empty;
            }
        }

        private static void NormalizeForeignKeyColumnList(MetaSqlModel model)
        {
            foreach (var row in model.ForeignKeyColumnList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'ForeignKeyColumn' contains a row with empty Id.");
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'ForeignKeyColumn' row '{row.Id}' is missing required property 'Ordinal'.");
                row.ForeignKeyId ??= string.Empty;
                row.SourceColumnId ??= string.Empty;
                row.TargetColumnId ??= string.Empty;
            }
        }

        private static void NormalizeIndexList(MetaSqlModel model)
        {
            foreach (var row in model.IndexList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'Index' contains a row with empty Id.");
                row.FilterSql ??= string.Empty;
                row.IsClustered ??= string.Empty;
                row.IsUnique ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'Index' row '{row.Id}' is missing required property 'Name'.");
                row.TableId ??= string.Empty;
            }
        }

        private static void NormalizeIndexColumnList(MetaSqlModel model)
        {
            foreach (var row in model.IndexColumnList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'IndexColumn' contains a row with empty Id.");
                row.IsDescending ??= string.Empty;
                row.IsIncluded ??= string.Empty;
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'IndexColumn' row '{row.Id}' is missing required property 'Ordinal'.");
                row.IndexId ??= string.Empty;
                row.TableColumnId ??= string.Empty;
            }
        }

        private static void NormalizePrimaryKeyList(MetaSqlModel model)
        {
            foreach (var row in model.PrimaryKeyList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'PrimaryKey' contains a row with empty Id.");
                row.IsClustered ??= string.Empty;
                row.Name = RequireText(row.Name, $"Entity 'PrimaryKey' row '{row.Id}' is missing required property 'Name'.");
                row.TableId ??= string.Empty;
            }
        }

        private static void NormalizePrimaryKeyColumnList(MetaSqlModel model)
        {
            foreach (var row in model.PrimaryKeyColumnList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'PrimaryKeyColumn' contains a row with empty Id.");
                row.IsDescending ??= string.Empty;
                row.Ordinal = RequireText(row.Ordinal, $"Entity 'PrimaryKeyColumn' row '{row.Id}' is missing required property 'Ordinal'.");
                row.PrimaryKeyId ??= string.Empty;
                row.TableColumnId ??= string.Empty;
            }
        }

        private static void NormalizeSchemaList(MetaSqlModel model)
        {
            foreach (var row in model.SchemaList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'Schema' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'Schema' row '{row.Id}' is missing required property 'Name'.");
                row.DatabaseId ??= string.Empty;
            }
        }

        private static void NormalizeTableList(MetaSqlModel model)
        {
            foreach (var row in model.TableList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'Table' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'Table' row '{row.Id}' is missing required property 'Name'.");
                row.SchemaId ??= string.Empty;
            }
        }

        private static void NormalizeTableColumnList(MetaSqlModel model)
        {
            foreach (var row in model.TableColumnList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TableColumn' contains a row with empty Id.");
                row.ExpressionSql ??= string.Empty;
                row.IdentityIncrement ??= string.Empty;
                row.IdentitySeed ??= string.Empty;
                row.IsIdentity ??= string.Empty;
                row.IsNullable ??= string.Empty;
                row.MetaDataTypeId = RequireText(row.MetaDataTypeId, $"Entity 'TableColumn' row '{row.Id}' is missing required property 'MetaDataTypeId'.");
                row.Name = RequireText(row.Name, $"Entity 'TableColumn' row '{row.Id}' is missing required property 'Name'.");
                row.Ordinal ??= string.Empty;
                row.TableId ??= string.Empty;
            }
        }

        private static void NormalizeTableColumnDataTypeDetailList(MetaSqlModel model)
        {
            foreach (var row in model.TableColumnDataTypeDetailList)
            {
                ArgumentNullException.ThrowIfNull(row);
                row.Id = RequireIdentity(row.Id, "Entity 'TableColumnDataTypeDetail' contains a row with empty Id.");
                row.Name = RequireText(row.Name, $"Entity 'TableColumnDataTypeDetail' row '{row.Id}' is missing required property 'Name'.");
                row.Value = RequireText(row.Value, $"Entity 'TableColumnDataTypeDetail' row '{row.Id}' is missing required property 'Value'.");
                row.TableColumnId ??= string.Empty;
            }
        }

        private static Dictionary<string, T> BuildById<T>(
            IEnumerable<T> rows,
            Func<T, string> getId,
            string entityName)
            where T : class
        {
            var rowsById = new Dictionary<string, T>(StringComparer.Ordinal);
            foreach (var row in rows)
            {
                ArgumentNullException.ThrowIfNull(row);
                var id = RequireIdentity(getId(row), $"Entity '{entityName}' contains a row with empty Id.");
                if (!rowsById.TryAdd(id, row))
                {
                    throw new InvalidOperationException($"Entity '{entityName}' contains duplicate Id '{id}'.");
                }
            }

            return rowsById;
        }

        private static T RequireTarget<T>(
            Dictionary<string, T> rowsById,
            string targetId,
            string sourceEntityName,
            string sourceId,
            string relationshipName)
            where T : class
        {
            var normalizedTargetId = RequireIdentity(targetId, $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' is empty.");
            if (!rowsById.TryGetValue(normalizedTargetId, out var target))
            {
                throw new InvalidOperationException($"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' points to missing Id '{normalizedTargetId}'.");
            }

            return target;
        }

        private static string ResolveRelationshipId(
            string relationshipId,
            string? navigationId,
            string sourceEntityName,
            string sourceId,
            string relationshipName)
        {
            var normalizedRelationshipId = NormalizeIdentity(relationshipId);
            var normalizedNavigationId = NormalizeIdentity(navigationId);
            if (!string.IsNullOrEmpty(normalizedRelationshipId) &&
                !string.IsNullOrEmpty(normalizedNavigationId) &&
                !string.Equals(normalizedRelationshipId, normalizedNavigationId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' conflicts between '{normalizedRelationshipId}' and '{normalizedNavigationId}'.");
            }

            var resolvedTargetId = string.IsNullOrEmpty(normalizedRelationshipId)
                ? normalizedNavigationId
                : normalizedRelationshipId;
            return RequireIdentity(resolvedTargetId, $"Relationship '{sourceEntityName}.{relationshipName}' on row '{sourceEntityName}:{sourceId}' is empty.");
        }

        private static string RequireIdentity(string? value, string errorMessage)
        {
            var normalizedValue = NormalizeIdentity(value);
            if (string.IsNullOrEmpty(normalizedValue))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return normalizedValue;
        }

        private static string RequireText(string? value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return value;
        }

        private static string NormalizeIdentity(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim();
        }
    }
}
