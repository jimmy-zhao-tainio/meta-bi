namespace MetaSql.App;

internal static class MetaSqlDeployWorkspacePaths
{
    internal const string WorkspaceDirectoryName = "deploy";
    internal const string WorkspaceFileName = "workspace.xml";
    internal const string WorkspaceFileRelativePath = "deploy/workspace.xml";
    internal const string DefaultMigrationRoot = "deploy/migrate";
    internal const string DefaultArtifactDesiredSqlRoot = "deploy/desired-sql";

    internal static string GetWorkspaceDirectoryPath(string rootDirectory)
    {
        return Path.Combine(rootDirectory, WorkspaceDirectoryName);
    }

    internal static string GetWorkspaceFilePath(string rootDirectory)
    {
        return Path.Combine(rootDirectory, WorkspaceDirectoryName, WorkspaceFileName);
    }
}
