using System.Threading;
using System.Threading.Tasks;
using Meta.Adapters;
using Meta.Core.Domain;
using Meta.Core.Services;

namespace MetaSchema
{
    public static class MetaSchemaTooling
    {
        public static async Task<MetaSchemaModel> LoadAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            var workspace = await LoadWorkspaceAsync(workspacePath, searchUpward, cancellationToken).ConfigureAwait(false);
            return MetaSchemaModelFactory.CreateFromWorkspace(workspace);
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

    public sealed partial class MetaSchemaModel
    {
        public static MetaSchemaModel LoadFromXmlWorkspace(
            string workspacePath,
            bool searchUpward = true)
        {
            return LoadFromXmlWorkspaceAsync(workspacePath, searchUpward).GetAwaiter().GetResult();
        }

        public static Task<MetaSchemaModel> LoadFromXmlWorkspaceAsync(
            string workspacePath,
            bool searchUpward = true,
            CancellationToken cancellationToken = default)
        {
            return MetaSchemaTooling.LoadAsync(workspacePath, searchUpward, cancellationToken);
        }
    }
}
