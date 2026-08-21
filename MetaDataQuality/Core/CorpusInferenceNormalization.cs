using System.Globalization;
using MetaTransformScript;

namespace MetaDataQuality.Core;

internal static class CorpusInferenceNormalization
{
    public static bool IsStrictSubset(
        IReadOnlySet<string> candidateSubset,
        IReadOnlySet<string> superset)
    {
        if (candidateSubset.Count >= superset.Count)
        {
            return false;
        }

        foreach (var value in candidateSubset)
        {
            if (!superset.Contains(value))
            {
                return false;
            }
        }

        return true;
    }

    public static bool TryParseColumnExpression(string expression, out string columnName)
    {
        columnName = string.Empty;
        if (!TransformScriptSqlIdentifier.TryParseParts(expression, out var parts)
            || parts.Count == 0)
        {
            return false;
        }

        columnName = parts[^1];
        return !string.IsNullOrWhiteSpace(columnName);
    }

    public static int CompareCanonical(string first, string second)
    {
        return StringComparer.OrdinalIgnoreCase.Compare(first, second);
    }

    public static string NormalizeSignaturePart(string value) =>
        value.Trim().ToLowerInvariant();

    public static string FormatRatio(double value) =>
        value.ToString("0.####", CultureInfo.InvariantCulture);

    public static int ParseOrdinalOrMax(string ordinal)
    {
        return int.TryParse(ordinal, out var parsed)
            ? parsed
            : int.MaxValue;
    }
}
