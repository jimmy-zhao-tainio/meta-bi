using System;
using System.Xml.Serialization;

namespace MetaSql
{
    public sealed class Schema
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("DatabaseId")]
        public string DatabaseId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public Database Database { get; set; } = null!;

    }
}
