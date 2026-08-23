using MetaWeaveScript.Execution;

namespace MetaConvert.SchemaToDataVault;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    private static SchemaToRawDataVaultEvidence ReadEvidence(
        IReadOnlyDictionary<string, MetaWeaveScriptQueryOutput> relationOutputs)
    {
        var includedTables = ReadRelation(
            relationOutputs,
            "IncludedTables",
            row => new IncludedTableEvidence(
                row.RequiredString("TableId"),
                row.RequiredString("TableName"),
                row.RequiredString("SchemaId"),
                row.RequiredString("SchemaName"),
                row.RequiredString("SystemId")));
        var includedRelationships = ReadRelation(
            relationOutputs,
            "IncludedRelationships",
            row => new IncludedRelationshipEvidence(
                row.RequiredString("RelationshipId"),
                row.RequiredString("SourceTableId"),
                row.RequiredString("SourceTableName"),
                row.RequiredString("TargetTableId"),
                row.RequiredString("TargetTableName"),
                row.RequiredString("StructuralName")));
        var selectedKeys = ReadRelation(
            relationOutputs,
            "SelectedKeys",
            row => new SelectedKeyEvidence(
                row.RequiredString("KeyId"),
                row.OptionalString("KeyName"),
                row.RequiredString("TableId"),
                row.RequiredInt32("KeyPriority")));
        var selectedKeyFields = ReadRelation(
            relationOutputs,
            "SelectedKeyFields",
            row => new SelectedKeyFieldEvidence(
                row.RequiredString("TableId"),
                row.RequiredString("KeyId"),
                row.RequiredString("KeyFieldId"),
                row.RequiredString("FieldName"),
                row.RequiredInt32("KeyFieldNumber")));
        var keyAssessments = ReadRelation(
            relationOutputs,
            "KeyAssessments",
            row => new KeyAssessmentEvidence(
                row.RequiredString("TableId"),
                ParseKeyAssessment(row.RequiredString("KeyAssessment"))));

        ValidateEvidence(includedTables, selectedKeys, selectedKeyFields, keyAssessments);
        return new SchemaToRawDataVaultEvidence(
            includedTables,
            includedRelationships,
            selectedKeys,
            selectedKeyFields,
            keyAssessments);
    }

    private static void ValidateEvidence(
        IReadOnlyList<IncludedTableEvidence> includedTables,
        IReadOnlyList<SelectedKeyEvidence> selectedKeys,
        IReadOnlyList<SelectedKeyFieldEvidence> selectedKeyFields,
        IReadOnlyList<KeyAssessmentEvidence> keyAssessments)
    {
        var selectedKeysByTable = new Dictionary<string, SelectedKeyEvidence>(StringComparer.Ordinal);
        foreach (var selectedKey in selectedKeys)
        {
            if (!selectedKeysByTable.TryAdd(selectedKey.TableId, selectedKey))
            {
                throw new InvalidOperationException(
                    $"Relation 'SelectedKeys' returned more than one selected key for table '{selectedKey.TableId}'.");
            }
        }

        foreach (var field in selectedKeyFields)
        {
            if (!selectedKeysByTable.TryGetValue(field.TableId, out var selectedKey))
            {
                throw new InvalidOperationException(
                    $"Relation 'SelectedKeyFields' returned orphan field '{field.KeyFieldId}' for table '{field.TableId}' without a corresponding 'SelectedKeys' row.");
            }
            if (!string.Equals(field.KeyId, selectedKey.KeyId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Relation 'SelectedKeyFields' returned field '{field.KeyFieldId}' with key '{field.KeyId}' for table '{field.TableId}', but 'SelectedKeys' selected key '{selectedKey.KeyId}'.");
            }
        }

        var includedTableIds = includedTables
            .Select(table => table.TableId)
            .ToHashSet(StringComparer.Ordinal);
        var assessmentsByTable = new Dictionary<string, KeyAssessmentEvidence>(StringComparer.Ordinal);
        foreach (var assessment in keyAssessments)
        {
            if (!includedTableIds.Contains(assessment.TableId))
            {
                throw new InvalidOperationException(
                    $"Relation 'KeyAssessments' returned table '{assessment.TableId}' that is absent from 'IncludedTables'.");
            }
            if (!assessmentsByTable.TryAdd(assessment.TableId, assessment))
            {
                throw new InvalidOperationException(
                    $"Relation 'KeyAssessments' returned more than one assessment for table '{assessment.TableId}'.");
            }
        }

        foreach (var tableId in includedTableIds)
        {
            if (!assessmentsByTable.TryGetValue(tableId, out var assessment))
            {
                throw new InvalidOperationException(
                    $"Relation 'KeyAssessments' did not return an assessment for included table '{tableId}'.");
            }

            var hasSelectedKey = selectedKeysByTable.ContainsKey(tableId);
            if (hasSelectedKey != (assessment.Assessment == TableKeyAssessment.Selected))
            {
                throw new InvalidOperationException(
                    $"Relations 'KeyAssessments' and 'SelectedKeys' disagree for table '{tableId}'.");
            }
        }
    }

    private static TableKeyAssessment ParseKeyAssessment(string value) => value switch
    {
        "selected" => TableKeyAssessment.Selected,
        "no-modeled-key" => TableKeyAssessment.NoModeledKey,
        "no-modeled-key-fields" => TableKeyAssessment.NoModeledKeyFields,
        "key-fields-excluded" => TableKeyAssessment.KeyFieldsExcluded,
        _ => throw new InvalidOperationException(
            $"Relation 'KeyAssessments' returned unsupported assessment '{value}'.")
    };

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
        IReadOnlyList<SelectedKeyFieldEvidence> SelectedKeyFields,
        IReadOnlyList<KeyAssessmentEvidence> KeyAssessments);

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

    private sealed record KeyAssessmentEvidence(
        string TableId,
        TableKeyAssessment Assessment);

    private enum TableKeyAssessment
    {
        Selected,
        NoModeledKey,
        NoModeledKeyFields,
        KeyFieldsExcluded,
    }
}
