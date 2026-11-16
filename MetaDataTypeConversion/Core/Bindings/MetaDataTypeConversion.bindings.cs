// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaDataTypeConversion;
public sealed class MetaDataTypeConversionWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaDataTypeConversion.MetaDataTypeConversionModel>
{
    private const string ContractSignature = "6d6f64656c7c4d6574614461746154797065436f6e76657273696f6e0a656e746974797c436f6e76657273696f6e496d706c656d656e746174696f6e7c436f6e76657273696f6e496d706c656d656e746174696f6e4c6973740a70726f70657274797c436f6e76657273696f6e496d706c656d656e746174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c436f6e76657273696f6e496d706c656d656e746174696f6e7c4e616d657c72657175697265640a72656c6174696f6e736869707c436f6e76657273696f6e496d706c656d656e746174696f6e7c53716c436f6e76657273696f6e536e69707065747c53716c436f6e76657273696f6e536e697070657449647c6e756c6c61626c650a656e746974797c44617461547970654d617070696e677c44617461547970654d617070696e674c6973740a70726f70657274797c44617461547970654d617070696e677c4e6f7465737c6e756c6c61626c650a70726f70657274797c44617461547970654d617070696e677c536f75726365446174615479706549647c72657175697265640a70726f70657274797c44617461547970654d617070696e677c546172676574446174615479706549647c72657175697265640a72656c6174696f6e736869707c44617461547970654d617070696e677c436f6e76657273696f6e496d706c656d656e746174696f6e7c436f6e76657273696f6e496d706c656d656e746174696f6e49647c72657175697265640a656e746974797c53716c436f6e76657273696f6e506172616d657465727c53716c436f6e76657273696f6e506172616d657465724c6973740a70726f70657274797c53716c436f6e76657273696f6e506172616d657465727c4e616d657c72657175697265640a656e746974797c53716c436f6e76657273696f6e506172616d6574657242696e64696e677c53716c436f6e76657273696f6e506172616d6574657242696e64696e674c6973740a70726f70657274797c53716c436f6e76657273696f6e506172616d6574657242696e64696e677c4e616d657c72657175697265640a72656c6174696f6e736869707c53716c436f6e76657273696f6e506172616d6574657242696e64696e677c53716c436f6e76657273696f6e506172616d657465727c53716c436f6e76657273696f6e506172616d6574657249647c72657175697265640a72656c6174696f6e736869707c53716c436f6e76657273696f6e506172616d6574657242696e64696e677c53716c436f6e76657273696f6e536e69707065747c53716c436f6e76657273696f6e536e697070657449647c72657175697265640a656e746974797c53716c436f6e76657273696f6e536e69707065747c53716c436f6e76657273696f6e536e69707065744c6973740a70726f70657274797c53716c436f6e76657273696f6e536e69707065747c5472616e73666f726d5363726970747c7265717569726564";
    public global::MetaDataTypeConversion.MetaDataTypeConversionModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaDataTypeConversion.MetaDataTypeConversionModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataTypeConversion.ConversionImplementation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ConversionImplementation"))
        {
            var row = new global::MetaDataTypeConversion.ConversionImplementation
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.ConversionImplementationList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataTypeConversion.DataTypeMapping>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DataTypeMapping"))
        {
            var row = new global::MetaDataTypeConversion.DataTypeMapping
            {
                Id = source.Id
            };
            row.Notes = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Notes");
            row.SourceDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceDataTypeId");
            row.TargetDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TargetDataTypeId");
            typed.DataTypeMappingList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataTypeConversion.SqlConversionParameter>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SqlConversionParameter"))
        {
            var row = new global::MetaDataTypeConversion.SqlConversionParameter
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.SqlConversionParameterList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataTypeConversion.SqlConversionParameterBinding>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SqlConversionParameterBinding"))
        {
            var row = new global::MetaDataTypeConversion.SqlConversionParameterBinding
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.SqlConversionParameterBindingList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataTypeConversion.SqlConversionSnippet>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SqlConversionSnippet"))
        {
            var row = new global::MetaDataTypeConversion.SqlConversionSnippet
            {
                Id = source.Id
            };
            row.TransformScript = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TransformScript");
            typed.SqlConversionSnippetList.Add(row);
            index4.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ConversionImplementation"))
        {
            var row = index0[source.Id];
            row.SqlConversionSnippet = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "SqlConversionSnippetId", index4);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DataTypeMapping"))
        {
            var row = index1[source.Id];
            row.ConversionImplementation = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ConversionImplementationId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SqlConversionParameterBinding"))
        {
            var row = index3[source.Id];
            row.SqlConversionParameter = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SqlConversionParameterId", index2);
            row.SqlConversionSnippet = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SqlConversionSnippetId", index4);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaDataTypeConversion.MetaDataTypeConversionModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ConversionImplementationList, row => row.Id, "ConversionImplementation");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DataTypeMappingList, row => row.Id, "DataTypeMapping");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SqlConversionParameterList, row => row.Id, "SqlConversionParameter");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SqlConversionParameterBindingList, row => row.Id, "SqlConversionParameterBinding");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SqlConversionSnippetList, row => row.Id, "SqlConversionSnippet");
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
            if (row.SqlConversionSnippet is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SqlConversionSnippet, row.SqlConversionSnippet.Id, index4, "ConversionImplementation.SqlConversionSnippet");
                target.RelationshipIds.Add("SqlConversionSnippetId", row.SqlConversionSnippet.Id);
            }

            instance.GetOrCreateEntityRecords("ConversionImplementation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Notes is not null)
                target.Values.Add("Notes", row.Notes);
            if (row.SourceDataTypeId is not null)
                target.Values.Add("SourceDataTypeId", row.SourceDataTypeId);
            if (row.TargetDataTypeId is not null)
                target.Values.Add("TargetDataTypeId", row.TargetDataTypeId);
            if (row.ConversionImplementation is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ConversionImplementation, row.ConversionImplementation.Id, index0, "DataTypeMapping.ConversionImplementation");
                target.RelationshipIds.Add("ConversionImplementationId", row.ConversionImplementation.Id);
            }

            instance.GetOrCreateEntityRecords("DataTypeMapping").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("SqlConversionParameter").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.SqlConversionParameter is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SqlConversionParameter, row.SqlConversionParameter.Id, index2, "SqlConversionParameterBinding.SqlConversionParameter");
                target.RelationshipIds.Add("SqlConversionParameterId", row.SqlConversionParameter.Id);
            }

            if (row.SqlConversionSnippet is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SqlConversionSnippet, row.SqlConversionSnippet.Id, index4, "SqlConversionParameterBinding.SqlConversionSnippet");
                target.RelationshipIds.Add("SqlConversionSnippetId", row.SqlConversionSnippet.Id);
            }

            instance.GetOrCreateEntityRecords("SqlConversionParameterBinding").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.TransformScript is not null)
                target.Values.Add("TransformScript", row.TransformScript);
            instance.GetOrCreateEntityRecords("SqlConversionSnippet").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}