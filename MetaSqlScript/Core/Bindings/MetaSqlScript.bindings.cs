// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaSqlScript;
public sealed class MetaSqlScriptWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaSqlScript.MetaSqlScriptModel>
{
    private const string ContractSignature = "6d6f64656c7c4d65746153716c5363726970740a656e746974797c53716c5363726970747c53716c5363726970744c6973740a70726f70657274797c53716c5363726970747c4e616d657c72657175697265640a70726f70657274797c53716c5363726970747c53716c546578747c7265717569726564";
    public global::MetaSqlScript.MetaSqlScriptModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaSqlScript.MetaSqlScriptModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlScript.SqlScript>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SqlScript"))
        {
            var row = new global::MetaSqlScript.SqlScript
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.SqlText = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SqlText");
            typed.SqlScriptList.Add(row);
            index0.Add(source.Id, row);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaSqlScript.MetaSqlScriptModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SqlScriptList, row => row.Id, "SqlScript");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.SqlText is not null)
                target.Values.Add("SqlText", row.SqlText);
            instance.GetOrCreateEntityRecords("SqlScript").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}