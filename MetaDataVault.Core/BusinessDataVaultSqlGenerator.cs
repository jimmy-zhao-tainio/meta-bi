using System.Text;
using System.Text.RegularExpressions;
using BDV = MetaBusinessDataVault;
using DVI = MetaDataVaultImplementation;

namespace MetaDataVault.Core;

public sealed record BusinessDataVaultSqlGenerationResult(
    string OutputPath,
    int FileCount,
    int TableCount,
    int BusinessHubCount,
    int BusinessLinkCount,
    int BusinessHubSatelliteCount,
    int BusinessLinkSatelliteCount);

public interface IBusinessDataVaultSqlGenerator
{
    Task<BusinessDataVaultSqlGenerationResult> GenerateAsync(
        string businessDataVaultWorkspacePath,
        string implementationWorkspacePath,
        string dataTypeConversionWorkspacePath,
        string outputPath,
        CancellationToken cancellationToken = default);
}

public sealed class BusinessDataVaultSqlGenerator : IBusinessDataVaultSqlGenerator
{
    private static readonly Regex TokenPattern = new(@"\{(?<name>[A-Za-z][A-Za-z0-9]*)\}", RegexOptions.Compiled);

    public async Task<BusinessDataVaultSqlGenerationResult> GenerateAsync(
        string businessDataVaultWorkspacePath,
        string implementationWorkspacePath,
        string dataTypeConversionWorkspacePath,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(businessDataVaultWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(implementationWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(dataTypeConversionWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        var bdv = await BusinessDataVaultSqlModelLoaders.LoadBusinessDataVaultAsync(businessDataVaultWorkspacePath, cancellationToken).ConfigureAwait(false);
        var implementation = await BusinessDataVaultSqlModelLoaders.LoadImplementationAsync(implementationWorkspacePath, cancellationToken).ConfigureAwait(false);
        var conversions = await BusinessDataVaultSqlModelLoaders.LoadDataTypeConversionAsync(dataTypeConversionWorkspacePath, cancellationToken).ConfigureAwait(false);

        if (bdv.BusinessPointInTimeList.Count > 0)
        {
            throw new InvalidOperationException("BusinessPointInTime SQL generation is not implemented yet.");
        }

        if (bdv.BusinessBridgeList.Count > 0)
        {
            throw new InvalidOperationException("BusinessBridge SQL generation is not implemented yet.");
        }

        var outputRoot = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(outputRoot);

        var scripts = new List<(string FileName, string Sql)>();

        foreach (var hub in bdv.BusinessHubList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{hub.Name}.sql", RenderBusinessHubSql(hub, bdv, implementation, conversions)));
        }

        foreach (var link in bdv.BusinessLinkList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{link.Name}.sql", RenderBusinessLinkSql(link, bdv, implementation, conversions)));
        }

        foreach (var satellite in bdv.BusinessHubSatelliteList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{satellite.Name}.sql", RenderBusinessHubSatelliteSql(satellite, bdv, implementation, conversions)));
        }

        foreach (var satellite in bdv.BusinessLinkSatelliteList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{satellite.Name}.sql", RenderBusinessLinkSatelliteSql(satellite, bdv, implementation, conversions)));
        }

        foreach (var script in scripts)
        {
            var path = Path.Combine(outputRoot, script.FileName);
            await File.WriteAllTextAsync(path, script.Sql, new UTF8Encoding(false), cancellationToken).ConfigureAwait(false);
        }

        return new BusinessDataVaultSqlGenerationResult(
            outputRoot,
            scripts.Count,
            scripts.Count,
            bdv.BusinessHubList.Count,
            bdv.BusinessLinkList.Count,
            bdv.BusinessHubSatelliteList.Count,
            bdv.BusinessLinkSatelliteList.Count);
    }



    private static string RenderBusinessHubSql(BDV.BusinessHub hub, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        var columns = new List<string>
        {
            RenderColumn(implementation.BusinessHubImplementation.HashKeyColumnName, RenderSqlType(implementation.BusinessHubImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessHubImplementation.HashKeyLength, null, null), conversions), false)
        };

        foreach (var keyPart in bdv.BusinessHubKeyPartList.Where(row => row.BusinessHubId == hub.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(keyPart.Name, RenderSqlType(keyPart.DataTypeId, BuildDetailBag(bdv.BusinessHubKeyPartDataTypeDetailList.Where(detail => detail.BusinessHubKeyPartId == keyPart.Id).Select(detail => (detail.Name, detail.Value))), conversions), false));
        }

        AppendOptionalColumn(columns, implementation.BusinessHubImplementation.LoadTimestampColumnName, implementation.BusinessHubImplementation.LoadTimestampDataTypeId, new DetailBag(null, implementation.BusinessHubImplementation.LoadTimestampPrecision, null), conversions);
        AppendOptionalColumn(columns, implementation.BusinessHubImplementation.RecordSourceColumnName, implementation.BusinessHubImplementation.RecordSourceDataTypeId, new DetailBag(implementation.BusinessHubImplementation.RecordSourceLength, null, null), conversions);

        var primaryKeyColumns = new[] { implementation.BusinessHubImplementation.HashKeyColumnName };
        var uniqueColumns = bdv.BusinessHubKeyPartList.Where(row => row.BusinessHubId == hub.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).Select(row => row.Name).ToList();

        return RenderCreateTableSql(hub.Name, columns, primaryKeyColumns, uniqueColumns.Count == 0 ? null : uniqueColumns, Array.Empty<string>());
    }

    private static string RenderBusinessLinkSql(BDV.BusinessLink link, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        var columns = new List<string>
        {
            RenderColumn(implementation.BusinessLinkImplementation.HashKeyColumnName, RenderSqlType(implementation.BusinessLinkImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessLinkImplementation.HashKeyLength, null, null), conversions), false)
        };
        var foreignKeys = new List<string>();
        var endColumns = new List<string>();

        foreach (var end in bdv.BusinessLinkEndList.Where(row => row.BusinessLinkId == link.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.RoleName, StringComparer.Ordinal))
        {
            var columnName = RenderPattern(implementation.BusinessLinkImplementation.EndHashKeyColumnPattern, new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["RoleName"] = end.RoleName,
                ["HubName"] = end.BusinessHub.Name
            });
            columns.Add(RenderColumn(columnName, RenderSqlType(implementation.BusinessHubImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessHubImplementation.HashKeyLength, null, null), conversions), false));
            foreignKeys.Add($"    CONSTRAINT {Quote($"FK_{link.Name}_{end.BusinessHub.Name}_{columnName}")} FOREIGN KEY ({Quote(columnName)}) REFERENCES {Quote(end.BusinessHub.Name)} ({Quote(implementation.BusinessHubImplementation.HashKeyColumnName)})");
            endColumns.Add(columnName);
        }

        AppendOptionalColumn(columns, implementation.BusinessLinkImplementation.LoadTimestampColumnName, implementation.BusinessLinkImplementation.LoadTimestampDataTypeId, new DetailBag(null, implementation.BusinessLinkImplementation.LoadTimestampPrecision, null), conversions);
        AppendOptionalColumn(columns, implementation.BusinessLinkImplementation.RecordSourceColumnName, implementation.BusinessLinkImplementation.RecordSourceDataTypeId, new DetailBag(implementation.BusinessLinkImplementation.RecordSourceLength, null, null), conversions);

        var primaryKeyColumns = new[] { implementation.BusinessLinkImplementation.HashKeyColumnName };
        return RenderCreateTableSql(link.Name, columns, primaryKeyColumns, endColumns.Count == 0 ? null : endColumns, foreignKeys);
    }

    private static string RenderBusinessHubSatelliteSql(BDV.BusinessHubSatellite satellite, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        var columns = new List<string>
        {
            RenderColumn(implementation.BusinessHubSatelliteImplementation.ParentHashKeyColumnName, RenderSqlType(implementation.BusinessHubSatelliteImplementation.ParentHashKeyDataTypeId, new DetailBag(implementation.BusinessHubSatelliteImplementation.ParentHashKeyLength, null, null), conversions), false)
        };
        var primaryKeyColumns = new List<string> { implementation.BusinessHubSatelliteImplementation.ParentHashKeyColumnName };

        foreach (var keyPart in bdv.BusinessHubSatelliteKeyPartList.Where(row => row.BusinessHubSatelliteId == satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(keyPart.Name, RenderSqlType(keyPart.DataTypeId, BuildDetailBag(bdv.BusinessHubSatelliteKeyPartDataTypeDetailList.Where(detail => detail.BusinessHubSatelliteKeyPartId == keyPart.Id).Select(detail => (detail.Name, detail.Value))), conversions), false));
            primaryKeyColumns.Add(keyPart.Name);
        }

        foreach (var attribute in bdv.BusinessHubSatelliteAttributeList.Where(row => row.BusinessHubSatelliteId == satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(attribute.Name, RenderSqlType(attribute.DataTypeId, BuildDetailBag(bdv.BusinessHubSatelliteAttributeDataTypeDetailList.Where(detail => detail.BusinessHubSatelliteAttributeId == attribute.Id).Select(detail => (detail.Name, detail.Value))), conversions), false));
        }

        AppendOptionalColumn(columns, implementation.BusinessHubSatelliteImplementation.HashDiffColumnName, implementation.BusinessHubSatelliteImplementation.HashDiffDataTypeId, new DetailBag(implementation.BusinessHubSatelliteImplementation.HashDiffLength, null, null), conversions);
        AppendOptionalColumn(columns, implementation.BusinessHubSatelliteImplementation.LoadTimestampColumnName, implementation.BusinessHubSatelliteImplementation.LoadTimestampDataTypeId, new DetailBag(null, implementation.BusinessHubSatelliteImplementation.LoadTimestampPrecision, null), conversions, primaryKeyColumns);
        AppendOptionalColumn(columns, implementation.BusinessHubSatelliteImplementation.RecordSourceColumnName, implementation.BusinessHubSatelliteImplementation.RecordSourceDataTypeId, new DetailBag(implementation.BusinessHubSatelliteImplementation.RecordSourceLength, null, null), conversions);

        var foreignKeys = new[]
        {
            $"    CONSTRAINT {Quote($"FK_{satellite.Name}_{satellite.BusinessHub.Name}")} FOREIGN KEY ({Quote(implementation.BusinessHubSatelliteImplementation.ParentHashKeyColumnName)}) REFERENCES {Quote(satellite.BusinessHub.Name)} ({Quote(implementation.BusinessHubImplementation.HashKeyColumnName)})"
        };

        return RenderCreateTableSql(satellite.Name, columns, primaryKeyColumns, null, foreignKeys);
    }

    private static string RenderBusinessLinkSatelliteSql(BDV.BusinessLinkSatellite satellite, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        var columns = new List<string>
        {
            RenderColumn(implementation.BusinessLinkSatelliteImplementation.ParentHashKeyColumnName, RenderSqlType(implementation.BusinessLinkSatelliteImplementation.ParentHashKeyDataTypeId, new DetailBag(implementation.BusinessLinkSatelliteImplementation.ParentHashKeyLength, null, null), conversions), false)
        };
        var primaryKeyColumns = new List<string> { implementation.BusinessLinkSatelliteImplementation.ParentHashKeyColumnName };

        foreach (var keyPart in bdv.BusinessLinkSatelliteKeyPartList.Where(row => row.BusinessLinkSatelliteId == satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(keyPart.Name, RenderSqlType(keyPart.DataTypeId, BuildDetailBag(bdv.BusinessLinkSatelliteKeyPartDataTypeDetailList.Where(detail => detail.BusinessLinkSatelliteKeyPartId == keyPart.Id).Select(detail => (detail.Name, detail.Value))), conversions), false));
            primaryKeyColumns.Add(keyPart.Name);
        }

        foreach (var attribute in bdv.BusinessLinkSatelliteAttributeList.Where(row => row.BusinessLinkSatelliteId == satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(attribute.Name, RenderSqlType(attribute.DataTypeId, BuildDetailBag(bdv.BusinessLinkSatelliteAttributeDataTypeDetailList.Where(detail => detail.BusinessLinkSatelliteAttributeId == attribute.Id).Select(detail => (detail.Name, detail.Value))), conversions), false));
        }

        AppendOptionalColumn(columns, implementation.BusinessLinkSatelliteImplementation.HashDiffColumnName, implementation.BusinessLinkSatelliteImplementation.HashDiffDataTypeId, new DetailBag(implementation.BusinessLinkSatelliteImplementation.HashDiffLength, null, null), conversions);
        AppendOptionalColumn(columns, implementation.BusinessLinkSatelliteImplementation.LoadTimestampColumnName, implementation.BusinessLinkSatelliteImplementation.LoadTimestampDataTypeId, new DetailBag(null, implementation.BusinessLinkSatelliteImplementation.LoadTimestampPrecision, null), conversions, primaryKeyColumns);
        AppendOptionalColumn(columns, implementation.BusinessLinkSatelliteImplementation.RecordSourceColumnName, implementation.BusinessLinkSatelliteImplementation.RecordSourceDataTypeId, new DetailBag(implementation.BusinessLinkSatelliteImplementation.RecordSourceLength, null, null), conversions);

        var foreignKeys = new[]
        {
            $"    CONSTRAINT {Quote($"FK_{satellite.Name}_{satellite.BusinessLink.Name}")} FOREIGN KEY ({Quote(implementation.BusinessLinkSatelliteImplementation.ParentHashKeyColumnName)}) REFERENCES {Quote(satellite.BusinessLink.Name)} ({Quote(implementation.BusinessLinkImplementation.HashKeyColumnName)})"
        };

        return RenderCreateTableSql(satellite.Name, columns, primaryKeyColumns, null, foreignKeys);
    }

    private static void AppendOptionalColumn(List<string> columns, string columnName, string dataTypeId, DetailBag details, DataTypeConversionModel conversions, List<string>? primaryKeyColumns = null)
    {
        if (string.IsNullOrWhiteSpace(columnName) || string.IsNullOrWhiteSpace(dataTypeId))
        {
            return;
        }

        columns.Add(RenderColumn(columnName, RenderSqlType(dataTypeId, details, conversions), false));
        if (primaryKeyColumns is not null)
        {
            primaryKeyColumns.Add(columnName);
        }
    }

    private static string RenderCreateTableSql(string tableName, IReadOnlyList<string> columns, IEnumerable<string> primaryKeyColumns, IReadOnlyList<string>? uniqueColumns, IEnumerable<string> foreignKeys)
    {
        var items = new List<string>();
        items.AddRange(columns.Select(item => "    " + item));
        items.Add($"    CONSTRAINT {Quote($"PK_{tableName}")} PRIMARY KEY ({string.Join(", ", primaryKeyColumns.Select(Quote))})");
        if (uniqueColumns is not null && uniqueColumns.Count > 0)
        {
            items.Add($"    CONSTRAINT {Quote($"UQ_{tableName}")} UNIQUE ({string.Join(", ", uniqueColumns.Select(Quote))})");
        }
        items.AddRange(foreignKeys);

        var builder = new StringBuilder();
        builder.AppendLine($"CREATE TABLE {Quote(tableName)} (");
        builder.AppendLine(string.Join("," + Environment.NewLine, items));
        builder.AppendLine(");");
        return builder.ToString();
    }

    private static string RenderColumn(string columnName, string sqlType, bool isNullable)
    {
        return $"{Quote(columnName)} {sqlType} {(isNullable ? "NULL" : "NOT NULL")}";
    }

    private static string RenderSqlType(string sourceDataTypeId, DetailBag details, DataTypeConversionModel conversions)
    {
        var sqlServerDataTypeId = ResolveSqlServerDataTypeId(sourceDataTypeId, conversions);
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

    private static string ResolveSqlServerDataTypeId(string sourceDataTypeId, DataTypeConversionModel conversions)
    {
        if (sourceDataTypeId.StartsWith("sqlserver:type:", StringComparison.Ordinal))
        {
            return sourceDataTypeId;
        }

        if (!conversions.SourceToTargetDataTypeIds.TryGetValue(sourceDataTypeId, out var targetDataTypeId) || string.IsNullOrWhiteSpace(targetDataTypeId))
        {
            throw new InvalidOperationException($"No sanctioned SQL Server data type mapping exists for '{sourceDataTypeId}'.");
        }

        if (!targetDataTypeId.StartsWith("sqlserver:type:", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Data type mapping for '{sourceDataTypeId}' does not resolve to a SQL Server data type. Found '{targetDataTypeId}'.");
        }

        return targetDataTypeId;
    }

    private static DetailBag BuildDetailBag(IEnumerable<(string Name, string Value)> details)
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

        return new DetailBag(length, precision, scale);
    }

    private static string RenderPattern(string pattern, IReadOnlyDictionary<string, string> tokens)
    {
        return TokenPattern.Replace(pattern, match =>
        {
            var tokenName = match.Groups["name"].Value;
            if (!tokens.TryGetValue(tokenName, out var tokenValue) || string.IsNullOrWhiteSpace(tokenValue))
            {
                throw new InvalidOperationException($"Pattern '{pattern}' references missing token '{tokenName}'.");
            }

            return tokenValue;
        });
    }

    private static int ParseOrdinal(string ordinal)
    {
        return int.TryParse(ordinal, out var value) ? value : int.MaxValue;
    }

    private static string Quote(string identifier) => $"[{identifier}]";

    private static string RequireNumber(string? value, string dataTypeId, string detailName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Data type '{dataTypeId}' requires detail '{detailName}' for SQL generation.");
        }

        return value;
    }

    private readonly record struct DetailBag(string? Length, string? Precision, string? Scale);
}

