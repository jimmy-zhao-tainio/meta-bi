using Meta.Core.Operations;
using MetaBi.Cli.Common;

internal static partial class Program
{
    private static int RunInit(string[] args, int startIndex)
    {
        var parse = ParseInitArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-pipeline init --help");
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

        Presenter.WriteOk($"Created {Path.GetFileName(targetValidation.FullPath)}");
        Presenter.WriteKeyValueBlock("MetaPipeline", new[]
        {
            ("Workspace", targetValidation.FullPath),
            ("Pipelines", "0"),
            ("Tasks", "0"),
        });
        return 0;
    }

    private static int RunAddPipeline(string[] args, int startIndex)
    {
        var parse = ParseAddPipelineArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-pipeline add-pipeline --help");
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

            Presenter.WriteOk("Added pipeline");
            Presenter.WriteKeyValueBlock("MetaPipeline", new[]
            {
                ("Workspace", workspacePath),
                ("Pipeline", pipeline.Name),
                ("Tasks", "0"),
            });
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "MetaPipeline workspace update failed.",
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
            return Fail(parse.ErrorMessage, "meta-pipeline inspect --help");
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MetaPipeline.MetaPipelineModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            Presenter.WriteOk("Loaded MetaPipeline workspace");
            Presenter.WriteKeyValueBlock("MetaPipeline", new[]
            {
                ("Workspace", workspacePath),
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
                    .Where(item => string.Equals(item.PipelineId, pipeline.Id, StringComparison.Ordinal))
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
                "MetaPipeline workspace inspection failed.",
                "check the workspace path and instance data integrity, then retry.",
                4,
                new[] { $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}", $"  {ex.Message}" });
        }
    }

    private static int RunAddTransform(string[] args, int startIndex)
    {
        var parse = ParseAddTransformArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, "meta-pipeline add-transform --help");
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MetaPipeline.MetaPipelineModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            var pipeline = ResolvePipeline(model, parse.PipelineName);
            var executionDefinition = new MetaPipeline.MetaPipelineExecutionWorkspaceResolver().ResolveByIds(
                parse.TransformWorkspacePath,
                parse.BindingWorkspacePath,
                parse.TransformScriptId,
                parse.TransformBindingId,
                parse.TargetSqlIdentifier);

            var sourceConnection = GetOrAddConnectionReference(
                model,
                pipeline,
                parse.SourceConnectionReferenceName,
                parse.SourceConnectionEnvironmentVariableName);
            var targetConnection = GetOrAddConnectionReference(
                model,
                pipeline,
                parse.TargetConnectionReferenceName,
                parse.TargetConnectionEnvironmentVariableName);
            if (string.Equals(sourceConnection.Name, targetConnection.Name, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(sourceConnection.EnvironmentVariableName, targetConnection.EnvironmentVariableName, StringComparison.Ordinal))
            {
                throw new MetaPipeline.MetaPipelineConfigurationException(
                    $"Connection reference '{sourceConnection.Name}' cannot point to both environment variable '{sourceConnection.EnvironmentVariableName}' and '{targetConnection.EnvironmentVariableName}'.");
            }

            var taskBaseName = parse.TaskName.Trim();
            var transformTaskName = taskBaseName;
            var targetWriteTaskName = taskBaseName + ".target-write";
            EnsureTaskNameAvailable(model, pipeline, transformTaskName);
            EnsureTaskNameAvailable(model, pipeline, targetWriteTaskName);

            var nextOrdinal = ResolveNextTaskOrdinal(model, pipeline);
            var transformTaskId = ScopedId(pipeline.Id, transformTaskName);
            var targetWriteTaskId = ScopedId(pipeline.Id, targetWriteTaskName);
            var rowStreamName = taskBaseName + ".rows";
            var rowStreamId = ScopedId(pipeline.Id, rowStreamName);
            var operations = new List<WorkspaceOp>
            {
                CreateUpsertOperation(
                    "ConnectionReference",
                    CreateRowPatch(
                        sourceConnection.Id,
                        new Dictionary<string, string>
                        {
                            ["Name"] = sourceConnection.Name,
                            ["EnvironmentVariableName"] = sourceConnection.EnvironmentVariableName,
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
                            ["TransformBindingId"] = executionDefinition.TransformBindingId,
                        },
                        new Dictionary<string, string>
                        {
                            ["PipelineTaskId"] = transformTaskId,
                            ["SourceConnectionReferenceId"] = sourceConnection.Id,
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

            foreach (var column in executionDefinition.RowStreamShape.Columns)
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
            operations.AddRange(new[]
            {
                CreateUpsertOperation(
                    "RowStreamProducer",
                    CreateRowPatch(
                        ScopedId(transformTaskId, "Produces", rowStreamId),
                        relationships: new Dictionary<string, string>
                        {
                            ["PipelineTaskId"] = transformTaskId,
                            ["RowStreamId"] = rowStreamId,
                        })),
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
                        })),
                CreateUpsertOperation(
                    "TargetWriteTask",
                    CreateRowPatch(
                        targetWriteDetailId,
                        relationships: new Dictionary<string, string>
                        {
                            ["PipelineTaskId"] = targetWriteTaskId,
                            ["TargetConnectionReferenceId"] = targetConnection.Id,
                        })),
                CreateUpsertOperation(
                    "InsertRowsTargetWriteTask",
                    CreateRowPatch(
                        ScopedId(targetWriteDetailId, "InsertRows"),
                        new Dictionary<string, string>
                        {
                            ["TargetSqlIdentifier"] = executionDefinition.TargetSqlIdentifier,
                            ["BatchSize"] = parse.BatchSizeSpecified ? parse.BatchSize.ToString() : string.Empty,
                        },
                        new Dictionary<string, string>
                        {
                            ["TargetWriteTaskId"] = targetWriteDetailId,
                        })),
                CreateUpsertOperation(
                    "RowStreamConsumer",
                    CreateRowPatch(
                        ScopedId(targetWriteTaskId, "Consumes", rowStreamId),
                        relationships: new Dictionary<string, string>
                        {
                            ["PipelineTaskId"] = targetWriteTaskId,
                            ["RowStreamId"] = rowStreamId,
                        })),
                CreateUpsertOperation(
                    "TaskDependency",
                    CreateRowPatch(
                        ScopedId(transformTaskId, "Before", targetWriteTaskId),
                        relationships: new Dictionary<string, string>
                        {
                            ["PipelineId"] = pipeline.Id,
                            ["PredecessorId"] = transformTaskId,
                            ["SuccessorId"] = targetWriteTaskId,
                        })),
            });

            ApplyInstanceUpserts(workspacePath, operations);

            Presenter.WriteOk("Added transform pipeline tasks");
            Presenter.WriteKeyValueBlock("MetaPipeline", new[]
            {
                ("Workspace", workspacePath),
                ("Pipeline", pipeline.Name),
                ("TransformTask", transformTaskName),
                ("TargetWriteTask", targetWriteTaskName),
                ("TargetWrite", "InsertRows"),
                ("RowStream", rowStreamName),
                ("Columns", executionDefinition.RowStreamShape.ColumnCount.ToString()),
            });
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "MetaPipeline workspace update failed.",
                "check the pipeline name and task inputs, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}",
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  BindingWorkspace: {Path.GetFullPath(parse.BindingWorkspacePath)}",
                    $"  {ex.Message}",
                });
        }
    }
}
