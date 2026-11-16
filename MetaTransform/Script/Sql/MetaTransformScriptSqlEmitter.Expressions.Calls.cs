using MetaTransformScript;

namespace MetaTransformScript.Sql;

internal sealed partial class MetaTransformScriptSqlEmitter
{
    private string RenderCoalesceExpression(CoalesceExpression coalesceExpression)
    {
        var expressions = GetOrderedItems(model.CoalesceExpressionExpressionsItemList, static row => row.CoalesceExpression?.Id, nameof(CoalesceExpressionExpressionsItem.CoalesceExpression), static row => row.Ordinal, coalesceExpression.Id)
            .Select(row => RenderScalarExpression(row.ScalarExpression))
            .ToArray();
        return "COALESCE(" + string.Join(", ", expressions) + ")";
    }

    private string RenderNullIfExpression(NullIfExpression nullIfExpression)
    {
        var first = RenderScalarExpression(GetOwnerLink(
            model.NullIfExpressionFirstExpressionLinkList, static row => row.NullIfExpression?.Id, nameof(NullIfExpressionFirstExpressionLink.NullIfExpression),
            nullIfExpression.Id,
            "NullIfExpression.FirstExpression").ScalarExpression);
        var second = RenderScalarExpression(GetOwnerLink(
            model.NullIfExpressionSecondExpressionLinkList, static row => row.NullIfExpression?.Id, nameof(NullIfExpressionSecondExpressionLink.NullIfExpression),
            nullIfExpression.Id,
            "NullIfExpression.SecondExpression").ScalarExpression);
        return $"NULLIF({first}, {second})";
    }

    private string RenderIIfCall(IIfCall iIfCall)
    {
        var predicate = RenderBooleanExpression(GetOwnerLink(
            model.IIfCallPredicateLinkList, static row => row.IIfCall?.Id, nameof(IIfCallPredicateLink.IIfCall),
            iIfCall.Id,
            "IIfCall.Predicate").BooleanExpression);
        var thenExpression = RenderScalarExpression(GetOwnerLink(
            model.IIfCallThenExpressionLinkList, static row => row.IIfCall?.Id, nameof(IIfCallThenExpressionLink.IIfCall),
            iIfCall.Id,
            "IIfCall.ThenExpression").ScalarExpression);
        var elseExpression = RenderScalarExpression(GetOwnerLink(
            model.IIfCallElseExpressionLinkList, static row => row.IIfCall?.Id, nameof(IIfCallElseExpressionLink.IIfCall),
            iIfCall.Id,
            "IIfCall.ElseExpression").ScalarExpression);
        return $"IIF({predicate}, {thenExpression}, {elseExpression})";
    }

    private string RenderCastCall(CastCall castCall)
    {
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.CastCallParameterLinkList, static row => row.CastCall?.Id, nameof(CastCallParameterLink.CastCall),
            castCall.Id,
            "CastCall.Parameter").ScalarExpression);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.CastCallDataTypeLinkList, static row => row.CastCall?.Id, nameof(CastCallDataTypeLink.CastCall),
            castCall.Id,
            "CastCall.DataType").DataTypeReference);
        return $"CAST({parameter} AS {dataType})";
    }

    private string RenderTryCastCall(TryCastCall tryCastCall)
    {
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.TryCastCallParameterLinkList, static row => row.TryCastCall?.Id, nameof(TryCastCallParameterLink.TryCastCall),
            tryCastCall.Id,
            "TryCastCall.Parameter").ScalarExpression);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.TryCastCallDataTypeLinkList, static row => row.TryCastCall?.Id, nameof(TryCastCallDataTypeLink.TryCastCall),
            tryCastCall.Id,
            "TryCastCall.DataType").DataTypeReference);
        return $"TRY_CAST({parameter} AS {dataType})";
    }

    private string RenderConvertCall(ConvertCall convertCall)
    {
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.ConvertCallDataTypeLinkList, static row => row.ConvertCall?.Id, nameof(ConvertCallDataTypeLink.ConvertCall),
            convertCall.Id,
            "ConvertCall.DataType").DataTypeReference);
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.ConvertCallParameterLinkList, static row => row.ConvertCall?.Id, nameof(ConvertCallParameterLink.ConvertCall),
            convertCall.Id,
            "ConvertCall.Parameter").ScalarExpression);
        var styleLink = FindOwnerLink(model.ConvertCallStyleLinkList, static row => row.ConvertCall?.Id, nameof(ConvertCallStyleLink.ConvertCall), convertCall.Id);
        return styleLink is null
            ? $"CONVERT({dataType}, {parameter})"
            : $"CONVERT({dataType}, {parameter}, {RenderScalarExpression(styleLink.ScalarExpression)})";
    }

    private string RenderTryConvertCall(TryConvertCall tryConvertCall)
    {
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.TryConvertCallDataTypeLinkList, static row => row.TryConvertCall?.Id, nameof(TryConvertCallDataTypeLink.TryConvertCall),
            tryConvertCall.Id,
            "TryConvertCall.DataType").DataTypeReference);
        var parameter = RenderScalarExpression(GetOwnerLink(
            model.TryConvertCallParameterLinkList, static row => row.TryConvertCall?.Id, nameof(TryConvertCallParameterLink.TryConvertCall),
            tryConvertCall.Id,
            "TryConvertCall.Parameter").ScalarExpression);
        var styleLink = FindOwnerLink(model.TryConvertCallStyleLinkList, static row => row.TryConvertCall?.Id, nameof(TryConvertCallStyleLink.TryConvertCall), tryConvertCall.Id);
        return styleLink is null
            ? $"TRY_CONVERT({dataType}, {parameter})"
            : $"TRY_CONVERT({dataType}, {parameter}, {RenderScalarExpression(styleLink.ScalarExpression)})";
    }

    private string RenderParseCall(ParseCall parseCall)
    {
        var stringValue = RenderScalarExpression(GetOwnerLink(
            model.ParseCallStringValueLinkList, static row => row.ParseCall?.Id, nameof(ParseCallStringValueLink.ParseCall),
            parseCall.Id,
            "ParseCall.StringValue").ScalarExpression);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.ParseCallDataTypeLinkList, static row => row.ParseCall?.Id, nameof(ParseCallDataTypeLink.ParseCall),
            parseCall.Id,
            "ParseCall.DataType").DataTypeReference);
        var cultureLink = FindOwnerLink(model.ParseCallCultureLinkList, static row => row.ParseCall?.Id, nameof(ParseCallCultureLink.ParseCall), parseCall.Id);
        return cultureLink is null
            ? $"PARSE({stringValue} AS {dataType})"
            : $"PARSE({stringValue} AS {dataType} USING {RenderScalarExpression(cultureLink.ScalarExpression)})";
    }

    private string RenderTryParseCall(TryParseCall tryParseCall)
    {
        var stringValue = RenderScalarExpression(GetOwnerLink(
            model.TryParseCallStringValueLinkList, static row => row.TryParseCall?.Id, nameof(TryParseCallStringValueLink.TryParseCall),
            tryParseCall.Id,
            "TryParseCall.StringValue").ScalarExpression);
        var dataType = RenderDataTypeReference(GetOwnerLink(
            model.TryParseCallDataTypeLinkList, static row => row.TryParseCall?.Id, nameof(TryParseCallDataTypeLink.TryParseCall),
            tryParseCall.Id,
            "TryParseCall.DataType").DataTypeReference);
        var cultureLink = FindOwnerLink(model.TryParseCallCultureLinkList, static row => row.TryParseCall?.Id, nameof(TryParseCallCultureLink.TryParseCall), tryParseCall.Id);
        return cultureLink is null
            ? $"TRY_PARSE({stringValue} AS {dataType})"
            : $"TRY_PARSE({stringValue} AS {dataType} USING {RenderScalarExpression(cultureLink.ScalarExpression)})";
    }

    private string RenderLeftFunctionCall(LeftFunctionCall leftFunctionCall)
    {
        var parameters = GetOrderedItems(model.LeftFunctionCallParametersItemList, static row => row.LeftFunctionCall?.Id, nameof(LeftFunctionCallParametersItem.LeftFunctionCall), static row => row.Ordinal, leftFunctionCall.Id)
            .Select(row => RenderScalarExpression(row.ScalarExpression))
            .ToArray();
        return "LEFT(" + string.Join(", ", parameters) + ")";
    }

    private string RenderRightFunctionCall(RightFunctionCall rightFunctionCall)
    {
        var parameters = GetOrderedItems(model.RightFunctionCallParametersItemList, static row => row.RightFunctionCall?.Id, nameof(RightFunctionCallParametersItem.RightFunctionCall), static row => row.Ordinal, rightFunctionCall.Id)
            .Select(row => RenderScalarExpression(row.ScalarExpression))
            .ToArray();
        return "RIGHT(" + string.Join(", ", parameters) + ")";
    }

    private string RenderAtTimeZoneCall(AtTimeZoneCall atTimeZoneCall)
    {
        var dateValue = RenderScalarExpression(GetOwnerLink(
            model.AtTimeZoneCallDateValueLinkList, static row => row.AtTimeZoneCall?.Id, nameof(AtTimeZoneCallDateValueLink.AtTimeZoneCall),
            atTimeZoneCall.Id,
            "AtTimeZoneCall.DateValue").ScalarExpression);
        var timeZone = RenderScalarExpression(GetOwnerLink(
            model.AtTimeZoneCallTimeZoneLinkList, static row => row.AtTimeZoneCall?.Id, nameof(AtTimeZoneCallTimeZoneLink.AtTimeZoneCall),
            atTimeZoneCall.Id,
            "AtTimeZoneCall.TimeZone").ScalarExpression);
        return $"{dateValue} AT TIME ZONE {timeZone}";
    }

    private string RenderNextValueForExpression(NextValueForExpression nextValueForExpression)
    {
        var sequenceName = RenderSchemaObjectName(GetOwnerLink(
            model.NextValueForExpressionSequenceNameLinkList, static row => row.NextValueForExpression?.Id, nameof(NextValueForExpressionSequenceNameLink.NextValueForExpression),
            nextValueForExpression.Id,
            "NextValueForExpression.SequenceName").SchemaObjectName);
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
        var renderedName = FindOwnerLink(model.DataTypeReferenceNameLinkList, static row => row.DataTypeReference?.Id, nameof(DataTypeReferenceNameLink.DataTypeReference), dataTypeReference.Id) is { } nameLink
            ? RenderSchemaObjectName(nameLink.SchemaObjectName)
            : FindByBaseId(model.ParameterizedDataTypeReferenceList, static row => row.DataTypeReference?.Id, nameof(ParameterizedDataTypeReference.DataTypeReference), dataTypeReference.Id) is { } parameterizedForName
                && FindByBaseId(model.SqlDataTypeReferenceList, static row => row.ParameterizedDataTypeReference?.Id, nameof(SqlDataTypeReference.ParameterizedDataTypeReference), parameterizedForName.Id) is { } sqlDataTypeReference
                    ? RenderSqlDataTypeOption(sqlDataTypeReference.SqlDataTypeOption)
                    : throw new InvalidOperationException($"Unsupported MetaTransformScript DataTypeReference id '{dataTypeReference.Id}'.");

        var parameterizedDataTypeReference = FindByBaseId(model.ParameterizedDataTypeReferenceList, static row => row.DataTypeReference?.Id, nameof(ParameterizedDataTypeReference.DataTypeReference), dataTypeReference.Id);
        if (parameterizedDataTypeReference is null)
        {
            return renderedName;
        }

        var parameters = GetOrderedItems(model.ParameterizedDataTypeReferenceParametersItemList, static row => row.ParameterizedDataTypeReference?.Id, nameof(ParameterizedDataTypeReferenceParametersItem.ParameterizedDataTypeReference), static row => row.Ordinal, parameterizedDataTypeReference.Id)
            .Select(row => RenderLiteral(row.Literal))
            .ToArray();
        return parameters.Length == 0
            ? renderedName
            : renderedName + "(" + string.Join(", ", parameters) + ")";
    }

    private static string RenderSqlDataTypeOption(string? sqlDataTypeOption)
    {
        return MetaTransformScriptSqlServerDataTypes.RenderSqlName(sqlDataTypeOption);
    }

    private string RenderLiteral(Literal literal)
    {
        var stringLiteral = FindByBaseId(model.StringLiteralList, static row => row.Literal?.Id, nameof(StringLiteral.Literal), literal.Id);
        if (stringLiteral is not null)
        {
            var prefix = IsTrue(stringLiteral.IsNational) ? "N" : string.Empty;
            return prefix + "'" + (literal.Value ?? string.Empty).Replace("'", "''", StringComparison.Ordinal) + "'";
        }

        if (FindByBaseId(model.IntegerLiteralList, static row => row.Literal?.Id, nameof(IntegerLiteral.Literal), literal.Id) is not null)
        {
            return RequireLiteralValue(literal);
        }

        if (FindByBaseId(model.NumericLiteralList, static row => row.Literal?.Id, nameof(NumericLiteral.Literal), literal.Id) is not null)
        {
            return RequireLiteralValue(literal);
        }

        if (FindByBaseId(model.RealLiteralList, static row => row.Literal?.Id, nameof(RealLiteral.Literal), literal.Id) is not null)
        {
            return RequireLiteralValue(literal);
        }

        if (FindByBaseId(model.BinaryLiteralList, static row => row.Literal?.Id, nameof(BinaryLiteral.Literal), literal.Id) is not null)
        {
            return RequireLiteralValue(literal);
        }

        if (FindByBaseId(model.NullLiteralList, static row => row.Literal?.Id, nameof(NullLiteral.Literal), literal.Id) is not null)
        {
            return "NULL";
        }

        if (FindByBaseId(model.MaxLiteralList, static row => row.Literal?.Id, nameof(MaxLiteral.Literal), literal.Id) is not null)
        {
            return "max";
        }

        throw new InvalidOperationException($"Unsupported MetaTransformScript Literal type '{literal.LiteralType}'.");
    }

    private static string RequireLiteralValue(Literal literal)
    {
        return literal.Value ?? throw new InvalidOperationException($"Literal '{literal.Id}' is missing its required value.");
    }

    private string RenderFunctionCall(FunctionCall functionCall)
    {
        if (IsTrue(functionCall.WithArrayWrapper))
        {
            throw new InvalidOperationException("Phase-1 emitter does not support FunctionCall.WithArrayWrapper=true.");
        }

        var functionName = RenderIdentifier(GetOwnerLink(model.FunctionCallFunctionNameLinkList, static row => row.FunctionCall?.Id, nameof(FunctionCallFunctionNameLink.FunctionCall), functionCall.Id, "FunctionCall.FunctionName").Identifier);
        var args = GetOrderedItems(model.FunctionCallParametersItemList, static row => row.FunctionCall?.Id, nameof(FunctionCallParametersItem.FunctionCall), static row => row.Ordinal, functionCall.Id)
            .Select(row => RenderScalarExpression(row.ScalarExpression))
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

        var callTargetLink = FindOwnerLink(model.FunctionCallCallTargetLinkList, static row => row.FunctionCall?.Id, nameof(FunctionCallCallTargetLink.FunctionCall), functionCall.Id);
        var overClauseLink = FindOwnerLink(model.FunctionCallOverClauseLinkList, static row => row.FunctionCall?.Id, nameof(FunctionCallOverClauseLink.FunctionCall), functionCall.Id);
        var withinGroupOrderByClauseLink = FindOwnerLink(model.FunctionCallWithinGroupOrderByClauseLinkList, static row => row.FunctionCall?.Id, nameof(FunctionCallWithinGroupOrderByClauseLink.FunctionCall), functionCall.Id);
        var withinGroupSuffix = withinGroupOrderByClauseLink is null
            ? string.Empty
            : $" WITHIN GROUP ({RenderOrderByClause(withinGroupOrderByClauseLink.OrderByClause)})";
        if (callTargetLink is not null)
        {
            var renderedCall = $"{RenderCallTarget(callTargetLink.CallTarget)}.{functionName}({uniqueRowFilter}{string.Join(", ", args)}){withinGroupSuffix}";
            return overClauseLink is null ? renderedCall : renderedCall + " " + RenderOverClause(overClauseLink.OverClause);
        }

        var rendered = $"{functionName}({uniqueRowFilter}{string.Join(", ", args)}){withinGroupSuffix}";
        return overClauseLink is null ? rendered : rendered + " " + RenderOverClause(overClauseLink.OverClause);
    }

    private string RenderCallTarget(CallTarget callTarget)
    {
        var multiPartCallTarget = FindByBaseId(model.MultiPartIdentifierCallTargetList, static row => row.CallTarget?.Id, nameof(MultiPartIdentifierCallTarget.CallTarget), callTarget.Id)
            ?? throw new InvalidOperationException($"Unsupported MetaTransformScript CallTarget id '{callTarget.Id}'.");
        return RenderMultiPartIdentifier(GetOwnerLink(
            model.MultiPartIdentifierCallTargetMultiPartIdentifierLinkList, static row => row.MultiPartIdentifierCallTarget?.Id, nameof(MultiPartIdentifierCallTargetMultiPartIdentifierLink.MultiPartIdentifierCallTarget),
            multiPartCallTarget.Id,
            "MultiPartIdentifierCallTarget.MultiPartIdentifier").MultiPartIdentifier);
    }
}
