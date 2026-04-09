using System;
using System.Linq;
using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformScriptNavigator
{
    public GroupByClause? TryGetGroupByClause(QuerySpecification querySpecification)
    {
        if (!groupByClauseLinkByOwnerId.TryGetValue(querySpecification.Id, out var link))
        {
            return null;
        }

        return groupByClauseById.GetValueOrDefault(link.ValueId);
    }

    public IReadOnlyList<GroupingSpecification> GetGroupingSpecifications(GroupByClause groupByClause)
    {
        if (!groupingSpecificationsByGroupByClauseOwnerId.TryGetValue(groupByClause.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => groupingSpecificationById.GetValueOrDefault(item.ValueId))
            .Where(item => item is not null)
            .Cast<GroupingSpecification>()
            .ToArray();
    }

    public ExpressionGroupingSpecification? TryGetExpressionGroupingSpecification(GroupingSpecification groupingSpecification) =>
        expressionGroupingSpecificationByBaseId.GetValueOrDefault(groupingSpecification.Id);

    public ScalarExpression? TryGetExpressionGroupingSpecificationExpression(ExpressionGroupingSpecification expressionGroupingSpecification)
    {
        return expressionGroupingSpecificationExpressionLinkByOwnerId.TryGetValue(expressionGroupingSpecification.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public BooleanExpression? TryGetWhereSearchCondition(QuerySpecification querySpecification)
    {
        if (!whereClauseLinkByOwnerId.TryGetValue(querySpecification.Id, out var whereClauseLink))
        {
            return null;
        }

        if (!whereClauseById.TryGetValue(whereClauseLink.ValueId, out var whereClause))
        {
            return null;
        }

        return whereClauseSearchConditionLinkByOwnerId.TryGetValue(whereClause.Id, out var searchConditionLink)
            ? new BooleanExpression { Id = searchConditionLink.ValueId }
            : null;
    }

    public BooleanExpression? TryGetHavingSearchCondition(QuerySpecification querySpecification)
    {
        if (!havingClauseLinkByOwnerId.TryGetValue(querySpecification.Id, out var havingClauseLink))
        {
            return null;
        }

        if (!havingClauseById.TryGetValue(havingClauseLink.ValueId, out var havingClause))
        {
            return null;
        }

        return havingClauseSearchConditionLinkByOwnerId.TryGetValue(havingClause.Id, out var searchConditionLink)
            ? new BooleanExpression { Id = searchConditionLink.ValueId }
            : null;
    }

    public ScalarSubquery? TryGetScalarSubquery(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return scalarSubqueryByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public string? TryGetScalarSubqueryQueryExpressionId(ScalarSubquery scalarSubquery)
    {
        return scalarSubqueryQueryExpressionLinkByOwnerId.TryGetValue(scalarSubquery.Id, out var link)
            ? link.ValueId
            : null;
    }

    public BinaryExpression? TryGetBinaryExpression(ScalarExpression scalarExpression) =>
        binaryExpressionByBaseId.GetValueOrDefault(scalarExpression.Id);

    public (ScalarExpression? First, ScalarExpression? Second)? TryGetBinaryExpressionOperands(BinaryExpression binaryExpression)
    {
        if (!binaryExpressionFirstExpressionLinkByOwnerId.TryGetValue(binaryExpression.Id, out var firstLink) ||
            !binaryExpressionSecondExpressionLinkByOwnerId.TryGetValue(binaryExpression.Id, out var secondLink))
        {
            return null;
        }

        return (
            scalarExpressionById.GetValueOrDefault(firstLink.ValueId),
            scalarExpressionById.GetValueOrDefault(secondLink.ValueId));
    }

    public UnaryExpression? TryGetUnaryExpression(ScalarExpression scalarExpression) =>
        unaryExpressionByBaseId.GetValueOrDefault(scalarExpression.Id);

    public ScalarExpression? TryGetUnaryExpressionOperand(UnaryExpression unaryExpression)
    {
        return unaryExpressionExpressionLinkByOwnerId.TryGetValue(unaryExpression.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ParenthesisExpression? TryGetParenthesisExpression(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return parenthesisExpressionByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public ScalarExpression? TryGetParenthesisExpressionOperand(ParenthesisExpression parenthesisExpression)
    {
        return parenthesisExpressionExpressionLinkByOwnerId.TryGetValue(parenthesisExpression.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public FunctionCall? TryGetFunctionCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return functionCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public IReadOnlyList<ScalarExpression> GetFunctionCallParameters(FunctionCall functionCall)
    {
        return GetOrderedScalarExpressions(functionCallParametersByOwnerId, functionCall.Id);
    }

    public string? TryGetFunctionCallName(FunctionCall functionCall)
    {
        return functionCallFunctionNameLinkByOwnerId.TryGetValue(functionCall.Id, out var link)
            ? identifierById.GetValueOrDefault(link.ValueId)?.Value
            : null;
    }

    public bool HasFunctionCallOverClause(FunctionCall functionCall) =>
        functionCallOverClauseLinkByOwnerId.ContainsKey(functionCall.Id);

    public LeftFunctionCall? TryGetLeftFunctionCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return leftFunctionCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public IReadOnlyList<ScalarExpression> GetLeftFunctionCallParameters(LeftFunctionCall leftFunctionCall)
    {
        return GetOrderedScalarExpressions(leftFunctionCallParametersByOwnerId, leftFunctionCall.Id);
    }

    public RightFunctionCall? TryGetRightFunctionCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return rightFunctionCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public IReadOnlyList<ScalarExpression> GetRightFunctionCallParameters(RightFunctionCall rightFunctionCall)
    {
        return GetOrderedScalarExpressions(rightFunctionCallParametersByOwnerId, rightFunctionCall.Id);
    }

    public ParameterlessCall? TryGetParameterlessCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return parameterlessCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public CoalesceExpression? TryGetCoalesceExpression(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return coalesceExpressionByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public IReadOnlyList<ScalarExpression> GetCoalesceExpressions(CoalesceExpression coalesceExpression)
    {
        return GetOrderedScalarExpressions(coalesceExpressionExpressionsByOwnerId, coalesceExpression.Id);
    }

    public NullIfExpression? TryGetNullIfExpression(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return nullIfExpressionByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public (ScalarExpression? First, ScalarExpression? Second)? TryGetNullIfExpressionOperands(NullIfExpression nullIfExpression)
    {
        if (!nullIfExpressionFirstExpressionLinkByOwnerId.TryGetValue(nullIfExpression.Id, out var firstLink) ||
            !nullIfExpressionSecondExpressionLinkByOwnerId.TryGetValue(nullIfExpression.Id, out var secondLink))
        {
            return null;
        }

        return (
            scalarExpressionById.GetValueOrDefault(firstLink.ValueId),
            scalarExpressionById.GetValueOrDefault(secondLink.ValueId));
    }

    public IIfCall? TryGetIIfCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return iIfCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public BooleanExpression? TryGetIIfPredicate(IIfCall iIfCall)
    {
        return iIfCallPredicateLinkByOwnerId.TryGetValue(iIfCall.Id, out var link)
            ? new BooleanExpression { Id = link.ValueId }
            : null;
    }

    public ScalarExpression? TryGetIIfThenExpression(IIfCall iIfCall)
    {
        return iIfCallThenExpressionLinkByOwnerId.TryGetValue(iIfCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ScalarExpression? TryGetIIfElseExpression(IIfCall iIfCall)
    {
        return iIfCallElseExpressionLinkByOwnerId.TryGetValue(iIfCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public SearchedCaseExpression? TryGetSearchedCaseExpression(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        if (!caseExpressionByPrimaryExpressionId.TryGetValue(primaryExpression.Id, out var caseExpression))
        {
            return null;
        }

        return searchedCaseExpressionByCaseExpressionId.GetValueOrDefault(caseExpression.Id);
    }

    public IReadOnlyList<SearchedWhenClause> GetSearchedWhenClauses(SearchedCaseExpression searchedCaseExpression)
    {
        if (!searchedCaseExpressionWhenClausesByOwnerId.TryGetValue(searchedCaseExpression.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => searchedWhenClauseById.GetValueOrDefault(item.ValueId))
            .Where(item => item is not null)
            .Cast<SearchedWhenClause>()
            .ToArray();
    }

    public BooleanExpression? TryGetSearchedWhenClauseCondition(SearchedWhenClause searchedWhenClause)
    {
        return searchedWhenClauseWhenExpressionLinkByOwnerId.TryGetValue(searchedWhenClause.Id, out var link)
            ? new BooleanExpression { Id = link.ValueId }
            : null;
    }

    public ScalarExpression? TryGetWhenClauseThenExpression(SearchedWhenClause whenClause)
    {
        return whenClauseThenExpressionLinkByOwnerId.TryGetValue(whenClause.BaseId, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ScalarExpression? TryGetCaseElseExpression(SearchedCaseExpression searchedCaseExpression)
    {
        return caseExpressionElseExpressionLinkByOwnerId.TryGetValue(searchedCaseExpression.BaseId, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public SimpleCaseExpression? TryGetSimpleCaseExpression(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        if (!caseExpressionByPrimaryExpressionId.TryGetValue(primaryExpression.Id, out var caseExpression))
        {
            return null;
        }

        return simpleCaseExpressionByCaseExpressionId.GetValueOrDefault(caseExpression.Id);
    }

    public ScalarExpression? TryGetSimpleCaseInputExpression(SimpleCaseExpression simpleCaseExpression)
    {
        return simpleCaseExpressionInputExpressionLinkByOwnerId.TryGetValue(simpleCaseExpression.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public IReadOnlyList<SimpleWhenClause> GetSimpleWhenClauses(SimpleCaseExpression simpleCaseExpression)
    {
        if (!simpleCaseExpressionWhenClausesByOwnerId.TryGetValue(simpleCaseExpression.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => simpleWhenClauseById.GetValueOrDefault(item.ValueId))
            .Where(item => item is not null)
            .Cast<SimpleWhenClause>()
            .ToArray();
    }

    public ScalarExpression? TryGetSimpleWhenClauseWhenExpression(SimpleWhenClause simpleWhenClause)
    {
        return simpleWhenClauseWhenExpressionLinkByOwnerId.TryGetValue(simpleWhenClause.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ScalarExpression? TryGetWhenClauseThenExpression(SimpleWhenClause whenClause)
    {
        return whenClauseThenExpressionLinkByOwnerId.TryGetValue(whenClause.BaseId, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ScalarExpression? TryGetCaseElseExpression(SimpleCaseExpression simpleCaseExpression)
    {
        return caseExpressionElseExpressionLinkByOwnerId.TryGetValue(simpleCaseExpression.BaseId, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public CastCall? TryGetCastCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return castCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public ScalarExpression? TryGetCastCallParameter(CastCall castCall)
    {
        return castCallParameterLinkByOwnerId.TryGetValue(castCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ConvertCall? TryGetConvertCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return convertCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public ScalarExpression? TryGetConvertCallParameter(ConvertCall convertCall)
    {
        return convertCallParameterLinkByOwnerId.TryGetValue(convertCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ScalarExpression? TryGetConvertCallStyle(ConvertCall convertCall)
    {
        return convertCallStyleLinkByOwnerId.TryGetValue(convertCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public TryCastCall? TryGetTryCastCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return tryCastCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public ScalarExpression? TryGetTryCastCallParameter(TryCastCall tryCastCall)
    {
        return tryCastCallParameterLinkByOwnerId.TryGetValue(tryCastCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public TryConvertCall? TryGetTryConvertCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return tryConvertCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public ScalarExpression? TryGetTryConvertCallParameter(TryConvertCall tryConvertCall)
    {
        return tryConvertCallParameterLinkByOwnerId.TryGetValue(tryConvertCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ScalarExpression? TryGetTryConvertCallStyle(TryConvertCall tryConvertCall)
    {
        return tryConvertCallStyleLinkByOwnerId.TryGetValue(tryConvertCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ParseCall? TryGetParseCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return parseCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public ScalarExpression? TryGetParseCallStringValue(ParseCall parseCall)
    {
        return parseCallStringValueLinkByOwnerId.TryGetValue(parseCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ScalarExpression? TryGetParseCallCulture(ParseCall parseCall)
    {
        return parseCallCultureLinkByOwnerId.TryGetValue(parseCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public TryParseCall? TryGetTryParseCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return tryParseCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public ScalarExpression? TryGetTryParseCallStringValue(TryParseCall tryParseCall)
    {
        return tryParseCallStringValueLinkByOwnerId.TryGetValue(tryParseCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ScalarExpression? TryGetTryParseCallCulture(TryParseCall tryParseCall)
    {
        return tryParseCallCultureLinkByOwnerId.TryGetValue(tryParseCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public AtTimeZoneCall? TryGetAtTimeZoneCall(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return atTimeZoneCallByPrimaryExpressionId.GetValueOrDefault(primaryExpression.Id);
    }

    public ScalarExpression? TryGetAtTimeZoneDateValue(AtTimeZoneCall atTimeZoneCall)
    {
        return atTimeZoneCallDateValueLinkByOwnerId.TryGetValue(atTimeZoneCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ScalarExpression? TryGetAtTimeZoneTimeZone(AtTimeZoneCall atTimeZoneCall)
    {
        return atTimeZoneCallTimeZoneLinkByOwnerId.TryGetValue(atTimeZoneCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public BooleanBinaryExpression? TryGetBooleanBinaryExpression(BooleanExpression booleanExpression) =>
        booleanBinaryExpressionByBaseId.GetValueOrDefault(booleanExpression.Id);

    public (BooleanExpression? First, BooleanExpression? Second)? TryGetBooleanBinaryExpressionChildren(BooleanBinaryExpression booleanBinaryExpression)
    {
        if (!booleanBinaryExpressionFirstExpressionLinkByOwnerId.TryGetValue(booleanBinaryExpression.Id, out var firstLink) ||
            !booleanBinaryExpressionSecondExpressionLinkByOwnerId.TryGetValue(booleanBinaryExpression.Id, out var secondLink))
        {
            return null;
        }

        return (new BooleanExpression { Id = firstLink.ValueId }, new BooleanExpression { Id = secondLink.ValueId });
    }

    public BooleanComparisonExpression? TryGetBooleanComparisonExpression(BooleanExpression booleanExpression) =>
        booleanComparisonExpressionByBaseId.GetValueOrDefault(booleanExpression.Id);

    public (ScalarExpression? First, ScalarExpression? Second)? TryGetBooleanComparisonExpressionOperands(BooleanComparisonExpression booleanComparisonExpression)
    {
        if (!booleanComparisonExpressionFirstExpressionLinkByOwnerId.TryGetValue(booleanComparisonExpression.Id, out var firstLink) ||
            !booleanComparisonExpressionSecondExpressionLinkByOwnerId.TryGetValue(booleanComparisonExpression.Id, out var secondLink))
        {
            return null;
        }

        return (
            scalarExpressionById.GetValueOrDefault(firstLink.ValueId),
            scalarExpressionById.GetValueOrDefault(secondLink.ValueId));
    }

    public BooleanNotExpression? TryGetBooleanNotExpression(BooleanExpression booleanExpression) =>
        booleanNotExpressionByBaseId.GetValueOrDefault(booleanExpression.Id);

    public BooleanExpression? TryGetBooleanNotExpressionOperand(BooleanNotExpression booleanNotExpression)
    {
        return booleanNotExpressionExpressionLinkByOwnerId.TryGetValue(booleanNotExpression.Id, out var link)
            ? new BooleanExpression { Id = link.ValueId }
            : null;
    }

    public BooleanParenthesisExpression? TryGetBooleanParenthesisExpression(BooleanExpression booleanExpression) =>
        booleanParenthesisExpressionByBaseId.GetValueOrDefault(booleanExpression.Id);

    public BooleanExpression? TryGetBooleanParenthesisExpressionOperand(BooleanParenthesisExpression booleanParenthesisExpression)
    {
        return booleanParenthesisExpressionExpressionLinkByOwnerId.TryGetValue(booleanParenthesisExpression.Id, out var link)
            ? new BooleanExpression { Id = link.ValueId }
            : null;
    }

    public ExistsPredicate? TryGetExistsPredicate(BooleanExpression booleanExpression) =>
        existsPredicateByBaseId.GetValueOrDefault(booleanExpression.Id);

    public ScalarSubquery? TryGetExistsPredicateSubquery(ExistsPredicate existsPredicate)
    {
        return existsPredicateSubqueryLinkByOwnerId.TryGetValue(existsPredicate.Id, out var link)
            ? scalarSubqueryByPrimaryExpressionId.Values.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal))
            : null;
    }

    public InPredicate? TryGetInPredicate(BooleanExpression booleanExpression) =>
        inPredicateByBaseId.GetValueOrDefault(booleanExpression.Id);

    public ScalarExpression? TryGetInPredicateExpression(InPredicate inPredicate)
    {
        return inPredicateExpressionLinkByOwnerId.TryGetValue(inPredicate.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ScalarSubquery? TryGetInPredicateSubquery(InPredicate inPredicate)
    {
        return inPredicateSubqueryLinkByOwnerId.TryGetValue(inPredicate.Id, out var link)
            ? scalarSubqueryByPrimaryExpressionId.Values.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal))
            : null;
    }

    public SubqueryComparisonPredicate? TryGetSubqueryComparisonPredicate(BooleanExpression booleanExpression) =>
        subqueryComparisonPredicateByBaseId.GetValueOrDefault(booleanExpression.Id);

    public ScalarExpression? TryGetSubqueryComparisonPredicateExpression(SubqueryComparisonPredicate subqueryComparisonPredicate)
    {
        return subqueryComparisonPredicateExpressionLinkByOwnerId.TryGetValue(subqueryComparisonPredicate.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ValueId)
            : null;
    }

    public ScalarSubquery? TryGetSubqueryComparisonPredicateSubquery(SubqueryComparisonPredicate subqueryComparisonPredicate)
    {
        return subqueryComparisonPredicateSubqueryLinkByOwnerId.TryGetValue(subqueryComparisonPredicate.Id, out var link)
            ? scalarSubqueryByPrimaryExpressionId.Values.FirstOrDefault(item => string.Equals(item.Id, link.ValueId, StringComparison.Ordinal))
            : null;
    }

    private IReadOnlyList<ScalarExpression> GetOrderedScalarExpressions<T>(
        IReadOnlyDictionary<string, List<T>> itemsByOwnerId,
        string ownerId)
    {
        if (!itemsByOwnerId.TryGetValue(ownerId, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal((string?)item!.GetType().GetProperty("Ordinal")?.GetValue(item) ?? string.Empty))
            .Select(item => scalarExpressionById.GetValueOrDefault((string?)item!.GetType().GetProperty("ValueId")?.GetValue(item) ?? string.Empty))
            .Where(item => item is not null)
            .Cast<ScalarExpression>()
            .ToArray();
    }
}
