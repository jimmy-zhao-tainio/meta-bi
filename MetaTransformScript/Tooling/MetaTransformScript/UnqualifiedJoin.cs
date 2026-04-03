using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class UnqualifiedJoin
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("UnqualifiedJoinType")]
        public string UnqualifiedJoinType { get; set; } = string.Empty;
        public bool ShouldSerializeUnqualifiedJoinType() => !string.IsNullOrWhiteSpace(UnqualifiedJoinType);

        [XmlIgnore]
        public JoinTableReference Base { get; set; } = null!;

    }
}
