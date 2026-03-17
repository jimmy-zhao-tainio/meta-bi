using MetaBusinessDataVault;
using MetaSql;

namespace MetaDataVault.ToMetaSql;

public static partial class Converter
{
    private static void PopulateBusinessHelperMetaSqlModel(
        MetaBusinessDataVaultModel model,
        ConversionContext context,
        IReadOnlyDictionary<string, Table> hubTablesByHubId,
        IReadOnlyDictionary<string, TableColumn> hubHashKeyColumnsByHubId)
    {
        var businessPointInTimeImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessPointInTimeImplementationList, nameof(context.ImplementationModel.BusinessPointInTimeImplementationList));
        var businessBridgeImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessBridgeImplementationList, nameof(context.ImplementationModel.BusinessBridgeImplementationList));

        var businessPointInTimeHubSatellitesByPointInTimeId = GroupById(model.BusinessPointInTimeHubSatelliteList, row => row.BusinessPointInTimeId);
        var businessPointInTimeLinkSatellitesByPointInTimeId = GroupById(model.BusinessPointInTimeLinkSatelliteList, row => row.BusinessPointInTimeId);
        var businessPointInTimeStampsByPointInTimeId = GroupById(model.BusinessPointInTimeStampList, row => row.BusinessPointInTimeId);
        var businessPointInTimeStampDetailsByStampId = GroupById(model.BusinessPointInTimeStampDataTypeDetailList, row => row.BusinessPointInTimeStampId);
        var businessHubKeyPartDetailsByKeyPartId = GroupById(model.BusinessHubKeyPartDataTypeDetailList, row => row.BusinessHubKeyPartId);
        var businessHubSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessHubSatelliteAttributeDataTypeDetailList, row => row.BusinessHubSatelliteAttributeId);
        var businessLinkSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessLinkSatelliteAttributeDataTypeDetailList, row => row.BusinessLinkSatelliteAttributeId);
        var businessBridgeHubKeyPartProjectionsByBridgeId = GroupById(model.BusinessBridgeHubKeyPartProjectionList, row => row.BusinessBridgeId);
        var businessBridgeHubSatelliteAttributeProjectionsByBridgeId = GroupById(model.BusinessBridgeHubSatelliteAttributeProjectionList, row => row.BusinessBridgeId);
        var businessBridgeLinkSatelliteAttributeProjectionsByBridgeId = GroupById(model.BusinessBridgeLinkSatelliteAttributeProjectionList, row => row.BusinessBridgeId);

        foreach (var pointInTime in model.BusinessPointInTimeList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                $"BusinessPointInTime:{pointInTime.Id}",
                ApplyPattern(businessPointInTimeImplementation.TableNamePattern, ("Name", pointInTime.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:ParentHashKey",
                businessPointInTimeImplementation.ParentHashKeyColumnName,
                businessPointInTimeImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessPointInTimeImplementation.ParentHashKeyLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:SnapshotTimestamp",
                businessPointInTimeImplementation.SnapshotTimestampColumnName,
                businessPointInTimeImplementation.SnapshotTimestampDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessPointInTimeImplementation.SnapshotTimestampPrecision));

            var stampMembers = GetGroup(businessPointInTimeStampsByPointInTimeId, pointInTime.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.DataTypeId,
                    row.Ordinal,
                    GetDetailPairs(
                        businessPointInTimeStampDetailsByStampId,
                        row.Id,
                        detail => detail.Name,
                        detail => detail.Value)));

            AddOrderedBusinessMembers(context, table, reservedColumnNames, stampMembers, "Stamp");

            foreach (var hubSatellite in GetGroup(businessPointInTimeHubSatellitesByPointInTimeId, pointInTime.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                AddImplementationColumn(
                    context,
                    table,
                    $"{table.Id}:Column:HubSatelliteRef:{hubSatellite.Id}",
                    ApplyPattern(businessPointInTimeImplementation.SatelliteReferenceColumnNamePattern, ("SatelliteName", hubSatellite.BusinessHubSatellite.Name)),
                    businessPointInTimeImplementation.SatelliteReferenceDataTypeId,
                    "false",
                    reservedColumnNames,
                    ("Precision", businessPointInTimeImplementation.SatelliteReferencePrecision));
            }

            foreach (var linkSatellite in GetGroup(businessPointInTimeLinkSatellitesByPointInTimeId, pointInTime.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                AddImplementationColumn(
                    context,
                    table,
                    $"{table.Id}:Column:LinkSatelliteRef:{linkSatellite.Id}",
                    ApplyPattern(businessPointInTimeImplementation.SatelliteReferenceColumnNamePattern, ("SatelliteName", linkSatellite.BusinessLinkSatellite.Name)),
                    businessPointInTimeImplementation.SatelliteReferenceDataTypeId,
                    "false",
                    reservedColumnNames,
                    ("Precision", businessPointInTimeImplementation.SatelliteReferencePrecision));
            }

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
                businessPointInTimeImplementation.AuditIdColumnName,
                businessPointInTimeImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            if (hubTablesByHubId.TryGetValue(pointInTime.BusinessHubId, out var parentHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(pointInTime.BusinessHubId, out var parentHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    $"{table.Id}:ForeignKey:AnchorHub",
                    $"FK_{table.Name}_{parentHubTable.Name}",
                    parentHubTable,
                    new[] { (parentHashKeyColumn, parentHubHashKey) });
            }
        }

        foreach (var bridge in model.BusinessBridgeList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                $"BusinessBridge:{bridge.Id}",
                ApplyPattern(businessBridgeImplementation.TableNamePattern, ("Name", bridge.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var rootHashKeyColumn = AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RootHashKey",
                businessBridgeImplementation.RootHashKeyColumnName,
                businessBridgeImplementation.RootHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessBridgeImplementation.RootHashKeyLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:RelatedHashKey",
                businessBridgeImplementation.RelatedHashKeyColumnName,
                businessBridgeImplementation.RelatedHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessBridgeImplementation.RelatedHashKeyLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:Depth",
                businessBridgeImplementation.DepthColumnName,
                businessBridgeImplementation.DepthDataTypeId,
                "false",
                reservedColumnNames);

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:Path",
                businessBridgeImplementation.PathColumnName,
                businessBridgeImplementation.PathDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessBridgeImplementation.PathLength));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:EffectiveFrom",
                businessBridgeImplementation.EffectiveFromColumnName,
                businessBridgeImplementation.EffectiveFromDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessBridgeImplementation.EffectiveFromPrecision));

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:EffectiveTo",
                businessBridgeImplementation.EffectiveToColumnName,
                businessBridgeImplementation.EffectiveToDataTypeId,
                "false",
                reservedColumnNames,
                ("Precision", businessBridgeImplementation.EffectiveToPrecision));

            var members = GetGroup(businessBridgeHubKeyPartProjectionsByBridgeId, bridge.Id)
                .Select(row => CreateBusinessColumnMember(
                    row.Id,
                    row.Name,
                    row.BusinessHubKeyPart.DataTypeId,
                    row.Ordinal,
                    GetDetailPairs(
                        businessHubKeyPartDetailsByKeyPartId,
                        row.BusinessHubKeyPartId,
                        detail => detail.Name,
                        detail => detail.Value)))
                .Concat(
                    GetGroup(businessBridgeHubSatelliteAttributeProjectionsByBridgeId, bridge.Id)
                        .Select(row => CreateBusinessColumnMember(
                            row.Id,
                            row.Name,
                            row.BusinessHubSatelliteAttribute.DataTypeId,
                            row.Ordinal,
                            GetDetailPairs(
                                businessHubSatelliteAttributeDetailsByAttributeId,
                                row.BusinessHubSatelliteAttributeId,
                                detail => detail.Name,
                                detail => detail.Value))))
                .Concat(
                    GetGroup(businessBridgeLinkSatelliteAttributeProjectionsByBridgeId, bridge.Id)
                        .Select(row => CreateBusinessColumnMember(
                            row.Id,
                            row.Name,
                            row.BusinessLinkSatelliteAttribute.DataTypeId,
                            row.Ordinal,
                            GetDetailPairs(
                                businessLinkSatelliteAttributeDetailsByAttributeId,
                                row.BusinessLinkSatelliteAttributeId,
                                detail => detail.Name,
                                detail => detail.Value))));

            AddOrderedBusinessMembers(context, table, reservedColumnNames, members, "Projection");

            AddImplementationColumn(
                context,
                table,
                $"{table.Id}:Column:AuditId",
                businessBridgeImplementation.AuditIdColumnName,
                businessBridgeImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames);

            if (hubTablesByHubId.TryGetValue(bridge.AnchorHubId, out var anchorHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(bridge.AnchorHubId, out var anchorHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    $"{table.Id}:ForeignKey:AnchorHub",
                    $"FK_{table.Name}_{anchorHubTable.Name}",
                    anchorHubTable,
                    new[] { (rootHashKeyColumn, anchorHubHashKey) });
            }
        }
    }
}
