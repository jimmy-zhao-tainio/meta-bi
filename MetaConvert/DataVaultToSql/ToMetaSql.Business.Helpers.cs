using MetaBusinessDataVault;
using MetaDataVault.Core;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

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

        var businessPointInTimeHubSatellitesByPointInTimeId = GroupById(model.BusinessPointInTimeHubSatelliteList, row => row.BusinessPointInTime.Id);
        var businessPointInTimeLinkSatellitesByPointInTimeId = GroupById(model.BusinessPointInTimeLinkSatelliteList, row => row.BusinessPointInTime.Id);
        var businessPointInTimeStampsByPointInTimeId = GroupById(model.BusinessPointInTimeStampList, row => row.BusinessPointInTime.Id);
        var businessPointInTimeStampDetailsByStampId = GroupById(model.BusinessPointInTimeStampDataTypeDetailList, row => row.BusinessPointInTimeStamp.Id);
        var businessBridgeTraversalsByBridgeId = GroupById(model.BusinessBridgeTraversalList, row => row.BusinessBridge.Id);

        foreach (var pointInTime in model.BusinessPointInTimeList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var table = AddTable(
                context,
                businessPointInTimeImplementation.SchemaName,
                ApplyPattern(businessPointInTimeImplementation.TableNamePattern, ("Name", pointInTime.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var parentHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessPointInTimeImplementation.ParentHashKeyColumnName,
                businessPointInTimeImplementation.ParentHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessPointInTimeImplementation.ParentHashKeyLength));

            AddImplementationColumn(
                context,
                table,
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

            AddOrderedBusinessMembers(context, table, reservedColumnNames, stampMembers);

            foreach (var hubSatellite in GetGroup(businessPointInTimeHubSatellitesByPointInTimeId, pointInTime.Id).OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
            {
                AddImplementationColumn(
                    context,
                    table,
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
                    ApplyPattern(businessPointInTimeImplementation.SatelliteReferenceColumnNamePattern, ("SatelliteName", linkSatellite.BusinessLinkSatellite.Name)),
                    businessPointInTimeImplementation.SatelliteReferenceDataTypeId,
                    "false",
                    reservedColumnNames,
                    ("Precision", businessPointInTimeImplementation.SatelliteReferencePrecision));
            }

            AddImplementationColumn(
                context,
                table,
                businessPointInTimeImplementation.AuditIdColumnName,
                businessPointInTimeImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessPointInTimeImplementation.AuditIdDefaultExpressionSql);

            if (hubTablesByHubId.TryGetValue(pointInTime.BusinessHub.Id, out var parentHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(pointInTime.BusinessHub.Id, out var parentHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        businessPointInTimeImplementation.AnchorHubForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("ParentTableName", parentHubTable.Name)),
                    parentHubTable,
                    new[] { (parentHashKeyColumn, parentHubHashKey) });
            }
        }
        
        foreach (var bridge in model.BusinessBridgeList.OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            var traversals = BusinessDataVaultRules.GetBridgeTraversalChain(
                bridge,
                GetGroup(businessBridgeTraversalsByBridgeId, bridge.Id));
            var terminalHubId = traversals[^1].TargetRole.BusinessHub.Id;

            var table = AddTable(
                context,
                businessBridgeImplementation.SchemaName,
                ApplyPattern(businessBridgeImplementation.TableNamePattern, ("Name", bridge.Name)));

            var reservedColumnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var rootHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessBridgeImplementation.RootHashKeyColumnName,
                businessBridgeImplementation.RootHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessBridgeImplementation.RootHashKeyLength));
            var relatedHashKeyColumn = AddImplementationColumn(
                context,
                table,
                businessBridgeImplementation.RelatedHashKeyColumnName,
                businessBridgeImplementation.RelatedHashKeyDataTypeId,
                "false",
                reservedColumnNames,
                ("Length", businessBridgeImplementation.RelatedHashKeyLength));

            AddOptionalImplementationColumn(
                context,
                table,
                businessBridgeImplementation.DepthColumnName,
                businessBridgeImplementation.DepthDataTypeId,
                reservedColumnNames);
            AddOptionalImplementationColumn(
                context,
                table,
                businessBridgeImplementation.PathColumnName,
                businessBridgeImplementation.PathDataTypeId,
                reservedColumnNames,
                ("Length", businessBridgeImplementation.PathLength));
            AddOptionalImplementationColumn(
                context,
                table,
                businessBridgeImplementation.EffectiveFromColumnName,
                businessBridgeImplementation.EffectiveFromDataTypeId,
                reservedColumnNames,
                ("Precision", businessBridgeImplementation.EffectiveFromPrecision));
            AddOptionalImplementationColumn(
                context,
                table,
                businessBridgeImplementation.EffectiveToColumnName,
                businessBridgeImplementation.EffectiveToDataTypeId,
                reservedColumnNames,
                ("Precision", businessBridgeImplementation.EffectiveToPrecision));

            AddImplementationColumn(
                context,
                table,
                businessBridgeImplementation.AuditIdColumnName,
                businessBridgeImplementation.AuditIdDataTypeId,
                "false",
                reservedColumnNames,
                businessBridgeImplementation.AuditIdDefaultExpressionSql);

            if (hubTablesByHubId.TryGetValue(bridge.BusinessHub.Id, out var anchorHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(bridge.BusinessHub.Id, out var anchorHubHashKey))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        businessBridgeImplementation.AnchorHubForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("ParentTableName", anchorHubTable.Name)),
                    anchorHubTable,
                    new[] { (rootHashKeyColumn, anchorHubHashKey) });
            }

            if (hubTablesByHubId.TryGetValue(terminalHubId, out var terminalHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(terminalHubId, out var terminalHubHashKey) &&
                !string.IsNullOrWhiteSpace(businessBridgeImplementation.RelatedHubForeignKeyNamePattern))
            {
                AddForeignKey(
                    context,
                    table,
                    ApplyPattern(
                        businessBridgeImplementation.RelatedHubForeignKeyNamePattern,
                        ("TableName", table.Name),
                        ("ParentTableName", terminalHubTable.Name)),
                    terminalHubTable,
                    new[] { (relatedHashKeyColumn, terminalHubHashKey) });
            }
        }
    }

    private static TableColumn? AddOptionalImplementationColumn(
        ConversionContext context,
        Table table,
        string name,
        string metaDataTypeId,
        HashSet<string> reservedColumnNames,
        params (string Name, string Value)[] details)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(metaDataTypeId))
        {
            return null;
        }

        return AddImplementationColumn(
            context,
            table,
            name,
            metaDataTypeId,
            "false",
            reservedColumnNames,
            details);
    }
}
