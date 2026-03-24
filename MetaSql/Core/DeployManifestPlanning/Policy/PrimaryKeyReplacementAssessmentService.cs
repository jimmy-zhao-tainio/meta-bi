using Meta.Core.Domain;

namespace MetaSql;

/// <summary>
/// Assesses whether a shared primary key difference can be executed as ReplacePrimaryKey.
/// </summary>
internal sealed class PrimaryKeyReplacementAssessmentService
{
    public (bool Executable, string Reason) Assess(
        MetaSqlDifference difference,
        ManifestPlanningLookupContext lookup)
    {
        var sourceId = difference.SourceId ?? string.Empty;
        var liveId = difference.LiveId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sourceId) || string.IsNullOrWhiteSpace(liveId))
        {
            return (false, $"{difference.DisplayName}: missing SourceId/LiveId for changed primary key.");
        }

        if (!lookup.SourcePrimaryKeysById.TryGetValue(sourceId, out var sourcePrimaryKey) ||
            !lookup.LivePrimaryKeysById.TryGetValue(liveId, out var livePrimaryKey))
        {
            return (false, $"{difference.DisplayName}: changed primary key row is missing in source or live workspace.");
        }

        var sourceTableId = sourcePrimaryKey.RelationshipIds["TableId"];
        var liveTableId = livePrimaryKey.RelationshipIds["TableId"];
        if (!string.Equals(sourceTableId, liveTableId, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplacePrimaryKey requires the same table scope in source and live.");
        }

        var sourceName = GetValue(sourcePrimaryKey, "Name");
        var liveName = GetValue(livePrimaryKey, "Name");
        if (!string.Equals(sourceName, liveName, StringComparison.Ordinal))
        {
            return (false, $"{difference.DisplayName}: ReplacePrimaryKey requires identical primary key names.");
        }

        if (!lookup.SourceTablesById.ContainsKey(sourceTableId))
        {
            return (false, $"{difference.DisplayName}: source primary key references missing table '{sourceTableId}'.");
        }

        if (!(lookup.LiveTablesById.ContainsKey(sourceTableId) || lookup.PlannedAddedTableIds.Contains(sourceTableId)))
        {
            return (false, $"{difference.DisplayName}: primary key table '{sourceTableId}' is not present in live and not planned as AddTable.");
        }

        var sourceIsClusteredRaw = GetValue(sourcePrimaryKey, "IsClustered");
        var liveIsClusteredRaw = GetValue(livePrimaryKey, "IsClustered");
        if (!TryParseOptionalBoolean(sourceIsClusteredRaw, out var sourceIsClustered))
        {
            return (false, $"{difference.DisplayName}: source primary key has malformed IsClustered value '{sourceIsClusteredRaw}'.");
        }

        if (!TryParseOptionalBoolean(liveIsClusteredRaw, out var liveIsClustered))
        {
            return (false, $"{difference.DisplayName}: live primary key has malformed IsClustered value '{liveIsClusteredRaw}'.");
        }

        if (sourceIsClustered || liveIsClustered)
        {
            return (false, $"{difference.DisplayName}: clustered primary key replacement is blocked in this slice.");
        }

        var sourceMembers = GetOrderedPrimaryKeyMembers(lookup.SourcePrimaryKeyColumnsByPrimaryKeyId, sourceId);
        var liveMembers = GetOrderedPrimaryKeyMembers(lookup.LivePrimaryKeyColumnsByPrimaryKeyId, liveId);
        if (sourceMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: source primary key has no member rows.");
        }

        if (liveMembers.Count == 0)
        {
            return (false, $"{difference.DisplayName}: live primary key has no member rows.");
        }

        var sourcePkColumnIds = new List<string>(sourceMembers.Count);
        foreach (var sourceMember in sourceMembers)
        {
            var sourceColumnId = sourceMember.RelationshipIds["TableColumnId"];
            sourcePkColumnIds.Add(sourceColumnId);
            if (!lookup.SourceColumnsById.TryGetValue(sourceColumnId, out var sourceColumn))
            {
                return (false, $"{difference.DisplayName}: source primary key references missing table column '{sourceColumnId}'.");
            }

            if (!string.Equals(sourceColumn.RelationshipIds["TableId"], sourceTableId, StringComparison.Ordinal))
            {
                return (false, $"{difference.DisplayName}: source primary key member column '{sourceColumnId}' is outside the source table scope.");
            }

            if (!(lookup.LiveColumnsById.ContainsKey(sourceColumnId) || lookup.PlannedAddedColumnIds.Contains(sourceColumnId)))
            {
                return (false, $"{difference.DisplayName}: source primary key member column '{sourceColumnId}' is not present in live and not planned as AddTableColumn.");
            }

            var nullableRaw = GetValue(sourceColumn, "IsNullable");
            if (!TryParseOptionalBoolean(nullableRaw, out var isNullable))
            {
                return (false, $"{difference.DisplayName}: source primary key member column '{sourceColumnId}' has malformed IsNullable value '{nullableRaw}'.");
            }

            if (isNullable)
            {
                return (false, $"{difference.DisplayName}: source primary key member column '{sourceColumnId}' is nullable.");
            }

            var isDescendingRaw = GetValue(sourceMember, "IsDescending");
            if (!TryParseOptionalBoolean(isDescendingRaw, out _))
            {
                return (false, $"{difference.DisplayName}: source primary key member '{sourceMember.Id}' has malformed IsDescending value '{isDescendingRaw}'.");
            }
        }

        var livePkColumnIds = new List<string>(liveMembers.Count);
        foreach (var liveMember in liveMembers)
        {
            var liveColumnId = liveMember.RelationshipIds["TableColumnId"];
            livePkColumnIds.Add(liveColumnId);
            if (!lookup.LiveColumnsById.TryGetValue(liveColumnId, out var liveColumn))
            {
                return (false, $"{difference.DisplayName}: live primary key references missing table column '{liveColumnId}'.");
            }

            if (!string.Equals(liveColumn.RelationshipIds["TableId"], liveTableId, StringComparison.Ordinal))
            {
                return (false, $"{difference.DisplayName}: live primary key member column '{liveColumnId}' is outside the live table scope.");
            }

            var isDescendingRaw = GetValue(liveMember, "IsDescending");
            if (!TryParseOptionalBoolean(isDescendingRaw, out _))
            {
                return (false, $"{difference.DisplayName}: live primary key member '{liveMember.Id}' has malformed IsDescending value '{isDescendingRaw}'.");
            }
        }

        var sourceForeignKeyMatchKeys = lookup.SourceForeignKeysById.Values
            .Select(BuildForeignKeyMatchKey)
            .ToHashSet(StringComparer.Ordinal);
        var liveForeignKeyMatchKeys = lookup.LiveForeignKeysById.Values
            .Select(BuildForeignKeyMatchKey)
            .ToHashSet(StringComparer.Ordinal);

        var sourceDependentForeignKeys = GetOrderedTargetTableForeignKeys(lookup.SourceForeignKeysByTargetTableId, sourceTableId);
        foreach (var sourceForeignKey in sourceDependentForeignKeys)
        {
            var sourceForeignKeyId = sourceForeignKey.Id;
            var sourceForeignKeyMembers = GetOrderedForeignKeyMembers(lookup.SourceForeignKeyColumnsByForeignKeyId, sourceForeignKeyId);
            if (sourceForeignKeyMembers.Count == 0)
            {
                return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' has no member rows.");
            }

            var sourceTargetColumnIds = sourceForeignKeyMembers
                .Select(row => row.RelationshipIds["TargetColumnId"])
                .ToList();
            var referencesSourcePrimaryKey = sourceTargetColumnIds.SequenceEqual(sourcePkColumnIds);
            var intersectsSourcePrimaryKey = sourceTargetColumnIds.Any(row => sourcePkColumnIds.Contains(row, StringComparer.Ordinal));
            if (!referencesSourcePrimaryKey && intersectsSourcePrimaryKey)
            {
                return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' has unsupported target-column shape for ReplacePrimaryKey choreography.");
            }

            if (!referencesSourcePrimaryKey)
            {
                continue;
            }

            var sourceForeignKeySourceTableId = sourceForeignKey.RelationshipIds["SourceTableId"];
            if (!(lookup.LiveTablesById.ContainsKey(sourceForeignKeySourceTableId) || lookup.PlannedAddedTableIds.Contains(sourceForeignKeySourceTableId)))
            {
                return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' source table '{sourceForeignKeySourceTableId}' is not present in live and not planned as AddTable.");
            }

            foreach (var sourceForeignKeyMember in sourceForeignKeyMembers)
            {
                var sourceColumnId = sourceForeignKeyMember.RelationshipIds["SourceColumnId"];
                var targetColumnId = sourceForeignKeyMember.RelationshipIds["TargetColumnId"];
                if (!lookup.SourceColumnsById.TryGetValue(sourceColumnId, out var sourceColumn))
                {
                    return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' references missing source column '{sourceColumnId}'.");
                }

                if (!lookup.SourceColumnsById.TryGetValue(targetColumnId, out var targetColumn))
                {
                    return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' references missing target column '{targetColumnId}'.");
                }

                if (!(lookup.LiveColumnsById.ContainsKey(sourceColumnId) || lookup.PlannedAddedColumnIds.Contains(sourceColumnId)))
                {
                    return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' source column '{sourceColumnId}' is not present in live and not planned as AddTableColumn.");
                }

                if (!(lookup.LiveColumnsById.ContainsKey(targetColumnId) || lookup.PlannedAddedColumnIds.Contains(targetColumnId)))
                {
                    return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' target column '{targetColumnId}' is not present in live and not planned as AddTableColumn.");
                }

                if (!string.Equals(targetColumn.RelationshipIds["TableId"], sourceTableId, StringComparison.Ordinal))
                {
                    return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' target column '{targetColumnId}' is outside the source primary key table scope.");
                }
            }

            var sourceForeignKeyMatchKey = BuildForeignKeyMatchKey(sourceForeignKey);
            if (!(liveForeignKeyMatchKeys.Contains(sourceForeignKeyMatchKey) || lookup.PlannedAddedForeignKeyIds.Contains(sourceForeignKeyId)))
            {
                return (false, $"{difference.DisplayName}: source dependent foreign key '{sourceForeignKeyId}' is not present in live and not planned as AddForeignKey.");
            }
        }

        var liveDependentForeignKeys = GetOrderedTargetTableForeignKeys(lookup.LiveForeignKeysByTargetTableId, liveTableId);
        foreach (var liveForeignKey in liveDependentForeignKeys)
        {
            var liveForeignKeyId = liveForeignKey.Id;
            var liveForeignKeyMembers = GetOrderedForeignKeyMembers(lookup.LiveForeignKeyColumnsByForeignKeyId, liveForeignKeyId);
            if (liveForeignKeyMembers.Count == 0)
            {
                return (false, $"{difference.DisplayName}: live dependent foreign key '{liveForeignKeyId}' has no member rows.");
            }

            var liveTargetColumnIds = liveForeignKeyMembers
                .Select(row => row.RelationshipIds["TargetColumnId"])
                .ToList();
            var referencesLivePrimaryKey = liveTargetColumnIds.SequenceEqual(livePkColumnIds);
            var intersectsLivePrimaryKey = liveTargetColumnIds.Any(row => livePkColumnIds.Contains(row, StringComparer.Ordinal));
            if (!referencesLivePrimaryKey && intersectsLivePrimaryKey)
            {
                return (false, $"{difference.DisplayName}: live dependent foreign key '{liveForeignKeyId}' has unsupported target-column shape for ReplacePrimaryKey choreography.");
            }

            if (!referencesLivePrimaryKey)
            {
                continue;
            }

            var liveForeignKeyMatchKey = BuildForeignKeyMatchKey(liveForeignKey);
            if (!(sourceForeignKeyMatchKeys.Contains(liveForeignKeyMatchKey) || lookup.PlannedDroppedForeignKeyIds.Contains(liveForeignKeyId)))
            {
                return (false, $"{difference.DisplayName}: live dependent foreign key '{liveForeignKeyId}' has no source equivalent and is not planned as DropForeignKey.");
            }
        }

        return (true, string.Empty);
    }

    private static List<GenericRecord> GetOrderedPrimaryKeyMembers(
        IReadOnlyDictionary<string, List<GenericRecord>> membersByPrimaryKeyId,
        string primaryKeyId)
    {
        if (!membersByPrimaryKeyId.TryGetValue(primaryKeyId, out var members))
        {
            return [];
        }

        return members
            .OrderBy(row => ParseOrdinal(GetValue(row, "Ordinal")))
            .ThenBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static List<GenericRecord> GetOrderedTargetTableForeignKeys(
        IReadOnlyDictionary<string, List<GenericRecord>> foreignKeysByTargetTableId,
        string targetTableId)
    {
        if (!foreignKeysByTargetTableId.TryGetValue(targetTableId, out var foreignKeys))
        {
            return [];
        }

        return foreignKeys
            .OrderBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static List<GenericRecord> GetOrderedForeignKeyMembers(
        IReadOnlyDictionary<string, List<GenericRecord>> membersByForeignKeyId,
        string foreignKeyId)
    {
        if (!membersByForeignKeyId.TryGetValue(foreignKeyId, out var members))
        {
            return [];
        }

        return members
            .OrderBy(row => ParseOrdinal(GetValue(row, "Ordinal")))
            .ThenBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
    }

    private static string BuildForeignKeyMatchKey(GenericRecord foreignKey)
    {
        var sourceTableId = foreignKey.RelationshipIds["SourceTableId"];
        var name = GetValue(foreignKey, "Name");
        return sourceTableId + "|" + name;
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

    private static int ParseOrdinal(string value)
    {
        return int.TryParse(value, out var ordinal) ? ordinal : int.MaxValue;
    }

    private static string GetValue(GenericRecord record, string propertyName)
    {
        return record.Values.TryGetValue(propertyName, out var value) ? value : string.Empty;
    }
}
