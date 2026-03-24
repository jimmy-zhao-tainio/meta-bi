using Meta.Core.Domain;

namespace MetaSql;

/// <summary>
/// Evaluates exact-scope destructive approvals for data-bearing drop operations.
/// </summary>
internal sealed class DestructiveApprovalAssessmentService
{
    public (bool Approved, string Reason) AssessDataDropTableApproval(
        MetaSqlDifference difference,
        IReadOnlyDictionary<string, GenericRecord> liveTablesById,
        IReadOnlyDictionary<string, GenericRecord> liveSchemasById,
        MetaSqlDestructiveApprovalSet approvalSet)
    {
        var liveId = difference.LiveId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(liveId) || !liveTablesById.TryGetValue(liveId, out var liveTable))
        {
            return (false, $"{difference.DisplayName}: live table row is missing.");
        }

        if (!TryGetTableScope(liveTable, liveSchemasById, out var schemaName, out var tableName))
        {
            return (false, $"{difference.DisplayName}: live table scope is missing.");
        }

        if (approvalSet.HasDataDropTable(schemaName, tableName))
        {
            return (true, string.Empty);
        }

        var requiredKey = MetaSqlDestructiveApprovalSet.BuildTableKey(schemaName, tableName);
        return (false, $"{difference.DisplayName}: missing approval DataDropTable({requiredKey}).");
    }

    public (bool Approved, string Reason) AssessDataDropColumnApproval(
        MetaSqlDifference difference,
        IReadOnlyDictionary<string, GenericRecord> liveColumnsById,
        IReadOnlyDictionary<string, GenericRecord> liveTablesById,
        IReadOnlyDictionary<string, GenericRecord> liveSchemasById,
        MetaSqlDestructiveApprovalSet approvalSet)
    {
        var liveId = difference.LiveId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(liveId) || !liveColumnsById.TryGetValue(liveId, out var liveColumn))
        {
            return (false, $"{difference.DisplayName}: live column row is missing.");
        }

        if (!TryGetColumnScope(liveColumn, liveTablesById, liveSchemasById, out var schemaName, out var tableName, out var columnName))
        {
            return (false, $"{difference.DisplayName}: live column scope is missing.");
        }

        if (approvalSet.HasDataDropColumn(schemaName, tableName, columnName))
        {
            return (true, string.Empty);
        }

        var requiredKey = MetaSqlDestructiveApprovalSet.BuildColumnKey(schemaName, tableName, columnName);
        return (false, $"{difference.DisplayName}: missing approval DataDropColumn({requiredKey}).");
    }

    private static bool TryGetTableScope(
        GenericRecord tableRecord,
        IReadOnlyDictionary<string, GenericRecord> schemasById,
        out string schemaName,
        out string tableName)
    {
        schemaName = string.Empty;
        tableName = string.Empty;

        if (!tableRecord.RelationshipIds.TryGetValue("SchemaId", out var schemaId) ||
            string.IsNullOrWhiteSpace(schemaId) ||
            !schemasById.TryGetValue(schemaId, out var schemaRecord))
        {
            return false;
        }

        schemaName = GetValue(schemaRecord, "Name");
        tableName = GetValue(tableRecord, "Name");
        return !string.IsNullOrWhiteSpace(schemaName) && !string.IsNullOrWhiteSpace(tableName);
    }

    private static bool TryGetColumnScope(
        GenericRecord columnRecord,
        IReadOnlyDictionary<string, GenericRecord> tablesById,
        IReadOnlyDictionary<string, GenericRecord> schemasById,
        out string schemaName,
        out string tableName,
        out string columnName)
    {
        schemaName = string.Empty;
        tableName = string.Empty;
        columnName = string.Empty;

        if (!columnRecord.RelationshipIds.TryGetValue("TableId", out var tableId) ||
            string.IsNullOrWhiteSpace(tableId) ||
            !tablesById.TryGetValue(tableId, out var tableRecord))
        {
            return false;
        }

        if (!TryGetTableScope(tableRecord, schemasById, out schemaName, out tableName))
        {
            return false;
        }

        columnName = GetValue(columnRecord, "Name");
        return !string.IsNullOrWhiteSpace(columnName);
    }

    private static string GetValue(GenericRecord record, string propertyName)
    {
        return record.Values.TryGetValue(propertyName, out var value) ? value : string.Empty;
    }
}
