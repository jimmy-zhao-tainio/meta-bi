namespace MetaSql;

/// <summary>
/// Plans foreign-key-family differences into manifest actions/blocks.
/// </summary>
internal sealed class ForeignKeyDifferencePlanner
{
    private readonly ManifestEntryFactory manifestEntryFactory;
    private readonly ManifestBlockFactory manifestBlockFactory;
    private readonly ForeignKeyReplacementAssessmentService foreignKeyReplacementAssessmentService;

    public ForeignKeyDifferencePlanner(
        ManifestEntryFactory manifestEntryFactory,
        ManifestBlockFactory manifestBlockFactory,
        ForeignKeyReplacementAssessmentService foreignKeyReplacementAssessmentService)
    {
        this.manifestEntryFactory = manifestEntryFactory;
        this.manifestBlockFactory = manifestBlockFactory;
        this.foreignKeyReplacementAssessmentService = foreignKeyReplacementAssessmentService;
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
                var assessment = foreignKeyReplacementAssessmentService.Assess(difference, lookup);
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
