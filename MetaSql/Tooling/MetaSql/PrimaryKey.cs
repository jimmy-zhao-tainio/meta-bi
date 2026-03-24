using System;
using System.Xml.Serialization;

namespace MetaSql
{
    public sealed class PrimaryKey
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("TableId")]
        public string TableId { get; set; } = string.Empty;

        [XmlElement("IsClustered")]
        public string IsClustered { get; set; } = string.Empty;
        public bool ShouldSerializeIsClustered() => !string.IsNullOrWhiteSpace(IsClustered);

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public Table Table { get; set; } = null!;

    }
}
