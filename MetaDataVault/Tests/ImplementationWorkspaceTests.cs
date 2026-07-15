using MetaDataVaultImplementation;

namespace MetaDataVault.Tests;

public sealed class ImplementationWorkspaceTests
{
    [Fact]
    public async Task DefaultImplementationUsesThirtyTwoByteHashStorage()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "MetaDataVaultImplementation");
        var model = await MetaDataVaultImplementationModel.LoadFromXmlWorkspaceAsync(workspacePath, searchUpward: false);

        Assert.Equal("32", model.RawHubImplementationList.Single().HashKeyLength);
        Assert.Equal("32", model.RawHubSatelliteImplementationList.Single().ParentHashKeyLength);
        Assert.Equal("32", model.RawLinkImplementationList.Single().HashKeyLength);
        Assert.Equal("32", model.RawLinkSatelliteImplementationList.Single().ParentHashKeyLength);

        Assert.Equal("32", model.BusinessHubImplementationList.Single().HashKeyLength);
        Assert.Equal("32", model.BusinessHubSatelliteImplementationList.Single().ParentHashKeyLength);
        Assert.Equal("32", model.BusinessLinkImplementationList.Single().HashKeyLength);
        Assert.Equal("32", model.BusinessLinkSatelliteImplementationList.Single().ParentHashKeyLength);
        Assert.Equal("32", model.BusinessReferenceImplementationList.Single().HashKeyLength);
        Assert.Equal("32", model.BusinessReferenceSatelliteImplementationList.Single().ParentHashKeyLength);
        Assert.Equal("32", model.BusinessSameAsLinkImplementationList.Single().HashKeyLength);
        Assert.Equal("32", model.BusinessSameAsLinkSatelliteImplementationList.Single().ParentHashKeyLength);
        Assert.Equal("32", model.BusinessHierarchicalLinkImplementationList.Single().HashKeyLength);
        Assert.Equal("32", model.BusinessHierarchicalLinkSatelliteImplementationList.Single().ParentHashKeyLength);
        Assert.Equal("32", model.BusinessPointInTimeImplementationList.Single().ParentHashKeyLength);
        Assert.Equal("32", model.BusinessBridgeImplementationList.Single().RootHashKeyLength);
        Assert.Equal("32", model.BusinessBridgeImplementationList.Single().RelatedHashKeyLength);
    }
}
