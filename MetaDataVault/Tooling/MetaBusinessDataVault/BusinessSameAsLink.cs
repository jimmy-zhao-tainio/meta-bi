using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessSameAsLink
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("EquivalentHubId")]
        public string EquivalentHubId { get; set; } = string.Empty;

        [XmlAttribute("PrimaryHubId")]
        public string PrimaryHubId { get; set; } = string.Empty;

        [XmlElement("Description")]
        public string Description { get; set; } = string.Empty;
        public bool ShouldSerializeDescription() => !string.IsNullOrWhiteSpace(Description);

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessHub EquivalentHub { get; set; } = null!;

        [XmlIgnore]
        public BusinessHub PrimaryHub { get; set; } = null!;

    }
}
