using Meta.Core.Ddl;
using MRDV = MetaRawDataVault;

namespace MetaDataVault.Core;

internal static class RawHubSatelliteDdlConverter
{
    public static DdlTable Build(MRDV.RawHubSatellite satellite, RawDataVaultSqlGenerationContext context)
    {
        EnsureSupported(satellite);

        var columns = new List<DdlColumn>();
        var usedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var parentHashKeyColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawHubSatelliteImplementation.ParentHashKeyColumnName);
        columns.Add(RawDataVaultSqlGenerationContext.RenderColumn(parentHashKeyColumnName, context.RenderSqlType(context.Implementation.RawHubSatelliteImplementation.ParentHashKeyDataTypeId, new SqlDataTypeDetail(context.Implementation.RawHubSatelliteImplementation.ParentHashKeyLength, null, null)), false));

        var attrs = context.RawDataVault.RawHubSatelliteAttributeList
            .Where(row => row.RawHubSatelliteId == satellite.Id)
            .OrderBy(row => context.ParseOrdinal(row.Ordinal))
            .ThenBy(row => row.Name, StringComparer.Ordinal)
            .ToList();

        foreach (var attr in attrs)
        {
            var attributeColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, attr.Name);
            columns.Add(RawDataVaultSqlGenerationContext.RenderColumn(
                attributeColumnName,
                context.RenderSqlType(attr.SourceField.DataTypeId, context.BuildSqlDataTypeDetail(context.RawDataVault.SourceFieldDataTypeDetailList.Where(detail => detail.SourceFieldId == attr.SourceFieldId).Select(detail => (detail.Name, detail.Value)))),
                IsNullable(attr.SourceField.IsNullable)));
        }

        var hashDiffColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawHubSatelliteImplementation.HashDiffColumnName);
        var loadTimestampColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawHubSatelliteImplementation.LoadTimestampColumnName);
        var recordSourceColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawHubSatelliteImplementation.RecordSourceColumnName);
        var auditIdColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawHubSatelliteImplementation.AuditIdColumnName);
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, hashDiffColumnName, context.RenderSqlType(context.Implementation.RawHubSatelliteImplementation.HashDiffDataTypeId, new SqlDataTypeDetail(context.Implementation.RawHubSatelliteImplementation.HashDiffLength, null, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, loadTimestampColumnName, context.RenderSqlType(context.Implementation.RawHubSatelliteImplementation.LoadTimestampDataTypeId, new SqlDataTypeDetail(null, context.Implementation.RawHubSatelliteImplementation.LoadTimestampPrecision, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, recordSourceColumnName, context.RenderSqlType(context.Implementation.RawHubSatelliteImplementation.RecordSourceDataTypeId, new SqlDataTypeDetail(context.Implementation.RawHubSatelliteImplementation.RecordSourceLength, null, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, auditIdColumnName, context.RenderSqlType(context.Implementation.RawHubSatelliteImplementation.AuditIdDataTypeId, new SqlDataTypeDetail(null, null, null)));

        var foreignKeys = new[]
        {
            RawDataVaultSqlGenerationContext.RenderForeignKeyConstraint($"FK_{context.ResolveRawHubSatelliteTableName(satellite)}_{satellite.RawHub.Name}", parentHashKeyColumnName, context.ResolveRawHubTableName(satellite.RawHub), context.Implementation.RawHubImplementation.HashKeyColumnName)
        };

        var primaryKey = new[] { parentHashKeyColumnName, loadTimestampColumnName };
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
