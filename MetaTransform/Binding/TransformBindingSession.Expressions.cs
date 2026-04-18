using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private void BindScalarExpression(
        ScalarExpression scalarExpression,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeGroupingContext? groupingContext,
        bool withinAggregate)
    {
        var binaryExpression = navigator.TryGetBinaryExpression(scalarExpression);
        if (binaryExpression is not null)
        {
            var operands = navigator.TryGetBinaryExpressionOperands(binaryExpression);
            if (operands is not null)
            {
                if (operands.Value.First is not null)
                {
                    BindScalarExpression(operands.Value.First, scope, inputRowset, groupingContext, withinAggregate);
                }

                if (operands.Value.Second is not null)
                {
                    BindScalarExpression(operands.Value.Second, scope, inputRowset, groupingContext, withinAggregate);
                }
            }

            return;
        }

        var unaryExpression = navigator.TryGetUnaryExpression(scalarExpression);
        if (unaryExpression is not null)
        {
            var operand = navigator.TryGetUnaryExpressionOperand(unaryExpression);
            if (operand is not null)
            {
                BindScalarExpression(operand, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var directColumnReference = navigator.TryGetDirectColumnReference(scalarExpression);
        if (directColumnReference is not null)
        {
            var boundColumnReference = BindColumnReference(directColumnReference, scope, groupingContext, withinAggregate);
            if (boundColumnReference is not null)
            {
                boundColumnReferences.Add(boundColumnReference);
            }

            return;
        }

        var parenthesisExpression = navigator.TryGetParenthesisExpression(scalarExpression);
        if (parenthesisExpression is not null)
        {
            var operand = navigator.TryGetParenthesisExpressionOperand(parenthesisExpression);
            if (operand is not null)
            {
                BindScalarExpression(operand, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var scalarSubquery = navigator.TryGetScalarSubquery(scalarExpression);
        if (scalarSubquery is not null)
        {
            BindScalarSubquery(scalarSubquery, scope, inputRowset);
            return;
        }

        var functionCall = navigator.TryGetFunctionCall(scalarExpression);
        if (functionCall is not null)
        {
            var isAggregate = IsAggregateFunctionCall(functionCall);
            if (navigator.HasFunctionCallCallTarget(functionCall))
            {
                var callTargetParts = navigator.GetFunctionCallCallTargetParts(functionCall);
                if (callTargetParts.Count == 0)
                {
                    issues.Add(new TransformBindingIssue(
                        "UnsupportedFunctionCallTargetShape",
                        $"Function call '{functionCall.Id}' uses a call target shape that is not yet supported by binding.",
                        functionCall.Id));
                }
                else
                {
                    var boundCallTarget = BindColumnReferenceFromIdentifierParts(
                        callTargetParts,
                        $"{functionCall.Id}:call-target",
                        scope,
                        groupingContext,
                        withinAggregate || isAggregate);
                    if (boundCallTarget is not null)
                    {
                        boundColumnReferences.Add(boundCallTarget);
                    }
                }
            }

            var functionParameters = navigator.GetFunctionCallParameters(functionCall);
            for (var parameterOrdinal = 0; parameterOrdinal < functionParameters.Count; parameterOrdinal++)
            {
                var parameter = functionParameters[parameterOrdinal];
                if (ShouldSkipFunctionCallParameterBinding(functionCall, parameter, parameterOrdinal))
                {
                    continue;
                }

                BindScalarExpression(parameter, scope, inputRowset, groupingContext, withinAggregate || isAggregate);
            }

            BindFunctionCallWindowClauses(functionCall, scope, inputRowset, groupingContext);

            return;
        }

        var leftFunctionCall = navigator.TryGetLeftFunctionCall(scalarExpression);
        if (leftFunctionCall is not null)
        {
            foreach (var parameter in navigator.GetLeftFunctionCallParameters(leftFunctionCall))
            {
                BindScalarExpression(parameter, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var rightFunctionCall = navigator.TryGetRightFunctionCall(scalarExpression);
        if (rightFunctionCall is not null)
        {
            foreach (var parameter in navigator.GetRightFunctionCallParameters(rightFunctionCall))
            {
                BindScalarExpression(parameter, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        if (navigator.TryGetParameterlessCall(scalarExpression) is not null)
        {
            return;
        }

        if (navigator.TryGetNextValueForExpression(scalarExpression) is not null)
        {
            return;
        }

        if (navigator.IsValueExpression(scalarExpression))
        {
            return;
        }

        var coalesceExpression = navigator.TryGetCoalesceExpression(scalarExpression);
        if (coalesceExpression is not null)
        {
            foreach (var expression in navigator.GetCoalesceExpressions(coalesceExpression))
            {
                BindScalarExpression(expression, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var nullIfExpression = navigator.TryGetNullIfExpression(scalarExpression);
        if (nullIfExpression is not null)
        {
            var operands = navigator.TryGetNullIfExpressionOperands(nullIfExpression);
            if (operands is not null)
            {
                if (operands.Value.First is not null)
                {
                    BindScalarExpression(operands.Value.First, scope, inputRowset, groupingContext, withinAggregate);
                }

                if (operands.Value.Second is not null)
                {
                    BindScalarExpression(operands.Value.Second, scope, inputRowset, groupingContext, withinAggregate);
                }
            }

            return;
        }

        var iIfCall = navigator.TryGetIIfCall(scalarExpression);
        if (iIfCall is not null)
        {
            var predicate = navigator.TryGetIIfPredicate(iIfCall);
            if (predicate is not null)
            {
                BindBooleanExpression(predicate, scope, inputRowset, groupingContext, withinAggregate);
            }

            var thenExpression = navigator.TryGetIIfThenExpression(iIfCall);
            if (thenExpression is not null)
            {
                BindScalarExpression(thenExpression, scope, inputRowset, groupingContext, withinAggregate);
            }

            var elseExpression = navigator.TryGetIIfElseExpression(iIfCall);
            if (elseExpression is not null)
            {
                BindScalarExpression(elseExpression, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var searchedCaseExpression = navigator.TryGetSearchedCaseExpression(scalarExpression);
        if (searchedCaseExpression is not null)
        {
            BindSearchedCaseExpression(searchedCaseExpression, scope, inputRowset, groupingContext, withinAggregate);
            return;
        }

        var simpleCaseExpression = navigator.TryGetSimpleCaseExpression(scalarExpression);
        if (simpleCaseExpression is not null)
        {
            BindSimpleCaseExpression(simpleCaseExpression, scope, inputRowset, groupingContext, withinAggregate);
            return;
        }

        var castCall = navigator.TryGetCastCall(scalarExpression);
        if (castCall is not null)
        {
            var parameter = navigator.TryGetCastCallParameter(castCall);
            if (parameter is not null)
            {
                BindScalarExpression(parameter, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var tryCastCall = navigator.TryGetTryCastCall(scalarExpression);
        if (tryCastCall is not null)
        {
            var parameter = navigator.TryGetTryCastCallParameter(tryCastCall);
            if (parameter is not null)
            {
                BindScalarExpression(parameter, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var convertCall = navigator.TryGetConvertCall(scalarExpression);
        if (convertCall is not null)
        {
            var parameter = navigator.TryGetConvertCallParameter(convertCall);
            if (parameter is not null)
            {
                BindScalarExpression(parameter, scope, inputRowset, groupingContext, withinAggregate);
            }

            var style = navigator.TryGetConvertCallStyle(convertCall);
            if (style is not null)
            {
                BindScalarExpression(style, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var tryConvertCall = navigator.TryGetTryConvertCall(scalarExpression);
        if (tryConvertCall is not null)
        {
            var parameter = navigator.TryGetTryConvertCallParameter(tryConvertCall);
            if (parameter is not null)
            {
                BindScalarExpression(parameter, scope, inputRowset, groupingContext, withinAggregate);
            }

            var style = navigator.TryGetTryConvertCallStyle(tryConvertCall);
            if (style is not null)
            {
                BindScalarExpression(style, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var parseCall = navigator.TryGetParseCall(scalarExpression);
        if (parseCall is not null)
        {
            var stringValue = navigator.TryGetParseCallStringValue(parseCall);
            if (stringValue is not null)
            {
                BindScalarExpression(stringValue, scope, inputRowset, groupingContext, withinAggregate);
            }

            var culture = navigator.TryGetParseCallCulture(parseCall);
            if (culture is not null)
            {
                BindScalarExpression(culture, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var tryParseCall = navigator.TryGetTryParseCall(scalarExpression);
        if (tryParseCall is not null)
        {
            var stringValue = navigator.TryGetTryParseCallStringValue(tryParseCall);
            if (stringValue is not null)
            {
                BindScalarExpression(stringValue, scope, inputRowset, groupingContext, withinAggregate);
            }

            var culture = navigator.TryGetTryParseCallCulture(tryParseCall);
            if (culture is not null)
            {
                BindScalarExpression(culture, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var atTimeZoneCall = navigator.TryGetAtTimeZoneCall(scalarExpression);
        if (atTimeZoneCall is not null)
        {
            var dateValue = navigator.TryGetAtTimeZoneDateValue(atTimeZoneCall);
            if (dateValue is not null)
            {
                BindScalarExpression(dateValue, scope, inputRowset, groupingContext, withinAggregate);
            }

            var timeZone = navigator.TryGetAtTimeZoneTimeZone(atTimeZoneCall);
            if (timeZone is not null)
            {
                BindScalarExpression(timeZone, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        issues.Add(new TransformBindingIssue(
            "UnsupportedScalarExpressionShape",
            $"ScalarExpression '{scalarExpression.Id}' is not yet supported by binding traversal.",
            scalarExpression.Id));
    }

    private void BindBooleanExpression(
        BooleanExpression booleanExpression,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeGroupingContext? groupingContext,
        bool withinAggregate = false)
    {
        var booleanBinaryExpression = navigator.TryGetBooleanBinaryExpression(booleanExpression);
        if (booleanBinaryExpression is not null)
        {
            var children = navigator.TryGetBooleanBinaryExpressionChildren(booleanBinaryExpression);
            if (children is not null)
            {
                if (children.Value.First is not null)
                {
                    BindBooleanExpression(children.Value.First, scope, inputRowset, groupingContext, withinAggregate);
                }

                if (children.Value.Second is not null)
                {
                    BindBooleanExpression(children.Value.Second, scope, inputRowset, groupingContext, withinAggregate);
                }
            }

            return;
        }

        var booleanNotExpression = navigator.TryGetBooleanNotExpression(booleanExpression);
        if (booleanNotExpression is not null)
        {
            var operand = navigator.TryGetBooleanNotExpressionOperand(booleanNotExpression);
            if (operand is not null)
            {
                BindBooleanExpression(operand, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var booleanParenthesisExpression = navigator.TryGetBooleanParenthesisExpression(booleanExpression);
        if (booleanParenthesisExpression is not null)
        {
            var operand = navigator.TryGetBooleanParenthesisExpressionOperand(booleanParenthesisExpression);
            if (operand is not null)
            {
                BindBooleanExpression(operand, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var booleanComparisonExpression = navigator.TryGetBooleanComparisonExpression(booleanExpression);
        if (booleanComparisonExpression is not null)
        {
            var operands = navigator.TryGetBooleanComparisonExpressionOperands(booleanComparisonExpression);
            if (operands is not null)
            {
                if (operands.Value.First is not null)
                {
                    BindScalarExpression(operands.Value.First, scope, inputRowset, groupingContext, withinAggregate);
                }

                if (operands.Value.Second is not null)
                {
                    BindScalarExpression(operands.Value.Second, scope, inputRowset, groupingContext, withinAggregate);
                }
            }

            return;
        }

        var booleanTernaryExpression = navigator.TryGetBooleanTernaryExpression(booleanExpression);
        if (booleanTernaryExpression is not null)
        {
            var operands = navigator.TryGetBooleanTernaryExpressionOperands(booleanTernaryExpression);
            if (operands is not null)
            {
                if (operands.Value.First is not null)
                {
                    BindScalarExpression(operands.Value.First, scope, inputRowset, groupingContext, withinAggregate);
                }

                if (operands.Value.Second is not null)
                {
                    BindScalarExpression(operands.Value.Second, scope, inputRowset, groupingContext, withinAggregate);
                }

                if (operands.Value.Third is not null)
                {
                    BindScalarExpression(operands.Value.Third, scope, inputRowset, groupingContext, withinAggregate);
                }
            }

            return;
        }

        var booleanIsNullExpression = navigator.TryGetBooleanIsNullExpression(booleanExpression);
        if (booleanIsNullExpression is not null)
        {
            var operand = navigator.TryGetBooleanIsNullExpressionOperand(booleanIsNullExpression);
            if (operand is not null)
            {
                BindScalarExpression(operand, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var likePredicate = navigator.TryGetLikePredicate(booleanExpression);
        if (likePredicate is not null)
        {
            var operands = navigator.TryGetLikePredicateOperands(likePredicate);
            if (operands is not null)
            {
                if (operands.Value.First is not null)
                {
                    BindScalarExpression(operands.Value.First, scope, inputRowset, groupingContext, withinAggregate);
                }

                if (operands.Value.Second is not null)
                {
                    BindScalarExpression(operands.Value.Second, scope, inputRowset, groupingContext, withinAggregate);
                }
            }

            var escapeExpression = navigator.TryGetLikePredicateEscapeExpression(likePredicate);
            if (escapeExpression is not null)
            {
                BindScalarExpression(escapeExpression, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var distinctPredicate = navigator.TryGetDistinctPredicate(booleanExpression);
        if (distinctPredicate is not null)
        {
            var operands = navigator.TryGetDistinctPredicateOperands(distinctPredicate);
            if (operands is not null)
            {
                if (operands.Value.First is not null)
                {
                    BindScalarExpression(operands.Value.First, scope, inputRowset, groupingContext, withinAggregate);
                }

                if (operands.Value.Second is not null)
                {
                    BindScalarExpression(operands.Value.Second, scope, inputRowset, groupingContext, withinAggregate);
                }
            }

            return;
        }

        var fullTextPredicate = navigator.TryGetFullTextPredicate(booleanExpression);
        if (fullTextPredicate is not null)
        {
            foreach (var column in navigator.GetFullTextPredicateColumns(fullTextPredicate))
            {
                var boundColumnReference = BindColumnReference(column, scope, groupingContext, withinAggregate);
                if (boundColumnReference is not null)
                {
                    boundColumnReferences.Add(boundColumnReference);
                }
            }

            var valueExpression = navigator.TryGetFullTextPredicateValueExpression(fullTextPredicate);
            if (valueExpression is not null)
            {
                BindScalarExpression(valueExpression, scope, inputRowset, groupingContext, withinAggregate);
            }

            return;
        }

        var existsPredicate = navigator.TryGetExistsPredicate(booleanExpression);
        if (existsPredicate is not null)
        {
            var subquery = navigator.TryGetExistsPredicateSubquery(existsPredicate);
            if (subquery is not null)
            {
                BindPredicateSubquery(subquery, scope, inputRowset, existsPredicate.Id, requireSingleColumn: false);
            }

            return;
        }

        var inPredicate = navigator.TryGetInPredicate(booleanExpression);
        if (inPredicate is not null)
        {
            var expression = navigator.TryGetInPredicateExpression(inPredicate);
            if (expression is not null)
            {
                BindScalarExpression(expression, scope, inputRowset, groupingContext, withinAggregate);
            }

            foreach (var value in navigator.GetInPredicateValues(inPredicate))
            {
                BindScalarExpression(value, scope, inputRowset, groupingContext, withinAggregate);
            }

            var subquery = navigator.TryGetInPredicateSubquery(inPredicate);
            if (subquery is not null)
            {
                BindPredicateSubquery(subquery, scope, inputRowset, inPredicate.Id, requireSingleColumn: true);
            }

            return;
        }

        var subqueryComparisonPredicate = navigator.TryGetSubqueryComparisonPredicate(booleanExpression);
        if (subqueryComparisonPredicate is not null)
        {
            var expression = navigator.TryGetSubqueryComparisonPredicateExpression(subqueryComparisonPredicate);
            if (expression is not null)
            {
                BindScalarExpression(expression, scope, inputRowset, groupingContext, withinAggregate);
            }

            var subquery = navigator.TryGetSubqueryComparisonPredicateSubquery(subqueryComparisonPredicate);
            if (subquery is not null)
            {
                BindPredicateSubquery(subquery, scope, inputRowset, subqueryComparisonPredicate.Id, requireSingleColumn: true);
            }

            return;
        }

        issues.Add(new TransformBindingIssue(
            "UnsupportedBooleanExpressionShape",
            $"BooleanExpression '{booleanExpression.Id}' is not yet supported by binding traversal.",
            booleanExpression.Id));
    }

    private void BindSearchedCaseExpression(
        SearchedCaseExpression searchedCaseExpression,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeGroupingContext? groupingContext,
        bool withinAggregate)
    {
        foreach (var whenClause in navigator.GetSearchedWhenClauses(searchedCaseExpression))
        {
            var whenCondition = navigator.TryGetSearchedWhenClauseCondition(whenClause);
            if (whenCondition is not null)
            {
                BindBooleanExpression(whenCondition, scope, inputRowset, groupingContext, withinAggregate);
            }

            var thenExpression = navigator.TryGetWhenClauseThenExpression(whenClause);
            if (thenExpression is not null)
            {
                BindScalarExpression(thenExpression, scope, inputRowset, groupingContext, withinAggregate);
            }
        }

        var elseExpression = navigator.TryGetCaseElseExpression(searchedCaseExpression);
        if (elseExpression is not null)
        {
            BindScalarExpression(elseExpression, scope, inputRowset, groupingContext, withinAggregate);
        }
    }

    private void BindSimpleCaseExpression(
        SimpleCaseExpression simpleCaseExpression,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        RuntimeGroupingContext? groupingContext,
        bool withinAggregate)
    {
        var inputExpression = navigator.TryGetSimpleCaseInputExpression(simpleCaseExpression);
        if (inputExpression is not null)
        {
            BindScalarExpression(inputExpression, scope, inputRowset, groupingContext, withinAggregate);
        }

        foreach (var whenClause in navigator.GetSimpleWhenClauses(simpleCaseExpression))
        {
            var whenExpression = navigator.TryGetSimpleWhenClauseWhenExpression(whenClause);
            if (whenExpression is not null)
            {
                BindScalarExpression(whenExpression, scope, inputRowset, groupingContext, withinAggregate);
            }

            var thenExpression = navigator.TryGetWhenClauseThenExpression(whenClause);
            if (thenExpression is not null)
            {
                BindScalarExpression(thenExpression, scope, inputRowset, groupingContext, withinAggregate);
            }
        }

        var elseExpression = navigator.TryGetCaseElseExpression(simpleCaseExpression);
        if (elseExpression is not null)
        {
            BindScalarExpression(elseExpression, scope, inputRowset, groupingContext, withinAggregate);
        }
    }

    private bool IsAggregateFunctionCall(FunctionCall functionCall)
    {
        if (navigator.HasFunctionCallOverClause(functionCall))
        {
            return false;
        }

        var functionName = navigator.TryGetFunctionCallName(functionCall);
        if (string.IsNullOrWhiteSpace(functionName))
        {
            return false;
        }

        return functionName.Trim().ToUpperInvariant() switch
        {
            "APPROX_COUNT_DISTINCT" => true,
            "AVG" => true,
            "CHECKSUM_AGG" => true,
            "COUNT" => true,
            "COUNT_BIG" => true,
            "GROUPING" => true,
            "GROUPING_ID" => true,
            "MAX" => true,
            "MIN" => true,
            "STDEV" => true,
            "STDEVP" => true,
            "STDDEV_SAMP" => true,
            "STDDEV_POP" => true,
            "STRING_AGG" => true,
            "SUM" => true,
            "VAR" => true,
            "VARP" => true,
            "VAR_SAMP" => true,
            "VAR_POP" => true,
            _ => false
        };
    }

    private bool ShouldSkipFunctionCallParameterBinding(FunctionCall functionCall, ScalarExpression parameter, int parameterOrdinal)
    {
        var directColumnReference = navigator.TryGetDirectColumnReference(parameter);
        if (directColumnReference is null)
        {
            return false;
        }

        var functionName = navigator.TryGetFunctionCallName(functionCall);
        if (string.IsNullOrWhiteSpace(functionName))
        {
            return false;
        }

        var normalizedFunctionName = functionName.Trim().ToUpperInvariant();
        var parts = navigator.GetColumnReferenceParts(directColumnReference);

        if (parameterOrdinal == 0 &&
            parts.Count == 1 &&
            IsDatePartToken(parts[0]) &&
            IsDatePartFunctionName(normalizedFunctionName))
        {
            return true;
        }

        if (parts.Count != 0)
        {
            return false;
        }

        return normalizedFunctionName switch
        {
            "COUNT" => true,
            "COUNT_BIG" => true,
            _ => false
        };
    }

    private static bool IsDatePartFunctionName(string functionName)
    {
        return functionName switch
        {
            "DATEADD" => true,
            "DATEDIFF" => true,
            "DATEDIFF_BIG" => true,
            "DATENAME" => true,
            "DATEPART" => true,
            "DATETRUNC" => true,
            "EXTRACT" => true,
            _ => false
        };
    }

    private static bool IsDatePartToken(string token)
    {
        return token.Trim().ToUpperInvariant() switch
        {
            "YEAR" => true,
            "YY" => true,
            "YYYY" => true,
            "QUARTER" => true,
            "QQ" => true,
            "Q" => true,
            "MONTH" => true,
            "MM" => true,
            "M" => true,
            "DAYOFYEAR" => true,
            "DY" => true,
            "Y" => true,
            "DAY" => true,
            "DD" => true,
            "D" => true,
            "WEEK" => true,
            "WK" => true,
            "WW" => true,
            "ISOWK" => true,
            "ISOWW" => true,
            "WEEKDAY" => true,
            "DW" => true,
            "W" => true,
            "HOUR" => true,
            "HH" => true,
            "MINUTE" => true,
            "MI" => true,
            "N" => true,
            "SECOND" => true,
            "SS" => true,
            "S" => true,
            "MILLISECOND" => true,
            "MS" => true,
            "MICROSECOND" => true,
            "MCS" => true,
            "NANOSECOND" => true,
            "NS" => true,
            "TZOFFSET" => true,
            "ISO_WEEK" => true,
            _ => false
        };
    }

    private void BindScalarSubquery(
        ScalarSubquery scalarSubquery,
        BindingScope scope,
        RuntimeRowset? inputRowset)
    {
        BindPredicateSubquery(scalarSubquery, scope, inputRowset, scalarSubquery.Id, requireSingleColumn: true);
    }

    private void BindPredicateSubquery(
        ScalarSubquery scalarSubquery,
        BindingScope scope,
        RuntimeRowset? inputRowset,
        string syntaxId,
        bool requireSingleColumn)
    {
        var queryExpressionId = navigator.TryGetScalarSubqueryQueryExpressionId(scalarSubquery);
        if (string.IsNullOrWhiteSpace(queryExpressionId))
        {
            issues.Add(new TransformBindingIssue(
                "ScalarSubqueryQueryExpressionMissing",
                $"ScalarSubquery '{scalarSubquery.Id}' is missing its inner query expression.",
                syntaxId));
            return;
        }

        var subqueryBinding = BindQueryExpression(
            queryExpressionId,
            $"{queryExpressionId}:output-rowset",
            $"Subquery:{scalarSubquery.Id}",
            null,
            int.MaxValue,
            scope.VisibleTableSources,
            inputRowset,
            ["Value"]);

        if (subqueryBinding is null)
        {
            return;
        }

        if (requireSingleColumn && subqueryBinding.OutputRowset.Columns.Count != 1)
        {
            issues.Add(new TransformBindingIssue(
                "SubqueryOutputColumnCountMismatch",
                $"Subquery '{scalarSubquery.Id}' produces {subqueryBinding.OutputRowset.Columns.Count} columns where binding currently requires exactly one.",
                syntaxId));
        }
    }
}
