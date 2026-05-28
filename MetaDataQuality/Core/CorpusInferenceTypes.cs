using MetaDataQuality;

namespace MetaDataQuality.Core;

internal sealed class RelationshipAggregate
{
    public RelationshipAggregate(
        string relationshipSignature,
        string canonicalSideAObjectName,
        string canonicalSideBObjectName)
    {
        RelationshipSignature = relationshipSignature;
        CanonicalSideAObjectName = canonicalSideAObjectName;
        CanonicalSideBObjectName = canonicalSideBObjectName;
    }

    public string RelationshipSignature { get; }

    public string CanonicalSideAObjectName { get; }

    public string CanonicalSideBObjectName { get; }

    public Dictionary<string, PatternAggregate> Patterns { get; } = new(StringComparer.Ordinal);

    public HashSet<string> OccurrenceIds { get; } = new(StringComparer.Ordinal);

    public HashSet<string> TransformScriptIds { get; } = new(StringComparer.Ordinal);

    public void Register(OccurrenceObservation observation)
    {
        OccurrenceIds.Add(observation.OccurrenceId);
        TransformScriptIds.Add(observation.TransformScriptId);
        if (!Patterns.TryGetValue(observation.CanonicalPatternSignature, out var pattern))
        {
            pattern = new PatternAggregate(observation.CanonicalPatternSignature, observation.CanonicalKeyParts);
            Patterns.Add(observation.CanonicalPatternSignature, pattern);
        }

        pattern.Register(observation);
    }
}

internal sealed class PatternAggregate
{
    public PatternAggregate(
        string canonicalPatternSignature,
        IReadOnlySet<string> canonicalKeyParts)
    {
        CanonicalPatternSignature = canonicalPatternSignature;
        KeyParts = canonicalKeyParts.ToHashSet(StringComparer.Ordinal);
        KeyPartCount = KeyParts.Count;
    }

    public string CanonicalPatternSignature { get; }

    public IReadOnlySet<string> KeyParts { get; }

    public int KeyPartCount { get; }

    public List<OccurrenceObservation> Observations { get; } = [];

    public HashSet<string> OccurrenceIds { get; } = new(StringComparer.Ordinal);

    public HashSet<string> TransformScriptIds { get; } = new(StringComparer.Ordinal);

    public void Register(OccurrenceObservation observation)
    {
        Observations.Add(observation);
        OccurrenceIds.Add(observation.OccurrenceId);
        TransformScriptIds.Add(observation.TransformScriptId);
    }
}

internal sealed record OccurrenceObservation(
    string OccurrenceId,
    string TransformScriptId,
    string TransformScriptName,
    string JoinPatternId,
    string QualifiedJoinType,
    string LeftObjectName,
    string RightObjectName,
    string CanonicalSideAObjectName,
    string CanonicalSideBObjectName,
    string RelationshipSignature,
    string CanonicalPatternSignature,
    string DirectionalSignature,
    IReadOnlySet<string> CanonicalKeyParts,
    IReadOnlyList<string> DirectionalPairs);

internal sealed record MaterializedRelationship(
    CorpusRelationship Row,
    RelationshipAggregate Aggregate,
    IReadOnlyList<MaterializedPattern> Patterns);

internal sealed record MaterializedPattern(
    CorpusRelationshipPattern Row,
    PatternAggregate Aggregate);

internal sealed class ColumnEquivalenceAggregate
{
    public ColumnEquivalenceAggregate(
        string canonicalUndirectedSignature,
        string canonicalSideAColumnName,
        string canonicalSideBColumnName)
    {
        CanonicalUndirectedSignature = canonicalUndirectedSignature;
        CanonicalSideAColumnName = canonicalSideAColumnName;
        CanonicalSideBColumnName = canonicalSideBColumnName;
    }

    public string CanonicalUndirectedSignature { get; }

    public string CanonicalSideAColumnName { get; }

    public string CanonicalSideBColumnName { get; }

    public List<ColumnEquivalenceObservation> Observations { get; } = [];

    public HashSet<string> OccurrenceIds { get; } = new(StringComparer.Ordinal);

    public HashSet<string> TransformScriptIds { get; } = new(StringComparer.Ordinal);

    public void Register(ColumnEquivalenceObservation observation)
    {
        Observations.Add(observation);
        OccurrenceIds.Add(observation.OccurrenceId);
        TransformScriptIds.Add(observation.TransformScriptId);
    }
}

internal sealed record ColumnEquivalenceObservation(
    string OccurrenceId,
    string TransformScriptId,
    string TransformScriptName,
    string RelationshipSignature);

internal sealed record MaterializedColumnEquivalence(
    CorpusColumnEquivalence Row,
    ColumnEquivalenceAggregate Aggregate);

internal readonly record struct DirectionCounts(
    int SideAToSideBCount,
    int SideBToSideACount)
{
    public int TotalCount => SideAToSideBCount + SideBToSideACount;
}
