namespace MetaSql.App;

public sealed class MetaSqlDeploymentSession
{
    public MetaSqlTargetInspection Deploy(MetaSqlTargetInspection inspection)
    {
        ArgumentNullException.ThrowIfNull(inspection);

        new MetaSqlDeploymentGuard().EnsureCanDeploy(inspection);

        var executor = new SqlServerScriptExecutor();
        executor.ExecuteScripts(inspection.Context.ConnectionString, inspection.MatchingScripts.Select(item => item.Path));

        var postScriptInspection = new MetaSqlTargetInspector().Inspect(inspection.Context);
        if (postScriptInspection.AllBlockers.Count > 0)
        {
            throw new InvalidOperationException(
                $"target '{inspection.Context.Name}' still has blocking issues after matching scripts were applied.");
        }

        executor.ExecutePlan(postScriptInspection.Context.ConnectionString, postScriptInspection.Plan);
        return new MetaSqlTargetInspector().Inspect(inspection.Context);
    }
}
