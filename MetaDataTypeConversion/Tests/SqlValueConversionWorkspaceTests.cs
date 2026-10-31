using Meta.Integration;
using MetaDataType;
using MetaDataTypeConversion.Core;
using MetaTransformScript.Sql;

namespace MetaDataTypeConversion.Tests;

public sealed class SqlValueConversionWorkspaceTests
{
    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task SnippetAndBindings_RoundTrip_AndSupportMetadataInstantiation(string representation)
    {
        var directory = Path.Combine(Path.GetTempPath(), "MetaDataTypeConversion-value-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var original = MetaDataTypeConversionInstance.CreateBuiltIn();
            original.DataTypeMappingList.Single(row => row.Id.EndsWith(":int-to-nvarchar", StringComparison.Ordinal)).ConversionImplementation =
                original.ConversionImplementationList.Single(row => row.Name == "SqlExpression");
            var castPath = Path.Combine(directory, "cast");
            await TypedWorkspaceModelMapper.CreateAsync(original, castPath, representation);
            var loaded = TypedWorkspaceModelMapper.Load<MetaDataTypeConversionModel>(castPath, searchUpward: false);
            var implementation = loaded.ConversionImplementationList.Single(row => row.Name == "SqlExpression");
            var snippet = loaded.SqlConversionSnippetList.Single(row => row.Id == "sqlserver:snippet:cast");
            Assert.Same(snippet, implementation.SqlConversionSnippet);
            var binding = loaded.SqlConversionParameterBindingList.Single(row => row.SqlConversionSnippet == snippet && row.Name == "value");
            Assert.Same(snippet, binding.SqlConversionSnippet);
            Assert.Same(loaded.SqlConversionParameterList.Single(row => row.Name == "SourceExpression"), binding.SqlConversionParameter);
            Assert.Same(implementation, loaded.DataTypeMappingList.Single(row => row.Id.EndsWith(":int-to-nvarchar", StringComparison.Ordinal)).ConversionImplementation);
            Assert.Equal("CAST($(value) AS $(type))", snippet.TransformScript);
            var first = Instantiate(loaded);
            Assert.Equal("CAST((([s].[Value] + 2) * 3) AS nvarchar(24))", first);

            // Author arbitrary expression composition and a fixed style argument in INSTANCE DATA.
            snippet.TransformScript = "COALESCE(CONVERT($(type), $(value), 112), N'unknown')";
            var convertPath = Path.Combine(directory, "convert");
            await TypedWorkspaceModelMapper.CreateAsync(loaded, convertPath, representation);
            var changed = TypedWorkspaceModelMapper.Load<MetaDataTypeConversionModel>(convertPath, searchUpward: false);
            Assert.Equal(snippet.TransformScript, changed.SqlConversionSnippetList.Single(row => row.Id == snippet.Id).TransformScript);
            var second = Instantiate(changed);
            Assert.Equal("COALESCE(CONVERT(nvarchar(24), (([s].[Value] + 2) * 3), 112), N'unknown')", second);

            // Carry the composed syntax through the same workspace representation too.
            var result = new MetaDataTypeConversionService().GenerateSqlExpression(changed, MetaDataTypeInstance.BuiltIn,
                new("sqlserver:type:int", "SqlServer", "([s].[Value] + 2) * 3", new(Length: 24)));
            var syntaxPath = Path.Combine(directory, "syntax");
            await TypedWorkspaceModelMapper.CreateAsync(result.Expression.Model, syntaxPath, representation);
            var syntaxModel = TypedWorkspaceModelMapper.Load<MetaTransformScript.MetaTransformScriptModel>(syntaxPath, searchUpward: false);
            var root = syntaxModel.ScalarExpressionList.Single(row => row.Id == result.Expression.Root.Id);
            Assert.Equal("COALESCE(CONVERT(nvarchar(24), (([s].[Value] + 2) * 3), 112), N'unknown')",
                new MetaTransformScriptSqlService().ExportScalarExpression(new(syntaxModel, root)));
        }
        finally
        {
            if (Directory.Exists(directory)) Directory.Delete(directory, recursive: true);
        }
    }

    private static string Instantiate(MetaDataTypeConversionModel model)
    {
        var result = new MetaDataTypeConversionService().GenerateSqlExpression(model, MetaDataTypeInstance.BuiltIn,
            new("sqlserver:type:int", "SqlServer", "([s].[Value] + 2) * 3", new(Length: 24)));
        return new MetaTransformScriptSqlService().ExportScalarExpression(result.Expression);
    }
}
