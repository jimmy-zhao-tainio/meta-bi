using MetaBusinessDataVault;
using MetaDataVault.Core;
using MetaDataVaultImplementation;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

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
                businessHubImplementation.SchemaName,
                ApplyPattern(businessHubImplementation.TableNamePattern, ("Name", hub.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessHubImplementation.HashKeyColumnName,
                businessHubImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.HashKeyLength));

            foreach (var keyPart in BusinessDataVaultRules.GetHubKeyPartChain(
                         hub,
                         GetGroup(businessHubKeyPartsByHubId, hub.Id)))
            {
                AddBusinessTypedColumn(
                    context,
                    table,
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
                businessHubImplementation.LoadTimestampColumnName,
                businessHubImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                businessHubImplementation.LoadTimestampDefaultExpressionSql,
                ("Precision", businessHubImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                businessHubImplementation.RecordSourceColumnName,
                businessHubImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessHubImplementation.AuditIdColumnName,
                businessHubImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessHubImplementation.AuditIdDefaultExpressionSql);

            AddPrimaryKey(context, table, ApplyPattern(businessHubImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)), hashKeyColumn);

            hubTablesByHubId[hub.Id] = table;
            hubHashKeyColumnsByHubId[hub.Id] = hashKeyColumn;
        }
    }

    private static void PopulateBusinessHubSatellites(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        BusinessHubSatelliteImplementation businessHubSatelliteImplementation,
        IReadOnlyDictionary<string, List<BusinessSatelliteAttribute>> businessSatelliteAttributesBySatelliteId,
        IReadOnlyDictionary<string, List<BusinessSatelliteAttributeDataTypeDetail>> businessSatelliteAttributeDetailsByAttributeId,
        IReadOnlyDictionary<string, Table> hubTablesByHubId,
        IReadOnlyDictionary<string, TableColumn> hubHashKeyColumnsByHubId)
    {
        foreach (var satellite in model.BusinessHubSatelliteList.OrderBy(row => row.BusinessSatellite.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessHubSatelliteImplementation.SchemaName,
                ApplyPattern(
                    businessHubSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessHub.Name),
                    ("Name", satellite.BusinessSatellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessHubSatelliteImplementation.ParentHashKeyColumnName,
                businessHubSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubSatelliteImplementation.ParentHashKeyLength));

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

            AddImplementationColumn(
                context,
                table,
                businessHubSatelliteImplementation.HashDiffColumnName,
                businessHubSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubSatelliteImplementation.HashDiffLength));

            var loadTimestampColumn = AddImplementationColumn(
                context,
                table,
                businessHubSatelliteImplementation.LoadTimestampColumnName,
                businessHubSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                businessHubSatelliteImplementation.LoadTimestampDefaultExpressionSql,
                ("Precision", businessHubSatelliteImplementation.LoadTimestampPrecision));

            AddPrimaryKey(
                context,
                table,
                ApplyPattern(businessHubSatelliteImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)),
                parentHashKeyColumn,
                loadTimestampColumn);

            AddImplementationColumn(
                context,
                table,
                businessHubSatelliteImplementation.RecordSourceColumnName,
                businessHubSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessHubSatelliteImplementation.AuditIdColumnName,
                businessHubSatelliteImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessHubSatelliteImplementation.AuditIdDefaultExpressionSql);

            if (hubTablesByHubId.TryGetValue(satellite.BusinessHub.Id, out var parentTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(satellite.BusinessHub.Id, out var parentHashKeyTarget))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        businessHubSatelliteImplementation.ParentForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("ParentTableName", parentTable.Name)),
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }
    }
}
