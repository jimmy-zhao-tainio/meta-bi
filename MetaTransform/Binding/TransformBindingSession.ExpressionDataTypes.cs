using System.Globalization;
using MetaTransformScript;
using MetaTransformScript.Sql;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    private RuntimeColumnDataType? TryResolveExpressionDataType(ScalarExpression expression)
    {
        var directColumnReference = navigator.TryGetDirectColumnReference(expression);
        if (directColumnReference is not null)
        {
            var boundColumnReference = boundColumnReferences
                .LastOrDefault(item => string.Equals(
                    item.SyntaxColumnReferenceId,
                    directColumnReference.Id,
                    StringComparison.Ordinal));
            return ApplyCurrentNullabilityRefinement(boundColumnReference);
        }

        var literal = navigator.TryGetLiteral(expression);
        if (literal is not null)
        {
            return CreateLiteralDataType(literal);
        }

        var parenthesisExpression = navigator.TryGetParenthesisExpression(expression);
        var parenthesizedExpression = parenthesisExpression is null
            ? null
            : navigator.TryGetParenthesisExpressionOperand(parenthesisExpression);
        if (parenthesizedExpression is not null)
        {
            return TryResolveExpressionDataType(parenthesizedExpression);
        }

        return TryCreateExplicitConversionDataType(expression) ??
               TryCreateCoalesceDataType(expression) ??
               TryCreateCaseDataType(expression);
    }

    private RuntimeColumnDataType? TryCreateCoalesceDataType(ScalarExpression expression)
    {
        var coalesceExpression = navigator.TryGetCoalesceExpression(expression);
        if (coalesceExpression is null)
        {
            return null;
        }

        var resultDataTypes = navigator.GetCoalesceExpressions(coalesceExpression)
            .Select(TryResolveExpressionDataType)
            .ToArray();
        if (resultDataTypes.Length == 0 || resultDataTypes.Any(item => item is null))
        {
            return null;
        }

        var firstDataType = resultDataTypes[0]!;
        if (resultDataTypes.Skip(1).Any(item => !HasSameDataTypeContract(firstDataType, item!)))
        {
            return null;
        }

        bool? isNullable = resultDataTypes.Any(item => item!.IsNullable == false)
            ? false
            : resultDataTypes.All(item => item!.IsNullable == true)
                ? true
                : null;

        return firstDataType with
        {
            IsNullable = isNullable,
            DisplayName = "COALESCE expression"
        };
    }

    private RuntimeColumnDataType? TryCreateCaseDataType(ScalarExpression expression)
    {
        IReadOnlyList<ScalarExpression?> thenExpressions;
        ScalarExpression? elseExpression;

        if (navigator.TryGetSearchedCaseExpression(expression) is { } searchedCaseExpression)
        {
            thenExpressions = navigator.GetSearchedWhenClauses(searchedCaseExpression)
                .Select(navigator.TryGetWhenClauseThenExpression)
                .ToArray();
            elseExpression = navigator.TryGetCaseElseExpression(searchedCaseExpression);
        }
        else if (navigator.TryGetSimpleCaseExpression(expression) is { } simpleCaseExpression)
        {
            thenExpressions = navigator.GetSimpleWhenClauses(simpleCaseExpression)
                .Select(navigator.TryGetWhenClauseThenExpression)
                .ToArray();
            elseExpression = navigator.TryGetCaseElseExpression(simpleCaseExpression);
        }
        else
        {
            return null;
        }

        if (thenExpressions.Count == 0 || thenExpressions.Any(item => item is null))
        {
            return null;
        }

        var resultExpressions = thenExpressions.Cast<ScalarExpression>().ToList();
        if (elseExpression is not null)
        {
            resultExpressions.Add(elseExpression);
        }

        var resultDataTypes = resultExpressions
            .Select(TryResolveExpressionDataType)
            .ToArray();
        if (resultDataTypes.Any(item => item is null))
        {
            return null;
        }

        var firstDataType = resultDataTypes[0]!;
        if (resultDataTypes.Skip(1).Any(item => !HasSameDataTypeContract(firstDataType, item!)))
        {
            return null;
        }

        bool? isNullable = elseExpression is null
            ? true
            : resultDataTypes.Any(item => item!.IsNullable == true)
                ? true
                : resultDataTypes.All(item => item!.IsNullable == false)
                    ? false
                    : null;

        return firstDataType with
        {
            IsNullable = isNullable,
            DisplayName = "CASE expression"
        };
    }

    private static bool HasSameDataTypeContract(
        RuntimeColumnDataType first,
        RuntimeColumnDataType second)
    {
        return string.Equals(first.MetaDataTypeId, second.MetaDataTypeId, StringComparison.OrdinalIgnoreCase) &&
               first.Length == second.Length &&
               first.Precision == second.Precision &&
               first.Scale == second.Scale;
    }

    private RuntimeColumnDataType? TryCreateExplicitConversionDataType(ScalarExpression expression)
    {
        if (!navigator.TryGetExplicitConversion(
                expression,
                out var dataTypeReference,
                out var inputExpression) ||
            navigator.TryGetSqlDataTypeReference(dataTypeReference) is not { } sqlDataTypeReference ||
            !MetaTransformScriptSqlServerDataTypes.TryGetMetaDataTypeId(
                sqlDataTypeReference.SqlDataTypeOption,
                out var metaDataTypeId))
        {
            return null;
        }

        var parameters = navigator.GetSqlDataTypeParameters(sqlDataTypeReference);
        var typeOption = sqlDataTypeReference.SqlDataTypeOption;
        var typeName = MetaTransformScriptSqlServerDataTypes.RenderSqlName(typeOption);
        var length = IsLengthParameterizedType(typeOption)
            ? TryGetDataTypeParameter(parameters, 0)
            : null;
        var precision = IsPrecisionParameterizedType(typeOption)
            ? TryGetDataTypeParameter(parameters, 0)
            : null;
        var scale = IsScaleParameterizedType(typeOption)
            ? TryGetDataTypeParameter(parameters, 1)
            : null;

        return new RuntimeColumnDataType(
            metaDataTypeId,
            TryResolveExpressionNullability(inputExpression),
            length,
            precision,
            scale,
            $"explicit {typeName} conversion");
    }

    private bool? TryResolveExpressionNullability(ScalarExpression expression)
    {
        var directColumnReference = navigator.TryGetDirectColumnReference(expression);
        if (directColumnReference is not null)
        {
            var boundColumnReference = boundColumnReferences
                .LastOrDefault(item => string.Equals(
                    item.SyntaxColumnReferenceId,
                    directColumnReference.Id,
                    StringComparison.Ordinal));
            return ApplyCurrentNullabilityRefinement(boundColumnReference)?.IsNullable;
        }

        var literal = navigator.TryGetLiteral(expression);
        if (literal is not null)
        {
            return CreateLiteralDataType(literal)?.IsNullable;
        }

        var parenthesisExpression = navigator.TryGetParenthesisExpression(expression);
        var parenthesizedExpression = parenthesisExpression is null
            ? null
            : navigator.TryGetParenthesisExpressionOperand(parenthesisExpression);
        if (parenthesizedExpression is not null)
        {
            return TryResolveExpressionNullability(parenthesizedExpression);
        }

        if (navigator.TryGetExplicitConversion(
                expression,
                out _,
                out var convertedExpression))
        {
            return TryResolveExpressionNullability(convertedExpression);
        }

        var binaryExpression = navigator.TryGetBinaryExpression(expression);
        var binaryOperands = binaryExpression is null
            ? null
            : navigator.TryGetBinaryExpressionOperands(binaryExpression);
        if (binaryOperands is not null)
        {
            return CombineNullPropagatingOperands(
                binaryOperands.Value.First,
                binaryOperands.Value.Second);
        }

        var unaryExpression = navigator.TryGetUnaryExpression(expression);
        var unaryOperand = unaryExpression is null
            ? null
            : navigator.TryGetUnaryExpressionOperand(unaryExpression);
        if (unaryOperand is not null)
        {
            return TryResolveExpressionNullability(unaryOperand);
        }

        var functionCall = navigator.TryGetFunctionCall(expression);
        if (functionCall is not null)
        {
            return TryResolveKnownFunctionNullability(functionCall);
        }

        var coalesceExpression = navigator.TryGetCoalesceExpression(expression);
        if (coalesceExpression is not null)
        {
            var nullabilities = navigator.GetCoalesceExpressions(coalesceExpression)
                .Select(TryResolveExpressionNullability)
                .ToArray();
            if (nullabilities.Any(item => item == false))
            {
                return false;
            }

            return nullabilities.Length > 0 && nullabilities.All(item => item == true)
                ? true
                : null;
        }

        return TryCreateCaseDataType(expression)?.IsNullable;
    }

    private bool? TryResolveKnownFunctionNullability(FunctionCall functionCall)
    {
        var functionName = navigator.TryGetFunctionCallName(functionCall)?.Trim();
        var parameters = navigator.GetFunctionCallParameters(functionCall);

        if (string.Equals(functionName, "DATALENGTH", StringComparison.OrdinalIgnoreCase) &&
            parameters.Count == 1)
        {
            return TryResolveExpressionNullability(parameters[0]);
        }

        if (string.Equals(functionName, "HASHBYTES", StringComparison.OrdinalIgnoreCase) &&
            parameters.Count == 2)
        {
            return CombineNullPropagatingOperands(parameters[0], parameters[1]);
        }

        if (string.Equals(functionName, "SESSION_CONTEXT", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return null;
    }

    private bool? CombineNullPropagatingOperands(
        ScalarExpression? first,
        ScalarExpression? second)
    {
        if (first is null || second is null)
        {
            return null;
        }

        var firstNullability = TryResolveExpressionNullability(first);
        var secondNullability = TryResolveExpressionNullability(second);
        if (firstNullability == true || secondNullability == true)
        {
            return true;
        }

        return firstNullability == false && secondNullability == false
            ? false
            : null;
    }

    private RuntimeColumnDataType? ApplyCurrentNullabilityRefinement(RuntimeColumnReference? boundColumnReference)
    {
        var dataType = boundColumnReference?.ResolvedColumn.DataType;
        if (dataType is null || boundColumnReference is null)
        {
            return dataType;
        }

        return nonNullableColumnScopeStack.Any(scope => scope.Contains(boundColumnReference.ResolvedColumn.Id))
            ? dataType with { IsNullable = false }
            : dataType;
    }

    private static bool IsLengthParameterizedType(string? sqlDataTypeOption) =>
        sqlDataTypeOption is "Binary" or "Char" or "NChar" or "NVarChar" or "VarBinary" or "VarChar";

    private static bool IsPrecisionParameterizedType(string? sqlDataTypeOption) =>
        sqlDataTypeOption is "DateTime2" or "DateTimeOffset" or "Decimal" or "Numeric" or "Time";

    private static bool IsScaleParameterizedType(string? sqlDataTypeOption) =>
        sqlDataTypeOption is "Decimal" or "Numeric";

    private static int? TryGetDataTypeParameter(IReadOnlyList<Literal> parameters, int index)
    {
        if (index >= parameters.Count ||
            !string.Equals(parameters[index].LiteralType, "Integer", StringComparison.OrdinalIgnoreCase) ||
            !int.TryParse(parameters[index].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            return null;
        }

        return value;
    }

    private RuntimeColumnDataType? CreateLiteralDataType(Literal literal)
    {
        var value = literal.Value ?? string.Empty;
        var literalType = literal.LiteralType?.Trim() ?? string.Empty;
        return literalType.ToUpperInvariant() switch
        {
            "STRING" => new RuntimeColumnDataType(
                navigator.IsNationalStringLiteral(literal) ? "sqlserver:type:nvarchar" : "sqlserver:type:varchar",
                IsNullable: false,
                Length: value.Length,
                Precision: null,
                Scale: null,
                DisplayName: "string literal"),
            "INTEGER" => CreateIntegerLiteralDataType(value),
            "NUMERIC" => CreateNumericLiteralDataType(value),
            "REAL" => new RuntimeColumnDataType(
                "sqlserver:type:float",
                IsNullable: false,
                Length: null,
                Precision: null,
                Scale: null,
                DisplayName: "real literal"),
            "BINARY" => new RuntimeColumnDataType(
                "sqlserver:type:varbinary",
                IsNullable: false,
                Length: Math.Max(0, (value.Length - 2) / 2),
                Precision: null,
                Scale: null,
                DisplayName: "binary literal"),
            _ => null
        };
    }

    private static RuntimeColumnDataType? CreateIntegerLiteralDataType(string value)
    {
        if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
        {
            return null;
        }

        return new RuntimeColumnDataType(
            number is >= int.MinValue and <= int.MaxValue ? "sqlserver:type:int" : "sqlserver:type:bigint",
            IsNullable: false,
            Length: null,
            Precision: null,
            Scale: null,
            DisplayName: "integer literal");
    }

    private static RuntimeColumnDataType? CreateNumericLiteralDataType(string value)
    {
        var normalized = value.Trim();
        if (normalized.StartsWith('+') || normalized.StartsWith('-'))
        {
            normalized = normalized[1..];
        }

        var parts = normalized.Split('.', StringSplitOptions.None);
        if (parts.Length != 2 || parts.Any(static item => item.Length == 0 || item.Any(static character => !char.IsDigit(character))))
        {
            return null;
        }

        var precision = parts[0].Length + parts[1].Length;
        return new RuntimeColumnDataType(
            "sqlserver:type:decimal",
            IsNullable: false,
            Length: null,
            Precision: precision,
            Scale: parts[1].Length,
            DisplayName: "numeric literal");
    }
}
