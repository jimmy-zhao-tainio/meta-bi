using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessHubSatelliteAttributeDataTypeDetail
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessHubSatelliteAttributeId")]
        public string BusinessHubSatelliteAttributeId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessHubSatelliteAttribute BusinessHubSatelliteAttribute { get; set; } = null!;

    }
}
