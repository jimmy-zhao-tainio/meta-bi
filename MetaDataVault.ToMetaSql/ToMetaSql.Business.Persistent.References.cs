using MetaBusinessDataVault;
using MetaDataVaultImplementation;
using SqlModel;

namespace MetaDataVault.ToMetaSql;

public static partial class Converter
{
    private static void PopulateBusinessReferences(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        BusinessReferenceImplementation businessReferenceImplementation,
        IReadOnlyDictionary<string, List<BusinessReferenceKeyPart>> businessReferenceKeyPartsByReferenceId,
        IReadOnlyDictionary<string, List<BusinessReferenceKeyPartDataTypeDetail>> businessReferenceKeyPartDetailsByKeyPartId,
        Dictionary<string, Table> referenceTablesByReferenceId,
        Dictionary<string, TableColumn> referenceHashKeyColumnsByReferenceId)
    {
        foreach (var reference in model.BusinessReferenceList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                $"BusinessReference:{reference.Id}",
                ApplyPattern(businessReferenceImplementation.TableNamePattern, ("Name", reference.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashKey",
                businessReferenceImplementation.HashKeyColumnName,
                businessReferenceImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessReferenceImplementation.HashKeyLength));

            foreach (var keyPart in GetGroup(businessReferenceKeyPartsByReferenceId, reference.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                AddBusinessTypedColumn(
                    context,
                    table,
                    $"{table.Id}:Column:KeyPart:{keyPart.Id}",
                    keyPart.Name,
                    keyPart.DataTypeId,
                    reservedColumnNames,
                    GetDetailPairs(
                        businessReferenceKeyPartDetailsByKeyPartId,
                        keyPart.Id,
                        row => row.Name,
                        row => row.Value));
            }

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                businessReferenceImplementation.LoadTimestampColumnName,
                businessReferenceImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessReferenceImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                businessReferenceImplementation.RecordSourceColumnName,
                businessReferenceImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessReferenceImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
                businessReferenceImplementation.AuditIdColumnName,
                businessReferenceImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(context, table, $"{table.Id}:PrimaryKey", $"PK_{table.Name}", hashKeyColumn);

            referenceTablesByReferenceId[reference.Id] = table;
            referenceHashKeyColumnsByReferenceId[reference.Id] = hashKeyColumn;
        }
    }

    private static void PopulateBusinessReferenceSatellites(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        BusinessReferenceSatelliteImplementation businessReferenceSatelliteImplementation,
        IReadOnlyDictionary<string, List<BusinessReferenceSatelliteKeyPart>> businessReferenceSatelliteKeyPartsBySatelliteId,
        IReadOnlyDictionary<string, List<BusinessReferenceSatelliteKeyPartDataTypeDetail>> businessReferenceSatelliteKeyPartDetailsByKeyPartId,
        IReadOnlyDictionary<string, List<BusinessReferenceSatelliteAttribute>> businessReferenceSatelliteAttributesBySatelliteId,
        IReadOnlyDictionary<string, List<BusinessReferenceSatelliteAttributeDataTypeDetail>> businessReferenceSatelliteAttributeDetailsByAttributeId,
        IReadOnlyDictionary<string, Table> referenceTablesByReferenceId,
        IReadOnlyDictionary<string, TableColumn> referenceHashKeyColumnsByReferenceId)
    {
        foreach (var satellite in model.BusinessReferenceSatelliteList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                $"BusinessReferenceSatellite:{satellite.Id}",
                ApplyPattern(
                    businessReferenceSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessReference.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:ParentHashKey",
                businessReferenceSatelliteImplementation.ParentHashKeyColumnName,
                businessReferenceSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessReferenceSatelliteImplementation.ParentHashKeyLength));

            var members = GetGroup(businessReferenceSatelliteKeyPartsBySatelliteId, satellite.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.DataTypeId,
                    row.Ordinal,
                    GetDetailPairs(
                        businessReferenceSatelliteKeyPartDetailsByKeyPartId,
                        row.Id,
                        detail => detail.Name,
                        detail => detail.Value)))
                .Concat(
                    GetGroup(businessReferenceSatelliteAttributesBySatelliteId, satellite.Id)
                        .Select(row => CreateBusinessColumnMember(
                            row.Id,
                            row.Name,
                            row.DataTypeId,
                            row.Ordinal,
                            GetDetailPairs(
                                businessReferenceSatelliteAttributeDetailsByAttributeId,
                                row.Id,
                                detail => detail.Name,
                                detail => detail.Value))));

            AddOrderedBusinessMembers(context, table, reservedColumnNames, members, "Member");

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashDiff",
                businessReferenceSatelliteImplementation.HashDiffColumnName,
                businessReferenceSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessReferenceSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                businessReferenceSatelliteImplementation.LoadTimestampColumnName,
                businessReferenceSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessReferenceSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                businessReferenceSatelliteImplementation.RecordSourceColumnName,
                businessReferenceSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessReferenceSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
                businessReferenceSatelliteImplementation.AuditIdColumnName,
                businessReferenceSatelliteImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            if (referenceTablesByReferenceId.TryGetValue(satellite.BusinessReferenceId, out var parentTable) &&
                referenceHashKeyColumnsByReferenceId.TryGetValue(satellite.BusinessReferenceId, out var parentHashKeyTarget))
            {
                AddForeignKey(
                    context,
                    table,
                    $"{table.Id}:ForeignKey:ParentReference",
                    $"FK_{table.Name}_{parentTable.Name}",
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }
    }
}
