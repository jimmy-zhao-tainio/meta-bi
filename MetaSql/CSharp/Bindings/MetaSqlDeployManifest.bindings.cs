// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaSqlDeployManifest;
public sealed class MetaSqlDeployManifestWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaSqlDeployManifest.MetaSqlDeployManifestModel>
{
    private const string ContractSignature = "6d6f64656c7c4d65746153716c4465706c6f794d616e69666573740a656e746974797c416464466f726569676e4b65797c416464466f726569676e4b65794c6973740a70726f70657274797c416464466f726569676e4b65797c536f75726365466f726569676e4b657949647c72657175697265640a72656c6174696f6e736869707c416464466f726569676e4b65797c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c41646446756e6374696f6e7c41646446756e6374696f6e4c6973740a70726f70657274797c41646446756e6374696f6e7c536f7572636546756e6374696f6e49647c72657175697265640a72656c6174696f6e736869707c41646446756e6374696f6e7c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c416464496e6465787c416464496e6465784c6973740a70726f70657274797c416464496e6465787c536f75726365496e64657849647c72657175697265640a72656c6174696f6e736869707c416464496e6465787c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c4164645072696d6172794b65797c4164645072696d6172794b65794c6973740a70726f70657274797c4164645072696d6172794b65797c536f757263655072696d6172794b657949647c72657175697265640a72656c6174696f6e736869707c4164645072696d6172794b65797c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c416464536368656d617c416464536368656d614c6973740a70726f70657274797c416464536368656d617c536f75726365536368656d6149647c72657175697265640a72656c6174696f6e736869707c416464536368656d617c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c41646453746f72656450726f6365647572657c41646453746f72656450726f6365647572654c6973740a70726f70657274797c41646453746f72656450726f6365647572657c536f7572636553746f72656450726f63656475726549647c72657175697265640a72656c6174696f6e736869707c41646453746f72656450726f6365647572657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c4164645461626c657c4164645461626c654c6973740a70726f70657274797c4164645461626c657c536f757263655461626c6549647c72657175697265640a72656c6174696f6e736869707c4164645461626c657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c4164645461626c65436f6c756d6e7c4164645461626c65436f6c756d6e4c6973740a70726f70657274797c4164645461626c65436f6c756d6e7c536f757263655461626c65436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c4164645461626c65436f6c756d6e7c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c416464566965777c416464566965774c6973740a70726f70657274797c416464566965777c536f757263655669657749647c72657175697265640a72656c6174696f6e736869707c416464566965777c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c416c7465725461626c65436f6c756d6e7c416c7465725461626c65436f6c756d6e4c6973740a70726f70657274797c416c7465725461626c65436f6c756d6e7c4c6976655461626c65436f6c756d6e49647c72657175697265640a70726f70657274797c416c7465725461626c65436f6c756d6e7c536f757263655461626c65436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c416c7465725461626c65436f6c756d6e7c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c426c6f636b466f726569676e4b6579446966666572656e63657c426c6f636b466f726569676e4b6579446966666572656e63654c6973740a70726f70657274797c426c6f636b466f726569676e4b6579446966666572656e63657c446966666572656e636553756d6d6172797c72657175697265640a70726f70657274797c426c6f636b466f726569676e4b6579446966666572656e63657c4c697665466f726569676e4b657949647c72657175697265640a70726f70657274797c426c6f636b466f726569676e4b6579446966666572656e63657c536f75726365466f726569676e4b657949647c72657175697265640a72656c6174696f6e736869707c426c6f636b466f726569676e4b6579446966666572656e63657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c426c6f636b46756e6374696f6e446966666572656e63657c426c6f636b46756e6374696f6e446966666572656e63654c6973740a70726f70657274797c426c6f636b46756e6374696f6e446966666572656e63657c446966666572656e636553756d6d6172797c72657175697265640a70726f70657274797c426c6f636b46756e6374696f6e446966666572656e63657c4c69766546756e6374696f6e49647c72657175697265640a70726f70657274797c426c6f636b46756e6374696f6e446966666572656e63657c536f7572636546756e6374696f6e49647c72657175697265640a72656c6174696f6e736869707c426c6f636b46756e6374696f6e446966666572656e63657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c426c6f636b496e646578446966666572656e63657c426c6f636b496e646578446966666572656e63654c6973740a70726f70657274797c426c6f636b496e646578446966666572656e63657c446966666572656e636553756d6d6172797c72657175697265640a70726f70657274797c426c6f636b496e646578446966666572656e63657c4c697665496e64657849647c72657175697265640a70726f70657274797c426c6f636b496e646578446966666572656e63657c536f75726365496e64657849647c72657175697265640a72656c6174696f6e736869707c426c6f636b496e646578446966666572656e63657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c426c6f636b5072696d6172794b6579446966666572656e63657c426c6f636b5072696d6172794b6579446966666572656e63654c6973740a70726f70657274797c426c6f636b5072696d6172794b6579446966666572656e63657c446966666572656e636553756d6d6172797c72657175697265640a70726f70657274797c426c6f636b5072696d6172794b6579446966666572656e63657c4c6976655072696d6172794b657949647c72657175697265640a70726f70657274797c426c6f636b5072696d6172794b6579446966666572656e63657c536f757263655072696d6172794b657949647c72657175697265640a72656c6174696f6e736869707c426c6f636b5072696d6172794b6579446966666572656e63657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c426c6f636b53746f72656450726f636564757265446966666572656e63657c426c6f636b53746f72656450726f636564757265446966666572656e63654c6973740a70726f70657274797c426c6f636b53746f72656450726f636564757265446966666572656e63657c446966666572656e636553756d6d6172797c72657175697265640a70726f70657274797c426c6f636b53746f72656450726f636564757265446966666572656e63657c4c69766553746f72656450726f63656475726549647c72657175697265640a70726f70657274797c426c6f636b53746f72656450726f636564757265446966666572656e63657c536f7572636553746f72656450726f63656475726549647c72657175697265640a72656c6174696f6e736869707c426c6f636b53746f72656450726f636564757265446966666572656e63657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c426c6f636b5461626c65436f6c756d6e446966666572656e63657c426c6f636b5461626c65436f6c756d6e446966666572656e63654c6973740a70726f70657274797c426c6f636b5461626c65436f6c756d6e446966666572656e63657c446966666572656e636553756d6d6172797c72657175697265640a70726f70657274797c426c6f636b5461626c65436f6c756d6e446966666572656e63657c4c6976655461626c65436f6c756d6e49647c72657175697265640a70726f70657274797c426c6f636b5461626c65436f6c756d6e446966666572656e63657c536f757263655461626c65436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c426c6f636b5461626c65436f6c756d6e446966666572656e63657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c426c6f636b5461626c65446966666572656e63657c426c6f636b5461626c65446966666572656e63654c6973740a70726f70657274797c426c6f636b5461626c65446966666572656e63657c446966666572656e636553756d6d6172797c72657175697265640a70726f70657274797c426c6f636b5461626c65446966666572656e63657c4c6976655461626c6549647c72657175697265640a70726f70657274797c426c6f636b5461626c65446966666572656e63657c536f757263655461626c6549647c72657175697265640a72656c6174696f6e736869707c426c6f636b5461626c65446966666572656e63657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c426c6f636b56696577446966666572656e63657c426c6f636b56696577446966666572656e63654c6973740a70726f70657274797c426c6f636b56696577446966666572656e63657c446966666572656e636553756d6d6172797c72657175697265640a70726f70657274797c426c6f636b56696577446966666572656e63657c4c6976655669657749647c72657175697265640a70726f70657274797c426c6f636b56696577446966666572656e63657c536f757263655669657749647c72657175697265640a72656c6174696f6e736869707c426c6f636b56696577446966666572656e63657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c4465706c6f794d616e69666573747c4465706c6f794d616e69666573744c6973740a70726f70657274797c4465706c6f794d616e69666573747c437265617465645574637c72657175697265640a70726f70657274797c4465706c6f794d616e69666573747c45787065637465644c697665446174616261736550726573656e63657c72657175697265640a70726f70657274797c4465706c6f794d616e69666573747c4c697665496e7374616e636546696e6765727072696e747c72657175697265640a70726f70657274797c4465706c6f794d616e69666573747c4e616d657c72657175697265640a70726f70657274797c4465706c6f794d616e69666573747c536f75726365496e7374616e636546696e6765727072696e747c72657175697265640a70726f70657274797c4465706c6f794d616e69666573747c5461726765744465736372697074696f6e7c6e756c6c61626c650a656e746974797c44726f70466f726569676e4b65797c44726f70466f726569676e4b65794c6973740a70726f70657274797c44726f70466f726569676e4b65797c4c697665466f726569676e4b657949647c72657175697265640a72656c6174696f6e736869707c44726f70466f726569676e4b65797c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c44726f7046756e6374696f6e7c44726f7046756e6374696f6e4c6973740a70726f70657274797c44726f7046756e6374696f6e7c4c69766546756e6374696f6e49647c72657175697265640a72656c6174696f6e736869707c44726f7046756e6374696f6e7c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c44726f70496e6465787c44726f70496e6465784c6973740a70726f70657274797c44726f70496e6465787c4c697665496e64657849647c72657175697265640a72656c6174696f6e736869707c44726f70496e6465787c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c44726f705072696d6172794b65797c44726f705072696d6172794b65794c6973740a70726f70657274797c44726f705072696d6172794b65797c4c6976655072696d6172794b657949647c72657175697265640a72656c6174696f6e736869707c44726f705072696d6172794b65797c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c44726f7053746f72656450726f6365647572657c44726f7053746f72656450726f6365647572654c6973740a70726f70657274797c44726f7053746f72656450726f6365647572657c4c69766553746f72656450726f63656475726549647c72657175697265640a72656c6174696f6e736869707c44726f7053746f72656450726f6365647572657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c44726f705461626c657c44726f705461626c654c6973740a70726f70657274797c44726f705461626c657c4c6976655461626c6549647c72657175697265640a72656c6174696f6e736869707c44726f705461626c657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c44726f705461626c65436f6c756d6e7c44726f705461626c65436f6c756d6e4c6973740a70726f70657274797c44726f705461626c65436f6c756d6e7c4c6976655461626c65436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c44726f705461626c65436f6c756d6e7c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c44726f70566965777c44726f70566965774c6973740a70726f70657274797c44726f70566965777c4c6976655669657749647c72657175697265640a72656c6174696f6e736869707c44726f70566965777c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c5265706c616365466f726569676e4b65797c5265706c616365466f726569676e4b65794c6973740a70726f70657274797c5265706c616365466f726569676e4b65797c4c697665466f726569676e4b657949647c72657175697265640a70726f70657274797c5265706c616365466f726569676e4b65797c536f75726365466f726569676e4b657949647c72657175697265640a72656c6174696f6e736869707c5265706c616365466f726569676e4b65797c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c5265706c61636546756e6374696f6e7c5265706c61636546756e6374696f6e4c6973740a70726f70657274797c5265706c61636546756e6374696f6e7c4c69766546756e6374696f6e49647c72657175697265640a70726f70657274797c5265706c61636546756e6374696f6e7c536f7572636546756e6374696f6e49647c72657175697265640a72656c6174696f6e736869707c5265706c61636546756e6374696f6e7c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c5265706c616365496e6465787c5265706c616365496e6465784c6973740a70726f70657274797c5265706c616365496e6465787c4c697665496e64657849647c72657175697265640a70726f70657274797c5265706c616365496e6465787c536f75726365496e64657849647c72657175697265640a72656c6174696f6e736869707c5265706c616365496e6465787c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c5265706c6163655072696d6172794b65797c5265706c6163655072696d6172794b65794c6973740a70726f70657274797c5265706c6163655072696d6172794b65797c4c6976655072696d6172794b657949647c72657175697265640a70726f70657274797c5265706c6163655072696d6172794b65797c536f757263655072696d6172794b657949647c72657175697265640a72656c6174696f6e736869707c5265706c6163655072696d6172794b65797c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c5265706c61636553746f72656450726f6365647572657c5265706c61636553746f72656450726f6365647572654c6973740a70726f70657274797c5265706c61636553746f72656450726f6365647572657c4c69766553746f72656450726f63656475726549647c72657175697265640a70726f70657274797c5265706c61636553746f72656450726f6365647572657c536f7572636553746f72656450726f63656475726549647c72657175697265640a72656c6174696f6e736869707c5265706c61636553746f72656450726f6365647572657c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c5265706c616365566965777c5265706c616365566965774c6973740a70726f70657274797c5265706c616365566965777c4c6976655669657749647c72657175697265640a70726f70657274797c5265706c616365566965777c536f757263655669657749647c72657175697265640a72656c6174696f6e736869707c5265706c616365566965777c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c72657175697265640a656e746974797c5472756e636174655461626c65436f6c756d6e446174617c5472756e636174655461626c65436f6c756d6e446174614c6973740a70726f70657274797c5472756e636174655461626c65436f6c756d6e446174617c4c6976655461626c65436f6c756d6e49647c72657175697265640a70726f70657274797c5472756e636174655461626c65436f6c756d6e446174617c536f757263655461626c65436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c5472756e636174655461626c65436f6c756d6e446174617c4465706c6f794d616e69666573747c4465706c6f794d616e696665737449647c7265717569726564";
    public global::MetaSqlDeployManifest.MetaSqlDeployManifestModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaSqlDeployManifest.MetaSqlDeployManifestModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.AddForeignKey>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddForeignKey"))
        {
            var row = new global::MetaSqlDeployManifest.AddForeignKey
            {
                Id = source.Id
            };
            row.SourceForeignKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceForeignKeyId");
            typed.AddForeignKeyList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.AddFunction>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddFunction"))
        {
            var row = new global::MetaSqlDeployManifest.AddFunction
            {
                Id = source.Id
            };
            row.SourceFunctionId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceFunctionId");
            typed.AddFunctionList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.AddIndex>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddIndex"))
        {
            var row = new global::MetaSqlDeployManifest.AddIndex
            {
                Id = source.Id
            };
            row.SourceIndexId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceIndexId");
            typed.AddIndexList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.AddPrimaryKey>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddPrimaryKey"))
        {
            var row = new global::MetaSqlDeployManifest.AddPrimaryKey
            {
                Id = source.Id
            };
            row.SourcePrimaryKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourcePrimaryKeyId");
            typed.AddPrimaryKeyList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.AddSchema>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddSchema"))
        {
            var row = new global::MetaSqlDeployManifest.AddSchema
            {
                Id = source.Id
            };
            row.SourceSchemaId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceSchemaId");
            typed.AddSchemaList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.AddStoredProcedure>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddStoredProcedure"))
        {
            var row = new global::MetaSqlDeployManifest.AddStoredProcedure
            {
                Id = source.Id
            };
            row.SourceStoredProcedureId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceStoredProcedureId");
            typed.AddStoredProcedureList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.AddTable>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddTable"))
        {
            var row = new global::MetaSqlDeployManifest.AddTable
            {
                Id = source.Id
            };
            row.SourceTableId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceTableId");
            typed.AddTableList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.AddTableColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddTableColumn"))
        {
            var row = new global::MetaSqlDeployManifest.AddTableColumn
            {
                Id = source.Id
            };
            row.SourceTableColumnId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceTableColumnId");
            typed.AddTableColumnList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.AddView>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddView"))
        {
            var row = new global::MetaSqlDeployManifest.AddView
            {
                Id = source.Id
            };
            row.SourceViewId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceViewId");
            typed.AddViewList.Add(row);
            index8.Add(source.Id, row);
        }

        var index9 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.AlterTableColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AlterTableColumn"))
        {
            var row = new global::MetaSqlDeployManifest.AlterTableColumn
            {
                Id = source.Id
            };
            row.LiveTableColumnId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveTableColumnId");
            row.SourceTableColumnId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceTableColumnId");
            typed.AlterTableColumnList.Add(row);
            index9.Add(source.Id, row);
        }

        var index10 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.BlockForeignKeyDifference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockForeignKeyDifference"))
        {
            var row = new global::MetaSqlDeployManifest.BlockForeignKeyDifference
            {
                Id = source.Id
            };
            row.DifferenceSummary = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DifferenceSummary");
            row.LiveForeignKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveForeignKeyId");
            row.SourceForeignKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceForeignKeyId");
            typed.BlockForeignKeyDifferenceList.Add(row);
            index10.Add(source.Id, row);
        }

        var index11 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.BlockFunctionDifference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockFunctionDifference"))
        {
            var row = new global::MetaSqlDeployManifest.BlockFunctionDifference
            {
                Id = source.Id
            };
            row.DifferenceSummary = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DifferenceSummary");
            row.LiveFunctionId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveFunctionId");
            row.SourceFunctionId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceFunctionId");
            typed.BlockFunctionDifferenceList.Add(row);
            index11.Add(source.Id, row);
        }

        var index12 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.BlockIndexDifference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockIndexDifference"))
        {
            var row = new global::MetaSqlDeployManifest.BlockIndexDifference
            {
                Id = source.Id
            };
            row.DifferenceSummary = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DifferenceSummary");
            row.LiveIndexId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveIndexId");
            row.SourceIndexId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceIndexId");
            typed.BlockIndexDifferenceList.Add(row);
            index12.Add(source.Id, row);
        }

        var index13 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.BlockPrimaryKeyDifference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockPrimaryKeyDifference"))
        {
            var row = new global::MetaSqlDeployManifest.BlockPrimaryKeyDifference
            {
                Id = source.Id
            };
            row.DifferenceSummary = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DifferenceSummary");
            row.LivePrimaryKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LivePrimaryKeyId");
            row.SourcePrimaryKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourcePrimaryKeyId");
            typed.BlockPrimaryKeyDifferenceList.Add(row);
            index13.Add(source.Id, row);
        }

        var index14 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.BlockStoredProcedureDifference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockStoredProcedureDifference"))
        {
            var row = new global::MetaSqlDeployManifest.BlockStoredProcedureDifference
            {
                Id = source.Id
            };
            row.DifferenceSummary = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DifferenceSummary");
            row.LiveStoredProcedureId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveStoredProcedureId");
            row.SourceStoredProcedureId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceStoredProcedureId");
            typed.BlockStoredProcedureDifferenceList.Add(row);
            index14.Add(source.Id, row);
        }

        var index15 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.BlockTableColumnDifference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockTableColumnDifference"))
        {
            var row = new global::MetaSqlDeployManifest.BlockTableColumnDifference
            {
                Id = source.Id
            };
            row.DifferenceSummary = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DifferenceSummary");
            row.LiveTableColumnId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveTableColumnId");
            row.SourceTableColumnId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceTableColumnId");
            typed.BlockTableColumnDifferenceList.Add(row);
            index15.Add(source.Id, row);
        }

        var index16 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.BlockTableDifference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockTableDifference"))
        {
            var row = new global::MetaSqlDeployManifest.BlockTableDifference
            {
                Id = source.Id
            };
            row.DifferenceSummary = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DifferenceSummary");
            row.LiveTableId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveTableId");
            row.SourceTableId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceTableId");
            typed.BlockTableDifferenceList.Add(row);
            index16.Add(source.Id, row);
        }

        var index17 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.BlockViewDifference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockViewDifference"))
        {
            var row = new global::MetaSqlDeployManifest.BlockViewDifference
            {
                Id = source.Id
            };
            row.DifferenceSummary = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DifferenceSummary");
            row.LiveViewId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveViewId");
            row.SourceViewId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceViewId");
            typed.BlockViewDifferenceList.Add(row);
            index17.Add(source.Id, row);
        }

        var index18 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.DeployManifest>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DeployManifest"))
        {
            var row = new global::MetaSqlDeployManifest.DeployManifest
            {
                Id = source.Id
            };
            row.CreatedUtc = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "CreatedUtc");
            row.ExpectedLiveDatabasePresence = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ExpectedLiveDatabasePresence");
            row.LiveInstanceFingerprint = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveInstanceFingerprint");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.SourceInstanceFingerprint = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceInstanceFingerprint");
            row.TargetDescription = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TargetDescription");
            typed.DeployManifestList.Add(row);
            index18.Add(source.Id, row);
        }

        var index19 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.DropForeignKey>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropForeignKey"))
        {
            var row = new global::MetaSqlDeployManifest.DropForeignKey
            {
                Id = source.Id
            };
            row.LiveForeignKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveForeignKeyId");
            typed.DropForeignKeyList.Add(row);
            index19.Add(source.Id, row);
        }

        var index20 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.DropFunction>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropFunction"))
        {
            var row = new global::MetaSqlDeployManifest.DropFunction
            {
                Id = source.Id
            };
            row.LiveFunctionId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveFunctionId");
            typed.DropFunctionList.Add(row);
            index20.Add(source.Id, row);
        }

        var index21 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.DropIndex>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropIndex"))
        {
            var row = new global::MetaSqlDeployManifest.DropIndex
            {
                Id = source.Id
            };
            row.LiveIndexId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveIndexId");
            typed.DropIndexList.Add(row);
            index21.Add(source.Id, row);
        }

        var index22 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.DropPrimaryKey>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropPrimaryKey"))
        {
            var row = new global::MetaSqlDeployManifest.DropPrimaryKey
            {
                Id = source.Id
            };
            row.LivePrimaryKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LivePrimaryKeyId");
            typed.DropPrimaryKeyList.Add(row);
            index22.Add(source.Id, row);
        }

        var index23 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.DropStoredProcedure>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropStoredProcedure"))
        {
            var row = new global::MetaSqlDeployManifest.DropStoredProcedure
            {
                Id = source.Id
            };
            row.LiveStoredProcedureId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveStoredProcedureId");
            typed.DropStoredProcedureList.Add(row);
            index23.Add(source.Id, row);
        }

        var index24 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.DropTable>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropTable"))
        {
            var row = new global::MetaSqlDeployManifest.DropTable
            {
                Id = source.Id
            };
            row.LiveTableId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveTableId");
            typed.DropTableList.Add(row);
            index24.Add(source.Id, row);
        }

        var index25 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.DropTableColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropTableColumn"))
        {
            var row = new global::MetaSqlDeployManifest.DropTableColumn
            {
                Id = source.Id
            };
            row.LiveTableColumnId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveTableColumnId");
            typed.DropTableColumnList.Add(row);
            index25.Add(source.Id, row);
        }

        var index26 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.DropView>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropView"))
        {
            var row = new global::MetaSqlDeployManifest.DropView
            {
                Id = source.Id
            };
            row.LiveViewId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveViewId");
            typed.DropViewList.Add(row);
            index26.Add(source.Id, row);
        }

        var index27 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.ReplaceForeignKey>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplaceForeignKey"))
        {
            var row = new global::MetaSqlDeployManifest.ReplaceForeignKey
            {
                Id = source.Id
            };
            row.LiveForeignKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveForeignKeyId");
            row.SourceForeignKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceForeignKeyId");
            typed.ReplaceForeignKeyList.Add(row);
            index27.Add(source.Id, row);
        }

        var index28 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.ReplaceFunction>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplaceFunction"))
        {
            var row = new global::MetaSqlDeployManifest.ReplaceFunction
            {
                Id = source.Id
            };
            row.LiveFunctionId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveFunctionId");
            row.SourceFunctionId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceFunctionId");
            typed.ReplaceFunctionList.Add(row);
            index28.Add(source.Id, row);
        }

        var index29 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.ReplaceIndex>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplaceIndex"))
        {
            var row = new global::MetaSqlDeployManifest.ReplaceIndex
            {
                Id = source.Id
            };
            row.LiveIndexId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveIndexId");
            row.SourceIndexId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceIndexId");
            typed.ReplaceIndexList.Add(row);
            index29.Add(source.Id, row);
        }

        var index30 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.ReplacePrimaryKey>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplacePrimaryKey"))
        {
            var row = new global::MetaSqlDeployManifest.ReplacePrimaryKey
            {
                Id = source.Id
            };
            row.LivePrimaryKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LivePrimaryKeyId");
            row.SourcePrimaryKeyId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourcePrimaryKeyId");
            typed.ReplacePrimaryKeyList.Add(row);
            index30.Add(source.Id, row);
        }

        var index31 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.ReplaceStoredProcedure>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplaceStoredProcedure"))
        {
            var row = new global::MetaSqlDeployManifest.ReplaceStoredProcedure
            {
                Id = source.Id
            };
            row.LiveStoredProcedureId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveStoredProcedureId");
            row.SourceStoredProcedureId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceStoredProcedureId");
            typed.ReplaceStoredProcedureList.Add(row);
            index31.Add(source.Id, row);
        }

        var index32 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.ReplaceView>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplaceView"))
        {
            var row = new global::MetaSqlDeployManifest.ReplaceView
            {
                Id = source.Id
            };
            row.LiveViewId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveViewId");
            row.SourceViewId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceViewId");
            typed.ReplaceViewList.Add(row);
            index32.Add(source.Id, row);
        }

        var index33 = new global::System.Collections.Generic.Dictionary<string, global::MetaSqlDeployManifest.TruncateTableColumnData>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TruncateTableColumnData"))
        {
            var row = new global::MetaSqlDeployManifest.TruncateTableColumnData
            {
                Id = source.Id
            };
            row.LiveTableColumnId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LiveTableColumnId");
            row.SourceTableColumnId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SourceTableColumnId");
            typed.TruncateTableColumnDataList.Add(row);
            index33.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddForeignKey"))
        {
            var row = index0[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddFunction"))
        {
            var row = index1[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddIndex"))
        {
            var row = index2[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddPrimaryKey"))
        {
            var row = index3[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddSchema"))
        {
            var row = index4[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddStoredProcedure"))
        {
            var row = index5[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddTable"))
        {
            var row = index6[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddTableColumn"))
        {
            var row = index7[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AddView"))
        {
            var row = index8[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AlterTableColumn"))
        {
            var row = index9[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockForeignKeyDifference"))
        {
            var row = index10[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockFunctionDifference"))
        {
            var row = index11[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockIndexDifference"))
        {
            var row = index12[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockPrimaryKeyDifference"))
        {
            var row = index13[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockStoredProcedureDifference"))
        {
            var row = index14[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockTableColumnDifference"))
        {
            var row = index15[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockTableDifference"))
        {
            var row = index16[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BlockViewDifference"))
        {
            var row = index17[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropForeignKey"))
        {
            var row = index19[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropFunction"))
        {
            var row = index20[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropIndex"))
        {
            var row = index21[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropPrimaryKey"))
        {
            var row = index22[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropStoredProcedure"))
        {
            var row = index23[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropTable"))
        {
            var row = index24[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropTableColumn"))
        {
            var row = index25[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DropView"))
        {
            var row = index26[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplaceForeignKey"))
        {
            var row = index27[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplaceFunction"))
        {
            var row = index28[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplaceIndex"))
        {
            var row = index29[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplacePrimaryKey"))
        {
            var row = index30[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplaceStoredProcedure"))
        {
            var row = index31[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ReplaceView"))
        {
            var row = index32[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TruncateTableColumnData"))
        {
            var row = index33[source.Id];
            row.DeployManifest = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DeployManifestId", index18);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaSqlDeployManifest.MetaSqlDeployManifestModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AddForeignKeyList, row => row.Id, "AddForeignKey");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AddFunctionList, row => row.Id, "AddFunction");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AddIndexList, row => row.Id, "AddIndex");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AddPrimaryKeyList, row => row.Id, "AddPrimaryKey");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AddSchemaList, row => row.Id, "AddSchema");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AddStoredProcedureList, row => row.Id, "AddStoredProcedure");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AddTableList, row => row.Id, "AddTable");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AddTableColumnList, row => row.Id, "AddTableColumn");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AddViewList, row => row.Id, "AddView");
        var index9 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AlterTableColumnList, row => row.Id, "AlterTableColumn");
        var index10 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BlockForeignKeyDifferenceList, row => row.Id, "BlockForeignKeyDifference");
        var index11 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BlockFunctionDifferenceList, row => row.Id, "BlockFunctionDifference");
        var index12 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BlockIndexDifferenceList, row => row.Id, "BlockIndexDifference");
        var index13 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BlockPrimaryKeyDifferenceList, row => row.Id, "BlockPrimaryKeyDifference");
        var index14 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BlockStoredProcedureDifferenceList, row => row.Id, "BlockStoredProcedureDifference");
        var index15 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BlockTableColumnDifferenceList, row => row.Id, "BlockTableColumnDifference");
        var index16 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BlockTableDifferenceList, row => row.Id, "BlockTableDifference");
        var index17 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BlockViewDifferenceList, row => row.Id, "BlockViewDifference");
        var index18 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DeployManifestList, row => row.Id, "DeployManifest");
        var index19 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DropForeignKeyList, row => row.Id, "DropForeignKey");
        var index20 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DropFunctionList, row => row.Id, "DropFunction");
        var index21 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DropIndexList, row => row.Id, "DropIndex");
        var index22 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DropPrimaryKeyList, row => row.Id, "DropPrimaryKey");
        var index23 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DropStoredProcedureList, row => row.Id, "DropStoredProcedure");
        var index24 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DropTableList, row => row.Id, "DropTable");
        var index25 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DropTableColumnList, row => row.Id, "DropTableColumn");
        var index26 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DropViewList, row => row.Id, "DropView");
        var index27 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ReplaceForeignKeyList, row => row.Id, "ReplaceForeignKey");
        var index28 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ReplaceFunctionList, row => row.Id, "ReplaceFunction");
        var index29 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ReplaceIndexList, row => row.Id, "ReplaceIndex");
        var index30 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ReplacePrimaryKeyList, row => row.Id, "ReplacePrimaryKey");
        var index31 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ReplaceStoredProcedureList, row => row.Id, "ReplaceStoredProcedure");
        var index32 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ReplaceViewList, row => row.Id, "ReplaceView");
        var index33 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TruncateTableColumnDataList, row => row.Id, "TruncateTableColumnData");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SourceForeignKeyId is not null)
                target.Values.Add("SourceForeignKeyId", row.SourceForeignKeyId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "AddForeignKey.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("AddForeignKey").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SourceFunctionId is not null)
                target.Values.Add("SourceFunctionId", row.SourceFunctionId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "AddFunction.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("AddFunction").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SourceIndexId is not null)
                target.Values.Add("SourceIndexId", row.SourceIndexId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "AddIndex.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("AddIndex").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SourcePrimaryKeyId is not null)
                target.Values.Add("SourcePrimaryKeyId", row.SourcePrimaryKeyId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "AddPrimaryKey.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("AddPrimaryKey").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SourceSchemaId is not null)
                target.Values.Add("SourceSchemaId", row.SourceSchemaId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "AddSchema.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("AddSchema").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SourceStoredProcedureId is not null)
                target.Values.Add("SourceStoredProcedureId", row.SourceStoredProcedureId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "AddStoredProcedure.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("AddStoredProcedure").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SourceTableId is not null)
                target.Values.Add("SourceTableId", row.SourceTableId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "AddTable.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("AddTable").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SourceTableColumnId is not null)
                target.Values.Add("SourceTableColumnId", row.SourceTableColumnId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "AddTableColumn.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("AddTableColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index8))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SourceViewId is not null)
                target.Values.Add("SourceViewId", row.SourceViewId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "AddView.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("AddView").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index9))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveTableColumnId is not null)
                target.Values.Add("LiveTableColumnId", row.LiveTableColumnId);
            if (row.SourceTableColumnId is not null)
                target.Values.Add("SourceTableColumnId", row.SourceTableColumnId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "AlterTableColumn.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("AlterTableColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index10))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DifferenceSummary is not null)
                target.Values.Add("DifferenceSummary", row.DifferenceSummary);
            if (row.LiveForeignKeyId is not null)
                target.Values.Add("LiveForeignKeyId", row.LiveForeignKeyId);
            if (row.SourceForeignKeyId is not null)
                target.Values.Add("SourceForeignKeyId", row.SourceForeignKeyId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "BlockForeignKeyDifference.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("BlockForeignKeyDifference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index11))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DifferenceSummary is not null)
                target.Values.Add("DifferenceSummary", row.DifferenceSummary);
            if (row.LiveFunctionId is not null)
                target.Values.Add("LiveFunctionId", row.LiveFunctionId);
            if (row.SourceFunctionId is not null)
                target.Values.Add("SourceFunctionId", row.SourceFunctionId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "BlockFunctionDifference.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("BlockFunctionDifference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index12))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DifferenceSummary is not null)
                target.Values.Add("DifferenceSummary", row.DifferenceSummary);
            if (row.LiveIndexId is not null)
                target.Values.Add("LiveIndexId", row.LiveIndexId);
            if (row.SourceIndexId is not null)
                target.Values.Add("SourceIndexId", row.SourceIndexId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "BlockIndexDifference.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("BlockIndexDifference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index13))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DifferenceSummary is not null)
                target.Values.Add("DifferenceSummary", row.DifferenceSummary);
            if (row.LivePrimaryKeyId is not null)
                target.Values.Add("LivePrimaryKeyId", row.LivePrimaryKeyId);
            if (row.SourcePrimaryKeyId is not null)
                target.Values.Add("SourcePrimaryKeyId", row.SourcePrimaryKeyId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "BlockPrimaryKeyDifference.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("BlockPrimaryKeyDifference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index14))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DifferenceSummary is not null)
                target.Values.Add("DifferenceSummary", row.DifferenceSummary);
            if (row.LiveStoredProcedureId is not null)
                target.Values.Add("LiveStoredProcedureId", row.LiveStoredProcedureId);
            if (row.SourceStoredProcedureId is not null)
                target.Values.Add("SourceStoredProcedureId", row.SourceStoredProcedureId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "BlockStoredProcedureDifference.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("BlockStoredProcedureDifference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index15))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DifferenceSummary is not null)
                target.Values.Add("DifferenceSummary", row.DifferenceSummary);
            if (row.LiveTableColumnId is not null)
                target.Values.Add("LiveTableColumnId", row.LiveTableColumnId);
            if (row.SourceTableColumnId is not null)
                target.Values.Add("SourceTableColumnId", row.SourceTableColumnId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "BlockTableColumnDifference.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("BlockTableColumnDifference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index16))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DifferenceSummary is not null)
                target.Values.Add("DifferenceSummary", row.DifferenceSummary);
            if (row.LiveTableId is not null)
                target.Values.Add("LiveTableId", row.LiveTableId);
            if (row.SourceTableId is not null)
                target.Values.Add("SourceTableId", row.SourceTableId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "BlockTableDifference.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("BlockTableDifference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index17))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DifferenceSummary is not null)
                target.Values.Add("DifferenceSummary", row.DifferenceSummary);
            if (row.LiveViewId is not null)
                target.Values.Add("LiveViewId", row.LiveViewId);
            if (row.SourceViewId is not null)
                target.Values.Add("SourceViewId", row.SourceViewId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "BlockViewDifference.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("BlockViewDifference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index18))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.CreatedUtc is not null)
                target.Values.Add("CreatedUtc", row.CreatedUtc);
            if (row.ExpectedLiveDatabasePresence is not null)
                target.Values.Add("ExpectedLiveDatabasePresence", row.ExpectedLiveDatabasePresence);
            if (row.LiveInstanceFingerprint is not null)
                target.Values.Add("LiveInstanceFingerprint", row.LiveInstanceFingerprint);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.SourceInstanceFingerprint is not null)
                target.Values.Add("SourceInstanceFingerprint", row.SourceInstanceFingerprint);
            if (row.TargetDescription is not null)
                target.Values.Add("TargetDescription", row.TargetDescription);
            instance.GetOrCreateEntityRecords("DeployManifest").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index19))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveForeignKeyId is not null)
                target.Values.Add("LiveForeignKeyId", row.LiveForeignKeyId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "DropForeignKey.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("DropForeignKey").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index20))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveFunctionId is not null)
                target.Values.Add("LiveFunctionId", row.LiveFunctionId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "DropFunction.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("DropFunction").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index21))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveIndexId is not null)
                target.Values.Add("LiveIndexId", row.LiveIndexId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "DropIndex.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("DropIndex").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index22))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LivePrimaryKeyId is not null)
                target.Values.Add("LivePrimaryKeyId", row.LivePrimaryKeyId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "DropPrimaryKey.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("DropPrimaryKey").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index23))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveStoredProcedureId is not null)
                target.Values.Add("LiveStoredProcedureId", row.LiveStoredProcedureId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "DropStoredProcedure.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("DropStoredProcedure").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index24))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveTableId is not null)
                target.Values.Add("LiveTableId", row.LiveTableId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "DropTable.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("DropTable").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index25))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveTableColumnId is not null)
                target.Values.Add("LiveTableColumnId", row.LiveTableColumnId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "DropTableColumn.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("DropTableColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index26))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveViewId is not null)
                target.Values.Add("LiveViewId", row.LiveViewId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "DropView.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("DropView").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index27))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveForeignKeyId is not null)
                target.Values.Add("LiveForeignKeyId", row.LiveForeignKeyId);
            if (row.SourceForeignKeyId is not null)
                target.Values.Add("SourceForeignKeyId", row.SourceForeignKeyId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "ReplaceForeignKey.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("ReplaceForeignKey").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index28))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveFunctionId is not null)
                target.Values.Add("LiveFunctionId", row.LiveFunctionId);
            if (row.SourceFunctionId is not null)
                target.Values.Add("SourceFunctionId", row.SourceFunctionId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "ReplaceFunction.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("ReplaceFunction").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index29))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveIndexId is not null)
                target.Values.Add("LiveIndexId", row.LiveIndexId);
            if (row.SourceIndexId is not null)
                target.Values.Add("SourceIndexId", row.SourceIndexId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "ReplaceIndex.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("ReplaceIndex").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index30))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LivePrimaryKeyId is not null)
                target.Values.Add("LivePrimaryKeyId", row.LivePrimaryKeyId);
            if (row.SourcePrimaryKeyId is not null)
                target.Values.Add("SourcePrimaryKeyId", row.SourcePrimaryKeyId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "ReplacePrimaryKey.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("ReplacePrimaryKey").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index31))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveStoredProcedureId is not null)
                target.Values.Add("LiveStoredProcedureId", row.LiveStoredProcedureId);
            if (row.SourceStoredProcedureId is not null)
                target.Values.Add("SourceStoredProcedureId", row.SourceStoredProcedureId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "ReplaceStoredProcedure.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("ReplaceStoredProcedure").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index32))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveViewId is not null)
                target.Values.Add("LiveViewId", row.LiveViewId);
            if (row.SourceViewId is not null)
                target.Values.Add("SourceViewId", row.SourceViewId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "ReplaceView.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("ReplaceView").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index33))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LiveTableColumnId is not null)
                target.Values.Add("LiveTableColumnId", row.LiveTableColumnId);
            if (row.SourceTableColumnId is not null)
                target.Values.Add("SourceTableColumnId", row.SourceTableColumnId);
            if (row.DeployManifest is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DeployManifest, row.DeployManifest.Id, index18, "TruncateTableColumnData.DeployManifest");
                target.RelationshipIds.Add("DeployManifestId", row.DeployManifest.Id);
            }

            instance.GetOrCreateEntityRecords("TruncateTableColumnData").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}