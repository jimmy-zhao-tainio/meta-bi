using System.Threading;
using System.Threading.Tasks;

namespace MetaDataVaultImplementation
{
    public static class MetaDataVaultImplementationTooling
    {
        public static MetaDataVaultImplementationModel Load(
            string workspacePath,
            bool searchUpward = true)
        {
            return MetaDataVaultImplementationModel.LoadFromXmlWorkspace(workspacePath, searchUpward);
        }

        public static Task<MetaDataVaultImplementationModel> LoadAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return MetaDataVaultImplementationModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward, cancellationToken);
        }

        public static void Save(MetaDataVaultImplementationModel model, string workspacePath)
        {
            if (model == null)
            {
                throw new global::System.ArgumentNullException(nameof(model));
            }

            model.SaveToXmlWorkspace(workspacePath);
        }

        public static Task SaveAsync(MetaDataVaultImplementationModel model, string workspacePath,
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
