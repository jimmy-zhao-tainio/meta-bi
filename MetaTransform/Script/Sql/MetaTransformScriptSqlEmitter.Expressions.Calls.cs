using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderCoalesceExpression(CoalesceExpression coalesceExpression)
    {
        var expressions = GetOrderedItems(model.CoalesceExpressionExpressionsItemList, coalesceExpression.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        return "COALESCE(" + string.Join(", ", expressions) + ")";
    }

    private string RenderNullIfExpression(NullIfExpression nullIfExpression)
    {
        var first = RenderScalarExpression(GetOwnerLink(
            model.NullIfExpressionFirstExpressionLinkList,
            nullIfExpression.Id,
            "NullIfExpression.FirstExpression").Value);
        var second = RenderScalarExpression(GetOwnerLink(
            model.NullIfExpressionSecondExpressionLinkList,
            nullIfExpression.Id,
            "NullIfExpression.SecondExpression").Value);
        return $"NULLIF({first}, {second})";
    }

    private string RenderIIfCall(IIfCall iIfCall)
    {
        var predicate = RenderBooleanExpression(GetOwnerLink(
            model.IIfCallPredicateLinkList,
            iIfCall.Id,
            "IIfCall.Predicate").Value);
        var thenExpression = RenderScalarExpression(GetOwnerLink(
            model.IIfCallThenExpressionLinkList,
            iIfCall.Id,
            "IIfCall.ThenExpression").Value);
        var elseExpression = RenderScalarExpression(GetOwnerLink(
            model.IIfCallElseExpressionLinkList,
            iIfCall.Id,
            "IIfCall.ElseExpression").Value);
        return $"IIF({predicate}, {thenExpression}, {elseExpression})";
    }

    private string RenderCastCall(CastCall castCall)
    {
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.CastCallParameterLinkList,
            castCall.Id,
            "CastCall.Parameter").Value);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.CastCallDataTypeLinkList,
            castCall.Id,
            "CastCall.DataType").Value);
        return $"CAST({parameter} AS {dataType})";
    }

    private string RenderTryCastCall(TryCastCall tryCastCall)
    {
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.TryCastCallParameterLinkList,
            tryCastCall.Id,
            "TryCastCall.Parameter").Value);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.TryCastCallDataTypeLinkList,
            tryCastCall.Id,
            "TryCastCall.DataType").Value);
        return $"TRY_CAST({parameter} AS {dataType})";
    }

    private string RenderConvertCall(ConvertCall convertCall)
    {
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.ConvertCallDataTypeLinkList,
            convertCall.Id,
            "ConvertCall.DataType").Value);
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.ConvertCallParameterLinkList,
            convertCall.Id,
            "ConvertCall.Parameter").Value);
        var styleLink = FindOwnerLink(model.ConvertCallStyleLinkList, convertCall.Id);
        return styleLink is null
            ? $"CONVERT({dataType}, {parameter})"
            : $"CONVERT({dataType}, {parameter}, {RenderScalarExpression(styleLink.Value)})";
    }

    private string RenderTryConvertCall(TryConvertCall tryConvertCall)
    {
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.TryConvertCallDataTypeLinkList,
            tryConvertCall.Id,
            "TryConvertCall.DataType").Value);
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.TryConvertCallParameterLinkList,
            tryConvertCall.Id,
            "TryConvertCall.Parameter").Value);
        var styleLink = FindOwnerLink(model.TryConvertCallStyleLinkList, tryConvertCall.Id);
        return styleLink is null
            ? $"TRY_CONVERT({dataType}, {parameter})"
            : $"TRY_CONVERT({dataType}, {parameter}, {RenderScalarExpression(styleLink.Value)})";
    }

    private string RenderParseCall(ParseCall parseCall)
    {
        var stringValue = RenderScalarExpression(GetOwnerLink(
            model.ParseCallStringValueLinkList,
            parseCall.Id,
            "ParseCall.StringValue").Value);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.ParseCallDataTypeLinkList,
            parseCall.Id,
            "ParseCall.DataType").Value);
        var cultureLink = FindOwnerLink(model.ParseCallCultureLinkList, parseCall.Id);
        return cultureLink is null
            ? $"PARSE({stringValue} AS {dataType})"
            : $"PARSE({stringValue} AS {dataType} USING {RenderScalarExpression(cultureLink.Value)})";
    }

    private string RenderTryParseCall(TryParseCall tryParseCall)
    {
        var stringValue = RenderScalarExpression(GetOwnerLink(
            model.TryParseCallStringValueLinkList,
            tryParseCall.Id,
            "TryParseCall.StringValue").Value);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.TryParseCallDataTypeLinkList,
            tryParseCall.Id,
            "TryParseCall.DataType").Value);
        var cultureLink = FindOwnerLink(model.TryParseCallCultureLinkList, tryParseCall.Id);
        return cultureLink is null
            ? $"TRY_PARSE({stringValue} AS {dataType})"
            : $"TRY_PARSE({stringValue} AS {dataType} USING {RenderScalarExpression(cultureLink.Value)})";
    }

    private string RenderLeftFunctionCall(LeftFunctionCall leftFunctionCall)
    {
        var parameters = GetOrderedItems(model.LeftFunctionCallParametersItemList, leftFunctionCall.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        return "LEFT(" + string.Join(", ", parameters) + ")";
    }

    private string RenderRightFunctionCall(RightFunctionCall rightFunctionCall)
    {
        var parameters = GetOrderedItems(model.RightFunctionCallParametersItemList, rightFunctionCall.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        return "RIGHT(" + string.Join(", ", parameters) + ")";
    }

    private string RenderAtTimeZoneCall(AtTimeZoneCall atTimeZoneCall)
    {
        var dateValue = RenderScalarExpression(GetOwnerLink(
            model.AtTimeZoneCallDateValueLinkList,
            atTimeZoneCall.Id,
            "AtTimeZoneCall.DateValue").Value);
        var timeZone = RenderScalarExpression(GetOwnerLink(
            model.AtTimeZoneCallTimeZoneLinkList,
            atTimeZoneCall.Id,
            "AtTimeZoneCall.TimeZone").Value);
        return $"{dateValue} AT TIME ZONE {timeZone}";
    }

    private string RenderNextValueForExpression(NextValueForExpression nextValueForExpression)
    {
        var sequenceName = RenderSchemaObjectName(GetOwnerLink(
            model.NextValueForExpressionSequenceNameLinkList,
            nextValueForExpression.Id,
            "NextValueForExpression.SequenceName").Value);
        return $"NEXT VALUE FOR {sequenceName}";
    }

    private static string RenderParameterlessCall(ParameterlessCall parameterlessCall)
    {
        return parameterlessCall.ParameterlessCallType switch
        {
            "CurrentTimestamp" => "CURRENT_TIMESTAMP",
            _ => throw new InvalidOperationException(
                $"Unsupported MetaTransformScript ParameterlessCallType '{parameterlessCall.ParameterlessCallType}'.")
        };
    }

    private string RenderDataTypeReference(DataTypeReference dataTypeReference)
    {
        var renderedName = FindOwnerLink(model.DataTypeReferenceNameLinkList, dataTypeReference.Id) is { } nameLink
            ? RenderSchemaObjectName(nameLink.Value)
            : FindByBaseId(model.ParameterizedDataTypeReferenceList, dataTypeReference.Id) is { } parameterizedForName
                && FindByBaseId(model.SqlDataTypeReferenceList, parameterizedForName.Id) is { } sqlDataTypeReference
                    ? RenderSqlDataTypeOption(sqlDataTypeReference.SqlDataTypeOption)
                    : throw new InvalidOperationException($"Unsupported MetaTransformScript DataTypeReference id '{dataTypeReference.Id}'.");

        var parameterizedDataTypeReference = FindByBaseId(model.ParameterizedDataTypeReferenceList, dataTypeReference.Id);
        if (parameterizedDataTypeReference is null)
        {
            return renderedName;
        }

        var parameters = GetOrderedItems(model.ParameterizedDataTypeReferenceParametersItemList, parameterizedDataTypeReference.Id)
            .Select(row => RenderLiteral(row.Value))
            .ToArray();
        return parameters.Length == 0
            ? renderedName
            : renderedName + "(" + string.Join(", ", parameters) + ")";
    }

    private static string RenderSqlDataTypeOption(string sqlDataTypeOption)
    {
        return MetaTransformScriptSqlServerDataTypes.RenderSqlName(sqlDataTypeOption);
    }

    private string RenderLiteral(Literal literal)
    {
        var stringLiteral = FindByBaseId(model.StringLiteralList, literal.Id);
        if (stringLiteral is not null)
        {
            var prefix = IsTrue(stringLiteral.IsNational) ? "N" : string.Empty;
            return prefix + "'" + (literal.Value ?? string.Empty).Replace("'", "''", StringComparison.Ordinal) + "'";
        }

        if (FindByBaseId(model.IntegerLiteralList, literal.Id) is not null)
        {
            return literal.Value;
        }

        if (FindByBaseId(model.NumericLiteralList, literal.Id) is not null)
        {
            return literal.Value;
        }

        if (FindByBaseId(model.RealLiteralList, literal.Id) is not null)
        {
            return literal.Value;
        }

        if (FindByBaseId(model.BinaryLiteralList, literal.Id) is not null)
        {
            return literal.Value;
        }

        if (FindByBaseId(model.NullLiteralList, literal.Id) is not null)
        {
            return "NULL";
        }

        if (FindByBaseId(model.MaxLiteralList, literal.Id) is not null)
        {
            return "max";
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript Literal type '{literal.LiteralType}'.");
    }

    private string RenderFunctionCall(FunctionCall functionCall)
    {
        if (IsTrue(functionCall.WithArrayWrapper))
        {
            throw new InvalidOperationException("Phase-1 emitter does not support FunctionCall.WithArrayWrapper=true.");
        }

        var functionName = RenderIdentifier(GetOwnerLink(model.FunctionCallFunctionNameLinkList, functionCall.Id, "FunctionCall.FunctionName").Value);
        var args = GetOrderedItems(model.FunctionCallParametersItemList, functionCall.Id)
            .Select(row => RenderScalarExpression(row.Value))
            .ToArray();
        var uniqueRowFilter =
            string.IsNullOrWhiteSpace(functionCall.UniqueRowFilter) ||
            string.Equals(functionCall.UniqueRowFilter, "NotSpecified", StringComparison.Ordinal)
                ? string.Empty
                : functionCall.UniqueRowFilter switch
                {
                    "Distinct" => "DISTINCT ",
                    _ => throw new InvalidOperationException(
                        $"Unsupported MetaTransformScript FunctionCall.UniqueRowFilter '{functionCall.UniqueRowFilter}'.")
                };

        var callTargetLink = FindOwnerLink(model.FunctionCallCallTargetLinkList, functionCall.Id);
        var overClauseLink = FindOwnerLink(model.FunctionCallOverClauseLinkList, functionCall.Id);
        var withinGroupOrderByClauseLink = FindOwnerLink(model.FunctionCallWithinGroupOrderByClauseLinkList, functionCall.Id);
        var withinGroupSuffix = withinGroupOrderByClauseLink is null
            ? string.Empty
            : $" WITHIN GROUP ({RenderOrderByClause(withinGroupOrderByClauseLink.Value)})";
        if (callTargetLink is not null)
        {
            var renderedCall = $"{RenderCallTarget(callTargetLink.Value)}.{functionName}({uniqueRowFilter}{string.Join(", ", args)}){withinGroupSuffix}";
            return overClauseLink is null ? renderedCall : renderedCall + " " + RenderOverClause(overClauseLink.Value);
        }

        var rendered = $"{functionName}({uniqueRowFilter}{string.Join(", ", args)}){withinGroupSuffix}";
        return overClauseLink is null ? rendered : rendered + " " + RenderOverClause(overClauseLink.Value);
    }

    private string RenderCallTarget(CallTarget callTarget)
    {
        var multiPartCallTarget = FindByBaseId(model.MultiPartIdentifierCallTargetList, callTarget.Id)
            ?? throw new InvalidOperationException($"Unsupported MetaTransformScript CallTarget id '{callTarget.Id}'.");
        return RenderMultiPartIdentifier(GetOwnerLink(
            model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList,
            multiPartCallTarget.Id,
            "MultiPartIdentifierCallTarget.MultiPartIdentifier").Value);
    }
}
