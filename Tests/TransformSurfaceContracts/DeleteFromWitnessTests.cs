using MetaOrchestration.Core;
using MetaTransform.Binding;

namespace MetaBi.TransformSurfaceContracts.Tests;

public sealed class DeleteFromWitnessTests
{
    [Fact]
    public async Task DeleteFrom_WithQualifiedJoin_IsVisibleToDataQualityPipelineAndOrchestration()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(new TransformScriptSeed(
            "delete-retired-stage-customers",
            """
DELETE t
FROM dbo.StageCustomer AS t
INNER JOIN dbo.RetiredCustomer AS r
    ON t.CustomerId = r.CustomerId
WHERE r.IsRetired = 1
"""));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "delete-retired-stage-customers", BoundStatementKind.Delete);
        ContractAssertions.AssertDataQualitySawJoin(workspace.DiscoverDataQuality(), "delete-retired-stage-customers");

        var script = workspace.ResolveScript("delete-retired-stage-customers");
        var execution = workspace.ResolvePipelineExecution(script);
        Assert.False(execution.IsSelect);

        workspace.BuildPipeline(new PipelineSeed("DeleteRetiredStageCustomers", script, null));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.RetiredCustomer", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.StageCustomer", OrchestrationObjectAccessKind.ReadWrite, "Target");
    }
}
