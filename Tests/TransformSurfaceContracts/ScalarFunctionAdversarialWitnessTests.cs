using MetaOrchestration.Core;
using MetaTransform.Binding;

namespace MetaBi.TransformSurfaceContracts.Tests;

public sealed class ScalarFunctionAdversarialWitnessTests
{
    [Fact]
    public async Task AmbiguousUnqualifiedScalarFunctionCall_DoesNotInventHelperBodyEvidence()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(
            new TransformScriptSeed(
                "dbo.fnAmbiguousRisk",
                """
CREATE FUNCTION dbo.fnAmbiguousRisk
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
                "audit.fnAmbiguousRisk",
                """
CREATE FUNCTION audit.fnAmbiguousRisk
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
        INNER JOIN audit.CustomerSegment AS s
            ON c.SegmentId = s.SegmentId
        WHERE s.SegmentId = @SegmentId
    );
END
"""),
            new TransformScriptSeed(
                "dbo.v_AmbiguousRisk",
                """
CREATE VIEW dbo.v_AmbiguousRisk AS
SELECT
    c.CustomerId,
    fnAmbiguousRisk(c.SegmentId) AS RiskScore
FROM dbo.Customer AS c
""",
                "dbo.AmbiguousRisk"));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());

        var dataQuality = workspace.DiscoverDataQuality();
        ContractAssertions.AssertNoDataQualityCandidates(dataQuality);

        var script = workspace.ResolveScript("dbo.v_AmbiguousRisk");
        workspace.BuildPipeline(new PipelineSeed("AmbiguousRisk", script, "dbo.AmbiguousRisk"));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.Customer", OrchestrationObjectAccessKind.Read, "Source");
        Assert.DoesNotContain(
            orchestration.Pipelines.SelectMany(pipeline => pipeline.Tasks).SelectMany(task => task.ObjectAccesses),
            item => string.Equals(item.SqlIdentifier, "dbo.CustomerSegment", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(item.SqlIdentifier, "audit.CustomerSegment", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task RecursiveScalarFunctionBodies_TerminateAndDeduplicateBodyEvidence()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(
            new TransformScriptSeed(
                "dbo.fnCycleA",
                """
CREATE FUNCTION dbo.fnCycleA
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
        WHERE dbo.fnCycleB(@SegmentId) >= 0
    );
END
"""),
            new TransformScriptSeed(
                "dbo.fnCycleB",
                """
CREATE FUNCTION dbo.fnCycleB
(
    @SegmentId INT
)
RETURNS BIGINT
AS
BEGIN
    RETURN dbo.fnCycleA(@SegmentId);
END
"""),
            new TransformScriptSeed(
                "dbo.v_CycleRisk",
                """
CREATE VIEW dbo.v_CycleRisk AS
SELECT
    c.CustomerId,
    dbo.fnCycleA(c.SegmentId) AS RiskScore
FROM dbo.Customer AS c
""",
                "dbo.CycleRisk"));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());

        var dataQuality = workspace.DiscoverDataQuality();
        ContractAssertions.AssertDataQualitySawJoin(dataQuality, "dbo.v_CycleRisk");
        Assert.Single(
            dataQuality.JoinPatternOccurrenceList,
            item => string.Equals(item.TransformScriptName, "dbo.v_CycleRisk", StringComparison.OrdinalIgnoreCase));

        var script = workspace.ResolveScript("dbo.v_CycleRisk");
        workspace.BuildPipeline(new PipelineSeed("CycleRisk", script, "dbo.CycleRisk"));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.CustomerSegment", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.CycleRisk", OrchestrationObjectAccessKind.Write, "InsertRowsTarget");
    }

    [Fact]
    public async Task ScalarFunctionCallBuriedInCaseExpression_StillPropagatesFunctionBodySources()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(
            new TransformScriptSeed(
                "dbo.fnCaseRisk",
                """
CREATE FUNCTION dbo.fnCaseRisk
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
                "dbo.v_CaseRisk",
                """
CREATE VIEW dbo.v_CaseRisk AS
SELECT
    c.CustomerId,
    CASE
        WHEN c.SegmentId IS NULL THEN 0
        ELSE dbo.fnCaseRisk(c.SegmentId)
    END AS RiskScore
FROM dbo.Customer AS c
""",
                "dbo.CaseRisk"));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "dbo.v_CaseRisk", BoundStatementKind.Select);
        ContractAssertions.AssertDataQualitySawJoin(workspace.DiscoverDataQuality(), "dbo.v_CaseRisk");

        var script = workspace.ResolveScript("dbo.v_CaseRisk");
        workspace.BuildPipeline(new PipelineSeed("CaseRisk", script, "dbo.CaseRisk"));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.CustomerSegment", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.CaseRisk", OrchestrationObjectAccessKind.Write, "InsertRowsTarget");
    }

    [Fact]
    public async Task ScalarFunctionCallBuriedInCoalesceExpression_StillPropagatesFunctionBodySources()
    {
        using var workspace = new ContractWorkspace();
        await workspace.ImportTransformScriptsAsync(
            new TransformScriptSeed(
                "dbo.fnCoalesceRisk",
                """
CREATE FUNCTION dbo.fnCoalesceRisk
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
                "dbo.v_CoalesceRisk",
                """
CREATE VIEW dbo.v_CoalesceRisk AS
SELECT
    c.CustomerId,
    COALESCE(dbo.fnCoalesceRisk(c.SegmentId), 0) AS RiskScore
FROM dbo.Customer AS c
""",
                "dbo.CoalesceRisk"));

        ContractAssertions.AssertBoundWithoutErrors(workspace.Bind());
        ContractAssertions.AssertStatementKind(workspace, "dbo.v_CoalesceRisk", BoundStatementKind.Select);
        ContractAssertions.AssertDataQualitySawJoin(workspace.DiscoverDataQuality(), "dbo.v_CoalesceRisk");

        var script = workspace.ResolveScript("dbo.v_CoalesceRisk");
        workspace.BuildPipeline(new PipelineSeed("CoalesceRisk", script, "dbo.CoalesceRisk"));
        var orchestration = workspace.AnalyzeOrchestration();

        Assert.True(orchestration.IsCompleteDag);
        Assert.Empty(orchestration.Issues);
        ContractAssertions.AssertAccess(orchestration, "dbo.CustomerSegment", OrchestrationObjectAccessKind.Read, "Source");
        ContractAssertions.AssertAccess(orchestration, "dbo.CoalesceRisk", OrchestrationObjectAccessKind.Write, "InsertRowsTarget");
    }
}
