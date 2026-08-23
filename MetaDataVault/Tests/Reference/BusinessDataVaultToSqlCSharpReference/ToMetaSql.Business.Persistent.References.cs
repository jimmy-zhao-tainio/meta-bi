using MetaBusinessDataVault;
using MetaDataVault.Core;
using MetaDataVaultImplementation;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

internal static partial class BusinessDataVaultToSqlCSharpReference
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

            foreach (var keyPart in BusinessDataVaultRules.GetReferenceKeyPartChain(
                         reference,
                         GetGroup(businessReferenceKeyPartsByReferenceId, reference.Id)))
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

            AddOptionalImplementationColumn(
                context,
                table,
                businessReferenceImplementation.LoadTimestampColumnName,
                businessReferenceImplementation.LoadTimestampDataTypeId,
                reservedColumnNames,
                businessReferenceImplementation.LoadTimestampDefaultExpressionSql,
                ("Precision", businessReferenceImplementation.LoadTimestampPrecision));

            AddOptionalImplementationColumn(
                context,
                table,
                businessReferenceImplementation.RecordSourceColumnName,
                businessReferenceImplementation.RecordSourceDataTypeId,
                reservedColumnNames,
                ("Length", businessReferenceImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessReferenceImplementation.AuditIdColumnName,
                businessReferenceImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessReferenceImplementation.AuditIdDefaultExpressionSql);

            AddPrimaryKey(context, table, ApplyPattern(businessReferenceImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)), hashKeyColumn);

            referenceTablesByReferenceId[reference.Id] = table;
            referenceHashKeyColumnsByReferenceId[reference.Id] = hashKeyColumn;
        }
    }

    private static void PopulateBusinessReferenceSatellites(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        BusinessReferenceSatelliteImplementation businessReferenceSatelliteImplementation,
        IReadOnlyDictionary<string, List<BusinessSatelliteAttribute>> businessSatelliteAttributesBySatelliteId,
        IReadOnlyDictionary<string, List<BusinessSatelliteAttributeDataTypeDetail>> businessSatelliteAttributeDetailsByAttributeId,
        IReadOnlyDictionary<string, Table> referenceTablesByReferenceId,
        IReadOnlyDictionary<string, TableColumn> referenceHashKeyColumnsByReferenceId)
    {
        foreach (var satellite in model.BusinessReferenceSatelliteList.OrderBy(row => row.BusinessSatellite.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessReferenceSatelliteImplementation.SchemaName,
                ApplyPattern(
                    businessReferenceSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessReference.Name),
                    ("Name", satellite.BusinessSatellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessReferenceSatelliteImplementation.ParentHashKeyColumnName,
                businessReferenceSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessReferenceSatelliteImplementation.ParentHashKeyLength));

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

            AddBusinessMembers(context, table, reservedColumnNames, members, "true");

            AddOptionalImplementationColumn(
                context,
                table,
                businessReferenceSatelliteImplementation.HashDiffColumnName,
                businessReferenceSatelliteImplementation.HashDiffDataTypeId,
                reservedColumnNames,
                ("Length", businessReferenceSatelliteImplementation.HashDiffLength));

            var loadTimestampColumn = AddImplementationColumn(
                context,
                table,
                businessReferenceSatelliteImplementation.LoadTimestampColumnName,
                businessReferenceSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                businessReferenceSatelliteImplementation.LoadTimestampDefaultExpressionSql,
                ("Precision", businessReferenceSatelliteImplementation.LoadTimestampPrecision));

            AddPrimaryKey(
                context,
                table,
                ApplyPattern(businessReferenceSatelliteImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)),
                parentHashKeyColumn,
                loadTimestampColumn);

            AddOptionalImplementationColumn(
                context,
                table,
                businessReferenceSatelliteImplementation.RecordSourceColumnName,
                businessReferenceSatelliteImplementation.RecordSourceDataTypeId,
                reservedColumnNames,
                ("Length", businessReferenceSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessReferenceSatelliteImplementation.AuditIdColumnName,
                businessReferenceSatelliteImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessReferenceSatelliteImplementation.AuditIdDefaultExpressionSql);

            if (referenceTablesByReferenceId.TryGetValue(satellite.BusinessReference.Id, out var parentTable) &&
                referenceHashKeyColumnsByReferenceId.TryGetValue(satellite.BusinessReference.Id, out var parentHashKeyTarget))
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
