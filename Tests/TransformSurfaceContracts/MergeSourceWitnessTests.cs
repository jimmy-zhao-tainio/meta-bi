using MetaOrchestration.Core;
using MetaTransform.Binding;

namespace MetaBi.TransformSurfaceContracts.Tests;

public sealed class MergeSourceWitnessTests
{
    [Fact]
    public async Task MergeSource_WithDerivedQualifiedJoin_IsVisibleToDataQualityPipelineAndOrchestration()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(new TransformScriptSeed(
            "merge-customer-segment",
            """
MERGE INTO dbo.CustomerSegmentSnapshot AS t
USING
(
    SELECT
        c.CustomerId,
        s.SegmentName
    FROM dbo.Customer AS c
    INNER JOIN dbo.CustomerSegment AS s
        ON c.SegmentId = s.SegmentId
) AS src
ON t.CustomerId = src.CustomerId
WHEN MATCHED THEN UPDATE SET t.SegmentName = src.SegmentName
WHEN NOT MATCHED BY TARGET THEN INSERT (CustomerId, SegmentName) VALUES (src.CustomerId, src.SegmentName);
"""));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "merge-customer-segment", BoundStatementKind.Merge);
        ContractAssertions.AssertDataQualitySawJoin(workspace.DiscoverDataQuality(), "merge-customer-segment");

        var script = workspace.ResolveScript("merge-customer-segment");
        var execution = workspace.ResolvePipelineExecution(script);
        Assert.False(execution.IsSelect);

        workspace.BuildPipeline(new PipelineSeed("MergeCustomerSegment", script, null));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.Customer", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.CustomerSegment", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.CustomerSegmentSnapshot", OrchestrationObjectAccessKind.ReadWrite, "Target");
    }
}
