using System;
using System.Xml.Serialization;

namespace MetaSchema
{
    public sealed class Schema
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("SystemId")]
        public string SystemId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public System System { get; set; } = null!;

    }
}
