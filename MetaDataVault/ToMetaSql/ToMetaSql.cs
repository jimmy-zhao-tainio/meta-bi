using Meta.Core.Domain;
using Meta.Core.Services;
using MetaDataVaultImplementation;
using MetaDataType.Instance;
using MetaDataTypeConversion.Instance;
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

        var workspaceService = new WorkspaceService();
        var dataVaultWorkspace = await workspaceService.LoadAsync(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
        var implementationModel = await MetaDataVaultImplementationModel.LoadFromXmlWorkspaceAsync(implementationWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);

        switch (dataVaultWorkspace.Model.Name)
        {
            case "MetaRawDataVault":
                {
                    var context = CreateContext(
                        pathToNewMetaSqlWorkspace,
                        databaseName,
                        implementationModel,
                        SqlServerBusinessTypeLowering.Create(MetaDataTypeInstance.Default, MetaDataTypeConversionInstance.Default));
                    var rawModel = await MetaRawDataVaultModel.LoadFromXmlWorkspaceAsync(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
                    var metaSqlModel = ConvertRaw(rawModel, context);
                    metaSqlModel.SaveToXmlWorkspace(pathToNewMetaSqlWorkspace);
                    return await workspaceService.LoadAsync(pathToNewMetaSqlWorkspace, searchUpward: false, cancellationToken).ConfigureAwait(false);
                }

            case "MetaBusinessDataVault":
                {
                    var context = CreateContext(
                        pathToNewMetaSqlWorkspace,
                        databaseName,
                        implementationModel,
                        SqlServerBusinessTypeLowering.Create(MetaDataTypeInstance.Default, MetaDataTypeConversionInstance.Default));
                    var businessModel = await MetaBusinessDataVaultModel.LoadFromXmlWorkspaceAsync(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
                    var metaSqlModel = ConvertBusiness(businessModel, context);
                    metaSqlModel.SaveToXmlWorkspace(pathToNewMetaSqlWorkspace);
                    return await workspaceService.LoadAsync(pathToNewMetaSqlWorkspace, searchUpward: false, cancellationToken).ConfigureAwait(false);
                }

            default:
                throw new InvalidOperationException(
                    $"Workspace '{dataVaultWorkspacePath}' uses model '{dataVaultWorkspace.Model.Name}'. Expected MetaRawDataVault or MetaBusinessDataVault.");
        }
    }

    private static ConversionContext CreateContext(
        string pathToNewMetaSqlWorkspace,
        string databaseName,
        MetaDataVaultImplementationModel implementationModel,
        SqlServerBusinessTypeLowering? businessTypeLowering = null)
    {
        var metaSqlModel = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = databaseName,
            Name = databaseName,
        };

        metaSqlModel.DatabaseList.Add(database);
        var schemasByName = new Dictionary<string, Schema>(StringComparer.OrdinalIgnoreCase);
        foreach (var schemaName in GetSchemaNames(implementationModel))
        {
            if (schemasByName.ContainsKey(schemaName))
            {
                continue;
            }

            var schema = new Schema
            {
                Id = $"{database.Id}.{schemaName}",
                Name = schemaName,
                DatabaseId = database.Id,
                Database = database,
            };
            metaSqlModel.SchemaList.Add(schema);
            schemasByName[schemaName] = schema;
        }

        return new ConversionContext
        {
            PathToNewMetaSqlWorkspace = pathToNewMetaSqlWorkspace,
            DatabaseName = databaseName,
            ImplementationModel = implementationModel,
            BusinessTypeLowering = businessTypeLowering,
            MetaSql = metaSqlModel,
            Database = database,
            SchemasByName = schemasByName,
        };
    }

    private static IEnumerable<string> GetSchemaNames(MetaDataVaultImplementationModel implementationModel)
    {
        return implementationModel.BusinessBridgeImplementationList.Select(row => row.SchemaName)
            .Concat(implementationModel.BusinessHierarchicalLinkImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessHierarchicalLinkSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessHubImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessHubSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessLinkImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessLinkSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessPointInTimeImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessReferenceImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessReferenceSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessSameAsLinkImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessSameAsLinkSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.RawHubImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.RawHubSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.RawLinkImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.RawLinkSatelliteImplementationList.Select(row => row.SchemaName))
            .Where(row => !string.IsNullOrWhiteSpace(row))
            .Select(row => row.Trim());
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
