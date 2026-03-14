using MetaSql.Core;
using MetaSql.Workflow;

namespace MetaSql.App;

public sealed record MetaSqlTargetInspection(
    MetaSqlTargetContext Context,
    SqlServerPreflightPlan Plan,
    string RenderedPlan,
    IReadOnlyList<Blocker> AllBlockers,
    IReadOnlyList<Blocker> ActionableBlockers,
    IReadOnlyList<BlockerScriptFile> MatchingScripts,
    IReadOnlyList<BlockerScriptFile> StaleScripts);
