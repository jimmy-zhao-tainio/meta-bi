#nullable enable
using System;
using System.Collections.Generic;

namespace MetaDataWarehouseImplementation;
public sealed partial class AccumulatingSnapshotFactTableImplementation
{
    public string Id { get; set; } = null !;
    public string MilestoneDateColumnPattern { get; set; } = null !;
    public string MilestoneDateDataTypeId { get; set; } = null !;
}

public sealed partial class AggregateFactTableImplementation
{
    public string Id { get; set; } = null !;
    public string SchemaName { get; set; } = null !;
    public string? SourceFactForeignKeyNamePattern { get; set; }
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class BridgeTableImplementation
{
    public string Id { get; set; } = null !;
    public string ParticipantForeignKeyNamePattern { get; set; } = null !;
    public string ParticipantKeyColumnPattern { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
    public string? WeightColumnName { get; set; }
    public string? WeightDataTypeId { get; set; }
}

public sealed partial class DimensionTableImplementation
{
    public string Id { get; set; } = null !;
    public string AttributeColumnPattern { get; set; } = null !;
    public string BusinessKeyColumnPattern { get; set; } = null !;
    public string PrimaryKeyNamePattern { get; set; } = null !;
    public string SchemaName { get; set; } = null !;
    public string SurrogateKeyColumnName { get; set; } = null !;
    public string SurrogateKeyDataTypeId { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class FactTableImplementation
{
    public string Id { get; set; } = null !;
    public string DegenerateDimensionColumnPattern { get; set; } = null !;
    public string DimensionForeignKeyNamePattern { get; set; } = null !;
    public string DimensionKeyColumnPattern { get; set; } = null !;
    public string MeasureColumnPattern { get; set; } = null !;
    public string? PrimaryKeyNamePattern { get; set; }
    public string SchemaName { get; set; } = null !;
    public string TableNamePattern { get; set; } = null !;
}

public sealed partial class IndexImplementation
{
    public string Id { get; set; } = null !;
    public string AppliesToBridgeTables { get; set; } = null !;
    public string AppliesToDimensionTables { get; set; } = null !;
    public string AppliesToFactTables { get; set; } = null !;
    public string ColumnNamePattern { get; set; } = null !;
    public string? FilterSql { get; set; }
    public string? IsClustered { get; set; }
    public string? IsUnique { get; set; }
    public string NamePattern { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
}

public sealed partial class PeriodicSnapshotFactTableImplementation
{
    public string Id { get; set; } = null !;
    public string PeriodDataTypeId { get; set; } = null !;
    public string? PeriodEndColumnName { get; set; }
    public string PeriodStartColumnName { get; set; } = null !;
}

public sealed partial class PlatformColumnImplementation
{
    public string Id { get; set; } = null !;
    public string AppliesToBridgeTables { get; set; } = null !;
    public string AppliesToDimensionTables { get; set; } = null !;
    public string AppliesToFactTables { get; set; } = null !;
    public string ColumnName { get; set; } = null !;
    public string DataTypeId { get; set; } = null !;
    public string? DefaultExpressionSql { get; set; }
    public string? IsNullable { get; set; }
    public string? Length { get; set; }
    public string Ordinal { get; set; } = null !;
    public string? Precision { get; set; }
    public string? Scale { get; set; }
}

public sealed partial class SlowlyChangingDimensionTableImplementation
{
    public string Id { get; set; } = null !;
    public string CurrentFlagColumnName { get; set; } = null !;
    public string CurrentFlagDataTypeId { get; set; } = null !;
    public string EffectiveFromColumnName { get; set; } = null !;
    public string EffectiveFromDataTypeId { get; set; } = null !;
    public string EffectiveToColumnName { get; set; } = null !;
    public string EffectiveToDataTypeId { get; set; } = null !;
    public string? HashDiffColumnName { get; set; }
    public string? HashDiffDataTypeId { get; set; }
}

public sealed partial class MetaDataWarehouseImplementationModel
{
    public static MetaDataWarehouseImplementationModel CreateEmpty() => new();
    public List<AccumulatingSnapshotFactTableImplementation> AccumulatingSnapshotFactTableImplementationList { get; set; } = new();
    public List<AggregateFactTableImplementation> AggregateFactTableImplementationList { get; set; } = new();
    public List<BridgeTableImplementation> BridgeTableImplementationList { get; set; } = new();
    public List<DimensionTableImplementation> DimensionTableImplementationList { get; set; } = new();
    public List<FactTableImplementation> FactTableImplementationList { get; set; } = new();
    public List<IndexImplementation> IndexImplementationList { get; set; } = new();
    public List<PeriodicSnapshotFactTableImplementation> PeriodicSnapshotFactTableImplementationList { get; set; } = new();
    public List<PlatformColumnImplementation> PlatformColumnImplementationList { get; set; } = new();
    public List<SlowlyChangingDimensionTableImplementation> SlowlyChangingDimensionTableImplementationList { get; set; } = new();
}

public static partial class MetaDataWarehouseImplementationInstance
{
    private static readonly MetaDataWarehouseImplementationModel _builtIn = CreateBuiltIn();
    public static MetaDataWarehouseImplementationModel BuiltIn => _builtIn;

    public static MetaDataWarehouseImplementationModel CreateBuiltIn()
    {
        var model = MetaDataWarehouseImplementationModel.CreateEmpty();
        var record0 = new AccumulatingSnapshotFactTableImplementation
        {
            Id = "default-accumulating-snapshot-fact",
            MilestoneDateColumnPattern = "{DateRoleName}Date",
            MilestoneDateDataTypeId = "meta:type:Date"
        };
        model.AccumulatingSnapshotFactTableImplementationList.Add(record0);
        var record1 = new AggregateFactTableImplementation
        {
            Id = "default-aggregate-fact-table",
            SchemaName = "dw",
            SourceFactForeignKeyNamePattern = "FK_{TableName}_{SourceFactName}",
            TableNamePattern = "Agg_{Name}"
        };
        model.AggregateFactTableImplementationList.Add(record1);
        var record2 = new BridgeTableImplementation
        {
            Id = "default-bridge-table",
            ParticipantForeignKeyNamePattern = "FK_{TableName}_{RoleName}",
            ParticipantKeyColumnPattern = "{RoleName}Key",
            PrimaryKeyNamePattern = "PK_{TableName}",
            SchemaName = "dw",
            TableNamePattern = "Bridge_{Name}",
            WeightColumnName = "Weight",
            WeightDataTypeId = "meta:type:Decimal"
        };
        model.BridgeTableImplementationList.Add(record2);
        var record3 = new DimensionTableImplementation
        {
            Id = "default-dimension-table",
            AttributeColumnPattern = "{Name}",
            BusinessKeyColumnPattern = "{PartName}",
            PrimaryKeyNamePattern = "PK_{TableName}",
            SchemaName = "dw",
            SurrogateKeyColumnName = "{Name}Key",
            SurrogateKeyDataTypeId = "meta:type:Int64",
            TableNamePattern = "Dim_{Name}"
        };
        model.DimensionTableImplementationList.Add(record3);
        var record4 = new FactTableImplementation
        {
            Id = "default-fact-table",
            DegenerateDimensionColumnPattern = "{Name}",
            DimensionForeignKeyNamePattern = "FK_{TableName}_{RoleName}",
            DimensionKeyColumnPattern = "{RoleName}Key",
            MeasureColumnPattern = "{Name}",
            PrimaryKeyNamePattern = "PK_{TableName}",
            SchemaName = "dw",
            TableNamePattern = "Fact_{Name}"
        };
        model.FactTableImplementationList.Add(record4);
        var record5 = new IndexImplementation
        {
            Id = "default-dimension-business-key-index",
            AppliesToBridgeTables = "false",
            AppliesToDimensionTables = "true",
            AppliesToFactTables = "false",
            ColumnNamePattern = "{ColumnName}",
            IsClustered = "false",
            IsUnique = "false",
            NamePattern = "IX_{TableName}_{ColumnName}",
            Ordinal = "10"
        };
        model.IndexImplementationList.Add(record5);
        var record6 = new IndexImplementation
        {
            Id = "default-fact-date-index",
            AppliesToBridgeTables = "false",
            AppliesToDimensionTables = "false",
            AppliesToFactTables = "true",
            ColumnNamePattern = "{ColumnName}",
            IsClustered = "false",
            IsUnique = "false",
            NamePattern = "IX_{TableName}_{ColumnName}",
            Ordinal = "20"
        };
        model.IndexImplementationList.Add(record6);
        var record7 = new PeriodicSnapshotFactTableImplementation
        {
            Id = "default-periodic-snapshot-fact",
            PeriodDataTypeId = "meta:type:Date",
            PeriodEndColumnName = "PeriodEndDate",
            PeriodStartColumnName = "PeriodStartDate"
        };
        model.PeriodicSnapshotFactTableImplementationList.Add(record7);
        var record8 = new PlatformColumnImplementation
        {
            Id = "platform-audit-id",
            AppliesToBridgeTables = "true",
            AppliesToDimensionTables = "true",
            AppliesToFactTables = "true",
            ColumnName = "AuditId",
            DataTypeId = "meta:type:Int64",
            DefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            IsNullable = "false",
            Ordinal = "9000"
        };
        model.PlatformColumnImplementationList.Add(record8);
        var record9 = new PlatformColumnImplementation
        {
            Id = "platform-insert-datetime2",
            AppliesToBridgeTables = "true",
            AppliesToDimensionTables = "true",
            AppliesToFactTables = "true",
            ColumnName = "InsertDateTime2",
            DataTypeId = "meta:type:DateTime2",
            DefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            IsNullable = "false",
            Ordinal = "9010",
            Precision = "7"
        };
        model.PlatformColumnImplementationList.Add(record9);
        var record10 = new SlowlyChangingDimensionTableImplementation
        {
            Id = "default-slowly-changing-dimension",
            CurrentFlagColumnName = "IsCurrent",
            CurrentFlagDataTypeId = "meta:type:Boolean",
            EffectiveFromColumnName = "EffectiveFromDateTime2",
            EffectiveFromDataTypeId = "meta:type:DateTime2",
            EffectiveToColumnName = "EffectiveToDateTime2",
            EffectiveToDataTypeId = "meta:type:DateTime2",
            HashDiffColumnName = "HashDiff",
            HashDiffDataTypeId = "meta:type:Binary"
        };
        model.SlowlyChangingDimensionTableImplementationList.Add(record10);
        return model;
    }
}