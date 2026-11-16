namespace MetaTransformScript;

public static partial class MetaTransformScriptInstance
{
    public static MetaTransformScriptModel LoadFromWorkspace(string workspacePath, bool searchUpward = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.Load(workspacePath, new MetaTransformScriptWorkspaceBinding(), searchUpward);
    }

    public static Task<MetaTransformScriptModel> LoadFromWorkspaceAsync(
        string workspacePath,
        bool searchUpward = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.LoadAsync(
            workspacePath,
            new MetaTransformScriptWorkspaceBinding(),
            searchUpward,
            cancellationToken);
    }

    public static void SaveToWorkspace(MetaTransformScriptModel model, string workspacePath)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        Meta.Integration.TypedWorkspaceModelMapper.Save(model, workspacePath, new MetaTransformScriptWorkspaceBinding());
    }

    public static Task SaveToWorkspaceAsync(
        MetaTransformScriptModel model,
        string workspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.SaveAsync(model, workspacePath, new MetaTransformScriptWorkspaceBinding(), cancellationToken);
    }
}
