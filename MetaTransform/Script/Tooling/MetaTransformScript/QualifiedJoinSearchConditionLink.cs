using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class QualifiedJoinSearchConditionLink
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("OwnerId")]
        public string OwnerId { get; set; } = string.Empty;

        [XmlAttribute("ValueId")]
        public string ValueId { get; set; } = string.Empty;

        [XmlIgnore]
        public QualifiedJoin Owner { get; set; } = null!;

        [XmlIgnore]
        public BooleanExpression Value { get; set; } = null!;

    }
}
