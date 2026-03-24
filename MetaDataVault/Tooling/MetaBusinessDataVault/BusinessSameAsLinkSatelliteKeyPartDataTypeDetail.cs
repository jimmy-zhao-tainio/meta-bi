using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLinkSatelliteKeyPartDataTypeDetail
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessSameAsLinkSatelliteKeyPartId")]
        public string BusinessSameAsLinkSatelliteKeyPartId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessSameAsLinkSatelliteKeyPart BusinessSameAsLinkSatelliteKeyPart { get; set; } = null!;

    }
}
