namespace MetaDataQuality.Core;

internal static class CorpusColumnEquivalenceBuilder
{
    public static Dictionary<string, ColumnEquivalenceAggregate> BuildColumnEquivalenceAggregates(
        IReadOnlyDictionary<string, RelationshipAggregate> relationshipBySignature)
    {
        var equivalenceBySignature = new Dictionary<string, ColumnEquivalenceAggregate>(StringComparer.Ordinal);
        foreach (var relationship in relationshipBySignature.Values
                     .OrderBy(item => item.RelationshipSignature, StringComparer.Ordinal))
        {
            foreach (var pattern in relationship.Patterns.Values
                         .OrderBy(item => item.CanonicalPatternSignature, StringComparer.Ordinal))
            {
                foreach (var observation in pattern.Observations
                             .OrderBy(static item => item.TransformScriptName, StringComparer.OrdinalIgnoreCase)
                             .ThenBy(static item => item.OccurrenceId, StringComparer.Ordinal))
                {
                    if (string.IsNullOrWhiteSpace(observation.LeftObjectName)
                        || string.IsNullOrWhiteSpace(observation.RightObjectName))
                    {
                        continue;
                    }

                    var seenSignatures = new HashSet<string>(StringComparer.Ordinal);
                    foreach (var pair in observation.DirectionalPairs)
                    {
                        if (!TryParseDirectionalPair(pair, out var leftColumn, out var rightColumn))
                        {
                            continue;
                        }

                        var leftNode = BuildColumnNode(observation.LeftObjectName, leftColumn);
                        var rightNode = BuildColumnNode(observation.RightObjectName, rightColumn);
                        if (string.IsNullOrWhiteSpace(leftNode) || string.IsNullOrWhiteSpace(rightNode))
                        {
                            continue;
                        }

                        var sideA = leftNode;
                        var sideB = rightNode;
                        if (CorpusInferenceNormalization.CompareCanonical(sideA, sideB) > 0)
                        {
                            (sideA, sideB) = (sideB, sideA);
                        }

                        var signature =
                            $"{CorpusInferenceNormalization.NormalizeSignaturePart(sideA)}|{CorpusInferenceNormalization.NormalizeSignaturePart(sideB)}";
                        if (!seenSignatures.Add(signature))
                        {
                            continue;
                        }

                        if (!equivalenceBySignature.TryGetValue(signature, out var aggregate))
                        {
                            aggregate = new ColumnEquivalenceAggregate(signature, sideA, sideB);
                            equivalenceBySignature.Add(signature, aggregate);
                        }

                        aggregate.Register(new ColumnEquivalenceObservation(
                            observation.OccurrenceId,
                            observation.TransformScriptId,
                            observation.TransformScriptName,
                            observation.RelationshipSignature));
                    }
                }
            }
        }

        return equivalenceBySignature;
    }

    private static string BuildColumnNode(string objectName, string columnName)
    {
        var objectPart = CorpusInferenceNormalization.NormalizeSignaturePart(objectName);
        var columnPart = CorpusInferenceNormalization.NormalizeSignaturePart(columnName);
        return string.IsNullOrWhiteSpace(objectPart) || string.IsNullOrWhiteSpace(columnPart)
            ? string.Empty
            : $"{objectPart}.{columnPart}";
    }

    private static bool TryParseDirectionalPair(string pair, out string leftColumn, out string rightColumn)
    {
        leftColumn = string.Empty;
        rightColumn = string.Empty;
        if (string.IsNullOrWhiteSpace(pair))
        {
            return false;
        }

        var separatorIndex = pair.IndexOf('=', StringComparison.Ordinal);
        if (separatorIndex <= 0 || separatorIndex >= pair.Length - 1)
        {
            return false;
        }

        leftColumn = pair[..separatorIndex].Trim();
        rightColumn = pair[(separatorIndex + 1)..].Trim();
        return !string.IsNullOrWhiteSpace(leftColumn) && !string.IsNullOrWhiteSpace(rightColumn);
    }
}
