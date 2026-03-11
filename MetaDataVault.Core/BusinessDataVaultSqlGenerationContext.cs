using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;

namespace MetaDataVault.Core;

internal sealed class BusinessDataVaultSqlGenerationContext
{
    public BusinessDataVaultSqlGenerationContext(
        BDV.MetaBusinessDataVaultModel businessDataVault,
        DataVaultImplementationModel implementation,
        DataTypeConversionModel conversions)
    {
        BusinessDataVault = businessDataVault ?? throw new ArgumentNullException(nameof(businessDataVault));
        Implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));
        Conversions = conversions ?? throw new ArgumentNullException(nameof(conversions));
    }

    public BDV.MetaBusinessDataVaultModel BusinessDataVault { get; }
    public DataVaultImplementationModel Implementation { get; }
    public DataTypeConversionModel Conversions { get; }

    public void EnsureRequiredBusinessImplementation()
    {
        BusinessDataVaultSqlGenerator.EnsureRequiredBusinessImplementation(Implementation);
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
            "sqlserver:type:varbinary" => details.Length is { Length: > 0 } length ? $"varbinary({length})" : "varbinary(max)",
            "sqlserver:type:nvarchar" => details.Length is { Length: > 0 } nvarcharLength ? $"nvarchar({nvarcharLength})" : "nvarchar(max)",
            "sqlserver:type:varchar" => details.Length is { Length: > 0 } varcharLength ? $"varchar({varcharLength})" : "varchar(max)",
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
            if (string.Equals(detail.Name, "Length", StringComparison.Ordinal))
            {
                length = detail.Value;
            }
            else if (string.Equals(detail.Name, "Precision", StringComparison.Ordinal) || string.Equals(detail.Name, "NumericPrecision", StringComparison.Ordinal))
            {
                precision = detail.Value;
            }
            else if (string.Equals(detail.Name, "Scale", StringComparison.Ordinal))
            {
                scale = detail.Value;
            }
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

    public int ParseOrdinal(string ordinal)
    {
        return int.TryParse(ordinal, out var value) ? value : int.MaxValue;
    }

    public static DdlColumn RenderColumn(string columnName, string sqlType, bool isNullable)
        => new() { Name = columnName, DataType = sqlType, IsNullable = isNullable };

    public static DdlForeignKeyConstraint RenderForeignKeyConstraint(string constraintName, string columnName, string referencedTableName, string referencedColumnName)
    {
        var constraint = new DdlForeignKeyConstraint
        {
            Name = constraintName,
            ReferencedSchema = "dbo",
            ReferencedTableName = referencedTableName,
        };
        constraint.ColumnNames.Add(columnName);
        constraint.ReferencedColumnNames.Add(referencedColumnName);
        return constraint;
    }

    public static DdlTable CreateTable(string tableName, IReadOnlyList<DdlColumn> columns, IEnumerable<string> primaryKeyColumns, IReadOnlyList<string>? uniqueColumns, IEnumerable<DdlForeignKeyConstraint> foreignKeys)
    {
        var table = new DdlTable
        {
            Schema = "dbo",
            Name = tableName,
            PrimaryKey = new DdlPrimaryKeyConstraint { Name = $"PK_{tableName}", IsClustered = true },
        };
        table.Columns.AddRange(columns);
        table.PrimaryKey.ColumnNames.AddRange(primaryKeyColumns);
        if (uniqueColumns is not null && uniqueColumns.Count > 0)
        {
            var uniqueConstraint = new DdlUniqueConstraint { Name = $"UQ_{tableName}" };
            uniqueConstraint.ColumnNames.AddRange(uniqueColumns);
            table.UniqueConstraints.Add(uniqueConstraint);
        }
        table.ForeignKeys.AddRange(foreignKeys);
        return table;
    }

    public static void AppendRequiredColumn(List<DdlColumn> columns, string columnName, string sqlType)
        => columns.Add(RenderColumn(columnName, sqlType, false));

    public static void AppendOptionalColumn(List<DdlColumn> columns, string? columnName, string? sqlType, List<string>? primaryKeyColumns = null)
    {
        if (string.IsNullOrWhiteSpace(columnName) || string.IsNullOrWhiteSpace(sqlType))
        {
            return;
        }
        columns.Add(RenderColumn(columnName, sqlType, false));
        primaryKeyColumns?.Add(columnName);
    }

    public static string RequireNumber(string? value, string sourceDataTypeId, string detailName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Data type '{sourceDataTypeId}' requires detail '{detailName}' for SQL generation.");
        }
        return value;
    }
}

