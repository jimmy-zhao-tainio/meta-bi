using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptSqlParser
{
    private sealed partial class Parser
    {
        public void ParseDocument()
        {
            if (MatchKeyword("CREATE"))
            {
                ParseCreateViewScript();
                return;
            }

            var selectStatement = ParseSelectStatement();
            SkipSemicolons();
            ExpectEndOfFile();

            var scriptName = string.IsNullOrWhiteSpace(fallbackName) ? "Script" : fallbackName!;
            builder.AddTransformScript(scriptName, sourcePath, selectStatement, schemaIdentifier: null, objectIdentifier: null);
        }

        private void ParseCreateViewScript()
        {
            ExpectKeyword("VIEW");

            var (schemaIdentifier, objectIdentifier, renderedName) = ParseCreateViewName();
            if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                throw Unsupported("CREATE VIEW column lists are not supported in parser phase 1.");
            }

            ExpectKeyword("AS");
            var selectStatement = ParseSelectStatement();
            SkipSemicolons();
            ExpectEndOfFile();

            builder.AddTransformScript(
                renderedName,
                sourcePath,
                selectStatement,
                schemaIdentifier?.Node,
                objectIdentifier.Node);
        }

        private (ParsedIdentifier? SchemaIdentifier, ParsedIdentifier ObjectIdentifier, string RenderedName) ParseCreateViewName()
        {
            var first = ParseIdentifier();
            if (!Match(MetaTransformScriptSqlTokenKind.Dot))
            {
                return (null, first, RenderIdentifier(first.Token));
            }

            var second = ParseIdentifier();
            if (Match(MetaTransformScriptSqlTokenKind.Dot))
            {
                throw Unsupported("CREATE VIEW names with more than two identifier parts are not supported in parser phase 1.");
            }

            return (first, second, $"{RenderIdentifier(first.Token)}.{RenderIdentifier(second.Token)}");
        }

        private BuiltNode ParseSelectStatement()
        {
            BuiltNode? xmlNamespaces = null;
            List<BuiltNode>? commonTableExpressions = null;
            if (MatchKeyword("WITH"))
            {
                if (PeekKeyword("XMLNAMESPACES"))
                {
                    xmlNamespaces = ParseXmlNamespacesClause();
                    if (Match(MetaTransformScriptSqlTokenKind.Comma))
                    {
                        commonTableExpressions = ParseCommonTableExpressions();
                    }
                }
                else
                {
                    commonTableExpressions = ParseCommonTableExpressions();
                }
            }

            var queryExpression = ParseQueryExpression();
            return builder.CreateSelectStatement(queryExpression, commonTableExpressions, xmlNamespaces);
        }

        private List<BuiltNode> ParseCommonTableExpressions()
        {
            var commonTableExpressions = new List<BuiltNode> { ParseCommonTableExpression() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                commonTableExpressions.Add(ParseCommonTableExpression());
            }

            return commonTableExpressions;
        }

        private BuiltNode ParseCommonTableExpression()
        {
            var expressionName = ParseIdentifier().Node;
            List<BuiltNode>? columns = null;
            if (Match(MetaTransformScriptSqlTokenKind.OpenParen))
            {
                columns = new List<BuiltNode> { ParseIdentifier().Node };
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    columns.Add(ParseIdentifier().Node);
                }

                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            }

            ExpectKeyword("AS");
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var queryExpression = ParseQueryExpression();
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateCommonTableExpression(expressionName, queryExpression, columns);
        }
    }
}
