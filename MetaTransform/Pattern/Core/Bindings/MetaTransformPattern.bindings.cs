// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaTransformPattern;
public sealed class MetaTransformPatternWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaTransformPattern.MetaTransformPatternModel>
{
    private const string ContractSignature = "6d6f64656c7c4d6574615472616e73666f726d5061747465726e0a656e746974797c5472616e73666f726d5061747465726e7c5472616e73666f726d5061747465726e4c6973740a70726f70657274797c5472616e73666f726d5061747465726e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c5472616e73666f726d5061747465726e7c4e616d657c72657175697265640a656e746974797c5472616e73666f726d5061747465726e4974656d7c5472616e73666f726d5061747465726e4974656d4c6973740a72656c6174696f6e736869707c5472616e73666f726d5061747465726e4974656d7c5472616e73666f726d5061747465726e4974656d7c50726576696f75734974656d49647c6e756c6c61626c650a72656c6174696f6e736869707c5472616e73666f726d5061747465726e4974656d7c5472616e73666f726d5061747465726e7c5472616e73666f726d5061747465726e49647c72657175697265640a656e746974797c5472616e73666f726d5061747465726e506c616365686f6c6465727c5472616e73666f726d5061747465726e506c616365686f6c6465724c6973740a70726f70657274797c5472616e73666f726d5061747465726e506c616365686f6c6465727c4e616d657c72657175697265640a72656c6174696f6e736869707c5472616e73666f726d5061747465726e506c616365686f6c6465727c5472616e73666f726d5061747465726e7c5472616e73666f726d5061747465726e49647c72657175697265640a656e746974797c5472616e73666f726d5061747465726e506c616365686f6c6465724974656d7c5472616e73666f726d5061747465726e506c616365686f6c6465724974656d4c6973740a72656c6174696f6e736869707c5472616e73666f726d5061747465726e506c616365686f6c6465724974656d7c5472616e73666f726d5061747465726e4974656d7c5472616e73666f726d5061747465726e4974656d49647c72657175697265640a72656c6174696f6e736869707c5472616e73666f726d5061747465726e506c616365686f6c6465724974656d7c5472616e73666f726d5061747465726e506c616365686f6c6465727c5472616e73666f726d5061747465726e506c616365686f6c64657249647c72657175697265640a656e746974797c5472616e73666f726d5061747465726e546578747c5472616e73666f726d5061747465726e546578744c6973740a70726f70657274797c5472616e73666f726d5061747465726e546578747c53716c546578747c72657175697265640a72656c6174696f6e736869707c5472616e73666f726d5061747465726e546578747c5472616e73666f726d5061747465726e4974656d7c5472616e73666f726d5061747465726e4974656d49647c7265717569726564";
    public global::MetaTransformPattern.MetaTransformPatternModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaTransformPattern.MetaTransformPatternModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformPattern.TransformPattern>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPattern"))
        {
            var row = new global::MetaTransformPattern.TransformPattern
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TransformPatternList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformPattern.TransformPatternItem>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPatternItem"))
        {
            var row = new global::MetaTransformPattern.TransformPatternItem
            {
                Id = source.Id
            };
            typed.TransformPatternItemList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformPattern.TransformPatternPlaceholder>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPatternPlaceholder"))
        {
            var row = new global::MetaTransformPattern.TransformPatternPlaceholder
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TransformPatternPlaceholderList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformPattern.TransformPatternPlaceholderItem>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPatternPlaceholderItem"))
        {
            var row = new global::MetaTransformPattern.TransformPatternPlaceholderItem
            {
                Id = source.Id
            };
            typed.TransformPatternPlaceholderItemList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformPattern.TransformPatternText>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPatternText"))
        {
            var row = new global::MetaTransformPattern.TransformPatternText
            {
                Id = source.Id
            };
            row.SqlText = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SqlText");
            typed.TransformPatternTextList.Add(row);
            index4.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPatternItem"))
        {
            var row = index1[source.Id];
            row.PreviousItem = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "PreviousItemId", index1);
            row.TransformPattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformPatternId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPatternPlaceholder"))
        {
            var row = index2[source.Id];
            row.TransformPattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformPatternId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPatternPlaceholderItem"))
        {
            var row = index3[source.Id];
            row.TransformPatternItem = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformPatternItemId", index1);
            row.TransformPatternPlaceholder = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformPatternPlaceholderId", index2);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformPatternText"))
        {
            var row = index4[source.Id];
            row.TransformPatternItem = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformPatternItemId", index1);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaTransformPattern.MetaTransformPatternModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TransformPatternList, row => row.Id, "TransformPattern");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TransformPatternItemList, row => row.Id, "TransformPatternItem");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TransformPatternPlaceholderList, row => row.Id, "TransformPatternPlaceholder");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TransformPatternPlaceholderItemList, row => row.Id, "TransformPatternPlaceholderItem");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TransformPatternTextList, row => row.Id, "TransformPatternText");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("TransformPattern").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.PreviousItem is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PreviousItem, row.PreviousItem.Id, index1, "TransformPatternItem.PreviousItem");
                target.RelationshipIds.Add("PreviousItemId", row.PreviousItem.Id);
            }

            if (row.TransformPattern is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformPattern, row.TransformPattern.Id, index0, "TransformPatternItem.TransformPattern");
                target.RelationshipIds.Add("TransformPatternId", row.TransformPattern.Id);
            }

            instance.GetOrCreateEntityRecords("TransformPatternItem").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.TransformPattern is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformPattern, row.TransformPattern.Id, index0, "TransformPatternPlaceholder.TransformPattern");
                target.RelationshipIds.Add("TransformPatternId", row.TransformPattern.Id);
            }

            instance.GetOrCreateEntityRecords("TransformPatternPlaceholder").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.TransformPatternItem is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformPatternItem, row.TransformPatternItem.Id, index1, "TransformPatternPlaceholderItem.TransformPatternItem");
                target.RelationshipIds.Add("TransformPatternItemId", row.TransformPatternItem.Id);
            }

            if (row.TransformPatternPlaceholder is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformPatternPlaceholder, row.TransformPatternPlaceholder.Id, index2, "TransformPatternPlaceholderItem.TransformPatternPlaceholder");
                target.RelationshipIds.Add("TransformPatternPlaceholderId", row.TransformPatternPlaceholder.Id);
            }

            instance.GetOrCreateEntityRecords("TransformPatternPlaceholderItem").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SqlText is not null)
                target.Values.Add("SqlText", row.SqlText);
            if (row.TransformPatternItem is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformPatternItem, row.TransformPatternItem.Id, index1, "TransformPatternText.TransformPatternItem");
                target.RelationshipIds.Add("TransformPatternItemId", row.TransformPatternItem.Id);
            }

            instance.GetOrCreateEntityRecords("TransformPatternText").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}