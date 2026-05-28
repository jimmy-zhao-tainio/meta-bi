using Meta.Core.Domain;

namespace MetaSql;

/// <summary>
/// Immutable planning inputs used across manifest-planning components.
/// </summary>
public sealed record ManifestPlanningContext
{
    public required Workspace SourceWorkspace { get; init; }
    public required Workspace LiveWorkspace { get; init; }
    public required MetaSqlLiveDatabasePresence LiveDatabasePresence { get; init; }
    public required IReadOnlyList<MetaSqlDifference> Differences { get; init; }
    public required string ManifestName { get; init; }
    public required string? TargetDescription { get; init; }
    public required IReadOnlyList<MetaSqlDifferenceBlocker>? FeasibilityBlockers { get; init; }
    public required IReadOnlyList<MetaSqlDestructiveApproval>? DestructiveApprovals { get; init; }
}
