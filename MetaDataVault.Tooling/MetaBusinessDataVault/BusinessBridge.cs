using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessBridge
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("AnchorHubId")]
        public string AnchorHubId { get; set; } = string.Empty;

        [XmlElement("Description")]
        public string Description { get; set; } = string.Empty;
        public bool ShouldSerializeDescription() => !string.IsNullOrWhiteSpace(Description);

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessHub AnchorHub { get; set; } = null!;

    }
}
