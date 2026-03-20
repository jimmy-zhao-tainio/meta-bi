using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class RawLink
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("SourceTableRelationshipId")]
        public string SourceTableRelationshipId { get; set; } = string.Empty;

        [XmlElement("LinkKind")]
        public string LinkKind { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public SourceTableRelationship SourceTableRelationship { get; set; } = null!;

    }
}
