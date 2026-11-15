using System.Text;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderWithClause(WithCtesAndXmlNamespaces withCtes)
    {
        var parts = new List<string>();

        var xmlNamespacesLink = FindOwnerLink(model.WithCtesAndXmlNamespacesXmlNamespacesLinkList, static row => row.WithCtesAndXmlNamespaces?.Id, nameof(WithCtesAndXmlNamespacesXmlNamespacesLink.WithCtesAndXmlNamespaces), withCtes.Id);
        if (xmlNamespacesLink is not null)
        {
            var xmlParts = GetOrderedItems(model.XmlNamespacesXmlNamespacesElementsItemList, static row => row.XmlNamespaces?.Id, nameof(XmlNamespacesXmlNamespacesElementsItem.XmlNamespaces), static row => row.Ordinal, xmlNamespacesLink.XmlNamespaces.Id)
                .Select(row => RenderXmlNamespacesElement(row.XmlNamespacesElement))
                .ToArray();
            parts.Add("XMLNAMESPACES (" + string.Join(", ", xmlParts) + ")");
        }

        var ctes = GetOrderedItems(model.WithCtesAndXmlNamespacesCommonTableExpressionsItemList, static row => row.WithCtesAndXmlNamespaces?.Id, nameof(WithCtesAndXmlNamespacesCommonTableExpressionsItem.WithCtesAndXmlNamespaces), static row => row.Ordinal, withCtes.Id)
            .Select(row => RenderCommonTableExpression(row.CommonTableExpression))
            .ToArray();
        parts.AddRange(ctes);

        return "WITH " + string.Join(", ", parts);
    }

    private string RenderCommonTableExpression(CommonTableExpression cte)
    {
        var name = RenderIdentifier(GetOwnerLink(model.CommonTableExpressionExpressionNameLinkList, static row => row.CommonTableExpression?.Id, nameof(CommonTableExpressionExpressionNameLink.CommonTableExpression), cte.Id, "CommonTableExpression.ExpressionName").Identifier);
        var queryExpression = GetOwnerLink(model.CommonTableExpressionQueryExpressionLinkList, static row => row.CommonTableExpression?.Id, nameof(CommonTableExpressionQueryExpressionLink.CommonTableExpression), cte.Id, "CommonTableExpression.QueryExpression").QueryExpression;
        var columns = GetOrderedItems(model.CommonTableExpressionColumnsItemList, static row => row.CommonTableExpression?.Id, nameof(CommonTableExpressionColumnsItem.CommonTableExpression), static row => row.Ordinal, cte.Id)
            .Select(row => RenderIdentifier(row.Identifier))
            .ToArray();
        var columnList = columns.Length == 0
            ? string.Empty
            : " (" + string.Join(", ", columns) + ")";
        return $"{name}{columnList} AS ({RenderQueryExpression(queryExpression)})";
    }

    private string RenderQueryExpression(QueryExpression queryExpression)
    {
        string renderedCore;

        var querySpecification = FindByBaseId(model.QuerySpecificationList, static row => row.QueryExpression?.Id, nameof(QuerySpecification.QueryExpression), queryExpression.Id);
        if (querySpecification is not null)
        {
            renderedCore = RenderQuerySpecification(querySpecification);
        }
        else
        {
            var binaryQueryExpression = FindByBaseId(model.BinaryQueryExpressionList, static row => row.QueryExpression?.Id, nameof(BinaryQueryExpression.QueryExpression), queryExpression.Id);
            if (binaryQueryExpression is not null)
            {
                var first = RenderQueryExpression(GetOwnerLink(model.BinaryQueryExpressionFirstQueryExpressionLinkList, static row => row.BinaryQueryExpression?.Id, nameof(BinaryQueryExpressionFirstQueryExpressionLink.BinaryQueryExpression), binaryQueryExpression.Id, "BinaryQueryExpression.FirstQueryExpression").QueryExpression);
                var second = RenderQueryExpression(GetOwnerLink(model.BinaryQueryExpressionSecondQueryExpressionLinkList, static row => row.BinaryQueryExpression?.Id, nameof(BinaryQueryExpressionSecondQueryExpressionLink.BinaryQueryExpression), binaryQueryExpression.Id, "BinaryQueryExpression.SecondQueryExpression").QueryExpression);
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
                var queryParenthesisExpression = FindByBaseId(model.QueryParenthesisExpressionList, static row => row.QueryExpression?.Id, nameof(QueryParenthesisExpression.QueryExpression), queryExpression.Id);
                if (queryParenthesisExpression is not null)
                {
                    var child = GetOwnerLink(
                        model.QueryParenthesisExpressionQueryExpressionLinkList, static row => row.QueryParenthesisExpression?.Id, nameof(QueryParenthesisExpressionQueryExpressionLink.QueryParenthesisExpression),
                        queryParenthesisExpression.Id,
                        "QueryParenthesisExpression.QueryExpression").QueryExpression;
                    renderedCore = "(" + RenderQueryExpression(child) + ")";
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported MetaTransformScript QueryExpression id '{queryExpression.Id}'.");
                }
            }
        }

        var orderByClauseLink = FindOwnerLink(model.QueryExpressionOrderByClauseLinkList, static row => row.QueryExpression?.Id, nameof(QueryExpressionOrderByClauseLink.QueryExpression), queryExpression.Id);
        if (orderByClauseLink is not null)
        {
            renderedCore += Environment.NewLine + RenderOrderByClause(orderByClauseLink.OrderByClause);
        }

        var offsetClauseLink = FindOwnerLink(model.QueryExpressionOffsetClauseLinkList, static row => row.QueryExpression?.Id, nameof(QueryExpressionOffsetClauseLink.QueryExpression), queryExpression.Id);
        if (offsetClauseLink is not null)
        {
            renderedCore += Environment.NewLine + RenderOffsetClause(offsetClauseLink.OffsetClause);
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

        var topRowFilterLink = FindOwnerLink(model.QuerySpecificationTopRowFilterLinkList, static row => row.QuerySpecification?.Id, nameof(QuerySpecificationTopRowFilterLink.QuerySpecification), querySpecification.Id);
        if (topRowFilterLink is not null)
        {
            builder.Append(' ');
            builder.Append(RenderTopRowFilter(topRowFilterLink.TopRowFilter));
        }

        var selectElements = GetOrderedItems(model.QuerySpecificationSelectElementsItemList, static row => row.QuerySpecification?.Id, nameof(QuerySpecificationSelectElementsItem.QuerySpecification), static row => row.Ordinal, querySpecification.Id)
            .Select(row => RenderSelectElement(row.SelectElement))
            .ToArray();

        builder.AppendLine();
        builder.Append("    ");
        builder.Append(string.Join("," + Environment.NewLine + "    ", selectElements));

        var fromClauseLink = FindOwnerLink(model.QuerySpecificationFromClauseLinkList, static row => row.QuerySpecification?.Id, nameof(QuerySpecificationFromClauseLink.QuerySpecification), querySpecification.Id);
        if (fromClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("FROM ");
            builder.Append(RenderFromClause(fromClauseLink.FromClause));
        }

        var whereClauseLink = FindOwnerLink(model.QuerySpecificationWhereClauseLinkList, static row => row.QuerySpecification?.Id, nameof(QuerySpecificationWhereClauseLink.QuerySpecification), querySpecification.Id);
        if (whereClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("WHERE ");
            builder.Append(RenderBooleanExpression(GetOwnerLink(model.WhereClauseSearchConditionLinkList, static row => row.WhereClause?.Id, nameof(WhereClauseSearchConditionLink.WhereClause), whereClauseLink.WhereClause.Id, "WhereClause.SearchCondition").BooleanExpression));
        }

        var groupByClauseLink = FindOwnerLink(model.QuerySpecificationGroupByClauseLinkList, static row => row.QuerySpecification?.Id, nameof(QuerySpecificationGroupByClauseLink.QuerySpecification), querySpecification.Id);
        if (groupByClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("GROUP BY ");
            builder.Append(RenderGroupByClause(groupByClauseLink.GroupByClause));
        }

        var havingClauseLink = FindOwnerLink(model.QuerySpecificationHavingClauseLinkList, static row => row.QuerySpecification?.Id, nameof(QuerySpecificationHavingClauseLink.QuerySpecification), querySpecification.Id);
        if (havingClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append("HAVING ");
            builder.Append(RenderBooleanExpression(GetOwnerLink(model.HavingClauseSearchConditionLinkList, static row => row.HavingClause?.Id, nameof(HavingClauseSearchConditionLink.HavingClause), havingClauseLink.HavingClause.Id, "HavingClause.SearchCondition").BooleanExpression));
        }

        var windowClauseLink = FindOwnerLink(model.QuerySpecificationWindowClauseLinkList, static row => row.QuerySpecification?.Id, nameof(QuerySpecificationWindowClauseLink.QuerySpecification), querySpecification.Id);
        if (windowClauseLink is not null)
        {
            builder.AppendLine();
            builder.Append(RenderWindowClause(windowClauseLink.WindowClause));
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

        var groupingSpecifications = GetOrderedItems(model.GroupByClauseGroupingSpecificationsItemList, static row => row.GroupByClause?.Id, nameof(GroupByClauseGroupingSpecificationsItem.GroupByClause), static row => row.Ordinal, groupByClause.Id)
            .Select(row => RenderGroupingSpecification(row.GroupingSpecification))
            .ToArray();
        var rendered = string.Join(", ", groupingSpecifications);
        return IsTrue(groupByClause.All)
            ? "ALL " + rendered
            : rendered;
    }

    private string RenderGroupingSpecification(GroupingSpecification groupingSpecification)
    {
        var expressionGroupingSpecification = FindByBaseId(model.ExpressionGroupingSpecificationList, static row => row.GroupingSpecification?.Id, nameof(ExpressionGroupingSpecification.GroupingSpecification), groupingSpecification.Id);
        if (expressionGroupingSpecification is not null)
        {
            if (IsTrue(expressionGroupingSpecification.DistributedAggregation))
            {
                throw new InvalidOperationException("Phase-1 emitter does not support distributed aggregation grouping specifications.");
            }

            return RenderScalarExpression(GetOwnerLink(
                model.ExpressionGroupingSpecificationExpressionLinkList, static row => row.ExpressionGroupingSpecification?.Id, nameof(ExpressionGroupingSpecificationExpressionLink.ExpressionGroupingSpecification),
                expressionGroupingSpecification.Id,
                "ExpressionGroupingSpecification.Expression").ScalarExpression);
        }

        var groupingSetsGroupingSpecification = FindByBaseId(model.GroupingSetsGroupingSpecificationList, static row => row.GroupingSpecification?.Id, nameof(GroupingSetsGroupingSpecification.GroupingSpecification), groupingSpecification.Id);
        if (groupingSetsGroupingSpecification is not null)
        {
            var sets = GetOrderedItems(model.GroupingSetsGroupingSpecificationSetsItemList, static row => row.GroupingSetsGroupingSpecification?.Id, nameof(GroupingSetsGroupingSpecificationSetsItem.GroupingSetsGroupingSpecification), static row => row.Ordinal, groupingSetsGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.GroupingSpecification))
                .ToArray();
            return "GROUPING SETS (" + string.Join(", ", sets) + ")";
        }

        var rollupGroupingSpecification = FindByBaseId(model.RollupGroupingSpecificationList, static row => row.GroupingSpecification?.Id, nameof(RollupGroupingSpecification.GroupingSpecification), groupingSpecification.Id);
        if (rollupGroupingSpecification is not null)
        {
            var arguments = GetOrderedItems(model.RollupGroupingSpecificationArgumentsItemList, static row => row.RollupGroupingSpecification?.Id, nameof(RollupGroupingSpecificationArgumentsItem.RollupGroupingSpecification), static row => row.Ordinal, rollupGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.GroupingSpecification))
                .ToArray();
            return "ROLLUP (" + string.Join(", ", arguments) + ")";
        }

        var cubeGroupingSpecification = FindByBaseId(model.CubeGroupingSpecificationList, static row => row.GroupingSpecification?.Id, nameof(CubeGroupingSpecification.GroupingSpecification), groupingSpecification.Id);
        if (cubeGroupingSpecification is not null)
        {
            var arguments = GetOrderedItems(model.CubeGroupingSpecificationArgumentsItemList, static row => row.CubeGroupingSpecification?.Id, nameof(CubeGroupingSpecificationArgumentsItem.CubeGroupingSpecification), static row => row.Ordinal, cubeGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.GroupingSpecification))
                .ToArray();
            return "CUBE (" + string.Join(", ", arguments) + ")";
        }

        var compositeGroupingSpecification = FindByBaseId(model.CompositeGroupingSpecificationList, static row => row.GroupingSpecification?.Id, nameof(CompositeGroupingSpecification.GroupingSpecification), groupingSpecification.Id);
        if (compositeGroupingSpecification is not null)
        {
            var items = GetOrderedItems(model.CompositeGroupingSpecificationItemsItemList, static row => row.CompositeGroupingSpecification?.Id, nameof(CompositeGroupingSpecificationItemsItem.CompositeGroupingSpecification), static row => row.Ordinal, compositeGroupingSpecification.Id)
                .Select(row => RenderGroupingSpecification(row.GroupingSpecification))
                .ToArray();
            return "(" + string.Join(", ", items) + ")";
        }

        if (FindByBaseId(model.GrandTotalGroupingSpecificationList, static row => row.GroupingSpecification?.Id, nameof(GrandTotalGroupingSpecification.GroupingSpecification), groupingSpecification.Id) is not null)
        {
            return "()";
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript GroupingSpecification id '{groupingSpecification.Id}'.");
    }

    private string RenderWindowClause(WindowClause windowClause)
    {
        var definitions = GetOrderedItems(model.WindowClauseWindowDefinitionItemList, static row => row.WindowClause?.Id, nameof(WindowClauseWindowDefinitionItem.WindowClause), static row => row.Ordinal, windowClause.Id)
            .Select(row => RenderWindowDefinition(row.WindowDefinition))
            .ToArray();
        return "WINDOW" + Environment.NewLine + "    " + string.Join("," + Environment.NewLine + "    ", definitions);
    }

    private string RenderWindowDefinition(WindowDefinition windowDefinition)
    {
        var name = RenderIdentifier(GetOwnerLink(model.WindowDefinitionWindowNameLinkList, static row => row.WindowDefinition?.Id, nameof(WindowDefinitionWindowNameLink.WindowDefinition), windowDefinition.Id, "WindowDefinition.WindowName").Identifier);
        var parts = new List<string>();

        var refWindowNameLink = FindOwnerLink(model.WindowDefinitionRefWindowNameLinkList, static row => row.WindowDefinition?.Id, nameof(WindowDefinitionRefWindowNameLink.WindowDefinition), windowDefinition.Id);
        if (refWindowNameLink is not null)
        {
            parts.Add(RenderIdentifier(refWindowNameLink.Identifier));
        }

        var partitions = GetOrderedItems(model.WindowDefinitionPartitionsItemList, static row => row.WindowDefinition?.Id, nameof(WindowDefinitionPartitionsItem.WindowDefinition), static row => row.Ordinal, windowDefinition.Id)
            .Select(row => RenderScalarExpression(row.ScalarExpression))
            .ToArray();
        if (partitions.Length > 0)
        {
            parts.Add("PARTITION BY " + string.Join(", ", partitions));
        }

        var orderByClauseLink = FindOwnerLink(model.WindowDefinitionOrderByClauseLinkList, static row => row.WindowDefinition?.Id, nameof(WindowDefinitionOrderByClauseLink.WindowDefinition), windowDefinition.Id);
        if (orderByClauseLink is not null)
        {
            parts.Add(RenderOrderByClause(orderByClauseLink.OrderByClause));
        }

        var windowFrameClauseLink = FindOwnerLink(model.WindowDefinitionWindowFrameClauseLinkList, static row => row.WindowDefinition?.Id, nameof(WindowDefinitionWindowFrameClauseLink.WindowDefinition), windowDefinition.Id);
        if (windowFrameClauseLink is not null)
        {
            parts.Add(RenderWindowFrameClause(windowFrameClauseLink.WindowFrameClause));
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

        var windowNameLink = FindOwnerLink(model.OverClauseWindowNameLinkList, static row => row.OverClause?.Id, nameof(OverClauseWindowNameLink.OverClause), overClause.Id);
        if (windowNameLink is not null)
        {
            parts.Add(RenderIdentifier(windowNameLink.Identifier));
        }

        var partitions = GetOrderedItems(model.OverClausePartitionsItemList, static row => row.OverClause?.Id, nameof(OverClausePartitionsItem.OverClause), static row => row.Ordinal, overClause.Id)
            .Select(row => RenderScalarExpression(row.ScalarExpression))
            .ToArray();
        if (partitions.Length > 0)
        {
            parts.Add("PARTITION BY " + string.Join(", ", partitions));
        }

        var orderByClauseLink = FindOwnerLink(model.OverClauseOrderByClauseLinkList, static row => row.OverClause?.Id, nameof(OverClauseOrderByClauseLink.OverClause), overClause.Id);
        if (orderByClauseLink is not null)
        {
            parts.Add(RenderOrderByClause(orderByClauseLink.OrderByClause));
        }

        var windowFrameClauseLink = FindOwnerLink(model.OverClauseWindowFrameClauseLinkList, static row => row.OverClause?.Id, nameof(OverClauseWindowFrameClauseLink.OverClause), overClause.Id);
        if (windowFrameClauseLink is not null)
        {
            parts.Add(RenderWindowFrameClause(windowFrameClauseLink.WindowFrameClause));
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
        var elements = GetOrderedItems(model.OrderByClauseOrderByElementsItemList, static row => row.OrderByClause?.Id, nameof(OrderByClauseOrderByElementsItem.OrderByClause), static row => row.Ordinal, orderByClause.Id)
            .Select(row => RenderExpressionWithSortOrder(row.ExpressionWithSortOrder))
            .ToArray();
        return "ORDER BY " + string.Join(", ", elements);
    }

    private string RenderExpressionWithSortOrder(ExpressionWithSortOrder expressionWithSortOrder)
    {
        var rendered = RenderScalarExpression(GetOwnerLink(
            model.ExpressionWithSortOrderExpressionLinkList, static row => row.ExpressionWithSortOrder?.Id, nameof(ExpressionWithSortOrderExpressionLink.ExpressionWithSortOrder),
            expressionWithSortOrder.Id,
            "ExpressionWithSortOrder.Expression").ScalarExpression);

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
            model.TopRowFilterExpressionLinkList, static row => row.TopRowFilter?.Id, nameof(TopRowFilterExpressionLink.TopRowFilter),
            topRowFilter.Id,
            "TopRowFilter.Expression").ScalarExpression);

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
            model.OffsetClauseOffsetExpressionLinkList, static row => row.OffsetClause?.Id, nameof(OffsetClauseOffsetExpressionLink.OffsetClause),
            offsetClause.Id,
            "OffsetClause.OffsetExpression").ScalarExpression);
        var rendered = "OFFSET " + offsetExpression + " ROWS";

        var fetchExpressionLink = FindOwnerLink(model.OffsetClauseFetchExpressionLinkList, static row => row.OffsetClause?.Id, nameof(OffsetClauseFetchExpressionLink.OffsetClause), offsetClause.Id);
        if (fetchExpressionLink is not null)
        {
            rendered += " FETCH NEXT " + RenderScalarExpression(fetchExpressionLink.ScalarExpression) + " ROWS ONLY";
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

        var top = RenderWindowDelimiter(GetOwnerLink(model.WindowFrameClauseTopLinkList, static row => row.WindowFrameClause?.Id, nameof(WindowFrameClauseTopLink.WindowFrameClause), windowFrameClause.Id, "WindowFrameClause.Top").WindowDelimiter);
        var bottomLink = FindOwnerLink(model.WindowFrameClauseBottomLinkList, static row => row.WindowFrameClause?.Id, nameof(WindowFrameClauseBottomLink.WindowFrameClause), windowFrameClause.Id);
        return bottomLink is null
            ? $"{frameType} {top}"
            : $"{frameType} BETWEEN {top} AND {RenderWindowDelimiter(bottomLink.WindowDelimiter)}";
    }

    private string RenderWindowDelimiter(WindowDelimiter windowDelimiter)
    {
        return windowDelimiter.WindowDelimiterType switch
        {
            "CurrentRow" => "CURRENT ROW",
            "UnboundedPreceding" => "UNBOUNDED PRECEDING",
            "ValuePreceding" => RenderScalarExpression(GetOwnerLink(
                model.WindowDelimiterOffsetValueLinkList, static row => row.WindowDelimiter?.Id, nameof(WindowDelimiterOffsetValueLink.WindowDelimiter),
                windowDelimiter.Id,
                "WindowDelimiter.OffsetValue").ScalarExpression) + " PRECEDING",
            "ValueFollowing" => RenderScalarExpression(GetOwnerLink(
                model.WindowDelimiterOffsetValueLinkList, static row => row.WindowDelimiter?.Id, nameof(WindowDelimiterOffsetValueLink.WindowDelimiter),
                windowDelimiter.Id,
                "WindowDelimiter.OffsetValue").ScalarExpression) + " FOLLOWING",
            "UnboundedFollowing" => "UNBOUNDED FOLLOWING",
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript WindowDelimiterType '{windowDelimiter.WindowDelimiterType}'.")
        };
    }
}
