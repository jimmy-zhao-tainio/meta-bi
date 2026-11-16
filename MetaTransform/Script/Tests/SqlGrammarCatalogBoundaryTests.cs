using Meta.Integration;
using MetaDataType;
using MetaDataType.Core;
using MetaSchema;
using MetaTransform.Binding;
using MetaTransformScript.Sql;

public sealed class SqlGrammarCatalogBoundaryTests
{
    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task NativeSyntaxBindsToSelectedPersistedIdentitiesAndExactResultFacets(string representation)
    {
        var directory = Path.Combine(Path.GetTempPath(), "sql-grammar-catalog", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var system = new DataTypeSystem { Id = "selected:system", Name = "SqlServer" };
            var catalog = MetaDataTypeModel.CreateEmpty();
            catalog.DataTypeSystemList.Add(system);
            catalog.DataTypeList.Add(new DataType { Id = "selected:text", Name = "nvarchar", DataTypeSystem = system });
            catalog.DataTypeList.Add(new DataType { Id = "selected:integer", Name = "int", DataTypeSystem = system });
            var contract = (await MetaDataTypeWorkspaceProvider.LoadDefaultStateAsync()).Model;
            var catalogPath = Path.Combine(directory, "catalog");
            await WorkspaceSurface.CreateAsync(new MetaDataTypeWorkspaceBinding().Write(catalog, contract), catalogPath, representation);

            var schema = MetaSchemaModel.CreateEmpty();
            var sourceSystem = new MetaSchema.System { Id = "source:system", Name = "Source" };
            var sourceSchema = new Schema { Id = "source:schema", Name = "dbo", System = sourceSystem };
            var table = new SchemaObject { Id = "source:object", Name = "Input", Schema = sourceSchema };
            var field = new Field { Id = "source:field", Name = "Value", Ordinal = "0", SchemaObject = table,
                MetaDataTypeId = "selected:text", IsNullable = "true" };
            schema.SystemList.Add(sourceSystem);
            schema.SchemaList.Add(sourceSchema);
            schema.SchemaObjectList.Add(table);
            schema.TableList.Add(new Table { Id = "source:table", SchemaObject = table });
            schema.FieldList.Add(field);
            schema.FieldDataTypeDetailList.Add(new FieldDataTypeDetail { Id = "source:length", Name = "Length", Value = "7", Field = field });
            var sql = new MetaTransformScriptSqlService();
            var model = sql.ImportFromSqlCode(
                "SELECT CAST(s.Value AS national character varying(25)) AS TextValue, CAST(1 AS integer) AS CountValue FROM dbo.Input AS s;",
                "dbo.Output");

            var bound = new TransformBindingService().BindTransform(model, Assert.Single(model.TransformScriptList), schema,
                MetaDataTypeWorkspaceProvider.LoadOrDefault(catalogPath));

            Assert.False(bound.HasErrors, string.Join(Environment.NewLine, bound.Issues.Select(row => row.Message)));
            Assert.NotNull(bound.TopLevelRowset);
            var columns = bound.TopLevelRowset.Columns;
            var text = Assert.Single(columns, row => row.Name == "TextValue").DataType!;
            Assert.Equal("selected:text", text.MetaDataTypeId);
            Assert.Equal(25, text.Length);
            Assert.True(text.IsNullable);
            Assert.Null(text.Precision);
            Assert.Null(text.Scale);
            var integer = Assert.Single(columns, row => row.Name == "CountValue").DataType!;
            Assert.Equal("selected:integer", integer.MetaDataTypeId);
            Assert.False(integer.IsNullable);
            Assert.Null(integer.Length);
            Assert.Contains("CAST(s.Value AS nvarchar(25)) AS TextValue", sql.ExportToSqlCode(model), StringComparison.Ordinal);
            Assert.Contains("CAST(1 AS int) AS CountValue", sql.ExportToSqlCode(model), StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void ExistingApplicationAliasesKeepTheirBoundedParseClassification()
    {
        var sql = new MetaTransformScriptSqlService();
        var model = sql.ImportFromSqlCode(
            "SELECT CAST(s.Value AS AccountNumber) AS Account, CAST(s.Value AS Flag) AS FlagValue FROM dbo.Input AS s;", "dbo.Aliases");

        var emitted = sql.ExportToSqlCode(model);

        Assert.Contains("CAST(s.Value AS nvarchar) AS Account", emitted, StringComparison.Ordinal);
        Assert.Contains("CAST(s.Value AS bit) AS FlagValue", emitted, StringComparison.Ordinal);
    }
}
