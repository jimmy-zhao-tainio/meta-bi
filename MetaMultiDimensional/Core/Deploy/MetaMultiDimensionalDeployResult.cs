namespace MetaMultiDimensional.Core.Deploy;

public sealed record MetaMultiDimensionalDeployResult
{
    public required string WorkspacePath { get; init; }

    public required string Server { get; init; }

    public required string DatabaseName { get; init; }

    public required bool DropExisting { get; init; }

    public required bool Processed { get; init; }

    public required int CubeCount { get; init; }

    public required int DimensionCount { get; init; }

    public required int MeasureGroupCount { get; init; }

    public required int MeasureCount { get; init; }
}
