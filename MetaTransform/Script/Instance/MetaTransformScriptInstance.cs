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
        return Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(workspacePath, searchUpward);
    }

    public static Task<MetaTransformScriptModel> LoadFromWorkspaceAsync(
        string workspacePath,
        bool searchUpward = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Core.Serialization.TypedWorkspaceXmlSerializer.LoadAsync<MetaTransformScriptModel>(workspacePath, searchUpward, cancellationToken);
    }

    public static void SaveToWorkspace(MetaTransformScriptModel model, string workspacePath)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(model, workspacePath);
    }

    public static Task SaveToWorkspaceAsync(
        MetaTransformScriptModel model,
        string workspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Core.Serialization.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath, cancellationToken);
    }
}
