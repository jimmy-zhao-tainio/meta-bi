using MetaSchema;
using MetaTransform.Binding;
using MetaTransformScript;
using MetaTransformScript.Sql;

public sealed class TransformBindingHardeningTests
{
    [Fact]
    public void BindTransform_AllowsSourceQualifiedMultipartColumnReferences()
    {
        var bound = BindSql("""
CREATE VIEW dbo.v_multipart AS
SELECT [SourceDb].[ExternalSchema].[SourceTable].[SomeColumn] AS SomeColumn
FROM [SourceDb].[ExternalSchema].[SourceTable]
""");

        Assert.DoesNotContain(bound.Issues, item => item.Code == "UnsupportedColumnReferenceShape");
        Assert.DoesNotContain(bound.Issues, item => item.Code == "ColumnReferenceNotFound");
        Assert.False(bound.HasErrors);
    }

    [Fact]
    public void BindTransform_AllowsQualifiedGroupedColumnInsideScalarWrapper()
    {
        var bound = BindSql("""
CREATE VIEW dbo.v_grouped AS
SELECT
    ISNULL(a.Code, 'Missing') AS Code,
    SUM(a.Amount) AS Amount
FROM dbo.Source AS a
GROUP BY Code
""");

        Assert.DoesNotContain(bound.Issues, item => item.Code == "UngroupedColumnReference");
        Assert.False(bound.HasErrors);
    }

    [Fact]
    public void BindTransform_ColumnReferenceNotFoundMessageListsVisibleSources()
    {
        var bound = BindSql(
            """
CREATE VIEW dbo.v_missing_column AS
SELECT s.MissingCode
FROM dbo.Source AS s
""",
            CreateSchema("ExecDb", ("dbo", "Source", ["SourceId", "Code"])));

        var issue = Assert.Single(bound.Issues, item => item.Code == "QualifiedColumnReferenceNotFound");
        Assert.Contains("s.MissingCode", issue.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("s (dbo.Source", issue.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("SourceId", issue.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Code", issue.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BindTransform_UngroupedColumnReferenceMessageListsGroupedKeys()
    {
        var bound = BindSql(
            """
CREATE VIEW dbo.v_grouping_failure AS
SELECT
    a.Code,
    a.OtherCode,
    SUM(a.Amount) AS Amount
FROM dbo.Source AS a
GROUP BY Code
""",
            CreateSchema("ExecDb", ("dbo", "Source", ["Code", "OtherCode", "Amount"])));

        var issue = Assert.Single(bound.Issues, item => item.Code == "UngroupedColumnReference");
        Assert.Contains("a.OtherCode", issue.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Grouped keys: Code", issue.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("a (dbo.Source", issue.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BindTransform_AllowsCteDerivedUnionGroupedProjection()
    {
        var bound = BindSql("""
CREATE VIEW dbo.v_cte_derived_union_grouped AS
WITH src AS
(
    SELECT s.Code, s.Amount
    FROM dbo.Source AS s
)
SELECT
    ISNULL(d.Code, 'Missing') AS Code,
    SUM(d.Amount) AS Amount
FROM
(
    SELECT Code, Amount
    FROM src
    UNION ALL
    SELECT Code, Amount
    FROM src
) AS d
GROUP BY Code
""");

        Assert.DoesNotContain(bound.Issues, item => item.Code == "UngroupedColumnReference");
        Assert.DoesNotContain(bound.Issues, item => item.Code == "ColumnReferenceNotFound");
        Assert.False(bound.HasErrors);
    }

    [Fact]
    public void BindingWorkspaceService_OnePartSourceInsideCte_ResolvesWithExecuteSystemDefaultSchema()
    {
        var transformModel = new MetaTransformScriptSqlService().ImportFromSqlCode("""
CREATE VIEW dbo.Target AS
WITH src AS
(
    SELECT s.CustomerId
    FROM SourceTable AS s
)
SELECT src.CustomerId
FROM src
""");
        var transformScript = Assert.Single(transformModel.TransformScriptList);
        transformModel.ScriptObjectViewList.Add(new ScriptObjectView
        {
            Id = "ScriptObjectView:Target",
            TransformScript = transformScript,
            TargetSqlIdentifier = "dbo.Target"
        });
        var sourceSchema = CreateSchema("ExecDb", ("dbo", "SourceTable", ["CustomerId"]));
        var targetSchema = CreateSchema("WarehouseDb", ("dbo", "Target", ["CustomerId"]));
        var tempRoot = Path.Combine(
            Path.GetTempPath(),
            "MetaTransform.Binding.Hardening.Tests",
            Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var sourceSchemaWorkspacePath = Path.Combine(tempRoot, "SourceSchemaWorkspace");
        var targetSchemaWorkspacePath = Path.Combine(tempRoot, "TargetSchemaWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(transformModel, transformWorkspacePath);
            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(sourceSchema, sourceSchemaWorkspacePath);
            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(targetSchema, targetSchemaWorkspacePath);

            var result = new TransformBindingWorkspaceService().BindValidatedToWorkspace(
                transformWorkspacePath,
                new[] { sourceSchemaWorkspacePath },
                targetSchemaWorkspacePath,
                executeSystemName: "ExecDb",
                executeSystemDefaultSchemaName: "dbo",
                newWorkspacePath: bindingWorkspacePath);

            Assert.Equal(1, result.TransformBindingCount);
            Assert.Equal(1, result.SourceColumnValidationCount);
            Assert.Equal(1, result.TargetColumnValidationCount);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [Fact]
    public void BindingWorkspaceService_RejectsExplicitViewAsTargetContract()
    {
        var transformModel = new MetaTransformScriptSqlService().ImportFromSqlCode("""
CREATE VIEW dbo.Target AS
SELECT s.CustomerId
FROM dbo.SourceView AS s
""");
        var transformScript = Assert.Single(transformModel.TransformScriptList);
        transformModel.ScriptObjectViewList.Add(new ScriptObjectView
        {
            Id = "ScriptObjectView:Target",
            TransformScript = transformScript,
            TargetSqlIdentifier = "dbo.Target"
        });
        var sourceSchema = CreateSchema("ExecDb", ("dbo", "SourceView", ["CustomerId"]));
        ReplaceTableWithView(sourceSchema, 0);
        var targetSchema = CreateSchema("WarehouseDb", ("dbo", "Target", ["CustomerId"]));
        ReplaceTableWithView(targetSchema, 0);
        var tempRoot = Path.Combine(
            Path.GetTempPath(),
            "MetaTransform.Binding.Hardening.Tests",
            Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(tempRoot, "TransformWorkspace");
        var sourceSchemaWorkspacePath = Path.Combine(tempRoot, "SourceSchemaWorkspace");
        var targetSchemaWorkspacePath = Path.Combine(tempRoot, "TargetSchemaWorkspace");
        var bindingWorkspacePath = Path.Combine(tempRoot, "BindingWorkspace");

        try
        {
            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(transformModel, transformWorkspacePath);
            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(sourceSchema, sourceSchemaWorkspacePath);
            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(targetSchema, targetSchemaWorkspacePath);

            var ex = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingWorkspaceService().BindValidatedToWorkspace(
                    transformWorkspacePath,
                    new[] { sourceSchemaWorkspacePath },
                    targetSchemaWorkspacePath,
                    executeSystemName: "ExecDb",
                    executeSystemDefaultSchemaName: null,
                    newWorkspacePath: bindingWorkspacePath));

            Assert.Equal("TargetSchemaObjectNotWritable", ex.Code);
            Assert.Contains("View", ex.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("writable table contracts", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    private static TransformBindingResult BindSql(string sql)
    {
        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql);
        var script = Assert.Single(model.TransformScriptList);
        return new TransformBindingService().BindTransform(model, script);
    }

    private static TransformBindingResult BindSql(string sql, MetaSchemaModel sourceSchema)
    {
        var model = new MetaTransformScriptSqlService().ImportFromSqlCode(sql);
        var script = Assert.Single(model.TransformScriptList);
        return new TransformBindingService().BindTransform(model, script, sourceSchema);
    }

    private static MetaSchemaModel CreateSchema(
        string systemName,
        params (string SchemaName, string TableName, string[] Columns)[] tables)
    {
        var model = MetaSchemaModel.CreateEmpty();
        var system = new MetaSchema.System
        {
            Id = "System:1",
            Name = systemName
        };
        model.SystemList.Add(system);

        var schemasByName = new Dictionary<string, Schema>(StringComparer.OrdinalIgnoreCase);
        var tableOrdinal = 0;

        foreach (var table in tables)
        {
            if (!schemasByName.TryGetValue(table.SchemaName, out var schema))
            {
                schema = new Schema
                {
                    Id = $"Schema:{schemasByName.Count + 1}",
                    System = system,
                    Name = table.SchemaName
                };
                schemasByName.Add(table.SchemaName, schema);
                model.SchemaList.Add(schema);
            }

            var tableId = $"Table:{++tableOrdinal}";
            var schemaObject = new SchemaObject
            {
                Id = tableId,
                Schema = schema,
                Name = table.TableName
            };
            model.SchemaObjectList.Add(schemaObject);

            var tableRow = new Table
            {
                Id = tableId,
                SchemaObject = schemaObject
            };
            model.TableList.Add(tableRow);

            for (var i = 0; i < table.Columns.Length; i++)
            {
                model.FieldList.Add(new Field
                {
                    Id = $"Field:{tableOrdinal}:{i + 1}",
                    SchemaObject = schemaObject,
                    Name = table.Columns[i],
                    MetaDataTypeId = "sqlserver:type:int",
                    IsNullable = "false",
                    Ordinal = i.ToString()
                });
            }
        }

        return model;
    }

    private static void ReplaceTableWithView(MetaSchemaModel model, int tableIndex)
    {
        var table = model.TableList[tableIndex];
        model.TableList.RemoveAt(tableIndex);
        model.ViewList.Add(new View
        {
            Id = table.Id,
            SchemaObject = table.SchemaObject
        });
    }
}
