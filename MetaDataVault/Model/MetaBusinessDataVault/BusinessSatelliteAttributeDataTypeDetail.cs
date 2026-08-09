#nullable enable

namespace MetaBusinessDataVault
{
    public sealed class BusinessSatelliteAttributeDataTypeDetail
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public BusinessSatelliteAttribute BusinessSatelliteAttribute { get; set; } = null!;

    }
}
