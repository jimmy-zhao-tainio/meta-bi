using MetaBusinessDataVault;
using MetaDataVaultImplementation;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

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
                businessReferenceImplementation.SchemaName,
                ApplyPattern(businessReferenceImplementation.TableNamePattern, ("Name", reference.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
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
                businessReferenceImplementation.LoadTimestampColumnName,
                businessReferenceImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessReferenceImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                businessReferenceImplementation.RecordSourceColumnName,
                businessReferenceImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessReferenceImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessReferenceImplementation.AuditIdColumnName,
                businessReferenceImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(context, table, ApplyPattern(businessReferenceImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)), hashKeyColumn);

            referenceTablesByReferenceId[reference.Id] = table;
            referenceHashKeyColumnsByReferenceId[reference.Id] = hashKeyColumn;
        }
    }

    private static void PopulateBusinessReferenceSatellites(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        BusinessReferenceSatelliteImplementation businessReferenceSatelliteImplementation,
        IReadOnlyDictionary<string, List<BusinessReferenceSatelliteAttribute>> businessReferenceSatelliteAttributesBySatelliteId,
        IReadOnlyDictionary<string, List<BusinessReferenceSatelliteAttributeDataTypeDetail>> businessReferenceSatelliteAttributeDetailsByAttributeId,
        IReadOnlyDictionary<string, Table> referenceTablesByReferenceId,
        IReadOnlyDictionary<string, TableColumn> referenceHashKeyColumnsByReferenceId)
    {
        foreach (var satellite in model.BusinessReferenceSatelliteList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessReferenceSatelliteImplementation.SchemaName,
                ApplyPattern(
                    businessReferenceSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessReference.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessReferenceSatelliteImplementation.ParentHashKeyColumnName,
                businessReferenceSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessReferenceSatelliteImplementation.ParentHashKeyLength));

            var members = GetGroup(businessReferenceSatelliteAttributesBySatelliteId, satellite.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.DataTypeId,
                    row.Ordinal,
                    GetDetailPairs(
                        businessReferenceSatelliteAttributeDetailsByAttributeId,
                        row.Id,
                        detail => detail.Name,
                        detail => detail.Value)));

            AddOrderedBusinessMembers(context, table, reservedColumnNames, members);

            AddImplementationColumn(
                context,
                table,
                businessReferenceSatelliteImplementation.HashDiffColumnName,
                businessReferenceSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessReferenceSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                businessReferenceSatelliteImplementation.LoadTimestampColumnName,
                businessReferenceSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessReferenceSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                businessReferenceSatelliteImplementation.RecordSourceColumnName,
                businessReferenceSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessReferenceSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
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
                    ApplyPattern(
                        businessReferenceSatelliteImplementation.ParentForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("ParentTableName", parentTable.Name)),
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }
    }
}
