namespace MetaSql;

public sealed record MetaSqlDeployResult
{
    public required int AppliedAddCount { get; init; }
    public required int AppliedDropCount { get; init; }
    public required int AppliedAlterCount { get; init; }
    public required int AppliedTruncateCount { get; init; }
    public required int AppliedReplaceCount { get; init; }
    public required int ExecutedStatementCount { get; init; }
}
