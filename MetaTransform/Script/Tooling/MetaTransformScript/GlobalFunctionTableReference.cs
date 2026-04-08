using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class GlobalFunctionTableReference
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlIgnore]
        public TableReferenceWithAlias Base { get; set; } = null!;

    }
}
