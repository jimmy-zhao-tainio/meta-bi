#nullable enable
using System;
using System.Collections.Generic;

namespace MetaDataTypeConversion;
public sealed partial class ConversionImplementation
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class DataTypeMapping
{
    public string Id { get; set; } = null !;
    public string? Notes { get; set; }
    public string SourceDataTypeId { get; set; } = null !;
    public string TargetDataTypeId { get; set; } = null !;
    public ConversionImplementation ConversionImplementation { get; set; } = null !;
}

public sealed partial class MetaDataTypeConversionModel
{
    public static MetaDataTypeConversionModel CreateEmpty() => new();
    public List<ConversionImplementation> ConversionImplementationList { get; set; } = new();
    public List<DataTypeMapping> DataTypeMappingList { get; set; } = new();
}

public static partial class MetaDataTypeConversionInstance
{
    private static readonly MetaDataTypeConversionModel _builtIn = CreateBuiltIn();
    public static MetaDataTypeConversionModel BuiltIn => _builtIn;

    public static MetaDataTypeConversionModel CreateBuiltIn()
    {
        var model = MetaDataTypeConversionModel.CreateEmpty();
        var record0 = new ConversionImplementation
        {
            Id = "MetaDataTypeConversion:implementation:direct",
            Description = "Direct sanctioned type mapping.",
            Name = "Direct"
        };
        model.ConversionImplementationList.Add(record0);
        var record1 = new ConversionImplementation
        {
            Id = "MetaDataTypeConversion:implementation:structural",
            Description = "Structural sanctioned type mapping for non-scalar types.",
            Name = "Structural"
        };
        model.ConversionImplementationList.Add(record1);
        var record2 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:AnsiString",
            SourceDataTypeId = "meta:type:AnsiString",
            TargetDataTypeId = "sqlserver:type:varchar"
        };
        model.DataTypeMappingList.Add(record2);
        var record3 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:AnsiStringFixedLength",
            SourceDataTypeId = "meta:type:AnsiStringFixedLength",
            TargetDataTypeId = "sqlserver:type:char"
        };
        model.DataTypeMappingList.Add(record3);
        var record4 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Binary",
            SourceDataTypeId = "meta:type:Binary",
            TargetDataTypeId = "sqlserver:type:binary"
        };
        model.DataTypeMappingList.Add(record4);
        var record5 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Boolean",
            SourceDataTypeId = "meta:type:Boolean",
            TargetDataTypeId = "sqlserver:type:bit"
        };
        model.DataTypeMappingList.Add(record5);
        var record6 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Byte",
            SourceDataTypeId = "meta:type:Byte",
            TargetDataTypeId = "sqlserver:type:tinyint"
        };
        model.DataTypeMappingList.Add(record6);
        var record7 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Date",
            SourceDataTypeId = "meta:type:Date",
            TargetDataTypeId = "sqlserver:type:date"
        };
        model.DataTypeMappingList.Add(record7);
        var record8 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:DateTime",
            SourceDataTypeId = "meta:type:DateTime",
            TargetDataTypeId = "sqlserver:type:datetime"
        };
        model.DataTypeMappingList.Add(record8);
        var record9 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:DateTime2",
            SourceDataTypeId = "meta:type:DateTime2",
            TargetDataTypeId = "sqlserver:type:datetime2"
        };
        model.DataTypeMappingList.Add(record9);
        var record10 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:DateTimeOffset",
            SourceDataTypeId = "meta:type:DateTimeOffset",
            TargetDataTypeId = "sqlserver:type:datetimeoffset"
        };
        model.DataTypeMappingList.Add(record10);
        var record11 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Decimal",
            SourceDataTypeId = "meta:type:Decimal",
            TargetDataTypeId = "sqlserver:type:decimal"
        };
        model.DataTypeMappingList.Add(record11);
        var record12 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Double",
            SourceDataTypeId = "meta:type:Double",
            TargetDataTypeId = "sqlserver:type:float"
        };
        model.DataTypeMappingList.Add(record12);
        var record13 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:geography",
            SourceDataTypeId = "meta:type:geography",
            TargetDataTypeId = "sqlserver:type:geography"
        };
        model.DataTypeMappingList.Add(record13);
        var record14 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:geometry",
            SourceDataTypeId = "meta:type:geometry",
            TargetDataTypeId = "sqlserver:type:geometry"
        };
        model.DataTypeMappingList.Add(record14);
        var record15 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Guid",
            SourceDataTypeId = "meta:type:Guid",
            TargetDataTypeId = "sqlserver:type:uniqueidentifier"
        };
        model.DataTypeMappingList.Add(record15);
        var record16 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:hierarchyid",
            SourceDataTypeId = "meta:type:hierarchyid",
            TargetDataTypeId = "sqlserver:type:hierarchyid"
        };
        model.DataTypeMappingList.Add(record16);
        var record17 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Int16",
            SourceDataTypeId = "meta:type:Int16",
            TargetDataTypeId = "sqlserver:type:smallint"
        };
        model.DataTypeMappingList.Add(record17);
        var record18 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Int32",
            SourceDataTypeId = "meta:type:Int32",
            TargetDataTypeId = "sqlserver:type:int"
        };
        model.DataTypeMappingList.Add(record18);
        var record19 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Int64",
            SourceDataTypeId = "meta:type:Int64",
            TargetDataTypeId = "sqlserver:type:bigint"
        };
        model.DataTypeMappingList.Add(record19);
        var record20 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Object",
            SourceDataTypeId = "meta:type:Object",
            TargetDataTypeId = "sqlserver:type:sql_variant"
        };
        model.DataTypeMappingList.Add(record20);
        var record21 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Single",
            SourceDataTypeId = "meta:type:Single",
            TargetDataTypeId = "sqlserver:type:real"
        };
        model.DataTypeMappingList.Add(record21);
        var record22 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:String",
            SourceDataTypeId = "meta:type:String",
            TargetDataTypeId = "sqlserver:type:nvarchar"
        };
        model.DataTypeMappingList.Add(record22);
        var record23 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:StringFixedLength",
            SourceDataTypeId = "meta:type:StringFixedLength",
            TargetDataTypeId = "sqlserver:type:nchar"
        };
        model.DataTypeMappingList.Add(record23);
        var record24 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Time",
            SourceDataTypeId = "meta:type:Time",
            TargetDataTypeId = "sqlserver:type:time"
        };
        model.DataTypeMappingList.Add(record24);
        var record25 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:meta:Xml",
            SourceDataTypeId = "meta:type:Xml",
            TargetDataTypeId = "sqlserver:type:xml"
        };
        model.DataTypeMappingList.Add(record25);
        var record26 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:AccountNumber",
            SourceDataTypeId = "sqlserver:type:AccountNumber",
            TargetDataTypeId = "meta:type:String"
        };
        model.DataTypeMappingList.Add(record26);
        var record27 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:AccountNumber-to-nvarchar",
            Notes = "SQL Server alias/user-defined type compatibility.",
            SourceDataTypeId = "sqlserver:type:AccountNumber",
            TargetDataTypeId = "sqlserver:type:nvarchar"
        };
        model.DataTypeMappingList.Add(record27);
        var record28 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:bigint",
            SourceDataTypeId = "sqlserver:type:bigint",
            TargetDataTypeId = "meta:type:Int64"
        };
        model.DataTypeMappingList.Add(record28);
        var record29 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:binary",
            SourceDataTypeId = "sqlserver:type:binary",
            TargetDataTypeId = "meta:type:Binary"
        };
        model.DataTypeMappingList.Add(record29);
        var record30 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:bit",
            SourceDataTypeId = "sqlserver:type:bit",
            TargetDataTypeId = "meta:type:Boolean"
        };
        model.DataTypeMappingList.Add(record30);
        var record31 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:char",
            SourceDataTypeId = "sqlserver:type:char",
            TargetDataTypeId = "meta:type:AnsiStringFixedLength"
        };
        model.DataTypeMappingList.Add(record31);
        var record32 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:char-to-nvarchar",
            Notes = "SQL Server write compatibility for text normalization.",
            SourceDataTypeId = "sqlserver:type:char",
            TargetDataTypeId = "sqlserver:type:nvarchar"
        };
        model.DataTypeMappingList.Add(record32);
        var record33 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:date",
            SourceDataTypeId = "sqlserver:type:date",
            TargetDataTypeId = "meta:type:Date"
        };
        model.DataTypeMappingList.Add(record33);
        var record34 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:datetime",
            SourceDataTypeId = "sqlserver:type:datetime",
            TargetDataTypeId = "meta:type:DateTime"
        };
        model.DataTypeMappingList.Add(record34);
        var record35 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:datetime2",
            SourceDataTypeId = "sqlserver:type:datetime2",
            TargetDataTypeId = "meta:type:DateTime2"
        };
        model.DataTypeMappingList.Add(record35);
        var record36 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:datetimeoffset",
            SourceDataTypeId = "sqlserver:type:datetimeoffset",
            TargetDataTypeId = "meta:type:DateTimeOffset"
        };
        model.DataTypeMappingList.Add(record36);
        var record37 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:decimal",
            SourceDataTypeId = "sqlserver:type:decimal",
            TargetDataTypeId = "meta:type:Decimal"
        };
        model.DataTypeMappingList.Add(record37);
        var record38 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:Flag",
            SourceDataTypeId = "sqlserver:type:Flag",
            TargetDataTypeId = "meta:type:Boolean"
        };
        model.DataTypeMappingList.Add(record38);
        var record39 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:Flag-to-bit",
            Notes = "SQL Server alias/user-defined type compatibility.",
            SourceDataTypeId = "sqlserver:type:Flag",
            TargetDataTypeId = "sqlserver:type:bit"
        };
        model.DataTypeMappingList.Add(record39);
        var record40 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:float",
            SourceDataTypeId = "sqlserver:type:float",
            TargetDataTypeId = "meta:type:Double"
        };
        model.DataTypeMappingList.Add(record40);
        var record41 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:geography",
            SourceDataTypeId = "sqlserver:type:geography",
            TargetDataTypeId = "meta:type:geography"
        };
        model.DataTypeMappingList.Add(record41);
        var record42 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:geometry",
            SourceDataTypeId = "sqlserver:type:geometry",
            TargetDataTypeId = "meta:type:geometry"
        };
        model.DataTypeMappingList.Add(record42);
        var record43 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:hierarchyid",
            SourceDataTypeId = "sqlserver:type:hierarchyid",
            TargetDataTypeId = "meta:type:hierarchyid"
        };
        model.DataTypeMappingList.Add(record43);
        var record44 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:int",
            SourceDataTypeId = "sqlserver:type:int",
            TargetDataTypeId = "meta:type:Int32"
        };
        model.DataTypeMappingList.Add(record44);
        var record45 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:int-to-nvarchar",
            Notes = "SQL Server write compatibility for text normalization.",
            SourceDataTypeId = "sqlserver:type:int",
            TargetDataTypeId = "sqlserver:type:nvarchar"
        };
        model.DataTypeMappingList.Add(record45);
        var record46 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:money",
            SourceDataTypeId = "sqlserver:type:money",
            TargetDataTypeId = "meta:type:Decimal"
        };
        model.DataTypeMappingList.Add(record46);
        var record47 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:Name",
            SourceDataTypeId = "sqlserver:type:Name",
            TargetDataTypeId = "meta:type:String"
        };
        model.DataTypeMappingList.Add(record47);
        var record48 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:Name-to-nvarchar",
            Notes = "SQL Server alias/user-defined type compatibility.",
            SourceDataTypeId = "sqlserver:type:Name",
            TargetDataTypeId = "sqlserver:type:nvarchar"
        };
        model.DataTypeMappingList.Add(record48);
        var record49 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:NameStyle",
            SourceDataTypeId = "sqlserver:type:NameStyle",
            TargetDataTypeId = "meta:type:Boolean"
        };
        model.DataTypeMappingList.Add(record49);
        var record50 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:NameStyle-to-bit",
            Notes = "SQL Server alias/user-defined type compatibility.",
            SourceDataTypeId = "sqlserver:type:NameStyle",
            TargetDataTypeId = "sqlserver:type:bit"
        };
        model.DataTypeMappingList.Add(record50);
        var record51 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:nchar",
            SourceDataTypeId = "sqlserver:type:nchar",
            TargetDataTypeId = "meta:type:StringFixedLength"
        };
        model.DataTypeMappingList.Add(record51);
        var record52 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:numeric",
            SourceDataTypeId = "sqlserver:type:numeric",
            TargetDataTypeId = "meta:type:Decimal"
        };
        model.DataTypeMappingList.Add(record52);
        var record53 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:nvarchar",
            SourceDataTypeId = "sqlserver:type:nvarchar",
            TargetDataTypeId = "meta:type:String"
        };
        model.DataTypeMappingList.Add(record53);
        var record54 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:OrderNumber",
            SourceDataTypeId = "sqlserver:type:OrderNumber",
            TargetDataTypeId = "meta:type:String"
        };
        model.DataTypeMappingList.Add(record54);
        var record55 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:OrderNumber-to-nvarchar",
            Notes = "SQL Server alias/user-defined type compatibility.",
            SourceDataTypeId = "sqlserver:type:OrderNumber",
            TargetDataTypeId = "sqlserver:type:nvarchar"
        };
        model.DataTypeMappingList.Add(record55);
        var record56 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:Phone",
            SourceDataTypeId = "sqlserver:type:Phone",
            TargetDataTypeId = "meta:type:String"
        };
        model.DataTypeMappingList.Add(record56);
        var record57 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:Phone-to-nvarchar",
            Notes = "SQL Server alias/user-defined type compatibility.",
            SourceDataTypeId = "sqlserver:type:Phone",
            TargetDataTypeId = "sqlserver:type:nvarchar"
        };
        model.DataTypeMappingList.Add(record57);
        var record58 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:real",
            SourceDataTypeId = "sqlserver:type:real",
            TargetDataTypeId = "meta:type:Single"
        };
        model.DataTypeMappingList.Add(record58);
        var record59 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:smallint",
            SourceDataTypeId = "sqlserver:type:smallint",
            TargetDataTypeId = "meta:type:Int16"
        };
        model.DataTypeMappingList.Add(record59);
        var record60 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:smallmoney",
            SourceDataTypeId = "sqlserver:type:smallmoney",
            TargetDataTypeId = "meta:type:Decimal"
        };
        model.DataTypeMappingList.Add(record60);
        var record61 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:smallmoney-to-decimal",
            Notes = "SQL Server write compatibility for decimal normalization.",
            SourceDataTypeId = "sqlserver:type:smallmoney",
            TargetDataTypeId = "sqlserver:type:decimal"
        };
        model.DataTypeMappingList.Add(record61);
        var record62 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:sql_variant",
            SourceDataTypeId = "sqlserver:type:sql_variant",
            TargetDataTypeId = "meta:type:Object"
        };
        model.DataTypeMappingList.Add(record62);
        var record63 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:sysname",
            SourceDataTypeId = "sqlserver:type:sysname",
            TargetDataTypeId = "meta:type:String"
        };
        model.DataTypeMappingList.Add(record63);
        var record64 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:sysname-to-nvarchar",
            Notes = "SQL Server alias/user-defined type compatibility.",
            SourceDataTypeId = "sqlserver:type:sysname",
            TargetDataTypeId = "sqlserver:type:nvarchar"
        };
        model.DataTypeMappingList.Add(record64);
        var record65 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:time",
            SourceDataTypeId = "sqlserver:type:time",
            TargetDataTypeId = "meta:type:Time"
        };
        model.DataTypeMappingList.Add(record65);
        var record66 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:tinyint",
            SourceDataTypeId = "sqlserver:type:tinyint",
            TargetDataTypeId = "meta:type:Byte"
        };
        model.DataTypeMappingList.Add(record66);
        var record67 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:uniqueidentifier",
            SourceDataTypeId = "sqlserver:type:uniqueidentifier",
            TargetDataTypeId = "meta:type:Guid"
        };
        model.DataTypeMappingList.Add(record67);
        var record68 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:varbinary",
            SourceDataTypeId = "sqlserver:type:varbinary",
            TargetDataTypeId = "meta:type:Binary"
        };
        model.DataTypeMappingList.Add(record68);
        var record69 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:varchar",
            SourceDataTypeId = "sqlserver:type:varchar",
            TargetDataTypeId = "meta:type:AnsiString"
        };
        model.DataTypeMappingList.Add(record69);
        var record70 = new DataTypeMapping
        {
            Id = "MetaDataTypeConversion:mapping:sqlserver:xml",
            SourceDataTypeId = "sqlserver:type:xml",
            TargetDataTypeId = "meta:type:Xml"
        };
        model.DataTypeMappingList.Add(record70);
        record2.ConversionImplementation = record0;
        record3.ConversionImplementation = record0;
        record4.ConversionImplementation = record0;
        record5.ConversionImplementation = record0;
        record6.ConversionImplementation = record0;
        record7.ConversionImplementation = record0;
        record8.ConversionImplementation = record0;
        record9.ConversionImplementation = record0;
        record10.ConversionImplementation = record0;
        record11.ConversionImplementation = record0;
        record12.ConversionImplementation = record0;
        record13.ConversionImplementation = record1;
        record14.ConversionImplementation = record1;
        record15.ConversionImplementation = record0;
        record16.ConversionImplementation = record1;
        record17.ConversionImplementation = record0;
        record18.ConversionImplementation = record0;
        record19.ConversionImplementation = record0;
        record20.ConversionImplementation = record1;
        record21.ConversionImplementation = record0;
        record22.ConversionImplementation = record0;
        record23.ConversionImplementation = record0;
        record24.ConversionImplementation = record0;
        record25.ConversionImplementation = record1;
        record26.ConversionImplementation = record0;
        record27.ConversionImplementation = record0;
        record28.ConversionImplementation = record0;
        record29.ConversionImplementation = record0;
        record30.ConversionImplementation = record0;
        record31.ConversionImplementation = record0;
        record32.ConversionImplementation = record0;
        record33.ConversionImplementation = record0;
        record34.ConversionImplementation = record0;
        record35.ConversionImplementation = record0;
        record36.ConversionImplementation = record0;
        record37.ConversionImplementation = record0;
        record38.ConversionImplementation = record0;
        record39.ConversionImplementation = record0;
        record40.ConversionImplementation = record0;
        record41.ConversionImplementation = record1;
        record42.ConversionImplementation = record1;
        record43.ConversionImplementation = record1;
        record44.ConversionImplementation = record0;
        record45.ConversionImplementation = record0;
        record46.ConversionImplementation = record0;
        record47.ConversionImplementation = record0;
        record48.ConversionImplementation = record0;
        record49.ConversionImplementation = record0;
        record50.ConversionImplementation = record0;
        record51.ConversionImplementation = record0;
        record52.ConversionImplementation = record0;
        record53.ConversionImplementation = record0;
        record54.ConversionImplementation = record0;
        record55.ConversionImplementation = record0;
        record56.ConversionImplementation = record0;
        record57.ConversionImplementation = record0;
        record58.ConversionImplementation = record0;
        record59.ConversionImplementation = record0;
        record60.ConversionImplementation = record0;
        record61.ConversionImplementation = record0;
        record62.ConversionImplementation = record1;
        record63.ConversionImplementation = record0;
        record64.ConversionImplementation = record0;
        record65.ConversionImplementation = record0;
        record66.ConversionImplementation = record0;
        record67.ConversionImplementation = record0;
        record68.ConversionImplementation = record0;
        record69.ConversionImplementation = record0;
        record70.ConversionImplementation = record1;
        return model;
    }
}