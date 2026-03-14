using System.Text.Json;

namespace MetaSql.App;

public sealed class MetaSqlArtifactWriter
{
    private const string ConfigFileName = "meta-sql.json";

    public void Write(string repoRoot, string artifactRoot, IEnumerable<string> targetNames)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(repoRoot);
        ArgumentException.ThrowIfNullOrWhiteSpace(artifactRoot);
        ArgumentNullException.ThrowIfNull(targetNames);

        var sourceRoot = Path.GetFullPath(repoRoot);
        var destinationRoot = Path.GetFullPath(artifactRoot);
        var configPath = Path.Combine(sourceRoot, ConfigFileName);
        if (!File.Exists(configPath))
        {
            throw new InvalidOperationException($"could not find '{ConfigFileName}' in '{sourceRoot}'.");
        }

        var sourceConfig = JsonSerializer.Deserialize<SourceConfig>(
            File.ReadAllText(configPath),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException($"configuration file '{configPath}' is empty.");

        var requestedTargets = targetNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (requestedTargets.Length == 0)
        {
            throw new InvalidOperationException("artifact packaging requires at least one target name.");
        }

        if (sourceConfig.Targets == null)
        {
            throw new InvalidOperationException($"configuration file '{configPath}' does not define any targets.");
        }

        var sourceMigrationRoot = ResolvePath(
            sourceRoot,
            string.IsNullOrWhiteSpace(sourceConfig.MigrationRoot) ? "deploy/migrate" : sourceConfig.MigrationRoot);
        var packagedTargets = new Dictionary<string, PackagedTargetConfig>(StringComparer.OrdinalIgnoreCase);
        foreach (var targetName in requestedTargets)
        {
            if (!sourceConfig.Targets.TryGetValue(targetName, out var target))
            {
                throw new InvalidOperationException($"target '{targetName}' was not found in '{configPath}'.");
            }

            if (string.IsNullOrWhiteSpace(target.DesiredSql))
            {
                throw new InvalidOperationException($"target '{targetName}' is missing desiredSql in '{configPath}'.");
            }

            var desiredSourcePath = ResolvePath(sourceRoot, target.DesiredSql);
            var desiredDestinationPath = Path.Combine(destinationRoot, "meta-sql", "desired-sql", targetName);
            CopyDirectory(desiredSourcePath, desiredDestinationPath);

            string? packagedTraitsPath = null;
            if (!string.IsNullOrWhiteSpace(target.TraitsFile))
            {
                var traitsSourcePath = ResolvePath(sourceRoot, target.TraitsFile);
                var traitsDestinationPath = Path.Combine(destinationRoot, "meta-sql", "traits", $"{targetName}.json");
                Directory.CreateDirectory(Path.GetDirectoryName(traitsDestinationPath)!);
                File.Copy(traitsSourcePath, traitsDestinationPath, overwrite: true);
                packagedTraitsPath = Path.GetRelativePath(destinationRoot, traitsDestinationPath).Replace('\\', '/');
            }

            CopyDirectory(
                Path.Combine(sourceMigrationRoot, "target", targetName),
                Path.Combine(destinationRoot, "meta-sql", "migrate", "target", targetName));

            packagedTargets[targetName] = new PackagedTargetConfig(
                Path.GetRelativePath(destinationRoot, desiredDestinationPath).Replace('\\', '/'),
                packagedTraitsPath,
                target.ConnectionString,
                target.ConnectionStringEnvVar);
        }

        CopyDirectory(
            Path.Combine(sourceMigrationRoot, "baseline"),
            Path.Combine(destinationRoot, "meta-sql", "migrate", "baseline"));

        var packagedConfig = new PackagedConfig(
            RootMode: "artifact",
            MigrationRoot: "meta-sql/migrate",
            Targets: packagedTargets);
        Directory.CreateDirectory(destinationRoot);
        File.WriteAllText(
            Path.Combine(destinationRoot, ConfigFileName),
            JsonSerializer.Serialize(packagedConfig, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
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

    private sealed record SourceConfig(
        string? MigrationRoot,
        Dictionary<string, SourceTargetConfig>? Targets);

    private sealed record SourceTargetConfig(
        string? DesiredSql,
        string? TraitsFile,
        string? ConnectionString,
        string? ConnectionStringEnvVar);

    private sealed record PackagedConfig(
        string RootMode,
        string MigrationRoot,
        Dictionary<string, PackagedTargetConfig> Targets);

    private sealed record PackagedTargetConfig(
        string DesiredSql,
        string? TraitsFile,
        string? ConnectionString,
        string? ConnectionStringEnvVar);
}
