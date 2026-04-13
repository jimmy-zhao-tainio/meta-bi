using System.Text;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderScalarExpression(ScalarExpression scalarExpression)
    {
        var binaryExpression = FindByBaseId(model.BinaryExpressionList, scalarExpression.Id);
        if (binaryExpression is not null)
        {
            var left = RenderScalarExpression(GetOwnerLink(model.BinaryExpressionFirstExpressionLinkList, binaryExpression.Id, "BinaryExpression.FirstExpression").ScalarExpression);
            var right = RenderScalarExpression(GetOwnerLink(model.BinaryExpressionSecondExpressionLinkList, binaryExpression.Id, "BinaryExpression.SecondExpression").ScalarExpression);
            var op = binaryExpression.BinaryExpressionType switch
            {
                "Add" => "+",
                "Subtract" => "-",
                "Multiply" => "*",
                "Divide" => "/",
                "Modulo" => "%",
                _ => throw new InvalidOperationException($"Unsupported MetaTransformScript BinaryExpressionType '{binaryExpression.BinaryExpressionType}'.")
            };
            return $"{left} {op} {right}";
        }

        var unaryExpression = FindByBaseId(model.UnaryExpressionList, scalarExpression.Id);
        if (unaryExpression is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(
                model.UnaryExpressionExpressionLinkList,
                unaryExpression.Id,
                "UnaryExpression.Expression").ScalarExpression);
            var op = unaryExpression.UnaryExpressionType switch
            {
                "Negative" => "-",
                "Positive" => "+",
                _ => throw new InvalidOperationException(
                    $"Unsupported MetaTransformScript UnaryExpressionType '{unaryExpression.UnaryExpressionType}'.")
            };
            return op + expression;
        }

        var primaryExpression = FindByBaseId(model.PrimaryExpressionList, scalarExpression.Id)
            ?? throw new InvalidOperationException($"Unsupported MetaTransformScript ScalarExpression id '{scalarExpression.Id}'.");
        return RenderPrimaryExpression(primaryExpression);
    }

    private string RenderPrimaryExpression(PrimaryExpression primaryExpression)
    {
        string rendered;

        if (FindByBaseId(model.ColumnReferenceExpressionList, primaryExpression.Id) is { } columnReference)
        {
            rendered = RenderColumnReferenceExpression(columnReference);
        }
        else if (FindByBaseId(model.ParenthesisExpressionList, primaryExpression.Id) is { } parenthesisExpression)
        {
            rendered = "(" + RenderScalarExpression(GetOwnerLink(
                model.ParenthesisExpressionExpressionLinkList,
                parenthesisExpression.Id,
                "ParenthesisExpression.Expression").ScalarExpression) + ")";
        }
        else if (FindByBaseId(model.CaseExpressionList, primaryExpression.Id) is { } caseExpression)
        {
            if (FindByBaseId(model.SimpleCaseExpressionList, caseExpression.Id) is { } simpleCaseExpression)
            {
                rendered = RenderSimpleCaseExpression(simpleCaseExpression);
            }
            else if (FindByBaseId(model.SearchedCaseExpressionList, caseExpression.Id) is { } searchedCaseExpression)
            {
                rendered = RenderSearchedCaseExpression(searchedCaseExpression);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported MetaTransformScript CaseExpression id '{caseExpression.Id}'.");
            }
        }
        else if (FindByBaseId(model.CoalesceExpressionList, primaryExpression.Id) is { } coalesceExpression)
        {
            rendered = RenderCoalesceExpression(coalesceExpression);
        }
        else if (FindByBaseId(model.NullIfExpressionList, primaryExpression.Id) is { } nullIfExpression)
        {
            rendered = RenderNullIfExpression(nullIfExpression);
        }
        else if (FindByBaseId(model.IIfCallList, primaryExpression.Id) is { } iIfCall)
        {
            rendered = RenderIIfCall(iIfCall);
        }
        else if (FindByBaseId(model.CastCallList, primaryExpression.Id) is { } castCall)
        {
            rendered = RenderCastCall(castCall);
        }
        else if (FindByBaseId(model.TryCastCallList, primaryExpression.Id) is { } tryCastCall)
        {
            rendered = RenderTryCastCall(tryCastCall);
        }
        else if (FindByBaseId(model.ConvertCallList, primaryExpression.Id) is { } convertCall)
        {
            rendered = RenderConvertCall(convertCall);
        }
        else if (FindByBaseId(model.TryConvertCallList, primaryExpression.Id) is { } tryConvertCall)
        {
            rendered = RenderTryConvertCall(tryConvertCall);
        }
        else if (FindByBaseId(model.LeftFunctionCallList, primaryExpression.Id) is { } leftFunctionCall)
        {
            rendered = RenderLeftFunctionCall(leftFunctionCall);
        }
        else if (FindByBaseId(model.RightFunctionCallList, primaryExpression.Id) is { } rightFunctionCall)
        {
            rendered = RenderRightFunctionCall(rightFunctionCall);
        }
        else if (FindByBaseId(model.ParseCallList, primaryExpression.Id) is { } parseCall)
        {
            rendered = RenderParseCall(parseCall);
        }
        else if (FindByBaseId(model.TryParseCallList, primaryExpression.Id) is { } tryParseCall)
        {
            rendered = RenderTryParseCall(tryParseCall);
        }
        else if (FindByBaseId(model.AtTimeZoneCallList, primaryExpression.Id) is { } atTimeZoneCall)
        {
            rendered = RenderAtTimeZoneCall(atTimeZoneCall);
        }
        else if (FindByBaseId(model.NextValueForExpressionList, primaryExpression.Id) is { } nextValueForExpression)
        {
            rendered = RenderNextValueForExpression(nextValueForExpression);
        }
        else if (FindByBaseId(model.ParameterlessCallList, primaryExpression.Id) is { } parameterlessCall)
        {
            rendered = RenderParameterlessCall(parameterlessCall);
        }
        else if (FindByBaseId(model.FunctionCallList, primaryExpression.Id) is { } functionCall)
        {
            rendered = RenderFunctionCall(functionCall);
        }
        else if (FindByBaseId(model.ScalarSubqueryList, primaryExpression.Id) is { } scalarSubquery)
        {
            rendered = RenderScalarSubquery(scalarSubquery);
        }
        else if (FindByBaseId(model.ValueExpressionList, primaryExpression.Id) is { } valueExpression)
        {
            rendered = RenderValueExpression(valueExpression);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported MetaTransformScript PrimaryExpression id '{primaryExpression.Id}'.");
        }

        var collationLink = FindOwnerLink(model.PrimaryExpressionCollationLinkList, primaryExpression.Id);
        return collationLink is null
            ? rendered
            : rendered + " COLLATE " + RenderIdentifier(collationLink.Identifier);
    }

    private string RenderScalarSubquery(ScalarSubquery scalarSubquery)
    {
        var queryExpression = GetOwnerLink(
            model.ScalarSubqueryQueryExpressionLinkList,
            scalarSubquery.Id,
            "ScalarSubquery.QueryExpression").QueryExpression;
        return "(" + RenderQueryExpression(queryExpression) + ")";
    }

    private string RenderColumnReferenceExpression(ColumnReferenceExpression columnReference)
    {
        var multiPartIdentifierLink = FindOwnerLink(model.ColumnReferenceExpressionMultiPartIdentifierLinkList, columnReference.Id);
        if (multiPartIdentifierLink is not null)
        {
            return RenderMultiPartIdentifier(multiPartIdentifierLink.MultiPartIdentifier);
        }

        if (string.Equals(columnReference.ColumnType, "Wildcard", StringComparison.Ordinal))
        {
            return "*";
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript ColumnReferenceExpression id '{columnReference.Id}'.");
    }

    private string RenderValueExpression(ValueExpression valueExpression)
    {
        var literal = FindByBaseId(model.LiteralList, valueExpression.Id);
        if (literal is not null)
        {
            return RenderLiteral(literal);
        }

        var globalVariableExpression = FindByBaseId(model.GlobalVariableExpressionList, valueExpression.Id);
        if (globalVariableExpression is not null)
        {
            return globalVariableExpression.Name;
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript ValueExpression id '{valueExpression.Id}'.");
    }

    private string RenderSimpleCaseExpression(SimpleCaseExpression simpleCaseExpression)
    {
        var caseExpression = GetById(model.CaseExpressionList, simpleCaseExpression.CaseExpressionId, "SimpleCaseExpression.Base");
        var inputExpression = RenderScalarExpression(GetOwnerLink(
            model.SimpleCaseExpressionInputExpressionLinkList,
            simpleCaseExpression.Id,
            "SimpleCaseExpression.InputExpression").ScalarExpression);
        var whenClauses = GetOrderedItems(model.SimpleCaseExpressionWhenClausesItemList, simpleCaseExpression.Id)
            .Select(row => RenderSimpleWhenClause(row.SimpleWhenClause))
            .ToArray();
        return RenderCaseExpression("CASE " + inputExpression, whenClauses, caseExpression);
    }

    private string RenderSearchedCaseExpression(SearchedCaseExpression searchedCaseExpression)
    {
        var caseExpression = GetById(model.CaseExpressionList, searchedCaseExpression.CaseExpressionId, "SearchedCaseExpression.Base");
        var whenClauses = GetOrderedItems(model.SearchedCaseExpressionWhenClausesItemList, searchedCaseExpression.Id)
            .Select(row => RenderSearchedWhenClause(row.SearchedWhenClause))
            .ToArray();
        return RenderCaseExpression("CASE", whenClauses, caseExpression);
    }

    private string RenderCaseExpression(string header, IReadOnlyList<string> whenClauses, CaseExpression caseExpression)
    {
        if (whenClauses.Count == 0)
        {
            throw new InvalidOperationException($"CaseExpression '{caseExpression.Id}' had no WHEN clauses.");
        }

        var builder = new StringBuilder();
        builder.AppendLine(header);

        foreach (var whenClause in whenClauses)
        {
            builder.Append("    ");
            builder.AppendLine(whenClause);
        }

        var elseExpressionLink = FindOwnerLink(model.CaseExpressionElseExpressionLinkList, caseExpression.Id);
        if (elseExpressionLink is not null)
        {
            builder.Append("    ELSE ");
            builder.AppendLine(RenderScalarExpression(elseExpressionLink.ScalarExpression));
        }

        builder.Append("END");
        return builder.ToString();
    }

    private string RenderSimpleWhenClause(SimpleWhenClause simpleWhenClause)
    {
        var whenClause = GetById(model.WhenClauseList, simpleWhenClause.WhenClauseId, "SimpleWhenClause.Base");
        var whenExpression = RenderScalarExpression(GetOwnerLink(
            model.SimpleWhenClauseWhenExpressionLinkList,
            simpleWhenClause.Id,
            "SimpleWhenClause.WhenExpression").ScalarExpression);
        var thenExpression = RenderScalarExpression(GetOwnerLink(
            model.WhenClauseThenExpressionLinkList,
            whenClause.Id,
            "WhenClause.ThenExpression").ScalarExpression);
        return $"WHEN {whenExpression} THEN {thenExpression}";
    }

    private string RenderSearchedWhenClause(SearchedWhenClause searchedWhenClause)
    {
        var whenClause = GetById(model.WhenClauseList, searchedWhenClause.WhenClauseId, "SearchedWhenClause.Base");
        var whenExpression = RenderBooleanExpression(GetOwnerLink(
            model.SearchedWhenClauseWhenExpressionLinkList,
            searchedWhenClause.Id,
            "SearchedWhenClause.WhenExpression").BooleanExpression);
        var thenExpression = RenderScalarExpression(GetOwnerLink(
            model.WhenClauseThenExpressionLinkList,
            whenClause.Id,
            "WhenClause.ThenExpression").ScalarExpression);
        return $"WHEN {whenExpression} THEN {thenExpression}";
    }
}
