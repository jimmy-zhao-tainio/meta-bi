using System;
using System.Xml.Serialization;

namespace MetaDataTypeConversion
{
    public sealed class DataTypeMapping
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("ConversionImplementationId")]
        public string ConversionImplementationId { get; set; } = string.Empty;

        [XmlElement("Notes")]
        public string Notes { get; set; } = string.Empty;
        public bool ShouldSerializeNotes() => !string.IsNullOrWhiteSpace(Notes);

        [XmlElement("SourceDataTypeId")]
        public string SourceDataTypeId { get; set; } = string.Empty;

        [XmlElement("TargetDataTypeId")]
        public string TargetDataTypeId { get; set; } = string.Empty;

        [XmlIgnore]
        public ConversionImplementation ConversionImplementation { get; set; } = null!;

    }
}
