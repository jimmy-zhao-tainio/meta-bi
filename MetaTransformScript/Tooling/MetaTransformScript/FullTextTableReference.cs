using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class FullTextTableReference
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("FullTextFunctionType")]
        public string FullTextFunctionType { get; set; } = string.Empty;
        public bool ShouldSerializeFullTextFunctionType() => !string.IsNullOrWhiteSpace(FullTextFunctionType);

        [XmlIgnore]
        public TableReferenceWithAlias Base { get; set; } = null!;

    }
}
