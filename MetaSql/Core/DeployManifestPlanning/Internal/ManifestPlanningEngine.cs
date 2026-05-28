using Meta.Adapters;
using Meta.Core.Domain;
using MetaSqlDeployManifest;

namespace MetaSql;

public sealed record MetaSqlDeployManifestBuildResult
{
    public required MetaSqlDeployManifestModel ManifestModel { get; init; }
    public required int AddCount { get; init; }
    public required int DropCount { get; init; }
    public required int AlterCount { get; init; }
    public required int TruncateCount { get; init; }
    public required int ReplaceCount { get; init; }
    public required int BlockCount { get; init; }
    public bool IsDeployable => BlockCount == 0;
}

internal sealed class ManifestPlanningEngine
{
    private readonly TableColumnAlterAssessmentService tableColumnAlterAssessmentService;
    private readonly DifferencePlanDispatcher dispatcher;
    private readonly AddedTableDependencyExpansionService addedTableExpansionService;
    private readonly AlteredColumnConstraintExpansionService alteredColumnExpansionService;
    private readonly PrimaryKeyReplacementForeignKeyExpansionService primaryKeyForeignKeyExpansionService;

    public ManifestPlanningEngine(
        TableColumnAlterAssessmentService tableColumnAlterAssessmentService,
        DifferencePlanDispatcher dispatcher,
        AddedTableDependencyExpansionService addedTableExpansionService,
        AlteredColumnConstraintExpansionService alteredColumnExpansionService,
        PrimaryKeyReplacementForeignKeyExpansionService primaryKeyForeignKeyExpansionService)
    {
        this.tableColumnAlterAssessmentService = tableColumnAlterAssessmentService;
        this.dispatcher = dispatcher;
        this.addedTableExpansionService = addedTableExpansionService;
        this.alteredColumnExpansionService = alteredColumnExpansionService;
        this.primaryKeyForeignKeyExpansionService = primaryKeyForeignKeyExpansionService;
    }

    public MetaSqlDeployManifestBuildResult BuildManifest(
        Workspace sourceWorkspace,
        Workspace liveWorkspace,
        MetaSqlLiveDatabasePresence liveDatabasePresence,
        IReadOnlyList<MetaSqlDifference> differences,
        string manifestName,
        string? targetDescription,
        IReadOnlyList<MetaSqlDifferenceBlocker>? feasibilityBlockers = null,
        IReadOnlyList<MetaSqlDestructiveApproval>? destructiveApprovals = null)
    {
        ArgumentNullException.ThrowIfNull(sourceWorkspace);
        ArgumentNullException.ThrowIfNull(liveWorkspace);
        ArgumentNullException.ThrowIfNull(differences);
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestName);

        MetaSqlDiffService.EnsureMetaSqlWorkspace(sourceWorkspace, nameof(sourceWorkspace));
        MetaSqlDiffService.EnsureMetaSqlWorkspace(liveWorkspace, nameof(liveWorkspace));

        var context = new ManifestPlanningContext
        {
            SourceWorkspace = sourceWorkspace,
            LiveWorkspace = liveWorkspace,
            LiveDatabasePresence = liveDatabasePresence,
            Differences = differences,
            ManifestName = manifestName,
            TargetDescription = targetDescription,
            FeasibilityBlockers = feasibilityBlockers,
            DestructiveApprovals = destructiveApprovals,
        };

        var model = MetaSqlDeployManifestModel.CreateEmpty();
        var root = new DeployManifest
        {
            Id = "DeployManifest:1",
            Name = manifestName,
            SourceInstanceFingerprint = MetaSqlInstanceFingerprint.Compute(sourceWorkspace),
            LiveInstanceFingerprint = MetaSqlInstanceFingerprint.Compute(liveWorkspace),
            ExpectedLiveDatabasePresence = context.LiveDatabasePresence.ToString(),
            CreatedUtc = DateTime.UtcNow.ToString("O"),
            TargetDescription = targetDescription ?? string.Empty,
        };
        model.DeployManifestList.Add(root);

        var lookup = BuildLookupContext(context);
        var delta = new ManifestPlanDelta
        {
            ManifestModel = model,
            Root = root,
        };

        delta = dispatcher.Dispatch(context, lookup, delta);
        delta = addedTableExpansionService.Apply(context, lookup, delta);
        delta = alteredColumnExpansionService.Apply(context, lookup, delta);
        delta = primaryKeyForeignKeyExpansionService.Apply(context, lookup, delta);

        return delta.BuildResult();
    }

    private ManifestPlanningLookupContext BuildLookupContext(ManifestPlanningContext context)
    {
        return new ManifestPlanningLookupContext
        {
            SourceWorkspace = context.SourceWorkspace,
            LiveWorkspace = context.LiveWorkspace,
            SourceColumnsById = GetRecordIndex(context.SourceWorkspace, "TableColumn"),
            LiveColumnsById = GetRecordIndex(context.LiveWorkspace, "TableColumn"),
            SourceTablesById = GetRecordIndex(context.SourceWorkspace, "Table"),
            LiveTablesById = GetRecordIndex(context.LiveWorkspace, "Table"),
            SourceSchemasById = GetRecordIndex(context.SourceWorkspace, "Schema"),
            LiveSchemasById = GetRecordIndex(context.LiveWorkspace, "Schema"),
            SourcePrimaryKeysById = GetRecordIndex(context.SourceWorkspace, "PrimaryKey"),
            LivePrimaryKeysById = GetRecordIndex(context.LiveWorkspace, "PrimaryKey"),
            SourcePrimaryKeyColumnsByPrimaryKeyId = GetGroupedRecords(context.SourceWorkspace, "PrimaryKeyColumn", "PrimaryKeyId"),
            LivePrimaryKeyColumnsByPrimaryKeyId = GetGroupedRecords(context.LiveWorkspace, "PrimaryKeyColumn", "PrimaryKeyId"),
            SourceIndexesById = GetRecordIndex(context.SourceWorkspace, "Index"),
            LiveIndexesById = GetRecordIndex(context.LiveWorkspace, "Index"),
            SourceForeignKeysById = GetRecordIndex(context.SourceWorkspace, "ForeignKey"),
            LiveForeignKeysById = GetRecordIndex(context.LiveWorkspace, "ForeignKey"),
            SourceForeignKeysByTargetTableId = GetGroupedRecords(context.SourceWorkspace, "ForeignKey", "TargetTableId"),
            LiveForeignKeysByTargetTableId = GetGroupedRecords(context.LiveWorkspace, "ForeignKey", "TargetTableId"),
            SourceForeignKeyColumnsByForeignKeyId = GetGroupedRecords(context.SourceWorkspace, "ForeignKeyColumn", "ForeignKeyId"),
            LiveForeignKeyColumnsByForeignKeyId = GetGroupedRecords(context.LiveWorkspace, "ForeignKeyColumn", "ForeignKeyId"),
            SourceIndexColumnsByIndexId = GetGroupedRecords(context.SourceWorkspace, "IndexColumn", "IndexId"),
            LiveIndexColumnsByIndexId = GetGroupedRecords(context.LiveWorkspace, "IndexColumn", "IndexId"),
            SourceColumnDetailsByColumnId = GetGroupedRecords(context.SourceWorkspace, "TableColumnDataTypeDetail", "TableColumnId"),
            LiveColumnDetailsByColumnId = GetGroupedRecords(context.LiveWorkspace, "TableColumnDataTypeDetail", "TableColumnId"),
            SourcePrimaryKeyColumnsByColumnId = GetGroupedRecords(context.SourceWorkspace, "PrimaryKeyColumn", "TableColumnId"),
            LivePrimaryKeyColumnsByColumnId = GetGroupedRecords(context.LiveWorkspace, "PrimaryKeyColumn", "TableColumnId"),
            SourceForeignKeySourceColumnsByColumnId = GetGroupedRecords(context.SourceWorkspace, "ForeignKeyColumn", "SourceColumnId"),
            LiveForeignKeySourceColumnsByColumnId = GetGroupedRecords(context.LiveWorkspace, "ForeignKeyColumn", "SourceColumnId"),
            SourceForeignKeyTargetColumnsByColumnId = GetGroupedRecords(context.SourceWorkspace, "ForeignKeyColumn", "TargetColumnId"),
            LiveForeignKeyTargetColumnsByColumnId = GetGroupedRecords(context.LiveWorkspace, "ForeignKeyColumn", "TargetColumnId"),
            SourceIndexColumnsByColumnId = GetGroupedRecords(context.SourceWorkspace, "IndexColumn", "TableColumnId"),
            LiveIndexColumnsByColumnId = GetGroupedRecords(context.LiveWorkspace, "IndexColumn", "TableColumnId"),
            BlockerByColumnPairKey = tableColumnAlterAssessmentService.BuildColumnBlockerLookup(context.FeasibilityBlockers),
            BlockerByLiveColumnId = tableColumnAlterAssessmentService.BuildLiveColumnBlockerLookup(context.FeasibilityBlockers),
            ApprovalSet = MetaSqlDestructiveApprovalSet.From(context.DestructiveApprovals),
            PlannedAddedTableIds = context.Differences
                .Where(row => row.ObjectKind == MetaSqlObjectKind.Table && row.DifferenceKind == MetaSqlDifferenceKind.MissingInLive)
                .Select(row => row.SourceId)
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row!)
                .ToHashSet(StringComparer.Ordinal),
            PlannedAddedColumnIds = context.Differences
                .Where(row => row.ObjectKind == MetaSqlObjectKind.TableColumn && row.DifferenceKind == MetaSqlDifferenceKind.MissingInLive)
                .Select(row => row.SourceId)
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row!)
                .ToHashSet(StringComparer.Ordinal),
            PlannedDroppedForeignKeyIds = context.Differences
                .Where(row => row.ObjectKind == MetaSqlObjectKind.ForeignKey && row.DifferenceKind == MetaSqlDifferenceKind.ExtraInLive)
                .Select(row => row.LiveId)
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row!)
                .ToHashSet(StringComparer.Ordinal),
            PlannedAddedForeignKeyIds = context.Differences
                .Where(row => row.ObjectKind == MetaSqlObjectKind.ForeignKey && row.DifferenceKind == MetaSqlDifferenceKind.MissingInLive)
                .Select(row => row.SourceId)
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row => row!)
                .ToHashSet(StringComparer.Ordinal),
        };
    }

    private static Dictionary<string, GenericRecord> GetRecordIndex(Workspace workspace, string entityName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName)
            .ToDictionary(row => row.Id, StringComparer.Ordinal);
    }

    private static Dictionary<string, List<GenericRecord>> GetGroupedRecords(Workspace workspace, string entityName, string relationshipName)
    {
        return workspace.Instance.GetOrCreateEntityRecords(entityName)
            .GroupBy(row => row.RelationshipIds[relationshipName], StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(row => row.Id, StringComparer.Ordinal).ToList(),
                StringComparer.Ordinal);
    }
}
