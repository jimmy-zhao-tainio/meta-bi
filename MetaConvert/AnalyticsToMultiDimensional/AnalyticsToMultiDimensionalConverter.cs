using Meta.Integration;
using MetaAnalytics;
using MetaMultiDimensional;
using MetaWeave.Core;
using MetaWeaveScript.Execution;

namespace MetaConvert.AnalyticsToMultiDimensional;

public static class AnalyticsToMultiDimensionalConverter
{
    private static readonly Lazy<MetaWeaveScriptDirection> ForwardDirection = new(
        static () => new MetaWeaveScriptDirectionLoader().Load(
            ResolveWeaveWorkspacePath(),
            "forward"));

    public static MetaMultiDimensionalModel Convert(MetaAnalyticsModel source)
        => Convert(source, progress: null);

    public static MetaMultiDimensionalModel Convert(
        MetaAnalyticsModel source,
        Action<MetaWeaveScriptExecutionProgress>? progress)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            ForwardDirection.Value,
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(source),
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaMultiDimensionalModel.CreateEmpty()),
            progress);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                "The sanctioned Analytics-to-MultiDimensional weave rejected the source workspace:" +
                Environment.NewLine +
                string.Join(
                    Environment.NewLine,
                    result.Issues.Select(static issue =>
                        $"{issue.Code}: {issue.Message}")));
        }

        return TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            MetaMultiDimensionalModel.CreateEmpty);
    }

    private static string ResolveWeaveWorkspacePath()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Weaves",
            "AnalyticsToMultiDimensional");
        if (!File.Exists(Path.Combine(path, "workspace.meta")))
        {
            throw new InvalidOperationException(
                $"The sanctioned Analytics-to-MultiDimensional weave was not found at '{path}'.");
        }

        return path;
    }
}
