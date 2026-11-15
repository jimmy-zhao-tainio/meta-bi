// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaTransformPatternInstance;
public sealed class MetaTransformPatternInstanceWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaTransformPatternInstance.MetaTransformPatternInstanceModel>
{
    private const string ContractSignature = "6d6f64656c7c4d6574615472616e73666f726d5061747465726e496e7374616e63650a656e746974797c5472616e73666f726d5061747465726e496e7374616e63657c5472616e73666f726d5061747465726e496e7374616e63654c6973740a70726f70657274797c5472616e73666f726d5061747465726e496e7374616e63657c4e616d657c72657175697265640a70726f70657274797c5472616e73666f726d5061747465726e496e7374616e63657c5472616e73666f726d5061747465726e49647c72657175697265640a656e746974797c5472616e73666f726d5061747465726e496e7374616e6365506c616365686f6c6465727c5472616e73666f726d5061747465726e496e7374616e6365506c616365686f6c6465724c6973740a70726f70657274797c5472616e73666f726d5061747465726e496e7374616e6365506c616365686f6c6465727c53716c546578747c6e756c6c61626c650a70726f70657274797c5472616e73666f726d5061747465726e496e7374616e6365506c616365686f6c6465727c5472616e73666f726d5061747465726e506c616365686f6c64657249647c72657175697265640a72656c6174696f6e736869707c5472616e73666f726d5061747465726e496e7374616e6365506c616365686f6c6465727c5472616e73666f726d5061747465726e496e7374616e63657c5472616e73666f726d5061747465726e496e7374616e636549647c7265717569726564";
    public global::MetaTransformPatternInstance.MetaTransformPatternInstanceModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaTransformPatternInstance.MetaTransformPatternInstanceModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformPatternInstance.TransformPatternInstance>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPatternInstance"))
        {
            var row = new global::MetaTransformPatternInstance.TransformPatternInstance
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.TransformPatternId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TransformPatternId");
            typed.TransformPatternInstanceList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformPatternInstance.TransformPatternInstancePlaceholder>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPatternInstancePlaceholder"))
        {
            var row = new global::MetaTransformPatternInstance.TransformPatternInstancePlaceholder
            {
                Id = source.Id
            };
            row.SqlText = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "SqlText");
            row.TransformPatternPlaceholderId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TransformPatternPlaceholderId");
            typed.TransformPatternInstancePlaceholderList.Add(row);
            index1.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPatternInstancePlaceholder"))
        {
            var row = index1[source.Id];
            row.TransformPatternInstance = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformPatternInstanceId", index0);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaTransformPatternInstance.MetaTransformPatternInstanceModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TransformPatternInstanceList, row => row.Id, "TransformPatternInstance");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TransformPatternInstancePlaceholderList, row => row.Id, "TransformPatternInstancePlaceholder");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.TransformPatternId is not null)
                target.Values.Add("TransformPatternId", row.TransformPatternId);
            instance.GetOrCreateEntityRecords("TransformPatternInstance").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SqlText is not null)
                target.Values.Add("SqlText", row.SqlText);
            if (row.TransformPatternPlaceholderId is not null)
                target.Values.Add("TransformPatternPlaceholderId", row.TransformPatternPlaceholderId);
            if (row.TransformPatternInstance is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformPatternInstance, row.TransformPatternInstance.Id, index0, "TransformPatternInstancePlaceholder.TransformPatternInstance");
                target.RelationshipIds.Add("TransformPatternInstanceId", row.TransformPatternInstance.Id);
            }

            instance.GetOrCreateEntityRecords("TransformPatternInstancePlaceholder").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}