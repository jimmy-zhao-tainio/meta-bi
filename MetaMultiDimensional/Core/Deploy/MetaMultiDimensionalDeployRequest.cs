namespace MetaMultiDimensional.Core.Deploy;

public sealed record MetaMultiDimensionalDeployRequest
{
    public required string WorkspacePath { get; init; }

    public required string Server { get; init; }

    public string? DatabaseName { get; init; }

    public bool DropExisting { get; init; }

    public bool Process { get; init; } = true;
}
