namespace MetaPipeline;

public sealed class SqlServerBulkInsertTargetWriteOperation : IPipelineTargetWriteOperation
{
    private readonly SqlServerBulkCopyRowStreamWriter writer;

    public SqlServerBulkInsertTargetWriteOperation(
        string connectionString,
        string targetSqlIdentifier,
        PipelineRowStreamShape shape,
        MetaPipelineExecutionContext? executionContext = null,
        string targetDataTypeSystemName = "SqlServer",
        string? dataTypeWorkspacePath = null,
        int? timeoutSeconds = null)
    {
        writer = new SqlServerBulkCopyRowStreamWriter(
            connectionString,
            targetSqlIdentifier,
            shape,
            executionContext,
            targetDataTypeSystemName,
            dataTypeWorkspacePath,
            timeoutSeconds);
    }

    public string Name => "SqlServerBulkInsert";

    public PipelineRowStreamShape Shape => writer.Shape;

    public ValueTask BeginAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.CompletedTask;
    }

    public Task WriteBatchAsync(PipelineDataBatch batch, CancellationToken cancellationToken = default) =>
        writer.WriteBatchAsync(batch, cancellationToken);

    public ValueTask CompleteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync() => writer.DisposeAsync();
}
