using MetaSchema;

internal static class GeneratedCliSchemaWorkspaceBuilder
{
    public static void SaveWorkspace(string workspacePath, IReadOnlyList<GeneratedCliScenario> scenarios)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        ArgumentNullException.ThrowIfNull(scenarios);

        var model = BuildSchemaModel(scenarios);
        model.SaveToXmlWorkspace(workspacePath);
    }

    private static MetaSchemaModel BuildSchemaModel(IReadOnlyList<GeneratedCliScenario> scenarios)
    {
        var model = MetaSchemaModel.CreateEmpty();
        model.SystemList.Add(new MetaSchema.System
        {
            Id = "System:1",
            Name = "GeneratedCliSystem"
        });

        model.SchemaList.Add(new Schema
        {
            Id = "Schema:1",
            SystemId = "System:1",
            Name = "dbo"
        });

        var tableOrdinal = 0;
        var fieldOrdinal = 0;

        foreach (var scenario in scenarios)
        {
            AddSourceTable(model, "Schema:1", scenario.SourceTableName, ref tableOrdinal, ref fieldOrdinal);
            AddLookupTable(model, "Schema:1", scenario.LookupTableName, ref tableOrdinal, ref fieldOrdinal);
            AddTargetTable(model, "Schema:1", scenario.TargetTableName, ref tableOrdinal, ref fieldOrdinal);
        }

        return model;
    }

    private static void AddSourceTable(
        MetaSchemaModel model,
        string schemaId,
        string tableName,
        ref int tableOrdinal,
        ref int fieldOrdinal)
    {
        var tableId = $"Table:{++tableOrdinal}";
        model.TableList.Add(new Table
        {
            Id = tableId,
            SchemaId = schemaId,
            Name = tableName
        });

        var ordinal = 0;
        AddField(model, tableId, "CustomerId", ordinal++, isNullable: false, ref fieldOrdinal);
        AddField(model, tableId, "OrderAmount", ordinal++, isNullable: false, ref fieldOrdinal);
        AddField(model, tableId, "CreatedAt", ordinal++, isNullable: false, ref fieldOrdinal);
        AddField(model, tableId, "RegionCode", ordinal++, isNullable: true, ref fieldOrdinal);
    }

    private static void AddLookupTable(
        MetaSchemaModel model,
        string schemaId,
        string tableName,
        ref int tableOrdinal,
        ref int fieldOrdinal)
    {
        var tableId = $"Table:{++tableOrdinal}";
        model.TableList.Add(new Table
        {
            Id = tableId,
            SchemaId = schemaId,
            Name = tableName
        });

        var ordinal = 0;
        AddField(model, tableId, "CustomerId", ordinal++, isNullable: false, ref fieldOrdinal);
        AddField(model, tableId, "RegionCode", ordinal++, isNullable: true, ref fieldOrdinal);
    }

    private static void AddTargetTable(
        MetaSchemaModel model,
        string schemaId,
        string tableName,
        ref int tableOrdinal,
        ref int fieldOrdinal)
    {
        var tableId = $"Table:{++tableOrdinal}";
        model.TableList.Add(new Table
        {
            Id = tableId,
            SchemaId = schemaId,
            Name = tableName
        });

        var ordinal = 0;
        AddField(model, tableId, "CustomerId", ordinal++, isNullable: false, ref fieldOrdinal);
        AddField(model, tableId, "OrderAmount", ordinal++, isNullable: false, ref fieldOrdinal);
        AddField(model, tableId, "CreatedAt", ordinal++, isNullable: false, ref fieldOrdinal);
        AddField(model, tableId, "AuditTag", ordinal++, isNullable: true, ref fieldOrdinal);
    }

    private static void AddField(
        MetaSchemaModel model,
        string tableId,
        string fieldName,
        int ordinal,
        bool isNullable,
        ref int fieldOrdinal)
    {
        model.FieldList.Add(new Field
        {
            Id = $"Field:{++fieldOrdinal}",
            TableId = tableId,
            Name = fieldName,
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = isNullable ? "true" : "false",
            Ordinal = ordinal.ToString()
        });
    }
}
