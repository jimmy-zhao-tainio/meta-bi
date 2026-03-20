using System;
using System.Xml.Serialization;

namespace MetaSqlDeployManifest
{
    public sealed class AddTable
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("DeployManifestId")]
        public string DeployManifestId { get; set; } = string.Empty;

        [XmlElement("SourceTableId")]
        public string SourceTableId { get; set; } = string.Empty;

        [XmlIgnore]
        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
