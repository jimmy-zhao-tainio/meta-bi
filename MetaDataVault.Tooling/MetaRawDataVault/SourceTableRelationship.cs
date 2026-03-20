using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class SourceTableRelationship
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
        public SourceTable SourceTable { get; set; } = null!;

        [XmlIgnore]
        public SourceTable TargetTable { get; set; } = null!;

    }
}
