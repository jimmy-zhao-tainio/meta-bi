using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class BinaryQueryExpression
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("All")]
        public string All { get; set; } = string.Empty;
        public bool ShouldSerializeAll() => !string.IsNullOrWhiteSpace(All);

        [XmlElement("BinaryQueryExpressionType")]
        public string BinaryQueryExpressionType { get; set; } = string.Empty;
        public bool ShouldSerializeBinaryQueryExpressionType() => !string.IsNullOrWhiteSpace(BinaryQueryExpressionType);

        [XmlIgnore]
        public QueryExpression Base { get; set; } = null!;

    }
}
