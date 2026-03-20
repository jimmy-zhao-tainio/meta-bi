using System;
using System.Xml.Serialization;

namespace MetaDataType
{
    public sealed class DataType
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("DataTypeSystemId")]
        public string DataTypeSystemId { get; set; } = string.Empty;

        [XmlElement("Category")]
        public string Category { get; set; } = string.Empty;
        public bool ShouldSerializeCategory() => !string.IsNullOrWhiteSpace(Category);

        [XmlElement("Description")]
        public string Description { get; set; } = string.Empty;
        public bool ShouldSerializeDescription() => !string.IsNullOrWhiteSpace(Description);

        [XmlElement("IsCanonical")]
        public string IsCanonical { get; set; } = string.Empty;
        public bool ShouldSerializeIsCanonical() => !string.IsNullOrWhiteSpace(IsCanonical);

        [XmlElement("Name")]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore]
        public DataTypeSystem DataTypeSystem { get; set; } = null!;

    }
}
