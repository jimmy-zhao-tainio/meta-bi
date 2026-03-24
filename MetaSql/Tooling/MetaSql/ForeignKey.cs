using System;
using System.Xml.Serialization;

namespace MetaSql
{
    public sealed class ForeignKey
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("SourceTableId")]
        public string SourceTableId { get; set; } = string.Empty;

        [XmlAttribute("TargetTableId")]
        public string TargetTableId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public Table SourceTable { get; set; } = null!;

        [XmlIgnore]
        public Table TargetTable { get; set; } = null!;

    }
}
