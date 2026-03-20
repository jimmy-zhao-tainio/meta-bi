using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class SourceSystem
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("Description")]
        public string Description { get; set; } = string.Empty;
        public bool ShouldSerializeDescription() => !string.IsNullOrWhiteSpace(Description);

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

    }
}
