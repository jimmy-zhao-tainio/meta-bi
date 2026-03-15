using MetaSql.App;
using MetaSql.Workflow;
using MetaSql.Workflow.Resolve;

public sealed class ResolveConsoleInteractionTests
{
    [Fact]
    public void BlockerPromptExplainsPatternBeforeAskingWhereSqlShouldGo()
    {
        var blocker = new Blocker(
            "blk_manual",
            BlockerKind.ManualRequired,
            "dbo.S_CustomerProfile",
            ["Live column [Customer] exists but is not present in desired SQL."]);
        var scenario = new ResolverScenario
        {
            SubjectKind = ResolverSubjectKind.Column,
            StateClass = ResolverStateClass.Persistent,
            Family = ResolverScenarioFamily.OneToOneReplacementCandidate,
            ObjectName = blocker.ObjectName,
            LiveOnlyCount = 1,
            DesiredOnlyCount = 1,
            SameIdentityChangedCount = 0,
            LiveOnlyNames = ["Customer"],
            DesiredOnlyNames = ["CustomerName"],
            SameIdentityChangedNames = [],
            Evidence = new ResolverScenarioEvidence
            {
                DataPresent = ResolverPresence.Yes,
                RelationshipsPresent = ResolverPresence.No,
                RelationshipKinds = ResolverRelationshipKinds.None,
                TypeCompatible = true,
                NullabilityCompatible = true,
                NameSimilarityPercent = 70,
                CrossObjectMovementSuspected = false,
                HistoricalPattern = ResolverHistoricalPattern.Unknown,
                RelationshipCount = 0
            },
            Reasons =
            [
                "In the live DB only: [Customer].",
                "In desired SQL only: [CustomerName].",
                "The live table has rows.",
                "MetaSql cannot decide automatically whether this is a rename, a replacement, or another manual change."
            ]
        };
        scenario.Validate();
        var issue = new MetaSqlIssue(
            MetaSqlIssueKind.ManualScenario,
            blocker.ObjectName,
            scenario.Reasons,
            NeedsAttention: true,
            Blocker: blocker,
            Scenario: scenario,
            Interpretations:
            [
                new ResolverInterpretation("Rename", 90, ["Types are compatible.", "Nullability is compatible.", "Name similarity is 70%."]),
                new ResolverInterpretation("Replace in place", 60, ["An old member disappeared and a new member appeared on the same object."])
            ]);
        var inspection = TestSupport.CreateInspection(
            issues: [issue],
            actionableIssues: [issue],
            actionableBlockers: [blocker]);

        var prompt = ResolveConsoleInteraction.BuildIssuePrompt(inspection, issue);

        Assert.Contains("Issue:", prompt);
        Assert.Contains("dbo.S_CustomerProfile", prompt);
        Assert.Contains("In the live DB only: [Customer].", prompt);
        Assert.Contains("In desired SQL only: [CustomerName].", prompt);
        Assert.Contains("Detected pattern:", prompt);
        Assert.Contains("One live-only member and one desired-only member were found on the same object.", prompt);
        Assert.Contains("Possible interpretations:", prompt);
        Assert.Contains("Rename", prompt);
        Assert.Contains("Replace in place", prompt);
        Assert.Contains("Where should the SQL for this change go?", prompt);
        Assert.Contains("Baseline", prompt);
        Assert.Contains("Target-specific for prod", prompt);
        Assert.Contains("Leave it unresolved", prompt);
        Assert.Contains("deploy-test and deploy will continue to stop here.", prompt);
    }

    [Fact]
    public void StaleScriptPromptExplainsArchiveAndLeaveChoices()
    {
        var staleScript = new BlockerScriptFile(
            @"C:\repo\deploy\migrate\target\prod\old.sql",
            new BlockerScriptHeader("blk_old", BlockerKind.ManualRequired, "dbo.S_CustomerProfile"));
        var inspection = TestSupport.CreateInspection(
            issues:
            [
                new MetaSqlIssue(
                    MetaSqlIssueKind.StaleScript,
                    "dbo.S_CustomerProfile",
                    [$"Script '{staleScript.Path}' no longer matches the current target state."],
                    NeedsAttention: true,
                    StaleScript: staleScript)
            ],
            actionableIssues:
            [
                new MetaSqlIssue(
                    MetaSqlIssueKind.StaleScript,
                    "dbo.S_CustomerProfile",
                    [$"Script '{staleScript.Path}' no longer matches the current target state."],
                    NeedsAttention: true,
                    StaleScript: staleScript)
            ],
            staleScripts: [staleScript]);

        var prompt = ResolveConsoleInteraction.BuildStaleScriptPrompt(inspection, staleScript);

        Assert.Contains("Stale script:", prompt);
        Assert.Contains(@"C:\repo\deploy\migrate\target\prod\old.sql", prompt);
        Assert.Contains("This script no longer matches the current target state.", prompt);
        Assert.Contains("Move it to archive", prompt);
        Assert.Contains("Leave it where it is for now", prompt);
        Assert.Contains("deploy-test and deploy will continue to treat it as stale.", prompt);
    }
}
