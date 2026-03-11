using Meta.Core.Ddl;
using MRDV = MetaRawDataVault;
using System.Security.Cryptography;
using System.Text;

namespace MetaDataVault.Core;

internal sealed class RawDataVaultSqlGenerationContext
{
    public RawDataVaultSqlGenerationContext(
        MRDV.MetaRawDataVaultModel rawDataVault,
        RawDataVaultImplementationModel implementation,
        DataTypeConversionModel conversions)
    {
        RawDataVault = rawDataVault ?? throw new ArgumentNullException(nameof(rawDataVault));
        Implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
        Conversions = conversions ?? throw new ArgumentNullException(nameof(conversions));
    }

    public MRDV.MetaRawDataVaultModel RawDataVault { get; }
    public RawDataVaultImplementationModel Implementation { get; }
    public DataTypeConversionModel Conversions { get; }

    public void EnsureRequiredRawImplementation()
    {
        RequireImplementationValue(Implementation.RawHubImplementation.TableNamePattern, "RawHubImplementation.TableNamePattern");
        RequireImplementationValue(Implementation.RawHubImplementation.HashKeyColumnName, "RawHubImplementation.HashKeyColumnName");
        RequireImplementationValue(Implementation.RawHubImplementation.HashKeyDataTypeId, "RawHubImplementation.HashKeyDataTypeId");
        RequireImplementationValue(Implementation.RawHubImplementation.HashKeyLength, "RawHubImplementation.HashKeyLength");
        RequireImplementationValue(Implementation.RawHubImplementation.LoadTimestampColumnName, "RawHubImplementation.LoadTimestampColumnName");
        RequireImplementationValue(Implementation.RawHubImplementation.LoadTimestampDataTypeId, "RawHubImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(Implementation.RawHubImplementation.LoadTimestampPrecision, "RawHubImplementation.LoadTimestampPrecision");
        RequireImplementationValue(Implementation.RawHubImplementation.RecordSourceColumnName, "RawHubImplementation.RecordSourceColumnName");
        RequireImplementationValue(Implementation.RawHubImplementation.RecordSourceDataTypeId, "RawHubImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(Implementation.RawHubImplementation.RecordSourceLength, "RawHubImplementation.RecordSourceLength");
        RequireImplementationValue(Implementation.RawHubImplementation.AuditIdColumnName, "RawHubImplementation.AuditIdColumnName");
        RequireImplementationValue(Implementation.RawHubImplementation.AuditIdDataTypeId, "RawHubImplementation.AuditIdDataTypeId");

        RequireImplementationValue(Implementation.RawLinkImplementation.TableNamePattern, "RawLinkImplementation.TableNamePattern");
        RequireImplementationValue(Implementation.RawLinkImplementation.HashKeyColumnName, "RawLinkImplementation.HashKeyColumnName");
        RequireImplementationValue(Implementation.RawLinkImplementation.HashKeyDataTypeId, "RawLinkImplementation.HashKeyDataTypeId");
        RequireImplementationValue(Implementation.RawLinkImplementation.HashKeyLength, "RawLinkImplementation.HashKeyLength");
        RequireImplementationValue(Implementation.RawLinkImplementation.EndHashKeyColumnPattern, "RawLinkImplementation.EndHashKeyColumnPattern");
        RequireImplementationValue(Implementation.RawLinkImplementation.LoadTimestampColumnName, "RawLinkImplementation.LoadTimestampColumnName");
        RequireImplementationValue(Implementation.RawLinkImplementation.LoadTimestampDataTypeId, "RawLinkImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(Implementation.RawLinkImplementation.LoadTimestampPrecision, "RawLinkImplementation.LoadTimestampPrecision");
        RequireImplementationValue(Implementation.RawLinkImplementation.RecordSourceColumnName, "RawLinkImplementation.RecordSourceColumnName");
        RequireImplementationValue(Implementation.RawLinkImplementation.RecordSourceDataTypeId, "RawLinkImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(Implementation.RawLinkImplementation.RecordSourceLength, "RawLinkImplementation.RecordSourceLength");
        RequireImplementationValue(Implementation.RawLinkImplementation.AuditIdColumnName, "RawLinkImplementation.AuditIdColumnName");
        RequireImplementationValue(Implementation.RawLinkImplementation.AuditIdDataTypeId, "RawLinkImplementation.AuditIdDataTypeId");

        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.TableNamePattern, "RawHubSatelliteImplementation.TableNamePattern");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.ParentHashKeyColumnName, "RawHubSatelliteImplementation.ParentHashKeyColumnName");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.ParentHashKeyDataTypeId, "RawHubSatelliteImplementation.ParentHashKeyDataTypeId");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.ParentHashKeyLength, "RawHubSatelliteImplementation.ParentHashKeyLength");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.HashDiffColumnName, "RawHubSatelliteImplementation.HashDiffColumnName");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.HashDiffDataTypeId, "RawHubSatelliteImplementation.HashDiffDataTypeId");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.HashDiffLength, "RawHubSatelliteImplementation.HashDiffLength");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.LoadTimestampColumnName, "RawHubSatelliteImplementation.LoadTimestampColumnName");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.LoadTimestampDataTypeId, "RawHubSatelliteImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.LoadTimestampPrecision, "RawHubSatelliteImplementation.LoadTimestampPrecision");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.RecordSourceColumnName, "RawHubSatelliteImplementation.RecordSourceColumnName");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.RecordSourceDataTypeId, "RawHubSatelliteImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.RecordSourceLength, "RawHubSatelliteImplementation.RecordSourceLength");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.AuditIdColumnName, "RawHubSatelliteImplementation.AuditIdColumnName");
        RequireImplementationValue(Implementation.RawHubSatelliteImplementation.AuditIdDataTypeId, "RawHubSatelliteImplementation.AuditIdDataTypeId");

        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.TableNamePattern, "RawLinkSatelliteImplementation.TableNamePattern");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.ParentHashKeyColumnName, "RawLinkSatelliteImplementation.ParentHashKeyColumnName");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.ParentHashKeyDataTypeId, "RawLinkSatelliteImplementation.ParentHashKeyDataTypeId");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.ParentHashKeyLength, "RawLinkSatelliteImplementation.ParentHashKeyLength");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.HashDiffColumnName, "RawLinkSatelliteImplementation.HashDiffColumnName");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.HashDiffDataTypeId, "RawLinkSatelliteImplementation.HashDiffDataTypeId");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.HashDiffLength, "RawLinkSatelliteImplementation.HashDiffLength");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.LoadTimestampColumnName, "RawLinkSatelliteImplementation.LoadTimestampColumnName");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.LoadTimestampDataTypeId, "RawLinkSatelliteImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.LoadTimestampPrecision, "RawLinkSatelliteImplementation.LoadTimestampPrecision");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.RecordSourceColumnName, "RawLinkSatelliteImplementation.RecordSourceColumnName");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.RecordSourceDataTypeId, "RawLinkSatelliteImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.RecordSourceLength, "RawLinkSatelliteImplementation.RecordSourceLength");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.AuditIdColumnName, "RawLinkSatelliteImplementation.AuditIdColumnName");
        RequireImplementationValue(Implementation.RawLinkSatelliteImplementation.AuditIdDataTypeId, "RawLinkSatelliteImplementation.AuditIdDataTypeId");
    }

    public string RenderTable(DdlTable table)
    {
        var database = new DdlDatabase();
        database.Tables.Add(table);
        return DdlSqlServerRenderer.RenderSchema(database);
    }

    public string RenderSqlType(string sourceDataTypeId, SqlDataTypeDetail details)
    {
        var sqlServerDataTypeId = ResolveSqlServerDataTypeId(sourceDataTypeId);
        return sqlServerDataTypeId switch
        {
            "sqlserver:type:binary" => $"binary({RequireNumber(details.Length, sourceDataTypeId, "Length")})",
            "sqlserver:type:varbinary" => HasConcreteLength(details.Length) ? $"varbinary({details.Length})" : "varbinary(max)",
            "sqlserver:type:nvarchar" => HasConcreteLength(details.Length) ? $"nvarchar({details.Length})" : "nvarchar(max)",
            "sqlserver:type:varchar" => HasConcreteLength(details.Length) ? $"varchar({details.Length})" : "varchar(max)",
            "sqlserver:type:nchar" => $"nchar({RequireNumber(details.Length, sourceDataTypeId, "Length")})",
            "sqlserver:type:char" => $"char({RequireNumber(details.Length, sourceDataTypeId, "Length")})",
            "sqlserver:type:datetime2" => details.Precision is { Length: > 0 } precision ? $"datetime2({precision})" : "datetime2",
            "sqlserver:type:time" => details.Precision is { Length: > 0 } timePrecision ? $"time({timePrecision})" : "time",
            "sqlserver:type:decimal" => $"decimal({RequireNumber(details.Precision, sourceDataTypeId, "Precision")}, {RequireNumber(details.Scale, sourceDataTypeId, "Scale")})",
            "sqlserver:type:int" => "int",
            "sqlserver:type:smallint" => "smallint",
            "sqlserver:type:bigint" => "bigint",
            "sqlserver:type:bit" => "bit",
            "sqlserver:type:tinyint" => "tinyint",
            "sqlserver:type:date" => "date",
            "sqlserver:type:datetime" => "datetime",
            "sqlserver:type:datetimeoffset" => "datetimeoffset",
            "sqlserver:type:float" => "float",
            "sqlserver:type:money" => "money",
            "sqlserver:type:smallmoney" => "smallmoney",
            "sqlserver:type:uniqueidentifier" => "uniqueidentifier",
            _ => throw new InvalidOperationException($"SQL generation does not support data type '{sourceDataTypeId}' resolved as '{sqlServerDataTypeId}'.")
        };
    }

    private static bool HasConcreteLength(string? length)
    {
        return !string.IsNullOrWhiteSpace(length) && !string.Equals(length, "-1", StringComparison.Ordinal);
    }

    public string ResolveSqlServerDataTypeId(string sourceDataTypeId)
    {
        if (sourceDataTypeId.StartsWith("sqlserver:type:", StringComparison.Ordinal))
        {
            return sourceDataTypeId;
        }

        if (!Conversions.SourceToTargetDataTypeIds.TryGetValue(sourceDataTypeId, out var targetDataTypeId) || string.IsNullOrWhiteSpace(targetDataTypeId))
        {
            throw new InvalidOperationException($"No sanctioned SQL Server data type mapping exists for '{sourceDataTypeId}'.");
        }

        if (!targetDataTypeId.StartsWith("sqlserver:type:", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Data type mapping for '{sourceDataTypeId}' does not resolve to a SQL Server data type. Found '{targetDataTypeId}'.");
        }

        return targetDataTypeId;
    }

    public SqlDataTypeDetail BuildSqlDataTypeDetail(IEnumerable<(string Name, string Value)> details)
    {
        string? length = null;
        string? precision = null;
        string? scale = null;
        foreach (var detail in details)
        {
            if (string.Equals(detail.Name, "Length", StringComparison.Ordinal)) length = detail.Value;
            else if (string.Equals(detail.Name, "Precision", StringComparison.Ordinal) || string.Equals(detail.Name, "NumericPrecision", StringComparison.Ordinal)) precision = detail.Value;
            else if (string.Equals(detail.Name, "Scale", StringComparison.Ordinal)) scale = detail.Value;
        }

        return new SqlDataTypeDetail(length, precision, scale);
    }

    public string RenderPattern(string pattern, IReadOnlyDictionary<string, string> tokens)
    {
        return System.Text.RegularExpressions.Regex.Replace(pattern, @"\{(?<name>[A-Za-z][A-Za-z0-9]*)\}", match =>
        {
            var tokenName = match.Groups["name"].Value;
            if (!tokens.TryGetValue(tokenName, out var tokenValue) || string.IsNullOrWhiteSpace(tokenValue))
            {
                throw new InvalidOperationException($"Pattern '{pattern}' references missing token '{tokenName}'.");
            }
            return tokenValue;
        });
    }

    public string ResolveRawHubTableName(MRDV.RawHub hub)
        => RenderPattern(Implementation.RawHubImplementation.TableNamePattern, new Dictionary<string, string>(StringComparer.Ordinal) { ["Name"] = hub.Name });

    public string ResolveRawLinkTableName(MRDV.RawLink link)
        => RenderPattern(Implementation.RawLinkImplementation.TableNamePattern, new Dictionary<string, string>(StringComparer.Ordinal) { ["Name"] = link.Name });

    public string ResolveRawHubSatelliteTableName(MRDV.RawHubSatellite satellite)
        => RenderPattern(Implementation.RawHubSatelliteImplementation.TableNamePattern, new Dictionary<string, string>(StringComparer.Ordinal) { ["ParentName"] = satellite.RawHub.Name, ["Name"] = satellite.Name });

    public string ResolveRawLinkSatelliteTableName(MRDV.RawLinkSatellite satellite)
        => RenderPattern(Implementation.RawLinkSatelliteImplementation.TableNamePattern, new Dictionary<string, string>(StringComparer.Ordinal) { ["ParentName"] = satellite.RawLink.Name, ["Name"] = satellite.Name });

    public int ParseOrdinal(string ordinal) => int.TryParse(ordinal, out var value) ? value : int.MaxValue;

    public static DdlColumn RenderColumn(string columnName, string sqlType, bool isNullable)
        => new() { Name = columnName, DataType = sqlType, IsNullable = isNullable };

    public static string ReserveColumnName(ISet<string> usedColumnNames, string desiredName)
    {
        ArgumentNullException.ThrowIfNull(usedColumnNames);
        if (string.IsNullOrWhiteSpace(desiredName))
        {
            throw new InvalidOperationException("Column name cannot be blank.");
        }

        var resolvedName = desiredName;
        while (!usedColumnNames.Add(resolvedName))
        {
            resolvedName = "_" + resolvedName;
        }

        return resolvedName;
    }

    public static DdlForeignKeyConstraint RenderForeignKeyConstraint(string constraintName, string columnName, string referencedTableName, string referencedColumnName)
    {
        var constraint = new DdlForeignKeyConstraint { Name = FitSqlIdentifier(constraintName), ReferencedSchema = "dbo", ReferencedTableName = referencedTableName };
        constraint.ColumnNames.Add(columnName);
        constraint.ReferencedColumnNames.Add(referencedColumnName);
        return constraint;
    }

    public static DdlTable CreateTable(string tableName, IReadOnlyList<DdlColumn> columns, IEnumerable<string> primaryKeyColumns, IReadOnlyList<string>? uniqueColumns, IEnumerable<DdlForeignKeyConstraint> foreignKeys)
    {
        var table = new DdlTable { Schema = "dbo", Name = tableName, PrimaryKey = new DdlPrimaryKeyConstraint { Name = FitSqlIdentifier($"PK_{tableName}"), IsClustered = true } };
        table.Columns.AddRange(columns);
        table.PrimaryKey.ColumnNames.AddRange(primaryKeyColumns);
        if (uniqueColumns is not null && uniqueColumns.Count > 0)
        {
            var uq = new DdlUniqueConstraint { Name = FitSqlIdentifier($"UQ_{tableName}") };
            uq.ColumnNames.AddRange(uniqueColumns);
            table.UniqueConstraints.Add(uq);
        }
        table.ForeignKeys.AddRange(foreignKeys);
        return table;
    }

    public static void AppendRequiredColumn(List<DdlColumn> columns, string columnName, string sqlType)
        => columns.Add(RenderColumn(columnName, sqlType, false));

    private static void RequireImplementationValue(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException($"MetaDataVaultImplementation is missing required property '{propertyName}' for current SQL generation.");
    }

    private static string RequireNumber(string? value, string sourceDataTypeId, string detailName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException($"Data type '{sourceDataTypeId}' requires detail '{detailName}' for SQL generation.");
        return value;
    }

    private static string FitSqlIdentifier(string value)
    {
        const int maxLength = 128;
        const int hashLength = 12;
        if (value.Length <= maxLength)
        {
            return value;
        }

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        var hash = Convert.ToHexString(hashBytes).Substring(0, hashLength);
        var prefixLength = maxLength - hashLength - 1;
        return value.Substring(0, prefixLength) + "_" + hash;
    }
}
