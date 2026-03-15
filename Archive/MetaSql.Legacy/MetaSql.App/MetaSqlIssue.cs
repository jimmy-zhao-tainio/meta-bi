using MetaSql.Workflow;
using MetaSql.Workflow.Resolve;

namespace MetaSql.App;

public sealed record MetaSqlIssue(
    MetaSqlIssueKind Kind,
    string ObjectName,
    IReadOnlyList<string> Details,
    bool NeedsAttention,
    Blocker? Blocker = null,
    BlockerScriptFile? StaleScript = null,
    ResolverScenario? Scenario = null,
    IReadOnlyList<ResolverInterpretation>? Interpretations = null);
