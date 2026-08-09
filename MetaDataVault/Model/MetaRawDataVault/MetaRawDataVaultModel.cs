#nullable enable

using System.Collections.Generic;

namespace MetaRawDataVault;

public sealed partial class MetaRawDataVaultModel
{
    public static MetaRawDataVaultModel CreateEmpty() => new();

    public List<Field> FieldList { get; set; } = new();
    public List<FieldDataTypeDetail> FieldDataTypeDetailList { get; set; } = new();
    public List<RawHub> RawHubList { get; set; } = new();
    public List<RawHubKeyPart> RawHubKeyPartList { get; set; } = new();
    public List<RawHubSatellite> RawHubSatelliteList { get; set; } = new();
    public List<RawHubSatelliteAttribute> RawHubSatelliteAttributeList { get; set; } = new();
    public List<RawLink> RawLinkList { get; set; } = new();
    public List<RawLinkRole> RawLinkRoleList { get; set; } = new();
    public List<RawLinkSatellite> RawLinkSatelliteList { get; set; } = new();
    public List<RawLinkSatelliteAttribute> RawLinkSatelliteAttributeList { get; set; } = new();
}
