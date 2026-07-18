using MRDV = global::MetaRawDataVault;

namespace MetaConvert.SchemaToDataVault;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    private static MRDV.MetaRawDataVaultModel CreateModel(FromMetaSchemaDraft draft)
    {
        var model = MRDV.MetaRawDataVaultModel.CreateEmpty();
        model.FieldList.AddRange(draft.Fields);
        model.FieldDataTypeDetailList.AddRange(draft.FieldDetails);
        model.RawHubList.AddRange(draft.RawHubs);
        model.RawHubKeyPartList.AddRange(draft.RawHubKeyParts);
        model.RawHubSatelliteList.AddRange(draft.RawHubSatellites);
        model.RawHubSatelliteAttributeList.AddRange(draft.RawHubSatelliteAttributes);
        model.RawLinkList.AddRange(draft.RawLinks);
        model.RawLinkHubList.AddRange(draft.RawLinkHubs);
        model.RawLinkSatelliteList.AddRange(draft.RawLinkSatellites);
        model.RawLinkSatelliteAttributeList.AddRange(draft.RawLinkSatelliteAttributes);
        return model;
    }
}
