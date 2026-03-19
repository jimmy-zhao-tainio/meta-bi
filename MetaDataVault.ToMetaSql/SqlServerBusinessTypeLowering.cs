using MetaDataTypeConversion;

namespace MetaDataVault.ToMetaSql;

internal sealed class SqlServerBusinessTypeLowering
{
    private const string DirectConversionImplementationId = "MetaDataTypeConversion:implementation:direct";
    private const string SqlServerTypePrefix = "sqlserver:type:";

    private readonly IReadOnlyDictionary<string, string> _sqlServerTypesByLogicalTypeId;

    private SqlServerBusinessTypeLowering(IReadOnlyDictionary<string, string> sqlServerTypesByLogicalTypeId)
    {
        _sqlServerTypesByLogicalTypeId = sqlServerTypesByLogicalTypeId;
    }

    public static SqlServerBusinessTypeLowering Create(MetaDataTypeConversionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var sqlServerTypesByLogicalTypeId = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var mapping in model.DataTypeMappingList
                     .Where(row => string.Equals(row.ConversionImplementationId, DirectConversionImplementationId, StringComparison.Ordinal))
                     .Where(row => row.TargetDataTypeId.StartsWith("sqlserver:type:", StringComparison.Ordinal)))
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

        return new SqlServerBusinessTypeLowering(sqlServerTypesByLogicalTypeId);
    }

    public string LowerOrKeep(string sourceTypeId)
    {
        if (string.IsNullOrWhiteSpace(sourceTypeId))
        {
            throw new InvalidOperationException("Business column type id is required.");
        }

        if (sourceTypeId.StartsWith(SqlServerTypePrefix, StringComparison.Ordinal))
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
}
