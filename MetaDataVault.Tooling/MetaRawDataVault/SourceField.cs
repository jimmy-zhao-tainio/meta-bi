using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class SourceField
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("SourceTableId")]
        public string SourceTableId { get; set; } = string.Empty;

        [XmlElement("DataTypeId")]
        public string DataTypeId { get; set; } = string.Empty;

        [XmlElement("IsNullable")]
        public string IsNullable { get; set; } = string.Empty;
        public bool ShouldSerializeIsNullable() => !string.IsNullOrWhiteSpace(IsNullable);

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;
        public bool ShouldSerializeOrdinal() => !string.IsNullOrWhiteSpace(Ordinal);

        [XmlIgnore]
        public SourceTable SourceTable { get; set; } = null!;

    }
}
