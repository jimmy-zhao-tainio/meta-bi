namespace MetaSql;

/// <summary>
/// Expands dependent foreign-key actions implied by primary-key replacements.
/// </summary>
internal sealed class PrimaryKeyReplacementForeignKeyExpansionService
{
    private readonly ManifestEntryFactory manifestEntryFactory;
    private readonly ManifestBlockFactory manifestBlockFactory;

    public PrimaryKeyReplacementForeignKeyExpansionService(
        ManifestEntryFactory manifestEntryFactory,
        ManifestBlockFactory manifestBlockFactory)
    {
        this.manifestEntryFactory = manifestEntryFactory;
        this.manifestBlockFactory = manifestBlockFactory;
    }

    public ManifestPlanDelta Apply(
        ManifestPlanningContext context,
        ManifestPlanningLookupContext lookup,
        ManifestPlanDelta current)
    {
        var model = current.ManifestModel;
        var root = current.Root;
        var replaceForeignKeyByPair = model.ReplaceForeignKeyList
            .ToDictionary(
                row => BuildReplaceForeignKeyPairKey(row.SourceForeignKeyId, row.LiveForeignKeyId),
                row => row,
                StringComparer.Ordinal);

        foreach (var replacePrimaryKey in model.ReplacePrimaryKeyList
                     .OrderBy(row => row.SourcePrimaryKeyId, StringComparer.Ordinal)
                     .ThenBy(row => row.LivePrimaryKeyId, StringComparer.Ordinal))
        {
            if (!lookup.SourcePrimaryKeysById.TryGetValue(replacePrimaryKey.SourcePrimaryKeyId, out var sourcePrimaryKey) ||
                !lookup.LivePrimaryKeysById.TryGetValue(replacePrimaryKey.LivePrimaryKeyId, out var livePrimaryKey))
            {
                current.BlockCount += manifestBlockFactory.AddPrimaryKeyDependencyBlock(
                    model,
                    root,
                    replacePrimaryKey,
                    "ReplacePrimaryKey references missing source/live primary key row.");
                continue;
            }

            var sourceMembers = GetOrderedPrimaryKeyMembers(lookup.SourcePrimaryKeyColumnsByPrimaryKeyId, sourcePrimaryKey.Id);
            var liveMembers = GetOrderedPrimaryKeyMembers(lookup.LivePrimaryKeyColumnsByPrimaryKeyId, livePrimaryKey.Id);
            if (sourceMembers.Count == 0 || liveMembers.Count == 0)
            {
                current.BlockCount += manifestBlockFactory.AddPrimaryKeyDependencyBlock(
                    model,
                    root,
                    replacePrimaryKey,
                    "ReplacePrimaryKey requires source/live primary key member rows.");
                continue;
            }

            var sourceDependentForeignKeys = GetOrderedTargetTableForeignKeys(lookup.SourceForeignKeysByTargetTableId, sourcePrimaryKey.RelationshipIds["TableId"]);
            var liveDependentForeignKeys = GetOrderedTargetTableForeignKeys(lookup.LiveForeignKeysByTargetTableId, livePrimaryKey.RelationshipIds["TableId"]);

            var sourceByMatchKey = sourceDependentForeignKeys
                .ToDictionary(BuildForeignKeyMatchKey, row => row, StringComparer.Ordinal);
            var liveByMatchKey = liveDependentForeignKeys
                .ToDictionary(BuildForeignKeyMatchKey, row => row, StringComparer.Ordinal);
            if (sourceByMatchKey.Count != sourceDependentForeignKeys.Count ||
                liveByMatchKey.Count != liveDependentForeignKeys.Count)
            {
                current.BlockCount += manifestBlockFactory.AddPrimaryKeyDependencyBlock(
                    model,
                    root,
                    replacePrimaryKey,
                    "ReplacePrimaryKey has ambiguous dependent foreign key identity by (SourceTableId, Name).");
                continue;
            }

            var allKeys = sourceByMatchKey.Keys
                .Concat(liveByMatchKey.Keys)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(row => row, StringComparer.Ordinal)
                .ToList();

            foreach (var matchKey in allKeys)
            {
                sourceByMatchKey.TryGetValue(matchKey, out var sourceForeignKey);
                liveByMatchKey.TryGetValue(matchKey, out var liveForeignKey);

                var sourceExists = sourceForeignKey is not null;
                var liveExists = liveForeignKey is not null;
                var sourcePlanned = sourceExists &&
                                    (model.AddForeignKeyList.Any(row => string.Equals(row.SourceForeignKeyId, sourceForeignKey!.Id, StringComparison.Ordinal)) ||
                                     model.ReplaceForeignKeyList.Any(row => string.Equals(row.SourceForeignKeyId, sourceForeignKey!.Id, StringComparison.Ordinal)));
                var livePlanned = liveExists &&
                                  (model.DropForeignKeyList.Any(row => string.Equals(row.LiveForeignKeyId, liveForeignKey!.Id, StringComparison.Ordinal)) ||
                                   model.ReplaceForeignKeyList.Any(row => string.Equals(row.LiveForeignKeyId, liveForeignKey!.Id, StringComparison.Ordinal)));

                if (sourceExists && liveExists)
                {
                    var pairKey = BuildReplaceForeignKeyPairKey(sourceForeignKey!.Id, liveForeignKey!.Id);
                    if (!replaceForeignKeyByPair.ContainsKey(pairKey))
                    {
                        current.ReplaceCount += manifestEntryFactory.ReplaceEntry(model, root, new MetaSqlDifference
                        {
                            ObjectKind = MetaSqlObjectKind.ForeignKey,
                            DifferenceKind = MetaSqlDifferenceKind.Different,
                            SourceId = sourceForeignKey.Id,
                            LiveId = liveForeignKey.Id,
                            DisplayName = GetValue(sourceForeignKey, "Name"),
                            ScopeDisplayName = string.Empty,
                        });
                        replaceForeignKeyByPair[pairKey] = model.ReplaceForeignKeyList.Last();
                    }

                    continue;
                }

                if (sourceExists && !liveExists)
                {
                    if (sourcePlanned)
                    {
                        continue;
                    }

                    current.BlockCount += manifestBlockFactory.AddPrimaryKeyDependencyBlock(
                        model,
                        root,
                        replacePrimaryKey,
                        $"ReplacePrimaryKey requires explicit add action for dependent source foreign key '{sourceForeignKey!.Id}'.");
                    continue;
                }

                if (!sourceExists && liveExists)
                {
                    if (livePlanned)
                    {
                        continue;
                    }

                    current.BlockCount += manifestBlockFactory.AddPrimaryKeyDependencyBlock(
                        model,
                        root,
                        replacePrimaryKey,
                        $"ReplacePrimaryKey requires explicit drop action for dependent live foreign key '{liveForeignKey!.Id}'.");
                    continue;
                }

                current.BlockCount += manifestBlockFactory.AddPrimaryKeyDependencyBlock(
                    model,
                    root,
                    replacePrimaryKey,
                    $"ReplacePrimaryKey has partial dependent foreign key coverage for '{matchKey}'. Dependent FK choreography must be explicit.");
            }
        }

        return current;
    }

    private static List<Meta.Core.Domain.GenericRecord> GetOrderedPrimaryKeyMembers(
        IReadOnlyDictionary<string, List<Meta.Core.Domain.GenericRecord>> membersByPrimaryKeyId,
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

    private static List<Meta.Core.Domain.GenericRecord> GetOrderedTargetTableForeignKeys(
        IReadOnlyDictionary<string, List<Meta.Core.Domain.GenericRecord>> foreignKeysByTargetTableId,
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

    private static string BuildForeignKeyMatchKey(Meta.Core.Domain.GenericRecord foreignKey)
    {
        var sourceTableId = foreignKey.RelationshipIds["SourceTableId"];
        var name = GetValue(foreignKey, "Name");
        return sourceTableId + "|" + name;
    }

    private static string BuildReplaceForeignKeyPairKey(string sourceForeignKeyId, string liveForeignKeyId)
    {
        return sourceForeignKeyId + "|" + liveForeignKeyId;
    }

    private static int ParseOrdinal(string value)
    {
        return int.TryParse(value, out var ordinal) ? ordinal : int.MaxValue;
    }

    private static string GetValue(Meta.Core.Domain.GenericRecord record, string propertyName)
    {
        return record.Values.TryGetValue(propertyName, out var value) ? value : string.Empty;
    }
}
