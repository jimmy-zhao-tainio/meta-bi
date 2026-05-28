using System.Globalization;
using Meta.Core.Domain;

namespace MetaDataVault.Core;

internal static class OrdinalAssignment
{
    private sealed record OrdinalScope(string RelationshipColumnName, IReadOnlyList<string> EntityNames);

    private static readonly IReadOnlyDictionary<string, OrdinalScope> BusinessScopes = new Dictionary<string, OrdinalScope>(StringComparer.Ordinal)
    {
        ["BusinessHubKeyPart"] = new("BusinessHubId", ["BusinessHubKeyPart"]),
        ["BusinessLinkHub"] = new("BusinessLinkId", ["BusinessLinkHub"]),
        ["BusinessReferenceKeyPart"] = new("BusinessReferenceId", ["BusinessReferenceKeyPart"]),
        ["BusinessHubSatelliteAttribute"] = new("BusinessHubSatelliteId", ["BusinessHubSatelliteAttribute"]),
        ["BusinessLinkSatelliteAttribute"] = new("BusinessLinkSatelliteId", ["BusinessLinkSatelliteAttribute"]),
        ["BusinessSameAsLinkSatelliteAttribute"] = new("BusinessSameAsLinkSatelliteId", ["BusinessSameAsLinkSatelliteAttribute"]),
        ["BusinessHierarchicalLinkSatelliteAttribute"] = new("BusinessHierarchicalLinkSatelliteId", ["BusinessHierarchicalLinkSatelliteAttribute"]),
        ["BusinessReferenceSatelliteAttribute"] = new("BusinessReferenceSatelliteId", ["BusinessReferenceSatelliteAttribute"]),
        ["BusinessPointInTimeStamp"] = new("BusinessPointInTimeId", ["BusinessPointInTimeStamp"]),
        ["BusinessPointInTimeHubSatellite"] = new("BusinessPointInTimeId", ["BusinessPointInTimeHubSatellite", "BusinessPointInTimeLinkSatellite"]),
        ["BusinessPointInTimeLinkSatellite"] = new("BusinessPointInTimeId", ["BusinessPointInTimeHubSatellite", "BusinessPointInTimeLinkSatellite"]),
        ["BusinessBridgeLink"] = new("BusinessBridgeId", ["BusinessBridgeLink", "BusinessBridgeHub"]),
        ["BusinessBridgeHub"] = new("BusinessBridgeId", ["BusinessBridgeLink", "BusinessBridgeHub"]),
    };

    private static readonly IReadOnlyDictionary<string, OrdinalScope> RawScopes = new Dictionary<string, OrdinalScope>(StringComparer.Ordinal)
    {
        ["SourceField"] = new("SourceTableId", ["SourceField"]),
        ["SourceTableRelationshipField"] = new("SourceTableRelationshipId", ["SourceTableRelationshipField"]),
        ["RawHubKeyPart"] = new("RawHubId", ["RawHubKeyPart"]),
        ["RawHubSatelliteAttribute"] = new("RawHubSatelliteId", ["RawHubSatelliteAttribute"]),
        ["RawLinkHub"] = new("RawLinkId", ["RawLinkHub"]),
        ["RawLinkSatelliteAttribute"] = new("RawLinkSatelliteId", ["RawLinkSatelliteAttribute"]),
    };

    public static void AssignBusinessOrdinalIfMissing(
        Workspace workspace,
        string entityName,
        IDictionary<string, string> values,
        IReadOnlyList<BusinessDataVaultRelationshipAssignment> relationships)
    {
        AssignOrdinalIfMissing(
            workspace,
            entityName,
            values,
            relationships.Select(row => (row.ColumnName, row.TargetRecordId)),
            BusinessScopes);
    }

    public static void AssignRawOrdinalIfMissing(
        Workspace workspace,
        string entityName,
        IDictionary<string, string> values,
        IReadOnlyList<RawDataVaultRelationshipAssignment> relationships)
    {
        AssignOrdinalIfMissing(
            workspace,
            entityName,
            values,
            relationships.Select(row => (row.ColumnName, row.TargetRecordId)),
            RawScopes);
    }

    private static void AssignOrdinalIfMissing(
        Workspace workspace,
        string entityName,
        IDictionary<string, string> values,
        IEnumerable<(string ColumnName, string TargetRecordId)> relationships,
        IReadOnlyDictionary<string, OrdinalScope> scopes)
    {
        if (values.ContainsKey("Ordinal") || !scopes.TryGetValue(entityName, out var scope))
        {
            return;
        }

        var parentRecordId = relationships
            .Where(row => string.Equals(row.ColumnName, scope.RelationshipColumnName, StringComparison.Ordinal))
            .Select(row => row.TargetRecordId)
            .FirstOrDefault();
        if (string.IsNullOrWhiteSpace(parentRecordId))
        {
            return;
        }

        var nextOrdinal = scope.EntityNames
            .SelectMany(workspace.Instance.GetOrCreateEntityRecords)
            .Where(row => string.Equals(row.RelationshipIds.GetValueOrDefault(scope.RelationshipColumnName), parentRecordId, StringComparison.Ordinal))
            .Select(row => ParseOrdinal(row.Values.GetValueOrDefault("Ordinal")))
            .DefaultIfEmpty(0)
            .Max() + 1;

        values["Ordinal"] = nextOrdinal.ToString(CultureInfo.InvariantCulture);
    }

    private static int ParseOrdinal(string? value)
    {
        return int.TryParse(value, out var ordinal) && ordinal > 0
            ? ordinal
            : 0;
    }
}
