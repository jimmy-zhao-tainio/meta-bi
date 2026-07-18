using MetaRawDataVault;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

public static partial class Converter
{
    private static MetaSqlModel ConvertRaw(MetaRawDataVaultModel model, ConversionContext context)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(context);

        var fieldDetailsByFieldId = GroupById(model.FieldDataTypeDetailList, row => row.Field.Id);
        var rawHubKeyPartsByHubId = GroupById(model.RawHubKeyPartList, row => row.RawHub.Id);
        var rawHubSatelliteAttributesBySatelliteId = GroupById(model.RawHubSatelliteAttributeList, row => row.RawHubSatellite.Id);
        var rawHubSatellitesByHubId = GroupById(model.RawHubSatelliteList, row => row.RawHub.Id);
        var rawLinkHubsByLinkId = GroupById(model.RawLinkHubList, row => row.RawLink.Id);
        var rawLinkSatellitesByLinkId = GroupById(model.RawLinkSatelliteList, row => row.RawLink.Id);
        var rawLinkSatelliteAttributesBySatelliteId = GroupById(model.RawLinkSatelliteAttributeList, row => row.RawLinkSatellite.Id);

        PopulateRawMetaSqlModel(
            model,
            context,
            fieldDetailsByFieldId,
            rawHubKeyPartsByHubId,
            rawHubSatellitesByHubId,
            rawHubSatelliteAttributesBySatelliteId,
            rawLinkHubsByLinkId,
            rawLinkSatellitesByLinkId,
            rawLinkSatelliteAttributesBySatelliteId);

        return context.MetaSql;
    }
}
