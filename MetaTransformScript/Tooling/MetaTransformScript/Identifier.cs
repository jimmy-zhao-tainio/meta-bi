using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class Identifier
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("QuoteType")]
        public string QuoteType { get; set; } = string.Empty;
        public bool ShouldSerializeQuoteType() => !string.IsNullOrWhiteSpace(QuoteType);

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;
        public bool ShouldSerializeValue() => !string.IsNullOrWhiteSpace(Value);

    }
}
