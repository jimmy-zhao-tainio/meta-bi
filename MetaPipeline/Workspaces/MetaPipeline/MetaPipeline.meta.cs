#nullable enable
using System;
using System.Collections.Generic;

namespace MetaPipeline;
public sealed partial class ConnectionReference
{
    public string Id { get; set; } = null !;
    public string EnvironmentVariableName { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Pipeline Pipeline { get; set; } = null !;
}

public sealed partial class ExecutableTask
{
    public string Id { get; set; } = null !;
    public string? Arguments { get; set; }
    public string ExecutablePath { get; set; } = null !;
    public string? SuccessExitCode { get; set; }
    public string? TimeoutSeconds { get; set; }
    public string? WorkingDirectory { get; set; }
    public PipelineTask PipelineTask { get; set; } = null !;
}

public sealed partial class InsertRowsTargetWriteTask
{
    public string Id { get; set; } = null !;
    public string? BatchSize { get; set; }
    public string? TargetDataTypeSystemName { get; set; }
    public string TargetSqlIdentifier { get; set; } = null !;
    public TargetWriteTask TargetWriteTask { get; set; } = null !;
}

public sealed partial class Pipeline
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class PipelineTask
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Pipeline Pipeline { get; set; } = null !;
}

public sealed partial class RowStream
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Pipeline Pipeline { get; set; } = null !;
}

public sealed partial class RowStreamColumn
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public RowStream RowStream { get; set; } = null !;
}

public sealed partial class RowStreamConsumer
{
    public string Id { get; set; } = null !;
    public PipelineTask PipelineTask { get; set; } = null !;
    public RowStream RowStream { get; set; } = null !;
}

public sealed partial class RowStreamProducer
{
    public string Id { get; set; } = null !;
    public PipelineTask PipelineTask { get; set; } = null !;
    public RowStream RowStream { get; set; } = null !;
}

public sealed partial class TargetWriteTask
{
    public string Id { get; set; } = null !;
    public PipelineTask PipelineTask { get; set; } = null !;
    public ConnectionReference TargetConnectionReference { get; set; } = null !;
}

public sealed partial class TaskDependency
{
    public string Id { get; set; } = null !;
    public Pipeline Pipeline { get; set; } = null !;
    public PipelineTask Predecessor { get; set; } = null !;
    public PipelineTask Successor { get; set; } = null !;
}

public sealed partial class TransformExecutionTask
{
    public string Id { get; set; } = null !;
    public string BindingWorkspacePath { get; set; } = null !;
    public string? TimeoutSeconds { get; set; }
    public string TransformBindingId { get; set; } = null !;
    public string TransformScriptId { get; set; } = null !;
    public string TransformWorkspacePath { get; set; } = null !;
    public ConnectionReference ExecutionConnectionReference { get; set; } = null !;
    public PipelineTask PipelineTask { get; set; } = null !;
}

public sealed partial class MetaPipelineModel
{
    public static MetaPipelineModel CreateEmpty() => new();
    public List<ConnectionReference> ConnectionReferenceList { get; set; } = new();
    public List<ExecutableTask> ExecutableTaskList { get; set; } = new();
    public List<InsertRowsTargetWriteTask> InsertRowsTargetWriteTaskList { get; set; } = new();
    public List<Pipeline> PipelineList { get; set; } = new();
    public List<PipelineTask> PipelineTaskList { get; set; } = new();
    public List<RowStream> RowStreamList { get; set; } = new();
    public List<RowStreamColumn> RowStreamColumnList { get; set; } = new();
    public List<RowStreamConsumer> RowStreamConsumerList { get; set; } = new();
    public List<RowStreamProducer> RowStreamProducerList { get; set; } = new();
    public List<TargetWriteTask> TargetWriteTaskList { get; set; } = new();
    public List<TaskDependency> TaskDependencyList { get; set; } = new();
    public List<TransformExecutionTask> TransformExecutionTaskList { get; set; } = new();
}

public static partial class MetaPipelineInstance
{
    private static readonly MetaPipelineModel _builtIn = CreateBuiltIn();
    public static MetaPipelineModel BuiltIn => _builtIn;

    public static MetaPipelineModel CreateBuiltIn()
    {
        var model = MetaPipelineModel.CreateEmpty();
        return model;
    }
}