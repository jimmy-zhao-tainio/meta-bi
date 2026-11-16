using Meta.Integration;
using Meta.Operations.Domain;
using MetaConvert.DataVaultToSql;
using MetaDataType;
using MetaDataType.Core;
using MetaDataTypeConversion;
using MetaDataTypeConversion.Core;

namespace MetaDataVault.Tests;

public sealed class SelectedConversionWorkspaceTests
{
    [Theory]
    [InlineData("Demos/RawDataVaultCliIntegration/Workspace")]
    [InlineData("MetaDataVault/Workspaces/SampleBusinessDataVaultCommerceHelpers")]
    public async Task CatalogIdentityChangesPreserveCompletePhysicalProjection(string relativeSourcePath)
    {
        var directory = Path.Combine(Path.GetTempPath(), "selected-vault-types", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var catalogState = await MetaDataTypeWorkspaceProvider.LoadDefaultStateAsync();
            var catalogBinding = new MetaDataTypeWorkspaceBinding();
            var catalog = catalogBinding.Read(catalogState);
            catalog.DataTypeSystemList.Single(row => row.Name == "Meta").Id = "selected:system:business";
            catalog.DataTypeSystemList.Single(row => row.Name == "SqlServer").Id = "selected:system:sql";
            var mappingState = await MetaDataTypeConversionWorkspaceProvider.LoadDefaultStateAsync();
            var mappingBinding = new MetaDataTypeConversionWorkspaceBinding();
            var mappings = mappingBinding.Read(mappingState);
            mappings.ConversionImplementationList.Single(row => row.Name == "Direct").Id = "selected:implementation:direct";
            var catalogPath = Path.Combine(directory, "catalog");
            var mappingPath = Path.Combine(directory, "mappings");
            await WorkspaceSurface.CreateAsync(catalogBinding.Write(catalog, catalogState.Model), catalogPath, "xml");
            await WorkspaceSurface.CreateAsync(mappingBinding.Write(mappings, mappingState.Model), mappingPath, "xml");

            var repository = CliTestSupport.FindRepositoryRoot();
            var sourcePath = Path.Combine(repository, relativeSourcePath.Replace('/', Path.DirectorySeparatorChar));
            var implementation = Path.Combine(repository, "MetaDataVault", "Workspaces", "MetaDataVaultImplementation");
            var expected = await Converter.ConvertAsync(sourcePath, implementation, "SelectedVault");
            var actual = await Converter.ConvertAsync(sourcePath, implementation, "SelectedVault",
                dataTypeWorkspacePath: catalogPath, dataTypeConversionWorkspacePath: mappingPath);

            Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, actual));
            Assert.NotEmpty(actual.Instance.GetOrCreateEntityRecords("TableColumn"));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
