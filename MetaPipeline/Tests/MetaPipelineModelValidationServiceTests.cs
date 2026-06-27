namespace MetaPipeline.Tests;

public sealed class MetaPipelineModelValidationServiceTests
{
    [Fact]
    public void ValidatePipeline_WhenPipelineHasNoTasks_ReturnsError()
    {
        var model = MetaPipelineModel.CreateEmpty();
        model.PipelineList.Add(new Pipeline
        {
            Id = "CustomerLoad",
            Name = "CustomerLoad",
            Description = "Test",
        });

        var result = new MetaPipelineModelValidationService().ValidatePipeline(model, "CustomerLoad");

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, item => item.Contains("has no PipelineTask rows", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidatePipeline_WhenMultipleTransformTasksFormSerialChain_IsValid()
    {
        var model = MetaPipelineModel.CreateEmpty();
        var pipeline = AddPipeline(model);
        var source = AddConnection(model, pipeline);
        var firstTask = AddTransformTask(model, pipeline, source, "first", "1");
        var secondTask = AddTransformTask(model, pipeline, source, "second", "2");
        model.TaskDependencyList.Add(new TaskDependency
        {
            Id = "CustomerLoad.first.Before.CustomerLoad.second",
            Pipeline = pipeline,
            Predecessor = firstTask,
            Successor = secondTask,
        });

        var result = new MetaPipelineModelValidationService().ValidatePipeline(model, "CustomerLoad");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidatePipeline_WhenExecutableTaskOnly_IsValid()
    {
        var model = MetaPipelineModel.CreateEmpty();
        var pipeline = AddPipeline(model);
        var task = new PipelineTask
        {
            Id = "CustomerLoad.prepare",
            Pipeline = pipeline,
            Name = "prepare",
        };
        model.PipelineTaskList.Add(task);
        model.ExecutableTaskList.Add(new ExecutableTask
        {
            Id = "CustomerLoad.prepare.Executable",
            PipelineTask = task,
            ExecutablePath = "cmd.exe",
        });

        var result = new MetaPipelineModelValidationService().ValidatePipeline(model, "CustomerLoad");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidatePipeline_WhenTaskHasExecutableAndTransformDetails_ReturnsError()
    {
        var model = MetaPipelineModel.CreateEmpty();
        var pipeline = AddPipeline(model);
        var source = AddConnection(model, pipeline);
        var task = AddTransformTask(model, pipeline, source, "prepare", "1");
        model.ExecutableTaskList.Add(new ExecutableTask
        {
            Id = "CustomerLoad.prepare.Executable",
            PipelineTask = task,
            ExecutablePath = "cmd.exe",
        });

        var result = new MetaPipelineModelValidationService().ValidatePipeline(model, "CustomerLoad");

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, item => item.Contains("multiple detail kinds", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidatePipeline_WhenSerialChainBranches_ReturnsError()
    {
        var model = MetaPipelineModel.CreateEmpty();
        var pipeline = AddPipeline(model);
        var source = AddConnection(model, pipeline);
        var firstTask = AddTransformTask(model, pipeline, source, "first", "1");
        var secondTask = AddTransformTask(model, pipeline, source, "second", "2");
        var thirdTask = AddTransformTask(model, pipeline, source, "third", "3");
        model.TaskDependencyList.Add(new TaskDependency
        {
            Id = "CustomerLoad.first.Before.CustomerLoad.second",
            Pipeline = pipeline,
            Predecessor = firstTask,
            Successor = secondTask,
        });
        model.TaskDependencyList.Add(new TaskDependency
        {
            Id = "CustomerLoad.first.Before.CustomerLoad.third",
            Pipeline = pipeline,
            Predecessor = firstTask,
            Successor = thirdTask,
        });

        var result = new MetaPipelineModelValidationService().ValidatePipeline(model, "CustomerLoad");

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, item => item.Contains("multiple successors", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidatePipeline_WhenTargetWriteHasNoInsertRowsDetail_ReturnsError()
    {
        var model = MetaPipelineModel.CreateEmpty();
        var pipeline = new Pipeline
        {
            Id = "CustomerLoad",
            Name = "CustomerLoad",
        };
        model.PipelineList.Add(pipeline);

        var source = new ConnectionReference
        {
            Id = "CustomerLoad.source",
            Pipeline = pipeline,
            Name = "source",
            EnvironmentVariableName = "SOURCE_ENV",
        };
        model.ConnectionReferenceList.Add(source);
        var target = new ConnectionReference
        {
            Id = "CustomerLoad.target",
            Pipeline = pipeline,
            Name = "target",
            EnvironmentVariableName = "TARGET_ENV",
        };
        model.ConnectionReferenceList.Add(target);

        var loadTask = new PipelineTask
        {
            Id = "CustomerLoad.load",
            Pipeline = pipeline,
            Name = "load",
        };
        model.PipelineTaskList.Add(loadTask);
        var targetWritePipelineTask = new PipelineTask
        {
            Id = "CustomerLoad.load.target-write",
            Pipeline = pipeline,
            Name = "load.target-write",
        };
        model.PipelineTaskList.Add(targetWritePipelineTask);

        model.TransformExecutionTaskList.Add(new TransformExecutionTask
        {
            Id = "CustomerLoad.load.TransformExecution",
            PipelineTask = loadTask,
            ExecutionConnectionReference = source,
            TransformScriptId = "TransformScript:1",
            TransformBindingId = "TransformScript:1:binding",
        });

        model.TargetWriteTaskList.Add(new TargetWriteTask
        {
            Id = "CustomerLoad.load.target-write.TargetWrite",
            PipelineTask = targetWritePipelineTask,
            TargetConnectionReference = target,
        });
        var rowStream = new RowStream
        {
            Id = "CustomerLoad.load.rows",
            Pipeline = pipeline,
            Name = "load.rows",
        };
        model.RowStreamList.Add(rowStream);
        model.RowStreamColumnList.Add(new RowStreamColumn
        {
            Id = "CustomerLoad.load.rows.CustomerId",
            RowStream = rowStream,
            Name = "CustomerId",
            Ordinal = "0",
        });
        model.RowStreamProducerList.Add(new RowStreamProducer
        {
            Id = "CustomerLoad.load.produces",
            PipelineTask = loadTask,
            RowStream = rowStream,
        });
        model.RowStreamConsumerList.Add(new RowStreamConsumer
        {
            Id = "CustomerLoad.load.target-write.consumes",
            PipelineTask = targetWritePipelineTask,
            RowStream = rowStream,
        });

        model.TaskDependencyList.Add(new TaskDependency
        {
            Id = "CustomerLoad.load.before.target-write",
            Pipeline = pipeline,
            Predecessor = loadTask,
            Successor = targetWritePipelineTask,
        });

        var result = new MetaPipelineModelValidationService().ValidatePipeline(model, "CustomerLoad");

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, item => item.Contains("must have exactly one detail row: InsertRowsTargetWriteTask", StringComparison.OrdinalIgnoreCase));
    }

    private static Pipeline AddPipeline(MetaPipelineModel model)
    {
        var pipeline = new Pipeline
        {
            Id = "CustomerLoad",
            Name = "CustomerLoad",
            Description = "Test",
        };
        model.PipelineList.Add(pipeline);
        return pipeline;
    }

    private static ConnectionReference AddConnection(
        MetaPipelineModel model,
        Pipeline pipeline)
    {
        var source = new ConnectionReference
        {
            Id = "CustomerLoad.source",
            Pipeline = pipeline,
            Name = "source",
            EnvironmentVariableName = "SOURCE_ENV",
        };
        model.ConnectionReferenceList.Add(source);
        return source;
    }

    private static PipelineTask AddTransformTask(
        MetaPipelineModel model,
        Pipeline pipeline,
        ConnectionReference source,
        string name,
        string token)
    {
        var task = new PipelineTask
        {
            Id = "CustomerLoad." + name,
            Pipeline = pipeline,
            Name = name,
        };
        model.PipelineTaskList.Add(task);
        model.TransformExecutionTaskList.Add(new TransformExecutionTask
        {
            Id = task.Id + ".TransformExecution",
            PipelineTask = task,
            ExecutionConnectionReference = source,
            TransformScriptId = "TransformScript:" + token,
            TransformBindingId = "binding:" + token,
        });
        return task;
    }

}
