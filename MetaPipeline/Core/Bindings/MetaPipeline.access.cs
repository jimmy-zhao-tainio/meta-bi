// Derived typed member access. Keep outside authored workspace source sets.
#nullable enable
namespace MetaPipeline;
public interface IMetaPipelineCollectionVisitor
{
    void Visit<TEntity>(string entityName, global::System.Func<global::MetaPipeline.MetaPipelineModel, global::System.Collections.Generic.IList<TEntity>> rows, global::System.Func<TEntity, string> readId, global::System.Action<TEntity, string> writeId)
        where TEntity : class;
}

public interface IMetaPipelineRelationshipVisitor
{
    void Visit<TRow, TTarget>(string entityName, string navigationName, string targetEntityName, bool isNullable, global::System.Func<global::MetaPipeline.MetaPipelineModel, global::System.Collections.Generic.IList<TRow>> rows, global::System.Func<TRow, TTarget?> read, global::System.Action<TRow, TTarget?> write, global::System.Func<TTarget, string> readTargetId)
        where TRow : class where TTarget : class;
}

public static class MetaPipelineModelAccess
{
    public static void VisitCollections(IMetaPipelineCollectionVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
        visitor.Visit<global::MetaPipeline.ConnectionReference>("ConnectionReference", static model => model.ConnectionReferenceList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaPipeline.ExecutableTask>("ExecutableTask", static model => model.ExecutableTaskList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaPipeline.InsertRowsTargetWriteTask>("InsertRowsTargetWriteTask", static model => model.InsertRowsTargetWriteTaskList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaPipeline.Pipeline>("Pipeline", static model => model.PipelineList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaPipeline.PipelineTask>("PipelineTask", static model => model.PipelineTaskList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaPipeline.RowStream>("RowStream", static model => model.RowStreamList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaPipeline.RowStreamColumn>("RowStreamColumn", static model => model.RowStreamColumnList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaPipeline.RowStreamConsumer>("RowStreamConsumer", static model => model.RowStreamConsumerList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaPipeline.RowStreamProducer>("RowStreamProducer", static model => model.RowStreamProducerList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaPipeline.TargetWriteTask>("TargetWriteTask", static model => model.TargetWriteTaskList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaPipeline.TaskDependency>("TaskDependency", static model => model.TaskDependencyList, static row => row.Id, static (row, id) => row.Id = id);
        visitor.Visit<global::MetaPipeline.TransformExecutionTask>("TransformExecutionTask", static model => model.TransformExecutionTaskList, static row => row.Id, static (row, id) => row.Id = id);
    }

    public static void VisitRelationships(IMetaPipelineRelationshipVisitor visitor)
    {
        global::System.ArgumentNullException.ThrowIfNull(visitor);
        visitor.Visit<global::MetaPipeline.ConnectionReference, global::MetaPipeline.Pipeline>("ConnectionReference", "Pipeline", "Pipeline", false, static model => model.ConnectionReferenceList, static row => row.Pipeline, static (row, target) => row.Pipeline = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.ExecutableTask, global::MetaPipeline.PipelineTask>("ExecutableTask", "PipelineTask", "PipelineTask", false, static model => model.ExecutableTaskList, static row => row.PipelineTask, static (row, target) => row.PipelineTask = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.InsertRowsTargetWriteTask, global::MetaPipeline.TargetWriteTask>("InsertRowsTargetWriteTask", "TargetWriteTask", "TargetWriteTask", false, static model => model.InsertRowsTargetWriteTaskList, static row => row.TargetWriteTask, static (row, target) => row.TargetWriteTask = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.PipelineTask, global::MetaPipeline.Pipeline>("PipelineTask", "Pipeline", "Pipeline", false, static model => model.PipelineTaskList, static row => row.Pipeline, static (row, target) => row.Pipeline = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.RowStream, global::MetaPipeline.Pipeline>("RowStream", "Pipeline", "Pipeline", false, static model => model.RowStreamList, static row => row.Pipeline, static (row, target) => row.Pipeline = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.RowStreamColumn, global::MetaPipeline.RowStream>("RowStreamColumn", "RowStream", "RowStream", false, static model => model.RowStreamColumnList, static row => row.RowStream, static (row, target) => row.RowStream = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.RowStreamConsumer, global::MetaPipeline.PipelineTask>("RowStreamConsumer", "PipelineTask", "PipelineTask", false, static model => model.RowStreamConsumerList, static row => row.PipelineTask, static (row, target) => row.PipelineTask = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.RowStreamConsumer, global::MetaPipeline.RowStream>("RowStreamConsumer", "RowStream", "RowStream", false, static model => model.RowStreamConsumerList, static row => row.RowStream, static (row, target) => row.RowStream = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.RowStreamProducer, global::MetaPipeline.PipelineTask>("RowStreamProducer", "PipelineTask", "PipelineTask", false, static model => model.RowStreamProducerList, static row => row.PipelineTask, static (row, target) => row.PipelineTask = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.RowStreamProducer, global::MetaPipeline.RowStream>("RowStreamProducer", "RowStream", "RowStream", false, static model => model.RowStreamProducerList, static row => row.RowStream, static (row, target) => row.RowStream = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.TargetWriteTask, global::MetaPipeline.PipelineTask>("TargetWriteTask", "PipelineTask", "PipelineTask", false, static model => model.TargetWriteTaskList, static row => row.PipelineTask, static (row, target) => row.PipelineTask = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.TargetWriteTask, global::MetaPipeline.ConnectionReference>("TargetWriteTask", "TargetConnectionReference", "ConnectionReference", false, static model => model.TargetWriteTaskList, static row => row.TargetConnectionReference, static (row, target) => row.TargetConnectionReference = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.TaskDependency, global::MetaPipeline.Pipeline>("TaskDependency", "Pipeline", "Pipeline", false, static model => model.TaskDependencyList, static row => row.Pipeline, static (row, target) => row.Pipeline = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.TaskDependency, global::MetaPipeline.PipelineTask>("TaskDependency", "Predecessor", "PipelineTask", false, static model => model.TaskDependencyList, static row => row.Predecessor, static (row, target) => row.Predecessor = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.TaskDependency, global::MetaPipeline.PipelineTask>("TaskDependency", "Successor", "PipelineTask", false, static model => model.TaskDependencyList, static row => row.Successor, static (row, target) => row.Successor = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.TransformExecutionTask, global::MetaPipeline.ConnectionReference>("TransformExecutionTask", "ExecutionConnectionReference", "ConnectionReference", false, static model => model.TransformExecutionTaskList, static row => row.ExecutionConnectionReference, static (row, target) => row.ExecutionConnectionReference = target!, static target => target.Id);
        visitor.Visit<global::MetaPipeline.TransformExecutionTask, global::MetaPipeline.PipelineTask>("TransformExecutionTask", "PipelineTask", "PipelineTask", false, static model => model.TransformExecutionTaskList, static row => row.PipelineTask, static (row, target) => row.PipelineTask = target!, static target => target.Id);
    }
}