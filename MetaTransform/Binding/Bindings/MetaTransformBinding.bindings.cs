// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaTransformBinding;
public sealed class MetaTransformBindingWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaTransformBinding.MetaTransformBindingModel>
{
    private const string ContractSignature = "6d6f64656c7c4d6574615472616e73666f726d42696e64696e670a656e746974797c436f6c756d6e7c436f6c756d6e4c6973740a70726f70657274797c436f6c756d6e7c4e616d657c72657175697265640a70726f70657274797c436f6c756d6e7c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c436f6c756d6e7c526f777365747c526f7773657449647c72657175697265640a656e746974797c436f6c756d6e5265666572656e63657c436f6c756d6e5265666572656e63654c6973740a70726f70657274797c436f6c756d6e5265666572656e63657c4d6574615472616e73666f726d536372697074436f6c756d6e5265666572656e636549647c72657175697265640a72656c6174696f6e736869707c436f6c756d6e5265666572656e63657c436f6c756d6e7c436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c436f6c756d6e5265666572656e63657c5461626c65536f757263657c5461626c65536f7572636549647c72657175697265640a72656c6174696f6e736869707c436f6c756d6e5265666572656e63657c5472616e73666f726d42696e64696e677c5472616e73666f726d42696e64696e6749647c72657175697265640a656e746974797c44656c6574657c44656c6574654c6973740a70726f70657274797c44656c6574657c4d6574615472616e73666f726d53637269707444656c65746553746174656d656e7449647c72657175697265640a72656c6174696f6e736869707c44656c6574657c56616c69646174696f6e546172676574526f777365744c696e6b7c56616c69646174696f6e546172676574526f777365744c696e6b49647c72657175697265640a656e746974797c496e73657274517565727957726974657c496e73657274517565727957726974654c6973740a70726f70657274797c496e73657274517565727957726974657c4d6574615472616e73666f726d536372697074517565727945787072657373696f6e49647c72657175697265640a72656c6174696f6e736869707c496e73657274517565727957726974657c57726974657c577269746549647c72657175697265640a656e746974797c496e7365727456616c75657357726974657c496e7365727456616c75657357726974654c6973740a70726f70657274797c496e7365727456616c75657357726974657c4d6574615472616e73666f726d536372697074526f7756616c756549647c72657175697265640a72656c6174696f6e736869707c496e7365727456616c75657357726974657c57726974657c577269746549647c72657175697265640a656e746974797c4d6572676544656c6574657c4d6572676544656c6574654c6973740a70726f70657274797c4d6572676544656c6574657c4d6574615472616e73666f726d5363726970744d6572676544656c657465416374696f6e49647c72657175697265640a72656c6174696f6e736869707c4d6572676544656c6574657c56616c69646174696f6e546172676574526f777365744c696e6b7c56616c69646174696f6e546172676574526f777365744c696e6b49647c72657175697265640a656e746974797c4d65726765496e7365727457726974657c4d65726765496e7365727457726974654c6973740a70726f70657274797c4d65726765496e7365727457726974657c4d6574615472616e73666f726d5363726970744d65726765496e73657274416374696f6e49647c72657175697265640a72656c6174696f6e736869707c4d65726765496e7365727457726974657c57726974657c577269746549647c72657175697265640a656e746974797c4d6572676555706461746557726974657c4d6572676555706461746557726974654c6973740a70726f70657274797c4d6572676555706461746557726974657c4d6574615472616e73666f726d5363726970744d65726765557064617465416374696f6e49647c72657175697265640a72656c6174696f6e736869707c4d6572676555706461746557726974657c57726974657c577269746549647c72657175697265640a656e746974797c4f7574707574526f777365747c4f7574707574526f777365744c6973740a72656c6174696f6e736869707c4f7574707574526f777365747c526f777365747c526f7773657449647c72657175697265640a72656c6174696f6e736869707c4f7574707574526f777365747c5472616e73666f726d42696e64696e677c5472616e73666f726d42696e64696e6749647c72657175697265640a656e746974797c526f777365747c526f777365744c6973740a70726f70657274797c526f777365747c44657269766174696f6e4b696e647c72657175697265640a70726f70657274797c526f777365747c4e616d657c72657175697265640a70726f70657274797c526f777365747c53716c4964656e7469666965727c6e756c6c61626c650a72656c6174696f6e736869707c526f777365747c5472616e73666f726d42696e64696e677c5472616e73666f726d42696e64696e6749647c72657175697265640a656e746974797c536f757263655461726765747c536f757263655461726765744c6973740a70726f70657274797c536f757263655461726765747c496e707574526f6c657c6e756c6c61626c650a70726f70657274797c536f757263655461726765747c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c536f757263655461726765747c526f777365747c536f7572636549647c72657175697265640a72656c6174696f6e736869707c536f757263655461726765747c526f777365747c54617267657449647c72657175697265640a656e746974797c53746f72656450726f6365647572654f7065726174696f6e42696e64696e677c53746f72656450726f6365647572654f7065726174696f6e42696e64696e674c6973740a70726f70657274797c53746f72656450726f6365647572654f7065726174696f6e42696e64696e677c4d6574615472616e73666f726d53637269707453746f72656450726f636564757265436f6e74726163744f7065726174696f6e49647c72657175697265640a72656c6174696f6e736869707c53746f72656450726f6365647572654f7065726174696f6e42696e64696e677c526f777365747c526f7773657449647c72657175697265640a72656c6174696f6e736869707c53746f72656450726f6365647572654f7065726174696f6e42696e64696e677c5472616e73666f726d42696e64696e677c5472616e73666f726d42696e64696e6749647c72657175697265640a656e746974797c5461626c65536f757263657c5461626c65536f757263654c6973740a70726f70657274797c5461626c65536f757263657c4578706f7365644e616d657c72657175697265640a70726f70657274797c5461626c65536f757263657c4d6574615472616e73666f726d5363726970745461626c655265666572656e636549647c72657175697265640a72656c6174696f6e736869707c5461626c65536f757263657c526f777365747c526f7773657449647c72657175697265640a72656c6174696f6e736869707c5461626c65536f757263657c5472616e73666f726d42696e64696e677c5472616e73666f726d42696e64696e6749647c72657175697265640a656e746974797c546172676574436f6c756d6e5265666572656e63657c546172676574436f6c756d6e5265666572656e63654c6973740a70726f70657274797c546172676574436f6c756d6e5265666572656e63657c4d657461536368656d614669656c6449647c72657175697265640a70726f70657274797c546172676574436f6c756d6e5265666572656e63657c4d6574615472616e73666f726d536372697074436f6c756d6e5265666572656e636549647c72657175697265640a72656c6174696f6e736869707c546172676574436f6c756d6e5265666572656e63657c436f6c756d6e7c436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c546172676574436f6c756d6e5265666572656e63657c5472616e73666f726d42696e64696e675461726765747c5472616e73666f726d42696e64696e6754617267657449647c72657175697265640a656e746974797c5472616e73666f726d42696e64696e677c5472616e73666f726d42696e64696e674c6973740a70726f70657274797c5472616e73666f726d42696e64696e677c4d6574615472616e73666f726d5363726970745472616e73666f726d53637269707449647c72657175697265640a70726f70657274797c5472616e73666f726d42696e64696e677c5472616e73666f726d5363726970744e616d657c6e756c6c61626c650a656e746974797c5472616e73666f726d42696e64696e675461726765747c5472616e73666f726d42696e64696e675461726765744c6973740a70726f70657274797c5472616e73666f726d42696e64696e675461726765747c53716c4964656e7469666965727c72657175697265640a72656c6174696f6e736869707c5472616e73666f726d42696e64696e675461726765747c5472616e73666f726d42696e64696e677c5472616e73666f726d42696e64696e6749647c72657175697265640a656e746974797c5472756e636174657c5472756e636174654c6973740a70726f70657274797c5472756e636174657c4d6574615472616e73666f726d5363726970745472756e6361746553746174656d656e7449647c72657175697265640a72656c6174696f6e736869707c5472756e636174657c56616c69646174696f6e546172676574526f777365744c696e6b7c56616c69646174696f6e546172676574526f777365744c696e6b49647c72657175697265640a656e746974797c55706461746557726974657c55706461746557726974654c6973740a70726f70657274797c55706461746557726974657c4d6574615472616e73666f726d536372697074536574436c6175736549647c72657175697265640a72656c6174696f6e736869707c55706461746557726974657c57726974657c577269746549647c72657175697265640a656e746974797c56616c69646174696f6e7c56616c69646174696f6e4c6973740a72656c6174696f6e736869707c56616c69646174696f6e7c5472616e73666f726d42696e64696e677c5472616e73666f726d42696e64696e6749647c72657175697265640a656e746974797c56616c69646174696f6e536f75726365436f6c756d6e4c696e6b7c56616c69646174696f6e536f75726365436f6c756d6e4c696e6b4c6973740a70726f70657274797c56616c69646174696f6e536f75726365436f6c756d6e4c696e6b7c4d657461536368656d614669656c6449647c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e536f75726365436f6c756d6e4c696e6b7c436f6c756d6e7c436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e536f75726365436f6c756d6e4c696e6b7c56616c69646174696f6e536f75726365526f777365744c696e6b7c56616c69646174696f6e536f75726365526f777365744c696e6b49647c72657175697265640a656e746974797c56616c69646174696f6e536f75726365526f777365744c696e6b7c56616c69646174696f6e536f75726365526f777365744c696e6b4c6973740a70726f70657274797c56616c69646174696f6e536f75726365526f777365744c696e6b7c4d657461536368656d615461626c6549647c72657175697265640a70726f70657274797c56616c69646174696f6e536f75726365526f777365744c696e6b7c5265736f6c76656453716c4964656e7469666965727c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e536f75726365526f777365744c696e6b7c526f777365747c526f7773657449647c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e536f75726365526f777365744c696e6b7c56616c69646174696f6e7c56616c69646174696f6e49647c72657175697265640a656e746974797c56616c69646174696f6e53746f72656450726f6365647572654f7065726174696f6e4c696e6b7c56616c69646174696f6e53746f72656450726f6365647572654f7065726174696f6e4c696e6b4c6973740a70726f70657274797c56616c69646174696f6e53746f72656450726f6365647572654f7065726174696f6e4c696e6b7c4d657461536368656d615461626c6549647c72657175697265640a70726f70657274797c56616c69646174696f6e53746f72656450726f6365647572654f7065726174696f6e4c696e6b7c5265736f6c76656453716c4964656e7469666965727c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e53746f72656450726f6365647572654f7065726174696f6e4c696e6b7c53746f72656450726f6365647572654f7065726174696f6e42696e64696e677c53746f72656450726f6365647572654f7065726174696f6e42696e64696e6749647c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e53746f72656450726f6365647572654f7065726174696f6e4c696e6b7c56616c69646174696f6e7c56616c69646174696f6e49647c72657175697265640a656e746974797c56616c69646174696f6e546172676574436f6c756d6e4c696e6b7c56616c69646174696f6e546172676574436f6c756d6e4c696e6b4c6973740a70726f70657274797c56616c69646174696f6e546172676574436f6c756d6e4c696e6b7c4d657461536368656d614669656c6449647c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e546172676574436f6c756d6e4c696e6b7c436f6c756d6e7c436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e546172676574436f6c756d6e4c696e6b7c56616c69646174696f6e546172676574526f777365744c696e6b7c56616c69646174696f6e546172676574526f777365744c696e6b49647c72657175697265640a656e746974797c56616c69646174696f6e546172676574436f6c756d6e5479706545786163747c56616c69646174696f6e546172676574436f6c756d6e5479706545786163744c6973740a70726f70657274797c56616c69646174696f6e546172676574436f6c756d6e5479706545786163747c536f757263654d657461446174615479706549647c72657175697265640a70726f70657274797c56616c69646174696f6e546172676574436f6c756d6e5479706545786163747c5461726765744d657461446174615479706549647c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e546172676574436f6c756d6e5479706545786163747c56616c69646174696f6e546172676574436f6c756d6e4c696e6b7c56616c69646174696f6e546172676574436f6c756d6e4c696e6b49647c72657175697265640a656e746974797c56616c69646174696f6e54617267657449676e6f726564436f6c756d6e7c56616c69646174696f6e54617267657449676e6f726564436f6c756d6e4c6973740a70726f70657274797c56616c69646174696f6e54617267657449676e6f726564436f6c756d6e7c4d657461536368656d614669656c6449647c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e54617267657449676e6f726564436f6c756d6e7c56616c69646174696f6e546172676574526f777365744c696e6b7c56616c69646174696f6e546172676574526f777365744c696e6b49647c72657175697265640a656e746974797c56616c69646174696f6e546172676574526f777365744c696e6b7c56616c69646174696f6e546172676574526f777365744c696e6b4c6973740a70726f70657274797c56616c69646174696f6e546172676574526f777365744c696e6b7c4d657461536368656d615461626c6549647c72657175697265640a70726f70657274797c56616c69646174696f6e546172676574526f777365744c696e6b7c5265736f6c76656453716c4964656e7469666965727c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e546172676574526f777365744c696e6b7c526f777365747c526f7773657449647c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e546172676574526f777365744c696e6b7c5472616e73666f726d42696e64696e675461726765747c5472616e73666f726d42696e64696e6754617267657449647c72657175697265640a72656c6174696f6e736869707c56616c69646174696f6e546172676574526f777365744c696e6b7c56616c69646174696f6e7c56616c69646174696f6e49647c72657175697265640a656e746974797c57726974657c57726974654c6973740a72656c6174696f6e736869707c57726974657c56616c69646174696f6e546172676574526f777365744c696e6b7c56616c69646174696f6e546172676574526f777365744c696e6b49647c72657175697265640a656e746974797c577269746556616c75657c577269746556616c75654c6973740a72656c6174696f6e736869707c577269746556616c75657c56616c69646174696f6e546172676574436f6c756d6e4c696e6b7c56616c69646174696f6e546172676574436f6c756d6e4c696e6b49647c72657175697265640a72656c6174696f6e736869707c577269746556616c75657c57726974657c577269746549647c72657175697265640a656e746974797c577269746556616c75655363616c617245787072657373696f6e7c577269746556616c75655363616c617245787072657373696f6e4c6973740a70726f70657274797c577269746556616c75655363616c617245787072657373696f6e7c4d6574615472616e73666f726d5363726970745363616c617245787072657373696f6e49647c72657175697265640a72656c6174696f6e736869707c577269746556616c75655363616c617245787072657373696f6e7c577269746556616c75657c577269746556616c756549647c7265717569726564";
    public global::MetaTransformBinding.MetaTransformBindingModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaTransformBinding.MetaTransformBindingModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.Column>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Column"))
        {
            var row = new global::MetaTransformBinding.Column
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.ColumnList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.ColumnReference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ColumnReference"))
        {
            var row = new global::MetaTransformBinding.ColumnReference
            {
                Id = source.Id
            };
            row.MetaTransformScriptColumnReferenceId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptColumnReferenceId");
            typed.ColumnReferenceList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.Delete>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Delete"))
        {
            var row = new global::MetaTransformBinding.Delete
            {
                Id = source.Id
            };
            row.MetaTransformScriptDeleteStatementId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptDeleteStatementId");
            typed.DeleteList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.InsertQueryWrite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "InsertQueryWrite"))
        {
            var row = new global::MetaTransformBinding.InsertQueryWrite
            {
                Id = source.Id
            };
            row.MetaTransformScriptQueryExpressionId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptQueryExpressionId");
            typed.InsertQueryWriteList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.InsertValuesWrite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "InsertValuesWrite"))
        {
            var row = new global::MetaTransformBinding.InsertValuesWrite
            {
                Id = source.Id
            };
            row.MetaTransformScriptRowValueId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptRowValueId");
            typed.InsertValuesWriteList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.MergeDelete>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MergeDelete"))
        {
            var row = new global::MetaTransformBinding.MergeDelete
            {
                Id = source.Id
            };
            row.MetaTransformScriptMergeDeleteActionId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptMergeDeleteActionId");
            typed.MergeDeleteList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.MergeInsertWrite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MergeInsertWrite"))
        {
            var row = new global::MetaTransformBinding.MergeInsertWrite
            {
                Id = source.Id
            };
            row.MetaTransformScriptMergeInsertActionId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptMergeInsertActionId");
            typed.MergeInsertWriteList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.MergeUpdateWrite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MergeUpdateWrite"))
        {
            var row = new global::MetaTransformBinding.MergeUpdateWrite
            {
                Id = source.Id
            };
            row.MetaTransformScriptMergeUpdateActionId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptMergeUpdateActionId");
            typed.MergeUpdateWriteList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.OutputRowset>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "OutputRowset"))
        {
            var row = new global::MetaTransformBinding.OutputRowset
            {
                Id = source.Id
            };
            typed.OutputRowsetList.Add(row);
            index8.Add(source.Id, row);
        }

        var index9 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.Rowset>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Rowset"))
        {
            var row = new global::MetaTransformBinding.Rowset
            {
                Id = source.Id
            };
            row.DerivationKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DerivationKind");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.SqlIdentifier = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "SqlIdentifier");
            typed.RowsetList.Add(row);
            index9.Add(source.Id, row);
        }

        var index10 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.SourceTarget>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SourceTarget"))
        {
            var row = new global::MetaTransformBinding.SourceTarget
            {
                Id = source.Id
            };
            row.InputRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "InputRole");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.SourceTargetList.Add(row);
            index10.Add(source.Id, row);
        }

        var index11 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.StoredProcedureOperationBinding>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "StoredProcedureOperationBinding"))
        {
            var row = new global::MetaTransformBinding.StoredProcedureOperationBinding
            {
                Id = source.Id
            };
            row.MetaTransformScriptStoredProcedureContractOperationId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptStoredProcedureContractOperationId");
            typed.StoredProcedureOperationBindingList.Add(row);
            index11.Add(source.Id, row);
        }

        var index12 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.TableSource>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableSource"))
        {
            var row = new global::MetaTransformBinding.TableSource
            {
                Id = source.Id
            };
            row.ExposedName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ExposedName");
            row.MetaTransformScriptTableReferenceId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptTableReferenceId");
            typed.TableSourceList.Add(row);
            index12.Add(source.Id, row);
        }

        var index13 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.TargetColumnReference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TargetColumnReference"))
        {
            var row = new global::MetaTransformBinding.TargetColumnReference
            {
                Id = source.Id
            };
            row.MetaSchemaFieldId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaSchemaFieldId");
            row.MetaTransformScriptColumnReferenceId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptColumnReferenceId");
            typed.TargetColumnReferenceList.Add(row);
            index13.Add(source.Id, row);
        }

        var index14 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.TransformBinding>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformBinding"))
        {
            var row = new global::MetaTransformBinding.TransformBinding
            {
                Id = source.Id
            };
            row.MetaTransformScriptTransformScriptId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptTransformScriptId");
            row.TransformScriptName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TransformScriptName");
            typed.TransformBindingList.Add(row);
            index14.Add(source.Id, row);
        }

        var index15 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.TransformBindingTarget>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformBindingTarget"))
        {
            var row = new global::MetaTransformBinding.TransformBindingTarget
            {
                Id = source.Id
            };
            row.SqlIdentifier = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SqlIdentifier");
            typed.TransformBindingTargetList.Add(row);
            index15.Add(source.Id, row);
        }

        var index16 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.Truncate>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Truncate"))
        {
            var row = new global::MetaTransformBinding.Truncate
            {
                Id = source.Id
            };
            row.MetaTransformScriptTruncateStatementId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptTruncateStatementId");
            typed.TruncateList.Add(row);
            index16.Add(source.Id, row);
        }

        var index17 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.UpdateWrite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "UpdateWrite"))
        {
            var row = new global::MetaTransformBinding.UpdateWrite
            {
                Id = source.Id
            };
            row.MetaTransformScriptSetClauseId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptSetClauseId");
            typed.UpdateWriteList.Add(row);
            index17.Add(source.Id, row);
        }

        var index18 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.Validation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Validation"))
        {
            var row = new global::MetaTransformBinding.Validation
            {
                Id = source.Id
            };
            typed.ValidationList.Add(row);
            index18.Add(source.Id, row);
        }

        var index19 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.ValidationSourceColumnLink>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationSourceColumnLink"))
        {
            var row = new global::MetaTransformBinding.ValidationSourceColumnLink
            {
                Id = source.Id
            };
            row.MetaSchemaFieldId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaSchemaFieldId");
            typed.ValidationSourceColumnLinkList.Add(row);
            index19.Add(source.Id, row);
        }

        var index20 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.ValidationSourceRowsetLink>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationSourceRowsetLink"))
        {
            var row = new global::MetaTransformBinding.ValidationSourceRowsetLink
            {
                Id = source.Id
            };
            row.MetaSchemaTableId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaSchemaTableId");
            row.ResolvedSqlIdentifier = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ResolvedSqlIdentifier");
            typed.ValidationSourceRowsetLinkList.Add(row);
            index20.Add(source.Id, row);
        }

        var index21 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.ValidationStoredProcedureOperationLink>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationStoredProcedureOperationLink"))
        {
            var row = new global::MetaTransformBinding.ValidationStoredProcedureOperationLink
            {
                Id = source.Id
            };
            row.MetaSchemaTableId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaSchemaTableId");
            row.ResolvedSqlIdentifier = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ResolvedSqlIdentifier");
            typed.ValidationStoredProcedureOperationLinkList.Add(row);
            index21.Add(source.Id, row);
        }

        var index22 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.ValidationTargetColumnLink>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationTargetColumnLink"))
        {
            var row = new global::MetaTransformBinding.ValidationTargetColumnLink
            {
                Id = source.Id
            };
            row.MetaSchemaFieldId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaSchemaFieldId");
            typed.ValidationTargetColumnLinkList.Add(row);
            index22.Add(source.Id, row);
        }

        var index23 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.ValidationTargetColumnTypeExact>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationTargetColumnTypeExact"))
        {
            var row = new global::MetaTransformBinding.ValidationTargetColumnTypeExact
            {
                Id = source.Id
            };
            row.SourceMetaDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceMetaDataTypeId");
            row.TargetMetaDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TargetMetaDataTypeId");
            typed.ValidationTargetColumnTypeExactList.Add(row);
            index23.Add(source.Id, row);
        }

        var index24 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.ValidationTargetIgnoredColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationTargetIgnoredColumn"))
        {
            var row = new global::MetaTransformBinding.ValidationTargetIgnoredColumn
            {
                Id = source.Id
            };
            row.MetaSchemaFieldId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaSchemaFieldId");
            typed.ValidationTargetIgnoredColumnList.Add(row);
            index24.Add(source.Id, row);
        }

        var index25 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.ValidationTargetRowsetLink>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationTargetRowsetLink"))
        {
            var row = new global::MetaTransformBinding.ValidationTargetRowsetLink
            {
                Id = source.Id
            };
            row.MetaSchemaTableId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaSchemaTableId");
            row.ResolvedSqlIdentifier = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ResolvedSqlIdentifier");
            typed.ValidationTargetRowsetLinkList.Add(row);
            index25.Add(source.Id, row);
        }

        var index26 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.Write>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Write"))
        {
            var row = new global::MetaTransformBinding.Write
            {
                Id = source.Id
            };
            typed.WriteList.Add(row);
            index26.Add(source.Id, row);
        }

        var index27 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.WriteValue>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "WriteValue"))
        {
            var row = new global::MetaTransformBinding.WriteValue
            {
                Id = source.Id
            };
            typed.WriteValueList.Add(row);
            index27.Add(source.Id, row);
        }

        var index28 = new global::System.Collections.Generic.Dictionary<string, global::MetaTransformBinding.WriteValueScalarExpression>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "WriteValueScalarExpression"))
        {
            var row = new global::MetaTransformBinding.WriteValueScalarExpression
            {
                Id = source.Id
            };
            row.MetaTransformScriptScalarExpressionId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaTransformScriptScalarExpressionId");
            typed.WriteValueScalarExpressionList.Add(row);
            index28.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Column"))
        {
            var row = index0[source.Id];
            row.Rowset = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RowsetId", index9);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ColumnReference"))
        {
            var row = index1[source.Id];
            row.Column = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ColumnId", index0);
            row.TableSource = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableSourceId", index12);
            row.TransformBinding = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformBindingId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Delete"))
        {
            var row = index2[source.Id];
            row.ValidationTargetRowsetLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationTargetRowsetLinkId", index25);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "InsertQueryWrite"))
        {
            var row = index3[source.Id];
            row.Write = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "WriteId", index26);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "InsertValuesWrite"))
        {
            var row = index4[source.Id];
            row.Write = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "WriteId", index26);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MergeDelete"))
        {
            var row = index5[source.Id];
            row.ValidationTargetRowsetLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationTargetRowsetLinkId", index25);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MergeInsertWrite"))
        {
            var row = index6[source.Id];
            row.Write = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "WriteId", index26);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MergeUpdateWrite"))
        {
            var row = index7[source.Id];
            row.Write = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "WriteId", index26);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "OutputRowset"))
        {
            var row = index8[source.Id];
            row.Rowset = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RowsetId", index9);
            row.TransformBinding = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformBindingId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Rowset"))
        {
            var row = index9[source.Id];
            row.TransformBinding = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformBindingId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SourceTarget"))
        {
            var row = index10[source.Id];
            row.Source = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SourceId", index9);
            row.Target = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TargetId", index9);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "StoredProcedureOperationBinding"))
        {
            var row = index11[source.Id];
            row.Rowset = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RowsetId", index9);
            row.TransformBinding = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformBindingId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableSource"))
        {
            var row = index12[source.Id];
            row.Rowset = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RowsetId", index9);
            row.TransformBinding = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformBindingId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TargetColumnReference"))
        {
            var row = index13[source.Id];
            row.Column = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ColumnId", index0);
            row.TransformBindingTarget = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformBindingTargetId", index15);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransformBindingTarget"))
        {
            var row = index15[source.Id];
            row.TransformBinding = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformBindingId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Truncate"))
        {
            var row = index16[source.Id];
            row.ValidationTargetRowsetLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationTargetRowsetLinkId", index25);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "UpdateWrite"))
        {
            var row = index17[source.Id];
            row.Write = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "WriteId", index26);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Validation"))
        {
            var row = index18[source.Id];
            row.TransformBinding = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformBindingId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationSourceColumnLink"))
        {
            var row = index19[source.Id];
            row.Column = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ColumnId", index0);
            row.ValidationSourceRowsetLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationSourceRowsetLinkId", index20);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationSourceRowsetLink"))
        {
            var row = index20[source.Id];
            row.Rowset = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RowsetId", index9);
            row.Validation = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationStoredProcedureOperationLink"))
        {
            var row = index21[source.Id];
            row.StoredProcedureOperationBinding = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "StoredProcedureOperationBindingId", index11);
            row.Validation = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationTargetColumnLink"))
        {
            var row = index22[source.Id];
            row.Column = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ColumnId", index0);
            row.ValidationTargetRowsetLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationTargetRowsetLinkId", index25);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationTargetColumnTypeExact"))
        {
            var row = index23[source.Id];
            row.ValidationTargetColumnLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationTargetColumnLinkId", index22);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationTargetIgnoredColumn"))
        {
            var row = index24[source.Id];
            row.ValidationTargetRowsetLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationTargetRowsetLinkId", index25);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ValidationTargetRowsetLink"))
        {
            var row = index25[source.Id];
            row.Rowset = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RowsetId", index9);
            row.TransformBindingTarget = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TransformBindingTargetId", index15);
            row.Validation = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Write"))
        {
            var row = index26[source.Id];
            row.ValidationTargetRowsetLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationTargetRowsetLinkId", index25);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "WriteValue"))
        {
            var row = index27[source.Id];
            row.ValidationTargetColumnLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ValidationTargetColumnLinkId", index22);
            row.Write = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "WriteId", index26);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "WriteValueScalarExpression"))
        {
            var row = index28[source.Id];
            row.WriteValue = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "WriteValueId", index27);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaTransformBinding.MetaTransformBindingModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ColumnList, row => row.Id, "Column");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ColumnReferenceList, row => row.Id, "ColumnReference");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DeleteList, row => row.Id, "Delete");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.InsertQueryWriteList, row => row.Id, "InsertQueryWrite");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.InsertValuesWriteList, row => row.Id, "InsertValuesWrite");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.MergeDeleteList, row => row.Id, "MergeDelete");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.MergeInsertWriteList, row => row.Id, "MergeInsertWrite");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.MergeUpdateWriteList, row => row.Id, "MergeUpdateWrite");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.OutputRowsetList, row => row.Id, "OutputRowset");
        var index9 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RowsetList, row => row.Id, "Rowset");
        var index10 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SourceTargetList, row => row.Id, "SourceTarget");
        var index11 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.StoredProcedureOperationBindingList, row => row.Id, "StoredProcedureOperationBinding");
        var index12 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TableSourceList, row => row.Id, "TableSource");
        var index13 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TargetColumnReferenceList, row => row.Id, "TargetColumnReference");
        var index14 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TransformBindingList, row => row.Id, "TransformBinding");
        var index15 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TransformBindingTargetList, row => row.Id, "TransformBindingTarget");
        var index16 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TruncateList, row => row.Id, "Truncate");
        var index17 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.UpdateWriteList, row => row.Id, "UpdateWrite");
        var index18 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ValidationList, row => row.Id, "Validation");
        var index19 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ValidationSourceColumnLinkList, row => row.Id, "ValidationSourceColumnLink");
        var index20 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ValidationSourceRowsetLinkList, row => row.Id, "ValidationSourceRowsetLink");
        var index21 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ValidationStoredProcedureOperationLinkList, row => row.Id, "ValidationStoredProcedureOperationLink");
        var index22 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ValidationTargetColumnLinkList, row => row.Id, "ValidationTargetColumnLink");
        var index23 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ValidationTargetColumnTypeExactList, row => row.Id, "ValidationTargetColumnTypeExact");
        var index24 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ValidationTargetIgnoredColumnList, row => row.Id, "ValidationTargetIgnoredColumn");
        var index25 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ValidationTargetRowsetLinkList, row => row.Id, "ValidationTargetRowsetLink");
        var index26 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.WriteList, row => row.Id, "Write");
        var index27 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.WriteValueList, row => row.Id, "WriteValue");
        var index28 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.WriteValueScalarExpressionList, row => row.Id, "WriteValueScalarExpression");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Rowset is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Rowset, row.Rowset.Id, index9, "Column.Rowset");
                target.RelationshipIds.Add("RowsetId", row.Rowset.Id);
            }

            instance.GetOrCreateEntityRecords("Column").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptColumnReferenceId is not null)
                target.Values.Add("MetaTransformScriptColumnReferenceId", row.MetaTransformScriptColumnReferenceId);
            if (row.Column is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Column, row.Column.Id, index0, "ColumnReference.Column");
                target.RelationshipIds.Add("ColumnId", row.Column.Id);
            }

            if (row.TableSource is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TableSource, row.TableSource.Id, index12, "ColumnReference.TableSource");
                target.RelationshipIds.Add("TableSourceId", row.TableSource.Id);
            }

            if (row.TransformBinding is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformBinding, row.TransformBinding.Id, index14, "ColumnReference.TransformBinding");
                target.RelationshipIds.Add("TransformBindingId", row.TransformBinding.Id);
            }

            instance.GetOrCreateEntityRecords("ColumnReference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptDeleteStatementId is not null)
                target.Values.Add("MetaTransformScriptDeleteStatementId", row.MetaTransformScriptDeleteStatementId);
            if (row.ValidationTargetRowsetLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ValidationTargetRowsetLink, row.ValidationTargetRowsetLink.Id, index25, "Delete.ValidationTargetRowsetLink");
                target.RelationshipIds.Add("ValidationTargetRowsetLinkId", row.ValidationTargetRowsetLink.Id);
            }

            instance.GetOrCreateEntityRecords("Delete").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptQueryExpressionId is not null)
                target.Values.Add("MetaTransformScriptQueryExpressionId", row.MetaTransformScriptQueryExpressionId);
            if (row.Write is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Write, row.Write.Id, index26, "InsertQueryWrite.Write");
                target.RelationshipIds.Add("WriteId", row.Write.Id);
            }

            instance.GetOrCreateEntityRecords("InsertQueryWrite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptRowValueId is not null)
                target.Values.Add("MetaTransformScriptRowValueId", row.MetaTransformScriptRowValueId);
            if (row.Write is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Write, row.Write.Id, index26, "InsertValuesWrite.Write");
                target.RelationshipIds.Add("WriteId", row.Write.Id);
            }

            instance.GetOrCreateEntityRecords("InsertValuesWrite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptMergeDeleteActionId is not null)
                target.Values.Add("MetaTransformScriptMergeDeleteActionId", row.MetaTransformScriptMergeDeleteActionId);
            if (row.ValidationTargetRowsetLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ValidationTargetRowsetLink, row.ValidationTargetRowsetLink.Id, index25, "MergeDelete.ValidationTargetRowsetLink");
                target.RelationshipIds.Add("ValidationTargetRowsetLinkId", row.ValidationTargetRowsetLink.Id);
            }

            instance.GetOrCreateEntityRecords("MergeDelete").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptMergeInsertActionId is not null)
                target.Values.Add("MetaTransformScriptMergeInsertActionId", row.MetaTransformScriptMergeInsertActionId);
            if (row.Write is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Write, row.Write.Id, index26, "MergeInsertWrite.Write");
                target.RelationshipIds.Add("WriteId", row.Write.Id);
            }

            instance.GetOrCreateEntityRecords("MergeInsertWrite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptMergeUpdateActionId is not null)
                target.Values.Add("MetaTransformScriptMergeUpdateActionId", row.MetaTransformScriptMergeUpdateActionId);
            if (row.Write is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Write, row.Write.Id, index26, "MergeUpdateWrite.Write");
                target.RelationshipIds.Add("WriteId", row.Write.Id);
            }

            instance.GetOrCreateEntityRecords("MergeUpdateWrite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index8))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Rowset is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Rowset, row.Rowset.Id, index9, "OutputRowset.Rowset");
                target.RelationshipIds.Add("RowsetId", row.Rowset.Id);
            }

            if (row.TransformBinding is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformBinding, row.TransformBinding.Id, index14, "OutputRowset.TransformBinding");
                target.RelationshipIds.Add("TransformBindingId", row.TransformBinding.Id);
            }

            instance.GetOrCreateEntityRecords("OutputRowset").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index9))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DerivationKind is not null)
                target.Values.Add("DerivationKind", row.DerivationKind);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.SqlIdentifier is not null)
                target.Values.Add("SqlIdentifier", row.SqlIdentifier);
            if (row.TransformBinding is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformBinding, row.TransformBinding.Id, index14, "Rowset.TransformBinding");
                target.RelationshipIds.Add("TransformBindingId", row.TransformBinding.Id);
            }

            instance.GetOrCreateEntityRecords("Rowset").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index10))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.InputRole is not null)
                target.Values.Add("InputRole", row.InputRole);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Source is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Source, row.Source.Id, index9, "SourceTarget.Source");
                target.RelationshipIds.Add("SourceId", row.Source.Id);
            }

            if (row.Target is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Target, row.Target.Id, index9, "SourceTarget.Target");
                target.RelationshipIds.Add("TargetId", row.Target.Id);
            }

            instance.GetOrCreateEntityRecords("SourceTarget").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index11))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptStoredProcedureContractOperationId is not null)
                target.Values.Add("MetaTransformScriptStoredProcedureContractOperationId", row.MetaTransformScriptStoredProcedureContractOperationId);
            if (row.Rowset is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Rowset, row.Rowset.Id, index9, "StoredProcedureOperationBinding.Rowset");
                target.RelationshipIds.Add("RowsetId", row.Rowset.Id);
            }

            if (row.TransformBinding is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformBinding, row.TransformBinding.Id, index14, "StoredProcedureOperationBinding.TransformBinding");
                target.RelationshipIds.Add("TransformBindingId", row.TransformBinding.Id);
            }

            instance.GetOrCreateEntityRecords("StoredProcedureOperationBinding").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index12))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.ExposedName is not null)
                target.Values.Add("ExposedName", row.ExposedName);
            if (row.MetaTransformScriptTableReferenceId is not null)
                target.Values.Add("MetaTransformScriptTableReferenceId", row.MetaTransformScriptTableReferenceId);
            if (row.Rowset is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Rowset, row.Rowset.Id, index9, "TableSource.Rowset");
                target.RelationshipIds.Add("RowsetId", row.Rowset.Id);
            }

            if (row.TransformBinding is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformBinding, row.TransformBinding.Id, index14, "TableSource.TransformBinding");
                target.RelationshipIds.Add("TransformBindingId", row.TransformBinding.Id);
            }

            instance.GetOrCreateEntityRecords("TableSource").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index13))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaSchemaFieldId is not null)
                target.Values.Add("MetaSchemaFieldId", row.MetaSchemaFieldId);
            if (row.MetaTransformScriptColumnReferenceId is not null)
                target.Values.Add("MetaTransformScriptColumnReferenceId", row.MetaTransformScriptColumnReferenceId);
            if (row.Column is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Column, row.Column.Id, index0, "TargetColumnReference.Column");
                target.RelationshipIds.Add("ColumnId", row.Column.Id);
            }

            if (row.TransformBindingTarget is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformBindingTarget, row.TransformBindingTarget.Id, index15, "TargetColumnReference.TransformBindingTarget");
                target.RelationshipIds.Add("TransformBindingTargetId", row.TransformBindingTarget.Id);
            }

            instance.GetOrCreateEntityRecords("TargetColumnReference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index14))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptTransformScriptId is not null)
                target.Values.Add("MetaTransformScriptTransformScriptId", row.MetaTransformScriptTransformScriptId);
            if (row.TransformScriptName is not null)
                target.Values.Add("TransformScriptName", row.TransformScriptName);
            instance.GetOrCreateEntityRecords("TransformBinding").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index15))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SqlIdentifier is not null)
                target.Values.Add("SqlIdentifier", row.SqlIdentifier);
            if (row.TransformBinding is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformBinding, row.TransformBinding.Id, index14, "TransformBindingTarget.TransformBinding");
                target.RelationshipIds.Add("TransformBindingId", row.TransformBinding.Id);
            }

            instance.GetOrCreateEntityRecords("TransformBindingTarget").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index16))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptTruncateStatementId is not null)
                target.Values.Add("MetaTransformScriptTruncateStatementId", row.MetaTransformScriptTruncateStatementId);
            if (row.ValidationTargetRowsetLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ValidationTargetRowsetLink, row.ValidationTargetRowsetLink.Id, index25, "Truncate.ValidationTargetRowsetLink");
                target.RelationshipIds.Add("ValidationTargetRowsetLinkId", row.ValidationTargetRowsetLink.Id);
            }

            instance.GetOrCreateEntityRecords("Truncate").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index17))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptSetClauseId is not null)
                target.Values.Add("MetaTransformScriptSetClauseId", row.MetaTransformScriptSetClauseId);
            if (row.Write is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Write, row.Write.Id, index26, "UpdateWrite.Write");
                target.RelationshipIds.Add("WriteId", row.Write.Id);
            }

            instance.GetOrCreateEntityRecords("UpdateWrite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index18))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.TransformBinding is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformBinding, row.TransformBinding.Id, index14, "Validation.TransformBinding");
                target.RelationshipIds.Add("TransformBindingId", row.TransformBinding.Id);
            }

            instance.GetOrCreateEntityRecords("Validation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index19))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaSchemaFieldId is not null)
                target.Values.Add("MetaSchemaFieldId", row.MetaSchemaFieldId);
            if (row.Column is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Column, row.Column.Id, index0, "ValidationSourceColumnLink.Column");
                target.RelationshipIds.Add("ColumnId", row.Column.Id);
            }

            if (row.ValidationSourceRowsetLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ValidationSourceRowsetLink, row.ValidationSourceRowsetLink.Id, index20, "ValidationSourceColumnLink.ValidationSourceRowsetLink");
                target.RelationshipIds.Add("ValidationSourceRowsetLinkId", row.ValidationSourceRowsetLink.Id);
            }

            instance.GetOrCreateEntityRecords("ValidationSourceColumnLink").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index20))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaSchemaTableId is not null)
                target.Values.Add("MetaSchemaTableId", row.MetaSchemaTableId);
            if (row.ResolvedSqlIdentifier is not null)
                target.Values.Add("ResolvedSqlIdentifier", row.ResolvedSqlIdentifier);
            if (row.Rowset is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Rowset, row.Rowset.Id, index9, "ValidationSourceRowsetLink.Rowset");
                target.RelationshipIds.Add("RowsetId", row.Rowset.Id);
            }

            if (row.Validation is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Validation, row.Validation.Id, index18, "ValidationSourceRowsetLink.Validation");
                target.RelationshipIds.Add("ValidationId", row.Validation.Id);
            }

            instance.GetOrCreateEntityRecords("ValidationSourceRowsetLink").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index21))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaSchemaTableId is not null)
                target.Values.Add("MetaSchemaTableId", row.MetaSchemaTableId);
            if (row.ResolvedSqlIdentifier is not null)
                target.Values.Add("ResolvedSqlIdentifier", row.ResolvedSqlIdentifier);
            if (row.StoredProcedureOperationBinding is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.StoredProcedureOperationBinding, row.StoredProcedureOperationBinding.Id, index11, "ValidationStoredProcedureOperationLink.StoredProcedureOperationBinding");
                target.RelationshipIds.Add("StoredProcedureOperationBindingId", row.StoredProcedureOperationBinding.Id);
            }

            if (row.Validation is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Validation, row.Validation.Id, index18, "ValidationStoredProcedureOperationLink.Validation");
                target.RelationshipIds.Add("ValidationId", row.Validation.Id);
            }

            instance.GetOrCreateEntityRecords("ValidationStoredProcedureOperationLink").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index22))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaSchemaFieldId is not null)
                target.Values.Add("MetaSchemaFieldId", row.MetaSchemaFieldId);
            if (row.Column is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Column, row.Column.Id, index0, "ValidationTargetColumnLink.Column");
                target.RelationshipIds.Add("ColumnId", row.Column.Id);
            }

            if (row.ValidationTargetRowsetLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ValidationTargetRowsetLink, row.ValidationTargetRowsetLink.Id, index25, "ValidationTargetColumnLink.ValidationTargetRowsetLink");
                target.RelationshipIds.Add("ValidationTargetRowsetLinkId", row.ValidationTargetRowsetLink.Id);
            }

            instance.GetOrCreateEntityRecords("ValidationTargetColumnLink").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index23))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SourceMetaDataTypeId is not null)
                target.Values.Add("SourceMetaDataTypeId", row.SourceMetaDataTypeId);
            if (row.TargetMetaDataTypeId is not null)
                target.Values.Add("TargetMetaDataTypeId", row.TargetMetaDataTypeId);
            if (row.ValidationTargetColumnLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ValidationTargetColumnLink, row.ValidationTargetColumnLink.Id, index22, "ValidationTargetColumnTypeExact.ValidationTargetColumnLink");
                target.RelationshipIds.Add("ValidationTargetColumnLinkId", row.ValidationTargetColumnLink.Id);
            }

            instance.GetOrCreateEntityRecords("ValidationTargetColumnTypeExact").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index24))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaSchemaFieldId is not null)
                target.Values.Add("MetaSchemaFieldId", row.MetaSchemaFieldId);
            if (row.ValidationTargetRowsetLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ValidationTargetRowsetLink, row.ValidationTargetRowsetLink.Id, index25, "ValidationTargetIgnoredColumn.ValidationTargetRowsetLink");
                target.RelationshipIds.Add("ValidationTargetRowsetLinkId", row.ValidationTargetRowsetLink.Id);
            }

            instance.GetOrCreateEntityRecords("ValidationTargetIgnoredColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index25))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaSchemaTableId is not null)
                target.Values.Add("MetaSchemaTableId", row.MetaSchemaTableId);
            if (row.ResolvedSqlIdentifier is not null)
                target.Values.Add("ResolvedSqlIdentifier", row.ResolvedSqlIdentifier);
            if (row.Rowset is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Rowset, row.Rowset.Id, index9, "ValidationTargetRowsetLink.Rowset");
                target.RelationshipIds.Add("RowsetId", row.Rowset.Id);
            }

            if (row.TransformBindingTarget is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TransformBindingTarget, row.TransformBindingTarget.Id, index15, "ValidationTargetRowsetLink.TransformBindingTarget");
                target.RelationshipIds.Add("TransformBindingTargetId", row.TransformBindingTarget.Id);
            }

            if (row.Validation is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Validation, row.Validation.Id, index18, "ValidationTargetRowsetLink.Validation");
                target.RelationshipIds.Add("ValidationId", row.Validation.Id);
            }

            instance.GetOrCreateEntityRecords("ValidationTargetRowsetLink").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index26))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.ValidationTargetRowsetLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ValidationTargetRowsetLink, row.ValidationTargetRowsetLink.Id, index25, "Write.ValidationTargetRowsetLink");
                target.RelationshipIds.Add("ValidationTargetRowsetLinkId", row.ValidationTargetRowsetLink.Id);
            }

            instance.GetOrCreateEntityRecords("Write").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index27))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.ValidationTargetColumnLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ValidationTargetColumnLink, row.ValidationTargetColumnLink.Id, index22, "WriteValue.ValidationTargetColumnLink");
                target.RelationshipIds.Add("ValidationTargetColumnLinkId", row.ValidationTargetColumnLink.Id);
            }

            if (row.Write is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Write, row.Write.Id, index26, "WriteValue.Write");
                target.RelationshipIds.Add("WriteId", row.Write.Id);
            }

            instance.GetOrCreateEntityRecords("WriteValue").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index28))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaTransformScriptScalarExpressionId is not null)
                target.Values.Add("MetaTransformScriptScalarExpressionId", row.MetaTransformScriptScalarExpressionId);
            if (row.WriteValue is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.WriteValue, row.WriteValue.Id, index27, "WriteValueScalarExpression.WriteValue");
                target.RelationshipIds.Add("WriteValueId", row.WriteValue.Id);
            }

            instance.GetOrCreateEntityRecords("WriteValueScalarExpression").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}