using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessLinkHub
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessHubId")]
        public string BusinessHubId { get; set; } = string.Empty;

        [XmlAttribute("BusinessLinkId")]
        public string BusinessLinkId { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlElement("RoleName")]
        public string RoleName { get; set; } = string.Empty;
        public bool ShouldSerializeRoleName() => !string.IsNullOrWhiteSpace(RoleName);

        [XmlIgnore]
        public BusinessHub BusinessHub { get; set; } = null!;

        [XmlIgnore]
        public BusinessLink BusinessLink { get; set; } = null!;

    }
}
