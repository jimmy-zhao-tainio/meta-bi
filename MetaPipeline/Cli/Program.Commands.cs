using Meta.Core.Operations;
using Meta.Core.Presentation.Cli;

internal static partial class Program
{
    private static int RunNewWorkspace(string[] args, int startIndex)
    {
        var parse = ParseNewWorkspaceArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, $"{Cli.Name} --help");
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

    private static int RunAddPipeline(string[] args, int startIndex)
    {
        var parse = ParseAddPipelineArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("add-pipeline"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MetaPipeline.MetaPipelineModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            EnsurePipelineNameAvailable(model, parse.Name);

            var pipeline = new MetaPipeline.Pipeline
            {
                Id = NaturalId(parse.Name),
                Name = parse.Name.Trim(),
                Description = parse.Description.Trim(),
            };
            ApplyInstanceUpserts(
                workspacePath,
                CreateUpsertOperation(
                    "Pipeline",
                    CreateRowPatch(
                        pipeline.Id,
                        new Dictionary<string, string>
                        {
                            ["Name"] = pipeline.Name,
                            ["Description"] = pipeline.Description,
                        })));

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

    private static int RunInspect(string[] args, int startIndex)
    {
        var parse = ParseWorkspaceOnlyArgs(args, startIndex, "meta-pipeline inspect --help");
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("inspect"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MetaPipeline.MetaPipelineModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
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
                var tasks = model.PipelineTaskList
                    .Where(item => string.Equals(item.Pipeline.Id, pipeline.Id, StringComparison.Ordinal))
                    .OrderBy(static item => ParseOrdinalOrMax(item.Ordinal))
                    .ThenBy(static item => item.Name, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                Presenter.WriteKeyValueBlock($"Pipeline: {pipeline.Name}", new[]
                {
                    ("Id", pipeline.Id),
                    ("Tasks", tasks.Length.ToString()),
                });

                foreach (var task in tasks)
                {
                    Presenter.WriteInfo($"  {task.Ordinal}. {task.Name} [{ResolveTaskLabel(model, task)}]");
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

    private static int RunAddStep(string[] args, int startIndex)
    {
        var parse = ParseAddStepArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("add-step"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MetaPipeline.MetaPipelineModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
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
            var nextOrdinal = ResolveNextTaskOrdinal(model, pipeline);
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

                var mutationOperations = new List<WorkspaceOp>
                {
                    CreateUpsertOperation(
                        "ConnectionReference",
                        CreateRowPatch(
                            executionConnection.Id,
                            new Dictionary<string, string>
                            {
                                ["Name"] = executionConnection.Name,
                                ["EnvironmentVariableName"] = executionConnection.EnvironmentVariableName,
                            },
                            new Dictionary<string, string>
                            {
                                ["PipelineId"] = pipeline.Id,
                            })),
                    CreateUpsertOperation(
                        "PipelineTask",
                        CreateRowPatch(
                            transformTaskId,
                            new Dictionary<string, string>
                            {
                                ["Name"] = transformTaskName,
                                ["Ordinal"] = nextOrdinal.ToString(),
                            },
                            new Dictionary<string, string>
                            {
                                ["PipelineId"] = pipeline.Id,
                            })),
                    CreateUpsertOperation(
                        "TransformExecutionTask",
                        CreateRowPatch(
                            ScopedId(transformTaskId, "TransformExecution"),
                            new Dictionary<string, string>
                            {
                                ["TransformScriptId"] = executionDefinition.TransformScriptId,
                                ["TransformBindingId"] = executionDefinition.TransformBindingId
                                    ?? throw new MetaPipeline.MetaPipelineConfigurationException("Transform execution requires a transform binding."),
                                ["TimeoutSeconds"] = parse.TimeoutSecondsSpecified ? parse.TimeoutSeconds!.Value.ToString() : string.Empty,
                            },
                            new Dictionary<string, string>
                            {
                                ["PipelineTaskId"] = transformTaskId,
                                ["ExecutionConnectionReferenceId"] = executionConnection.Id,
                            })),
                };

                if (previousTerminalTask is not null)
                {
                    mutationOperations.Add(CreateSerialDependencyOperation(pipeline, previousTerminalTask.Id, transformTaskId));
                }

                ApplyInstanceUpserts(workspacePath, mutationOperations);

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

            var operations = new List<WorkspaceOp>
            {
                CreateUpsertOperation(
                    "ConnectionReference",
                    CreateRowPatch(
                        executionConnection.Id,
                        new Dictionary<string, string>
                        {
                            ["Name"] = executionConnection.Name,
                            ["EnvironmentVariableName"] = executionConnection.EnvironmentVariableName,
                        },
                        new Dictionary<string, string>
                        {
                            ["PipelineId"] = pipeline.Id,
                        })),
                CreateUpsertOperation(
                    "ConnectionReference",
                    CreateRowPatch(
                        targetConnection.Id,
                        new Dictionary<string, string>
                        {
                            ["Name"] = targetConnection.Name,
                            ["EnvironmentVariableName"] = targetConnection.EnvironmentVariableName,
                        },
                        new Dictionary<string, string>
                        {
                            ["PipelineId"] = pipeline.Id,
                        })),
                CreateUpsertOperation(
                    "PipelineTask",
                    CreateRowPatch(
                        transformTaskId,
                        new Dictionary<string, string>
                        {
                            ["Name"] = transformTaskName,
                            ["Ordinal"] = nextOrdinal.ToString(),
                        },
                        new Dictionary<string, string>
                        {
                            ["PipelineId"] = pipeline.Id,
                        })),
                CreateUpsertOperation(
                    "TransformExecutionTask",
                    CreateRowPatch(
                        ScopedId(transformTaskId, "TransformExecution"),
                        new Dictionary<string, string>
                        {
                            ["TransformScriptId"] = executionDefinition.TransformScriptId,
                            ["TransformBindingId"] = transformBindingId,
                            ["TimeoutSeconds"] = parse.TimeoutSecondsSpecified ? parse.TimeoutSeconds!.Value.ToString() : string.Empty,
                        },
                        new Dictionary<string, string>
                        {
                            ["PipelineTaskId"] = transformTaskId,
                            ["ExecutionConnectionReferenceId"] = executionConnection.Id,
                        })),
                CreateUpsertOperation(
                    "RowStream",
                    CreateRowPatch(
                        rowStreamId,
                        new Dictionary<string, string>
                        {
                            ["Name"] = rowStreamName,
                        },
                        new Dictionary<string, string>
                        {
                            ["PipelineId"] = pipeline.Id,
                        })),
            };

            foreach (var column in rowStreamShape.Columns)
            {
                operations.Add(
                    CreateUpsertOperation(
                        "RowStreamColumn",
                        CreateRowPatch(
                            ScopedId(rowStreamId, column.Name),
                            new Dictionary<string, string>
                            {
                                ["Name"] = column.Name,
                                ["Ordinal"] = column.Ordinal.ToString(),
                            },
                            new Dictionary<string, string>
                            {
                                ["RowStreamId"] = rowStreamId,
                            })));
            }

            var targetWriteDetailId = ScopedId(targetWriteTaskId, "TargetWrite");
            operations.Add(
                CreateUpsertOperation(
                    "RowStreamProducer",
                    CreateRowPatch(
                        ScopedId(transformTaskId, "Produces", rowStreamId),
                        relationships: new Dictionary<string, string>
                        {
                            ["PipelineTaskId"] = transformTaskId,
                            ["RowStreamId"] = rowStreamId,
                        })));

            operations.Add(
                CreateUpsertOperation(
                    "PipelineTask",
                    CreateRowPatch(
                        targetWriteTaskId,
                        new Dictionary<string, string>
                        {
                            ["Name"] = targetWriteTaskName,
                            ["Ordinal"] = (nextOrdinal + 1).ToString(),
                        },
                        new Dictionary<string, string>
                        {
                            ["PipelineId"] = pipeline.Id,
                        })));
            operations.Add(
                CreateUpsertOperation(
                    "TargetWriteTask",
                    CreateRowPatch(
                        targetWriteDetailId,
                        relationships: new Dictionary<string, string>
                        {
                            ["PipelineTaskId"] = targetWriteTaskId,
                            ["TargetConnectionReferenceId"] = targetConnection.Id,
                        })));
            operations.Add(
                CreateUpsertOperation(
                    "InsertRowsTargetWriteTask",
                    CreateRowPatch(
                        ScopedId(targetWriteDetailId, "InsertRows"),
                        new Dictionary<string, string>
                        {
                            ["TargetSqlIdentifier"] = targetSqlIdentifier,
                            ["BatchSize"] = parse.BatchSizeSpecified ? parse.BatchSize.ToString() : string.Empty,
                            ["TargetDataTypeSystemName"] = parse.TargetDataTypeSystemSpecified ? parse.TargetDataTypeSystemName.Trim() : string.Empty,
                        },
                        new Dictionary<string, string>
                        {
                            ["TargetWriteTaskId"] = targetWriteDetailId,
                        })));

            operations.Add(
                CreateUpsertOperation(
                    "RowStreamConsumer",
                    CreateRowPatch(
                        ScopedId(targetWriteTaskId, "Consumes", rowStreamId),
                        relationships: new Dictionary<string, string>
                        {
                            ["PipelineTaskId"] = targetWriteTaskId,
                            ["RowStreamId"] = rowStreamId,
                        })));
            if (previousTerminalTask is not null)
            {
                operations.Add(CreateSerialDependencyOperation(pipeline, previousTerminalTask.Id, transformTaskId));
            }

            operations.Add(
                CreateUpsertOperation(
                    "TaskDependency",
                    CreateRowPatch(
                        ScopedId(transformTaskId, "Before", targetWriteTaskId),
                        relationships: new Dictionary<string, string>
                        {
                            ["PipelineId"] = pipeline.Id,
                            ["PredecessorId"] = transformTaskId,
                            ["SuccessorId"] = targetWriteTaskId,
                        })));

            ApplyInstanceUpserts(workspacePath, operations);

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
