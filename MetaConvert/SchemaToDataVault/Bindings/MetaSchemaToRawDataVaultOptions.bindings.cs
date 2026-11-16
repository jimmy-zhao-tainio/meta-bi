// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaSchemaToRawDataVaultOptions;
public sealed class MetaSchemaToRawDataVaultOptionsWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaSchemaToRawDataVaultOptions.MetaSchemaToRawDataVaultOptionsModel>
{
    private const string ContractSignature = "6d6f64656c7c4d657461536368656d61546f526177446174615661756c744f7074696f6e730a656e746974797c436f6e76657273696f6e4f7074696f6e737c436f6e76657273696f6e4f7074696f6e734c6973740a656e746974797c49676e6f7265644669656c644e616d657c49676e6f7265644669656c644e616d654c6973740a70726f70657274797c49676e6f7265644669656c644e616d657c56616c75657c72657175697265640a72656c6174696f6e736869707c49676e6f7265644669656c644e616d657c436f6e76657273696f6e4f7074696f6e737c436f6e76657273696f6e4f7074696f6e7349647c72657175697265640a656e746974797c49676e6f7265644669656c645375666669787c49676e6f7265644669656c645375666669784c6973740a70726f70657274797c49676e6f7265644669656c645375666669787c56616c75657c72657175697265640a72656c6174696f6e736869707c49676e6f7265644669656c645375666669787c436f6e76657273696f6e4f7074696f6e737c436f6e76657273696f6e4f7074696f6e7349647c72657175697265640a656e746974797c496e636c75646556696577734f7074696f6e7c496e636c75646556696577734f7074696f6e4c6973740a72656c6174696f6e736869707c496e636c75646556696577734f7074696f6e7c436f6e76657273696f6e4f7074696f6e737c436f6e76657273696f6e4f7074696f6e7349647c7265717569726564";
    public global::MetaSchemaToRawDataVaultOptions.MetaSchemaToRawDataVaultOptionsModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaSchemaToRawDataVaultOptions.MetaSchemaToRawDataVaultOptionsModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchemaToRawDataVaultOptions.ConversionOptions>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ConversionOptions"))
        {
            var row = new global::MetaSchemaToRawDataVaultOptions.ConversionOptions
            {
                Id = source.Id
            };
            typed.ConversionOptionsList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchemaToRawDataVaultOptions.IgnoredFieldName>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "IgnoredFieldName"))
        {
            var row = new global::MetaSchemaToRawDataVaultOptions.IgnoredFieldName
            {
                Id = source.Id
            };
            row.Value = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Value");
            typed.IgnoredFieldNameList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchemaToRawDataVaultOptions.IgnoredFieldSuffix>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "IgnoredFieldSuffix"))
        {
            var row = new global::MetaSchemaToRawDataVaultOptions.IgnoredFieldSuffix
            {
                Id = source.Id
            };
            row.Value = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Value");
            typed.IgnoredFieldSuffixList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchemaToRawDataVaultOptions.IncludeViewsOption>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "IncludeViewsOption"))
        {
            var row = new global::MetaSchemaToRawDataVaultOptions.IncludeViewsOption
            {
                Id = source.Id
            };
            typed.IncludeViewsOptionList.Add(row);
            index3.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "IgnoredFieldName"))
        {
            var row = index1[source.Id];
            row.ConversionOptions = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ConversionOptionsId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "IgnoredFieldSuffix"))
        {
            var row = index2[source.Id];
            row.ConversionOptions = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ConversionOptionsId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "IncludeViewsOption"))
        {
            var row = index3[source.Id];
            row.ConversionOptions = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ConversionOptionsId", index0);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaSchemaToRawDataVaultOptions.MetaSchemaToRawDataVaultOptionsModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ConversionOptionsList, row => row.Id, "ConversionOptions");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.IgnoredFieldNameList, row => row.Id, "IgnoredFieldName");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.IgnoredFieldSuffixList, row => row.Id, "IgnoredFieldSuffix");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.IncludeViewsOptionList, row => row.Id, "IncludeViewsOption");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            instance.GetOrCreateEntityRecords("ConversionOptions").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Value is not null)
                target.Values.Add("Value", row.Value);
            if (row.ConversionOptions is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ConversionOptions, row.ConversionOptions.Id, index0, "IgnoredFieldName.ConversionOptions");
                target.RelationshipIds.Add("ConversionOptionsId", row.ConversionOptions.Id);
            }

            instance.GetOrCreateEntityRecords("IgnoredFieldName").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Value is not null)
                target.Values.Add("Value", row.Value);
            if (row.ConversionOptions is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ConversionOptions, row.ConversionOptions.Id, index0, "IgnoredFieldSuffix.ConversionOptions");
                target.RelationshipIds.Add("ConversionOptionsId", row.ConversionOptions.Id);
            }

            instance.GetOrCreateEntityRecords("IgnoredFieldSuffix").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.ConversionOptions is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ConversionOptions, row.ConversionOptions.Id, index0, "IncludeViewsOption.ConversionOptions");
                target.RelationshipIds.Add("ConversionOptionsId", row.ConversionOptions.Id);
            }

            instance.GetOrCreateEntityRecords("IncludeViewsOption").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}