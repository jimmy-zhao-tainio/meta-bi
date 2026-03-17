using Meta.Core.Domain;
using MetaRawDataVault;

namespace MetaDataVault.ToMetaSql;

public static partial class Converter
{
    private static Workspace ConvertRaw(MetaRawDataVaultModel model, ConversionContext context)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(context);

        var sourceFieldDetailsByFieldId = GroupById(model.SourceFieldDataTypeDetailList, row => row.SourceFieldId);
        var rawHubKeyPartsByHubId = GroupById(model.RawHubKeyPartList, row => row.RawHubId);
        var rawHubSatelliteAttributesBySatelliteId = GroupById(model.RawHubSatelliteAttributeList, row => row.RawHubSatelliteId);
        var rawHubSatellitesByHubId = GroupById(model.RawHubSatelliteList, row => row.RawHubId);
        var rawLinkHubsByLinkId = GroupById(model.RawLinkHubList, row => row.RawLinkId);
        var rawLinkSatellitesByLinkId = GroupById(model.RawLinkSatelliteList, row => row.RawLinkId);
        var rawLinkSatelliteAttributesBySatelliteId = GroupById(model.RawLinkSatelliteAttributeList, row => row.RawLinkSatelliteId);

        PopulateRawMetaSqlModel(
            model,
            context,
            sourceFieldDetailsByFieldId,
            rawHubKeyPartsByHubId,
            rawHubSatellitesByHubId,
            rawHubSatelliteAttributesBySatelliteId,
            rawLinkHubsByLinkId,
            rawLinkSatellitesByLinkId,
            rawLinkSatelliteAttributesBySatelliteId);

        return context.MetaSql.ToXmlWorkspace(context.PathToNewMetaSqlWorkspace);
    }
}
