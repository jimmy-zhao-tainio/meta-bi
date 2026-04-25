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
            definition.RowStreamShape,
            request.BatchSize);
        await using var targetWriteOperation = new SqlServerBulkInsertTargetWriteOperation(
            request.TargetConnectionString,
            definition.TargetSqlIdentifier,
            definition.RowStreamShape);

        var execution = await bufferedExecutionService.ExecuteAsync(
            source,
            targetWriteOperation,
            cancellationToken).ConfigureAwait(false);

        var completedAtUtc = DateTimeOffset.UtcNow;
        return new MetaPipelineExecutionResult(
            execution.Succeeded ? MetaPipelineExecutionStatus.Succeeded : MetaPipelineExecutionStatus.Failed,
            definition.TransformScriptName,
            definition.TargetSqlIdentifier,
            targetWriteOperation.Name,
            definition.RowStreamShape.ColumnCount,
            execution.RowCount,
            execution.BatchCount,
            startedAtUtc,
            completedAtUtc,
            execution.FailureStage,
            execution.FailureMessage);
    }
}
