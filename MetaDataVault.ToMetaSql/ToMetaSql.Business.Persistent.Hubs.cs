using MetaBusinessDataVault;
using MetaDataVaultImplementation;
using SqlModel;

namespace MetaDataVault.ToMetaSql;

public static partial class Converter
{
    private static void PopulateBusinessHubs(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        BusinessHubImplementation businessHubImplementation,
        IReadOnlyDictionary<string, List<BusinessHubKeyPart>> businessHubKeyPartsByHubId,
        IReadOnlyDictionary<string, List<BusinessHubKeyPartDataTypeDetail>> businessHubKeyPartDetailsByKeyPartId,
        Dictionary<string, Table> hubTablesByHubId,
        Dictionary<string, TableColumn> hubHashKeyColumnsByHubId)
    {
        foreach (var hub in model.BusinessHubList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                $"BusinessHub:{hub.Id}",
                ApplyPattern(businessHubImplementation.TableNamePattern, ("Name", hub.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashKey",
                businessHubImplementation.HashKeyColumnName,
                businessHubImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.HashKeyLength));

            foreach (var keyPart in GetGroup(businessHubKeyPartsByHubId, hub.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                AddBusinessTypedColumn(
                    context,
                    table,
                    $"{table.Id}:Column:KeyPart:{keyPart.Id}",
                    keyPart.Name,
                    keyPart.DataTypeId,
                    reservedColumnNames,
                    GetDetailPairs(
                        businessHubKeyPartDetailsByKeyPartId,
                        keyPart.Id,
                        row => row.Name,
                        row => row.Value));
            }

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                businessHubImplementation.LoadTimestampColumnName,
                businessHubImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessHubImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                businessHubImplementation.RecordSourceColumnName,
                businessHubImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
                businessHubImplementation.AuditIdColumnName,
                businessHubImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(context, table, $"{table.Id}:PrimaryKey", $"PK_{table.Name}", hashKeyColumn);

            hubTablesByHubId[hub.Id] = table;
            hubHashKeyColumnsByHubId[hub.Id] = hashKeyColumn;
        }
    }

    private static void PopulateBusinessHubSatellites(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        BusinessHubSatelliteImplementation businessHubSatelliteImplementation,
        IReadOnlyDictionary<string, List<BusinessHubSatelliteKeyPart>> businessHubSatelliteKeyPartsBySatelliteId,
        IReadOnlyDictionary<string, List<BusinessHubSatelliteKeyPartDataTypeDetail>> businessHubSatelliteKeyPartDetailsByKeyPartId,
        IReadOnlyDictionary<string, List<BusinessHubSatelliteAttribute>> businessHubSatelliteAttributesBySatelliteId,
        IReadOnlyDictionary<string, List<BusinessHubSatelliteAttributeDataTypeDetail>> businessHubSatelliteAttributeDetailsByAttributeId,
        IReadOnlyDictionary<string, Table> hubTablesByHubId,
        IReadOnlyDictionary<string, TableColumn> hubHashKeyColumnsByHubId)
    {
        foreach (var satellite in model.BusinessHubSatelliteList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                $"BusinessHubSatellite:{satellite.Id}",
                ApplyPattern(
                    businessHubSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessHub.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:ParentHashKey",
                businessHubSatelliteImplementation.ParentHashKeyColumnName,
                businessHubSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubSatelliteImplementation.ParentHashKeyLength));

            var members = GetGroup(businessHubSatelliteKeyPartsBySatelliteId, satellite.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.DataTypeId,
                    row.Ordinal,
                    GetDetailPairs(
                        businessHubSatelliteKeyPartDetailsByKeyPartId,
                        row.Id,
                        detail => detail.Name,
                        detail => detail.Value)))
                .Concat(
                    GetGroup(businessHubSatelliteAttributesBySatelliteId, satellite.Id)
                        .Select(row => CreateBusinessColumnMember(
                            row.Id,
                            row.Name,
                            row.DataTypeId,
                            row.Ordinal,
                            GetDetailPairs(
                                businessHubSatelliteAttributeDetailsByAttributeId,
                                row.Id,
                                detail => detail.Name,
                                detail => detail.Value))));

            AddOrderedBusinessMembers(context, table, reservedColumnNames, members, "Member");

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashDiff",
                businessHubSatelliteImplementation.HashDiffColumnName,
                businessHubSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                businessHubSatelliteImplementation.LoadTimestampColumnName,
                businessHubSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessHubSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                businessHubSatelliteImplementation.RecordSourceColumnName,
                businessHubSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
                businessHubSatelliteImplementation.AuditIdColumnName,
                businessHubSatelliteImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            if (hubTablesByHubId.TryGetValue(satellite.BusinessHubId, out var parentTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(satellite.BusinessHubId, out var parentHashKeyTarget))
            {
                AddForeignKey(
                    context,
                    table,
                    $"{table.Id}:ForeignKey:ParentHub",
                    $"FK_{table.Name}_{parentTable.Name}",
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }
    }
}
