#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessSatelliteAttribute
    {
        public string Id { get; set; } = string.Empty;

        public string DataTypeId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public BusinessSatellite BusinessSatellite { get; set; } = null!;

    }
}
