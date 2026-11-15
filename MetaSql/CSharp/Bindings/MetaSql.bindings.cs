// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaSql;
public sealed class MetaSqlWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaSql.MetaSqlModel>
{
    private const string ContractSignature = "6d6f64656c7c4d65746153716c0a656e746974797c44617461626173657c44617461626173654c6973740a70726f70657274797c44617461626173657c436f6c6c6174696f6e7c6e756c6c61626c650a70726f70657274797c44617461626173657c4e616d657c72657175697265640a656e746974797c466f726569676e4b65797c466f726569676e4b65794c6973740a70726f70657274797c466f726569676e4b65797c4e616d657c72657175697265640a72656c6174696f6e736869707c466f726569676e4b65797c5461626c657c536f757263655461626c6549647c72657175697265640a72656c6174696f6e736869707c466f726569676e4b65797c5461626c657c5461726765745461626c6549647c72657175697265640a656e746974797c466f726569676e4b6579436f6c756d6e7c466f726569676e4b6579436f6c756d6e4c6973740a70726f70657274797c466f726569676e4b6579436f6c756d6e7c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c466f726569676e4b6579436f6c756d6e7c466f726569676e4b65797c466f726569676e4b657949647c72657175697265640a72656c6174696f6e736869707c466f726569676e4b6579436f6c756d6e7c5461626c65436f6c756d6e7c536f75726365436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c466f726569676e4b6579436f6c756d6e7c5461626c65436f6c756d6e7c546172676574436f6c756d6e49647c72657175697265640a656e746974797c46756e6374696f6e7c46756e6374696f6e4c6973740a70726f70657274797c46756e6374696f6e7c446566696e6974696f6e53716c7c72657175697265640a70726f70657274797c46756e6374696f6e7c4465706c6f794f7264696e616c7c6e756c6c61626c650a70726f70657274797c46756e6374696f6e7c46756e6374696f6e4b696e647c72657175697265640a70726f70657274797c46756e6374696f6e7c4e616d657c72657175697265640a72656c6174696f6e736869707c46756e6374696f6e7c536368656d617c536368656d6149647c72657175697265640a656e746974797c496e6465787c496e6465784c6973740a70726f70657274797c496e6465787c46696c74657253716c7c6e756c6c61626c650a70726f70657274797c496e6465787c4973436c757374657265647c6e756c6c61626c650a70726f70657274797c496e6465787c4973556e697175657c6e756c6c61626c650a70726f70657274797c496e6465787c4e616d657c72657175697265640a72656c6174696f6e736869707c496e6465787c5461626c657c5461626c6549647c72657175697265640a656e746974797c496e646578436f6c756d6e7c496e646578436f6c756d6e4c6973740a70726f70657274797c496e646578436f6c756d6e7c497344657363656e64696e677c6e756c6c61626c650a70726f70657274797c496e646578436f6c756d6e7c4973496e636c756465647c6e756c6c61626c650a70726f70657274797c496e646578436f6c756d6e7c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c496e646578436f6c756d6e7c496e6465787c496e64657849647c72657175697265640a72656c6174696f6e736869707c496e646578436f6c756d6e7c5461626c65436f6c756d6e7c5461626c65436f6c756d6e49647c72657175697265640a656e746974797c5072696d6172794b65797c5072696d6172794b65794c6973740a70726f70657274797c5072696d6172794b65797c4973436c757374657265647c6e756c6c61626c650a70726f70657274797c5072696d6172794b65797c4e616d657c72657175697265640a72656c6174696f6e736869707c5072696d6172794b65797c5461626c657c5461626c6549647c72657175697265640a656e746974797c5072696d6172794b6579436f6c756d6e7c5072696d6172794b6579436f6c756d6e4c6973740a70726f70657274797c5072696d6172794b6579436f6c756d6e7c497344657363656e64696e677c6e756c6c61626c650a70726f70657274797c5072696d6172794b6579436f6c756d6e7c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c5072696d6172794b6579436f6c756d6e7c5072696d6172794b65797c5072696d6172794b657949647c72657175697265640a72656c6174696f6e736869707c5072696d6172794b6579436f6c756d6e7c5461626c65436f6c756d6e7c5461626c65436f6c756d6e49647c72657175697265640a656e746974797c536368656d617c536368656d614c6973740a70726f70657274797c536368656d617c4e616d657c72657175697265640a72656c6174696f6e736869707c536368656d617c44617461626173657c446174616261736549647c72657175697265640a656e746974797c53746f72656450726f6365647572657c53746f72656450726f6365647572654c6973740a70726f70657274797c53746f72656450726f6365647572657c446566696e6974696f6e53716c7c72657175697265640a70726f70657274797c53746f72656450726f6365647572657c4465706c6f794f7264696e616c7c6e756c6c61626c650a70726f70657274797c53746f72656450726f6365647572657c4e616d657c72657175697265640a72656c6174696f6e736869707c53746f72656450726f6365647572657c536368656d617c536368656d6149647c72657175697265640a656e746974797c5461626c657c5461626c654c6973740a70726f70657274797c5461626c657c4e616d657c72657175697265640a72656c6174696f6e736869707c5461626c657c536368656d617c536368656d6149647c72657175697265640a656e746974797c5461626c65436f6c756d6e7c5461626c65436f6c756d6e4c6973740a70726f70657274797c5461626c65436f6c756d6e7c44656661756c7445787072657373696f6e53716c7c6e756c6c61626c650a70726f70657274797c5461626c65436f6c756d6e7c45787072657373696f6e53716c7c6e756c6c61626c650a70726f70657274797c5461626c65436f6c756d6e7c4964656e74697479496e6372656d656e747c6e756c6c61626c650a70726f70657274797c5461626c65436f6c756d6e7c4964656e74697479536565647c6e756c6c61626c650a70726f70657274797c5461626c65436f6c756d6e7c49734964656e746974797c6e756c6c61626c650a70726f70657274797c5461626c65436f6c756d6e7c49734e756c6c61626c657c6e756c6c61626c650a70726f70657274797c5461626c65436f6c756d6e7c4d657461446174615479706549647c72657175697265640a70726f70657274797c5461626c65436f6c756d6e7c4e616d657c72657175697265640a70726f70657274797c5461626c65436f6c756d6e7c4f7264696e616c7c6e756c6c61626c650a72656c6174696f6e736869707c5461626c65436f6c756d6e7c5461626c657c5461626c6549647c72657175697265640a656e746974797c5461626c65436f6c756d6e446174615479706544657461696c7c5461626c65436f6c756d6e446174615479706544657461696c4c6973740a70726f70657274797c5461626c65436f6c756d6e446174615479706544657461696c7c4e616d657c72657175697265640a70726f70657274797c5461626c65436f6c756d6e446174615479706544657461696c7c56616c75657c72657175697265640a72656c6174696f6e736869707c5461626c65436f6c756d6e446174615479706544657461696c7c5461626c65436f6c756d6e7c5461626c65436f6c756d6e49647c72657175697265640a656e746974797c566965777c566965774c6973740a70726f70657274797c566965777c446566696e6974696f6e53716c7c72657175697265640a70726f70657274797c566965777c4465706c6f794f7264696e616c7c6e756c6c61626c650a70726f70657274797c566965777c4e616d657c72657175697265640a72656c6174696f6e736869707c566965777c536368656d617c536368656d6149647c7265717569726564";
    public global::MetaSql.MetaSqlModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaSql.MetaSqlModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.Database>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Database"))
        {
            var row = new global::MetaSql.Database
            {
                Id = source.Id
            };
            row.Collation = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Collation");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.DatabaseList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.ForeignKey>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ForeignKey"))
        {
            var row = new global::MetaSql.ForeignKey
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.ForeignKeyList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.ForeignKeyColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ForeignKeyColumn"))
        {
            var row = new global::MetaSql.ForeignKeyColumn
            {
                Id = source.Id
            };
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.ForeignKeyColumnList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.Function>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Function"))
        {
            var row = new global::MetaSql.Function
            {
                Id = source.Id
            };
            row.DefinitionSql = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DefinitionSql");
            row.DeployOrdinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DeployOrdinal");
            row.FunctionKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "FunctionKind");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.FunctionList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.Index>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Index"))
        {
            var row = new global::MetaSql.Index
            {
                Id = source.Id
            };
            row.FilterSql = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "FilterSql");
            row.IsClustered = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsClustered");
            row.IsUnique = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsUnique");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.IndexList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.IndexColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "IndexColumn"))
        {
            var row = new global::MetaSql.IndexColumn
            {
                Id = source.Id
            };
            row.IsDescending = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsDescending");
            row.IsIncluded = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsIncluded");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.IndexColumnList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.PrimaryKey>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PrimaryKey"))
        {
            var row = new global::MetaSql.PrimaryKey
            {
                Id = source.Id
            };
            row.IsClustered = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsClustered");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.PrimaryKeyList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.PrimaryKeyColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PrimaryKeyColumn"))
        {
            var row = new global::MetaSql.PrimaryKeyColumn
            {
                Id = source.Id
            };
            row.IsDescending = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsDescending");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.PrimaryKeyColumnList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.Schema>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Schema"))
        {
            var row = new global::MetaSql.Schema
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.SchemaList.Add(row);
            index8.Add(source.Id, row);
        }

        var index9 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.StoredProcedure>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "StoredProcedure"))
        {
            var row = new global::MetaSql.StoredProcedure
            {
                Id = source.Id
            };
            row.DefinitionSql = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DefinitionSql");
            row.DeployOrdinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DeployOrdinal");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.StoredProcedureList.Add(row);
            index9.Add(source.Id, row);
        }

        var index10 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.Table>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Table"))
        {
            var row = new global::MetaSql.Table
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TableList.Add(row);
            index10.Add(source.Id, row);
        }

        var index11 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.TableColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableColumn"))
        {
            var row = new global::MetaSql.TableColumn
            {
                Id = source.Id
            };
            row.DefaultExpressionSql = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DefaultExpressionSql");
            row.ExpressionSql = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "ExpressionSql");
            row.IdentityIncrement = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IdentityIncrement");
            row.IdentitySeed = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IdentitySeed");
            row.IsIdentity = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsIdentity");
            row.IsNullable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsNullable");
            row.MetaDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaDataTypeId");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Ordinal");
            typed.TableColumnList.Add(row);
            index11.Add(source.Id, row);
        }

        var index12 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.TableColumnDataTypeDetail>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableColumnDataTypeDetail"))
        {
            var row = new global::MetaSql.TableColumnDataTypeDetail
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Value = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Value");
            typed.TableColumnDataTypeDetailList.Add(row);
            index12.Add(source.Id, row);
        }

        var index13 = new global::System.Collections.Generic.Dictionary<string, global::MetaSql.View>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "View"))
        {
            var row = new global::MetaSql.View
            {
                Id = source.Id
            };
            row.DefinitionSql = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DefinitionSql");
            row.DeployOrdinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DeployOrdinal");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.ViewList.Add(row);
            index13.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ForeignKey"))
        {
            var row = index1[source.Id];
            row.SourceTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SourceTableId", index10);
            row.TargetTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TargetTableId", index10);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ForeignKeyColumn"))
        {
            var row = index2[source.Id];
            row.ForeignKey = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ForeignKeyId", index1);
            row.SourceColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SourceColumnId", index11);
            row.TargetColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TargetColumnId", index11);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Function"))
        {
            var row = index3[source.Id];
            row.Schema = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SchemaId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Index"))
        {
            var row = index4[source.Id];
            row.Table = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableId", index10);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "IndexColumn"))
        {
            var row = index5[source.Id];
            row.Index = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "IndexId", index4);
            row.TableColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableColumnId", index11);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PrimaryKey"))
        {
            var row = index6[source.Id];
            row.Table = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableId", index10);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PrimaryKeyColumn"))
        {
            var row = index7[source.Id];
            row.PrimaryKey = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PrimaryKeyId", index6);
            row.TableColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableColumnId", index11);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Schema"))
        {
            var row = index8[source.Id];
            row.Database = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DatabaseId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "StoredProcedure"))
        {
            var row = index9[source.Id];
            row.Schema = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SchemaId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Table"))
        {
            var row = index10[source.Id];
            row.Schema = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SchemaId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableColumn"))
        {
            var row = index11[source.Id];
            row.Table = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableId", index10);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableColumnDataTypeDetail"))
        {
            var row = index12[source.Id];
            row.TableColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableColumnId", index11);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "View"))
        {
            var row = index13[source.Id];
            row.Schema = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SchemaId", index8);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaSql.MetaSqlModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DatabaseList, row => row.Id, "Database");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ForeignKeyList, row => row.Id, "ForeignKey");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ForeignKeyColumnList, row => row.Id, "ForeignKeyColumn");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FunctionList, row => row.Id, "Function");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.IndexList, row => row.Id, "Index");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.IndexColumnList, row => row.Id, "IndexColumn");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PrimaryKeyList, row => row.Id, "PrimaryKey");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PrimaryKeyColumnList, row => row.Id, "PrimaryKeyColumn");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SchemaList, row => row.Id, "Schema");
        var index9 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.StoredProcedureList, row => row.Id, "StoredProcedure");
        var index10 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TableList, row => row.Id, "Table");
        var index11 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TableColumnList, row => row.Id, "TableColumn");
        var index12 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TableColumnDataTypeDetailList, row => row.Id, "TableColumnDataTypeDetail");
        var index13 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ViewList, row => row.Id, "View");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Collation is not null)
                target.Values.Add("Collation", row.Collation);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("Database").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.SourceTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SourceTable, row.SourceTable.Id, index10, "ForeignKey.SourceTable");
                target.RelationshipIds.Add("SourceTableId", row.SourceTable.Id);
            }

            if (row.TargetTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TargetTable, row.TargetTable.Id, index10, "ForeignKey.TargetTable");
                target.RelationshipIds.Add("TargetTableId", row.TargetTable.Id);
            }

            instance.GetOrCreateEntityRecords("ForeignKey").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.ForeignKey is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ForeignKey, row.ForeignKey.Id, index1, "ForeignKeyColumn.ForeignKey");
                target.RelationshipIds.Add("ForeignKeyId", row.ForeignKey.Id);
            }

            if (row.SourceColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SourceColumn, row.SourceColumn.Id, index11, "ForeignKeyColumn.SourceColumn");
                target.RelationshipIds.Add("SourceColumnId", row.SourceColumn.Id);
            }

            if (row.TargetColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TargetColumn, row.TargetColumn.Id, index11, "ForeignKeyColumn.TargetColumn");
                target.RelationshipIds.Add("TargetColumnId", row.TargetColumn.Id);
            }

            instance.GetOrCreateEntityRecords("ForeignKeyColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DefinitionSql is not null)
                target.Values.Add("DefinitionSql", row.DefinitionSql);
            if (row.DeployOrdinal is not null)
                target.Values.Add("DeployOrdinal", row.DeployOrdinal);
            if (row.FunctionKind is not null)
                target.Values.Add("FunctionKind", row.FunctionKind);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Schema is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Schema, row.Schema.Id, index8, "Function.Schema");
                target.RelationshipIds.Add("SchemaId", row.Schema.Id);
            }

            instance.GetOrCreateEntityRecords("Function").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.FilterSql is not null)
                target.Values.Add("FilterSql", row.FilterSql);
            if (row.IsClustered is not null)
                target.Values.Add("IsClustered", row.IsClustered);
            if (row.IsUnique is not null)
                target.Values.Add("IsUnique", row.IsUnique);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Table is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Table, row.Table.Id, index10, "Index.Table");
                target.RelationshipIds.Add("TableId", row.Table.Id);
            }

            instance.GetOrCreateEntityRecords("Index").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.IsDescending is not null)
                target.Values.Add("IsDescending", row.IsDescending);
            if (row.IsIncluded is not null)
                target.Values.Add("IsIncluded", row.IsIncluded);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Index is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Index, row.Index.Id, index4, "IndexColumn.Index");
                target.RelationshipIds.Add("IndexId", row.Index.Id);
            }

            if (row.TableColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TableColumn, row.TableColumn.Id, index11, "IndexColumn.TableColumn");
                target.RelationshipIds.Add("TableColumnId", row.TableColumn.Id);
            }

            instance.GetOrCreateEntityRecords("IndexColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.IsClustered is not null)
                target.Values.Add("IsClustered", row.IsClustered);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Table is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Table, row.Table.Id, index10, "PrimaryKey.Table");
                target.RelationshipIds.Add("TableId", row.Table.Id);
            }

            instance.GetOrCreateEntityRecords("PrimaryKey").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.IsDescending is not null)
                target.Values.Add("IsDescending", row.IsDescending);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.PrimaryKey is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PrimaryKey, row.PrimaryKey.Id, index6, "PrimaryKeyColumn.PrimaryKey");
                target.RelationshipIds.Add("PrimaryKeyId", row.PrimaryKey.Id);
            }

            if (row.TableColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TableColumn, row.TableColumn.Id, index11, "PrimaryKeyColumn.TableColumn");
                target.RelationshipIds.Add("TableColumnId", row.TableColumn.Id);
            }

            instance.GetOrCreateEntityRecords("PrimaryKeyColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index8))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Database is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Database, row.Database.Id, index0, "Schema.Database");
                target.RelationshipIds.Add("DatabaseId", row.Database.Id);
            }

            instance.GetOrCreateEntityRecords("Schema").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index9))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DefinitionSql is not null)
                target.Values.Add("DefinitionSql", row.DefinitionSql);
            if (row.DeployOrdinal is not null)
                target.Values.Add("DeployOrdinal", row.DeployOrdinal);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Schema is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Schema, row.Schema.Id, index8, "StoredProcedure.Schema");
                target.RelationshipIds.Add("SchemaId", row.Schema.Id);
            }

            instance.GetOrCreateEntityRecords("StoredProcedure").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index10))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Schema is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Schema, row.Schema.Id, index8, "Table.Schema");
                target.RelationshipIds.Add("SchemaId", row.Schema.Id);
            }

            instance.GetOrCreateEntityRecords("Table").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index11))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DefaultExpressionSql is not null)
                target.Values.Add("DefaultExpressionSql", row.DefaultExpressionSql);
            if (row.ExpressionSql is not null)
                target.Values.Add("ExpressionSql", row.ExpressionSql);
            if (row.IdentityIncrement is not null)
                target.Values.Add("IdentityIncrement", row.IdentityIncrement);
            if (row.IdentitySeed is not null)
                target.Values.Add("IdentitySeed", row.IdentitySeed);
            if (row.IsIdentity is not null)
                target.Values.Add("IsIdentity", row.IsIdentity);
            if (row.IsNullable is not null)
                target.Values.Add("IsNullable", row.IsNullable);
            if (row.MetaDataTypeId is not null)
                target.Values.Add("MetaDataTypeId", row.MetaDataTypeId);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Table is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Table, row.Table.Id, index10, "TableColumn.Table");
                target.RelationshipIds.Add("TableId", row.Table.Id);
            }

            instance.GetOrCreateEntityRecords("TableColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index12))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Value is not null)
                target.Values.Add("Value", row.Value);
            if (row.TableColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TableColumn, row.TableColumn.Id, index11, "TableColumnDataTypeDetail.TableColumn");
                target.RelationshipIds.Add("TableColumnId", row.TableColumn.Id);
            }

            instance.GetOrCreateEntityRecords("TableColumnDataTypeDetail").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index13))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DefinitionSql is not null)
                target.Values.Add("DefinitionSql", row.DefinitionSql);
            if (row.DeployOrdinal is not null)
                target.Values.Add("DeployOrdinal", row.DeployOrdinal);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Schema is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Schema, row.Schema.Id, index8, "View.Schema");
                target.RelationshipIds.Add("SchemaId", row.Schema.Id);
            }

            instance.GetOrCreateEntityRecords("View").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}