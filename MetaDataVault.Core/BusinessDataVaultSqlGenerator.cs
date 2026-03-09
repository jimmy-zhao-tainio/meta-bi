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
    int BusinessLinkSatelliteCount,
    int BusinessPointInTimeCount,
    int BusinessBridgeCount);

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

        var outputRoot = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(outputRoot);

        EnsureRequiredBusinessImplementation(implementation);

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

        foreach (var pointInTime in bdv.BusinessPointInTimeList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{pointInTime.Name}.sql", RenderBusinessPointInTimeSql(pointInTime, bdv, implementation, conversions)));
        }

        foreach (var bridge in bdv.BusinessBridgeList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{bridge.Name}.sql", RenderBusinessBridgeSql(bridge, bdv, implementation, conversions)));
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
            bdv.BusinessLinkSatelliteList.Count,
            bdv.BusinessPointInTimeList.Count,
            bdv.BusinessBridgeList.Count);
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
        EnsureSupportedBusinessLink(link, bdv);

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
        EnsureSupportedBusinessHubSatellite(satellite, bdv);

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
        EnsureSupportedBusinessLinkSatellite(satellite, bdv);

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

    private static string RenderBusinessPointInTimeSql(BDV.BusinessPointInTime pointInTime, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessPointInTime(pointInTime, bdv);

        var columns = new List<string>
        {
            RenderColumn(implementation.BusinessPointInTimeImplementation.ParentHashKeyColumnName, RenderSqlType(implementation.BusinessPointInTimeImplementation.ParentHashKeyDataTypeId, new DetailBag(implementation.BusinessPointInTimeImplementation.ParentHashKeyLength, null, null), conversions), false),
            RenderColumn(implementation.BusinessPointInTimeImplementation.SnapshotTimestampColumnName, RenderSqlType(implementation.BusinessPointInTimeImplementation.SnapshotTimestampDataTypeId, new DetailBag(null, implementation.BusinessPointInTimeImplementation.SnapshotTimestampPrecision, null), conversions), false)
        };

        foreach (var satellite in bdv.BusinessPointInTimeHubSatelliteList.Where(row => row.BusinessPointInTimeId == pointInTime.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.BusinessHubSatellite.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(RenderPattern(implementation.BusinessPointInTimeImplementation.SatelliteReferenceColumnNamePattern, new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["SatelliteName"] = satellite.BusinessHubSatellite.Name
            }), RenderSqlType(implementation.BusinessPointInTimeImplementation.SatelliteReferenceDataTypeId, new DetailBag(null, implementation.BusinessPointInTimeImplementation.SatelliteReferencePrecision, null), conversions), false));
        }

        foreach (var satellite in bdv.BusinessPointInTimeLinkSatelliteList.Where(row => row.BusinessPointInTimeId == pointInTime.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.BusinessLinkSatellite.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(RenderPattern(implementation.BusinessPointInTimeImplementation.SatelliteReferenceColumnNamePattern, new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["SatelliteName"] = satellite.BusinessLinkSatellite.Name
            }), RenderSqlType(implementation.BusinessPointInTimeImplementation.SatelliteReferenceDataTypeId, new DetailBag(null, implementation.BusinessPointInTimeImplementation.SatelliteReferencePrecision, null), conversions), false));
        }

        var foreignKeys = new[]
        {
            $"    CONSTRAINT {Quote($"FK_{pointInTime.Name}_{pointInTime.BusinessHub.Name}")} FOREIGN KEY ({Quote(implementation.BusinessPointInTimeImplementation.ParentHashKeyColumnName)}) REFERENCES {Quote(pointInTime.BusinessHub.Name)} ({Quote(implementation.BusinessHubImplementation.HashKeyColumnName)})"
        };

        return RenderCreateTableSql(pointInTime.Name, columns, new[]
        {
            implementation.BusinessPointInTimeImplementation.ParentHashKeyColumnName,
            implementation.BusinessPointInTimeImplementation.SnapshotTimestampColumnName
        }, null, foreignKeys);
    }

    private static string RenderBusinessBridgeSql(BDV.BusinessBridge bridge, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessBridge(bridge);
        var bridgePath = ResolveBridgePath(bridge, bdv);

        var columns = new List<string>
        {
            RenderColumn(implementation.BusinessBridgeImplementation.RootHashKeyColumnName, RenderSqlType(implementation.BusinessBridgeImplementation.RootHashKeyDataTypeId, new DetailBag(implementation.BusinessBridgeImplementation.RootHashKeyLength, null, null), conversions), false),
            RenderColumn(implementation.BusinessBridgeImplementation.RelatedHashKeyColumnName, RenderSqlType(implementation.BusinessBridgeImplementation.RelatedHashKeyDataTypeId, new DetailBag(implementation.BusinessBridgeImplementation.RelatedHashKeyLength, null, null), conversions), false)
        };
        var primaryKeyColumns = new List<string>
        {
            implementation.BusinessBridgeImplementation.RootHashKeyColumnName,
            implementation.BusinessBridgeImplementation.RelatedHashKeyColumnName
        };

        AppendOptionalColumn(columns, implementation.BusinessBridgeImplementation.DepthColumnName, implementation.BusinessBridgeImplementation.DepthDataTypeId, new DetailBag(null, null, null), conversions);
        AppendOptionalColumn(columns, implementation.BusinessBridgeImplementation.PathColumnName, implementation.BusinessBridgeImplementation.PathDataTypeId, new DetailBag(implementation.BusinessBridgeImplementation.PathLength, null, null), conversions);
        if (!string.IsNullOrWhiteSpace(implementation.BusinessBridgeImplementation.EffectiveFromColumnName) && !string.IsNullOrWhiteSpace(implementation.BusinessBridgeImplementation.EffectiveFromDataTypeId))
        {
            AppendOptionalColumn(columns, implementation.BusinessBridgeImplementation.EffectiveFromColumnName, implementation.BusinessBridgeImplementation.EffectiveFromDataTypeId, new DetailBag(null, implementation.BusinessBridgeImplementation.EffectiveFromPrecision, null), conversions);
            primaryKeyColumns.Add(implementation.BusinessBridgeImplementation.EffectiveFromColumnName);
        }
        AppendOptionalColumn(columns, implementation.BusinessBridgeImplementation.EffectiveToColumnName, implementation.BusinessBridgeImplementation.EffectiveToDataTypeId, new DetailBag(null, implementation.BusinessBridgeImplementation.EffectiveToPrecision, null), conversions);

        var foreignKeys = new List<string>
        {
            $"    CONSTRAINT {Quote($"FK_{bridge.Name}_{bridge.AnchorHub.Name}_{implementation.BusinessBridgeImplementation.RootHashKeyColumnName}")} FOREIGN KEY ({Quote(implementation.BusinessBridgeImplementation.RootHashKeyColumnName)}) REFERENCES {Quote(bridge.AnchorHub.Name)} ({Quote(implementation.BusinessHubImplementation.HashKeyColumnName)})",
            $"    CONSTRAINT {Quote($"FK_{bridge.Name}_{bridgePath.RelatedHub.Name}_{implementation.BusinessBridgeImplementation.RelatedHashKeyColumnName}")} FOREIGN KEY ({Quote(implementation.BusinessBridgeImplementation.RelatedHashKeyColumnName)}) REFERENCES {Quote(bridgePath.RelatedHub.Name)} ({Quote(implementation.BusinessHubImplementation.HashKeyColumnName)})"
        };

        return RenderCreateTableSql(bridge.Name, columns, primaryKeyColumns, null, foreignKeys);
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

    private static void EnsureSupportedBusinessLink(BDV.BusinessLink link, BDV.MetaBusinessDataVaultModel bdv)
    {
        if (!string.Equals(link.LinkKind, "standard", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"SQL generation currently supports only BusinessLink.LinkKind='standard'. Link '{link.Name}' uses '{link.LinkKind}'.");
        }

        if (bdv.BusinessLinkEndList.Count(row => row.BusinessLinkId == link.Id && IsTrue(row.IsDrivingKey)) > 0)
        {
            throw new InvalidOperationException($"SQL generation does not yet support BusinessLinkEnd.IsDrivingKey semantics. Link '{link.Name}' sets IsDrivingKey.");
        }
    }

    private static void EnsureSupportedBusinessHubSatellite(BDV.BusinessHubSatellite satellite, BDV.MetaBusinessDataVaultModel bdv)
    {
        var satelliteKeyParts = bdv.BusinessHubSatelliteKeyPartList
            .Where(row => row.BusinessHubSatelliteId == satellite.Id)
            .ToList();

        if (!string.Equals(satellite.SatelliteKind, "standard", StringComparison.Ordinal) &&
            !string.Equals(satellite.SatelliteKind, "multi-active", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"SQL generation currently supports BusinessHubSatellite.SatelliteKind values 'standard' and 'multi-active'. Satellite '{satellite.Name}' uses '{satellite.SatelliteKind}'.");
        }

        if (string.Equals(satellite.SatelliteKind, "standard", StringComparison.Ordinal) && satelliteKeyParts.Count > 0)
        {
            throw new InvalidOperationException($"SQL generation does not allow BusinessHubSatellite.SatelliteKind='standard' with BusinessHubSatelliteKeyPart rows. Satellite '{satellite.Name}' declares {satelliteKeyParts.Count} key part(s).");
        }

        if (string.Equals(satellite.SatelliteKind, "multi-active", StringComparison.Ordinal) && satelliteKeyParts.Count == 0)
        {
            throw new InvalidOperationException($"SQL generation requires BusinessHubSatellite.SatelliteKind='multi-active' satellites to declare at least one BusinessHubSatelliteKeyPart. Satellite '{satellite.Name}' declares none.");
        }
    }

    private static void EnsureSupportedBusinessLinkSatellite(BDV.BusinessLinkSatellite satellite, BDV.MetaBusinessDataVaultModel bdv)
    {
        var satelliteKeyParts = bdv.BusinessLinkSatelliteKeyPartList
            .Where(row => row.BusinessLinkSatelliteId == satellite.Id)
            .ToList();

        if (!string.Equals(satellite.SatelliteKind, "standard", StringComparison.Ordinal) &&
            !string.Equals(satellite.SatelliteKind, "multi-active", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"SQL generation currently supports BusinessLinkSatellite.SatelliteKind values 'standard' and 'multi-active'. Satellite '{satellite.Name}' uses '{satellite.SatelliteKind}'.");
        }

        if (string.Equals(satellite.SatelliteKind, "standard", StringComparison.Ordinal) && satelliteKeyParts.Count > 0)
        {
            throw new InvalidOperationException($"SQL generation does not allow BusinessLinkSatellite.SatelliteKind='standard' with BusinessLinkSatelliteKeyPart rows. Satellite '{satellite.Name}' declares {satelliteKeyParts.Count} key part(s).");
        }

        if (string.Equals(satellite.SatelliteKind, "multi-active", StringComparison.Ordinal) && satelliteKeyParts.Count == 0)
        {
            throw new InvalidOperationException($"SQL generation requires BusinessLinkSatellite.SatelliteKind='multi-active' satellites to declare at least one BusinessLinkSatelliteKeyPart. Satellite '{satellite.Name}' declares none.");
        }
    }

    private static void EnsureSupportedBusinessPointInTime(BDV.BusinessPointInTime pointInTime, BDV.MetaBusinessDataVaultModel bdv)
    {
        var multiActiveHubSatellite = bdv.BusinessPointInTimeHubSatelliteList
            .Where(row => row.BusinessPointInTimeId == pointInTime.Id)
            .Select(row => row.BusinessHubSatellite)
            .FirstOrDefault(row => string.Equals(row.SatelliteKind, "multi-active", StringComparison.Ordinal));
        if (multiActiveHubSatellite is not null)
        {
            throw new InvalidOperationException($"SQL generation does not yet support BusinessPointInTime references to multi-active hub satellites. Point-in-time '{pointInTime.Name}' references '{multiActiveHubSatellite.Name}'.");
        }

        var multiActiveLinkSatellite = bdv.BusinessPointInTimeLinkSatelliteList
            .Where(row => row.BusinessPointInTimeId == pointInTime.Id)
            .Select(row => row.BusinessLinkSatellite)
            .FirstOrDefault(row => string.Equals(row.SatelliteKind, "multi-active", StringComparison.Ordinal));
        if (multiActiveLinkSatellite is not null)
        {
            throw new InvalidOperationException($"SQL generation does not yet support BusinessPointInTime references to multi-active link satellites. Point-in-time '{pointInTime.Name}' references '{multiActiveLinkSatellite.Name}'.");
        }
    }

    private static void EnsureSupportedBusinessBridge(BDV.BusinessBridge bridge)
    {
        if (!string.Equals(bridge.BridgeKind, "standard", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"SQL generation currently supports only BusinessBridge.BridgeKind='standard'. Bridge '{bridge.Name}' uses '{bridge.BridgeKind}'.");
        }
    }

    private static BridgePath ResolveBridgePath(BDV.BusinessBridge bridge, BDV.MetaBusinessDataVaultModel bdv)
    {
        var bridgeHubs = bdv.BusinessBridgeHubList
            .Where(row => row.BusinessBridgeId == bridge.Id)
            .OrderBy(row => ParseOrdinal(row.Ordinal))
            .ToList();
        var bridgeLinks = bdv.BusinessBridgeLinkList
            .Where(row => row.BusinessBridgeId == bridge.Id)
            .OrderBy(row => ParseOrdinal(row.Ordinal))
            .ToList();

        if (bridgeHubs.Count == 0)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Name}' must declare at least one BusinessBridgeHub beyond AnchorHub.");
        }

        if (bridgeLinks.Count != bridgeHubs.Count)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Name}' must declare one BusinessBridgeLink for each hop from AnchorHub through ordered BusinessBridgeHub rows.");
        }

        var currentHub = bridge.AnchorHub;
        for (var index = 0; index < bridgeLinks.Count; index++)
        {
            var bridgeLink = bridgeLinks[index];
            var bridgeHub = bridgeHubs[index];
            if (!LinkConnectsHubs(bridgeLink.BusinessLink, currentHub.Id, bridgeHub.BusinessHub.Id, bdv))
            {
                throw new InvalidOperationException($"Bridge '{bridge.Name}' is inconsistent: link '{bridgeLink.BusinessLink.Name}' does not connect '{currentHub.Name}' to '{bridgeHub.BusinessHub.Name}' in ordinal path order.");
            }

            currentHub = bridgeHub.BusinessHub;
        }

        return new BridgePath(bridgeHubs[^1].BusinessHub);
    }

    private static bool LinkConnectsHubs(BDV.BusinessLink link, string firstHubId, string secondHubId, BDV.MetaBusinessDataVaultModel bdv)
    {
        var hubIds = bdv.BusinessLinkEndList
            .Where(row => row.BusinessLinkId == link.Id)
            .Select(row => row.BusinessHubId)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        return hubIds.Length == 2 &&
               hubIds.Contains(firstHubId, StringComparer.Ordinal) &&
               hubIds.Contains(secondHubId, StringComparer.Ordinal);
    }

    private static bool IsTrue(string? value)
    {
        return bool.TryParse(value, out var parsed) && parsed;
    }

    private static void EnsureRequiredBusinessImplementation(DataVaultImplementationModel implementation)
    {
        RequireImplementationValue(implementation.BusinessHubImplementation.HashKeyColumnName, "BusinessHubImplementation.HashKeyColumnName");
        RequireImplementationValue(implementation.BusinessHubImplementation.HashKeyDataTypeId, "BusinessHubImplementation.HashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessHubImplementation.HashKeyLength, "BusinessHubImplementation.HashKeyLength");
        RequireImplementationValue(implementation.BusinessHubImplementation.LoadTimestampColumnName, "BusinessHubImplementation.LoadTimestampColumnName");
        RequireImplementationValue(implementation.BusinessHubImplementation.LoadTimestampDataTypeId, "BusinessHubImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessHubImplementation.LoadTimestampPrecision, "BusinessHubImplementation.LoadTimestampPrecision");
        RequireImplementationValue(implementation.BusinessHubImplementation.RecordSourceColumnName, "BusinessHubImplementation.RecordSourceColumnName");
        RequireImplementationValue(implementation.BusinessHubImplementation.RecordSourceDataTypeId, "BusinessHubImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(implementation.BusinessHubImplementation.RecordSourceLength, "BusinessHubImplementation.RecordSourceLength");

        RequireImplementationValue(implementation.BusinessLinkImplementation.HashKeyColumnName, "BusinessLinkImplementation.HashKeyColumnName");
        RequireImplementationValue(implementation.BusinessLinkImplementation.HashKeyDataTypeId, "BusinessLinkImplementation.HashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessLinkImplementation.HashKeyLength, "BusinessLinkImplementation.HashKeyLength");
        RequireImplementationValue(implementation.BusinessLinkImplementation.EndHashKeyColumnPattern, "BusinessLinkImplementation.EndHashKeyColumnPattern");
        RequireImplementationValue(implementation.BusinessLinkImplementation.LoadTimestampColumnName, "BusinessLinkImplementation.LoadTimestampColumnName");
        RequireImplementationValue(implementation.BusinessLinkImplementation.LoadTimestampDataTypeId, "BusinessLinkImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessLinkImplementation.LoadTimestampPrecision, "BusinessLinkImplementation.LoadTimestampPrecision");
        RequireImplementationValue(implementation.BusinessLinkImplementation.RecordSourceColumnName, "BusinessLinkImplementation.RecordSourceColumnName");
        RequireImplementationValue(implementation.BusinessLinkImplementation.RecordSourceDataTypeId, "BusinessLinkImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(implementation.BusinessLinkImplementation.RecordSourceLength, "BusinessLinkImplementation.RecordSourceLength");

        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.ParentHashKeyColumnName, "BusinessHubSatelliteImplementation.ParentHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.ParentHashKeyDataTypeId, "BusinessHubSatelliteImplementation.ParentHashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.ParentHashKeyLength, "BusinessHubSatelliteImplementation.ParentHashKeyLength");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.HashDiffColumnName, "BusinessHubSatelliteImplementation.HashDiffColumnName");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.HashDiffDataTypeId, "BusinessHubSatelliteImplementation.HashDiffDataTypeId");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.HashDiffLength, "BusinessHubSatelliteImplementation.HashDiffLength");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.LoadTimestampColumnName, "BusinessHubSatelliteImplementation.LoadTimestampColumnName");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.LoadTimestampDataTypeId, "BusinessHubSatelliteImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.LoadTimestampPrecision, "BusinessHubSatelliteImplementation.LoadTimestampPrecision");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.RecordSourceColumnName, "BusinessHubSatelliteImplementation.RecordSourceColumnName");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.RecordSourceDataTypeId, "BusinessHubSatelliteImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.RecordSourceLength, "BusinessHubSatelliteImplementation.RecordSourceLength");

        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.ParentHashKeyColumnName, "BusinessLinkSatelliteImplementation.ParentHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.ParentHashKeyDataTypeId, "BusinessLinkSatelliteImplementation.ParentHashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.ParentHashKeyLength, "BusinessLinkSatelliteImplementation.ParentHashKeyLength");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.HashDiffColumnName, "BusinessLinkSatelliteImplementation.HashDiffColumnName");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.HashDiffDataTypeId, "BusinessLinkSatelliteImplementation.HashDiffDataTypeId");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.HashDiffLength, "BusinessLinkSatelliteImplementation.HashDiffLength");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.LoadTimestampColumnName, "BusinessLinkSatelliteImplementation.LoadTimestampColumnName");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.LoadTimestampDataTypeId, "BusinessLinkSatelliteImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.LoadTimestampPrecision, "BusinessLinkSatelliteImplementation.LoadTimestampPrecision");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.RecordSourceColumnName, "BusinessLinkSatelliteImplementation.RecordSourceColumnName");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.RecordSourceDataTypeId, "BusinessLinkSatelliteImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.RecordSourceLength, "BusinessLinkSatelliteImplementation.RecordSourceLength");

        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.ParentHashKeyColumnName, "BusinessPointInTimeImplementation.ParentHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.ParentHashKeyDataTypeId, "BusinessPointInTimeImplementation.ParentHashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.ParentHashKeyLength, "BusinessPointInTimeImplementation.ParentHashKeyLength");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SnapshotTimestampColumnName, "BusinessPointInTimeImplementation.SnapshotTimestampColumnName");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SnapshotTimestampDataTypeId, "BusinessPointInTimeImplementation.SnapshotTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SnapshotTimestampPrecision, "BusinessPointInTimeImplementation.SnapshotTimestampPrecision");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SatelliteReferenceColumnNamePattern, "BusinessPointInTimeImplementation.SatelliteReferenceColumnNamePattern");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SatelliteReferenceDataTypeId, "BusinessPointInTimeImplementation.SatelliteReferenceDataTypeId");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SatelliteReferencePrecision, "BusinessPointInTimeImplementation.SatelliteReferencePrecision");

        RequireImplementationValue(implementation.BusinessBridgeImplementation.RootHashKeyColumnName, "BusinessBridgeImplementation.RootHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.RootHashKeyDataTypeId, "BusinessBridgeImplementation.RootHashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.RootHashKeyLength, "BusinessBridgeImplementation.RootHashKeyLength");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.RelatedHashKeyColumnName, "BusinessBridgeImplementation.RelatedHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.RelatedHashKeyDataTypeId, "BusinessBridgeImplementation.RelatedHashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.RelatedHashKeyLength, "BusinessBridgeImplementation.RelatedHashKeyLength");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.DepthColumnName, "BusinessBridgeImplementation.DepthColumnName");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.DepthDataTypeId, "BusinessBridgeImplementation.DepthDataTypeId");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.PathColumnName, "BusinessBridgeImplementation.PathColumnName");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.PathDataTypeId, "BusinessBridgeImplementation.PathDataTypeId");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.PathLength, "BusinessBridgeImplementation.PathLength");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.EffectiveFromColumnName, "BusinessBridgeImplementation.EffectiveFromColumnName");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.EffectiveFromDataTypeId, "BusinessBridgeImplementation.EffectiveFromDataTypeId");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.EffectiveFromPrecision, "BusinessBridgeImplementation.EffectiveFromPrecision");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.EffectiveToColumnName, "BusinessBridgeImplementation.EffectiveToColumnName");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.EffectiveToDataTypeId, "BusinessBridgeImplementation.EffectiveToDataTypeId");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.EffectiveToPrecision, "BusinessBridgeImplementation.EffectiveToPrecision");
    }

    private static void RequireImplementationValue(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"MetaDataVaultImplementation is missing required property '{propertyName}' for current SQL generation.");
        }
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
    private readonly record struct BridgePath(BDV.BusinessHub RelatedHub);
}
