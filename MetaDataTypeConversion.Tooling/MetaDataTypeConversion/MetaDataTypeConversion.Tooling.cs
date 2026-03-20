using System.Threading;
using System.Threading.Tasks;

namespace MetaDataTypeConversion
{
    public static class MetaDataTypeConversionTooling
    {
        public static MetaDataTypeConversionModel Load(
            string workspacePath,
            bool searchUpward = true)
        {
            return MetaDataTypeConversionModel.LoadFromXmlWorkspace(workspacePath, searchUpward);
        }

        public static Task<MetaDataTypeConversionModel> LoadAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return MetaDataTypeConversionModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward, cancellationToken);
        }

        public static void Save(MetaDataTypeConversionModel model, string workspacePath)
        {
            if (model == null)
            {
                throw new global::System.ArgumentNullException(nameof(model));
            }

            model.SaveToXmlWorkspace(workspacePath);
        }

        public static Task SaveAsync(MetaDataTypeConversionModel model, string workspacePath,
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
