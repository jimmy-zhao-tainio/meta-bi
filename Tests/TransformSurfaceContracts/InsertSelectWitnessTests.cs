using MetaOrchestration.Core;
using MetaTransformScript;

namespace MetaBi.TransformSurfaceContracts.Tests;

public sealed class InsertSelectWitnessTests
{
    [Fact]
    public async Task InsertSelect_WithQualifiedJoin_IsVisibleToDataQualityPipelineAndOrchestration()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(new TransformScriptSeed(
            "load-stage-customer",
            """
INSERT INTO dbo.StageCustomer (CustomerId, SegmentName)
SELECT
    c.CustomerId,
    s.SegmentName
FROM dbo.Customer AS c
INNER JOIN dbo.CustomerSegment AS s
    ON c.SegmentId = s.SegmentId
"""));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "load-stage-customer", TransformScriptStatementKind.Insert);
        ContractAssertions.AssertDataQualitySawJoin(workspace.DiscoverDataQuality(), "load-stage-customer");

        var script = workspace.ResolveScript("load-stage-customer");
        var execution = workspace.ResolvePipelineExecution(script);
        Assert.False(execution.IsSelect);
        Assert.Null(execution.TargetSqlIdentifier);
        Assert.Null(execution.RowStreamShape);

        workspace.BuildPipeline(new PipelineSeed("LoadStageCustomer", script, null));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.Customer", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.CustomerSegment", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.StageCustomer", OrchestrationObjectAccessKind.Write, "Target");
    }
}
