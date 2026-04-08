using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class ExpressionGroupingSpecification
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlAttribute("BaseId")]
        public string BaseId { get; set; } = string.Empty;

        [XmlElement("DistributedAggregation")]
        public string DistributedAggregation { get; set; } = string.Empty;
        public bool ShouldSerializeDistributedAggregation() => !string.IsNullOrWhiteSpace(DistributedAggregation);

        [XmlIgnore]
        public GroupingSpecification Base { get; set; } = null!;

    }
}
