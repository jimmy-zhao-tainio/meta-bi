using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class MultiPartIdentifier
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("Count")]
        public string Count { get; set; } = string.Empty;
        public bool ShouldSerializeCount() => !string.IsNullOrWhiteSpace(Count);

    }
}
