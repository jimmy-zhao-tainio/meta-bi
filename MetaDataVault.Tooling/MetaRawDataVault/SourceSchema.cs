using System;
using System.Xml.Serialization;

namespace MetaRawDataVault
{
    public sealed class SourceSchema
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("SourceSystemId")]
        public string SourceSystemId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public SourceSystem SourceSystem { get; set; } = null!;

    }
}
