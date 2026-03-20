using System;
using System.Xml.Serialization;

namespace MetaDataType
{
    public sealed class DataTypeSystem
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
