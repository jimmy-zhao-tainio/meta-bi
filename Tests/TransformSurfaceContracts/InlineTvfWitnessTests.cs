using MetaOrchestration.Core;
using MetaPipeline;
using MetaTransform.Binding;

namespace MetaBi.TransformSurfaceContracts.Tests;

public sealed class InlineTvfWitnessTests
{
    [Fact]
    public async Task InlineTvfDefinition_IsBindableQueryHelperButNotExecutablePipelineStep()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(new TransformScriptSeed(
            "dbo.fnCustomerSegments",
            """
CREATE FUNCTION dbo.fnCustomerSegments
(
    @minCustomerId INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        c.CustomerId,
        s.SegmentName
    FROM dbo.Customer AS c
    INNER JOIN dbo.CustomerSegment AS s
        ON c.SegmentId = s.SegmentId
    WHERE c.CustomerId >= @minCustomerId
)
"""));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "dbo.fnCustomerSegments", BoundStatementKind.Select);
        ContractAssertions.AssertDataQualitySawJoin(workspace.DiscoverDataQuality(), "dbo.fnCustomerSegments");

        var script = workspace.ResolveScript("dbo.fnCustomerSegments");
        var ex = Assert.Throws<MetaPipelineConfigurationException>(() => workspace.ResolvePipelineExecution(script));
        Assert.Contains("parameterless transform scripts only", ex.Message, StringComparison.OrdinalIgnoreCase);

        workspace.BuildPipeline(new PipelineSeed("InlineTvfTask", script, null));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.False(orchestration.IsCompleteDag);
        ContractAssertions.AssertOnlyIssue(orchestration, OrchestrationIssueCode.NonExecutableTransformScript);
        Assert.Empty(Assert.Single(Assert.Single(orchestration.Pipelines).Tasks).ObjectAccesses);
    }
}
