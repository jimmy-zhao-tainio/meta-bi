using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class RawHubKeyPart
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("RawHubId")]
        public string RawHubId { get; set; } = string.Empty;

        [XmlAttribute("SourceFieldId")]
        public string SourceFieldId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public RawHub RawHub { get; set; } = null!;

        [XmlIgnore]
        public SourceField SourceField { get; set; } = null!;

    }
}
