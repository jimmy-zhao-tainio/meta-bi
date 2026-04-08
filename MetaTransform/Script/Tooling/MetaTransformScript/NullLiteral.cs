using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class NullLiteral
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("LiteralType")]
        public string LiteralType { get; set; } = string.Empty;
        public bool ShouldSerializeLiteralType() => !string.IsNullOrWhiteSpace(LiteralType);

        [XmlIgnore]
        public Literal Base { get; set; } = null!;

    }
}
