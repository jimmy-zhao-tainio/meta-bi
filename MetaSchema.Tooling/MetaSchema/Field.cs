using System;
using System.Xml.Serialization;

namespace MetaSchema
{
    public sealed class Field
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("TableId")]
        public string TableId { get; set; } = string.Empty;

        [XmlElement("IsNullable")]
        public string IsNullable { get; set; } = string.Empty;
        public bool ShouldSerializeIsNullable() => !string.IsNullOrWhiteSpace(IsNullable);

        [XmlElement("MetaDataTypeId")]
        public string MetaDataTypeId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;
        public bool ShouldSerializeOrdinal() => !string.IsNullOrWhiteSpace(Ordinal);

        [XmlIgnore]
        public Table Table { get; set; } = null!;

    }
}
