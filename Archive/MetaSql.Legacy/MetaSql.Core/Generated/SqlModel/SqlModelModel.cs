using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Meta.Core.Domain;

namespace SqlModel
{
    public sealed partial class SqlModelModel
    {
        internal SqlModelModel(
            IReadOnlyList<CheckConstraint> checkConstraintList,
            IReadOnlyList<Database> databaseList,
            IReadOnlyList<DefaultConstraint> defaultConstraintList,
            IReadOnlyList<ForeignKey> foreignKeyList,
            IReadOnlyList<ForeignKeyColumn> foreignKeyColumnList,
            IReadOnlyList<Function> functionList,
            IReadOnlyList<FunctionParameter> functionParameterList,
            IReadOnlyList<Index> indexList,
            IReadOnlyList<IndexColumn> indexColumnList,
            IReadOnlyList<PrimaryKey> primaryKeyList,
            IReadOnlyList<PrimaryKeyColumn> primaryKeyColumnList,
            IReadOnlyList<Schema> schemaList,
            IReadOnlyList<Sequence> sequenceList,
            IReadOnlyList<StoredProcedure> storedProcedureList,
            IReadOnlyList<StoredProcedureParameter> storedProcedureParameterList,
            IReadOnlyList<Synonym> synonymList,
            IReadOnlyList<Table> tableList,
            IReadOnlyList<TableColumn> tableColumnList,
            IReadOnlyList<Trigger> triggerList,
            IReadOnlyList<UniqueConstraint> uniqueConstraintList,
            IReadOnlyList<UniqueConstraintColumn> uniqueConstraintColumnList,
            IReadOnlyList<UserDefinedType> userDefinedTypeList,
            IReadOnlyList<View> viewList,
            IReadOnlyList<ViewColumn> viewColumnList
        )
        {
            CheckConstraintList = checkConstraintList;
            DatabaseList = databaseList;
            DefaultConstraintList = defaultConstraintList;
            ForeignKeyList = foreignKeyList;
            ForeignKeyColumnList = foreignKeyColumnList;
            FunctionList = functionList;
            FunctionParameterList = functionParameterList;
            IndexList = indexList;
            IndexColumnList = indexColumnList;
            PrimaryKeyList = primaryKeyList;
            PrimaryKeyColumnList = primaryKeyColumnList;
            SchemaList = schemaList;
            SequenceList = sequenceList;
            StoredProcedureList = storedProcedureList;
            StoredProcedureParameterList = storedProcedureParameterList;
            SynonymList = synonymList;
            TableList = tableList;
            TableColumnList = tableColumnList;
            TriggerList = triggerList;
            UniqueConstraintList = uniqueConstraintList;
            UniqueConstraintColumnList = uniqueConstraintColumnList;
            UserDefinedTypeList = userDefinedTypeList;
            ViewList = viewList;
            ViewColumnList = viewColumnList;
        }

        public IReadOnlyList<CheckConstraint> CheckConstraintList { get; }
        public IReadOnlyList<Database> DatabaseList { get; }
        public IReadOnlyList<DefaultConstraint> DefaultConstraintList { get; }
        public IReadOnlyList<ForeignKey> ForeignKeyList { get; }
        public IReadOnlyList<ForeignKeyColumn> ForeignKeyColumnList { get; }
        public IReadOnlyList<Function> FunctionList { get; }
        public IReadOnlyList<FunctionParameter> FunctionParameterList { get; }
        public IReadOnlyList<Index> IndexList { get; }
        public IReadOnlyList<IndexColumn> IndexColumnList { get; }
        public IReadOnlyList<PrimaryKey> PrimaryKeyList { get; }
        public IReadOnlyList<PrimaryKeyColumn> PrimaryKeyColumnList { get; }
        public IReadOnlyList<Schema> SchemaList { get; }
        public IReadOnlyList<Sequence> SequenceList { get; }
        public IReadOnlyList<StoredProcedure> StoredProcedureList { get; }
        public IReadOnlyList<StoredProcedureParameter> StoredProcedureParameterList { get; }
        public IReadOnlyList<Synonym> SynonymList { get; }
        public IReadOnlyList<Table> TableList { get; }
        public IReadOnlyList<TableColumn> TableColumnList { get; }
        public IReadOnlyList<Trigger> TriggerList { get; }
        public IReadOnlyList<UniqueConstraint> UniqueConstraintList { get; }
        public IReadOnlyList<UniqueConstraintColumn> UniqueConstraintColumnList { get; }
        public IReadOnlyList<UserDefinedType> UserDefinedTypeList { get; }
        public IReadOnlyList<View> ViewList { get; }
        public IReadOnlyList<ViewColumn> ViewColumnList { get; }
    }

    internal static class SqlModelModelFactory
    {
        internal static SqlModelModel CreateFromWorkspace(Workspace workspace)
        {
            if (workspace == null)
            {
                throw new global::System.ArgumentNullException(nameof(workspace));
            }

            var checkConstraintList = new List<CheckConstraint>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("CheckConstraint", out var checkConstraintListRecords))
            {
                foreach (var record in checkConstraintListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    checkConstraintList.Add(new CheckConstraint
                    {
                        Id = record.Id ?? string.Empty,
                        ExpressionSql = record.Values.TryGetValue("ExpressionSql", out var expressionSqlValue) ? expressionSqlValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
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

            var defaultConstraintList = new List<DefaultConstraint>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("DefaultConstraint", out var defaultConstraintListRecords))
            {
                foreach (var record in defaultConstraintListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    defaultConstraintList.Add(new DefaultConstraint
                    {
                        Id = record.Id ?? string.Empty,
                        ExpressionSql = record.Values.TryGetValue("ExpressionSql", out var expressionSqlValue) ? expressionSqlValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TableColumnId = record.RelationshipIds.TryGetValue("TableColumnId", out var tableColumnRelationshipId) ? tableColumnRelationshipId ?? string.Empty : string.Empty,
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
                        OnDeleteAction = record.Values.TryGetValue("OnDeleteAction", out var onDeleteActionValue) ? onDeleteActionValue ?? string.Empty : string.Empty,
                        OnUpdateAction = record.Values.TryGetValue("OnUpdateAction", out var onUpdateActionValue) ? onUpdateActionValue ?? string.Empty : string.Empty,
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

            var functionList = new List<Function>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Function", out var functionListRecords))
            {
                foreach (var record in functionListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    functionList.Add(new Function
                    {
                        Id = record.Id ?? string.Empty,
                        DefinitionSql = record.Values.TryGetValue("DefinitionSql", out var definitionSqlValue) ? definitionSqlValue ?? string.Empty : string.Empty,
                        FunctionKind = record.Values.TryGetValue("FunctionKind", out var functionKindValue) ? functionKindValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        ReturnLength = record.Values.TryGetValue("ReturnLength", out var returnLengthValue) ? returnLengthValue ?? string.Empty : string.Empty,
                        ReturnPrecision = record.Values.TryGetValue("ReturnPrecision", out var returnPrecisionValue) ? returnPrecisionValue ?? string.Empty : string.Empty,
                        ReturnScale = record.Values.TryGetValue("ReturnScale", out var returnScaleValue) ? returnScaleValue ?? string.Empty : string.Empty,
                        ReturnTypeName = record.Values.TryGetValue("ReturnTypeName", out var returnTypeNameValue) ? returnTypeNameValue ?? string.Empty : string.Empty,
                        SchemaId = record.RelationshipIds.TryGetValue("SchemaId", out var schemaRelationshipId) ? schemaRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var functionParameterList = new List<FunctionParameter>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("FunctionParameter", out var functionParameterListRecords))
            {
                foreach (var record in functionParameterListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    functionParameterList.Add(new FunctionParameter
                    {
                        Id = record.Id ?? string.Empty,
                        IsNullable = record.Values.TryGetValue("IsNullable", out var isNullableValue) ? isNullableValue ?? string.Empty : string.Empty,
                        Length = record.Values.TryGetValue("Length", out var lengthValue) ? lengthValue ?? string.Empty : string.Empty,
                        Mode = record.Values.TryGetValue("Mode", out var modeValue) ? modeValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        Precision = record.Values.TryGetValue("Precision", out var precisionValue) ? precisionValue ?? string.Empty : string.Empty,
                        Scale = record.Values.TryGetValue("Scale", out var scaleValue) ? scaleValue ?? string.Empty : string.Empty,
                        TypeName = record.Values.TryGetValue("TypeName", out var typeNameValue) ? typeNameValue ?? string.Empty : string.Empty,
                        FunctionId = record.RelationshipIds.TryGetValue("FunctionId", out var functionRelationshipId) ? functionRelationshipId ?? string.Empty : string.Empty,
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

            var sequenceList = new List<Sequence>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Sequence", out var sequenceListRecords))
            {
                foreach (var record in sequenceListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    sequenceList.Add(new Sequence
                    {
                        Id = record.Id ?? string.Empty,
                        IncrementValue = record.Values.TryGetValue("IncrementValue", out var incrementValueValue) ? incrementValueValue ?? string.Empty : string.Empty,
                        IsCycling = record.Values.TryGetValue("IsCycling", out var isCyclingValue) ? isCyclingValue ?? string.Empty : string.Empty,
                        MaximumValue = record.Values.TryGetValue("MaximumValue", out var maximumValueValue) ? maximumValueValue ?? string.Empty : string.Empty,
                        MinimumValue = record.Values.TryGetValue("MinimumValue", out var minimumValueValue) ? minimumValueValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        StartValue = record.Values.TryGetValue("StartValue", out var startValueValue) ? startValueValue ?? string.Empty : string.Empty,
                        TypeName = record.Values.TryGetValue("TypeName", out var typeNameValue) ? typeNameValue ?? string.Empty : string.Empty,
                        SchemaId = record.RelationshipIds.TryGetValue("SchemaId", out var schemaRelationshipId) ? schemaRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var storedProcedureList = new List<StoredProcedure>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("StoredProcedure", out var storedProcedureListRecords))
            {
                foreach (var record in storedProcedureListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    storedProcedureList.Add(new StoredProcedure
                    {
                        Id = record.Id ?? string.Empty,
                        DefinitionSql = record.Values.TryGetValue("DefinitionSql", out var definitionSqlValue) ? definitionSqlValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SchemaId = record.RelationshipIds.TryGetValue("SchemaId", out var schemaRelationshipId) ? schemaRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var storedProcedureParameterList = new List<StoredProcedureParameter>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("StoredProcedureParameter", out var storedProcedureParameterListRecords))
            {
                foreach (var record in storedProcedureParameterListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    storedProcedureParameterList.Add(new StoredProcedureParameter
                    {
                        Id = record.Id ?? string.Empty,
                        IsNullable = record.Values.TryGetValue("IsNullable", out var isNullableValue) ? isNullableValue ?? string.Empty : string.Empty,
                        Length = record.Values.TryGetValue("Length", out var lengthValue) ? lengthValue ?? string.Empty : string.Empty,
                        Mode = record.Values.TryGetValue("Mode", out var modeValue) ? modeValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        Precision = record.Values.TryGetValue("Precision", out var precisionValue) ? precisionValue ?? string.Empty : string.Empty,
                        Scale = record.Values.TryGetValue("Scale", out var scaleValue) ? scaleValue ?? string.Empty : string.Empty,
                        TypeName = record.Values.TryGetValue("TypeName", out var typeNameValue) ? typeNameValue ?? string.Empty : string.Empty,
                        StoredProcedureId = record.RelationshipIds.TryGetValue("StoredProcedureId", out var storedProcedureRelationshipId) ? storedProcedureRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var synonymList = new List<Synonym>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Synonym", out var synonymListRecords))
            {
                foreach (var record in synonymListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    synonymList.Add(new Synonym
                    {
                        Id = record.Id ?? string.Empty,
                        BaseObjectName = record.Values.TryGetValue("BaseObjectName", out var baseObjectNameValue) ? baseObjectNameValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SchemaId = record.RelationshipIds.TryGetValue("SchemaId", out var schemaRelationshipId) ? schemaRelationshipId ?? string.Empty : string.Empty,
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
                        Collation = record.Values.TryGetValue("Collation", out var collationValue) ? collationValue ?? string.Empty : string.Empty,
                        ExpressionSql = record.Values.TryGetValue("ExpressionSql", out var expressionSqlValue) ? expressionSqlValue ?? string.Empty : string.Empty,
                        IdentityIncrement = record.Values.TryGetValue("IdentityIncrement", out var identityIncrementValue) ? identityIncrementValue ?? string.Empty : string.Empty,
                        IdentitySeed = record.Values.TryGetValue("IdentitySeed", out var identitySeedValue) ? identitySeedValue ?? string.Empty : string.Empty,
                        IsIdentity = record.Values.TryGetValue("IsIdentity", out var isIdentityValue) ? isIdentityValue ?? string.Empty : string.Empty,
                        IsNullable = record.Values.TryGetValue("IsNullable", out var isNullableValue) ? isNullableValue ?? string.Empty : string.Empty,
                        Length = record.Values.TryGetValue("Length", out var lengthValue) ? lengthValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        Precision = record.Values.TryGetValue("Precision", out var precisionValue) ? precisionValue ?? string.Empty : string.Empty,
                        Scale = record.Values.TryGetValue("Scale", out var scaleValue) ? scaleValue ?? string.Empty : string.Empty,
                        TypeName = record.Values.TryGetValue("TypeName", out var typeNameValue) ? typeNameValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var triggerList = new List<Trigger>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("Trigger", out var triggerListRecords))
            {
                foreach (var record in triggerListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    triggerList.Add(new Trigger
                    {
                        Id = record.Id ?? string.Empty,
                        DefinitionSql = record.Values.TryGetValue("DefinitionSql", out var definitionSqlValue) ? definitionSqlValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TriggerEvents = record.Values.TryGetValue("TriggerEvents", out var triggerEventsValue) ? triggerEventsValue ?? string.Empty : string.Empty,
                        TriggerTiming = record.Values.TryGetValue("TriggerTiming", out var triggerTimingValue) ? triggerTimingValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var uniqueConstraintList = new List<UniqueConstraint>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("UniqueConstraint", out var uniqueConstraintListRecords))
            {
                foreach (var record in uniqueConstraintListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    uniqueConstraintList.Add(new UniqueConstraint
                    {
                        Id = record.Id ?? string.Empty,
                        IsClustered = record.Values.TryGetValue("IsClustered", out var isClusteredValue) ? isClusteredValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        TableId = record.RelationshipIds.TryGetValue("TableId", out var tableRelationshipId) ? tableRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var uniqueConstraintColumnList = new List<UniqueConstraintColumn>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("UniqueConstraintColumn", out var uniqueConstraintColumnListRecords))
            {
                foreach (var record in uniqueConstraintColumnListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    uniqueConstraintColumnList.Add(new UniqueConstraintColumn
                    {
                        Id = record.Id ?? string.Empty,
                        IsDescending = record.Values.TryGetValue("IsDescending", out var isDescendingValue) ? isDescendingValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        TableColumnId = record.RelationshipIds.TryGetValue("TableColumnId", out var tableColumnRelationshipId) ? tableColumnRelationshipId ?? string.Empty : string.Empty,
                        UniqueConstraintId = record.RelationshipIds.TryGetValue("UniqueConstraintId", out var uniqueConstraintRelationshipId) ? uniqueConstraintRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var userDefinedTypeList = new List<UserDefinedType>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("UserDefinedType", out var userDefinedTypeListRecords))
            {
                foreach (var record in userDefinedTypeListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    userDefinedTypeList.Add(new UserDefinedType
                    {
                        Id = record.Id ?? string.Empty,
                        BaseTypeName = record.Values.TryGetValue("BaseTypeName", out var baseTypeNameValue) ? baseTypeNameValue ?? string.Empty : string.Empty,
                        Length = record.Values.TryGetValue("Length", out var lengthValue) ? lengthValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Precision = record.Values.TryGetValue("Precision", out var precisionValue) ? precisionValue ?? string.Empty : string.Empty,
                        Scale = record.Values.TryGetValue("Scale", out var scaleValue) ? scaleValue ?? string.Empty : string.Empty,
                        SchemaId = record.RelationshipIds.TryGetValue("SchemaId", out var schemaRelationshipId) ? schemaRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var viewList = new List<View>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("View", out var viewListRecords))
            {
                foreach (var record in viewListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    viewList.Add(new View
                    {
                        Id = record.Id ?? string.Empty,
                        DefinitionSql = record.Values.TryGetValue("DefinitionSql", out var definitionSqlValue) ? definitionSqlValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        SchemaId = record.RelationshipIds.TryGetValue("SchemaId", out var schemaRelationshipId) ? schemaRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var viewColumnList = new List<ViewColumn>();
            if (workspace.Instance.RecordsByEntity.TryGetValue("ViewColumn", out var viewColumnListRecords))
            {
                foreach (var record in viewColumnListRecords.OrderBy(item => item.Id, global::System.StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Id, global::System.StringComparer.Ordinal))
                {
                    viewColumnList.Add(new ViewColumn
                    {
                        Id = record.Id ?? string.Empty,
                        IsNullable = record.Values.TryGetValue("IsNullable", out var isNullableValue) ? isNullableValue ?? string.Empty : string.Empty,
                        Length = record.Values.TryGetValue("Length", out var lengthValue) ? lengthValue ?? string.Empty : string.Empty,
                        Name = record.Values.TryGetValue("Name", out var nameValue) ? nameValue ?? string.Empty : string.Empty,
                        Ordinal = record.Values.TryGetValue("Ordinal", out var ordinalValue) ? ordinalValue ?? string.Empty : string.Empty,
                        Precision = record.Values.TryGetValue("Precision", out var precisionValue) ? precisionValue ?? string.Empty : string.Empty,
                        Scale = record.Values.TryGetValue("Scale", out var scaleValue) ? scaleValue ?? string.Empty : string.Empty,
                        TypeName = record.Values.TryGetValue("TypeName", out var typeNameValue) ? typeNameValue ?? string.Empty : string.Empty,
                        ViewId = record.RelationshipIds.TryGetValue("ViewId", out var viewRelationshipId) ? viewRelationshipId ?? string.Empty : string.Empty,
                    });
                }
            }

            var checkConstraintListById = new Dictionary<string, CheckConstraint>(global::System.StringComparer.Ordinal);
            foreach (var row in checkConstraintList)
            {
                checkConstraintListById[row.Id] = row;
            }

            var databaseListById = new Dictionary<string, Database>(global::System.StringComparer.Ordinal);
            foreach (var row in databaseList)
            {
                databaseListById[row.Id] = row;
            }

            var defaultConstraintListById = new Dictionary<string, DefaultConstraint>(global::System.StringComparer.Ordinal);
            foreach (var row in defaultConstraintList)
            {
                defaultConstraintListById[row.Id] = row;
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

            var functionListById = new Dictionary<string, Function>(global::System.StringComparer.Ordinal);
            foreach (var row in functionList)
            {
                functionListById[row.Id] = row;
            }

            var functionParameterListById = new Dictionary<string, FunctionParameter>(global::System.StringComparer.Ordinal);
            foreach (var row in functionParameterList)
            {
                functionParameterListById[row.Id] = row;
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

            var sequenceListById = new Dictionary<string, Sequence>(global::System.StringComparer.Ordinal);
            foreach (var row in sequenceList)
            {
                sequenceListById[row.Id] = row;
            }

            var storedProcedureListById = new Dictionary<string, StoredProcedure>(global::System.StringComparer.Ordinal);
            foreach (var row in storedProcedureList)
            {
                storedProcedureListById[row.Id] = row;
            }

            var storedProcedureParameterListById = new Dictionary<string, StoredProcedureParameter>(global::System.StringComparer.Ordinal);
            foreach (var row in storedProcedureParameterList)
            {
                storedProcedureParameterListById[row.Id] = row;
            }

            var synonymListById = new Dictionary<string, Synonym>(global::System.StringComparer.Ordinal);
            foreach (var row in synonymList)
            {
                synonymListById[row.Id] = row;
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

            var triggerListById = new Dictionary<string, Trigger>(global::System.StringComparer.Ordinal);
            foreach (var row in triggerList)
            {
                triggerListById[row.Id] = row;
            }

            var uniqueConstraintListById = new Dictionary<string, UniqueConstraint>(global::System.StringComparer.Ordinal);
            foreach (var row in uniqueConstraintList)
            {
                uniqueConstraintListById[row.Id] = row;
            }

            var uniqueConstraintColumnListById = new Dictionary<string, UniqueConstraintColumn>(global::System.StringComparer.Ordinal);
            foreach (var row in uniqueConstraintColumnList)
            {
                uniqueConstraintColumnListById[row.Id] = row;
            }

            var userDefinedTypeListById = new Dictionary<string, UserDefinedType>(global::System.StringComparer.Ordinal);
            foreach (var row in userDefinedTypeList)
            {
                userDefinedTypeListById[row.Id] = row;
            }

            var viewListById = new Dictionary<string, View>(global::System.StringComparer.Ordinal);
            foreach (var row in viewList)
            {
                viewListById[row.Id] = row;
            }

            var viewColumnListById = new Dictionary<string, ViewColumn>(global::System.StringComparer.Ordinal);
            foreach (var row in viewColumnList)
            {
                viewColumnListById[row.Id] = row;
            }

            foreach (var row in checkConstraintList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "CheckConstraint",
                    row.Id,
                    "TableId");
            }

            foreach (var row in defaultConstraintList)
            {
                row.TableColumn = RequireTarget(
                    tableColumnListById,
                    row.TableColumnId,
                    "DefaultConstraint",
                    row.Id,
                    "TableColumnId");
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

            foreach (var row in functionList)
            {
                row.Schema = RequireTarget(
                    schemaListById,
                    row.SchemaId,
                    "Function",
                    row.Id,
                    "SchemaId");
            }

            foreach (var row in functionParameterList)
            {
                row.Function = RequireTarget(
                    functionListById,
                    row.FunctionId,
                    "FunctionParameter",
                    row.Id,
                    "FunctionId");
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

            foreach (var row in sequenceList)
            {
                row.Schema = RequireTarget(
                    schemaListById,
                    row.SchemaId,
                    "Sequence",
                    row.Id,
                    "SchemaId");
            }

            foreach (var row in storedProcedureList)
            {
                row.Schema = RequireTarget(
                    schemaListById,
                    row.SchemaId,
                    "StoredProcedure",
                    row.Id,
                    "SchemaId");
            }

            foreach (var row in storedProcedureParameterList)
            {
                row.StoredProcedure = RequireTarget(
                    storedProcedureListById,
                    row.StoredProcedureId,
                    "StoredProcedureParameter",
                    row.Id,
                    "StoredProcedureId");
            }

            foreach (var row in synonymList)
            {
                row.Schema = RequireTarget(
                    schemaListById,
                    row.SchemaId,
                    "Synonym",
                    row.Id,
                    "SchemaId");
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

            foreach (var row in triggerList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "Trigger",
                    row.Id,
                    "TableId");
            }

            foreach (var row in uniqueConstraintList)
            {
                row.Table = RequireTarget(
                    tableListById,
                    row.TableId,
                    "UniqueConstraint",
                    row.Id,
                    "TableId");
            }

            foreach (var row in uniqueConstraintColumnList)
            {
                row.TableColumn = RequireTarget(
                    tableColumnListById,
                    row.TableColumnId,
                    "UniqueConstraintColumn",
                    row.Id,
                    "TableColumnId");
            }

            foreach (var row in uniqueConstraintColumnList)
            {
                row.UniqueConstraint = RequireTarget(
                    uniqueConstraintListById,
                    row.UniqueConstraintId,
                    "UniqueConstraintColumn",
                    row.Id,
                    "UniqueConstraintId");
            }

            foreach (var row in userDefinedTypeList)
            {
                row.Schema = RequireTarget(
                    schemaListById,
                    row.SchemaId,
                    "UserDefinedType",
                    row.Id,
                    "SchemaId");
            }

            foreach (var row in viewList)
            {
                row.Schema = RequireTarget(
                    schemaListById,
                    row.SchemaId,
                    "View",
                    row.Id,
                    "SchemaId");
            }

            foreach (var row in viewColumnList)
            {
                row.View = RequireTarget(
                    viewListById,
                    row.ViewId,
                    "ViewColumn",
                    row.Id,
                    "ViewId");
            }

            return new SqlModelModel(
                new ReadOnlyCollection<CheckConstraint>(checkConstraintList),
                new ReadOnlyCollection<Database>(databaseList),
                new ReadOnlyCollection<DefaultConstraint>(defaultConstraintList),
                new ReadOnlyCollection<ForeignKey>(foreignKeyList),
                new ReadOnlyCollection<ForeignKeyColumn>(foreignKeyColumnList),
                new ReadOnlyCollection<Function>(functionList),
                new ReadOnlyCollection<FunctionParameter>(functionParameterList),
                new ReadOnlyCollection<Index>(indexList),
                new ReadOnlyCollection<IndexColumn>(indexColumnList),
                new ReadOnlyCollection<PrimaryKey>(primaryKeyList),
                new ReadOnlyCollection<PrimaryKeyColumn>(primaryKeyColumnList),
                new ReadOnlyCollection<Schema>(schemaList),
                new ReadOnlyCollection<Sequence>(sequenceList),
                new ReadOnlyCollection<StoredProcedure>(storedProcedureList),
                new ReadOnlyCollection<StoredProcedureParameter>(storedProcedureParameterList),
                new ReadOnlyCollection<Synonym>(synonymList),
                new ReadOnlyCollection<Table>(tableList),
                new ReadOnlyCollection<TableColumn>(tableColumnList),
                new ReadOnlyCollection<Trigger>(triggerList),
                new ReadOnlyCollection<UniqueConstraint>(uniqueConstraintList),
                new ReadOnlyCollection<UniqueConstraintColumn>(uniqueConstraintColumnList),
                new ReadOnlyCollection<UserDefinedType>(userDefinedTypeList),
                new ReadOnlyCollection<View>(viewList),
                new ReadOnlyCollection<ViewColumn>(viewColumnList)
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
