using MetaSql.App;
using MetaSql.Core;
using MetaSql.Workflow;
using MetaSql.Workflow.Resolve;

public sealed class IssueClassifierTests
{
    [Fact]
    public void ClassifiesRenameShapedColumnMismatchIntoOneToOneReplacementFamily()
    {
        var desired = new DesiredSqlModel(
        [
            new DesiredTable(
                "dbo",
                "S_CustomerProfile",
                "CREATE TABLE [dbo].[S_CustomerProfile] (...);",
                [
                    new DesiredColumn("CustomerId", "int NOT NULL", "int", false),
                    new DesiredColumn("CustomerName", "nvarchar(200) NULL", "nvarchar(200)", true)
                ],
                [],
                [],
                [])
        ]);

        var live = new LiveDatabaseSnapshot(
            new Dictionary<string, LiveTable>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.S_CustomerProfile"] = new(
                    "dbo",
                    "S_CustomerProfile",
                    1,
                    new Dictionary<string, LiveColumn>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["CustomerId"] = new("CustomerId", "int", false),
                        ["Customer"] = new("Customer", "nvarchar(200)", true)
                    },
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase))
            });

        var blocker = new Blocker(
            "blk_manual",
            BlockerKind.ManualRequired,
            "dbo.S_CustomerProfile",
            ["Live column [Customer] exists but is not present in desired SQL."]);

        var issue = Assert.Single(new MetaSqlIssueClassifier().Classify(
            desired,
            live,
            new Dictionary<string, SqlObjectTraits>(StringComparer.OrdinalIgnoreCase),
            [blocker],
            []));

        Assert.Equal(MetaSqlIssueKind.ManualScenario, issue.Kind);
        Assert.NotNull(issue.Scenario);
        Assert.Equal(ResolverScenarioFamily.OneToOneReplacementCandidate, issue.Scenario!.Family);
        Assert.Equal(ResolverSubjectKind.Column, issue.Scenario.SubjectKind);
        Assert.Equal(ResolverPresence.Yes, issue.Scenario.Evidence.DataPresent);
        Assert.Equal(ResolverPresence.No, issue.Scenario.Evidence.RelationshipsPresent);
        Assert.Equal(["Customer"], issue.Scenario.LiveOnlyNames);
        Assert.Equal(["CustomerName"], issue.Scenario.DesiredOnlyNames);
        Assert.Contains("In the live DB only: [Customer].", issue.Details);
        Assert.Contains("In desired SQL only: [CustomerName].", issue.Details);
        Assert.Contains("MetaSql cannot decide automatically whether this is a rename, a replacement, or another manual change.", issue.Details);
        Assert.NotNull(issue.Interpretations);
        Assert.Equal("Rename", issue.Interpretations![0].Label);
    }

    [Fact]
    public void ClassifiesInPlaceColumnDifferenceIntoSameIdentityShapeChangeFamily()
    {
        var desired = new DesiredSqlModel(
        [
            new DesiredTable(
                "dbo",
                "S_CustomerProfile",
                "CREATE TABLE [dbo].[S_CustomerProfile] (...);",
                [
                    new DesiredColumn("CustomerId", "int NOT NULL", "int", false),
                    new DesiredColumn("CustomerName", "nvarchar(200) NOT NULL", "nvarchar(200)", false)
                ],
                [],
                [],
                [])
        ]);

        var live = new LiveDatabaseSnapshot(
            new Dictionary<string, LiveTable>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.S_CustomerProfile"] = new(
                    "dbo",
                    "S_CustomerProfile",
                    0,
                    new Dictionary<string, LiveColumn>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["CustomerId"] = new("CustomerId", "int", false),
                        ["CustomerName"] = new("CustomerName", "nvarchar(100)", true)
                    },
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase))
            });

        var blocker = new Blocker(
            "blk_manual",
            BlockerKind.ManualRequired,
            "dbo.S_CustomerProfile",
            ["Column [CustomerName] differs. Desired: nvarchar(200) NOT NULL. Live: nvarchar(100) NULL."]);

        var issue = Assert.Single(new MetaSqlIssueClassifier().Classify(
            desired,
            live,
            new Dictionary<string, SqlObjectTraits>(StringComparer.OrdinalIgnoreCase),
            [blocker],
            []));

        Assert.NotNull(issue.Scenario);
        Assert.Equal(ResolverScenarioFamily.SameIdentityShapeChange, issue.Scenario!.Family);
        Assert.Equal(["CustomerName"], issue.Scenario.SameIdentityChangedNames);
        Assert.Equal(ResolverPresence.No, issue.Scenario.Evidence.DataPresent);
        Assert.Contains("Changed in place: [CustomerName].", issue.Details);
        Assert.Contains("The live table is empty.", issue.Details);
    }

    [Fact]
    public void ClassifiesLiveOnlyTableIntoRemovalFamily()
    {
        var live = new LiveDatabaseSnapshot(
            new Dictionary<string, LiveTable>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.ObsoleteHelper"] = new(
                    "dbo",
                    "ObsoleteHelper",
                    3,
                    new Dictionary<string, LiveColumn>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["Id"] = new("Id", "int", false)
                    },
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "PK_ObsoleteHelper" },
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "IX_ObsoleteHelper_Id" })
            });

        var blocker = new Blocker(
            "blk_manual",
            BlockerKind.ManualRequired,
            "dbo.ObsoleteHelper",
            ["Live table [dbo].[ObsoleteHelper] exists but is not present in desired SQL."]);

        var issue = Assert.Single(new MetaSqlIssueClassifier().Classify(
            new DesiredSqlModel([]),
            live,
            new Dictionary<string, SqlObjectTraits>(StringComparer.OrdinalIgnoreCase),
            [blocker],
            []));

        Assert.NotNull(issue.Scenario);
        Assert.Equal(ResolverScenarioFamily.LiveOnlyRemovalCandidate, issue.Scenario!.Family);
        Assert.Equal(ResolverSubjectKind.Table, issue.Scenario.SubjectKind);
        Assert.Equal(ResolverPresence.Yes, issue.Scenario.Evidence.DataPresent);
        Assert.Equal(ResolverPresence.Yes, issue.Scenario.Evidence.RelationshipsPresent);
        Assert.Contains("Live table [dbo].[ObsoleteHelper] exists only in the live DB.", issue.Details);
    }
}
