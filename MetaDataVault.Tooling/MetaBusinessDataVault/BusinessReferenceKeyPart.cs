using System;
using System.Xml.Serialization;

namespace MetaBusinessDataVault
{
    public sealed class BusinessReferenceKeyPart
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BusinessReferenceId")]
        public string BusinessReferenceId { get; set; } = string.Empty;

        [XmlElement("DataTypeId")]
        public string DataTypeId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public BusinessReference BusinessReference { get; set; } = null!;

    }
}
