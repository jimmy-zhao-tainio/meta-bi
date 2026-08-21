using MetaOrchestration.Core;
using MetaTransformScript;

namespace MetaBi.TransformSurfaceContracts.Tests;

public sealed class SelectViewWitnessTests
{
    [Fact]
    public async Task SelectView_WithQualifiedJoin_IsVisibleToDataQualityPipelineAndOrchestration()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(new TransformScriptSeed(
            "dbo.v_LoadCustomer",
            """
CREATE VIEW dbo.v_LoadCustomer AS
SELECT
    c.CustomerId,
    s.SegmentName
FROM dbo.Customer AS c
INNER JOIN dbo.CustomerSegment AS s
    ON c.SegmentId = s.SegmentId
""",
            "dbo.DimCustomer"));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "dbo.v_LoadCustomer", TransformScriptStatementKind.Select);
        ContractAssertions.AssertDataQualitySawJoin(workspace.DiscoverDataQuality(), "dbo.v_LoadCustomer");

        var script = workspace.ResolveScript("dbo.v_LoadCustomer");
        var execution = workspace.ResolvePipelineExecution(script);
        Assert.True(execution.IsSelect);
        Assert.Equal("dbo.DimCustomer", execution.TargetSqlIdentifier);
        Assert.NotNull(execution.RowStreamShape);

        workspace.BuildPipeline(new PipelineSeed("LoadCustomer", script, "dbo.DimCustomer"));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.Customer", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.CustomerSegment", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.DimCustomer", OrchestrationObjectAccessKind.Write, "InsertRowsTarget");
    }
}
