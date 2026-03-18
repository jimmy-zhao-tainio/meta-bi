using Meta.Core.Domain;
using Meta.Core.Services;
using MetaDataVaultImplementation;
using MetaBusinessDataVault;
using MetaRawDataVault;
using MetaSql;

namespace MetaDataVault.ToMetaSql;

public static partial class Converter
{
    public static async Task<Workspace> ConvertAsync(
        string dataVaultWorkspacePath,
        string pathToNewMetaSqlWorkspace,
        string implementationWorkspacePath,
        string databaseName,
        string defaultSchemaName = "dbo",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dataVaultWorkspacePath))
        {
            throw new ArgumentException("Data Vault workspace path is required.", nameof(dataVaultWorkspacePath));
        }

        if (string.IsNullOrWhiteSpace(pathToNewMetaSqlWorkspace))
        {
            throw new ArgumentException("MetaSql workspace path is required.", nameof(pathToNewMetaSqlWorkspace));
        }

        if (string.IsNullOrWhiteSpace(implementationWorkspacePath))
        {
            throw new ArgumentException("Implementation workspace path is required.", nameof(implementationWorkspacePath));
        }

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("Database name is required.", nameof(databaseName));
        }

        if (string.IsNullOrWhiteSpace(defaultSchemaName))
        {
            throw new ArgumentException("Default schema name is required.", nameof(defaultSchemaName));
        }

        var workspaceService = new WorkspaceService();
        var dataVaultWorkspace = await workspaceService.LoadAsync(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
        var implementationModel = await MetaDataVaultImplementationModel.LoadFromXmlWorkspaceAsync(implementationWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
        var context = CreateContext(
            pathToNewMetaSqlWorkspace,
            databaseName,
            defaultSchemaName,
            implementationModel);

        switch (dataVaultWorkspace.Model.Name)
        {
            case "MetaRawDataVault":
                {
                    var rawModel = await MetaRawDataVaultModel.LoadFromXmlWorkspaceAsync(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
                    return ConvertRaw(rawModel, context);
                }

            case "MetaBusinessDataVault":
                {
                    var businessModel = await MetaBusinessDataVaultModel.LoadFromXmlWorkspaceAsync(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
                    return ConvertBusiness(businessModel, context);
                }

            default:
                throw new InvalidOperationException(
                    $"Workspace '{dataVaultWorkspacePath}' uses model '{dataVaultWorkspace.Model.Name}'. Expected MetaRawDataVault or MetaBusinessDataVault.");
        }
    }

    private static ConversionContext CreateContext(
        string pathToNewMetaSqlWorkspace,
        string databaseName,
        string defaultSchemaName,
        MetaDataVaultImplementationModel implementationModel)
    {
        var metaSqlModel = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = databaseName,
            Name = databaseName,
            Platform = "sqlserver",
        };

        var schema = new Schema
        {
            Id = $"{database.Id}.{defaultSchemaName}",
            Name = defaultSchemaName,
            DatabaseId = database.Id,
            Database = database,
        };

        metaSqlModel.DatabaseList.Add(database);
        metaSqlModel.SchemaList.Add(schema);

        return new ConversionContext
        {
            PathToNewMetaSqlWorkspace = pathToNewMetaSqlWorkspace,
            DatabaseName = databaseName,
            DefaultSchemaName = defaultSchemaName,
            ImplementationModel = implementationModel,
            MetaSql = metaSqlModel,
            Database = database,
            DefaultSchema = schema,
        };
    }

    private static Dictionary<string, T> IndexById<T>(IEnumerable<T> rows, Func<T, string> idSelector)
        where T : class
    {
        var index = new Dictionary<string, T>(StringComparer.Ordinal);
        foreach (var row in rows)
        {
            index[idSelector(row)] = row;
        }

        return index;
    }

    private static Dictionary<string, List<T>> GroupById<T>(IEnumerable<T> rows, Func<T, string> keySelector)
        where T : class
    {
        var groups = new Dictionary<string, List<T>>(StringComparer.Ordinal);
        foreach (var row in rows)
        {
            var key = keySelector(row);
            if (!groups.TryGetValue(key, out var bucket))
            {
                bucket = new List<T>();
                groups[key] = bucket;
            }

            bucket.Add(row);
        }

        return groups;
    }

    private static IReadOnlyList<T> GetGroup<T>(IReadOnlyDictionary<string, List<T>> groups, string key)
        where T : class
    {
        return groups.TryGetValue(key, out var bucket)
            ? bucket
            : Array.Empty<T>();
    }
}
