using Meta.Integration;
using MetaBi.Tests.Common;
using MetaDataType;
using MetaDataTypeConversion.Core;
using MetaTransformScript.Sql;

namespace MetaDataTypeConversion.Tests;

public sealed class SqlValueConversionWorkspaceTests
{
    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task SnippetAndBindings_RoundTrip_AndDriveProductionCli(string representation)
    {
        var directory = Path.Combine(Path.GetTempPath(), "MetaDataTypeConversion-value-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var original = MetaDataTypeConversionInstance.CreateBuiltIn();
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
            Assert.Equal("SqlServer", snippet.Dialect);
            var first = GenerateWithCli(castPath);
            Assert.Equal(0, first.ExitCode);
            Assert.Contains("CAST((([s].[Value] + 2) * 3) AS nvarchar(24))", first.Output);

            // Author arbitrary expression composition and a fixed style argument in INSTANCE DATA.
            snippet.SqlText = "COALESCE(CONVERT($(type), $(value), 112), N'unknown')";
            var convertPath = Path.Combine(directory, "convert");
            await TypedWorkspaceModelMapper.CreateAsync(loaded, convertPath, representation);
            var changed = TypedWorkspaceModelMapper.Load<MetaDataTypeConversionModel>(convertPath, searchUpward: false);
            Assert.Equal(snippet.SqlText, changed.SqlConversionSnippetList.Single(row => row.Id == snippet.Id).SqlText);
            var second = GenerateWithCli(convertPath);
            Assert.Equal(0, second.ExitCode);
            Assert.Contains("COALESCE(CONVERT(nvarchar(24), (([s].[Value] + 2) * 3), 112), N'unknown')", second.Output);
            Assert.DoesNotContain("CAST(", second.Output);

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

    [Fact]
    public void ProductionCli_ReportsRejectedGenerationAsFailure()
    {
        var workspace = Path.Combine(CliTestRunner.FindRepositoryRoot(), "MetaDataTypeConversion", "Workspace");
        var missingLength = CliTestRunner.RunStandardCli("meta-data-type-conversion",
            $"sql-expression --workspace \"{workspace}\" --source-data-type sqlserver:type:int --target-data-type-system SqlServer --input-expression 1");
        Assert.NotEqual(0, missingLength.ExitCode);
        Assert.Contains("explicit length", missingLength.Output);
        var structural = CliTestRunner.RunStandardCli("meta-data-type-conversion",
            $"sql-expression --workspace \"{workspace}\" --source-data-type meta:type:Xml --target-data-type-system SqlServer --input-expression 1");
        Assert.NotEqual(0, structural.ExitCode);
        Assert.Contains("Structural", structural.Output);
        var invalidInput = CliTestRunner.RunStandardCli("meta-data-type-conversion",
            $"sql-expression --workspace \"{workspace}\" --source-data-type sqlserver:type:int --target-data-type-system SqlServer --input-expression \"1; SELECT 2\" --length 24");
        Assert.Equal(4, invalidInput.ExitCode);
        Assert.Contains("Cannot generate SQL value conversion", invalidInput.Output);
        Assert.Contains("EndOfFile", invalidInput.Output);
    }

    private static (int ExitCode, string Output) GenerateWithCli(string workspace) =>
        CliTestRunner.RunStandardCli("meta-data-type-conversion",
            $"sql-expression --workspace \"{workspace}\" --source-data-type sqlserver:type:int --target-data-type-system SqlServer --input-expression \"([s].[Value] + 2) * 3\" --length 24");
}
