using static MetaTransformScript.Sql.Parsing.MetaTransformScriptOwnedSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptOwnedSqlParser
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
            if (Current.Kind == MetaTransformScriptOwnedSqlTokenKind.OpenParen)
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
            if (!Match(MetaTransformScriptOwnedSqlTokenKind.Dot))
            {
                return (null, first, RenderIdentifier(first.Token));
            }

            var second = ParseIdentifier();
            if (Match(MetaTransformScriptOwnedSqlTokenKind.Dot))
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
                    if (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
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
            while (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
            {
                commonTableExpressions.Add(ParseCommonTableExpression());
            }

            return commonTableExpressions;
        }

        private BuiltNode ParseCommonTableExpression()
        {
            var expressionName = ParseIdentifier().Node;
            List<BuiltNode>? columns = null;
            if (Match(MetaTransformScriptOwnedSqlTokenKind.OpenParen))
            {
                columns = new List<BuiltNode> { ParseIdentifier().Node };
                while (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
                {
                    columns.Add(ParseIdentifier().Node);
                }

                Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            }

            ExpectKeyword("AS");
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
            var queryExpression = ParseQueryExpression();
            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return builder.CreateCommonTableExpression(expressionName, queryExpression, columns);
        }
    }
}
