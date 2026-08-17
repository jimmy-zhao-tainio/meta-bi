using Meta.Operations.Domain;
using Meta.Integration;
using MetaDataVaultImplementation;
using MetaDataType;
using MetaDataTypeConversion;
using MetaBusinessDataVault;
using MetaRawDataVault;
using MetaSql;
using MetaWeave.Core;
using MetaWeaveScript.Execution;

namespace MetaConvert.DataVaultToSql;

public static partial class Converter
{
    private static readonly Lazy<MetaWeaveScriptDirection> RawForwardDirection = new(
        static () => new MetaWeaveScriptDirectionLoader().Load(
            ResolveRawWeaveWorkspacePath(),
            "forward"));

    public static async Task<InMemoryWorkspace> ConvertAsync(
        string dataVaultWorkspacePath,
        string implementationWorkspacePath,
        string databaseName,
        CancellationToken cancellationToken = default)
        => await ConvertAsync(
            dataVaultWorkspacePath,
            implementationWorkspacePath,
            databaseName,
            cancellationToken,
            progress: null).ConfigureAwait(false);

    public static async Task<InMemoryWorkspace> ConvertAsync(
        string dataVaultWorkspacePath,
        string implementationWorkspacePath,
        string databaseName,
        CancellationToken cancellationToken,
        Action<MetaWeaveScriptExecutionProgress>? progress)
    {
        if (string.IsNullOrWhiteSpace(dataVaultWorkspacePath))
        {
            throw new ArgumentException("Data Vault workspace path is required.", nameof(dataVaultWorkspacePath));
        }

        if (string.IsNullOrWhiteSpace(implementationWorkspacePath))
        {
            throw new ArgumentException("Implementation workspace path is required.", nameof(implementationWorkspacePath));
        }

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("Database name is required.", nameof(databaseName));
        }

        var dataVaultWorkspace = await Meta.Integration.TypedWorkspaceModelMapper
            .LoadStateAsync(dataVaultWorkspacePath, cancellationToken)
            .ConfigureAwait(false);
        var implementationModel = await Meta.Integration.TypedWorkspaceModelMapper.LoadAsync<MetaDataVaultImplementationModel>(implementationWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);

        switch (dataVaultWorkspace.Model.Name)
        {
            case "MetaRawDataVault":
                {
                    var rawModel = await Meta.Integration.TypedWorkspaceModelMapper.LoadAsync<MetaRawDataVaultModel>(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
                    var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
                        RawForwardDirection.Value,
                        new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["raw"] = Meta.Integration.TypedWorkspaceModelMapper.ToInMemoryWorkspace(rawModel),
                            ["implementation"] = Meta.Integration.TypedWorkspaceModelMapper.ToInMemoryWorkspace(implementationModel),
                            ["dataTypes"] = Meta.Integration.TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeInstance.BuiltIn),
                            ["typeConversions"] = Meta.Integration.TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeConversionInstance.BuiltIn),
                        },
                        Meta.Integration.TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaSqlModel.CreateEmpty()),
                        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["databaseName"] = databaseName,
                        },
                        progress);

                    if (!result.IsSuccess)
                    {
                        throw new InvalidOperationException(
                            "The sanctioned Raw-Data-Vault-to-SQL weave rejected the source workspaces:" +
                            Environment.NewLine +
                            string.Join(
                                Environment.NewLine,
                                result.Issues.Select(static issue => $"{issue.Code}: {issue.Message}")));
                    }

                    return result.OutputWorkspace!;
                }

            case "MetaBusinessDataVault":
                {
                    var context = CreateContext(
                        databaseName,
                        implementationModel,
                        SqlServerBusinessTypeLowering.Create(MetaDataTypeInstance.BuiltIn, MetaDataTypeConversionInstance.BuiltIn));
                    var businessModel = await Meta.Integration.TypedWorkspaceModelMapper.LoadAsync<MetaBusinessDataVaultModel>(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
                    var metaSqlModel = ConvertBusiness(businessModel, context);
                    return Meta.Integration.TypedWorkspaceModelMapper.ToInMemoryWorkspace(metaSqlModel);
                }

            default:
                throw new InvalidOperationException(
                    $"Workspace '{dataVaultWorkspacePath}' uses model '{dataVaultWorkspace.Model.Name}'. Expected MetaRawDataVault or MetaBusinessDataVault.");
        }
    }

    private static string ResolveRawWeaveWorkspacePath()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Weaves",
            "RawDataVaultToSql");
        if (!File.Exists(Path.Combine(path, "workspace.meta")))
        {
            throw new InvalidOperationException(
                $"The sanctioned Raw-Data-Vault-to-SQL weave was not found at '{path}'.");
        }

        return path;
    }

    private static ConversionContext CreateContext(
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
                Database = database,
            };
            metaSqlModel.SchemaList.Add(schema);
            schemasByName[schemaName] = schema;
        }

        return new ConversionContext
        {
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
        return implementationModel.BusinessHierarchicalLinkImplementationList.Select(row => row.SchemaName)
            .Concat(implementationModel.BusinessHierarchicalLinkSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessBridgeImplementationList.Select(row => row.SchemaName))
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
