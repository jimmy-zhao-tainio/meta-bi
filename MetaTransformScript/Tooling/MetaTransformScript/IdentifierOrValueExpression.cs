using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class IdentifierOrValueExpression
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;
        public bool ShouldSerializeValue() => !string.IsNullOrWhiteSpace(Value);

    }
}
