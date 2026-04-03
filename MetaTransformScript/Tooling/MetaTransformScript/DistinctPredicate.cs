using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class DistinctPredicate
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("IsNot")]
        public string IsNot { get; set; } = string.Empty;
        public bool ShouldSerializeIsNot() => !string.IsNullOrWhiteSpace(IsNot);

        [XmlIgnore]
        public BooleanExpression Base { get; set; } = null!;

    }
}
