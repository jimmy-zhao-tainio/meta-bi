using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class NamedTableReferenceSchemaObjectLink
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("OwnerId")]
        public string OwnerId { get; set; } = string.Empty;

        [XmlAttribute("ValueId")]
        public string ValueId { get; set; } = string.Empty;

        [XmlIgnore]
        public NamedTableReference Owner { get; set; } = null!;

        [XmlIgnore]
        public SchemaObjectName Value { get; set; } = null!;

    }
}
