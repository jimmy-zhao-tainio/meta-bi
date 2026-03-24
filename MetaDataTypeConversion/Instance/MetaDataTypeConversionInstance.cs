using MetaDataTypeConversion;

namespace MetaDataTypeConversion.Instance;

public static class MetaDataTypeConversionInstance
{
    private static MetaDataTypeConversionModel CreateDefault()
    {
        var model = MetaDataTypeConversionModel.CreateEmpty();

        foreach (var implementation in ConversionImplementations)
        {
            model.ConversionImplementationList.Add(new ConversionImplementation
            {
                Id = implementation.Id,
                Name = implementation.Name,
                Description = implementation.Description ?? string.Empty,
            });
        }

        var implementationsById = model.ConversionImplementationList.ToDictionary(row => row.Id, StringComparer.Ordinal);

        foreach (var mapping in DataTypeMappings)
        {
            model.DataTypeMappingList.Add(new DataTypeMapping
            {
                Id = mapping.Id,
                SourceDataTypeId = mapping.SourceDataTypeId,
                TargetDataTypeId = mapping.TargetDataTypeId,
                ConversionImplementationId = mapping.ConversionImplementationId,
                ConversionImplementation = implementationsById[mapping.ConversionImplementationId],
                Notes = mapping.Notes ?? string.Empty,
            });
        }

        return model;
    }

    private readonly record struct ConversionImplementationSeed(string Id, string Name, string? Description = null);
    private readonly record struct DataTypeMappingSeed(string Id, string SourceDataTypeId, string TargetDataTypeId, string ConversionImplementationId, string? Notes = null);

    private static readonly ConversionImplementationSeed[] ConversionImplementations =
    [
        new("MetaDataTypeConversion:implementation:direct", "Direct", "Direct sanctioned type mapping."),
        new("MetaDataTypeConversion:implementation:structural", "Structural", "Structural sanctioned type mapping for non-scalar types.")
    ];

    private static readonly DataTypeMappingSeed[] DataTypeMappings =
    [
        new("MetaDataTypeConversion:mapping:sqlserver:char", "sqlserver:type:char", "meta:type:AnsiStringFixedLength", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:varchar", "sqlserver:type:varchar", "meta:type:AnsiString", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:nchar", "sqlserver:type:nchar", "meta:type:StringFixedLength", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:nvarchar", "sqlserver:type:nvarchar", "meta:type:String", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:smallmoney", "sqlserver:type:smallmoney", "meta:type:Decimal", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:money", "sqlserver:type:money", "meta:type:Decimal", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:bit", "sqlserver:type:bit", "meta:type:Boolean", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:tinyint", "sqlserver:type:tinyint", "meta:type:Byte", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:smallint", "sqlserver:type:smallint", "meta:type:Int16", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:int", "sqlserver:type:int", "meta:type:Int32", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:bigint", "sqlserver:type:bigint", "meta:type:Int64", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:binary", "sqlserver:type:binary", "meta:type:Binary", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:decimal", "sqlserver:type:decimal", "meta:type:Decimal", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:float", "sqlserver:type:float", "meta:type:Double", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:time", "sqlserver:type:time", "meta:type:Time", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:date", "sqlserver:type:date", "meta:type:Date", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:datetime", "sqlserver:type:datetime", "meta:type:DateTime", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:datetime2", "sqlserver:type:datetime2", "meta:type:DateTime2", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:datetimeoffset", "sqlserver:type:datetimeoffset", "meta:type:DateTimeOffset", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:varbinary", "sqlserver:type:varbinary", "meta:type:Binary", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:uniqueidentifier", "sqlserver:type:uniqueidentifier", "meta:type:Guid", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:sqlserver:xml", "sqlserver:type:xml", "meta:type:Xml", "MetaDataTypeConversion:implementation:structural"),
        new("MetaDataTypeConversion:mapping:sqlserver:sql_variant", "sqlserver:type:sql_variant", "meta:type:Object", "MetaDataTypeConversion:implementation:structural"),
        new("MetaDataTypeConversion:mapping:sqlserver:geometry", "sqlserver:type:geometry", "meta:type:geometry", "MetaDataTypeConversion:implementation:structural"),
        new("MetaDataTypeConversion:mapping:sqlserver:geography", "sqlserver:type:geography", "meta:type:geography", "MetaDataTypeConversion:implementation:structural"),
        new("MetaDataTypeConversion:mapping:sqlserver:hierarchyid", "sqlserver:type:hierarchyid", "meta:type:hierarchyid", "MetaDataTypeConversion:implementation:structural"),
        new("MetaDataTypeConversion:mapping:meta:AnsiStringFixedLength", "meta:type:AnsiStringFixedLength", "sqlserver:type:char", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:AnsiString", "meta:type:AnsiString", "sqlserver:type:varchar", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:StringFixedLength", "meta:type:StringFixedLength", "sqlserver:type:nchar", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:String", "meta:type:String", "sqlserver:type:nvarchar", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Decimal", "meta:type:Decimal", "sqlserver:type:decimal", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Boolean", "meta:type:Boolean", "sqlserver:type:bit", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Byte", "meta:type:Byte", "sqlserver:type:tinyint", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Int16", "meta:type:Int16", "sqlserver:type:smallint", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Int32", "meta:type:Int32", "sqlserver:type:int", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Int64", "meta:type:Int64", "sqlserver:type:bigint", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Binary", "meta:type:Binary", "sqlserver:type:binary", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Double", "meta:type:Double", "sqlserver:type:float", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Time", "meta:type:Time", "sqlserver:type:time", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Date", "meta:type:Date", "sqlserver:type:date", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:DateTime", "meta:type:DateTime", "sqlserver:type:datetime", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:DateTime2", "meta:type:DateTime2", "sqlserver:type:datetime2", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:DateTimeOffset", "meta:type:DateTimeOffset", "sqlserver:type:datetimeoffset", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Guid", "meta:type:Guid", "sqlserver:type:uniqueidentifier", "MetaDataTypeConversion:implementation:direct"),
        new("MetaDataTypeConversion:mapping:meta:Xml", "meta:type:Xml", "sqlserver:type:xml", "MetaDataTypeConversion:implementation:structural"),
        new("MetaDataTypeConversion:mapping:meta:Object", "meta:type:Object", "sqlserver:type:sql_variant", "MetaDataTypeConversion:implementation:structural"),
        new("MetaDataTypeConversion:mapping:meta:geometry", "meta:type:geometry", "sqlserver:type:geometry", "MetaDataTypeConversion:implementation:structural"),
        new("MetaDataTypeConversion:mapping:meta:geography", "meta:type:geography", "sqlserver:type:geography", "MetaDataTypeConversion:implementation:structural"),
        new("MetaDataTypeConversion:mapping:meta:hierarchyid", "meta:type:hierarchyid", "sqlserver:type:hierarchyid", "MetaDataTypeConversion:implementation:structural")
    ];

    public static MetaDataTypeConversionModel Default { get; } = CreateDefault();
}
