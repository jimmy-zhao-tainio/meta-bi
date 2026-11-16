// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaPipeline;
public sealed class MetaPipelineWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaPipeline.MetaPipelineModel>
{
    private const string ContractSignature = "6d6f64656c7c4d657461506970656c696e650a656e746974797c436f6e6e656374696f6e5265666572656e63657c436f6e6e656374696f6e5265666572656e63654c6973740a70726f70657274797c436f6e6e656374696f6e5265666572656e63657c456e7669726f6e6d656e745661726961626c654e616d657c72657175697265640a70726f70657274797c436f6e6e656374696f6e5265666572656e63657c4e616d657c72657175697265640a72656c6174696f6e736869707c436f6e6e656374696f6e5265666572656e63657c506970656c696e657c506970656c696e6549647c72657175697265640a656e746974797c45786563757461626c655461736b7c45786563757461626c655461736b4c6973740a70726f70657274797c45786563757461626c655461736b7c417267756d656e74737c6e756c6c61626c650a70726f70657274797c45786563757461626c655461736b7c45786563757461626c65506174687c72657175697265640a70726f70657274797c45786563757461626c655461736b7c5375636365737345786974436f64657c6e756c6c61626c650a70726f70657274797c45786563757461626c655461736b7c54696d656f75745365636f6e64737c6e756c6c61626c650a70726f70657274797c45786563757461626c655461736b7c576f726b696e674469726563746f72797c6e756c6c61626c650a72656c6174696f6e736869707c45786563757461626c655461736b7c506970656c696e655461736b7c506970656c696e655461736b49647c72657175697265640a656e746974797c496e73657274526f777354617267657457726974655461736b7c496e73657274526f777354617267657457726974655461736b4c6973740a70726f70657274797c496e73657274526f777354617267657457726974655461736b7c426174636853697a657c6e756c6c61626c650a70726f70657274797c496e73657274526f777354617267657457726974655461736b7c546172676574446174615479706553797374656d4e616d657c6e756c6c61626c650a70726f70657274797c496e73657274526f777354617267657457726974655461736b7c54617267657453716c4964656e7469666965727c72657175697265640a72656c6174696f6e736869707c496e73657274526f777354617267657457726974655461736b7c54617267657457726974655461736b7c54617267657457726974655461736b49647c72657175697265640a656e746974797c506970656c696e657c506970656c696e654c6973740a70726f70657274797c506970656c696e657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c506970656c696e657c4e616d657c72657175697265640a656e746974797c506970656c696e655461736b7c506970656c696e655461736b4c6973740a70726f70657274797c506970656c696e655461736b7c4e616d657c72657175697265640a72656c6174696f6e736869707c506970656c696e655461736b7c506970656c696e657c506970656c696e6549647c72657175697265640a656e746974797c526f7753747265616d7c526f7753747265616d4c6973740a70726f70657274797c526f7753747265616d7c4e616d657c72657175697265640a72656c6174696f6e736869707c526f7753747265616d7c506970656c696e657c506970656c696e6549647c72657175697265640a656e746974797c526f7753747265616d436f6c756d6e7c526f7753747265616d436f6c756d6e4c6973740a70726f70657274797c526f7753747265616d436f6c756d6e7c4e616d657c72657175697265640a70726f70657274797c526f7753747265616d436f6c756d6e7c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c526f7753747265616d436f6c756d6e7c526f7753747265616d7c526f7753747265616d49647c72657175697265640a656e746974797c526f7753747265616d436f6e73756d65727c526f7753747265616d436f6e73756d65724c6973740a72656c6174696f6e736869707c526f7753747265616d436f6e73756d65727c506970656c696e655461736b7c506970656c696e655461736b49647c72657175697265640a72656c6174696f6e736869707c526f7753747265616d436f6e73756d65727c526f7753747265616d7c526f7753747265616d49647c72657175697265640a656e746974797c526f7753747265616d50726f64756365727c526f7753747265616d50726f64756365724c6973740a72656c6174696f6e736869707c526f7753747265616d50726f64756365727c506970656c696e655461736b7c506970656c696e655461736b49647c72657175697265640a72656c6174696f6e736869707c526f7753747265616d50726f64756365727c526f7753747265616d7c526f7753747265616d49647c72657175697265640a656e746974797c54617267657457726974655461736b7c54617267657457726974655461736b4c6973740a72656c6174696f6e736869707c54617267657457726974655461736b7c506970656c696e655461736b7c506970656c696e655461736b49647c72657175697265640a72656c6174696f6e736869707c54617267657457726974655461736b7c436f6e6e656374696f6e5265666572656e63657c546172676574436f6e6e656374696f6e5265666572656e636549647c72657175697265640a656e746974797c5461736b446570656e64656e63797c5461736b446570656e64656e63794c6973740a72656c6174696f6e736869707c5461736b446570656e64656e63797c506970656c696e657c506970656c696e6549647c72657175697265640a72656c6174696f6e736869707c5461736b446570656e64656e63797c506970656c696e655461736b7c5072656465636573736f7249647c72657175697265640a72656c6174696f6e736869707c5461736b446570656e64656e63797c506970656c696e655461736b7c537563636573736f7249647c72657175697265640a656e746974797c5472616e73666f726d457865637574696f6e5461736b7c5472616e73666f726d457865637574696f6e5461736b4c6973740a70726f70657274797c5472616e73666f726d457865637574696f6e5461736b7c42696e64696e67576f726b7370616365506174687c72657175697265640a70726f70657274797c5472616e73666f726d457865637574696f6e5461736b7c54696d656f75745365636f6e64737c6e756c6c61626c650a70726f70657274797c5472616e73666f726d457865637574696f6e5461736b7c5472616e73666f726d42696e64696e6749647c72657175697265640a70726f70657274797c5472616e73666f726d457865637574696f6e5461736b7c5472616e73666f726d53637269707449647c72657175697265640a70726f70657274797c5472616e73666f726d457865637574696f6e5461736b7c5472616e73666f726d576f726b7370616365506174687c72657175697265640a72656c6174696f6e736869707c5472616e73666f726d457865637574696f6e5461736b7c436f6e6e656374696f6e5265666572656e63657c457865637574696f6e436f6e6e656374696f6e5265666572656e636549647c72657175697265640a72656c6174696f6e736869707c5472616e73666f726d457865637574696f6e5461736b7c506970656c696e655461736b7c506970656c696e655461736b49647c7265717569726564";
    public global::MetaPipeline.MetaPipelineModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaPipeline.MetaPipelineModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.ConnectionReference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ConnectionReference"))
        {
            var row = new global::MetaPipeline.ConnectionReference
            {
                Id = source.Id
            };
            row.EnvironmentVariableName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "EnvironmentVariableName");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.ConnectionReferenceList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.ExecutableTask>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ExecutableTask"))
        {
            var row = new global::MetaPipeline.ExecutableTask
            {
                Id = source.Id
            };
            row.Arguments = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Arguments");
            row.ExecutablePath = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ExecutablePath");
            row.SuccessExitCode = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "SuccessExitCode");
            row.TimeoutSeconds = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TimeoutSeconds");
            row.WorkingDirectory = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "WorkingDirectory");
            typed.ExecutableTaskList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.InsertRowsTargetWriteTask>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "InsertRowsTargetWriteTask"))
        {
            var row = new global::MetaPipeline.InsertRowsTargetWriteTask
            {
                Id = source.Id
            };
            row.BatchSize = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "BatchSize");
            row.TargetDataTypeSystemName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TargetDataTypeSystemName");
            row.TargetSqlIdentifier = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TargetSqlIdentifier");
            typed.InsertRowsTargetWriteTaskList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.Pipeline>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Pipeline"))
        {
            var row = new global::MetaPipeline.Pipeline
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.PipelineList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.PipelineTask>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PipelineTask"))
        {
            var row = new global::MetaPipeline.PipelineTask
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.PipelineTaskList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.RowStream>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RowStream"))
        {
            var row = new global::MetaPipeline.RowStream
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.RowStreamList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.RowStreamColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RowStreamColumn"))
        {
            var row = new global::MetaPipeline.RowStreamColumn
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.RowStreamColumnList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.RowStreamConsumer>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RowStreamConsumer"))
        {
            var row = new global::MetaPipeline.RowStreamConsumer
            {
                Id = source.Id
            };
            typed.RowStreamConsumerList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.RowStreamProducer>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RowStreamProducer"))
        {
            var row = new global::MetaPipeline.RowStreamProducer
            {
                Id = source.Id
            };
            typed.RowStreamProducerList.Add(row);
            index8.Add(source.Id, row);
        }

        var index9 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.TargetWriteTask>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TargetWriteTask"))
        {
            var row = new global::MetaPipeline.TargetWriteTask
            {
                Id = source.Id
            };
            typed.TargetWriteTaskList.Add(row);
            index9.Add(source.Id, row);
        }

        var index10 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.TaskDependency>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TaskDependency"))
        {
            var row = new global::MetaPipeline.TaskDependency
            {
                Id = source.Id
            };
            typed.TaskDependencyList.Add(row);
            index10.Add(source.Id, row);
        }

        var index11 = new global::System.Collections.Generic.Dictionary<string, global::MetaPipeline.TransformExecutionTask>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformExecutionTask"))
        {
            var row = new global::MetaPipeline.TransformExecutionTask
            {
                Id = source.Id
            };
            row.BindingWorkspacePath = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "BindingWorkspacePath");
            row.TimeoutSeconds = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TimeoutSeconds");
            row.TransformBindingId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TransformBindingId");
            row.TransformScriptId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TransformScriptId");
            row.TransformWorkspacePath = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TransformWorkspacePath");
            typed.TransformExecutionTaskList.Add(row);
            index11.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ConnectionReference"))
        {
            var row = index0[source.Id];
            row.Pipeline = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineId", index3);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ExecutableTask"))
        {
            var row = index1[source.Id];
            row.PipelineTask = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineTaskId", index4);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "InsertRowsTargetWriteTask"))
        {
            var row = index2[source.Id];
            row.TargetWriteTask = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TargetWriteTaskId", index9);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PipelineTask"))
        {
            var row = index4[source.Id];
            row.Pipeline = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineId", index3);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RowStream"))
        {
            var row = index5[source.Id];
            row.Pipeline = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineId", index3);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RowStreamColumn"))
        {
            var row = index6[source.Id];
            row.RowStream = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RowStreamId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RowStreamConsumer"))
        {
            var row = index7[source.Id];
            row.PipelineTask = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineTaskId", index4);
            row.RowStream = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RowStreamId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RowStreamProducer"))
        {
            var row = index8[source.Id];
            row.PipelineTask = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineTaskId", index4);
            row.RowStream = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RowStreamId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TargetWriteTask"))
        {
            var row = index9[source.Id];
            row.PipelineTask = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineTaskId", index4);
            row.TargetConnectionReference = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TargetConnectionReferenceId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TaskDependency"))
        {
            var row = index10[source.Id];
            row.Pipeline = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineId", index3);
            row.Predecessor = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PredecessorId", index4);
            row.Successor = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SuccessorId", index4);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformExecutionTask"))
        {
            var row = index11[source.Id];
            row.ExecutionConnectionReference = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ExecutionConnectionReferenceId", index0);
            row.PipelineTask = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineTaskId", index4);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaPipeline.MetaPipelineModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ConnectionReferenceList, row => row.Id, "ConnectionReference");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ExecutableTaskList, row => row.Id, "ExecutableTask");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.InsertRowsTargetWriteTaskList, row => row.Id, "InsertRowsTargetWriteTask");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PipelineList, row => row.Id, "Pipeline");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PipelineTaskList, row => row.Id, "PipelineTask");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RowStreamList, row => row.Id, "RowStream");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RowStreamColumnList, row => row.Id, "RowStreamColumn");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RowStreamConsumerList, row => row.Id, "RowStreamConsumer");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RowStreamProducerList, row => row.Id, "RowStreamProducer");
        var index9 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TargetWriteTaskList, row => row.Id, "TargetWriteTask");
        var index10 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TaskDependencyList, row => row.Id, "TaskDependency");
        var index11 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TransformExecutionTaskList, row => row.Id, "TransformExecutionTask");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.EnvironmentVariableName is not null)
                target.Values.Add("EnvironmentVariableName", row.EnvironmentVariableName);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Pipeline is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Pipeline, row.Pipeline.Id, index3, "ConnectionReference.Pipeline");
                target.RelationshipIds.Add("PipelineId", row.Pipeline.Id);
            }

            instance.GetOrCreateEntityRecords("ConnectionReference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Arguments is not null)
                target.Values.Add("Arguments", row.Arguments);
            if (row.ExecutablePath is not null)
                target.Values.Add("ExecutablePath", row.ExecutablePath);
            if (row.SuccessExitCode is not null)
                target.Values.Add("SuccessExitCode", row.SuccessExitCode);
            if (row.TimeoutSeconds is not null)
                target.Values.Add("TimeoutSeconds", row.TimeoutSeconds);
            if (row.WorkingDirectory is not null)
                target.Values.Add("WorkingDirectory", row.WorkingDirectory);
            if (row.PipelineTask is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PipelineTask, row.PipelineTask.Id, index4, "ExecutableTask.PipelineTask");
                target.RelationshipIds.Add("PipelineTaskId", row.PipelineTask.Id);
            }

            instance.GetOrCreateEntityRecords("ExecutableTask").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BatchSize is not null)
                target.Values.Add("BatchSize", row.BatchSize);
            if (row.TargetDataTypeSystemName is not null)
                target.Values.Add("TargetDataTypeSystemName", row.TargetDataTypeSystemName);
            if (row.TargetSqlIdentifier is not null)
                target.Values.Add("TargetSqlIdentifier", row.TargetSqlIdentifier);
            if (row.TargetWriteTask is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TargetWriteTask, row.TargetWriteTask.Id, index9, "InsertRowsTargetWriteTask.TargetWriteTask");
                target.RelationshipIds.Add("TargetWriteTaskId", row.TargetWriteTask.Id);
            }

            instance.GetOrCreateEntityRecords("InsertRowsTargetWriteTask").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("Pipeline").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Pipeline is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Pipeline, row.Pipeline.Id, index3, "PipelineTask.Pipeline");
                target.RelationshipIds.Add("PipelineId", row.Pipeline.Id);
            }

            instance.GetOrCreateEntityRecords("PipelineTask").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Pipeline is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Pipeline, row.Pipeline.Id, index3, "RowStream.Pipeline");
                target.RelationshipIds.Add("PipelineId", row.Pipeline.Id);
            }

            instance.GetOrCreateEntityRecords("RowStream").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.RowStream is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RowStream, row.RowStream.Id, index5, "RowStreamColumn.RowStream");
                target.RelationshipIds.Add("RowStreamId", row.RowStream.Id);
            }

            instance.GetOrCreateEntityRecords("RowStreamColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.PipelineTask is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PipelineTask, row.PipelineTask.Id, index4, "RowStreamConsumer.PipelineTask");
                target.RelationshipIds.Add("PipelineTaskId", row.PipelineTask.Id);
            }

            if (row.RowStream is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RowStream, row.RowStream.Id, index5, "RowStreamConsumer.RowStream");
                target.RelationshipIds.Add("RowStreamId", row.RowStream.Id);
            }

            instance.GetOrCreateEntityRecords("RowStreamConsumer").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index8))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.PipelineTask is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PipelineTask, row.PipelineTask.Id, index4, "RowStreamProducer.PipelineTask");
                target.RelationshipIds.Add("PipelineTaskId", row.PipelineTask.Id);
            }

            if (row.RowStream is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RowStream, row.RowStream.Id, index5, "RowStreamProducer.RowStream");
                target.RelationshipIds.Add("RowStreamId", row.RowStream.Id);
            }

            instance.GetOrCreateEntityRecords("RowStreamProducer").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index9))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.PipelineTask is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PipelineTask, row.PipelineTask.Id, index4, "TargetWriteTask.PipelineTask");
                target.RelationshipIds.Add("PipelineTaskId", row.PipelineTask.Id);
            }

            if (row.TargetConnectionReference is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TargetConnectionReference, row.TargetConnectionReference.Id, index0, "TargetWriteTask.TargetConnectionReference");
                target.RelationshipIds.Add("TargetConnectionReferenceId", row.TargetConnectionReference.Id);
            }

            instance.GetOrCreateEntityRecords("TargetWriteTask").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index10))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Pipeline is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Pipeline, row.Pipeline.Id, index3, "TaskDependency.Pipeline");
                target.RelationshipIds.Add("PipelineId", row.Pipeline.Id);
            }

            if (row.Predecessor is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Predecessor, row.Predecessor.Id, index4, "TaskDependency.Predecessor");
                target.RelationshipIds.Add("PredecessorId", row.Predecessor.Id);
            }

            if (row.Successor is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Successor, row.Successor.Id, index4, "TaskDependency.Successor");
                target.RelationshipIds.Add("SuccessorId", row.Successor.Id);
            }

            instance.GetOrCreateEntityRecords("TaskDependency").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index11))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BindingWorkspacePath is not null)
                target.Values.Add("BindingWorkspacePath", row.BindingWorkspacePath);
            if (row.TimeoutSeconds is not null)
                target.Values.Add("TimeoutSeconds", row.TimeoutSeconds);
            if (row.TransformBindingId is not null)
                target.Values.Add("TransformBindingId", row.TransformBindingId);
            if (row.TransformScriptId is not null)
                target.Values.Add("TransformScriptId", row.TransformScriptId);
            if (row.TransformWorkspacePath is not null)
                target.Values.Add("TransformWorkspacePath", row.TransformWorkspacePath);
            if (row.ExecutionConnectionReference is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ExecutionConnectionReference, row.ExecutionConnectionReference.Id, index0, "TransformExecutionTask.ExecutionConnectionReference");
                target.RelationshipIds.Add("ExecutionConnectionReferenceId", row.ExecutionConnectionReference.Id);
            }

            if (row.PipelineTask is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PipelineTask, row.PipelineTask.Id, index4, "TransformExecutionTask.PipelineTask");
                target.RelationshipIds.Add("PipelineTaskId", row.PipelineTask.Id);
            }

            instance.GetOrCreateEntityRecords("TransformExecutionTask").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}