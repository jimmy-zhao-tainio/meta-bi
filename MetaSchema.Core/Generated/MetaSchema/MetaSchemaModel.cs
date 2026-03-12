using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Meta.Core.Domain;

namespace MetaSchema
{
    public sealed partial class MetaSchemaModel
    {
        internal MetaSchemaModel(
            IReadOnlyList<Field> fieldList,
            IReadOnlyList<FieldDataTypeDetail> fieldDataTypeDetailList,
            IReadOnlyList<Schema> schemaList,
            IReadOnlyList<System> systemList,
            IReadOnlyList<Table> tableList,
            IReadOnlyList<TableKey> tableKeyList,
            IReadOnlyList<TableKeyField> tableKeyFieldList,
            IReadOnlyList<TableRelationship> tableRelationshipList,
            IReadOnlyList<TableRelationshipField> tableRelationshipFieldList
        )
        {
            FieldList = fieldList;
            FieldDataTypeDetailList = fieldDataTypeDetailList;
            SchemaList = schemaList;
            SystemList = systemList;
            TableList = tableList;
            TableKeyList = tableKeyList;
            TableKeyFieldList = tableKeyFieldList;
            TableRelationshipList = tableRelationshipList;
            TableRelationshipFieldList = tableRelationshipFieldList;
        }

        public IReadOnlyList<Field> FieldList { get; }
        public IReadOnlyList<FieldDataTypeDetail> FieldDataTypeDetailList { get; }
        public IReadOnlyList<Schema> SchemaList { get; }
        public IReadOnlyList<System> SystemList { get; }
        public IReadOnlyList<Table> TableList { get; }
        public IReadOnlyList<TableKey> TableKeyList { get; }
        public IReadOnlyList<TableKeyField> TableKeyFieldList { get; }
        public IReadOnlyList<TableRelationship> TableRelationshipList { get; }
        public IReadOnlyList<TableRelationshipField> TableRelationshipFieldList { get; }
    }

    internal static class MetaSchemaModelFactory
    {
        internal static MetaSchemaModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var fieldList = new List<Field>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Field", out var fieldListRecords))
            {
                foreach (var record in fieldListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    fieldList.Add(new Field
                    {
                        Id = record.Id ?? string.Empty,
                        DataTypeId = record.Values.TryGetValue("DataTypeId", out var dataTypeIdValue) ? dataTypeIdValue ?? string.Empty : string.Empty,
                        IsNullable = record.Values.TryGetValue("IsNullable", out var isNullableValue) ? isNullableValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var fieldDataTypeDetailList = new List<FieldDataTypeDetail>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("FieldDataTypeDetail", out var fieldDataTypeDetailListRecords))
            {
                foreach (var record in fieldDataTypeDetailListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    fieldDataTypeDetailList.Add(new FieldDataTypeDetail
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Value = record.Values.TryGetValue("Value", out var valueValue) ? valueValue ?? string.Empty : string.Empty,
                        FieldId = record.RelationshipIds.TryGetValue("FieldId", out var fieldRelationshipId) ? fieldRelationshipId ?? string.Empty : string.Empty,
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
                        SystemId = record.RelationshipIds.TryGetValue("SystemId", out var systemRelationshipId) ? systemRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var systemList = new List<System>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("System", out var systemListRecords))
            {
                foreach (var record in systemListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    systemList.Add(new System
                    {
                        Id = record.Id ?? string.Empty,
                        Description = record.Values.TryGetValue("Description", out var descriptionValue) ? descriptionValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
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
                        ObjectType = record.Values.TryGetValue("ObjectType", out var objectTypeValue) ? objectTypeValue ?? string.Empty : string.Empty,
                        SchemaId = record.RelationshipIds.TryGetValue("SchemaId", out var schemaRelationshipId) ? schemaRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableKeyList = new List<TableKey>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("TableKey", out var tableKeyListRecords))
            {
                foreach (var record in tableKeyListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableKeyList.Add(new TableKey
                    {
                        Id = record.Id ?? string.Empty,
                        KeyType = record.Values.TryGetValue("KeyType", out var keyTypeValue) ? keyTypeValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableKeyFieldList = new List<TableKeyField>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("TableKeyField", out var tableKeyFieldListRecords))
            {
                foreach (var record in tableKeyFieldListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableKeyFieldList.Add(new TableKeyField
                    {
                        Id = record.Id ?? string.Empty,
                        FieldName = record.Values.TryGetValue("FieldName", out var fieldNameValue) ? fieldNameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        FieldId = record.RelationshipIds.TryGetValue("FieldId", out var fieldRelationshipId) ? fieldRelationshipId ?? string.Empty : string.Empty,
                        TableKeyId = record.RelationshipIds.TryGetValue("TableKeyId", out var tableKeyRelationshipId) ? tableKeyRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableRelationshipList = new List<TableRelationship>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("TableRelationship", out var tableRelationshipListRecords))
            {
                foreach (var record in tableRelationshipListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableRelationshipList.Add(new TableRelationship
                    {
                        Id = record.Id ?? string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SourceTableId = record.RelationshipIds.TryGetValue("SourceTableId", out var sourceTableRelationshipId) ? sourceTableRelationshipId ?? string.Empty : string.Empty,
                        TargetTableId = record.RelationshipIds.TryGetValue("TargetTableId", out var targetTableRelationshipId) ? targetTableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var tableRelationshipFieldList = new List<TableRelationshipField>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("TableRelationshipField", out var tableRelationshipFieldListRecords))
            {
                foreach (var record in tableRelationshipFieldListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    tableRelationshipFieldList.Add(new TableRelationshipField
                    {
                        Id = record.Id ?? string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        SourceFieldId = record.RelationshipIds.TryGetValue("SourceFieldId", out var sourceFieldRelationshipId) ? sourceFieldRelationshipId ?? string.Empty : string.Empty,
                        TableRelationshipId = record.RelationshipIds.TryGetValue("TableRelationshipId", out var tableRelationshipRelationshipId) ? tableRelationshipRelationshipId ?? string.Empty : string.Empty,
                        TargetFieldId = record.RelationshipIds.TryGetValue("TargetFieldId", out var targetFieldRelationshipId) ? targetFieldRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var fieldListById = new Dictionary<string, Field>(global::System.StringComparer.Ordinal);
            foreach (var row in fieldList)
            {
                fieldListById[row.Id] = row;
            }

            var fieldDataTypeDetailListById = new Dictionary<string, FieldDataTypeDetail>(global::System.StringComparer.Ordinal);
            foreach (var row in fieldDataTypeDetailList)
            {
                fieldDataTypeDetailListById[row.Id] = row;
            }

            var schemaListById = new Dictionary<string, Schema>(global::System.StringComparer.Ordinal);
            foreach (var row in schemaList)
            {
                schemaListById[row.Id] = row;
            }

            var systemListById = new Dictionary<string, System>(global::System.StringComparer.Ordinal);
            foreach (var row in systemList)
            {
                systemListById[row.Id] = row;
            }

            var tableListById = new Dictionary<string, Table>(global::System.StringComparer.Ordinal);
            foreach (var row in tableList)
            {
                tableListById[row.Id] = row;
            }

            var tableKeyListById = new Dictionary<string, TableKey>(global::System.StringComparer.Ordinal);
            foreach (var row in tableKeyList)
            {
                tableKeyListById[row.Id] = row;
            }

            var tableKeyFieldListById = new Dictionary<string, TableKeyField>(global::System.StringComparer.Ordinal);
            foreach (var row in tableKeyFieldList)
            {
                tableKeyFieldListById[row.Id] = row;
            }

            var tableRelationshipListById = new Dictionary<string, TableRelationship>(global::System.StringComparer.Ordinal);
            foreach (var row in tableRelationshipList)
            {
                tableRelationshipListById[row.Id] = row;
            }

            var tableRelationshipFieldListById = new Dictionary<string, TableRelationshipField>(global::System.StringComparer.Ordinal);
            foreach (var row in tableRelationshipFieldList)
            {
                tableRelationshipFieldListById[row.Id] = row;
            }

            foreach (var row in fieldList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "Field",
                    row.Id,
                    "TableId");
            }

            foreach (var row in fieldDataTypeDetailList)
            {
                row.Field = RequireTarget(
                    fieldListById,
                    row.FieldId,
                    "FieldDataTypeDetail",
                    row.Id,
                    "FieldId");
            }

            foreach (var row in schemaList)
            {
                row.System = RequireTarget(
                    systemListById,
                    row.SystemId,
                    "Schema",
                    row.Id,
                    "SystemId");
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

            foreach (var row in tableKeyList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "TableKey",
                    row.Id,
                    "TableId");
            }

            foreach (var row in tableKeyFieldList)
            {
                row.Field = RequireTarget(
                    fieldListById,
                    row.FieldId,
                    "TableKeyField",
                    row.Id,
                    "FieldId");
            }

            foreach (var row in tableKeyFieldList)
            {
                row.TableKey = RequireTarget(
                    tableKeyListById,
                    row.TableKeyId,
                    "TableKeyField",
                    row.Id,
                    "TableKeyId");
            }

            foreach (var row in tableRelationshipList)
            {
                row.SourceTable = RequireTarget(
                    tableListById,
                    row.SourceTableId,
                    "TableRelationship",
                    row.Id,
                    "SourceTableId");
            }

            foreach (var row in tableRelationshipList)
            {
                row.TargetTable = RequireTarget(
                    tableListById,
                    row.TargetTableId,
                    "TableRelationship",
                    row.Id,
                    "TargetTableId");
            }

            foreach (var row in tableRelationshipFieldList)
            {
                row.SourceField = RequireTarget(
                    fieldListById,
                    row.SourceFieldId,
                    "TableRelationshipField",
                    row.Id,
                    "SourceFieldId");
            }

            foreach (var row in tableRelationshipFieldList)
            {
                row.TableRelationship = RequireTarget(
                    tableRelationshipListById,
                    row.TableRelationshipId,
                    "TableRelationshipField",
                    row.Id,
                    "TableRelationshipId");
            }

            foreach (var row in tableRelationshipFieldList)
            {
                row.TargetField = RequireTarget(
                    fieldListById,
                    row.TargetFieldId,
                    "TableRelationshipField",
                    row.Id,
                    "TargetFieldId");
            }

            return new MetaSchemaModel(
                new ReadOnlyCollection<Field>(fieldList),
                new ReadOnlyCollection<FieldDataTypeDetail>(fieldDataTypeDetailList),
                new ReadOnlyCollection<Schema>(schemaList),
                new ReadOnlyCollection<System>(systemList),
                new ReadOnlyCollection<Table>(tableList),
                new ReadOnlyCollection<TableKey>(tableKeyList),
                new ReadOnlyCollection<TableKeyField>(tableKeyFieldList),
                new ReadOnlyCollection<TableRelationship>(tableRelationshipList),
                new ReadOnlyCollection<TableRelationshipField>(tableRelationshipFieldList)
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
