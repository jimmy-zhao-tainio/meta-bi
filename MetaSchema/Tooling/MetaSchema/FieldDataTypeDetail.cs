using System;
using System.Xml.Serialization;

namespace MetaSchema
{
    public sealed class FieldDataTypeDetail
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("FieldId")]
        public string FieldId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;

        [XmlIgnore]
        public Field Field { get; set; } = null!;

    }
}
