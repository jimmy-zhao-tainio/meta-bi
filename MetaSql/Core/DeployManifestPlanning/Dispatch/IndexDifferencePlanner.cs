namespace MetaSql;

/// <summary>
/// Plans index-family differences into manifest actions/blocks.
/// </summary>
internal sealed class IndexDifferencePlanner
{
    private readonly ManifestEntryFactory manifestEntryFactory;
    private readonly ManifestBlockFactory manifestBlockFactory;
    private readonly IndexReplacementAssessmentService indexReplacementAssessmentService;

    public IndexDifferencePlanner(
        ManifestEntryFactory manifestEntryFactory,
        ManifestBlockFactory manifestBlockFactory,
        IndexReplacementAssessmentService indexReplacementAssessmentService)
    {
        this.manifestEntryFactory = manifestEntryFactory;
        this.manifestBlockFactory = manifestBlockFactory;
        this.indexReplacementAssessmentService = indexReplacementAssessmentService;
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
                delta.DropCount += manifestEntryFactory.DropEntry(delta.ManifestModel, delta.Root, difference);
                break;
            case MetaSqlDifferenceKind.Different:
            {
                var assessment = indexReplacementAssessmentService.Assess(difference, lookup);
                if (assessment.Executable)
                {
                    delta.ReplaceCount += manifestEntryFactory.ReplaceEntry(delta.ManifestModel, delta.Root, difference);
                }
                else
                {
                    delta.BlockCount += manifestBlockFactory.BlockEntry(delta.ManifestModel, delta.Root, difference, assessment.Reason);
                }

                break;
            }
            default:
                throw new InvalidOperationException($"Unsupported difference kind '{difference.DifferenceKind}'.");
        }
    }
}
