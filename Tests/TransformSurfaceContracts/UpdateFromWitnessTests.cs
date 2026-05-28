using MetaOrchestration.Core;
using MetaTransform.Binding;

namespace MetaBi.TransformSurfaceContracts.Tests;

public sealed class UpdateFromWitnessTests
{
    [Fact]
    public async Task UpdateFrom_WithQualifiedJoin_IsVisibleToDataQualityPipelineAndOrchestration()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(new TransformScriptSeed(
            "sync-customer-segment",
            """
UPDATE dbo.StageCustomer AS t
SET t.SegmentName = s.SegmentName
FROM dbo.Customer AS c
INNER JOIN dbo.CustomerSegment AS s
    ON c.SegmentId = s.SegmentId
WHERE t.CustomerId = c.CustomerId
"""));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "sync-customer-segment", BoundStatementKind.Update);
        ContractAssertions.AssertDataQualitySawJoin(workspace.DiscoverDataQuality(), "sync-customer-segment");

        var script = workspace.ResolveScript("sync-customer-segment");
        var execution = workspace.ResolvePipelineExecution(script);
        Assert.False(execution.IsSelect);

        workspace.BuildPipeline(new PipelineSeed("SyncCustomerSegment", script, null));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.Customer", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.CustomerSegment", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.StageCustomer", OrchestrationObjectAccessKind.ReadWrite, "Target");
    }
}
