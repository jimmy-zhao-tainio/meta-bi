using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class Literal
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("LiteralType")]
        public string LiteralType { get; set; } = string.Empty;
        public bool ShouldSerializeLiteralType() => !string.IsNullOrWhiteSpace(LiteralType);

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;
        public bool ShouldSerializeValue() => !string.IsNullOrWhiteSpace(Value);

        [XmlIgnore]
        public ValueExpression Base { get; set; } = null!;

    }
}
