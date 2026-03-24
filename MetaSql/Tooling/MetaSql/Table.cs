using System;
using System.Xml.Serialization;

namespace MetaSql
{
    public sealed class Table
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("SchemaId")]
        public string SchemaId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public Schema Schema { get; set; } = null!;

    }
}
