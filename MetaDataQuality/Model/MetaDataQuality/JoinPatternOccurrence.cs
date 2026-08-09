#nullable enable

namespace MetaDataQuality
{
    public sealed class JoinPatternOccurrence
    {
        public string Id { get; set; } = string.Empty;

        public string? CteId { get; set; }

        public string? CteName { get; set; }

        public string? FirstTableReferenceId { get; set; }

        public string JoinTableReferenceId { get; set; } = string.Empty;

        public string QualifiedJoinId { get; set; } = string.Empty;

        public string QueryExpressionId { get; set; } = string.Empty;

        public string QuerySpecificationId { get; set; } = string.Empty;

        public string? ScopePath { get; set; }

        public string? SearchConditionBooleanExpressionId { get; set; }

        public string? SecondTableReferenceId { get; set; }

        public string TransformScriptId { get; set; } = string.Empty;

        public string TransformScriptName { get; set; } = string.Empty;

        public JoinPattern JoinPattern { get; set; } = null!;

    }
}
