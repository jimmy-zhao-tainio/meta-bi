using MetaSql.App;
using MetaSql.Core;
using MetaSql.Workflow;

public sealed class DeployTestOutputRendererTests
{
    [Fact]
    public void UsesRenderedPlanWhenNothingNeedsAttention()
    {
        var inspection = TestSupport.CreateInspection() with
        {
            Plan = new SqlServerPreflightPlan(
                1,
                0,
                ["CREATE TABLE [dbo].[S_CustomerProfile] (...);"],
                [],
                [],
                [],
                [],
                [],
                []),
            RenderedPlan = "Add table:\r\n    CREATE TABLE [dbo].[S_CustomerProfile] (...);"
        };

        var rendered = new MetaSqlDeployTestOutputRenderer().Render(inspection);

        Assert.Equal(inspection.RenderedPlan, rendered);
    }

    [Fact]
    public void HidesBlockedFalloutAndPointsToResolveWhenActionIsNeeded()
    {
        var issue = new MetaSqlIssue(
            MetaSqlIssueKind.ManualScenario,
            "dbo.S_CustomerProfile",
            [
                "In the live DB only: [Customer].",
                "In desired SQL only: [CustomerName].",
                "MetaSql cannot decide automatically whether this is a rename, a replacement, or another manual change."
            ],
            NeedsAttention: true);
        var inspection = TestSupport.CreateInspection(
            issues: [issue],
            actionableIssues: [issue],
            actionableBlockers:
            [
                new Blocker(
                    "blk_manual",
                    BlockerKind.ManualRequired,
                    "dbo.S_CustomerProfile",
                    ["Live column [Customer] exists but is not present in desired SQL."])
            ]) with
        {
            AllBlockers =
            [
                new Blocker(
                    "blk_manual",
                    BlockerKind.ManualRequired,
                    "dbo.S_CustomerProfile",
                    ["Live column [Customer] exists but is not present in desired SQL."]),
                new Blocker(
                    "blk_blocked",
                    BlockerKind.Blocked,
                    "dbo.S_CustomerProfile",
                    ["Waiting on table-level manual work before ADD CONSTRAINT [PK_S_CustomerProfile]."])
            ],
            RenderedPlan = """
                Manual required:
                    dbo.S_CustomerProfile
                        Live column [Customer] exists but is not present in desired SQL.

                Blocked:
                    dbo.S_CustomerProfile
                        Waiting on table-level manual work before ADD CONSTRAINT [PK_S_CustomerProfile].
                """
        };

        var rendered = new MetaSqlDeployTestOutputRenderer().Render(inspection);

        Assert.Contains("Action needed:", rendered);
        Assert.Contains("dbo.S_CustomerProfile", rendered);
        Assert.Contains("In the live DB only: [Customer].", rendered);
        Assert.Contains("In desired SQL only: [CustomerName].", rendered);
        Assert.Contains("Run:", rendered);
        Assert.Contains("meta-sql resolve prod", rendered);
        Assert.DoesNotContain("Blocked:", rendered);
        Assert.DoesNotContain("Waiting on table-level manual work", rendered);
    }

    [Fact]
    public void ShowsStaleScriptAsActionNeeded()
    {
        var staleScript = new BlockerScriptFile(
            @"C:\repo\deploy\migrate\target\prod\old.sql",
            new BlockerScriptHeader("blk_old", BlockerKind.ManualRequired, "dbo.S_CustomerProfile"));
        var issue = new MetaSqlIssue(
            MetaSqlIssueKind.StaleScript,
            "dbo.S_CustomerProfile",
            [$"Script '{staleScript.Path}' no longer matches the current target state."],
            NeedsAttention: true,
            StaleScript: staleScript);
        var inspection = TestSupport.CreateInspection(
            issues: [issue],
            actionableIssues: [issue],
            staleScripts: [staleScript]);

        var rendered = new MetaSqlDeployTestOutputRenderer().Render(inspection);

        Assert.Contains("Action needed:", rendered);
        Assert.Contains("dbo.S_CustomerProfile", rendered);
        Assert.Contains(@"Script 'C:\repo\deploy\migrate\target\prod\old.sql' no longer matches the current target state.", rendered);
        Assert.Contains("meta-sql resolve prod", rendered);
    }
}
