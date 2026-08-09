#nullable enable

using System.Collections.Generic;

namespace MetaBusinessDataVault
{
    public sealed partial class MetaBusinessDataVaultModel
    {
        public static MetaBusinessDataVaultModel CreateEmpty() => new();

        public List<BusinessBridge> BusinessBridgeList { get; set; } = new();
        public List<BusinessBridgeTraversal> BusinessBridgeTraversalList { get; set; } = new();
        public List<BusinessHierarchicalLink> BusinessHierarchicalLinkList { get; set; } = new();
        public List<BusinessHierarchicalLinkSatellite> BusinessHierarchicalLinkSatelliteList { get; set; } = new();
        public List<BusinessHub> BusinessHubList { get; set; } = new();
        public List<BusinessHubKeyPart> BusinessHubKeyPartList { get; set; } = new();
        public List<BusinessHubKeyPartDataTypeDetail> BusinessHubKeyPartDataTypeDetailList { get; set; } = new();
        public List<BusinessHubSatellite> BusinessHubSatelliteList { get; set; } = new();
        public List<BusinessLink> BusinessLinkList { get; set; } = new();
        public List<BusinessLinkRole> BusinessLinkRoleList { get; set; } = new();
        public List<BusinessLinkSatellite> BusinessLinkSatelliteList { get; set; } = new();
        public List<BusinessPointInTime> BusinessPointInTimeList { get; set; } = new();
        public List<BusinessPointInTimeHubSatellite> BusinessPointInTimeHubSatelliteList { get; set; } = new();
        public List<BusinessPointInTimeLinkSatellite> BusinessPointInTimeLinkSatelliteList { get; set; } = new();
        public List<BusinessPointInTimeStamp> BusinessPointInTimeStampList { get; set; } = new();
        public List<BusinessPointInTimeStampDataTypeDetail> BusinessPointInTimeStampDataTypeDetailList { get; set; } = new();
        public List<BusinessReference> BusinessReferenceList { get; set; } = new();
        public List<BusinessReferenceKeyPart> BusinessReferenceKeyPartList { get; set; } = new();
        public List<BusinessReferenceKeyPartDataTypeDetail> BusinessReferenceKeyPartDataTypeDetailList { get; set; } = new();
        public List<BusinessReferenceSatellite> BusinessReferenceSatelliteList { get; set; } = new();
        public List<BusinessSameAsLink> BusinessSameAsLinkList { get; set; } = new();
        public List<BusinessSameAsLinkSatellite> BusinessSameAsLinkSatelliteList { get; set; } = new();
        public List<BusinessSatellite> BusinessSatelliteList { get; set; } = new();
        public List<BusinessSatelliteAttribute> BusinessSatelliteAttributeList { get; set; } = new();
        public List<BusinessSatelliteAttributeDataTypeDetail> BusinessSatelliteAttributeDataTypeDetailList { get; set; } = new();
    }
}
