using System.Xml.Linq;
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

    public static void WriteDeployWorkspace(
        string rootDirectory,
        MetaSqlRootMode mode = MetaSqlRootMode.Repo,
        string? migrationRoot = null,
        params DeployWorkspaceTargetSpec[] targets)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootDirectory);
        ArgumentNullException.ThrowIfNull(targets);

        var workspaceDirectory = Path.Combine(rootDirectory, "deploy");
        var metadataDirectory = Path.Combine(workspaceDirectory, "metadata");
        var instanceDirectory = Path.Combine(metadataDirectory, "instance");
        Directory.CreateDirectory(instanceDirectory);

        var templateWorkspaceDirectory = Path.Combine(GetRepositoryRoot(), "MetaSql.Workspaces", "MetaSqlDeploy");
        File.Copy(Path.Combine(templateWorkspaceDirectory, "workspace.xml"), Path.Combine(workspaceDirectory, "workspace.xml"), overwrite: true);
        File.Copy(Path.Combine(templateWorkspaceDirectory, "metadata", "model.xml"), Path.Combine(metadataDirectory, "model.xml"), overwrite: true);

        var configurationId = "deploy-config";
        new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                "MetaSqlDeploy",
                new XElement(
                    "DeployConfigurationList",
                    new XElement(
                        "DeployConfiguration",
                        new XAttribute("Id", configurationId),
                        new XElement("RootMode", mode == MetaSqlRootMode.Artifact ? "artifact" : "repo"),
                        new XElement("MigrationRoot", (migrationRoot ?? "deploy/migrate").Replace('\\', '/'))))))
            .Save(Path.Combine(instanceDirectory, "DeployConfiguration.xml"));

        new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                "MetaSqlDeploy",
                new XElement(
                    "DeployTargetList",
                    targets.Select(target =>
                    {
                        var element = new XElement(
                            "DeployTarget",
                            new XAttribute("Id", target.Id ?? $"deploy-target:{target.Name}"),
                            new XAttribute("DeployConfigurationId", configurationId),
                            new XElement("Name", target.Name),
                            new XElement("DesiredSql", target.DesiredSql.Replace('\\', '/')));
                        if (!string.IsNullOrWhiteSpace(target.TraitsFile))
                        {
                            element.Add(new XElement("TraitsFile", target.TraitsFile.Replace('\\', '/')));
                        }

                        if (!string.IsNullOrWhiteSpace(target.ConnectionString))
                        {
                            element.Add(new XElement("ConnectionString", target.ConnectionString));
                        }

                        if (!string.IsNullOrWhiteSpace(target.ConnectionStringEnvVar))
                        {
                            element.Add(new XElement("ConnectionStringEnvVar", target.ConnectionStringEnvVar));
                        }

                        return element;
                    }))))
            .Save(Path.Combine(instanceDirectory, "DeployTarget.xml"));
    }

    public static MetaSqlTargetInspection CreateInspection(
        MetaSqlRootMode mode = MetaSqlRootMode.Repo,
        string? rootDirectory = null,
        DesiredSqlModel? desiredModel = null,
        LiveDatabaseSnapshot? liveSnapshot = null,
        IReadOnlyDictionary<string, SqlObjectTraits>? traitsByObject = null,
        IReadOnlyList<MetaSqlIssue>? issues = null,
        IReadOnlyList<MetaSqlIssue>? actionableIssues = null,
        IReadOnlyList<Blocker>? actionableBlockers = null,
        IReadOnlyList<BlockerScriptFile>? staleScripts = null)
    {
        var root = rootDirectory ?? CreateTempDirectory();
        var migrationRoot = Path.Combine(root, "deploy", "migrate");
        var context = new MetaSqlTargetContext(
            mode,
            "prod",
            root,
            Path.Combine(root, "deploy", "workspace.xml"),
            Path.Combine(root, "sql"),
            null,
            "Server=.;Database=Db;Integrated Security=true",
            migrationRoot,
            Path.Combine(migrationRoot, "baseline"),
            Path.Combine(migrationRoot, "target", "prod"),
            Path.Combine(migrationRoot, "archive"));

        return new MetaSqlTargetInspection(
            context,
            desiredModel ?? new DesiredSqlModel([]),
            liveSnapshot ?? new LiveDatabaseSnapshot(new Dictionary<string, LiveTable>(StringComparer.OrdinalIgnoreCase)),
            traitsByObject ?? new Dictionary<string, SqlObjectTraits>(StringComparer.OrdinalIgnoreCase),
            new SqlServerPreflightPlan(0, 0, [], [], [], [], [], [], []),
            "No structural changes.",
            issues ?? [],
            actionableIssues ?? issues?.Where(item => item.NeedsAttention).ToArray() ?? [],
            actionableBlockers ?? [],
            actionableBlockers ?? [],
            [],
            staleScripts ?? []);
    }

    private static string GetRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "MetaSql.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("could not locate the repository root from the test runtime directory.");
    }
}

internal sealed record DeployWorkspaceTargetSpec(
    string Name,
    string DesiredSql,
    string? TraitsFile = null,
    string? ConnectionString = null,
    string? ConnectionStringEnvVar = null,
    string? Id = null);
