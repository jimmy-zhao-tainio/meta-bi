namespace MetaSql;

internal static class SqlServerDefaultExpressionComparison
{
    public static bool AreEquivalent(string? left, string? right)
    {
        return string.Equals(
            Normalize(left),
            Normalize(right),
            StringComparison.Ordinal);
    }

    private static string Normalize(string? expressionSql)
    {
        if (string.IsNullOrWhiteSpace(expressionSql))
        {
            return string.Empty;
        }

        var current = expressionSql.Trim();
        while (HasSingleOuterParenthesisPair(current))
        {
            current = current[1..^1].Trim();
        }

        return NormalizeTokens(current);
    }

    private static string NormalizeTokens(string value)
    {
        var builder = new System.Text.StringBuilder(value.Length);
        var inString = false;

        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];
            if (inString)
            {
                builder.Append(ch);
                if (ch == '\'' && i + 1 < value.Length && value[i + 1] == '\'')
                {
                    builder.Append(value[++i]);
                }
                else if (ch == '\'')
                {
                    inString = false;
                }

                continue;
            }

            if (char.IsWhiteSpace(ch))
            {
                continue;
            }

            if (ch == '\'')
            {
                inString = true;
                builder.Append(ch);
                continue;
            }

            if (ch == '[')
            {
                var close = value.IndexOf(']', i + 1);
                if (close > i && IsSimpleBracketedIdentifier(value, i + 1, close))
                {
                    builder.Append(value.AsSpan(i + 1, close - i - 1).ToString().ToLowerInvariant());
                    i = close;
                    continue;
                }
            }

            builder.Append(char.ToLowerInvariant(ch));
        }

        return builder.ToString();
    }

    private static bool IsSimpleBracketedIdentifier(string value, int start, int endExclusive)
    {
        for (var i = start; i < endExclusive; i++)
        {
            var ch = value[i];
            if (!char.IsLetterOrDigit(ch) && ch != '_')
            {
                return false;
            }
        }

        return endExclusive > start;
    }

    private static bool HasSingleOuterParenthesisPair(string value)
    {
        if (value.Length < 2 || value[0] != '(' || value[^1] != ')')
        {
            return false;
        }

        var depth = 0;
        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];
            if (ch == '(')
            {
                depth++;
            }
            else if (ch == ')')
            {
                depth--;
                if (depth == 0 && i < value.Length - 1)
                {
                    return false;
                }
            }

            if (depth < 0)
            {
                return false;
            }
        }

        return depth == 0;
    }
}
