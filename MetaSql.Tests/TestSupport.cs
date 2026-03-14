using MetaSql.App;
using MetaSql.Core;
using MetaSql.Workflow;

internal static class TestSupport
{
    public static string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    public static MetaSqlTargetInspection CreateInspection(
        MetaSqlRootMode mode = MetaSqlRootMode.Repo,
        string? rootDirectory = null,
        IReadOnlyList<Blocker>? actionableBlockers = null,
        IReadOnlyList<BlockerScriptFile>? staleScripts = null)
    {
        var root = rootDirectory ?? CreateTempDirectory();
        var migrationRoot = mode == MetaSqlRootMode.Artifact
            ? Path.Combine(root, "meta-sql", "migrate")
            : Path.Combine(root, "deploy", "migrate");
        var context = new MetaSqlTargetContext(
            mode,
            "prod",
            root,
            Path.Combine(root, "meta-sql.json"),
            Path.Combine(root, "sql"),
            null,
            "Server=.;Database=Db;Integrated Security=true",
            migrationRoot,
            Path.Combine(migrationRoot, "baseline"),
            Path.Combine(migrationRoot, "target", "prod"),
            Path.Combine(migrationRoot, "archive"));

        return new MetaSqlTargetInspection(
            context,
            new SqlServerPreflightPlan(0, 0, [], [], [], [], [], [], []),
            "No structural changes.",
            actionableBlockers ?? [],
            actionableBlockers ?? [],
            [],
            staleScripts ?? []);
    }
}
