using System.Text;
using System.Text.RegularExpressions;
using Meta.Core.Ddl;
using BDV = MetaBusinessDataVault;
using DVI = MetaDataVaultImplementation;

namespace MetaDataVault.Core;

public sealed record BusinessDataVaultSqlGenerationResult(
    string OutputPath,
    int FileCount,
    int TableCount,
    int BusinessHubCount,
    int BusinessLinkCount,
    int BusinessSameAsLinkCount,
    int BusinessHierarchicalLinkCount,
    int BusinessHubSatelliteCount,
    int BusinessLinkSatelliteCount,
    int BusinessSameAsLinkSatelliteCount,
    int BusinessHierarchicalLinkSatelliteCount,
    int BusinessReferenceCount,
    int BusinessReferenceSatelliteCount,
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

        foreach (var link in bdv.BusinessSameAsLinkList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{link.Name}.sql", RenderBusinessSameAsLinkSql(link, implementation, conversions)));
        }

        foreach (var link in bdv.BusinessHierarchicalLinkList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{link.Name}.sql", RenderBusinessHierarchicalLinkSql(link, implementation, conversions)));
        }

        foreach (var satellite in bdv.BusinessHubSatelliteList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{satellite.Name}.sql", RenderBusinessHubSatelliteSql(satellite, bdv, implementation, conversions)));
        }

        foreach (var satellite in bdv.BusinessLinkSatelliteList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{satellite.Name}.sql", RenderBusinessLinkSatelliteSql(satellite, bdv, implementation, conversions)));
        }

        foreach (var satellite in bdv.BusinessSameAsLinkSatelliteList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{satellite.Name}.sql", RenderBusinessSameAsLinkSatelliteSql(satellite, bdv, implementation, conversions)));
        }

        foreach (var satellite in bdv.BusinessHierarchicalLinkSatelliteList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{satellite.Name}.sql", RenderBusinessHierarchicalLinkSatelliteSql(satellite, bdv, implementation, conversions)));
        }

        foreach (var reference in bdv.BusinessReferenceList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{reference.Name}.sql", RenderBusinessReferenceSql(reference, bdv, implementation, conversions)));
        }

        foreach (var satellite in bdv.BusinessReferenceSatelliteList.OrderBy(row => row.Name, StringComparer.Ordinal))
        {
            scripts.Add(($"{satellite.Name}.sql", RenderBusinessReferenceSatelliteSql(satellite, bdv, implementation, conversions)));
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
            bdv.BusinessSameAsLinkList.Count,
            bdv.BusinessHierarchicalLinkList.Count,
            bdv.BusinessHubSatelliteList.Count,
            bdv.BusinessLinkSatelliteList.Count,
            bdv.BusinessSameAsLinkSatelliteList.Count,
            bdv.BusinessHierarchicalLinkSatelliteList.Count,
            bdv.BusinessReferenceList.Count,
            bdv.BusinessReferenceSatelliteList.Count,
            bdv.BusinessPointInTimeList.Count,
            bdv.BusinessBridgeList.Count);
    }

    private static string RenderBusinessHubSql(BDV.BusinessHub hub, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        var columns = new List<DdlColumn>
        {
            RenderColumn(implementation.BusinessHubImplementation.HashKeyColumnName, RenderSqlType(implementation.BusinessHubImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessHubImplementation.HashKeyLength, null, null), conversions), false)
        };

        foreach (var keyPart in bdv.BusinessHubKeyPartList.Where(row => row.BusinessHubId == hub.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(keyPart.Name, RenderSqlType(keyPart.DataTypeId, BuildDetailBag(bdv.BusinessHubKeyPartDataTypeDetailList.Where(detail => detail.BusinessHubKeyPartId == keyPart.Id).Select(detail => (detail.Name, detail.Value))), conversions), false));
        }

        AppendOptionalColumn(columns, implementation.BusinessHubImplementation.LoadTimestampColumnName, implementation.BusinessHubImplementation.LoadTimestampDataTypeId, new DetailBag(null, implementation.BusinessHubImplementation.LoadTimestampPrecision, null), conversions);
        AppendOptionalColumn(columns, implementation.BusinessHubImplementation.RecordSourceColumnName, implementation.BusinessHubImplementation.RecordSourceDataTypeId, new DetailBag(implementation.BusinessHubImplementation.RecordSourceLength, null, null), conversions);
        AppendRequiredColumn(columns, implementation.BusinessHubImplementation.AuditIdColumnName, implementation.BusinessHubImplementation.AuditIdDataTypeId, new DetailBag(null, null, null), conversions);

        var primaryKeyColumns = new[] { implementation.BusinessHubImplementation.HashKeyColumnName };
        var uniqueColumns = bdv.BusinessHubKeyPartList.Where(row => row.BusinessHubId == hub.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).Select(row => row.Name).ToList();

        return RenderCreateTableSql(hub.Name, columns, primaryKeyColumns, uniqueColumns.Count == 0 ? null : uniqueColumns, Array.Empty<DdlForeignKeyConstraint>());
    }

    private static string RenderBusinessLinkSql(BDV.BusinessLink link, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessLink(link, bdv);

        var columns = new List<DdlColumn>
        {
            RenderColumn(implementation.BusinessLinkImplementation.HashKeyColumnName, RenderSqlType(implementation.BusinessLinkImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessLinkImplementation.HashKeyLength, null, null), conversions), false)
        };
        var foreignKeys = new List<DdlForeignKeyConstraint>();
        var endColumns = new List<string>();

        foreach (var end in bdv.BusinessLinkHubList.Where(row => row.BusinessLinkId == link.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.RoleName, StringComparer.Ordinal))
        {
            var columnName = RenderPattern(implementation.BusinessLinkImplementation.EndHashKeyColumnPattern, new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["RoleName"] = end.RoleName,
                ["HubName"] = end.BusinessHub.Name
            });
            columns.Add(RenderColumn(columnName, RenderSqlType(implementation.BusinessHubImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessHubImplementation.HashKeyLength, null, null), conversions), false));
            foreignKeys.Add(RenderForeignKeyConstraint($"FK_{link.Name}_{end.BusinessHub.Name}_{columnName}", columnName, end.BusinessHub.Name, implementation.BusinessHubImplementation.HashKeyColumnName));
            endColumns.Add(columnName);
        }

        AppendOptionalColumn(columns, implementation.BusinessLinkImplementation.LoadTimestampColumnName, implementation.BusinessLinkImplementation.LoadTimestampDataTypeId, new DetailBag(null, implementation.BusinessLinkImplementation.LoadTimestampPrecision, null), conversions);
        AppendOptionalColumn(columns, implementation.BusinessLinkImplementation.RecordSourceColumnName, implementation.BusinessLinkImplementation.RecordSourceDataTypeId, new DetailBag(implementation.BusinessLinkImplementation.RecordSourceLength, null, null), conversions);
        AppendRequiredColumn(columns, implementation.BusinessLinkImplementation.AuditIdColumnName, implementation.BusinessLinkImplementation.AuditIdDataTypeId, new DetailBag(null, null, null), conversions);

        var primaryKeyColumns = new[] { implementation.BusinessLinkImplementation.HashKeyColumnName };
        return RenderCreateTableSql(link.Name, columns, primaryKeyColumns, endColumns.Count == 0 ? null : endColumns, foreignKeys);
    }

    private static string RenderBusinessSameAsLinkSql(BDV.BusinessSameAsLink link, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessSameAsLink(link);

        var columns = new List<DdlColumn>
        {
            RenderColumn(implementation.BusinessSameAsLinkImplementation.HashKeyColumnName, RenderSqlType(implementation.BusinessSameAsLinkImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessSameAsLinkImplementation.HashKeyLength, null, null), conversions), false),
            RenderColumn(implementation.BusinessSameAsLinkImplementation.PrimaryHashKeyColumnName, RenderSqlType(implementation.BusinessHubImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessHubImplementation.HashKeyLength, null, null), conversions), false),
            RenderColumn(implementation.BusinessSameAsLinkImplementation.EquivalentHashKeyColumnName, RenderSqlType(implementation.BusinessHubImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessHubImplementation.HashKeyLength, null, null), conversions), false)
        };

        AppendOptionalColumn(columns, implementation.BusinessSameAsLinkImplementation.LoadTimestampColumnName, implementation.BusinessSameAsLinkImplementation.LoadTimestampDataTypeId, new DetailBag(null, implementation.BusinessSameAsLinkImplementation.LoadTimestampPrecision, null), conversions);
        AppendOptionalColumn(columns, implementation.BusinessSameAsLinkImplementation.RecordSourceColumnName, implementation.BusinessSameAsLinkImplementation.RecordSourceDataTypeId, new DetailBag(implementation.BusinessSameAsLinkImplementation.RecordSourceLength, null, null), conversions);
        AppendRequiredColumn(columns, implementation.BusinessSameAsLinkImplementation.AuditIdColumnName, implementation.BusinessSameAsLinkImplementation.AuditIdDataTypeId, new DetailBag(null, null, null), conversions);

        var foreignKeys = new DdlForeignKeyConstraint[]
        {
            RenderForeignKeyConstraint($"FK_{link.Name}_{link.PrimaryHub.Name}_{implementation.BusinessSameAsLinkImplementation.PrimaryHashKeyColumnName}", implementation.BusinessSameAsLinkImplementation.PrimaryHashKeyColumnName, link.PrimaryHub.Name, implementation.BusinessHubImplementation.HashKeyColumnName),
            RenderForeignKeyConstraint($"FK_{link.Name}_{link.EquivalentHub.Name}_{implementation.BusinessSameAsLinkImplementation.EquivalentHashKeyColumnName}", implementation.BusinessSameAsLinkImplementation.EquivalentHashKeyColumnName, link.EquivalentHub.Name, implementation.BusinessHubImplementation.HashKeyColumnName)
        };

        return RenderCreateTableSql(
            link.Name,
            columns,
            new[] { implementation.BusinessSameAsLinkImplementation.HashKeyColumnName },
            new[] { implementation.BusinessSameAsLinkImplementation.PrimaryHashKeyColumnName, implementation.BusinessSameAsLinkImplementation.EquivalentHashKeyColumnName },
            foreignKeys);
    }

    private static string RenderBusinessHierarchicalLinkSql(BDV.BusinessHierarchicalLink link, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessHierarchicalLink(link);

        var columns = new List<DdlColumn>
        {
            RenderColumn(implementation.BusinessHierarchicalLinkImplementation.HashKeyColumnName, RenderSqlType(implementation.BusinessHierarchicalLinkImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessHierarchicalLinkImplementation.HashKeyLength, null, null), conversions), false),
            RenderColumn(implementation.BusinessHierarchicalLinkImplementation.ParentHashKeyColumnName, RenderSqlType(implementation.BusinessHubImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessHubImplementation.HashKeyLength, null, null), conversions), false),
            RenderColumn(implementation.BusinessHierarchicalLinkImplementation.ChildHashKeyColumnName, RenderSqlType(implementation.BusinessHubImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessHubImplementation.HashKeyLength, null, null), conversions), false)
        };

        AppendOptionalColumn(columns, implementation.BusinessHierarchicalLinkImplementation.LoadTimestampColumnName, implementation.BusinessHierarchicalLinkImplementation.LoadTimestampDataTypeId, new DetailBag(null, implementation.BusinessHierarchicalLinkImplementation.LoadTimestampPrecision, null), conversions);
        AppendOptionalColumn(columns, implementation.BusinessHierarchicalLinkImplementation.RecordSourceColumnName, implementation.BusinessHierarchicalLinkImplementation.RecordSourceDataTypeId, new DetailBag(implementation.BusinessHierarchicalLinkImplementation.RecordSourceLength, null, null), conversions);
        AppendRequiredColumn(columns, implementation.BusinessHierarchicalLinkImplementation.AuditIdColumnName, implementation.BusinessHierarchicalLinkImplementation.AuditIdDataTypeId, new DetailBag(null, null, null), conversions);

        var foreignKeys = new DdlForeignKeyConstraint[]
        {
            RenderForeignKeyConstraint($"FK_{link.Name}_{link.ParentHub.Name}_{implementation.BusinessHierarchicalLinkImplementation.ParentHashKeyColumnName}", implementation.BusinessHierarchicalLinkImplementation.ParentHashKeyColumnName, link.ParentHub.Name, implementation.BusinessHubImplementation.HashKeyColumnName),
            RenderForeignKeyConstraint($"FK_{link.Name}_{link.ChildHub.Name}_{implementation.BusinessHierarchicalLinkImplementation.ChildHashKeyColumnName}", implementation.BusinessHierarchicalLinkImplementation.ChildHashKeyColumnName, link.ChildHub.Name, implementation.BusinessHubImplementation.HashKeyColumnName)
        };

        return RenderCreateTableSql(
            link.Name,
            columns,
            new[] { implementation.BusinessHierarchicalLinkImplementation.HashKeyColumnName },
            new[] { implementation.BusinessHierarchicalLinkImplementation.ParentHashKeyColumnName, implementation.BusinessHierarchicalLinkImplementation.ChildHashKeyColumnName },
            foreignKeys);
    }


    private static string RenderBusinessReferenceSql(BDV.BusinessReference reference, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        var columns = new List<DdlColumn>
        {
            RenderColumn(implementation.BusinessReferenceImplementation.HashKeyColumnName, RenderSqlType(implementation.BusinessReferenceImplementation.HashKeyDataTypeId, new DetailBag(implementation.BusinessReferenceImplementation.HashKeyLength, null, null), conversions), false)
        };

        foreach (var keyPart in bdv.BusinessReferenceKeyPartList.Where(row => row.BusinessReferenceId == reference.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(keyPart.Name, RenderSqlType(keyPart.DataTypeId, BuildDetailBag(bdv.BusinessReferenceKeyPartDataTypeDetailList.Where(detail => detail.BusinessReferenceKeyPartId == keyPart.Id).Select(detail => (detail.Name, detail.Value))), conversions), false));
        }

        AppendOptionalColumn(columns, implementation.BusinessReferenceImplementation.LoadTimestampColumnName, implementation.BusinessReferenceImplementation.LoadTimestampDataTypeId, new DetailBag(null, implementation.BusinessReferenceImplementation.LoadTimestampPrecision, null), conversions);
        AppendOptionalColumn(columns, implementation.BusinessReferenceImplementation.RecordSourceColumnName, implementation.BusinessReferenceImplementation.RecordSourceDataTypeId, new DetailBag(implementation.BusinessReferenceImplementation.RecordSourceLength, null, null), conversions);
        AppendRequiredColumn(columns, implementation.BusinessReferenceImplementation.AuditIdColumnName, implementation.BusinessReferenceImplementation.AuditIdDataTypeId, new DetailBag(null, null, null), conversions);

        var primaryKeyColumns = new[] { implementation.BusinessReferenceImplementation.HashKeyColumnName };
        var uniqueColumns = bdv.BusinessReferenceKeyPartList.Where(row => row.BusinessReferenceId == reference.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).Select(row => row.Name).ToList();

        return RenderCreateTableSql(reference.Name, columns, primaryKeyColumns, uniqueColumns.Count == 0 ? null : uniqueColumns, Array.Empty<DdlForeignKeyConstraint>());
    }
    private static string RenderBusinessHubSatelliteSql(BDV.BusinessHubSatellite satellite, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessHubSatellite(satellite, bdv);

        var columns = new List<DdlColumn>
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
        AppendRequiredColumn(columns, implementation.BusinessHubSatelliteImplementation.AuditIdColumnName, implementation.BusinessHubSatelliteImplementation.AuditIdDataTypeId, new DetailBag(null, null, null), conversions);

        var foreignKeys = new DdlForeignKeyConstraint[]
        {
            RenderForeignKeyConstraint($"FK_{satellite.Name}_{satellite.BusinessHub.Name}", implementation.BusinessHubSatelliteImplementation.ParentHashKeyColumnName, satellite.BusinessHub.Name, implementation.BusinessHubImplementation.HashKeyColumnName)
        };

        return RenderCreateTableSql(satellite.Name, columns, primaryKeyColumns, null, foreignKeys);
    }

    private static string RenderBusinessLinkSatelliteSql(BDV.BusinessLinkSatellite satellite, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessLinkSatellite(satellite, bdv);

        var columns = new List<DdlColumn>
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
        AppendRequiredColumn(columns, implementation.BusinessLinkSatelliteImplementation.AuditIdColumnName, implementation.BusinessLinkSatelliteImplementation.AuditIdDataTypeId, new DetailBag(null, null, null), conversions);

        var foreignKeys = new DdlForeignKeyConstraint[]
        {
            RenderForeignKeyConstraint($"FK_{satellite.Name}_{satellite.BusinessLink.Name}", implementation.BusinessLinkSatelliteImplementation.ParentHashKeyColumnName, satellite.BusinessLink.Name, implementation.BusinessLinkImplementation.HashKeyColumnName)
        };

        return RenderCreateTableSql(satellite.Name, columns, primaryKeyColumns, null, foreignKeys);
    }

    private static string RenderBusinessSameAsLinkSatelliteSql(BDV.BusinessSameAsLinkSatellite satellite, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessSameAsLinkSatellite(satellite, bdv);
        return RenderLinkSatelliteSql(
            satellite.Name,
            satellite.BusinessSameAsLink.Name,
            implementation.BusinessSameAsLinkSatelliteImplementation.ParentHashKeyColumnName,
            implementation.BusinessSameAsLinkSatelliteImplementation.ParentHashKeyDataTypeId,
            implementation.BusinessSameAsLinkSatelliteImplementation.ParentHashKeyLength,
            implementation.BusinessSameAsLinkSatelliteImplementation.HashDiffColumnName,
            implementation.BusinessSameAsLinkSatelliteImplementation.HashDiffDataTypeId,
            implementation.BusinessSameAsLinkSatelliteImplementation.HashDiffLength,
            implementation.BusinessSameAsLinkSatelliteImplementation.LoadTimestampColumnName,
            implementation.BusinessSameAsLinkSatelliteImplementation.LoadTimestampDataTypeId,
            implementation.BusinessSameAsLinkSatelliteImplementation.LoadTimestampPrecision,
            implementation.BusinessSameAsLinkSatelliteImplementation.RecordSourceColumnName,
            implementation.BusinessSameAsLinkSatelliteImplementation.RecordSourceDataTypeId,
            implementation.BusinessSameAsLinkSatelliteImplementation.RecordSourceLength,
            implementation.BusinessSameAsLinkSatelliteImplementation.AuditIdColumnName,
            implementation.BusinessSameAsLinkSatelliteImplementation.AuditIdDataTypeId,
            implementation.BusinessSameAsLinkImplementation.HashKeyColumnName,
            bdv.BusinessSameAsLinkSatelliteKeyPartList.Where(row => row.BusinessSameAsLinkSatelliteId == satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal).Select(row => (row.Name, row.DataTypeId, BuildDetailBag(bdv.BusinessSameAsLinkSatelliteKeyPartDataTypeDetailList.Where(detail => detail.BusinessSameAsLinkSatelliteKeyPartId == row.Id).Select(detail => (detail.Name, detail.Value))))),
            bdv.BusinessSameAsLinkSatelliteAttributeList.Where(row => row.BusinessSameAsLinkSatelliteId == satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal).Select(row => (row.Name, row.DataTypeId, BuildDetailBag(bdv.BusinessSameAsLinkSatelliteAttributeDataTypeDetailList.Where(detail => detail.BusinessSameAsLinkSatelliteAttributeId == row.Id).Select(detail => (detail.Name, detail.Value))))),
            conversions);
    }

    private static string RenderBusinessHierarchicalLinkSatelliteSql(BDV.BusinessHierarchicalLinkSatellite satellite, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessHierarchicalLinkSatellite(satellite, bdv);
        return RenderLinkSatelliteSql(
            satellite.Name,
            satellite.BusinessHierarchicalLink.Name,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.ParentHashKeyColumnName,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.ParentHashKeyDataTypeId,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.ParentHashKeyLength,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.HashDiffColumnName,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.HashDiffDataTypeId,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.HashDiffLength,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.LoadTimestampColumnName,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.LoadTimestampDataTypeId,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.LoadTimestampPrecision,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.RecordSourceColumnName,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.RecordSourceDataTypeId,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.RecordSourceLength,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.AuditIdColumnName,
            implementation.BusinessHierarchicalLinkSatelliteImplementation.AuditIdDataTypeId,
            implementation.BusinessHierarchicalLinkImplementation.HashKeyColumnName,
            bdv.BusinessHierarchicalLinkSatelliteKeyPartList.Where(row => row.BusinessHierarchicalLinkSatelliteId == satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal).Select(row => (row.Name, row.DataTypeId, BuildDetailBag(bdv.BusinessHierarchicalLinkSatelliteKeyPartDataTypeDetailList.Where(detail => detail.BusinessHierarchicalLinkSatelliteKeyPartId == row.Id).Select(detail => (detail.Name, detail.Value))))),
            bdv.BusinessHierarchicalLinkSatelliteAttributeList.Where(row => row.BusinessHierarchicalLinkSatelliteId == satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal).Select(row => (row.Name, row.DataTypeId, BuildDetailBag(bdv.BusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList.Where(detail => detail.BusinessHierarchicalLinkSatelliteAttributeId == row.Id).Select(detail => (detail.Name, detail.Value))))),
            conversions);
    }


    private static string RenderBusinessReferenceSatelliteSql(BDV.BusinessReferenceSatellite satellite, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessReferenceSatellite(satellite, bdv);
        return RenderLinkSatelliteSql(
            satellite.Name,
            satellite.BusinessReference.Name,
            implementation.BusinessReferenceSatelliteImplementation.ParentHashKeyColumnName,
            implementation.BusinessReferenceSatelliteImplementation.ParentHashKeyDataTypeId,
            implementation.BusinessReferenceSatelliteImplementation.ParentHashKeyLength,
            implementation.BusinessReferenceSatelliteImplementation.HashDiffColumnName,
            implementation.BusinessReferenceSatelliteImplementation.HashDiffDataTypeId,
            implementation.BusinessReferenceSatelliteImplementation.HashDiffLength,
            implementation.BusinessReferenceSatelliteImplementation.LoadTimestampColumnName,
            implementation.BusinessReferenceSatelliteImplementation.LoadTimestampDataTypeId,
            implementation.BusinessReferenceSatelliteImplementation.LoadTimestampPrecision,
            implementation.BusinessReferenceSatelliteImplementation.RecordSourceColumnName,
            implementation.BusinessReferenceSatelliteImplementation.RecordSourceDataTypeId,
            implementation.BusinessReferenceSatelliteImplementation.RecordSourceLength,
            implementation.BusinessReferenceSatelliteImplementation.AuditIdColumnName,
            implementation.BusinessReferenceSatelliteImplementation.AuditIdDataTypeId,
            implementation.BusinessReferenceImplementation.HashKeyColumnName,
            bdv.BusinessReferenceSatelliteKeyPartList.Where(row => row.BusinessReferenceSatelliteId == satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal).Select(row => (row.Name, row.DataTypeId, BuildDetailBag(bdv.BusinessReferenceSatelliteKeyPartDataTypeDetailList.Where(detail => detail.BusinessReferenceSatelliteKeyPartId == row.Id).Select(detail => (detail.Name, detail.Value))))),
            bdv.BusinessReferenceSatelliteAttributeList.Where(row => row.BusinessReferenceSatelliteId == satellite.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal).Select(row => (row.Name, row.DataTypeId, BuildDetailBag(bdv.BusinessReferenceSatelliteAttributeDataTypeDetailList.Where(detail => detail.BusinessReferenceSatelliteAttributeId == row.Id).Select(detail => (detail.Name, detail.Value))))),
            conversions);
    }
    private static string RenderLinkSatelliteSql(
        string satelliteName,
        string parentName,
        string parentHashKeyColumnName,
        string parentHashKeyDataTypeId,
        string parentHashKeyLength,
        string? hashDiffColumnName,
        string? hashDiffDataTypeId,
        string? hashDiffLength,
        string? loadTimestampColumnName,
        string? loadTimestampDataTypeId,
        string? loadTimestampPrecision,
        string? recordSourceColumnName,
        string? recordSourceDataTypeId,
        string? recordSourceLength,
        string? auditIdColumnName,
        string? auditIdDataTypeId,
        string parentTableHashKeyColumnName,
        IEnumerable<(string Name, string DataTypeId, DetailBag Detail)> keyParts,
        IEnumerable<(string Name, string DataTypeId, DetailBag Detail)> attributes,
        DataTypeConversionModel conversions)
    {
        var columns = new List<DdlColumn>
        {
            RenderColumn(parentHashKeyColumnName, RenderSqlType(parentHashKeyDataTypeId, new DetailBag(parentHashKeyLength, null, null), conversions), false)
        };
        var primaryKeyColumns = new List<string> { parentHashKeyColumnName };

        foreach (var keyPart in keyParts)
        {
            columns.Add(RenderColumn(keyPart.Name, RenderSqlType(keyPart.DataTypeId, keyPart.Detail, conversions), false));
            primaryKeyColumns.Add(keyPart.Name);
        }

        foreach (var attribute in attributes)
        {
            columns.Add(RenderColumn(attribute.Name, RenderSqlType(attribute.DataTypeId, attribute.Detail, conversions), false));
        }

        AppendOptionalColumn(columns, hashDiffColumnName!, hashDiffDataTypeId!, new DetailBag(hashDiffLength, null, null), conversions);
        AppendOptionalColumn(columns, loadTimestampColumnName!, loadTimestampDataTypeId!, new DetailBag(null, loadTimestampPrecision, null), conversions, primaryKeyColumns);
        AppendOptionalColumn(columns, recordSourceColumnName!, recordSourceDataTypeId!, new DetailBag(recordSourceLength, null, null), conversions);
        AppendRequiredColumn(columns, auditIdColumnName!, auditIdDataTypeId!, new DetailBag(null, null, null), conversions);

        var foreignKeys = new DdlForeignKeyConstraint[]
        {
            RenderForeignKeyConstraint($"FK_{satelliteName}_{parentName}", parentHashKeyColumnName, parentName, parentTableHashKeyColumnName)
        };

        return RenderCreateTableSql(satelliteName, columns, primaryKeyColumns, null, foreignKeys);
    }

    private static string RenderBusinessPointInTimeSql(BDV.BusinessPointInTime pointInTime, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessPointInTime(pointInTime, bdv);

        var columns = new List<DdlColumn>
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

        AppendRequiredColumn(columns, implementation.BusinessPointInTimeImplementation.AuditIdColumnName, implementation.BusinessPointInTimeImplementation.AuditIdDataTypeId, new DetailBag(null, null, null), conversions);

        var foreignKeys = new DdlForeignKeyConstraint[]
        {
            RenderForeignKeyConstraint($"FK_{pointInTime.Name}_{pointInTime.BusinessHub.Name}", implementation.BusinessPointInTimeImplementation.ParentHashKeyColumnName, pointInTime.BusinessHub.Name, implementation.BusinessHubImplementation.HashKeyColumnName)
        };

        return RenderCreateTableSql(pointInTime.Name, columns, new[]
        {
            implementation.BusinessPointInTimeImplementation.ParentHashKeyColumnName,
            implementation.BusinessPointInTimeImplementation.SnapshotTimestampColumnName
        }, null, foreignKeys);
    }

    private static string RenderBusinessBridgeSql(BDV.BusinessBridge bridge, BDV.MetaBusinessDataVaultModel bdv, DataVaultImplementationModel implementation, DataTypeConversionModel conversions)
    {
        EnsureSupportedBusinessBridge(bridge, bdv);
        var bridgePath = ResolveBridgePath(bridge, bdv);

        var columns = new List<DdlColumn>
        {
            RenderColumn(implementation.BusinessBridgeImplementation.RootHashKeyColumnName, RenderSqlType(implementation.BusinessBridgeImplementation.RootHashKeyDataTypeId, new DetailBag(implementation.BusinessBridgeImplementation.RootHashKeyLength, null, null), conversions), false),
            RenderColumn(implementation.BusinessBridgeImplementation.RelatedHashKeyColumnName, RenderSqlType(implementation.BusinessBridgeImplementation.RelatedHashKeyDataTypeId, new DetailBag(implementation.BusinessBridgeImplementation.RelatedHashKeyLength, null, null), conversions), false)
        };
        var primaryKeyColumns = new List<string>
        {
            implementation.BusinessBridgeImplementation.RootHashKeyColumnName,
            implementation.BusinessBridgeImplementation.RelatedHashKeyColumnName
        };

        foreach (var projection in bdv.BusinessBridgeHubKeyPartProjectionList.Where(row => row.BusinessBridgeId == bridge.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(projection.Name, RenderSqlType(projection.BusinessHubKeyPart.DataTypeId, BuildDetailBag(bdv.BusinessHubKeyPartDataTypeDetailList.Where(detail => detail.BusinessHubKeyPartId == projection.BusinessHubKeyPartId).Select(detail => (detail.Name, detail.Value))), conversions), false));
        }

        foreach (var projection in bdv.BusinessBridgeHubSatelliteAttributeProjectionList.Where(row => row.BusinessBridgeId == bridge.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(projection.Name, RenderSqlType(projection.BusinessHubSatelliteAttribute.DataTypeId, BuildDetailBag(bdv.BusinessHubSatelliteAttributeDataTypeDetailList.Where(detail => detail.BusinessHubSatelliteAttributeId == projection.BusinessHubSatelliteAttributeId).Select(detail => (detail.Name, detail.Value))), conversions), false));
        }

        foreach (var projection in bdv.BusinessBridgeLinkSatelliteAttributeProjectionList.Where(row => row.BusinessBridgeId == bridge.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Name, StringComparer.Ordinal))
        {
            columns.Add(RenderColumn(projection.Name, RenderSqlType(projection.BusinessLinkSatelliteAttribute.DataTypeId, BuildDetailBag(bdv.BusinessLinkSatelliteAttributeDataTypeDetailList.Where(detail => detail.BusinessLinkSatelliteAttributeId == projection.BusinessLinkSatelliteAttributeId).Select(detail => (detail.Name, detail.Value))), conversions), false));
        }

        AppendOptionalColumn(columns, implementation.BusinessBridgeImplementation.DepthColumnName, implementation.BusinessBridgeImplementation.DepthDataTypeId, new DetailBag(null, null, null), conversions);
        AppendOptionalColumn(columns, implementation.BusinessBridgeImplementation.PathColumnName, implementation.BusinessBridgeImplementation.PathDataTypeId, new DetailBag(implementation.BusinessBridgeImplementation.PathLength, null, null), conversions);
        if (!string.IsNullOrWhiteSpace(implementation.BusinessBridgeImplementation.EffectiveFromColumnName) && !string.IsNullOrWhiteSpace(implementation.BusinessBridgeImplementation.EffectiveFromDataTypeId))
        {
            AppendOptionalColumn(columns, implementation.BusinessBridgeImplementation.EffectiveFromColumnName, implementation.BusinessBridgeImplementation.EffectiveFromDataTypeId, new DetailBag(null, implementation.BusinessBridgeImplementation.EffectiveFromPrecision, null), conversions);
            primaryKeyColumns.Add(implementation.BusinessBridgeImplementation.EffectiveFromColumnName);
        }
        AppendOptionalColumn(columns, implementation.BusinessBridgeImplementation.EffectiveToColumnName, implementation.BusinessBridgeImplementation.EffectiveToDataTypeId, new DetailBag(null, implementation.BusinessBridgeImplementation.EffectiveToPrecision, null), conversions);
        AppendRequiredColumn(columns, implementation.BusinessBridgeImplementation.AuditIdColumnName, implementation.BusinessBridgeImplementation.AuditIdDataTypeId, new DetailBag(null, null, null), conversions);

        var foreignKeys = new List<DdlForeignKeyConstraint>
        {
            RenderForeignKeyConstraint($"FK_{bridge.Name}_{bridge.AnchorHub.Name}_{implementation.BusinessBridgeImplementation.RootHashKeyColumnName}", implementation.BusinessBridgeImplementation.RootHashKeyColumnName, bridge.AnchorHub.Name, implementation.BusinessHubImplementation.HashKeyColumnName),
            RenderForeignKeyConstraint($"FK_{bridge.Name}_{bridgePath.RelatedHub.Name}_{implementation.BusinessBridgeImplementation.RelatedHashKeyColumnName}", implementation.BusinessBridgeImplementation.RelatedHashKeyColumnName, bridgePath.RelatedHub.Name, implementation.BusinessHubImplementation.HashKeyColumnName)
        };

        return RenderCreateTableSql(bridge.Name, columns, primaryKeyColumns, null, foreignKeys);
    }

    private static void AppendRequiredColumn(List<DdlColumn> columns, string columnName, string dataTypeId, DetailBag details, DataTypeConversionModel conversions)
    {
        RequireImplementationValue(columnName, "RequiredColumnName");
        RequireImplementationValue(dataTypeId, "RequiredDataTypeId");
        columns.Add(RenderColumn(columnName, RenderSqlType(dataTypeId, details, conversions), false));
    }

    private static void AppendOptionalColumn(List<DdlColumn> columns, string columnName, string dataTypeId, DetailBag details, DataTypeConversionModel conversions, List<string>? primaryKeyColumns = null)
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

    private static string RenderCreateTableSql(string tableName, IReadOnlyList<DdlColumn> columns, IEnumerable<string> primaryKeyColumns, IReadOnlyList<string>? uniqueColumns, IEnumerable<DdlForeignKeyConstraint> foreignKeys)
    {
        var table = new DdlTable
        {
            Schema = "dbo",
            Name = tableName,
            PrimaryKey = new DdlPrimaryKeyConstraint
            {
                Name = $"PK_{tableName}",
                IsClustered = true,
            },
        };
        table.Columns.AddRange(columns);
        table.PrimaryKey.ColumnNames.AddRange(primaryKeyColumns);
        if (uniqueColumns is not null && uniqueColumns.Count > 0)
        {
            var uniqueConstraint = new DdlUniqueConstraint
            {
                Name = $"UQ_{tableName}",
            };
            uniqueConstraint.ColumnNames.AddRange(uniqueColumns);
            table.UniqueConstraints.Add(uniqueConstraint);
        }

        table.ForeignKeys.AddRange(foreignKeys);

        var database = new DdlDatabase();
        database.Tables.Add(table);
        return DdlSqlServerRenderer.RenderSchema(database);
    }

    private static DdlForeignKeyConstraint RenderForeignKeyConstraint(string constraintName, string columnName, string referencedTableName, string referencedColumnName)
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

    private static DdlColumn RenderColumn(string columnName, string sqlType, bool isNullable)
    {
        return new DdlColumn
        {
            Name = columnName,
            DataType = sqlType,
            IsNullable = isNullable,
        };
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
        var endCount = bdv.BusinessLinkHubList.Count(row => row.BusinessLinkId == link.Id);
        if (endCount < 2)
        {
            throw new InvalidOperationException($"BusinessLink '{link.Name}' must declare at least two BusinessLinkHub rows.");
        }
    }

    private static void EnsureSupportedBusinessSameAsLink(BDV.BusinessSameAsLink link)
    {
        if (!string.Equals(link.PrimaryHubId, link.EquivalentHubId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"BusinessSameAsLink '{link.Name}' must bind two records from the same BusinessHub.");
        }
    }

    private static void EnsureSupportedBusinessHierarchicalLink(BDV.BusinessHierarchicalLink link)
    {
        if (!string.Equals(link.ParentHubId, link.ChildHubId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"BusinessHierarchicalLink '{link.Name}' must bind parent and child records from the same BusinessHub.");
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

        EnsureSupportedSatelliteKind(
            satellite.Name,
            satellite.SatelliteKind,
            satelliteKeyParts.Count,
            "BusinessLinkSatellite",
            "BusinessLinkSatelliteKeyPart");
    }

    private static void EnsureSupportedBusinessSameAsLinkSatellite(BDV.BusinessSameAsLinkSatellite satellite, BDV.MetaBusinessDataVaultModel bdv)
    {
        var satelliteKeyParts = bdv.BusinessSameAsLinkSatelliteKeyPartList
            .Where(row => row.BusinessSameAsLinkSatelliteId == satellite.Id)
            .ToList();

        EnsureSupportedSatelliteKind(
            satellite.Name,
            satellite.SatelliteKind,
            satelliteKeyParts.Count,
            "BusinessSameAsLinkSatellite",
            "BusinessSameAsLinkSatelliteKeyPart");
    }

    private static void EnsureSupportedBusinessHierarchicalLinkSatellite(BDV.BusinessHierarchicalLinkSatellite satellite, BDV.MetaBusinessDataVaultModel bdv)
    {
        var satelliteKeyParts = bdv.BusinessHierarchicalLinkSatelliteKeyPartList
            .Where(row => row.BusinessHierarchicalLinkSatelliteId == satellite.Id)
            .ToList();

        EnsureSupportedSatelliteKind(
            satellite.Name,
            satellite.SatelliteKind,
            satelliteKeyParts.Count,
            "BusinessHierarchicalLinkSatellite",
            "BusinessHierarchicalLinkSatelliteKeyPart");
    }

    private static void EnsureSupportedBusinessReferenceSatellite(BDV.BusinessReferenceSatellite satellite, BDV.MetaBusinessDataVaultModel bdv)
    {
        var satelliteKeyParts = bdv.BusinessReferenceSatelliteKeyPartList
            .Where(row => row.BusinessReferenceSatelliteId == satellite.Id)
            .ToList();

        EnsureSupportedSatelliteKind(
            satellite.Name,
            satellite.SatelliteKind,
            satelliteKeyParts.Count,
            "BusinessReferenceSatellite",
            "BusinessReferenceSatelliteKeyPart");
    }

    private static void EnsureSupportedSatelliteKind(string satelliteName, string satelliteKind, int keyPartCount, string entityName, string keyPartEntityName)
    {
        if (!string.Equals(satelliteKind, "standard", StringComparison.Ordinal) &&
            !string.Equals(satelliteKind, "multi-active", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"SQL generation currently supports {entityName}.SatelliteKind values 'standard' and 'multi-active'. Satellite '{satelliteName}' uses '{satelliteKind}'.");
        }

        if (string.Equals(satelliteKind, "standard", StringComparison.Ordinal) && keyPartCount > 0)
        {
            throw new InvalidOperationException($"SQL generation does not allow {entityName}.SatelliteKind='standard' with {keyPartEntityName} rows. Satellite '{satelliteName}' declares {keyPartCount} key part(s).");
        }

        if (string.Equals(satelliteKind, "multi-active", StringComparison.Ordinal) && keyPartCount == 0)
        {
            throw new InvalidOperationException($"SQL generation requires {entityName}.SatelliteKind='multi-active' satellites to declare at least one {keyPartEntityName}. Satellite '{satelliteName}' declares none.");
        }
    }

    private static void EnsureSupportedBusinessPointInTime(BDV.BusinessPointInTime pointInTime, BDV.MetaBusinessDataVaultModel bdv)
    {
        var hubSatelliteRows = bdv.BusinessPointInTimeHubSatelliteList
            .Where(row => row.BusinessPointInTimeId == pointInTime.Id)
            .OrderBy(row => ParseOrdinal(row.Ordinal))
            .ToList();
        var linkSatelliteRows = bdv.BusinessPointInTimeLinkSatelliteList
            .Where(row => row.BusinessPointInTimeId == pointInTime.Id)
            .OrderBy(row => ParseOrdinal(row.Ordinal))
            .ToList();
        var stampRows = bdv.BusinessPointInTimeStampList
            .Where(row => row.BusinessPointInTimeId == pointInTime.Id)
            .OrderBy(row => ParseOrdinal(row.Ordinal))
            .ToList();

        if (stampRows.Count > 0)
        {
            throw new InvalidOperationException($"SQL generation does not yet support BusinessPointInTimeStamp rows. Point-in-time '{pointInTime.Name}' declares {stampRows.Count} explicit stamp row(s).");
        }


        if (hubSatelliteRows.Count == 0 && linkSatelliteRows.Count == 0)
        {
            throw new InvalidOperationException($"BusinessPointInTime '{pointInTime.Name}' must reference at least one hub or link satellite.");
        }

        var duplicateOrdinals = hubSatelliteRows.Select(row => row.Ordinal)
            .Concat(linkSatelliteRows.Select(row => row.Ordinal))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .GroupBy(value => value, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
        if (duplicateOrdinals.Length > 0)
        {
            throw new InvalidOperationException($"BusinessPointInTime '{pointInTime.Name}' declares duplicate Ordinal values: {string.Join(", ", duplicateOrdinals)}.");
        }

        var multiActiveHubSatellite = hubSatelliteRows
            .Select(row => row.BusinessHubSatellite)
            .FirstOrDefault(row => string.Equals(row.SatelliteKind, "multi-active", StringComparison.Ordinal));
        if (multiActiveHubSatellite is not null)
        {
            throw new InvalidOperationException($"SQL generation does not yet support BusinessPointInTime references to multi-active hub satellites. Point-in-time '{pointInTime.Name}' references '{multiActiveHubSatellite.Name}'.");
        }

        var multiActiveLinkSatellite = linkSatelliteRows
            .Select(row => row.BusinessLinkSatellite)
            .FirstOrDefault(row => string.Equals(row.SatelliteKind, "multi-active", StringComparison.Ordinal));
        if (multiActiveLinkSatellite is not null)
        {
            throw new InvalidOperationException($"SQL generation does not yet support BusinessPointInTime references to multi-active link satellites. Point-in-time '{pointInTime.Name}' references '{multiActiveLinkSatellite.Name}'.");
        }

        var wrongHubSatellite = hubSatelliteRows
            .Select(row => row.BusinessHubSatellite)
            .FirstOrDefault(row => !string.Equals(row.BusinessHubId, pointInTime.BusinessHubId, StringComparison.Ordinal));
        if (wrongHubSatellite is not null)
        {
            throw new InvalidOperationException($"BusinessPointInTime '{pointInTime.Name}' can only reference BusinessHubSatellite rows belonging to hub '{pointInTime.BusinessHub.Name}'. Satellite '{wrongHubSatellite.Name}' belongs to '{wrongHubSatellite.BusinessHub.Name}'.");
        }

        var wrongLinkSatellite = linkSatelliteRows
            .Select(row => row.BusinessLinkSatellite)
            .FirstOrDefault(row => !bdv.BusinessLinkHubList
                .Where(end => end.BusinessLinkId == row.BusinessLinkId)
                .Any(end => string.Equals(end.BusinessHubId, pointInTime.BusinessHubId, StringComparison.Ordinal)));
        if (wrongLinkSatellite is not null)
        {
            throw new InvalidOperationException($"BusinessPointInTime '{pointInTime.Name}' can only reference BusinessLinkSatellite rows whose link connects to hub '{pointInTime.BusinessHub.Name}'. Satellite '{wrongLinkSatellite.Name}' belongs to link '{wrongLinkSatellite.BusinessLink.Name}', which does not connect to that hub.");
        }
    }

    private static void EnsureSupportedBusinessBridge(BDV.BusinessBridge bridge, BDV.MetaBusinessDataVaultModel bdv)
    {

        var hubKeyPartProjections = bdv.BusinessBridgeHubKeyPartProjectionList
            .Where(row => row.BusinessBridgeId == bridge.Id)
            .OrderBy(row => ParseOrdinal(row.Ordinal))
            .ThenBy(row => row.Name, StringComparer.Ordinal)
            .ToList();
        var hubSatelliteAttributeProjections = bdv.BusinessBridgeHubSatelliteAttributeProjectionList
            .Where(row => row.BusinessBridgeId == bridge.Id)
            .OrderBy(row => ParseOrdinal(row.Ordinal))
            .ThenBy(row => row.Name, StringComparer.Ordinal)
            .ToList();
        var linkSatelliteAttributeProjections = bdv.BusinessBridgeLinkSatelliteAttributeProjectionList
            .Where(row => row.BusinessBridgeId == bridge.Id)
            .OrderBy(row => ParseOrdinal(row.Ordinal))
            .ThenBy(row => row.Name, StringComparer.Ordinal)
            .ToList();

        if (hubKeyPartProjections.Count == 0 && hubSatelliteAttributeProjections.Count == 0 && linkSatelliteAttributeProjections.Count == 0)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Name}' must declare at least one bridge projection row.");
        }

        var duplicateProjectionNames = hubKeyPartProjections.Select(row => row.Name)
            .Concat(hubSatelliteAttributeProjections.Select(row => row.Name))
            .Concat(linkSatelliteAttributeProjections.Select(row => row.Name))
            .GroupBy(value => value, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
        if (duplicateProjectionNames.Length > 0)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Name}' declares duplicate projection names: {string.Join(", ", duplicateProjectionNames)}.");
        }

        var duplicateOrdinals = hubKeyPartProjections.Select(row => row.Ordinal)
            .Concat(hubSatelliteAttributeProjections.Select(row => row.Ordinal))
            .Concat(linkSatelliteAttributeProjections.Select(row => row.Ordinal))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .GroupBy(value => value, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
        if (duplicateOrdinals.Length > 0)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Name}' declares duplicate Ordinal values across bridge projections: {string.Join(", ", duplicateOrdinals)}.");
        }

        var path = ResolveBridgePath(bridge, bdv);
        var allowedHubIds = new HashSet<string>(path.HubIds, StringComparer.Ordinal);
        var allowedLinkIds = new HashSet<string>(path.LinkIds, StringComparer.Ordinal);

        var wrongHubKeyPart = hubKeyPartProjections
            .Select(row => row.BusinessHubKeyPart)
            .FirstOrDefault(row => !allowedHubIds.Contains(row.BusinessHubId));
        if (wrongHubKeyPart is not null)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Name}' can only project BusinessHubKeyPart rows from hubs on its ordered path. Key part '{wrongHubKeyPart.Name}' belongs to hub '{wrongHubKeyPart.BusinessHub.Name}'.");
        }

        var wrongHubSatelliteAttribute = hubSatelliteAttributeProjections
            .Select(row => row.BusinessHubSatelliteAttribute)
            .FirstOrDefault(row => !allowedHubIds.Contains(row.BusinessHubSatellite.BusinessHubId));
        if (wrongHubSatelliteAttribute is not null)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Name}' can only project BusinessHubSatelliteAttribute rows from hubs on its ordered path. Attribute '{wrongHubSatelliteAttribute.Name}' belongs to satellite '{wrongHubSatelliteAttribute.BusinessHubSatellite.Name}' on hub '{wrongHubSatelliteAttribute.BusinessHubSatellite.BusinessHub.Name}'.");
        }

        var wrongLinkSatelliteAttribute = linkSatelliteAttributeProjections
            .Select(row => row.BusinessLinkSatelliteAttribute)
            .FirstOrDefault(row => !allowedLinkIds.Contains(row.BusinessLinkSatellite.BusinessLinkId));
        if (wrongLinkSatelliteAttribute is not null)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Name}' can only project BusinessLinkSatelliteAttribute rows from links on its ordered path. Attribute '{wrongLinkSatelliteAttribute.Name}' belongs to satellite '{wrongLinkSatelliteAttribute.BusinessLinkSatellite.Name}' on link '{wrongLinkSatelliteAttribute.BusinessLinkSatellite.BusinessLink.Name}'.");
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

        return new BridgePath(bridgeHubs[^1].BusinessHub, new[] { bridge.AnchorHubId }.Concat(bridgeHubs.Select(row => row.BusinessHubId)).ToArray(), bridgeLinks.Select(row => row.BusinessLinkId).ToArray());
    }

    private static bool LinkConnectsHubs(BDV.BusinessLink link, string firstHubId, string secondHubId, BDV.MetaBusinessDataVaultModel bdv)
    {
        var hubIds = bdv.BusinessLinkHubList
            .Where(row => row.BusinessLinkId == link.Id)
            .Select(row => row.BusinessHubId)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        return hubIds.Length == 2 &&
               hubIds.Contains(firstHubId, StringComparer.Ordinal) &&
               hubIds.Contains(secondHubId, StringComparer.Ordinal);
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
        RequireImplementationValue(implementation.BusinessHubImplementation.AuditIdColumnName, "BusinessHubImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessHubImplementation.AuditIdDataTypeId, "BusinessHubImplementation.AuditIdDataTypeId");

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
        RequireImplementationValue(implementation.BusinessLinkImplementation.AuditIdColumnName, "BusinessLinkImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessLinkImplementation.AuditIdDataTypeId, "BusinessLinkImplementation.AuditIdDataTypeId");

        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.HashKeyColumnName, "BusinessSameAsLinkImplementation.HashKeyColumnName");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.HashKeyDataTypeId, "BusinessSameAsLinkImplementation.HashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.HashKeyLength, "BusinessSameAsLinkImplementation.HashKeyLength");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.PrimaryHashKeyColumnName, "BusinessSameAsLinkImplementation.PrimaryHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.EquivalentHashKeyColumnName, "BusinessSameAsLinkImplementation.EquivalentHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.LoadTimestampColumnName, "BusinessSameAsLinkImplementation.LoadTimestampColumnName");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.LoadTimestampDataTypeId, "BusinessSameAsLinkImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.LoadTimestampPrecision, "BusinessSameAsLinkImplementation.LoadTimestampPrecision");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.RecordSourceColumnName, "BusinessSameAsLinkImplementation.RecordSourceColumnName");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.RecordSourceDataTypeId, "BusinessSameAsLinkImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.RecordSourceLength, "BusinessSameAsLinkImplementation.RecordSourceLength");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.AuditIdColumnName, "BusinessSameAsLinkImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessSameAsLinkImplementation.AuditIdDataTypeId, "BusinessSameAsLinkImplementation.AuditIdDataTypeId");

        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.HashKeyColumnName, "BusinessHierarchicalLinkImplementation.HashKeyColumnName");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.HashKeyDataTypeId, "BusinessHierarchicalLinkImplementation.HashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.HashKeyLength, "BusinessHierarchicalLinkImplementation.HashKeyLength");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.ParentHashKeyColumnName, "BusinessHierarchicalLinkImplementation.ParentHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.ChildHashKeyColumnName, "BusinessHierarchicalLinkImplementation.ChildHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.LoadTimestampColumnName, "BusinessHierarchicalLinkImplementation.LoadTimestampColumnName");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.LoadTimestampDataTypeId, "BusinessHierarchicalLinkImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.LoadTimestampPrecision, "BusinessHierarchicalLinkImplementation.LoadTimestampPrecision");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.RecordSourceColumnName, "BusinessHierarchicalLinkImplementation.RecordSourceColumnName");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.RecordSourceDataTypeId, "BusinessHierarchicalLinkImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.RecordSourceLength, "BusinessHierarchicalLinkImplementation.RecordSourceLength");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.AuditIdColumnName, "BusinessHierarchicalLinkImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkImplementation.AuditIdDataTypeId, "BusinessHierarchicalLinkImplementation.AuditIdDataTypeId");

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
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.AuditIdColumnName, "BusinessHubSatelliteImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessHubSatelliteImplementation.AuditIdDataTypeId, "BusinessHubSatelliteImplementation.AuditIdDataTypeId");

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
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.AuditIdColumnName, "BusinessLinkSatelliteImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessLinkSatelliteImplementation.AuditIdDataTypeId, "BusinessLinkSatelliteImplementation.AuditIdDataTypeId");

        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.ParentHashKeyColumnName, "BusinessSameAsLinkSatelliteImplementation.ParentHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.ParentHashKeyDataTypeId, "BusinessSameAsLinkSatelliteImplementation.ParentHashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.ParentHashKeyLength, "BusinessSameAsLinkSatelliteImplementation.ParentHashKeyLength");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.HashDiffColumnName, "BusinessSameAsLinkSatelliteImplementation.HashDiffColumnName");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.HashDiffDataTypeId, "BusinessSameAsLinkSatelliteImplementation.HashDiffDataTypeId");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.HashDiffLength, "BusinessSameAsLinkSatelliteImplementation.HashDiffLength");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.LoadTimestampColumnName, "BusinessSameAsLinkSatelliteImplementation.LoadTimestampColumnName");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.LoadTimestampDataTypeId, "BusinessSameAsLinkSatelliteImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.LoadTimestampPrecision, "BusinessSameAsLinkSatelliteImplementation.LoadTimestampPrecision");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.RecordSourceColumnName, "BusinessSameAsLinkSatelliteImplementation.RecordSourceColumnName");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.RecordSourceDataTypeId, "BusinessSameAsLinkSatelliteImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.RecordSourceLength, "BusinessSameAsLinkSatelliteImplementation.RecordSourceLength");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.AuditIdColumnName, "BusinessSameAsLinkSatelliteImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessSameAsLinkSatelliteImplementation.AuditIdDataTypeId, "BusinessSameAsLinkSatelliteImplementation.AuditIdDataTypeId");

        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.ParentHashKeyColumnName, "BusinessHierarchicalLinkSatelliteImplementation.ParentHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.ParentHashKeyDataTypeId, "BusinessHierarchicalLinkSatelliteImplementation.ParentHashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.ParentHashKeyLength, "BusinessHierarchicalLinkSatelliteImplementation.ParentHashKeyLength");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.HashDiffColumnName, "BusinessHierarchicalLinkSatelliteImplementation.HashDiffColumnName");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.HashDiffDataTypeId, "BusinessHierarchicalLinkSatelliteImplementation.HashDiffDataTypeId");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.HashDiffLength, "BusinessHierarchicalLinkSatelliteImplementation.HashDiffLength");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.LoadTimestampColumnName, "BusinessHierarchicalLinkSatelliteImplementation.LoadTimestampColumnName");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.LoadTimestampDataTypeId, "BusinessHierarchicalLinkSatelliteImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.LoadTimestampPrecision, "BusinessHierarchicalLinkSatelliteImplementation.LoadTimestampPrecision");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.RecordSourceColumnName, "BusinessHierarchicalLinkSatelliteImplementation.RecordSourceColumnName");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.RecordSourceDataTypeId, "BusinessHierarchicalLinkSatelliteImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.RecordSourceLength, "BusinessHierarchicalLinkSatelliteImplementation.RecordSourceLength");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.AuditIdColumnName, "BusinessHierarchicalLinkSatelliteImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessHierarchicalLinkSatelliteImplementation.AuditIdDataTypeId, "BusinessHierarchicalLinkSatelliteImplementation.AuditIdDataTypeId");

        RequireImplementationValue(implementation.BusinessReferenceImplementation.HashKeyColumnName, "BusinessReferenceImplementation.HashKeyColumnName");
        RequireImplementationValue(implementation.BusinessReferenceImplementation.HashKeyDataTypeId, "BusinessReferenceImplementation.HashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessReferenceImplementation.HashKeyLength, "BusinessReferenceImplementation.HashKeyLength");
        RequireImplementationValue(implementation.BusinessReferenceImplementation.LoadTimestampColumnName, "BusinessReferenceImplementation.LoadTimestampColumnName");
        RequireImplementationValue(implementation.BusinessReferenceImplementation.LoadTimestampDataTypeId, "BusinessReferenceImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessReferenceImplementation.LoadTimestampPrecision, "BusinessReferenceImplementation.LoadTimestampPrecision");
        RequireImplementationValue(implementation.BusinessReferenceImplementation.RecordSourceColumnName, "BusinessReferenceImplementation.RecordSourceColumnName");
        RequireImplementationValue(implementation.BusinessReferenceImplementation.RecordSourceDataTypeId, "BusinessReferenceImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(implementation.BusinessReferenceImplementation.RecordSourceLength, "BusinessReferenceImplementation.RecordSourceLength");
        RequireImplementationValue(implementation.BusinessReferenceImplementation.AuditIdColumnName, "BusinessReferenceImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessReferenceImplementation.AuditIdDataTypeId, "BusinessReferenceImplementation.AuditIdDataTypeId");

        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.ParentHashKeyColumnName, "BusinessReferenceSatelliteImplementation.ParentHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.ParentHashKeyDataTypeId, "BusinessReferenceSatelliteImplementation.ParentHashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.ParentHashKeyLength, "BusinessReferenceSatelliteImplementation.ParentHashKeyLength");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.HashDiffColumnName, "BusinessReferenceSatelliteImplementation.HashDiffColumnName");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.HashDiffDataTypeId, "BusinessReferenceSatelliteImplementation.HashDiffDataTypeId");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.HashDiffLength, "BusinessReferenceSatelliteImplementation.HashDiffLength");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.LoadTimestampColumnName, "BusinessReferenceSatelliteImplementation.LoadTimestampColumnName");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.LoadTimestampDataTypeId, "BusinessReferenceSatelliteImplementation.LoadTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.LoadTimestampPrecision, "BusinessReferenceSatelliteImplementation.LoadTimestampPrecision");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.RecordSourceColumnName, "BusinessReferenceSatelliteImplementation.RecordSourceColumnName");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.RecordSourceDataTypeId, "BusinessReferenceSatelliteImplementation.RecordSourceDataTypeId");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.RecordSourceLength, "BusinessReferenceSatelliteImplementation.RecordSourceLength");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.AuditIdColumnName, "BusinessReferenceSatelliteImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessReferenceSatelliteImplementation.AuditIdDataTypeId, "BusinessReferenceSatelliteImplementation.AuditIdDataTypeId");

        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.ParentHashKeyColumnName, "BusinessPointInTimeImplementation.ParentHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.ParentHashKeyDataTypeId, "BusinessPointInTimeImplementation.ParentHashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.ParentHashKeyLength, "BusinessPointInTimeImplementation.ParentHashKeyLength");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SnapshotTimestampColumnName, "BusinessPointInTimeImplementation.SnapshotTimestampColumnName");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SnapshotTimestampDataTypeId, "BusinessPointInTimeImplementation.SnapshotTimestampDataTypeId");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SnapshotTimestampPrecision, "BusinessPointInTimeImplementation.SnapshotTimestampPrecision");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SatelliteReferenceColumnNamePattern, "BusinessPointInTimeImplementation.SatelliteReferenceColumnNamePattern");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SatelliteReferenceDataTypeId, "BusinessPointInTimeImplementation.SatelliteReferenceDataTypeId");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.AuditIdColumnName, "BusinessPointInTimeImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.AuditIdDataTypeId, "BusinessPointInTimeImplementation.AuditIdDataTypeId");
        RequireImplementationValue(implementation.BusinessPointInTimeImplementation.SatelliteReferencePrecision, "BusinessPointInTimeImplementation.SatelliteReferencePrecision");

        RequireImplementationValue(implementation.BusinessBridgeImplementation.RootHashKeyColumnName, "BusinessBridgeImplementation.RootHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.RootHashKeyDataTypeId, "BusinessBridgeImplementation.RootHashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.RootHashKeyLength, "BusinessBridgeImplementation.RootHashKeyLength");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.RelatedHashKeyColumnName, "BusinessBridgeImplementation.RelatedHashKeyColumnName");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.RelatedHashKeyDataTypeId, "BusinessBridgeImplementation.RelatedHashKeyDataTypeId");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.AuditIdColumnName, "BusinessBridgeImplementation.AuditIdColumnName");
        RequireImplementationValue(implementation.BusinessBridgeImplementation.AuditIdDataTypeId, "BusinessBridgeImplementation.AuditIdDataTypeId");
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

    private static string RequireNumber(string? value, string dataTypeId, string detailName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Data type '{dataTypeId}' requires detail '{detailName}' for SQL generation.");
        }

        return value;
    }

    private readonly record struct DetailBag(string? Length, string? Precision, string? Scale);
    private readonly record struct BridgePath(BDV.BusinessHub RelatedHub, IReadOnlyList<string> HubIds, IReadOnlyList<string> LinkIds);
}














