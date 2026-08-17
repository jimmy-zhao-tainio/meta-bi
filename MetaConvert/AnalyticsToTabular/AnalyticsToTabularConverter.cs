using Meta.Integration;
using MetaAnalytics;
using MetaTabular;
using MetaWeave.Core;
using MetaWeaveScript.Execution;

namespace MetaConvert.AnalyticsToTabular;

public static class AnalyticsToTabularConverter
{
    private static readonly Lazy<MetaWeaveScriptDirection> ForwardDirection = new(
        static () => new MetaWeaveScriptDirectionLoader().Load(
            ResolveWeaveWorkspacePath(),
            "forward"));

    public static MetaTabularModel Convert(MetaAnalyticsModel source)
        => Convert(source, progress: null);

    public static MetaTabularModel Convert(
        MetaAnalyticsModel source,
        Action<MetaWeaveScriptExecutionProgress>? progress)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            ForwardDirection.Value,
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(source),
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaTabularModel.CreateEmpty()),
            progress);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                "The sanctioned Analytics-to-Tabular weave rejected the source workspace:" +
                Environment.NewLine +
                string.Join(
                    Environment.NewLine,
                    result.Issues.Select(static issue =>
                        $"{issue.Code}: {issue.Message}")));
        }

        return TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            MetaTabularModel.CreateEmpty);
    }

    private static string ResolveWeaveWorkspacePath()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Weaves",
            "AnalyticsToTabular");
        if (!File.Exists(Path.Combine(path, "workspace.meta")))
        {
            throw new InvalidOperationException(
                $"The sanctioned Analytics-to-Tabular weave was not found at '{path}'.");
        }

        return path;
    }
}
