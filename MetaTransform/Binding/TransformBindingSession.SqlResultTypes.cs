using MetaTransformScript;

namespace MetaTransform.Binding;

internal sealed partial class TransformBindingSession
{
    // SQL Server expression semantics, independent of authored source-to-destination mappings.
    // https://learn.microsoft.com/sql/t-sql/data-types/precision-scale-and-length-transact-sql
    private RuntimeColumnDataType SqlResult(string name, bool? nullable, string display,
        int? length = null, int? precision = null, int? scale = null) =>
        new(typeValidator.ResolveSqlTypeName(name), nullable, length, precision, scale, display);

    private string SqlName(RuntimeColumnDataType type) =>
        typeValidator.Resolve(type.MetaDataTypeId, type.DisplayName).Name.ToLowerInvariant();

    private static bool IsDecimal(string name) => name is "decimal" or "numeric";
    private static bool IsNumeric(string name) => name is "tinyint" or "smallint" or "int" or "bigint" or
        "decimal" or "numeric" or "money" or "smallmoney" or "real" or "float";
    private static bool IsText(string name) => name is "char" or "varchar" or "nchar" or "nvarchar";

    private static string CommonTextName(string first, string second)
    {
        var unicode = first is "nchar" or "nvarchar" || second is "nchar" or "nvarchar";
        var variable = first is "varchar" or "nvarchar" || second is "varchar" or "nvarchar";
        return unicode ? variable ? "nvarchar" : "nchar" : variable ? "varchar" : "char";
    }

    private static bool? NullableUnion(params RuntimeColumnDataType[] types) =>
        types.Any(t => t.IsNullable == true) ? true : types.All(t => t.IsNullable == false) ? false : null;

    private (int Precision, int Scale)? DecimalShape(RuntimeColumnDataType type) => SqlName(type) switch
    {
        "tinyint" => (3, 0), "smallint" => (5, 0), "int" => (type.IntegerLiteralPrecision ?? 10, 0), "bigint" => (19, 0),
        "money" => (19, 4), "smallmoney" => (10, 4),
        "decimal" or "numeric" when type.Precision.HasValue && type.Scale.HasValue => (type.Precision.Value, type.Scale.Value),
        _ => null
    };

    private RuntimeColumnDataType? TryCreateFunctionDataType(ScalarExpression expression)
    {
        var function = navigator.TryGetFunctionCall(expression);
        if (function is null || navigator.HasFunctionCallCallTarget(function)) return null;
        var name = navigator.TryGetFunctionCallName(function)?.ToUpperInvariant();
        var parameters = navigator.GetFunctionCallParameters(function);
        var display = $"{name} expression";

        if (name is "COUNT" or "COUNT_BIG" or "APPROX_COUNT_DISTINCT" && parameters.Count == 1)
            return SqlResult(name == "COUNT" ? "int" : "bigint", true, display);
        if (name is "GROUPING" or "GROUPING_ID" && parameters.Count > 0)
            return SqlResult(name == "GROUPING" ? "tinyint" : "int", false, display);
        if ((name is "RANK" or "DENSE_RANK" or "ROW_NUMBER" && parameters.Count == 0 || name == "NTILE" && parameters.Count == 1) && navigator.HasFunctionCallOverClause(function))
            return SqlResult("bigint", false, display);

        if (name == "CONCAT" && parameters.Count >= 2)
        {
            var inputs = parameters.Select(TryResolveExpressionDataType).ToArray();
            // Nontext argument formatting is not yet modeled here.
            if (inputs.Any(t => t is null || !IsText(SqlName(t)))) return null;
            var unicode = inputs.Any(t => SqlName(t!) is "nvarchar" or "nchar");
            int? length = inputs.Any(t => t!.Length == -1) ? -1 : inputs.Any(t => !t!.Length.HasValue) ? null :
                Math.Min(unicode ? 4000 : 8000, inputs.Sum(t => t!.Length!.Value));
            return SqlResult(unicode ? "nvarchar" : "varchar", false, display, length);
        }

        var first = parameters.Count > 0 ? TryResolveExpressionDataType(parameters[0]) : null;
        if (first is null) return null;
        var firstName = SqlName(first);
        if (name is "MIN" or "MAX" && parameters.Count == 1 && (IsNumeric(firstName!) || IsText(firstName!) || firstName is "date" or "datetime" or "datetime2" or "datetimeoffset" or "smalldatetime" or "time" or "uniqueidentifier"))
            return first with { IsNullable = true, DisplayName = display };
        if (name is "SUM" or "AVG" && parameters.Count == 1)
        {
            if (IsDecimal(firstName!))
                return first.Scale is int scale ? SqlResult(firstName!, true, display, precision: 38, scale: name == "AVG" ? Math.Max(6, scale) : scale) : null;
            var resultName = firstName switch
            {
                "tinyint" or "smallint" or "int" => "int", "bigint" => "bigint",
                "money" or "smallmoney" => "money", "float" or "real" => "float", _ => null
            };
            return resultName is null ? null : SqlResult(resultName, true, display);
        }
        if (name is "STDEV" or "STDEVP" or "VAR" or "VARP" && parameters.Count == 1)
            return IsNumeric(firstName!) ? SqlResult("float", true, display) : null;
        if (name == "ROUND" && parameters.Count is 2 or 3 && IsNumeric(firstName!))
        {
            var operands = parameters.Select((parameter, i) => i == 0 ? first : TryResolveExpressionDataType(parameter)).ToArray();
            if (operands.Any(t => t is null)) return null;
            var nullable = NullableUnion(operands.Select(t => t!).ToArray());
            return firstName switch
            {
                "tinyint" or "smallint" => SqlResult("int", nullable, display),
                "real" => SqlResult("float", nullable, display),
                "smallmoney" => SqlResult("money", nullable, display),
                _ => first with { IsNullable = nullable, DisplayName = display }
            };
        }
        if (name == "ISNULL" && parameters.Count == 2)
        {
            var replacement = TryResolveExpressionDataType(parameters[1]);
            if (replacement is null) return null;
            var result = IsNullLiteral(parameters[0]) ? replacement : first;
            return result with { IsNullable = first.IsNullable == false || replacement.IsNullable == false ? false : NullableUnion(first, replacement), DisplayName = display };
        }
        if (name == "SUBSTRING" && parameters.Count == 3 && IsText(firstName!))
        {
            int? length = first.Length;
            if (first.Length != -1 && TryIntegerLiteral(parameters[2]) is int requested && requested >= 0)
                length = first.Length is int finite ? Math.Min(finite, requested) : null;
            var operands = parameters.Select((parameter, i) => i == 0 ? first : TryResolveExpressionDataType(parameter)).ToArray();
            return SqlResult(firstName is "nchar" or "nvarchar" ? "nvarchar" : "varchar", operands.Any(t => t is null) ? null : NullableUnion(operands.Select(t => t!).ToArray()), display, length);
        }
        if (name is "LTRIM" or "RTRIM" or "TRIM" or "LOWER" or "UPPER" && parameters.Count == 1 && IsText(firstName!))
            return SqlResult(firstName is "nchar" or "nvarchar" ? "nvarchar" : "varchar", first.IsNullable, display, first.Length);
        if (name == "ABS" && parameters.Count == 1 && IsNumeric(firstName!))
            return IsDecimal(firstName!) ? SqlResult(firstName!, first.IsNullable, display, precision: 38, scale: first.Scale) :
                SqlResult(firstName is "smallint" or "tinyint" ? "int" : firstName == "smallmoney" ? "money" : firstName == "real" ? "float" : firstName!, first.IsNullable, display);
        return null;
    }

    private RuntimeColumnDataType? TryCreateOperatorDataType(ScalarExpression expression)
    {
        if (navigator.TryGetUnaryExpression(expression) is { } unary &&
            navigator.TryGetUnaryExpressionOperand(unary) is { } operand &&
            TryResolveExpressionDataType(operand) is { } operandType)
        {
            var name = SqlName(operandType);
            if (unary.UnaryExpressionType == "BitwiseNot" && name is not ("tinyint" or "smallint" or "int" or "bigint")) return null;
            if (!IsNumeric(name)) return null;
            return name == "tinyint" && unary.UnaryExpressionType == "Negative"
                ? SqlResult("smallint", operandType.IsNullable, "unary expression")
                : operandType with { DisplayName = "unary expression" };
        }
        if (navigator.TryGetBinaryExpression(expression) is not { } binary ||
            navigator.TryGetBinaryExpressionOperands(binary) is not { First: { } left, Second: { } right } ||
            TryResolveExpressionDataType(left) is not { } first ||
            TryResolveExpressionDataType(right) is not { } second) return null;

        var firstName = SqlName(first);
        var secondName = SqlName(second);
        var operation = binary.BinaryExpressionType;
        var nullable = NullableUnion(first, second);
        if (operation == "Add" && IsText(firstName) && IsText(secondName))
        {
            var name = CommonTextName(firstName, secondName);
            int? length = first.Length == -1 || second.Length == -1 ? -1 :
                first.Length is int a && second.Length is int b ? Math.Min(name is "nchar" or "nvarchar" ? 4000 : 8000, a + b) : null;
            return SqlResult(name, nullable, "concatenation", length);
        }
        if (!IsNumeric(firstName) || !IsNumeric(secondName)) return null;
        if (operation is not ("Add" or "Subtract" or "Multiply" or "Divide" or "Modulo")) return null;
        if (operation == "Modulo" && (firstName is "real" or "float" || secondName is "real" or "float")) return null;
        var result = CommonDataType(first, second);
        if (result is null) return null;
        if (!IsDecimal(SqlName(result))) return result with { IsNullable = nullable, DisplayName = "arithmetic expression" };
        if (DecimalOperandShape(first, left) is not { } x || DecimalOperandShape(second, right) is not { } y) return null;
        var (p1, s1) = x;
        var (p2, s2) = y;
        var (precision, scale) = operation switch
        {
            "Add" or "Subtract" => (Math.Max(s1, s2) + Math.Max(p1 - s1, p2 - s2) + 1, Math.Max(s1, s2)),
            "Multiply" => (p1 + p2 + 1, s1 + s2),
            "Divide" => (p1 - s1 + s2 + Math.Max(6, s1 + p2 + 1), Math.Max(6, s1 + p2 + 1)),
            "Modulo" => (Math.Min(p1 - s1, p2 - s2) + Math.Max(s1, s2), Math.Max(s1, s2)),
            _ => (0, 0)
        };
        if (precision == 0) return null;
        if (precision > 38)
        {
            var integral = precision - scale;
            // Addition/subtraction reserve the operands' integral digits; the
            // extra carry digit in the nominal precision does not consume scale.
            scale = operation is "Add" or "Subtract"
                ? Math.Min(scale, Math.Max(0, 38 - Math.Max(p1 - s1, p2 - s2)))
                : operation is "Multiply" or "Divide" && integral > 32
                ? Math.Min(scale, 6)
                : Math.Max(0, Math.Min(scale, 38 - integral));
            precision = 38;
        }
        return SqlResult(firstName == "numeric" || secondName == "numeric" ? "numeric" : "decimal", nullable, "decimal arithmetic expression", precision: precision, scale: scale);
    }

    private RuntimeColumnDataType? TryCreateNullIfDataType(ScalarExpression expression)
    {
        if (navigator.TryGetNullIfExpression(expression) is not { } nullIf ||
            navigator.TryGetNullIfExpressionOperands(nullIf)?.First is not { } first) return null;
        return TryResolveExpressionDataType(first) is { } type ? type with { IsNullable = true, DisplayName = "NULLIF expression" } : null;
    }

    private RuntimeColumnDataType? TryCreateSubqueryDataType(ScalarExpression expression)
    {
        if (navigator.TryGetScalarSubquery(expression) is not { } subquery ||
            navigator.TryGetScalarSubqueryQueryExpressionId(subquery) is not { } queryId) return null;
        var rowset = boundRowsets.LastOrDefault(r => r.Id == $"{queryId}:output-rowset");
        return rowset?.Columns.Count == 1 && rowset.Columns[0].DataType is { } type
            ? type with { IsNullable = true, DisplayName = "scalar subquery" } : null;
    }

    private int? TryIntegerLiteral(ScalarExpression expression) => navigator.TryGetLiteral(expression) is { LiteralType: "Integer", Value: { } value } &&
        int.TryParse(value, out var number) ? number : null;

    private bool IsNullLiteral(ScalarExpression expression) => navigator.TryGetLiteral(expression)?.LiteralType is "Null";

    // SQL Server uses an integer constant's minimum decimal precision when combining it with decimal.
    private (int Precision, int Scale)? DecimalOperandShape(RuntimeColumnDataType type, ScalarExpression expression) =>
        TryIntegerLiteral(expression) is int n ? (Math.Max(1, n.ToString(System.Globalization.CultureInfo.InvariantCulture).TrimStart('-').Length), 0) : DecimalShape(type);

    private RuntimeColumnDataType? CommonResultDataType(IEnumerable<RuntimeColumnDataType?> types)
    {
        RuntimeColumnDataType? result = null;
        foreach (var type in types)
        {
            if (type is null) return null;
            if (type.IsUntypedNull) continue;
            result = result is null ? type : CommonDataType(result, type);
            if (result is null) return null;
        }
        return result;
    }

    private RuntimeColumnDataType? CommonDataType(RuntimeColumnDataType first, RuntimeColumnDataType second)
    {
        var firstName = SqlName(first);
        var secondName = SqlName(second);
        var nullable = NullableUnion(first, second);
        if (first.IsUntypedNull) return second with { IsNullable = nullable };
        if (second.IsUntypedNull) return first with { IsNullable = nullable };
        if (HasSameDataTypeContract(first, second)) return first with
        {
            IsNullable = nullable,
            IntegerLiteralPrecision = first.IntegerLiteralPrecision is int a && second.IntegerLiteralPrecision is int b ? Math.Max(a, b) : null
        };
        if (IsNumeric(firstName) && IsNumeric(secondName))
        {
            string[] precedence = ["float", "real", "decimal", "money", "smallmoney", "bigint", "int", "smallint", "tinyint"];
            var higher = Array.IndexOf(precedence, IsDecimal(firstName) ? "decimal" : firstName) <= Array.IndexOf(precedence, IsDecimal(secondName) ? "decimal" : secondName) ? first : second;
            if (!IsDecimal(SqlName(higher))) return higher with { IsNullable = nullable };
            if (DecimalShape(first) is not { } x || DecimalShape(second) is not { } y) return null;
            var scale = Math.Max(x.Scale, y.Scale);
            var integral = Math.Max(x.Precision - x.Scale, y.Precision - y.Scale);
            return SqlResult(SqlName(higher), nullable, "common decimal type", precision: Math.Min(38, integral + scale), scale: Math.Min(scale, 38 - integral));
        }
        if (IsText(firstName) && IsText(secondName))
        {
            var name = CommonTextName(firstName, secondName);
            int? length = first.Length == -1 || second.Length == -1 ? -1 :
                first.Length is int a && second.Length is int b ? Math.Min(name is "nchar" or "nvarchar" ? 4000 : 8000, Math.Max(a, b)) : null;
            return SqlResult(name, nullable, "common string type", length);
        }
        return null;
    }
}
