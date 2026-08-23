using MetaOrchestration.Core;
using MetaPipeline;
using MetaTransformScript;

namespace MetaBi.TransformSurfaceContracts.Tests;

public sealed class ScalarFunctionWitnessTests
{
    [Fact]
    public async Task ScalarFunctionDefinition_IsBindableHelperButNotExecutablePipelineStep()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(new TransformScriptSeed(
            "dbo.fnAddOne",
            """
CREATE FUNCTION dbo.fnAddOne
(
    @value INT
)
RETURNS INT
AS
BEGIN
    RETURN @value + 1;
END
"""));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "dbo.fnAddOne", TransformScriptStatementKind.ScalarFunction);
        ContractAssertions.AssertNoDataQualityCandidates(workspace.DiscoverDataQuality());

        var script = workspace.ResolveScript("dbo.fnAddOne");
        var ex = Assert.Throws<MetaPipelineConfigurationException>(() => workspace.ResolvePipelineExecution(script));
        Assert.Contains("scalar function definition", ex.Message, StringComparison.OrdinalIgnoreCase);

        workspace.BuildPipeline(new PipelineSeed("ScalarFunctionTask", script, null));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.False(orchestration.IsCompleteDag);
        ContractAssertions.AssertOnlyIssue(orchestration, OrchestrationIssueCode.NonExecutableTransformScript);
        Assert.Empty(Assert.Single(Assert.Single(orchestration.Pipelines).Tasks).ObjectAccesses);
    }
}
