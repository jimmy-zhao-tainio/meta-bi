using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderBooleanExpression(BooleanExpression booleanExpression)
    {
        var booleanBinary = FindByBaseId(model.BooleanBinaryExpressionList, booleanExpression.Id);
        if (booleanBinary is not null)
        {
            var left = RenderBooleanExpression(GetOwnerLink(model.BooleanBinaryExpressionFirstExpressionLinkList, booleanBinary.Id, "BooleanBinaryExpression.FirstExpression").BooleanExpression);
            var right = RenderBooleanExpression(GetOwnerLink(model.BooleanBinaryExpressionSecondExpressionLinkList, booleanBinary.Id, "BooleanBinaryExpression.SecondExpression").BooleanExpression);
            var op = booleanBinary.BinaryExpressionType switch
            {
                "And" => "AND",
                "Or" => "OR",
                _ => throw new InvalidOperationException($"Unsupported MetaTransformScript BooleanBinaryExpressionType '{booleanBinary.BinaryExpressionType}'.")
            };
            return $"{left} {op} {right}";
        }

        var booleanComparison = FindByBaseId(model.BooleanComparisonExpressionList, booleanExpression.Id);
        if (booleanComparison is not null)
        {
            var left = RenderScalarExpression(GetOwnerLink(model.BooleanComparisonExpressionFirstExpressionLinkList, booleanComparison.Id, "BooleanComparisonExpression.FirstExpression").ScalarExpression);
            var right = RenderScalarExpression(GetOwnerLink(model.BooleanComparisonExpressionSecondExpressionLinkList, booleanComparison.Id, "BooleanComparisonExpression.SecondExpression").ScalarExpression);
            var op = RenderComparisonOperator(booleanComparison.ComparisonType);
            return $"{left} {op} {right}";
        }

        var subqueryComparisonPredicate = FindByBaseId(model.SubqueryComparisonPredicateList, booleanExpression.Id);
        if (subqueryComparisonPredicate is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(
                model.SubqueryComparisonPredicateExpressionLinkList,
                subqueryComparisonPredicate.Id,
                "SubqueryComparisonPredicate.Expression").ScalarExpression);
            var subquery = RenderScalarSubquery(GetOwnerLink(
                model.SubqueryComparisonPredicateSubqueryLinkList,
                subqueryComparisonPredicate.Id,
                "SubqueryComparisonPredicate.Subquery").ScalarSubquery);
            var comparison = RenderComparisonOperator(subqueryComparisonPredicate.ComparisonType);
            var quantifier = subqueryComparisonPredicate.SubqueryComparisonPredicateType switch
            {
                "All" => "ALL",
                "Any" => "ANY",
                _ => throw new InvalidOperationException(
                    $"Unsupported MetaTransformScript SubqueryComparisonPredicateType '{subqueryComparisonPredicate.SubqueryComparisonPredicateType}'.")
            };
            return $"{expression} {comparison} {quantifier} {subquery}";
        }

        var booleanNot = FindByBaseId(model.BooleanNotExpressionList, booleanExpression.Id);
        if (booleanNot is not null)
        {
            var child = GetOwnerLink(model.BooleanNotExpressionExpressionLinkList, booleanNot.Id, "BooleanNotExpression.Expression").BooleanExpression;
            if (FindByBaseId(model.BooleanTernaryExpressionList, child.Id) is { } negatedBetween
                && string.Equals(negatedBetween.TernaryExpressionType, "Between", StringComparison.Ordinal))
            {
                var first = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionFirstExpressionLinkList, negatedBetween.Id, "BooleanTernaryExpression.FirstExpression").ScalarExpression);
                var second = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionSecondExpressionLinkList, negatedBetween.Id, "BooleanTernaryExpression.SecondExpression").ScalarExpression);
                var third = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionThirdExpressionLinkList, negatedBetween.Id, "BooleanTernaryExpression.ThirdExpression").ScalarExpression);
                return $"{first} NOT BETWEEN {second} AND {third}";
            }

            return FindByBaseId(model.BooleanParenthesisExpressionList, child.Id) is not null
                ? $"NOT {RenderBooleanExpression(child)}"
                : $"NOT ({RenderBooleanExpression(child)})";
        }

        var booleanParenthesis = FindByBaseId(model.BooleanParenthesisExpressionList, booleanExpression.Id);
        if (booleanParenthesis is not null)
        {
            return $"({RenderBooleanExpression(GetOwnerLink(model.BooleanParenthesisExpressionExpressionLinkList, booleanParenthesis.Id, "BooleanParenthesisExpression.Expression").BooleanExpression)})";
        }

        var booleanIsNull = FindByBaseId(model.BooleanIsNullExpressionList, booleanExpression.Id);
        if (booleanIsNull is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(model.BooleanIsNullExpressionExpressionLinkList, booleanIsNull.Id, "BooleanIsNullExpression.Expression").ScalarExpression);
            return $"{expression} IS {(IsTrue(booleanIsNull.IsNot) ? "NOT " : string.Empty)}NULL";
        }

        var booleanTernary = FindByBaseId(model.BooleanTernaryExpressionList, booleanExpression.Id);
        if (booleanTernary is not null)
        {
            if (!string.Equals(booleanTernary.TernaryExpressionType, "Between", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Unsupported MetaTransformScript TernaryExpressionType '{booleanTernary.TernaryExpressionType}'.");
            }

            var first = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionFirstExpressionLinkList, booleanTernary.Id, "BooleanTernaryExpression.FirstExpression").ScalarExpression);
            var second = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionSecondExpressionLinkList, booleanTernary.Id, "BooleanTernaryExpression.SecondExpression").ScalarExpression);
            var third = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionThirdExpressionLinkList, booleanTernary.Id, "BooleanTernaryExpression.ThirdExpression").ScalarExpression);
            return $"{first} BETWEEN {second} AND {third}";
        }

        var inPredicate = FindByBaseId(model.InPredicateList, booleanExpression.Id);
        if (inPredicate is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(model.InPredicateExpressionLinkList, inPredicate.Id, "InPredicate.Expression").ScalarExpression);
            var notText = IsTrue(inPredicate.NotDefined) ? " NOT" : string.Empty;
            var subqueryLink = FindOwnerLink(model.InPredicateSubqueryLinkList, inPredicate.Id);
            if (subqueryLink is not null)
            {
                return $"{expression}{notText} IN {RenderScalarSubquery(subqueryLink.ScalarSubquery)}";
            }

            var values = GetOrderedItems(model.InPredicateValuesItemList, inPredicate.Id)
                .Select(row => RenderScalarExpression(row.ScalarExpression))
                .ToArray();
            return $"{expression}{notText} IN ({string.Join(", ", values)})";
        }

        var distinctPredicate = FindByBaseId(model.DistinctPredicateList, booleanExpression.Id);
        if (distinctPredicate is not null)
        {
            var first = RenderScalarExpression(GetOwnerLink(
                model.DistinctPredicateFirstExpressionLinkList,
                distinctPredicate.Id,
                "DistinctPredicate.FirstExpression").ScalarExpression);
            var second = RenderScalarExpression(GetOwnerLink(
                model.DistinctPredicateSecondExpressionLinkList,
                distinctPredicate.Id,
                "DistinctPredicate.SecondExpression").ScalarExpression);
            var notText = IsTrue(distinctPredicate.IsNot) ? " NOT" : string.Empty;
            return $"{first} IS{notText} DISTINCT FROM {second}";
        }

        var likePredicate = FindByBaseId(model.LikePredicateList, booleanExpression.Id);
        if (likePredicate is not null)
        {
            if (IsTrue(likePredicate.OdbcEscape))
            {
                throw new InvalidOperationException("Phase-1 emitter does not support OdbcEscape LIKE predicates.");
            }

            var first = RenderScalarExpression(GetOwnerLink(model.LikePredicateFirstExpressionLinkList, likePredicate.Id, "LikePredicate.FirstExpression").ScalarExpression);
            var second = RenderScalarExpression(GetOwnerLink(model.LikePredicateSecondExpressionLinkList, likePredicate.Id, "LikePredicate.SecondExpression").ScalarExpression);
            var notText = IsTrue(likePredicate.NotDefined) ? " NOT" : string.Empty;
            var escapeLink = FindOwnerLink(model.LikePredicateEscapeExpressionLinkList, likePredicate.Id);
            return escapeLink is null
                ? $"{first}{notText} LIKE {second}"
                : $"{first}{notText} LIKE {second} ESCAPE {RenderScalarExpression(escapeLink.ScalarExpression)}";
        }

        var fullTextPredicate = FindByBaseId(model.FullTextPredicateList, booleanExpression.Id);
        if (fullTextPredicate is not null)
        {
            var functionName = fullTextPredicate.FullTextFunctionType switch
            {
                "Contains" => "CONTAINS",
                "FreeText" => "FREETEXT",
                _ => throw new InvalidOperationException(
                    $"Unsupported MetaTransformScript FullTextPredicate.FullTextFunctionType '{fullTextPredicate.FullTextFunctionType}'.")
            };
            var columns = GetOrderedItems(model.FullTextPredicateColumnsItemList, fullTextPredicate.Id)
                .Select(row => RenderColumnReferenceExpression(row.ColumnReferenceExpression))
                .ToArray();
            var value = RenderValueExpression(GetOwnerLink(
                model.FullTextPredicateValueLinkList,
                fullTextPredicate.Id,
                "FullTextPredicate.ValueExpression").ValueExpression);
            return $"{functionName}({RenderFullTextColumns(columns)}, {value})";
        }

        var existsPredicate = FindByBaseId(model.ExistsPredicateList, booleanExpression.Id);
        if (existsPredicate is not null)
        {
            var subquery = RenderScalarSubquery(GetOwnerLink(
                model.ExistsPredicateSubqueryLinkList,
                existsPredicate.Id,
                "ExistsPredicate.Subquery").ScalarSubquery);
            return "EXISTS " + subquery;
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript BooleanExpression id '{booleanExpression.Id}'.");
    }

    private static string RenderComparisonOperator(string? comparisonType)
    {
        return comparisonType switch
        {
            "Equals" => "=",
            "GreaterThan" => ">",
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript ComparisonType '{comparisonType}'.")
        };
    }
}
