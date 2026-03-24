namespace MetaSql;

/// <summary>
/// Expands add actions for dependencies implied by added tables.
/// </summary>
internal sealed class AddedTableDependencyExpansionService
{
    private readonly ManifestEntryFactory manifestEntryFactory;

    public AddedTableDependencyExpansionService(ManifestEntryFactory manifestEntryFactory)
    {
        this.manifestEntryFactory = manifestEntryFactory;
    }

    public ManifestPlanDelta Apply(
        ManifestPlanningContext context,
        ManifestPlanningLookupContext lookup,
        ManifestPlanDelta current)
    {
        var model = current.ManifestModel;
        var root = current.Root;
        var addedTableIds = model.AddTableList
            .Select(row => row.SourceTableId)
            .ToHashSet(StringComparer.Ordinal);
        if (addedTableIds.Count == 0)
        {
            return current;
        }

        var primaryKeyIds = model.AddPrimaryKeyList
            .Select(row => row.SourcePrimaryKeyId)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var sourcePrimaryKey in context.SourceWorkspace.Instance.GetOrCreateEntityRecords("PrimaryKey")
                     .Where(row => addedTableIds.Contains(row.RelationshipIds["TableId"]))
                     .OrderBy(row => row.Id, StringComparer.Ordinal))
        {
            if (!primaryKeyIds.Add(sourcePrimaryKey.Id))
            {
                continue;
            }

            current.AddCount += manifestEntryFactory.AddEntry(model, root, new MetaSqlDifference
            {
                ObjectKind = MetaSqlObjectKind.PrimaryKey,
                DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                SourceId = sourcePrimaryKey.Id,
                LiveId = sourcePrimaryKey.Id,
                DisplayName = sourcePrimaryKey.Id,
                ScopeDisplayName = string.Empty,
            });
        }

        var foreignKeyIds = model.AddForeignKeyList
            .Select(row => row.SourceForeignKeyId)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var sourceForeignKey in context.SourceWorkspace.Instance.GetOrCreateEntityRecords("ForeignKey")
                     .Where(row => addedTableIds.Contains(row.RelationshipIds["SourceTableId"]))
                     .OrderBy(row => row.Id, StringComparer.Ordinal))
        {
            if (!foreignKeyIds.Add(sourceForeignKey.Id))
            {
                continue;
            }

            current.AddCount += manifestEntryFactory.AddEntry(model, root, new MetaSqlDifference
            {
                ObjectKind = MetaSqlObjectKind.ForeignKey,
                DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                SourceId = sourceForeignKey.Id,
                LiveId = sourceForeignKey.Id,
                DisplayName = sourceForeignKey.Id,
                ScopeDisplayName = string.Empty,
            });
        }

        var indexIds = model.AddIndexList
            .Select(row => row.SourceIndexId)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var sourceIndex in context.SourceWorkspace.Instance.GetOrCreateEntityRecords("Index")
                     .Where(row => addedTableIds.Contains(row.RelationshipIds["TableId"]))
                     .OrderBy(row => row.Id, StringComparer.Ordinal))
        {
            if (!indexIds.Add(sourceIndex.Id))
            {
                continue;
            }

            current.AddCount += manifestEntryFactory.AddEntry(model, root, new MetaSqlDifference
            {
                ObjectKind = MetaSqlObjectKind.Index,
                DifferenceKind = MetaSqlDifferenceKind.MissingInLive,
                SourceId = sourceIndex.Id,
                LiveId = sourceIndex.Id,
                DisplayName = sourceIndex.Id,
                ScopeDisplayName = string.Empty,
            });
        }

        return current;
    }
}
