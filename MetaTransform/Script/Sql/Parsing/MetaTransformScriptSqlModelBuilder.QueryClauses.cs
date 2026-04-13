using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    public BuiltNode CreateExpressionGroupingSpecification(BuiltNode expression)
    {
        var groupingSpecification = new GroupingSpecification
        {
            Id = NextId(nameof(GroupingSpecification))
        };
        model.GroupingSpecificationList.Add(groupingSpecification);

        var expressionGroupingSpecification = new ExpressionGroupingSpecification
        {
            Id = NextId(nameof(ExpressionGroupingSpecification)),
            GroupingSpecificationId = groupingSpecification.Id
        };
        model.ExpressionGroupingSpecificationList.Add(expressionGroupingSpecification);
        model.ExpressionGroupingSpecificationExpressionLinkList.Add(new ExpressionGroupingSpecificationExpressionLink
        {
            Id = NextId(nameof(ExpressionGroupingSpecificationExpressionLink)),
            ExpressionGroupingSpecificationId = expressionGroupingSpecification.Id,
            ScalarExpressionId = expression.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create(
            (nameof(GroupingSpecification), groupingSpecification.Id),
            (nameof(ExpressionGroupingSpecification), expressionGroupingSpecification.Id));
    }

    public BuiltNode CreateGroupingSetsGroupingSpecification(IReadOnlyList<BuiltNode> sets)
    {
        var groupingSpecification = new GroupingSpecification
        {
            Id = NextId(nameof(GroupingSpecification))
        };
        model.GroupingSpecificationList.Add(groupingSpecification);

        var groupingSetsGroupingSpecification = new GroupingSetsGroupingSpecification
        {
            Id = NextId(nameof(GroupingSetsGroupingSpecification)),
            GroupingSpecificationId = groupingSpecification.Id
        };
        model.GroupingSetsGroupingSpecificationList.Add(groupingSetsGroupingSpecification);

        for (var ordinal = 0; ordinal < sets.Count; ordinal++)
        {
            model.GroupingSetsGroupingSpecificationSetsItemList.Add(new GroupingSetsGroupingSpecificationSetsItem
            {
                Id = NextId(nameof(GroupingSetsGroupingSpecificationSetsItem)),
                GroupingSetsGroupingSpecificationId = groupingSetsGroupingSpecification.Id,
                GroupingSpecificationId = sets[ordinal].GetId(nameof(GroupingSpecification)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(GroupingSpecification), groupingSpecification.Id),
            (nameof(GroupingSetsGroupingSpecification), groupingSetsGroupingSpecification.Id));
    }

    public BuiltNode CreateRollupGroupingSpecification(IReadOnlyList<BuiltNode> arguments)
    {
        var groupingSpecification = new GroupingSpecification
        {
            Id = NextId(nameof(GroupingSpecification))
        };
        model.GroupingSpecificationList.Add(groupingSpecification);

        var rollupGroupingSpecification = new RollupGroupingSpecification
        {
            Id = NextId(nameof(RollupGroupingSpecification)),
            GroupingSpecificationId = groupingSpecification.Id
        };
        model.RollupGroupingSpecificationList.Add(rollupGroupingSpecification);

        for (var ordinal = 0; ordinal < arguments.Count; ordinal++)
        {
            model.RollupGroupingSpecificationArgumentsItemList.Add(new RollupGroupingSpecificationArgumentsItem
            {
                Id = NextId(nameof(RollupGroupingSpecificationArgumentsItem)),
                RollupGroupingSpecificationId = rollupGroupingSpecification.Id,
                GroupingSpecificationId = arguments[ordinal].GetId(nameof(GroupingSpecification)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(GroupingSpecification), groupingSpecification.Id),
            (nameof(RollupGroupingSpecification), rollupGroupingSpecification.Id));
    }

    public BuiltNode CreateCubeGroupingSpecification(IReadOnlyList<BuiltNode> arguments)
    {
        var groupingSpecification = new GroupingSpecification
        {
            Id = NextId(nameof(GroupingSpecification))
        };
        model.GroupingSpecificationList.Add(groupingSpecification);

        var cubeGroupingSpecification = new CubeGroupingSpecification
        {
            Id = NextId(nameof(CubeGroupingSpecification)),
            GroupingSpecificationId = groupingSpecification.Id
        };
        model.CubeGroupingSpecificationList.Add(cubeGroupingSpecification);

        for (var ordinal = 0; ordinal < arguments.Count; ordinal++)
        {
            model.CubeGroupingSpecificationArgumentsItemList.Add(new CubeGroupingSpecificationArgumentsItem
            {
                Id = NextId(nameof(CubeGroupingSpecificationArgumentsItem)),
                CubeGroupingSpecificationId = cubeGroupingSpecification.Id,
                GroupingSpecificationId = arguments[ordinal].GetId(nameof(GroupingSpecification)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(GroupingSpecification), groupingSpecification.Id),
            (nameof(CubeGroupingSpecification), cubeGroupingSpecification.Id));
    }

    public BuiltNode CreateCompositeGroupingSpecification(IReadOnlyList<BuiltNode> items)
    {
        var groupingSpecification = new GroupingSpecification
        {
            Id = NextId(nameof(GroupingSpecification))
        };
        model.GroupingSpecificationList.Add(groupingSpecification);

        var compositeGroupingSpecification = new CompositeGroupingSpecification
        {
            Id = NextId(nameof(CompositeGroupingSpecification)),
            GroupingSpecificationId = groupingSpecification.Id
        };
        model.CompositeGroupingSpecificationList.Add(compositeGroupingSpecification);

        for (var ordinal = 0; ordinal < items.Count; ordinal++)
        {
            model.CompositeGroupingSpecificationItemsItemList.Add(new CompositeGroupingSpecificationItemsItem
            {
                Id = NextId(nameof(CompositeGroupingSpecificationItemsItem)),
                CompositeGroupingSpecificationId = compositeGroupingSpecification.Id,
                GroupingSpecificationId = items[ordinal].GetId(nameof(GroupingSpecification)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(GroupingSpecification), groupingSpecification.Id),
            (nameof(CompositeGroupingSpecification), compositeGroupingSpecification.Id));
    }

    public BuiltNode CreateGrandTotalGroupingSpecification()
    {
        var groupingSpecification = new GroupingSpecification
        {
            Id = NextId(nameof(GroupingSpecification))
        };
        model.GroupingSpecificationList.Add(groupingSpecification);

        var grandTotalGroupingSpecification = new GrandTotalGroupingSpecification
        {
            Id = NextId(nameof(GrandTotalGroupingSpecification)),
            GroupingSpecificationId = groupingSpecification.Id
        };
        model.GrandTotalGroupingSpecificationList.Add(grandTotalGroupingSpecification);

        return BuiltNode.Create(
            (nameof(GroupingSpecification), groupingSpecification.Id),
            (nameof(GrandTotalGroupingSpecification), grandTotalGroupingSpecification.Id));
    }

    public BuiltNode CreateGroupByClause(IReadOnlyList<BuiltNode> groupingSpecifications, bool all = false)
    {
        var groupByClause = new GroupByClause
        {
            Id = NextId(nameof(GroupByClause)),
            All = all ? "true" : string.Empty
        };
        model.GroupByClauseList.Add(groupByClause);

        for (var ordinal = 0; ordinal < groupingSpecifications.Count; ordinal++)
        {
            model.GroupByClauseGroupingSpecificationsItemList.Add(new GroupByClauseGroupingSpecificationsItem
            {
                Id = NextId(nameof(GroupByClauseGroupingSpecificationsItem)),
                GroupByClauseId = groupByClause.Id,
                GroupingSpecificationId = groupingSpecifications[ordinal].GetId(nameof(GroupingSpecification)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(GroupByClause), groupByClause.Id));
    }

    public BuiltNode CreateTopRowFilter(BuiltNode expression, bool percent, bool withTies)
    {
        var topRowFilter = new TopRowFilter
        {
            Id = NextId(nameof(TopRowFilter)),
            Percent = percent ? "true" : string.Empty,
            WithTies = withTies ? "true" : string.Empty
        };
        model.TopRowFilterList.Add(topRowFilter);
        model.TopRowFilterExpressionLinkList.Add(new TopRowFilterExpressionLink
        {
            Id = NextId(nameof(TopRowFilterExpressionLink)),
            TopRowFilterId = topRowFilter.Id,
            ScalarExpressionId = expression.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create((nameof(TopRowFilter), topRowFilter.Id));
    }

    public BuiltNode CreateExpressionWithSortOrder(BuiltNode expression, string sortOrder)
    {
        var expressionWithSortOrder = new ExpressionWithSortOrder
        {
            Id = NextId(nameof(ExpressionWithSortOrder)),
            SortOrder = sortOrder
        };
        model.ExpressionWithSortOrderList.Add(expressionWithSortOrder);
        model.ExpressionWithSortOrderExpressionLinkList.Add(new ExpressionWithSortOrderExpressionLink
        {
            Id = NextId(nameof(ExpressionWithSortOrderExpressionLink)),
            ExpressionWithSortOrderId = expressionWithSortOrder.Id,
            ScalarExpressionId = expression.GetId(nameof(ScalarExpression))
        });

        return BuiltNode.Create((nameof(ExpressionWithSortOrder), expressionWithSortOrder.Id));
    }

    public BuiltNode CreateOrderByClause(IReadOnlyList<BuiltNode> orderByElements)
    {
        var orderByClause = new OrderByClause
        {
            Id = NextId(nameof(OrderByClause))
        };
        model.OrderByClauseList.Add(orderByClause);

        for (var ordinal = 0; ordinal < orderByElements.Count; ordinal++)
        {
            model.OrderByClauseOrderByElementsItemList.Add(new OrderByClauseOrderByElementsItem
            {
                Id = NextId(nameof(OrderByClauseOrderByElementsItem)),
                OrderByClauseId = orderByClause.Id,
                ExpressionWithSortOrderId = orderByElements[ordinal].GetId(nameof(ExpressionWithSortOrder)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(OrderByClause), orderByClause.Id));
    }

    public BuiltNode CreateOffsetClause(BuiltNode offsetExpression, BuiltNode? fetchExpression = null, bool withApproximate = false)
    {
        var offsetClause = new OffsetClause
        {
            Id = NextId(nameof(OffsetClause)),
            WithApproximate = withApproximate ? "true" : string.Empty
        };
        model.OffsetClauseList.Add(offsetClause);
        model.OffsetClauseOffsetExpressionLinkList.Add(new OffsetClauseOffsetExpressionLink
        {
            Id = NextId(nameof(OffsetClauseOffsetExpressionLink)),
            OffsetClauseId = offsetClause.Id,
            ScalarExpressionId = offsetExpression.GetId(nameof(ScalarExpression))
        });

        if (fetchExpression is not null)
        {
            model.OffsetClauseFetchExpressionLinkList.Add(new OffsetClauseFetchExpressionLink
            {
                Id = NextId(nameof(OffsetClauseFetchExpressionLink)),
                OffsetClauseId = offsetClause.Id,
                ScalarExpressionId = fetchExpression.GetId(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create((nameof(OffsetClause), offsetClause.Id));
    }

    public BuiltNode AttachWithinGroupOrderByClause(BuiltNode functionCall, BuiltNode orderByClause)
    {
        model.FunctionCallWithinGroupOrderByClauseLinkList.Add(new FunctionCallWithinGroupOrderByClauseLink
        {
            Id = NextId(nameof(FunctionCallWithinGroupOrderByClauseLink)),
            FunctionCallId = functionCall.GetId(nameof(FunctionCall)),
            OrderByClauseId = orderByClause.GetId(nameof(OrderByClause))
        });

        return functionCall;
    }

    public BuiltNode CreateWhereClause(BuiltNode searchCondition)
    {
        var row = new WhereClause
        {
            Id = NextId(nameof(WhereClause))
        };
        model.WhereClauseList.Add(row);
        model.WhereClauseSearchConditionLinkList.Add(new WhereClauseSearchConditionLink
        {
            Id = NextId(nameof(WhereClauseSearchConditionLink)),
            WhereClauseId = row.Id,
            BooleanExpressionId = searchCondition.GetId(nameof(BooleanExpression))
        });
        return BuiltNode.Create((nameof(WhereClause), row.Id));
    }

    public BuiltNode CreateHavingClause(BuiltNode searchCondition)
    {
        var row = new HavingClause
        {
            Id = NextId(nameof(HavingClause))
        };
        model.HavingClauseList.Add(row);
        model.HavingClauseSearchConditionLinkList.Add(new HavingClauseSearchConditionLink
        {
            Id = NextId(nameof(HavingClauseSearchConditionLink)),
            HavingClauseId = row.Id,
            BooleanExpressionId = searchCondition.GetId(nameof(BooleanExpression))
        });
        return BuiltNode.Create((nameof(HavingClause), row.Id));
    }

    public BuiltNode CreateQuerySpecification(
        IReadOnlyList<BuiltNode> selectElements,
        BuiltNode? fromClause = null,
        BuiltNode? whereClause = null,
        BuiltNode? groupByClause = null,
        BuiltNode? havingClause = null,
        BuiltNode? topRowFilter = null,
        BuiltNode? windowClause = null,
        string? uniqueRowFilter = null)
    {
        var queryExpression = new QueryExpression
        {
            Id = NextId(nameof(QueryExpression))
        };
        model.QueryExpressionList.Add(queryExpression);

        var specification = new QuerySpecification
        {
            Id = NextId(nameof(QuerySpecification)),
            QueryExpressionId = queryExpression.Id,
            UniqueRowFilter = uniqueRowFilter ?? string.Empty
        };
        model.QuerySpecificationList.Add(specification);

        for (var ordinal = 0; ordinal < selectElements.Count; ordinal++)
        {
            model.QuerySpecificationSelectElementsItemList.Add(new QuerySpecificationSelectElementsItem
            {
                Id = NextId(nameof(QuerySpecificationSelectElementsItem)),
                QuerySpecificationId = specification.Id,
                SelectElementId = selectElements[ordinal].GetId(nameof(SelectElement)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        if (fromClause is not null)
        {
            model.QuerySpecificationFromClauseLinkList.Add(new QuerySpecificationFromClauseLink
            {
                Id = NextId(nameof(QuerySpecificationFromClauseLink)),
                QuerySpecificationId = specification.Id,
                FromClauseId = fromClause.GetId(nameof(FromClause))
            });
        }

        if (whereClause is not null)
        {
            model.QuerySpecificationWhereClauseLinkList.Add(new QuerySpecificationWhereClauseLink
            {
                Id = NextId(nameof(QuerySpecificationWhereClauseLink)),
                QuerySpecificationId = specification.Id,
                WhereClauseId = whereClause.GetId(nameof(WhereClause))
            });
        }

        if (groupByClause is not null)
        {
            model.QuerySpecificationGroupByClauseLinkList.Add(new QuerySpecificationGroupByClauseLink
            {
                Id = NextId(nameof(QuerySpecificationGroupByClauseLink)),
                QuerySpecificationId = specification.Id,
                GroupByClauseId = groupByClause.GetId(nameof(GroupByClause))
            });
        }

        if (havingClause is not null)
        {
            model.QuerySpecificationHavingClauseLinkList.Add(new QuerySpecificationHavingClauseLink
            {
                Id = NextId(nameof(QuerySpecificationHavingClauseLink)),
                QuerySpecificationId = specification.Id,
                HavingClauseId = havingClause.GetId(nameof(HavingClause))
            });
        }

        if (topRowFilter is not null)
        {
            model.QuerySpecificationTopRowFilterLinkList.Add(new QuerySpecificationTopRowFilterLink
            {
                Id = NextId(nameof(QuerySpecificationTopRowFilterLink)),
                QuerySpecificationId = specification.Id,
                TopRowFilterId = topRowFilter.GetId(nameof(TopRowFilter))
            });
        }

        if (windowClause is not null)
        {
            model.QuerySpecificationWindowClauseLinkList.Add(new QuerySpecificationWindowClauseLink
            {
                Id = NextId(nameof(QuerySpecificationWindowClauseLink)),
                QuerySpecificationId = specification.Id,
                WindowClauseId = windowClause.GetId(nameof(WindowClause))
            });
        }

        return BuiltNode.Create(
            (nameof(QueryExpression), queryExpression.Id),
            (nameof(QuerySpecification), specification.Id));
    }

    public BuiltNode AttachOffsetClause(BuiltNode queryExpression, BuiltNode offsetClause)
    {
        model.QueryExpressionOffsetClauseLinkList.Add(new QueryExpressionOffsetClauseLink
        {
            Id = NextId(nameof(QueryExpressionOffsetClauseLink)),
            QueryExpressionId = queryExpression.GetId(nameof(QueryExpression)),
            OffsetClauseId = offsetClause.GetId(nameof(OffsetClause))
        });

        return queryExpression;
    }
}
