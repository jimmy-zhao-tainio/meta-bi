namespace MetaSql.App;

public sealed class MetaSqlDeploymentGuard
{
    public void EnsureCanDeploy(MetaSqlTargetInspection inspection)
    {
        ArgumentNullException.ThrowIfNull(inspection);

        if (inspection.ActionableBlockers.Count > 0)
        {
            throw new InvalidOperationException(
                $"target '{inspection.Context.Name}' still has unresolved manual issues.");
        }

        if (inspection.StaleScripts.Count > 0)
        {
            throw new InvalidOperationException(
                $"target '{inspection.Context.Name}' has active stale scripts in baseline or target/{inspection.Context.Name}. Resolve them before deploy.");
        }
    }
}
