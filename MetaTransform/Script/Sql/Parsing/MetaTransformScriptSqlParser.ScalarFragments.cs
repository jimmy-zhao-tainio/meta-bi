using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptSqlParser
{
    internal BuiltNode ParseScalarFragment(string sql, MetaTransformScriptSqlModelBuilder builder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);
        return new Parser(sql, new MetaTransformScriptSqlLexer(sql).Tokenize(), builder, null, null)
            .ParseScalarFragment();
    }

    private sealed partial class Parser
    {
        public BuiltNode ParseScalarFragment()
        {
            var expression = ParseScalarExpression();
            Expect(MetaTransformScriptSqlTokenKind.EndOfFile);
            return expression;
        }
    }
}
