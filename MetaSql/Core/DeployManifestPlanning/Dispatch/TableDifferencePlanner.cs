namespace MetaSql;

/// <summary>
/// Plans table-family differences into manifest actions/blocks.
/// </summary>
internal sealed class TableDifferencePlanner
{
    private readonly ManifestEntryFactory manifestEntryFactory;
    private readonly ManifestBlockFactory manifestBlockFactory;
    private readonly DestructiveApprovalAssessmentService destructiveApprovalAssessmentService;

    public TableDifferencePlanner(
        ManifestEntryFactory manifestEntryFactory,
        ManifestBlockFactory manifestBlockFactory,
        DestructiveApprovalAssessmentService destructiveApprovalAssessmentService)
    {
        this.manifestEntryFactory = manifestEntryFactory;
        this.manifestBlockFactory = manifestBlockFactory;
        this.destructiveApprovalAssessmentService = destructiveApprovalAssessmentService;
    }

    public void Plan(
        ManifestPlanningLookupContext lookup,
        ManifestPlanDelta delta,
        MetaSqlDifference difference)
    {
        switch (difference.DifferenceKind)
        {
            case MetaSqlDifferenceKind.MissingInLive:
                delta.AddCount += manifestEntryFactory.AddEntry(delta.ManifestModel, delta.Root, difference);
                break;
            case MetaSqlDifferenceKind.ExtraInLive:
            {
                var approval = destructiveApprovalAssessmentService.AssessDataDropTableApproval(
                    difference,
                    lookup.LiveTablesById,
                    lookup.LiveSchemasById,
                    lookup.ApprovalSet);
                if (approval.Approved)
                {
                    delta.DropCount += manifestEntryFactory.DropEntry(delta.ManifestModel, delta.Root, difference);
                }
                else
                {
                    delta.BlockCount += manifestBlockFactory.BlockEntry(delta.ManifestModel, delta.Root, difference, approval.Reason);
                }

                break;
            }
            case MetaSqlDifferenceKind.Different:
                delta.BlockCount += manifestBlockFactory.BlockEntry(delta.ManifestModel, delta.Root, difference);
                break;
            default:
                throw new InvalidOperationException($"Unsupported difference kind '{difference.DifferenceKind}'.");
        }
    }
}
