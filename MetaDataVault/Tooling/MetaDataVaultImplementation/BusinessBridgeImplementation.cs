using System;
using System.Xml.Serialization;

namespace MetaDataVaultImplementation
{
    public sealed class BusinessBridgeImplementation
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("AnchorHubForeignKeyNamePattern")]
        public string AnchorHubForeignKeyNamePattern { get; set; } = string.Empty;

        [XmlElement("AuditIdColumnName")]
        public string AuditIdColumnName { get; set; } = string.Empty;

        [XmlElement("AuditIdDataTypeId")]
        public string AuditIdDataTypeId { get; set; } = string.Empty;

        [XmlElement("DepthColumnName")]
        public string DepthColumnName { get; set; } = string.Empty;
        public bool ShouldSerializeDepthColumnName() => !string.IsNullOrWhiteSpace(DepthColumnName);

        [XmlElement("DepthDataTypeId")]
        public string DepthDataTypeId { get; set; } = string.Empty;
        public bool ShouldSerializeDepthDataTypeId() => !string.IsNullOrWhiteSpace(DepthDataTypeId);

        [XmlElement("EffectiveFromColumnName")]
        public string EffectiveFromColumnName { get; set; } = string.Empty;
        public bool ShouldSerializeEffectiveFromColumnName() => !string.IsNullOrWhiteSpace(EffectiveFromColumnName);

        [XmlElement("EffectiveFromDataTypeId")]
        public string EffectiveFromDataTypeId { get; set; } = string.Empty;
        public bool ShouldSerializeEffectiveFromDataTypeId() => !string.IsNullOrWhiteSpace(EffectiveFromDataTypeId);

        [XmlElement("EffectiveFromPrecision")]
        public string EffectiveFromPrecision { get; set; } = string.Empty;
        public bool ShouldSerializeEffectiveFromPrecision() => !string.IsNullOrWhiteSpace(EffectiveFromPrecision);

        [XmlElement("EffectiveToColumnName")]
        public string EffectiveToColumnName { get; set; } = string.Empty;
        public bool ShouldSerializeEffectiveToColumnName() => !string.IsNullOrWhiteSpace(EffectiveToColumnName);

        [XmlElement("EffectiveToDataTypeId")]
        public string EffectiveToDataTypeId { get; set; } = string.Empty;
        public bool ShouldSerializeEffectiveToDataTypeId() => !string.IsNullOrWhiteSpace(EffectiveToDataTypeId);

        [XmlElement("EffectiveToPrecision")]
        public string EffectiveToPrecision { get; set; } = string.Empty;
        public bool ShouldSerializeEffectiveToPrecision() => !string.IsNullOrWhiteSpace(EffectiveToPrecision);

        [XmlElement("PathColumnName")]
        public string PathColumnName { get; set; } = string.Empty;
        public bool ShouldSerializePathColumnName() => !string.IsNullOrWhiteSpace(PathColumnName);

        [XmlElement("PathDataTypeId")]
        public string PathDataTypeId { get; set; } = string.Empty;
        public bool ShouldSerializePathDataTypeId() => !string.IsNullOrWhiteSpace(PathDataTypeId);

        [XmlElement("PathLength")]
        public string PathLength { get; set; } = string.Empty;
        public bool ShouldSerializePathLength() => !string.IsNullOrWhiteSpace(PathLength);

        [XmlElement("RelatedHashKeyColumnName")]
        public string RelatedHashKeyColumnName { get; set; } = string.Empty;

        [XmlElement("RelatedHashKeyDataTypeId")]
        public string RelatedHashKeyDataTypeId { get; set; } = string.Empty;

        [XmlElement("RelatedHashKeyLength")]
        public string RelatedHashKeyLength { get; set; } = string.Empty;

        [XmlElement("RootHashKeyColumnName")]
        public string RootHashKeyColumnName { get; set; } = string.Empty;

        [XmlElement("RootHashKeyDataTypeId")]
        public string RootHashKeyDataTypeId { get; set; } = string.Empty;

        [XmlElement("RootHashKeyLength")]
        public string RootHashKeyLength { get; set; } = string.Empty;

        [XmlElement("SchemaName")]
        public string SchemaName { get; set; } = string.Empty;

        [XmlElement("TableNamePattern")]
        public string TableNamePattern { get; set; } = string.Empty;

    }
}

