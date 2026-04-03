using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class TableSampleClause
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("System")]
        public string System { get; set; } = string.Empty;
        public bool ShouldSerializeSystem() => !string.IsNullOrWhiteSpace(System);

        [XmlElement("TableSampleClauseOption")]
        public string TableSampleClauseOption { get; set; } = string.Empty;
        public bool ShouldSerializeTableSampleClauseOption() => !string.IsNullOrWhiteSpace(TableSampleClauseOption);

    }
}
