using static MetaTransformScript.Sql.Parsing.MetaTransformScriptOwnedSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptOwnedSqlParser
{
    private sealed partial class Parser
    {
        private BuiltNode ParseScalarExpression()
        {
            var left = ParseScalarPrimary();
            while (Match(MetaTransformScriptOwnedSqlTokenKind.Plus))
            {
                left = builder.CreateBinaryExpression(left, ParseScalarPrimary(), "Add");
            }

            return left;
        }

        private BuiltNode ParseScalarPrimary()
        {
            if (PeekKeyword("CASE"))
            {
                return ParseCaseExpression();
            }

            if (Current.Kind == MetaTransformScriptOwnedSqlTokenKind.StringLiteral)
            {
                var token = Advance();
                return builder.CreateStringLiteral(token.Value);
            }

            if (Current.Kind == MetaTransformScriptOwnedSqlTokenKind.NumberLiteral)
            {
                var token = Advance();
                return builder.CreateNumberLiteral(token.Value);
            }

            if (Current.Kind == MetaTransformScriptOwnedSqlTokenKind.Identifier)
            {
                var identifiers = ParseIdentifierTokenChain();
                if (Current.Kind == MetaTransformScriptOwnedSqlTokenKind.OpenParen)
                {
                    return ParseFunctionLikeExpression(identifiers, callTarget: null);
                }

                var multiPartIdentifier = builder.CreateMultiPartIdentifier(
                    identifiers.Select(token => builder.CreateIdentifier(token.Value, token.QuoteType)).ToArray());
                return ParseTrailingCollation(builder.CreateColumnReferenceExpression(multiPartIdentifier));
            }

            if (Current.Kind == MetaTransformScriptOwnedSqlTokenKind.OpenParen)
            {
                if (PeekKeywordAfterOpenParen("SELECT"))
                {
                    return ParseScalarSubquery();
                }

                Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
                var expression = ParseScalarExpression();
                Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
                return ParseTrailingCollation(builder.CreateParenthesisExpression(expression));
            }

            throw ParseError($"Expected a scalar expression but found '{Current.Text}'.");
        }

        private bool PeekKeywordAfterOpenParen(string keyword)
        {
            if (Current.Kind != MetaTransformScriptOwnedSqlTokenKind.OpenParen)
            {
                return false;
            }

            var probe = position + 1;
            while (probe < tokens.Count)
            {
                var token = tokens[probe];
                if (token.Kind == MetaTransformScriptOwnedSqlTokenKind.Semicolon)
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

            if (Match(MetaTransformScriptOwnedSqlTokenKind.OpenParen))
            {
                var inner = ParseBooleanExpression();
                Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
                return builder.CreateBooleanParenthesisExpression(inner);
            }

            return ParseComparisonExpression();
        }

        private BuiltNode ParseComparisonExpression()
        {
            var first = ParseScalarExpression();
            if (MatchKeyword("BETWEEN"))
            {
                var secondBetween = ParseScalarExpression();
                ExpectKeyword("AND");
                var thirdBetween = ParseScalarExpression();
                return builder.CreateBooleanTernaryExpression(first, secondBetween, thirdBetween, "Between");
            }

            if (MatchKeyword("IN"))
            {
                Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
                if (PeekKeyword("SELECT"))
                {
                    var subquery = ParseQueryExpression();
                    Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
                    return builder.CreateInPredicateSubquery(first, builder.CreateScalarSubquery(subquery), notDefined: false);
                }

                var values = new List<BuiltNode> { ParseScalarExpression() };
                while (Match(MetaTransformScriptOwnedSqlTokenKind.Comma))
                {
                    values.Add(ParseScalarExpression());
                }

                Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
                return builder.CreateInPredicate(first, values, notDefined: false);
            }

            if (MatchKeyword("LIKE"))
            {
                return builder.CreateLikePredicate(first, ParseScalarExpression(), notDefined: false);
            }

            if (MatchKeyword("IS"))
            {
                var isNot = MatchKeyword("NOT");
                ExpectKeyword("NULL");
                return builder.CreateBooleanIsNullExpression(first, isNot);
            }

            var comparisonType = Current.Kind switch
            {
                MetaTransformScriptOwnedSqlTokenKind.Equals => "Equals",
                MetaTransformScriptOwnedSqlTokenKind.GreaterThan => "GreaterThan",
                MetaTransformScriptOwnedSqlTokenKind.GreaterThanOrEqual => "GreaterThanOrEqualTo",
                MetaTransformScriptOwnedSqlTokenKind.LessThan => "LessThan",
                MetaTransformScriptOwnedSqlTokenKind.LessThanOrEqual => "LessThanOrEqualTo",
                MetaTransformScriptOwnedSqlTokenKind.NotEqual => "NotEqualToBrackets",
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
            Expect(MetaTransformScriptOwnedSqlTokenKind.OpenParen);
            if (!PeekKeyword("SELECT"))
            {
                throw Unsupported("Parenthesized scalar expressions are not supported in parser phase 1.");
            }

            var queryExpression = ParseQueryExpression();
            Expect(MetaTransformScriptOwnedSqlTokenKind.CloseParen);
            return builder.CreateScalarSubquery(queryExpression);
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
