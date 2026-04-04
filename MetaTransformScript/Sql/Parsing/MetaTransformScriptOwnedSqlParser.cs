using MetaTransformScript;
using static MetaTransformScript.Sql.Parsing.MetaTransformScriptOwnedSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptOwnedSqlParser
{
    public MetaTransformScriptModel ParseSqlCode(
        string sqlCode,
        string? sourcePath = null,
        string? fallbackName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlCode);

        var tokens = new MetaTransformScriptOwnedSqlLexer(sqlCode).Tokenize();
        var builder = new MetaTransformScriptOwnedSqlModelBuilder();
        new Parser(tokens, builder, sourcePath, fallbackName).ParseDocument();
        return builder.Build();
    }

    private sealed partial class Parser
    {
        private static readonly HashSet<string> ClauseKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "AS",
            "FROM",
            "WHERE",
            "GROUP",
            "HAVING",
            "WINDOW",
            "ORDER",
            "OFFSET",
            "FETCH",
            "UNION",
            "EXCEPT",
            "INTERSECT",
            "JOIN",
            "INNER",
            "LEFT",
            "RIGHT",
            "FULL",
            "CROSS",
            "OUTER",
            "ON",
            "GO"
        };

        private readonly IReadOnlyList<MetaTransformScriptOwnedSqlToken> tokens;
        private readonly MetaTransformScriptOwnedSqlModelBuilder builder;
        private readonly string? sourcePath;
        private readonly string? fallbackName;
        private int position;

        public Parser(
            IReadOnlyList<MetaTransformScriptOwnedSqlToken> tokens,
            MetaTransformScriptOwnedSqlModelBuilder builder,
            string? sourcePath,
            string? fallbackName)
        {
            this.tokens = tokens;
            this.builder = builder;
            this.sourcePath = sourcePath;
            this.fallbackName = fallbackName;
        }
    }
}
