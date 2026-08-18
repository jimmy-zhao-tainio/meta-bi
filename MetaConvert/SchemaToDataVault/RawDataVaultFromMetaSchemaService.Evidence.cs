using MetaWeaveScript.Execution;

namespace MetaConvert.SchemaToDataVault;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    private static SchemaToRawDataVaultEvidence ReadEvidence(
        IReadOnlyDictionary<string, MetaWeaveScriptQueryOutput> relationOutputs)
        => new(
            ReadRelation(
                relationOutputs,
                "IncludedTables",
                row => new IncludedTableEvidence(
                    row.RequiredString("TableId"),
                    row.RequiredString("TableName"),
                    row.RequiredString("SchemaId"),
                    row.RequiredString("SchemaName"),
                    row.RequiredString("SystemId"))),
            ReadRelation(
                relationOutputs,
                "IncludedRelationships",
                row => new IncludedRelationshipEvidence(
                    row.RequiredString("RelationshipId"),
                    row.RequiredString("SourceTableId"),
                    row.RequiredString("SourceTableName"),
                    row.RequiredString("TargetTableId"),
                    row.RequiredString("TargetTableName"),
                    row.RequiredString("StructuralName"))),
            ReadRelation(
                relationOutputs,
                "SelectedKeys",
                row => new SelectedKeyEvidence(
                    row.RequiredString("KeyId"),
                    row.OptionalString("KeyName"),
                    row.RequiredString("TableId"),
                    row.RequiredInt32("KeyPriority"))),
            ReadRelation(
                relationOutputs,
                "SelectedKeyFields",
                row => new SelectedKeyFieldEvidence(
                    row.RequiredString("TableId"),
                    row.RequiredString("KeyId"),
                    row.RequiredString("KeyFieldId"),
                    row.RequiredString("FieldName"),
                    row.RequiredInt32("KeyFieldNumber"))));

    private static IReadOnlyList<T> ReadRelation<T>(
        IReadOnlyDictionary<string, MetaWeaveScriptQueryOutput> relationOutputs,
        string relationName,
        Func<RelationEvidenceRow, T> create)
    {
        if (!relationOutputs.TryGetValue(relationName, out var output))
        {
            throw new InvalidOperationException(
                $"The sanctioned MetaSchema-to-Raw-Data-Vault weave did not return relation '{relationName}'.");
        }

        var columns = output.Columns
            .Select((column, index) => (column.Name, Index: index))
            .ToDictionary(column => column.Name, column => column.Index, StringComparer.OrdinalIgnoreCase);
        return output.Rows
            .Select(row => create(new RelationEvidenceRow(relationName, columns, row)))
            .ToList();
    }

    private sealed class RelationEvidenceRow
    {
        private readonly string relationName;
        private readonly IReadOnlyDictionary<string, int> columns;
        private readonly MetaWeaveScriptQueryRow row;

        public RelationEvidenceRow(
            string relationName,
            IReadOnlyDictionary<string, int> columns,
            MetaWeaveScriptQueryRow row)
        {
            this.relationName = relationName;
            this.columns = columns;
            this.row = row;
        }

        public string RequiredString(string columnName)
        {
            var value = Value(columnName);
            if (value.Kind != MetaWeaveScriptValueKind.String)
            {
                throw InvalidValue(columnName, "a non-null string");
            }

            return value.StringValue!;
        }

        public string? OptionalString(string columnName)
        {
            var value = Value(columnName);
            if (value.IsNull)
            {
                return null;
            }
            if (value.Kind != MetaWeaveScriptValueKind.String)
            {
                throw InvalidValue(columnName, "a string or NULL");
            }

            return value.StringValue;
        }

        public int RequiredInt32(string columnName)
        {
            var value = Value(columnName);
            if (value.Kind != MetaWeaveScriptValueKind.Integer ||
                value.IntegerValue is < int.MinValue or > int.MaxValue)
            {
                throw InvalidValue(columnName, "a 32-bit integer");
            }

            return (int)value.IntegerValue;
        }

        private MetaWeaveScriptValue Value(string columnName)
        {
            if (!columns.TryGetValue(columnName, out var index))
            {
                throw new InvalidOperationException(
                    $"Relation '{relationName}' did not return required evidence column '{columnName}'.");
            }
            if (index >= row.Values.Count)
            {
                throw InvalidValue(columnName, "a value");
            }

            return row.Values[index];
        }

        private InvalidOperationException InvalidValue(string columnName, string expected)
            => new(
                $"Relation '{relationName}' returned '{columnName}' without {expected}.");
    }

    private sealed record SchemaToRawDataVaultEvidence(
        IReadOnlyList<IncludedTableEvidence> IncludedTables,
        IReadOnlyList<IncludedRelationshipEvidence> IncludedRelationships,
        IReadOnlyList<SelectedKeyEvidence> SelectedKeys,
        IReadOnlyList<SelectedKeyFieldEvidence> SelectedKeyFields);

    private sealed record IncludedTableEvidence(
        string TableId,
        string TableName,
        string SchemaId,
        string SchemaName,
        string SystemId);

    private sealed record IncludedRelationshipEvidence(
        string RelationshipId,
        string SourceTableId,
        string SourceTableName,
        string TargetTableId,
        string TargetTableName,
        string StructuralName);

    private sealed record SelectedKeyEvidence(
        string KeyId,
        string? KeyName,
        string TableId,
        int KeyPriority);

    private sealed record SelectedKeyFieldEvidence(
        string TableId,
        string KeyId,
        string KeyFieldId,
        string FieldName,
        int KeyFieldNumber);
}
