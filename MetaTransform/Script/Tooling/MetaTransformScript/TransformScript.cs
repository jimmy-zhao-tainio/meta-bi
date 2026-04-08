using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class TransformScript
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("LanguageProfileId")]
        public string LanguageProfileId { get; set; } = string.Empty;
        public bool ShouldSerializeLanguageProfileId() => !string.IsNullOrWhiteSpace(LanguageProfileId);

        [XmlElement("SourcePath")]
        public string SourcePath { get; set; } = string.Empty;
        public bool ShouldSerializeSourcePath() => !string.IsNullOrWhiteSpace(SourcePath);

    }
}
