namespace MetaTabular.Core.Deploy;

public sealed record MetaTabularDeployRequest
{
    public required string WorkspacePath { get; init; }

    public required string Server { get; init; }

    public string? DatabaseName { get; init; }

    public bool DropExisting { get; init; }

    public bool Process { get; init; } = true;
}
