// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaBusinessDataVault;
public sealed class MetaBusinessDataVaultWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaBusinessDataVault.MetaBusinessDataVaultModel>
{
    private const string ContractSignature = "6d6f64656c7c4d657461427573696e657373446174615661756c740a656e746974797c427573696e6573734272696467657c427573696e6573734272696467654c6973740a70726f70657274797c427573696e6573734272696467657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c427573696e6573734272696467657c4e616d657c72657175697265640a72656c6174696f6e736869707c427573696e6573734272696467657c427573696e6573734875627c427573696e65737348756249647c72657175697265640a656e746974797c427573696e65737342726964676554726176657273616c7c427573696e65737342726964676554726176657273616c4c6973740a72656c6174696f6e736869707c427573696e65737342726964676554726176657273616c7c427573696e6573734272696467657c427573696e65737342726964676549647c72657175697265640a72656c6174696f6e736869707c427573696e65737342726964676554726176657273616c7c427573696e65737342726964676554726176657273616c7c50726576696f757354726176657273616c49647c6e756c6c61626c650a72656c6174696f6e736869707c427573696e65737342726964676554726176657273616c7c427573696e6573734c696e6b526f6c657c536f75726365526f6c6549647c72657175697265640a72656c6174696f6e736869707c427573696e65737342726964676554726176657273616c7c427573696e6573734c696e6b526f6c657c546172676574526f6c6549647c72657175697265640a656e746974797c427573696e65737348696572617263686963616c4c696e6b7c427573696e65737348696572617263686963616c4c696e6b4c6973740a70726f70657274797c427573696e65737348696572617263686963616c4c696e6b7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c427573696e65737348696572617263686963616c4c696e6b7c4e616d657c72657175697265640a72656c6174696f6e736869707c427573696e65737348696572617263686963616c4c696e6b7c427573696e6573734875627c4368696c6448756249647c72657175697265640a72656c6174696f6e736869707c427573696e65737348696572617263686963616c4c696e6b7c427573696e6573734875627c506172656e7448756249647c72657175697265640a656e746974797c427573696e65737348696572617263686963616c4c696e6b536174656c6c6974657c427573696e65737348696572617263686963616c4c696e6b536174656c6c6974654c6973740a72656c6174696f6e736869707c427573696e65737348696572617263686963616c4c696e6b536174656c6c6974657c427573696e65737348696572617263686963616c4c696e6b7c427573696e65737348696572617263686963616c4c696e6b49647c72657175697265640a72656c6174696f6e736869707c427573696e65737348696572617263686963616c4c696e6b536174656c6c6974657c427573696e657373536174656c6c6974657c427573696e657373536174656c6c69746549647c72657175697265640a656e746974797c427573696e6573734875627c427573696e6573734875624c6973740a70726f70657274797c427573696e6573734875627c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c427573696e6573734875627c4e616d657c72657175697265640a656e746974797c427573696e6573734875624b6579506172747c427573696e6573734875624b6579506172744c6973740a70726f70657274797c427573696e6573734875624b6579506172747c446174615479706549647c72657175697265640a70726f70657274797c427573696e6573734875624b6579506172747c4e616d657c72657175697265640a72656c6174696f6e736869707c427573696e6573734875624b6579506172747c427573696e6573734875627c427573696e65737348756249647c72657175697265640a72656c6174696f6e736869707c427573696e6573734875624b6579506172747c427573696e6573734875624b6579506172747c50726576696f75734b65795061727449647c6e756c6c61626c650a656e746974797c427573696e6573734875624b657950617274446174615479706544657461696c7c427573696e6573734875624b657950617274446174615479706544657461696c4c6973740a70726f70657274797c427573696e6573734875624b657950617274446174615479706544657461696c7c4e616d657c72657175697265640a70726f70657274797c427573696e6573734875624b657950617274446174615479706544657461696c7c56616c75657c72657175697265640a72656c6174696f6e736869707c427573696e6573734875624b657950617274446174615479706544657461696c7c427573696e6573734875624b6579506172747c427573696e6573734875624b65795061727449647c72657175697265640a656e746974797c427573696e657373487562536174656c6c6974657c427573696e657373487562536174656c6c6974654c6973740a72656c6174696f6e736869707c427573696e657373487562536174656c6c6974657c427573696e6573734875627c427573696e65737348756249647c72657175697265640a72656c6174696f6e736869707c427573696e657373487562536174656c6c6974657c427573696e657373536174656c6c6974657c427573696e657373536174656c6c69746549647c72657175697265640a656e746974797c427573696e6573734c696e6b7c427573696e6573734c696e6b4c6973740a70726f70657274797c427573696e6573734c696e6b7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c427573696e6573734c696e6b7c4e616d657c72657175697265640a656e746974797c427573696e6573734c696e6b526f6c657c427573696e6573734c696e6b526f6c654c6973740a70726f70657274797c427573696e6573734c696e6b526f6c657c4e616d657c72657175697265640a72656c6174696f6e736869707c427573696e6573734c696e6b526f6c657c427573696e6573734875627c427573696e65737348756249647c72657175697265640a72656c6174696f6e736869707c427573696e6573734c696e6b526f6c657c427573696e6573734c696e6b7c427573696e6573734c696e6b49647c72657175697265640a656e746974797c427573696e6573734c696e6b536174656c6c6974657c427573696e6573734c696e6b536174656c6c6974654c6973740a72656c6174696f6e736869707c427573696e6573734c696e6b536174656c6c6974657c427573696e6573734c696e6b7c427573696e6573734c696e6b49647c72657175697265640a72656c6174696f6e736869707c427573696e6573734c696e6b536174656c6c6974657c427573696e657373536174656c6c6974657c427573696e657373536174656c6c69746549647c72657175697265640a656e746974797c427573696e657373506f696e74496e54696d657c427573696e657373506f696e74496e54696d654c6973740a70726f70657274797c427573696e657373506f696e74496e54696d657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c427573696e657373506f696e74496e54696d657c4e616d657c72657175697265640a72656c6174696f6e736869707c427573696e657373506f696e74496e54696d657c427573696e6573734875627c427573696e65737348756249647c72657175697265640a656e746974797c427573696e657373506f696e74496e54696d65487562536174656c6c6974657c427573696e657373506f696e74496e54696d65487562536174656c6c6974654c6973740a72656c6174696f6e736869707c427573696e657373506f696e74496e54696d65487562536174656c6c6974657c427573696e657373487562536174656c6c6974657c427573696e657373487562536174656c6c69746549647c72657175697265640a72656c6174696f6e736869707c427573696e657373506f696e74496e54696d65487562536174656c6c6974657c427573696e657373506f696e74496e54696d657c427573696e657373506f696e74496e54696d6549647c72657175697265640a656e746974797c427573696e657373506f696e74496e54696d654c696e6b536174656c6c6974657c427573696e657373506f696e74496e54696d654c696e6b536174656c6c6974654c6973740a72656c6174696f6e736869707c427573696e657373506f696e74496e54696d654c696e6b536174656c6c6974657c427573696e6573734c696e6b536174656c6c6974657c427573696e6573734c696e6b536174656c6c69746549647c72657175697265640a72656c6174696f6e736869707c427573696e657373506f696e74496e54696d654c696e6b536174656c6c6974657c427573696e657373506f696e74496e54696d657c427573696e657373506f696e74496e54696d6549647c72657175697265640a656e746974797c427573696e657373506f696e74496e54696d655374616d707c427573696e657373506f696e74496e54696d655374616d704c6973740a70726f70657274797c427573696e657373506f696e74496e54696d655374616d707c446174615479706549647c72657175697265640a70726f70657274797c427573696e657373506f696e74496e54696d655374616d707c4e616d657c72657175697265640a72656c6174696f6e736869707c427573696e657373506f696e74496e54696d655374616d707c427573696e657373506f696e74496e54696d657c427573696e657373506f696e74496e54696d6549647c72657175697265640a656e746974797c427573696e657373506f696e74496e54696d655374616d70446174615479706544657461696c7c427573696e657373506f696e74496e54696d655374616d70446174615479706544657461696c4c6973740a70726f70657274797c427573696e657373506f696e74496e54696d655374616d70446174615479706544657461696c7c4e616d657c72657175697265640a70726f70657274797c427573696e657373506f696e74496e54696d655374616d70446174615479706544657461696c7c56616c75657c72657175697265640a72656c6174696f6e736869707c427573696e657373506f696e74496e54696d655374616d70446174615479706544657461696c7c427573696e657373506f696e74496e54696d655374616d707c427573696e657373506f696e74496e54696d655374616d7049647c72657175697265640a656e746974797c427573696e6573735265666572656e63657c427573696e6573735265666572656e63654c6973740a70726f70657274797c427573696e6573735265666572656e63657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c427573696e6573735265666572656e63657c4e616d657c72657175697265640a656e746974797c427573696e6573735265666572656e63654b6579506172747c427573696e6573735265666572656e63654b6579506172744c6973740a70726f70657274797c427573696e6573735265666572656e63654b6579506172747c446174615479706549647c72657175697265640a70726f70657274797c427573696e6573735265666572656e63654b6579506172747c4e616d657c72657175697265640a72656c6174696f6e736869707c427573696e6573735265666572656e63654b6579506172747c427573696e6573735265666572656e63657c427573696e6573735265666572656e636549647c72657175697265640a72656c6174696f6e736869707c427573696e6573735265666572656e63654b6579506172747c427573696e6573735265666572656e63654b6579506172747c50726576696f75734b65795061727449647c6e756c6c61626c650a656e746974797c427573696e6573735265666572656e63654b657950617274446174615479706544657461696c7c427573696e6573735265666572656e63654b657950617274446174615479706544657461696c4c6973740a70726f70657274797c427573696e6573735265666572656e63654b657950617274446174615479706544657461696c7c4e616d657c72657175697265640a70726f70657274797c427573696e6573735265666572656e63654b657950617274446174615479706544657461696c7c56616c75657c72657175697265640a72656c6174696f6e736869707c427573696e6573735265666572656e63654b657950617274446174615479706544657461696c7c427573696e6573735265666572656e63654b6579506172747c427573696e6573735265666572656e63654b65795061727449647c72657175697265640a656e746974797c427573696e6573735265666572656e6365536174656c6c6974657c427573696e6573735265666572656e6365536174656c6c6974654c6973740a72656c6174696f6e736869707c427573696e6573735265666572656e6365536174656c6c6974657c427573696e6573735265666572656e63657c427573696e6573735265666572656e636549647c72657175697265640a72656c6174696f6e736869707c427573696e6573735265666572656e6365536174656c6c6974657c427573696e657373536174656c6c6974657c427573696e657373536174656c6c69746549647c72657175697265640a656e746974797c427573696e65737353616d6541734c696e6b7c427573696e65737353616d6541734c696e6b4c6973740a70726f70657274797c427573696e65737353616d6541734c696e6b7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c427573696e65737353616d6541734c696e6b7c4e616d657c72657175697265640a72656c6174696f6e736869707c427573696e65737353616d6541734c696e6b7c427573696e6573734875627c4571756976616c656e7448756249647c72657175697265640a72656c6174696f6e736869707c427573696e65737353616d6541734c696e6b7c427573696e6573734875627c5072696d61727948756249647c72657175697265640a656e746974797c427573696e65737353616d6541734c696e6b536174656c6c6974657c427573696e65737353616d6541734c696e6b536174656c6c6974654c6973740a72656c6174696f6e736869707c427573696e65737353616d6541734c696e6b536174656c6c6974657c427573696e65737353616d6541734c696e6b7c427573696e65737353616d6541734c696e6b49647c72657175697265640a72656c6174696f6e736869707c427573696e65737353616d6541734c696e6b536174656c6c6974657c427573696e657373536174656c6c6974657c427573696e657373536174656c6c69746549647c72657175697265640a656e746974797c427573696e657373536174656c6c6974657c427573696e657373536174656c6c6974654c6973740a70726f70657274797c427573696e657373536174656c6c6974657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c427573696e657373536174656c6c6974657c4e616d657c72657175697265640a656e746974797c427573696e657373536174656c6c6974654174747269627574657c427573696e657373536174656c6c6974654174747269627574654c6973740a70726f70657274797c427573696e657373536174656c6c6974654174747269627574657c446174615479706549647c72657175697265640a70726f70657274797c427573696e657373536174656c6c6974654174747269627574657c4e616d657c72657175697265640a72656c6174696f6e736869707c427573696e657373536174656c6c6974654174747269627574657c427573696e657373536174656c6c6974657c427573696e657373536174656c6c69746549647c72657175697265640a656e746974797c427573696e657373536174656c6c697465417474726962757465446174615479706544657461696c7c427573696e657373536174656c6c697465417474726962757465446174615479706544657461696c4c6973740a70726f70657274797c427573696e657373536174656c6c697465417474726962757465446174615479706544657461696c7c4e616d657c72657175697265640a70726f70657274797c427573696e657373536174656c6c697465417474726962757465446174615479706544657461696c7c56616c75657c72657175697265640a72656c6174696f6e736869707c427573696e657373536174656c6c697465417474726962757465446174615479706544657461696c7c427573696e657373536174656c6c6974654174747269627574657c427573696e657373536174656c6c69746541747472696275746549647c7265717569726564";
    public global::MetaBusinessDataVault.MetaBusinessDataVaultModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaBusinessDataVault.MetaBusinessDataVaultModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessBridge>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessBridge"))
        {
            var row = new global::MetaBusinessDataVault.BusinessBridge
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessBridgeList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessBridgeTraversal>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessBridgeTraversal"))
        {
            var row = new global::MetaBusinessDataVault.BusinessBridgeTraversal
            {
                Id = source.Id
            };
            typed.BusinessBridgeTraversalList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessHierarchicalLink>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessHierarchicalLink"))
        {
            var row = new global::MetaBusinessDataVault.BusinessHierarchicalLink
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessHierarchicalLinkList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessHierarchicalLinkSatellite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessHierarchicalLinkSatellite"))
        {
            var row = new global::MetaBusinessDataVault.BusinessHierarchicalLinkSatellite
            {
                Id = source.Id
            };
            typed.BusinessHierarchicalLinkSatelliteList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessHub>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessHub"))
        {
            var row = new global::MetaBusinessDataVault.BusinessHub
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessHubList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessHubKeyPart>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessHubKeyPart"))
        {
            var row = new global::MetaBusinessDataVault.BusinessHubKeyPart
            {
                Id = source.Id
            };
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessHubKeyPartList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessHubKeyPartDataTypeDetail>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessHubKeyPartDataTypeDetail"))
        {
            var row = new global::MetaBusinessDataVault.BusinessHubKeyPartDataTypeDetail
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Value = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Value");
            typed.BusinessHubKeyPartDataTypeDetailList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessHubSatellite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessHubSatellite"))
        {
            var row = new global::MetaBusinessDataVault.BusinessHubSatellite
            {
                Id = source.Id
            };
            typed.BusinessHubSatelliteList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessLink>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessLink"))
        {
            var row = new global::MetaBusinessDataVault.BusinessLink
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessLinkList.Add(row);
            index8.Add(source.Id, row);
        }

        var index9 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessLinkRole>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessLinkRole"))
        {
            var row = new global::MetaBusinessDataVault.BusinessLinkRole
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessLinkRoleList.Add(row);
            index9.Add(source.Id, row);
        }

        var index10 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessLinkSatellite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessLinkSatellite"))
        {
            var row = new global::MetaBusinessDataVault.BusinessLinkSatellite
            {
                Id = source.Id
            };
            typed.BusinessLinkSatelliteList.Add(row);
            index10.Add(source.Id, row);
        }

        var index11 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessPointInTime>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessPointInTime"))
        {
            var row = new global::MetaBusinessDataVault.BusinessPointInTime
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessPointInTimeList.Add(row);
            index11.Add(source.Id, row);
        }

        var index12 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessPointInTimeHubSatellite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessPointInTimeHubSatellite"))
        {
            var row = new global::MetaBusinessDataVault.BusinessPointInTimeHubSatellite
            {
                Id = source.Id
            };
            typed.BusinessPointInTimeHubSatelliteList.Add(row);
            index12.Add(source.Id, row);
        }

        var index13 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessPointInTimeLinkSatellite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessPointInTimeLinkSatellite"))
        {
            var row = new global::MetaBusinessDataVault.BusinessPointInTimeLinkSatellite
            {
                Id = source.Id
            };
            typed.BusinessPointInTimeLinkSatelliteList.Add(row);
            index13.Add(source.Id, row);
        }

        var index14 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessPointInTimeStamp>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessPointInTimeStamp"))
        {
            var row = new global::MetaBusinessDataVault.BusinessPointInTimeStamp
            {
                Id = source.Id
            };
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessPointInTimeStampList.Add(row);
            index14.Add(source.Id, row);
        }

        var index15 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessPointInTimeStampDataTypeDetail>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessPointInTimeStampDataTypeDetail"))
        {
            var row = new global::MetaBusinessDataVault.BusinessPointInTimeStampDataTypeDetail
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Value = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Value");
            typed.BusinessPointInTimeStampDataTypeDetailList.Add(row);
            index15.Add(source.Id, row);
        }

        var index16 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessReference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessReference"))
        {
            var row = new global::MetaBusinessDataVault.BusinessReference
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessReferenceList.Add(row);
            index16.Add(source.Id, row);
        }

        var index17 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessReferenceKeyPart>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessReferenceKeyPart"))
        {
            var row = new global::MetaBusinessDataVault.BusinessReferenceKeyPart
            {
                Id = source.Id
            };
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessReferenceKeyPartList.Add(row);
            index17.Add(source.Id, row);
        }

        var index18 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessReferenceKeyPartDataTypeDetail>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessReferenceKeyPartDataTypeDetail"))
        {
            var row = new global::MetaBusinessDataVault.BusinessReferenceKeyPartDataTypeDetail
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Value = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Value");
            typed.BusinessReferenceKeyPartDataTypeDetailList.Add(row);
            index18.Add(source.Id, row);
        }

        var index19 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessReferenceSatellite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessReferenceSatellite"))
        {
            var row = new global::MetaBusinessDataVault.BusinessReferenceSatellite
            {
                Id = source.Id
            };
            typed.BusinessReferenceSatelliteList.Add(row);
            index19.Add(source.Id, row);
        }

        var index20 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessSameAsLink>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessSameAsLink"))
        {
            var row = new global::MetaBusinessDataVault.BusinessSameAsLink
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessSameAsLinkList.Add(row);
            index20.Add(source.Id, row);
        }

        var index21 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessSameAsLinkSatellite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessSameAsLinkSatellite"))
        {
            var row = new global::MetaBusinessDataVault.BusinessSameAsLinkSatellite
            {
                Id = source.Id
            };
            typed.BusinessSameAsLinkSatelliteList.Add(row);
            index21.Add(source.Id, row);
        }

        var index22 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessSatellite>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessSatellite"))
        {
            var row = new global::MetaBusinessDataVault.BusinessSatellite
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessSatelliteList.Add(row);
            index22.Add(source.Id, row);
        }

        var index23 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessSatelliteAttribute>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessSatelliteAttribute"))
        {
            var row = new global::MetaBusinessDataVault.BusinessSatelliteAttribute
            {
                Id = source.Id
            };
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BusinessSatelliteAttributeList.Add(row);
            index23.Add(source.Id, row);
        }

        var index24 = new global::System.Collections.Generic.Dictionary<string, global::MetaBusinessDataVault.BusinessSatelliteAttributeDataTypeDetail>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessSatelliteAttributeDataTypeDetail"))
        {
            var row = new global::MetaBusinessDataVault.BusinessSatelliteAttributeDataTypeDetail
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Value = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Value");
            typed.BusinessSatelliteAttributeDataTypeDetailList.Add(row);
            index24.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessBridge"))
        {
            var row = index0[source.Id];
            row.BusinessHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessHubId", index4);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessBridgeTraversal"))
        {
            var row = index1[source.Id];
            row.BusinessBridge = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessBridgeId", index0);
            row.PreviousTraversal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "PreviousTraversalId", index1);
            row.SourceRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SourceRoleId", index9);
            row.TargetRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TargetRoleId", index9);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessHierarchicalLink"))
        {
            var row = index2[source.Id];
            row.ChildHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ChildHubId", index4);
            row.ParentHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ParentHubId", index4);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessHierarchicalLinkSatellite"))
        {
            var row = index3[source.Id];
            row.BusinessHierarchicalLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessHierarchicalLinkId", index2);
            row.BusinessSatellite = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessSatelliteId", index22);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessHubKeyPart"))
        {
            var row = index5[source.Id];
            row.BusinessHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessHubId", index4);
            row.PreviousKeyPart = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "PreviousKeyPartId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessHubKeyPartDataTypeDetail"))
        {
            var row = index6[source.Id];
            row.BusinessHubKeyPart = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessHubKeyPartId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessHubSatellite"))
        {
            var row = index7[source.Id];
            row.BusinessHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessHubId", index4);
            row.BusinessSatellite = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessSatelliteId", index22);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessLinkRole"))
        {
            var row = index9[source.Id];
            row.BusinessHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessHubId", index4);
            row.BusinessLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessLinkId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessLinkSatellite"))
        {
            var row = index10[source.Id];
            row.BusinessLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessLinkId", index8);
            row.BusinessSatellite = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessSatelliteId", index22);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessPointInTime"))
        {
            var row = index11[source.Id];
            row.BusinessHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessHubId", index4);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessPointInTimeHubSatellite"))
        {
            var row = index12[source.Id];
            row.BusinessHubSatellite = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessHubSatelliteId", index7);
            row.BusinessPointInTime = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessPointInTimeId", index11);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessPointInTimeLinkSatellite"))
        {
            var row = index13[source.Id];
            row.BusinessLinkSatellite = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessLinkSatelliteId", index10);
            row.BusinessPointInTime = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessPointInTimeId", index11);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessPointInTimeStamp"))
        {
            var row = index14[source.Id];
            row.BusinessPointInTime = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessPointInTimeId", index11);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessPointInTimeStampDataTypeDetail"))
        {
            var row = index15[source.Id];
            row.BusinessPointInTimeStamp = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessPointInTimeStampId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessReferenceKeyPart"))
        {
            var row = index17[source.Id];
            row.BusinessReference = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessReferenceId", index16);
            row.PreviousKeyPart = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "PreviousKeyPartId", index17);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessReferenceKeyPartDataTypeDetail"))
        {
            var row = index18[source.Id];
            row.BusinessReferenceKeyPart = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessReferenceKeyPartId", index17);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessReferenceSatellite"))
        {
            var row = index19[source.Id];
            row.BusinessReference = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessReferenceId", index16);
            row.BusinessSatellite = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessSatelliteId", index22);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessSameAsLink"))
        {
            var row = index20[source.Id];
            row.EquivalentHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "EquivalentHubId", index4);
            row.PrimaryHub = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PrimaryHubId", index4);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessSameAsLinkSatellite"))
        {
            var row = index21[source.Id];
            row.BusinessSameAsLink = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessSameAsLinkId", index20);
            row.BusinessSatellite = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessSatelliteId", index22);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessSatelliteAttribute"))
        {
            var row = index23[source.Id];
            row.BusinessSatellite = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessSatelliteId", index22);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BusinessSatelliteAttributeDataTypeDetail"))
        {
            var row = index24[source.Id];
            row.BusinessSatelliteAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BusinessSatelliteAttributeId", index23);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaBusinessDataVault.MetaBusinessDataVaultModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessBridgeList, row => row.Id, "BusinessBridge");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessBridgeTraversalList, row => row.Id, "BusinessBridgeTraversal");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessHierarchicalLinkList, row => row.Id, "BusinessHierarchicalLink");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessHierarchicalLinkSatelliteList, row => row.Id, "BusinessHierarchicalLinkSatellite");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessHubList, row => row.Id, "BusinessHub");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessHubKeyPartList, row => row.Id, "BusinessHubKeyPart");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessHubKeyPartDataTypeDetailList, row => row.Id, "BusinessHubKeyPartDataTypeDetail");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessHubSatelliteList, row => row.Id, "BusinessHubSatellite");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessLinkList, row => row.Id, "BusinessLink");
        var index9 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessLinkRoleList, row => row.Id, "BusinessLinkRole");
        var index10 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessLinkSatelliteList, row => row.Id, "BusinessLinkSatellite");
        var index11 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessPointInTimeList, row => row.Id, "BusinessPointInTime");
        var index12 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessPointInTimeHubSatelliteList, row => row.Id, "BusinessPointInTimeHubSatellite");
        var index13 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessPointInTimeLinkSatelliteList, row => row.Id, "BusinessPointInTimeLinkSatellite");
        var index14 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessPointInTimeStampList, row => row.Id, "BusinessPointInTimeStamp");
        var index15 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessPointInTimeStampDataTypeDetailList, row => row.Id, "BusinessPointInTimeStampDataTypeDetail");
        var index16 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessReferenceList, row => row.Id, "BusinessReference");
        var index17 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessReferenceKeyPartList, row => row.Id, "BusinessReferenceKeyPart");
        var index18 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessReferenceKeyPartDataTypeDetailList, row => row.Id, "BusinessReferenceKeyPartDataTypeDetail");
        var index19 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessReferenceSatelliteList, row => row.Id, "BusinessReferenceSatellite");
        var index20 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessSameAsLinkList, row => row.Id, "BusinessSameAsLink");
        var index21 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessSameAsLinkSatelliteList, row => row.Id, "BusinessSameAsLinkSatellite");
        var index22 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessSatelliteList, row => row.Id, "BusinessSatellite");
        var index23 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessSatelliteAttributeList, row => row.Id, "BusinessSatelliteAttribute");
        var index24 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BusinessSatelliteAttributeDataTypeDetailList, row => row.Id, "BusinessSatelliteAttributeDataTypeDetail");
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
            if (row.BusinessHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessHub, row.BusinessHub.Id, index4, "BusinessBridge.BusinessHub");
                target.RelationshipIds.Add("BusinessHubId", row.BusinessHub.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessBridge").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BusinessBridge is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessBridge, row.BusinessBridge.Id, index0, "BusinessBridgeTraversal.BusinessBridge");
                target.RelationshipIds.Add("BusinessBridgeId", row.BusinessBridge.Id);
            }

            if (row.PreviousTraversal is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PreviousTraversal, row.PreviousTraversal.Id, index1, "BusinessBridgeTraversal.PreviousTraversal");
                target.RelationshipIds.Add("PreviousTraversalId", row.PreviousTraversal.Id);
            }

            if (row.SourceRole is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SourceRole, row.SourceRole.Id, index9, "BusinessBridgeTraversal.SourceRole");
                target.RelationshipIds.Add("SourceRoleId", row.SourceRole.Id);
            }

            if (row.TargetRole is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TargetRole, row.TargetRole.Id, index9, "BusinessBridgeTraversal.TargetRole");
                target.RelationshipIds.Add("TargetRoleId", row.TargetRole.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessBridgeTraversal").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.ChildHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ChildHub, row.ChildHub.Id, index4, "BusinessHierarchicalLink.ChildHub");
                target.RelationshipIds.Add("ChildHubId", row.ChildHub.Id);
            }

            if (row.ParentHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ParentHub, row.ParentHub.Id, index4, "BusinessHierarchicalLink.ParentHub");
                target.RelationshipIds.Add("ParentHubId", row.ParentHub.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessHierarchicalLink").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BusinessHierarchicalLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessHierarchicalLink, row.BusinessHierarchicalLink.Id, index2, "BusinessHierarchicalLinkSatellite.BusinessHierarchicalLink");
                target.RelationshipIds.Add("BusinessHierarchicalLinkId", row.BusinessHierarchicalLink.Id);
            }

            if (row.BusinessSatellite is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessSatellite, row.BusinessSatellite.Id, index22, "BusinessHierarchicalLinkSatellite.BusinessSatellite");
                target.RelationshipIds.Add("BusinessSatelliteId", row.BusinessSatellite.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessHierarchicalLinkSatellite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("BusinessHub").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.BusinessHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessHub, row.BusinessHub.Id, index4, "BusinessHubKeyPart.BusinessHub");
                target.RelationshipIds.Add("BusinessHubId", row.BusinessHub.Id);
            }

            if (row.PreviousKeyPart is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PreviousKeyPart, row.PreviousKeyPart.Id, index5, "BusinessHubKeyPart.PreviousKeyPart");
                target.RelationshipIds.Add("PreviousKeyPartId", row.PreviousKeyPart.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessHubKeyPart").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Value is not null)
                target.Values.Add("Value", row.Value);
            if (row.BusinessHubKeyPart is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessHubKeyPart, row.BusinessHubKeyPart.Id, index5, "BusinessHubKeyPartDataTypeDetail.BusinessHubKeyPart");
                target.RelationshipIds.Add("BusinessHubKeyPartId", row.BusinessHubKeyPart.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessHubKeyPartDataTypeDetail").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BusinessHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessHub, row.BusinessHub.Id, index4, "BusinessHubSatellite.BusinessHub");
                target.RelationshipIds.Add("BusinessHubId", row.BusinessHub.Id);
            }

            if (row.BusinessSatellite is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessSatellite, row.BusinessSatellite.Id, index22, "BusinessHubSatellite.BusinessSatellite");
                target.RelationshipIds.Add("BusinessSatelliteId", row.BusinessSatellite.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessHubSatellite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index8))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("BusinessLink").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index9))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.BusinessHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessHub, row.BusinessHub.Id, index4, "BusinessLinkRole.BusinessHub");
                target.RelationshipIds.Add("BusinessHubId", row.BusinessHub.Id);
            }

            if (row.BusinessLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessLink, row.BusinessLink.Id, index8, "BusinessLinkRole.BusinessLink");
                target.RelationshipIds.Add("BusinessLinkId", row.BusinessLink.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessLinkRole").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index10))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BusinessLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessLink, row.BusinessLink.Id, index8, "BusinessLinkSatellite.BusinessLink");
                target.RelationshipIds.Add("BusinessLinkId", row.BusinessLink.Id);
            }

            if (row.BusinessSatellite is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessSatellite, row.BusinessSatellite.Id, index22, "BusinessLinkSatellite.BusinessSatellite");
                target.RelationshipIds.Add("BusinessSatelliteId", row.BusinessSatellite.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessLinkSatellite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index11))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.BusinessHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessHub, row.BusinessHub.Id, index4, "BusinessPointInTime.BusinessHub");
                target.RelationshipIds.Add("BusinessHubId", row.BusinessHub.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessPointInTime").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index12))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BusinessHubSatellite is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessHubSatellite, row.BusinessHubSatellite.Id, index7, "BusinessPointInTimeHubSatellite.BusinessHubSatellite");
                target.RelationshipIds.Add("BusinessHubSatelliteId", row.BusinessHubSatellite.Id);
            }

            if (row.BusinessPointInTime is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessPointInTime, row.BusinessPointInTime.Id, index11, "BusinessPointInTimeHubSatellite.BusinessPointInTime");
                target.RelationshipIds.Add("BusinessPointInTimeId", row.BusinessPointInTime.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessPointInTimeHubSatellite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index13))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BusinessLinkSatellite is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessLinkSatellite, row.BusinessLinkSatellite.Id, index10, "BusinessPointInTimeLinkSatellite.BusinessLinkSatellite");
                target.RelationshipIds.Add("BusinessLinkSatelliteId", row.BusinessLinkSatellite.Id);
            }

            if (row.BusinessPointInTime is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessPointInTime, row.BusinessPointInTime.Id, index11, "BusinessPointInTimeLinkSatellite.BusinessPointInTime");
                target.RelationshipIds.Add("BusinessPointInTimeId", row.BusinessPointInTime.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessPointInTimeLinkSatellite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index14))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.BusinessPointInTime is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessPointInTime, row.BusinessPointInTime.Id, index11, "BusinessPointInTimeStamp.BusinessPointInTime");
                target.RelationshipIds.Add("BusinessPointInTimeId", row.BusinessPointInTime.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessPointInTimeStamp").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index15))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Value is not null)
                target.Values.Add("Value", row.Value);
            if (row.BusinessPointInTimeStamp is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessPointInTimeStamp, row.BusinessPointInTimeStamp.Id, index14, "BusinessPointInTimeStampDataTypeDetail.BusinessPointInTimeStamp");
                target.RelationshipIds.Add("BusinessPointInTimeStampId", row.BusinessPointInTimeStamp.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessPointInTimeStampDataTypeDetail").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index16))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("BusinessReference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index17))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.BusinessReference is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessReference, row.BusinessReference.Id, index16, "BusinessReferenceKeyPart.BusinessReference");
                target.RelationshipIds.Add("BusinessReferenceId", row.BusinessReference.Id);
            }

            if (row.PreviousKeyPart is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PreviousKeyPart, row.PreviousKeyPart.Id, index17, "BusinessReferenceKeyPart.PreviousKeyPart");
                target.RelationshipIds.Add("PreviousKeyPartId", row.PreviousKeyPart.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessReferenceKeyPart").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index18))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Value is not null)
                target.Values.Add("Value", row.Value);
            if (row.BusinessReferenceKeyPart is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessReferenceKeyPart, row.BusinessReferenceKeyPart.Id, index17, "BusinessReferenceKeyPartDataTypeDetail.BusinessReferenceKeyPart");
                target.RelationshipIds.Add("BusinessReferenceKeyPartId", row.BusinessReferenceKeyPart.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessReferenceKeyPartDataTypeDetail").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index19))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BusinessReference is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessReference, row.BusinessReference.Id, index16, "BusinessReferenceSatellite.BusinessReference");
                target.RelationshipIds.Add("BusinessReferenceId", row.BusinessReference.Id);
            }

            if (row.BusinessSatellite is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessSatellite, row.BusinessSatellite.Id, index22, "BusinessReferenceSatellite.BusinessSatellite");
                target.RelationshipIds.Add("BusinessSatelliteId", row.BusinessSatellite.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessReferenceSatellite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index20))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.EquivalentHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.EquivalentHub, row.EquivalentHub.Id, index4, "BusinessSameAsLink.EquivalentHub");
                target.RelationshipIds.Add("EquivalentHubId", row.EquivalentHub.Id);
            }

            if (row.PrimaryHub is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PrimaryHub, row.PrimaryHub.Id, index4, "BusinessSameAsLink.PrimaryHub");
                target.RelationshipIds.Add("PrimaryHubId", row.PrimaryHub.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessSameAsLink").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index21))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BusinessSameAsLink is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessSameAsLink, row.BusinessSameAsLink.Id, index20, "BusinessSameAsLinkSatellite.BusinessSameAsLink");
                target.RelationshipIds.Add("BusinessSameAsLinkId", row.BusinessSameAsLink.Id);
            }

            if (row.BusinessSatellite is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessSatellite, row.BusinessSatellite.Id, index22, "BusinessSameAsLinkSatellite.BusinessSatellite");
                target.RelationshipIds.Add("BusinessSatelliteId", row.BusinessSatellite.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessSameAsLinkSatellite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index22))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("BusinessSatellite").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index23))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.BusinessSatellite is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessSatellite, row.BusinessSatellite.Id, index22, "BusinessSatelliteAttribute.BusinessSatellite");
                target.RelationshipIds.Add("BusinessSatelliteId", row.BusinessSatellite.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessSatelliteAttribute").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index24))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Value is not null)
                target.Values.Add("Value", row.Value);
            if (row.BusinessSatelliteAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BusinessSatelliteAttribute, row.BusinessSatelliteAttribute.Id, index23, "BusinessSatelliteAttributeDataTypeDetail.BusinessSatelliteAttribute");
                target.RelationshipIds.Add("BusinessSatelliteAttributeId", row.BusinessSatelliteAttribute.Id);
            }

            instance.GetOrCreateEntityRecords("BusinessSatelliteAttributeDataTypeDetail").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}