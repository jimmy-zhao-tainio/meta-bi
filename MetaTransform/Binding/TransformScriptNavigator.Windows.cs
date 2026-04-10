using System;
using System.Linq;
using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformScriptNavigator
{
    public OrderByClause? TryGetFunctionCallWithinGroupOrderByClause(FunctionCall functionCall)
    {
        var link = model.FunctionCallWithinGroupOrderByClauseLinkList
            .FirstOrDefault(item => string.Equals(item.OwnerId, functionCall.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.OrderByClauseList.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal));
    }

    public OverClause? TryGetFunctionCallOverClause(FunctionCall functionCall)
    {
        return functionCallOverClauseLinkByOwnerId.TryGetValue(functionCall.Id, out var link)
            ? model.OverClauseList.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal))
            : null;
    }

    public WindowClause? TryGetWindowClause(QuerySpecification querySpecification)
    {
        var link = model.QuerySpecificationWindowClauseLinkList
            .FirstOrDefault(item => string.Equals(item.OwnerId, querySpecification.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.WindowClauseList.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal));
    }

    public IReadOnlyList<WindowDefinition> GetWindowDefinitions(WindowClause windowClause)
    {
        return model.WindowClauseWindowDefinitionItemList
            .Where(item => string.Equals(item.OwnerId, windowClause.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => model.WindowDefinitionList.FirstOrDefault(value => string.Equals(value.Id, item.ValueId, StringComparison.Ordinal)))
            .Where(item => item is not null)
            .Cast<WindowDefinition>()
            .ToArray();
    }

    public IReadOnlyList<ScalarExpression> GetOverClausePartitions(OverClause overClause)
    {
        return model.OverClausePartitionsItemList
            .Where(item => string.Equals(item.OwnerId, overClause.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => scalarExpressionById.GetValueOrDefault(item.ValueId))
            .Where(item => item is not null)
            .Cast<ScalarExpression>()
            .ToArray();
    }

    public OrderByClause? TryGetOverClauseOrderByClause(OverClause overClause)
    {
        var link = model.OverClauseOrderByClauseLinkList
            .FirstOrDefault(item => string.Equals(item.OwnerId, overClause.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.OrderByClauseList.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal));
    }

    public WindowFrameClause? TryGetOverClauseWindowFrameClause(OverClause overClause)
    {
        var link = model.OverClauseWindowFrameClauseLinkList
            .FirstOrDefault(item => string.Equals(item.OwnerId, overClause.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.WindowFrameClauseList.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal));
    }

    public IReadOnlyList<ScalarExpression> GetWindowDefinitionPartitions(WindowDefinition windowDefinition)
    {
        return model.WindowDefinitionPartitionsItemList
            .Where(item => string.Equals(item.OwnerId, windowDefinition.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => scalarExpressionById.GetValueOrDefault(item.ValueId))
            .Where(item => item is not null)
            .Cast<ScalarExpression>()
            .ToArray();
    }

    public OrderByClause? TryGetWindowDefinitionOrderByClause(WindowDefinition windowDefinition)
    {
        var link = model.WindowDefinitionOrderByClauseLinkList
            .FirstOrDefault(item => string.Equals(item.OwnerId, windowDefinition.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.OrderByClauseList.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal));
    }

    public WindowFrameClause? TryGetWindowDefinitionWindowFrameClause(WindowDefinition windowDefinition)
    {
        var link = model.WindowDefinitionWindowFrameClauseLinkList
            .FirstOrDefault(item => string.Equals(item.OwnerId, windowDefinition.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.WindowFrameClauseList.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal));
    }

    public IReadOnlyList<ExpressionWithSortOrder> GetOrderByElements(OrderByClause orderByClause)
    {
        return model.OrderByClauseOrderByElementsItemList
            .Where(item => string.Equals(item.OwnerId, orderByClause.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => model.ExpressionWithSortOrderList.FirstOrDefault(value => string.Equals(value.Id, item.ValueId, StringComparison.Ordinal)))
            .Where(item => item is not null)
            .Cast<ExpressionWithSortOrder>()
            .ToArray();
    }

    public ScalarExpression? TryGetExpressionWithSortOrderExpression(ExpressionWithSortOrder expressionWithSortOrder)
    {
        var link = model.ExpressionWithSortOrderExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.OwnerId, expressionWithSortOrder.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : scalarExpressionById.GetValueOrDefault(link.ValueId);
    }

    public WindowDelimiter? TryGetWindowFrameClauseTop(WindowFrameClause windowFrameClause)
    {
        var link = model.WindowFrameClauseTopLinkList
            .FirstOrDefault(item => string.Equals(item.OwnerId, windowFrameClause.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.WindowDelimiterList.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal));
    }

    public WindowDelimiter? TryGetWindowFrameClauseBottom(WindowFrameClause windowFrameClause)
    {
        var link = model.WindowFrameClauseBottomLinkList
            .FirstOrDefault(item => string.Equals(item.OwnerId, windowFrameClause.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.WindowDelimiterList.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal));
    }

    public ScalarExpression? TryGetWindowDelimiterOffsetValue(WindowDelimiter windowDelimiter)
    {
        var link = model.WindowDelimiterOffsetValueLinkList
            .FirstOrDefault(item => string.Equals(item.OwnerId, windowDelimiter.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : scalarExpressionById.GetValueOrDefault(link.ValueId);
    }
}
