using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatelliteKeyPartDataTypeDetail
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessLinkSatelliteKeyPartId")]
        public string BusinessLinkSatelliteKeyPartId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessLinkSatelliteKeyPart BusinessLinkSatelliteKeyPart { get; set; } = null!;

    }
}
