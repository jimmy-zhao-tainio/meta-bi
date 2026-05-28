using MetaOrchestration.Core;
using MetaTransform.Binding;

namespace MetaBi.TransformSurfaceContracts.Tests;

public sealed class ScalarFunctionCallBodyWitnessTests
{
    [Fact]
    public async Task ViewCallingSupportedScalarFunction_PropagatesFunctionBodySourcesAcrossStack()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(
            new TransformScriptSeed(
                "dbo.fnSegmentPeerCount",
                """
CREATE FUNCTION dbo.fnSegmentPeerCount
(
    @SegmentId INT
)
RETURNS BIGINT
AS
BEGIN
    RETURN
    (
        SELECT COUNT_BIG(*)
        FROM dbo.Customer AS c
        INNER JOIN dbo.CustomerSegment AS s
            ON c.SegmentId = s.SegmentId
        WHERE s.SegmentId = @SegmentId
    );
END
"""),
            new TransformScriptSeed(
                "dbo.v_CustomerSegmentStats",
                """
CREATE VIEW dbo.v_CustomerSegmentStats AS
SELECT
    c.CustomerId,
    dbo.fnSegmentPeerCount(c.SegmentId) AS SegmentPeerCount
FROM dbo.Customer AS c
""",
                "dbo.CustomerSegmentStats"));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "dbo.fnSegmentPeerCount", BoundStatementKind.ScalarFunction);
        ContractAssertions.AssertStatementKind(workspace, "dbo.v_CustomerSegmentStats", BoundStatementKind.Select);
        ContractAssertions.AssertDataQualitySawJoin(workspace.DiscoverDataQuality(), "dbo.v_CustomerSegmentStats");

        var viewScript = workspace.ResolveScript("dbo.v_CustomerSegmentStats");
        var execution = workspace.ResolvePipelineExecution(viewScript);
        Assert.True(execution.IsSelect);
        Assert.Equal("dbo.CustomerSegmentStats", execution.TargetSqlIdentifier);

        workspace.BuildPipeline(new PipelineSeed("CustomerSegmentStats", viewScript, "dbo.CustomerSegmentStats"));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.Customer", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.CustomerSegment", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.CustomerSegmentStats", OrchestrationObjectAccessKind.Write, "InsertRowsTarget");
    }

    [Fact]
    public async Task ViewFilteringWithSupportedScalarFunction_PropagatesFunctionBodySourcesAcrossStack()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(
            new TransformScriptSeed(
                "dbo.fnSegmentPeerCount",
                """
CREATE FUNCTION dbo.fnSegmentPeerCount
(
    @SegmentId INT
)
RETURNS BIGINT
AS
BEGIN
    RETURN
    (
        SELECT COUNT_BIG(*)
        FROM dbo.Customer AS c
        INNER JOIN dbo.CustomerSegment AS s
            ON c.SegmentId = s.SegmentId
        WHERE s.SegmentId = @SegmentId
    );
END
"""),
            new TransformScriptSeed(
                "dbo.v_CustomerSegmentFilter",
                """
CREATE VIEW dbo.v_CustomerSegmentFilter AS
SELECT
    c.CustomerId
FROM dbo.Customer AS c
WHERE dbo.fnSegmentPeerCount(c.SegmentId) > 1
""",
                "dbo.FilteredCustomerSegment"));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "dbo.v_CustomerSegmentFilter", BoundStatementKind.Select);
        ContractAssertions.AssertDataQualitySawJoin(workspace.DiscoverDataQuality(), "dbo.v_CustomerSegmentFilter");

        var viewScript = workspace.ResolveScript("dbo.v_CustomerSegmentFilter");
        workspace.BuildPipeline(new PipelineSeed("CustomerSegmentFilter", viewScript, "dbo.FilteredCustomerSegment"));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.Customer", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.CustomerSegment", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.FilteredCustomerSegment", OrchestrationObjectAccessKind.Write, "InsertRowsTarget");
    }
}
