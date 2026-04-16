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

        return groupByClauseById.GetValueOrDefault(link.GroupByClauseId);
    }

    public IReadOnlyList<GroupingSpecification> GetGroupingSpecifications(GroupByClause groupByClause)
    {
        if (!groupingSpecificationsByGroupByClauseOwnerId.TryGetValue(groupByClause.Id, out var items))
        {
            return [];
        }

        return items
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => groupingSpecificationById.GetValueOrDefault(item.GroupingSpecificationId))
            .Where(item => item is not null)
            .Cast<GroupingSpecification>()
            .ToArray();
    }

    public bool IsGroupByAll(GroupByClause groupByClause) =>
        string.Equals(groupByClause.All, "true", StringComparison.OrdinalIgnoreCase);

    public ExpressionGroupingSpecification? TryGetExpressionGroupingSpecification(GroupingSpecification groupingSpecification) =>
        expressionGroupingSpecificationByBaseId.GetValueOrDefault(groupingSpecification.Id);

    public GroupingSetsGroupingSpecification? TryGetGroupingSetsGroupingSpecification(GroupingSpecification groupingSpecification)
    {
        return model.GroupingSetsGroupingSpecificationList
            .FirstOrDefault(item => string.Equals(item.GroupingSpecificationId, groupingSpecification.Id, StringComparison.Ordinal));
    }

    public IReadOnlyList<GroupingSpecification> GetGroupingSets(GroupingSetsGroupingSpecification groupingSetsGroupingSpecification)
    {
        return model.GroupingSetsGroupingSpecificationSetsItemList
            .Where(item => string.Equals(item.GroupingSetsGroupingSpecificationId, groupingSetsGroupingSpecification.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => groupingSpecificationById.GetValueOrDefault(item.GroupingSpecificationId))
            .Where(item => item is not null)
            .Cast<GroupingSpecification>()
            .ToArray();
    }

    public RollupGroupingSpecification? TryGetRollupGroupingSpecification(GroupingSpecification groupingSpecification)
    {
        return model.RollupGroupingSpecificationList
            .FirstOrDefault(item => string.Equals(item.GroupingSpecificationId, groupingSpecification.Id, StringComparison.Ordinal));
    }

    public IReadOnlyList<GroupingSpecification> GetRollupArguments(RollupGroupingSpecification rollupGroupingSpecification)
    {
        return model.RollupGroupingSpecificationArgumentsItemList
            .Where(item => string.Equals(item.RollupGroupingSpecificationId, rollupGroupingSpecification.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => groupingSpecificationById.GetValueOrDefault(item.GroupingSpecificationId))
            .Where(item => item is not null)
            .Cast<GroupingSpecification>()
            .ToArray();
    }

    public CubeGroupingSpecification? TryGetCubeGroupingSpecification(GroupingSpecification groupingSpecification)
    {
        return model.CubeGroupingSpecificationList
            .FirstOrDefault(item => string.Equals(item.GroupingSpecificationId, groupingSpecification.Id, StringComparison.Ordinal));
    }

    public IReadOnlyList<GroupingSpecification> GetCubeArguments(CubeGroupingSpecification cubeGroupingSpecification)
    {
        return model.CubeGroupingSpecificationArgumentsItemList
            .Where(item => string.Equals(item.CubeGroupingSpecificationId, cubeGroupingSpecification.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => groupingSpecificationById.GetValueOrDefault(item.GroupingSpecificationId))
            .Where(item => item is not null)
            .Cast<GroupingSpecification>()
            .ToArray();
    }

    public CompositeGroupingSpecification? TryGetCompositeGroupingSpecification(GroupingSpecification groupingSpecification)
    {
        return model.CompositeGroupingSpecificationList
            .FirstOrDefault(item => string.Equals(item.GroupingSpecificationId, groupingSpecification.Id, StringComparison.Ordinal));
    }

    public IReadOnlyList<GroupingSpecification> GetCompositeGroupingItems(CompositeGroupingSpecification compositeGroupingSpecification)
    {
        return model.CompositeGroupingSpecificationItemsItemList
            .Where(item => string.Equals(item.CompositeGroupingSpecificationId, compositeGroupingSpecification.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => groupingSpecificationById.GetValueOrDefault(item.GroupingSpecificationId))
            .Where(item => item is not null)
            .Cast<GroupingSpecification>()
            .ToArray();
    }

    public bool IsGrandTotalGroupingSpecification(GroupingSpecification groupingSpecification)
    {
        return model.GrandTotalGroupingSpecificationList
            .Any(item => string.Equals(item.GroupingSpecificationId, groupingSpecification.Id, StringComparison.Ordinal));
    }

    public ScalarExpression? TryGetExpressionGroupingSpecificationExpression(ExpressionGroupingSpecification expressionGroupingSpecification)
    {
        return expressionGroupingSpecificationExpressionLinkByOwnerId.TryGetValue(expressionGroupingSpecification.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public BooleanExpression? TryGetWhereSearchCondition(QuerySpecification querySpecification)
    {
        if (!whereClauseLinkByOwnerId.TryGetValue(querySpecification.Id, out var whereClauseLink))
        {
            return null;
        }

        if (!whereClauseById.TryGetValue(whereClauseLink.WhereClauseId, out var whereClause))
        {
            return null;
        }

        return whereClauseSearchConditionLinkByOwnerId.TryGetValue(whereClause.Id, out var searchConditionLink)
            ? new BooleanExpression { Id = searchConditionLink.BooleanExpressionId }
            : null;
    }

    public BooleanExpression? TryGetHavingSearchCondition(QuerySpecification querySpecification)
    {
        if (!havingClauseLinkByOwnerId.TryGetValue(querySpecification.Id, out var havingClauseLink))
        {
            return null;
        }

        if (!havingClauseById.TryGetValue(havingClauseLink.HavingClauseId, out var havingClause))
        {
            return null;
        }

        return havingClauseSearchConditionLinkByOwnerId.TryGetValue(havingClause.Id, out var searchConditionLink)
            ? new BooleanExpression { Id = searchConditionLink.BooleanExpressionId }
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
            ? link.QueryExpressionId
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
            scalarExpressionById.GetValueOrDefault(firstLink.ScalarExpressionId),
            scalarExpressionById.GetValueOrDefault(secondLink.ScalarExpressionId));
    }

    public UnaryExpression? TryGetUnaryExpression(ScalarExpression scalarExpression) =>
        unaryExpressionByBaseId.GetValueOrDefault(scalarExpression.Id);

    public ScalarExpression? TryGetUnaryExpressionOperand(UnaryExpression unaryExpression)
    {
        return unaryExpressionExpressionLinkByOwnerId.TryGetValue(unaryExpression.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            ? identifierById.GetValueOrDefault(link.IdentifierId)?.Value
            : null;
    }

    public bool HasFunctionCallCallTarget(FunctionCall functionCall)
    {
        return model.FunctionCallCallTargetLinkList
            .Any(item => string.Equals(item.FunctionCallId, functionCall.Id, StringComparison.Ordinal));
    }

    public IReadOnlyList<string> GetFunctionCallCallTargetParts(FunctionCall functionCall)
    {
        var callTargetLink = model.FunctionCallCallTargetLinkList
            .FirstOrDefault(item => string.Equals(item.FunctionCallId, functionCall.Id, StringComparison.Ordinal));
        if (callTargetLink is null)
        {
            return [];
        }

        var multiPartIdentifierCallTarget = model.MultiPartIdentifierCallTargetList
            .FirstOrDefault(item => string.Equals(item.CallTargetId, callTargetLink.CallTargetId, StringComparison.Ordinal));
        if (multiPartIdentifierCallTarget is null)
        {
            return [];
        }

        var multiPartIdentifierLink = model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList
            .FirstOrDefault(item => string.Equals(item.MultiPartIdentifierCallTargetId, multiPartIdentifierCallTarget.Id, StringComparison.Ordinal));
        return multiPartIdentifierLink is null
            ? []
            : GetMultiPartIdentifierParts(multiPartIdentifierLink.MultiPartIdentifierId);
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

    public NextValueForExpression? TryGetNextValueForExpression(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return null;
        }

        return model.NextValueForExpressionList
            .FirstOrDefault(item => string.Equals(item.PrimaryExpressionId, primaryExpression.Id, StringComparison.Ordinal));
    }

    public bool IsValueExpression(ScalarExpression scalarExpression)
    {
        if (!primaryExpressionByScalarExpressionId.TryGetValue(scalarExpression.Id, out var primaryExpression))
        {
            return false;
        }

        return model.ValueExpressionList
            .Any(item => string.Equals(item.PrimaryExpressionId, primaryExpression.Id, StringComparison.Ordinal));
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
            scalarExpressionById.GetValueOrDefault(firstLink.ScalarExpressionId),
            scalarExpressionById.GetValueOrDefault(secondLink.ScalarExpressionId));
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
            ? new BooleanExpression { Id = link.BooleanExpressionId }
            : null;
    }

    public ScalarExpression? TryGetIIfThenExpression(IIfCall iIfCall)
    {
        return iIfCallThenExpressionLinkByOwnerId.TryGetValue(iIfCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public ScalarExpression? TryGetIIfElseExpression(IIfCall iIfCall)
    {
        return iIfCallElseExpressionLinkByOwnerId.TryGetValue(iIfCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            .Select(item => searchedWhenClauseById.GetValueOrDefault(item.SearchedWhenClauseId))
            .Where(item => item is not null)
            .Cast<SearchedWhenClause>()
            .ToArray();
    }

    public BooleanExpression? TryGetSearchedWhenClauseCondition(SearchedWhenClause searchedWhenClause)
    {
        return searchedWhenClauseWhenExpressionLinkByOwnerId.TryGetValue(searchedWhenClause.Id, out var link)
            ? new BooleanExpression { Id = link.BooleanExpressionId }
            : null;
    }

    public ScalarExpression? TryGetWhenClauseThenExpression(SearchedWhenClause whenClause)
    {
        return whenClauseThenExpressionLinkByOwnerId.TryGetValue(whenClause.WhenClauseId, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public ScalarExpression? TryGetCaseElseExpression(SearchedCaseExpression searchedCaseExpression)
    {
        return caseExpressionElseExpressionLinkByOwnerId.TryGetValue(searchedCaseExpression.CaseExpressionId, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            .Select(item => simpleWhenClauseById.GetValueOrDefault(item.SimpleWhenClauseId))
            .Where(item => item is not null)
            .Cast<SimpleWhenClause>()
            .ToArray();
    }

    public ScalarExpression? TryGetSimpleWhenClauseWhenExpression(SimpleWhenClause simpleWhenClause)
    {
        return simpleWhenClauseWhenExpressionLinkByOwnerId.TryGetValue(simpleWhenClause.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public ScalarExpression? TryGetWhenClauseThenExpression(SimpleWhenClause whenClause)
    {
        return whenClauseThenExpressionLinkByOwnerId.TryGetValue(whenClause.WhenClauseId, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public ScalarExpression? TryGetCaseElseExpression(SimpleCaseExpression simpleCaseExpression)
    {
        return caseExpressionElseExpressionLinkByOwnerId.TryGetValue(simpleCaseExpression.CaseExpressionId, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public ScalarExpression? TryGetConvertCallStyle(ConvertCall convertCall)
    {
        return convertCallStyleLinkByOwnerId.TryGetValue(convertCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public ScalarExpression? TryGetTryConvertCallStyle(TryConvertCall tryConvertCall)
    {
        return tryConvertCallStyleLinkByOwnerId.TryGetValue(tryConvertCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public ScalarExpression? TryGetParseCallCulture(ParseCall parseCall)
    {
        return parseCallCultureLinkByOwnerId.TryGetValue(parseCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public ScalarExpression? TryGetTryParseCallCulture(TryParseCall tryParseCall)
    {
        return tryParseCallCultureLinkByOwnerId.TryGetValue(tryParseCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public ScalarExpression? TryGetAtTimeZoneTimeZone(AtTimeZoneCall atTimeZoneCall)
    {
        return atTimeZoneCallTimeZoneLinkByOwnerId.TryGetValue(atTimeZoneCall.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
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

        return (new BooleanExpression { Id = firstLink.BooleanExpressionId }, new BooleanExpression { Id = secondLink.BooleanExpressionId });
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
            scalarExpressionById.GetValueOrDefault(firstLink.ScalarExpressionId),
            scalarExpressionById.GetValueOrDefault(secondLink.ScalarExpressionId));
    }

    public BooleanTernaryExpression? TryGetBooleanTernaryExpression(BooleanExpression booleanExpression)
    {
        return model.BooleanTernaryExpressionList
            .FirstOrDefault(item => string.Equals(item.BooleanExpressionId, booleanExpression.Id, StringComparison.Ordinal));
    }

    public (ScalarExpression? First, ScalarExpression? Second, ScalarExpression? Third)? TryGetBooleanTernaryExpressionOperands(BooleanTernaryExpression booleanTernaryExpression)
    {
        var firstLink = model.BooleanTernaryExpressionFirstExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.BooleanTernaryExpressionId, booleanTernaryExpression.Id, StringComparison.Ordinal));
        var secondLink = model.BooleanTernaryExpressionSecondExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.BooleanTernaryExpressionId, booleanTernaryExpression.Id, StringComparison.Ordinal));
        var thirdLink = model.BooleanTernaryExpressionThirdExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.BooleanTernaryExpressionId, booleanTernaryExpression.Id, StringComparison.Ordinal));

        if (firstLink is null || secondLink is null || thirdLink is null)
        {
            return null;
        }

        return (
            scalarExpressionById.GetValueOrDefault(firstLink.ScalarExpressionId),
            scalarExpressionById.GetValueOrDefault(secondLink.ScalarExpressionId),
            scalarExpressionById.GetValueOrDefault(thirdLink.ScalarExpressionId));
    }

    public BooleanIsNullExpression? TryGetBooleanIsNullExpression(BooleanExpression booleanExpression)
    {
        return model.BooleanIsNullExpressionList
            .FirstOrDefault(item => string.Equals(item.BooleanExpressionId, booleanExpression.Id, StringComparison.Ordinal));
    }

    public ScalarExpression? TryGetBooleanIsNullExpressionOperand(BooleanIsNullExpression booleanIsNullExpression)
    {
        var link = model.BooleanIsNullExpressionExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.BooleanIsNullExpressionId, booleanIsNullExpression.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId);
    }

    public LikePredicate? TryGetLikePredicate(BooleanExpression booleanExpression)
    {
        return model.LikePredicateList
            .FirstOrDefault(item => string.Equals(item.BooleanExpressionId, booleanExpression.Id, StringComparison.Ordinal));
    }

    public (ScalarExpression? First, ScalarExpression? Second)? TryGetLikePredicateOperands(LikePredicate likePredicate)
    {
        var firstLink = model.LikePredicateFirstExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.LikePredicateId, likePredicate.Id, StringComparison.Ordinal));
        var secondLink = model.LikePredicateSecondExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.LikePredicateId, likePredicate.Id, StringComparison.Ordinal));

        if (firstLink is null || secondLink is null)
        {
            return null;
        }

        return (
            scalarExpressionById.GetValueOrDefault(firstLink.ScalarExpressionId),
            scalarExpressionById.GetValueOrDefault(secondLink.ScalarExpressionId));
    }

    public ScalarExpression? TryGetLikePredicateEscapeExpression(LikePredicate likePredicate)
    {
        var link = model.LikePredicateEscapeExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.LikePredicateId, likePredicate.Id, StringComparison.Ordinal));
        return link is null
            ? null
            : scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId);
    }

    public DistinctPredicate? TryGetDistinctPredicate(BooleanExpression booleanExpression)
    {
        return model.DistinctPredicateList
            .FirstOrDefault(item => string.Equals(item.BooleanExpressionId, booleanExpression.Id, StringComparison.Ordinal));
    }

    public (ScalarExpression? First, ScalarExpression? Second)? TryGetDistinctPredicateOperands(DistinctPredicate distinctPredicate)
    {
        var firstLink = model.DistinctPredicateFirstExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.DistinctPredicateId, distinctPredicate.Id, StringComparison.Ordinal));
        var secondLink = model.DistinctPredicateSecondExpressionLinkList
            .FirstOrDefault(item => string.Equals(item.DistinctPredicateId, distinctPredicate.Id, StringComparison.Ordinal));

        if (firstLink is null || secondLink is null)
        {
            return null;
        }

        return (
            scalarExpressionById.GetValueOrDefault(firstLink.ScalarExpressionId),
            scalarExpressionById.GetValueOrDefault(secondLink.ScalarExpressionId));
    }

    public FullTextPredicate? TryGetFullTextPredicate(BooleanExpression booleanExpression)
    {
        return model.FullTextPredicateList
            .FirstOrDefault(item => string.Equals(item.BooleanExpressionId, booleanExpression.Id, StringComparison.Ordinal));
    }

    public IReadOnlyList<ColumnReferenceExpression> GetFullTextPredicateColumns(FullTextPredicate fullTextPredicate)
    {
        return model.FullTextPredicateColumnsItemList
            .Where(item => string.Equals(item.FullTextPredicateId, fullTextPredicate.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => model.ColumnReferenceExpressionList
                .FirstOrDefault(column => string.Equals(column.Id, item.ColumnReferenceExpressionId, StringComparison.Ordinal)))
            .Where(item => item is not null)
            .Cast<ColumnReferenceExpression>()
            .ToArray();
    }

    public ScalarExpression? TryGetFullTextPredicateValueExpression(FullTextPredicate fullTextPredicate)
    {
        var link = model.FullTextPredicateValueLinkList
            .FirstOrDefault(item => string.Equals(item.FullTextPredicateId, fullTextPredicate.Id, StringComparison.Ordinal));
        if (link is null)
        {
            return null;
        }

        return TryGetScalarExpressionFromValueExpressionId(link.ValueExpressionId);
    }

    public BooleanNotExpression? TryGetBooleanNotExpression(BooleanExpression booleanExpression) =>
        booleanNotExpressionByBaseId.GetValueOrDefault(booleanExpression.Id);

    public BooleanExpression? TryGetBooleanNotExpressionOperand(BooleanNotExpression booleanNotExpression)
    {
        return booleanNotExpressionExpressionLinkByOwnerId.TryGetValue(booleanNotExpression.Id, out var link)
            ? new BooleanExpression { Id = link.BooleanExpressionId }
            : null;
    }

    public BooleanParenthesisExpression? TryGetBooleanParenthesisExpression(BooleanExpression booleanExpression) =>
        booleanParenthesisExpressionByBaseId.GetValueOrDefault(booleanExpression.Id);

    public BooleanExpression? TryGetBooleanParenthesisExpressionOperand(BooleanParenthesisExpression booleanParenthesisExpression)
    {
        return booleanParenthesisExpressionExpressionLinkByOwnerId.TryGetValue(booleanParenthesisExpression.Id, out var link)
            ? new BooleanExpression { Id = link.BooleanExpressionId }
            : null;
    }

    public ExistsPredicate? TryGetExistsPredicate(BooleanExpression booleanExpression) =>
        existsPredicateByBaseId.GetValueOrDefault(booleanExpression.Id);

    public ScalarSubquery? TryGetExistsPredicateSubquery(ExistsPredicate existsPredicate)
    {
        return existsPredicateSubqueryLinkByOwnerId.TryGetValue(existsPredicate.Id, out var link)
            ? scalarSubqueryByPrimaryExpressionId.Values.FirstOrDefault(item => string.Equals(item.Id, link.ScalarSubqueryId, StringComparison.Ordinal))
            : null;
    }

    public InPredicate? TryGetInPredicate(BooleanExpression booleanExpression) =>
        inPredicateByBaseId.GetValueOrDefault(booleanExpression.Id);

    public ScalarExpression? TryGetInPredicateExpression(InPredicate inPredicate)
    {
        return inPredicateExpressionLinkByOwnerId.TryGetValue(inPredicate.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public ScalarSubquery? TryGetInPredicateSubquery(InPredicate inPredicate)
    {
        return inPredicateSubqueryLinkByOwnerId.TryGetValue(inPredicate.Id, out var link)
            ? scalarSubqueryByPrimaryExpressionId.Values.FirstOrDefault(item => string.Equals(item.Id, link.ScalarSubqueryId, StringComparison.Ordinal))
            : null;
    }

    public IReadOnlyList<ScalarExpression> GetInPredicateValues(InPredicate inPredicate)
    {
        return model.InPredicateValuesItemList
            .Where(item => string.Equals(item.InPredicateId, inPredicate.Id, StringComparison.Ordinal))
            .OrderBy(item => ParseOrdinal(item.Ordinal))
            .Select(item => scalarExpressionById.GetValueOrDefault(item.ScalarExpressionId))
            .Where(item => item is not null)
            .Cast<ScalarExpression>()
            .ToArray();
    }

    public SubqueryComparisonPredicate? TryGetSubqueryComparisonPredicate(BooleanExpression booleanExpression) =>
        subqueryComparisonPredicateByBaseId.GetValueOrDefault(booleanExpression.Id);

    public ScalarExpression? TryGetSubqueryComparisonPredicateExpression(SubqueryComparisonPredicate subqueryComparisonPredicate)
    {
        return subqueryComparisonPredicateExpressionLinkByOwnerId.TryGetValue(subqueryComparisonPredicate.Id, out var link)
            ? scalarExpressionById.GetValueOrDefault(link.ScalarExpressionId)
            : null;
    }

    public ScalarSubquery? TryGetSubqueryComparisonPredicateSubquery(SubqueryComparisonPredicate subqueryComparisonPredicate)
    {
        return subqueryComparisonPredicateSubqueryLinkByOwnerId.TryGetValue(subqueryComparisonPredicate.Id, out var link)
            ? scalarSubqueryByPrimaryExpressionId.Values.FirstOrDefault(item => string.Equals(item.Id, link.ScalarSubqueryId, StringComparison.Ordinal))
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
            .Select(item => scalarExpressionById.GetValueOrDefault((string?)item!.GetType().GetProperty("ScalarExpressionId")?.GetValue(item) ?? string.Empty))
            .Where(item => item is not null)
            .Cast<ScalarExpression>()
            .ToArray();
    }

    private ScalarExpression? TryGetScalarExpressionFromValueExpressionId(string valueExpressionId)
    {
        var valueExpression = model.ValueExpressionList
            .FirstOrDefault(item => string.Equals(item.Id, valueExpressionId, StringComparison.Ordinal));
        if (valueExpression is null)
        {
            return null;
        }

        var primaryExpression = model.PrimaryExpressionList
            .FirstOrDefault(item => string.Equals(item.Id, valueExpression.PrimaryExpressionId, StringComparison.Ordinal));
        if (primaryExpression is null)
        {
            return null;
        }

        return scalarExpressionById.GetValueOrDefault(primaryExpression.ScalarExpressionId);
    }
}
