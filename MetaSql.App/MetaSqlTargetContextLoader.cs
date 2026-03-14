using System.Text.Json;

namespace MetaSql.App;

public sealed class MetaSqlTargetContextLoader
{
    private const string ConfigFileName = "meta-sql.json";

    public MetaSqlTargetContext Load(string targetName, string? startDirectory = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetName);

        var rootDirectory = FindConfigRoot(startDirectory ?? Directory.GetCurrentDirectory());
        var configPath = Path.Combine(rootDirectory, ConfigFileName);
        var config = JsonSerializer.Deserialize<MetaSqlConfig>(
            File.ReadAllText(configPath),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException($"configuration file '{configPath}' is empty.");

        var mode = ParseRootMode(config.RootMode);
        if (config.Targets == null || !config.Targets.TryGetValue(targetName, out var target))
        {
            throw mode == MetaSqlRootMode.Artifact
                ? new InvalidOperationException($"target '{targetName}' is not packaged in this artifact.")
                : new InvalidOperationException($"target '{targetName}' was not found in '{configPath}'.");
        }

        if (string.IsNullOrWhiteSpace(target.DesiredSql))
        {
            throw new InvalidOperationException($"target '{targetName}' is missing desiredSql in '{configPath}'.");
        }

        var connectionString = ResolveConnectionString(mode, targetName, target);
        var migrationRootPath = ResolvePath(
            rootDirectory,
            string.IsNullOrWhiteSpace(config.MigrationRoot)
                ? GetDefaultMigrationRoot(mode)
                : config.MigrationRoot);
        return new MetaSqlTargetContext(
            mode,
            targetName,
            rootDirectory,
            configPath,
            ResolvePath(rootDirectory, target.DesiredSql),
            string.IsNullOrWhiteSpace(target.TraitsFile) ? null : ResolvePath(rootDirectory, target.TraitsFile),
            connectionString,
            migrationRootPath,
            Path.Combine(migrationRootPath, "baseline"),
            Path.Combine(migrationRootPath, "target", targetName),
            Path.Combine(migrationRootPath, "archive"));
    }

    private static string FindConfigRoot(string startDirectory)
    {
        var current = new DirectoryInfo(Path.GetFullPath(startDirectory));
        while (current != null)
        {
            var configPath = Path.Combine(current.FullName, ConfigFileName);
            if (File.Exists(configPath))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException($"could not find '{ConfigFileName}' in '{Path.GetFullPath(startDirectory)}' or any parent directory.");
    }

    private static string ResolveConnectionString(MetaSqlRootMode mode, string targetName, MetaSqlTargetConfig target)
    {
        if (!string.IsNullOrWhiteSpace(target.ConnectionStringEnvVar))
        {
            var value = Environment.GetEnvironmentVariable(target.ConnectionStringEnvVar);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw mode == MetaSqlRootMode.Artifact
                    ? new InvalidOperationException($"artifact mode requires connection from env var '{target.ConnectionStringEnvVar}' for target '{targetName}'.")
                    : new InvalidOperationException(
                        $"target '{targetName}' expects environment variable '{target.ConnectionStringEnvVar}' to contain the connection string.");
            }

            return value;
        }

        if (!string.IsNullOrWhiteSpace(target.ConnectionString))
        {
            return target.ConnectionString;
        }

        if (mode == MetaSqlRootMode.Artifact)
        {
            throw new InvalidOperationException(
                $"artifact mode requires a connection source for target '{targetName}'. Use connectionStringEnvVar, or connectionString only for local/dev use.");
        }

        throw new InvalidOperationException($"target '{targetName}' must define connectionString or connectionStringEnvVar.");
    }

    private static string ResolvePath(string rootDirectory, string path)
    {
        return Path.IsPathRooted(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(rootDirectory, path));
    }

    private static MetaSqlRootMode ParseRootMode(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            null or "" or "repo" => MetaSqlRootMode.Repo,
            "artifact" => MetaSqlRootMode.Artifact,
            _ => throw new InvalidOperationException($"unsupported rootMode '{value}' in '{ConfigFileName}'.")
        };
    }

    private static string GetDefaultMigrationRoot(MetaSqlRootMode mode)
    {
        return mode switch
        {
            MetaSqlRootMode.Repo => "deploy/migrate",
            MetaSqlRootMode.Artifact => "meta-sql/migrate",
            _ => throw new InvalidOperationException($"unsupported root mode '{mode}'.")
        };
    }

    private sealed record MetaSqlConfig(
        string? RootMode,
        string? MigrationRoot,
        Dictionary<string, MetaSqlTargetConfig>? Targets);

    private sealed record MetaSqlTargetConfig(
        string? DesiredSql,
        string? TraitsFile,
        string? ConnectionString,
        string? ConnectionStringEnvVar);
}
