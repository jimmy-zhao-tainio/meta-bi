namespace MetaPipeline;

public sealed class MetaPipelineSqlServerTransferService
{
    private readonly MetaPipelineTransferWorkspaceResolver workspaceResolver;
    private readonly BufferedPipelineTransferService bufferedTransferService;

    public MetaPipelineSqlServerTransferService(
        MetaPipelineTransferWorkspaceResolver? workspaceResolver = null,
        BufferedPipelineTransferService? bufferedTransferService = null)
    {
        this.workspaceResolver = workspaceResolver ?? new MetaPipelineTransferWorkspaceResolver();
        this.bufferedTransferService = bufferedTransferService ?? new BufferedPipelineTransferService();
    }

    public async Task<MetaPipelineTransferResult> TransferAsync(
        MetaPipelineSqlServerTransferRequest request,
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

        var source = new SqlServerPipelineBatchSource(
            request.SourceConnectionString,
            definition.SourceSql,
            definition.Columns,
            request.BatchSize);
        await using var writer = new SqlServerPipelineTargetWriter(
            request.TargetConnectionString,
            definition.TargetSqlIdentifier,
            definition.Columns);

        var transfer = await bufferedTransferService.TransferAsync(
            source,
            writer,
            cancellationToken).ConfigureAwait(false);

        var completedAtUtc = DateTimeOffset.UtcNow;
        return new MetaPipelineTransferResult(
            transfer.Succeeded ? MetaPipelineTransferStatus.Succeeded : MetaPipelineTransferStatus.Failed,
            definition.TransformScriptName,
            definition.TargetSqlIdentifier,
            definition.Columns.Count,
            transfer.RowCount,
            transfer.BatchCount,
            startedAtUtc,
            completedAtUtc,
            transfer.FailureMessage);
    }
}
