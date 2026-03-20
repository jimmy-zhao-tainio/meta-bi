using System;
using System.Xml.Serialization;

namespace MetaSql
{
    public sealed class Database
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("Collation")]
        public string Collation { get; set; } = string.Empty;
        public bool ShouldSerializeCollation() => !string.IsNullOrWhiteSpace(Collation);

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Platform")]
        public string Platform { get; set; } = string.Empty;

    }
}
