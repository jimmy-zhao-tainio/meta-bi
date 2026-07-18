using MetaBusinessDataVault;
using MetaDataVault.Core;
using MetaDataVaultImplementation;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

public static partial class Converter
{
    private static (Dictionary<string, Table> HubTablesByHubId, Dictionary<string, TableColumn> HubHashKeyColumnsByHubId) PopulateBusinessPersistentMetaSqlModel(
        MetaBusinessDataVaultModel model,
        ConversionContext context)
    {
        var businessHubImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessHubImplementationList, nameof(context.ImplementationModel.BusinessHubImplementationList));
        var businessLinkImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessLinkImplementationList, nameof(context.ImplementationModel.BusinessLinkImplementationList));
        var businessReferenceImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessReferenceImplementationList, nameof(context.ImplementationModel.BusinessReferenceImplementationList));
        var businessHubSatelliteImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessHubSatelliteImplementationList, nameof(context.ImplementationModel.BusinessHubSatelliteImplementationList));
        var businessLinkSatelliteImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessLinkSatelliteImplementationList, nameof(context.ImplementationModel.BusinessLinkSatelliteImplementationList));
        var businessReferenceSatelliteImplementation = RequireSingleImplementation(context.ImplementationModel.BusinessReferenceSatelliteImplementationList, nameof(context.ImplementationModel.BusinessReferenceSatelliteImplementationList));

        var businessHubKeyPartsByHubId = GroupById(model.BusinessHubKeyPartList, row => row.BusinessHub.Id);
        var businessHubKeyPartDetailsByKeyPartId = GroupById(model.BusinessHubKeyPartDataTypeDetailList, row => row.BusinessHubKeyPart.Id);
        var businessHubSatelliteAttributesBySatelliteId = GroupById(model.BusinessHubSatelliteAttributeList, row => row.BusinessHubSatellite.Id);
        var businessHubSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessHubSatelliteAttributeDataTypeDetailList, row => row.BusinessHubSatelliteAttribute.Id);
        BusinessDataVaultRules.ValidateLinkRoleNames(model);
        var businessLinkRolesByLinkId = GroupById(model.BusinessLinkRoleList, row => row.BusinessLink.Id);
        var businessLinkSatelliteAttributesBySatelliteId = GroupById(model.BusinessLinkSatelliteAttributeList, row => row.BusinessLinkSatellite.Id);
        var businessLinkSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessLinkSatelliteAttributeDataTypeDetailList, row => row.BusinessLinkSatelliteAttribute.Id);
        var businessReferenceKeyPartsByReferenceId = GroupById(model.BusinessReferenceKeyPartList, row => row.BusinessReference.Id);
        var businessReferenceKeyPartDetailsByKeyPartId = GroupById(model.BusinessReferenceKeyPartDataTypeDetailList, row => row.BusinessReferenceKeyPart.Id);
        var businessReferenceSatelliteAttributesBySatelliteId = GroupById(model.BusinessReferenceSatelliteAttributeList, row => row.BusinessReferenceSatellite.Id);
        var businessReferenceSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessReferenceSatelliteAttributeDataTypeDetailList, row => row.BusinessReferenceSatelliteAttribute.Id);

        var hubTablesByHubId = new Dictionary<string, Table>(StringComparer.Ordinal);
        var hubHashKeyColumnsByHubId = new Dictionary<string, TableColumn>(StringComparer.Ordinal);
        var linkTablesByLinkId = new Dictionary<string, Table>(StringComparer.Ordinal);
        var linkHashKeyColumnsByLinkId = new Dictionary<string, TableColumn>(StringComparer.Ordinal);
        var referenceTablesByReferenceId = new Dictionary<string, Table>(StringComparer.Ordinal);
        var referenceHashKeyColumnsByReferenceId = new Dictionary<string, TableColumn>(StringComparer.Ordinal);

        PopulateBusinessHubs(
            model,
            context,
            businessHubImplementation,
            businessHubKeyPartsByHubId,
            businessHubKeyPartDetailsByKeyPartId,
            hubTablesByHubId,
            hubHashKeyColumnsByHubId);

        PopulateBusinessHubSatellites(
            model,
            context,
            businessHubSatelliteImplementation,
            businessHubSatelliteAttributesBySatelliteId,
            businessHubSatelliteAttributeDetailsByAttributeId,
            hubTablesByHubId,
            hubHashKeyColumnsByHubId);

        PopulateBusinessLinks(
            model,
            context,
            businessHubImplementation,
            businessLinkImplementation,
            businessLinkRolesByLinkId,
            hubTablesByHubId,
            hubHashKeyColumnsByHubId,
            linkTablesByLinkId,
            linkHashKeyColumnsByLinkId);

        PopulateBusinessLinkSatellites(
            model,
            context,
            businessLinkSatelliteImplementation,
            businessLinkSatelliteAttributesBySatelliteId,
            businessLinkSatelliteAttributeDetailsByAttributeId,
            linkTablesByLinkId,
            linkHashKeyColumnsByLinkId);

        PopulateBusinessReferences(
            model,
            context,
            businessReferenceImplementation,
            businessReferenceKeyPartsByReferenceId,
            businessReferenceKeyPartDetailsByKeyPartId,
            referenceTablesByReferenceId,
            referenceHashKeyColumnsByReferenceId);

        PopulateBusinessReferenceSatellites(
            model,
            context,
            businessReferenceSatelliteImplementation,
            businessReferenceSatelliteAttributesBySatelliteId,
            businessReferenceSatelliteAttributeDetailsByAttributeId,
            referenceTablesByReferenceId,
            referenceHashKeyColumnsByReferenceId);

        PopulateBusinessLinkVariantMetaSqlModel(model, context, hubTablesByHubId, hubHashKeyColumnsByHubId);

        return (hubTablesByHubId, hubHashKeyColumnsByHubId);
    }

    private static TableColumn AddBusinessTypedColumn(
        ConversionContext context,
        Table table,
        string name,
        string sourceMetaDataTypeId,
        HashSet<string> reservedColumnNames,
        IEnumerable<(string Name, string Value)> details,
        string isNullable = "false")
    {
        if (string.IsNullOrWhiteSpace(sourceMetaDataTypeId))
        {
            throw new InvalidOperationException("Business column type id is required.");
        }

        var resolvedMetaDataTypeId = context.BusinessTypeLowering?.LowerRequired(sourceMetaDataTypeId)
            ?? throw new InvalidOperationException(
                $"Business logical type '{sourceMetaDataTypeId}' requires a sanctioned SQL Server lowering path.");

        var column = AddColumn(
            context,
            table,
            name,
            resolvedMetaDataTypeId,
            isNullable,
            reservedColumnNames);

        foreach (var (detailName, detailValue) in details)
        {
            AddDetail(context, column, detailName, detailValue);
        }

        return column;
    }

    private static void AddOrderedBusinessMembers(
        ConversionContext context,
        Table table,
        HashSet<string> reservedColumnNames,
        IEnumerable<BusinessColumnMemberSpec> members)
    {
        foreach (var member in members.OrderBy(row => ParseOrdinal(row.Ordinal)).ThenBy(row => row.Id, StringComparer.Ordinal))
        {
            AddBusinessTypedColumn(
                context,
                table,
                member.Name,
                member.DataTypeId,
                reservedColumnNames,
                member.Details);
        }
    }

    private static BusinessColumnMemberSpec CreateBusinessColumnMember(
        string id,
        string name,
        string dataTypeId,
        string ordinal,
        IEnumerable<(string Name, string Value)> details)
    {
        return new BusinessColumnMemberSpec(id, name, dataTypeId, ordinal, details.ToList());
    }

    private static IEnumerable<(string Name, string Value)> GetDetailPairs<TDetail>(
        IReadOnlyDictionary<string, List<TDetail>> groups,
        string key,
        Func<TDetail, string> nameSelector,
        Func<TDetail, string> valueSelector)
        where TDetail : class
    {
        foreach (var detail in GetGroup(groups, key).OrderBy(nameSelector, StringComparer.OrdinalIgnoreCase))
        {
            yield return (nameSelector(detail), valueSelector(detail));
        }
    }

    private sealed record BusinessColumnMemberSpec(
        string Id,
        string Name,
        string DataTypeId,
        string Ordinal,
        IReadOnlyList<(string Name, string Value)> Details);
}
