namespace MetaSql.Workflow.Resolve;

public enum ResolverPresence
{
    Unknown = 0,
    No,
    Yes
}

public enum ResolverSubjectKind
{
    Table,
    Column,
    Index,
    Constraint,
    Mixed
}

public enum ResolverStateClass
{
    Unknown = 0,
    Persistent,
    Replaceable
}

public enum ResolverScenarioFamily
{
    AdditiveOnly = 0,
    LiveOnlyRemovalCandidate,
    OneToOneReplacementCandidate,
    OneToManySplitCandidate,
    ManyToOneMergeCandidate,
    SameIdentityShapeChange,
    RelocationCandidate,
    SupportingObjectOnlyChange,
    ReplaceableStructureChurn,
    ComplexComposite,
    Unknown
}

[Flags]
public enum ResolverRelationshipKinds
{
    None = 0,
    Index = 1 << 0,
    PrimaryKey = 1 << 1,
    UniqueKey = 1 << 2,
    ForeignKey = 1 << 3,
    CheckConstraint = 1 << 4,
    DefaultConstraint = 1 << 5,
    Other = 1 << 6
}

public enum ResolverHistoricalPattern
{
    Unknown = 0,
    NoneSeen,
    SimilarSeenBefore,
    ExactLikeSeenBefore
}

public sealed record ResolverScenarioEvidence
{
    // Logical only: if the owning live table has one or more rows, this is Yes.
    public required ResolverPresence DataPresent { get; init; }

    // Logical only: if known structural dependencies exist, this is Yes.
    public required ResolverPresence RelationshipsPresent { get; init; }
    public required ResolverRelationshipKinds RelationshipKinds { get; init; }

    // Ranking/explanation only. These never authorize behavior.
    public required bool TypeCompatible { get; init; }
    public required bool NullabilityCompatible { get; init; }
    public required int NameSimilarityPercent { get; init; }
    public required bool CrossObjectMovementSuspected { get; init; }
    public required ResolverHistoricalPattern HistoricalPattern { get; init; }
    public required int RelationshipCount { get; init; }
}

public sealed record ResolverScenario
{
    public required ResolverSubjectKind SubjectKind { get; init; }
    public required ResolverStateClass StateClass { get; init; }
    public required ResolverScenarioFamily Family { get; init; }
    public required string ObjectName { get; init; }
    public required int LiveOnlyCount { get; init; }
    public required int DesiredOnlyCount { get; init; }
    public required int SameIdentityChangedCount { get; init; }
    public required IReadOnlyList<string> LiveOnlyNames { get; init; }
    public required IReadOnlyList<string> DesiredOnlyNames { get; init; }
    public required IReadOnlyList<string> SameIdentityChangedNames { get; init; }
    public required ResolverScenarioEvidence Evidence { get; init; }
    public required IReadOnlyList<string> Reasons { get; init; }

    public void Validate()
    {
        Require(LiveOnlyNames.Count == LiveOnlyCount, "LiveOnlyNames count must match LiveOnlyCount.");
        Require(DesiredOnlyNames.Count == DesiredOnlyCount, "DesiredOnlyNames count must match DesiredOnlyCount.");
        Require(SameIdentityChangedNames.Count == SameIdentityChangedCount, "SameIdentityChangedNames count must match SameIdentityChangedCount.");

        switch (Family)
        {
            case ResolverScenarioFamily.AdditiveOnly:
                Require(LiveOnlyCount == 0, "AdditiveOnly requires no live-only members.");
                Require(DesiredOnlyCount > 0, "AdditiveOnly requires desired-only members.");
                Require(SameIdentityChangedCount == 0, "AdditiveOnly requires no same-identity shape changes.");
                break;

            case ResolverScenarioFamily.LiveOnlyRemovalCandidate:
                Require(LiveOnlyCount > 0, "LiveOnlyRemovalCandidate requires live-only members.");
                Require(DesiredOnlyCount == 0, "LiveOnlyRemovalCandidate requires no desired-only members.");
                Require(SameIdentityChangedCount == 0, "LiveOnlyRemovalCandidate requires no same-identity shape changes.");
                break;

            case ResolverScenarioFamily.OneToOneReplacementCandidate:
                Require(LiveOnlyCount == 1, "OneToOneReplacementCandidate requires exactly one live-only member.");
                Require(DesiredOnlyCount == 1, "OneToOneReplacementCandidate requires exactly one desired-only member.");
                Require(SameIdentityChangedCount == 0, "OneToOneReplacementCandidate requires no same-identity shape changes.");
                break;

            case ResolverScenarioFamily.OneToManySplitCandidate:
                Require(LiveOnlyCount == 1, "OneToManySplitCandidate requires exactly one live-only member.");
                Require(DesiredOnlyCount > 1, "OneToManySplitCandidate requires multiple desired-only members.");
                Require(SameIdentityChangedCount == 0, "OneToManySplitCandidate requires no same-identity shape changes.");
                break;

            case ResolverScenarioFamily.ManyToOneMergeCandidate:
                Require(LiveOnlyCount > 1, "ManyToOneMergeCandidate requires multiple live-only members.");
                Require(DesiredOnlyCount == 1, "ManyToOneMergeCandidate requires exactly one desired-only member.");
                Require(SameIdentityChangedCount == 0, "ManyToOneMergeCandidate requires no same-identity shape changes.");
                break;

            case ResolverScenarioFamily.SameIdentityShapeChange:
                Require(SameIdentityChangedCount > 0, "SameIdentityShapeChange requires same-identity shape changes.");
                Require(LiveOnlyCount == 0, "SameIdentityShapeChange requires no live-only members.");
                Require(DesiredOnlyCount == 0, "SameIdentityShapeChange requires no desired-only members.");
                break;

            case ResolverScenarioFamily.RelocationCandidate:
                Require(Evidence.CrossObjectMovementSuspected, "RelocationCandidate requires CrossObjectMovementSuspected=true.");
                break;

            case ResolverScenarioFamily.SupportingObjectOnlyChange:
                Require(SubjectKind is ResolverSubjectKind.Index or ResolverSubjectKind.Constraint or ResolverSubjectKind.Mixed,
                    "SupportingObjectOnlyChange should be index, constraint, or mixed.");
                break;

            case ResolverScenarioFamily.ReplaceableStructureChurn:
                Require(StateClass == ResolverStateClass.Replaceable, "ReplaceableStructureChurn requires StateClass=Replaceable.");
                break;

            case ResolverScenarioFamily.ComplexComposite:
            case ResolverScenarioFamily.Unknown:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(Family), Family, "Unknown resolver scenario family.");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}

public sealed record ResolverInterpretation(
    string Label,
    int Rank,
    IReadOnlyList<string> Reasons);
