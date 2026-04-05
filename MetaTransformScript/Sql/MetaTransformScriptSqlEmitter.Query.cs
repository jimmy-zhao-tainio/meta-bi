using System.Text;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderWithClause(WithCtesAndXmlNamespaces withCtes)
    {
        var parts = new List<string>();

        var xmlNamespacesLink = FindOwnerLink(model.WithCtesAndXmlNamespacesXmlNamespacesLinkList, withCtes.Id);
        if (xmlNamespacesLink is not null)
        {
            var xmlParts = GetOrderedItems(model.XmlNamespacesXmlNamespacesElementsItemList, xmlNamespacesLink.Value.Id)
                .Select(row => RenderXmlNamespacesElement(row.Value))
                .ToArray();
            parts.Add("XMLNAMESPACES (" + string.Join(", ", xmlParts) + ")");
        }

        var ctes = GetOrderedItems(model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList, withCtes.Id)
            .Select(row => RenderCommonTableExpression(row.Value))
            .ToArray();
        parts.AddRange(ctes);

        return "WITH " + string.Join(", ", parts);
    }

    private string RenderCommonTableExpression(CommonTableExpression cte)
    {
        var name = RenderIdentifier(GetOwnerLink(model.CommonTableExpressionExpressionNameLinkList, cte.Id, "CommonTableExpression.ExpressionName").Value);
        var queryExpression = GetOwnerLink(model.CommonTableExpressionQueryExpressionLinkList, cte.Id, "CommonTableExpression.QueryExpression").Value;
        var columns = GetOrderedItems(model.CommonTableExpressionColumnsItemList, cte.Id)
            .Select(row => RenderIdentifier(row.Value))
            .ToArray();
        var columnList = columns.Length == 0
            ? string.Empty
            : " (" + string.Join(", ", columns) + ")";
        return $"{name}{columnList} AS ({RenderQueryExpression(queryExpression)})";
    }

    private string RenderQueryExpression(QueryExpression queryExpression)
    {
        string renderedCore;

        var querySpecification = FindByBaseId(model.QuerySpecificationList, queryExpression.Id);
        if (querySpecification is not null)
        {
            renderedCore = RenderQuerySpecification(querySpecification);
        }
        else
        {
            var binaryQueryExpression = FindByBaseId(model.BinaryQueryExpressionList, queryExpression.Id);
            if (binaryQueryExpression is not null)
            {
                var first = RenderQueryExpression(GetOwnerLink(model.BinaryQueryExpressionFirstQueryExpressionLinkList, binaryQueryExpression.Id, "BinaryQueryExpression.FirstQueryExpression").Value);
                var second = RenderQueryExpression(GetOwnerLink(model.BinaryQueryExpressionSecondQueryExpressionLinkList, binaryQueryExpression.Id, "BinaryQueryExpression.SecondQueryExpression").Value);
                var operatorText = binaryQueryExpression.BinaryQueryExpressionType switch
                {
                    "Union" when IsTrue(binaryQueryExpression.All) => "UNION ALL",
                    "Union" => "UNION",
                    "Except" => "EXCEPT",
                    "Intersect" => "INTERSECT",
                    _ => throw new InvalidOperationException($"Unsupported MetaTransformScript BinaryQueryExpressionType '{binaryQueryExpression.BinaryQueryExpressionType}'.")
                };

                renderedCore = $"{first}{Environment.NewLine}{operatorText}{Environment.NewLine}{second}";
            }
            else
            {
                var queryParenthesisExpression = FindByBaseId(model.QueryParenthesisExpressionList, queryExpression.Id);
                if (queryParenthesisExpression is not null)
                {
                    var child = GetOwnerLink(
                        model.QueryParenthesisExpressionQueryExpressionLinkList,
                        queryParenthesisExpression.Id,
                        "QueryParenthesisExpression.QueryExpression").Value;
                    renderedCore = "(" + RenderQueryExpression(child) + ")";
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported MetaTransformScript QueryExpression id '{queryExpression.Id}'.");
                }
            }
        }

        var orderByClauseLink = FindOwnerLink(model.QueryExpressionOrderByClauseLinkList, queryExpression.Id);
        if (orderByClauseLink is not null)
        {
            renderedCore += Environment.NewLine + RenderOrderByClause(orderByClauseLink.Value);
        }

        var offsetClauseLink = FindOwnerLink(model.QueryExpressionOffsetClauseLinkList, queryExpression.Id);
        if (offsetClauseLink is not null)
        {
            renderedCore += Environment.NewLine + RenderOffsetClause(offsetClauseLink.Value);
        }

        return renderedCore;
    }

    private string RenderQuerySpecification(QuerySpecification querySpecification)
    {
        var builder = new StringBuilder();
        builder.Append("SELECT");

        if (!string.IsNullOrWhiteSpace(querySpecification.UniqueRowFilter) &&
            !string.Equals(querySpecification.UniqueRowFilter, "NotSpecified", StringComparison.Ordinal))
        {
            builder.Append(' ');
            builder.Append(querySpecification.UniqueRowFilter.ToUpperInvariant());
        }

        var topRowFilterLink = FindOwnerLink(model.QuerySpecificationTopRowFilterLinkList, querySpecification.Id);
        if (topRowFilterLink is not null)
        {
            builder.Append(' ');
            builder.Append(RenderTopRowFilter(topRowFilterLink.Value));
        }

        var selectElements = GetOrderedItems(model.QuerySpecificationSelectElementsItemList, querySpecification.Id)
            .Select(row => RenderSelectElement(row.Value))
            .ToArray();

        builder.AppendLine();
        builder.Append("    ");
        builder.Append(string.Join("," + Environment.NewLine + "    ", selectElements));

        var fromClauseLink = FindOwnerLink(model.QuerySpecificationFromClauseLinkList, querySpecification.Id);
        if (fromClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("FROM ");
            builder.Append(RenderFromClause(fromClauseLink.Value));
        }

        var whereClauseLink = FindOwnerLink(model.QuerySpecificationWhereClauseLinkList, querySpecification.Id);
        if (whereClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("WHERE ");
            builder.Append(RenderBooleanExpression(GetOwnerLink(model.WhereClauseSearchConditionLinkList, whereClauseLink.Value.Id, "WhereClause.SearchCondition").Value));
        }

        var groupByClauseLink = FindOwnerLink(model.QuerySpecificationGroupByClauseLinkList, querySpecification.Id);
        if (groupByClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("GROUP BY ");
            builder.Append(RenderGroupByClause(groupByClauseLink.Value));
        }

        var havingClauseLink = FindOwnerLink(model.QuerySpecificationHavingClauseLinkList, querySpecification.Id);
        if (havingClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("HAVING ");
            builder.Append(RenderBooleanExpression(GetOwnerLink(model.HavingClauseSearchConditionLinkList, havingClauseLink.Value.Id, "HavingClause.SearchCondition").Value));
        }

        var windowClauseLink = FindOwnerLink(model.QuerySpecificationWindowClauseLinkList, querySpecification.Id);
        if (windowClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append(RenderWindowClause(windowClauseLink.Value));
        }

        return builder.ToString();
    }

    private string RenderGroupByClause(GroupByClause groupByClause)
    {
        if (!string.IsNullOrWhiteSpace(groupByClause.GroupByOption) &&
            !string.Equals(groupByClause.GroupByOption, "None", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Unsupported MetaTransformScript GroupByOption '{groupByClause.GroupByOption}'.");
        }

        var groupingSpecifications = GetOrderedItems(model.GroupByClauseGroupingSpecificationsItemList, groupByClause.Id)
            .Select(row => RenderGroupingSpecification(row.Value))
            .ToArray();
        var rendered = string.Join(", ", groupingSpecifications);
        return IsTrue(groupByClause.All)
            ? "ALL " + rendered
            : rendered;
    }

    private string RenderGroupingSpecification(GroupingSpecification groupingSpecification)
    {
        var expressionGroupingSpecification = FindByBaseId(model.ExpressionGroupingSpecificationList, groupingSpecification.Id);
        if (expressionGroupingSpecification is not null)
        {
            if (IsTrue(expressionGroupingSpecification.DistributedAggregation))
            {
                throw new InvalidOperationException("Phase-1 emitter does not support distributed aggregation grouping specifications.");
            }

            return RenderScalarExpression(GetOwnerLink(
                model.ExpressionGroupingSpecificationExpressionLinkList,
                expressionGroupingSpecification.Id,
                "ExpressionGroupingSpecification.Expression").Value);
        }

        var groupingSetsGroupingSpecification = FindByBaseId(model.GroupingSetsGroupingSpecificationList, groupingSpecification.Id);
        if (groupingSetsGroupingSpecification is not null)
        {
            var sets = GetOrderedItems(model.GroupingSetsGroupingSpecificationSetsItemList, groupingSetsGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.Value))
                .ToArray();
            return "GROUPING SETS (" + string.Join(", ", sets) + ")";
        }

        var rollupGroupingSpecification = FindByBaseId(model.RollupGroupingSpecificationList, groupingSpecification.Id);
        if (rollupGroupingSpecification is not null)
        {
            var arguments = GetOrderedItems(model.RollupGroupingSpecificationArgumentsItemList, rollupGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.Value))
                .ToArray();
            return "ROLLUP (" + string.Join(", ", arguments) + ")";
        }

        var cubeGroupingSpecification = FindByBaseId(model.CubeGroupingSpecificationList, groupingSpecification.Id);
        if (cubeGroupingSpecification is not null)
        {
            var arguments = GetOrderedItems(model.CubeGroupingSpecificationArgumentsItemList, cubeGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.Value))
                .ToArray();
            return "CUBE (" + string.Join(", ", arguments) + ")";
        }

        var compositeGroupingSpecification = FindByBaseId(model.CompositeGroupingSpecificationList, groupingSpecification.Id);
        if (compositeGroupingSpecification is not null)
        {
            var items = GetOrderedItems(model.CompositeGroupingSpecificationItemsItemList, compositeGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.Value))
                .ToArray();
            return "(" + string.Join(", ", items) + ")";
        }

        if (FindByBaseId(model.GrandTotalGroupingSpecificationList, groupingSpecification.Id) is not null)
        {
            return "()";
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript GroupingSpecification id '{groupingSpecification.Id}'.");
    }

    private string RenderWindowClause(WindowClause windowClause)
    {
        var definitions = GetOrderedItems(model.WindowClauseWindowDefinitionItemList, windowClause.Id)
            .Select(row => RenderWindowDefinition(row.Value))
            .ToArray();
        return "WINDOW" + Environment.NewLine + "    " + string.Join("," + Environment.NewLine + "    ", definitions);
    }

    private string RenderWindowDefinition(WindowDefinition windowDefinition)
    {
        var name = RenderIdentifier(GetOwnerLink(model.WindowDefinitionWindowNameLinkList, windowDefinition.Id, "WindowDefinition.WindowName").Value);
        var parts = new List<string>();

        var refWindowNameLink = FindOwnerLink(model.WindowDefinitionRefWindowNameLinkList, windowDefinition.Id);
        if (refWindowNameLink is not null)
        {
            parts.Add(RenderIdentifier(refWindowNameLink.Value));
        }

        var partitions = GetOrderedItems(model.WindowDefinitionPartitionsItemList, windowDefinition.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        if (partitions.Length > 0)
        {
            parts.Add("PARTITION BY " + string.Join(", ", partitions));
        }

        var orderByClauseLink = FindOwnerLink(model.WindowDefinitionOrderByClauseLinkList, windowDefinition.Id);
        if (orderByClauseLink is not null)
        {
            parts.Add(RenderOrderByClause(orderByClauseLink.Value));
        }

        var windowFrameClauseLink = FindOwnerLink(model.WindowDefinitionWindowFrameClauseLinkList, windowDefinition.Id);
        if (windowFrameClauseLink is not null)
        {
            parts.Add(RenderWindowFrameClause(windowFrameClauseLink.Value));
        }

        if (parts.Count == 0)
        {
            throw new InvalidOperationException($"WindowDefinition '{windowDefinition.Id}' had no renderable content.");
        }

        return $"{name} AS ({string.Join(" ", parts)})";
    }

    private string RenderOverClause(OverClause overClause)
    {
        var parts = new List<string>();

        var windowNameLink = FindOwnerLink(model.OverClauseWindowNameLinkList, overClause.Id);
        if (windowNameLink is not null)
        {
            parts.Add(RenderIdentifier(windowNameLink.Value));
        }

        var partitions = GetOrderedItems(model.OverClausePartitionsItemList, overClause.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        if (partitions.Length > 0)
        {
            parts.Add("PARTITION BY " + string.Join(", ", partitions));
        }

        var orderByClauseLink = FindOwnerLink(model.OverClauseOrderByClauseLinkList, overClause.Id);
        if (orderByClauseLink is not null)
        {
            parts.Add(RenderOrderByClause(orderByClauseLink.Value));
        }

        var windowFrameClauseLink = FindOwnerLink(model.OverClauseWindowFrameClauseLinkList, overClause.Id);
        if (windowFrameClauseLink is not null)
        {
            parts.Add(RenderWindowFrameClause(windowFrameClauseLink.Value));
        }

        if (parts.Count == 1 && windowNameLink is not null && partitions.Length == 0 && orderByClauseLink is null && windowFrameClauseLink is null)
        {
            return "OVER " + parts[0];
        }

        if (parts.Count == 0)
        {
            throw new InvalidOperationException($"OverClause '{overClause.Id}' had no renderable content.");
        }

        return "OVER (" + string.Join(" ", parts) + ")";
    }

    private string RenderOrderByClause(OrderByClause orderByClause)
    {
        var elements = GetOrderedItems(model.OrderByClauseOrderByElementsItemList, orderByClause.Id)
            .Select(row => RenderExpressionWithSortOrder(row.Value))
            .ToArray();
        return "ORDER BY " + string.Join(", ", elements);
    }

    private string RenderExpressionWithSortOrder(ExpressionWithSortOrder expressionWithSortOrder)
    {
        var rendered = RenderScalarExpression(GetOwnerLink(
            model.ExpressionWithSortOrderExpressionLinkList,
            expressionWithSortOrder.Id,
            "ExpressionWithSortOrder.Expression").Value);

        return expressionWithSortOrder.SortOrder switch
        {
            "Descending" => rendered + " DESC",
            "Ascending" => rendered + " ASC",
            "NotSpecified" or "" => rendered,
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript SortOrder '{expressionWithSortOrder.SortOrder}'.")
        };
    }

    private string RenderTopRowFilter(TopRowFilter topRowFilter)
    {
        if (IsTrue(topRowFilter.WithApproximate))
        {
            throw new InvalidOperationException("Phase-1 emitter does not support approximate TOP row filters.");
        }

        var expression = RenderScalarExpression(GetOwnerLink(
            model.TopRowFilterExpressionLinkList,
            topRowFilter.Id,
            "TopRowFilter.Expression").Value);

        var builder = new StringBuilder();
        builder.Append("TOP ");
        builder.Append(expression);

        if (IsTrue(topRowFilter.Percent))
        {
            builder.Append(" PERCENT");
        }

        if (IsTrue(topRowFilter.WithTies))
        {
            builder.Append(" WITH TIES");
        }

        return builder.ToString();
    }

    private string RenderOffsetClause(OffsetClause offsetClause)
    {
        if (IsTrue(offsetClause.WithApproximate))
        {
            throw new InvalidOperationException("Phase-1 emitter does not support approximate OFFSET/FETCH clauses.");
        }

        var offsetExpression = RenderScalarExpression(GetOwnerLink(
            model.OffsetClauseOffsetExpressionLinkList,
            offsetClause.Id,
            "OffsetClause.OffsetExpression").Value);
        var rendered = "OFFSET " + offsetExpression + " ROWS";

        var fetchExpressionLink = FindOwnerLink(model.OffsetClauseFetchExpressionLinkList, offsetClause.Id);
        if (fetchExpressionLink is not null)
        {
            rendered += " FETCH NEXT " + RenderScalarExpression(fetchExpressionLink.Value) + " ROWS ONLY";
        }

        return rendered;
    }

    private string RenderWindowFrameClause(WindowFrameClause windowFrameClause)
    {
        var frameType = windowFrameClause.WindowFrameType switch
        {
            "Rows" => "ROWS",
            "Range" => "RANGE",
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript WindowFrameType '{windowFrameClause.WindowFrameType}'.")
        };

        var top = RenderWindowDelimiter(GetOwnerLink(model.WindowFrameClauseTopLinkList, windowFrameClause.Id, "WindowFrameClause.Top").Value);
        var bottomLink = FindOwnerLink(model.WindowFrameClauseBottomLinkList, windowFrameClause.Id);
        return bottomLink is null
            ? $"{frameType} {top}"
            : $"{frameType} BETWEEN {top} AND {RenderWindowDelimiter(bottomLink.Value)}";
    }

    private string RenderWindowDelimiter(WindowDelimiter windowDelimiter)
    {
        return windowDelimiter.WindowDelimiterType switch
        {
            "CurrentRow" => "CURRENT ROW",
            "UnboundedPreceding" => "UNBOUNDED PRECEDING",
            "ValuePreceding" => RenderScalarExpression(GetOwnerLink(
                model.WindowDelimiterOffsetValueLinkList,
                windowDelimiter.Id,
                "WindowDelimiter.OffsetValue").Value) + " PRECEDING",
            "ValueFollowing" => RenderScalarExpression(GetOwnerLink(
                model.WindowDelimiterOffsetValueLinkList,
                windowDelimiter.Id,
                "WindowDelimiter.OffsetValue").Value) + " FOLLOWING",
            "UnboundedFollowing" => "UNBOUNDED FOLLOWING",
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript WindowDelimiterType '{windowDelimiter.WindowDelimiterType}'.")
        };
    }
}
