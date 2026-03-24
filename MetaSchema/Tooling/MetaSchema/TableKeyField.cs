using System;
using System.Xml.Serialization;

namespace MetaSchema
{
    public sealed class TableKeyField
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("FieldId")]
        public string FieldId { get; set; } = string.Empty;

        [XmlAttribute("TableKeyId")]
        public string TableKeyId { get; set; } = string.Empty;

        [XmlElement("FieldName")]
        public string FieldName { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public Field Field { get; set; } = null!;

        [XmlIgnore]
        public TableKey TableKey { get; set; } = null!;

    }
}
