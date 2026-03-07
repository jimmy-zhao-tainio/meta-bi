using Meta.Core.Domain;

namespace MetaDataType.Core;

internal static class MetaDataTypeSeed
{
    public static void Populate(GenericInstance instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        foreach (var dataTypeSystemName in MetaDataTypeSeedData.DataTypeSystems)
        {
            instance.GetOrCreateEntityRecords("DataTypeSystem").Add(new GenericRecord
            {
                Id = BuildDataTypeSystemId(dataTypeSystemName),
                Values = { ["Name"] = dataTypeSystemName }
            });
        }

        foreach (var dataType in MetaDataTypeSeedData.DataTypes)
        {
            var record = new GenericRecord
            {
                Id = BuildDataTypeId(dataType.DataTypeSystem, dataType.Name),
                RelationshipIds =
                {
                    ["DataTypeSystemId"] = BuildDataTypeSystemId(dataType.DataTypeSystem)
                }
            };
            record.Values["Name"] = dataType.Name;
            if (!string.IsNullOrWhiteSpace(dataType.Category))
            {
                record.Values["Category"] = dataType.Category;
            }

            if (string.Equals(dataType.DataTypeSystem, "Meta", StringComparison.Ordinal))
            {
                record.Values["IsCanonical"] = "true";
            }

            instance.GetOrCreateEntityRecords("DataType").Add(record);
        }
    }

    internal static string BuildDataTypeSystemId(string dataTypeSystemName)
    {
        return NormalizeKey(dataTypeSystemName) + ":type-system";
    }

    internal static string BuildDataTypeId(string dataTypeSystemName, string dataTypeName)
    {
        return NormalizeKey(dataTypeSystemName) + ":type:" + dataTypeName;
    }

    private static string NormalizeKey(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}