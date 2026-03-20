using System;
using System.Xml.Serialization;

namespace MetaSql
{
    public sealed class PrimaryKeyColumn
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("PrimaryKeyId")]
        public string PrimaryKeyId { get; set; } = string.Empty;

        [XmlAttribute("TableColumnId")]
        public string TableColumnId { get; set; } = string.Empty;

        [XmlElement("IsDescending")]
        public string IsDescending { get; set; } = string.Empty;
        public bool ShouldSerializeIsDescending() => !string.IsNullOrWhiteSpace(IsDescending);

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public PrimaryKey PrimaryKey { get; set; } = null!;

        [XmlIgnore]
        public TableColumn TableColumn { get; set; } = null!;

    }
}
