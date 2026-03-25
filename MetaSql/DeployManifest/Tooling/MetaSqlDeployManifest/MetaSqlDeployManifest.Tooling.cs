using System.Threading;
using System.Threading.Tasks;

namespace MetaSqlDeployManifest
{
    public static class MetaSqlDeployManifestTooling
    {
        public static MetaSqlDeployManifestModel Load(
            string workspacePath,
            bool searchUpward = true)
        {
            return MetaSqlDeployManifestModel.LoadFromXmlWorkspace(workspacePath, searchUpward);
        }

        public static Task<MetaSqlDeployManifestModel> LoadAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward, cancellationToken);
        }

        public static void Save(MetaSqlDeployManifestModel model, string workspacePath)
        {
            if (model == null)
            {
                throw new global::System.ArgumentNullException(nameof(model));
            }

            model.SaveToXmlWorkspace(workspacePath);
        }

        public static Task SaveAsync(MetaSqlDeployManifestModel model, string workspacePath,
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
