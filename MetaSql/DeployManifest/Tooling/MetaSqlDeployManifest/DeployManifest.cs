using System;
using System.Xml.Serialization;

namespace MetaSqlDeployManifest
{
    public sealed class DeployManifest
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("CreatedUtc")]
        public string CreatedUtc { get; set; } = string.Empty;

        [XmlElement("ExpectedLiveDatabasePresence")]
        public string ExpectedLiveDatabasePresence { get; set; } = string.Empty;

        [XmlElement("LiveInstanceFingerprint")]
        public string LiveInstanceFingerprint { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("SourceInstanceFingerprint")]
        public string SourceInstanceFingerprint { get; set; } = string.Empty;

        [XmlElement("TargetDescription")]
        public string TargetDescription { get; set; } = string.Empty;
        public bool ShouldSerializeTargetDescription() => !string.IsNullOrWhiteSpace(TargetDescription);

    }
}
