using MetaSchema;
using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql.Parsing;

public sealed class BindingPhase1Tests
{
    [Fact]
    public void BindSimpleSelectWithAliasesAndLiteralAlias_DerivesExpectedOutputRowset()
    {
        var model = ParseCorpus("001_basic_select.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.NotNull(bound.TopLevelRowset);
        Assert.Equal(
            ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"],
            bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
        Assert.Equal(3, bound.BoundColumnReferences.Count);
    }

    [Fact]
    public void BindSelectStarAcrossJoinedNamedSources_ExpandsColumnsInVisibleOrder()
    {
        var model = ParseCorpus("002_select_star.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customers", ["CustomerId", "CustomerName"]),
            ("dbo", "Orders", ["OrderId", "CustomerId"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        Assert.False(bound.HasErrors);
        Assert.NotNull(bound.TopLevelRowset);
        Assert.Equal(
            ["CustomerId", "CustomerName", "OrderId"],
            bound.TopLevelRowset!.Columns.Select(item => item.Name).ToArray());
    }

    [Fact]
    public void BindAmbiguousUnqualifiedColumn_ProducesExplicitIssue()
    {
        var sql = """
CREATE VIEW dbo.v_ambiguous AS
SELECT
    Id
FROM dbo.Customer AS c
INNER JOIN dbo.[Order] AS o
    ON o.CustomerId = c.Id;
""";

        var model = new MetaTransformScriptSqlParser().ParseSqlCode(sql);
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "Customer", ["Id", "CustomerId"]),
            ("dbo", "Order", ["Id", "CustomerId"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        var issue = Assert.Single(bound.Issues, item => item.Code == "ColumnReferenceAmbiguous");
        Assert.Equal("ColumnReferenceAmbiguous", issue.Code);
    }

    [Fact]
    public void BindWithoutResolvedLanguageProfile_FailsExplicitly()
    {
        var model = ParseCorpus("001_basic_select.sql");
        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]));

        var bound = new TransformBindingService().BindSingleTransform(model, sourceSchema);

        var issue = Assert.Single(bound.Issues);
        Assert.Equal("ActiveLanguageProfileMissing", issue.Code);
        Assert.Null(bound.TopLevelScope);
        Assert.Null(bound.TopLevelRowset);
    }

    [Fact]
    public void BindSimpleSelect_EmitsBindingModelWithFinalOutputRowset()
    {
        var model = ParseCorpus("001_basic_select.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);

        var binding = Assert.Single(bindingModel.TransformBindingList);
        Assert.Equal(model.TransformScriptList[0].Id, binding.TransformScriptId);

        var finalLink = Assert.Single(bindingModel.TransformBindingFinalRowsetLinkList);
        var finalRowset = Assert.Single(bindingModel.BoundRowsetList, item => item.Id == finalLink.ValueId);
        Assert.Equal("FinalOutput", finalRowset.RowsetRole);
        Assert.Equal(
            ["CustomerId", "CustomerName", "CreatedAtAlias", "LiteralValue"],
            bindingModel.BoundColumnList
                .Where(item => item.OwnerId == finalRowset.Id)
                .OrderBy(item => int.Parse(item.Ordinal))
                .Select(item => item.Name)
                .ToArray());
    }

    [Fact]
    public void BindingModel_CanRoundTripAsWorkspaceArtifact()
    {
        var model = ParseCorpus("001_basic_select.sql");
        model.TransformScriptList[0].LanguageProfileId = "MetaTransformSqlServer_v1";

        var sourceSchema = CreateSourceSchema(
            ("dbo", "SourceTable", ["CustomerId", "CustomerName", "CreatedAt"]));

        var bindingModel = new TransformBindingService().BindSingleTransformModel(model, sourceSchema);
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaTransform.Binding.Tests", Guid.NewGuid().ToString("N"));

        try
        {
            bindingModel.SaveToXmlWorkspace(workspacePath);
            var reloaded = MetaTransformBindingModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);

            Assert.Single(reloaded.TransformBindingList);
            Assert.Single(reloaded.TransformBindingFinalRowsetLinkList);
            Assert.NotEmpty(reloaded.BoundRowsetList);
            Assert.NotEmpty(reloaded.BoundColumnList);
        }
        finally
        {
            if (Directory.Exists(workspacePath))
            {
                Directory.Delete(workspacePath, recursive: true);
            }
        }
    }

    private static MetaTransformScriptModel ParseCorpus(string fileName)
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "Reference",
            "Corpus",
            fileName));

        return new MetaTransformScriptSqlParser().ParseSqlCode(File.ReadAllText(path));
    }

    private static MetaSchemaModel CreateSourceSchema(params (string SchemaName, string TableName, string[] Columns)[] tables)
    {
        var model = MetaSchemaModel.CreateEmpty();
        var system = new MetaSchema.System
        {
            Id = "System:1",
            Name = "TestSystem"
        };
        model.SystemList.Add(system);

        var schemaIdsByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var tableOrdinal = 0;

        foreach (var table in tables)
        {
            if (!schemaIdsByName.TryGetValue(table.SchemaName, out var schemaId))
            {
                schemaId = $"Schema:{schemaIdsByName.Count + 1}";
                schemaIdsByName.Add(table.SchemaName, schemaId);
                model.SchemaList.Add(new Schema
                {
                    Id = schemaId,
                    SystemId = system.Id,
                    Name = table.SchemaName
                });
            }

            var tableId = $"Table:{++tableOrdinal}";
            model.TableList.Add(new Table
            {
                Id = tableId,
                SchemaId = schemaId,
                Name = table.TableName
            });

            for (var i = 0; i < table.Columns.Length; i++)
            {
                model.FieldList.Add(new Field
                {
                    Id = $"Field:{tableOrdinal}:{i + 1}",
                    TableId = tableId,
                    Name = table.Columns[i],
                    MetaDataTypeId = "sqlserver:type:int",
                    Ordinal = i.ToString()
                });
            }
        }

        return model;
    }
}
