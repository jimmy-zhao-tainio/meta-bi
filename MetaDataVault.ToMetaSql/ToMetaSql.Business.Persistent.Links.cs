using MetaBusinessDataVault;
using MetaDataVaultImplementation;
using SqlModel;

namespace MetaDataVault.ToMetaSql;

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
                $"BusinessLink:{link.Id}",
                ApplyPattern(businessLinkImplementation.TableNamePattern, ("Name", link.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashKey",
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
                    $"{table.Id}:Column:EndHashKey:{linkHub.Id}",
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
                        $"{table.Id}:ForeignKey:Hub:{linkHub.Id}",
                        $"FK_{table.Name}_{targetHubTable.Name}_{endHashKeyColumn.Name}",
                        targetHubTable,
                        new[] { (endHashKeyColumn, targetHubHashKey) });
                }
            }

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                businessLinkImplementation.LoadTimestampColumnName,
                businessLinkImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessLinkImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                businessLinkImplementation.RecordSourceColumnName,
                businessLinkImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessLinkImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
                businessLinkImplementation.AuditIdColumnName,
                businessLinkImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(context, table, $"{table.Id}:PrimaryKey", $"PK_{table.Name}", hashKeyColumn);

            linkTablesByLinkId[link.Id] = table;
            linkHashKeyColumnsByLinkId[link.Id] = hashKeyColumn;
        }
    }

    private static void PopulateBusinessLinkSatellites(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        BusinessLinkSatelliteImplementation businessLinkSatelliteImplementation,
        IReadOnlyDictionary<string, List<BusinessLinkSatelliteKeyPart>> businessLinkSatelliteKeyPartsBySatelliteId,
        IReadOnlyDictionary<string, List<BusinessLinkSatelliteKeyPartDataTypeDetail>> businessLinkSatelliteKeyPartDetailsByKeyPartId,
        IReadOnlyDictionary<string, List<BusinessLinkSatelliteAttribute>> businessLinkSatelliteAttributesBySatelliteId,
        IReadOnlyDictionary<string, List<BusinessLinkSatelliteAttributeDataTypeDetail>> businessLinkSatelliteAttributeDetailsByAttributeId,
        IReadOnlyDictionary<string, Table> linkTablesByLinkId,
        IReadOnlyDictionary<string, TableColumn> linkHashKeyColumnsByLinkId)
    {
        foreach (var satellite in model.BusinessLinkSatelliteList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                $"BusinessLinkSatellite:{satellite.Id}",
                ApplyPattern(
                    businessLinkSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessLink.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:ParentHashKey",
                businessLinkSatelliteImplementation.ParentHashKeyColumnName,
                businessLinkSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessLinkSatelliteImplementation.ParentHashKeyLength));

            var members = GetGroup(businessLinkSatelliteKeyPartsBySatelliteId, satellite.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.DataTypeId,
                    row.Ordinal,
                    GetDetailPairs(
                        businessLinkSatelliteKeyPartDetailsByKeyPartId,
                        row.Id,
                        detail => detail.Name,
                        detail => detail.Value)))
                .Concat(
                    GetGroup(businessLinkSatelliteAttributesBySatelliteId, satellite.Id)
                        .Select(row => CreateBusinessColumnMember(
                            row.Id,
                            row.Name,
                            row.DataTypeId,
                            row.Ordinal,
                            GetDetailPairs(
                                businessLinkSatelliteAttributeDetailsByAttributeId,
                                row.Id,
                                detail => detail.Name,
                                detail => detail.Value))));

            AddOrderedBusinessMembers(context, table, reservedColumnNames, members, "Member");

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashDiff",
                businessLinkSatelliteImplementation.HashDiffColumnName,
                businessLinkSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessLinkSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                businessLinkSatelliteImplementation.LoadTimestampColumnName,
                businessLinkSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessLinkSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                businessLinkSatelliteImplementation.RecordSourceColumnName,
                businessLinkSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessLinkSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
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
                    $"{table.Id}:ForeignKey:ParentLink",
                    $"FK_{table.Name}_{parentTable.Name}",
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }
    }
}
