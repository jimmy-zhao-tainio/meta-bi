using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformScriptNavigator
{
    public QueryParenthesisExpression? TryGetQueryParenthesisExpression(string queryExpressionId) =>
        queryParenthesisExpressionByQueryExpressionId.GetValueOrDefault(queryExpressionId);

    public string? TryGetQueryParenthesisExpressionInnerQueryExpressionId(QueryParenthesisExpression queryParenthesisExpression)
    {
        return queryParenthesisExpressionQueryExpressionLinkByOwnerId.TryGetValue(queryParenthesisExpression.Id, out var link)
            ? link.QueryExpressionId
            : null;
    }

    public TableReference? TryGetJoinParenthesisInnerJoinReference(TableReference tableReference)
    {
        if (!joinParenthesisTableReferenceByTableReferenceId.TryGetValue(tableReference.Id, out var joinParenthesis))
        {
            return null;
        }

        return joinParenthesisTableReferenceJoinLinkByOwnerId.TryGetValue(joinParenthesis.Id, out var link)
            ? tableReferenceById.GetValueOrDefault(link.TableReferenceId)
            : null;
    }
}
