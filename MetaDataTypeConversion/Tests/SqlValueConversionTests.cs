using MetaDataType;
using MetaDataTypeConversion.Core;
using MetaTransformScript.Sql;

namespace MetaDataTypeConversion.Tests;

public sealed class SqlValueConversionTests
{
    private static readonly MetaTransformScriptSqlService Syntax = new();
    private static readonly MetaDataTypeConversionService Service = new();

    [Fact]
    public void AuthoredMapping_InstantiatesSnippet_AndPreservesCompoundExpression()
    {
        var model = CreateSnippetModel();
        var result = Generate(model, "sqlserver:type:int", new(Length: 24), "([s].[Quantity] + 2) * 3");
        Assert.Equal("sqlserver:type:nvarchar", result.Mapping.TargetDataTypeId);
        Assert.Equal("CAST((([s].[Quantity] + 2) * 3) AS nvarchar(24))", Syntax.ExportScalarExpression(result.Expression));
        Assert.Single(result.Expression.Model.CastCallList);

        // Same mapping, different authored reusable snippet and bindings; no generator changes.
        model.ConversionImplementationList.Single(row => row.Name == "SqlExpression").SqlConversionSnippet =
            model.SqlConversionSnippetList.Single(row => row.Id == "sqlserver:snippet:convert");
        var changed = Generate(model, "sqlserver:type:int", new(Length: 24), "([s].[Quantity] + 2) * 3");
        Assert.Equal("CONVERT(nvarchar(24), (([s].[Quantity] + 2) * 3))", Syntax.ExportScalarExpression(changed.Expression));
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
        var model = CreateSnippetModel();
        model.DataTypeMappingList.Single(row => row.SourceDataTypeId == source).ConversionImplementation =
            model.ConversionImplementationList.Single(row => row.Name == "SqlExpression");
        var result = Generate(model, source, new(length, max, precision, scale));
        var sql = Syntax.ExportScalarExpression(result.Expression);
        Assert.Equal($"CAST(([s].[Value]) AS {expectedType})", sql);
        // Syntax survives the existing sanctioned importer/emitter, including type parameter order.
        Assert.Equal(sql, Syntax.ExportScalarExpression(Syntax.ImportScalarExpression(sql)));
    }

    [Fact]
    public void Direct_IsPassThrough_AndStructuralIsNeverACastFallback()
    {
        var model = CreateSnippetModel();
        Assert.Equal("[s].[Value]", Syntax.ExportScalarExpression(Generate(model, "meta:type:String", new()).Expression));
        Assert.Contains("Structural", Assert.Throws<InvalidOperationException>(() => Generate(model, "meta:type:Xml", new())).Message);
    }

    [Theory]
    [InlineData("TRY_CAST($(value) AS $(type))", "TRY_CAST(([s].[Value]) AS nvarchar(24))")]
    [InlineData("CONVERT($(type), $(value), 112)", "CONVERT(nvarchar(24), ([s].[Value]), 112)")]
    [InlineData("COALESCE(CONVERT($(type), $(value), 112), N'unknown')", "COALESCE(CONVERT(nvarchar(24), ([s].[Value]), 112), N'unknown')")]
    [InlineData("CAST($(value) * 2 AS $(type))", "CAST(([s].[Value]) * 2 AS nvarchar(24))")]
    public void AuthoredSnippetControlsFunctionsCompositionAndFixedArguments(string text, string expected)
    {
        var model = CreateSnippetModel();
        model.SqlConversionSnippetList.Single(row => row.Id == "sqlserver:snippet:cast").TransformScript = text;
        Assert.Equal(expected, Syntax.ExportScalarExpression(Generate(model, "sqlserver:type:int", new(Length: 24)).Expression));
        Assert.Equal("sqlserver:type:nvarchar", Service.Resolve(model, "sqlserver:type:int", "SqlServer").TargetDataTypeId);
        Assert.Single(Service.ResolveCompatibility(model, "sqlserver:type:int", "sqlserver:type:nvarchar").Path);
    }

    [Fact]
    public void MissingImplementationSnippet_AndDanglingSnippet_FailWithIdentities()
    {
        var model = CreateSnippetModel();
        var implementation = model.ConversionImplementationList.Single(row => row.Name == "SqlExpression");
        implementation.SqlConversionSnippet = null;
        Assert.Contains(implementation.Id, Assert.Throws<InvalidOperationException>(() => Generate(model, "sqlserver:type:int", new(Length: 24))).Message);
        implementation.SqlConversionSnippet = new() { Id = "missing-snippet" };
        Assert.Contains("missing-snippet", Assert.Throws<InvalidOperationException>(() => Generate(model, "sqlserver:type:int", new(Length: 24))).Message);
        var mapping = model.DataTypeMappingList.Single(row => row.Id.EndsWith(":int-to-nvarchar", StringComparison.Ordinal));
        mapping.ConversionImplementation = null!;
        Assert.Contains(mapping.Id, Assert.Throws<InvalidOperationException>(() => Generate(model, "sqlserver:type:int", new(Length: 24))).Message);
    }

    [Fact]
    public void MissingCatalogType_FailsClearly()
    {
        var model = CreateSnippetModel();
        Assert.Contains("sqlserver:type:nvarchar", Assert.Throws<InvalidOperationException>(() => Service.GenerateSqlExpression(
            model, MetaDataTypeModel.CreateEmpty(), new("sqlserver:type:int", "SqlServer", "1", new(Length: 24)))).Message);
    }

    [Fact]
    public void CompatibilityPath_IsNotAnExecutableConversionPlan()
    {
        var model = CreateSnippetModel();
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
        Assert.Throws<MetaTransformScriptSqlImportException>(() => Generate(CreateSnippetModel(), "sqlserver:type:int", new(Length: 24), input));
    }

    [Theory]
    [InlineData(null, false, null, null)]
    [InlineData(0, false, null, null)]
    [InlineData(4001, false, null, null)]
    [InlineData(24, true, null, null)]
    [InlineData(24, false, 19, 4)]
    public void MissingOrInapplicableFacets_DoNotInventDefaults(int? length, bool max, int? precision, int? scale)
    {
        Assert.Throws<ArgumentException>(() => Generate(CreateSnippetModel(), "sqlserver:type:int", new(length, max, precision, scale)));
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
        var model = CreateSnippetModel();
        model.DataTypeMappingList.Single(row => row.SourceDataTypeId == source).ConversionImplementation =
            model.ConversionImplementationList.Single(row => row.Name == "SqlExpression");
        Assert.Throws<ArgumentException>(() => Generate(model, source, new(Precision: precision, Scale: scale)));
    }

    [Fact]
    public void PlaceholderNamesAndBindingsAreAuthoredData_AndRepeatedUsesPreserveExpressionSemantics()
    {
        var model = CreateSnippetModel();
        var snippet = model.SqlConversionSnippetList.Single(row => row.Id == "sqlserver:snippet:cast");
        snippet.TransformScript = "CAST($( operand ) * 2 + $(operand) AS $(destination))";
        foreach (var binding in model.SqlConversionParameterBindingList.Where(row => row.SqlConversionSnippet == snippet))
            binding.Name = binding.Name == "value" ? "operand" : "destination";
        Assert.Equal("CAST((1 + 2) * 2 + (1 + 2) AS nvarchar(24))",
            Syntax.ExportScalarExpression(Generate(model, "sqlserver:type:int", new(Length: 24), "1 + 2").Expression));
    }

    [Fact]
    public void EscapedMarkersAndMarkersInsideBoundSql_AreNotExpandedAgain()
    {
        var model = CreateSnippetModel();
        model.SqlConversionSnippetList.Single(row => row.Id == "sqlserver:snippet:cast").TransformScript =
            "COALESCE(CAST($(value) AS $(type)), N'$$(literal)')";
        Assert.Equal("COALESCE(CAST((N'$(leave-alone)') AS nvarchar(24)), N'$(literal)')",
            Syntax.ExportScalarExpression(Generate(model, "sqlserver:type:int", new(Length: 24), "N'$(leave-alone)'").Expression));
    }

    [Theory]
    [InlineData("CAST($(missing) AS $(type))", "no binding")]
    [InlineData("CAST($(value AS $(type))", "invalid placeholder")]
    [InlineData("CAST($(value", "unclosed placeholder")]
    [InlineData("CAST(1 AS $(type))", "unused placeholder")]
    public void InvalidSnippetBindingsFailClearly(string text, string diagnostic)
    {
        var model = CreateSnippetModel();
        model.SqlConversionSnippetList.Single(row => row.Id == "sqlserver:snippet:cast").TransformScript = text;
        Assert.Contains(diagnostic, Assert.Throws<InvalidOperationException>(() => Generate(model, "sqlserver:type:int", new(Length: 24))).Message);
    }

    [Fact]
    public void DuplicateBindingsAndUnsupportedInputsFailClearly()
    {
        var model = CreateSnippetModel();
        var original = model.SqlConversionParameterBindingList.Single(row => row.SqlConversionSnippet.Id == "sqlserver:snippet:cast" && row.Name == "value");
        var duplicate = new SqlConversionParameterBinding
        {
            Id = "duplicate", Name = "VALUE", SqlConversionSnippet = original.SqlConversionSnippet,
            SqlConversionParameter = original.SqlConversionParameter
        };
        model.SqlConversionParameterBindingList.Add(duplicate);
        Assert.Contains("more than once", Assert.Throws<InvalidOperationException>(() => Generate(model, "sqlserver:type:int", new(Length: 24))).Message);
        model.SqlConversionParameterBindingList.Remove(duplicate);
        var unknown = new SqlConversionParameter { Id = "unknown-input", Name = "Unknown" };
        model.SqlConversionParameterList.Add(unknown);
        original.SqlConversionParameter = unknown;
        Assert.Contains("unknown-input", Assert.Throws<InvalidOperationException>(() => Generate(model, "sqlserver:type:int", new(Length: 24))).Message);
    }

    [Fact]
    public void NewImplementationAndSnippetAreUsableWithoutRegisteringGeneratorCode()
    {
        var model = CreateSnippetModel();
        var snippet = new SqlConversionSnippet
        {
            Id = "custom:format",
            TransformScript = "REPLACE(CONVERT($(destination), $(operand)), N'1', N'one')"
        };
        model.SqlConversionSnippetList.Add(snippet);
        var implementation = new ConversionImplementation { Id = "custom:implementation", Name = "Custom", SqlConversionSnippet = snippet };
        model.ConversionImplementationList.Add(implementation);
        model.DataTypeMappingList.Single(row => row.Id.EndsWith(":int-to-nvarchar", StringComparison.Ordinal)).ConversionImplementation = implementation;
        model.SqlConversionParameterBindingList.Add(new()
        {
            Id = "custom:operand", Name = "operand", SqlConversionSnippet = snippet,
            SqlConversionParameter = model.SqlConversionParameterList.Single(row => row.Name == "SourceExpression")
        });
        model.SqlConversionParameterBindingList.Add(new()
        {
            Id = "custom:destination", Name = "destination", SqlConversionSnippet = snippet,
            SqlConversionParameter = model.SqlConversionParameterList.Single(row => row.Name == "DestinationType")
        });
        Assert.Equal("REPLACE(CONVERT(nvarchar(24), (1)), N'1', N'one')",
            Syntax.ExportScalarExpression(Generate(model, "sqlserver:type:int", new(Length: 24), "1").Expression));
    }

    [Fact]
    public void SourceOnlyFixedTypeSnippet_DoesNotResolveUnusedDestinationInputs()
    {
        var model = CreateSnippetModel();
        var snippet = model.SqlConversionSnippetList.Single(row => row.Id == "sqlserver:snippet:cast");
        snippet.TransformScript = "CONVERT(nvarchar(8), $(value), 112)";
        model.SqlConversionParameterBindingList.RemoveAll(row => row.SqlConversionSnippet == snippet && row.Name == "type");
        // Missing target catalog entry and absent destination facets are irrelevant to this snippet.
        var result = Service.GenerateSqlExpression(model, MetaDataTypeModel.CreateEmpty(),
            new("sqlserver:type:int", "SqlServer", "@source", new()));
        Assert.Equal("CONVERT(nvarchar(8), (@source), 112)", Syntax.ExportScalarExpression(result.Expression));
    }

    [Fact]
    public void SourceOnlySnippet_CanTargetASeparateTypeSystem()
    {
        var model = CreateSnippetModel();
        var implementation = model.ConversionImplementationList.Single(row => row.Name == "SqlExpression");
        var snippet = implementation.SqlConversionSnippet!;
        snippet.TransformScript = "CAST($(value) AS nvarchar(25))";
        model.SqlConversionParameterBindingList.RemoveAll(row => row.SqlConversionSnippet == snippet && row.Name == "type");
        model.DataTypeMappingList.Add(new()
        {
            Id = "bdv:mapping:nvarchar", SourceDataTypeId = "sqlserver:type:nvarchar",
            TargetDataTypeId = "bdv:type:Text25", ConversionImplementation = implementation
        });

        var result = Service.GenerateSqlExpression(model, MetaDataTypeModel.CreateEmpty(),
            new("sqlserver:type:nvarchar", "bdv", "[source].[Name]", new()));

        Assert.Equal("bdv:type:Text25", result.Mapping.TargetDataTypeId);
        Assert.Equal("CAST(([source].[Name]) AS nvarchar(25))", Syntax.ExportScalarExpression(result.Expression));
    }

    [Fact]
    public void DestinationOnlySnippet_DoesNotParseUnusedSourceExpression()
    {
        var model = CreateSnippetModel();
        var snippet = model.SqlConversionSnippetList.Single(row => row.Id == "sqlserver:snippet:cast");
        snippet.TransformScript = "CAST(NULL AS $(type))";
        model.SqlConversionParameterBindingList.RemoveAll(row => row.SqlConversionSnippet == snippet && row.Name == "value");
        var result = Generate(model, "sqlserver:type:int", new(Length: 24), "unused; not a scalar expression");
        Assert.Equal("CAST(NULL AS nvarchar(24))", Syntax.ExportScalarExpression(result.Expression));
    }

    private static MetaDataTypeConversionModel CreateSnippetModel()
    {
        var model = MetaDataTypeConversionInstance.CreateBuiltIn();
        // Explicit test authoring, not a change to the built-in mapping used by existing BI operations.
        model.DataTypeMappingList.Single(row => row.Id.EndsWith(":int-to-nvarchar", StringComparison.Ordinal)).ConversionImplementation =
            model.ConversionImplementationList.Single(row => row.Name == "SqlExpression");
        return model;
    }

    private static SqlValueConversionResult Generate(MetaDataTypeConversionModel model, string source, SqlServerTypeFacets facets, string input = "[s].[Value]") =>
        Service.GenerateSqlExpression(model, MetaDataTypeInstance.BuiltIn, new(source, "SqlServer", input, facets));
}
