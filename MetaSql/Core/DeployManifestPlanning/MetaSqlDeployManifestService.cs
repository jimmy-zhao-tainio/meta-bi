using Meta.Operations.Domain;
using MetaDataType;
using MetaDataType.Core;
using MetaSql.Extractors.SqlServer;

namespace MetaSql;

/// <summary>
/// Orchestrates deploy-manifest planning and keeps policy/execution boundaries explicit.
/// </summary>
public sealed class MetaSqlDeployManifestService
{
    private readonly ManifestPlanningEngine engine;
    private readonly SqlServerDataTypeCatalog types;

    public MetaSqlDeployManifestService(MetaDataTypeModel? catalog = null)
    {
        types = new SqlServerDataTypeCatalog(catalog ?? MetaDataTypeWorkspaceProvider.GetDefaultWorkspace());
        var entryFactory = new ManifestEntryFactory();
        var blockFactory = new ManifestBlockFactory();

        var destructiveApprovalAssessmentService = new DestructiveApprovalAssessmentService();
        var tableColumnAlterAssessmentService = new TableColumnAlterAssessmentService(types);
        var primaryKeyReplacementAssessmentService = new PrimaryKeyReplacementAssessmentService();
        var foreignKeyReplacementAssessmentService = new ForeignKeyReplacementAssessmentService();
        var indexReplacementAssessmentService = new IndexReplacementAssessmentService();

        var dispatcher = new DifferencePlanDispatcher(
            new TableDifferencePlanner(entryFactory, blockFactory, destructiveApprovalAssessmentService),
            new TableColumnDifferencePlanner(entryFactory, blockFactory, tableColumnAlterAssessmentService, destructiveApprovalAssessmentService),
            new PrimaryKeyDifferencePlanner(entryFactory, blockFactory, primaryKeyReplacementAssessmentService),
            new ForeignKeyDifferencePlanner(entryFactory, blockFactory, foreignKeyReplacementAssessmentService),
            new IndexDifferencePlanner(entryFactory, blockFactory, indexReplacementAssessmentService),
            new SqlModuleDifferencePlanner(entryFactory, blockFactory));
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
        InMemoryWorkspace sourceWorkspace,
        InMemoryWorkspace liveWorkspace,
        MetaSqlLiveDatabasePresence liveDatabasePresence,
        IReadOnlyList<MetaSqlDifference> differences,
        string manifestName,
        string? targetDescription,
        IReadOnlyList<MetaSqlDifferenceBlocker>? feasibilityBlockers = null,
        IReadOnlyList<MetaSqlDestructiveApproval>? destructiveApprovals = null)
    {
        types.ValidateColumns(sourceWorkspace, "Source workspace");
        types.ValidateColumns(liveWorkspace, "Live workspace");
        return engine.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            types,
            liveDatabasePresence,
            differences,
            manifestName,
            targetDescription,
            feasibilityBlockers,
            destructiveApprovals);
    }
}
