#nullable enable
using System;
using System.Collections.Generic;

namespace MetaDataVaultImplementation;
public sealed partial class BusinessBridgeImplementation
{
    public string Id { get; set; } = null !;
    public string AnchorHubForeignKeyNamePattern { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string? DepthColumnName { get; set; }
    public string? DepthDataTypeId { get; set; }
    public string? EffectiveFromColumnName { get; set; }
    public string? EffectiveFromDataTypeId { get; set; }
    public string? EffectiveFromPrecision { get; set; }
    public string? EffectiveToColumnName { get; set; }
    public string? EffectiveToDataTypeId { get; set; }
    public string? EffectiveToPrecision { get; set; }
    public string? PathColumnName { get; set; }
    public string? PathDataTypeId { get; set; }
    public string? PathLength { get; set; }
    public string RelatedHashKeyColumnName { get; set; } = null !;
    public string RelatedHashKeyDataTypeId { get; set; } = null !;
    public string RelatedHashKeyLength { get; set; } = null !;
    public string RelatedHubForeignKeyNamePattern { get; set; } = null !;
    public string RootHashKeyColumnName { get; set; } = null !;
    public string RootHashKeyDataTypeId { get; set; } = null !;
    public string RootHashKeyLength { get; set; } = null !;
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BusinessHierarchicalLinkImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string ChildHashKeyColumnName { get; set; } = null !;
    public string ChildHubForeignKeyNamePattern { get; set; } = null !;
    public string HashKeyColumnName { get; set; } = null !;
    public string HashKeyDataTypeId { get; set; } = null !;
    public string HashKeyLength { get; set; } = null !;
    public string? LoadTimestampColumnName { get; set; }
    public string? LoadTimestampDataTypeId { get; set; }
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string? LoadTimestampPrecision { get; set; }
    public string ParentHashKeyColumnName { get; set; } = null !;
    public string ParentHubForeignKeyNamePattern { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string? RecordSourceColumnName { get; set; }
    public string? RecordSourceDataTypeId { get; set; }
    public string? RecordSourceLength { get; set; }
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BusinessHierarchicalLinkSatelliteImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string? HashDiffColumnName { get; set; }
    public string? HashDiffDataTypeId { get; set; }
    public string? HashDiffLength { get; set; }
    public string LoadTimestampColumnName { get; set; } = null !;
    public string LoadTimestampDataTypeId { get; set; } = null !;
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string? LoadTimestampPrecision { get; set; }
    public string ParentForeignKeyNamePattern { get; set; } = null !;
    public string ParentHashKeyColumnName { get; set; } = null !;
    public string ParentHashKeyDataTypeId { get; set; } = null !;
    public string ParentHashKeyLength { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string? RecordSourceColumnName { get; set; }
    public string? RecordSourceDataTypeId { get; set; }
    public string? RecordSourceLength { get; set; }
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BusinessHubImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string HashKeyColumnName { get; set; } = null !;
    public string HashKeyDataTypeId { get; set; } = null !;
    public string HashKeyLength { get; set; } = null !;
    public string? LoadTimestampColumnName { get; set; }
    public string? LoadTimestampDataTypeId { get; set; }
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string? LoadTimestampPrecision { get; set; }
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string? RecordSourceColumnName { get; set; }
    public string? RecordSourceDataTypeId { get; set; }
    public string? RecordSourceLength { get; set; }
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BusinessHubSatelliteImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string? HashDiffColumnName { get; set; }
    public string? HashDiffDataTypeId { get; set; }
    public string? HashDiffLength { get; set; }
    public string LoadTimestampColumnName { get; set; } = null !;
    public string LoadTimestampDataTypeId { get; set; } = null !;
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string? LoadTimestampPrecision { get; set; }
    public string ParentForeignKeyNamePattern { get; set; } = null !;
    public string ParentHashKeyColumnName { get; set; } = null !;
    public string ParentHashKeyDataTypeId { get; set; } = null !;
    public string ParentHashKeyLength { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string? RecordSourceColumnName { get; set; }
    public string? RecordSourceDataTypeId { get; set; }
    public string? RecordSourceLength { get; set; }
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BusinessLinkImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string EndHashKeyColumnPattern { get; set; } = null !;
    public string HashKeyColumnName { get; set; } = null !;
    public string HashKeyDataTypeId { get; set; } = null !;
    public string HashKeyLength { get; set; } = null !;
    public string HubForeignKeyNamePattern { get; set; } = null !;
    public string? LoadTimestampColumnName { get; set; }
    public string? LoadTimestampDataTypeId { get; set; }
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string? LoadTimestampPrecision { get; set; }
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string? RecordSourceColumnName { get; set; }
    public string? RecordSourceDataTypeId { get; set; }
    public string? RecordSourceLength { get; set; }
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BusinessLinkSatelliteImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string? HashDiffColumnName { get; set; }
    public string? HashDiffDataTypeId { get; set; }
    public string? HashDiffLength { get; set; }
    public string LoadTimestampColumnName { get; set; } = null !;
    public string LoadTimestampDataTypeId { get; set; } = null !;
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string? LoadTimestampPrecision { get; set; }
    public string ParentForeignKeyNamePattern { get; set; } = null !;
    public string ParentHashKeyColumnName { get; set; } = null !;
    public string ParentHashKeyDataTypeId { get; set; } = null !;
    public string ParentHashKeyLength { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string? RecordSourceColumnName { get; set; }
    public string? RecordSourceDataTypeId { get; set; }
    public string? RecordSourceLength { get; set; }
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BusinessPointInTimeImplementation
{
    public string Id { get; set; } = null !;
    public string AnchorHubForeignKeyNamePattern { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string ParentHashKeyColumnName { get; set; } = null !;
    public string ParentHashKeyDataTypeId { get; set; } = null !;
    public string ParentHashKeyLength { get; set; } = null !;
    public string SatelliteReferenceColumnNamePattern { get; set; } = null !;
    public string SatelliteReferenceDataTypeId { get; set; } = null !;
    public string SatelliteReferencePrecision { get; set; } = null !;
    public string SchemaName { get; set; } = null !;
    public string SnapshotTimestampColumnName { get; set; } = null !;
    public string SnapshotTimestampDataTypeId { get; set; } = null !;
    public string SnapshotTimestampPrecision { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BusinessReferenceImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string HashKeyColumnName { get; set; } = null !;
    public string HashKeyDataTypeId { get; set; } = null !;
    public string HashKeyLength { get; set; } = null !;
    public string? LoadTimestampColumnName { get; set; }
    public string? LoadTimestampDataTypeId { get; set; }
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string? LoadTimestampPrecision { get; set; }
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string? RecordSourceColumnName { get; set; }
    public string? RecordSourceDataTypeId { get; set; }
    public string? RecordSourceLength { get; set; }
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BusinessReferenceSatelliteImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string? HashDiffColumnName { get; set; }
    public string? HashDiffDataTypeId { get; set; }
    public string? HashDiffLength { get; set; }
    public string LoadTimestampColumnName { get; set; } = null !;
    public string LoadTimestampDataTypeId { get; set; } = null !;
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string? LoadTimestampPrecision { get; set; }
    public string ParentForeignKeyNamePattern { get; set; } = null !;
    public string ParentHashKeyColumnName { get; set; } = null !;
    public string ParentHashKeyDataTypeId { get; set; } = null !;
    public string ParentHashKeyLength { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string? RecordSourceColumnName { get; set; }
    public string? RecordSourceDataTypeId { get; set; }
    public string? RecordSourceLength { get; set; }
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BusinessSameAsLinkImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string EquivalentHashKeyColumnName { get; set; } = null !;
    public string EquivalentHubForeignKeyNamePattern { get; set; } = null !;
    public string HashKeyColumnName { get; set; } = null !;
    public string HashKeyDataTypeId { get; set; } = null !;
    public string HashKeyLength { get; set; } = null !;
    public string? LoadTimestampColumnName { get; set; }
    public string? LoadTimestampDataTypeId { get; set; }
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string? LoadTimestampPrecision { get; set; }
    public string PrimaryHashKeyColumnName { get; set; } = null !;
    public string PrimaryHubForeignKeyNamePattern { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string? RecordSourceColumnName { get; set; }
    public string? RecordSourceDataTypeId { get; set; }
    public string? RecordSourceLength { get; set; }
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BusinessSameAsLinkSatelliteImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string? HashDiffColumnName { get; set; }
    public string? HashDiffDataTypeId { get; set; }
    public string? HashDiffLength { get; set; }
    public string LoadTimestampColumnName { get; set; } = null !;
    public string LoadTimestampDataTypeId { get; set; } = null !;
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string? LoadTimestampPrecision { get; set; }
    public string ParentForeignKeyNamePattern { get; set; } = null !;
    public string ParentHashKeyColumnName { get; set; } = null !;
    public string ParentHashKeyDataTypeId { get; set; } = null !;
    public string ParentHashKeyLength { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string? RecordSourceColumnName { get; set; }
    public string? RecordSourceDataTypeId { get; set; }
    public string? RecordSourceLength { get; set; }
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class RawHubImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string HashKeyColumnName { get; set; } = null !;
    public string HashKeyDataTypeId { get; set; } = null !;
    public string HashKeyLength { get; set; } = null !;
    public string LoadTimestampColumnName { get; set; } = null !;
    public string LoadTimestampDataTypeId { get; set; } = null !;
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string LoadTimestampPrecision { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string RecordSourceColumnName { get; set; } = null !;
    public string RecordSourceDataTypeId { get; set; } = null !;
    public string RecordSourceLength { get; set; } = null !;
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class RawHubSatelliteImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string HashDiffColumnName { get; set; } = null !;
    public string HashDiffDataTypeId { get; set; } = null !;
    public string HashDiffLength { get; set; } = null !;
    public string LoadTimestampColumnName { get; set; } = null !;
    public string LoadTimestampDataTypeId { get; set; } = null !;
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string LoadTimestampPrecision { get; set; } = null !;
    public string ParentForeignKeyNamePattern { get; set; } = null !;
    public string ParentHashKeyColumnName { get; set; } = null !;
    public string ParentHashKeyDataTypeId { get; set; } = null !;
    public string ParentHashKeyLength { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string RecordSourceColumnName { get; set; } = null !;
    public string RecordSourceDataTypeId { get; set; } = null !;
    public string RecordSourceLength { get; set; } = null !;
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class RawLinkImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string EndHashKeyColumnPattern { get; set; } = null !;
    public string HashKeyColumnName { get; set; } = null !;
    public string HashKeyDataTypeId { get; set; } = null !;
    public string HashKeyLength { get; set; } = null !;
    public string HubForeignKeyNamePattern { get; set; } = null !;
    public string LoadTimestampColumnName { get; set; } = null !;
    public string LoadTimestampDataTypeId { get; set; } = null !;
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string LoadTimestampPrecision { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string RecordSourceColumnName { get; set; } = null !;
    public string RecordSourceDataTypeId { get; set; } = null !;
    public string RecordSourceLength { get; set; } = null !;
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class RawLinkSatelliteImplementation
{
    public string Id { get; set; } = null !;
    public string AuditIdColumnName { get; set; } = null !;
    public string AuditIdDataTypeId { get; set; } = null !;
    public string? AuditIdDefaultExpressionSql { get; set; }
    public string HashDiffColumnName { get; set; } = null !;
    public string HashDiffDataTypeId { get; set; } = null !;
    public string HashDiffLength { get; set; } = null !;
    public string LoadTimestampColumnName { get; set; } = null !;
    public string LoadTimestampDataTypeId { get; set; } = null !;
    public string? LoadTimestampDefaultExpressionSql { get; set; }
    public string LoadTimestampPrecision { get; set; } = null !;
    public string ParentForeignKeyNamePattern { get; set; } = null !;
    public string ParentHashKeyColumnName { get; set; } = null !;
    public string ParentHashKeyDataTypeId { get; set; } = null !;
    public string ParentHashKeyLength { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string RecordSourceColumnName { get; set; } = null !;
    public string RecordSourceDataTypeId { get; set; } = null !;
    public string RecordSourceLength { get; set; } = null !;
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class MetaDataVaultImplementationModel
{
    public static MetaDataVaultImplementationModel CreateEmpty() => new();
    public List<BusinessBridgeImplementation> BusinessBridgeImplementationList { get; set; } = new();
    public List<BusinessHierarchicalLinkImplementation> BusinessHierarchicalLinkImplementationList { get; set; } = new();
    public List<BusinessHierarchicalLinkSatelliteImplementation> BusinessHierarchicalLinkSatelliteImplementationList { get; set; } = new();
    public List<BusinessHubImplementation> BusinessHubImplementationList { get; set; } = new();
    public List<BusinessHubSatelliteImplementation> BusinessHubSatelliteImplementationList { get; set; } = new();
    public List<BusinessLinkImplementation> BusinessLinkImplementationList { get; set; } = new();
    public List<BusinessLinkSatelliteImplementation> BusinessLinkSatelliteImplementationList { get; set; } = new();
    public List<BusinessPointInTimeImplementation> BusinessPointInTimeImplementationList { get; set; } = new();
    public List<BusinessReferenceImplementation> BusinessReferenceImplementationList { get; set; } = new();
    public List<BusinessReferenceSatelliteImplementation> BusinessReferenceSatelliteImplementationList { get; set; } = new();
    public List<BusinessSameAsLinkImplementation> BusinessSameAsLinkImplementationList { get; set; } = new();
    public List<BusinessSameAsLinkSatelliteImplementation> BusinessSameAsLinkSatelliteImplementationList { get; set; } = new();
    public List<RawHubImplementation> RawHubImplementationList { get; set; } = new();
    public List<RawHubSatelliteImplementation> RawHubSatelliteImplementationList { get; set; } = new();
    public List<RawLinkImplementation> RawLinkImplementationList { get; set; } = new();
    public List<RawLinkSatelliteImplementation> RawLinkSatelliteImplementationList { get; set; } = new();
}

public static partial class MetaDataVaultImplementationInstance
{
    private static readonly MetaDataVaultImplementationModel _builtIn = CreateBuiltIn();
    public static MetaDataVaultImplementationModel BuiltIn => _builtIn;

    public static MetaDataVaultImplementationModel CreateBuiltIn()
    {
        var model = MetaDataVaultImplementationModel.CreateEmpty();
        var record0 = new BusinessBridgeImplementation
        {
            Id = "default-business-bridge",
            AnchorHubForeignKeyNamePattern = "FK_{TableName}_{ParentTableName}",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            DepthColumnName = "Depth",
            DepthDataTypeId = "meta:type:Int32",
            EffectiveFromColumnName = "EffectiveFrom",
            EffectiveFromDataTypeId = "meta:type:DateTime2",
            EffectiveFromPrecision = "7",
            EffectiveToColumnName = "EffectiveTo",
            EffectiveToDataTypeId = "meta:type:DateTime2",
            EffectiveToPrecision = "7",
            PathColumnName = "Path",
            PathDataTypeId = "meta:type:String",
            PathLength = "4000",
            RelatedHashKeyColumnName = "RelatedHashKey",
            RelatedHashKeyDataTypeId = "meta:type:Binary",
            RelatedHashKeyLength = "32",
            RelatedHubForeignKeyNamePattern = "FK_{TableName}_{ParentTableName}_Related",
            RootHashKeyColumnName = "RootHashKey",
            RootHashKeyDataTypeId = "meta:type:Binary",
            RootHashKeyLength = "32",
            SchemaName = "dbo",
            TableNamePattern = "BR_{Name}"
        };
        model.BusinessBridgeImplementationList.Add(record0);
        var record1 = new BusinessHierarchicalLinkImplementation
        {
            Id = "default-business-hierarchical-link",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            ChildHashKeyColumnName = "ChildHashKey",
            ChildHubForeignKeyNamePattern = "FK_{TableName}_{TargetTableName}_{SourceColumnName}",
            HashKeyColumnName = "HashKey",
            HashKeyDataTypeId = "meta:type:Binary",
            HashKeyLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            ParentHashKeyColumnName = "ParentHashKey",
            ParentHubForeignKeyNamePattern = "FK_{TableName}_{TargetTableName}_{SourceColumnName}",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "BHAL_{Name}"
        };
        model.BusinessHierarchicalLinkImplementationList.Add(record1);
        var record2 = new BusinessHierarchicalLinkSatelliteImplementation
        {
            Id = "default-business-hierarchical-link-satellite",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            HashDiffColumnName = "HashDiff",
            HashDiffDataTypeId = "meta:type:Binary",
            HashDiffLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            ParentForeignKeyNamePattern = "FK_{TableName}_{ParentTableName}",
            ParentHashKeyColumnName = "LinkHashKey",
            ParentHashKeyDataTypeId = "meta:type:Binary",
            ParentHashKeyLength = "32",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "BHALS_{ParentName}_{Name}"
        };
        model.BusinessHierarchicalLinkSatelliteImplementationList.Add(record2);
        var record3 = new BusinessHubImplementation
        {
            Id = "default-business-hub",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            HashKeyColumnName = "HashKey",
            HashKeyDataTypeId = "meta:type:Binary",
            HashKeyLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "BH_{Name}"
        };
        model.BusinessHubImplementationList.Add(record3);
        var record4 = new BusinessHubSatelliteImplementation
        {
            Id = "default-business-hub-satellite",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            HashDiffColumnName = "HashDiff",
            HashDiffDataTypeId = "meta:type:Binary",
            HashDiffLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            ParentForeignKeyNamePattern = "FK_{TableName}_{ParentTableName}",
            ParentHashKeyColumnName = "HubHashKey",
            ParentHashKeyDataTypeId = "meta:type:Binary",
            ParentHashKeyLength = "32",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "BHS_{ParentName}_{Name}"
        };
        model.BusinessHubSatelliteImplementationList.Add(record4);
        var record5 = new BusinessLinkImplementation
        {
            Id = "default-business-link",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            EndHashKeyColumnPattern = "{RoleName}HashKey",
            HashKeyColumnName = "HashKey",
            HashKeyDataTypeId = "meta:type:Binary",
            HashKeyLength = "32",
            HubForeignKeyNamePattern = "FK_{TableName}_{TargetTableName}_{SourceColumnName}",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "BL_{Name}"
        };
        model.BusinessLinkImplementationList.Add(record5);
        var record6 = new BusinessLinkSatelliteImplementation
        {
            Id = "default-business-link-satellite",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            HashDiffColumnName = "HashDiff",
            HashDiffDataTypeId = "meta:type:Binary",
            HashDiffLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            ParentForeignKeyNamePattern = "FK_{TableName}_{ParentTableName}",
            ParentHashKeyColumnName = "LinkHashKey",
            ParentHashKeyDataTypeId = "meta:type:Binary",
            ParentHashKeyLength = "32",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "BLS_{ParentName}_{Name}"
        };
        model.BusinessLinkSatelliteImplementationList.Add(record6);
        var record7 = new BusinessPointInTimeImplementation
        {
            Id = "default-business-pit",
            AnchorHubForeignKeyNamePattern = "FK_{TableName}_{ParentTableName}",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            ParentHashKeyColumnName = "HubHashKey",
            ParentHashKeyDataTypeId = "meta:type:Binary",
            ParentHashKeyLength = "32",
            SatelliteReferenceColumnNamePattern = "{SatelliteName}LoadTimestamp",
            SatelliteReferenceDataTypeId = "meta:type:DateTime2",
            SatelliteReferencePrecision = "7",
            SchemaName = "dbo",
            SnapshotTimestampColumnName = "SnapshotTimestamp",
            SnapshotTimestampDataTypeId = "meta:type:DateTime2",
            SnapshotTimestampPrecision = "7",
            TableNamePattern = "PIT_{Name}"
        };
        model.BusinessPointInTimeImplementationList.Add(record7);
        var record8 = new BusinessReferenceImplementation
        {
            Id = "default-business-reference",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            HashKeyColumnName = "HashKey",
            HashKeyDataTypeId = "meta:type:Binary",
            HashKeyLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "REF_{Name}"
        };
        model.BusinessReferenceImplementationList.Add(record8);
        var record9 = new BusinessReferenceSatelliteImplementation
        {
            Id = "default-business-reference-satellite",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            HashDiffColumnName = "HashDiff",
            HashDiffDataTypeId = "meta:type:Binary",
            HashDiffLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            ParentForeignKeyNamePattern = "FK_{TableName}_{ParentTableName}",
            ParentHashKeyColumnName = "ReferenceHashKey",
            ParentHashKeyDataTypeId = "meta:type:Binary",
            ParentHashKeyLength = "32",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "RSAT_{ParentName}_{Name}"
        };
        model.BusinessReferenceSatelliteImplementationList.Add(record9);
        var record10 = new BusinessSameAsLinkImplementation
        {
            Id = "default-business-same-as-link",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            EquivalentHashKeyColumnName = "EquivalentHashKey",
            EquivalentHubForeignKeyNamePattern = "FK_{TableName}_{TargetTableName}_{SourceColumnName}",
            HashKeyColumnName = "HashKey",
            HashKeyDataTypeId = "meta:type:Binary",
            HashKeyLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            PrimaryHashKeyColumnName = "PrimaryHashKey",
            PrimaryHubForeignKeyNamePattern = "FK_{TableName}_{TargetTableName}_{SourceColumnName}",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "BSAL_{Name}"
        };
        model.BusinessSameAsLinkImplementationList.Add(record10);
        var record11 = new BusinessSameAsLinkSatelliteImplementation
        {
            Id = "default-business-same-as-link-satellite",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            HashDiffColumnName = "HashDiff",
            HashDiffDataTypeId = "meta:type:Binary",
            HashDiffLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            ParentForeignKeyNamePattern = "FK_{TableName}_{ParentTableName}",
            ParentHashKeyColumnName = "LinkHashKey",
            ParentHashKeyDataTypeId = "meta:type:Binary",
            ParentHashKeyLength = "32",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "BSALS_{ParentName}_{Name}"
        };
        model.BusinessSameAsLinkSatelliteImplementationList.Add(record11);
        var record12 = new RawHubImplementation
        {
            Id = "default-raw-hub",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            HashKeyColumnName = "HashKey",
            HashKeyDataTypeId = "meta:type:Binary",
            HashKeyLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "H_{Name}"
        };
        model.RawHubImplementationList.Add(record12);
        var record13 = new RawHubSatelliteImplementation
        {
            Id = "default-raw-hub-satellite",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            HashDiffColumnName = "HashDiff",
            HashDiffDataTypeId = "meta:type:Binary",
            HashDiffLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            ParentForeignKeyNamePattern = "FK_{TableName}_{ParentTableName}",
            ParentHashKeyColumnName = "HubHashKey",
            ParentHashKeyDataTypeId = "meta:type:Binary",
            ParentHashKeyLength = "32",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "HS_{ParentName}_{Name}"
        };
        model.RawHubSatelliteImplementationList.Add(record13);
        var record14 = new RawLinkImplementation
        {
            Id = "default-raw-link",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            EndHashKeyColumnPattern = "{Role}HashKey",
            HashKeyColumnName = "HashKey",
            HashKeyDataTypeId = "meta:type:Binary",
            HashKeyLength = "32",
            HubForeignKeyNamePattern = "FK_{TableName}_{TargetTableName}_{SourceColumnName}",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "L_{Name}"
        };
        model.RawLinkImplementationList.Add(record14);
        var record15 = new RawLinkSatelliteImplementation
        {
            Id = "default-raw-link-satellite",
            AuditIdColumnName = "AuditId",
            AuditIdDataTypeId = "meta:type:Int64",
            AuditIdDefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            HashDiffColumnName = "HashDiff",
            HashDiffDataTypeId = "meta:type:Binary",
            HashDiffLength = "32",
            LoadTimestampColumnName = "LoadTimestamp",
            LoadTimestampDataTypeId = "meta:type:DateTime2",
            LoadTimestampDefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            LoadTimestampPrecision = "7",
            ParentForeignKeyNamePattern = "FK_{TableName}_{ParentTableName}",
            ParentHashKeyColumnName = "LinkHashKey",
            ParentHashKeyDataTypeId = "meta:type:Binary",
            ParentHashKeyLength = "32",
            PrimaryKeyNamePattern = "PK_{TableName}",
            RecordSourceColumnName = "RecordSource",
            RecordSourceDataTypeId = "meta:type:String",
            RecordSourceLength = "256",
            SchemaName = "dbo",
            TableNamePattern = "LS_{ParentName}_{Name}"
        };
        model.RawLinkSatelliteImplementationList.Add(record15);
        return model;
    }
}