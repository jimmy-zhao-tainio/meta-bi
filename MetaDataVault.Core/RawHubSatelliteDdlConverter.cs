using Meta.Core.Ddl;
using MRDV = MetaRawDataVault;

namespace MetaDataVault.Core;

internal static class RawHubSatelliteDdlConverter
{
    public static DdlTable Build(MRDV.RawHubSatellite satellite, RawDataVaultSqlGenerationContext context)
    {
        EnsureSupported(satellite);

        var columns = new List<DdlColumn>
        {
            RawDataVaultSqlGenerationContext.RenderColumn(context.Implementation.RawHubSatelliteImplementation.ParentHashKeyColumnName, context.RenderSqlType(context.Implementation.RawHubSatelliteImplementation.ParentHashKeyDataTypeId, new SqlDataTypeDetail(context.Implementation.RawHubSatelliteImplementation.ParentHashKeyLength, null, null)), false)
        };

        var attrs = context.RawDataVault.RawHubSatelliteAttributeList
            .Where(row => row.RawHubSatelliteId == satellite.Id)
            .OrderBy(row => context.ParseOrdinal(row.Ordinal))
            .ThenBy(row => row.Name, StringComparer.Ordinal)
            .ToList();

        foreach (var attr in attrs)
        {
            columns.Add(RawDataVaultSqlGenerationContext.RenderColumn(
                attr.Name,
                context.RenderSqlType(attr.SourceField.DataTypeId, context.BuildSqlDataTypeDetail(context.RawDataVault.SourceFieldDataTypeDetailList.Where(detail => detail.SourceFieldId == attr.SourceFieldId).Select(detail => (detail.Name, detail.Value)))),
                IsNullable(attr.SourceField.IsNullable)));
        }

        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, context.Implementation.RawHubSatelliteImplementation.HashDiffColumnName, context.RenderSqlType(context.Implementation.RawHubSatelliteImplementation.HashDiffDataTypeId, new SqlDataTypeDetail(context.Implementation.RawHubSatelliteImplementation.HashDiffLength, null, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, context.Implementation.RawHubSatelliteImplementation.LoadTimestampColumnName, context.RenderSqlType(context.Implementation.RawHubSatelliteImplementation.LoadTimestampDataTypeId, new SqlDataTypeDetail(null, context.Implementation.RawHubSatelliteImplementation.LoadTimestampPrecision, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, context.Implementation.RawHubSatelliteImplementation.RecordSourceColumnName, context.RenderSqlType(context.Implementation.RawHubSatelliteImplementation.RecordSourceDataTypeId, new SqlDataTypeDetail(context.Implementation.RawHubSatelliteImplementation.RecordSourceLength, null, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, context.Implementation.RawHubSatelliteImplementation.AuditIdColumnName, context.RenderSqlType(context.Implementation.RawHubSatelliteImplementation.AuditIdDataTypeId, new SqlDataTypeDetail(null, null, null)));

        var foreignKeys = new[]
        {
            RawDataVaultSqlGenerationContext.RenderForeignKeyConstraint($"FK_{context.ResolveRawHubSatelliteTableName(satellite)}_{satellite.RawHub.Name}", context.Implementation.RawHubSatelliteImplementation.ParentHashKeyColumnName, context.ResolveRawHubTableName(satellite.RawHub), context.Implementation.RawHubImplementation.HashKeyColumnName)
        };

        var primaryKey = new[] { context.Implementation.RawHubSatelliteImplementation.ParentHashKeyColumnName, context.Implementation.RawHubSatelliteImplementation.LoadTimestampColumnName };
        return RawDataVaultSqlGenerationContext.CreateTable(context.ResolveRawHubSatelliteTableName(satellite), columns, primaryKey, null, foreignKeys);
    }

    private static void EnsureSupported(MRDV.RawHubSatellite satellite)
    {
        if (!string.IsNullOrWhiteSpace(satellite.SatelliteKind) && !string.Equals(satellite.SatelliteKind, "standard", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"RawHubSatellite '{satellite.Id}' uses unsupported SatelliteKind '{satellite.SatelliteKind}'.");
        }
    }

    private static bool IsNullable(string value) => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
}
