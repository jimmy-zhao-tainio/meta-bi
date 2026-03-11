using Meta.Core.Ddl;
using MRDV = MetaRawDataVault;

namespace MetaDataVault.Core;

internal static class RawLinkDdlConverter
{
    public static DdlTable Build(MRDV.RawLink link, RawDataVaultSqlGenerationContext context)
    {
        EnsureSupported(link);

        var columns = new List<DdlColumn>
        {
            RawDataVaultSqlGenerationContext.RenderColumn(context.Implementation.RawLinkImplementation.HashKeyColumnName, context.RenderSqlType(context.Implementation.RawLinkImplementation.HashKeyDataTypeId, new SqlDataTypeDetail(context.Implementation.RawLinkImplementation.HashKeyLength, null, null)), false)
        };
        var foreignKeys = new List<DdlForeignKeyConstraint>();
        var uniqueColumns = new List<string>();

        var hubs = context.RawDataVault.RawLinkHubList
            .Where(row => row.RawLinkId == link.Id)
            .OrderBy(row => context.ParseOrdinal(row.Ordinal))
            .ThenBy(row => row.RoleName, StringComparer.Ordinal)
            .ThenBy(row => row.RawHub.Name, StringComparer.Ordinal)
            .ToList();

        foreach (var hub in hubs)
        {
            var roleName = string.IsNullOrWhiteSpace(hub.RoleName) ? hub.RawHub.Name : hub.RoleName;
            var columnName = context.RenderPattern(context.Implementation.RawLinkImplementation.EndHashKeyColumnPattern, new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["RoleName"] = roleName,
                ["HubName"] = hub.RawHub.Name
            });
            columns.Add(RawDataVaultSqlGenerationContext.RenderColumn(columnName, context.RenderSqlType(context.Implementation.RawHubImplementation.HashKeyDataTypeId, new SqlDataTypeDetail(context.Implementation.RawHubImplementation.HashKeyLength, null, null)), false));
            foreignKeys.Add(RawDataVaultSqlGenerationContext.RenderForeignKeyConstraint($"FK_{context.ResolveRawLinkTableName(link)}_{hub.RawHub.Name}_{columnName}", columnName, context.ResolveRawHubTableName(hub.RawHub), context.Implementation.RawHubImplementation.HashKeyColumnName));
            uniqueColumns.Add(columnName);
        }

        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, context.Implementation.RawLinkImplementation.LoadTimestampColumnName, context.RenderSqlType(context.Implementation.RawLinkImplementation.LoadTimestampDataTypeId, new SqlDataTypeDetail(null, context.Implementation.RawLinkImplementation.LoadTimestampPrecision, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, context.Implementation.RawLinkImplementation.RecordSourceColumnName, context.RenderSqlType(context.Implementation.RawLinkImplementation.RecordSourceDataTypeId, new SqlDataTypeDetail(context.Implementation.RawLinkImplementation.RecordSourceLength, null, null)));
        RawDataVaultSqlGenerationContext.AppendRequiredColumn(columns, context.Implementation.RawLinkImplementation.AuditIdColumnName, context.RenderSqlType(context.Implementation.RawLinkImplementation.AuditIdDataTypeId, new SqlDataTypeDetail(null, null, null)));

        return RawDataVaultSqlGenerationContext.CreateTable(
            context.ResolveRawLinkTableName(link),
            columns,
            new[] { context.Implementation.RawLinkImplementation.HashKeyColumnName },
            uniqueColumns,
            foreignKeys);
    }

    private static void EnsureSupported(MRDV.RawLink link)
    {
        if (!string.IsNullOrWhiteSpace(link.LinkKind) && !string.Equals(link.LinkKind, "standard", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"RawLink '{link.Id}' uses unsupported LinkKind '{link.LinkKind}'.");
        }
    }
}
