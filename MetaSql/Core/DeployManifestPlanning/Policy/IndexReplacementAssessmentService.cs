using Meta.Core.Domain;

namespace MetaSql;

/// <summary>
/// Assesses whether a shared index difference can be executed as ReplaceIndex.
/// </summary>
internal sealed class IndexReplacementAssessmentService
{
    public (bool Executable, string Reason) Assess(
        MetaSqlDifference difference,
        ManifestPlanningLookupContext lookup)
    {
        var sourceId = difference.SourceId ?? string.Empty;
        var liveId = difference.LiveId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(liveId))
        {
            return (false, $"{difference.DisplayName}: missing SourceId/LiveId for changed index.");
        }

        if (!lookup.SourceIndexesById.TryGetValue(sourceId, out var sourceIndex) ||
            !lookup.LiveIndexesById.TryGetValue(liveId, out var liveIndex))
        {
            return (false, $"{difference.DisplayName}: changed index row is missing in source or live workspace.");
        }

        var sourceTableId = sourceIndex.RelationshipIds["TableId"];
        var liveTableId = liveIndex.RelationshipIds["TableId"];
        if (!string.Equals(sourceTableId, liveTableId, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplaceIndex requires the same table scope in source and live.");
        }

        var sourceName = GetValue(sourceIndex, "Name");
        var liveName = GetValue(liveIndex, "Name");
        if (!string.Equals(sourceName, liveName, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplaceIndex requires identical index names.");
        }

        var sourceIsClusteredRaw = GetValue(sourceIndex, "IsClustered");
        var liveIsClusteredRaw = GetValue(liveIndex, "IsClustered");
        if (!TryParseOptionalBoolean(sourceIsClusteredRaw, out var sourceIsClustered) ||
            !TryParseOptionalBoolean(liveIsClusteredRaw, out var liveIsClustered))
        {
            return (false, $"{difference.DisplayName}: ReplaceIndex requires parseable IsClustered values.");
        }

        if (sourceIsClustered || liveIsClustered)
        {
            return (false, $"{difference.DisplayName}: clustered index replacement is blocked in this slice.");
        }

        var sourceIsUniqueRaw = GetValue(sourceIndex, "IsUnique");
        var liveIsUniqueRaw = GetValue(liveIndex, "IsUnique");
        if (!TryParseOptionalBoolean(sourceIsUniqueRaw, out _) ||
            !TryParseOptionalBoolean(liveIsUniqueRaw, out _))
        {
            return (false, $"{difference.DisplayName}: ReplaceIndex requires parseable IsUnique values.");
        }

        var sourceMembers = GetOrderedIndexMembers(sourceId, lookup.SourceIndexColumnsByIndexId);
        var liveMembers = GetOrderedIndexMembers(liveId, lookup.LiveIndexColumnsByIndexId);
        if (sourceMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: source index has no member rows.");
        }

        if (liveMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: live index has no member rows.");
        }

        foreach (var sourceMember in sourceMembers)
        {
            var sourceColumnId = sourceMember.RelationshipIds["TableColumnId"];
            if (!lookup.SourceColumnsById.TryGetValue(sourceColumnId, out var sourceColumn))
            {
                return (false, $"{difference.DisplayName}: source index member references missing source column '{sourceColumnId}'.");
            }

            var sourceColumnTableId = sourceColumn.RelationshipIds["TableId"];
            if (!lookup.PlannedAddedTableIds.Contains(sourceColumnTableId) &&
                !lookup.LiveTablesById.ContainsKey(sourceColumnTableId))
            {
                return (false, $"{difference.DisplayName}: source index member references table '{sourceColumnTableId}' that is neither live nor planned add.");
            }

            var sourceMemberIsIncludedRaw = GetValue(sourceMember, "IsIncluded");
            var sourceMemberIsDescendingRaw = GetValue(sourceMember, "IsDescending");
            if (!TryParseOptionalBoolean(sourceMemberIsIncludedRaw, out _) ||
                !TryParseOptionalBoolean(sourceMemberIsDescendingRaw, out _))
            {
                return (false, $"{difference.DisplayName}: source index member has non-boolean IsIncluded/IsDescending.");
            }
        }

        foreach (var liveMember in liveMembers)
        {
            var liveColumnId = liveMember.RelationshipIds["TableColumnId"];
            if (!lookup.LiveColumnsById.TryGetValue(liveColumnId, out var liveColumn))
            {
                return (false, $"{difference.DisplayName}: live index member references missing live column '{liveColumnId}'.");
            }

            var liveColumnTableId = liveColumn.RelationshipIds["TableId"];
            if (lookup.PlannedAddedTableIds.Contains(liveColumnTableId))
            {
                return (false, $"{difference.DisplayName}: live index member references table planned as add, unsupported for ReplaceIndex.");
            }

            var liveMemberIsIncludedRaw = GetValue(liveMember, "IsIncluded");
            var liveMemberIsDescendingRaw = GetValue(liveMember, "IsDescending");
            if (!TryParseOptionalBoolean(liveMemberIsIncludedRaw, out _) ||
                !TryParseOptionalBoolean(liveMemberIsDescendingRaw, out _))
            {
                return (false, $"{difference.DisplayName}: live index member has non-boolean IsIncluded/IsDescending.");
            }
        }

        return (true, string.Empty);
    }

    private static IReadOnlyList<GenericRecord> GetOrderedIndexMembers(
        string indexId,
        IReadOnlyDictionary<string, List<GenericRecord>> membersByIndexId)
    {
        if (!membersByIndexId.TryGetValue(indexId, out var members))
        {
            return Array.Empty<GenericRecord>();
        }

        return members
            .OrderBy(row => ParseOrdinal(GetValue(row, "Ordinal")))
            .ThenBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static bool TryParseOptionalBoolean(string value, out bool parsedValue)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            parsedValue = false;
            return true;
        }

        if (string.Equals(value.Trim(), "true", StringComparison.OrdinalIgnoreCase))
        {
            parsedValue = true;
            return true;
        }

        if (string.Equals(value.Trim(), "false", StringComparison.OrdinalIgnoreCase))
        {
            parsedValue = false;
            return true;
        }

        parsedValue = false;
        return false;
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
