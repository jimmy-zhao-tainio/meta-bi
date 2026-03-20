using System;
using System.Xml.Serialization;

namespace MetaSql
{
    public sealed class Index
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("TableId")]
        public string TableId { get; set; } = string.Empty;

        [XmlElement("FilterSql")]
        public string FilterSql { get; set; } = string.Empty;
        public bool ShouldSerializeFilterSql() => !string.IsNullOrWhiteSpace(FilterSql);

        [XmlElement("IsClustered")]
        public string IsClustered { get; set; } = string.Empty;
        public bool ShouldSerializeIsClustered() => !string.IsNullOrWhiteSpace(IsClustered);

        [XmlElement("IsUnique")]
        public string IsUnique { get; set; } = string.Empty;
        public bool ShouldSerializeIsUnique() => !string.IsNullOrWhiteSpace(IsUnique);

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public Table Table { get; set; } = null!;

    }
}
