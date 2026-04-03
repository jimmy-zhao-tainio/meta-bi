using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class InPredicate
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("NotDefined")]
        public string NotDefined { get; set; } = string.Empty;
        public bool ShouldSerializeNotDefined() => !string.IsNullOrWhiteSpace(NotDefined);

        [XmlIgnore]
        public BooleanExpression Base { get; set; } = null!;

    }
}
