using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class FunctionCall
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("UniqueRowFilter")]
        public string UniqueRowFilter { get; set; } = string.Empty;
        public bool ShouldSerializeUniqueRowFilter() => !string.IsNullOrWhiteSpace(UniqueRowFilter);

        [XmlElement("WithArrayWrapper")]
        public string WithArrayWrapper { get; set; } = string.Empty;
        public bool ShouldSerializeWithArrayWrapper() => !string.IsNullOrWhiteSpace(WithArrayWrapper);

        [XmlIgnore]
        public PrimaryExpression Base { get; set; } = null!;

    }
}
