using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class RawLinkSatellite
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("RawLinkId")]
        public string RawLinkId { get; set; } = string.Empty;

        [XmlAttribute("SourceTableId")]
        public string SourceTableId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("SatelliteKind")]
        public string SatelliteKind { get; set; } = string.Empty;

        [XmlIgnore]
        public RawLink RawLink { get; set; } = null!;

        [XmlIgnore]
        public SourceTable SourceTable { get; set; } = null!;

    }
}
