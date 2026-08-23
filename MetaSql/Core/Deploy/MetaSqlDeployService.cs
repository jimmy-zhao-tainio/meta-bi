namespace MetaSql;

/// <summary>
/// Orchestrates deploy execution from a manifest-driven plan.
/// </summary>
public sealed class MetaSqlDeployService
{
    private readonly MetaSqlDeployExecutionEngine executionEngine = new();

    public async Task<MetaSqlDeployResult> DeployAsync(
        MetaSqlDeployRequest request,
        CancellationToken cancellationToken = default)
    {
        return await executionEngine.ExecuteCoreAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
