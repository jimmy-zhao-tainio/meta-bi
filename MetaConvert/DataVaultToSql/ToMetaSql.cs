using Meta.Operations.Domain;
using Meta.Integration;
using MetaDataVaultImplementation;
using MetaDataType;
using MetaDataTypeConversion;
using MetaDataType.Core;
using MetaDataTypeConversion.Core;
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

    private static readonly Lazy<MetaWeaveScriptDirection> BusinessForwardDirection = new(
        static () => new MetaWeaveScriptDirectionLoader().Load(
            ResolveBusinessWeaveWorkspacePath(),
            "forward"));

    public static Task<InMemoryWorkspace> ConvertAsync(
        string dataVaultWorkspacePath,
        string implementationWorkspacePath,
        string databaseName,
        CancellationToken cancellationToken = default)
        => ConvertCoreAsync(
            dataVaultWorkspacePath,
            implementationWorkspacePath,
            databaseName,
            cancellationToken,
            progress: null,
            dataTypeWorkspacePath: null,
            dataTypeConversionWorkspacePath: null);

    public static Task<InMemoryWorkspace> ConvertAsync(
        string dataVaultWorkspacePath,
        string implementationWorkspacePath,
        string databaseName,
        CancellationToken cancellationToken,
        Action<MetaWeaveScriptExecutionProgress>? progress)
        => ConvertCoreAsync(dataVaultWorkspacePath, implementationWorkspacePath, databaseName,
            cancellationToken, progress, dataTypeWorkspacePath: null, dataTypeConversionWorkspacePath: null);

    public static Task<InMemoryWorkspace> ConvertAsync(
        string dataVaultWorkspacePath,
        string implementationWorkspacePath,
        string databaseName,
        string? dataTypeWorkspacePath,
        string? dataTypeConversionWorkspacePath,
        CancellationToken cancellationToken = default,
        Action<MetaWeaveScriptExecutionProgress>? progress = null)
        => ConvertCoreAsync(dataVaultWorkspacePath, implementationWorkspacePath, databaseName,
            cancellationToken, progress, dataTypeWorkspacePath, dataTypeConversionWorkspacePath);

    private static async Task<InMemoryWorkspace> ConvertCoreAsync(
        string dataVaultWorkspacePath,
        string implementationWorkspacePath,
        string databaseName,
        CancellationToken cancellationToken,
        Action<MetaWeaveScriptExecutionProgress>? progress,
        string? dataTypeWorkspacePath,
        string? dataTypeConversionWorkspacePath)
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

        var sourceTask = TypedWorkspaceModelMapper.LoadStateAsync(dataVaultWorkspacePath, cancellationToken);
        var implementationTask = TypedWorkspaceModelMapper.LoadStateAsync(implementationWorkspacePath, cancellationToken);
        var dataTypesTask = string.IsNullOrWhiteSpace(dataTypeWorkspacePath)
            ? MetaDataTypeWorkspaceProvider.LoadDefaultStateAsync(cancellationToken)
            : TypedWorkspaceModelMapper.LoadStateAsync(dataTypeWorkspacePath, cancellationToken);
        var conversionsTask = string.IsNullOrWhiteSpace(dataTypeConversionWorkspacePath)
            ? MetaDataTypeConversionWorkspaceProvider.LoadDefaultStateAsync(cancellationToken)
            : TypedWorkspaceModelMapper.LoadStateAsync(dataTypeConversionWorkspacePath, cancellationToken);
        var targetContractTask = TypedWorkspaceModelMapper.LoadStateAsync(
            Path.Combine(AppContext.BaseDirectory, "Workspaces", "MetaSql"), cancellationToken);
        await Task.WhenAll(sourceTask, implementationTask, dataTypesTask, conversionsTask, targetContractTask).ConfigureAwait(false);
        var dataVaultWorkspace = await sourceTask.ConfigureAwait(false);
        var implementationWorkspace = await implementationTask.ConfigureAwait(false);
        var dataTypes = await dataTypesTask.ConfigureAwait(false);
        var typeConversions = await conversionsTask.ConfigureAwait(false);
        var targetContract = (await targetContractTask.ConfigureAwait(false)).Model;

        switch (dataVaultWorkspace.Model.Name)
        {
            case "MetaRawDataVault":
                {
                    var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
                        RawForwardDirection.Value,
                        new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["raw"] = dataVaultWorkspace,
                            ["implementation"] = implementationWorkspace,
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
                    var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
                        BusinessForwardDirection.Value,
                        new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["business"] = dataVaultWorkspace,
                            ["implementation"] = implementationWorkspace,
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
                            "The sanctioned Business-Data-Vault-to-SQL weave rejected the source workspaces:" +
                            Environment.NewLine +
                            string.Join(
                                Environment.NewLine,
                                result.Issues.Select(static issue => $"{issue.Code}: {issue.Message}")));
                    }

                    return result.OutputWorkspace!;
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

    private static string ResolveBusinessWeaveWorkspacePath()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Weaves",
            "BusinessDataVaultToSql");
        if (!File.Exists(Path.Combine(path, "workspace.meta")))
        {
            throw new InvalidOperationException(
                $"The sanctioned Business-Data-Vault-to-SQL weave was not found at '{path}'.");
        }

        return path;
    }

}
