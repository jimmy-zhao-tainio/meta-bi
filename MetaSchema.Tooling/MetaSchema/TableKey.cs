using System;
using System.Xml.Serialization;

namespace MetaSchema
{
    public sealed class TableKey
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("TableId")]
        public string TableId { get; set; } = string.Empty;

        [XmlElement("KeyType")]
        public string KeyType { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public Table Table { get; set; } = null!;

    }
}
