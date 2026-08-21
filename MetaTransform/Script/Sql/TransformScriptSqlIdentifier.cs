using System.Text;

namespace MetaTransformScript;

public static class TransformScriptSqlIdentifier
{
    public static string FormatParts(IEnumerable<string> parts) =>
        string.Join(".", parts.Select(FormatPart));

    public static bool TryParseParts(string? sqlIdentifier, out IReadOnlyList<string> parts)
    {
        parts = [];
        if (string.IsNullOrWhiteSpace(sqlIdentifier))
        {
            return false;
        }

        var parsed = new List<string>();
        var buffer = new StringBuilder();
        var quoteMode = QuoteMode.None;
        var quoteClosed = false;
        var partWasQuoted = false;

        for (var index = 0; index < sqlIdentifier.Length; index++)
        {
            var character = sqlIdentifier[index];

            if (quoteMode == QuoteMode.SquareBracket)
            {
                if (character == ']')
                {
                    if (index + 1 < sqlIdentifier.Length && sqlIdentifier[index + 1] == ']')
                    {
                        buffer.Append(']');
                        index++;
                    }
                    else
                    {
                        quoteMode = QuoteMode.None;
                        quoteClosed = true;
                    }
                }
                else
                {
                    buffer.Append(character);
                }

                continue;
            }

            if (quoteMode == QuoteMode.DoubleQuote)
            {
                if (character == '"')
                {
                    if (index + 1 < sqlIdentifier.Length && sqlIdentifier[index + 1] == '"')
                    {
                        buffer.Append('"');
                        index++;
                    }
                    else
                    {
                        quoteMode = QuoteMode.None;
                        quoteClosed = true;
                    }
                }
                else
                {
                    buffer.Append(character);
                }

                continue;
            }

            if (quoteClosed)
            {
                if (char.IsWhiteSpace(character))
                {
                    continue;
                }

                if (character == '.')
                {
                    if (!TryAddPart(parsed, buffer, partWasQuoted))
                    {
                        return false;
                    }

                    quoteClosed = false;
                    partWasQuoted = false;
                    continue;
                }

                return false;
            }

            if (character == '.')
            {
                if (!TryAddPart(parsed, buffer, partWasQuoted))
                {
                    return false;
                }

                partWasQuoted = false;
                continue;
            }

            if (character is '[' or '"')
            {
                if (buffer.ToString().Trim().Length != 0)
                {
                    return false;
                }

                buffer.Clear();
                quoteMode = character == '[' ? QuoteMode.SquareBracket : QuoteMode.DoubleQuote;
                partWasQuoted = true;
                continue;
            }

            buffer.Append(character);
        }

        if (quoteMode != QuoteMode.None || !TryAddPart(parsed, buffer, partWasQuoted))
        {
            return false;
        }

        parts = parsed;
        return true;
    }

    private static string FormatPart(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Length > 0 &&
            (char.IsLetter(trimmed[0]) || trimmed[0] == '_') &&
            trimmed.All(static character => char.IsLetterOrDigit(character) || character == '_'))
        {
            return trimmed;
        }

        return "[" + trimmed.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }

    private static bool TryAddPart(List<string> parts, StringBuilder buffer, bool partWasQuoted)
    {
        var part = partWasQuoted ? buffer.ToString() : buffer.ToString().Trim();
        buffer.Clear();
        if (part.Length == 0)
        {
            return false;
        }

        parts.Add(part);
        return true;
    }

    private enum QuoteMode
    {
        None,
        SquareBracket,
        DoubleQuote,
    }
}
