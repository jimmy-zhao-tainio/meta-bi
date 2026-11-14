using Meta.Integration;
using Meta.Operations;
using Meta.Operations.Domain;
using MetaBi.Tests.Common;
using MetaDataTypeConversion.Core;

namespace MetaDataTypeConversion.Tests;

public sealed class WorkspaceBindingIntegrationTests
{
    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task SelectedWorkspace_UsesSavedInstanceAndPreservesExactValues(string representation)
    {
        var root = NewRoot();
        try
        {
            var path = Path.Combine(root, "selected");
            var create = CliTestRunner.RunStandardCli("meta-data-type-conversion", $"create --{representation} \"{path}\"");
            Assert.Equal(0, create.ExitCode);
            var baseline = await TypedWorkspaceModelMapper.LoadStateAsync(path);
            var state = InMemoryOperations.ApplyBatch(baseline,
            [
                new Operation.InsertRecord("SqlConversionSnippet", "chosen/snippet",
                    new Dictionary<string, string> { ["TransformScript"] = "CONVERT($(type), REVERSE($(source)))" }),
                new Operation.InsertRecord("SqlConversionParameter", "chosen/source", new Dictionary<string, string> { ["Name"] = "SourceExpression" }),
                new Operation.InsertRecord("SqlConversionParameter", "chosen/type", new Dictionary<string, string> { ["Name"] = "DestinationType" }),
                new Operation.InsertRecord("SqlConversionParameterBinding", "chosen/bind-source", new Dictionary<string, string> { ["Name"] = "source" },
                    new Dictionary<string, string> { ["SqlConversionSnippetId"] = "chosen/snippet", ["SqlConversionParameterId"] = "chosen/source" }),
                new Operation.InsertRecord("SqlConversionParameterBinding", "chosen/bind-type", new Dictionary<string, string> { ["Name"] = "type" },
                    new Dictionary<string, string> { ["SqlConversionSnippetId"] = "chosen/snippet", ["SqlConversionParameterId"] = "chosen/type" }),
                new Operation.InsertRecord("ConversionImplementation", "chosen/implementation", new Dictionary<string, string> { ["Name"] = "Reverse", ["Description"] = " Åsa äter räksmörgås.\r\n\t " },
                    new Dictionary<string, string> { ["SqlConversionSnippetId"] = "chosen/snippet" }),
                new Operation.InsertRecord("DataTypeMapping", "chosen/mapping", new Dictionary<string, string> { ["SourceDataTypeId"] = "custom:type:Text", ["TargetDataTypeId"] = "sqlserver:type:int", ["Notes"] = "" },
                    new Dictionary<string, string> { ["ConversionImplementationId"] = "chosen/implementation" }),
            ]);
            var authored = Path.Combine(root, "authored");
            await WorkspaceSurface.CreateAsync(state, authored, representation);
            var first = MetaDataTypeConversionWorkspaceProvider.LoadOrDefault(authored);
            var mapping = first.DataTypeMappingList.Single(row => row.Id == "chosen/mapping");
            Assert.Same(first.ConversionImplementationList.Single(row => row.Id == "chosen/implementation"), mapping.ConversionImplementation);
            Assert.Equal(" Åsa äter räksmörgås.\r\n\t ", mapping.ConversionImplementation.Description);
            Assert.Equal("", mapping.Notes);
            Assert.Null(first.ConversionImplementationList.Single(row => row.Name == "Direct").SqlConversionSnippet);
            Assert.Equal("CONVERT(int, REVERSE(s.Code))", SqlConversionSnippetService.Instantiate(first, mapping, "s.Code", "int"));
            var check = CliTestRunner.RunStandardCli("meta-data-type-conversion", $"check --workspace \"{authored}\"");
            Assert.Equal(0, check.ExitCode);
            var resolve = CliTestRunner.RunStandardCli("meta-data-type-conversion", $"resolve --workspace \"{authored}\" --source-data-type custom:type:Text --target-data-type-system SqlServer");
            Assert.Equal(0, resolve.ExitCode);
            // Successful resolution of a source absent from the seed proves the CLI used this workspace.
            Assert.DoesNotContain(MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace().DataTypeMappingList,
                row => row.SourceDataTypeId == "custom:type:Text");
            Assert.Equal("Reverse", new MetaDataTypeConversionService().Resolve(first,
                "custom:type:Text", "SqlServer").ConversionImplementationName);

            await using (var workspace = await WorkspaceSurface.OpenAsync(authored))
                await workspace.ExecuteAsync([new Operation.SetProperty("SqlConversionSnippet", "chosen/snippet", "TransformScript", "TRY_CONVERT($(type), REVERSE($(source)))")]);

            var second = MetaDataTypeConversionWorkspaceProvider.LoadOrDefault(authored);
            var selected = second.DataTypeMappingList.Single(row => row.Id == mapping.Id);
            Assert.NotSame(first, second);
            Assert.NotSame(mapping.ConversionImplementation, selected.ConversionImplementation);
            Assert.Equal("TRY_CONVERT(int, REVERSE(s.Code))", SqlConversionSnippetService.Instantiate(second, selected, "s.Code", "int"));
            Assert.Equal("CONVERT(int, REVERSE(s.Code))", SqlConversionSnippetService.Instantiate(first, mapping, "s.Code", "int"));
            var saved = await TypedWorkspaceModelMapper.LoadStateAsync(authored);
            Assert.Null(InMemoryWorkspaceComparer.FindDifference(saved, new MetaDataTypeConversionWorkspaceBinding().Write(second, saved.Model)));
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Fact]
    public void DefaultWorkspace_LoadsFreshIndependentGraphs()
    {
        var first = MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace();
        var second = MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace();
        Assert.NotSame(first, second);
        Assert.NotSame(first.DataTypeMappingList[0], second.DataTypeMappingList[0]);
        var original = second.ConversionImplementationList[0].Name;
        first.ConversionImplementationList[0].Name = "changed only in first";
        Assert.Equal(original, second.ConversionImplementationList[0].Name);
        Assert.Equal(original, MetaDataTypeConversionWorkspaceProvider.GetDefaultWorkspace().ConversionImplementationList[0].Name);
    }

    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task SelectedWorkspace_ChangedContractFailsThroughProviderAndCli(string representation)
    {
        var root = NewRoot();
        try
        {
            var initial = await MetaDataTypeConversionWorkspaceProvider.LoadDefaultStateAsync();
            var changed = InMemoryOperations.Apply(initial, new Operation.AddProperty("DataTypeMapping", "Unexpected", false));
            var path = Path.Combine(root, "changed");
            await WorkspaceSurface.CreateAsync(changed, path, representation);
            Assert.Throws<InvalidOperationException>(() => MetaDataTypeConversionWorkspaceProvider.LoadOrDefault(path));
            var result = CliTestRunner.RunStandardCli("meta-data-type-conversion", $"check --workspace \"{path}\"");
            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("contract", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Null(InMemoryWorkspaceComparer.FindDifference(changed, await TypedWorkspaceModelMapper.LoadStateAsync(path)));
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    private static string NewRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "conversion-explicit-binding", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
