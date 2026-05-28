using MetaDataQuality;

namespace MetaDataQuality.Core;

public sealed class MetaDataQualityDiscoveryResult
{
    public required MetaDataQualityModel Model { get; init; }

    public required int TransformScriptCount { get; init; }
}
