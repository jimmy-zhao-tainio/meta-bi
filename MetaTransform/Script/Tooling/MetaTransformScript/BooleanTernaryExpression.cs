using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class BooleanTernaryExpression
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("TernaryExpressionType")]
        public string TernaryExpressionType { get; set; } = string.Empty;
        public bool ShouldSerializeTernaryExpressionType() => !string.IsNullOrWhiteSpace(TernaryExpressionType);

        [XmlIgnore]
        public BooleanExpression Base { get; set; } = null!;

    }
}
