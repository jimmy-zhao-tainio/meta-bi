using MetaDataQuality;

namespace MetaDataQuality.Core;

internal static class CorpusObservationBuilder
{
    public static Dictionary<string, RelationshipAggregate> BuildRelationshipAggregates(
        IReadOnlyList<JoinPatternOccurrence> occurrences,
        IReadOnlyDictionary<string, JoinPattern> joinPatternById,
        IReadOnlyDictionary<string, JoinPatternKeyPart[]> keyPartsByPatternId,
        IReadOnlyDictionary<string, JoinPatternOccurrenceBaseTable[]> baseTablesByOccurrenceId)
    {
        var relationshipBySignature = new Dictionary<string, RelationshipAggregate>(StringComparer.Ordinal);
        foreach (var occurrence in occurrences
                     .OrderBy(item => item.TransformScriptName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(item => item.Id, StringComparer.Ordinal))
        {
            if (!joinPatternById.TryGetValue(occurrence.JoinPattern.Id, out var joinPattern))
            {
                continue;
            }

            if (!TryBuildOccurrenceObservation(
                    occurrence,
                    joinPattern,
                    keyPartsByPatternId,
                    baseTablesByOccurrenceId,
                    out var observation))
            {
                continue;
            }

            if (!relationshipBySignature.TryGetValue(observation.RelationshipSignature, out var relationship))
            {
                relationship = new RelationshipAggregate(
                    observation.RelationshipSignature,
                    observation.CanonicalSideAObjectName,
                    observation.CanonicalSideBObjectName);
                relationshipBySignature.Add(observation.RelationshipSignature, relationship);
            }

            relationship.Register(observation);
        }

        return relationshipBySignature;
    }

    private static bool TryBuildOccurrenceObservation(
        JoinPatternOccurrence occurrence,
        JoinPattern joinPattern,
        IReadOnlyDictionary<string, JoinPatternKeyPart[]> keyPartsByPatternId,
        IReadOnlyDictionary<string, JoinPatternOccurrenceBaseTable[]> baseTablesByOccurrenceId,
        out OccurrenceObservation observation)
    {
        observation = default!;
        if (!keyPartsByPatternId.TryGetValue(joinPattern.Id, out var keyParts) || keyParts.Length == 0)
        {
            return false;
        }

        if (!TryResolveSingleSideTableName(baseTablesByOccurrenceId, occurrence.Id, occurrence.FirstTableReferenceId, out var leftObjectName)
            || !TryResolveSingleSideTableName(baseTablesByOccurrenceId, occurrence.Id, occurrence.SecondTableReferenceId, out var rightObjectName))
        {
            return false;
        }

        var leftToRightPairs = new List<(string LeftColumn, string RightColumn)>();
        foreach (var keyPart in keyParts.OrderBy(item => CorpusInferenceNormalization.ParseOrdinalOrMax(item.Ordinal)))
        {
            if (!string.IsNullOrWhiteSpace(keyPart.FirstJoinInputColumnName)
                && !string.IsNullOrWhiteSpace(keyPart.SecondJoinInputColumnName))
            {
                leftToRightPairs.Add((keyPart.FirstJoinInputColumnName.Trim(), keyPart.SecondJoinInputColumnName.Trim()));
                continue;
            }

            var leftExpression = string.IsNullOrWhiteSpace(keyPart.FirstExpressionDisplay)
                ? keyPart.FirstExpressionId
                : keyPart.FirstExpressionDisplay;
            var rightExpression = string.IsNullOrWhiteSpace(keyPart.SecondExpressionDisplay)
                ? keyPart.SecondExpressionId
                : keyPart.SecondExpressionDisplay;
            if (!CorpusInferenceNormalization.TryParseColumnExpression(leftExpression, out var leftColumn)
                || !CorpusInferenceNormalization.TryParseColumnExpression(rightExpression, out var rightColumn))
            {
                continue;
            }

            leftToRightPairs.Add((leftColumn, rightColumn));
        }

        if (leftToRightPairs.Count == 0)
        {
            return false;
        }

        var sideA = leftObjectName;
        var sideB = rightObjectName;
        if (CorpusInferenceNormalization.CompareCanonical(sideA, sideB) > 0)
        {
            (sideA, sideB) = (sideB, sideA);
        }

        var canonicalPairs = leftToRightPairs
            .Select(pair =>
            {
                var leftNormalized = CorpusInferenceNormalization.NormalizeSignaturePart(pair.LeftColumn);
                var rightNormalized = CorpusInferenceNormalization.NormalizeSignaturePart(pair.RightColumn);
                if (sideA.Equals(sideB, StringComparison.OrdinalIgnoreCase))
                {
                    return StringComparer.Ordinal.Compare(leftNormalized, rightNormalized) <= 0
                        ? $"{leftNormalized}={rightNormalized}"
                        : $"{rightNormalized}={leftNormalized}";
                }

                if (leftObjectName.Equals(sideA, StringComparison.OrdinalIgnoreCase)
                    && rightObjectName.Equals(sideB, StringComparison.OrdinalIgnoreCase))
                {
                    return $"{leftNormalized}={rightNormalized}";
                }

                return $"{rightNormalized}={leftNormalized}";
            })
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static item => item, StringComparer.Ordinal)
            .ToArray();
        if (canonicalPairs.Length == 0)
        {
            return false;
        }

        var directionalPairs = leftToRightPairs
            .Select(pair => $"{CorpusInferenceNormalization.NormalizeSignaturePart(pair.LeftColumn)}={CorpusInferenceNormalization.NormalizeSignaturePart(pair.RightColumn)}")
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static item => item, StringComparer.Ordinal)
            .ToArray();

        var relationshipSignature = $"{CorpusInferenceNormalization.NormalizeSignaturePart(sideA)}|{CorpusInferenceNormalization.NormalizeSignaturePart(sideB)}";
        var patternSignature = string.Join("&", canonicalPairs);
        var directionalSignature = $"{leftObjectName}->{rightObjectName}|{string.Join("&", directionalPairs)}";

        observation = new OccurrenceObservation(
            occurrence.Id,
            occurrence.TransformScriptId,
            occurrence.TransformScriptName,
            joinPattern.Id,
            joinPattern.QualifiedJoinType ?? string.Empty,
            leftObjectName,
            rightObjectName,
            sideA,
            sideB,
            relationshipSignature,
            patternSignature,
            directionalSignature,
            canonicalPairs.ToHashSet(StringComparer.Ordinal),
            directionalPairs.ToArray());
        return true;
    }

    private static bool TryResolveSingleSideTableName(
        IReadOnlyDictionary<string, JoinPatternOccurrenceBaseTable[]> baseTablesByOccurrenceId,
        string occurrenceId,
        string? joinInputTableReferenceId,
        out string baseObjectName)
    {
        baseObjectName = string.Empty;
        if (string.IsNullOrWhiteSpace(occurrenceId)
            || string.IsNullOrWhiteSpace(joinInputTableReferenceId)
            || !baseTablesByOccurrenceId.TryGetValue(occurrenceId, out var rows))
        {
            return false;
        }

        var values = rows
            .Where(row => string.Equals(row.JoinInputTableReferenceId, joinInputTableReferenceId, StringComparison.Ordinal))
            .Select(static row => row.BaseObjectName)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (values.Length != 1)
        {
            return false;
        }

        baseObjectName = values[0];
        return true;
    }
}
