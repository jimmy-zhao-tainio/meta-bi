namespace MetaTabular.Core.Deploy;

public sealed class MetaTabularProcessRequest
{
    public string Server { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
    public string RefreshType { get; init; } = "Full";
    public string? TableName { get; init; }
    public string? PartitionName { get; init; }
}
