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
        IReadOnlyDictionary<string, List<BusinessLinkHub>> businessLinkHubsByLinkId,
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

            foreach (var linkHub in GetGroup(businessLinkHubsByLinkId, link.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                var endHashKeyColumn = AddImplementationColumn(
                    context,
                    table,
                    ApplyPattern(businessLinkImplementation.EndHashKeyColumnPattern, ("RoleName", linkHub.RoleName)),
                    businessHubImplementation.HashKeyDataTypeId,
                    "false",
                    reservedColumnNames,
                    ("Length", businessHubImplementation.HashKeyLength));

                if (hubTablesByHubId.TryGetValue(linkHub.BusinessHubId, out var targetHubTable) &&
                    hubHashKeyColumnsByHubId.TryGetValue(linkHub.BusinessHubId, out var targetHubHashKey))
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
                reservedColumnNames);

            AddPrimaryKey(context, table, ApplyPattern(businessLinkImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)), hashKeyColumn);

            linkTablesByLinkId[link.Id] = table;
            linkHashKeyColumnsByLinkId[link.Id] = hashKeyColumn;
        }
    }

    private static void PopulateBusinessLinkSatellites(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        BusinessLinkSatelliteImplementation businessLinkSatelliteImplementation,
        IReadOnlyDictionary<string, List<BusinessLinkSatelliteAttribute>> businessLinkSatelliteAttributesBySatelliteId,
        IReadOnlyDictionary<string, List<BusinessLinkSatelliteAttributeDataTypeDetail>> businessLinkSatelliteAttributeDetailsByAttributeId,
        IReadOnlyDictionary<string, Table> linkTablesByLinkId,
        IReadOnlyDictionary<string, TableColumn> linkHashKeyColumnsByLinkId)
    {
        foreach (var satellite in model.BusinessLinkSatelliteList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessLinkSatelliteImplementation.SchemaName,
                ApplyPattern(
                    businessLinkSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessLink.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessLinkSatelliteImplementation.ParentHashKeyColumnName,
                businessLinkSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessLinkSatelliteImplementation.ParentHashKeyLength));

            var members = GetGroup(businessLinkSatelliteAttributesBySatelliteId, satellite.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.DataTypeId,
                    row.Ordinal,
                    GetDetailPairs(
                        businessLinkSatelliteAttributeDetailsByAttributeId,
                        row.Id,
                        detail => detail.Name,
                        detail => detail.Value)));

            AddOrderedBusinessMembers(context, table, reservedColumnNames, members);

            AddImplementationColumn(
                context,
                table,
                businessLinkSatelliteImplementation.HashDiffColumnName,
                businessLinkSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessLinkSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                businessLinkSatelliteImplementation.LoadTimestampColumnName,
                businessLinkSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessLinkSatelliteImplementation.LoadTimestampPrecision));

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
                reservedColumnNames);

            if (linkTablesByLinkId.TryGetValue(satellite.BusinessLinkId, out var parentTable) &&
                linkHashKeyColumnsByLinkId.TryGetValue(satellite.BusinessLinkId, out var parentHashKeyTarget))
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
