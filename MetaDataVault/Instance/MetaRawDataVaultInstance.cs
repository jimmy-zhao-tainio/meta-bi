using MetaRawDataVault;

namespace MetaRawDataVault.Instance;

public static class MetaRawDataVaultInstance
{
    public static MetaRawDataVaultModel CreateEmpty()
    {
        return MetaRawDataVaultModel.CreateEmpty();
    }

    public static MetaRawDataVaultModel LoadFromWorkspace(string workspacePath, bool searchUpward = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.Load<MetaRawDataVaultModel>(workspacePath, searchUpward);
    }

    public static Task<MetaRawDataVaultModel> LoadFromWorkspaceAsync(
        string workspacePath,
        bool searchUpward = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.LoadAsync<MetaRawDataVaultModel>(workspacePath, searchUpward, cancellationToken);
    }

    public static void SaveToWorkspace(MetaRawDataVaultModel model, string workspacePath)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        Meta.Integration.TypedWorkspaceModelMapper.Save(model, workspacePath);
    }

    public static Task SaveToWorkspaceAsync(
        MetaRawDataVaultModel model,
        string workspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return Meta.Integration.TypedWorkspaceModelMapper.SaveAsync(model, workspacePath, cancellationToken);
    }
}
