using MetaDataType;
using MetaDataTypeConversion;

namespace MetaConvert.DataVaultToSql;

internal sealed class SqlServerBusinessTypeLowering
{
    private const string DirectConversionImplementationId = "MetaDataTypeConversion:implementation:direct";
    private const string MetaTypeSystemId = "Meta";
    private const string SqlServerTypeSystemId = "SqlServer";

    private readonly IReadOnlyDictionary<string, string> _sqlServerTypesByLogicalTypeId;
    private readonly IReadOnlyDictionary<string, DataType> _dataTypesById;

    private SqlServerBusinessTypeLowering(
        IReadOnlyDictionary<string, string> sqlServerTypesByLogicalTypeId,
        IReadOnlyDictionary<string, DataType> dataTypesById)
    {
        _sqlServerTypesByLogicalTypeId = sqlServerTypesByLogicalTypeId;
        _dataTypesById = dataTypesById;
    }

    public static SqlServerBusinessTypeLowering Create(MetaDataTypeModel dataTypeModel, MetaDataTypeConversionModel conversionModel)
    {
        ArgumentNullException.ThrowIfNull(dataTypeModel);
        ArgumentNullException.ThrowIfNull(conversionModel);

        var dataTypesById = dataTypeModel.DataTypeList.ToDictionary(row => row.Id, StringComparer.Ordinal);

        var sqlServerTypesByLogicalTypeId = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var mapping in conversionModel.DataTypeMappingList
                     .Where(row => string.Equals(row.ConversionImplementation.Id, DirectConversionImplementationId, StringComparison.Ordinal))
                     .Where(row => IsSqlServerType(dataTypesById, row.TargetDataTypeId)))
        {
            if (sqlServerTypesByLogicalTypeId.TryGetValue(mapping.SourceDataTypeId, out var existingTargetDataTypeId))
            {
                if (!string.Equals(existingTargetDataTypeId, mapping.TargetDataTypeId, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Business logical type '{mapping.SourceDataTypeId}' has conflicting sanctioned SQL Server mappings.");
                }

                continue;
            }

            sqlServerTypesByLogicalTypeId.Add(mapping.SourceDataTypeId, mapping.TargetDataTypeId);
        }

        return new SqlServerBusinessTypeLowering(sqlServerTypesByLogicalTypeId, dataTypesById);
    }

    public string LowerRequired(string sourceTypeId)
    {
        if (string.IsNullOrWhiteSpace(sourceTypeId))
        {
            throw new InvalidOperationException("MetaDataVault column type id is required.");
        }

        if (!_dataTypesById.TryGetValue(sourceTypeId, out var sourceType))
        {
            throw new InvalidOperationException(
                $"MetaDataVault column type '{sourceTypeId}' is not sanctioned in MetaDataType.");
        }

        if (!string.Equals(sourceType.DataTypeSystem.Id, MetaTypeSystemId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"MetaDataVault column type '{sourceTypeId}' must belong to DataTypeSystem '{MetaTypeSystemId}'.");
        }

        if (_sqlServerTypesByLogicalTypeId.TryGetValue(sourceTypeId, out var sqlServerTypeId))
        {
            return sqlServerTypeId;
        }

        throw new InvalidOperationException(
            $"MetaDataVault logical type '{sourceTypeId}' has no sanctioned direct SqlServer lowering.");
    }

    public LoweredSqlServerType LowerRawFieldRequired(string fieldDataTypeId)
    {
        if (string.IsNullOrWhiteSpace(fieldDataTypeId))
        {
            throw new InvalidOperationException("MetaRawDataVault field type id is required.");
        }

        if (!_dataTypesById.TryGetValue(fieldDataTypeId, out var fieldType))
        {
            throw new InvalidOperationException(
                $"MetaRawDataVault field type '{fieldDataTypeId}' is not sanctioned in MetaDataType.");
        }

        string sqlServerTypeId;
        if (_sqlServerTypesByLogicalTypeId.TryGetValue(fieldDataTypeId, out var mappedSqlServerTypeId))
        {
            sqlServerTypeId = mappedSqlServerTypeId;
        }
        else if (string.Equals(fieldType.DataTypeSystem.Id, SqlServerTypeSystemId, StringComparison.Ordinal))
        {
            sqlServerTypeId = fieldDataTypeId;
        }
        else if (string.Equals(fieldType.DataTypeSystem.Id, MetaTypeSystemId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"MetaRawDataVault field logical type '{fieldDataTypeId}' has no sanctioned direct SqlServer lowering.");
        }
        else
        {
            throw new InvalidOperationException(
                $"MetaRawDataVault field type '{fieldDataTypeId}' must belong to DataTypeSystem '{MetaTypeSystemId}' or '{SqlServerTypeSystemId}'.");
        }

        if (!_dataTypesById.TryGetValue(sqlServerTypeId, out var sqlServerType) ||
            !string.Equals(sqlServerType.DataTypeSystem.Id, SqlServerTypeSystemId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"MetaRawDataVault field type '{fieldDataTypeId}' lowered to non-SqlServer type '{sqlServerTypeId}'.");
        }

        return new LoweredSqlServerType(sqlServerTypeId, GetDefaultDetails(sqlServerType.Name));
    }

    private static bool IsSqlServerType(IReadOnlyDictionary<string, DataType> dataTypesById, string dataTypeId)
    {
        if (!dataTypesById.TryGetValue(dataTypeId, out var dataType))
        {
            return false;
        }

        return string.Equals(dataType.DataTypeSystem.Id, SqlServerTypeSystemId, StringComparison.Ordinal);
    }

    private static IReadOnlyList<(string Name, string Value)> GetDefaultDetails(string sqlServerTypeName)
    {
        return sqlServerTypeName.ToLowerInvariant() switch
        {
            "char" or "varchar" or "nchar" or "nvarchar" => [("Length", "256")],
            "binary" or "varbinary" => [("Length", "32")],
            "decimal" or "numeric" => [("Precision", "18"), ("Scale", "4")],
            "time" or "datetime2" or "datetimeoffset" => [("Precision", "7")],
            _ => [],
        };
    }
}

internal sealed record LoweredSqlServerType(
    string DataTypeId,
    IReadOnlyList<(string Name, string Value)> DefaultDetails);
