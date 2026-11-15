// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaSchema;
public sealed class MetaSchemaWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaSchema.MetaSchemaModel>
{
    private const string ContractSignature = "6d6f64656c7c4d657461536368656d610a656e746974797c4669656c647c4669656c644c6973740a70726f70657274797c4669656c647c4964656e74697479496e6372656d656e747c6e756c6c61626c650a70726f70657274797c4669656c647c4964656e74697479536565647c6e756c6c61626c650a70726f70657274797c4669656c647c49734964656e746974797c6e756c6c61626c650a70726f70657274797c4669656c647c49734e756c6c61626c657c6e756c6c61626c650a70726f70657274797c4669656c647c4d657461446174615479706549647c72657175697265640a70726f70657274797c4669656c647c4e616d657c72657175697265640a70726f70657274797c4669656c647c4f7264696e616c7c6e756c6c61626c650a72656c6174696f6e736869707c4669656c647c536368656d614f626a6563747c536368656d614f626a65637449647c72657175697265640a656e746974797c4669656c64446174615479706544657461696c7c4669656c64446174615479706544657461696c4c6973740a70726f70657274797c4669656c64446174615479706544657461696c7c4e616d657c72657175697265640a70726f70657274797c4669656c64446174615479706544657461696c7c56616c75657c72657175697265640a72656c6174696f6e736869707c4669656c64446174615479706544657461696c7c4669656c647c4669656c6449647c72657175697265640a656e746974797c4b65797c4b65794c6973740a70726f70657274797c4b65797c4e616d657c72657175697265640a72656c6174696f6e736869707c4b65797c5461626c657c5461626c6549647c72657175697265640a656e746974797c4b65794669656c647c4b65794669656c644c6973740a70726f70657274797c4b65794669656c647c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c4b65794669656c647c4669656c647c4669656c6449647c72657175697265640a72656c6174696f6e736869707c4b65794669656c647c4b65797c4b657949647c72657175697265640a656e746974797c5072696d6172794b65797c5072696d6172794b65794c6973740a72656c6174696f6e736869707c5072696d6172794b65797c4b65797c4b657949647c72657175697265640a656e746974797c536368656d617c536368656d614c6973740a70726f70657274797c536368656d617c4e616d657c72657175697265640a72656c6174696f6e736869707c536368656d617c53797374656d7c53797374656d49647c72657175697265640a656e746974797c536368656d614f626a6563747c536368656d614f626a6563744c6973740a70726f70657274797c536368656d614f626a6563747c4e616d657c72657175697265640a72656c6174696f6e736869707c536368656d614f626a6563747c536368656d617c536368656d6149647c72657175697265640a656e746974797c53797374656d7c53797374656d4c6973740a70726f70657274797c53797374656d7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c53797374656d7c4e616d657c72657175697265640a656e746974797c5461626c657c5461626c654c6973740a72656c6174696f6e736869707c5461626c657c536368656d614f626a6563747c536368656d614f626a65637449647c72657175697265640a656e746974797c5461626c6552656c6174696f6e736869707c5461626c6552656c6174696f6e736869704c6973740a70726f70657274797c5461626c6552656c6174696f6e736869707c4e616d657c72657175697265640a72656c6174696f6e736869707c5461626c6552656c6174696f6e736869707c5461626c657c536f757263655461626c6549647c72657175697265640a72656c6174696f6e736869707c5461626c6552656c6174696f6e736869707c5461626c657c5461726765745461626c6549647c72657175697265640a656e746974797c5461626c6552656c6174696f6e736869704669656c647c5461626c6552656c6174696f6e736869704669656c644c6973740a70726f70657274797c5461626c6552656c6174696f6e736869704669656c647c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c5461626c6552656c6174696f6e736869704669656c647c4669656c647c536f757263654669656c6449647c72657175697265640a72656c6174696f6e736869707c5461626c6552656c6174696f6e736869704669656c647c5461626c6552656c6174696f6e736869707c5461626c6552656c6174696f6e7368697049647c72657175697265640a72656c6174696f6e736869707c5461626c6552656c6174696f6e736869704669656c647c4669656c647c5461726765744669656c6449647c72657175697265640a656e746974797c556e697175654b65797c556e697175654b65794c6973740a72656c6174696f6e736869707c556e697175654b65797c4b65797c4b657949647c72657175697265640a656e746974797c566965777c566965774c6973740a72656c6174696f6e736869707c566965777c536368656d614f626a6563747c536368656d614f626a65637449647c7265717569726564";
    public global::MetaSchema.MetaSchemaModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaSchema.MetaSchemaModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.Field>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Field"))
        {
            var row = new global::MetaSchema.Field
            {
                Id = source.Id
            };
            row.IdentityIncrement = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IdentityIncrement");
            row.IdentitySeed = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IdentitySeed");
            row.IsIdentity = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsIdentity");
            row.IsNullable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsNullable");
            row.MetaDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaDataTypeId");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Ordinal");
            typed.FieldList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.FieldDataTypeDetail>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FieldDataTypeDetail"))
        {
            var row = new global::MetaSchema.FieldDataTypeDetail
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Value = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Value");
            typed.FieldDataTypeDetailList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.Key>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Key"))
        {
            var row = new global::MetaSchema.Key
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.KeyList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.KeyField>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "KeyField"))
        {
            var row = new global::MetaSchema.KeyField
            {
                Id = source.Id
            };
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.KeyFieldList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.PrimaryKey>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PrimaryKey"))
        {
            var row = new global::MetaSchema.PrimaryKey
            {
                Id = source.Id
            };
            typed.PrimaryKeyList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.Schema>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Schema"))
        {
            var row = new global::MetaSchema.Schema
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.SchemaList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.SchemaObject>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SchemaObject"))
        {
            var row = new global::MetaSchema.SchemaObject
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.SchemaObjectList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.System>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "System"))
        {
            var row = new global::MetaSchema.System
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.SystemList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.Table>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Table"))
        {
            var row = new global::MetaSchema.Table
            {
                Id = source.Id
            };
            typed.TableList.Add(row);
            index8.Add(source.Id, row);
        }

        var index9 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.TableRelationship>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableRelationship"))
        {
            var row = new global::MetaSchema.TableRelationship
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TableRelationshipList.Add(row);
            index9.Add(source.Id, row);
        }

        var index10 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.TableRelationshipField>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableRelationshipField"))
        {
            var row = new global::MetaSchema.TableRelationshipField
            {
                Id = source.Id
            };
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.TableRelationshipFieldList.Add(row);
            index10.Add(source.Id, row);
        }

        var index11 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.UniqueKey>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "UniqueKey"))
        {
            var row = new global::MetaSchema.UniqueKey
            {
                Id = source.Id
            };
            typed.UniqueKeyList.Add(row);
            index11.Add(source.Id, row);
        }

        var index12 = new global::System.Collections.Generic.Dictionary<string, global::MetaSchema.View>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "View"))
        {
            var row = new global::MetaSchema.View
            {
                Id = source.Id
            };
            typed.ViewList.Add(row);
            index12.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Field"))
        {
            var row = index0[source.Id];
            row.SchemaObject = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SchemaObjectId", index6);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FieldDataTypeDetail"))
        {
            var row = index1[source.Id];
            row.Field = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FieldId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Key"))
        {
            var row = index2[source.Id];
            row.Table = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "KeyField"))
        {
            var row = index3[source.Id];
            row.Field = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FieldId", index0);
            row.Key = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "KeyId", index2);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PrimaryKey"))
        {
            var row = index4[source.Id];
            row.Key = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "KeyId", index2);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Schema"))
        {
            var row = index5[source.Id];
            row.System = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SystemId", index7);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SchemaObject"))
        {
            var row = index6[source.Id];
            row.Schema = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SchemaId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Table"))
        {
            var row = index8[source.Id];
            row.SchemaObject = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SchemaObjectId", index6);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableRelationship"))
        {
            var row = index9[source.Id];
            row.SourceTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SourceTableId", index8);
            row.TargetTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TargetTableId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableRelationshipField"))
        {
            var row = index10[source.Id];
            row.SourceField = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SourceFieldId", index0);
            row.TableRelationship = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableRelationshipId", index9);
            row.TargetField = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TargetFieldId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "UniqueKey"))
        {
            var row = index11[source.Id];
            row.Key = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "KeyId", index2);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "View"))
        {
            var row = index12[source.Id];
            row.SchemaObject = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SchemaObjectId", index6);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaSchema.MetaSchemaModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FieldList, row => row.Id, "Field");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FieldDataTypeDetailList, row => row.Id, "FieldDataTypeDetail");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.KeyList, row => row.Id, "Key");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.KeyFieldList, row => row.Id, "KeyField");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PrimaryKeyList, row => row.Id, "PrimaryKey");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SchemaList, row => row.Id, "Schema");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SchemaObjectList, row => row.Id, "SchemaObject");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SystemList, row => row.Id, "System");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TableList, row => row.Id, "Table");
        var index9 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TableRelationshipList, row => row.Id, "TableRelationship");
        var index10 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TableRelationshipFieldList, row => row.Id, "TableRelationshipField");
        var index11 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.UniqueKeyList, row => row.Id, "UniqueKey");
        var index12 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ViewList, row => row.Id, "View");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
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
            if (row.SchemaObject is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SchemaObject, row.SchemaObject.Id, index6, "Field.SchemaObject");
                target.RelationshipIds.Add("SchemaObjectId", row.SchemaObject.Id);
            }

            instance.GetOrCreateEntityRecords("Field").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Value is not null)
                target.Values.Add("Value", row.Value);
            if (row.Field is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Field, row.Field.Id, index0, "FieldDataTypeDetail.Field");
                target.RelationshipIds.Add("FieldId", row.Field.Id);
            }

            instance.GetOrCreateEntityRecords("FieldDataTypeDetail").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Table is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Table, row.Table.Id, index8, "Key.Table");
                target.RelationshipIds.Add("TableId", row.Table.Id);
            }

            instance.GetOrCreateEntityRecords("Key").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Field is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Field, row.Field.Id, index0, "KeyField.Field");
                target.RelationshipIds.Add("FieldId", row.Field.Id);
            }

            if (row.Key is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Key, row.Key.Id, index2, "KeyField.Key");
                target.RelationshipIds.Add("KeyId", row.Key.Id);
            }

            instance.GetOrCreateEntityRecords("KeyField").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Key is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Key, row.Key.Id, index2, "PrimaryKey.Key");
                target.RelationshipIds.Add("KeyId", row.Key.Id);
            }

            instance.GetOrCreateEntityRecords("PrimaryKey").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.System is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.System, row.System.Id, index7, "Schema.System");
                target.RelationshipIds.Add("SystemId", row.System.Id);
            }

            instance.GetOrCreateEntityRecords("Schema").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Schema is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Schema, row.Schema.Id, index5, "SchemaObject.Schema");
                target.RelationshipIds.Add("SchemaId", row.Schema.Id);
            }

            instance.GetOrCreateEntityRecords("SchemaObject").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("System").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index8))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SchemaObject is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SchemaObject, row.SchemaObject.Id, index6, "Table.SchemaObject");
                target.RelationshipIds.Add("SchemaObjectId", row.SchemaObject.Id);
            }

            instance.GetOrCreateEntityRecords("Table").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index9))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.SourceTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SourceTable, row.SourceTable.Id, index8, "TableRelationship.SourceTable");
                target.RelationshipIds.Add("SourceTableId", row.SourceTable.Id);
            }

            if (row.TargetTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TargetTable, row.TargetTable.Id, index8, "TableRelationship.TargetTable");
                target.RelationshipIds.Add("TargetTableId", row.TargetTable.Id);
            }

            instance.GetOrCreateEntityRecords("TableRelationship").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index10))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.SourceField is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SourceField, row.SourceField.Id, index0, "TableRelationshipField.SourceField");
                target.RelationshipIds.Add("SourceFieldId", row.SourceField.Id);
            }

            if (row.TableRelationship is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TableRelationship, row.TableRelationship.Id, index9, "TableRelationshipField.TableRelationship");
                target.RelationshipIds.Add("TableRelationshipId", row.TableRelationship.Id);
            }

            if (row.TargetField is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TargetField, row.TargetField.Id, index0, "TableRelationshipField.TargetField");
                target.RelationshipIds.Add("TargetFieldId", row.TargetField.Id);
            }

            instance.GetOrCreateEntityRecords("TableRelationshipField").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index11))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Key is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Key, row.Key.Id, index2, "UniqueKey.Key");
                target.RelationshipIds.Add("KeyId", row.Key.Id);
            }

            instance.GetOrCreateEntityRecords("UniqueKey").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index12))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SchemaObject is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SchemaObject, row.SchemaObject.Id, index6, "View.SchemaObject");
                target.RelationshipIds.Add("SchemaObjectId", row.SchemaObject.Id);
            }

            instance.GetOrCreateEntityRecords("View").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}