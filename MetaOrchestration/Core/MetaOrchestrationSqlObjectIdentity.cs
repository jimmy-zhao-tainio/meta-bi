using System.Text;

namespace MetaOrchestration.Core;

internal static class MetaOrchestrationSqlObjectIdentity
{
    private const int MaximumPartCount = 4;

    public static string NormalizeKey(string sqlIdentifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlIdentifier);

        return string.Join(".", ParseParts(sqlIdentifier).Select(CanonicalizePart));
    }

    private static IReadOnlyList<string> ParseParts(string sqlIdentifier)
    {
        var parts = new List<string>();
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
                    AddPart(parts, buffer, partWasQuoted, sqlIdentifier);
                    quoteClosed = false;
                    partWasQuoted = false;
                    continue;
                }

                throw InvalidIdentifier(sqlIdentifier, "contains characters after a quoted part");
            }

            if (character == '.')
            {
                AddPart(parts, buffer, partWasQuoted, sqlIdentifier);
                partWasQuoted = false;
                continue;
            }

            if (character is '[' or '"')
            {
                if (buffer.ToString().Trim().Length != 0)
                {
                    throw InvalidIdentifier(sqlIdentifier, "contains a quote delimiter inside an unquoted part");
                }

                buffer.Clear();
                quoteMode = character == '[' ? QuoteMode.SquareBracket : QuoteMode.DoubleQuote;
                partWasQuoted = true;
                continue;
            }

            buffer.Append(character);
        }

        if (quoteMode != QuoteMode.None)
        {
            throw InvalidIdentifier(sqlIdentifier, "contains an unterminated quoted part");
        }

        AddPart(parts, buffer, partWasQuoted, sqlIdentifier);
        if (parts.Count > MaximumPartCount)
        {
            throw InvalidIdentifier(sqlIdentifier, $"uses {parts.Count} parts; expected between one and {MaximumPartCount}");
        }

        return parts;
    }

    private static void AddPart(
        List<string> parts,
        StringBuilder buffer,
        bool partWasQuoted,
        string sqlIdentifier)
    {
        var part = partWasQuoted ? buffer.ToString() : buffer.ToString().Trim();
        buffer.Clear();
        if (part.Length == 0)
        {
            throw InvalidIdentifier(sqlIdentifier, "contains an empty part");
        }

        parts.Add(part);
    }

    private static string CanonicalizePart(string part)
    {
        var normalized = part.ToUpperInvariant();
        return IsOrdinaryIdentifier(normalized)
            ? normalized
            : "[" + normalized.Replace("]", "]]", StringComparison.Ordinal) + "]";
    }

    private static bool IsOrdinaryIdentifier(string part)
    {
        if (part.Length == 0 || !(char.IsLetter(part[0]) || part[0] is '_' or '@' or '#'))
        {
            return false;
        }

        return part.Skip(1).All(static character =>
            char.IsLetterOrDigit(character) || character is '_' or '@' or '#' or '$');
    }

    private static InvalidOperationException InvalidIdentifier(string sqlIdentifier, string reason) =>
        new($"SQL object identifier '{sqlIdentifier}' {reason}.");

    private enum QuoteMode
    {
        None,
        SquareBracket,
        DoubleQuote,
    }
}
