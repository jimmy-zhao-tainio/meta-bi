namespace MetaPipeline;

public sealed record MetaPipelineSqlServerExecutionRequest(
    string TransformWorkspacePath,
    string BindingWorkspacePath,
    string ExecutionConnectionString,
    string? TargetConnectionString,
    string TransformScriptId,
    string TransformBindingId,
    string? TargetSqlIdentifier = null,
    int BatchSize = 1000,
    int? TimeoutSeconds = null,
    string TargetWriteModelName = "InsertRows",
    string? TransformTaskName = null,
    string? TargetWriteTaskName = null,
    MetaPipelineExecutionContext? ExecutionContext = null,
    string TargetDataTypeSystemName = "SqlServer",
    string? DataTypeConversionWorkspacePath = null);
