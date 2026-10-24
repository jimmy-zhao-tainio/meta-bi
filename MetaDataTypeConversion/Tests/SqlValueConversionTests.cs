using MetaDataType;
using MetaDataTypeConversion.Core;
using MetaTransformScript.Sql;

namespace MetaDataTypeConversion.Tests;

public sealed class SqlValueConversionTests
{
    private static readonly MetaTransformScriptSqlService Syntax = new();
    private static readonly MetaDataTypeConversionService Service = new();

    [Fact]
    public void RealMapping_UsesAuthoredTemplate_AndPreservesCompoundExpression()
    {
        var model = MetaDataTypeConversionInstance.CreateBuiltIn();
        var result = Generate(model, "sqlserver:type:int", new(Length: 24), "([s].[Quantity] + 2) * 3");
        Assert.Equal("sqlserver:type:nvarchar", result.Mapping.TargetDataTypeId);
        Assert.Equal("CAST(([s].[Quantity] + 2) * 3 AS nvarchar(24))", Syntax.ExportScalarExpression(result.Expression));
        Assert.Single(result.Expression.Model.CastCallList);

        // Same mapping, different authored reusable template; no generator changes.
        model.ConversionImplementationList.Single(row => row.Name == "SqlCast").SqlConversionTemplate =
            model.SqlConversionTemplateList.Single(row => row.Function == "Convert");
        var changed = Generate(model, "sqlserver:type:int", new(Length: 24), "([s].[Quantity] + 2) * 3");
        Assert.Equal("CONVERT(nvarchar(24), ([s].[Quantity] + 2) * 3)", Syntax.ExportScalarExpression(changed.Expression));
        Assert.Single(changed.Expression.Model.ConvertCallList);
    }

    [Theory]
    [InlineData("meta:type:Decimal", null, false, 19, 4, "decimal(19, 4)")]
    [InlineData("meta:type:String", 200, false, null, null, "nvarchar(200)")]
    [InlineData("meta:type:String", null, true, null, null, "nvarchar(max)")]
    [InlineData("meta:type:DateTime2", null, false, null, 3, "datetime2(3)")]
    [InlineData("meta:type:Double", null, false, 53, null, "float(53)")]
    [InlineData("meta:type:Int32", null, false, null, null, "int")]
    public void ExplicitTargetFacets_SurviveSyntaxModelAndRendering(string source, int? length, bool max, int? precision, int? scale, string expectedType)
    {
        var model = MetaDataTypeConversionInstance.CreateBuiltIn();
        model.DataTypeMappingList.Single(row => row.SourceDataTypeId == source).ConversionImplementation =
            model.ConversionImplementationList.Single(row => row.Name == "SqlCast");
        var result = Generate(model, source, new(length, max, precision, scale));
        var sql = Syntax.ExportScalarExpression(result.Expression);
        Assert.Equal($"CAST([s].[Value] AS {expectedType})", sql);
        // Syntax survives the existing sanctioned importer/emitter, including type parameter order.
        Assert.Equal(sql, Syntax.ExportScalarExpression(Syntax.ImportScalarExpression(sql)));
    }

    [Fact]
    public void Direct_IsPassThrough_AndStructuralIsNeverACastFallback()
    {
        var model = MetaDataTypeConversionInstance.CreateBuiltIn();
        Assert.Equal("[s].[Value]", Syntax.ExportScalarExpression(Generate(model, "meta:type:String", new()).Expression));
        Assert.Contains("Structural", Assert.Throws<InvalidOperationException>(() => Generate(model, "meta:type:Xml", new())).Message);
    }

    [Theory]
    [InlineData("TryCast")]
    [InlineData("TryConvert")]
    [InlineData("Parse")]
    [InlineData("Unknown")]
    public void UnsupportedFunctionsFailWithoutChangingFailureBehavior(string function)
    {
        var model = MetaDataTypeConversionInstance.CreateBuiltIn();
        model.SqlConversionTemplateList.Single(row => row.Function == "Cast").Function = function;
        var error = Assert.Throws<InvalidOperationException>(() => Generate(model, "sqlserver:type:int", new(Length: 24)));
        Assert.Contains(function, error.Message);
        Assert.Contains("strict", error.Message);
        Assert.Equal("sqlserver:type:nvarchar", Service.Resolve(model, "sqlserver:type:int", "SqlServer").TargetDataTypeId);
        Assert.Single(Service.ResolveCompatibility(model, "sqlserver:type:int", "sqlserver:type:nvarchar").Path);
    }

    [Fact]
    public void MissingImplementationTemplate_AndDanglingTemplate_FailWithIdentities()
    {
        var model = MetaDataTypeConversionInstance.CreateBuiltIn();
        var implementation = model.ConversionImplementationList.Single(row => row.Name == "SqlCast");
        implementation.SqlConversionTemplate = null;
        Assert.Contains(implementation.Id, Assert.Throws<InvalidOperationException>(() => Generate(model, "sqlserver:type:int", new(Length: 24))).Message);
        implementation.SqlConversionTemplate = new() { Id = "missing-template" };
        Assert.Contains("missing-template", Assert.Throws<InvalidOperationException>(() => Generate(model, "sqlserver:type:int", new(Length: 24))).Message);
        var mapping = model.DataTypeMappingList.Single(row => row.Id.EndsWith(":int-to-nvarchar", StringComparison.Ordinal));
        mapping.ConversionImplementation = null!;
        Assert.Contains(mapping.Id, Assert.Throws<InvalidOperationException>(() => Generate(model, "sqlserver:type:int", new(Length: 24))).Message);
    }

    [Fact]
    public void UnsupportedDialect_AndMissingCatalogType_FailClearly()
    {
        var model = MetaDataTypeConversionInstance.CreateBuiltIn();
        var template = model.SqlConversionTemplateList.Single(row => row.Function == "Cast");
        template.Dialect = "PostgreSql";
        Assert.Contains("PostgreSql", Assert.Throws<InvalidOperationException>(() => Generate(model, "sqlserver:type:int", new(Length: 24))).Message);
        template.Dialect = "SqlServer";
        Assert.Contains("sqlserver:type:nvarchar", Assert.Throws<InvalidOperationException>(() => Service.GenerateSqlExpression(
            model, MetaDataTypeModel.CreateEmpty(), new("sqlserver:type:int", "SqlServer", "1", new(Length: 24)))).Message);
    }

    [Fact]
    public void CompatibilityPath_IsNotAnExecutableConversionPlan()
    {
        var model = MetaDataTypeConversionInstance.CreateBuiltIn();
        // Int32 -> int -> nvarchar -> String remains compatibility evidence, not a conversion program.
        var path = Service.ResolveCompatibility(model, "meta:type:Int32", "meta:type:String");
        Assert.True(path.Path.Count > 1);
        Assert.Equal("[s].[Value]", Syntax.ExportScalarExpression(Generate(model, "meta:type:Int32", new()).Expression));
    }

    [Theory]
    [InlineData("1; SELECT 2")]
    [InlineData("1, 2")]
    [InlineData("1 FROM dbo.Secret")]
    [InlineData("1 AS Alias")]
    public void InputMustBeExactlyOneScalarExpression(string input)
    {
        Assert.Throws<MetaTransformScriptSqlImportException>(() => Generate(MetaDataTypeConversionInstance.CreateBuiltIn(), "sqlserver:type:int", new(Length: 24), input));
    }

    [Theory]
    [InlineData(null, false, null, null)]
    [InlineData(0, false, null, null)]
    [InlineData(4001, false, null, null)]
    [InlineData(24, true, null, null)]
    [InlineData(24, false, 19, 4)]
    public void MissingOrInapplicableFacets_DoNotInventDefaults(int? length, bool max, int? precision, int? scale)
    {
        Assert.Throws<ArgumentException>(() => Generate(MetaDataTypeConversionInstance.CreateBuiltIn(), "sqlserver:type:int", new(length, max, precision, scale)));
    }

    [Theory]
    [InlineData("meta:type:Decimal", null, null)]
    [InlineData("meta:type:Decimal", 19, null)]
    [InlineData("meta:type:Decimal", 39, 4)]
    [InlineData("meta:type:Decimal", 19, 20)]
    [InlineData("meta:type:DateTime2", null, null)]
    [InlineData("meta:type:DateTime2", null, 8)]
    [InlineData("meta:type:Double", null, null)]
    [InlineData("meta:type:Double", 54, null)]
    public void NumericAndTemporalFacets_RequireExplicitValidParameters(string source, int? precision, int? scale)
    {
        var model = MetaDataTypeConversionInstance.CreateBuiltIn();
        model.DataTypeMappingList.Single(row => row.SourceDataTypeId == source).ConversionImplementation =
            model.ConversionImplementationList.Single(row => row.Name == "SqlCast");
        Assert.Throws<ArgumentException>(() => Generate(model, source, new(Precision: precision, Scale: scale)));
    }

    private static SqlValueConversionResult Generate(MetaDataTypeConversionModel model, string source, SqlServerTypeFacets facets, string input = "[s].[Value]") =>
        Service.GenerateSqlExpression(model, MetaDataTypeInstance.BuiltIn, new(source, "SqlServer", input, facets));
}
