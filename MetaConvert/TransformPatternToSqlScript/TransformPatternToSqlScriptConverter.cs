using Meta.Integration;
using Meta.Operations.Domain;
using MetaSqlScript;
using MetaTransformPattern;
using MetaTransformPatternInstance;
using MetaWeave.Core;
using MetaWeaveScript.Execution;

namespace MetaConvert.TransformPatternToSqlScript;

public static class TransformPatternToSqlScriptConverter
{
    private static readonly Lazy<MetaWeaveScriptDirection> ForwardDirection = new(
        static () => new MetaWeaveScriptDirectionLoader().Load(
            ResolveWeaveWorkspacePath(),
            "forward"));

    public static MetaSqlScriptModel Convert(
        MetaTransformPatternModel patterns,
        MetaTransformPatternInstanceModel instances)
        => Convert(patterns, instances, progress: null);

    public static MetaSqlScriptModel Convert(
        MetaTransformPatternModel patterns,
        MetaTransformPatternInstanceModel instances,
        Action<MetaWeaveScriptExecutionProgress>? progress)
    {
        ArgumentNullException.ThrowIfNull(patterns);
        ArgumentNullException.ThrowIfNull(instances);

        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            ForwardDirection.Value,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["pattern"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(patterns),
                ["instance"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(instances),
            },
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaSqlScriptModel.CreateEmpty()),
            progress: progress);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                "The sanctioned Transform-Pattern-to-SQL-Script weave rejected the source workspace:" +
                Environment.NewLine +
                string.Join(
                    Environment.NewLine,
                    result.Issues.Select(static issue =>
                        $"{issue.Code}: {issue.Message}")));
        }

        return TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            MetaSqlScriptModel.CreateEmpty);
    }

    private static string ResolveWeaveWorkspacePath()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Weaves",
            "TransformPatternToSqlScript");
        if (!File.Exists(Path.Combine(path, "workspace.meta")))
        {
            throw new InvalidOperationException(
                $"The sanctioned Transform-Pattern-to-SQL-Script weave was not found at '{path}'.");
        }

        return path;
    }
}
