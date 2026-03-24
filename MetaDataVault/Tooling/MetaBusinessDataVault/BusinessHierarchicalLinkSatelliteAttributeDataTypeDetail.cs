using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessHierarchicalLinkSatelliteAttributeId")]
        public string BusinessHierarchicalLinkSatelliteAttributeId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessHierarchicalLinkSatelliteAttribute BusinessHierarchicalLinkSatelliteAttribute { get; set; } = null!;

    }
}
