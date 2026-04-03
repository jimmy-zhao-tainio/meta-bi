using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class BooleanBinaryExpression
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("BinaryExpressionType")]
        public string BinaryExpressionType { get; set; } = string.Empty;
        public bool ShouldSerializeBinaryExpressionType() => !string.IsNullOrWhiteSpace(BinaryExpressionType);

        [XmlIgnore]
        public BooleanExpression Base { get; set; } = null!;

    }
}
