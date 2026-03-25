using System;
using System.Xml.Serialization;

namespace MetaSqlDeployManifest
{
    public sealed class AddSchema
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("DeployManifestId")]
        public string DeployManifestId { get; set; } = string.Empty;

        [XmlElement("SourceSchemaId")]
        public string SourceSchemaId { get; set; } = string.Empty;

        [XmlIgnore]
        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
