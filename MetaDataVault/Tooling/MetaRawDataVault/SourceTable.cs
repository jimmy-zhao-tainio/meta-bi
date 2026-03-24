using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class SourceTable
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("SourceSchemaId")]
        public string SourceSchemaId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public SourceSchema SourceSchema { get; set; } = null!;

    }
}
