using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptSqlParser
{
    private sealed partial class Parser
    {
        private BuiltNode ParseFromClause()
        {
            var tableReferences = new List<BuiltNode> { ParseTableReference() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                tableReferences.Add(ParseTableReference());
            }

            return builder.CreateFromClause(tableReferences);
        }

        private BuiltNode ParseTableReference()
        {
            var currentReference = ParseTableReferencePrimary();

            while (true)
            {
                if (MatchKeyword("PIVOT"))
                {
                    currentReference = ParsePivotedTableReference(currentReference);
                    continue;
                }

                if (MatchKeyword("UNPIVOT"))
                {
                    currentReference = ParseUnpivotedTableReference(currentReference);
                    continue;
                }

                if (MatchKeyword("INNER"))
                {
                    ExpectKeyword("JOIN");
                    var right = ParseTableReferencePrimary();
                    ExpectKeyword("ON");
                    currentReference = builder.CreateQualifiedJoin(currentReference, right, "Inner", ParseBooleanExpression());
                    continue;
                }

                if (MatchKeyword("LEFT"))
                {
                    MatchKeyword("OUTER");
                    ExpectKeyword("JOIN");
                    var right = ParseTableReferencePrimary();
                    ExpectKeyword("ON");
                    currentReference = builder.CreateQualifiedJoin(currentReference, right, "LeftOuter", ParseBooleanExpression());
                    continue;
                }

                if (MatchKeyword("RIGHT"))
                {
                    MatchKeyword("OUTER");
                    ExpectKeyword("JOIN");
                    var right = ParseTableReferencePrimary();
                    ExpectKeyword("ON");
                    currentReference = builder.CreateQualifiedJoin(currentReference, right, "RightOuter", ParseBooleanExpression());
                    continue;
                }

                if (MatchKeyword("FULL"))
                {
                    MatchKeyword("OUTER");
                    ExpectKeyword("JOIN");
                    var right = ParseTableReferencePrimary();
                    ExpectKeyword("ON");
                    currentReference = builder.CreateQualifiedJoin(currentReference, right, "FullOuter", ParseBooleanExpression());
                    continue;
                }

                if (MatchKeyword("CROSS"))
                {
                    if (MatchKeyword("JOIN"))
                    {
                        currentReference = builder.CreateUnqualifiedJoin(currentReference, ParseTableReferencePrimary(), "CrossJoin");
                        continue;
                    }

                    if (MatchKeyword("APPLY"))
                    {
                        currentReference = builder.CreateUnqualifiedJoin(currentReference, ParseTableReferencePrimary(), "CrossApply");
                        continue;
                    }

                    throw Unsupported("Unsupported CROSS table-reference form.");
                }

                if (MatchKeyword("OUTER"))
                {
                    ExpectKeyword("APPLY");
                    currentReference = builder.CreateUnqualifiedJoin(currentReference, ParseTableReferencePrimary(), "OuterApply");
                    continue;
                }

                return currentReference;
            }
        }

        private BuiltNode ParseTableReferencePrimary()
        {
            if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                return ParseParenthesizedTableReference();
            }

            return ParseNamedOrFunctionTableReference();
        }

        private BuiltNode ParseNamedOrFunctionTableReference()
        {
            var schemaObjectName = ParseSchemaObjectName();

            if (Match(MetaTransformScriptSqlTokenKind.OpenParen))
            {
                return ParseSchemaObjectFunctionTableReference(schemaObjectName);
            }

            BuiltNode? alias = null;

            if (MatchKeyword("AS"))
            {
                alias = ParseIdentifier().Node;
            }
            else if (CanStartAlias())
            {
                alias = ParseIdentifier().Node;
            }

            return builder.CreateNamedTableReference(schemaObjectName, alias);
        }

        private BuiltNode ParseSchemaObjectFunctionTableReference(BuiltNode schemaObjectName)
        {
            var parameters = new List<BuiltNode>();
            if (!Match(MetaTransformScriptSqlTokenKind.CloseParen))
            {
                parameters.Add(ParseScalarExpression());
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    parameters.Add(ParseScalarExpression());
                }

                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            }

            var (alias, columns) = ParseRequiredTableAliasAndColumns();
            return builder.CreateSchemaObjectFunctionTableReference(schemaObjectName, parameters, alias, columns);
        }

        private BuiltNode ParseParenthesizedTableReference()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            if (!PeekKeyword("SELECT"))
            {
                throw Unsupported("Only query-derived tables are supported in parenthesized table-reference position in parser phase 1.");
            }

            var queryExpression = ParseQueryExpression();
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            var (alias, columns) = ParseRequiredTableAliasAndColumns();
            return builder.CreateQueryDerivedTable(queryExpression, alias, columns);
        }

        private BuiltNode ParsePivotedTableReference(BuiltNode sourceTableReference)
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);

            var aggregateIdentifierTokens = ParseIdentifierTokenChain();
            var aggregateFunctionIdentifier = builder.CreateMultiPartIdentifier(
                aggregateIdentifierTokens.Select(token => builder.CreateIdentifier(token.Value, token.QuoteType)).ToArray());

            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var valueColumns = new List<BuiltNode> { ParseColumnReferenceExpression() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                valueColumns.Add(ParseColumnReferenceExpression());
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            ExpectKeyword("FOR");
            var pivotColumn = ParseColumnReferenceExpression();
            ExpectKeyword("IN");
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);

            var inColumns = new List<BuiltNode> { ParseIdentifier().Node };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                inColumns.Add(ParseIdentifier().Node);
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);

            var alias = ParseRequiredTableAlias();
            return builder.CreatePivotedTableReference(
                sourceTableReference,
                aggregateFunctionIdentifier,
                valueColumns,
                pivotColumn,
                inColumns,
                alias);
        }

        private BuiltNode ParseUnpivotedTableReference(BuiltNode sourceTableReference)
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var valueColumn = ParseIdentifier().Node;
            ExpectKeyword("FOR");
            var pivotColumn = ParseIdentifier().Node;
            ExpectKeyword("IN");
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);

            var inColumns = new List<BuiltNode> { ParseColumnReferenceExpression() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                inColumns.Add(ParseColumnReferenceExpression());
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);

            var alias = ParseRequiredTableAlias();
            return builder.CreateUnpivotedTableReference(
                sourceTableReference,
                valueColumn,
                pivotColumn,
                inColumns,
                alias);
        }

        private (BuiltNode Alias, IReadOnlyList<BuiltNode> Columns) ParseRequiredTableAliasAndColumns()
        {
            BuiltNode? alias = null;
            if (MatchKeyword("AS"))
            {
                alias = ParseIdentifier().Node;
            }
            else if (CanStartAlias())
            {
                alias = ParseIdentifier().Node;
            }

            if (alias is null)
            {
                throw ParseError("Derived tables and table-valued function references require an alias.");
            }

            var columns = new List<BuiltNode>();
            if (Match(MetaTransformScriptSqlTokenKind.OpenParen))
            {
                columns.Add(ParseIdentifier().Node);
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    columns.Add(ParseIdentifier().Node);
                }

                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            }

            return (alias, columns);
        }

        private BuiltNode ParseRequiredTableAlias()
        {
            BuiltNode? alias = null;
            if (MatchKeyword("AS"))
            {
                alias = ParseIdentifier().Node;
            }
            else if (CanStartAlias())
            {
                alias = ParseIdentifier().Node;
            }

            if (alias is null)
            {
                throw ParseError("PIVOT and UNPIVOT table references require an alias.");
            }

            return alias;
        }

        private BuiltNode ParseColumnReferenceExpression()
        {
            var multiPartIdentifier = ParseMultiPartIdentifier();
            return builder.CreateColumnReferenceExpression(multiPartIdentifier);
        }

        private BuiltNode ParseSchemaObjectName()
        {
            var first = ParseIdentifier();
            if (!Match(MetaTransformScriptSqlTokenKind.Dot))
            {
                return builder.CreateSchemaObjectName(null, first.Node);
            }

            var second = ParseIdentifier();
            if (Match(MetaTransformScriptSqlTokenKind.Dot))
            {
                throw Unsupported("Schema object names with more than two identifier parts are not supported in parser phase 1.");
            }

            return builder.CreateSchemaObjectName(first.Node, second.Node);
        }
    }
}
