namespace MetaTabular.Core.Deploy;

public sealed class MetaTabularProcessResult
{
    public string Server { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
    public string RefreshType { get; init; } = string.Empty;
    public string TargetKind { get; init; } = string.Empty;
    public string? TableName { get; init; }
    public string? PartitionName { get; init; }
}
