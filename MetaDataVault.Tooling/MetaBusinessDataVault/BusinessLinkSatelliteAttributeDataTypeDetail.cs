using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatelliteAttributeDataTypeDetail
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessLinkSatelliteAttributeId")]
        public string BusinessLinkSatelliteAttributeId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessLinkSatelliteAttribute BusinessLinkSatelliteAttribute { get; set; } = null!;

    }
}
