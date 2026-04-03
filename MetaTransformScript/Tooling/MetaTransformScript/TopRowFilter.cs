using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class TopRowFilter
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("Percent")]
        public string Percent { get; set; } = string.Empty;
        public bool ShouldSerializePercent() => !string.IsNullOrWhiteSpace(Percent);

        [XmlElement("WithApproximate")]
        public string WithApproximate { get; set; } = string.Empty;
        public bool ShouldSerializeWithApproximate() => !string.IsNullOrWhiteSpace(WithApproximate);

        [XmlElement("WithTies")]
        public string WithTies { get; set; } = string.Empty;
        public bool ShouldSerializeWithTies() => !string.IsNullOrWhiteSpace(WithTies);

    }
}
