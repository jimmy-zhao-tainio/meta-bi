using MetaBusinessDataVault;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

internal static partial class BusinessDataVaultToSqlCSharpReference
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

        var businessSatelliteAttributesBySatelliteId = GroupById(model.BusinessSatelliteAttributeList, row => row.BusinessSatellite.Id);
        var businessSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessSatelliteAttributeDataTypeDetailList, row => row.BusinessSatelliteAttribute.Id);

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

            AddOptionalImplementationColumn(
                context,
                table,
                businessSameAsLinkImplementation.LoadTimestampColumnName,
                businessSameAsLinkImplementation.LoadTimestampDataTypeId,
                reservedColumnNames,
                businessSameAsLinkImplementation.LoadTimestampDefaultExpressionSql,
                ("Precision", businessSameAsLinkImplementation.LoadTimestampPrecision));

            AddOptionalImplementationColumn(
                context,
                table,
                businessSameAsLinkImplementation.RecordSourceColumnName,
                businessSameAsLinkImplementation.RecordSourceDataTypeId,
                reservedColumnNames,
                ("Length", businessSameAsLinkImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessSameAsLinkImplementation.AuditIdColumnName,
                businessSameAsLinkImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessSameAsLinkImplementation.AuditIdDefaultExpressionSql);

            AddPrimaryKey(context, table, ApplyPattern(businessSameAsLinkImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)), hashKeyColumn);

            if (hubTablesByHubId.TryGetValue(link.PrimaryHub.Id, out var primaryHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.PrimaryHub.Id, out var primaryHubHashKey))
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

            if (hubTablesByHubId.TryGetValue(link.EquivalentHub.Id, out var equivalentHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.EquivalentHub.Id, out var equivalentHubHashKey))
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

        foreach (var satellite in model.BusinessSameAsLinkSatelliteList.OrderBy(row => row.BusinessSatellite.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessSameAsLinkSatelliteImplementation.SchemaName,
                ApplyPattern(
                    businessSameAsLinkSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessSameAsLink.Name),
                    ("Name", satellite.BusinessSatellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessSameAsLinkSatelliteImplementation.ParentHashKeyColumnName,
                businessSameAsLinkSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessSameAsLinkSatelliteImplementation.ParentHashKeyLength));

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
                businessSameAsLinkSatelliteImplementation.HashDiffColumnName,
                businessSameAsLinkSatelliteImplementation.HashDiffDataTypeId,
                reservedColumnNames,
                ("Length", businessSameAsLinkSatelliteImplementation.HashDiffLength));

            var loadTimestampColumn = AddImplementationColumn(
                context,
                table,
                businessSameAsLinkSatelliteImplementation.LoadTimestampColumnName,
                businessSameAsLinkSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                businessSameAsLinkSatelliteImplementation.LoadTimestampDefaultExpressionSql,
                ("Precision", businessSameAsLinkSatelliteImplementation.LoadTimestampPrecision));

            AddPrimaryKey(
                context,
                table,
                ApplyPattern(businessSameAsLinkSatelliteImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)),
                parentHashKeyColumn,
                loadTimestampColumn);

            AddOptionalImplementationColumn(
                context,
                table,
                businessSameAsLinkSatelliteImplementation.RecordSourceColumnName,
                businessSameAsLinkSatelliteImplementation.RecordSourceDataTypeId,
                reservedColumnNames,
                ("Length", businessSameAsLinkSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessSameAsLinkSatelliteImplementation.AuditIdColumnName,
                businessSameAsLinkSatelliteImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessSameAsLinkSatelliteImplementation.AuditIdDefaultExpressionSql);

            if (sameAsTablesByLinkId.TryGetValue(satellite.BusinessSameAsLink.Id, out var parentTable) &&
                sameAsHashKeyColumnsByLinkId.TryGetValue(satellite.BusinessSameAsLink.Id, out var parentHashKeyTarget))
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

            AddOptionalImplementationColumn(
                context,
                table,
                businessHierarchicalLinkImplementation.LoadTimestampColumnName,
                businessHierarchicalLinkImplementation.LoadTimestampDataTypeId,
                reservedColumnNames,
                businessHierarchicalLinkImplementation.LoadTimestampDefaultExpressionSql,
                ("Precision", businessHierarchicalLinkImplementation.LoadTimestampPrecision));

            AddOptionalImplementationColumn(
                context,
                table,
                businessHierarchicalLinkImplementation.RecordSourceColumnName,
                businessHierarchicalLinkImplementation.RecordSourceDataTypeId,
                reservedColumnNames,
                ("Length", businessHierarchicalLinkImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkImplementation.AuditIdColumnName,
                businessHierarchicalLinkImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessHierarchicalLinkImplementation.AuditIdDefaultExpressionSql);

            AddPrimaryKey(context, table, ApplyPattern(businessHierarchicalLinkImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)), hashKeyColumn);

            if (hubTablesByHubId.TryGetValue(link.ParentHub.Id, out var parentHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.ParentHub.Id, out var parentHubHashKey))
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

            if (hubTablesByHubId.TryGetValue(link.ChildHub.Id, out var childHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.ChildHub.Id, out var childHubHashKey))
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

        foreach (var satellite in model.BusinessHierarchicalLinkSatelliteList.OrderBy(row => row.BusinessSatellite.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessHierarchicalLinkSatelliteImplementation.SchemaName,
                ApplyPattern(
                    businessHierarchicalLinkSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessHierarchicalLink.Name),
                    ("Name", satellite.BusinessSatellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkSatelliteImplementation.ParentHashKeyColumnName,
                businessHierarchicalLinkSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHierarchicalLinkSatelliteImplementation.ParentHashKeyLength));

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
                businessHierarchicalLinkSatelliteImplementation.HashDiffColumnName,
                businessHierarchicalLinkSatelliteImplementation.HashDiffDataTypeId,
                reservedColumnNames,
                ("Length", businessHierarchicalLinkSatelliteImplementation.HashDiffLength));

            var loadTimestampColumn = AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkSatelliteImplementation.LoadTimestampColumnName,
                businessHierarchicalLinkSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                businessHierarchicalLinkSatelliteImplementation.LoadTimestampDefaultExpressionSql,
                ("Precision", businessHierarchicalLinkSatelliteImplementation.LoadTimestampPrecision));

            AddPrimaryKey(
                context,
                table,
                ApplyPattern(businessHierarchicalLinkSatelliteImplementation.PrimaryKeyNamePattern, ("TableName", table.Name)),
                parentHashKeyColumn,
                loadTimestampColumn);

            AddOptionalImplementationColumn(
                context,
                table,
                businessHierarchicalLinkSatelliteImplementation.RecordSourceColumnName,
                businessHierarchicalLinkSatelliteImplementation.RecordSourceDataTypeId,
                reservedColumnNames,
                ("Length", businessHierarchicalLinkSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                businessHierarchicalLinkSatelliteImplementation.AuditIdColumnName,
                businessHierarchicalLinkSatelliteImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessHierarchicalLinkSatelliteImplementation.AuditIdDefaultExpressionSql);

            if (hierarchicalTablesByLinkId.TryGetValue(satellite.BusinessHierarchicalLink.Id, out var parentTable) &&
                hierarchicalHashKeyColumnsByLinkId.TryGetValue(satellite.BusinessHierarchicalLink.Id, out var parentHashKeyTarget))
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
