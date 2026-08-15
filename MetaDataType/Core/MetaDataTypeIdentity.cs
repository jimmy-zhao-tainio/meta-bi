namespace MetaDataType.Core;

public static class MetaDataTypeIdentity
{
    public static string BuildDataTypeSystemId(string dataTypeSystemName)
    {
        return dataTypeSystemName.Trim();
    }

    public static string BuildDataTypeId(string dataTypeSystemName, string dataTypeName)
    {
        return dataTypeSystemName.Trim().ToLowerInvariant() + ":type:" + dataTypeName;
    }
}
