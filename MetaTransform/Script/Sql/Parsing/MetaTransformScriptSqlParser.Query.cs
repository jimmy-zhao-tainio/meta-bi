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
                    throw ParseError("OFFSET requires ORDER BY.");
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
            var all = MatchKeyword("ALL");

            var groupingSpecifications = new List<BuiltNode> { ParseGroupingSpecification() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                groupingSpecifications.Add(ParseGroupingSpecification());
            }

            return builder.CreateGroupByClause(groupingSpecifications, all);
        }

        private BuiltNode ParseGroupingSpecification()
        {
            if (MatchKeyword("GROUPING"))
            {
                ExpectKeyword("SETS");
                return builder.CreateGroupingSetsGroupingSpecification(ParseGroupingSpecificationList());
            }

            if (MatchKeyword("ROLLUP"))
            {
                return builder.CreateRollupGroupingSpecification(ParseGroupingSpecificationList());
            }

            if (MatchKeyword("CUBE"))
            {
                return builder.CreateCubeGroupingSpecification(ParseGroupingSpecificationList());
            }

            if (Match(MetaTransformScriptSqlTokenKind.OpenParen))
            {
                if (Match(MetaTransformScriptSqlTokenKind.CloseParen))
                {
                    return builder.CreateGrandTotalGroupingSpecification();
                }

                var items = new List<BuiltNode> { ParseGroupingSpecification() };
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    items.Add(ParseGroupingSpecification());
                }

                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                return builder.CreateCompositeGroupingSpecification(items);
            }

            return builder.CreateExpressionGroupingSpecification(ParseScalarExpression());
        }

        private List<BuiltNode> ParseGroupingSpecificationList()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var items = new List<BuiltNode> { ParseGroupingSpecification() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                items.Add(ParseGroupingSpecification());
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return items;
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
                var aliasToken = ParseIdentifierToken();
                Expect(MetaTransformScriptSqlTokenKind.Equals);
                var assignedExpression = ParseScalarExpression();
                var alias = builder.CreateIdentifierOrValueExpression(
                    builder.CreateIdentifier(aliasToken.Value, aliasToken.QuoteType));
                return builder.CreateSelectScalarExpression(assignedExpression, alias);
            }

            var expression = ParseScalarExpression();
            BuiltNode? aliasNode = null;
            if (MatchKeyword("AS"))
            {
                var aliasToken = ParseIdentifierToken();
                aliasNode = builder.CreateIdentifierOrValueExpression(
                    builder.CreateIdentifier(aliasToken.Value, aliasToken.QuoteType));
            }
            else if (CanStartAlias())
            {
                var aliasToken = ParseIdentifierToken();
                aliasNode = builder.CreateIdentifierOrValueExpression(
                    builder.CreateIdentifier(aliasToken.Value, aliasToken.QuoteType));
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
