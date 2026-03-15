using MetaSql.Core;
using MetaSql.Workflow;

namespace MetaSql.App;

public sealed record MetaSqlTargetInspection(
    MetaSqlTargetContext Context,
    DesiredSqlModel DesiredModel,
    LiveDatabaseSnapshot LiveSnapshot,
    IReadOnlyDictionary<string, SqlObjectTraits> TraitsByObject,
    SqlServerPreflightPlan Plan,
    string RenderedPlan,
    IReadOnlyList<MetaSqlIssue> Issues,
    IReadOnlyList<MetaSqlIssue> ActionableIssues,
    IReadOnlyList<Blocker> AllBlockers,
    IReadOnlyList<Blocker> ActionableBlockers,
    IReadOnlyList<BlockerScriptFile> MatchingScripts,
    IReadOnlyList<BlockerScriptFile> StaleScripts);
