namespace MetaSql;

/// <summary>
/// Plans table-column-family differences into manifest actions/blocks.
/// </summary>
internal sealed class TableColumnDifferencePlanner
{
    private readonly ManifestEntryFactory manifestEntryFactory;
    private readonly ManifestBlockFactory manifestBlockFactory;
    private readonly TableColumnAlterAssessmentService tableColumnAlterAssessmentService;
    private readonly DestructiveApprovalAssessmentService destructiveApprovalAssessmentService;

    public TableColumnDifferencePlanner(
        ManifestEntryFactory manifestEntryFactory,
        ManifestBlockFactory manifestBlockFactory,
        TableColumnAlterAssessmentService tableColumnAlterAssessmentService,
        DestructiveApprovalAssessmentService destructiveApprovalAssessmentService)
    {
        this.manifestEntryFactory = manifestEntryFactory;
        this.manifestBlockFactory = manifestBlockFactory;
        this.tableColumnAlterAssessmentService = tableColumnAlterAssessmentService;
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
                var liveId = difference.LiveId ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(liveId) &&
                    lookup.BlockerByLiveColumnId.TryGetValue(liveId, out var blockers) &&
                    blockers.Count > 0)
                {
                    foreach (var blocker in blockers)
                    {
                        delta.BlockCount += manifestBlockFactory.BlockEntry(delta.ManifestModel, delta.Root, difference, blocker.Reason);
                    }

                    break;
                }

                var approval = destructiveApprovalAssessmentService.AssessDataDropColumnApproval(
                    difference,
                    lookup.LiveColumnsById,
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
            {
                var assessment = tableColumnAlterAssessmentService.Assess(difference, lookup);
                if (!assessment.Executable)
                {
                    delta.BlockCount += manifestBlockFactory.BlockEntry(delta.ManifestModel, delta.Root, difference, assessment.Reason);
                    break;
                }

                if (assessment.RequiresDataTruncation)
                {
                    delta.TruncateCount += manifestEntryFactory.TruncateEntry(delta.ManifestModel, delta.Root, difference);
                }

                delta.AlterCount += manifestEntryFactory.AlterEntry(delta.ManifestModel, delta.Root, difference);
                break;
            }
            default:
                throw new InvalidOperationException($"Unsupported difference kind '{difference.DifferenceKind}'.");
        }
    }
}
