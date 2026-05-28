using MetaOrchestration.Core;
using MetaTransform.Binding;

namespace MetaBi.TransformSurfaceContracts.Tests;

public sealed class TruncateWitnessTests
{
    [Fact]
    public async Task Truncate_IsExecutableMutationButNotADataQualityCandidateSource()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(new TransformScriptSeed(
            "truncate-stage-customer",
            "TRUNCATE TABLE dbo.StageCustomer"));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "truncate-stage-customer", BoundStatementKind.Truncate);
        ContractAssertions.AssertNoDataQualityCandidates(workspace.DiscoverDataQuality());

        var script = workspace.ResolveScript("truncate-stage-customer");
        var execution = workspace.ResolvePipelineExecution(script);
        Assert.False(execution.IsSelect);

        workspace.BuildPipeline(new PipelineSeed("TruncateStageCustomer", script, null));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.StageCustomer", OrchestrationObjectAccessKind.ResetWrite, "Target");
    }
}
