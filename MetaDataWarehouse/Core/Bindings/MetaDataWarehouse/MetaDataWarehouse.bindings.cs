// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaDataWarehouse;
public sealed class MetaDataWarehouseWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaDataWarehouse.MetaDataWarehouseModel>
{
    private const string ContractSignature = "6d6f64656c7c4d6574614461746157617265686f7573650a656e746974797c416363756d756c6174696e67536e617073686f74466163747c416363756d756c6174696e67536e617073686f74466163744c6973740a70726f70657274797c416363756d756c6174696e67536e617073686f74466163747c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c416363756d756c6174696e67536e617073686f74466163747c466163747c4661637449647c72657175697265640a656e746974797c416363756d756c6174696e67536e617073686f744d696c6573746f6e657c416363756d756c6174696e67536e617073686f744d696c6573746f6e654c6973740a70726f70657274797c416363756d756c6174696e67536e617073686f744d696c6573746f6e657c44617465526f6c654e616d657c72657175697265640a70726f70657274797c416363756d756c6174696e67536e617073686f744d696c6573746f6e657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c416363756d756c6174696e67536e617073686f744d696c6573746f6e657c4e616d657c72657175697265640a70726f70657274797c416363756d756c6174696e67536e617073686f744d696c6573746f6e657c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c416363756d756c6174696e67536e617073686f744d696c6573746f6e657c416363756d756c6174696e67536e617073686f74466163747c416363756d756c6174696e67536e617073686f744661637449647c72657175697265640a656e746974797c416767726567617465466163747c416767726567617465466163744c6973740a70726f70657274797c416767726567617465466163747c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c416767726567617465466163747c466163747c416767726567617465644661637449647c72657175697265640a72656c6174696f6e736869707c416767726567617465466163747c466163747c536f757263654661637449647c72657175697265640a656e746974797c4272696467655061727469636970616e747c4272696467655061727469636970616e744c6973740a70726f70657274797c4272696467655061727469636970616e747c497352657175697265647c6e756c6c61626c650a70726f70657274797c4272696467655061727469636970616e747c4f7264696e616c7c72657175697265640a70726f70657274797c4272696467655061727469636970616e747c526f6c654e616d657c72657175697265640a72656c6174696f6e736869707c4272696467655061727469636970616e747c4272696467655461626c657c4272696467655461626c6549647c72657175697265640a72656c6174696f6e736869707c4272696467655061727469636970616e747c44696d656e73696f6e7c44696d656e73696f6e49647c72657175697265640a656e746974797c4272696467655461626c657c4272696467655461626c654c6973740a70726f70657274797c4272696467655461626c657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c4272696467655461626c657c4e616d657c72657175697265640a72656c6174696f6e736869707c4272696467655461626c657c57617265686f7573657c57617265686f75736549647c72657175697265640a656e746974797c4272696467655765696768747c4272696467655765696768744c6973740a70726f70657274797c4272696467655765696768747c446174615479706549647c72657175697265640a70726f70657274797c4272696467655765696768747c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c4272696467655765696768747c4e616d657c72657175697265640a72656c6174696f6e736869707c4272696467655765696768747c4272696467655461626c657c4272696467655461626c6549647c72657175697265640a656e746974797c436f6e666f726d656444696d656e73696f6e7c436f6e666f726d656444696d656e73696f6e4c6973740a70726f70657274797c436f6e666f726d656444696d656e73696f6e7c436f6e666f726d616e63654e616d657c72657175697265640a70726f70657274797c436f6e666f726d656444696d656e73696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c436f6e666f726d656444696d656e73696f6e7c44696d656e73696f6e7c44696d656e73696f6e49647c72657175697265640a656e746974797c446567656e657261746544696d656e73696f6e7c446567656e657261746544696d656e73696f6e4c6973740a70726f70657274797c446567656e657261746544696d656e73696f6e7c446174615479706549647c72657175697265640a70726f70657274797c446567656e657261746544696d656e73696f6e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c446567656e657261746544696d656e73696f6e7c4e616d657c72657175697265640a70726f70657274797c446567656e657261746544696d656e73696f6e7c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c446567656e657261746544696d656e73696f6e7c466163747c4661637449647c72657175697265640a656e746974797c44696d656e73696f6e7c44696d656e73696f6e4c6973740a70726f70657274797c44696d656e73696f6e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c44696d656e73696f6e7c4e616d657c72657175697265640a72656c6174696f6e736869707c44696d656e73696f6e7c57617265686f7573657c57617265686f75736549647c72657175697265640a656e746974797c44696d656e73696f6e4174747269627574657c44696d656e73696f6e4174747269627574654c6973740a70726f70657274797c44696d656e73696f6e4174747269627574657c446174615479706549647c72657175697265640a70726f70657274797c44696d656e73696f6e4174747269627574657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c44696d656e73696f6e4174747269627574657c49734e756c6c61626c657c6e756c6c61626c650a70726f70657274797c44696d656e73696f6e4174747269627574657c4e616d657c72657175697265640a70726f70657274797c44696d656e73696f6e4174747269627574657c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c44696d656e73696f6e4174747269627574657c44696d656e73696f6e7c44696d656e73696f6e49647c72657175697265640a656e746974797c44696d656e73696f6e427573696e6573734b65797c44696d656e73696f6e427573696e6573734b65794c6973740a70726f70657274797c44696d656e73696f6e427573696e6573734b65797c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c44696d656e73696f6e427573696e6573734b65797c4e616d657c72657175697265640a72656c6174696f6e736869707c44696d656e73696f6e427573696e6573734b65797c44696d656e73696f6e7c44696d656e73696f6e49647c72657175697265640a656e746974797c44696d656e73696f6e427573696e6573734b6579506172747c44696d656e73696f6e427573696e6573734b6579506172744c6973740a70726f70657274797c44696d656e73696f6e427573696e6573734b6579506172747c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c44696d656e73696f6e427573696e6573734b6579506172747c44696d656e73696f6e4174747269627574657c44696d656e73696f6e41747472696275746549647c72657175697265640a72656c6174696f6e736869707c44696d656e73696f6e427573696e6573734b6579506172747c44696d656e73696f6e427573696e6573734b65797c44696d656e73696f6e427573696e6573734b657949647c72657175697265640a656e746974797c44696d656e73696f6e4869657261726368797c44696d656e73696f6e4869657261726368794c6973740a70726f70657274797c44696d656e73696f6e4869657261726368797c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c44696d656e73696f6e4869657261726368797c4e616d657c72657175697265640a72656c6174696f6e736869707c44696d656e73696f6e4869657261726368797c44696d656e73696f6e7c44696d656e73696f6e49647c72657175697265640a656e746974797c44696d656e73696f6e4869657261726368794c6576656c7c44696d656e73696f6e4869657261726368794c6576656c4c6973740a70726f70657274797c44696d656e73696f6e4869657261726368794c6576656c7c4e616d657c72657175697265640a70726f70657274797c44696d656e73696f6e4869657261726368794c6576656c7c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c44696d656e73696f6e4869657261726368794c6576656c7c44696d656e73696f6e4174747269627574657c44696d656e73696f6e41747472696275746549647c72657175697265640a72656c6174696f6e736869707c44696d656e73696f6e4869657261726368794c6576656c7c44696d656e73696f6e4869657261726368797c44696d656e73696f6e48696572617263687949647c72657175697265640a656e746974797c466163747c466163744c6973740a70726f70657274797c466163747c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c466163747c4e616d657c72657175697265640a72656c6174696f6e736869707c466163747c57617265686f7573657c57617265686f75736549647c72657175697265640a656e746974797c466163744272696467657c466163744272696467654c6973740a70726f70657274797c466163744272696467657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c466163744272696467657c4f7264696e616c7c72657175697265640a70726f70657274797c466163744272696467657c526f6c654e616d657c72657175697265640a72656c6174696f6e736869707c466163744272696467657c4272696467655461626c657c4272696467655461626c6549647c72657175697265640a72656c6174696f6e736869707c466163744272696467657c466163747c4661637449647c72657175697265640a656e746974797c4661637444696d656e73696f6e7c4661637444696d656e73696f6e4c6973740a70726f70657274797c4661637444696d656e73696f6e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c4661637444696d656e73696f6e7c497352657175697265647c6e756c6c61626c650a70726f70657274797c4661637444696d656e73696f6e7c4f7264696e616c7c72657175697265640a70726f70657274797c4661637444696d656e73696f6e7c526f6c654e616d657c72657175697265640a72656c6174696f6e736869707c4661637444696d656e73696f6e7c44696d656e73696f6e7c44696d656e73696f6e49647c72657175697265640a72656c6174696f6e736869707c4661637444696d656e73696f6e7c466163747c4661637449647c72657175697265640a656e746974797c46616374477261696e7c46616374477261696e4c6973740a70726f70657274797c46616374477261696e7c4465736372697074696f6e7c72657175697265640a70726f70657274797c46616374477261696e7c4e616d657c72657175697265640a72656c6174696f6e736869707c46616374477261696e7c466163747c4661637449647c72657175697265640a656e746974797c466163746c657373466163747c466163746c657373466163744c6973740a70726f70657274797c466163746c657373466163747c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c466163746c657373466163747c466163747c4661637449647c72657175697265640a656e746974797c466163744d6561737572657c466163744d6561737572654c6973740a70726f70657274797c466163744d6561737572657c446174615479706549647c72657175697265640a70726f70657274797c466163744d6561737572657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c466163744d6561737572657c49734e756c6c61626c657c6e756c6c61626c650a70726f70657274797c466163744d6561737572657c4e616d657c72657175697265640a70726f70657274797c466163744d6561737572657c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c466163744d6561737572657c466163747c4661637449647c72657175697265640a656e746974797c4a756e6b44696d656e73696f6e7c4a756e6b44696d656e73696f6e4c6973740a70726f70657274797c4a756e6b44696d656e73696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c4a756e6b44696d656e73696f6e7c44696d656e73696f6e7c44696d656e73696f6e49647c72657175697265640a656e746974797c4a756e6b44696d656e73696f6e436f6d706f6e656e747c4a756e6b44696d656e73696f6e436f6d706f6e656e744c6973740a70726f70657274797c4a756e6b44696d656e73696f6e436f6d706f6e656e747c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c4a756e6b44696d656e73696f6e436f6d706f6e656e747c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c4a756e6b44696d656e73696f6e436f6d706f6e656e747c44696d656e73696f6e4174747269627574657c44696d656e73696f6e41747472696275746549647c72657175697265640a72656c6174696f6e736869707c4a756e6b44696d656e73696f6e436f6d706f6e656e747c4a756e6b44696d656e73696f6e7c4a756e6b44696d656e73696f6e49647c72657175697265640a656e746974797c4d696e6944696d656e73696f6e7c4d696e6944696d656e73696f6e4c6973740a70726f70657274797c4d696e6944696d656e73696f6e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c4d696e6944696d656e73696f6e7c526f6c654e616d657c6e756c6c61626c650a72656c6174696f6e736869707c4d696e6944696d656e73696f6e7c44696d656e73696f6e7c50726f66696c6544696d656e73696f6e49647c72657175697265640a72656c6174696f6e736869707c4d696e6944696d656e73696f6e7c44696d656e73696f6e7c536f7572636544696d656e73696f6e49647c72657175697265640a656e746974797c4f757472696767657244696d656e73696f6e7c4f757472696767657244696d656e73696f6e4c6973740a70726f70657274797c4f757472696767657244696d656e73696f6e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c4f757472696767657244696d656e73696f6e7c497352657175697265647c6e756c6c61626c650a70726f70657274797c4f757472696767657244696d656e73696f6e7c4f7264696e616c7c72657175697265640a70726f70657274797c4f757472696767657244696d656e73696f6e7c526f6c654e616d657c72657175697265640a72656c6174696f6e736869707c4f757472696767657244696d656e73696f6e7c44696d656e73696f6e7c4368696c6444696d656e73696f6e49647c72657175697265640a72656c6174696f6e736869707c4f757472696767657244696d656e73696f6e7c44696d656e73696f6e7c506172656e7444696d656e73696f6e49647c72657175697265640a656e746974797c506572696f646963536e617073686f74466163747c506572696f646963536e617073686f74466163744c6973740a70726f70657274797c506572696f646963536e617073686f74466163747c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c506572696f646963536e617073686f74466163747c506572696f644e616d657c72657175697265640a72656c6174696f6e736869707c506572696f646963536e617073686f74466163747c466163747c4661637449647c72657175697265640a656e746974797c536c6f776c794368616e67696e6744696d656e73696f6e7c536c6f776c794368616e67696e6744696d656e73696f6e4c6973740a70726f70657274797c536c6f776c794368616e67696e6744696d656e73696f6e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c536c6f776c794368616e67696e6744696d656e73696f6e7c4e616d657c6e756c6c61626c650a72656c6174696f6e736869707c536c6f776c794368616e67696e6744696d656e73696f6e7c44696d656e73696f6e7c44696d656e73696f6e49647c72657175697265640a656e746974797c5472616e73616374696f6e466163747c5472616e73616374696f6e466163744c6973740a70726f70657274797c5472616e73616374696f6e466163747c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c5472616e73616374696f6e466163747c466163747c4661637449647c72657175697265640a656e746974797c547970653144696d656e73696f6e4174747269627574657c547970653144696d656e73696f6e4174747269627574654c6973740a70726f70657274797c547970653144696d656e73696f6e4174747269627574657c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c547970653144696d656e73696f6e4174747269627574657c44696d656e73696f6e4174747269627574657c44696d656e73696f6e41747472696275746549647c72657175697265640a72656c6174696f6e736869707c547970653144696d656e73696f6e4174747269627574657c536c6f776c794368616e67696e6744696d656e73696f6e7c536c6f776c794368616e67696e6744696d656e73696f6e49647c72657175697265640a656e746974797c547970653244696d656e73696f6e4174747269627574657c547970653244696d656e73696f6e4174747269627574654c6973740a70726f70657274797c547970653244696d656e73696f6e4174747269627574657c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c547970653244696d656e73696f6e4174747269627574657c44696d656e73696f6e4174747269627574657c44696d656e73696f6e41747472696275746549647c72657175697265640a72656c6174696f6e736869707c547970653244696d656e73696f6e4174747269627574657c536c6f776c794368616e67696e6744696d656e73696f6e7c536c6f776c794368616e67696e6744696d656e73696f6e49647c72657175697265640a656e746974797c57617265686f7573657c57617265686f7573654c6973740a70726f70657274797c57617265686f7573657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c57617265686f7573657c4e616d657c7265717569726564";
    public global::MetaDataWarehouse.MetaDataWarehouseModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaDataWarehouse.MetaDataWarehouseModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.AccumulatingSnapshotFact>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AccumulatingSnapshotFact"))
        {
            var row = new global::MetaDataWarehouse.AccumulatingSnapshotFact
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.AccumulatingSnapshotFactList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.AccumulatingSnapshotMilestone>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AccumulatingSnapshotMilestone"))
        {
            var row = new global::MetaDataWarehouse.AccumulatingSnapshotMilestone
            {
                Id = source.Id
            };
            row.DateRoleName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DateRoleName");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.AccumulatingSnapshotMilestoneList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.AggregateFact>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AggregateFact"))
        {
            var row = new global::MetaDataWarehouse.AggregateFact
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.AggregateFactList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.BridgeParticipant>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BridgeParticipant"))
        {
            var row = new global::MetaDataWarehouse.BridgeParticipant
            {
                Id = source.Id
            };
            row.IsRequired = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsRequired");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            row.RoleName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "RoleName");
            typed.BridgeParticipantList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.BridgeTable>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BridgeTable"))
        {
            var row = new global::MetaDataWarehouse.BridgeTable
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BridgeTableList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.BridgeWeight>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BridgeWeight"))
        {
            var row = new global::MetaDataWarehouse.BridgeWeight
            {
                Id = source.Id
            };
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.BridgeWeightList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.ConformedDimension>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ConformedDimension"))
        {
            var row = new global::MetaDataWarehouse.ConformedDimension
            {
                Id = source.Id
            };
            row.ConformanceName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ConformanceName");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.ConformedDimensionList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.DegenerateDimension>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DegenerateDimension"))
        {
            var row = new global::MetaDataWarehouse.DegenerateDimension
            {
                Id = source.Id
            };
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.DegenerateDimensionList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.Dimension>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Dimension"))
        {
            var row = new global::MetaDataWarehouse.Dimension
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.DimensionList.Add(row);
            index8.Add(source.Id, row);
        }

        var index9 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.DimensionAttribute>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DimensionAttribute"))
        {
            var row = new global::MetaDataWarehouse.DimensionAttribute
            {
                Id = source.Id
            };
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.IsNullable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsNullable");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.DimensionAttributeList.Add(row);
            index9.Add(source.Id, row);
        }

        var index10 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.DimensionBusinessKey>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DimensionBusinessKey"))
        {
            var row = new global::MetaDataWarehouse.DimensionBusinessKey
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.DimensionBusinessKeyList.Add(row);
            index10.Add(source.Id, row);
        }

        var index11 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.DimensionBusinessKeyPart>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DimensionBusinessKeyPart"))
        {
            var row = new global::MetaDataWarehouse.DimensionBusinessKeyPart
            {
                Id = source.Id
            };
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.DimensionBusinessKeyPartList.Add(row);
            index11.Add(source.Id, row);
        }

        var index12 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.DimensionHierarchy>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DimensionHierarchy"))
        {
            var row = new global::MetaDataWarehouse.DimensionHierarchy
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.DimensionHierarchyList.Add(row);
            index12.Add(source.Id, row);
        }

        var index13 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.DimensionHierarchyLevel>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DimensionHierarchyLevel"))
        {
            var row = new global::MetaDataWarehouse.DimensionHierarchyLevel
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.DimensionHierarchyLevelList.Add(row);
            index13.Add(source.Id, row);
        }

        var index14 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.Fact>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Fact"))
        {
            var row = new global::MetaDataWarehouse.Fact
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.FactList.Add(row);
            index14.Add(source.Id, row);
        }

        var index15 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.FactBridge>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FactBridge"))
        {
            var row = new global::MetaDataWarehouse.FactBridge
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            row.RoleName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "RoleName");
            typed.FactBridgeList.Add(row);
            index15.Add(source.Id, row);
        }

        var index16 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.FactDimension>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FactDimension"))
        {
            var row = new global::MetaDataWarehouse.FactDimension
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.IsRequired = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsRequired");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            row.RoleName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "RoleName");
            typed.FactDimensionList.Add(row);
            index16.Add(source.Id, row);
        }

        var index17 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.FactGrain>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FactGrain"))
        {
            var row = new global::MetaDataWarehouse.FactGrain
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.FactGrainList.Add(row);
            index17.Add(source.Id, row);
        }

        var index18 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.FactlessFact>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FactlessFact"))
        {
            var row = new global::MetaDataWarehouse.FactlessFact
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.FactlessFactList.Add(row);
            index18.Add(source.Id, row);
        }

        var index19 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.FactMeasure>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FactMeasure"))
        {
            var row = new global::MetaDataWarehouse.FactMeasure
            {
                Id = source.Id
            };
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.IsNullable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsNullable");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.FactMeasureList.Add(row);
            index19.Add(source.Id, row);
        }

        var index20 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.JunkDimension>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "JunkDimension"))
        {
            var row = new global::MetaDataWarehouse.JunkDimension
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.JunkDimensionList.Add(row);
            index20.Add(source.Id, row);
        }

        var index21 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.JunkDimensionComponent>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "JunkDimensionComponent"))
        {
            var row = new global::MetaDataWarehouse.JunkDimensionComponent
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.JunkDimensionComponentList.Add(row);
            index21.Add(source.Id, row);
        }

        var index22 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.MiniDimension>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MiniDimension"))
        {
            var row = new global::MetaDataWarehouse.MiniDimension
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.RoleName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "RoleName");
            typed.MiniDimensionList.Add(row);
            index22.Add(source.Id, row);
        }

        var index23 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.OutriggerDimension>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "OutriggerDimension"))
        {
            var row = new global::MetaDataWarehouse.OutriggerDimension
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.IsRequired = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsRequired");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            row.RoleName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "RoleName");
            typed.OutriggerDimensionList.Add(row);
            index23.Add(source.Id, row);
        }

        var index24 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.PeriodicSnapshotFact>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PeriodicSnapshotFact"))
        {
            var row = new global::MetaDataWarehouse.PeriodicSnapshotFact
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.PeriodName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "PeriodName");
            typed.PeriodicSnapshotFactList.Add(row);
            index24.Add(source.Id, row);
        }

        var index25 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.SlowlyChangingDimension>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SlowlyChangingDimension"))
        {
            var row = new global::MetaDataWarehouse.SlowlyChangingDimension
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Name");
            typed.SlowlyChangingDimensionList.Add(row);
            index25.Add(source.Id, row);
        }

        var index26 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.TransactionFact>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransactionFact"))
        {
            var row = new global::MetaDataWarehouse.TransactionFact
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.TransactionFactList.Add(row);
            index26.Add(source.Id, row);
        }

        var index27 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.Type1DimensionAttribute>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Type1DimensionAttribute"))
        {
            var row = new global::MetaDataWarehouse.Type1DimensionAttribute
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.Type1DimensionAttributeList.Add(row);
            index27.Add(source.Id, row);
        }

        var index28 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.Type2DimensionAttribute>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Type2DimensionAttribute"))
        {
            var row = new global::MetaDataWarehouse.Type2DimensionAttribute
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.Type2DimensionAttributeList.Add(row);
            index28.Add(source.Id, row);
        }

        var index29 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouse.Warehouse>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Warehouse"))
        {
            var row = new global::MetaDataWarehouse.Warehouse
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.WarehouseList.Add(row);
            index29.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AccumulatingSnapshotFact"))
        {
            var row = index0[source.Id];
            row.Fact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FactId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AccumulatingSnapshotMilestone"))
        {
            var row = index1[source.Id];
            row.AccumulatingSnapshotFact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AccumulatingSnapshotFactId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AggregateFact"))
        {
            var row = index2[source.Id];
            row.AggregatedFact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AggregatedFactId", index14);
            row.SourceFact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SourceFactId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BridgeParticipant"))
        {
            var row = index3[source.Id];
            row.BridgeTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BridgeTableId", index4);
            row.Dimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BridgeTable"))
        {
            var row = index4[source.Id];
            row.Warehouse = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "WarehouseId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BridgeWeight"))
        {
            var row = index5[source.Id];
            row.BridgeTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BridgeTableId", index4);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ConformedDimension"))
        {
            var row = index6[source.Id];
            row.Dimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DegenerateDimension"))
        {
            var row = index7[source.Id];
            row.Fact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FactId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Dimension"))
        {
            var row = index8[source.Id];
            row.Warehouse = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "WarehouseId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DimensionAttribute"))
        {
            var row = index9[source.Id];
            row.Dimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DimensionBusinessKey"))
        {
            var row = index10[source.Id];
            row.Dimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DimensionBusinessKeyPart"))
        {
            var row = index11[source.Id];
            row.DimensionAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionAttributeId", index9);
            row.DimensionBusinessKey = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionBusinessKeyId", index10);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DimensionHierarchy"))
        {
            var row = index12[source.Id];
            row.Dimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DimensionHierarchyLevel"))
        {
            var row = index13[source.Id];
            row.DimensionAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionAttributeId", index9);
            row.DimensionHierarchy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionHierarchyId", index12);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Fact"))
        {
            var row = index14[source.Id];
            row.Warehouse = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "WarehouseId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FactBridge"))
        {
            var row = index15[source.Id];
            row.BridgeTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BridgeTableId", index4);
            row.Fact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FactId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FactDimension"))
        {
            var row = index16[source.Id];
            row.Dimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionId", index8);
            row.Fact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FactId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FactGrain"))
        {
            var row = index17[source.Id];
            row.Fact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FactId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FactlessFact"))
        {
            var row = index18[source.Id];
            row.Fact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FactId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FactMeasure"))
        {
            var row = index19[source.Id];
            row.Fact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FactId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "JunkDimension"))
        {
            var row = index20[source.Id];
            row.Dimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "JunkDimensionComponent"))
        {
            var row = index21[source.Id];
            row.DimensionAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionAttributeId", index9);
            row.JunkDimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "JunkDimensionId", index20);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MiniDimension"))
        {
            var row = index22[source.Id];
            row.ProfileDimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ProfileDimensionId", index8);
            row.SourceDimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SourceDimensionId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "OutriggerDimension"))
        {
            var row = index23[source.Id];
            row.ChildDimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ChildDimensionId", index8);
            row.ParentDimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ParentDimensionId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PeriodicSnapshotFact"))
        {
            var row = index24[source.Id];
            row.Fact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FactId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SlowlyChangingDimension"))
        {
            var row = index25[source.Id];
            row.Dimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TransactionFact"))
        {
            var row = index26[source.Id];
            row.Fact = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FactId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Type1DimensionAttribute"))
        {
            var row = index27[source.Id];
            row.DimensionAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionAttributeId", index9);
            row.SlowlyChangingDimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SlowlyChangingDimensionId", index25);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Type2DimensionAttribute"))
        {
            var row = index28[source.Id];
            row.DimensionAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DimensionAttributeId", index9);
            row.SlowlyChangingDimension = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SlowlyChangingDimensionId", index25);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaDataWarehouse.MetaDataWarehouseModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AccumulatingSnapshotFactList, row => row.Id, "AccumulatingSnapshotFact");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AccumulatingSnapshotMilestoneList, row => row.Id, "AccumulatingSnapshotMilestone");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AggregateFactList, row => row.Id, "AggregateFact");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BridgeParticipantList, row => row.Id, "BridgeParticipant");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BridgeTableList, row => row.Id, "BridgeTable");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BridgeWeightList, row => row.Id, "BridgeWeight");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ConformedDimensionList, row => row.Id, "ConformedDimension");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DegenerateDimensionList, row => row.Id, "DegenerateDimension");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DimensionList, row => row.Id, "Dimension");
        var index9 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DimensionAttributeList, row => row.Id, "DimensionAttribute");
        var index10 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DimensionBusinessKeyList, row => row.Id, "DimensionBusinessKey");
        var index11 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DimensionBusinessKeyPartList, row => row.Id, "DimensionBusinessKeyPart");
        var index12 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DimensionHierarchyList, row => row.Id, "DimensionHierarchy");
        var index13 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DimensionHierarchyLevelList, row => row.Id, "DimensionHierarchyLevel");
        var index14 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FactList, row => row.Id, "Fact");
        var index15 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FactBridgeList, row => row.Id, "FactBridge");
        var index16 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FactDimensionList, row => row.Id, "FactDimension");
        var index17 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FactGrainList, row => row.Id, "FactGrain");
        var index18 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FactlessFactList, row => row.Id, "FactlessFact");
        var index19 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FactMeasureList, row => row.Id, "FactMeasure");
        var index20 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.JunkDimensionList, row => row.Id, "JunkDimension");
        var index21 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.JunkDimensionComponentList, row => row.Id, "JunkDimensionComponent");
        var index22 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.MiniDimensionList, row => row.Id, "MiniDimension");
        var index23 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.OutriggerDimensionList, row => row.Id, "OutriggerDimension");
        var index24 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PeriodicSnapshotFactList, row => row.Id, "PeriodicSnapshotFact");
        var index25 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SlowlyChangingDimensionList, row => row.Id, "SlowlyChangingDimension");
        var index26 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TransactionFactList, row => row.Id, "TransactionFact");
        var index27 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.Type1DimensionAttributeList, row => row.Id, "Type1DimensionAttribute");
        var index28 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.Type2DimensionAttributeList, row => row.Id, "Type2DimensionAttribute");
        var index29 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.WarehouseList, row => row.Id, "Warehouse");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Fact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Fact, row.Fact.Id, index14, "AccumulatingSnapshotFact.Fact");
                target.RelationshipIds.Add("FactId", row.Fact.Id);
            }

            instance.GetOrCreateEntityRecords("AccumulatingSnapshotFact").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DateRoleName is not null)
                target.Values.Add("DateRoleName", row.DateRoleName);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.AccumulatingSnapshotFact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AccumulatingSnapshotFact, row.AccumulatingSnapshotFact.Id, index0, "AccumulatingSnapshotMilestone.AccumulatingSnapshotFact");
                target.RelationshipIds.Add("AccumulatingSnapshotFactId", row.AccumulatingSnapshotFact.Id);
            }

            instance.GetOrCreateEntityRecords("AccumulatingSnapshotMilestone").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.AggregatedFact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AggregatedFact, row.AggregatedFact.Id, index14, "AggregateFact.AggregatedFact");
                target.RelationshipIds.Add("AggregatedFactId", row.AggregatedFact.Id);
            }

            if (row.SourceFact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SourceFact, row.SourceFact.Id, index14, "AggregateFact.SourceFact");
                target.RelationshipIds.Add("SourceFactId", row.SourceFact.Id);
            }

            instance.GetOrCreateEntityRecords("AggregateFact").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.IsRequired is not null)
                target.Values.Add("IsRequired", row.IsRequired);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.RoleName is not null)
                target.Values.Add("RoleName", row.RoleName);
            if (row.BridgeTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BridgeTable, row.BridgeTable.Id, index4, "BridgeParticipant.BridgeTable");
                target.RelationshipIds.Add("BridgeTableId", row.BridgeTable.Id);
            }

            if (row.Dimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Dimension, row.Dimension.Id, index8, "BridgeParticipant.Dimension");
                target.RelationshipIds.Add("DimensionId", row.Dimension.Id);
            }

            instance.GetOrCreateEntityRecords("BridgeParticipant").Add(target);
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
            if (row.Warehouse is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Warehouse, row.Warehouse.Id, index29, "BridgeTable.Warehouse");
                target.RelationshipIds.Add("WarehouseId", row.Warehouse.Id);
            }

            instance.GetOrCreateEntityRecords("BridgeTable").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.BridgeTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BridgeTable, row.BridgeTable.Id, index4, "BridgeWeight.BridgeTable");
                target.RelationshipIds.Add("BridgeTableId", row.BridgeTable.Id);
            }

            instance.GetOrCreateEntityRecords("BridgeWeight").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.ConformanceName is not null)
                target.Values.Add("ConformanceName", row.ConformanceName);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Dimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Dimension, row.Dimension.Id, index8, "ConformedDimension.Dimension");
                target.RelationshipIds.Add("DimensionId", row.Dimension.Id);
            }

            instance.GetOrCreateEntityRecords("ConformedDimension").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Fact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Fact, row.Fact.Id, index14, "DegenerateDimension.Fact");
                target.RelationshipIds.Add("FactId", row.Fact.Id);
            }

            instance.GetOrCreateEntityRecords("DegenerateDimension").Add(target);
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
            if (row.Warehouse is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Warehouse, row.Warehouse.Id, index29, "Dimension.Warehouse");
                target.RelationshipIds.Add("WarehouseId", row.Warehouse.Id);
            }

            instance.GetOrCreateEntityRecords("Dimension").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index9))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.IsNullable is not null)
                target.Values.Add("IsNullable", row.IsNullable);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Dimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Dimension, row.Dimension.Id, index8, "DimensionAttribute.Dimension");
                target.RelationshipIds.Add("DimensionId", row.Dimension.Id);
            }

            instance.GetOrCreateEntityRecords("DimensionAttribute").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index10))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Dimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Dimension, row.Dimension.Id, index8, "DimensionBusinessKey.Dimension");
                target.RelationshipIds.Add("DimensionId", row.Dimension.Id);
            }

            instance.GetOrCreateEntityRecords("DimensionBusinessKey").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index11))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.DimensionAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DimensionAttribute, row.DimensionAttribute.Id, index9, "DimensionBusinessKeyPart.DimensionAttribute");
                target.RelationshipIds.Add("DimensionAttributeId", row.DimensionAttribute.Id);
            }

            if (row.DimensionBusinessKey is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DimensionBusinessKey, row.DimensionBusinessKey.Id, index10, "DimensionBusinessKeyPart.DimensionBusinessKey");
                target.RelationshipIds.Add("DimensionBusinessKeyId", row.DimensionBusinessKey.Id);
            }

            instance.GetOrCreateEntityRecords("DimensionBusinessKeyPart").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index12))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Dimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Dimension, row.Dimension.Id, index8, "DimensionHierarchy.Dimension");
                target.RelationshipIds.Add("DimensionId", row.Dimension.Id);
            }

            instance.GetOrCreateEntityRecords("DimensionHierarchy").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index13))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.DimensionAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DimensionAttribute, row.DimensionAttribute.Id, index9, "DimensionHierarchyLevel.DimensionAttribute");
                target.RelationshipIds.Add("DimensionAttributeId", row.DimensionAttribute.Id);
            }

            if (row.DimensionHierarchy is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DimensionHierarchy, row.DimensionHierarchy.Id, index12, "DimensionHierarchyLevel.DimensionHierarchy");
                target.RelationshipIds.Add("DimensionHierarchyId", row.DimensionHierarchy.Id);
            }

            instance.GetOrCreateEntityRecords("DimensionHierarchyLevel").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index14))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Warehouse is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Warehouse, row.Warehouse.Id, index29, "Fact.Warehouse");
                target.RelationshipIds.Add("WarehouseId", row.Warehouse.Id);
            }

            instance.GetOrCreateEntityRecords("Fact").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index15))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.RoleName is not null)
                target.Values.Add("RoleName", row.RoleName);
            if (row.BridgeTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BridgeTable, row.BridgeTable.Id, index4, "FactBridge.BridgeTable");
                target.RelationshipIds.Add("BridgeTableId", row.BridgeTable.Id);
            }

            if (row.Fact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Fact, row.Fact.Id, index14, "FactBridge.Fact");
                target.RelationshipIds.Add("FactId", row.Fact.Id);
            }

            instance.GetOrCreateEntityRecords("FactBridge").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index16))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.IsRequired is not null)
                target.Values.Add("IsRequired", row.IsRequired);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.RoleName is not null)
                target.Values.Add("RoleName", row.RoleName);
            if (row.Dimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Dimension, row.Dimension.Id, index8, "FactDimension.Dimension");
                target.RelationshipIds.Add("DimensionId", row.Dimension.Id);
            }

            if (row.Fact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Fact, row.Fact.Id, index14, "FactDimension.Fact");
                target.RelationshipIds.Add("FactId", row.Fact.Id);
            }

            instance.GetOrCreateEntityRecords("FactDimension").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index17))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Fact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Fact, row.Fact.Id, index14, "FactGrain.Fact");
                target.RelationshipIds.Add("FactId", row.Fact.Id);
            }

            instance.GetOrCreateEntityRecords("FactGrain").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index18))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Fact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Fact, row.Fact.Id, index14, "FactlessFact.Fact");
                target.RelationshipIds.Add("FactId", row.Fact.Id);
            }

            instance.GetOrCreateEntityRecords("FactlessFact").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index19))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.IsNullable is not null)
                target.Values.Add("IsNullable", row.IsNullable);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Fact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Fact, row.Fact.Id, index14, "FactMeasure.Fact");
                target.RelationshipIds.Add("FactId", row.Fact.Id);
            }

            instance.GetOrCreateEntityRecords("FactMeasure").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index20))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Dimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Dimension, row.Dimension.Id, index8, "JunkDimension.Dimension");
                target.RelationshipIds.Add("DimensionId", row.Dimension.Id);
            }

            instance.GetOrCreateEntityRecords("JunkDimension").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index21))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.DimensionAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DimensionAttribute, row.DimensionAttribute.Id, index9, "JunkDimensionComponent.DimensionAttribute");
                target.RelationshipIds.Add("DimensionAttributeId", row.DimensionAttribute.Id);
            }

            if (row.JunkDimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.JunkDimension, row.JunkDimension.Id, index20, "JunkDimensionComponent.JunkDimension");
                target.RelationshipIds.Add("JunkDimensionId", row.JunkDimension.Id);
            }

            instance.GetOrCreateEntityRecords("JunkDimensionComponent").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index22))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.RoleName is not null)
                target.Values.Add("RoleName", row.RoleName);
            if (row.ProfileDimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ProfileDimension, row.ProfileDimension.Id, index8, "MiniDimension.ProfileDimension");
                target.RelationshipIds.Add("ProfileDimensionId", row.ProfileDimension.Id);
            }

            if (row.SourceDimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SourceDimension, row.SourceDimension.Id, index8, "MiniDimension.SourceDimension");
                target.RelationshipIds.Add("SourceDimensionId", row.SourceDimension.Id);
            }

            instance.GetOrCreateEntityRecords("MiniDimension").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index23))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.IsRequired is not null)
                target.Values.Add("IsRequired", row.IsRequired);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.RoleName is not null)
                target.Values.Add("RoleName", row.RoleName);
            if (row.ChildDimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ChildDimension, row.ChildDimension.Id, index8, "OutriggerDimension.ChildDimension");
                target.RelationshipIds.Add("ChildDimensionId", row.ChildDimension.Id);
            }

            if (row.ParentDimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ParentDimension, row.ParentDimension.Id, index8, "OutriggerDimension.ParentDimension");
                target.RelationshipIds.Add("ParentDimensionId", row.ParentDimension.Id);
            }

            instance.GetOrCreateEntityRecords("OutriggerDimension").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index24))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.PeriodName is not null)
                target.Values.Add("PeriodName", row.PeriodName);
            if (row.Fact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Fact, row.Fact.Id, index14, "PeriodicSnapshotFact.Fact");
                target.RelationshipIds.Add("FactId", row.Fact.Id);
            }

            instance.GetOrCreateEntityRecords("PeriodicSnapshotFact").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index25))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Dimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Dimension, row.Dimension.Id, index8, "SlowlyChangingDimension.Dimension");
                target.RelationshipIds.Add("DimensionId", row.Dimension.Id);
            }

            instance.GetOrCreateEntityRecords("SlowlyChangingDimension").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index26))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Fact is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Fact, row.Fact.Id, index14, "TransactionFact.Fact");
                target.RelationshipIds.Add("FactId", row.Fact.Id);
            }

            instance.GetOrCreateEntityRecords("TransactionFact").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index27))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.DimensionAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DimensionAttribute, row.DimensionAttribute.Id, index9, "Type1DimensionAttribute.DimensionAttribute");
                target.RelationshipIds.Add("DimensionAttributeId", row.DimensionAttribute.Id);
            }

            if (row.SlowlyChangingDimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SlowlyChangingDimension, row.SlowlyChangingDimension.Id, index25, "Type1DimensionAttribute.SlowlyChangingDimension");
                target.RelationshipIds.Add("SlowlyChangingDimensionId", row.SlowlyChangingDimension.Id);
            }

            instance.GetOrCreateEntityRecords("Type1DimensionAttribute").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index28))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.DimensionAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DimensionAttribute, row.DimensionAttribute.Id, index9, "Type2DimensionAttribute.DimensionAttribute");
                target.RelationshipIds.Add("DimensionAttributeId", row.DimensionAttribute.Id);
            }

            if (row.SlowlyChangingDimension is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SlowlyChangingDimension, row.SlowlyChangingDimension.Id, index25, "Type2DimensionAttribute.SlowlyChangingDimension");
                target.RelationshipIds.Add("SlowlyChangingDimensionId", row.SlowlyChangingDimension.Id);
            }

            instance.GetOrCreateEntityRecords("Type2DimensionAttribute").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index29))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("Warehouse").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}