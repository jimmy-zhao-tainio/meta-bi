using System.Globalization;

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
        var parts = SplitMultipartIdentifier(expression);
        if (parts.Length == 0)
        {
            return false;
        }

        var lastPart = parts[^1];
        var unquoted = UnquoteIdentifier(lastPart);
        if (string.IsNullOrWhiteSpace(unquoted))
        {
            return false;
        }

        columnName = unquoted;
        if (IsBracketQuoted(lastPart))
        {
            return true;
        }

        return columnName.All(static ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '@' || ch == '#' || ch == '$');
    }

    public static string[] SplitMultipartIdentifier(string value)
    {
        var parts = new List<string>();
        var start = 0;
        var inBracket = false;
        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];
            if (ch == '[')
            {
                inBracket = true;
                continue;
            }

            if (ch == ']')
            {
                inBracket = false;
                continue;
            }

            if (ch == '.' && !inBracket)
            {
                AddIdentifierPart(parts, value[start..i]);
                start = i + 1;
            }
        }

        AddIdentifierPart(parts, value[start..]);
        return parts.ToArray();
    }

    public static string UnquoteIdentifier(string value)
    {
        var trimmed = value.Trim();
        return IsBracketQuoted(trimmed)
            ? trimmed[1..^1].Replace("]]", "]", StringComparison.Ordinal)
            : trimmed;
    }

    public static bool IsBracketQuoted(string value)
    {
        var trimmed = value.Trim();
        return trimmed.Length >= 2 && trimmed[0] == '[' && trimmed[^1] == ']';
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

    private static void AddIdentifierPart(ICollection<string> parts, string value)
    {
        var trimmed = value.Trim();
        if (!string.IsNullOrWhiteSpace(trimmed))
        {
            parts.Add(trimmed);
        }
    }
}
