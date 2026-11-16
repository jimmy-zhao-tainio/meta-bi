// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaRawDataVault;
public sealed class MetaRawDataVaultWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaRawDataVault.MetaRawDataVaultModel>
{
    private const string ContractSignature = "6d6f64656c7c4d657461526177446174615661756c740a656e746974797c4669656c647c4669656c644c6973740a70726f70657274797c4669656c647c446174615479706549647c72657175697265640a70726f70657274797c4669656c647c4e616d657c72657175697265640a656e746974797c4669656c64446174615479706544657461696c7c4669656c64446174615479706544657461696c4c6973740a70726f70657274797c4669656c64446174615479706544657461696c7c4e616d657c72657175697265640a70726f70657274797c4669656c64446174615479706544657461696c7c56616c75657c72657175697265640a72656c6174696f6e736869707c4669656c64446174615479706544657461696c7c4669656c647c4669656c6449647c72657175697265640a656e746974797c5261774875627c5261774875624c6973740a70726f70657274797c5261774875627c4e616d657c72657175697265640a656e746974797c5261774875624b6579506172747c5261774875624b6579506172744c6973740a70726f70657274797c5261774875624b6579506172747c4e616d657c72657175697265640a72656c6174696f6e736869707c5261774875624b6579506172747c4669656c647c4669656c6449647c72657175697265640a72656c6174696f6e736869707c5261774875624b6579506172747c5261774875627c52617748756249647c72657175697265640a656e746974797c526177487562536174656c6c6974657c526177487562536174656c6c6974654c6973740a70726f70657274797c526177487562536174656c6c6974657c4e616d657c72657175697265640a70726f70657274797c526177487562536174656c6c6974657c536174656c6c6974654b696e647c72657175697265640a72656c6174696f6e736869707c526177487562536174656c6c6974657c5261774875627c52617748756249647c72657175697265640a656e746974797c526177487562536174656c6c6974654174747269627574657c526177487562536174656c6c6974654174747269627574654c6973740a70726f70657274797c526177487562536174656c6c6974654174747269627574657c4e616d657c72657175697265640a72656c6174696f6e736869707c526177487562536174656c6c6974654174747269627574657c4669656c647c4669656c6449647c72657175697265640a72656c6174696f6e736869707c526177487562536174656c6c6974654174747269627574657c526177487562536174656c6c6974657c526177487562536174656c6c69746549647c72657175697265640a656e746974797c5261774c696e6b7c5261774c696e6b4c6973740a70726f70657274797c5261774c696e6b7c4c696e6b4b696e647c72657175697265640a70726f70657274797c5261774c696e6b7c4e616d657c72657175697265640a656e746974797c5261774c696e6b526f6c657c5261774c696e6b526f6c654c6973740a70726f70657274797c5261774c696e6b526f6c657c4e616d657c72657175697265640a72656c6174696f6e736869707c5261774c696e6b526f6c657c5261774875627c52617748756249647c72657175697265640a72656c6174696f6e736869707c5261774c696e6b526f6c657c5261774c696e6b7c5261774c696e6b49647c72657175697265640a656e746974797c5261774c696e6b536174656c6c6974657c5261774c696e6b536174656c6c6974654c6973740a70726f70657274797c5261774c696e6b536174656c6c6974657c4e616d657c72657175697265640a70726f70657274797c5261774c696e6b536174656c6c6974657c536174656c6c6974654b696e647c72657175697265640a72656c6174696f6e736869707c5261774c696e6b536174656c6c6974657c5261774c696e6b7c5261774c696e6b49647c72657175697265640a656e746974797c5261774c696e6b536174656c6c6974654174747269627574657c5261774c696e6b536174656c6c6974654174747269627574654c6973740a70726f70657274797c5261774c696e6b536174656c6c6974654174747269627574657c4e616d657c72657175697265640a72656c6174696f6e736869707c5261774c696e6b536174656c6c6974654174747269627574657c4669656c647c4669656c6449647c72657175697265640a72656c6174696f6e736869707c5261774c696e6b536174656c6c6974654174747269627574657c5261774c696e6b536174656c6c6974657c5261774c696e6b536174656c6c69746549647c7265717569726564";
    public global::MetaRawDataVault.MetaRawDataVaultModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaRawDataVault.MetaRawDataVaultModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaRawDataVault.Field>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Field"))
        {
            var row = new global::MetaRawDataVault.Field
            {
                Id = source.Id
            };
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.FieldList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaRawDataVault.FieldDataTypeDetail>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FieldDataTypeDetail"))
        {
            var row = new global::MetaRawDataVault.FieldDataTypeDetail
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Value = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Value");
            typed.FieldDataTypeDetailList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaRawDataVault.RawHub>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawHub"))
        {
            var row = new global::MetaRawDataVault.RawHub
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.RawHubList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaRawDataVault.RawHubKeyPart>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawHubKeyPart"))
        {
            var row = new global::MetaRawDataVault.RawHubKeyPart
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.RawHubKeyPartList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaRawDataVault.RawHubSatellite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawHubSatellite"))
        {
            var row = new global::MetaRawDataVault.RawHubSatellite
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.SatelliteKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SatelliteKind");
            typed.RawHubSatelliteList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaRawDataVault.RawHubSatelliteAttribute>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawHubSatelliteAttribute"))
        {
            var row = new global::MetaRawDataVault.RawHubSatelliteAttribute
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.RawHubSatelliteAttributeList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaRawDataVault.RawLink>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawLink"))
        {
            var row = new global::MetaRawDataVault.RawLink
            {
                Id = source.Id
            };
            row.LinkKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LinkKind");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.RawLinkList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaRawDataVault.RawLinkRole>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawLinkRole"))
        {
            var row = new global::MetaRawDataVault.RawLinkRole
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.RawLinkRoleList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaRawDataVault.RawLinkSatellite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawLinkSatellite"))
        {
            var row = new global::MetaRawDataVault.RawLinkSatellite
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.SatelliteKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SatelliteKind");
            typed.RawLinkSatelliteList.Add(row);
            index8.Add(source.Id, row);
        }

        var index9 = new global::System.Collections.Generic.Dictionary<string, global::MetaRawDataVault.RawLinkSatelliteAttribute>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawLinkSatelliteAttribute"))
        {
            var row = new global::MetaRawDataVault.RawLinkSatelliteAttribute
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.RawLinkSatelliteAttributeList.Add(row);
            index9.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FieldDataTypeDetail"))
        {
            var row = index1[source.Id];
            row.Field = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FieldId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawHubKeyPart"))
        {
            var row = index3[source.Id];
            row.Field = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FieldId", index0);
            row.RawHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RawHubId", index2);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawHubSatellite"))
        {
            var row = index4[source.Id];
            row.RawHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RawHubId", index2);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawHubSatelliteAttribute"))
        {
            var row = index5[source.Id];
            row.Field = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FieldId", index0);
            row.RawHubSatellite = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RawHubSatelliteId", index4);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawLinkRole"))
        {
            var row = index7[source.Id];
            row.RawHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RawHubId", index2);
            row.RawLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RawLinkId", index6);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawLinkSatellite"))
        {
            var row = index8[source.Id];
            row.RawLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RawLinkId", index6);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RawLinkSatelliteAttribute"))
        {
            var row = index9[source.Id];
            row.Field = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FieldId", index0);
            row.RawLinkSatellite = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RawLinkSatelliteId", index8);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaRawDataVault.MetaRawDataVaultModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FieldList, row => row.Id, "Field");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FieldDataTypeDetailList, row => row.Id, "FieldDataTypeDetail");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RawHubList, row => row.Id, "RawHub");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RawHubKeyPartList, row => row.Id, "RawHubKeyPart");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RawHubSatelliteList, row => row.Id, "RawHubSatellite");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RawHubSatelliteAttributeList, row => row.Id, "RawHubSatelliteAttribute");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RawLinkList, row => row.Id, "RawLink");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RawLinkRoleList, row => row.Id, "RawLinkRole");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RawLinkSatelliteList, row => row.Id, "RawLinkSatellite");
        var index9 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RawLinkSatelliteAttributeList, row => row.Id, "RawLinkSatelliteAttribute");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
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
            instance.GetOrCreateEntityRecords("RawHub").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Field is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Field, row.Field.Id, index0, "RawHubKeyPart.Field");
                target.RelationshipIds.Add("FieldId", row.Field.Id);
            }

            if (row.RawHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RawHub, row.RawHub.Id, index2, "RawHubKeyPart.RawHub");
                target.RelationshipIds.Add("RawHubId", row.RawHub.Id);
            }

            instance.GetOrCreateEntityRecords("RawHubKeyPart").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.SatelliteKind is not null)
                target.Values.Add("SatelliteKind", row.SatelliteKind);
            if (row.RawHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RawHub, row.RawHub.Id, index2, "RawHubSatellite.RawHub");
                target.RelationshipIds.Add("RawHubId", row.RawHub.Id);
            }

            instance.GetOrCreateEntityRecords("RawHubSatellite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Field is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Field, row.Field.Id, index0, "RawHubSatelliteAttribute.Field");
                target.RelationshipIds.Add("FieldId", row.Field.Id);
            }

            if (row.RawHubSatellite is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RawHubSatellite, row.RawHubSatellite.Id, index4, "RawHubSatelliteAttribute.RawHubSatellite");
                target.RelationshipIds.Add("RawHubSatelliteId", row.RawHubSatellite.Id);
            }

            instance.GetOrCreateEntityRecords("RawHubSatelliteAttribute").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LinkKind is not null)
                target.Values.Add("LinkKind", row.LinkKind);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("RawLink").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.RawHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RawHub, row.RawHub.Id, index2, "RawLinkRole.RawHub");
                target.RelationshipIds.Add("RawHubId", row.RawHub.Id);
            }

            if (row.RawLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RawLink, row.RawLink.Id, index6, "RawLinkRole.RawLink");
                target.RelationshipIds.Add("RawLinkId", row.RawLink.Id);
            }

            instance.GetOrCreateEntityRecords("RawLinkRole").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index8))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.SatelliteKind is not null)
                target.Values.Add("SatelliteKind", row.SatelliteKind);
            if (row.RawLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RawLink, row.RawLink.Id, index6, "RawLinkSatellite.RawLink");
                target.RelationshipIds.Add("RawLinkId", row.RawLink.Id);
            }

            instance.GetOrCreateEntityRecords("RawLinkSatellite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index9))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Field is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Field, row.Field.Id, index0, "RawLinkSatelliteAttribute.Field");
                target.RelationshipIds.Add("FieldId", row.Field.Id);
            }

            if (row.RawLinkSatellite is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RawLinkSatellite, row.RawLinkSatellite.Id, index8, "RawLinkSatelliteAttribute.RawLinkSatellite");
                target.RelationshipIds.Add("RawLinkSatelliteId", row.RawLinkSatellite.Id);
            }

            instance.GetOrCreateEntityRecords("RawLinkSatelliteAttribute").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}