using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class RawLinkHub
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("RawHubId")]
        public string RawHubId { get; set; } = string.Empty;

        [XmlAttribute("RawLinkId")]
        public string RawLinkId { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlElement("RoleName")]
        public string RoleName { get; set; } = string.Empty;
        public bool ShouldSerializeRoleName() => !string.IsNullOrWhiteSpace(RoleName);

        [XmlIgnore]
        public RawHub RawHub { get; set; } = null!;

        [XmlIgnore]
        public RawLink RawLink { get; set; } = null!;

    }
}
