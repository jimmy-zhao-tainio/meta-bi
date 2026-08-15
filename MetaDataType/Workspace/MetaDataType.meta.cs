#nullable enable
using System;
using System.Collections.Generic;

namespace MetaDataType;
public sealed partial class DataType
{
    public string Id { get; set; } = null !;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? IsCanonical { get; set; }
    public string Name { get; set; } = null !;
    public DataTypeSystem DataTypeSystem { get; set; } = null !;
}

public sealed partial class DataTypeSystem
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class MetaDataTypeModel
{
    public static MetaDataTypeModel CreateEmpty() => new();
    public List<DataType> DataTypeList { get; set; } = new();
    public List<DataTypeSystem> DataTypeSystemList { get; set; } = new();
}

public static partial class MetaDataTypeInstance
{
    private static readonly MetaDataTypeModel _builtIn = CreateBuiltIn();
    public static MetaDataTypeModel BuiltIn => _builtIn;

    public static MetaDataTypeModel CreateBuiltIn()
    {
        var model = MetaDataTypeModel.CreateEmpty();
        var record0 = new DataType
        {
            Id = "csharp:type:bool",
            Category = "Logical",
            Name = "bool"
        };
        model.DataTypeList.Add(record0);
        var record1 = new DataType
        {
            Id = "csharp:type:byte[]",
            Category = "Binary",
            Name = "byte[]"
        };
        model.DataTypeList.Add(record1);
        var record2 = new DataType
        {
            Id = "csharp:type:DateOnly",
            Category = "Temporal",
            Name = "DateOnly"
        };
        model.DataTypeList.Add(record2);
        var record3 = new DataType
        {
            Id = "csharp:type:DateTime",
            Category = "Temporal",
            Name = "DateTime"
        };
        model.DataTypeList.Add(record3);
        var record4 = new DataType
        {
            Id = "csharp:type:DateTimeOffset",
            Category = "Temporal",
            Name = "DateTimeOffset"
        };
        model.DataTypeList.Add(record4);
        var record5 = new DataType
        {
            Id = "csharp:type:decimal",
            Category = "Numeric",
            Name = "decimal"
        };
        model.DataTypeList.Add(record5);
        var record6 = new DataType
        {
            Id = "csharp:type:double",
            Category = "Numeric",
            Name = "double"
        };
        model.DataTypeList.Add(record6);
        var record7 = new DataType
        {
            Id = "csharp:type:Guid",
            Category = "Identifier",
            Name = "Guid"
        };
        model.DataTypeList.Add(record7);
        var record8 = new DataType
        {
            Id = "csharp:type:int",
            Category = "Numeric",
            Name = "int"
        };
        model.DataTypeList.Add(record8);
        var record9 = new DataType
        {
            Id = "csharp:type:long",
            Category = "Numeric",
            Name = "long"
        };
        model.DataTypeList.Add(record9);
        var record10 = new DataType
        {
            Id = "csharp:type:object",
            Category = "Structured",
            Name = "object"
        };
        model.DataTypeList.Add(record10);
        var record11 = new DataType
        {
            Id = "csharp:type:string",
            Category = "Text",
            Name = "string"
        };
        model.DataTypeList.Add(record11);
        var record12 = new DataType
        {
            Id = "meta:type:AnsiString",
            Category = "Text",
            IsCanonical = "true",
            Name = "AnsiString"
        };
        model.DataTypeList.Add(record12);
        var record13 = new DataType
        {
            Id = "meta:type:AnsiStringFixedLength",
            Category = "Text",
            IsCanonical = "true",
            Name = "AnsiStringFixedLength"
        };
        model.DataTypeList.Add(record13);
        var record14 = new DataType
        {
            Id = "meta:type:Binary",
            Category = "Binary",
            IsCanonical = "true",
            Name = "Binary"
        };
        model.DataTypeList.Add(record14);
        var record15 = new DataType
        {
            Id = "meta:type:Boolean",
            Category = "Logical",
            IsCanonical = "true",
            Name = "Boolean"
        };
        model.DataTypeList.Add(record15);
        var record16 = new DataType
        {
            Id = "meta:type:Byte",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "Byte"
        };
        model.DataTypeList.Add(record16);
        var record17 = new DataType
        {
            Id = "meta:type:Date",
            Category = "Temporal",
            IsCanonical = "true",
            Name = "Date"
        };
        model.DataTypeList.Add(record17);
        var record18 = new DataType
        {
            Id = "meta:type:DateTime",
            Category = "Temporal",
            IsCanonical = "true",
            Name = "DateTime"
        };
        model.DataTypeList.Add(record18);
        var record19 = new DataType
        {
            Id = "meta:type:DateTime2",
            Category = "Temporal",
            IsCanonical = "true",
            Name = "DateTime2"
        };
        model.DataTypeList.Add(record19);
        var record20 = new DataType
        {
            Id = "meta:type:DateTimeOffset",
            Category = "Temporal",
            IsCanonical = "true",
            Name = "DateTimeOffset"
        };
        model.DataTypeList.Add(record20);
        var record21 = new DataType
        {
            Id = "meta:type:Decimal",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "Decimal"
        };
        model.DataTypeList.Add(record21);
        var record22 = new DataType
        {
            Id = "meta:type:Double",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "Double"
        };
        model.DataTypeList.Add(record22);
        var record23 = new DataType
        {
            Id = "meta:type:geography",
            Category = "Spatial",
            IsCanonical = "true",
            Name = "geography"
        };
        model.DataTypeList.Add(record23);
        var record24 = new DataType
        {
            Id = "meta:type:geometry",
            Category = "Spatial",
            IsCanonical = "true",
            Name = "geometry"
        };
        model.DataTypeList.Add(record24);
        var record25 = new DataType
        {
            Id = "meta:type:Guid",
            Category = "Identifier",
            IsCanonical = "true",
            Name = "Guid"
        };
        model.DataTypeList.Add(record25);
        var record26 = new DataType
        {
            Id = "meta:type:hierarchyid",
            Category = "Spatial",
            IsCanonical = "true",
            Name = "hierarchyid"
        };
        model.DataTypeList.Add(record26);
        var record27 = new DataType
        {
            Id = "meta:type:Int16",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "Int16"
        };
        model.DataTypeList.Add(record27);
        var record28 = new DataType
        {
            Id = "meta:type:Int32",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "Int32"
        };
        model.DataTypeList.Add(record28);
        var record29 = new DataType
        {
            Id = "meta:type:Int64",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "Int64"
        };
        model.DataTypeList.Add(record29);
        var record30 = new DataType
        {
            Id = "meta:type:Object",
            Category = "Structured",
            IsCanonical = "true",
            Name = "Object"
        };
        model.DataTypeList.Add(record30);
        var record31 = new DataType
        {
            Id = "meta:type:SByte",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "SByte"
        };
        model.DataTypeList.Add(record31);
        var record32 = new DataType
        {
            Id = "meta:type:Single",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "Single"
        };
        model.DataTypeList.Add(record32);
        var record33 = new DataType
        {
            Id = "meta:type:String",
            Category = "Text",
            IsCanonical = "true",
            Name = "String"
        };
        model.DataTypeList.Add(record33);
        var record34 = new DataType
        {
            Id = "meta:type:StringFixedLength",
            Category = "Text",
            IsCanonical = "true",
            Name = "StringFixedLength"
        };
        model.DataTypeList.Add(record34);
        var record35 = new DataType
        {
            Id = "meta:type:Time",
            Category = "Temporal",
            IsCanonical = "true",
            Name = "Time"
        };
        model.DataTypeList.Add(record35);
        var record36 = new DataType
        {
            Id = "meta:type:UInt16",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "UInt16"
        };
        model.DataTypeList.Add(record36);
        var record37 = new DataType
        {
            Id = "meta:type:UInt32",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "UInt32"
        };
        model.DataTypeList.Add(record37);
        var record38 = new DataType
        {
            Id = "meta:type:UInt64",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "UInt64"
        };
        model.DataTypeList.Add(record38);
        var record39 = new DataType
        {
            Id = "meta:type:VarNumeric",
            Category = "Numeric",
            IsCanonical = "true",
            Name = "VarNumeric"
        };
        model.DataTypeList.Add(record39);
        var record40 = new DataType
        {
            Id = "meta:type:Xml",
            Category = "Structured",
            IsCanonical = "true",
            Name = "Xml"
        };
        model.DataTypeList.Add(record40);
        var record41 = new DataType
        {
            Id = "snowflake:type:binary",
            Category = "Binary",
            Name = "binary"
        };
        model.DataTypeList.Add(record41);
        var record42 = new DataType
        {
            Id = "snowflake:type:boolean",
            Category = "Logical",
            Name = "boolean"
        };
        model.DataTypeList.Add(record42);
        var record43 = new DataType
        {
            Id = "snowflake:type:date",
            Category = "Temporal",
            Name = "date"
        };
        model.DataTypeList.Add(record43);
        var record44 = new DataType
        {
            Id = "snowflake:type:float",
            Category = "Numeric",
            Name = "float"
        };
        model.DataTypeList.Add(record44);
        var record45 = new DataType
        {
            Id = "snowflake:type:number",
            Category = "Numeric",
            Name = "number"
        };
        model.DataTypeList.Add(record45);
        var record46 = new DataType
        {
            Id = "snowflake:type:timestamp_ntz",
            Category = "Temporal",
            Name = "timestamp_ntz"
        };
        model.DataTypeList.Add(record46);
        var record47 = new DataType
        {
            Id = "snowflake:type:timestamp_tz",
            Category = "Temporal",
            Name = "timestamp_tz"
        };
        model.DataTypeList.Add(record47);
        var record48 = new DataType
        {
            Id = "snowflake:type:varchar",
            Category = "Text",
            Name = "varchar"
        };
        model.DataTypeList.Add(record48);
        var record49 = new DataType
        {
            Id = "snowflake:type:variant",
            Category = "Structured",
            Name = "variant"
        };
        model.DataTypeList.Add(record49);
        var record50 = new DataType
        {
            Id = "sqlserver:type:AccountNumber",
            Category = "Text",
            Name = "AccountNumber"
        };
        model.DataTypeList.Add(record50);
        var record51 = new DataType
        {
            Id = "sqlserver:type:bigint",
            Category = "Numeric",
            Name = "bigint"
        };
        model.DataTypeList.Add(record51);
        var record52 = new DataType
        {
            Id = "sqlserver:type:binary",
            Category = "Binary",
            Name = "binary"
        };
        model.DataTypeList.Add(record52);
        var record53 = new DataType
        {
            Id = "sqlserver:type:bit",
            Category = "Logical",
            Name = "bit"
        };
        model.DataTypeList.Add(record53);
        var record54 = new DataType
        {
            Id = "sqlserver:type:char",
            Category = "Text",
            Name = "char"
        };
        model.DataTypeList.Add(record54);
        var record55 = new DataType
        {
            Id = "sqlserver:type:date",
            Category = "Temporal",
            Name = "date"
        };
        model.DataTypeList.Add(record55);
        var record56 = new DataType
        {
            Id = "sqlserver:type:datetime",
            Category = "Temporal",
            Name = "datetime"
        };
        model.DataTypeList.Add(record56);
        var record57 = new DataType
        {
            Id = "sqlserver:type:datetime2",
            Category = "Temporal",
            Name = "datetime2"
        };
        model.DataTypeList.Add(record57);
        var record58 = new DataType
        {
            Id = "sqlserver:type:datetimeoffset",
            Category = "Temporal",
            Name = "datetimeoffset"
        };
        model.DataTypeList.Add(record58);
        var record59 = new DataType
        {
            Id = "sqlserver:type:decimal",
            Category = "Numeric",
            Name = "decimal"
        };
        model.DataTypeList.Add(record59);
        var record60 = new DataType
        {
            Id = "sqlserver:type:Flag",
            Category = "Logical",
            Name = "Flag"
        };
        model.DataTypeList.Add(record60);
        var record61 = new DataType
        {
            Id = "sqlserver:type:float",
            Category = "Numeric",
            Name = "float"
        };
        model.DataTypeList.Add(record61);
        var record62 = new DataType
        {
            Id = "sqlserver:type:geography",
            Category = "Spatial",
            Name = "geography"
        };
        model.DataTypeList.Add(record62);
        var record63 = new DataType
        {
            Id = "sqlserver:type:geometry",
            Category = "Spatial",
            Name = "geometry"
        };
        model.DataTypeList.Add(record63);
        var record64 = new DataType
        {
            Id = "sqlserver:type:hierarchyid",
            Category = "Spatial",
            Name = "hierarchyid"
        };
        model.DataTypeList.Add(record64);
        var record65 = new DataType
        {
            Id = "sqlserver:type:int",
            Category = "Numeric",
            Name = "int"
        };
        model.DataTypeList.Add(record65);
        var record66 = new DataType
        {
            Id = "sqlserver:type:money",
            Category = "Numeric",
            Name = "money"
        };
        model.DataTypeList.Add(record66);
        var record67 = new DataType
        {
            Id = "sqlserver:type:Name",
            Category = "Text",
            Name = "Name"
        };
        model.DataTypeList.Add(record67);
        var record68 = new DataType
        {
            Id = "sqlserver:type:NameStyle",
            Category = "Logical",
            Name = "NameStyle"
        };
        model.DataTypeList.Add(record68);
        var record69 = new DataType
        {
            Id = "sqlserver:type:nchar",
            Category = "Text",
            Name = "nchar"
        };
        model.DataTypeList.Add(record69);
        var record70 = new DataType
        {
            Id = "sqlserver:type:numeric",
            Category = "Numeric",
            Name = "numeric"
        };
        model.DataTypeList.Add(record70);
        var record71 = new DataType
        {
            Id = "sqlserver:type:nvarchar",
            Category = "Text",
            Name = "nvarchar"
        };
        model.DataTypeList.Add(record71);
        var record72 = new DataType
        {
            Id = "sqlserver:type:OrderNumber",
            Category = "Text",
            Name = "OrderNumber"
        };
        model.DataTypeList.Add(record72);
        var record73 = new DataType
        {
            Id = "sqlserver:type:Phone",
            Category = "Text",
            Name = "Phone"
        };
        model.DataTypeList.Add(record73);
        var record74 = new DataType
        {
            Id = "sqlserver:type:real",
            Category = "Numeric",
            Name = "real"
        };
        model.DataTypeList.Add(record74);
        var record75 = new DataType
        {
            Id = "sqlserver:type:smallint",
            Category = "Numeric",
            Name = "smallint"
        };
        model.DataTypeList.Add(record75);
        var record76 = new DataType
        {
            Id = "sqlserver:type:smallmoney",
            Category = "Numeric",
            Name = "smallmoney"
        };
        model.DataTypeList.Add(record76);
        var record77 = new DataType
        {
            Id = "sqlserver:type:sql_variant",
            Category = "Structured",
            Name = "sql_variant"
        };
        model.DataTypeList.Add(record77);
        var record78 = new DataType
        {
            Id = "sqlserver:type:sysname",
            Category = "Text",
            Name = "sysname"
        };
        model.DataTypeList.Add(record78);
        var record79 = new DataType
        {
            Id = "sqlserver:type:time",
            Category = "Temporal",
            Name = "time"
        };
        model.DataTypeList.Add(record79);
        var record80 = new DataType
        {
            Id = "sqlserver:type:tinyint",
            Category = "Numeric",
            Name = "tinyint"
        };
        model.DataTypeList.Add(record80);
        var record81 = new DataType
        {
            Id = "sqlserver:type:uniqueidentifier",
            Category = "Identifier",
            Name = "uniqueidentifier"
        };
        model.DataTypeList.Add(record81);
        var record82 = new DataType
        {
            Id = "sqlserver:type:varbinary",
            Category = "Binary",
            Name = "varbinary"
        };
        model.DataTypeList.Add(record82);
        var record83 = new DataType
        {
            Id = "sqlserver:type:varchar",
            Category = "Text",
            Name = "varchar"
        };
        model.DataTypeList.Add(record83);
        var record84 = new DataType
        {
            Id = "sqlserver:type:xml",
            Category = "Structured",
            Name = "xml"
        };
        model.DataTypeList.Add(record84);
        var record85 = new DataType
        {
            Id = "ssis:type:DT_BOOL",
            Category = "Logical",
            Name = "DT_BOOL"
        };
        model.DataTypeList.Add(record85);
        var record86 = new DataType
        {
            Id = "ssis:type:DT_BYTES",
            Category = "Binary",
            Name = "DT_BYTES"
        };
        model.DataTypeList.Add(record86);
        var record87 = new DataType
        {
            Id = "ssis:type:DT_CY",
            Category = "Numeric",
            Name = "DT_CY"
        };
        model.DataTypeList.Add(record87);
        var record88 = new DataType
        {
            Id = "ssis:type:DT_DBDATE",
            Category = "Temporal",
            Name = "DT_DBDATE"
        };
        model.DataTypeList.Add(record88);
        var record89 = new DataType
        {
            Id = "ssis:type:DT_DBTIME2",
            Category = "Temporal",
            Name = "DT_DBTIME2"
        };
        model.DataTypeList.Add(record89);
        var record90 = new DataType
        {
            Id = "ssis:type:DT_DBTIMESTAMP",
            Category = "Temporal",
            Name = "DT_DBTIMESTAMP"
        };
        model.DataTypeList.Add(record90);
        var record91 = new DataType
        {
            Id = "ssis:type:DT_DBTIMESTAMP2",
            Category = "Temporal",
            Name = "DT_DBTIMESTAMP2"
        };
        model.DataTypeList.Add(record91);
        var record92 = new DataType
        {
            Id = "ssis:type:DT_DBTIMESTAMPOFFSET",
            Category = "Temporal",
            Name = "DT_DBTIMESTAMPOFFSET"
        };
        model.DataTypeList.Add(record92);
        var record93 = new DataType
        {
            Id = "ssis:type:DT_GUID",
            Category = "Identifier",
            Name = "DT_GUID"
        };
        model.DataTypeList.Add(record93);
        var record94 = new DataType
        {
            Id = "ssis:type:DT_I2",
            Category = "Numeric",
            Name = "DT_I2"
        };
        model.DataTypeList.Add(record94);
        var record95 = new DataType
        {
            Id = "ssis:type:DT_I4",
            Category = "Numeric",
            Name = "DT_I4"
        };
        model.DataTypeList.Add(record95);
        var record96 = new DataType
        {
            Id = "ssis:type:DT_I8",
            Category = "Numeric",
            Name = "DT_I8"
        };
        model.DataTypeList.Add(record96);
        var record97 = new DataType
        {
            Id = "ssis:type:DT_IMAGE",
            Category = "Binary",
            Name = "DT_IMAGE"
        };
        model.DataTypeList.Add(record97);
        var record98 = new DataType
        {
            Id = "ssis:type:DT_NTEXT",
            Category = "Structured",
            Name = "DT_NTEXT"
        };
        model.DataTypeList.Add(record98);
        var record99 = new DataType
        {
            Id = "ssis:type:DT_NUMERIC",
            Category = "Numeric",
            Name = "DT_NUMERIC"
        };
        model.DataTypeList.Add(record99);
        var record100 = new DataType
        {
            Id = "ssis:type:DT_R4",
            Category = "Numeric",
            Name = "DT_R4"
        };
        model.DataTypeList.Add(record100);
        var record101 = new DataType
        {
            Id = "ssis:type:DT_R8",
            Category = "Numeric",
            Name = "DT_R8"
        };
        model.DataTypeList.Add(record101);
        var record102 = new DataType
        {
            Id = "ssis:type:DT_STR",
            Category = "Text",
            Name = "DT_STR"
        };
        model.DataTypeList.Add(record102);
        var record103 = new DataType
        {
            Id = "ssis:type:DT_TEXT",
            Category = "Text",
            Name = "DT_TEXT"
        };
        model.DataTypeList.Add(record103);
        var record104 = new DataType
        {
            Id = "ssis:type:DT_UI1",
            Category = "Numeric",
            Name = "DT_UI1"
        };
        model.DataTypeList.Add(record104);
        var record105 = new DataType
        {
            Id = "ssis:type:DT_WSTR",
            Category = "Text",
            Name = "DT_WSTR"
        };
        model.DataTypeList.Add(record105);
        var record106 = new DataType
        {
            Id = "synapse:type:bigint",
            Category = "Numeric",
            Name = "bigint"
        };
        model.DataTypeList.Add(record106);
        var record107 = new DataType
        {
            Id = "synapse:type:bit",
            Category = "Logical",
            Name = "bit"
        };
        model.DataTypeList.Add(record107);
        var record108 = new DataType
        {
            Id = "synapse:type:date",
            Category = "Temporal",
            Name = "date"
        };
        model.DataTypeList.Add(record108);
        var record109 = new DataType
        {
            Id = "synapse:type:datetime2",
            Category = "Temporal",
            Name = "datetime2"
        };
        model.DataTypeList.Add(record109);
        var record110 = new DataType
        {
            Id = "synapse:type:datetimeoffset",
            Category = "Temporal",
            Name = "datetimeoffset"
        };
        model.DataTypeList.Add(record110);
        var record111 = new DataType
        {
            Id = "synapse:type:decimal",
            Category = "Numeric",
            Name = "decimal"
        };
        model.DataTypeList.Add(record111);
        var record112 = new DataType
        {
            Id = "synapse:type:float",
            Category = "Numeric",
            Name = "float"
        };
        model.DataTypeList.Add(record112);
        var record113 = new DataType
        {
            Id = "synapse:type:int",
            Category = "Numeric",
            Name = "int"
        };
        model.DataTypeList.Add(record113);
        var record114 = new DataType
        {
            Id = "synapse:type:varbinary",
            Category = "Binary",
            Name = "varbinary"
        };
        model.DataTypeList.Add(record114);
        var record115 = new DataType
        {
            Id = "synapse:type:varchar",
            Category = "Text",
            Name = "varchar"
        };
        model.DataTypeList.Add(record115);
        var record116 = new DataTypeSystem
        {
            Id = "CSharp",
            Name = "CSharp"
        };
        model.DataTypeSystemList.Add(record116);
        var record117 = new DataTypeSystem
        {
            Id = "Meta",
            Name = "Meta"
        };
        model.DataTypeSystemList.Add(record117);
        var record118 = new DataTypeSystem
        {
            Id = "Snowflake",
            Name = "Snowflake"
        };
        model.DataTypeSystemList.Add(record118);
        var record119 = new DataTypeSystem
        {
            Id = "SqlServer",
            Name = "SqlServer"
        };
        model.DataTypeSystemList.Add(record119);
        var record120 = new DataTypeSystem
        {
            Id = "SSIS",
            Name = "SSIS"
        };
        model.DataTypeSystemList.Add(record120);
        var record121 = new DataTypeSystem
        {
            Id = "Synapse",
            Name = "Synapse"
        };
        model.DataTypeSystemList.Add(record121);
        record0.DataTypeSystem = record116;
        record1.DataTypeSystem = record116;
        record2.DataTypeSystem = record116;
        record3.DataTypeSystem = record116;
        record4.DataTypeSystem = record116;
        record5.DataTypeSystem = record116;
        record6.DataTypeSystem = record116;
        record7.DataTypeSystem = record116;
        record8.DataTypeSystem = record116;
        record9.DataTypeSystem = record116;
        record10.DataTypeSystem = record116;
        record11.DataTypeSystem = record116;
        record12.DataTypeSystem = record117;
        record13.DataTypeSystem = record117;
        record14.DataTypeSystem = record117;
        record15.DataTypeSystem = record117;
        record16.DataTypeSystem = record117;
        record17.DataTypeSystem = record117;
        record18.DataTypeSystem = record117;
        record19.DataTypeSystem = record117;
        record20.DataTypeSystem = record117;
        record21.DataTypeSystem = record117;
        record22.DataTypeSystem = record117;
        record23.DataTypeSystem = record117;
        record24.DataTypeSystem = record117;
        record25.DataTypeSystem = record117;
        record26.DataTypeSystem = record117;
        record27.DataTypeSystem = record117;
        record28.DataTypeSystem = record117;
        record29.DataTypeSystem = record117;
        record30.DataTypeSystem = record117;
        record31.DataTypeSystem = record117;
        record32.DataTypeSystem = record117;
        record33.DataTypeSystem = record117;
        record34.DataTypeSystem = record117;
        record35.DataTypeSystem = record117;
        record36.DataTypeSystem = record117;
        record37.DataTypeSystem = record117;
        record38.DataTypeSystem = record117;
        record39.DataTypeSystem = record117;
        record40.DataTypeSystem = record117;
        record41.DataTypeSystem = record118;
        record42.DataTypeSystem = record118;
        record43.DataTypeSystem = record118;
        record44.DataTypeSystem = record118;
        record45.DataTypeSystem = record118;
        record46.DataTypeSystem = record118;
        record47.DataTypeSystem = record118;
        record48.DataTypeSystem = record118;
        record49.DataTypeSystem = record118;
        record50.DataTypeSystem = record119;
        record51.DataTypeSystem = record119;
        record52.DataTypeSystem = record119;
        record53.DataTypeSystem = record119;
        record54.DataTypeSystem = record119;
        record55.DataTypeSystem = record119;
        record56.DataTypeSystem = record119;
        record57.DataTypeSystem = record119;
        record58.DataTypeSystem = record119;
        record59.DataTypeSystem = record119;
        record60.DataTypeSystem = record119;
        record61.DataTypeSystem = record119;
        record62.DataTypeSystem = record119;
        record63.DataTypeSystem = record119;
        record64.DataTypeSystem = record119;
        record65.DataTypeSystem = record119;
        record66.DataTypeSystem = record119;
        record67.DataTypeSystem = record119;
        record68.DataTypeSystem = record119;
        record69.DataTypeSystem = record119;
        record70.DataTypeSystem = record119;
        record71.DataTypeSystem = record119;
        record72.DataTypeSystem = record119;
        record73.DataTypeSystem = record119;
        record74.DataTypeSystem = record119;
        record75.DataTypeSystem = record119;
        record76.DataTypeSystem = record119;
        record77.DataTypeSystem = record119;
        record78.DataTypeSystem = record119;
        record79.DataTypeSystem = record119;
        record80.DataTypeSystem = record119;
        record81.DataTypeSystem = record119;
        record82.DataTypeSystem = record119;
        record83.DataTypeSystem = record119;
        record84.DataTypeSystem = record119;
        record85.DataTypeSystem = record120;
        record86.DataTypeSystem = record120;
        record87.DataTypeSystem = record120;
        record88.DataTypeSystem = record120;
        record89.DataTypeSystem = record120;
        record90.DataTypeSystem = record120;
        record91.DataTypeSystem = record120;
        record92.DataTypeSystem = record120;
        record93.DataTypeSystem = record120;
        record94.DataTypeSystem = record120;
        record95.DataTypeSystem = record120;
        record96.DataTypeSystem = record120;
        record97.DataTypeSystem = record120;
        record98.DataTypeSystem = record120;
        record99.DataTypeSystem = record120;
        record100.DataTypeSystem = record120;
        record101.DataTypeSystem = record120;
        record102.DataTypeSystem = record120;
        record103.DataTypeSystem = record120;
        record104.DataTypeSystem = record120;
        record105.DataTypeSystem = record120;
        record106.DataTypeSystem = record121;
        record107.DataTypeSystem = record121;
        record108.DataTypeSystem = record121;
        record109.DataTypeSystem = record121;
        record110.DataTypeSystem = record121;
        record111.DataTypeSystem = record121;
        record112.DataTypeSystem = record121;
        record113.DataTypeSystem = record121;
        record114.DataTypeSystem = record121;
        record115.DataTypeSystem = record121;
        return model;
    }
}