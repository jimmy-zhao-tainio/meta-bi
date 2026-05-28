using MetaDataQuality;
using MetaDataQuality.Core;
using MetaOrchestration.Core;
using MetaPipeline;
using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql;

namespace MetaBi.TransformSurfaceContracts.Tests;

internal sealed class ContractWorkspace : IDisposable
{
    private readonly string rootPath;

    public ContractWorkspace()
    {
        rootPath = Path.Combine(Path.GetTempPath(), "MetaBi.TransformSurfaceContracts.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootPath);
    }

    public string TransformWorkspacePath => Path.Combine(rootPath, "Transform");

    public string BindingWorkspacePath => Path.Combine(rootPath, "Binding");

    public string PipelineWorkspacePath => Path.Combine(rootPath, "Pipeline");

    public MetaTransformScriptModel TransformModel { get; private set; } = MetaTransformScriptModel.CreateEmpty();

    public MetaTransformBindingModel BindingModel { get; private set; } = MetaTransformBindingModel.CreateEmpty();

    public async Task ImportTransformScriptsAsync(params TransformScriptSeed[] scripts)
    {
        var service = new MetaTransformScriptSqlService();
        for (var index = 0; index < scripts.Length; index++)
        {
            var script = scripts[index];
            if (index == 0)
            {
                await service.ImportFromSqlCodeToWorkspaceAsync(
                    script.Sql,
                    script.TargetSqlIdentifier,
                    TransformWorkspacePath,
                    script.Name);
            }
            else
            {
                await service.AddSqlCodeToWorkspaceAsync(
                    script.Sql,
                    script.TargetSqlIdentifier,
                    TransformWorkspacePath,
                    script.Name);
            }
        }

        TransformModel = MetaTransformScriptModel.LoadFromXmlWorkspace(TransformWorkspacePath, searchUpward: false);
    }

    public BindToWorkspaceResult Bind()
    {
        var result = new TransformBindingWorkspaceService().BindToWorkspace(
            TransformWorkspacePath,
            BindingWorkspacePath);
        BindingModel = result.Model;
        return result;
    }

    public MetaDataQualityModel DiscoverDataQuality()
    {
        return new MetaDataQualityCandidateDiscoveryService().Discover(TransformModel);
    }

    public void BuildPipeline(params PipelineSeed[] pipelines)
    {
        var model = MetaPipelineModel.CreateEmpty();
        foreach (var pipelineSeed in pipelines)
        {
            var binding = ResolveBinding(pipelineSeed.Script);
            var pipeline = new Pipeline
            {
                Id = $"pipeline:{pipelineSeed.PipelineName}",
                Name = pipelineSeed.PipelineName,
            };
            model.PipelineList.Add(pipeline);

            var connection = new ConnectionReference
            {
                Id = $"{pipeline.Id}:connection:execution",
                Pipeline = pipeline,
                Name = "Execution",
                EnvironmentVariableName = "EXECUTION_SQL",
            };
            model.ConnectionReferenceList.Add(connection);

            var transformTask = new PipelineTask
            {
                Id = $"{pipeline.Id}:task:transform",
                Pipeline = pipeline,
                Name = "transform",
                Ordinal = "1",
            };
            model.PipelineTaskList.Add(transformTask);
            model.TransformExecutionTaskList.Add(new TransformExecutionTask
            {
                Id = $"{transformTask.Id}:execution",
                PipelineTask = transformTask,
                ExecutionConnectionReference = connection,
                TransformScriptId = pipelineSeed.Script.Id,
                TransformBindingId = binding.Id,
            });

            if (string.IsNullOrWhiteSpace(pipelineSeed.InsertRowsTarget))
            {
                continue;
            }

            var rowStream = new RowStream
            {
                Id = $"{pipeline.Id}:rowstream:1",
                Pipeline = pipeline,
                Name = "transform.rows",
            };
            model.RowStreamList.Add(rowStream);
            model.RowStreamProducerList.Add(new RowStreamProducer
            {
                Id = $"{transformTask.Id}:producer",
                PipelineTask = transformTask,
                RowStream = rowStream,
            });

            var targetWriteTask = new PipelineTask
            {
                Id = $"{pipeline.Id}:task:target-write",
                Pipeline = pipeline,
                Name = "target-write",
                Ordinal = "2",
            };
            model.PipelineTaskList.Add(targetWriteTask);
            var targetWrite = new TargetWriteTask
            {
                Id = $"{targetWriteTask.Id}:target-write",
                PipelineTask = targetWriteTask,
                TargetConnectionReference = connection,
            };
            model.TargetWriteTaskList.Add(targetWrite);
            model.InsertRowsTargetWriteTaskList.Add(new InsertRowsTargetWriteTask
            {
                Id = $"{targetWrite.Id}:insert-rows",
                TargetWriteTask = targetWrite,
                TargetSqlIdentifier = pipelineSeed.InsertRowsTarget,
            });
            model.RowStreamConsumerList.Add(new RowStreamConsumer
            {
                Id = $"{targetWriteTask.Id}:consumer",
                PipelineTask = targetWriteTask,
                RowStream = rowStream,
            });
            model.TaskDependencyList.Add(new MetaPipeline.TaskDependency
            {
                Id = $"{transformTask.Id}:before:{targetWriteTask.Id}",
                Pipeline = pipeline,
                Predecessor = transformTask,
                Successor = targetWriteTask,
            });
        }

        model.SaveToXmlWorkspace(PipelineWorkspacePath);
    }

    public MetaPipelineExecutionDefinition ResolvePipelineExecution(TransformScript script)
    {
        return new MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
            TransformWorkspacePath,
            BindingWorkspacePath,
            script.Id,
            ResolveBinding(script).Id);
    }

    public OrchestrationAnalysisResult AnalyzeOrchestration()
    {
        return new MetaOrchestrationAnalysisService().Analyze(
            new OrchestrationAnalysisRequest(
                PipelineWorkspacePath,
                TransformWorkspacePath,
                BindingWorkspacePath,
                "ContractWitness"));
    }

    public TransformScript ResolveScript(string name)
    {
        return TransformModel.TransformScriptList.Single(item =>
            string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public TransformBinding ResolveBinding(TransformScript script)
    {
        return BindingModel.TransformBindingList.Single(item =>
            string.Equals(item.MetaTransformScriptTransformScriptId, script.Id, StringComparison.Ordinal));
    }

    public BoundStatementKind GetStatementKind(TransformScript script)
    {
        return new TransformScriptStatementKindService().GetStatementKind(TransformModel, script);
    }

    public void Dispose()
    {
        if (Directory.Exists(rootPath))
        {
            Directory.Delete(rootPath, recursive: true);
        }
    }
}

internal sealed record TransformScriptSeed(
    string Name,
    string Sql,
    string? TargetSqlIdentifier = null);

internal sealed record PipelineSeed(
    string PipelineName,
    TransformScript Script,
    string? InsertRowsTarget);
