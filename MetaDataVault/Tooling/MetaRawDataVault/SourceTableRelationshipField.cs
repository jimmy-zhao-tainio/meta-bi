using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class SourceTableRelationshipField
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("SourceFieldId")]
        public string SourceFieldId { get; set; } = string.Empty;

        [XmlAttribute("SourceTableRelationshipId")]
        public string SourceTableRelationshipId { get; set; } = string.Empty;

        [XmlAttribute("TargetFieldId")]
        public string TargetFieldId { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;

        [XmlIgnore]
        public SourceField SourceField { get; set; } = null!;

        [XmlIgnore]
        public SourceTableRelationship SourceTableRelationship { get; set; } = null!;

        [XmlIgnore]
        public SourceField TargetField { get; set; } = null!;

    }
}
