using Meta.Integration;
using Meta.Operations;
using Meta.Operations.Domain;
using MetaBi.Tests.Common;
using MetaDataType.Core;

namespace MetaDataType.Tests;

public sealed class WorkspaceBindingIntegrationTests
{
    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task SelectedWorkspace_UsesAuthoredArbitraryIdentitiesAndPreservesCanonicalReferences(string representation)
    {
        var root = NewRoot();
        try
        {
            var created = Path.Combine(root, "created");
            var create = CliTestRunner.RunStandardCli("meta-data-type", $"create --{representation} \"{created}\"");
            Assert.True(create.ExitCode == 0, create.Output);
            var contract = (await TypedWorkspaceModelMapper.LoadStateAsync(created)).Model;
            var state = CreateSelectedState(contract);
            var selected = Path.Combine(root, "selected");
            await WorkspaceSurface.CreateAsync(state, selected, representation);

            var first = MetaDataTypeWorkspaceProvider.LoadOrDefault(selected);
            var type = Assert.Single(first.DataTypeList);
            var system = Assert.Single(first.DataTypeSystemList);
            Assert.Equal("chosen/type/7", type.Id);
            Assert.Equal("chosen/system/4", system.Id);
            Assert.Equal("SqlServer", system.Name);
            Assert.Equal("nvarchar", type.Name);
            Assert.Same(system, type.DataTypeSystem);
            Assert.Equal(" Åsa äter räksmörgås.\r\n\t ", type.Description);
            Assert.Equal("", type.Category);
            Assert.Null(type.IsCanonical);
            var saved = await TypedWorkspaceModelMapper.LoadStateAsync(selected);
            Assert.Null(InMemoryWorkspaceComparer.FindDifference(saved,
                new MetaDataTypeWorkspaceBinding().Write(first, saved.Model)));

            await using (var workspace = await WorkspaceSurface.OpenAsync(selected))
                await workspace.ExecuteAsync([
                    new Operation.SetProperty("DataType", type.Id, "Description", "changed after persistence"),
                ]);

            var second = MetaDataTypeWorkspaceProvider.LoadOrDefault(selected);
            var changedType = Assert.Single(second.DataTypeList);
            Assert.NotSame(first, second);
            Assert.NotSame(type, changedType);
            Assert.Equal("changed after persistence", changedType.Description);
            Assert.Equal(" Åsa äter räksmörgås.\r\n\t ", type.Description);
            Assert.Same(Assert.Single(second.DataTypeSystemList), changedType.DataTypeSystem);
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Fact]
    public async Task CliCreate_UsesEditedPackagedWorkspaceWithoutRebuilding()
    {
        var root = NewRoot();
        try
        {
            var install = Path.Combine(root, "installation");
            Directory.CreateDirectory(install);
            foreach (var file in Directory.GetFiles(AppContext.BaseDirectory, "*.dll"))
                File.Copy(file, Path.Combine(install, Path.GetFileName(file)));
            foreach (var suffix in new[] { ".deps.json", ".runtimeconfig.json" })
                File.Copy(Path.Combine(AppContext.BaseDirectory, "meta-data-type" + suffix),
                    Path.Combine(install, "meta-data-type" + suffix));
            CopyDirectory(Path.Combine(AppContext.BaseDirectory, "meta-data-type.MetaCli"),
                Path.Combine(install, "meta-data-type.MetaCli"));
            CopyDirectory(Path.Combine(AppContext.BaseDirectory, "runtimes"),
                Path.Combine(install, "runtimes"));
            var defaults = await MetaDataTypeWorkspaceProvider.LoadDefaultStateAsync();
            var selected = CreateSelectedState(defaults.Model);
            await WorkspaceSurface.CreateAsync(selected, Path.Combine(install, "Workspaces", "MetaDataType"), "csharp");

            var output = Path.Combine(root, "created");
            var create = CliTestRunner.RunStandardCli(Path.Combine(install, "meta-data-type"), $"create --xml \"{output}\"");

            Assert.True(create.ExitCode == 0, create.Output);
            var saved = await TypedWorkspaceModelMapper.LoadStateAsync(output);
            Assert.Null(InMemoryWorkspaceComparer.FindDifference(selected, saved));
            var typed = MetaDataTypeWorkspaceProvider.LoadOrDefault(output);
            Assert.Same(Assert.Single(typed.DataTypeSystemList), Assert.Single(typed.DataTypeList).DataTypeSystem);
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Theory]
    [InlineData("xml")]
    [InlineData("csharp")]
    public async Task SelectedWorkspace_ChangedContractFailsWithoutChangingTheWorkspace(string representation)
    {
        var root = NewRoot();
        try
        {
            var seed = await MetaDataTypeWorkspaceProvider.LoadDefaultStateAsync();
            var changed = InMemoryOperations.Apply(seed, new Operation.AddProperty("DataType", "Unexpected", false));
            var path = Path.Combine(root, "changed");
            await WorkspaceSurface.CreateAsync(changed, path, representation);

            var exception = Assert.Throws<InvalidOperationException>(() => MetaDataTypeWorkspaceProvider.LoadOrDefault(path));

            Assert.Contains("contract", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Null(InMemoryWorkspaceComparer.FindDifference(changed, await TypedWorkspaceModelMapper.LoadStateAsync(path)));
        }
        finally { Directory.Delete(root, recursive: true); }
    }

    [Fact]
    public void DefaultProviderAndStartingData_ReturnFreshIndependentGraphs()
    {
        var first = MetaDataTypeWorkspaceProvider.GetDefaultWorkspace();
        var second = MetaDataTypeWorkspaceProvider.GetDefaultWorkspace();
        Assert.NotSame(first, second);
        Assert.NotSame(first.DataTypeList[0], second.DataTypeList[0]);
        var original = second.DataTypeList[0].Name;
        first.DataTypeList[0].Name = "only changed in first";
        Assert.Equal(original, second.DataTypeList[0].Name);
        Assert.Equal(original, MetaDataTypeWorkspaceProvider.GetDefaultWorkspace().DataTypeList[0].Name);

        var service = new MetaDataTypeWorkspaceService();
        var firstSeed = service.CreateWorkspace();
        var secondSeed = service.CreateWorkspace();
        Assert.NotSame(firstSeed, secondSeed);
        Assert.NotSame(firstSeed.DataTypeList[0], secondSeed.DataTypeList[0]);
        firstSeed.DataTypeList[0].Name = "only changed in first seed";
        Assert.Equal(original, secondSeed.DataTypeList[0].Name);
    }

    private static InMemoryWorkspace CreateSelectedState(GenericModel model) =>
        InMemoryOperations.ApplyBatch(new InMemoryWorkspace(model,
            new GenericInstance { ModelName = model.Name }),
        [
            new Operation.InsertRecord("DataTypeSystem", "chosen/system/4",
                new Dictionary<string, string> { ["Name"] = "SqlServer", ["Description"] = "" }),
            new Operation.InsertRecord("DataType", "chosen/type/7",
                new Dictionary<string, string> { ["Name"] = "nvarchar", ["Category"] = "", ["Description"] = " Åsa äter räksmörgås.\r\n\t " },
                new Dictionary<string, string> { ["DataTypeSystemId"] = "chosen/system/4" }),
        ]);

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.GetFiles(source))
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)));
        foreach (var directory in Directory.GetDirectories(source))
            CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
    }

    private static string NewRoot()
    {
        var path = Path.Combine(Path.GetTempPath(), "catalog-explicit-binding", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
