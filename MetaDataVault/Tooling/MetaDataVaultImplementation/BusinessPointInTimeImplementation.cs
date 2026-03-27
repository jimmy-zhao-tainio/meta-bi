using System;
using System.Xml.Serialization;

namespace MetaDataVaultImplementation
{
    public sealed class BusinessPointInTimeImplementation
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("AnchorHubForeignKeyNamePattern")]
        public string AnchorHubForeignKeyNamePattern { get; set; } = string.Empty;

        [XmlElement("AuditIdColumnName")]
        public string AuditIdColumnName { get; set; } = string.Empty;

        [XmlElement("AuditIdDataTypeId")]
        public string AuditIdDataTypeId { get; set; } = string.Empty;

        [XmlElement("ParentHashKeyColumnName")]
        public string ParentHashKeyColumnName { get; set; } = string.Empty;

        [XmlElement("ParentHashKeyDataTypeId")]
        public string ParentHashKeyDataTypeId { get; set; } = string.Empty;

        [XmlElement("ParentHashKeyLength")]
        public string ParentHashKeyLength { get; set; } = string.Empty;

        [XmlElement("SatelliteReferenceColumnNamePattern")]
        public string SatelliteReferenceColumnNamePattern { get; set; } = string.Empty;

        [XmlElement("SatelliteReferenceDataTypeId")]
        public string SatelliteReferenceDataTypeId { get; set; } = string.Empty;

        [XmlElement("SatelliteReferencePrecision")]
        public string SatelliteReferencePrecision { get; set; } = string.Empty;

        [XmlElement("SchemaName")]
        public string SchemaName { get; set; } = string.Empty;

        [XmlElement("SnapshotTimestampColumnName")]
        public string SnapshotTimestampColumnName { get; set; } = string.Empty;

        [XmlElement("SnapshotTimestampDataTypeId")]
        public string SnapshotTimestampDataTypeId { get; set; } = string.Empty;

        [XmlElement("SnapshotTimestampPrecision")]
        public string SnapshotTimestampPrecision { get; set; } = string.Empty;

        [XmlElement("TableNamePattern")]
        public string TableNamePattern { get; set; } = string.Empty;

    }
}
