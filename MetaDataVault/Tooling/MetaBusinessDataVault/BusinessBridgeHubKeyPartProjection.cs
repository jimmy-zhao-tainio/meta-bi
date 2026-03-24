using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeHubKeyPartProjection
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessBridgeId")]
        public string BusinessBridgeId { get; set; } = string.Empty;

        [XmlAttribute("BusinessHubKeyPartId")]
        public string BusinessHubKeyPartId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessBridge BusinessBridge { get; set; } = null!;

        [XmlIgnore]
        public BusinessHubKeyPart BusinessHubKeyPart { get; set; } = null!;

    }
}
