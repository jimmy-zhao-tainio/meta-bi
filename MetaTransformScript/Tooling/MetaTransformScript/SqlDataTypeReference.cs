using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class SqlDataTypeReference
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("SqlDataTypeOption")]
        public string SqlDataTypeOption { get; set; } = string.Empty;
        public bool ShouldSerializeSqlDataTypeOption() => !string.IsNullOrWhiteSpace(SqlDataTypeOption);

        [XmlIgnore]
        public ParameterizedDataTypeReference Base { get; set; } = null!;

    }
}
