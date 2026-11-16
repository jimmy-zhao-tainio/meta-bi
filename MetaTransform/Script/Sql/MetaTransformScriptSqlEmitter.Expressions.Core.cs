using System.Text;
using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    internal string RenderScalarExpressionForScriptObject(ScalarExpression scalarExpression) =>
        RenderScalarExpression(scalarExpression);

    private string RenderScalarExpression(ScalarExpression scalarExpression)
    {
        var binaryExpression = FindByBaseId(model.BinaryExpressionList, static row => row.ScalarExpression?.Id, nameof(BinaryExpression.ScalarExpression), scalarExpression.Id);
        if (binaryExpression is not null)
        {
            var left = RenderScalarExpression(GetOwnerLink(model.BinaryExpressionFirstExpressionLinkList, static row => row.BinaryExpression?.Id, nameof(BinaryExpressionFirstExpressionLink.BinaryExpression), binaryExpression.Id, "BinaryExpression.FirstExpression").ScalarExpression);
            var right = RenderScalarExpression(GetOwnerLink(model.BinaryExpressionSecondExpressionLinkList, static row => row.BinaryExpression?.Id, nameof(BinaryExpressionSecondExpressionLink.BinaryExpression), binaryExpression.Id, "BinaryExpression.SecondExpression").ScalarExpression);
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

        var unaryExpression = FindByBaseId(model.UnaryExpressionList, static row => row.ScalarExpression?.Id, nameof(UnaryExpression.ScalarExpression), scalarExpression.Id);
        if (unaryExpression is not null)
        {
            var expression = RenderScalarExpression(GetOwnerLink(
                model.UnaryExpressionExpressionLinkList, static row => row.UnaryExpression?.Id, nameof(UnaryExpressionExpressionLink.UnaryExpression),
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

        var primaryExpression = FindByBaseId(model.PrimaryExpressionList, static row => row.ScalarExpression?.Id, nameof(PrimaryExpression.ScalarExpression), scalarExpression.Id)
            ?? throw new InvalidOperationException($"Unsupported MetaTransformScript ScalarExpression id '{scalarExpression.Id}'.");
        return RenderPrimaryExpression(primaryExpression);
    }

    private string RenderPrimaryExpression(PrimaryExpression primaryExpression)
    {
        string rendered;

        if (FindByBaseId(model.ColumnReferenceExpressionList, static row => row.PrimaryExpression?.Id, nameof(ColumnReferenceExpression.PrimaryExpression), primaryExpression.Id) is { } columnReference)
        {
            rendered = RenderColumnReferenceExpression(columnReference);
        }
        else if (FindByBaseId(model.ParenthesisExpressionList, static row => row.PrimaryExpression?.Id, nameof(ParenthesisExpression.PrimaryExpression), primaryExpression.Id) is { } parenthesisExpression)
        {
            rendered = "(" + RenderScalarExpression(GetOwnerLink(
                model.ParenthesisExpressionExpressionLinkList, static row => row.ParenthesisExpression?.Id, nameof(ParenthesisExpressionExpressionLink.ParenthesisExpression),
                parenthesisExpression.Id,
                "ParenthesisExpression.Expression").ScalarExpression) + ")";
        }
        else if (FindByBaseId(model.CaseExpressionList, static row => row.PrimaryExpression?.Id, nameof(CaseExpression.PrimaryExpression), primaryExpression.Id) is { } caseExpression)
        {
            if (FindByBaseId(model.SimpleCaseExpressionList, static row => row.CaseExpression?.Id, nameof(SimpleCaseExpression.CaseExpression), caseExpression.Id) is { } simpleCaseExpression)
            {
                rendered = RenderSimpleCaseExpression(simpleCaseExpression);
            }
            else if (FindByBaseId(model.SearchedCaseExpressionList, static row => row.CaseExpression?.Id, nameof(SearchedCaseExpression.CaseExpression), caseExpression.Id) is { } searchedCaseExpression)
            {
                rendered = RenderSearchedCaseExpression(searchedCaseExpression);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported MetaTransformScript CaseExpression id '{caseExpression.Id}'.");
            }
        }
        else if (FindByBaseId(model.CoalesceExpressionList, static row => row.PrimaryExpression?.Id, nameof(CoalesceExpression.PrimaryExpression), primaryExpression.Id) is { } coalesceExpression)
        {
            rendered = RenderCoalesceExpression(coalesceExpression);
        }
        else if (FindByBaseId(model.NullIfExpressionList, static row => row.PrimaryExpression?.Id, nameof(NullIfExpression.PrimaryExpression), primaryExpression.Id) is { } nullIfExpression)
        {
            rendered = RenderNullIfExpression(nullIfExpression);
        }
        else if (FindByBaseId(model.IIfCallList, static row => row.PrimaryExpression?.Id, nameof(IIfCall.PrimaryExpression), primaryExpression.Id) is { } iIfCall)
        {
            rendered = RenderIIfCall(iIfCall);
        }
        else if (FindByBaseId(model.CastCallList, static row => row.PrimaryExpression?.Id, nameof(CastCall.PrimaryExpression), primaryExpression.Id) is { } castCall)
        {
            rendered = RenderCastCall(castCall);
        }
        else if (FindByBaseId(model.TryCastCallList, static row => row.PrimaryExpression?.Id, nameof(TryCastCall.PrimaryExpression), primaryExpression.Id) is { } tryCastCall)
        {
            rendered = RenderTryCastCall(tryCastCall);
        }
        else if (FindByBaseId(model.ConvertCallList, static row => row.PrimaryExpression?.Id, nameof(ConvertCall.PrimaryExpression), primaryExpression.Id) is { } convertCall)
        {
            rendered = RenderConvertCall(convertCall);
        }
        else if (FindByBaseId(model.TryConvertCallList, static row => row.PrimaryExpression?.Id, nameof(TryConvertCall.PrimaryExpression), primaryExpression.Id) is { } tryConvertCall)
        {
            rendered = RenderTryConvertCall(tryConvertCall);
        }
        else if (FindByBaseId(model.LeftFunctionCallList, static row => row.PrimaryExpression?.Id, nameof(LeftFunctionCall.PrimaryExpression), primaryExpression.Id) is { } leftFunctionCall)
        {
            rendered = RenderLeftFunctionCall(leftFunctionCall);
        }
        else if (FindByBaseId(model.RightFunctionCallList, static row => row.PrimaryExpression?.Id, nameof(RightFunctionCall.PrimaryExpression), primaryExpression.Id) is { } rightFunctionCall)
        {
            rendered = RenderRightFunctionCall(rightFunctionCall);
        }
        else if (FindByBaseId(model.ParseCallList, static row => row.PrimaryExpression?.Id, nameof(ParseCall.PrimaryExpression), primaryExpression.Id) is { } parseCall)
        {
            rendered = RenderParseCall(parseCall);
        }
        else if (FindByBaseId(model.TryParseCallList, static row => row.PrimaryExpression?.Id, nameof(TryParseCall.PrimaryExpression), primaryExpression.Id) is { } tryParseCall)
        {
            rendered = RenderTryParseCall(tryParseCall);
        }
        else if (FindByBaseId(model.AtTimeZoneCallList, static row => row.PrimaryExpression?.Id, nameof(AtTimeZoneCall.PrimaryExpression), primaryExpression.Id) is { } atTimeZoneCall)
        {
            rendered = RenderAtTimeZoneCall(atTimeZoneCall);
        }
        else if (FindByBaseId(model.NextValueForExpressionList, static row => row.PrimaryExpression?.Id, nameof(NextValueForExpression.PrimaryExpression), primaryExpression.Id) is { } nextValueForExpression)
        {
            rendered = RenderNextValueForExpression(nextValueForExpression);
        }
        else if (FindByBaseId(model.ParameterlessCallList, static row => row.PrimaryExpression?.Id, nameof(ParameterlessCall.PrimaryExpression), primaryExpression.Id) is { } parameterlessCall)
        {
            rendered = RenderParameterlessCall(parameterlessCall);
        }
        else if (FindByBaseId(model.FunctionCallList, static row => row.PrimaryExpression?.Id, nameof(FunctionCall.PrimaryExpression), primaryExpression.Id) is { } functionCall)
        {
            rendered = RenderFunctionCall(functionCall);
        }
        else if (FindByBaseId(model.ScalarSubqueryList, static row => row.PrimaryExpression?.Id, nameof(ScalarSubquery.PrimaryExpression), primaryExpression.Id) is { } scalarSubquery)
        {
            rendered = RenderScalarSubquery(scalarSubquery);
        }
        else if (FindByBaseId(model.ValueExpressionList, static row => row.PrimaryExpression?.Id, nameof(ValueExpression.PrimaryExpression), primaryExpression.Id) is { } valueExpression)
        {
            rendered = RenderValueExpression(valueExpression);
        }
        else
        {
            throw new InvalidOperationException($"Unsupported MetaTransformScript PrimaryExpression id '{primaryExpression.Id}'.");
        }

        var collationLink = FindOwnerLink(model.PrimaryExpressionCollationLinkList, static row => row.PrimaryExpression?.Id, nameof(PrimaryExpressionCollationLink.PrimaryExpression), primaryExpression.Id);
        return collationLink is null
            ? rendered
            : rendered + " COLLATE " + RenderIdentifier(collationLink.Identifier);
    }

    private string RenderScalarSubquery(ScalarSubquery scalarSubquery)
    {
        var queryExpression = GetOwnerLink(
            model.ScalarSubqueryQueryExpressionLinkList, static row => row.ScalarSubquery?.Id, nameof(ScalarSubqueryQueryExpressionLink.ScalarSubquery),
            scalarSubquery.Id,
            "ScalarSubquery.QueryExpression").QueryExpression;
        return "(" + RenderQueryExpression(queryExpression) + ")";
    }

    private string RenderColumnReferenceExpression(ColumnReferenceExpression columnReference)
    {
        var multiPartIdentifierLink = FindOwnerLink(model.ColumnReferenceExpressionMultiPartIdentifierLinkList, static row => row.ColumnReferenceExpression?.Id, nameof(ColumnReferenceExpressionMultiPartIdentifierLink.ColumnReferenceExpression), columnReference.Id);
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
        var literal = FindByBaseId(model.LiteralList, static row => row.ValueExpression?.Id, nameof(Literal.ValueExpression), valueExpression.Id);
        if (literal is not null)
        {
            return RenderLiteral(literal);
        }

        var globalVariableExpression = FindByBaseId(model.GlobalVariableExpressionList, static row => row.ValueExpression?.Id, nameof(GlobalVariableExpression.ValueExpression), valueExpression.Id);
        if (globalVariableExpression is not null)
        {
            return globalVariableExpression.Name
                ?? throw new InvalidOperationException($"GlobalVariableExpression '{globalVariableExpression.Id}' is missing its required name.");
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript ValueExpression id '{valueExpression.Id}'.");
    }

    private string RenderSimpleCaseExpression(SimpleCaseExpression simpleCaseExpression)
    {
        var caseExpression = GetById(model.CaseExpressionList, static row => row.Id, simpleCaseExpression.CaseExpression.Id, "SimpleCaseExpression.Base");
        var inputExpression = RenderScalarExpression(GetOwnerLink(
            model.SimpleCaseExpressionInputExpressionLinkList, static row => row.SimpleCaseExpression?.Id, nameof(SimpleCaseExpressionInputExpressionLink.SimpleCaseExpression),
            simpleCaseExpression.Id,
            "SimpleCaseExpression.InputExpression").ScalarExpression);
        var whenClauses = GetOrderedItems(model.SimpleCaseExpressionWhenClausesItemList, static row => row.SimpleCaseExpression?.Id, nameof(SimpleCaseExpressionWhenClausesItem.SimpleCaseExpression), static row => row.Ordinal, simpleCaseExpression.Id)
            .Select(row => RenderSimpleWhenClause(row.SimpleWhenClause))
            .ToArray();
        return RenderCaseExpression("CASE " + inputExpression, whenClauses, caseExpression);
    }

    private string RenderSearchedCaseExpression(SearchedCaseExpression searchedCaseExpression)
    {
        var caseExpression = GetById(model.CaseExpressionList, static row => row.Id, searchedCaseExpression.CaseExpression.Id, "SearchedCaseExpression.Base");
        var whenClauses = GetOrderedItems(model.SearchedCaseExpressionWhenClausesItemList, static row => row.SearchedCaseExpression?.Id, nameof(SearchedCaseExpressionWhenClausesItem.SearchedCaseExpression), static row => row.Ordinal, searchedCaseExpression.Id)
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

        var elseExpressionLink = FindOwnerLink(model.CaseExpressionElseExpressionLinkList, static row => row.CaseExpression?.Id, nameof(CaseExpressionElseExpressionLink.CaseExpression), caseExpression.Id);
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
        var whenClause = GetById(model.WhenClauseList, static row => row.Id, simpleWhenClause.WhenClause.Id, "SimpleWhenClause.Base");
        var whenExpression = RenderScalarExpression(GetOwnerLink(
            model.SimpleWhenClauseWhenExpressionLinkList, static row => row.SimpleWhenClause?.Id, nameof(SimpleWhenClauseWhenExpressionLink.SimpleWhenClause),
            simpleWhenClause.Id,
            "SimpleWhenClause.WhenExpression").ScalarExpression);
        var thenExpression = RenderScalarExpression(GetOwnerLink(
            model.WhenClauseThenExpressionLinkList, static row => row.WhenClause?.Id, nameof(WhenClauseThenExpressionLink.WhenClause),
            whenClause.Id,
            "WhenClause.ThenExpression").ScalarExpression);
        return $"WHEN {whenExpression} THEN {thenExpression}";
    }

    private string RenderSearchedWhenClause(SearchedWhenClause searchedWhenClause)
    {
        var whenClause = GetById(model.WhenClauseList, static row => row.Id, searchedWhenClause.WhenClause.Id, "SearchedWhenClause.Base");
        var whenExpression = RenderBooleanExpression(GetOwnerLink(
            model.SearchedWhenClauseWhenExpressionLinkList, static row => row.SearchedWhenClause?.Id, nameof(SearchedWhenClauseWhenExpressionLink.SearchedWhenClause),
            searchedWhenClause.Id,
            "SearchedWhenClause.WhenExpression").BooleanExpression);
        var thenExpression = RenderScalarExpression(GetOwnerLink(
            model.WhenClauseThenExpressionLinkList, static row => row.WhenClause?.Id, nameof(WhenClauseThenExpressionLink.WhenClause),
            whenClause.Id,
            "WhenClause.ThenExpression").ScalarExpression);
        return $"WHEN {whenExpression} THEN {thenExpression}";
    }
}
