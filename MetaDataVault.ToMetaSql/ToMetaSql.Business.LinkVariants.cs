using MetaBusinessDataVault;
using MetaSql;

namespace MetaDataVault.ToMetaSql;

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

        var businessSameAsLinkSatelliteKeyPartsBySatelliteId = GroupById(model.BusinessSameAsLinkSatelliteKeyPartList, row => row.BusinessSameAsLinkSatelliteId);
        var businessSameAsLinkSatelliteKeyPartDetailsByKeyPartId = GroupById(model.BusinessSameAsLinkSatelliteKeyPartDataTypeDetailList, row => row.BusinessSameAsLinkSatelliteKeyPartId);
        var businessSameAsLinkSatelliteAttributesBySatelliteId = GroupById(model.BusinessSameAsLinkSatelliteAttributeList, row => row.BusinessSameAsLinkSatelliteId);
        var businessSameAsLinkSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessSameAsLinkSatelliteAttributeDataTypeDetailList, row => row.BusinessSameAsLinkSatelliteAttributeId);
        var businessHierarchicalLinkSatelliteKeyPartsBySatelliteId = GroupById(model.BusinessHierarchicalLinkSatelliteKeyPartList, row => row.BusinessHierarchicalLinkSatelliteId);
        var businessHierarchicalLinkSatelliteKeyPartDetailsByKeyPartId = GroupById(model.BusinessHierarchicalLinkSatelliteKeyPartDataTypeDetailList, row => row.BusinessHierarchicalLinkSatelliteKeyPartId);
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
                $"BusinessSameAsLink:{link.Id}",
                ApplyPattern(businessSameAsLinkImplementation.TableNamePattern, ("Name", link.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashKey",
                businessSameAsLinkImplementation.HashKeyColumnName,
                businessSameAsLinkImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessSameAsLinkImplementation.HashKeyLength));

            var primaryHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:PrimaryHashKey",
                businessSameAsLinkImplementation.PrimaryHashKeyColumnName,
                businessHubImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.HashKeyLength));

            var equivalentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:EquivalentHashKey",
                businessSameAsLinkImplementation.EquivalentHashKeyColumnName,
                businessHubImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.HashKeyLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                businessSameAsLinkImplementation.LoadTimestampColumnName,
                businessSameAsLinkImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessSameAsLinkImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                businessSameAsLinkImplementation.RecordSourceColumnName,
                businessSameAsLinkImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessSameAsLinkImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
                businessSameAsLinkImplementation.AuditIdColumnName,
                businessSameAsLinkImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(context, table, $"{table.Id}:PrimaryKey", $"PK_{table.Name}", hashKeyColumn);

            if (hubTablesByHubId.TryGetValue(link.PrimaryHubId, out var primaryHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.PrimaryHubId, out var primaryHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    $"{table.Id}:ForeignKey:PrimaryHub",
                    $"FK_{table.Name}_{primaryHubTable.Name}_{primaryHashKeyColumn.Name}",
                    primaryHubTable,
                    new[] { (primaryHashKeyColumn, primaryHubHashKey) });
            }

            if (hubTablesByHubId.TryGetValue(link.EquivalentHubId, out var equivalentHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.EquivalentHubId, out var equivalentHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    $"{table.Id}:ForeignKey:EquivalentHub",
                    $"FK_{table.Name}_{equivalentHubTable.Name}_{equivalentHashKeyColumn.Name}",
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
                $"BusinessSameAsLinkSatellite:{satellite.Id}",
                ApplyPattern(
                    businessSameAsLinkSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessSameAsLink.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:ParentHashKey",
                businessSameAsLinkSatelliteImplementation.ParentHashKeyColumnName,
                businessSameAsLinkSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessSameAsLinkSatelliteImplementation.ParentHashKeyLength));

            var members = GetGroup(businessSameAsLinkSatelliteKeyPartsBySatelliteId, satellite.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.DataTypeId,
                    row.Ordinal,
                    GetDetailPairs(
                        businessSameAsLinkSatelliteKeyPartDetailsByKeyPartId,
                        row.Id,
                        detail => detail.Name,
                        detail => detail.Value)))
                .Concat(
                    GetGroup(businessSameAsLinkSatelliteAttributesBySatelliteId, satellite.Id)
                        .Select(row => CreateBusinessColumnMember(
                            row.Id,
                            row.Name,
                            row.DataTypeId,
                            row.Ordinal,
                            GetDetailPairs(
                                businessSameAsLinkSatelliteAttributeDetailsByAttributeId,
                                row.Id,
                                detail => detail.Name,
                                detail => detail.Value))));

            AddOrderedBusinessMembers(context, table, reservedColumnNames, members, "Member");

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashDiff",
                businessSameAsLinkSatelliteImplementation.HashDiffColumnName,
                businessSameAsLinkSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessSameAsLinkSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                businessSameAsLinkSatelliteImplementation.LoadTimestampColumnName,
                businessSameAsLinkSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessSameAsLinkSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                businessSameAsLinkSatelliteImplementation.RecordSourceColumnName,
                businessSameAsLinkSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessSameAsLinkSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
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
                    $"{table.Id}:ForeignKey:ParentSameAsLink",
                    $"FK_{table.Name}_{parentTable.Name}",
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }

        foreach (var link in model.BusinessHierarchicalLinkList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                $"BusinessHierarchicalLink:{link.Id}",
                ApplyPattern(businessHierarchicalLinkImplementation.TableNamePattern, ("Name", link.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var hashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashKey",
                businessHierarchicalLinkImplementation.HashKeyColumnName,
                businessHierarchicalLinkImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHierarchicalLinkImplementation.HashKeyLength));

            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:ParentHashKey",
                businessHierarchicalLinkImplementation.ParentHashKeyColumnName,
                businessHubImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.HashKeyLength));

            var childHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:ChildHashKey",
                businessHierarchicalLinkImplementation.ChildHashKeyColumnName,
                businessHubImplementation.HashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHubImplementation.HashKeyLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                businessHierarchicalLinkImplementation.LoadTimestampColumnName,
                businessHierarchicalLinkImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessHierarchicalLinkImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                businessHierarchicalLinkImplementation.RecordSourceColumnName,
                businessHierarchicalLinkImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHierarchicalLinkImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
                businessHierarchicalLinkImplementation.AuditIdColumnName,
                businessHierarchicalLinkImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            AddPrimaryKey(context, table, $"{table.Id}:PrimaryKey", $"PK_{table.Name}", hashKeyColumn);

            if (hubTablesByHubId.TryGetValue(link.ParentHubId, out var parentHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.ParentHubId, out var parentHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    $"{table.Id}:ForeignKey:ParentHub",
                    $"FK_{table.Name}_{parentHubTable.Name}_{parentHashKeyColumn.Name}",
                    parentHubTable,
                    new[] { (parentHashKeyColumn, parentHubHashKey) });
            }

            if (hubTablesByHubId.TryGetValue(link.ChildHubId, out var childHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(link.ChildHubId, out var childHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    $"{table.Id}:ForeignKey:ChildHub",
                    $"FK_{table.Name}_{childHubTable.Name}_{childHashKeyColumn.Name}",
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
                $"BusinessHierarchicalLinkSatellite:{satellite.Id}",
                ApplyPattern(
                    businessHierarchicalLinkSatelliteImplementation.TableNamePattern,
                    ("ParentName", satellite.BusinessHierarchicalLink.Name),
                    ("Name", satellite.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:ParentHashKey",
                businessHierarchicalLinkSatelliteImplementation.ParentHashKeyColumnName,
                businessHierarchicalLinkSatelliteImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHierarchicalLinkSatelliteImplementation.ParentHashKeyLength));

            var members = GetGroup(businessHierarchicalLinkSatelliteKeyPartsBySatelliteId, satellite.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.DataTypeId,
                    row.Ordinal,
                    GetDetailPairs(
                        businessHierarchicalLinkSatelliteKeyPartDetailsByKeyPartId,
                        row.Id,
                        detail => detail.Name,
                        detail => detail.Value)))
                .Concat(
                    GetGroup(businessHierarchicalLinkSatelliteAttributesBySatelliteId, satellite.Id)
                        .Select(row => CreateBusinessColumnMember(
                            row.Id,
                            row.Name,
                            row.DataTypeId,
                            row.Ordinal,
                            GetDetailPairs(
                                businessHierarchicalLinkSatelliteAttributeDetailsByAttributeId,
                                row.Id,
                                detail => detail.Name,
                                detail => detail.Value))));

            AddOrderedBusinessMembers(context, table, reservedColumnNames, members, "Member");

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:HashDiff",
                businessHierarchicalLinkSatelliteImplementation.HashDiffColumnName,
                businessHierarchicalLinkSatelliteImplementation.HashDiffDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHierarchicalLinkSatelliteImplementation.HashDiffLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:LoadTimestamp",
                businessHierarchicalLinkSatelliteImplementation.LoadTimestampColumnName,
                businessHierarchicalLinkSatelliteImplementation.LoadTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessHierarchicalLinkSatelliteImplementation.LoadTimestampPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RecordSource",
                businessHierarchicalLinkSatelliteImplementation.RecordSourceColumnName,
                businessHierarchicalLinkSatelliteImplementation.RecordSourceDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessHierarchicalLinkSatelliteImplementation.RecordSourceLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
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
                    $"{table.Id}:ForeignKey:ParentHierarchicalLink",
                    $"FK_{table.Name}_{parentTable.Name}",
                    parentTable,
                    new[] { (parentHashKeyColumn, parentHashKeyTarget) });
            }
        }
    }
}
