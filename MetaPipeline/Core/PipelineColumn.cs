namespace MetaPipeline;

public sealed record PipelineColumn(
    string Name,
    int Ordinal,
    string? SourceMetaDataTypeId = null,
    string? TargetMetaDataTypeId = null);
