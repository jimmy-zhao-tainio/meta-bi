using System.Threading;
using System.Threading.Tasks;
using Meta.Adapters;
using Meta.Core.Domain;
using Meta.Core.Services;

namespace MetaDataType
{
    public static class MetaDataTypeTooling
    {
        public static async Task<MetaDataTypeModel> LoadAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            var workspace = await LoadWorkspaceAsync(workspacePath, searchUpward, cancellationToken).ConfigureAwait(false);
            return MetaDataTypeModelFactory.CreateFromWorkspace(workspace);
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

    public sealed partial class MetaDataTypeModel
    {
        public static MetaDataTypeModel LoadFromXmlWorkspace(
            string workspacePath,
            bool searchUpward = true)
        {
            return LoadFromXmlWorkspaceAsync(workspacePath, searchUpward).GetAwaiter().GetResult();
        }

        public static Task<MetaDataTypeModel> LoadFromXmlWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            return MetaDataTypeTooling.LoadAsync(workspacePath, searchUpward, cancellationToken);
        }
    }
}
