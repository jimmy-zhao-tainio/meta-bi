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

    private static readonly Lazy<MetaWeaveScriptDirection> BusinessForwardDirection = new(
        static () => new MetaWeaveScriptDirectionLoader().Load(
            ResolveBusinessWeaveWorkspacePath(),
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
                    var businessModel = await Meta.Integration.TypedWorkspaceModelMapper.LoadAsync<MetaBusinessDataVaultModel>(dataVaultWorkspacePath, searchUpward: false, cancellationToken).ConfigureAwait(false);
                    var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
                        BusinessForwardDirection.Value,
                        new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["business"] = Meta.Integration.TypedWorkspaceModelMapper.ToInMemoryWorkspace(businessModel),
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
