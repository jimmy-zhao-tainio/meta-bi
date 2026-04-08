using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class QualifiedJoin
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("JoinHint")]
        public string JoinHint { get; set; } = string.Empty;
        public bool ShouldSerializeJoinHint() => !string.IsNullOrWhiteSpace(JoinHint);

        [XmlElement("QualifiedJoinType")]
        public string QualifiedJoinType { get; set; } = string.Empty;
        public bool ShouldSerializeQualifiedJoinType() => !string.IsNullOrWhiteSpace(QualifiedJoinType);

        [XmlIgnore]
        public JoinTableReference Base { get; set; } = null!;

    }
}
