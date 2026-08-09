#nullable enable

using System.Collections.Generic;

namespace MetaDataVaultImplementation
{
    public sealed partial class MetaDataVaultImplementationModel
    {
        public static MetaDataVaultImplementationModel CreateEmpty() => new();

        public List<BusinessBridgeImplementation> BusinessBridgeImplementationList { get; set; } = new();
        public List<BusinessHierarchicalLinkImplementation> BusinessHierarchicalLinkImplementationList { get; set; } = new();
        public List<BusinessHierarchicalLinkSatelliteImplementation> BusinessHierarchicalLinkSatelliteImplementationList { get; set; } = new();
        public List<BusinessHubImplementation> BusinessHubImplementationList { get; set; } = new();
        public List<BusinessHubSatelliteImplementation> BusinessHubSatelliteImplementationList { get; set; } = new();
        public List<BusinessLinkImplementation> BusinessLinkImplementationList { get; set; } = new();
        public List<BusinessLinkSatelliteImplementation> BusinessLinkSatelliteImplementationList { get; set; } = new();
        public List<BusinessPointInTimeImplementation> BusinessPointInTimeImplementationList { get; set; } = new();
        public List<BusinessReferenceImplementation> BusinessReferenceImplementationList { get; set; } = new();
        public List<BusinessReferenceSatelliteImplementation> BusinessReferenceSatelliteImplementationList { get; set; } = new();
        public List<BusinessSameAsLinkImplementation> BusinessSameAsLinkImplementationList { get; set; } = new();
        public List<BusinessSameAsLinkSatelliteImplementation> BusinessSameAsLinkSatelliteImplementationList { get; set; } = new();
        public List<RawHubImplementation> RawHubImplementationList { get; set; } = new();
        public List<RawHubSatelliteImplementation> RawHubSatelliteImplementationList { get; set; } = new();
        public List<RawLinkImplementation> RawLinkImplementationList { get; set; } = new();
        public List<RawLinkSatelliteImplementation> RawLinkSatelliteImplementationList { get; set; } = new();
    }
}
