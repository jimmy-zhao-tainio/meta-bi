#nullable enable

namespace MetaOrchestration
{
    public sealed class DependencyIssuePipeline
    {
        public string Id { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DependencyIssue DependencyIssue { get; set; } = null!;

        public PipelineReference PipelineReference { get; set; } = null!;

    }
}
