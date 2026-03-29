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
        return MetaRawDataVaultModel.LoadFromXmlWorkspace(workspacePath, searchUpward);
    }

    public static Task<MetaRawDataVaultModel> LoadFromWorkspaceAsync(
        string workspacePath,
        bool searchUpward = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return MetaRawDataVaultModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward, cancellationToken);
    }

    public static void SaveToWorkspace(MetaRawDataVaultModel model, string workspacePath)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        model.SaveToXmlWorkspace(workspacePath);
    }

    public static Task SaveToWorkspaceAsync(
        MetaRawDataVaultModel model,
        string workspacePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        return model.SaveToXmlWorkspaceAsync(workspacePath, cancellationToken);
    }
}
