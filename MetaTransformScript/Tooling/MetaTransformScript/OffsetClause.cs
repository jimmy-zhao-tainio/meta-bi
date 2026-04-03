using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class OffsetClause
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("WithApproximate")]
        public string WithApproximate { get; set; } = string.Empty;
        public bool ShouldSerializeWithApproximate() => !string.IsNullOrWhiteSpace(WithApproximate);

    }
}
