using System.Threading;
using System.Threading.Tasks;
using Meta.Adapters;
using Meta.Core.Domain;
using Meta.Core.Services;

namespace MetaDataTypeConversion
{
    public static class MetaDataTypeConversionTooling
    {
        public static async Task<MetaDataTypeConversionModel> LoadAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            var workspace = await LoadWorkspaceAsync(workspacePath, searchUpward, cancellationToken).ConfigureAwait(false);
            return MetaDataTypeConversionModelFactory.CreateFromWorkspace(workspace);
        }

        public static Task<Workspace> LoadWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            var services = new ServiceCollection();
            return services.WorkspaceService.LoadAsync(workspacePath, searchUpward, cancellationToken);
        }

        public static Task SaveWorkspaceAsync(
            Workspace workspace,
            CancellationToken cancellationToken = default)
        {
            var services = new ServiceCollection();
            return services.WorkspaceService.SaveAsync(workspace, cancellationToken);
        }

        public static Task<Workspace> ImportSqlAsync(
            string connectionString,
            string schema = "dbo",
            CancellationToken cancellationToken = default)
        {
            var services = new ServiceCollection();
            return services.ImportService.ImportSqlAsync(connectionString, schema, cancellationToken);
        }
    }

    public sealed partial class MetaDataTypeConversionModel
    {
        public static MetaDataTypeConversionModel LoadFromXmlWorkspace(
            string workspacePath,
            bool searchUpward = true)
        {
            return LoadFromXmlWorkspaceAsync(workspacePath, searchUpward).GetAwaiter().GetResult();
        }

        public static Task<MetaDataTypeConversionModel> LoadFromXmlWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            return MetaDataTypeConversionTooling.LoadAsync(workspacePath, searchUpward, cancellationToken);
        }
    }
}
