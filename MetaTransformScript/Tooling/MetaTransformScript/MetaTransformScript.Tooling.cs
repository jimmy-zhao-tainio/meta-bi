using System.Threading;
using System.Threading.Tasks;

namespace MetaTransformScript
{
    public static class MetaTransformScriptTooling
    {
        public static MetaTransformScriptModel Load(
            string workspacePath,
            bool searchUpward = true)
        {
            return MetaTransformScriptModel.LoadFromXmlWorkspace(workspacePath, searchUpward);
        }

        public static Task<MetaTransformScriptModel> LoadAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return MetaTransformScriptModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward, cancellationToken);
        }

        public static void Save(MetaTransformScriptModel model, string workspacePath)
        {
            if (model == null)
            {
                throw new global::System.ArgumentNullException(nameof(model));
            }

            model.SaveToXmlWorkspace(workspacePath);
        }

        public static Task SaveAsync(MetaTransformScriptModel model, string workspacePath,
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
