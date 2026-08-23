using MetaDataQuality;

namespace MetaDataQuality.Core;

public sealed class MetaDataQualityDiscoveryResult
{
    public required MetaDataQualityModel Model { get; init; }

    public required int TransformScriptCount { get; init; }

    public int AnalyzedTransformScriptCount { get; init; }

    public int BindingSkippedTransformScriptCount { get; init; }
}
