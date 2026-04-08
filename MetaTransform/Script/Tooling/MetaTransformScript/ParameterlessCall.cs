using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class ParameterlessCall
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("ParameterlessCallType")]
        public string ParameterlessCallType { get; set; } = string.Empty;
        public bool ShouldSerializeParameterlessCallType() => !string.IsNullOrWhiteSpace(ParameterlessCallType);

        [XmlIgnore]
        public PrimaryExpression Base { get; set; } = null!;

    }
}
