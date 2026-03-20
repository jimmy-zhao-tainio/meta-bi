using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class RawLinkSatelliteAttribute
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("RawLinkSatelliteId")]
        public string RawLinkSatelliteId { get; set; } = string.Empty;

        [XmlAttribute("SourceFieldId")]
        public string SourceFieldId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public RawLinkSatellite RawLinkSatellite { get; set; } = null!;

        [XmlIgnore]
        public SourceField SourceField { get; set; } = null!;

    }
}
