using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkSatelliteKeyPart
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessLinkSatelliteId")]
        public string BusinessLinkSatelliteId { get; set; } = string.Empty;

        [XmlElement("DataTypeId")]
        public string DataTypeId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessLinkSatellite BusinessLinkSatellite { get; set; } = null!;

    }
}
