using MRDV = global::MetaRawDataVault;

namespace MetaDataVault.FromMetaSchema;

public sealed partial class RawDataVaultFromMetaSchemaService
{
    private static MRDV.MetaRawDataVaultModel CreateModel(FromMetaSchemaDraft draft)
    {
        var model = MRDV.MetaRawDataVaultModel.CreateEmpty();
        model.SourceSystemList.AddRange(draft.SourceSystems);
        model.SourceSchemaList.AddRange(draft.SourceSchemas);
        model.SourceTableList.AddRange(draft.SourceTables);
        model.SourceFieldList.AddRange(draft.SourceFields);
        model.SourceFieldDataTypeDetailList.AddRange(draft.SourceFieldDetails);
        model.SourceTableRelationshipList.AddRange(draft.SourceRelationships);
        model.SourceTableRelationshipFieldList.AddRange(draft.SourceRelationshipFields);
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
