using Meta.Core.Ddl;
using MRDV = MetaRawDataVault;

namespace MetaDataVault.Core;

internal static class RawLinkSatelliteDdlConverter
{
    public static DdlTable Build(MRDV.RawLinkSatellite satellite, RawDataVaultSqlGenerationContext context)
    {
        EnsureSupported(satellite);

        var columns = new List<DdlColumn>();
        var usedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var parentHashKeyColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawLinkSatelliteImplementation.ParentHashKeyColumnName);
        columns.Add(RawDataVaultSqlGenerationContext.RenderColumn(parentHashKeyColumnName, context.RenderSqlType(context.Implementation.RawLinkSatelliteImplementation.ParentHashKeyDataTypeId, new SqlDataTypeDetail(context.Implementation.RawLinkSatelliteImplementation.ParentHashKeyLength, null, null)), false));

        var attrs = context.RawDataVault.RawLinkSatelliteAttributeList
            .Where(row => row.RawLinkSatelliteId == satellite.Id)
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

        var hashDiffColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawLinkSatelliteImplementation.HashDiffColumnName);
        var loadTimestampColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawLinkSatelliteImplementation.LoadTimestampColumnName);
        var recordSourceColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawLinkSatelliteImplementation.RecordSourceColumnName);
        var auditIdColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawLinkSatelliteImplementation.AuditIdColumnName);
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, hashDiffColumnName, context.RenderSqlType(context.Implementation.RawLinkSatelliteImplementation.HashDiffDataTypeId, new SqlDataTypeDetail(context.Implementation.RawLinkSatelliteImplementation.HashDiffLength, null, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, loadTimestampColumnName, context.RenderSqlType(context.Implementation.RawLinkSatelliteImplementation.LoadTimestampDataTypeId, new SqlDataTypeDetail(null, context.Implementation.RawLinkSatelliteImplementation.LoadTimestampPrecision, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, recordSourceColumnName, context.RenderSqlType(context.Implementation.RawLinkSatelliteImplementation.RecordSourceDataTypeId, new SqlDataTypeDetail(context.Implementation.RawLinkSatelliteImplementation.RecordSourceLength, null, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, auditIdColumnName, context.RenderSqlType(context.Implementation.RawLinkSatelliteImplementation.AuditIdDataTypeId, new SqlDataTypeDetail(null, null, null)));

        var foreignKeys = new[]
        {
            RawDataVaultSqlGenerationContext.RenderForeignKeyConstraint($"FK_{context.ResolveRawLinkSatelliteTableName(satellite)}_{satellite.RawLink.Name}", parentHashKeyColumnName, context.ResolveRawLinkTableName(satellite.RawLink), context.Implementation.RawLinkImplementation.HashKeyColumnName)
        };

        var primaryKey = new[] { parentHashKeyColumnName, loadTimestampColumnName };
        return RawDataVaultSqlGenerationContext.CreateTable(context.ResolveRawLinkSatelliteTableName(satellite), columns, primaryKey, null, foreignKeys);
    }

    private static void EnsureSupported(MRDV.RawLinkSatellite satellite)
    {
        if (!string.IsNullOrWhiteSpace(satellite.SatelliteKind) && !string.Equals(satellite.SatelliteKind, "standard", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"RawLinkSatellite '{satellite.Id}' uses unsupported SatelliteKind '{satellite.SatelliteKind}'.");
        }
    }

    private static bool IsNullable(string value) => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
}
