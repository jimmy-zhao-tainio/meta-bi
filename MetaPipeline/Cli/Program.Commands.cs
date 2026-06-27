using Meta.Core.Presentation.Cli;
using MetaCli.Core;

internal static partial class Program
{
    private static int RunNewWorkspace(MetaCliInvocation invocation)
    {
        var parse = ReadNewWorkspaceArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("new-workspace"));
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Fail(
                targetValidation.ErrorMessage,
                "choose a new folder or empty the target directory and retry.",
                4,
                targetValidation.Details);
        }

        var model = MetaPipeline.MetaPipelineModel.CreateEmpty();
        model.SaveToXmlWorkspace(targetValidation.FullPath);

        Presenter.WriteOk();
        return 0;
    }

    private static int RunAddPipeline(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model)
    {
        var parse = ReadAddPipelineArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("add-pipeline"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            EnsurePipelineNameAvailable(model, parse.Name);

            var pipeline = new MetaPipeline.Pipeline
            {
                Id = NaturalId(parse.Name),
                Name = parse.Name.Trim(),
                Description = parse.Description.Trim(),
            };
            model.PipelineList.Add(pipeline);
            model.SaveToXmlWorkspace(workspacePath);

            Presenter.WriteOk();
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot update pipeline workspace.",
                "check the workspace path and pipeline name, then retry.",
                4,
                new[] { $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}", $"  {ex.Message}" });
        }
    }

    private static int RunInspect(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model)
    {
        var parse = ReadWorkspaceOnlyArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("inspect"));
        }

        try
        {
            Presenter.WriteOk("Loaded MetaPipeline workspace");
            Presenter.WriteKeyValueBlock("MetaPipeline", new[]
            {
                ("Pipelines", model.PipelineList.Count.ToString()),
                ("Tasks", model.PipelineTaskList.Count.ToString()),
                ("Connections", model.ConnectionReferenceList.Count.ToString()),
                ("RowStreams", model.RowStreamList.Count.ToString()),
                ("RowStreamColumns", model.RowStreamColumnList.Count.ToString()),
                ("Dependencies", model.TaskDependencyList.Count.ToString()),
            });

            foreach (var pipeline in model.PipelineList.OrderBy(static item => item.Name, StringComparer.OrdinalIgnoreCase))
            {
                var tasks = ResolveOrderedPipelineTasks(model, pipeline);

                Presenter.WriteKeyValueBlock($"Pipeline: {pipeline.Name}", new[]
                {
                    ("Id", pipeline.Id),
                    ("Tasks", tasks.Count.ToString()),
                });

                foreach (var task in tasks)
                {
                    Presenter.WriteInfo($"  {task.Name} [{ResolveTaskLabel(model, task)}]");
                }
            }

            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot inspect pipeline workspace.",
                "check the workspace path and instance data integrity, then retry.",
                4,
                new[] { $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}", $"  {ex.Message}" });
        }
    }

    private static int RunAddExecutableStep(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model)
    {
        var parse = ReadAddExecutableStepArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("add-executable-step"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var pipeline = ResolvePipeline(model, parse.PipelineName);
            var taskName = ResolveExecutableStepName(parse.StepName, parse.ExecutablePath);
            EnsureTaskNameAvailable(model, pipeline, taskName);
            var taskId = ScopedId(pipeline.Id, taskName);
            var previousTerminalTask = ResolveCurrentTerminalTask(model, pipeline);

            var task = new MetaPipeline.PipelineTask
            {
                Id = taskId,
                Pipeline = pipeline,
                Name = taskName,
            };
            model.PipelineTaskList.Add(task);
            model.ExecutableTaskList.Add(new MetaPipeline.ExecutableTask
            {
                Id = ScopedId(taskId, "Executable"),
                PipelineTask = task,
                ExecutablePath = parse.ExecutablePath.Trim(),
                Arguments = parse.Arguments.Trim(),
                WorkingDirectory = parse.WorkingDirectory.Trim(),
                SuccessExitCode = parse.SuccessExitCodeSpecified ? parse.SuccessExitCode.ToString() : null,
                TimeoutSeconds = parse.TimeoutSecondsSpecified ? parse.TimeoutSeconds!.Value.ToString() : null,
            });

            if (previousTerminalTask is not null)
            {
                AddSerialDependency(model, pipeline, previousTerminalTask, task);
            }

            model.SaveToXmlWorkspace(workspacePath);

            Presenter.WriteOk();
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot update pipeline workspace.",
                "check the pipeline name and executable task inputs, then retry.",
                4,
                new[] { $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}", $"  {ex.Message}" });
        }
    }

    private static int RunAddStep(MetaCliInvocation invocation, MetaPipeline.MetaPipelineModel model)
    {
        var parse = ReadAddStepArgs(invocation);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("add-step"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var pipeline = ResolvePipeline(model, parse.PipelineName);
            var selection = new MetaPipeline.MetaPipelineTransformSelectionResolver().Resolve(
                parse.TransformWorkspacePath,
                parse.BindingWorkspacePath,
                parse.Script,
                parse.Binding);
            var executionDefinition = new MetaPipeline.MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                parse.TransformWorkspacePath,
                parse.BindingWorkspacePath,
                selection.TransformScriptId,
                selection.TransformBindingId,
                parse.TargetSqlIdentifier);

            var executionConnection = GetOrAddConnectionReference(
                model,
                pipeline,
                parse.ExecutionConnectionEnvironmentVariableName,
                parse.ExecutionConnectionEnvironmentVariableName);
            var taskBaseName = ResolveStepName(parse.StepName, executionDefinition.TransformScriptName);
            var transformTaskName = taskBaseName;
            EnsureTaskNameAvailable(model, pipeline, transformTaskName);
            var transformTaskId = ScopedId(pipeline.Id, transformTaskName);
            var previousTerminalTask = ResolveCurrentTerminalTask(model, pipeline);

            if (!executionDefinition.IsSelect)
            {
                if (!string.IsNullOrWhiteSpace(parse.TargetConnectionEnvironmentVariableName)
                    || parse.TargetWriteModelSpecified
                    || parse.BatchSizeSpecified
                    || parse.TargetDataTypeSystemSpecified)
                {
                    throw new MetaPipeline.MetaPipelineConfigurationException(
                        $"Transform script '{executionDefinition.TransformScriptName}' is not SELECT-kind and cannot use target connection, target write, batch-size, or target-data-type-system options.");
                }

                var mutationTransformTask = new MetaPipeline.PipelineTask
                {
                    Id = transformTaskId,
                    Pipeline = pipeline,
                    Name = transformTaskName,
                };
                model.PipelineTaskList.Add(mutationTransformTask);
                model.TransformExecutionTaskList.Add(new MetaPipeline.TransformExecutionTask
                {
                    Id = ScopedId(transformTaskId, "TransformExecution"),
                    PipelineTask = mutationTransformTask,
                    ExecutionConnectionReference = executionConnection,
                    TransformScriptId = executionDefinition.TransformScriptId,
                    TransformBindingId = executionDefinition.TransformBindingId
                        ?? throw new MetaPipeline.MetaPipelineConfigurationException("Transform execution requires a transform binding."),
                    TransformWorkspacePath = Path.GetFullPath(parse.TransformWorkspacePath),
                    BindingWorkspacePath = Path.GetFullPath(parse.BindingWorkspacePath),
                    TimeoutSeconds = parse.TimeoutSecondsSpecified ? parse.TimeoutSeconds!.Value.ToString() : null,
                });

                if (previousTerminalTask is not null)
                {
                    AddSerialDependency(model, pipeline, previousTerminalTask, mutationTransformTask);
                }

                model.SaveToXmlWorkspace(workspacePath);

                Presenter.WriteOk();
                return 0;
            }

            if (string.IsNullOrWhiteSpace(parse.TargetConnectionEnvironmentVariableName))
            {
                throw new MetaPipeline.MetaPipelineConfigurationException(
                    "SELECT-kind transform tasks require --target-connection-env <name>.");
            }

            var targetConnection = GetOrAddConnectionReference(
                model,
                pipeline,
                parse.TargetConnectionEnvironmentVariableName,
                parse.TargetConnectionEnvironmentVariableName);
            if (string.Equals(executionConnection.Name, targetConnection.Name, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(executionConnection.EnvironmentVariableName, targetConnection.EnvironmentVariableName, StringComparison.Ordinal))
            {
                throw new MetaPipeline.MetaPipelineConfigurationException(
                    $"Connection reference '{executionConnection.Name}' cannot point to both environment variable '{executionConnection.EnvironmentVariableName}' and '{targetConnection.EnvironmentVariableName}'.");
            }

            var targetWriteTaskName = taskBaseName + ".target-write";
            EnsureTaskNameAvailable(model, pipeline, targetWriteTaskName);

            var targetWriteTaskId = ScopedId(pipeline.Id, targetWriteTaskName);
            var rowStreamName = taskBaseName + ".rows";
            var rowStreamId = ScopedId(pipeline.Id, rowStreamName);
            var targetSqlIdentifier = RequireValue(
                executionDefinition.TargetSqlIdentifier,
                "SELECT-kind transform tasks require a target SQL identifier.");
            var transformBindingId = RequireValue(
                executionDefinition.TransformBindingId,
                "SELECT-kind transform tasks require a transform binding.");
            var rowStreamShape = executionDefinition.RowStreamShape
                ?? throw new MetaPipeline.MetaPipelineConfigurationException(
                    "SELECT-kind transform tasks require a resolved row-stream shape.");
            _ = MetaPipeline.SqlServerMultipartIdentifier.Parse(targetSqlIdentifier);

            var transformTask = new MetaPipeline.PipelineTask
            {
                Id = transformTaskId,
                Pipeline = pipeline,
                Name = transformTaskName,
            };
            model.PipelineTaskList.Add(transformTask);
            model.TransformExecutionTaskList.Add(new MetaPipeline.TransformExecutionTask
            {
                Id = ScopedId(transformTaskId, "TransformExecution"),
                PipelineTask = transformTask,
                ExecutionConnectionReference = executionConnection,
                TransformScriptId = executionDefinition.TransformScriptId,
                TransformBindingId = transformBindingId,
                TransformWorkspacePath = Path.GetFullPath(parse.TransformWorkspacePath),
                BindingWorkspacePath = Path.GetFullPath(parse.BindingWorkspacePath),
                TimeoutSeconds = parse.TimeoutSecondsSpecified ? parse.TimeoutSeconds!.Value.ToString() : null,
            });

            var rowStream = new MetaPipeline.RowStream
            {
                Id = rowStreamId,
                Pipeline = pipeline,
                Name = rowStreamName,
            };
            model.RowStreamList.Add(rowStream);

            foreach (var column in rowStreamShape.Columns)
            {
                model.RowStreamColumnList.Add(new MetaPipeline.RowStreamColumn
                {
                    Id = ScopedId(rowStreamId, column.Name),
                    RowStream = rowStream,
                    Name = column.Name,
                    Ordinal = column.Ordinal.ToString(),
                });
            }

            var targetWriteDetailId = ScopedId(targetWriteTaskId, "TargetWrite");
            model.RowStreamProducerList.Add(new MetaPipeline.RowStreamProducer
            {
                Id = ScopedId(transformTaskId, "Produces", rowStreamId),
                PipelineTask = transformTask,
                RowStream = rowStream,
            });

            var targetWritePipelineTask = new MetaPipeline.PipelineTask
            {
                Id = targetWriteTaskId,
                Pipeline = pipeline,
                Name = targetWriteTaskName,
            };
            model.PipelineTaskList.Add(targetWritePipelineTask);
            var targetWriteTask = new MetaPipeline.TargetWriteTask
            {
                Id = targetWriteDetailId,
                PipelineTask = targetWritePipelineTask,
                TargetConnectionReference = targetConnection,
            };
            model.TargetWriteTaskList.Add(targetWriteTask);
            model.InsertRowsTargetWriteTaskList.Add(new MetaPipeline.InsertRowsTargetWriteTask
            {
                Id = ScopedId(targetWriteDetailId, "InsertRows"),
                TargetWriteTask = targetWriteTask,
                TargetSqlIdentifier = targetSqlIdentifier,
                BatchSize = parse.BatchSizeSpecified ? parse.BatchSize.ToString() : null,
                TargetDataTypeSystemName = parse.TargetDataTypeSystemSpecified ? parse.TargetDataTypeSystemName.Trim() : null,
            });

            model.RowStreamConsumerList.Add(new MetaPipeline.RowStreamConsumer
            {
                Id = ScopedId(targetWriteTaskId, "Consumes", rowStreamId),
                PipelineTask = targetWritePipelineTask,
                RowStream = rowStream,
            });
            if (previousTerminalTask is not null)
            {
                AddSerialDependency(model, pipeline, previousTerminalTask, transformTask);
            }

            AddSerialDependency(model, pipeline, transformTask, targetWritePipelineTask);

            model.SaveToXmlWorkspace(workspacePath);

            Presenter.WriteOk();
            return 0;
        }
        catch (Exception ex)
        {
            var details = new List<string>
            {
                $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}",
                $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
            };
            if (!string.IsNullOrWhiteSpace(parse.BindingWorkspacePath))
            {
                details.Add($"  BindingWorkspace: {Path.GetFullPath(parse.BindingWorkspacePath)}");
            }

            details.Add($"  {ex.Message}");
            return Fail(
                "Cannot update pipeline workspace.",
                "check the pipeline name and task inputs, then retry.",
                4,
                details);
        }
    }

    private static string RequireValue(string? value, string errorMessage) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new MetaPipeline.MetaPipelineConfigurationException(errorMessage)
            : value.Trim();

    private static string ResolveStepName(string stepName, string transformScriptName)
    {
        if (!string.IsNullOrWhiteSpace(stepName))
        {
            return stepName.Trim();
        }

        var derived = DeriveStepName(transformScriptName);
        if (string.IsNullOrWhiteSpace(derived))
        {
            throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Transform script '{transformScriptName}' cannot be used to derive a step name. Use --step-name <name>.");
        }

        return derived;
    }

    private static string ResolveExecutableStepName(string stepName, string executablePath)
    {
        if (!string.IsNullOrWhiteSpace(stepName))
        {
            return stepName.Trim();
        }

        var fileName = Path.GetFileNameWithoutExtension(executablePath.Trim());
        var derived = DeriveStepName(fileName);
        if (string.IsNullOrWhiteSpace(derived))
        {
            throw new MetaPipeline.MetaPipelineConfigurationException(
                $"Executable path '{executablePath}' cannot be used to derive a step name. Use --step-name <name>.");
        }

        return derived;
    }

    private static string DeriveStepName(string value)
    {
        var output = new char[value.Length];
        var length = 0;
        var previousWasSeparator = false;

        foreach (var character in value.Trim())
        {
            if (char.IsLetterOrDigit(character))
            {
                output[length++] = char.ToLowerInvariant(character);
                previousWasSeparator = false;
                continue;
            }

            if (length > 0 && !previousWasSeparator)
            {
                output[length++] = '-';
                previousWasSeparator = true;
            }
        }

        while (length > 0 && output[length - 1] == '-')
        {
            length--;
        }

        return length == 0 ? string.Empty : new string(output, 0, length);
    }

}
