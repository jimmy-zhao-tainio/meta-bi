// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaDataType;
public sealed class MetaDataTypeWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaDataType.MetaDataTypeModel>
{
    private const string ContractSignature = "6d6f64656c7c4d65746144617461547970650a656e746974797c44617461547970657c44617461547970654c6973740a70726f70657274797c44617461547970657c43617465676f72797c6e756c6c61626c650a70726f70657274797c44617461547970657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c44617461547970657c497343616e6f6e6963616c7c6e756c6c61626c650a70726f70657274797c44617461547970657c4e616d657c72657175697265640a72656c6174696f6e736869707c44617461547970657c446174615479706553797374656d7c446174615479706553797374656d49647c72657175697265640a656e746974797c446174615479706553797374656d7c446174615479706553797374656d4c6973740a70726f70657274797c446174615479706553797374656d7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c446174615479706553797374656d7c4e616d657c7265717569726564";
    public global::MetaDataType.MetaDataTypeModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaDataType.MetaDataTypeModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataType.DataType>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DataType"))
        {
            var row = new global::MetaDataType.DataType
            {
                Id = source.Id
            };
            row.Category = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Category");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.IsCanonical = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsCanonical");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.DataTypeList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataType.DataTypeSystem>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DataTypeSystem"))
        {
            var row = new global::MetaDataType.DataTypeSystem
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.DataTypeSystemList.Add(row);
            index1.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DataType"))
        {
            var row = index0[source.Id];
            row.DataTypeSystem = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DataTypeSystemId", index1);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaDataType.MetaDataTypeModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DataTypeList, row => row.Id, "DataType");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DataTypeSystemList, row => row.Id, "DataTypeSystem");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Category is not null)
                target.Values.Add("Category", row.Category);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.IsCanonical is not null)
                target.Values.Add("IsCanonical", row.IsCanonical);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.DataTypeSystem is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DataTypeSystem, row.DataTypeSystem.Id, index1, "DataType.DataTypeSystem");
                target.RelationshipIds.Add("DataTypeSystemId", row.DataTypeSystem.Id);
            }

            instance.GetOrCreateEntityRecords("DataType").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("DataTypeSystem").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}