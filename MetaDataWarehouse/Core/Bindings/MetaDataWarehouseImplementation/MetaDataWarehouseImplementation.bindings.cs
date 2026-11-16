// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaDataWarehouseImplementation;
public sealed class MetaDataWarehouseImplementationWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaDataWarehouseImplementation.MetaDataWarehouseImplementationModel>
{
    private const string ContractSignature = "6d6f64656c7c4d6574614461746157617265686f757365496d706c656d656e746174696f6e0a656e746974797c416363756d756c6174696e67536e617073686f74466163745461626c65496d706c656d656e746174696f6e7c416363756d756c6174696e67536e617073686f74466163745461626c65496d706c656d656e746174696f6e4c6973740a70726f70657274797c416363756d756c6174696e67536e617073686f74466163745461626c65496d706c656d656e746174696f6e7c4d696c6573746f6e6544617465436f6c756d6e5061747465726e7c72657175697265640a70726f70657274797c416363756d756c6174696e67536e617073686f74466163745461626c65496d706c656d656e746174696f6e7c4d696c6573746f6e6544617465446174615479706549647c72657175697265640a656e746974797c416767726567617465466163745461626c65496d706c656d656e746174696f6e7c416767726567617465466163745461626c65496d706c656d656e746174696f6e4c6973740a70726f70657274797c416767726567617465466163745461626c65496d706c656d656e746174696f6e7c536368656d614e616d657c72657175697265640a70726f70657274797c416767726567617465466163745461626c65496d706c656d656e746174696f6e7c536f7572636546616374466f726569676e4b65794e616d655061747465726e7c6e756c6c61626c650a70726f70657274797c416767726567617465466163745461626c65496d706c656d656e746174696f6e7c5461626c654e616d655061747465726e7c72657175697265640a656e746974797c4272696467655461626c65496d706c656d656e746174696f6e7c4272696467655461626c65496d706c656d656e746174696f6e4c6973740a70726f70657274797c4272696467655461626c65496d706c656d656e746174696f6e7c5061727469636970616e74466f726569676e4b65794e616d655061747465726e7c72657175697265640a70726f70657274797c4272696467655461626c65496d706c656d656e746174696f6e7c5061727469636970616e744b6579436f6c756d6e5061747465726e7c72657175697265640a70726f70657274797c4272696467655461626c65496d706c656d656e746174696f6e7c5072696d6172794b65794e616d655061747465726e7c72657175697265640a70726f70657274797c4272696467655461626c65496d706c656d656e746174696f6e7c536368656d614e616d657c72657175697265640a70726f70657274797c4272696467655461626c65496d706c656d656e746174696f6e7c5461626c654e616d655061747465726e7c72657175697265640a70726f70657274797c4272696467655461626c65496d706c656d656e746174696f6e7c576569676874436f6c756d6e4e616d657c6e756c6c61626c650a70726f70657274797c4272696467655461626c65496d706c656d656e746174696f6e7c576569676874446174615479706549647c6e756c6c61626c650a656e746974797c44696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c44696d656e73696f6e5461626c65496d706c656d656e746174696f6e4c6973740a70726f70657274797c44696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c417474726962757465436f6c756d6e5061747465726e7c72657175697265640a70726f70657274797c44696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c427573696e6573734b6579436f6c756d6e5061747465726e7c72657175697265640a70726f70657274797c44696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c5072696d6172794b65794e616d655061747465726e7c72657175697265640a70726f70657274797c44696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c536368656d614e616d657c72657175697265640a70726f70657274797c44696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c537572726f676174654b6579436f6c756d6e4e616d657c72657175697265640a70726f70657274797c44696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c537572726f676174654b6579446174615479706549647c72657175697265640a70726f70657274797c44696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c5461626c654e616d655061747465726e7c72657175697265640a656e746974797c466163745461626c65496d706c656d656e746174696f6e7c466163745461626c65496d706c656d656e746174696f6e4c6973740a70726f70657274797c466163745461626c65496d706c656d656e746174696f6e7c446567656e657261746544696d656e73696f6e436f6c756d6e5061747465726e7c72657175697265640a70726f70657274797c466163745461626c65496d706c656d656e746174696f6e7c44696d656e73696f6e466f726569676e4b65794e616d655061747465726e7c72657175697265640a70726f70657274797c466163745461626c65496d706c656d656e746174696f6e7c44696d656e73696f6e4b6579436f6c756d6e5061747465726e7c72657175697265640a70726f70657274797c466163745461626c65496d706c656d656e746174696f6e7c4d656173757265436f6c756d6e5061747465726e7c72657175697265640a70726f70657274797c466163745461626c65496d706c656d656e746174696f6e7c5072696d6172794b65794e616d655061747465726e7c6e756c6c61626c650a70726f70657274797c466163745461626c65496d706c656d656e746174696f6e7c536368656d614e616d657c72657175697265640a70726f70657274797c466163745461626c65496d706c656d656e746174696f6e7c5461626c654e616d655061747465726e7c72657175697265640a656e746974797c496e646578496d706c656d656e746174696f6e7c496e646578496d706c656d656e746174696f6e4c6973740a70726f70657274797c496e646578496d706c656d656e746174696f6e7c4170706c696573546f4272696467655461626c65737c72657175697265640a70726f70657274797c496e646578496d706c656d656e746174696f6e7c4170706c696573546f44696d656e73696f6e5461626c65737c72657175697265640a70726f70657274797c496e646578496d706c656d656e746174696f6e7c4170706c696573546f466163745461626c65737c72657175697265640a70726f70657274797c496e646578496d706c656d656e746174696f6e7c436f6c756d6e4e616d655061747465726e7c72657175697265640a70726f70657274797c496e646578496d706c656d656e746174696f6e7c46696c74657253716c7c6e756c6c61626c650a70726f70657274797c496e646578496d706c656d656e746174696f6e7c4973436c757374657265647c6e756c6c61626c650a70726f70657274797c496e646578496d706c656d656e746174696f6e7c4973556e697175657c6e756c6c61626c650a70726f70657274797c496e646578496d706c656d656e746174696f6e7c4e616d655061747465726e7c72657175697265640a70726f70657274797c496e646578496d706c656d656e746174696f6e7c4f7264696e616c7c72657175697265640a656e746974797c506572696f646963536e617073686f74466163745461626c65496d706c656d656e746174696f6e7c506572696f646963536e617073686f74466163745461626c65496d706c656d656e746174696f6e4c6973740a70726f70657274797c506572696f646963536e617073686f74466163745461626c65496d706c656d656e746174696f6e7c506572696f64446174615479706549647c72657175697265640a70726f70657274797c506572696f646963536e617073686f74466163745461626c65496d706c656d656e746174696f6e7c506572696f64456e64436f6c756d6e4e616d657c6e756c6c61626c650a70726f70657274797c506572696f646963536e617073686f74466163745461626c65496d706c656d656e746174696f6e7c506572696f645374617274436f6c756d6e4e616d657c72657175697265640a656e746974797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e4c6973740a70726f70657274797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c4170706c696573546f4272696467655461626c65737c72657175697265640a70726f70657274797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c4170706c696573546f44696d656e73696f6e5461626c65737c72657175697265640a70726f70657274797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c4170706c696573546f466163745461626c65737c72657175697265640a70726f70657274797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c436f6c756d6e4e616d657c72657175697265640a70726f70657274797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c446174615479706549647c72657175697265640a70726f70657274797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c44656661756c7445787072657373696f6e53716c7c6e756c6c61626c650a70726f70657274797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c49734e756c6c61626c657c6e756c6c61626c650a70726f70657274797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c4c656e6774687c6e756c6c61626c650a70726f70657274797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c4f7264696e616c7c72657175697265640a70726f70657274797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c507265636973696f6e7c6e756c6c61626c650a70726f70657274797c506c6174666f726d436f6c756d6e496d706c656d656e746174696f6e7c5363616c657c6e756c6c61626c650a656e746974797c536c6f776c794368616e67696e6744696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c536c6f776c794368616e67696e6744696d656e73696f6e5461626c65496d706c656d656e746174696f6e4c6973740a70726f70657274797c536c6f776c794368616e67696e6744696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c43757272656e74466c6167436f6c756d6e4e616d657c72657175697265640a70726f70657274797c536c6f776c794368616e67696e6744696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c43757272656e74466c6167446174615479706549647c72657175697265640a70726f70657274797c536c6f776c794368616e67696e6744696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c45666665637469766546726f6d436f6c756d6e4e616d657c72657175697265640a70726f70657274797c536c6f776c794368616e67696e6744696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c45666665637469766546726f6d446174615479706549647c72657175697265640a70726f70657274797c536c6f776c794368616e67696e6744696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c456666656374697665546f436f6c756d6e4e616d657c72657175697265640a70726f70657274797c536c6f776c794368616e67696e6744696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c456666656374697665546f446174615479706549647c72657175697265640a70726f70657274797c536c6f776c794368616e67696e6744696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c4861736844696666436f6c756d6e4e616d657c6e756c6c61626c650a70726f70657274797c536c6f776c794368616e67696e6744696d656e73696f6e5461626c65496d706c656d656e746174696f6e7c4861736844696666446174615479706549647c6e756c6c61626c65";
    public global::MetaDataWarehouseImplementation.MetaDataWarehouseImplementationModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaDataWarehouseImplementation.MetaDataWarehouseImplementationModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouseImplementation.AccumulatingSnapshotFactTableImplementation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AccumulatingSnapshotFactTableImplementation"))
        {
            var row = new global::MetaDataWarehouseImplementation.AccumulatingSnapshotFactTableImplementation
            {
                Id = source.Id
            };
            row.MilestoneDateColumnPattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MilestoneDateColumnPattern");
            row.MilestoneDateDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MilestoneDateDataTypeId");
            typed.AccumulatingSnapshotFactTableImplementationList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouseImplementation.AggregateFactTableImplementation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AggregateFactTableImplementation"))
        {
            var row = new global::MetaDataWarehouseImplementation.AggregateFactTableImplementation
            {
                Id = source.Id
            };
            row.SchemaName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SchemaName");
            row.SourceFactForeignKeyNamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "SourceFactForeignKeyNamePattern");
            row.TableNamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TableNamePattern");
            typed.AggregateFactTableImplementationList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouseImplementation.BridgeTableImplementation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "BridgeTableImplementation"))
        {
            var row = new global::MetaDataWarehouseImplementation.BridgeTableImplementation
            {
                Id = source.Id
            };
            row.ParticipantForeignKeyNamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ParticipantForeignKeyNamePattern");
            row.ParticipantKeyColumnPattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ParticipantKeyColumnPattern");
            row.PrimaryKeyNamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "PrimaryKeyNamePattern");
            row.SchemaName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SchemaName");
            row.TableNamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TableNamePattern");
            row.WeightColumnName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "WeightColumnName");
            row.WeightDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "WeightDataTypeId");
            typed.BridgeTableImplementationList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouseImplementation.DimensionTableImplementation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DimensionTableImplementation"))
        {
            var row = new global::MetaDataWarehouseImplementation.DimensionTableImplementation
            {
                Id = source.Id
            };
            row.AttributeColumnPattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AttributeColumnPattern");
            row.BusinessKeyColumnPattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "BusinessKeyColumnPattern");
            row.PrimaryKeyNamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "PrimaryKeyNamePattern");
            row.SchemaName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SchemaName");
            row.SurrogateKeyColumnName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SurrogateKeyColumnName");
            row.SurrogateKeyDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SurrogateKeyDataTypeId");
            row.TableNamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TableNamePattern");
            typed.DimensionTableImplementationList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouseImplementation.FactTableImplementation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "FactTableImplementation"))
        {
            var row = new global::MetaDataWarehouseImplementation.FactTableImplementation
            {
                Id = source.Id
            };
            row.DegenerateDimensionColumnPattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DegenerateDimensionColumnPattern");
            row.DimensionForeignKeyNamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DimensionForeignKeyNamePattern");
            row.DimensionKeyColumnPattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DimensionKeyColumnPattern");
            row.MeasureColumnPattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MeasureColumnPattern");
            row.PrimaryKeyNamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "PrimaryKeyNamePattern");
            row.SchemaName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SchemaName");
            row.TableNamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TableNamePattern");
            typed.FactTableImplementationList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouseImplementation.IndexImplementation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "IndexImplementation"))
        {
            var row = new global::MetaDataWarehouseImplementation.IndexImplementation
            {
                Id = source.Id
            };
            row.AppliesToBridgeTables = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AppliesToBridgeTables");
            row.AppliesToDimensionTables = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AppliesToDimensionTables");
            row.AppliesToFactTables = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AppliesToFactTables");
            row.ColumnNamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ColumnNamePattern");
            row.FilterSql = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "FilterSql");
            row.IsClustered = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsClustered");
            row.IsUnique = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsUnique");
            row.NamePattern = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "NamePattern");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.IndexImplementationList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouseImplementation.PeriodicSnapshotFactTableImplementation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PeriodicSnapshotFactTableImplementation"))
        {
            var row = new global::MetaDataWarehouseImplementation.PeriodicSnapshotFactTableImplementation
            {
                Id = source.Id
            };
            row.PeriodDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "PeriodDataTypeId");
            row.PeriodEndColumnName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "PeriodEndColumnName");
            row.PeriodStartColumnName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "PeriodStartColumnName");
            typed.PeriodicSnapshotFactTableImplementationList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouseImplementation.PlatformColumnImplementation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PlatformColumnImplementation"))
        {
            var row = new global::MetaDataWarehouseImplementation.PlatformColumnImplementation
            {
                Id = source.Id
            };
            row.AppliesToBridgeTables = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AppliesToBridgeTables");
            row.AppliesToDimensionTables = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AppliesToDimensionTables");
            row.AppliesToFactTables = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AppliesToFactTables");
            row.ColumnName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ColumnName");
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.DefaultExpressionSql = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DefaultExpressionSql");
            row.IsNullable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsNullable");
            row.Length = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Length");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            row.Precision = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Precision");
            row.Scale = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Scale");
            typed.PlatformColumnImplementationList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaDataWarehouseImplementation.SlowlyChangingDimensionTableImplementation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SlowlyChangingDimensionTableImplementation"))
        {
            var row = new global::MetaDataWarehouseImplementation.SlowlyChangingDimensionTableImplementation
            {
                Id = source.Id
            };
            row.CurrentFlagColumnName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "CurrentFlagColumnName");
            row.CurrentFlagDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "CurrentFlagDataTypeId");
            row.EffectiveFromColumnName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "EffectiveFromColumnName");
            row.EffectiveFromDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "EffectiveFromDataTypeId");
            row.EffectiveToColumnName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "EffectiveToColumnName");
            row.EffectiveToDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "EffectiveToDataTypeId");
            row.HashDiffColumnName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "HashDiffColumnName");
            row.HashDiffDataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "HashDiffDataTypeId");
            typed.SlowlyChangingDimensionTableImplementationList.Add(row);
            index8.Add(source.Id, row);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaDataWarehouseImplementation.MetaDataWarehouseImplementationModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AccumulatingSnapshotFactTableImplementationList, row => row.Id, "AccumulatingSnapshotFactTableImplementation");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AggregateFactTableImplementationList, row => row.Id, "AggregateFactTableImplementation");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.BridgeTableImplementationList, row => row.Id, "BridgeTableImplementation");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DimensionTableImplementationList, row => row.Id, "DimensionTableImplementation");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.FactTableImplementationList, row => row.Id, "FactTableImplementation");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.IndexImplementationList, row => row.Id, "IndexImplementation");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PeriodicSnapshotFactTableImplementationList, row => row.Id, "PeriodicSnapshotFactTableImplementation");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PlatformColumnImplementationList, row => row.Id, "PlatformColumnImplementation");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SlowlyChangingDimensionTableImplementationList, row => row.Id, "SlowlyChangingDimensionTableImplementation");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MilestoneDateColumnPattern is not null)
                target.Values.Add("MilestoneDateColumnPattern", row.MilestoneDateColumnPattern);
            if (row.MilestoneDateDataTypeId is not null)
                target.Values.Add("MilestoneDateDataTypeId", row.MilestoneDateDataTypeId);
            instance.GetOrCreateEntityRecords("AccumulatingSnapshotFactTableImplementation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SchemaName is not null)
                target.Values.Add("SchemaName", row.SchemaName);
            if (row.SourceFactForeignKeyNamePattern is not null)
                target.Values.Add("SourceFactForeignKeyNamePattern", row.SourceFactForeignKeyNamePattern);
            if (row.TableNamePattern is not null)
                target.Values.Add("TableNamePattern", row.TableNamePattern);
            instance.GetOrCreateEntityRecords("AggregateFactTableImplementation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.ParticipantForeignKeyNamePattern is not null)
                target.Values.Add("ParticipantForeignKeyNamePattern", row.ParticipantForeignKeyNamePattern);
            if (row.ParticipantKeyColumnPattern is not null)
                target.Values.Add("ParticipantKeyColumnPattern", row.ParticipantKeyColumnPattern);
            if (row.PrimaryKeyNamePattern is not null)
                target.Values.Add("PrimaryKeyNamePattern", row.PrimaryKeyNamePattern);
            if (row.SchemaName is not null)
                target.Values.Add("SchemaName", row.SchemaName);
            if (row.TableNamePattern is not null)
                target.Values.Add("TableNamePattern", row.TableNamePattern);
            if (row.WeightColumnName is not null)
                target.Values.Add("WeightColumnName", row.WeightColumnName);
            if (row.WeightDataTypeId is not null)
                target.Values.Add("WeightDataTypeId", row.WeightDataTypeId);
            instance.GetOrCreateEntityRecords("BridgeTableImplementation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AttributeColumnPattern is not null)
                target.Values.Add("AttributeColumnPattern", row.AttributeColumnPattern);
            if (row.BusinessKeyColumnPattern is not null)
                target.Values.Add("BusinessKeyColumnPattern", row.BusinessKeyColumnPattern);
            if (row.PrimaryKeyNamePattern is not null)
                target.Values.Add("PrimaryKeyNamePattern", row.PrimaryKeyNamePattern);
            if (row.SchemaName is not null)
                target.Values.Add("SchemaName", row.SchemaName);
            if (row.SurrogateKeyColumnName is not null)
                target.Values.Add("SurrogateKeyColumnName", row.SurrogateKeyColumnName);
            if (row.SurrogateKeyDataTypeId is not null)
                target.Values.Add("SurrogateKeyDataTypeId", row.SurrogateKeyDataTypeId);
            if (row.TableNamePattern is not null)
                target.Values.Add("TableNamePattern", row.TableNamePattern);
            instance.GetOrCreateEntityRecords("DimensionTableImplementation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DegenerateDimensionColumnPattern is not null)
                target.Values.Add("DegenerateDimensionColumnPattern", row.DegenerateDimensionColumnPattern);
            if (row.DimensionForeignKeyNamePattern is not null)
                target.Values.Add("DimensionForeignKeyNamePattern", row.DimensionForeignKeyNamePattern);
            if (row.DimensionKeyColumnPattern is not null)
                target.Values.Add("DimensionKeyColumnPattern", row.DimensionKeyColumnPattern);
            if (row.MeasureColumnPattern is not null)
                target.Values.Add("MeasureColumnPattern", row.MeasureColumnPattern);
            if (row.PrimaryKeyNamePattern is not null)
                target.Values.Add("PrimaryKeyNamePattern", row.PrimaryKeyNamePattern);
            if (row.SchemaName is not null)
                target.Values.Add("SchemaName", row.SchemaName);
            if (row.TableNamePattern is not null)
                target.Values.Add("TableNamePattern", row.TableNamePattern);
            instance.GetOrCreateEntityRecords("FactTableImplementation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AppliesToBridgeTables is not null)
                target.Values.Add("AppliesToBridgeTables", row.AppliesToBridgeTables);
            if (row.AppliesToDimensionTables is not null)
                target.Values.Add("AppliesToDimensionTables", row.AppliesToDimensionTables);
            if (row.AppliesToFactTables is not null)
                target.Values.Add("AppliesToFactTables", row.AppliesToFactTables);
            if (row.ColumnNamePattern is not null)
                target.Values.Add("ColumnNamePattern", row.ColumnNamePattern);
            if (row.FilterSql is not null)
                target.Values.Add("FilterSql", row.FilterSql);
            if (row.IsClustered is not null)
                target.Values.Add("IsClustered", row.IsClustered);
            if (row.IsUnique is not null)
                target.Values.Add("IsUnique", row.IsUnique);
            if (row.NamePattern is not null)
                target.Values.Add("NamePattern", row.NamePattern);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            instance.GetOrCreateEntityRecords("IndexImplementation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.PeriodDataTypeId is not null)
                target.Values.Add("PeriodDataTypeId", row.PeriodDataTypeId);
            if (row.PeriodEndColumnName is not null)
                target.Values.Add("PeriodEndColumnName", row.PeriodEndColumnName);
            if (row.PeriodStartColumnName is not null)
                target.Values.Add("PeriodStartColumnName", row.PeriodStartColumnName);
            instance.GetOrCreateEntityRecords("PeriodicSnapshotFactTableImplementation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AppliesToBridgeTables is not null)
                target.Values.Add("AppliesToBridgeTables", row.AppliesToBridgeTables);
            if (row.AppliesToDimensionTables is not null)
                target.Values.Add("AppliesToDimensionTables", row.AppliesToDimensionTables);
            if (row.AppliesToFactTables is not null)
                target.Values.Add("AppliesToFactTables", row.AppliesToFactTables);
            if (row.ColumnName is not null)
                target.Values.Add("ColumnName", row.ColumnName);
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.DefaultExpressionSql is not null)
                target.Values.Add("DefaultExpressionSql", row.DefaultExpressionSql);
            if (row.IsNullable is not null)
                target.Values.Add("IsNullable", row.IsNullable);
            if (row.Length is not null)
                target.Values.Add("Length", row.Length);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Precision is not null)
                target.Values.Add("Precision", row.Precision);
            if (row.Scale is not null)
                target.Values.Add("Scale", row.Scale);
            instance.GetOrCreateEntityRecords("PlatformColumnImplementation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index8))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.CurrentFlagColumnName is not null)
                target.Values.Add("CurrentFlagColumnName", row.CurrentFlagColumnName);
            if (row.CurrentFlagDataTypeId is not null)
                target.Values.Add("CurrentFlagDataTypeId", row.CurrentFlagDataTypeId);
            if (row.EffectiveFromColumnName is not null)
                target.Values.Add("EffectiveFromColumnName", row.EffectiveFromColumnName);
            if (row.EffectiveFromDataTypeId is not null)
                target.Values.Add("EffectiveFromDataTypeId", row.EffectiveFromDataTypeId);
            if (row.EffectiveToColumnName is not null)
                target.Values.Add("EffectiveToColumnName", row.EffectiveToColumnName);
            if (row.EffectiveToDataTypeId is not null)
                target.Values.Add("EffectiveToDataTypeId", row.EffectiveToDataTypeId);
            if (row.HashDiffColumnName is not null)
                target.Values.Add("HashDiffColumnName", row.HashDiffColumnName);
            if (row.HashDiffDataTypeId is not null)
                target.Values.Add("HashDiffDataTypeId", row.HashDiffDataTypeId);
            instance.GetOrCreateEntityRecords("SlowlyChangingDimensionTableImplementation").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}