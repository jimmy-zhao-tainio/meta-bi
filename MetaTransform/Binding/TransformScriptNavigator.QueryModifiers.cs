using System;
using System.Linq;
using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformScriptNavigator
{
    public bool HasDistinctRowFilter(QuerySpecification querySpecification) =>
        string.Equals(querySpecification.UniqueRowFilter, "Distinct", StringComparison.OrdinalIgnoreCase);

    public bool HasUnsupportedUniqueRowFilter(QuerySpecification querySpecification) =>
        !string.IsNullOrWhiteSpace(querySpecification.UniqueRowFilter) &&
        !HasDistinctRowFilter(querySpecification);

    public TopRowFilter? TryGetTopRowFilter(QuerySpecification querySpecification)
    {
        var link = model.QuerySpecificationTopRowFilterLinkList
            .FirstOrDefault(item => string.Equals(item.QuerySpecificationId, querySpecification.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.TopRowFilterList.FirstOrDefault(item => string.Equals(item.Id, link.TopRowFilterId, StringComparison.Ordinal));
    }

    public bool IsTopRowFilterWithTies(TopRowFilter topRowFilter) =>
        string.Equals(topRowFilter.WithTies, "true", StringComparison.OrdinalIgnoreCase);

    public ScalarExpression? TryGetTopRowFilterExpression(TopRowFilter topRowFilter)
    {
        var link = model.TopRowFilterExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.TopRowFilterId, topRowFilter.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.ScalarExpressionList.FirstOrDefault(item => string.Equals(item.Id, link.ScalarExpressionId, StringComparison.Ordinal));
    }

    public OrderByClause? TryGetQueryExpressionOrderByClause(string queryExpressionId)
    {
        var link = model.QueryExpressionOrderByClauseLinkList
            .FirstOrDefault(item => string.Equals(item.QueryExpressionId, queryExpressionId, StringComparison.Ordinal));
        return link is null
            ? null
            : model.OrderByClauseList.FirstOrDefault(item => string.Equals(item.Id, link.OrderByClauseId, StringComparison.Ordinal));
    }

    public OffsetClause? TryGetQueryExpressionOffsetClause(string queryExpressionId)
    {
        var link = model.QueryExpressionOffsetClauseLinkList
            .FirstOrDefault(item => string.Equals(item.QueryExpressionId, queryExpressionId, StringComparison.Ordinal));
        return link is null
            ? null
            : model.OffsetClauseList.FirstOrDefault(item => string.Equals(item.Id, link.OffsetClauseId, StringComparison.Ordinal));
    }

    public ScalarExpression? TryGetOffsetClauseOffsetExpression(OffsetClause offsetClause)
    {
        var link = model.OffsetClauseOffsetExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.OffsetClauseId, offsetClause.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.ScalarExpressionList.FirstOrDefault(item => string.Equals(item.Id, link.ScalarExpressionId, StringComparison.Ordinal));
    }

    public ScalarExpression? TryGetOffsetClauseFetchExpression(OffsetClause offsetClause)
    {
        var link = model.OffsetClauseFetchExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.OffsetClauseId, offsetClause.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : model.ScalarExpressionList.FirstOrDefault(item => string.Equals(item.Id, link.ScalarExpressionId, StringComparison.Ordinal));
    }
}
