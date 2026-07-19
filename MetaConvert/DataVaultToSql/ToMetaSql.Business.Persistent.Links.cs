using MetaBusinessDataVault;
using MetaDataVaultImplementation;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

public static partial class Converter
{
    private static void PopulateBusinessLinks(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        BusinessHubImplementation businessHubImplementation,
        BusinessLinkImplementation businessLinkImplementation,
        IReadOnlyDictionary<string, List<BusinessLinkRole>> businessLinkRolesByLinkId,
        IReadOnlyDictionary<string, Table> hubTablesByHubId,
        IReadOnlyDictionary<string, TableColumn> hubHashKeyColumnsByHubId,
        Dictionary<string, Table> linkTablesByLinkId,
        Dictionary<string, TableColumn> linkHashKeyColumnsByLinkId)
    {
        foreach (var link in model.BusinessLinkList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessLinkImplementation.SchemaName,
                ApplyPattern(businessLinkImplementation.TableNamePattern, ("Name", link.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessLinkImplementation.HashKeyColumnName,
                businessLinkImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessLinkImplementation.HashKeyLength));

            foreach (var linkRole in GetGroup(businessLinkRolesByLinkId, link.Id)
                .OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                var endHashKeyColumn = AddImplementationColumn(
                    context,
                    table,
                    ApplyPattern(businessLinkImplementation.EndHashKeyColumnPattern, ("RoleName", linkRole.Name)),
                    businessHubImplementation.HashKeyDataTypeId,
                    "false",
                    reservedColumnNames,
                    ("Length", businessHubImplementation.HashKeyLength));

                if (hubTablesByHubId.TryGetValue(linkRole.BusinessHub.Id, out var targetHubTable) &&
                    hubHashKeyColumnsByHubId.TryGetValue(linkRole.BusinessHub.Id, out var targetHubHashKey))
                {
                    AddForeignKey(
                        context,
                        table,
                        ApplyPattern(
                            businessLinkImplementation.HubForeignKeyNamePattern,
                            ("TableName", table.Name),
                            ("TargetTableName", targetHubTable.Name),
                            ("SourceColumnName", endHashKeyColumn.Name)),
                        targetHubTable,
                        new[] { (endHashKeyColumn, targetHubHashKey) });
                }
            }

            AddImplementationColumn(
                context,
                table,
                businessLinkImplementation.LoadTimestampColumnName,
                businessLinkImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                businessLinkImplementation.LoadTimestampDefaultExpressionSql,
                ("Precision", businessLinkImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                businessLinkImplementation.RecordSourceColumnName,
                businessLinkImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessLinkImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessLinkImplementation.AuditIdColumnName,
                businessLinkImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessLinkImplementation.AuditIdDefaultExpressionSql);

            AddPrimaryKey(context, table, ApplyPattern(businessLinkImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)), hashKeyColumn);

            linkTablesByLinkId[link.Id] = table;
            linkHashKeyColumnsByLinkId[link.Id] = hashKeyColumn;
        }
    }

    private static void PopulateBusinessLinkSatellites(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        BusinessLinkSatelliteImplementation businessLinkSatelliteImplementation,
        IReadOnlyDictionary<string, List<BusinessSatelliteAttribute>> businessSatelliteAttributesBySatelliteId,
        IReadOnlyDictionary<string, List<BusinessSatelliteAttributeDataTypeDetail>> businessSatelliteAttributeDetailsByAttributeId,
        IReadOnlyDictionary<string, Table> linkTablesByLinkId,
        IReadOnlyDictionary<string, TableColumn> linkHashKeyColumnsByLinkId)
    {
        foreach (var satellite in model.BusinessLinkSatelliteList.OrderBy(row => row.BusinessSatellite.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessLinkSatelliteImplementation.SchemaName,
                ApplyPattern(
                    businessLinkSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessLink.Name),
                    ("Name", satellite.BusinessSatellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessLinkSatelliteImplementation.ParentHashKeyColumnName,
                businessLinkSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessLinkSatelliteImplementation.ParentHashKeyLength));

            var members = GetGroup(businessSatelliteAttributesBySatelliteId, satellite.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.DataTypeId,
                    GetDetailPairs(
                        businessSatelliteAttributeDetailsByAttributeId,
                        row.Id,
                        detail => detail.Name,
                        detail => detail.Value)));

            AddBusinessMembers(context, table, reservedColumnNames, members);

            AddImplementationColumn(
                context,
                table,
                businessLinkSatelliteImplementation.HashDiffColumnName,
                businessLinkSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessLinkSatelliteImplementation.HashDiffLength));

            var loadTimestampColumn = AddImplementationColumn(
                context,
                table,
                businessLinkSatelliteImplementation.LoadTimestampColumnName,
                businessLinkSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                businessLinkSatelliteImplementation.LoadTimestampDefaultExpressionSql,
                ("Precision", businessLinkSatelliteImplementation.LoadTimestampPrecision));

            AddPrimaryKey(
                context,
                table,
                ApplyPattern(businessLinkSatelliteImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)),
                parentHashKeyColumn,
                loadTimestampColumn);

            AddImplementationColumn(
                context,
                table,
                businessLinkSatelliteImplementation.RecordSourceColumnName,
                businessLinkSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessLinkSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessLinkSatelliteImplementation.AuditIdColumnName,
                businessLinkSatelliteImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessLinkSatelliteImplementation.AuditIdDefaultExpressionSql);

            if (linkTablesByLinkId.TryGetValue(satellite.BusinessLink.Id, out var parentTable) &&
                linkHashKeyColumnsByLinkId.TryGetValue(satellite.BusinessLink.Id, out var parentHashKeyTarget))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        businessLinkSatelliteImplementation.ParentForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("ParentTableName", parentTable.Name)),
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }
    }
}
