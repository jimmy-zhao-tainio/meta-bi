using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class ColumnReferenceExpression
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("ColumnType")]
        public string ColumnType { get; set; } = string.Empty;
        public bool ShouldSerializeColumnType() => !string.IsNullOrWhiteSpace(ColumnType);

        [XmlIgnore]
        public PrimaryExpression Base { get; set; } = null!;

    }
}
