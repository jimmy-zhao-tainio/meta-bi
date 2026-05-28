using Meta.Core.Domain;

namespace MetaSql;

/// <summary>
/// Assesses whether a shared foreign key difference can be executed as ReplaceForeignKey.
/// </summary>
internal sealed class ForeignKeyReplacementAssessmentService
{
    public (bool Executable, string Reason) Assess(
        MetaSqlDifference difference,
        ManifestPlanningLookupContext lookup)
    {
        var sourceId = difference.SourceId ?? string.Empty;
        var liveId = difference.LiveId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(liveId))
        {
            return (false, $"{difference.DisplayName}: missing SourceId/LiveId for changed foreign key.");
        }

        if (!lookup.SourceForeignKeysById.TryGetValue(sourceId, out var sourceForeignKey) ||
            !lookup.LiveForeignKeysById.TryGetValue(liveId, out var liveForeignKey))
        {
            return (false, $"{difference.DisplayName}: changed foreign key row is missing in source or live workspace.");
        }

        var sourceTableId = sourceForeignKey.RelationshipIds["SourceTableId"];
        var liveTableId = liveForeignKey.RelationshipIds["SourceTableId"];
        if (!string.Equals(sourceTableId, liveTableId, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplaceForeignKey requires the same source table scope in source and live.");
        }

        var sourceName = GetValue(sourceForeignKey, "Name");
        var liveName = GetValue(liveForeignKey, "Name");
        if (!string.Equals(sourceName, liveName, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplaceForeignKey requires identical foreign key names.");
        }

        var sourceMembers = GetOrderedForeignKeyMembers(sourceId, lookup.SourceForeignKeyColumnsByForeignKeyId);
        var liveMembers = GetOrderedForeignKeyMembers(liveId, lookup.LiveForeignKeyColumnsByForeignKeyId);
        if (sourceMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: source foreign key has no member rows.");
        }

        if (liveMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: live foreign key has no member rows.");
        }

        if (sourceMembers.Count != liveMembers.Count)
        {
            return (false, $"{difference.DisplayName}: ReplaceForeignKey requires matching member counts in this slice.");
        }

        foreach (var sourceMember in sourceMembers)
        {
            var sourceColumnId = sourceMember.RelationshipIds["SourceColumnId"];
            var targetColumnId = sourceMember.RelationshipIds["TargetColumnId"];
            if (!lookup.SourceColumnsById.TryGetValue(sourceColumnId, out var sourceColumn))
            {
                return (false, $"{difference.DisplayName}: source foreign key member references missing source column '{sourceColumnId}'.");
            }

            if (!lookup.SourceColumnsById.TryGetValue(targetColumnId, out var targetColumn))
            {
                return (false, $"{difference.DisplayName}: source foreign key member references missing target column '{targetColumnId}'.");
            }

            var sourceColumnTableId = sourceColumn.RelationshipIds["TableId"];
            var sourceTargetTableId = targetColumn.RelationshipIds["TableId"];
            if (!lookup.PlannedAddedTableIds.Contains(sourceColumnTableId) &&
                !lookup.LiveTablesById.ContainsKey(sourceColumnTableId))
            {
                return (false, $"{difference.DisplayName}: source foreign key member references source table '{sourceColumnTableId}' that is neither live nor planned add.");
            }

            if (!lookup.PlannedAddedTableIds.Contains(sourceTargetTableId) &&
                !lookup.LiveTablesById.ContainsKey(sourceTargetTableId))
            {
                return (false, $"{difference.DisplayName}: source foreign key member references target table '{sourceTargetTableId}' that is neither live nor planned add.");
            }
        }

        foreach (var liveMember in liveMembers)
        {
            var liveSourceColumnId = liveMember.RelationshipIds["SourceColumnId"];
            if (!lookup.LiveColumnsById.TryGetValue(liveSourceColumnId, out var liveSourceColumn))
            {
                return (false, $"{difference.DisplayName}: live foreign key member references missing source column '{liveSourceColumnId}'.");
            }

            var liveSourceColumnTableId = liveSourceColumn.RelationshipIds["TableId"];
            if (lookup.PlannedAddedTableIds.Contains(liveSourceColumnTableId))
            {
                return (false, $"{difference.DisplayName}: live foreign key member references a table planned as add, which is unsupported for ReplaceForeignKey.");
            }
        }

        return (true, string.Empty);
    }

    private static IReadOnlyList<GenericRecord> GetOrderedForeignKeyMembers(
        string foreignKeyId,
        IReadOnlyDictionary<string, List<GenericRecord>> membersByForeignKeyId)
    {
        if (!membersByForeignKeyId.TryGetValue(foreignKeyId, out var members))
        {
            return Array.Empty<GenericRecord>();
        }

        return members
            .OrderBy(row => ParseOrdinal(GetValue(row, "Ordinal")))
            .ThenBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static string GetValue(GenericRecord record, string propertyName)
    {
        return record.Values.TryGetValue(propertyName, out var value) ? value : string.Empty;
    }

    private static int ParseOrdinal(string value)
    {
        return int.TryParse(value, out var ordinal) ? ordinal : int.MaxValue;
    }
}
