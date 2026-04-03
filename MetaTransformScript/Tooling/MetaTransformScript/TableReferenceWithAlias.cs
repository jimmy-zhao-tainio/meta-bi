using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class TableReferenceWithAlias
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("ForPath")]
        public string ForPath { get; set; } = string.Empty;
        public bool ShouldSerializeForPath() => !string.IsNullOrWhiteSpace(ForPath);

        [XmlIgnore]
        public TableReference Base { get; set; } = null!;

    }
}
