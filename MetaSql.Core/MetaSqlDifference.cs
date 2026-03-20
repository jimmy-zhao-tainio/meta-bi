namespace MetaSql;

public enum MetaSqlObjectKind
{
    Table = 0,
    TableColumn,
    PrimaryKey,
    ForeignKey,
    Index,
}

public enum MetaSqlDifferenceKind
{
    MissingInLive = 0,
    ExtraInLive,
    Different,
}

public sealed record MetaSqlDifference
{
    public required MetaSqlObjectKind ObjectKind { get; init; }
    public required MetaSqlDifferenceKind DifferenceKind { get; init; }
    public required string DisplayName { get; init; }
    public string? ScopeDisplayName { get; init; }
    public string? SourceId { get; init; }
    public string? LiveId { get; init; }
}
