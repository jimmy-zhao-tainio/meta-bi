using MetaTransformScript;

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

        if (!TransformScriptSqlIdentifier.TryParseParts(sqlIdentifier, out var parts, out var failureReason))
        {
            throw new MetaPipelineConfigurationException(
                $"SQL identifier '{sqlIdentifier}' {failureReason}.");
        }

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
}
