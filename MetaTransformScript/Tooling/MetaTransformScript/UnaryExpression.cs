using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class UnaryExpression
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("UnaryExpressionType")]
        public string UnaryExpressionType { get; set; } = string.Empty;
        public bool ShouldSerializeUnaryExpressionType() => !string.IsNullOrWhiteSpace(UnaryExpressionType);

        [XmlIgnore]
        public ScalarExpression Base { get; set; } = null!;

    }
}
