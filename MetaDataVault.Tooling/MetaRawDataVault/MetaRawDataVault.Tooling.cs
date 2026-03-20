using System.Threading;
using System.Threading.Tasks;

namespace MetaRawDataVault
{
    public static class MetaRawDataVaultTooling
    {
        public static MetaRawDataVaultModel Load(
            string workspacePath,
            bool searchUpward = true)
        {
            return MetaRawDataVaultModel.LoadFromXmlWorkspace(workspacePath, searchUpward);
        }

        public static Task<MetaRawDataVaultModel> LoadAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return MetaRawDataVaultModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward, cancellationToken);
        }

        public static void Save(MetaRawDataVaultModel model, string workspacePath)
        {
            if (model == null)
            {
                throw new global::System.ArgumentNullException(nameof(model));
            }

            model.SaveToXmlWorkspace(workspacePath);
        }

        public static Task SaveAsync(MetaRawDataVaultModel model, string workspacePath,
            CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                throw new global::System.ArgumentNullException(nameof(model));
            }

            cancellationToken.ThrowIfCancellationRequested();
            model.SaveToXmlWorkspace(workspacePath);
            return Task.CompletedTask;
        }
    }
}
