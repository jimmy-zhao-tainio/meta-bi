using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptSqlParser
{
    private sealed partial class Parser
    {
        private BuiltNode ParseQueryExpression()
        {
            var currentExpression = ParseQueryExpressionPrimary();

            while (true)
            {
                if (MatchKeyword("UNION"))
                {
                    var isAll = MatchKeyword("ALL");
                    currentExpression = builder.CreateBinaryQueryExpression(currentExpression, ParseQueryExpressionPrimary(), "Union", isAll);
                    continue;
                }

                if (MatchKeyword("EXCEPT"))
                {
                    currentExpression = builder.CreateBinaryQueryExpression(currentExpression, ParseQueryExpressionPrimary(), "Except", all: false);
                    continue;
                }

                if (MatchKeyword("INTERSECT"))
                {
                    currentExpression = builder.CreateBinaryQueryExpression(currentExpression, ParseQueryExpressionPrimary(), "Intersect", all: false);
                    continue;
                }

                break;
            }

            var hasOrderBy = false;
            if (MatchKeyword("ORDER"))
            {
                ExpectKeyword("BY");
                currentExpression = builder.AttachOrderByClause(currentExpression, ParseOrderByClause());
                hasOrderBy = true;
            }

            if (PeekKeyword("OFFSET"))
            {
                if (!hasOrderBy)
                {
                    throw ParseError("OFFSET requires ORDER BY in the owned parser.");
                }

                currentExpression = builder.AttachOffsetClause(currentExpression, ParseOffsetClause());
            }

            return currentExpression;
        }

        private BuiltNode ParseQueryExpressionPrimary()
        {
            if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                return ParseQueryParenthesisExpression();
            }

            return ParseQuerySpecification();
        }

        private BuiltNode ParseQueryParenthesisExpression()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var queryExpression = ParseQueryExpression();
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateQueryParenthesisExpression(queryExpression);
        }

        private BuiltNode ParseQuerySpecification()
        {
            ExpectKeyword("SELECT");

            var uniqueRowFilter =
                MatchKeyword("DISTINCT") ? "Distinct" :
                string.Empty;

            BuiltNode? topRowFilter = null;
            if (MatchKeyword("TOP"))
            {
                topRowFilter = ParseTopRowFilter();
            }

            var selectElements = new List<BuiltNode> { ParseSelectElement() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                selectElements.Add(ParseSelectElement());
            }

            BuiltNode? fromClause = null;
            if (MatchKeyword("FROM"))
            {
                fromClause = ParseFromClause();
            }

            BuiltNode? whereClause = null;
            if (MatchKeyword("WHERE"))
            {
                whereClause = builder.CreateWhereClause(ParseBooleanExpression());
            }

            BuiltNode? groupByClause = null;
            if (MatchKeyword("GROUP"))
            {
                ExpectKeyword("BY");
                groupByClause = ParseGroupByClause();
            }

            BuiltNode? havingClause = null;
            if (MatchKeyword("HAVING"))
            {
                havingClause = builder.CreateHavingClause(ParseBooleanExpression());
            }

            BuiltNode? windowClause = null;
            if (MatchKeyword("WINDOW"))
            {
                windowClause = ParseWindowClause();
            }

            return builder.CreateQuerySpecification(
                selectElements,
                fromClause,
                whereClause,
                groupByClause,
                havingClause,
                topRowFilter,
                windowClause,
                uniqueRowFilter);
        }

        private BuiltNode ParseTopRowFilter()
        {
            BuiltNode expression;
            if (Match(MetaTransformScriptSqlTokenKind.OpenParen))
            {
                expression = builder.CreateParenthesisExpression(ParseScalarExpression());
                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            }
            else
            {
                expression = ParseScalarExpression();
            }

            var percent = MatchKeyword("PERCENT");
            var withTies = false;
            if (MatchKeyword("WITH"))
            {
                ExpectKeyword("TIES");
                withTies = true;
            }

            return builder.CreateTopRowFilter(expression, percent, withTies);
        }

        private BuiltNode ParseOrderByClause()
        {
            var orderByElements = new List<BuiltNode> { ParseExpressionWithSortOrder() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                orderByElements.Add(ParseExpressionWithSortOrder());
            }

            return builder.CreateOrderByClause(orderByElements);
        }

        private BuiltNode ParseOffsetClause()
        {
            ExpectKeyword("OFFSET");
            var offsetExpression = ParseScalarExpression();
            ExpectRowOrRows();

            BuiltNode? fetchExpression = null;
            if (MatchKeyword("FETCH"))
            {
                if (!(MatchKeyword("NEXT") || MatchKeyword("FIRST")))
                {
                    throw ParseError($"Expected NEXT or FIRST but found '{Current.Text}'.");
                }

                fetchExpression = ParseScalarExpression();
                ExpectRowOrRows();
                ExpectKeyword("ONLY");
            }

            return builder.CreateOffsetClause(offsetExpression, fetchExpression);
        }

        private BuiltNode ParseExpressionWithSortOrder()
        {
            var expression = ParseScalarExpression();
            var sortOrder =
                MatchKeyword("DESC") ? "Descending" :
                MatchKeyword("ASC") ? "Ascending" :
                "NotSpecified";
            return builder.CreateExpressionWithSortOrder(expression, sortOrder);
        }

        private BuiltNode ParseGroupByClause()
        {
            if (MatchKeyword("ALL"))
            {
                throw Unsupported("GROUP BY ALL is not supported in the owned parser yet.");
            }

            var groupingSpecifications = new List<BuiltNode> { builder.CreateExpressionGroupingSpecification(ParseScalarExpression()) };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                groupingSpecifications.Add(builder.CreateExpressionGroupingSpecification(ParseScalarExpression()));
            }

            return builder.CreateGroupByClause(groupingSpecifications);
        }

        private BuiltNode ParseSelectElement()
        {
            if (Match(MetaTransformScriptSqlTokenKind.Star))
            {
                return builder.CreateSelectStarExpression();
            }

            if (FormsQualifiedStar())
            {
                var qualifier = ParseMultiPartIdentifier(expectTrailingStar: true);
                Expect(MetaTransformScriptSqlTokenKind.Star);
                return builder.CreateSelectStarExpression(qualifier);
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.Identifier &&
                Peek().Kind == MetaTransformScriptSqlTokenKind.Equals)
            {
                var alias = builder.CreateIdentifierOrValueExpression(ParseIdentifier().Node);
                Expect(MetaTransformScriptSqlTokenKind.Equals);
                return builder.CreateSelectScalarExpression(ParseScalarExpression(), alias);
            }

            var expression = ParseScalarExpression();
            BuiltNode? aliasNode = null;
            if (MatchKeyword("AS"))
            {
                aliasNode = builder.CreateIdentifierOrValueExpression(ParseIdentifier().Node);
            }
            else if (CanStartAlias())
            {
                aliasNode = builder.CreateIdentifierOrValueExpression(ParseIdentifier().Node);
            }

            return builder.CreateSelectScalarExpression(expression, aliasNode);
        }

        private void ExpectRowOrRows()
        {
            if (MatchKeyword("ROW") || MatchKeyword("ROWS"))
            {
                return;
            }

            throw ParseError($"Expected ROW or ROWS but found '{Current.Text}'.");
        }
    }
}
