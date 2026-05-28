using Meta.Core.Domain;

namespace MetaSql;

/// <summary>
/// Expands replacement/block actions implied by altered columns and dependent constraints.
/// </summary>
internal sealed class AlteredColumnConstraintExpansionService
{
    private readonly ManifestEntryFactory manifestEntryFactory;
    private readonly ManifestBlockFactory manifestBlockFactory;
    private readonly PrimaryKeyReplacementAssessmentService primaryKeyReplacementAssessmentService;
    private readonly ForeignKeyReplacementAssessmentService foreignKeyReplacementAssessmentService;
    private readonly IndexReplacementAssessmentService indexReplacementAssessmentService;

    public AlteredColumnConstraintExpansionService(
        ManifestEntryFactory manifestEntryFactory,
        ManifestBlockFactory manifestBlockFactory,
        PrimaryKeyReplacementAssessmentService primaryKeyReplacementAssessmentService,
        ForeignKeyReplacementAssessmentService foreignKeyReplacementAssessmentService,
        IndexReplacementAssessmentService indexReplacementAssessmentService)
    {
        this.manifestEntryFactory = manifestEntryFactory;
        this.manifestBlockFactory = manifestBlockFactory;
        this.primaryKeyReplacementAssessmentService = primaryKeyReplacementAssessmentService;
        this.foreignKeyReplacementAssessmentService = foreignKeyReplacementAssessmentService;
        this.indexReplacementAssessmentService = indexReplacementAssessmentService;
    }

    public ManifestPlanDelta Apply(
        ManifestPlanningContext context,
        ManifestPlanningLookupContext lookup,
        ManifestPlanDelta current)
    {
        _ = context;

        ExpandPrimaryAndForeignKeys(lookup, current);
        ExpandIndexes(lookup, current);
        return current;
    }

    private void ExpandPrimaryAndForeignKeys(
        ManifestPlanningLookupContext lookup,
        ManifestPlanDelta current)
    {
        var model = current.ManifestModel;
        var root = current.Root;

        var replacePrimaryKeyByPair = model.ReplacePrimaryKeyList
            .ToDictionary(
                row => BuildReplacePrimaryKeyPairKey(row.SourcePrimaryKeyId, row.LivePrimaryKeyId),
                row => row,
                StringComparer.Ordinal);
        var replaceForeignKeyByPair = model.ReplaceForeignKeyList
            .ToDictionary(
                row => BuildReplaceForeignKeyPairKey(row.SourceForeignKeyId, row.LiveForeignKeyId),
                row => row,
                StringComparer.Ordinal);

        var replacePrimaryKeySourceIds = model.ReplacePrimaryKeyList
            .Select(row => row.SourcePrimaryKeyId)
            .ToHashSet(StringComparer.Ordinal);
        var replacePrimaryKeyLiveIds = model.ReplacePrimaryKeyList
            .Select(row => row.LivePrimaryKeyId)
            .ToHashSet(StringComparer.Ordinal);
        var replaceForeignKeySourceIds = model.ReplaceForeignKeyList
            .Select(row => row.SourceForeignKeyId)
            .ToHashSet(StringComparer.Ordinal);
        var replaceForeignKeyLiveIds = model.ReplaceForeignKeyList
            .Select(row => row.LiveForeignKeyId)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var alterTableColumn in model.AlterTableColumnList
                     .OrderBy(row => row.SourceTableColumnId, StringComparer.Ordinal)
                     .ThenBy(row => row.LiveTableColumnId, StringComparer.Ordinal))
        {
            if (!lookup.SourceColumnsById.TryGetValue(alterTableColumn.SourceTableColumnId, out var sourceColumn) ||
                !lookup.LiveColumnsById.TryGetValue(alterTableColumn.LiveTableColumnId, out var liveColumn))
            {
                current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    "AlterTableColumn references missing source/live table-column row.");
                continue;
            }

            ExpandDependentPrimaryKeysForColumn(
                lookup,
                current,
                alterTableColumn,
                sourceColumn,
                liveColumn,
                replacePrimaryKeyByPair,
                replacePrimaryKeySourceIds,
                replacePrimaryKeyLiveIds);

            ExpandDependentForeignKeysForColumn(
                lookup,
                current,
                alterTableColumn,
                sourceColumn,
                liveColumn,
                replaceForeignKeyByPair,
                replaceForeignKeySourceIds,
                replaceForeignKeyLiveIds);
        }
    }

    private void ExpandDependentPrimaryKeysForColumn(
        ManifestPlanningLookupContext lookup,
        ManifestPlanDelta current,
        MetaSqlDeployManifest.AlterTableColumn alterTableColumn,
        GenericRecord sourceColumn,
        GenericRecord liveColumn,
        IDictionary<string, MetaSqlDeployManifest.ReplacePrimaryKey> replacePrimaryKeyByPair,
        ISet<string> replacePrimaryKeySourceIds,
        ISet<string> replacePrimaryKeyLiveIds)
    {
        var model = current.ManifestModel;
        var root = current.Root;
        var sourceDependentPrimaryKeyIds = lookup.SourcePrimaryKeyColumnsByColumnId.TryGetValue(sourceColumn.Id, out var sourcePrimaryKeyMembers)
            ? sourcePrimaryKeyMembers.Select(row => row.RelationshipIds["PrimaryKeyId"]).ToHashSet(StringComparer.Ordinal)
            : [];
        var liveDependentPrimaryKeyIds = lookup.LivePrimaryKeyColumnsByColumnId.TryGetValue(liveColumn.Id, out var livePrimaryKeyMembers)
            ? livePrimaryKeyMembers.Select(row => row.RelationshipIds["PrimaryKeyId"]).ToHashSet(StringComparer.Ordinal)
            : [];

        var sourcePrimaryKeysByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
        foreach (var sourcePrimaryKeyId in sourceDependentPrimaryKeyIds)
        {
            if (!lookup.SourcePrimaryKeysById.TryGetValue(sourcePrimaryKeyId, out var sourcePrimaryKey))
            {
                current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    $"AlterTableColumn references missing dependent source primary key '{sourcePrimaryKeyId}'.");
                continue;
            }

            var matchKey = BuildPrimaryKeyMatchKey(sourcePrimaryKey);
            if (!sourcePrimaryKeysByMatchKey.TryAdd(matchKey, sourcePrimaryKey))
            {
                current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    $"AlterTableColumn has ambiguous dependent source primary key identity by (TableId, Name): '{matchKey}'.");
            }
        }

        var livePrimaryKeysByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
        foreach (var livePrimaryKeyId in liveDependentPrimaryKeyIds)
        {
            if (!lookup.LivePrimaryKeysById.TryGetValue(livePrimaryKeyId, out var livePrimaryKey))
            {
                current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    $"AlterTableColumn references missing dependent live primary key '{livePrimaryKeyId}'.");
                continue;
            }

            var matchKey = BuildPrimaryKeyMatchKey(livePrimaryKey);
            if (!livePrimaryKeysByMatchKey.TryAdd(matchKey, livePrimaryKey))
            {
                current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    $"AlterTableColumn has ambiguous dependent live primary key identity by (TableId, Name): '{matchKey}'.");
            }
        }

        var allMatchKeys = sourcePrimaryKeysByMatchKey.Keys
            .Concat(livePrimaryKeysByMatchKey.Keys)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();

        foreach (var matchKey in allMatchKeys)
        {
            sourcePrimaryKeysByMatchKey.TryGetValue(matchKey, out var sourcePrimaryKey);
            livePrimaryKeysByMatchKey.TryGetValue(matchKey, out var livePrimaryKey);

            var sourceExists = sourcePrimaryKey is not null;
            var liveExists = livePrimaryKey is not null;
            var sourcePlanned = sourceExists &&
                                (model.AddPrimaryKeyList.Any(row => string.Equals(row.SourcePrimaryKeyId, sourcePrimaryKey!.Id, StringComparison.Ordinal)) ||
                                 replacePrimaryKeySourceIds.Contains(sourcePrimaryKey!.Id));
            var livePlanned = liveExists &&
                              (model.DropPrimaryKeyList.Any(row => string.Equals(row.LivePrimaryKeyId, livePrimaryKey!.Id, StringComparison.Ordinal)) ||
                               replacePrimaryKeyLiveIds.Contains(livePrimaryKey!.Id));

            if (sourceExists && liveExists)
            {
                var difference = new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.PrimaryKey,
                    DifferenceKind = MetaSqlDifferenceKind.Different,
                    SourceId = sourcePrimaryKey!.Id,
                    LiveId = livePrimaryKey!.Id,
                    DisplayName = GetValue(sourcePrimaryKey, "Name"),
                    ScopeDisplayName = string.Empty,
                };
                var primaryKeyAssessment = primaryKeyReplacementAssessmentService.Assess(difference, lookup);
                if (!primaryKeyAssessment.Executable)
                {
                    current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn requires executable replacement for dependent primary key '{GetValue(sourcePrimaryKey, "Name")}': {primaryKeyAssessment.Reason}");
                    continue;
                }

                var pairKey = BuildReplacePrimaryKeyPairKey(sourcePrimaryKey.Id, livePrimaryKey.Id);
                if (!replacePrimaryKeyByPair.ContainsKey(pairKey))
                {
                    current.ReplaceCount += manifestEntryFactory.ReplaceEntry(model, root, difference);
                    replacePrimaryKeyByPair[pairKey] = model.ReplacePrimaryKeyList.Last();
                    replacePrimaryKeySourceIds.Add(sourcePrimaryKey.Id);
                    replacePrimaryKeyLiveIds.Add(livePrimaryKey.Id);
                }

                continue;
            }

            if (sourceExists && !liveExists)
            {
                if (!sourcePlanned)
                {
                    current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn requires explicit add action for dependent source primary key '{sourcePrimaryKey!.Id}'.");
                }

                continue;
            }

            if (!sourceExists && liveExists)
            {
                if (!livePlanned)
                {
                    current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn requires explicit drop action for dependent live primary key '{livePrimaryKey!.Id}'.");
                }

                continue;
            }

            current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                model,
                root,
                alterTableColumn,
                $"AlterTableColumn has partial dependent primary key coverage for '{matchKey}'. Dependent primary key choreography must be explicit.");
        }
    }

    private void ExpandDependentForeignKeysForColumn(
        ManifestPlanningLookupContext lookup,
        ManifestPlanDelta current,
        MetaSqlDeployManifest.AlterTableColumn alterTableColumn,
        GenericRecord sourceColumn,
        GenericRecord liveColumn,
        IDictionary<string, MetaSqlDeployManifest.ReplaceForeignKey> replaceForeignKeyByPair,
        ISet<string> replaceForeignKeySourceIds,
        ISet<string> replaceForeignKeyLiveIds)
    {
        var model = current.ManifestModel;
        var root = current.Root;
        var sourceDependentForeignKeyIds = lookup.SourceForeignKeySourceColumnsByColumnId.TryGetValue(sourceColumn.Id, out var sourceForeignKeySourceMembers)
            ? sourceForeignKeySourceMembers.Select(row => row.RelationshipIds["ForeignKeyId"]).ToHashSet(StringComparer.Ordinal)
            : [];
        if (lookup.SourceForeignKeyTargetColumnsByColumnId.TryGetValue(sourceColumn.Id, out var sourceForeignKeyTargetMembers))
        {
            foreach (var sourceForeignKeyId in sourceForeignKeyTargetMembers.Select(row => row.RelationshipIds["ForeignKeyId"]))
            {
                sourceDependentForeignKeyIds.Add(sourceForeignKeyId);
            }
        }

        var liveDependentForeignKeyIds = lookup.LiveForeignKeySourceColumnsByColumnId.TryGetValue(liveColumn.Id, out var liveForeignKeySourceMembers)
            ? liveForeignKeySourceMembers.Select(row => row.RelationshipIds["ForeignKeyId"]).ToHashSet(StringComparer.Ordinal)
            : [];
        if (lookup.LiveForeignKeyTargetColumnsByColumnId.TryGetValue(liveColumn.Id, out var liveForeignKeyTargetMembers))
        {
            foreach (var liveForeignKeyId in liveForeignKeyTargetMembers.Select(row => row.RelationshipIds["ForeignKeyId"]))
            {
                liveDependentForeignKeyIds.Add(liveForeignKeyId);
            }
        }

        var sourceForeignKeysByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
        foreach (var sourceForeignKeyId in sourceDependentForeignKeyIds)
        {
            if (!lookup.SourceForeignKeysById.TryGetValue(sourceForeignKeyId, out var sourceForeignKey))
            {
                current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    $"AlterTableColumn references missing dependent source foreign key '{sourceForeignKeyId}'.");
                continue;
            }

            var matchKey = BuildForeignKeyMatchKey(sourceForeignKey);
            if (!sourceForeignKeysByMatchKey.TryAdd(matchKey, sourceForeignKey))
            {
                current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    $"AlterTableColumn has ambiguous dependent source foreign key identity by (SourceTableId, Name): '{matchKey}'.");
            }
        }

        var liveForeignKeysByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
        foreach (var liveForeignKeyId in liveDependentForeignKeyIds)
        {
            if (!lookup.LiveForeignKeysById.TryGetValue(liveForeignKeyId, out var liveForeignKey))
            {
                current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    $"AlterTableColumn references missing dependent live foreign key '{liveForeignKeyId}'.");
                continue;
            }

            var matchKey = BuildForeignKeyMatchKey(liveForeignKey);
            if (!liveForeignKeysByMatchKey.TryAdd(matchKey, liveForeignKey))
            {
                current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    $"AlterTableColumn has ambiguous dependent live foreign key identity by (SourceTableId, Name): '{matchKey}'.");
            }
        }

        var allMatchKeys = sourceForeignKeysByMatchKey.Keys
            .Concat(liveForeignKeysByMatchKey.Keys)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(row => row, StringComparer.Ordinal)
            .ToList();

        foreach (var matchKey in allMatchKeys)
        {
            sourceForeignKeysByMatchKey.TryGetValue(matchKey, out var sourceForeignKey);
            liveForeignKeysByMatchKey.TryGetValue(matchKey, out var liveForeignKey);

            var sourceExists = sourceForeignKey is not null;
            var liveExists = liveForeignKey is not null;
            var sourcePlanned = sourceExists &&
                                (model.AddForeignKeyList.Any(row => string.Equals(row.SourceForeignKeyId, sourceForeignKey!.Id, StringComparison.Ordinal)) ||
                                 replaceForeignKeySourceIds.Contains(sourceForeignKey!.Id));
            var livePlanned = liveExists &&
                              (model.DropForeignKeyList.Any(row => string.Equals(row.LiveForeignKeyId, liveForeignKey!.Id, StringComparison.Ordinal)) ||
                               replaceForeignKeyLiveIds.Contains(liveForeignKey!.Id));

            if (sourceExists && liveExists)
            {
                var difference = new MetaSqlDifference
                {
                    ObjectKind = MetaSqlObjectKind.ForeignKey,
                    DifferenceKind = MetaSqlDifferenceKind.Different,
                    SourceId = sourceForeignKey!.Id,
                    LiveId = liveForeignKey!.Id,
                    DisplayName = GetValue(sourceForeignKey, "Name"),
                    ScopeDisplayName = string.Empty,
                };
                var foreignKeyAssessment = foreignKeyReplacementAssessmentService.Assess(difference, lookup);
                if (!foreignKeyAssessment.Executable)
                {
                    current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn requires executable replacement for dependent foreign key '{GetValue(sourceForeignKey, "Name")}': {foreignKeyAssessment.Reason}");
                    continue;
                }

                var pairKey = BuildReplaceForeignKeyPairKey(sourceForeignKey.Id, liveForeignKey.Id);
                if (!replaceForeignKeyByPair.ContainsKey(pairKey))
                {
                    current.ReplaceCount += manifestEntryFactory.ReplaceEntry(model, root, difference);
                    replaceForeignKeyByPair[pairKey] = model.ReplaceForeignKeyList.Last();
                    replaceForeignKeySourceIds.Add(sourceForeignKey.Id);
                    replaceForeignKeyLiveIds.Add(liveForeignKey.Id);
                }

                continue;
            }

            if (sourceExists && !liveExists)
            {
                if (!sourcePlanned)
                {
                    current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn requires explicit add action for dependent source foreign key '{sourceForeignKey!.Id}'.");
                }

                continue;
            }

            if (!sourceExists && liveExists)
            {
                if (!livePlanned)
                {
                    current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn requires explicit drop action for dependent live foreign key '{liveForeignKey!.Id}'.");
                }

                continue;
            }

            current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                model,
                root,
                alterTableColumn,
                $"AlterTableColumn has partial dependent foreign key coverage for '{matchKey}'. Dependent foreign key choreography must be explicit.");
        }
    }

    private void ExpandIndexes(
        ManifestPlanningLookupContext lookup,
        ManifestPlanDelta current)
    {
        var model = current.ManifestModel;
        var root = current.Root;
        var replaceIndexByPair = model.ReplaceIndexList
            .ToDictionary(
                row => BuildReplaceIndexPairKey(row.SourceIndexId, row.LiveIndexId),
                row => row,
                StringComparer.Ordinal);

        var replaceSourceIndexIds = model.ReplaceIndexList
            .Select(row => row.SourceIndexId)
            .ToHashSet(StringComparer.Ordinal);
        var replaceLiveIndexIds = model.ReplaceIndexList
            .Select(row => row.LiveIndexId)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var alterTableColumn in model.AlterTableColumnList
                     .OrderBy(row => row.SourceTableColumnId, StringComparer.Ordinal)
                     .ThenBy(row => row.LiveTableColumnId, StringComparer.Ordinal))
        {
            if (!lookup.SourceColumnsById.TryGetValue(alterTableColumn.SourceTableColumnId, out var sourceColumn) ||
                !lookup.LiveColumnsById.TryGetValue(alterTableColumn.LiveTableColumnId, out var liveColumn))
            {
                current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    "AlterTableColumn references missing source/live table-column row.");
                continue;
            }

            var sourceDependentIndexIds = lookup.SourceIndexColumnsByColumnId.TryGetValue(sourceColumn.Id, out var sourceIndexMembers)
                ? sourceIndexMembers.Select(row => row.RelationshipIds["IndexId"]).ToHashSet(StringComparer.Ordinal)
                : [];
            var liveDependentIndexIds = lookup.LiveIndexColumnsByColumnId.TryGetValue(liveColumn.Id, out var liveIndexMembers)
                ? liveIndexMembers.Select(row => row.RelationshipIds["IndexId"]).ToHashSet(StringComparer.Ordinal)
                : [];

            var sourceByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
            foreach (var sourceIndexId in sourceDependentIndexIds)
            {
                if (!lookup.SourceIndexesById.TryGetValue(sourceIndexId, out var sourceIndex))
                {
                    current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn references missing dependent source index '{sourceIndexId}'.");
                    continue;
                }

                var key = BuildIndexMatchKey(sourceIndex);
                if (!sourceByMatchKey.TryAdd(key, sourceIndex))
                {
                    current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn has ambiguous dependent source index identity by (TableId, Name): '{key}'.");
                }
            }

            var liveByMatchKey = new Dictionary<string, GenericRecord>(StringComparer.Ordinal);
            foreach (var liveIndexId in liveDependentIndexIds)
            {
                if (!lookup.LiveIndexesById.TryGetValue(liveIndexId, out var liveIndex))
                {
                    current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn references missing dependent live index '{liveIndexId}'.");
                    continue;
                }

                var key = BuildIndexMatchKey(liveIndex);
                if (!liveByMatchKey.TryAdd(key, liveIndex))
                {
                    current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                        model,
                        root,
                        alterTableColumn,
                        $"AlterTableColumn has ambiguous dependent live index identity by (TableId, Name): '{key}'.");
                }
            }

            var allMatchKeys = sourceByMatchKey.Keys
                .Concat(liveByMatchKey.Keys)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(row => row, StringComparer.Ordinal)
                .ToList();

            foreach (var matchKey in allMatchKeys)
            {
                sourceByMatchKey.TryGetValue(matchKey, out var sourceIndex);
                liveByMatchKey.TryGetValue(matchKey, out var liveIndex);

                var sourceExists = sourceIndex is not null;
                var liveExists = liveIndex is not null;
                var sourcePlanned = sourceExists &&
                                    (model.AddIndexList.Any(row => string.Equals(row.SourceIndexId, sourceIndex!.Id, StringComparison.Ordinal)) ||
                                     replaceSourceIndexIds.Contains(sourceIndex!.Id));
                var livePlanned = liveExists &&
                                  (model.DropIndexList.Any(row => string.Equals(row.LiveIndexId, liveIndex!.Id, StringComparison.Ordinal)) ||
                                   replaceLiveIndexIds.Contains(liveIndex!.Id));

                if (sourceExists && liveExists)
                {
                    var difference = new MetaSqlDifference
                    {
                        ObjectKind = MetaSqlObjectKind.Index,
                        DifferenceKind = MetaSqlDifferenceKind.Different,
                        SourceId = sourceIndex!.Id,
                        LiveId = liveIndex!.Id,
                        DisplayName = GetValue(sourceIndex, "Name"),
                        ScopeDisplayName = string.Empty,
                    };
                    var assessment = indexReplacementAssessmentService.Assess(difference, lookup);
                    if (!assessment.Executable)
                    {
                        current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                            model,
                            root,
                            alterTableColumn,
                            $"AlterTableColumn requires executable replacement for dependent index '{GetValue(sourceIndex, "Name")}': {assessment.Reason}");
                        continue;
                    }

                    var pairKey = BuildReplaceIndexPairKey(sourceIndex.Id, liveIndex.Id);
                    if (!replaceIndexByPair.ContainsKey(pairKey))
                    {
                        current.ReplaceCount += manifestEntryFactory.ReplaceEntry(model, root, difference);
                        replaceIndexByPair[pairKey] = model.ReplaceIndexList.Last();
                        replaceSourceIndexIds.Add(sourceIndex.Id);
                        replaceLiveIndexIds.Add(liveIndex.Id);
                    }

                    continue;
                }

                if (sourceExists && !liveExists)
                {
                    if (!sourcePlanned)
                    {
                        current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                            model,
                            root,
                            alterTableColumn,
                            $"AlterTableColumn requires explicit add action for dependent source index '{sourceIndex!.Id}'.");
                    }

                    continue;
                }

                if (!sourceExists && liveExists)
                {
                    if (!livePlanned)
                    {
                        current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                            model,
                            root,
                            alterTableColumn,
                            $"AlterTableColumn requires explicit drop action for dependent live index '{liveIndex!.Id}'.");
                    }

                    continue;
                }

                current.BlockCount += manifestBlockFactory.AddColumnDependencyBlock(
                    model,
                    root,
                    alterTableColumn,
                    $"AlterTableColumn has partial dependent index coverage for '{matchKey}'. Dependent index choreography must be explicit.");
            }
        }
    }

    private static string BuildPrimaryKeyMatchKey(GenericRecord primaryKey)
    {
        var tableId = primaryKey.RelationshipIds["TableId"];
        var name = GetValue(primaryKey, "Name");
        return tableId + "|" + name;
    }

    private static string BuildForeignKeyMatchKey(GenericRecord foreignKey)
    {
        var sourceTableId = foreignKey.RelationshipIds["SourceTableId"];
        var name = GetValue(foreignKey, "Name");
        return sourceTableId + "|" + name;
    }

    private static string BuildIndexMatchKey(GenericRecord index)
    {
        var tableId = index.RelationshipIds["TableId"];
        var name = GetValue(index, "Name");
        return tableId + "|" + name;
    }

    private static string BuildReplacePrimaryKeyPairKey(string sourcePrimaryKeyId, string livePrimaryKeyId)
    {
        return sourcePrimaryKeyId + "|" + livePrimaryKeyId;
    }

    private static string BuildReplaceForeignKeyPairKey(string sourceForeignKeyId, string liveForeignKeyId)
    {
        return sourceForeignKeyId + "|" + liveForeignKeyId;
    }

    private static string BuildReplaceIndexPairKey(string sourceIndexId, string liveIndexId)
    {
        return sourceIndexId + "|" + liveIndexId;
    }

    private static string GetValue(GenericRecord record, string propertyName)
    {
        return record.Values.TryGetValue(propertyName, out var value) ? value : string.Empty;
    }
}
