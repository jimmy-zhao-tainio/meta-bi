using System.Threading;
using System.Threading.Tasks;

namespace MetaBusinessDataVault
{
    public static class MetaBusinessDataVaultTooling
    {
        public static MetaBusinessDataVaultModel Load(
            string workspacePath,
            bool searchUpward = true)
        {
            return MetaBusinessDataVaultModel.LoadFromXmlWorkspace(workspacePath, searchUpward);
        }

        public static Task<MetaBusinessDataVaultModel> LoadAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return MetaBusinessDataVaultModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward, cancellationToken);
        }

        public static void Save(MetaBusinessDataVaultModel model, string workspacePath)
        {
            if (model == null)
            {
                throw new global::System.ArgumentNullException(nameof(model));
            }

            model.SaveToXmlWorkspace(workspacePath);
        }

        public static Task SaveAsync(MetaBusinessDataVaultModel model, string workspacePath,
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
