using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class XmlNamespacesAliasElement
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlIgnore]
        public XmlNamespacesElement Base { get; set; } = null!;

    }
}
