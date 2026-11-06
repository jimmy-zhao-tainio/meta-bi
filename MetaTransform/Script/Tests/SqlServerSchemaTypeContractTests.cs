using Meta.Integration;
using MetaBi.Tests.Common;
using MetaDataType;
using MetaDataTypeConversion;
using MetaSchema;
using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript.Sql;

[Trait("Category", "Integration")]
public sealed class SqlServerSchemaTypeContractTests
{
    [Theory]
    [InlineData(8, -1, true)]
    [InlineData(-1, -1, true)]
    [InlineData(-1, 8, false)]
    [InlineData(8, 25, true)]
    [InlineData(25, 8, false)]
    [InlineData(8, 8, true)]
    [InlineData(null, 8, true)]
    [InlineData(8, null, true)]
    [InlineData(null, -1, true)]
    [InlineData(-1, null, true)]
    [InlineData(null, null, true)]
    public void LengthConformance_HandlesMaxAndPreservesFiniteAndUnknownFacets(int? sourceLength, int? targetLength, bool accepted)
    {
        using var fixture = new Fixture("sqlserver:type:nvarchar", "sqlserver:type:nvarchar",
            sourceLength: sourceLength, targetLength: targetLength);
        if (accepted)
        {
            var result = fixture.Bind();
            Assert.Equal(0, result.ErrorCount);
            Assert.Single(result.Model.ValidationTargetColumnTypeExactList);
            Assert.Empty(result.Model.ValidationTargetColumnTypeSanctionedConversionList);
        }
        else
        {
            var ex = Assert.Throws<TransformBindingValidationException>(() => fixture.Bind());
            Assert.Equal("TargetColumnLengthConformanceMismatch", ex.Code);
            Assert.Contains($"Length '{sourceLength}'", ex.Message);
            Assert.Contains($"Length '{targetLength}'", ex.Message);
        }
    }

    [Fact]
    public void NationalStringLiteral_BindsItsFiniteResultLengthToMaxTarget()
    {
        using var fixture = new Fixture("sqlserver:type:nvarchar", "sqlserver:type:nvarchar", "N'constant'",
            sourceLength: -1, targetLength: -1);
        var result = fixture.Bind();
        Assert.Equal(0, result.ErrorCount);
        Assert.Single(result.Model.ValidationTargetColumnTypeExactList);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DefaultSqlTypes_NeedNoConversionMappings(bool partial)
    {
        using var fixture = new Fixture("sqlserver:type:int", "sqlserver:type:int");
        var result = fixture.Bind(partial: partial);
        Assert.Single(result.Model.ValidationTargetColumnTypeExactList);
        Assert.Empty(result.Model.ValidationTargetColumnTypeSanctionedConversionList);
    }

    [Theory]
    [InlineData("unknown", false)]
    [InlineData("unknown", true)]
    [InlineData("sqlserver:type:not-in-catalog", false)]
    [InlineData("sqlserver:type:not-in-catalog", true)]
    [InlineData("meta:type:String", false)]
    [InlineData("meta:type:String", true)]
    public void InvalidSchemaTypes_FailWithFieldAndTypeDiagnostics(string id, bool target)
    {
        using var fixture = new Fixture(target ? "sqlserver:type:int" : id, target ? id : "sqlserver:type:int");
        var ex = Assert.Throws<TransformBindingValidationException>(() => fixture.Bind());
        Assert.StartsWith(target ? "TargetSchemaFieldMetaDataType" : "SourceSchemaFieldMetaDataType", ex.Code);
        Assert.Contains(target ? "TargetRows.Value" : "SourceRows.Value", ex.Message);
        if (id.Length > 0) Assert.Contains(id, ex.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MissingType_IsRejectedBySchemaAwareValidation(bool target)
    {
        // Typed workspace authoring itself rejects a missing required MetaDataTypeId.
        // Exercise the production validation entry point with a missing in-memory value too.
        using var fixture = new Fixture("sqlserver:type:int", "sqlserver:type:int");
        var binding = fixture.Bind();
        var source = TypedWorkspaceModelMapper.Load<MetaSchemaModel>(fixture.Source, searchUpward: false);
        var destination = TypedWorkspaceModelMapper.Load<MetaSchemaModel>(fixture.Target, searchUpward: false);
        (target ? destination : source).FieldList.Single().MetaDataTypeId = "";
        var ex = Assert.Throws<TransformBindingValidationException>(() =>
            new TransformBindingValidationService().ApplyValidation(binding.Model, source, destination));
        Assert.Equal((target ? "Target" : "Source") + "SchemaFieldMetaDataTypeMissing", ex.Code);
        Assert.Contains(target ? "TargetRows.Value" : "SourceRows.Value", ex.Message);
    }

    [Theory]
    [InlineData("meta:type:String")]
    [InlineData("sqlserver:type:pretend")]
    public void MatchingForeignIdsAndSqlLookingIds_AreRejected(string id)
    {
        using var fixture = new Fixture(id, id);
        var catalog = MetaDataTypeInstance.CreateBuiltIn();
        if (id.StartsWith("sqlserver:"))
            catalog.DataTypeList.Add(new DataType { Id = id, Name = "int", DataTypeSystem = catalog.DataTypeSystemList.Single(system => system.Name == "Meta") });
        fixture.SaveCatalog(catalog);
        var ex = Assert.Throws<TransformBindingValidationException>(() => fixture.Bind());
        Assert.Contains("does not belong to the SQL Server type system", ex.Message);
        Assert.Contains(id, ex.Message);
    }

    [Theory]
    [InlineData("s.Value", "int")]
    [InlineData("CAST(s.Value AS int)", "int")]
    [InlineData("CONVERT(int, s.Value)", "int")]
    [InlineData("1", "int")]
    [InlineData("2147483648", "bigint")]
    [InlineData("1.25", "decimal")]
    [InlineData("1e0", "float")]
    [InlineData("0x01", "varbinary")]
    [InlineData("N'hi'", "nvarchar")]
    [InlineData("'hi'", "varchar")]
    public void ArbitraryIdsAndExpressionTypes_UseSelectedCatalog(string expression, string sqlType)
    {
        var sourceType = expression.Contains("CAST") || expression.Contains("CONVERT") ? "nvarchar" : "int";
        using var fixture = new Fixture("user-" + sourceType, "user-" + sqlType, expression);
        fixture.SaveCatalog(CustomCatalog());
        var result = fixture.Bind();
        var assessment = Assert.Single(result.Model.ValidationTargetColumnTypeExactList);
        Assert.Equal("user-" + sqlType, assessment.SourceMetaDataTypeId);
        Assert.Equal(assessment.SourceMetaDataTypeId, assessment.TargetMetaDataTypeId);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SelectedCatalog_IsHonouredInBothValidatedBindingModes(bool partial)
    {
        using var fixture = new Fixture("user-nvarchar", "user-int", "CAST(s.Value AS int)");
        fixture.SaveCatalog(CustomCatalog());
        var result = fixture.Bind(partial: partial);
        Assert.Equal(0, result.ErrorCount);
        Assert.Equal("user-int", Assert.Single(result.Model.ValidationTargetColumnTypeExactList).SourceMetaDataTypeId);
    }

    [Theory]
    [InlineData("CAST(s.Value AS int)")]
    [InlineData("CONVERT(int, s.Value)")]
    [InlineData("1")]
    public void ExpressionTypesAbsentFromSelectedCatalog_DoNotFallBackToBuiltIn(string expression)
    {
        using var fixture = new Fixture("user-nvarchar", "user-nvarchar", expression);
        var catalog = CustomCatalog();
        catalog.DataTypeList.RemoveAll(type => type.Name == "int");
        fixture.SaveCatalog(catalog);
        var ex = Assert.Throws<SchemaTypeValidationException>(() => fixture.Bind());
        Assert.Contains("SQL expression type 'int'", ex.Message);
        Assert.Contains("selected MetaDataType catalog", ex.Message);
    }

    [Fact]
    public void Cli_HonoursSelectedCatalogForCastAndReportsInvalidCatalogTypes()
    {
        using var fixture = new Fixture("user-nvarchar", "user-int", "CAST(s.Value AS int)");
        fixture.SaveCatalog(CustomCatalog());
        var args = $"bind --transform-workspace \"{fixture.Transform}\" --source-schema \"{fixture.Source}\" --target-schema \"{fixture.Target}\" --execute-system TestSystem --execute-system-default-schema-name dbo --data-type-workspace \"{fixture.Catalog}\" --data-type-conversion-workspace \"{fixture.Conversions}\" --output-xml \"{fixture.Output}\"";
        var result = CliTestRunner.RunStandardCli("meta-transform-binding", args);
        Assert.True(result.ExitCode == 0, result.Output);
        var binding = TypedWorkspaceModelMapper.Load<MetaTransformBindingModel>(fixture.Output, searchUpward: false);
        Assert.Equal("user-int", Assert.Single(binding.ValidationTargetColumnTypeExactList).SourceMetaDataTypeId);
        // An explicitly supplied empty catalog must fail; the default must not fill its gaps.
        var empty = Path.Combine(fixture.Root, "EmptyCatalog");
        TypedWorkspaceModelMapper.Create(MetaDataTypeModel.CreateEmpty(), empty, "xml");
        var invalid = CliTestRunner.RunStandardCli("meta-transform-binding", args.Replace(fixture.Catalog, empty).Replace(fixture.Output, fixture.Output + "-invalid"));
        Assert.Equal(5, invalid.ExitCode);
        Assert.Contains("user-nvarchar", invalid.Output);
        Assert.Contains("SourceRows.Value", invalid.Output);
    }

    [Fact]
    public void SqlServerLookingSystemIdentity_DoesNotOverrideItsModeledName()
    {
        using var fixture = new Fixture("sqlserver:type:int", "sqlserver:type:int");
        var catalog = MetaDataTypeInstance.CreateBuiltIn();
        catalog.DataTypeSystemList.Single(system => system.Name == "SqlServer").Name = "OtherPlatform";
        fixture.SaveCatalog(catalog);
        var ex = Assert.Throws<TransformBindingValidationException>(() => fixture.Bind());
        Assert.Contains("does not belong to the SQL Server type system", ex.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EverySchemaFieldIsValidated_EvenWhenNotProjected(bool target)
    {
        using var fixture = new Fixture("sqlserver:type:int", "sqlserver:type:int", extraInvalidTarget: target);
        var ex = Assert.Throws<TransformBindingValidationException>(() => fixture.Bind());
        Assert.Contains("Unused", ex.Message);
        Assert.Contains("unknown-unused-type", ex.Message);
    }

    private static MetaDataTypeModel CustomCatalog()
    {
        var catalog = MetaDataTypeModel.CreateEmpty();
        var system = new DataTypeSystem { Id = "arbitrary-system-identity", Name = "SqlServer" };
        catalog.DataTypeSystemList.Add(system);
        foreach (var name in new[] { "int", "bigint", "nvarchar", "varchar", "decimal", "float", "varbinary" })
            catalog.DataTypeList.Add(new DataType { Id = "user-" + name, Name = name, DataTypeSystem = system });
        return catalog;
    }

    private sealed class Fixture : IDisposable
    {
        public string Root { get; } = Path.Combine(Path.GetTempPath(), "SchemaTypeContract", Guid.NewGuid().ToString("N"));
        public string Source => Path.Combine(Root, "Source");
        public string Target => Path.Combine(Root, "Target");
        public string Transform => Path.Combine(Root, "Transform");
        public string Output => Path.Combine(Root, "Binding");
        public string Catalog => Path.Combine(Root, "Catalog");
        public string Conversions => Path.Combine(Root, "Conversions");
        private bool selectedCatalog;

        public Fixture(string sourceId, string targetId, string expression = "s.Value", bool? extraInvalidTarget = null,
            int? sourceLength = null, int? targetLength = null)
        {
            var source = Schema("SourceRows", sourceId);
            var target = Schema("TargetRows", targetId);
            AddLength(source, sourceLength);
            AddLength(target, targetLength);
            if (extraInvalidTarget.HasValue)
            {
                var schema = extraInvalidTarget.Value ? target : source;
                schema.FieldList.Add(new Field { Id = "unused", Name = "Unused", Ordinal = "2", MetaDataTypeId = "unknown-unused-type", SchemaObject = schema.SchemaObjectList.Single() });
            }
            MetaTransformScriptTestHelper.SaveXml(source, Source);
            MetaTransformScriptTestHelper.SaveXml(target, Target);
            new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                $"SELECT {expression} AS Value FROM dbo.SourceRows AS s;", "dbo.TargetRows", Transform, "dbo.TypeContract").GetAwaiter().GetResult();
            TypedWorkspaceModelMapper.Create(MetaDataTypeConversionModel.CreateEmpty(), Conversions, "xml");
        }

        public void SaveCatalog(MetaDataTypeModel catalog)
        {
            TypedWorkspaceModelMapper.Create(catalog, Catalog, "xml");
            selectedCatalog = true;
        }

        public BindToWorkspaceResult Bind(bool partial = false) => new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(
            Transform, [Source], Target, "TestSystem", "dbo", Output,
            dataTypeConversionWorkspacePath: Conversions, allowPartial: partial, dataTypeWorkspacePath: selectedCatalog ? Catalog : null);

        public void Dispose() { if (Directory.Exists(Root)) Directory.Delete(Root, recursive: true); }

        private static void AddLength(MetaSchemaModel model, int? length)
        {
            if (length.HasValue)
                model.FieldDataTypeDetailList.Add(new FieldDataTypeDetail
                {
                    Id = "value:length", Field = model.FieldList.Single(), Name = "Length",
                    Value = length.Value.ToString(global::System.Globalization.CultureInfo.InvariantCulture)
                });
        }

        private static MetaSchemaModel Schema(string tableName, string typeId)
        {
            var model = MetaSchemaModel.CreateEmpty();
            var system = new MetaSchema.System { Id = "system", Name = "TestSystem" };
            var schema = new Schema { Id = "schema", Name = "dbo", System = system };
            var obj = new SchemaObject { Id = tableName, Name = tableName, Schema = schema };
            model.SystemList.Add(system);
            model.SchemaList.Add(schema);
            model.SchemaObjectList.Add(obj);
            model.TableList.Add(new Table { Id = tableName, SchemaObject = obj });
            model.FieldList.Add(new Field { Id = tableName + ":Value", Name = "Value", Ordinal = "1", IsNullable = "true", MetaDataTypeId = typeId, SchemaObject = obj });
            return model;
        }
    }
}
