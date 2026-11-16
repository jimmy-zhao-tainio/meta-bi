using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderBooleanExpression(BooleanExpression booleanExpression)
    {
        var booleanBinary = FindByBaseId(model.BooleanBinaryExpressionList, static row => row.BooleanExpression?.Id, nameof(BooleanBinaryExpression.BooleanExpression), booleanExpression.Id);
        if (booleanBinary is not null)
        {
            var left = RenderBooleanExpression(GetOwnerLink(model.BooleanBinaryExpressionFirstExpressionLinkList, static row => row.BooleanBinaryExpression?.Id, nameof(BooleanBinaryExpressionFirstExpressionLink.BooleanBinaryExpression), booleanBinary.Id, "BooleanBinaryExpression.FirstExpression").BooleanExpression);
            var right = RenderBooleanExpression(GetOwnerLink(model.BooleanBinaryExpressionSecondExpressionLinkList, static row => row.BooleanBinaryExpression?.Id, nameof(BooleanBinaryExpressionSecondExpressionLink.BooleanBinaryExpression), booleanBinary.Id, "BooleanBinaryExpression.SecondExpression").BooleanExpression);
            var op = booleanBinary.BinaryExpressionType switch
            {
                "And" => "AND",
                "Or" => "OR",
                _ => throw new InvalidOperationException($"Unsupported MetaTransformScript BooleanBinaryExpressionType '{booleanBinary.BinaryExpressionType}'.")
            };
            return $"{left} {op} {right}";
        }

        var booleanComparison = FindByBaseId(model.BooleanComparisonExpressionList, static row => row.BooleanExpression?.Id, nameof(BooleanComparisonExpression.BooleanExpression), booleanExpression.Id);
        if (booleanComparison is not null)
        {
            var left = RenderScalarExpression(GetOwnerLink(model.BooleanComparisonExpressionFirstExpressionLinkList, static row => row.BooleanComparisonExpression?.Id, nameof(BooleanComparisonExpressionFirstExpressionLink.BooleanComparisonExpression), booleanComparison.Id, "BooleanComparisonExpression.FirstExpression").ScalarExpression);
            var right = RenderScalarExpression(GetOwnerLink(model.BooleanComparisonExpressionSecondExpressionLinkList, static row => row.BooleanComparisonExpression?.Id, nameof(BooleanComparisonExpressionSecondExpressionLink.BooleanComparisonExpression), booleanComparison.Id, "BooleanComparisonExpression.SecondExpression").ScalarExpression);
            var op = RenderComparisonOperator(booleanComparison.ComparisonType);
            return $"{left} {op} {right}";
        }

        var subqueryComparisonPredicate = FindByBaseId(model.SubqueryComparisonPredicateList, static row => row.BooleanExpression?.Id, nameof(SubqueryComparisonPredicate.BooleanExpression), booleanExpression.Id);
        if (subqueryComparisonPredicate is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(
                model.SubqueryComparisonPredicateExpressionLinkList, static row => row.SubqueryComparisonPredicate?.Id, nameof(SubqueryComparisonPredicateExpressionLink.SubqueryComparisonPredicate),
                subqueryComparisonPredicate.Id,
                "SubqueryComparisonPredicate.Expression").ScalarExpression);
            var subquery = RenderScalarSubquery(GetOwnerLink(
                model.SubqueryComparisonPredicateSubqueryLinkList, static row => row.SubqueryComparisonPredicate?.Id, nameof(SubqueryComparisonPredicateSubqueryLink.SubqueryComparisonPredicate),
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

        var booleanNot = FindByBaseId(model.BooleanNotExpressionList, static row => row.BooleanExpression?.Id, nameof(BooleanNotExpression.BooleanExpression), booleanExpression.Id);
        if (booleanNot is not null)
        {
            var child = GetOwnerLink(model.BooleanNotExpressionExpressionLinkList, static row => row.BooleanNotExpression?.Id, nameof(BooleanNotExpressionExpressionLink.BooleanNotExpression), booleanNot.Id, "BooleanNotExpression.Expression").BooleanExpression;
            if (FindByBaseId(model.BooleanTernaryExpressionList, static row => row.BooleanExpression?.Id, nameof(BooleanTernaryExpression.BooleanExpression), child.Id) is { } negatedBetween
                && string.Equals(negatedBetween.TernaryExpressionType, "Between", StringComparison.Ordinal))
            {
                var first = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionFirstExpressionLinkList, static row => row.BooleanTernaryExpression?.Id, nameof(BooleanTernaryExpressionFirstExpressionLink.BooleanTernaryExpression), negatedBetween.Id, "BooleanTernaryExpression.FirstExpression").ScalarExpression);
                var second = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionSecondExpressionLinkList, static row => row.BooleanTernaryExpression?.Id, nameof(BooleanTernaryExpressionSecondExpressionLink.BooleanTernaryExpression), negatedBetween.Id, "BooleanTernaryExpression.SecondExpression").ScalarExpression);
                var third = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionThirdExpressionLinkList, static row => row.BooleanTernaryExpression?.Id, nameof(BooleanTernaryExpressionThirdExpressionLink.BooleanTernaryExpression), negatedBetween.Id, "BooleanTernaryExpression.ThirdExpression").ScalarExpression);
                return $"{first} NOT BETWEEN {second} AND {third}";
            }

            return FindByBaseId(model.BooleanParenthesisExpressionList, static row => row.BooleanExpression?.Id, nameof(BooleanParenthesisExpression.BooleanExpression), child.Id) is not null
                ? $"NOT {RenderBooleanExpression(child)}"
                : $"NOT ({RenderBooleanExpression(child)})";
        }

        var booleanParenthesis = FindByBaseId(model.BooleanParenthesisExpressionList, static row => row.BooleanExpression?.Id, nameof(BooleanParenthesisExpression.BooleanExpression), booleanExpression.Id);
        if (booleanParenthesis is not null)
        {
            return $"({RenderBooleanExpression(GetOwnerLink(model.BooleanParenthesisExpressionExpressionLinkList, static row => row.BooleanParenthesisExpression?.Id, nameof(BooleanParenthesisExpressionExpressionLink.BooleanParenthesisExpression), booleanParenthesis.Id, "BooleanParenthesisExpression.Expression").BooleanExpression)})";
        }

        var booleanIsNull = FindByBaseId(model.BooleanIsNullExpressionList, static row => row.BooleanExpression?.Id, nameof(BooleanIsNullExpression.BooleanExpression), booleanExpression.Id);
        if (booleanIsNull is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(model.BooleanIsNullExpressionExpressionLinkList, static row => row.BooleanIsNullExpression?.Id, nameof(BooleanIsNullExpressionExpressionLink.BooleanIsNullExpression), booleanIsNull.Id, "BooleanIsNullExpression.Expression").ScalarExpression);
            return $"{expression} IS {(IsTrue(booleanIsNull.IsNot) ? "NOT " : string.Empty)}NULL";
        }

        var booleanTernary = FindByBaseId(model.BooleanTernaryExpressionList, static row => row.BooleanExpression?.Id, nameof(BooleanTernaryExpression.BooleanExpression), booleanExpression.Id);
        if (booleanTernary is not null)
        {
            if (!string.Equals(booleanTernary.TernaryExpressionType, "Between", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Unsupported MetaTransformScript TernaryExpressionType '{booleanTernary.TernaryExpressionType}'.");
            }

            var first = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionFirstExpressionLinkList, static row => row.BooleanTernaryExpression?.Id, nameof(BooleanTernaryExpressionFirstExpressionLink.BooleanTernaryExpression), booleanTernary.Id, "BooleanTernaryExpression.FirstExpression").ScalarExpression);
            var second = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionSecondExpressionLinkList, static row => row.BooleanTernaryExpression?.Id, nameof(BooleanTernaryExpressionSecondExpressionLink.BooleanTernaryExpression), booleanTernary.Id, "BooleanTernaryExpression.SecondExpression").ScalarExpression);
            var third = RenderScalarExpression(GetOwnerLink(model.BooleanTernaryExpressionThirdExpressionLinkList, static row => row.BooleanTernaryExpression?.Id, nameof(BooleanTernaryExpressionThirdExpressionLink.BooleanTernaryExpression), booleanTernary.Id, "BooleanTernaryExpression.ThirdExpression").ScalarExpression);
            return $"{first} BETWEEN {second} AND {third}";
        }

        var inPredicate = FindByBaseId(model.InPredicateList, static row => row.BooleanExpression?.Id, nameof(InPredicate.BooleanExpression), booleanExpression.Id);
        if (inPredicate is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(model.InPredicateExpressionLinkList, static row => row.InPredicate?.Id, nameof(InPredicateExpressionLink.InPredicate), inPredicate.Id, "InPredicate.Expression").ScalarExpression);
            var notText = IsTrue(inPredicate.NotDefined) ? " NOT" : string.Empty;
            var subqueryLink = FindOwnerLink(model.InPredicateSubqueryLinkList, static row => row.InPredicate?.Id, nameof(InPredicateSubqueryLink.InPredicate), inPredicate.Id);
            if (subqueryLink is not null)
            {
                return $"{expression}{notText} IN {RenderScalarSubquery(subqueryLink.ScalarSubquery)}";
            }

            var values = GetOrderedItems(model.InPredicateValuesItemList, static row => row.InPredicate?.Id, nameof(InPredicateValuesItem.InPredicate), static row => row.Ordinal, inPredicate.Id)
                .Select(row => RenderScalarExpression(row.ScalarExpression))
                .ToArray();
            return $"{expression}{notText} IN ({string.Join(", ", values)})";
        }

        var distinctPredicate = FindByBaseId(model.DistinctPredicateList, static row => row.BooleanExpression?.Id, nameof(DistinctPredicate.BooleanExpression), booleanExpression.Id);
        if (distinctPredicate is not null)
        {
            var first = RenderScalarExpression(GetOwnerLink(
                model.DistinctPredicateFirstExpressionLinkList, static row => row.DistinctPredicate?.Id, nameof(DistinctPredicateFirstExpressionLink.DistinctPredicate),
                distinctPredicate.Id,
                "DistinctPredicate.FirstExpression").ScalarExpression);
            var second = RenderScalarExpression(GetOwnerLink(
                model.DistinctPredicateSecondExpressionLinkList, static row => row.DistinctPredicate?.Id, nameof(DistinctPredicateSecondExpressionLink.DistinctPredicate),
                distinctPredicate.Id,
                "DistinctPredicate.SecondExpression").ScalarExpression);
            var notText = IsTrue(distinctPredicate.IsNot) ? " NOT" : string.Empty;
            return $"{first} IS{notText} DISTINCT FROM {second}";
        }

        var likePredicate = FindByBaseId(model.LikePredicateList, static row => row.BooleanExpression?.Id, nameof(LikePredicate.BooleanExpression), booleanExpression.Id);
        if (likePredicate is not null)
        {
            if (IsTrue(likePredicate.OdbcEscape))
            {
                throw new InvalidOperationException("Phase-1 emitter does not support OdbcEscape LIKE predicates.");
            }

            var first = RenderScalarExpression(GetOwnerLink(model.LikePredicateFirstExpressionLinkList, static row => row.LikePredicate?.Id, nameof(LikePredicateFirstExpressionLink.LikePredicate), likePredicate.Id, "LikePredicate.FirstExpression").ScalarExpression);
            var second = RenderScalarExpression(GetOwnerLink(model.LikePredicateSecondExpressionLinkList, static row => row.LikePredicate?.Id, nameof(LikePredicateSecondExpressionLink.LikePredicate), likePredicate.Id, "LikePredicate.SecondExpression").ScalarExpression);
            var notText = IsTrue(likePredicate.NotDefined) ? " NOT" : string.Empty;
            var escapeLink = FindOwnerLink(model.LikePredicateEscapeExpressionLinkList, static row => row.LikePredicate?.Id, nameof(LikePredicateEscapeExpressionLink.LikePredicate), likePredicate.Id);
            return escapeLink is null
                ? $"{first}{notText} LIKE {second}"
                : $"{first}{notText} LIKE {second} ESCAPE {RenderScalarExpression(escapeLink.ScalarExpression)}";
        }

        var fullTextPredicate = FindByBaseId(model.FullTextPredicateList, static row => row.BooleanExpression?.Id, nameof(FullTextPredicate.BooleanExpression), booleanExpression.Id);
        if (fullTextPredicate is not null)
        {
            var functionName = fullTextPredicate.FullTextFunctionType switch
            {
                "Contains" => "CONTAINS",
                "FreeText" => "FREETEXT",
                _ => throw new InvalidOperationException(
                    $"Unsupported MetaTransformScript FullTextPredicate.FullTextFunctionType '{fullTextPredicate.FullTextFunctionType}'.")
            };
            var columns = GetOrderedItems(model.FullTextPredicateColumnsItemList, static row => row.FullTextPredicate?.Id, nameof(FullTextPredicateColumnsItem.FullTextPredicate), static row => row.Ordinal, fullTextPredicate.Id)
                .Select(row => RenderColumnReferenceExpression(row.ColumnReferenceExpression))
                .ToArray();
            var value = RenderValueExpression(GetOwnerLink(
                model.FullTextPredicateValueLinkList, static row => row.FullTextPredicate?.Id, nameof(FullTextPredicateValueLink.FullTextPredicate),
                fullTextPredicate.Id,
                "FullTextPredicate.ValueExpression").ValueExpression);
            return $"{functionName}({RenderFullTextColumns(columns)}, {value})";
        }

        var existsPredicate = FindByBaseId(model.ExistsPredicateList, static row => row.BooleanExpression?.Id, nameof(ExistsPredicate.BooleanExpression), booleanExpression.Id);
        if (existsPredicate is not null)
        {
            var subquery = RenderScalarSubquery(GetOwnerLink(
                model.ExistsPredicateSubqueryLinkList, static row => row.ExistsPredicate?.Id, nameof(ExistsPredicateSubqueryLink.ExistsPredicate),
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
            "GreaterThanOrEqualTo" => ">=",
            "LessThan" => "<",
            "LessThanOrEqualTo" => "<=",
            "NotEqualToBrackets" => "<>",
            _ => throw new InvalidOperationException($"Unsupported MetaTransformScript ComparisonType '{comparisonType}'.")
        };
    }
}
