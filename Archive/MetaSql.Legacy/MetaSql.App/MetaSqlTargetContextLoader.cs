using MSD = MetaSqlDeploy;

namespace MetaSql.App;

public sealed class MetaSqlTargetContextLoader
{
    public MetaSqlTargetContext Load(string targetName, string? startDirectory = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetName);

        var rootDirectory = FindConfigRoot(startDirectory ?? Directory.GetCurrentDirectory());
        var workspacePath = MetaSqlDeployWorkspacePaths.GetWorkspaceDirectoryPath(rootDirectory);
        var workspaceFilePath = MetaSqlDeployWorkspacePaths.GetWorkspaceFilePath(rootDirectory);
        var model = MSD.MetaSqlDeployModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
        var configuration = RequireSingleConfiguration(model, workspaceFilePath);
        var mode = ParseRootMode(configuration.RootMode, workspaceFilePath);
        var target = RequireTarget(model, configuration.Id, targetName, workspaceFilePath, mode);
        if (string.IsNullOrWhiteSpace(target.DesiredSql))
        {
            throw new InvalidOperationException($"target '{targetName}' is missing DesiredSql in '{workspaceFilePath}'.");
        }

        var connectionString = ResolveConnectionString(mode, targetName, target);
        var migrationRootPath = ResolvePath(
            rootDirectory,
            string.IsNullOrWhiteSpace(configuration.MigrationRoot)
                ? MetaSqlDeployWorkspacePaths.DefaultMigrationRoot
                : configuration.MigrationRoot);
        return new MetaSqlTargetContext(
            mode,
            targetName,
            rootDirectory,
            workspaceFilePath,
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
            var workspaceFilePath = MetaSqlDeployWorkspacePaths.GetWorkspaceFilePath(current.FullName);
            if (File.Exists(workspaceFilePath))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException(
            $"could not find '{MetaSqlDeployWorkspacePaths.WorkspaceFileRelativePath}' in '{Path.GetFullPath(startDirectory)}' or any parent directory.");
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

    private static MSD.DeployTarget RequireTarget(
        MSD.MetaSqlDeployModel model,
        string configurationId,
        string targetName,
        string workspaceFilePath,
        MetaSqlRootMode mode)
    {
        var matches = model.DeployTargetList
            .Where(item =>
                string.Equals(item.DeployConfigurationId, configurationId, StringComparison.Ordinal) &&
                string.Equals(item.Name, targetName, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        if (matches.Length == 0)
        {
            throw mode == MetaSqlRootMode.Artifact
                ? new InvalidOperationException($"target '{targetName}' is not packaged in this artifact.")
                : new InvalidOperationException($"target '{targetName}' was not found in '{workspaceFilePath}'.");
        }

        if (matches.Length > 1)
        {
            throw new InvalidOperationException($"deploy workspace '{workspaceFilePath}' defines target '{targetName}' more than once.");
        }

        return matches[0];
    }

    private static string ResolveConnectionString(MetaSqlRootMode mode, string targetName, MSD.DeployTarget target)
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
                $"artifact mode requires a connection source for target '{targetName}'. Use ConnectionStringEnvVar, or ConnectionString only for local/dev use.");
        }

        throw new InvalidOperationException($"target '{targetName}' must define ConnectionString or ConnectionStringEnvVar.");
    }

    private static string ResolvePath(string rootDirectory, string path)
    {
        return Path.IsPathRooted(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(rootDirectory, path));
    }

    private static MetaSqlRootMode ParseRootMode(string? value, string workspaceFilePath)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            null or "" or "repo" => MetaSqlRootMode.Repo,
            "artifact" => MetaSqlRootMode.Artifact,
            _ => throw new InvalidOperationException($"unsupported RootMode '{value}' in '{workspaceFilePath}'.")
        };
    }
}
