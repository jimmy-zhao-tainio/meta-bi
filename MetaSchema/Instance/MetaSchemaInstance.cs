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
        return Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaSchemaModel>(workspacePath, searchUpward);
    }

    public static Task<MetaSchemaModel> LoadFromWorkspaceAsync(
        string workspacePath,
        bool searchUpward = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Core.Serialization.TypedWorkspaceXmlSerializer.LoadAsync<MetaSchemaModel>(workspacePath, searchUpward, cancellationToken);
    }

    public static void SaveToWorkspace(MetaSchemaModel model, string workspacePath)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(model, workspacePath);
    }

    public static Task SaveToWorkspaceAsync(
        MetaSchemaModel model,
        string workspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Core.Serialization.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath, cancellationToken);
    }
}
