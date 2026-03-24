using System;
using System.Xml.Serialization;

namespace MetaSql
{
    public sealed class ForeignKeyColumn
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("ForeignKeyId")]
        public string ForeignKeyId { get; set; } = string.Empty;

        [XmlAttribute("SourceColumnId")]
        public string SourceColumnId { get; set; } = string.Empty;

        [XmlAttribute("TargetColumnId")]
        public string TargetColumnId { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public ForeignKey ForeignKey { get; set; } = null!;

        [XmlIgnore]
        public TableColumn SourceColumn { get; set; } = null!;

        [XmlIgnore]
        public TableColumn TargetColumn { get; set; } = null!;

    }
}
