using System.Xml.Linq;
using MSD = MetaSqlDeploy;

namespace MetaSql.App;

public sealed class MetaSqlArtifactWriter
{
    public void Write(string repoRoot, string artifactRoot, IEnumerable<string> targetNames)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(repoRoot);
        ArgumentException.ThrowIfNullOrWhiteSpace(artifactRoot);
        ArgumentNullException.ThrowIfNull(targetNames);

        var sourceRoot = Path.GetFullPath(repoRoot);
        var destinationRoot = Path.GetFullPath(artifactRoot);
        var sourceWorkspaceDirectory = MetaSqlDeployWorkspacePaths.GetWorkspaceDirectoryPath(sourceRoot);
        var sourceWorkspaceFile = MetaSqlDeployWorkspacePaths.GetWorkspaceFilePath(sourceRoot);
        if (!File.Exists(sourceWorkspaceFile))
        {
            throw new InvalidOperationException($"could not find '{MetaSqlDeployWorkspacePaths.WorkspaceFileRelativePath}' in '{sourceRoot}'.");
        }

        var model = MSD.MetaSqlDeployModel.LoadFromXmlWorkspace(sourceWorkspaceDirectory, searchUpward: false);
        var configuration = RequireSingleConfiguration(model, sourceWorkspaceFile);
        var sourceTargets = model.DeployTargetList
            .Where(item => string.Equals(item.DeployConfigurationId, configuration.Id, StringComparison.Ordinal))
            .ToArray();

        var requestedTargetNames = targetNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (requestedTargetNames.Length == 0)
        {
            throw new InvalidOperationException("artifact packaging requires at least one target name.");
        }

        var sourceMigrationRoot = ResolvePath(
            sourceRoot,
            string.IsNullOrWhiteSpace(configuration.MigrationRoot)
                ? MetaSqlDeployWorkspacePaths.DefaultMigrationRoot
                : configuration.MigrationRoot);
        var selectedTargets = new List<PackagedTarget>();
        foreach (var targetName in requestedTargetNames)
        {
            var matches = sourceTargets
                .Where(item => string.Equals(item.Name, targetName, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (matches.Length == 0)
            {
                throw new InvalidOperationException($"target '{targetName}' was not found in '{sourceWorkspaceFile}'.");
            }

            if (matches.Length > 1)
            {
                throw new InvalidOperationException($"deploy workspace '{sourceWorkspaceFile}' defines target '{targetName}' more than once.");
            }

            var sourceTarget = matches[0];
            if (string.IsNullOrWhiteSpace(sourceTarget.DesiredSql))
            {
                throw new InvalidOperationException($"target '{targetName}' is missing DesiredSql in '{sourceWorkspaceFile}'.");
            }

            var desiredSourcePath = ResolvePath(sourceRoot, sourceTarget.DesiredSql);
            var desiredDestinationPath = Path.Combine(destinationRoot, "deploy", "desired-sql", targetName);
            CopyDirectory(desiredSourcePath, desiredDestinationPath);

            string? packagedTraitsPath = null;
            if (!string.IsNullOrWhiteSpace(sourceTarget.TraitsFile))
            {
                var traitsSourcePath = ResolvePath(sourceRoot, sourceTarget.TraitsFile);
                var traitsDestinationPath = Path.Combine(destinationRoot, "deploy", "traits", $"{targetName}.json");
                Directory.CreateDirectory(Path.GetDirectoryName(traitsDestinationPath)!);
                File.Copy(traitsSourcePath, traitsDestinationPath, overwrite: true);
                packagedTraitsPath = Path.GetRelativePath(destinationRoot, traitsDestinationPath).Replace('\\', '/');
            }

            CopyDirectory(
                Path.Combine(sourceMigrationRoot, "target", targetName),
                Path.Combine(destinationRoot, "deploy", "migrate", "target", targetName));

            selectedTargets.Add(new PackagedTarget(
                sourceTarget.Id,
                sourceTarget.Name,
                Path.GetRelativePath(destinationRoot, desiredDestinationPath).Replace('\\', '/'),
                packagedTraitsPath,
                sourceTarget.ConnectionString,
                sourceTarget.ConnectionStringEnvVar));
        }

        CopyDirectory(
            Path.Combine(sourceMigrationRoot, "baseline"),
            Path.Combine(destinationRoot, "deploy", "migrate", "baseline"));
        CopyWorkspaceShell(sourceWorkspaceDirectory, destinationRoot);
        WritePackagedInstanceFiles(destinationRoot, configuration, selectedTargets);
    }

    private static MSD.DeployConfiguration RequireSingleConfiguration(MSD.MetaSqlDeployModel model, string workspaceFilePath)
    {
        if (model.DeployConfigurationList.Count == 0)
        {
            throw new InvalidOperationException($"deploy workspace '{workspaceFilePath}' does not define a DeployConfiguration row.");
        }

        if (model.DeployConfigurationList.Count > 1)
        {
            throw new InvalidOperationException($"deploy workspace '{workspaceFilePath}' defines multiple DeployConfiguration rows.");
        }

        return model.DeployConfigurationList[0];
    }

    private static void CopyWorkspaceShell(string sourceWorkspaceDirectory, string destinationRoot)
    {
        var destinationWorkspaceDirectory = MetaSqlDeployWorkspacePaths.GetWorkspaceDirectoryPath(destinationRoot);
        Directory.CreateDirectory(destinationWorkspaceDirectory);
        Directory.CreateDirectory(Path.Combine(destinationWorkspaceDirectory, "metadata"));
        File.Copy(
            Path.Combine(sourceWorkspaceDirectory, MetaSqlDeployWorkspacePaths.WorkspaceFileName),
            Path.Combine(destinationWorkspaceDirectory, MetaSqlDeployWorkspacePaths.WorkspaceFileName),
            overwrite: true);
        File.Copy(
            Path.Combine(sourceWorkspaceDirectory, "metadata", "model.xml"),
            Path.Combine(destinationWorkspaceDirectory, "metadata", "model.xml"),
            overwrite: true);
    }

    private static void WritePackagedInstanceFiles(
        string destinationRoot,
        MSD.DeployConfiguration sourceConfiguration,
        IReadOnlyList<PackagedTarget> targets)
    {
        var instanceDirectory = Path.Combine(
            MetaSqlDeployWorkspacePaths.GetWorkspaceDirectoryPath(destinationRoot),
            "metadata",
            "instance");
        Directory.CreateDirectory(instanceDirectory);
        WriteConfigurationFile(instanceDirectory, sourceConfiguration);
        WriteTargetFile(instanceDirectory, sourceConfiguration.Id, targets);
    }

    private static void WriteConfigurationFile(string instanceDirectory, MSD.DeployConfiguration sourceConfiguration)
    {
        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                "MetaSqlDeploy",
                new XElement(
                    "DeployConfigurationList",
                    new XElement(
                        "DeployConfiguration",
                        new XAttribute("Id", sourceConfiguration.Id),
                        new XElement("RootMode", "artifact"),
                        new XElement("MigrationRoot", MetaSqlDeployWorkspacePaths.DefaultMigrationRoot.Replace('\\', '/'))))));
        document.Save(Path.Combine(instanceDirectory, "DeployConfiguration.xml"));
    }

    private static void WriteTargetFile(string instanceDirectory, string configurationId, IReadOnlyList<PackagedTarget> targets)
    {
        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                "MetaSqlDeploy",
                new XElement(
                    "DeployTargetList",
                    targets
                        .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(item => item.Name, StringComparer.Ordinal)
                        .Select(item =>
                        {
                            var element = new XElement(
                                "DeployTarget",
                                new XAttribute("Id", item.Id),
                                new XAttribute("DeployConfigurationId", configurationId),
                                new XElement("Name", item.Name),
                                new XElement("DesiredSql", item.DesiredSql));
                            if (!string.IsNullOrWhiteSpace(item.TraitsFile))
                            {
                                element.Add(new XElement("TraitsFile", item.TraitsFile));
                            }

                            if (!string.IsNullOrWhiteSpace(item.ConnectionString))
                            {
                                element.Add(new XElement("ConnectionString", item.ConnectionString));
                            }

                            if (!string.IsNullOrWhiteSpace(item.ConnectionStringEnvVar))
                            {
                                element.Add(new XElement("ConnectionStringEnvVar", item.ConnectionStringEnvVar));
                            }

                            return element;
                        }))));
        document.Save(Path.Combine(instanceDirectory, "DeployTarget.xml"));
    }

    private static string ResolvePath(string rootDirectory, string path)
    {
        return Path.IsPathRooted(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(rootDirectory, path));
    }

    private static void CopyDirectory(string sourceDirectory, string destinationDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
        {
            return;
        }

        Directory.CreateDirectory(destinationDirectory);
        foreach (var sourceFile in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourceDirectory, sourceFile);
            var destinationFile = Path.Combine(destinationDirectory, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);
            File.Copy(sourceFile, destinationFile, overwrite: true);
        }
    }

    private sealed record PackagedTarget(
        string Id,
        string Name,
        string DesiredSql,
        string? TraitsFile,
        string ConnectionString,
        string ConnectionStringEnvVar);
}
