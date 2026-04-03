using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class GroupByClause
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("All")]
        public string All { get; set; } = string.Empty;
        public bool ShouldSerializeAll() => !string.IsNullOrWhiteSpace(All);

        [XmlElement("GroupByOption")]
        public string GroupByOption { get; set; } = string.Empty;
        public bool ShouldSerializeGroupByOption() => !string.IsNullOrWhiteSpace(GroupByOption);

    }
}
