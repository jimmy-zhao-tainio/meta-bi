using MetaBusinessDataVault;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

public static partial class Converter
{
    private static void PopulateBusinessLinkVariantMetaSqlModel(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        IReadOnlyDictionary<string, Table> hubTablesByHubId,
        IReadOnlyDictionary<string, TableColumn> hubHashKeyColumnsByHubId)
    {
        var businessHubImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessHubImplementationList, nameof(context.ImplementationModel.BusinessHubImplementationList));
        var businessSameAsLinkImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessSameAsLinkImplementationList, nameof(context.ImplementationModel.BusinessSameAsLinkImplementationList));
        var businessSameAsLinkSatelliteImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessSameAsLinkSatelliteImplementationList, nameof(context.ImplementationModel.BusinessSameAsLinkSatelliteImplementationList));
        var businessHierarchicalLinkImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessHierarchicalLinkImplementationList, nameof(context.ImplementationModel.BusinessHierarchicalLinkImplementationList));
        var businessHierarchicalLinkSatelliteImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessHierarchicalLinkSatelliteImplementationList, nameof(context.ImplementationModel.BusinessHierarchicalLinkSatelliteImplementationList));

        var businessSameAsLinkSatelliteAttributesBySatelliteId = GroupById(model.BusinessSameAsLinkSatelliteAttributeList, row => row.BusinessSameAsLinkSatelliteId);
        var businessSameAsLinkSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessSameAsLinkSatelliteAttributeDataTypeDetailList, row => row.BusinessSameAsLinkSatelliteAttributeId);
        var businessHierarchicalLinkSatelliteAttributesBySatelliteId = GroupById(model.BusinessHierarchicalLinkSatelliteAttributeList, row => row.BusinessHierarchicalLinkSatelliteId);
        var businessHierarchicalLinkSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessHierarchicalLinkSatelliteAttributeDataTypeDetailList, row => row.BusinessHierarchicalLinkSatelliteAttributeId);

        var sameAsTablesByLinkId = new Dictionary<string, Table>(StringComparer.Ordinal);
        var sameAsHashKeyColumnsByLinkId = new Dictionary<string, TableColumn>(StringComparer.Ordinal);
        var hierarchicalTablesByLinkId = new Dictionary<string, Table>(StringComparer.Ordinal);
        var hierarchicalHashKeyColumnsByLinkId = new Dictionary<string, TableColumn>(StringComparer.Ordinal);

        foreach (var link in model.BusinessSameAsLinkList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessSameAsLinkImplementation.SchemaName,
                ApplyPattern(businessSameAsLinkImplementation.TableNamePattern, ("Name", link.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessSameAsLinkImplementation.HashKeyColumnName,
                businessSameAsLinkImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessSameAsLinkImplementation.HashKeyLength));

            var primaryHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessSameAsLinkImplementation.PrimaryHashKeyColumnName,
                businessHubImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.HashKeyLength));

            var equivalentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessSameAsLinkImplementation.EquivalentHashKeyColumnName,
                businessHubImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.HashKeyLength));

            AddImplementationColumn(
                context,
                table,
                businessSameAsLinkImplementation.LoadTimestampColumnName,
                businessSameAsLinkImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessSameAsLinkImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                businessSameAsLinkImplementation.RecordSourceColumnName,
                businessSameAsLinkImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessSameAsLinkImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessSameAsLinkImplementation.AuditIdColumnName,
                businessSameAsLinkImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(context, table, ApplyPattern(businessSameAsLinkImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)), hashKeyColumn);

            if (hubTablesByHubId.TryGetValue(link.PrimaryHubId, out var primaryHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.PrimaryHubId, out var primaryHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        businessSameAsLinkImplementation.PrimaryHubForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("TargetTableName", primaryHubTable.Name),
                        ("SourceColumnName", primaryHashKeyColumn.Name)),
                    primaryHubTable,
                    new[] { (primaryHashKeyColumn, primaryHubHashKey) });
            }

            if (hubTablesByHubId.TryGetValue(link.EquivalentHubId, out var equivalentHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.EquivalentHubId, out var equivalentHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        businessSameAsLinkImplementation.EquivalentHubForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("TargetTableName", equivalentHubTable.Name),
                        ("SourceColumnName", equivalentHashKeyColumn.Name)),
                    equivalentHubTable,
                    new[] { (equivalentHashKeyColumn, equivalentHubHashKey) });
            }

            sameAsTablesByLinkId[link.Id] = table;
            sameAsHashKeyColumnsByLinkId[link.Id] = hashKeyColumn;
        }

        foreach (var satellite in model.BusinessSameAsLinkSatelliteList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessSameAsLinkSatelliteImplementation.SchemaName,
                ApplyPattern(
                    businessSameAsLinkSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessSameAsLink.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessSameAsLinkSatelliteImplementation.ParentHashKeyColumnName,
                businessSameAsLinkSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessSameAsLinkSatelliteImplementation.ParentHashKeyLength));

            var members = GetGroup(businessSameAsLinkSatelliteAttributesBySatelliteId, satellite.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.DataTypeId,
                    row.Ordinal,
                    GetDetailPairs(
                        businessSameAsLinkSatelliteAttributeDetailsByAttributeId,
                        row.Id,
                        detail => detail.Name,
                        detail => detail.Value)));

            AddOrderedBusinessMembers(context, table, reservedColumnNames, members);

            AddImplementationColumn(
                context,
                table,
                businessSameAsLinkSatelliteImplementation.HashDiffColumnName,
                businessSameAsLinkSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessSameAsLinkSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                businessSameAsLinkSatelliteImplementation.LoadTimestampColumnName,
                businessSameAsLinkSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessSameAsLinkSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                businessSameAsLinkSatelliteImplementation.RecordSourceColumnName,
                businessSameAsLinkSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessSameAsLinkSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessSameAsLinkSatelliteImplementation.AuditIdColumnName,
                businessSameAsLinkSatelliteImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            if (sameAsTablesByLinkId.TryGetValue(satellite.BusinessSameAsLinkId, out var parentTable) &&
                sameAsHashKeyColumnsByLinkId.TryGetValue(satellite.BusinessSameAsLinkId, out var parentHashKeyTarget))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        businessSameAsLinkSatelliteImplementation.ParentForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("ParentTableName", parentTable.Name)),
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }

        foreach (var link in model.BusinessHierarchicalLinkList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessHierarchicalLinkImplementation.SchemaName,
                ApplyPattern(businessHierarchicalLinkImplementation.TableNamePattern, ("Name", link.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkImplementation.HashKeyColumnName,
                businessHierarchicalLinkImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHierarchicalLinkImplementation.HashKeyLength));

            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkImplementation.ParentHashKeyColumnName,
                businessHubImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.HashKeyLength));

            var childHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkImplementation.ChildHashKeyColumnName,
                businessHubImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.HashKeyLength));

            AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkImplementation.LoadTimestampColumnName,
                businessHierarchicalLinkImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessHierarchicalLinkImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkImplementation.RecordSourceColumnName,
                businessHierarchicalLinkImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHierarchicalLinkImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkImplementation.AuditIdColumnName,
                businessHierarchicalLinkImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(context, table, ApplyPattern(businessHierarchicalLinkImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)), hashKeyColumn);

            if (hubTablesByHubId.TryGetValue(link.ParentHubId, out var parentHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.ParentHubId, out var parentHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        businessHierarchicalLinkImplementation.ParentHubForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("TargetTableName", parentHubTable.Name),
                        ("SourceColumnName", parentHashKeyColumn.Name)),
                    parentHubTable,
                    new[] { (parentHashKeyColumn, parentHubHashKey) });
            }

            if (hubTablesByHubId.TryGetValue(link.ChildHubId, out var childHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.ChildHubId, out var childHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        businessHierarchicalLinkImplementation.ChildHubForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("TargetTableName", childHubTable.Name),
                        ("SourceColumnName", childHashKeyColumn.Name)),
                    childHubTable,
                    new[] { (childHashKeyColumn, childHubHashKey) });
            }

            hierarchicalTablesByLinkId[link.Id] = table;
            hierarchicalHashKeyColumnsByLinkId[link.Id] = hashKeyColumn;
        }

        foreach (var satellite in model.BusinessHierarchicalLinkSatelliteList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessHierarchicalLinkSatelliteImplementation.SchemaName,
                ApplyPattern(
                    businessHierarchicalLinkSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessHierarchicalLink.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkSatelliteImplementation.ParentHashKeyColumnName,
                businessHierarchicalLinkSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHierarchicalLinkSatelliteImplementation.ParentHashKeyLength));

            var members = GetGroup(businessHierarchicalLinkSatelliteAttributesBySatelliteId, satellite.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.DataTypeId,
                    row.Ordinal,
                    GetDetailPairs(
                        businessHierarchicalLinkSatelliteAttributeDetailsByAttributeId,
                        row.Id,
                        detail => detail.Name,
                        detail => detail.Value)));

            AddOrderedBusinessMembers(context, table, reservedColumnNames, members);

            AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkSatelliteImplementation.HashDiffColumnName,
                businessHierarchicalLinkSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHierarchicalLinkSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkSatelliteImplementation.LoadTimestampColumnName,
                businessHierarchicalLinkSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessHierarchicalLinkSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkSatelliteImplementation.RecordSourceColumnName,
                businessHierarchicalLinkSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHierarchicalLinkSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkSatelliteImplementation.AuditIdColumnName,
                businessHierarchicalLinkSatelliteImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            if (hierarchicalTablesByLinkId.TryGetValue(satellite.BusinessHierarchicalLinkId, out var parentTable) &&
                hierarchicalHashKeyColumnsByLinkId.TryGetValue(satellite.BusinessHierarchicalLinkId, out var parentHashKeyTarget))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        businessHierarchicalLinkSatelliteImplementation.ParentForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("ParentTableName", parentTable.Name)),
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }
    }
}
