namespace MetaSql;

public sealed record MetaSqlDeployRequest
{
    public required string ManifestWorkspacePath { get; init; }
    public required string SourceWorkspacePath { get; init; }
    public required string ConnectionString { get; init; }
}
