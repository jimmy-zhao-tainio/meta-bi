using System.Globalization;
using MetaTransformScript.Sql.Parsing;
using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql;

public sealed record SqlScalarExpression(MetaTransformScriptModel Model, ScalarExpression Root);

public enum SqlConversionFunction { Cast, Convert }

/// <summary>Explicit SQL Server target parameters. No SQL defaults are supplied.</summary>
public sealed record SqlServerTypeFacets(int? Length = null, bool IsMax = false, int? Precision = null, int? Scale = null);

public sealed partial class MetaTransformScriptSqlService
{
    public SqlScalarExpression ImportScalarExpression(string sql)
    {
        var builder = new MetaTransformScriptSqlModelBuilder();
        var expression = ParseScalarInput(sql, builder);
        return new(builder.Build(), expression.GetRef<ScalarExpression>(nameof(ScalarExpression)));
    }

    public string ExportScalarExpression(SqlScalarExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        return new MetaTransformScriptSqlEmitter(expression.Model).RenderScalarExpressionForScriptObject(expression.Root);
    }

    /// <summary>Compose existing sanctioned CAST/CONVERT nodes; no mapping policy is applied here.</summary>
    public SqlScalarExpression CreateConversionExpression(
        string inputSql, string targetTypeName, SqlServerTypeFacets facets, SqlConversionFunction function)
    {
        ArgumentNullException.ThrowIfNull(facets);
        var parameters = GetExplicitTypeParameters(targetTypeName, facets);
        if (!MetaTransformScriptSqlServerDataTypes.TryMapSqlName(targetTypeName, out var option) ||
            !string.Equals(MetaTransformScriptSqlServerDataTypes.RenderSqlName(option), targetTypeName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"SQL conversion target '{targetTypeName}' must be a supported canonical SQL Server type.");
        }

        var builder = new MetaTransformScriptSqlModelBuilder();
        var input = ParseScalarInput(inputSql, builder);
        var type = builder.CreateSqlDataTypeReference(option, parameters.Select(value =>
            value == "max" ? builder.CreateMaxLiteral() : builder.CreateNumberLiteral(value)).ToArray());
        var result = function switch
        {
            SqlConversionFunction.Cast => builder.CreateCastCall(input, type),
            SqlConversionFunction.Convert => builder.CreateConvertCall(type, input),
            _ => throw new InvalidOperationException($"Unsupported SQL conversion function '{function}'.")
        };
        return new(builder.Build(), result.GetRef<ScalarExpression>(nameof(ScalarExpression)));
    }

    private static BuiltNode ParseScalarInput(string sql, MetaTransformScriptSqlModelBuilder builder)
    {
        try
        {
            return new MetaTransformScriptSqlParser().ParseScalarFragment(sql, builder);
        }
        catch (MetaTransformScriptSqlParserException exception)
        {
            throw CreateImportException(exception, sourcePath: null);
        }
    }

    private static IReadOnlyList<string> GetExplicitTypeParameters(string name, SqlServerTypeFacets facets)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var type = name.ToLowerInvariant();
        var lengthType = type is "char" or "varchar" or "nchar" or "nvarchar" or "binary" or "varbinary";
        var decimalType = type is "decimal" or "numeric";
        var temporalType = type is "time" or "datetime2" or "datetimeoffset";
        var floatType = type == "float";
        void Reject(string reason) => throw new ArgumentException($"Target type '{name}': {reason}", nameof(facets));
        if (!lengthType && (facets.Length.HasValue || facets.IsMax)) Reject("length/MAX is not applicable.");
        if (!decimalType && !floatType && facets.Precision.HasValue) Reject("precision is not applicable.");
        if (!decimalType && !temporalType && facets.Scale.HasValue) Reject("scale is not applicable.");
        string Number(int? value) => value!.Value.ToString(CultureInfo.InvariantCulture);
        if (lengthType)
        {
            if (facets.IsMax)
            {
                if (facets.Length.HasValue || type is not ("varchar" or "nvarchar" or "varbinary"))
                    Reject("MAX requires a variable-length type and cannot be combined with length.");
                return ["max"];
            }
            var limit = type is "nchar" or "nvarchar" ? 4000 : 8000;
            if (!facets.Length.HasValue || facets.Length < 1 || facets.Length > limit)
                Reject($"an explicit length from 1 through {limit} (or supported MAX) is required.");
            return [Number(facets.Length)];
        }
        if (decimalType)
        {
            if (facets.Precision is null or < 1 or > 38 || facets.Scale is null or < 0 || facets.Scale > facets.Precision)
                Reject("explicit precision (1..38) and scale (0..precision) are required.");
            return [Number(facets.Precision), Number(facets.Scale)];
        }
        if (temporalType)
        {
            if (facets.Scale is null or < 0 or > 7) Reject("an explicit fractional-second scale (0..7) is required.");
            return [Number(facets.Scale)];
        }
        if (floatType)
        {
            if (facets.Precision is null or < 1 or > 53) Reject("an explicit precision (1..53) is required.");
            return [Number(facets.Precision)];
        }
        return [];
    }
}
