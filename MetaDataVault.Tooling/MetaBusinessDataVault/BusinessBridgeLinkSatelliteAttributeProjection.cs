using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessBridgeLinkSatelliteAttributeProjection
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessBridgeId")]
        public string BusinessBridgeId { get; set; } = string.Empty;

        [XmlAttribute("BusinessLinkSatelliteAttributeId")]
        public string BusinessLinkSatelliteAttributeId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessBridge BusinessBridge { get; set; } = null!;

        [XmlIgnore]
        public BusinessLinkSatelliteAttribute BusinessLinkSatelliteAttribute { get; set; } = null!;

    }
}
