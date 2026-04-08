using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class CoalesceExpressionExpressionsItem
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("OwnerId")]
        public string OwnerId { get; set; } = string.Empty;

        [XmlAttribute("ValueId")]
        public string ValueId { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;
        public bool ShouldSerializeOrdinal() => !string.IsNullOrWhiteSpace(Ordinal);

        [XmlIgnore]
        public CoalesceExpression Owner { get; set; } = null!;

        [XmlIgnore]
        public ScalarExpression Value { get; set; } = null!;

    }
}
