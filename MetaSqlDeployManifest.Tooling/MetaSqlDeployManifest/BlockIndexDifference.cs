using System;
using System.Xml.Serialization;

namespace MetaSqlDeployManifest
{
    public sealed class BlockIndexDifference
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("DeployManifestId")]
        public string DeployManifestId { get; set; } = string.Empty;

        [XmlElement("DifferenceSummary")]
        public string DifferenceSummary { get; set; } = string.Empty;

        [XmlElement("LiveIndexId")]
        public string LiveIndexId { get; set; } = string.Empty;

        [XmlElement("SourceIndexId")]
        public string SourceIndexId { get; set; } = string.Empty;

        [XmlIgnore]
        public DeployManifest DeployManifest { get; set; } = null!;

    }
}
