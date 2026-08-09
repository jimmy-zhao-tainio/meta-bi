#nullable enable

namespace MetaDataQuality
{
    public sealed class JoinPatternKeyPart
    {
        public string Id { get; set; } = string.Empty;

        public string BooleanComparisonExpressionId { get; set; } = string.Empty;

        public string? FirstExpressionDisplay { get; set; }

        public string FirstExpressionId { get; set; } = string.Empty;

        public string? FirstJoinInputColumnName { get; set; }

        public string Ordinal { get; set; } = string.Empty;

        public string? SecondExpressionDisplay { get; set; }

        public string SecondExpressionId { get; set; } = string.Empty;

        public string? SecondJoinInputColumnName { get; set; }

        public JoinPattern JoinPattern { get; set; } = null!;

    }
}
