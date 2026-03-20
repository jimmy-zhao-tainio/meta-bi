using System;
using System.Xml.Serialization;

namespace MetaSql
{
    public sealed class TableColumnDataTypeDetail
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("TableColumnId")]
        public string TableColumnId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Value")]
        public string Value { get; set; } = string.Empty;

        [XmlIgnore]
        public TableColumn TableColumn { get; set; } = null!;

    }
}
