using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class WindowDelimiter
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("WindowDelimiterType")]
        public string WindowDelimiterType { get; set; } = string.Empty;
        public bool ShouldSerializeWindowDelimiterType() => !string.IsNullOrWhiteSpace(WindowDelimiterType);

    }
}
