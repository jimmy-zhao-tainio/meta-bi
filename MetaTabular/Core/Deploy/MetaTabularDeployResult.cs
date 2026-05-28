namespace MetaTabular.Core.Deploy;

public sealed record MetaTabularDeployResult
{
    public required string WorkspacePath { get; init; }

    public required string Server { get; init; }

    public required string DatabaseName { get; init; }

    public required bool DropExisting { get; init; }

    public required bool Processed { get; init; }

    public required int TableCount { get; init; }

    public required int ColumnCount { get; init; }

    public required int MeasureCount { get; init; }

    public required int RelationshipCount { get; init; }
}
