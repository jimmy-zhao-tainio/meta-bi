using Meta.Core.Domain;

namespace MetaSql;

/// <summary>
/// Orchestrates deploy-manifest planning and keeps policy/execution boundaries explicit.
/// </summary>
public sealed class MetaSqlDeployManifestService
{
    private readonly ManifestPlanningEngine engine;

    public MetaSqlDeployManifestService()
    {
        var entryFactory = new ManifestEntryFactory();
        var blockFactory = new ManifestBlockFactory();

        var destructiveApprovalAssessmentService = new DestructiveApprovalAssessmentService();
        var tableColumnAlterAssessmentService = new TableColumnAlterAssessmentService();
        var primaryKeyReplacementAssessmentService = new PrimaryKeyReplacementAssessmentService();
        var foreignKeyReplacementAssessmentService = new ForeignKeyReplacementAssessmentService();
        var indexReplacementAssessmentService = new IndexReplacementAssessmentService();

        var dispatcher = new DifferencePlanDispatcher(
            new TableDifferencePlanner(entryFactory, blockFactory, destructiveApprovalAssessmentService),
            new TableColumnDifferencePlanner(entryFactory, blockFactory, tableColumnAlterAssessmentService, destructiveApprovalAssessmentService),
            new PrimaryKeyDifferencePlanner(entryFactory, blockFactory, primaryKeyReplacementAssessmentService),
            new ForeignKeyDifferencePlanner(entryFactory, blockFactory, foreignKeyReplacementAssessmentService),
            new IndexDifferencePlanner(entryFactory, blockFactory, indexReplacementAssessmentService));
        var addedTableExpansionService = new AddedTableDependencyExpansionService(entryFactory);
        var alteredColumnExpansionService = new AlteredColumnConstraintExpansionService(
            entryFactory,
            blockFactory,
            primaryKeyReplacementAssessmentService,
            foreignKeyReplacementAssessmentService,
            indexReplacementAssessmentService);
        var primaryKeyForeignKeyExpansionService = new PrimaryKeyReplacementForeignKeyExpansionService(
            entryFactory,
            blockFactory);

        engine = new ManifestPlanningEngine(
            tableColumnAlterAssessmentService,
            dispatcher,
            addedTableExpansionService,
            alteredColumnExpansionService,
            primaryKeyForeignKeyExpansionService);
    }

    public MetaSqlDeployManifestBuildResult BuildManifest(
        Workspace sourceWorkspace,
        Workspace liveWorkspace,
        IReadOnlyList<MetaSqlDifference> differences,
        string manifestName,
        string? targetDescription,
        IReadOnlyList<MetaSqlDifferenceBlocker>? feasibilityBlockers = null,
        IReadOnlyList<MetaSqlDestructiveApproval>? destructiveApprovals = null)
    {
        return engine.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            differences,
            manifestName,
            targetDescription,
            feasibilityBlockers,
            destructiveApprovals);
    }
}
