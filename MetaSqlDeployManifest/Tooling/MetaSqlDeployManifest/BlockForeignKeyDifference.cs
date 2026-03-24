using System;
using System.Xml.Serialization;

namespace MetaSqlDeployManifest
{
    public sealed class BlockForeignKeyDifference
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("DeployManifestId")]
        public string DeployManifestId { get; set; } = string.Empty;

        [XmlElement("DifferenceSummary")]
        public string DifferenceSummary { get; set; } = string.Empty;

        [XmlElement("LiveForeignKeyId")]
        public string LiveForeignKeyId { get; set; } = string.Empty;

        [XmlElement("SourceForeignKeyId")]
        public string SourceForeignKeyId { get; set; } = string.Empty;

        [XmlIgnore]
        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
