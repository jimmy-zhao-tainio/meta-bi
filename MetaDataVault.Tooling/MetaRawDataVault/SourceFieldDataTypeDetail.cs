using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class SourceFieldDataTypeDetail
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("SourceFieldId")]
        public string SourceFieldId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;

        [XmlIgnore]
        public SourceField SourceField { get; set; } = null!;

    }
}
