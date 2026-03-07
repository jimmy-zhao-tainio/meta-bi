namespace MetaDataTypeConversion.Core;

internal static class MetaDataTypeConversionSeedData
{
    internal readonly record struct ConversionImplementationSeed(string Id, string Name, string? Description = null);
    internal readonly record struct DataTypeMappingSeed(string Id, string SourceDataTypeId, string TargetDataTypeId, string ConversionImplementationId, string? Notes = null);

    public static readonly ConversionImplementationSeed[] ConversionImplementations =
    [
        new("MetaDataTypeConversion:implementation:direct", "Direct", "Direct sanctioned type mapping."),
        new("MetaDataTypeConversion:implementation:structural", "Structural", "Structural sanctioned type mapping for non-scalar types.")
    ];

    public static readonly DataTypeMappingSeed[] DataTypeMappings =
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
        new("MetaDataTypeConversion:mapping:sqlserver:hierarchyid", "sqlserver:type:hierarchyid", "meta:type:hierarchyid", "MetaDataTypeConversion:implementation:structural")
    ];
}
