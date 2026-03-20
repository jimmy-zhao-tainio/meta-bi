using System.Threading;
using System.Threading.Tasks;

namespace MetaSql
{
    public static class MetaSqlTooling
    {
        public static MetaSqlModel Load(
            string workspacePath,
            bool searchUpward = true)
        {
            return MetaSqlModel.LoadFromXmlWorkspace(workspacePath, searchUpward);
        }

        public static Task<MetaSqlModel> LoadAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return MetaSqlModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward, cancellationToken);
        }

        public static void Save(MetaSqlModel model, string workspacePath)
        {
            if (model == null)
            {
                throw new global::System.ArgumentNullException(nameof(model));
            }

            model.SaveToXmlWorkspace(workspacePath);
        }

        public static Task SaveAsync(MetaSqlModel model, string workspacePath,
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
