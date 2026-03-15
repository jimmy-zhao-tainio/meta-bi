namespace MetaSql.Workflow;

public enum BlockerKind
{
    ManualRequired,
    Blocked
}

public sealed record Blocker(
    string Id,
    BlockerKind Kind,
    string ObjectName,
    IReadOnlyList<string> Reasons);
