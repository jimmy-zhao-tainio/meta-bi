using MetaSchema;

namespace MetaSchema.Instance;

public static class MetaSchemaInstance
{
    public static MetaSchemaModel CreateEmpty()
    {
        return MetaSchemaModel.CreateEmpty();
    }

    public static MetaSchemaModel LoadFromWorkspace(string workspacePath, bool searchUpward = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.Load<MetaSchemaModel>(workspacePath, searchUpward);
    }

    public static Task<MetaSchemaModel> LoadFromWorkspaceAsync(
        string workspacePath,
        bool searchUpward = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.LoadAsync<MetaSchemaModel>(workspacePath, searchUpward, cancellationToken);
    }

    public static void SaveToWorkspace(MetaSchemaModel model, string workspacePath)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        Meta.Integration.TypedWorkspaceModelMapper.Save(model, workspacePath);
    }

    public static Task SaveToWorkspaceAsync(
        MetaSchemaModel model,
        string workspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.SaveAsync(model, workspacePath, cancellationToken);
    }
}
