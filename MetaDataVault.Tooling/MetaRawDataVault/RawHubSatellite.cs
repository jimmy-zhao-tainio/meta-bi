using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class RawHubSatellite
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("RawHubId")]
        public string RawHubId { get; set; } = string.Empty;

        [XmlAttribute("SourceTableId")]
        public string SourceTableId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("SatelliteKind")]
        public string SatelliteKind { get; set; } = string.Empty;

        [XmlIgnore]
        public RawHub RawHub { get; set; } = null!;

        [XmlIgnore]
        public SourceTable SourceTable { get; set; } = null!;

    }
}
