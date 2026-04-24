namespace MetaPipeline;

public sealed class SqlServerMultipartIdentifier
{
    private SqlServerMultipartIdentifier(IReadOnlyList<string> parts)
    {
        Parts = parts;
    }

    public IReadOnlyList<string> Parts { get; }

    public static SqlServerMultipartIdentifier Parse(string sqlIdentifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlIdentifier);

        var parts = new List<string>();
        var buffer = new System.Text.StringBuilder();
        var quoteMode = QuoteMode.None;

        for (var index = 0; index < sqlIdentifier.Length; index++)
        {
            var ch = sqlIdentifier[index];

            if (quoteMode == QuoteMode.None)
            {
                if (ch == '.')
                {
                    AddPart(parts, buffer);
                    continue;
                }

                if (ch == '[')
                {
                    quoteMode = QuoteMode.SquareBracket;
                    continue;
                }

                if (ch == '"')
                {
                    quoteMode = QuoteMode.DoubleQuote;
                    continue;
                }

                buffer.Append(ch);
                continue;
            }

            if (quoteMode == QuoteMode.SquareBracket)
            {
                if (ch == ']')
                {
                    if (index + 1 < sqlIdentifier.Length && sqlIdentifier[index + 1] == ']')
                    {
                        buffer.Append(']');
                        index++;
                        continue;
                    }

                    quoteMode = QuoteMode.None;
                    continue;
                }

                buffer.Append(ch);
                continue;
            }

            if (ch == '"')
            {
                if (index + 1 < sqlIdentifier.Length && sqlIdentifier[index + 1] == '"')
                {
                    buffer.Append('"');
                    index++;
                    continue;
                }

                quoteMode = QuoteMode.None;
                continue;
            }

            buffer.Append(ch);
        }

        if (quoteMode != QuoteMode.None)
        {
            throw new MetaPipelineConfigurationException(
                $"SQL identifier '{sqlIdentifier}' contains an unterminated quoted part.");
        }

        AddPart(parts, buffer);

        return parts.Count switch
        {
            1 or 2 or 3 => new SqlServerMultipartIdentifier(parts),
            _ => throw new MetaPipelineConfigurationException(
                $"SQL identifier '{sqlIdentifier}' uses {parts.Count} parts; expected table, schema.table, or database.schema.table."),
        };
    }

    public string RenderBracketQuoted()
    {
        return string.Join(
            ".",
            Parts.Select(static part => "[" + part.Replace("]", "]]", StringComparison.Ordinal) + "]"));
    }

    private static void AddPart(List<string> parts, System.Text.StringBuilder buffer)
    {
        var value = buffer.ToString().Trim();
        buffer.Clear();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new MetaPipelineConfigurationException(
                "SQL identifier contains an empty part.");
        }

        parts.Add(value);
    }

    private enum QuoteMode
    {
        None,
        SquareBracket,
        DoubleQuote,
    }
}
