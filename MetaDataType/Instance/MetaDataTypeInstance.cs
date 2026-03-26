using MetaDataType;

namespace MetaDataType.Instance;

public static class MetaDataTypeInstance
{
    private static MetaDataTypeModel CreateDefault()
    {
        var model = MetaDataTypeModel.CreateEmpty();

        foreach (var system in DataTypeSystems)
        {
            model.DataTypeSystemList.Add(new DataTypeSystem
            {
                Id = BuildDataTypeSystemId(system.Name),
                Name = system.Name,
                Description = system.Description ?? string.Empty,
            });
        }

        var dataTypeSystemsById = model.DataTypeSystemList.ToDictionary(row => row.Id, StringComparer.Ordinal);

        foreach (var dataType in DataTypes)
        {
            var dataTypeSystemId = BuildDataTypeSystemId(dataType.DataTypeSystem);
            model.DataTypeList.Add(new DataType
            {
                Id = BuildDataTypeId(dataType.DataTypeSystem, dataType.Name),
                Name = dataType.Name,
                Category = dataType.Category ?? string.Empty,
                Description = dataType.Description ?? string.Empty,
                IsCanonical = dataType.IsCanonical ?? string.Empty,
                DataTypeSystemId = dataTypeSystemId,
                DataTypeSystem = dataTypeSystemsById[dataTypeSystemId],
            });
        }

        return model;
    }

    private readonly record struct DataTypeSystemSeed(string Name, string? Description = null);
    private readonly record struct DataTypeSeed(string DataTypeSystem, string Name, string? Category, string? IsCanonical = null, string? Description = null);

    private static readonly DataTypeSystemSeed[] DataTypeSystems =
    [
        new("CSharp"),
        new("Meta"),
        new("Snowflake"),
        new("SqlServer"),
        new("SSIS"),
        new("Synapse")
    ];

    private static readonly DataTypeSeed[] DataTypes =
    [
        new("CSharp", "bool", "Logical"),
        new("CSharp", "byte[]", "Binary"),
        new("CSharp", "DateOnly", "Temporal"),
        new("CSharp", "DateTime", "Temporal"),
        new("CSharp", "DateTimeOffset", "Temporal"),
        new("CSharp", "decimal", "Numeric"),
        new("CSharp", "double", "Numeric"),
        new("CSharp", "Guid", "Identifier"),
        new("CSharp", "int", "Numeric"),
        new("CSharp", "long", "Numeric"),
        new("CSharp", "object", "Structured"),
        new("CSharp", "string", "Text"),
        new("Meta", "AnsiString", "Text", "true"),
        new("Meta", "AnsiStringFixedLength", "Text", "true"),
        new("Meta", "Binary", "Binary", "true"),
        new("Meta", "Boolean", "Logical", "true"),
        new("Meta", "Byte", "Numeric", "true"),
        new("Meta", "Date", "Temporal", "true"),
        new("Meta", "DateTime", "Temporal", "true"),
        new("Meta", "DateTime2", "Temporal", "true"),
        new("Meta", "DateTimeOffset", "Temporal", "true"),
        new("Meta", "Decimal", "Numeric", "true"),
        new("Meta", "Double", "Numeric", "true"),
        new("Meta", "geography", "Spatial", "true"),
        new("Meta", "geometry", "Spatial", "true"),
        new("Meta", "Guid", "Identifier", "true"),
        new("Meta", "hierarchyid", "Spatial", "true"),
        new("Meta", "Int16", "Numeric", "true"),
        new("Meta", "Int32", "Numeric", "true"),
        new("Meta", "Int64", "Numeric", "true"),
        new("Meta", "Object", "Structured", "true"),
        new("Meta", "SByte", "Numeric", "true"),
        new("Meta", "Single", "Numeric", "true"),
        new("Meta", "String", "Text", "true"),
        new("Meta", "StringFixedLength", "Text", "true"),
        new("Meta", "Time", "Temporal", "true"),
        new("Meta", "UInt16", "Numeric", "true"),
        new("Meta", "UInt32", "Numeric", "true"),
        new("Meta", "UInt64", "Numeric", "true"),
        new("Meta", "VarNumeric", "Numeric", "true"),
        new("Meta", "Xml", "Structured", "true"),
        new("Snowflake", "binary", "Binary"),
        new("Snowflake", "boolean", "Logical"),
        new("Snowflake", "date", "Temporal"),
        new("Snowflake", "float", "Numeric"),
        new("Snowflake", "number", "Numeric"),
        new("Snowflake", "timestamp_ntz", "Temporal"),
        new("Snowflake", "timestamp_tz", "Temporal"),
        new("Snowflake", "varchar", "Text"),
        new("Snowflake", "variant", "Structured"),
        new("SqlServer", "binary", "Binary"),
        new("SqlServer", "bigint", "Numeric"),
        new("SqlServer", "bit", "Logical"),
        new("SqlServer", "char", "Text"),
        new("SqlServer", "date", "Temporal"),
        new("SqlServer", "datetime", "Temporal"),
        new("SqlServer", "datetime2", "Temporal"),
        new("SqlServer", "datetimeoffset", "Temporal"),
        new("SqlServer", "decimal", "Numeric"),
        new("SqlServer", "float", "Numeric"),
        new("SqlServer", "geography", "Spatial"),
        new("SqlServer", "geometry", "Spatial"),
        new("SqlServer", "hierarchyid", "Spatial"),
        new("SqlServer", "int", "Numeric"),
        new("SqlServer", "money", "Numeric"),
        new("SqlServer", "nchar", "Text"),
        new("SqlServer", "nvarchar", "Text"),
        new("SqlServer", "smallint", "Numeric"),
        new("SqlServer", "smallmoney", "Numeric"),
        new("SqlServer", "sql_variant", "Structured"),
        new("SqlServer", "time", "Temporal"),
        new("SqlServer", "tinyint", "Numeric"),
        new("SqlServer", "uniqueidentifier", "Identifier"),
        new("SqlServer", "varbinary", "Binary"),
        new("SqlServer", "varchar", "Text"),
        new("SqlServer", "xml", "Structured"),
        new("SSIS", "DT_BOOL", "Logical"),
        new("SSIS", "DT_BYTES", "Binary"),
        new("SSIS", "DT_CY", "Numeric"),
        new("SSIS", "DT_DBDATE", "Temporal"),
        new("SSIS", "DT_DBTIME2", "Temporal"),
        new("SSIS", "DT_DBTIMESTAMP", "Temporal"),
        new("SSIS", "DT_DBTIMESTAMP2", "Temporal"),
        new("SSIS", "DT_DBTIMESTAMPOFFSET", "Temporal"),
        new("SSIS", "DT_GUID", "Identifier"),
        new("SSIS", "DT_I2", "Numeric"),
        new("SSIS", "DT_I4", "Numeric"),
        new("SSIS", "DT_I8", "Numeric"),
        new("SSIS", "DT_IMAGE", "Binary"),
        new("SSIS", "DT_NTEXT", "Structured"),
        new("SSIS", "DT_NUMERIC", "Numeric"),
        new("SSIS", "DT_R4", "Numeric"),
        new("SSIS", "DT_R8", "Numeric"),
        new("SSIS", "DT_STR", "Text"),
        new("SSIS", "DT_TEXT", "Text"),
        new("SSIS", "DT_UI1", "Numeric"),
        new("SSIS", "DT_WSTR", "Text"),
        new("Synapse", "bigint", "Numeric"),
        new("Synapse", "bit", "Logical"),
        new("Synapse", "date", "Temporal"),
        new("Synapse", "datetime2", "Temporal"),
        new("Synapse", "datetimeoffset", "Temporal"),
        new("Synapse", "decimal", "Numeric"),
        new("Synapse", "float", "Numeric"),
        new("Synapse", "int", "Numeric"),
        new("Synapse", "varbinary", "Binary"),
        new("Synapse", "varchar", "Text")
    ];

    public static MetaDataTypeModel Default { get; } = CreateDefault();

    public static string BuildDataTypeSystemId(string dataTypeSystemName)
    {
        return dataTypeSystemName.Trim();
    }

    public static string BuildDataTypeId(string dataTypeSystemName, string dataTypeName)
    {
        return NormalizeKey(dataTypeSystemName) + ":type:" + dataTypeName;
    }

    private static string NormalizeKey(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}
