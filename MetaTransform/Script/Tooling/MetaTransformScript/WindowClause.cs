using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class WindowClause
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

    }
}
