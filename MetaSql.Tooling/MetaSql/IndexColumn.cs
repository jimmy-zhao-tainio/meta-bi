using System;
using System.Xml.Serialization;

namespace MetaSql
{
    public sealed class IndexColumn
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("IndexId")]
        public string IndexId { get; set; } = string.Empty;

        [XmlAttribute("TableColumnId")]
        public string TableColumnId { get; set; } = string.Empty;

        [XmlElement("IsDescending")]
        public string IsDescending { get; set; } = string.Empty;
        public bool ShouldSerializeIsDescending() => !string.IsNullOrWhiteSpace(IsDescending);

        [XmlElement("IsIncluded")]
        public string IsIncluded { get; set; } = string.Empty;
        public bool ShouldSerializeIsIncluded() => !string.IsNullOrWhiteSpace(IsIncluded);

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public Index Index { get; set; } = null!;

        [XmlIgnore]
        public TableColumn TableColumn { get; set; } = null!;

    }
}
