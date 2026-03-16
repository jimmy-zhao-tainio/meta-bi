using Meta.Core.Domain;
using Meta.Core.Services;
using MetaBusinessDataVault;
using MetaRawDataVault;
using SqlModel;

namespace MetaDataVault.ConvertToMetaSql;

public static class MetaDataVaultToMetaSqlConverter
{
    public static async Task<Workspace> ConvertAsync(
        string dataVaultWorkspacePath,
        string pathToNewMetaSqlWorkspace,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dataVaultWorkspacePath))
        {
            throw new ArgumentException("Data Vault workspace path is required.", nameof(dataVaultWorkspacePath));
        }

        if (string.IsNullOrWhiteSpace(pathToNewMetaSqlWorkspace))
        {
            throw new ArgumentException("MetaSql workspace path is required.", nameof(pathToNewMetaSqlWorkspace));
        }

        var workspaceService = new WorkspaceService();
        var dataVaultWorkspace = await workspaceService.LoadAsync(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);

        switch (dataVaultWorkspace.Model.Name)
        {
            case "MetaRawDataVault":
                _ = await MetaRawDataVaultModel.LoadFromXmlWorkspaceAsync(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
                break;

            case "MetaBusinessDataVault":
                _ = await MetaBusinessDataVaultModel.LoadFromXmlWorkspaceAsync(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
                break;

            default:
                throw new InvalidOperationException(
                    $"Workspace '{dataVaultWorkspacePath}' uses model '{dataVaultWorkspace.Model.Name}'. Expected MetaRawDataVault or MetaBusinessDataVault.");
        }

        // Stub boundary only: prove the typed source workspace can be loaded and
        // create the target SqlModel workspace in memory. Population comes later.
        var sqlModel = SqlModelModel.CreateEmpty();
        return sqlModel.ToXmlWorkspace(pathToNewMetaSqlWorkspace);
    }
}
