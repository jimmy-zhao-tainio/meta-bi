namespace MetaPipeline;

public sealed class MetaPipelineSqlServerExecutionService
{
    private readonly MetaPipelineExecutionWorkspaceResolver workspaceResolver;
    private readonly BufferedPipelineExecutionService bufferedExecutionService;

    public MetaPipelineSqlServerExecutionService(
        MetaPipelineExecutionWorkspaceResolver? workspaceResolver = null,
        BufferedPipelineExecutionService? bufferedExecutionService = null)
    {
        this.workspaceResolver = workspaceResolver ?? new MetaPipelineExecutionWorkspaceResolver();
        this.bufferedExecutionService = bufferedExecutionService ?? new BufferedPipelineExecutionService();
    }

    public async Task<MetaPipelineExecutionResult> ExecuteAsync(
        MetaPipelineSqlServerExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var startedAtUtc = DateTimeOffset.UtcNow;

        if (string.IsNullOrWhiteSpace(request.SourceConnectionString))
        {
            throw new MetaPipelineConfigurationException("Source connection string is required.");
        }

        if (string.IsNullOrWhiteSpace(request.TargetConnectionString))
        {
            throw new MetaPipelineConfigurationException("Target connection string is required.");
        }

        if (request.BatchSize <= 0)
        {
            throw new MetaPipelineConfigurationException("Batch size must be greater than zero.");
        }

        var definition = workspaceResolver.Resolve(
            request.TransformWorkspacePath,
            request.BindingWorkspacePath,
            request.TransformScriptName,
            request.TargetSqlIdentifier);

        var source = new SqlServerTransformRowStreamSource(
            request.SourceConnectionString,
            definition.SourceSql,
            definition.Columns,
            request.BatchSize);
        await using var writer = new SqlServerBulkCopyRowStreamWriter(
            request.TargetConnectionString,
            definition.TargetSqlIdentifier,
            definition.Columns);

        var execution = await bufferedExecutionService.ExecuteAsync(
            source,
            writer,
            cancellationToken).ConfigureAwait(false);

        var completedAtUtc = DateTimeOffset.UtcNow;
        return new MetaPipelineExecutionResult(
            execution.Succeeded ? MetaPipelineExecutionStatus.Succeeded : MetaPipelineExecutionStatus.Failed,
            definition.TransformScriptName,
            definition.TargetSqlIdentifier,
            definition.Columns.Count,
            execution.RowCount,
            execution.BatchCount,
            startedAtUtc,
            completedAtUtc,
            execution.FailureMessage);
    }
}
