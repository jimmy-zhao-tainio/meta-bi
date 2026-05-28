using MetaTransformScript;

namespace MetaTransformScript.Instance;

public static class MetaTransformScriptInstance
{
    public static MetaTransformScriptModel CreateEmpty()
    {
        return MetaTransformScriptModel.CreateEmpty();
    }

    public static MetaTransformScriptModel LoadFromWorkspace(string workspacePath, bool searchUpward = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return MetaTransformScriptModel.LoadFromXmlWorkspace(workspacePath, searchUpward);
    }

    public static Task<MetaTransformScriptModel> LoadFromWorkspaceAsync(
        string workspacePath,
        bool searchUpward = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return MetaTransformScriptModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward, cancellationToken);
    }

    public static void SaveToWorkspace(MetaTransformScriptModel model, string workspacePath)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        model.SaveToXmlWorkspace(workspacePath);
    }

    public static Task SaveToWorkspaceAsync(
        MetaTransformScriptModel model,
        string workspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return model.SaveToXmlWorkspaceAsync(workspacePath, cancellationToken);
    }
}
