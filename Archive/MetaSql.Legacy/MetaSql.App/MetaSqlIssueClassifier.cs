using MetaSql.Core;
using MetaSql.Workflow;
using MetaSql.Workflow.Resolve;

namespace MetaSql.App;

public sealed partial class MetaSqlIssueClassifier
{
    public IReadOnlyList<MetaSqlIssue> Classify(
        DesiredSqlModel desiredModel,
        LiveDatabaseSnapshot liveSnapshot,
        IReadOnlyDictionary<string, SqlObjectTraits> traitsByObject,
        IReadOnlyList<Blocker> blockers,
        IReadOnlyList<BlockerScriptFile> staleScripts)
    {
        ArgumentNullException.ThrowIfNull(desiredModel);
        ArgumentNullException.ThrowIfNull(liveSnapshot);
        ArgumentNullException.ThrowIfNull(traitsByObject);
        ArgumentNullException.ThrowIfNull(blockers);
        ArgumentNullException.ThrowIfNull(staleScripts);

        var issues = new List<MetaSqlIssue>(blockers.Count + staleScripts.Count);
        issues.AddRange(blockers.Select(blocker => ClassifyBlocker(blocker, desiredModel, liveSnapshot, traitsByObject)));
        issues.AddRange(staleScripts.Select(staleScript => new MetaSqlIssue(
            MetaSqlIssueKind.StaleScript,
            staleScript.Header.ObjectName,
            [$"Script '{staleScript.Path}' no longer matches the current target state."],
            NeedsAttention: true,
            StaleScript: staleScript)));
        return issues;
    }

    private static MetaSqlIssue ClassifyBlocker(
        Blocker blocker,
        DesiredSqlModel desiredModel,
        LiveDatabaseSnapshot liveSnapshot,
        IReadOnlyDictionary<string, SqlObjectTraits> traitsByObject)
    {
        ArgumentNullException.ThrowIfNull(blocker);

        if (blocker.Kind == BlockerKind.Blocked)
        {
            return new MetaSqlIssue(
                MetaSqlIssueKind.DependentBlocked,
                blocker.ObjectName,
                blocker.Reasons,
                NeedsAttention: false,
                Blocker: blocker);
        }

        var scenario = BuildScenario(blocker, desiredModel, liveSnapshot, traitsByObject);
        var interpretations = RankInterpretations(scenario);
        var details = BuildDisplayDetails(scenario, interpretations);
        return new MetaSqlIssue(
            MetaSqlIssueKind.ManualScenario,
            blocker.ObjectName,
            details,
            NeedsAttention: true,
            Blocker: blocker,
            Scenario: scenario,
            Interpretations: interpretations);
    }
}
