using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessHierarchicalLinkSatelliteKeyPartDataTypeDetail
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessHierarchicalLinkSatelliteKeyPartId")]
        public string BusinessHierarchicalLinkSatelliteKeyPartId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessHierarchicalLinkSatelliteKeyPart BusinessHierarchicalLinkSatelliteKeyPart { get; set; } = null!;

    }
}
