using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class BooleanComparisonExpression
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("ComparisonType")]
        public string ComparisonType { get; set; } = string.Empty;
        public bool ShouldSerializeComparisonType() => !string.IsNullOrWhiteSpace(ComparisonType);

        [XmlIgnore]
        public BooleanExpression Base { get; set; } = null!;

    }
}
