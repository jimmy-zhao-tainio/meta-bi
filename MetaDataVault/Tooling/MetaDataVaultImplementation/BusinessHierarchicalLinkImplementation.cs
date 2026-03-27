using System;
using System.Xml.Serialization;

namespace MetaDataVaultImplementation
{
    public sealed class BusinessHierarchicalLinkImplementation
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("AuditIdColumnName")]
        public string AuditIdColumnName { get; set; } = string.Empty;

        [XmlElement("AuditIdDataTypeId")]
        public string AuditIdDataTypeId { get; set; } = string.Empty;

        [XmlElement("ChildHashKeyColumnName")]
        public string ChildHashKeyColumnName { get; set; } = string.Empty;

        [XmlElement("ChildHubForeignKeyNamePattern")]
        public string ChildHubForeignKeyNamePattern { get; set; } = string.Empty;

        [XmlElement("HashKeyColumnName")]
        public string HashKeyColumnName { get; set; } = string.Empty;

        [XmlElement("HashKeyDataTypeId")]
        public string HashKeyDataTypeId { get; set; } = string.Empty;

        [XmlElement("HashKeyLength")]
        public string HashKeyLength { get; set; } = string.Empty;

        [XmlElement("LoadTimestampColumnName")]
        public string LoadTimestampColumnName { get; set; } = string.Empty;
        public bool ShouldSerializeLoadTimestampColumnName() => !string.IsNullOrWhiteSpace(LoadTimestampColumnName);

        [XmlElement("LoadTimestampDataTypeId")]
        public string LoadTimestampDataTypeId { get; set; } = string.Empty;
        public bool ShouldSerializeLoadTimestampDataTypeId() => !string.IsNullOrWhiteSpace(LoadTimestampDataTypeId);

        [XmlElement("LoadTimestampPrecision")]
        public string LoadTimestampPrecision { get; set; } = string.Empty;
        public bool ShouldSerializeLoadTimestampPrecision() => !string.IsNullOrWhiteSpace(LoadTimestampPrecision);

        [XmlElement("ParentHashKeyColumnName")]
        public string ParentHashKeyColumnName { get; set; } = string.Empty;

        [XmlElement("ParentHubForeignKeyNamePattern")]
        public string ParentHubForeignKeyNamePattern { get; set; } = string.Empty;

        [XmlElement("PrimaryKeyNamePattern")]
        public string PrimaryKeyNamePattern { get; set; } = string.Empty;

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
