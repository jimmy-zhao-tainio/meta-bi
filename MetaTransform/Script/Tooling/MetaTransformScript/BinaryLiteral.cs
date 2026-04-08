using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class BinaryLiteral
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("IsLargeObject")]
        public string IsLargeObject { get; set; } = string.Empty;
        public bool ShouldSerializeIsLargeObject() => !string.IsNullOrWhiteSpace(IsLargeObject);

        [XmlElement("LiteralType")]
        public string LiteralType { get; set; } = string.Empty;
        public bool ShouldSerializeLiteralType() => !string.IsNullOrWhiteSpace(LiteralType);

        [XmlIgnore]
        public Literal Base { get; set; } = null!;

    }
}
