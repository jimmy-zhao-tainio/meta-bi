using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptSqlParser
{
    private sealed partial class Parser
    {
        public TopLevelStatementShape ParseDocument()
        {
            if (MatchKeyword("CREATE"))
            {
                if (PeekKeyword("VIEW"))
                {
                    ParseCreateViewScript();
                    return TopLevelStatementShape.CreateWrappedSelect;
                }

                if (PeekKeyword("FUNCTION"))
                {
                    ParseCreateInlineTableValuedFunctionScript();
                    return TopLevelStatementShape.CreateWrappedSelect;
                }

                throw Unsupported($"CREATE wrapper '{Current.Value.ToUpperInvariant()}' is not supported yet.");
            }

            var selectStatement = ParseSelectStatement();
            SkipSemicolons();
            ExpectEndOfFile();

            if (string.IsNullOrWhiteSpace(bareSelectName))
            {
                throw Unsupported("Bare SELECT input requires an explicit script name.");
            }

            var scriptName = bareSelectName!;
            builder.AddTransformScript(scriptName, scriptName, sourcePath, selectStatement, schemaIdentifier: null, objectIdentifier: null);
            return TopLevelStatementShape.BareSelect;
        }

        private void ParseCreateViewScript()
        {
            ExpectKeyword("VIEW");

            var (schemaIdentifier, objectIdentifier, renderedName) = ParseCreateViewName();
            List<BuiltNode>? viewColumns = null;
            if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                viewColumns = ParseCreateViewColumnList();
            }

            if (MatchKeyword("WITH"))
            {
                ParseUnsupportedCreateViewOptions();
            }

            ExpectKeyword("AS");
            var selectStatement = ParseSelectStatement();

            if (MatchKeyword("WITH"))
            {
                ParseUnsupportedCreateViewTailClause();
            }

            SkipSemicolons();
            ExpectEndOfFile();

            builder.AddTransformScript(
                renderedName,
                renderedName,
                sourcePath,
                selectStatement,
                schemaIdentifier?.Node,
                objectIdentifier.Node,
                viewColumns,
                scriptObjectKind: "View");
        }

        private void ParseCreateInlineTableValuedFunctionScript()
        {
            ExpectKeyword("FUNCTION");

            var (schemaIdentifier, objectIdentifier, renderedName) = ParseCreateFunctionName();
            var functionParameters = ParseCreateFunctionParameters();

            ExpectKeyword("RETURNS");
            if (!MatchKeyword("TABLE"))
            {
                throw Unsupported("Only inline table-valued CREATE FUNCTION wrappers are supported (RETURNS TABLE).");
            }

            if (MatchKeyword("WITH"))
            {
                ParseUnsupportedCreateFunctionOptions();
            }

            ExpectKeyword("AS");
            ExpectKeyword("RETURN");

            BuiltNode selectStatement;
            if (Match(MetaTransformScriptSqlTokenKind.OpenParen))
            {
                selectStatement = ParseSelectStatement();
                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            }
            else
            {
                selectStatement = ParseSelectStatement();
            }

            SkipSemicolons();
            ExpectEndOfFile();

            builder.AddTransformScript(
                renderedName,
                renderedName,
                sourcePath,
                selectStatement,
                schemaIdentifier?.Node,
                objectIdentifier.Node,
                scriptObjectKind: "InlineTableValuedFunction",
                functionParameters: functionParameters);
        }

        private void ParseUnsupportedCreateViewOptions()
        {
            var optionNames = ParseCommaSeparatedOptionNames();
            var rendered = optionNames.Count == 0
                ? "WITH <view options>"
                : "WITH " + string.Join(", ", optionNames);
            throw Unsupported($"CREATE VIEW wrapper option clause '{rendered}' is not supported yet.");
        }

        private void ParseUnsupportedCreateFunctionOptions()
        {
            var optionNames = ParseCommaSeparatedOptionNames();
            var rendered = optionNames.Count == 0
                ? "WITH <function options>"
                : "WITH " + string.Join(", ", optionNames);
            throw Unsupported($"CREATE FUNCTION wrapper option clause '{rendered}' is not supported yet.");
        }

        private List<string> ParseCommaSeparatedOptionNames()
        {
            var optionNames = new List<string>();

            while (true)
            {
                optionNames.Add(ParseCreateViewOptionName());
                if (!Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    break;
                }
            }

            return optionNames;
        }

        private string ParseCreateViewOptionName()
        {
            return ParseIdentifierToken().Value.ToUpperInvariant();
        }

        private void ParseUnsupportedCreateViewTailClause()
        {
            if (MatchKeyword("CHECK"))
            {
                ExpectKeyword("OPTION");
                throw Unsupported("CREATE VIEW tail clause 'WITH CHECK OPTION' is not supported yet.");
            }

            throw Unsupported($"Unsupported CREATE VIEW tail clause beginning with 'WITH {Current.Value.ToUpperInvariant()}'.");
        }

        private (ParsedIdentifier? SchemaIdentifier, ParsedIdentifier ObjectIdentifier, string RenderedName) ParseCreateViewName()
        {
            return ParseCreateObjectName("VIEW");
        }

        private (ParsedIdentifier? SchemaIdentifier, ParsedIdentifier ObjectIdentifier, string RenderedName) ParseCreateFunctionName()
        {
            return ParseCreateObjectName("FUNCTION");
        }

        private (ParsedIdentifier? SchemaIdentifier, ParsedIdentifier ObjectIdentifier, string RenderedName) ParseCreateObjectName(string objectType)
        {
            var first = ParseIdentifier();
            if (!Match(MetaTransformScriptSqlTokenKind.Dot))
            {
                return (null, first, RenderIdentifier(first.Token));
            }

            var second = ParseIdentifier();
            if (Match(MetaTransformScriptSqlTokenKind.Dot))
            {
                throw Unsupported($"CREATE {objectType} names with more than two identifier parts are not supported.");
            }

            return (first, second, $"{RenderIdentifier(first.Token)}.{RenderIdentifier(second.Token)}");
        }

        private List<BuiltNode> ParseCreateViewColumnList()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var identifiers = new List<BuiltNode> { ParseIdentifier().Node };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                identifiers.Add(ParseIdentifier().Node);
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return identifiers;
        }

        private List<(BuiltNode ParameterName, BuiltNode DataTypeReference)> ParseCreateFunctionParameters()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var parameters = new List<(BuiltNode ParameterName, BuiltNode DataTypeReference)>();
            if (Match(MetaTransformScriptSqlTokenKind.CloseParen))
            {
                return parameters;
            }

            while (true)
            {
                var parameterName = ParseIdentifier();
                if (!parameterName.Token.Value.StartsWith('@'))
                {
                    throw Unsupported("CREATE FUNCTION parameters must be declared as @variables.");
                }

                var dataTypeReference = ParseDataTypeReference();
                if (Match(MetaTransformScriptSqlTokenKind.Equals))
                {
                    throw Unsupported("CREATE FUNCTION parameter default values are not supported yet.");
                }

                if (MatchKeyword("READONLY"))
                {
                    throw Unsupported("CREATE FUNCTION READONLY parameters are not supported yet.");
                }

                parameters.Add((parameterName.Node, dataTypeReference));

                if (!Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    break;
                }
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return parameters;
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
