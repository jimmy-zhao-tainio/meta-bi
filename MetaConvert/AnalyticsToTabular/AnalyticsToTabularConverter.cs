using Meta.Integration;
using Meta.Operations.Domain;
using MetaAnalytics;
using MetaAnalytics.Core;
using MetaTabular;
using MetaTabular.Core;
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
        => new MetaTabularWorkspaceBinding().Read(Convert(
            new MetaAnalyticsWorkspaceBinding().Write(source, AnalyticsModels.LoadContract()), progress));

    public static InMemoryWorkspace Convert(
        InMemoryWorkspace source,
        Action<MetaWeaveScriptExecutionProgress>? progress = null)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            ForwardDirection.Value,
            source,
            new InMemoryWorkspace(TabularModels.LoadContract(), new GenericInstance { ModelName = "MetaTabular" }),
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

        return result.OutputWorkspace!;
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
