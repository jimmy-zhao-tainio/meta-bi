using System;
using System.Xml.Serialization;

namespace MetaSql
{
    public sealed class TableColumn
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("TableId")]
        public string TableId { get; set; } = string.Empty;

        [XmlElement("ExpressionSql")]
        public string ExpressionSql { get; set; } = string.Empty;
        public bool ShouldSerializeExpressionSql() => !string.IsNullOrWhiteSpace(ExpressionSql);

        [XmlElement("IdentityIncrement")]
        public string IdentityIncrement { get; set; } = string.Empty;
        public bool ShouldSerializeIdentityIncrement() => !string.IsNullOrWhiteSpace(IdentityIncrement);

        [XmlElement("IdentitySeed")]
        public string IdentitySeed { get; set; } = string.Empty;
        public bool ShouldSerializeIdentitySeed() => !string.IsNullOrWhiteSpace(IdentitySeed);

        [XmlElement("IsIdentity")]
        public string IsIdentity { get; set; } = string.Empty;
        public bool ShouldSerializeIsIdentity() => !string.IsNullOrWhiteSpace(IsIdentity);

        [XmlElement("IsNullable")]
        public string IsNullable { get; set; } = string.Empty;
        public bool ShouldSerializeIsNullable() => !string.IsNullOrWhiteSpace(IsNullable);

        [XmlElement("MetaDataTypeId")]
        public string MetaDataTypeId { get; set; } = string.Empty;

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlElement("Ordinal")]
        public string Ordinal { get; set; } = string.Empty;
        public bool ShouldSerializeOrdinal() => !string.IsNullOrWhiteSpace(Ordinal);

        [XmlIgnore]
        public Table Table { get; set; } = null!;

    }
}
