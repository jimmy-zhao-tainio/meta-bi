using Meta.Integration;
using Meta.Operations.Domain;
using Dw = MetaDataWarehouse;
using Dwi = MetaDataWarehouseImplementation;
using MetaDataType;
using MetaDataTypeConversion;
using MetaSql;
using MetaWeave.Core;
using MetaWeaveScript.Execution;

namespace MetaConvert.DataWarehouseToSql;

public static class DataWarehouseToSqlConverter
{
    private static readonly Lazy<MetaWeaveScriptDirection> ForwardDirection = new(
        static () => new MetaWeaveScriptDirectionLoader().Load(
            ResolveWeaveWorkspacePath(),
            "forward"));

    public static Task<InMemoryWorkspace> ConvertAsync(
        string dataWarehouseWorkspacePath,
        string implementationWorkspacePath,
        string databaseName,
        CancellationToken cancellationToken = default)
        => ConvertAsync(
            dataWarehouseWorkspacePath,
            implementationWorkspacePath,
            databaseName,
            cancellationToken,
            progress: null);

    public static async Task<InMemoryWorkspace> ConvertAsync(
        string dataWarehouseWorkspacePath,
        string implementationWorkspacePath,
        string databaseName,
        CancellationToken cancellationToken,
        Action<MetaWeaveScriptExecutionProgress>? progress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataWarehouseWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(implementationWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var model = await TypedWorkspaceModelMapper.LoadAsync<Dw.MetaDataWarehouseModel>(
                dataWarehouseWorkspacePath,
                searchUpward: false,
                cancellationToken)
            .ConfigureAwait(false);
        var implementation = await TypedWorkspaceModelMapper.LoadAsync<Dwi.MetaDataWarehouseImplementationModel>(
                implementationWorkspacePath,
                searchUpward: false,
                cancellationToken)
            .ConfigureAwait(false);

        return TypedWorkspaceModelMapper.ToInMemoryWorkspace(
            ConvertToMetaSql(model, implementation, databaseName, progress));
    }

    public static MetaSqlModel ConvertToMetaSql(
        Dw.MetaDataWarehouseModel model,
        Dwi.MetaDataWarehouseImplementationModel implementation,
        string databaseName)
        => ConvertToMetaSql(model, implementation, databaseName, progress: null);

    public static MetaSqlModel ConvertToMetaSql(
        Dw.MetaDataWarehouseModel model,
        Dwi.MetaDataWarehouseImplementationModel implementation,
        string databaseName,
        Action<MetaWeaveScriptExecutionProgress>? progress)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(implementation);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            ForwardDirection.Value,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["warehouse"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(model),
                ["implementation"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(implementation),
                ["dataTypes"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeInstance.BuiltIn),
                ["typeConversions"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeConversionInstance.BuiltIn),
            },
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaSqlModel.CreateEmpty()),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["databaseName"] = databaseName,
            },
            progress);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                "The sanctioned Data-Warehouse-to-SQL weave rejected the source workspaces:" +
                Environment.NewLine +
                string.Join(
                    Environment.NewLine,
                    result.Issues.Select(static issue =>
                        $"{issue.Code}: {issue.Message}")));
        }

        return TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            MetaSqlModel.CreateEmpty);
    }

    private static string ResolveWeaveWorkspacePath()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Weaves",
            "DataWarehouseToSql");
        if (!File.Exists(Path.Combine(path, "workspace.meta")))
        {
            throw new InvalidOperationException(
                $"The sanctioned Data-Warehouse-to-SQL weave was not found at '{path}'.");
        }

        return path;
    }
}
