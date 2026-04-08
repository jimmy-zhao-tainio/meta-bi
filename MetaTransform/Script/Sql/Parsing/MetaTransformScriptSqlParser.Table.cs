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
            if (CanStartXmlNodesTableReference())
            {
                return ParseXmlNodesTableReference();
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                return ParseParenthesizedTableReference();
            }

            return ParseNamedOrFunctionTableReference();
        }

        private bool CanStartXmlNodesTableReference()
        {
            if (Current.Kind != MetaTransformScriptSqlTokenKind.Identifier)
            {
                return false;
            }

            var probe = position;
            if (PeekToken(probe).Kind != MetaTransformScriptSqlTokenKind.Identifier)
            {
                return false;
            }

            probe++;
            var identifierCount = 1;

            while (PeekToken(probe).Kind == MetaTransformScriptSqlTokenKind.Dot)
            {
                if (PeekToken(probe + 1).Kind != MetaTransformScriptSqlTokenKind.Identifier)
                {
                    return false;
                }

                if (identifierCount >= 1
                    && string.Equals(PeekToken(probe + 1).Value, "nodes", StringComparison.OrdinalIgnoreCase)
                    && PeekToken(probe + 2).Kind == MetaTransformScriptSqlTokenKind.OpenParen)
                {
                    return true;
                }

                probe += 2;
                identifierCount++;
            }

            return false;
        }

        private BuiltNode ParseXmlNodesTableReference()
        {
            var targetIdentifierTokens = new List<MetaTransformScriptSqlToken> { ParseIdentifierToken() };
            while (Current.Kind == MetaTransformScriptSqlTokenKind.Dot
                && Peek().Kind == MetaTransformScriptSqlTokenKind.Identifier
                && !string.Equals(Peek().Value, "nodes", StringComparison.OrdinalIgnoreCase))
            {
                Expect(MetaTransformScriptSqlTokenKind.Dot);
                targetIdentifierTokens.Add(ParseIdentifierToken());
            }

            var targetExpression = builder.CreateColumnReferenceExpression(
                builder.CreateMultiPartIdentifier(
                    targetIdentifierTokens
                        .Select(token => builder.CreateIdentifier(token.Value, token.QuoteType))
                        .ToArray()));

            Expect(MetaTransformScriptSqlTokenKind.Dot);
            var methodName = ParseIdentifierToken();
            if (!string.Equals(methodName.Value, "nodes", StringComparison.OrdinalIgnoreCase))
            {
                throw ParseError($"Expected 'nodes' but found '{methodName.Text}'.");
            }

            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            if (Current.Kind != MetaTransformScriptSqlTokenKind.StringLiteral)
            {
                throw ParseError($"Expected an XQuery string literal but found '{Current.Text}'.");
            }

            var xQueryLiteralToken = Advance();
            var xQueryLiteral = builder.CreateStringLiteral(xQueryLiteralToken.Value);
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);

            var (alias, columns) = ParseRequiredTableAliasAndColumns();
            return builder.CreateXmlNodesTableReference(targetExpression, xQueryLiteral, alias, columns);
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

            BuiltNode? tableSampleClause = null;
            if (MatchKeyword("TABLESAMPLE"))
            {
                tableSampleClause = ParseTableSampleClause();
            }

            return builder.CreateNamedTableReference(schemaObjectName, alias, tableSampleClause);
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
            if (PeekKeyword("SELECT"))
            {
                var queryExpression = ParseQueryExpression();
                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                var (alias, columns) = ParseRequiredTableAliasAndColumns();
                return builder.CreateQueryDerivedTable(queryExpression, alias, columns);
            }

            if (PeekKeyword("VALUES"))
            {
                Advance();
                var rowValues = new List<BuiltNode> { ParseRowValue() };
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    rowValues.Add(ParseRowValue());
                }

                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                var (alias, columns) = ParseRequiredTableAliasAndColumns();
                return builder.CreateInlineDerivedTable(rowValues, alias, columns);
            }

            var tableReference = ParseTableReference();
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);

            try
            {
                tableReference.GetId(nameof(JoinTableReference));
            }
            catch (InvalidOperationException)
            {
                throw Unsupported("Only query-derived tables, inline VALUES tables, and parenthesized join table references are supported in parenthesized table-reference position.");
            }

            return builder.CreateJoinParenthesisTableReference(tableReference);
        }

        private BuiltNode ParseRowValue()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var columnValues = new List<BuiltNode> { ParseScalarExpression() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                columnValues.Add(ParseScalarExpression());
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateRowValue(columnValues);
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

        private BuiltNode ParseTableSampleClause()
        {
            var system = MatchKeyword("SYSTEM");
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var sampleNumber = ParseScalarExpression();

            var option =
                MatchKeyword("PERCENT") ? "Percent" :
                MatchKeyword("ROWS") ? "Rows" :
                throw ParseError($"Expected PERCENT or ROWS but found '{Current.Text}'.");

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);

            BuiltNode? repeatSeed = null;
            if (MatchKeyword("REPEATABLE"))
            {
                Expect(MetaTransformScriptSqlTokenKind.OpenParen);
                repeatSeed = ParseScalarExpression();
                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            }

            return builder.CreateTableSampleClause(sampleNumber, option, repeatSeed, system);
        }

        private BuiltNode ParseSchemaObjectName()
        {
            var identifiers = ParseIdentifierChain();
            return builder.CreateSchemaObjectName(identifiers.Select(static identifier => identifier.Node).ToArray());
        }
    }
}
