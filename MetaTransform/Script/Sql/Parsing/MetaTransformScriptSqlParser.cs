using MetaTransformScript;
using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptSqlParser
{
    internal enum TopLevelStatementShape
    {
        BareSelect,
        CreateView
    }

    public MetaTransformScriptModel ParseSqlCode(
        string sqlCode,
        string? sourcePath = null,
        string? bareSelectName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlCode);

        var tokens = new MetaTransformScriptSqlLexer(sqlCode).Tokenize();
        var builder = new MetaTransformScriptSqlModelBuilder();
        new Parser(tokens, builder, sourcePath, bareSelectName).ParseDocument();
        return builder.Build();
    }

    internal TopLevelStatementShape ParseSqlCodeIntoBuilder(
        string sqlCode,
        MetaTransformScriptSqlModelBuilder builder,
        string? sourcePath = null,
        string? bareSelectName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sqlCode);
        ArgumentNullException.ThrowIfNull(builder);

        var tokens = new MetaTransformScriptSqlLexer(sqlCode).Tokenize();
        return new Parser(tokens, builder, sourcePath, bareSelectName).ParseDocument();
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

        private readonly IReadOnlyList<MetaTransformScriptSqlToken> tokens;
        private readonly MetaTransformScriptSqlModelBuilder builder;
        private readonly string? sourcePath;
        private readonly string? bareSelectName;
        private int position;

        public Parser(
            IReadOnlyList<MetaTransformScriptSqlToken> tokens,
            MetaTransformScriptSqlModelBuilder builder,
            string? sourcePath,
            string? bareSelectName)
        {
            this.tokens = tokens;
            this.builder = builder;
            this.sourcePath = sourcePath;
            this.bareSelectName = bareSelectName;
        }
    }
}
