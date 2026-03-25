using System;
using System.Xml.Serialization;

namespace MetaDataVaultImplementation
{
    public sealed class BusinessReferenceSatelliteImplementation
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("AuditIdColumnName")]
        public string AuditIdColumnName { get; set; } = string.Empty;

        [XmlElement("AuditIdDataTypeId")]
        public string AuditIdDataTypeId { get; set; } = string.Empty;

        [XmlElement("HashDiffColumnName")]
        public string HashDiffColumnName { get; set; } = string.Empty;
        public bool ShouldSerializeHashDiffColumnName() => !string.IsNullOrWhiteSpace(HashDiffColumnName);

        [XmlElement("HashDiffDataTypeId")]
        public string HashDiffDataTypeId { get; set; } = string.Empty;
        public bool ShouldSerializeHashDiffDataTypeId() => !string.IsNullOrWhiteSpace(HashDiffDataTypeId);

        [XmlElement("HashDiffLength")]
        public string HashDiffLength { get; set; } = string.Empty;
        public bool ShouldSerializeHashDiffLength() => !string.IsNullOrWhiteSpace(HashDiffLength);

        [XmlElement("LoadTimestampColumnName")]
        public string LoadTimestampColumnName { get; set; } = string.Empty;
        public bool ShouldSerializeLoadTimestampColumnName() => !string.IsNullOrWhiteSpace(LoadTimestampColumnName);

        [XmlElement("LoadTimestampDataTypeId")]
        public string LoadTimestampDataTypeId { get; set; } = string.Empty;
        public bool ShouldSerializeLoadTimestampDataTypeId() => !string.IsNullOrWhiteSpace(LoadTimestampDataTypeId);

        [XmlElement("LoadTimestampPrecision")]
        public string LoadTimestampPrecision { get; set; } = string.Empty;
        public bool ShouldSerializeLoadTimestampPrecision() => !string.IsNullOrWhiteSpace(LoadTimestampPrecision);

        [XmlElement("ParentForeignKeyNamePattern")]
        public string ParentForeignKeyNamePattern { get; set; } = string.Empty;

        [XmlElement("ParentHashKeyColumnName")]
        public string ParentHashKeyColumnName { get; set; } = string.Empty;

        [XmlElement("ParentHashKeyDataTypeId")]
        public string ParentHashKeyDataTypeId { get; set; } = string.Empty;

        [XmlElement("ParentHashKeyLength")]
        public string ParentHashKeyLength { get; set; } = string.Empty;

        [XmlElement("RecordSourceColumnName")]
        public string RecordSourceColumnName { get; set; } = string.Empty;
        public bool ShouldSerializeRecordSourceColumnName() => !string.IsNullOrWhiteSpace(RecordSourceColumnName);

        [XmlElement("RecordSourceDataTypeId")]
        public string RecordSourceDataTypeId { get; set; } = string.Empty;
        public bool ShouldSerializeRecordSourceDataTypeId() => !string.IsNullOrWhiteSpace(RecordSourceDataTypeId);

        [XmlElement("RecordSourceLength")]
        public string RecordSourceLength { get; set; } = string.Empty;
        public bool ShouldSerializeRecordSourceLength() => !string.IsNullOrWhiteSpace(RecordSourceLength);

        [XmlElement("SchemaName")]
        public string SchemaName { get; set; } = string.Empty;

        [XmlElement("TableNamePattern")]
        public string TableNamePattern { get; set; } = string.Empty;

    }
}

