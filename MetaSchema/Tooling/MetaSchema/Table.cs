using System;
using System.Xml.Serialization;

namespace MetaSchema
{
    public sealed class Table
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("SchemaId")]
        public string SchemaId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("ObjectType")]
        public string ObjectType { get; set; } = string.Empty;
        public bool ShouldSerializeObjectType() => !string.IsNullOrWhiteSpace(ObjectType);

        [XmlIgnore]
        public Schema Schema { get; set; } = null!;

    }
}
