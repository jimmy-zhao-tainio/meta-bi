namespace MetaDataQuality.Core;

public sealed class CorpusInferenceOptions
{
    public static readonly CorpusInferenceOptions Conservative = new();

    public int MinTablePairOccurrenceCount { get; init; } = 5;

    public int MinTablePairTransformCount { get; init; } = 3;

    public double DominantPatternMinRatio { get; init; } = 0.80;

    public int DominantPatternMinOccurrenceCount { get; init; } = 4;

    public double MinorityPatternMaxRatio { get; init; } = 0.20;

    public int MinRelationshipOccurrenceCount { get; init; } = 8;

    public int MinRelationshipTransformCount { get; init; } = 4;

    public double MinConsensusRatio { get; init; } = 0.85;

    public int MinDominantPatternOccurrenceCount { get; init; } = 6;

    public int MinLookupSideOccurrenceCount { get; init; } = 8;

    public int MinLookupSideTransformCount { get; init; } = 4;

    public double MinLookupSideConsistencyRatio { get; init; } = 0.85;

    public int MinKeyPartOccurrenceCount { get; init; } = 6;

    public int MinColumnAnchorOccurrenceCount { get; init; } = 8;

    public int MinColumnAnchorTransformCount { get; init; } = 4;

    public double DominantColumnEquivalenceMinRatio { get; init; } = 0.80;

    public int DominantColumnEquivalenceMinOccurrenceCount { get; init; } = 6;

    public double MinorityColumnEquivalenceMaxRatio { get; init; } = 0.20;

    public int MinCommonFilterOccurrenceCount { get; init; } = 6;

    public int MinCommonFilterTransformCount { get; init; } = 4;

    public double MinCommonFilterConsensusRatio { get; init; } = 0.85;

    public double MissingCommonFilterOutlierMaxRatio { get; init; } = 0.15;

    public int MinPatternOccurrenceCount { get; init; } = 8;

    public int MinPatternTransformCount { get; init; } = 4;

    public double DominantOptionalityMinRatio { get; init; } = 0.85;

    public int DominantOptionalityMinOccurrenceCount { get; init; } = 6;

    public double OutlierOptionalityMaxRatio { get; init; } = 0.15;

    public int OutlierOptionalityMinOccurrenceCount { get; init; } = 1;

    public int MinFanoutSignalOccurrenceCount { get; init; } = 8;

    public int MinFanoutSignalTransformCount { get; init; } = 4;

    public double MinFanoutSignalRatio { get; init; } = 0.70;

    public int MinOutputDuplicateSignalOccurrenceCount { get; init; } = 8;

    public int MinOutputDuplicateSignalTransformCount { get; init; } = 4;

    public double MinOutputDuplicateSignalRatio { get; init; } = 0.70;

    public int MinMediumConfidenceDistinctTransformCount { get; init; } = 4;

    public double MinMediumConfidenceConsensusRatio { get; init; } = 0.70;

    public int MinHighConfidenceDistinctTransformCount { get; init; } = 8;

    public double MinHighConfidenceConsensusRatio { get; init; } = 0.85;
}
