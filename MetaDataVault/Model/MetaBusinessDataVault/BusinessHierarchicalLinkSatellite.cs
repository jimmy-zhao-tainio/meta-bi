#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatellite
    {
        public string Id { get; set; } = string.Empty;

        public BusinessHierarchicalLink BusinessHierarchicalLink { get; set; } = null!;

        public BusinessSatellite BusinessSatellite { get; set; } = null!;

    }
}
