using System.Threading;
using System.Threading.Tasks;

namespace MetaDataType
{
    public static class MetaDataTypeTooling
    {
        public static MetaDataTypeModel Load(
            string workspacePath,
            bool searchUpward = true)
        {
            return MetaDataTypeModel.LoadFromXmlWorkspace(workspacePath, searchUpward);
        }

        public static Task<MetaDataTypeModel> LoadAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return MetaDataTypeModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward, cancellationToken);
        }

        public static void Save(MetaDataTypeModel model, string workspacePath)
        {
            if (model == null)
            {
                throw new global::System.ArgumentNullException(nameof(model));
            }

            model.SaveToXmlWorkspace(workspacePath);
        }

        public static Task SaveAsync(MetaDataTypeModel model, string workspacePath,
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
