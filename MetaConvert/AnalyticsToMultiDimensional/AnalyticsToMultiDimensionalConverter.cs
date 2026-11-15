using Meta.Integration;
using Meta.Operations.Domain;
using MetaAnalytics;
using MetaAnalytics.Core;
using MetaMultiDimensional;
using MetaMultiDimensional.Core;
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
        => new MetaMultiDimensionalWorkspaceBinding().Read(Convert(
            new MetaAnalyticsWorkspaceBinding().Write(source, AnalyticsModels.LoadContract()), progress));

    public static InMemoryWorkspace Convert(
        InMemoryWorkspace source,
        Action<MetaWeaveScriptExecutionProgress>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            ForwardDirection.Value,
            source,
            new InMemoryWorkspace(MultiDimensionalModels.LoadContract(), new GenericInstance { ModelName = "MetaMultiDimensional" }),
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

        return result.OutputWorkspace!;
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
