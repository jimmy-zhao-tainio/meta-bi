using Meta.Integration;
using MetaDataType;
using MetaSchema;
using MetaTransform.Binding;
using MetaTransformScript;
using MetaTransformScript.Sql;
using Microsoft.Data.SqlClient;

[Trait("Category", "Integration")]
public sealed class TransformBindingExpressionTypeIntegrationTests
{
    // Expected contracts are SQL Server result types, independent of Binding.
    public static IEnumerable<object[]> Expressions()
    {
        yield return Case("SUM(s.Amount)", "decimal", null, 38, 2);
        yield return Case("AVG(s.Amount)", "decimal", null, 38, 6);
        yield return Case("AVG(s.Number)", "int");
        yield return Case("COUNT(*)", "int");
        yield return Case("COUNT_BIG(*)", "bigint");
        yield return Case("MIN(s.Amount)", "decimal", null, 7, 2);
        yield return Case("MAX(s.TextValue)", "nvarchar", 50);
        yield return Case("STDEV(s.Amount)", "float");
        yield return Case("s.Amount + s.Amount", "decimal", null, 8, 2);
        yield return Case("s.Amount - s.Amount", "decimal", null, 8, 2);
        yield return Case("s.Amount * s.Amount", "decimal", null, 15, 4);
        yield return Case("s.Amount / s.Amount", "decimal", null, 17, 10);
        yield return Case("s.Amount % s.Amount", "decimal", null, 7, 2);
        yield return Case("s.Amount + s.Number", "decimal", null, 13, 2);
        yield return Case("s.Amount + 1", "decimal", null, 8, 2);
        yield return Case("s.Amount + 1.0", "numeric", null, 8, 2);
        yield return Case("2147483648", "numeric", null, 10, 0);
        yield return Case(".25", "numeric", null, 2, 2);
        yield return Case("0.25", "numeric", null, 2, 2);
        yield return Case("-s.Amount", "decimal", null, 7, 2);
        yield return Case("SUM(s.Amount) / COUNT(*)", "decimal", null, 38, 6);
        yield return Case("CASE WHEN s.Number > 0 THEN s.Amount ELSE 0 END", "decimal", null, 7, 2);
        yield return Case("COALESCE(s.Amount, 0)", "decimal", null, 7, 2);
        yield return Case("COALESCE(NULL, s.Amount)", "decimal", null, 7, 2);
        yield return Case("NULLIF(s.Amount, 0)", "decimal", null, 7, 2);
        yield return Case("ISNULL(s.Amount, 0)", "decimal", null, 7, 2);
        yield return Case("ROUND(s.Amount, 1)", "decimal", null, 7, 2);
        yield return Case("SUBSTRING(s.TextValue, 1, 3)", "nvarchar", 3);
        yield return Case("s.TextValue + N'! '", "nvarchar", 52);
        yield return Case("CONCAT(s.TextValue, N'! ')", "nvarchar", 52);
        yield return Case("CONCAT('DHL', ',', 'BARIAN')", "varchar", 10);
        yield return Case("(SELECT MAX(t.Amount) FROM dbo.SourceRows t)", "decimal", null, 7, 2);
        yield return Case("CAST(s.Amount AS decimal(30,20)) * CAST(s.Amount AS decimal(30,20))", "decimal", null, 38, 17);
        yield return Case("CAST(s.Amount AS decimal(30,10)) * CAST(s.Amount AS decimal(30,10))", "decimal", null, 38, 6);
        yield return Case("CAST(s.Amount AS decimal(38,2)) + CAST(s.Amount AS decimal(38,2))", "decimal", null, 38, 2);
        yield return Case("SUM(s.Amount) - SUM(s.Amount)", "decimal", null, 38, 2);
        yield return Case("CAST(s.Amount AS decimal(38,37)) + CAST(s.Amount AS decimal(37,0))", "decimal", null, 38, 1);
        yield return Case("CAST(s.Amount AS decimal(38,38)) + CAST(s.Amount AS decimal(38,38))", "decimal", null, 38, 38);
        yield return Case("AVG(CAST(s.Amount AS numeric(7,2)))", "numeric", null, 38, 6);
        yield return Case("CASE WHEN s.Number > 0 THEN s.Amount ELSE s.Number END", "decimal", null, 12, 2);
        yield return Case("-CAST(s.Number AS tinyint)", "smallint");
        yield return Case("ROUND(CAST(s.Number AS real),1)", "float");
        yield return Case("ABS(CAST(s.Number AS real))", "float");
        yield return Case("ABS(CAST(s.Number AS smallmoney))", "money");
        yield return Case("CAST(s.TextValue AS varchar)", "varchar", 30);
        yield return Case("CAST(s.Amount AS decimal)", "decimal", null, 18, 0);
        yield return Case("CAST(s.Amount AS decimal(9))", "decimal", null, 9, 0);
        yield return Case("CAST(s.TextValue AS nvarchar(max))", "nvarchar", -1);
        yield return Case("SUBSTRING(CAST(s.TextValue AS nvarchar(max)),1,3)", "nvarchar", -1);
        yield return Case("CASE WHEN s.Number>0 THEN CAST(s.TextValue AS nchar(5)) ELSE CAST(s.TextValue AS varchar(4)) END", "nvarchar", 5);
        yield return Case("CASE WHEN s.Number>0 THEN CAST(s.TextValue AS char(3)) ELSE CAST(s.TextValue AS varchar(4)) END", "varchar", 4);
        yield return Case("COALESCE(CAST(s.TextValue AS nchar(5)),CAST(s.TextValue AS varchar(4)))", "nvarchar", 5);
        yield return Case("CASE WHEN s.Number>0 THEN CAST(s.TextValue AS nchar(5)) ELSE CAST(s.TextValue AS varchar(6000)) END", "nvarchar", 4000);
    }

    private static object[] Case(string sql, string type, int? length = null, int? precision = null, int? scale = null)
        => [sql, type, length!, precision!, scale!];

    [Theory]
    [MemberData(nameof(Expressions))]
    public void SavedWorkspaces_ResolveExactResultContractThroughSelectedCatalog(
        string expression, string type, int? length, int? precision, int? scale)
    {
        using var fixture = new Fixture(expression, type, length, precision, scale);
        var bound = fixture.Analyze();
        Assert.Empty(bound.Issues);
        var actual = Assert.Single(bound.TopLevelRowset!.Columns).DataType;
        Assert.NotNull(actual);
        Assert.Equal(fixture.TypeId(type), actual.MetaDataTypeId);
        Assert.Equal(length, actual.Length);
        Assert.Equal(precision, actual.Precision);
        Assert.Equal(scale, actual.Scale);
        var validated = fixture.Validate();
        Assert.Equal(0, validated.ErrorCount);
        Assert.Single(validated.Model.ValidationTargetColumnTypeExactList);
    }

    [Theory]
    [InlineData("dbo.UnknownFunction(s.Number)")]
    [InlineData("JSON_VALUE(s.TextValue, '$.value')")]
    public void UnresolvedOutputType_IsRejectedInsteadOfSilentlyValidated(string expression)
    {
        using var fixture = new Fixture(expression, "int");
        var error = Assert.Throws<TransformBindingValidationException>(() => fixture.Validate());
        Assert.Equal("TargetWriteValueTypeNotResolved", error.Code);
        Assert.Contains("Value", error.Message);
        Assert.Contains("TargetRows", error.Message);
        Assert.False(Directory.Exists(fixture.Output));
    }

    [Theory]
    [InlineData("WITH c AS (SELECT SUM(s.Amount) AS Value FROM dbo.SourceRows s) SELECT Value FROM c;", "decimal", null, 38, 2)]
    [InlineData("SELECT s.Amount AS Value FROM dbo.SourceRows s UNION ALL SELECT CAST(0 AS decimal(9,4)) FROM dbo.SourceRows t;", "decimal", null, 9, 4)]
    [InlineData("SELECT s.Amount AS Value FROM dbo.SourceRows s UNION ALL SELECT 0 FROM dbo.SourceRows t;", "decimal", null, 7, 2)]
    [InlineData("SELECT s.TextValue AS Value FROM dbo.SourceRows s UNION ALL SELECT CAST(N'x' AS nvarchar(20)) FROM dbo.SourceRows t;", "nvarchar", 50, null, null)]
    [InlineData("SELECT NULL AS Value FROM dbo.SourceRows s UNION ALL SELECT t.TextValue FROM dbo.SourceRows t;", "nvarchar", 50, null, null)]
    public void CteAndSetOperations_PreserveResultEvidence(string sql, string type, int? length, int? precision, int? scale)
    {
        using var fixture = new Fixture("", type, length, precision, scale, sql);
        var actual = Assert.Single(fixture.Analyze().TopLevelRowset!.Columns).DataType;
        Assert.NotNull(actual);
        Assert.Equal(fixture.TypeId(type), actual.MetaDataTypeId);
        Assert.Equal(length, actual.Length);
        Assert.Equal(precision, actual.Precision);
        Assert.Equal(scale, actual.Scale);
        Assert.Single(fixture.Validate().Model.ValidationTargetColumnTypeExactList);
    }

    [Theory]
    [InlineData(37, 2, "TargetColumnPrecisionConformanceMismatch")]
    [InlineData(38, 1, "TargetColumnScaleConformanceMismatch")]
    public void AggregateEvidence_RejectsInsufficientTargetFacets(int precision, int scale, string code)
    {
        using var fixture = new Fixture("SUM(s.Amount)", "decimal", precision: precision, scale: scale);
        var error = Assert.Throws<TransformBindingValidationException>(() => fixture.Validate());
        Assert.Equal(code, error.Code);
    }

    [Theory]
    [InlineData("CAST(SUM(s.Amount) AS decimal(38,2))")]
    [InlineData("CAST((SELECT MAX(t.Amount) FROM dbo.SourceRows t) AS decimal(38,2))")]
    [InlineData("ROUND(s.Amount, CAST(NULL AS int))")]
    public void ComputedExpressions_PreservePossibleNullResults(string expression)
    {
        using var fixture = new Fixture(expression, "decimal", precision: 38, scale: 2, sourceNullable: false, targetNullable: false);
        Assert.True(Assert.Single(fixture.Analyze().TopLevelRowset!.Columns).DataType!.IsNullable);
        Assert.Equal("TargetColumnNullabilityConformanceMismatch", Assert.Throws<TransformBindingValidationException>(() => fixture.Validate()).Code);
    }

    [Theory]
    [InlineData("INNER JOIN", "a", false)]
    [InlineData("INNER JOIN", "b", false)]
    [InlineData("LEFT JOIN", "a", false)]
    [InlineData("LEFT JOIN", "b", true)]
    [InlineData("RIGHT JOIN", "a", true)]
    [InlineData("RIGHT JOIN", "b", false)]
    [InlineData("FULL JOIN", "a", true)]
    [InlineData("FULL JOIN", "b", true)]
    public void JoinNullExtension_IsReflectedInDestinationValidation(string join, string side, bool nullable)
    {
        using var fixture = new Fixture("", "int", sql: $"SELECT {side}.Number AS Value FROM dbo.SourceRows a {join} dbo.SourceRows b ON a.Number=b.Number;", sourceNullable: false, targetNullable: false);
        Assert.Equal(nullable, Assert.Single(fixture.Analyze().TopLevelRowset!.Columns).DataType!.IsNullable);
        if (nullable)
            Assert.Equal("TargetColumnNullabilityConformanceMismatch", Assert.Throws<TransformBindingValidationException>(() => fixture.Validate()).Code);
        else
            Assert.Single(fixture.Validate().Model.ValidationTargetColumnTypeExactList);
    }

    [Theory]
    [InlineData("CROSS APPLY", false)]
    [InlineData("OUTER APPLY", true)]
    public void ApplyNullExtension_IsReflectedInDestinationValidation(string apply, bool nullable)
    {
        using var fixture = new Fixture("", "int", sql: $"SELECT b.Number AS Value FROM dbo.SourceRows a {apply} (SELECT t.Number FROM dbo.SourceRows t WHERE t.Number=a.Number) b;", sourceNullable: false, targetNullable: false);
        Assert.Equal(nullable, Assert.Single(fixture.Analyze().TopLevelRowset!.Columns).DataType!.IsNullable);
        if (nullable)
            Assert.Equal("TargetColumnNullabilityConformanceMismatch", Assert.Throws<TransformBindingValidationException>(() => fixture.Validate()).Code);
        else
            Assert.Single(fixture.Validate().Model.ValidationTargetColumnTypeExactList);
    }

    [Theory]
    [InlineData("s.Number", false)]
    [InlineData("ROLLUP(s.Number)", true)]
    [InlineData("CUBE(s.Number)", true)]
    [InlineData("GROUPING SETS ((s.Number), ())", true)]
    public void SubtotalGroupingKeys_CannotClaimTheSourceNotNullContract(string grouping, bool nullable)
    {
        using var fixture = new Fixture("", "int", sql: $"SELECT s.Number AS Value FROM dbo.SourceRows s GROUP BY {grouping};", sourceNullable: false, targetNullable: false);
        Assert.Equal(nullable, Assert.Single(fixture.Analyze().TopLevelRowset!.Columns).DataType!.IsNullable);
        if (nullable)
            Assert.Equal("TargetColumnNullabilityConformanceMismatch", Assert.Throws<TransformBindingValidationException>(() => fixture.Validate()).Code);
        else
            Assert.Single(fixture.Validate().Model.ValidationTargetColumnTypeExactList);
    }

    [Fact]
    public async Task ExpectedExpressionContracts_AgreeWithLiveSqlServerMetadata()
    {
        await using var connection = new SqlConnection("Server=.;Database=master;Integrated Security=true;TrustServerCertificate=true;Encrypt=false;Pooling=false");
        await connection.OpenAsync();
        var mismatches = new List<string>();
        foreach (var row in Expressions())
        {
            var sql = $"DECLARE @SourceRows TABLE(Amount decimal(7,2), Number int, TextValue nvarchar(50)); SELECT {((string)row[0]).Replace("dbo.SourceRows", "@SourceRows")} AS Value FROM @SourceRows s;";
            await using var command = new SqlCommand("SELECT system_type_name, error_message FROM sys.dm_exec_describe_first_result_set(@sql, NULL, 0)", connection);
            command.Parameters.AddWithValue("@sql", sql);
            await using var reader = await command.ExecuteReaderAsync();
            Assert.True(await reader.ReadAsync(), sql);
            Assert.True(reader.IsDBNull(1), reader.IsDBNull(1) ? sql : reader.GetString(1));
            var expected = row[2] is int n ? $"{row[1]}({(n == -1 ? "max" : n.ToString())})" : row[3] is int p ? $"{row[1]}({p},{row[4]})" : (string)row[1];
            if (expected != reader.GetString(0)) mismatches.Add($"{row[0]}: expected {expected}; SQL Server reports {reader.GetString(0)}");
        }
        Assert.True(mismatches.Count == 0, string.Join("\n", mismatches));
    }

    private sealed class Fixture : IDisposable
    {
        private readonly string root = Path.Combine(Path.GetTempPath(), "BindingExpressionTypes", Guid.NewGuid().ToString("N"));
        private readonly MetaDataTypeModel catalog = MetaDataTypeInstance.CreateBuiltIn();
        public string Output => Path.Combine(root, "Binding");
        private string PathOf(string name) => Path.Combine(root, name);
        public string TypeId(string name) => catalog.DataTypeList.Single(t => t.Name == name && t.DataTypeSystem.Name == "SqlServer").Id;

        public Fixture(string expression, string type, int? length = null, int? precision = null, int? scale = null, string? sql = null, bool sourceNullable = true, bool targetNullable = true)
        {
            foreach (var t in catalog.DataTypeList) t.Id = "selected-" + Guid.NewGuid().ToString("N");
            catalog.DataTypeSystemList.Single(s => s.Name == "SqlServer").Id = "chosen-system";
            TypedWorkspaceModelMapper.Create(catalog, global::MetaDataType.Core.MetaDataTypeWorkspaceProvider.LoadContract(), new MetaDataTypeWorkspaceBinding(), PathOf("Catalog"), "xml");
            var source = Schema("SourceRows");
            AddField(source, "Amount", "decimal", null, 7, 2);
            AddField(source, "Number", "int");
            AddField(source, "TextValue", "nvarchar", 50);
            var target = Schema("TargetRows");
            AddField(target, "Value", type, length, precision, scale);
            foreach (var f in source.FieldList) f.IsNullable = sourceNullable ? "true" : "false";
            foreach (var f in target.FieldList) f.IsNullable = targetNullable ? "true" : "false";
            MetaTransformScriptTestHelper.SaveXml(source, PathOf("Source"));
            MetaTransformScriptTestHelper.SaveXml(target, PathOf("Target"));
            new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                sql ?? $"SELECT {expression} AS Value FROM dbo.SourceRows s;", "dbo.TargetRows", PathOf("Script"), "dbo.ExpressionType").GetAwaiter().GetResult();
        }

        public TransformBindingResult Analyze()
        {
            var script = TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(PathOf("Script"), new MetaTransformScriptWorkspaceBinding(), searchUpward: false);
            var source = TypedWorkspaceModelMapper.Load<MetaSchemaModel>(PathOf("Source"), new MetaSchemaWorkspaceBinding(), searchUpward: false);
            var selected = TypedWorkspaceModelMapper.Load<MetaDataTypeModel>(PathOf("Catalog"), new MetaDataTypeWorkspaceBinding(), searchUpward: false);
            return new TransformBindingService().BindTransform(script, script.TransformScriptList.Single(), source, selected);
        }

        public BindToWorkspaceResult Validate() => new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(
            PathOf("Script"), [PathOf("Source")], PathOf("Target"), "TestSystem", "dbo", Output,
            allowPartial: false, dataTypeWorkspacePath: PathOf("Catalog"));

        private static MetaSchemaModel Schema(string table)
        {
            var model = MetaSchemaModel.CreateEmpty();
            var system = new MetaSchema.System { Id = "system", Name = "TestSystem" };
            var schema = new Schema { Id = "schema", Name = "dbo", System = system };
            var obj = new SchemaObject { Id = table, Name = table, Schema = schema };
            model.SystemList.Add(system);
            model.SchemaList.Add(schema);
            model.SchemaObjectList.Add(obj);
            model.TableList.Add(new Table { Id = table, SchemaObject = obj });
            return model;
        }

        private void AddField(MetaSchemaModel model, string name, string type, int? length = null, int? precision = null, int? scale = null)
        {
            var field = new Field { Id = name, Name = name, Ordinal = (model.FieldList.Count + 1).ToString(), IsNullable = "true", MetaDataTypeId = TypeId(type), SchemaObject = model.SchemaObjectList.Single() };
            model.FieldList.Add(field);
            foreach (var (facet, value) in new[] { ("Length", length), ("Precision", precision), ("Scale", scale) })
                if (value.HasValue) model.FieldDataTypeDetailList.Add(new FieldDataTypeDetail { Id = name + facet, Field = field, Name = facet, Value = value.Value.ToString() });
        }

        public void Dispose() { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }
}
