namespace MetaTransformScript;

public static partial class MetaTransformScriptInstance
{
    public static MetaTransformScriptModel LoadFromWorkspace(string workspacePath, bool searchUpward = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(workspacePath, searchUpward);
    }

    public static Task<MetaTransformScriptModel> LoadFromWorkspaceAsync(
        string workspacePath,
        bool searchUpward = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.LoadAsync<MetaTransformScriptModel>(
            workspacePath,
            searchUpward,
            cancellationToken);
    }

    public static void SaveToWorkspace(MetaTransformScriptModel model, string workspacePath)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        Meta.Integration.TypedWorkspaceModelMapper.Save(model, workspacePath);
    }

    public static Task SaveToWorkspaceAsync(
        MetaTransformScriptModel model,
        string workspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.SaveAsync(model, workspacePath, cancellationToken);
    }
}
