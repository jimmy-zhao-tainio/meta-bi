using MetaBusinessDataVault;
using MetaSql;

namespace MetaDataVault.ToMetaSql;

public static partial class Converter
{
    private sealed record BusinessBridgePathMember(
        int Ordinal,
        BusinessBridgeLink? Link,
        BusinessBridgeHub? Hub);

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
        var businessBridgeLinksByBridgeId = GroupById(model.BusinessBridgeLinkList, row => row.BusinessBridgeId);
        var businessBridgeHubsByBridgeId = GroupById(model.BusinessBridgeHubList, row => row.BusinessBridgeId);
        var businessLinkHubsByLinkId = GroupById(model.BusinessLinkHubList, row => row.BusinessLinkId);

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
                reservedColumnNames);

            if (hubTablesByHubId.TryGetValue(pointInTime.BusinessHubId, out var parentHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(pointInTime.BusinessHubId, out var parentHubHashKey))
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
            var pathMembers = GetOrderedBridgePathMembers(
                bridge,
                GetGroup(businessBridgeLinksByBridgeId, bridge.Id),
                GetGroup(businessBridgeHubsByBridgeId, bridge.Id));
            var terminalHubId = ValidateBridgePath(bridge, pathMembers, businessLinkHubsByLinkId);

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
                reservedColumnNames);

            if (hubTablesByHubId.TryGetValue(bridge.AnchorHubId, out var anchorHubTable) &&
                hubHashKeyColumnsByHubId.TryGetValue(bridge.AnchorHubId, out var anchorHubHashKey))
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

    private static IReadOnlyList<BusinessBridgePathMember> GetOrderedBridgePathMembers(
        BusinessBridge bridge,
        IReadOnlyList<BusinessBridgeLink> bridgeLinks,
        IReadOnlyList<BusinessBridgeHub> bridgeHubs)
    {
        var members = bridgeLinks
            .Select(row => new BusinessBridgePathMember(ParseRequiredOrdinal(row.Ordinal, "BusinessBridgeLink", row.Id), row, null))
            .Concat(bridgeHubs.Select(row => new BusinessBridgePathMember(ParseRequiredOrdinal(row.Ordinal, "BusinessBridgeHub", row.Id), null, row)))
            .OrderBy(row => row.Ordinal)
            .ThenBy(row => row.Link?.Id ?? row.Hub!.Id, StringComparer.Ordinal)
            .ToList();

        if (members.Count == 0)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Id}' does not define any ordered path members.");
        }

        for (var i = 1; i < members.Count; i++)
        {
            if (members[i - 1].Ordinal == members[i].Ordinal)
            {
                throw new InvalidOperationException($"Bridge '{bridge.Id}' contains duplicate ordinal '{members[i].Ordinal}'.");
            }
        }

        return members;
    }

    private static string ValidateBridgePath(
        BusinessBridge bridge,
        IReadOnlyList<BusinessBridgePathMember> pathMembers,
        IReadOnlyDictionary<string, List<BusinessLinkHub>> businessLinkHubsByLinkId)
    {
        if (pathMembers.Count < 2)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Id}' must contain at least one link and one hub.");
        }

        if (pathMembers.Count % 2 != 0)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Id}' must alternate links and hubs, ending on a hub.");
        }

        if (pathMembers[0].Link is null)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Id}' must begin with a BusinessBridgeLink.");
        }

        if (pathMembers[^1].Hub is null)
        {
            throw new InvalidOperationException($"Bridge '{bridge.Id}' must end with a BusinessBridgeHub.");
        }

        var currentHubId = bridge.AnchorHubId;
        for (var i = 0; i < pathMembers.Count; i += 2)
        {
            var linkMember = pathMembers[i].Link
                ?? throw new InvalidOperationException($"Bridge '{bridge.Id}' must alternate link and hub members.");
            var hubMember = pathMembers[i + 1].Hub
                ?? throw new InvalidOperationException($"Bridge '{bridge.Id}' must alternate link and hub members.");
            var targetHubId = hubMember.BusinessHubId;
            var participatingHubIds = GetGroup(businessLinkHubsByLinkId, linkMember.BusinessLinkId)
                .Select(row => row.BusinessHubId)
                .ToHashSet(StringComparer.Ordinal);

            if (!participatingHubIds.Contains(currentHubId) || !participatingHubIds.Contains(targetHubId))
            {
                throw new InvalidOperationException(
                    $"Bridge '{bridge.Id}' cannot traverse link '{linkMember.BusinessLinkId}' from hub '{currentHubId}' to hub '{targetHubId}'.");
            }

            currentHubId = targetHubId;
        }

        return currentHubId;
    }

    private static int ParseRequiredOrdinal(string ordinal, string logicalName, string rowId)
    {
        if (!int.TryParse(ordinal, out var parsed) || parsed <= 0)
        {
            throw new InvalidOperationException($"{logicalName} '{rowId}' must use a positive integer ordinal.");
        }

        return parsed;
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
