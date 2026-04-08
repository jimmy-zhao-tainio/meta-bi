using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class GlobalVariableExpression
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;
        public bool ShouldSerializeName() => !string.IsNullOrWhiteSpace(Name);

        [XmlIgnore]
        public ValueExpression Base { get; set; } = null!;

    }
}
