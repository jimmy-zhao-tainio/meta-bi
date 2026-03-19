using MetaBusinessDataVault;
using MetaDataVaultImplementation;
using MetaSql;

namespace MetaDataVault.ToMetaSql;

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

        var businessHubKeyPartsByHubId = GroupById(model.BusinessHubKeyPartList, row => row.BusinessHubId);
        var businessHubKeyPartDetailsByKeyPartId = GroupById(model.BusinessHubKeyPartDataTypeDetailList, row => row.BusinessHubKeyPartId);
        var businessHubSatelliteKeyPartsBySatelliteId = GroupById(model.BusinessHubSatelliteKeyPartList, row => row.BusinessHubSatelliteId);
        var businessHubSatelliteKeyPartDetailsByKeyPartId = GroupById(model.BusinessHubSatelliteKeyPartDataTypeDetailList, row => row.BusinessHubSatelliteKeyPartId);
        var businessHubSatelliteAttributesBySatelliteId = GroupById(model.BusinessHubSatelliteAttributeList, row => row.BusinessHubSatelliteId);
        var businessHubSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessHubSatelliteAttributeDataTypeDetailList, row => row.BusinessHubSatelliteAttributeId);
        var businessLinkHubsByLinkId = GroupById(model.BusinessLinkHubList, row => row.BusinessLinkId);
        var businessLinkSatelliteKeyPartsBySatelliteId = GroupById(model.BusinessLinkSatelliteKeyPartList, row => row.BusinessLinkSatelliteId);
        var businessLinkSatelliteKeyPartDetailsByKeyPartId = GroupById(model.BusinessLinkSatelliteKeyPartDataTypeDetailList, row => row.BusinessLinkSatelliteKeyPartId);
        var businessLinkSatelliteAttributesBySatelliteId = GroupById(model.BusinessLinkSatelliteAttributeList, row => row.BusinessLinkSatelliteId);
        var businessLinkSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessLinkSatelliteAttributeDataTypeDetailList, row => row.BusinessLinkSatelliteAttributeId);
        var businessReferenceKeyPartsByReferenceId = GroupById(model.BusinessReferenceKeyPartList, row => row.BusinessReferenceId);
        var businessReferenceKeyPartDetailsByKeyPartId = GroupById(model.BusinessReferenceKeyPartDataTypeDetailList, row => row.BusinessReferenceKeyPartId);
        var businessReferenceSatelliteKeyPartsBySatelliteId = GroupById(model.BusinessReferenceSatelliteKeyPartList, row => row.BusinessReferenceSatelliteId);
        var businessReferenceSatelliteKeyPartDetailsByKeyPartId = GroupById(model.BusinessReferenceSatelliteKeyPartDataTypeDetailList, row => row.BusinessReferenceSatelliteKeyPartId);
        var businessReferenceSatelliteAttributesBySatelliteId = GroupById(model.BusinessReferenceSatelliteAttributeList, row => row.BusinessReferenceSatelliteId);
        var businessReferenceSatelliteAttributeDetailsByAttributeId = GroupById(model.BusinessReferenceSatelliteAttributeDataTypeDetailList, row => row.BusinessReferenceSatelliteAttributeId);

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
            businessHubSatelliteKeyPartsBySatelliteId,
            businessHubSatelliteKeyPartDetailsByKeyPartId,
            businessHubSatelliteAttributesBySatelliteId,
            businessHubSatelliteAttributeDetailsByAttributeId,
            hubTablesByHubId,
            hubHashKeyColumnsByHubId);

        PopulateBusinessLinks(
            model,
            context,
            businessHubImplementation,
            businessLinkImplementation,
            businessLinkHubsByLinkId,
            hubTablesByHubId,
            hubHashKeyColumnsByHubId,
            linkTablesByLinkId,
            linkHashKeyColumnsByLinkId);

        PopulateBusinessLinkSatellites(
            model,
            context,
            businessLinkSatelliteImplementation,
            businessLinkSatelliteKeyPartsBySatelliteId,
            businessLinkSatelliteKeyPartDetailsByKeyPartId,
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
            businessReferenceSatelliteKeyPartsBySatelliteId,
            businessReferenceSatelliteKeyPartDetailsByKeyPartId,
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
        string isNullable = "")
    {
        if (string.IsNullOrWhiteSpace(sourceMetaDataTypeId))
        {
            throw new InvalidOperationException("Business column type id is required.");
        }

        var resolvedMetaDataTypeId = context.BusinessTypeLowering?.LowerOrKeep(sourceMetaDataTypeId)
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
