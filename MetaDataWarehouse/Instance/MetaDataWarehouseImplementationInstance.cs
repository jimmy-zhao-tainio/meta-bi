using MetaDataWarehouseImplementation;

namespace MetaDataWarehouse.Instance;

public static class MetaDataWarehouseImplementationInstance
{
    public static MetaDataWarehouseImplementationModel Default { get; } = CreateDefault();

    private static MetaDataWarehouseImplementationModel CreateDefault()
    {
        var model = MetaDataWarehouseImplementationModel.CreateEmpty();

        model.DimensionTableImplementationList.Add(new DimensionTableImplementation
        {
            Id = "default-dimension-table",
            SchemaName = "dw",
            TableNamePattern = "Dim_{Name}",
            PrimaryKeyNamePattern = "PK_{TableName}",
            SurrogateKeyColumnName = "{Name}Key",
            SurrogateKeyDataTypeId = "meta:type:Int64",
            BusinessKeyColumnPattern = "{PartName}",
            AttributeColumnPattern = "{Name}",
        });

        model.SlowlyChangingDimensionTableImplementationList.Add(new SlowlyChangingDimensionTableImplementation
        {
            Id = "default-slowly-changing-dimension",
            EffectiveFromColumnName = "EffectiveFromDateTime2",
            EffectiveFromDataTypeId = "meta:type:DateTime2",
            EffectiveToColumnName = "EffectiveToDateTime2",
            EffectiveToDataTypeId = "meta:type:DateTime2",
            CurrentFlagColumnName = "IsCurrent",
            CurrentFlagDataTypeId = "meta:type:Boolean",
            HashDiffColumnName = "HashDiff",
            HashDiffDataTypeId = "meta:type:Binary",
        });

        model.FactTableImplementationList.Add(new FactTableImplementation
        {
            Id = "default-fact-table",
            SchemaName = "dw",
            TableNamePattern = "Fact_{Name}",
            PrimaryKeyNamePattern = "PK_{TableName}",
            DimensionForeignKeyNamePattern = "FK_{TableName}_{RoleName}",
            DimensionKeyColumnPattern = "{RoleName}Key",
            DegenerateDimensionColumnPattern = "{Name}",
            MeasureColumnPattern = "{Name}",
        });

        model.PeriodicSnapshotFactTableImplementationList.Add(new PeriodicSnapshotFactTableImplementation
        {
            Id = "default-periodic-snapshot-fact",
            PeriodStartColumnName = "PeriodStartDate",
            PeriodEndColumnName = "PeriodEndDate",
            PeriodDataTypeId = "meta:type:Date",
        });

        model.AccumulatingSnapshotFactTableImplementationList.Add(new AccumulatingSnapshotFactTableImplementation
        {
            Id = "default-accumulating-snapshot-fact",
            MilestoneDateColumnPattern = "{DateRoleName}Date",
            MilestoneDateDataTypeId = "meta:type:Date",
        });

        model.BridgeTableImplementationList.Add(new BridgeTableImplementation
        {
            Id = "default-bridge-table",
            SchemaName = "dw",
            TableNamePattern = "Bridge_{Name}",
            PrimaryKeyNamePattern = "PK_{TableName}",
            ParticipantForeignKeyNamePattern = "FK_{TableName}_{RoleName}",
            ParticipantKeyColumnPattern = "{RoleName}Key",
            WeightColumnName = "Weight",
            WeightDataTypeId = "meta:type:Decimal",
        });

        model.AggregateFactTableImplementationList.Add(new AggregateFactTableImplementation
        {
            Id = "default-aggregate-fact-table",
            SchemaName = "dw",
            TableNamePattern = "Agg_{Name}",
            SourceFactForeignKeyNamePattern = "FK_{TableName}_{SourceFactName}",
        });

        model.PlatformColumnImplementationList.Add(new PlatformColumnImplementation
        {
            Id = "platform-audit-id",
            ColumnName = "AuditId",
            DataTypeId = "meta:type:Int64",
            IsNullable = "false",
            DefaultExpressionSql = "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))",
            AppliesToDimensionTables = "true",
            AppliesToFactTables = "true",
            AppliesToBridgeTables = "true",
            Ordinal = "9000",
        });

        model.PlatformColumnImplementationList.Add(new PlatformColumnImplementation
        {
            Id = "platform-insert-datetime2",
            ColumnName = "InsertDateTime2",
            DataTypeId = "meta:type:DateTime2",
            Precision = "7",
            IsNullable = "false",
            DefaultExpressionSql = "CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))",
            AppliesToDimensionTables = "true",
            AppliesToFactTables = "true",
            AppliesToBridgeTables = "true",
            Ordinal = "9010",
        });

        model.IndexImplementationList.Add(new IndexImplementation
        {
            Id = "default-dimension-business-key-index",
            NamePattern = "IX_{TableName}_{ColumnName}",
            ColumnNamePattern = "{ColumnName}",
            IsUnique = "false",
            IsClustered = "false",
            AppliesToDimensionTables = "true",
            AppliesToFactTables = "false",
            AppliesToBridgeTables = "false",
            Ordinal = "10",
        });

        model.IndexImplementationList.Add(new IndexImplementation
        {
            Id = "default-fact-date-index",
            NamePattern = "IX_{TableName}_{ColumnName}",
            ColumnNamePattern = "{ColumnName}",
            IsUnique = "false",
            IsClustered = "false",
            AppliesToDimensionTables = "false",
            AppliesToFactTables = "true",
            AppliesToBridgeTables = "false",
            Ordinal = "20",
        });

        return model;
    }
}
