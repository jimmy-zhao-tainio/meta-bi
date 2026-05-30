namespace MetaSql;

/// <summary>
/// Plans schema-scoped SQL module changes from explicit object definitions.
/// </summary>
internal sealed class SqlModuleDifferencePlanner
{
    private readonly ManifestEntryFactory manifestEntryFactory;
    private readonly ManifestBlockFactory manifestBlockFactory;

    public SqlModuleDifferencePlanner(
        ManifestEntryFactory manifestEntryFactory,
        ManifestBlockFactory manifestBlockFactory)
    {
        this.manifestEntryFactory = manifestEntryFactory;
        this.manifestBlockFactory = manifestBlockFactory;
    }

    public void Plan(
        ManifestPlanningLookupContext lookup,
        ManifestPlanDelta delta,
        MetaSqlDifference difference)
    {
        switch (difference.DifferenceKind)
        {
            case MetaSqlDifferenceKind.MissingInLive:
                delta.AddCount += AddMissingSchemaEntryIfNeeded(lookup, delta, difference);
                delta.AddCount += manifestEntryFactory.AddEntry(delta.ManifestModel, delta.Root, difference);
                break;
            case MetaSqlDifferenceKind.ExtraInLive:
                delta.DropCount += manifestEntryFactory.DropEntry(delta.ManifestModel, delta.Root, difference);
                break;
            case MetaSqlDifferenceKind.Different:
                delta.ReplaceCount += manifestEntryFactory.ReplaceEntry(delta.ManifestModel, delta.Root, difference);
                break;
            default:
                delta.BlockCount += manifestBlockFactory.BlockEntry(delta.ManifestModel, delta.Root, difference);
                break;
        }
    }

    private int AddMissingSchemaEntryIfNeeded(
        ManifestPlanningLookupContext lookup,
        ManifestPlanDelta delta,
        MetaSqlDifference difference)
    {
        if (string.IsNullOrWhiteSpace(difference.SourceId))
        {
            return 0;
        }

        var entityName = difference.ObjectKind switch
        {
            MetaSqlObjectKind.View => "View",
            MetaSqlObjectKind.Function => "Function",
            MetaSqlObjectKind.StoredProcedure => "StoredProcedure",
            _ => throw new InvalidOperationException($"Unsupported SQL module kind '{difference.ObjectKind}'.")
        };
        var sourceRow = lookup.SourceWorkspace.Instance
            .GetOrCreateEntityRecords(entityName)
            .Single(row => string.Equals(row.Id, difference.SourceId, StringComparison.Ordinal));
        var sourceSchemaId = sourceRow.RelationshipIds["SchemaId"];
        var sourceSchema = lookup.SourceSchemasById[sourceSchemaId];
        var sourceSchemaName = sourceSchema.Values["Name"];
        var liveHasSchema = lookup.LiveSchemasById.Values.Any(row =>
            string.Equals(row.Values["Name"], sourceSchemaName, StringComparison.Ordinal));
        if (liveHasSchema ||
            delta.ManifestModel.AddSchemaList.Any(row => string.Equals(row.SourceSchemaId, sourceSchemaId, StringComparison.Ordinal)))
        {
            return 0;
        }

        return manifestEntryFactory.AddSchemaEntry(delta.ManifestModel, delta.Root, sourceSchemaId);
    }
}
