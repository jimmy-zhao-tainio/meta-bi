using MetaSql.App;
using MetaSql.Core;
using MetaSql.Workflow;

public sealed class BlockerAndScriptTests
{
    [Fact]
    public void BlockerIdentityIsStableForSamePlanNote()
    {
        var plan = new SqlServerPreflightPlan(
            DesiredTableCount: 0,
            LiveTableCount: 0,
            AddTables: [],
            AddColumns: [],
            AddIndexes: [],
            AddConstraints: [],
            DropTables: [],
            ManualRequiredItems: [new PlanNote("dbo.S_CustomerProfile", ["Non-additive change on persistent structure cannot be inferred safely from live shape."])],
            BlockedItems: []);

        var service = new BlockerIdentityService();

        var first = Assert.Single(service.Build(plan));
        var second = Assert.Single(service.Build(plan));

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public void BlockerScriptCatalogDetectsStaleScriptWhenBlockerDisappears()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            var emitted = Path.Combine(root, "dbo_S_CustomerProfile.blk_12345678.sql");
            File.WriteAllText(
                emitted,
                """
                -- meta-sql blocker-id: blk_12345678
                -- meta-sql blocker-kind: manual-required
                -- meta-sql object: dbo.S_CustomerProfile

                ALTER TABLE dbo.S_CustomerProfile DROP COLUMN LegacyName;
                """);

            var catalog = new BlockerScriptCatalog().Load(root, []);

            Assert.Single(catalog.Headers);
            Assert.Empty(catalog.Matched);
            Assert.Single(catalog.Stale);
            Assert.Equal("blk_12345678", catalog.Stale[0].Header.BlockerId);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void RejectsMalformedHeader()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            var path = Path.Combine(root, "bad.sql");
            File.WriteAllText(
                path,
                """
                -- meta-sql blocker-id: blk_bad
                -- meta-sql object: dbo.S_CustomerProfile

                ALTER TABLE dbo.S_CustomerProfile DROP COLUMN LegacyName;
                """);

            var exception = Assert.Throws<InvalidOperationException>(() => new BlockerScriptLoader().Load(path, requireSqlBody: true));

            Assert.Contains("is invalid", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void RejectsEmptyManualBody()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            var path = Path.Combine(root, "empty.sql");
            File.WriteAllText(
                path,
                """
                -- meta-sql blocker-id: blk_empty
                -- meta-sql blocker-kind: manual-required
                -- meta-sql object: dbo.S_CustomerProfile
                """);

            var exception = Assert.Throws<InvalidOperationException>(() => new BlockerScriptLoader().Load(path, requireSqlBody: true));

            Assert.Contains("has no SQL body", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void RejectsBadActiveScriptHeader()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(root, "bad.sql"),
                """
                -- meta-sql blocker-id: blk_bad
                -- meta-sql blocker-kind: blocked
                -- meta-sql object: dbo.S_CustomerProfile

                ALTER TABLE dbo.S_CustomerProfile DROP COLUMN LegacyName;
                """);

            Blocker[] blockers =
            [
                new Blocker(
                    "blk_bad",
                    BlockerKind.ManualRequired,
                    "dbo.S_CustomerProfile",
                    ["Non-additive change on persistent structure cannot be inferred safely from live shape."])
            ];

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                _ = new BlockerScriptCatalog().Load(root, blockers);
            });

            Assert.Contains("header does not match the current blocker", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ArchiveScriptsAreIgnoredByNormalActiveInput()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            var archivePath = Path.Combine(root, "archive");
            Directory.CreateDirectory(archivePath);
            File.WriteAllText(
                Path.Combine(archivePath, "old.sql"),
                """
                -- meta-sql blocker-id: blk_old
                -- meta-sql blocker-kind: manual-required
                -- meta-sql object: dbo.S_CustomerProfile

                ALTER TABLE dbo.S_CustomerProfile DROP COLUMN LegacyName;
                """);

            var catalog = new BlockerScriptCatalog().Load([], []);

            Assert.Empty(catalog.Headers);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void DuplicateActiveScriptsForSameBlockerAreCurrentlyBothAccepted()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            var baseline = Path.Combine(root, "baseline");
            var target = Path.Combine(root, "target");
            Directory.CreateDirectory(baseline);
            Directory.CreateDirectory(target);
            File.WriteAllText(
                Path.Combine(baseline, "one.sql"),
                """
                -- meta-sql blocker-id: blk_same
                -- meta-sql blocker-kind: manual-required
                -- meta-sql object: dbo.S_CustomerProfile

                ALTER TABLE dbo.S_CustomerProfile DROP COLUMN LegacyName;
                """);
            File.WriteAllText(
                Path.Combine(target, "two.sql"),
                """
                -- meta-sql blocker-id: blk_same
                -- meta-sql blocker-kind: manual-required
                -- meta-sql object: dbo.S_CustomerProfile

                ALTER TABLE dbo.S_CustomerProfile DROP COLUMN LegacyName;
                """);

            Blocker[] blockers =
            [
                new Blocker(
                    "blk_same",
                    BlockerKind.ManualRequired,
                    "dbo.S_CustomerProfile",
                    ["Non-additive change on persistent structure cannot be inferred safely from live shape."])
            ];

            var catalog = new BlockerScriptCatalog().Load([baseline, target], blockers);

            Assert.Equal(2, catalog.Headers.Count);
            Assert.Equal(2, catalog.Matched.Count);
            Assert.Empty(catalog.Stale);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void DeploymentGuardRefusesWhenActiveStaleScriptsRemain()
    {
        var inspection = TestSupport.CreateInspection(
            staleScripts:
            [
                new BlockerScriptFile(
                    @"C:\repo\deploy\migrate\baseline\old.sql",
                    new BlockerScriptHeader("blk_old", BlockerKind.ManualRequired, "dbo.S_CustomerProfile"))
            ]);

        var exception = Assert.Throws<InvalidOperationException>(() => new MetaSqlDeploymentGuard().EnsureCanDeploy(inspection));

        Assert.Contains("active stale scripts", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ResolutionSessionCreatesBaselineAndTargetScriptsInExpectedFolders()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            var inspection = TestSupport.CreateInspection(rootDirectory: root);
            var blocker = new Blocker(
                "blk_11111111",
                BlockerKind.ManualRequired,
                "dbo.S_CustomerProfile",
                ["Non-additive change on persistent structure cannot be inferred safely from live shape."]);
            var session = new MetaSqlResolutionSession();

            var baselinePath = session.CreateBaselineStub(inspection, blocker);
            var targetPath = session.CreateTargetStub(inspection, blocker with { Id = "blk_22222222" });

            Assert.StartsWith(inspection.Context.BaselinePath, baselinePath, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith(inspection.Context.TargetPath, targetPath, StringComparison.OrdinalIgnoreCase);
            Assert.True(File.Exists(baselinePath));
            Assert.True(File.Exists(targetPath));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ResolutionSessionArchivesStaleScriptIntoArchiveFolder()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            var inspection = TestSupport.CreateInspection(rootDirectory: root);
            Directory.CreateDirectory(inspection.Context.TargetPath);
            var stalePath = Path.Combine(inspection.Context.TargetPath, "dbo_S_CustomerProfile.blk_old.sql");
            File.WriteAllText(
                stalePath,
                """
                -- meta-sql blocker-id: blk_old
                -- meta-sql blocker-kind: manual-required
                -- meta-sql object: dbo.S_CustomerProfile
                """);

            var staleScript = new BlockerScriptFile(
                stalePath,
                new BlockerScriptHeader("blk_old", BlockerKind.ManualRequired, "dbo.S_CustomerProfile"));
            var archivedPath = new MetaSqlResolutionSession().ArchiveStaleScript(inspection, staleScript);

            Assert.StartsWith(inspection.Context.ArchivePath, archivedPath, StringComparison.OrdinalIgnoreCase);
            Assert.False(File.Exists(stalePath));
            Assert.True(File.Exists(archivedPath));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ResolutionSessionSummarySeparatesBlockedFromAttention()
    {
        var inspection = TestSupport.CreateInspection(
            actionableBlockers:
            [
                new Blocker(
                    "blk_manual",
                    BlockerKind.ManualRequired,
                    "dbo.S_CustomerProfile",
                    ["Live column [LegacyName] exists but is not present in desired SQL."])
            ]);
        var withBlocked = inspection with
        {
            AllBlockers =
            [
                .. inspection.AllBlockers,
                new Blocker(
                    "blk_blocked",
                    BlockerKind.Blocked,
                    "dbo.S_CustomerProfile",
                    ["Waiting on table-level manual work before ADD CONSTRAINT [PK_S_CustomerProfile]."])
            ]
        };

        var summary = new MetaSqlResolutionSession().BuildSummary(withBlocked);

        Assert.Contains("Found 1 issue requiring attention", summary, StringComparison.Ordinal);
        Assert.Contains("1 additional item is currently blocked.", summary, StringComparison.Ordinal);
    }
}
