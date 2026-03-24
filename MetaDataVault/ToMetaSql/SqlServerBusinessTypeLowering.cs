using MetaDataType;
using MetaDataTypeConversion;

namespace MetaDataVault.ToMetaSql;

internal sealed class SqlServerBusinessTypeLowering
{
    private const string DirectConversionImplementationId = "MetaDataTypeConversion:implementation:direct";
    private const string SqlServerTypeSystemId = "sqlserver:type-system";

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
                     .Where(row => string.Equals(row.ConversionImplementationId, DirectConversionImplementationId, StringComparison.Ordinal))
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

    public string LowerOrKeep(string sourceTypeId)
    {
        if (string.IsNullOrWhiteSpace(sourceTypeId))
        {
            throw new InvalidOperationException("Business column type id is required.");
        }

        if (!_dataTypesById.TryGetValue(sourceTypeId, out var sourceType))
        {
            throw new InvalidOperationException(
                $"Business column type '{sourceTypeId}' is not sanctioned in MetaDataType.");
        }

        if (string.Equals(sourceType.DataTypeSystemId, SqlServerTypeSystemId, StringComparison.Ordinal))
        {
            return sourceTypeId;
        }

        if (_sqlServerTypesByLogicalTypeId.TryGetValue(sourceTypeId, out var sqlServerTypeId))
        {
            return sqlServerTypeId;
        }

        throw new InvalidOperationException(
            $"Business logical type '{sourceTypeId}' has no sanctioned direct SQL Server lowering.");
    }

    private static bool IsSqlServerType(IReadOnlyDictionary<string, DataType> dataTypesById, string dataTypeId)
    {
        if (!dataTypesById.TryGetValue(dataTypeId, out var dataType))
        {
            return false;
        }

        return string.Equals(dataType.DataTypeSystemId, SqlServerTypeSystemId, StringComparison.Ordinal);
    }
}
