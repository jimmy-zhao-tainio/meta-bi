using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatellite
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessHierarchicalLinkId")]
        public string BusinessHierarchicalLinkId { get; set; } = string.Empty;

        [XmlElement("Description")]
        public string Description { get; set; } = string.Empty;
        public bool ShouldSerializeDescription() => !string.IsNullOrWhiteSpace(Description);

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("SatelliteKind")]
        public string SatelliteKind { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessHierarchicalLink BusinessHierarchicalLink { get; set; } = null!;

    }
}
