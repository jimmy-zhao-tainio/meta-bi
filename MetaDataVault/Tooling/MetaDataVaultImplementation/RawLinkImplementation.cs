using System;
using System.Xml.Serialization;

namespace MetaDataVaultImplementation
{
    public sealed class RawLinkImplementation
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("AuditIdColumnName")]
        public string AuditIdColumnName { get; set; } = string.Empty;

        [XmlElement("AuditIdDataTypeId")]
        public string AuditIdDataTypeId { get; set; } = string.Empty;

        [XmlElement("EndHashKeyColumnPattern")]
        public string EndHashKeyColumnPattern { get; set; } = string.Empty;

        [XmlElement("HashKeyColumnName")]
        public string HashKeyColumnName { get; set; } = string.Empty;

        [XmlElement("HashKeyDataTypeId")]
        public string HashKeyDataTypeId { get; set; } = string.Empty;

        [XmlElement("HashKeyLength")]
        public string HashKeyLength { get; set; } = string.Empty;

        [XmlElement("HubForeignKeyNamePattern")]
        public string HubForeignKeyNamePattern { get; set; } = string.Empty;

        [XmlElement("LoadTimestampColumnName")]
        public string LoadTimestampColumnName { get; set; } = string.Empty;

        [XmlElement("LoadTimestampDataTypeId")]
        public string LoadTimestampDataTypeId { get; set; } = string.Empty;

        [XmlElement("LoadTimestampPrecision")]
        public string LoadTimestampPrecision { get; set; } = string.Empty;

        [XmlElement("PrimaryKeyNamePattern")]
        public string PrimaryKeyNamePattern { get; set; } = string.Empty;

        [XmlElement("RecordSourceColumnName")]
        public string RecordSourceColumnName { get; set; } = string.Empty;

        [XmlElement("RecordSourceDataTypeId")]
        public string RecordSourceDataTypeId { get; set; } = string.Empty;

        [XmlElement("RecordSourceLength")]
        public string RecordSourceLength { get; set; } = string.Empty;

        [XmlElement("SchemaName")]
        public string SchemaName { get; set; } = string.Empty;

        [XmlElement("TableNamePattern")]
        public string TableNamePattern { get; set; } = string.Empty;

    }
}

