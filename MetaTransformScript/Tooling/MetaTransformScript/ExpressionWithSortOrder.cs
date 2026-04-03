using System;
using System.Xml.Serialization;

namespace MetaTransformScript
{
    public sealed class ExpressionWithSortOrder
    {
        [XmlAttribute("Id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("SortOrder")]
        public string SortOrder { get; set; } = string.Empty;
        public bool ShouldSerializeSortOrder() => !string.IsNullOrWhiteSpace(SortOrder);

    }
}
