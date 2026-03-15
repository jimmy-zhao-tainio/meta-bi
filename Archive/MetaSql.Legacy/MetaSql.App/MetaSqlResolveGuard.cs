namespace MetaSql.App;

public sealed class MetaSqlResolveGuard
{
    public void EnsureResolveAllowed(MetaSqlTargetContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Mode == MetaSqlRootMode.Artifact)
        {
            throw new InvalidOperationException(
                $"target '{context.Name}' is loaded from an artifact root. Resolve is repo-mode only.");
        }
    }
}
