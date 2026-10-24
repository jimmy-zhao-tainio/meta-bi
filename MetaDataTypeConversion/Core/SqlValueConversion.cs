using MetaDataType;
using MetaTransformScript.Sql;

namespace MetaDataTypeConversion.Core;

public sealed record SqlValueConversionRequest(
    string SourceDataTypeId,
    string TargetDataTypeSystemName,
    string InputExpression,
    SqlServerTypeFacets TargetFacets);

public sealed record SqlValueConversionResult(DataTypeMappingResolution Mapping, SqlScalarExpression Expression);

public sealed partial class MetaDataTypeConversionService
{
    public SqlValueConversionResult GenerateSqlExpression(
        MetaDataTypeConversionModel model, MetaDataTypeModel dataTypes, SqlValueConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(dataTypes);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.TargetFacets);
        // Value generation executes exactly one selected mapping, never a compatibility path.
        var check = Check(model);
        if (check.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, check.Errors));
        var resolution = Resolve(model, request.SourceDataTypeId, request.TargetDataTypeSystemName);
        var mapping = model.DataTypeMappingList.Single(row => row.Id == resolution.MappingId);
        var implementation = model.ConversionImplementationList.Single(row => row.Id == resolution.ConversionImplementationId);
        var syntax = new MetaTransformScriptSqlService();
        if (implementation.Id == "MetaDataTypeConversion:implementation:direct")
        {
            if (implementation.SqlConversionTemplate is not null)
                throw new InvalidOperationException("Direct is a pass-through and cannot have a SQL conversion template. Select a value-conversion implementation instead.");
            return new(resolution, syntax.ImportScalarExpression(request.InputExpression));
        }
        if (implementation.Id == "MetaDataTypeConversion:implementation:structural")
            throw new InvalidOperationException($"Mapping '{mapping.Id}' is Structural; it has no scalar SQL value conversion.");
        var reference = implementation.SqlConversionTemplate
            ?? throw new InvalidOperationException($"Mapping '{mapping.Id}', implementation '{implementation.Id}' has no SQL conversion template.");
        var template = model.SqlConversionTemplateList.SingleOrDefault(row => row.Id == reference.Id)
            ?? throw new InvalidOperationException($"Implementation '{implementation.Id}' references missing SQL conversion template '{reference.Id}'.");
        if (template.Dialect != "SqlServer" || !BelongsToDataTypeSystem(resolution.TargetDataTypeId, "SqlServer"))
            throw new InvalidOperationException($"Template '{template.Id}' dialect '{template.Dialect}' cannot realize mapping '{mapping.Id}'; only SQL Server targets are supported.");
        var function = template.Function switch
        {
            "Cast" => SqlConversionFunction.Cast,
            "Convert" => SqlConversionFunction.Convert,
            _ => throw new InvalidOperationException($"Template '{template.Id}' has unsupported SQL conversion function '{template.Function}'; supported strict functions are Cast and Convert.")
        };
        var target = dataTypes.DataTypeList.SingleOrDefault(row => row.Id == resolution.TargetDataTypeId)
            ?? throw new InvalidOperationException($"Mapping '{mapping.Id}' target '{resolution.TargetDataTypeId}' is absent from MetaDataType.");
        if (!string.Equals(target.DataTypeSystem?.Name, "SqlServer", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Target '{target.Id}' does not belong to the SqlServer data type system.");
        return new(resolution, syntax.CreateConversionExpression(request.InputExpression, target.Name, request.TargetFacets, function));
    }
}
