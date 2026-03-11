using Meta.Core.Ddl;
using MRDV = MetaRawDataVault;

namespace MetaDataVault.Core;

internal static class RawHubDdlConverter
{
    public static DdlTable Build(MRDV.RawHub hub, RawDataVaultSqlGenerationContext context)
    {
        var columns = new List<DdlColumn>();
        var usedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var hashKeyColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawHubImplementation.HashKeyColumnName);
        columns.Add(RawDataVaultSqlGenerationContext.RenderColumn(hashKeyColumnName, context.RenderSqlType(context.Implementation.RawHubImplementation.HashKeyDataTypeId, new SqlDataTypeDetail(context.Implementation.RawHubImplementation.HashKeyLength, null, null)), false));

        var keyParts = context.RawDataVault.RawHubKeyPartList
            .Where(row => row.RawHubId == hub.Id)
            .OrderBy(row => context.ParseOrdinal(row.Ordinal))
            .ThenBy(row => row.Name, StringComparer.Ordinal)
            .ToList();
        var uniqueColumns = new List<string>();

        foreach (var keyPart in keyParts)
        {
            var keyPartColumnName = RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, keyPart.Name);
            columns.Add(RawDataVaultSqlGenerationContext.RenderColumn(
                keyPartColumnName,
                context.RenderSqlType(keyPart.SourceField.DataTypeId, context.BuildSqlDataTypeDetail(context.RawDataVault.SourceFieldDataTypeDetailList.Where(detail => detail.SourceFieldId == keyPart.SourceFieldId).Select(detail => (detail.Name, detail.Value)))),
                IsNullable(keyPart.SourceField.IsNullable)));
            uniqueColumns.Add(keyPartColumnName);
        }

        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawHubImplementation.LoadTimestampColumnName), context.RenderSqlType(context.Implementation.RawHubImplementation.LoadTimestampDataTypeId, new SqlDataTypeDetail(null, context.Implementation.RawHubImplementation.LoadTimestampPrecision, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawHubImplementation.RecordSourceColumnName), context.RenderSqlType(context.Implementation.RawHubImplementation.RecordSourceDataTypeId, new SqlDataTypeDetail(context.Implementation.RawHubImplementation.RecordSourceLength, null, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, RawDataVaultSqlGenerationContext.ReserveColumnName(usedColumnNames, context.Implementation.RawHubImplementation.AuditIdColumnName), context.RenderSqlType(context.Implementation.RawHubImplementation.AuditIdDataTypeId, new SqlDataTypeDetail(null, null, null)));

        return RawDataVaultSqlGenerationContext.CreateTable(
            context.ResolveRawHubTableName(hub),
            columns,
            new[] { hashKeyColumnName },
            uniqueColumns.Count == 0 ? null : uniqueColumns,
            Array.Empty<DdlForeignKeyConstraint>());
    }

    private static bool IsNullable(string value) => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
}
