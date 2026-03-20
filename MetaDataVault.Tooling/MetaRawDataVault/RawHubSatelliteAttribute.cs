using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class RawHubSatelliteAttribute
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("RawHubSatelliteId")]
        public string RawHubSatelliteId { get; set; } = string.Empty;

        [XmlAttribute("SourceFieldId")]
        public string SourceFieldId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public RawHubSatellite RawHubSatellite { get; set; } = null!;

        [XmlIgnore]
        public SourceField SourceField { get; set; } = null!;

    }
}
