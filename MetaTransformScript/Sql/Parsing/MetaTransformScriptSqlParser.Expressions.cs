using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptSqlParser
{
    private sealed partial class Parser
    {
        private BuiltNode ParseScalarExpression()
        {
            var left = ParseScalarTerm();
            while (Current.Kind is MetaTransformScriptSqlTokenKind.Plus or MetaTransformScriptSqlTokenKind.Minus)
            {
                var operatorKind = Advance().Kind;
                var binaryExpressionType = operatorKind switch
                {
                    MetaTransformScriptSqlTokenKind.Plus => "Add",
                    MetaTransformScriptSqlTokenKind.Minus => "Subtract",
                    _ => throw new InvalidOperationException($"Unexpected scalar additive operator '{operatorKind}'.")
                };

                left = builder.CreateBinaryExpression(left, ParseScalarTerm(), binaryExpressionType);
            }

            return left;
        }

        private BuiltNode ParseScalarTerm()
        {
            var left = ParseScalarPrimary();
            while (Current.Kind is MetaTransformScriptSqlTokenKind.Star
                or MetaTransformScriptSqlTokenKind.Slash
                or MetaTransformScriptSqlTokenKind.Percent)
            {
                var operatorKind = Advance().Kind;
                var binaryExpressionType = operatorKind switch
                {
                    MetaTransformScriptSqlTokenKind.Star => "Multiply",
                    MetaTransformScriptSqlTokenKind.Slash => "Divide",
                    MetaTransformScriptSqlTokenKind.Percent => "Modulo",
                    _ => throw new InvalidOperationException($"Unexpected scalar multiplicative operator '{operatorKind}'.")
                };

                left = builder.CreateBinaryExpression(left, ParseScalarPrimary(), binaryExpressionType);
            }

            return left;
        }

        private BuiltNode ParseScalarPrimary()
        {
            if (Match(MetaTransformScriptSqlTokenKind.Minus))
            {
                return builder.CreateUnaryExpression(ParseScalarPrimary(), "Negative");
            }

            if (Match(MetaTransformScriptSqlTokenKind.Plus))
            {
                return builder.CreateUnaryExpression(ParseScalarPrimary(), "Positive");
            }

            if (PeekKeyword("CASE"))
            {
                return ParseCaseExpression();
            }

            if (PeekKeyword("NEXT"))
            {
                return ParseNextValueForExpression();
            }

            if (PeekKeyword("NULL"))
            {
                Advance();
                return builder.CreateNullLiteral();
            }

            if (PeekKeyword("CURRENT_TIMESTAMP"))
            {
                Advance();
                return builder.CreateParameterlessCall("CurrentTimestamp");
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.Identifier &&
                string.Equals(Current.Value, "@@SPID", StringComparison.OrdinalIgnoreCase))
            {
                var token = Advance();
                return builder.CreateGlobalVariableExpression(token.Value);
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.StringLiteral)
            {
                var token = Advance();
                return builder.CreateStringLiteral(token.Value);
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.BinaryLiteral)
            {
                var token = Advance();
                return builder.CreateBinaryLiteral(token.Value);
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.NumberLiteral)
            {
                var token = Advance();
                return token.Value.IndexOfAny(['E', 'e']) >= 0
                    ? builder.CreateRealLiteral(token.Value)
                    : builder.CreateNumberLiteral(token.Value);
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.Identifier)
            {
                var identifiers = ParseIdentifierTokenChain();
                if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
                {
                    return ParseFunctionLikeExpression(identifiers, callTarget: null);
                }

                var multiPartIdentifier = builder.CreateMultiPartIdentifier(
                    identifiers.Select(token => builder.CreateIdentifier(token.Value, token.QuoteType)).ToArray());
                return ParseTrailingScalarSuffixes(builder.CreateColumnReferenceExpression(multiPartIdentifier));
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                if (PeekKeywordAfterOpenParen("SELECT"))
                {
                    return ParseScalarSubquery();
                }

                Expect(MetaTransformScriptSqlTokenKind.OpenParen);
                var expression = ParseScalarExpression();
                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                return ParseTrailingScalarSuffixes(builder.CreateParenthesisExpression(expression));
            }

            throw ParseError($"Expected a scalar expression but found '{Current.Text}'.");
        }

        private bool PeekKeywordAfterOpenParen(string keyword)
        {
            if (Current.Kind != MetaTransformScriptSqlTokenKind.OpenParen)
            {
                return false;
            }

            var probe = position + 1;
            while (probe < tokens.Count)
            {
                var token = tokens[probe];
                if (token.Kind == MetaTransformScriptSqlTokenKind.Semicolon)
                {
                    probe++;
                    continue;
                }

                return IsKeyword(token, keyword);
            }

            return false;
        }

        private BuiltNode ParseBooleanExpression() => ParseBooleanOr();

        private BuiltNode ParseBooleanOr()
        {
            var left = ParseBooleanAnd();
            while (MatchKeyword("OR"))
            {
                left = builder.CreateBooleanBinaryExpression(left, ParseBooleanAnd(), "Or");
            }

            return left;
        }

        private BuiltNode ParseBooleanAnd()
        {
            var left = ParseBooleanNot();
            while (MatchKeyword("AND"))
            {
                left = builder.CreateBooleanBinaryExpression(left, ParseBooleanNot(), "And");
            }

            return left;
        }

        private BuiltNode ParseBooleanNot()
        {
            if (MatchKeyword("NOT"))
            {
                return builder.CreateBooleanNotExpression(ParseBooleanPrimary());
            }

            return ParseBooleanPrimary();
        }

        private BuiltNode ParseBooleanPrimary()
        {
            if (MatchKeyword("EXISTS"))
            {
                return builder.CreateExistsPredicate(ParseScalarSubquery());
            }

            if (PeekKeyword("CONTAINS") || PeekKeyword("FREETEXT"))
            {
                return ParseFullTextPredicate();
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                if (FormsScalarComparison())
                {
                    return ParseComparisonExpression();
                }

                Expect(MetaTransformScriptSqlTokenKind.OpenParen);
                var inner = ParseBooleanExpression();
                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                return builder.CreateBooleanParenthesisExpression(inner);
            }

            return ParseComparisonExpression();
        }

        private bool FormsScalarComparison()
        {
            var probe = position;
            return ProbeScalarExpression(ref probe) && IsScalarComparisonOperator(PeekToken(probe));
        }

        private bool ProbeScalarExpression(ref int probe)
        {
            if (!ProbeScalarTerm(ref probe))
            {
                return false;
            }

            while (PeekToken(probe).Kind is MetaTransformScriptSqlTokenKind.Plus or MetaTransformScriptSqlTokenKind.Minus)
            {
                probe++;
                if (!ProbeScalarTerm(ref probe))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ProbeScalarTerm(ref int probe)
        {
            if (!ProbeScalarPrimary(ref probe))
            {
                return false;
            }

            while (PeekToken(probe).Kind is MetaTransformScriptSqlTokenKind.Star
                or MetaTransformScriptSqlTokenKind.Slash
                or MetaTransformScriptSqlTokenKind.Percent)
            {
                probe++;
                if (!ProbeScalarPrimary(ref probe))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ProbeScalarPrimary(ref int probe)
        {
            var current = PeekToken(probe);
            if (current.Kind is MetaTransformScriptSqlTokenKind.Plus or MetaTransformScriptSqlTokenKind.Minus)
            {
                probe++;
                return ProbeScalarPrimary(ref probe);
            }

            if (IsKeyword(current, "CASE"))
            {
                if (!ProbeCaseExpression(ref probe))
                {
                    return false;
                }

                ProbeTrailingScalarSuffixes(ref probe);
                return true;
            }

            if (IsKeyword(current, "NEXT"))
            {
                probe++;
                if (!IsKeyword(PeekToken(probe), "VALUE"))
                {
                    return false;
                }

                probe++;
                if (!IsKeyword(PeekToken(probe), "FOR"))
                {
                    return false;
                }

                probe++;
                if (!ProbeSchemaObjectName(ref probe))
                {
                    return false;
                }

                return true;
            }

            if (IsKeyword(current, "NULL") || IsKeyword(current, "CURRENT_TIMESTAMP"))
            {
                probe++;
                return true;
            }

            if (current.Kind == MetaTransformScriptSqlTokenKind.Identifier &&
                string.Equals(current.Value, "@@SPID", StringComparison.OrdinalIgnoreCase))
            {
                probe++;
                return true;
            }

            if (current.Kind is MetaTransformScriptSqlTokenKind.StringLiteral
                or MetaTransformScriptSqlTokenKind.BinaryLiteral
                or MetaTransformScriptSqlTokenKind.NumberLiteral)
            {
                probe++;
                return true;
            }

            if (current.Kind == MetaTransformScriptSqlTokenKind.Identifier)
            {
                if (!ProbeIdentifierChain(ref probe))
                {
                    return false;
                }

                if (PeekToken(probe).Kind == MetaTransformScriptSqlTokenKind.OpenParen)
                {
                    if (!ConsumeBalancedParentheses(ref probe))
                    {
                        return false;
                    }

                    if (IsKeyword(PeekToken(probe), "OVER"))
                    {
                        probe++;
                        if (PeekToken(probe).Kind == MetaTransformScriptSqlTokenKind.OpenParen)
                        {
                            if (!ConsumeBalancedParentheses(ref probe))
                            {
                                return false;
                            }
                        }
                        else if (PeekToken(probe).Kind == MetaTransformScriptSqlTokenKind.Identifier)
                        {
                            probe++;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                ProbeTrailingScalarSuffixes(ref probe);
                return true;
            }

            if (current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                if (PeekKeywordAfterOpenParenAt(probe, "SELECT"))
                {
                    return ConsumeBalancedParentheses(ref probe);
                }

                probe++;
                if (!ProbeScalarExpression(ref probe))
                {
                    return false;
                }

                if (PeekToken(probe).Kind != MetaTransformScriptSqlTokenKind.CloseParen)
                {
                    return false;
                }

                probe++;
                ProbeTrailingScalarSuffixes(ref probe);
                return true;
            }

            return false;
        }

        private void ProbeTrailingScalarSuffixes(ref int probe)
        {
            while (true)
            {
                if (IsKeyword(PeekToken(probe), "COLLATE"))
                {
                    probe++;
                    if (PeekToken(probe).Kind == MetaTransformScriptSqlTokenKind.Identifier)
                    {
                        probe++;
                    }

                    continue;
                }

                if (IsKeyword(PeekToken(probe), "AT"))
                {
                    var checkpoint = probe;
                    probe++;
                    if (!IsKeyword(PeekToken(probe), "TIME"))
                    {
                        probe = checkpoint;
                        return;
                    }

                    probe++;
                    if (!IsKeyword(PeekToken(probe), "ZONE"))
                    {
                        probe = checkpoint;
                        return;
                    }

                    probe++;
                    if (!ProbeScalarExpression(ref probe))
                    {
                        probe = checkpoint;
                        return;
                    }

                    continue;
                }

                return;
            }
        }

        private bool ProbeCaseExpression(ref int probe)
        {
            if (!IsKeyword(PeekToken(probe), "CASE"))
            {
                return false;
            }

            var depth = 0;
            while (probe < tokens.Count)
            {
                var token = PeekToken(probe);
                if (IsKeyword(token, "CASE"))
                {
                    depth++;
                }
                else if (IsKeyword(token, "END"))
                {
                    depth--;
                    probe++;
                    return depth == 0;
                }

                probe++;
            }

            return false;
        }

        private bool ProbeSchemaObjectName(ref int probe) =>
            ProbeIdentifierChain(ref probe);

        private bool ProbeIdentifierChain(ref int probe)
        {
            if (PeekToken(probe).Kind != MetaTransformScriptSqlTokenKind.Identifier)
            {
                return false;
            }

            probe++;
            while (PeekToken(probe).Kind == MetaTransformScriptSqlTokenKind.Dot)
            {
                probe++;
                if (PeekToken(probe).Kind != MetaTransformScriptSqlTokenKind.Identifier)
                {
                    return false;
                }

                probe++;
            }

            return true;
        }

        private bool ConsumeBalancedParentheses(ref int probe)
        {
            if (PeekToken(probe).Kind != MetaTransformScriptSqlTokenKind.OpenParen)
            {
                return false;
            }

            var depth = 0;
            while (probe < tokens.Count)
            {
                var kind = PeekToken(probe).Kind;
                if (kind == MetaTransformScriptSqlTokenKind.OpenParen)
                {
                    depth++;
                }
                else if (kind == MetaTransformScriptSqlTokenKind.CloseParen)
                {
                    depth--;
                    probe++;
                    if (depth == 0)
                    {
                        return true;
                    }

                    continue;
                }

                probe++;
            }

            return false;
        }

        private bool PeekKeywordAfterOpenParenAt(int probe, string keyword)
        {
            if (PeekToken(probe).Kind != MetaTransformScriptSqlTokenKind.OpenParen)
            {
                return false;
            }

            probe++;
            while (probe < tokens.Count)
            {
                var token = PeekToken(probe);
                if (token.Kind == MetaTransformScriptSqlTokenKind.Semicolon)
                {
                    probe++;
                    continue;
                }

                return IsKeyword(token, keyword);
            }

            return false;
        }

        private bool IsScalarComparisonOperator(MetaTransformScriptSqlToken token) =>
            token.Kind is MetaTransformScriptSqlTokenKind.Equals
                or MetaTransformScriptSqlTokenKind.GreaterThan
                or MetaTransformScriptSqlTokenKind.GreaterThanOrEqual
                or MetaTransformScriptSqlTokenKind.LessThan
                or MetaTransformScriptSqlTokenKind.LessThanOrEqual
                or MetaTransformScriptSqlTokenKind.NotEqual
            || IsKeyword(token, "BETWEEN")
            || IsKeyword(token, "IN")
            || IsKeyword(token, "LIKE")
            || IsKeyword(token, "IS");

        private BuiltNode ParseComparisonExpression()
        {
            var first = ParseScalarExpression();
            if (MatchKeyword("NOT"))
            {
                if (MatchKeyword("BETWEEN"))
                {
                    var secondBetweenNegated = ParseScalarExpression();
                    ExpectKeyword("AND");
                    var thirdBetweenNegated = ParseScalarExpression();
                    return builder.CreateBooleanNotExpression(
                        builder.CreateBooleanTernaryExpression(first, secondBetweenNegated, thirdBetweenNegated, "Between"));
                }

                if (MatchKeyword("IN"))
                {
                    Expect(MetaTransformScriptSqlTokenKind.OpenParen);
                    if (PeekKeyword("SELECT"))
                    {
                        var subquery = ParseQueryExpression();
                        Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                        return builder.CreateInPredicateSubquery(first, builder.CreateScalarSubquery(subquery), notDefined: true);
                    }

                    var negatedValues = new List<BuiltNode> { ParseScalarExpression() };
                    while (Match(MetaTransformScriptSqlTokenKind.Comma))
                    {
                        negatedValues.Add(ParseScalarExpression());
                    }

                    Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                    return builder.CreateInPredicate(first, negatedValues, notDefined: true);
                }

                if (MatchKeyword("LIKE"))
                {
                    var pattern = ParseScalarExpression();
                    BuiltNode? escapeExpression = null;
                    if (MatchKeyword("ESCAPE"))
                    {
                        escapeExpression = ParseScalarExpression();
                    }

                    return builder.CreateLikePredicate(first, pattern, notDefined: true, escapeExpression);
                }

                throw ParseError($"Expected BETWEEN, IN, or LIKE after NOT but found '{Current.Text}'.");
            }

            if (MatchKeyword("BETWEEN"))
            {
                var secondBetween = ParseScalarExpression();
                ExpectKeyword("AND");
                var thirdBetween = ParseScalarExpression();
                return builder.CreateBooleanTernaryExpression(first, secondBetween, thirdBetween, "Between");
            }

            if (MatchKeyword("IN"))
            {
                Expect(MetaTransformScriptSqlTokenKind.OpenParen);
                if (PeekKeyword("SELECT"))
                {
                    var subquery = ParseQueryExpression();
                    Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                    return builder.CreateInPredicateSubquery(first, builder.CreateScalarSubquery(subquery), notDefined: false);
                }

                var values = new List<BuiltNode> { ParseScalarExpression() };
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    values.Add(ParseScalarExpression());
                }

                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                return builder.CreateInPredicate(first, values, notDefined: false);
            }

            if (MatchKeyword("LIKE"))
            {
                var pattern = ParseScalarExpression();
                BuiltNode? escapeExpression = null;
                if (MatchKeyword("ESCAPE"))
                {
                    escapeExpression = ParseScalarExpression();
                }

                return builder.CreateLikePredicate(first, pattern, notDefined: false, escapeExpression);
            }

            if (MatchKeyword("IS"))
            {
                var isNot = MatchKeyword("NOT");
                if (MatchKeyword("DISTINCT"))
                {
                    ExpectKeyword("FROM");
                    return builder.CreateDistinctPredicate(first, ParseScalarExpression(), isNot);
                }

                ExpectKeyword("NULL");
                return builder.CreateBooleanIsNullExpression(first, isNot);
            }

            var comparisonType = Current.Kind switch
            {
                MetaTransformScriptSqlTokenKind.Equals => "Equals",
                MetaTransformScriptSqlTokenKind.GreaterThan => "GreaterThan",
                MetaTransformScriptSqlTokenKind.GreaterThanOrEqual => "GreaterThanOrEqualTo",
                MetaTransformScriptSqlTokenKind.LessThan => "LessThan",
                MetaTransformScriptSqlTokenKind.LessThanOrEqual => "LessThanOrEqualTo",
                MetaTransformScriptSqlTokenKind.NotEqual => "NotEqualToBrackets",
                _ => string.Empty
            };

            if (string.IsNullOrWhiteSpace(comparisonType))
            {
                throw ParseError($"Expected a comparison operator but found '{Current.Text}'.");
            }

            Advance();
            if (MatchKeyword("ALL"))
            {
                var subquery = ParseScalarSubquery();
                return builder.CreateSubqueryComparisonPredicate(first, subquery, comparisonType, "All");
            }

            if (MatchKeyword("ANY"))
            {
                var subquery = ParseScalarSubquery();
                return builder.CreateSubqueryComparisonPredicate(first, subquery, comparisonType, "Any");
            }

            var second = ParseScalarExpression();
            return builder.CreateBooleanComparisonExpression(first, second, comparisonType);
        }

        private BuiltNode ParseScalarSubquery()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            if (!PeekKeyword("SELECT"))
            {
                throw Unsupported("This parenthesized scalar expression shape is not supported yet.");
            }

            var queryExpression = ParseQueryExpression();
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateScalarSubquery(queryExpression);
        }

        private BuiltNode ParseNextValueForExpression()
        {
            ExpectKeyword("NEXT");
            ExpectKeyword("VALUE");
            ExpectKeyword("FOR");
            return builder.CreateNextValueForExpression(ParseSchemaObjectName());
        }

        private BuiltNode ParseTrailingScalarSuffixes(BuiltNode expression)
        {
            expression = ParseTrailingCollation(expression);

            while (MatchKeyword("AT"))
            {
                ExpectKeyword("TIME");
                ExpectKeyword("ZONE");
                expression = builder.CreateAtTimeZoneCall(expression, ParseScalarExpression());
                expression = ParseTrailingCollation(expression);
            }

            return expression;
        }

        private BuiltNode ParseFullTextPredicate()
        {
            var functionType =
                MatchKeyword("CONTAINS") ? "Contains" :
                MatchKeyword("FREETEXT") ? "FreeText" :
                throw ParseError($"Expected CONTAINS or FREETEXT but found '{Current.Text}'.");

            Expect(MetaTransformScriptSqlTokenKind.OpenParen);

            List<BuiltNode> columns;
            if (Match(MetaTransformScriptSqlTokenKind.OpenParen))
            {
                columns = new List<BuiltNode> { ParseColumnReferenceExpression() };
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    columns.Add(ParseColumnReferenceExpression());
                }

                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            }
            else
            {
                columns = new List<BuiltNode> { ParseColumnReferenceExpression() };
            }

            Expect(MetaTransformScriptSqlTokenKind.Comma);
            var value = ParseScalarExpression();
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);

            return builder.CreateFullTextPredicate(functionType, columns, value);
        }

        private BuiltNode ParseCaseExpression()
        {
            ExpectKeyword("CASE");

            if (PeekKeyword("WHEN"))
            {
                var whenClauses = new List<(BuiltNode WhenExpression, BuiltNode ThenExpression)>();
                while (MatchKeyword("WHEN"))
                {
                    var whenExpression = ParseBooleanExpression();
                    ExpectKeyword("THEN");
                    var thenExpression = ParseScalarExpression();
                    whenClauses.Add((whenExpression, thenExpression));
                }

                BuiltNode? searchedElseExpression = null;
                if (MatchKeyword("ELSE"))
                {
                    searchedElseExpression = ParseScalarExpression();
                }

                ExpectKeyword("END");
                return ParseTrailingCollation(builder.CreateSearchedCaseExpression(whenClauses, searchedElseExpression));
            }

            var inputExpression = ParseScalarExpression();
            var simpleWhenClauses = new List<(BuiltNode WhenExpression, BuiltNode ThenExpression)>();
            while (MatchKeyword("WHEN"))
            {
                var whenExpression = ParseScalarExpression();
                ExpectKeyword("THEN");
                var thenExpression = ParseScalarExpression();
                simpleWhenClauses.Add((whenExpression, thenExpression));
            }

            BuiltNode? elseExpression = null;
            if (MatchKeyword("ELSE"))
            {
                elseExpression = ParseScalarExpression();
            }

            ExpectKeyword("END");
            return ParseTrailingCollation(builder.CreateSimpleCaseExpression(inputExpression, simpleWhenClauses, elseExpression));
        }
    }
}
