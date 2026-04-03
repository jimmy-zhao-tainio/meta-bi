using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class QuerySpecification
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("UniqueRowFilter")]
        public string UniqueRowFilter { get; set; } = string.Empty;
        public bool ShouldSerializeUniqueRowFilter() => !string.IsNullOrWhiteSpace(UniqueRowFilter);

        [XmlIgnore]
        public QueryExpression Base { get; set; } = null!;

    }
}
