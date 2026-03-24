using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLinkSatelliteAttributeDataTypeDetail
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessSameAsLinkSatelliteAttributeId")]
        public string BusinessSameAsLinkSatelliteAttributeId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessSameAsLinkSatelliteAttribute BusinessSameAsLinkSatelliteAttribute { get; set; } = null!;

    }
}
