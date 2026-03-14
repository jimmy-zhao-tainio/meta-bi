using MetaSql.Core;

public sealed class PlannerAndRenderingTests
{
    [Fact]
    public void PlannerAddsMissingColumnForPersistentAdditiveTable()
    {
        var desired = new DesiredSqlModel(
        [
            new DesiredTable(
                "dbo",
                "H_Customer",
                "CREATE TABLE [dbo].[H_Customer] (...);",
                [
                    new DesiredColumn("HashKey", "binary(16) NOT NULL", "binary(16)", false),
                    new DesiredColumn("CustomerId", "nvarchar(50) NOT NULL", "nvarchar(50)", false)
                ],
                [],
                [],
                [])
        ]);

        var live = new LiveDatabaseSnapshot(
            new Dictionary<string, LiveTable>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.H_Customer"] = new(
                    "dbo",
                    "H_Customer",
                    12,
                    new Dictionary<string, LiveColumn>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["HashKey"] = new("HashKey", "binary(16)", false)
                    },
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase))
            });

        var plan = new SqlServerPreflightPlanner().Plan(desired, live, new Dictionary<string, SqlObjectTraits>());

        var sql = Assert.Single(plan.AddColumns);
        Assert.Contains("ALTER TABLE [dbo].[H_Customer] ADD [CustomerId] nvarchar(50) NOT NULL;", sql);
        Assert.Empty(plan.ManualRequiredItems);
    }

    [Fact]
    public void PlannerRefusesNonAdditiveChangeForPersistentTable()
    {
        var desired = new DesiredSqlModel(
        [
            new DesiredTable(
                "dbo",
                "H_Customer",
                "CREATE TABLE [dbo].[H_Customer] (...);",
                [
                    new DesiredColumn("CustomerId", "nvarchar(50) NOT NULL", "nvarchar(50)", false)
                ],
                [],
                [],
                [])
        ]);

        var live = new LiveDatabaseSnapshot(
            new Dictionary<string, LiveTable>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.H_Customer"] = new(
                    "dbo",
                    "H_Customer",
                    12,
                    new Dictionary<string, LiveColumn>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["CustomerId"] = new("CustomerId", "nvarchar(100)", false)
                    },
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase))
            });

        var plan = new SqlServerPreflightPlanner().Plan(desired, live, new Dictionary<string, SqlObjectTraits>());

        var item = Assert.Single(plan.ManualRequiredItems);
        Assert.Equal("dbo.H_Customer", item.ObjectName);
        Assert.Contains(item.Reasons, reason => reason.Contains("Column [CustomerId] differs.", StringComparison.Ordinal));
        Assert.Empty(plan.DropTables);
        Assert.Empty(plan.AddTables);
    }

    [Fact]
    public void PlannerAllowsEmptyReplaceableTableToDropAndRecreate()
    {
        var desired = new DesiredSqlModel(
        [
            new DesiredTable(
                "dbo",
                "PIT_CustomerSnapshot",
                "CREATE TABLE [dbo].[PIT_CustomerSnapshot] (...);",
                [
                    new DesiredColumn("CustomerId", "nvarchar(50) NOT NULL", "nvarchar(50)", false)
                ],
                [],
                [],
                [])
        ]);

        var live = new LiveDatabaseSnapshot(
            new Dictionary<string, LiveTable>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.PIT_CustomerSnapshot"] = new(
                    "dbo",
                    "PIT_CustomerSnapshot",
                    0,
                    new Dictionary<string, LiveColumn>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["CustomerId"] = new("CustomerId", "nvarchar(100)", false)
                    },
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase))
            });

        var traits = new Dictionary<string, SqlObjectTraits>(StringComparer.OrdinalIgnoreCase)
        {
            ["dbo.PIT_CustomerSnapshot"] = new(SqlObjectStateClass.Replaceable, SqlObjectAutoPolicy.AdditivePlusEmptyDrop)
        };

        var plan = new SqlServerPreflightPlanner().Plan(desired, live, traits);

        Assert.Single(plan.DropTables);
        Assert.Single(plan.AddTables);
        Assert.Empty(plan.ManualRequiredItems);
    }

    [Fact]
    public void ExtraLiveIndexIsCurrentlyTolerated()
    {
        var desired = new DesiredSqlModel(
        [
            new DesiredTable(
                "dbo",
                "H_Customer",
                "CREATE TABLE [dbo].[H_Customer] ([HashKey] binary(16) NOT NULL);",
                [new DesiredColumn("HashKey", "binary(16) NOT NULL", "binary(16)", false)],
                [],
                [],
                [])
        ]);

        var live = new LiveDatabaseSnapshot(
            new Dictionary<string, LiveTable>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.H_Customer"] = new(
                    "dbo",
                    "H_Customer",
                    12,
                    new Dictionary<string, LiveColumn>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["HashKey"] = new("HashKey", "binary(16)", false)
                    },
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "IX_Extra" },
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase))
            });

        var plan = new SqlServerPreflightPlanner().Plan(desired, live, new Dictionary<string, SqlObjectTraits>());

        Assert.Empty(plan.ManualRequiredItems);
        Assert.Empty(plan.AddIndexes);
    }

    [Fact]
    public void ExtraLiveConstraintIsCurrentlyTolerated()
    {
        var desired = new DesiredSqlModel(
        [
            new DesiredTable(
                "dbo",
                "H_Customer",
                "CREATE TABLE [dbo].[H_Customer] ([HashKey] binary(16) NOT NULL);",
                [new DesiredColumn("HashKey", "binary(16) NOT NULL", "binary(16)", false)],
                [],
                [],
                [])
        ]);

        var live = new LiveDatabaseSnapshot(
            new Dictionary<string, LiveTable>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.H_Customer"] = new(
                    "dbo",
                    "H_Customer",
                    12,
                    new Dictionary<string, LiveColumn>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["HashKey"] = new("HashKey", "binary(16)", false)
                    },
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                    new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "UQ_Extra" })
            });

        var plan = new SqlServerPreflightPlanner().Plan(desired, live, new Dictionary<string, SqlObjectTraits>());

        Assert.Empty(plan.ManualRequiredItems);
        Assert.Empty(plan.AddConstraints);
    }

    [Fact]
    public void RendererUsesOperatorReadableSections()
    {
        var plan = new SqlServerPreflightPlan(
            DesiredTableCount: 1,
            LiveTableCount: 0,
            AddTables: ["CREATE TABLE [dbo].[H_Customer] (...);"],
            AddColumns: [],
            AddIndexes: [],
            AddConstraints: [],
            DropTables: [],
            ManualRequiredItems: [new PlanNote("dbo.S_CustomerProfile", ["Refused DROP COLUMN LegacyName because the table contains rows."])],
            BlockedItems: [new PlanNote("dbo.Bridge_CustomerSnapshot", ["Waiting on dbo.S_CustomerProfile"])]);

        var rendered = new SqlServerPreflightPlanRenderer().Render(plan);

        Assert.Contains("Add table:", rendered);
        Assert.Contains("Manual required:", rendered);
        Assert.Contains("Blocked:", rendered);
        Assert.DoesNotContain("status:", rendered, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("apply", rendered, StringComparison.OrdinalIgnoreCase);
    }
}
