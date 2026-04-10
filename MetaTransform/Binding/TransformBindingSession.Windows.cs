using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private void BindQueryWindowClause(
        QuerySpecification querySpecification,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeGroupingContext? groupingContext)
    {
        var windowClause = navigator.TryGetWindowClause(querySpecification);
        if (windowClause is null)
        {
            return;
        }

        foreach (var windowDefinition in navigator.GetWindowDefinitions(windowClause))
        {
            BindWindowDefinition(windowDefinition, scope, inputRowset, groupingContext);
        }
    }

    private void BindFunctionCallWindowClauses(
        FunctionCall functionCall,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeGroupingContext? groupingContext)
    {
        var withinGroupOrderByClause = navigator.TryGetFunctionCallWithinGroupOrderByClause(functionCall);
        if (withinGroupOrderByClause is not null)
        {
            BindOrderByClause(withinGroupOrderByClause, scope, inputRowset, groupingContext);
        }

        var overClause = navigator.TryGetFunctionCallOverClause(functionCall);
        if (overClause is not null)
        {
            BindOverClause(overClause, scope, inputRowset, groupingContext);
        }
    }

    private void BindWindowDefinition(
        WindowDefinition windowDefinition,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeGroupingContext? groupingContext)
    {
        foreach (var partition in navigator.GetWindowDefinitionPartitions(windowDefinition))
        {
            BindScalarExpression(partition, scope, inputRowset, groupingContext, withinAggregate: false);
        }

        var orderByClause = navigator.TryGetWindowDefinitionOrderByClause(windowDefinition);
        if (orderByClause is not null)
        {
            BindOrderByClause(orderByClause, scope, inputRowset, groupingContext);
        }

        var windowFrameClause = navigator.TryGetWindowDefinitionWindowFrameClause(windowDefinition);
        if (windowFrameClause is not null)
        {
            BindWindowFrameClause(windowFrameClause, scope, inputRowset, groupingContext);
        }
    }

    private void BindOverClause(
        OverClause overClause,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeGroupingContext? groupingContext)
    {
        foreach (var partition in navigator.GetOverClausePartitions(overClause))
        {
            BindScalarExpression(partition, scope, inputRowset, groupingContext, withinAggregate: false);
        }

        var orderByClause = navigator.TryGetOverClauseOrderByClause(overClause);
        if (orderByClause is not null)
        {
            BindOrderByClause(orderByClause, scope, inputRowset, groupingContext);
        }

        var windowFrameClause = navigator.TryGetOverClauseWindowFrameClause(overClause);
        if (windowFrameClause is not null)
        {
            BindWindowFrameClause(windowFrameClause, scope, inputRowset, groupingContext);
        }
    }

    private void BindOrderByClause(
        OrderByClause orderByClause,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeGroupingContext? groupingContext)
    {
        foreach (var orderByElement in navigator.GetOrderByElements(orderByClause))
        {
            var expression = navigator.TryGetExpressionWithSortOrderExpression(orderByElement);
            if (expression is not null)
            {
                BindScalarExpression(expression, scope, inputRowset, groupingContext, withinAggregate: false);
            }
        }
    }

    private void BindWindowFrameClause(
        WindowFrameClause windowFrameClause,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeGroupingContext? groupingContext)
    {
        var top = navigator.TryGetWindowFrameClauseTop(windowFrameClause);
        if (top is not null)
        {
            BindWindowDelimiter(top, scope, inputRowset, groupingContext);
        }

        var bottom = navigator.TryGetWindowFrameClauseBottom(windowFrameClause);
        if (bottom is not null)
        {
            BindWindowDelimiter(bottom, scope, inputRowset, groupingContext);
        }
    }

    private void BindWindowDelimiter(
        WindowDelimiter windowDelimiter,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeGroupingContext? groupingContext)
    {
        var offsetValue = navigator.TryGetWindowDelimiterOffsetValue(windowDelimiter);
        if (offsetValue is not null)
        {
            BindScalarExpression(offsetValue, scope, inputRowset, groupingContext, withinAggregate: false);
        }
    }
}
