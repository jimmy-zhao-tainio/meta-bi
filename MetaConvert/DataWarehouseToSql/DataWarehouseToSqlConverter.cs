using Meta.Integration;
using Meta.Operations.Domain;
using Dw = MetaDataWarehouse;
using Dwi = MetaDataWarehouseImplementation;
using MetaDataWarehouse.Core;
using MetaDataType;
using MetaDataType.Core;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;
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
        => ConvertCoreAsync(
            dataWarehouseWorkspacePath,
            implementationWorkspacePath,
            databaseName,
            cancellationToken,
            progress: null,
            dataTypeWorkspacePath: null,
            dataTypeConversionWorkspacePath: null);

    public static Task<InMemoryWorkspace> ConvertAsync(
        string dataWarehouseWorkspacePath,
        string implementationWorkspacePath,
        string databaseName,
        CancellationToken cancellationToken,
        Action<MetaWeaveScriptExecutionProgress>? progress)
        => ConvertCoreAsync(dataWarehouseWorkspacePath, implementationWorkspacePath, databaseName,
            cancellationToken, progress, dataTypeWorkspacePath: null, dataTypeConversionWorkspacePath: null);

    public static Task<InMemoryWorkspace> ConvertAsync(
        string dataWarehouseWorkspacePath,
        string implementationWorkspacePath,
        string databaseName,
        string? dataTypeWorkspacePath,
        string? dataTypeConversionWorkspacePath,
        CancellationToken cancellationToken = default,
        Action<MetaWeaveScriptExecutionProgress>? progress = null)
        => ConvertCoreAsync(dataWarehouseWorkspacePath, implementationWorkspacePath, databaseName,
            cancellationToken, progress, dataTypeWorkspacePath, dataTypeConversionWorkspacePath);

    private static async Task<InMemoryWorkspace> ConvertCoreAsync(
        string dataWarehouseWorkspacePath,
        string implementationWorkspacePath,
        string databaseName,
        CancellationToken cancellationToken,
        Action<MetaWeaveScriptExecutionProgress>? progress,
        string? dataTypeWorkspacePath,
        string? dataTypeConversionWorkspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataWarehouseWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(implementationWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var sourceTask = TypedWorkspaceModelMapper.LoadStateAsync(dataWarehouseWorkspacePath, cancellationToken);
        var implementationTask = TypedWorkspaceModelMapper.LoadStateAsync(implementationWorkspacePath, cancellationToken);
        var dataTypesTask = string.IsNullOrWhiteSpace(dataTypeWorkspacePath)
            ? MetaDataTypeWorkspaceProvider.LoadDefaultStateAsync(cancellationToken)
            : TypedWorkspaceModelMapper.LoadStateAsync(dataTypeWorkspacePath, cancellationToken);
        var conversionsTask = string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath)
            ? MetaDataTypeConversionWorkspaceProvider.LoadDefaultStateAsync(cancellationToken)
            : TypedWorkspaceModelMapper.LoadStateAsync(dataTypeConversionWorkspacePath, cancellationToken);
        await Task.WhenAll(sourceTask, implementationTask, dataTypesTask, conversionsTask).ConfigureAwait(false);
        return Convert(
            await sourceTask.ConfigureAwait(false),
            await implementationTask.ConfigureAwait(false),
            await dataTypesTask.ConfigureAwait(false),
            await conversionsTask.ConfigureAwait(false),
            databaseName, progress);
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
        => new MetaSqlWorkspaceBinding().Read(Convert(
            new Dw.MetaDataWarehouseWorkspaceBinding().Write(model, DataWarehouseModels.LoadContract()),
            new Dwi.MetaDataWarehouseImplementationWorkspaceBinding().Write(implementation, DataWarehouseImplementationModels.LoadContract()),
            MetaDataTypeWorkspaceProvider.LoadDefaultStateAsync().GetAwaiter().GetResult(),
            MetaDataTypeConversionWorkspaceProvider.LoadDefaultStateAsync().GetAwaiter().GetResult(),
            databaseName, progress));

    public static InMemoryWorkspace Convert(
        InMemoryWorkspace model,
        InMemoryWorkspace implementation,
        InMemoryWorkspace dataTypes,
        InMemoryWorkspace typeConversions,
        string databaseName,
        Action<MetaWeaveScriptExecutionProgress>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(implementation);
        ArgumentNullException.ThrowIfNull(dataTypes);
        ArgumentNullException.ThrowIfNull(typeConversions);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var targetContract = TypedWorkspaceModelMapper.LoadStateAsync(
            Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaSql")).GetAwaiter().GetResult().Model;

        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            ForwardDirection.Value,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["warehouse"] = model,
                ["implementation"] = implementation,
                ["dataTypes"] = dataTypes,
                ["typeConversions"] = typeConversions,
            },
            new InMemoryWorkspace(targetContract, new GenericInstance { ModelName = targetContract.Name }),
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

        return result.OutputWorkspace!;
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
