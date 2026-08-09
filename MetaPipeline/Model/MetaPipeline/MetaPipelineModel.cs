#nullable enable

using System.Collections.Generic;

namespace MetaPipeline
{
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
}
