using MetaSql.Core;
using MetaSql.Workflow;

namespace MetaSql.App;

public sealed class MetaSqlTargetInspector
{
    public MetaSqlTargetInspection Inspect(MetaSqlTargetContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var desiredModel = new DesiredSqlLoader().LoadFromDirectory(context.DesiredSqlPath);
        var liveSnapshot = new SqlServerLiveDatabaseInspector().Inspect(context.ConnectionString);
        var traits = new TraitsFileLoader().Load(context.TraitsFilePath);
        var plan = new SqlServerPreflightPlanner().Plan(desiredModel, liveSnapshot, traits);
        var renderedPlan = new SqlServerPreflightPlanRenderer().Render(plan);
        var blockers = new BlockerIdentityService().Build(plan);
        var catalog = new BlockerScriptCatalog().Load([context.BaselinePath, context.TargetPath], blockers);
        var matchedIds = catalog.Matched.Select(item => item.Header.BlockerId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var actionable = blockers
            .Where(item => item.Kind == BlockerKind.ManualRequired && !matchedIds.Contains(item.Id))
            .ToArray();
        var issues = new MetaSqlIssueClassifier().Classify(desiredModel, liveSnapshot, traits, blockers, catalog.Stale);
        var actionableIssues = issues.Where(issue => issue.NeedsAttention).ToArray();

        return new MetaSqlTargetInspection(
            context,
            desiredModel,
            liveSnapshot,
            traits,
            plan,
            renderedPlan,
            issues,
            actionableIssues,
            blockers,
            actionable,
            catalog.Matched,
            catalog.Stale);
    }
}
