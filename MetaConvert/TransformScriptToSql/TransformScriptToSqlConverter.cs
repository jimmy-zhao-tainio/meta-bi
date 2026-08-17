using Meta.Operations.Domain;
using Meta.Integration;
using MetaSql;
using MetaWeave.Core;
using MetaWeaveScript.Execution;

namespace MetaConvert.TransformScriptToSql;

public static class TransformScriptToSqlConverter
{
    private static readonly Lazy<MetaWeaveScriptDirection> ForwardDirection = new(
        static () => new MetaWeaveScriptDirectionLoader().Load(
            ResolveWeaveWorkspacePath(),
            "forward"));

    public static async Task<InMemoryWorkspace> ConvertAsync(
        string transformScriptWorkspacePath,
        string databaseName,
        CancellationToken cancellationToken = default,
        Action<MetaWeaveScriptExecutionProgress>? progress = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformScriptWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var sourceWorkspace = await TypedWorkspaceModelMapper.LoadStateAsync(
                transformScriptWorkspacePath,
                cancellationToken)
            .ConfigureAwait(false);
        var direction = ForwardDirection.Value;
        var targetWorkspace = TypedWorkspaceModelMapper.ToInMemoryWorkspace(
            MetaSqlModel.CreateEmpty());
        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            direction,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["transform"] = sourceWorkspace,
            },
            targetWorkspace,
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["databaseName"] = databaseName.Trim(),
            },
            progress);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                "The sanctioned TransformScript-to-SQL weave rejected the source workspace:" +
                Environment.NewLine +
                string.Join(
                    Environment.NewLine,
                    result.Issues.Select(static issue =>
                        $"{issue.Code}: {issue.Message}")));
        }

        return result.OutputWorkspace!;
    }

    public static async Task<InMemoryWorkspace> ConvertAsync(
        string transformScriptWorkspacePath,
        string pathToNewMetaSqlWorkspace,
        string databaseName,
        string outputRepresentation,
        string? connectionEnvironmentVariable = null,
        CancellationToken cancellationToken = default,
        Action<MetaWeaveScriptExecutionProgress>? progress = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformScriptWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(pathToNewMetaSqlWorkspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputRepresentation);

        var result = await ConvertAsync(
                transformScriptWorkspacePath,
                databaseName,
                cancellationToken,
                progress)
            .ConfigureAwait(false);
        var metaSql = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result,
            static () => new MetaSqlModel());
        await TypedWorkspaceModelMapper.CreateAsync(
                metaSql,
                pathToNewMetaSqlWorkspace,
                outputRepresentation,
                connectionEnvironmentVariable,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return result;
    }

    private static string ResolveWeaveWorkspacePath()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Weaves",
            "TransformScriptToSql");
        if (!File.Exists(Path.Combine(path, "workspace.meta")))
        {
            throw new InvalidOperationException(
                $"The sanctioned TransformScript-to-SQL weave was not found at '{path}'.");
        }

        return path;
    }
}
