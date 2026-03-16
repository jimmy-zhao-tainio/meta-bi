using Meta.Core.Services;

namespace MetaDataVault.Tests;

public sealed partial class CliTests
{
    [Fact]
    public void CheckBusinessMaterialization_SucceedsForRepeatedKeyPartSamples()
    {
        var repoRoot = FindRepositoryRoot();
        var businessPath = Path.Combine(repoRoot, "MetaBusiness.Workspaces", "SampleBusinessCommerceRepeatedKeyPart");
        var bdvPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceRepeatedKeyPart");
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var hubObjectWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce-RepeatedKeyPart");
        var hubKeyPartWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce");
        var linkRelationshipWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce-RepeatedKeyPart");
        var linkEndWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce-RepeatedKeyPart");
        var hubKeyPartFabricPath = Path.Combine(repoRoot, "Fabrics", "Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce");
        var linkEndFabricPath = Path.Combine(repoRoot, "Fabrics", "Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce-RepeatedKeyPart");

        var result = RunBusinessCli(
            $"check-business-materialization --business-workspace \"{businessPath}\" --bdv-workspace \"{bdvPath}\" --implementation-workspace \"{implementationPath}\" --weave-workspace \"{hubObjectWeavePath}\" --weave-workspace \"{hubKeyPartWeavePath}\" --weave-workspace \"{linkRelationshipWeavePath}\" --weave-workspace \"{linkEndWeavePath}\" --fabric-workspace \"{hubKeyPartFabricPath}\" --fabric-workspace \"{linkEndFabricPath}\"");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("OK: business datavault materialization contract", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("FlatAnchors: 2/2", result.Output);
        Assert.Contains("ScopedAnchors: 2/2", result.Output);
    }

    [Fact]
    public async Task MaterializeBusiness_PhysicalizesBusinessDataVaultNames()
    {
        var repoRoot = FindRepositoryRoot();
        var businessPath = Path.Combine(repoRoot, "MetaBusiness.Workspaces", "SampleBusinessCommerceRepeatedKeyPart");
        var bdvPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceRepeatedKeyPart");
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var hubObjectWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce-RepeatedKeyPart");
        var hubKeyPartWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce");
        var linkRelationshipWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce-RepeatedKeyPart");
        var linkEndWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce-RepeatedKeyPart");
        var hubKeyPartFabricPath = Path.Combine(repoRoot, "Fabrics", "Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce");
        var linkEndFabricPath = Path.Combine(repoRoot, "Fabrics", "Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkHubParticipant-Commerce-RepeatedKeyPart");
        var outputPath = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"), "MaterializedBusinessDataVault");

        try
        {
            var result = RunBusinessCli(
                $"materialize-business --business-workspace \"{businessPath}\" --bdv-workspace \"{bdvPath}\" --implementation-workspace \"{implementationPath}\" --weave-workspace \"{hubObjectWeavePath}\" --weave-workspace \"{hubKeyPartWeavePath}\" --weave-workspace \"{linkRelationshipWeavePath}\" --weave-workspace \"{linkEndWeavePath}\" --fabric-workspace \"{hubKeyPartFabricPath}\" --fabric-workspace \"{linkEndFabricPath}\" --new-workspace \"{outputPath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("OK: business datavault materialized", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("MaterializedTables: 5", result.Output);

            var materializedWorkspace = await new WorkspaceService().LoadAsync(outputPath, searchUpward: false);
            var hubs = materializedWorkspace.Instance.GetOrCreateEntityRecords("BusinessHub").ToDictionary(record => record.Id, StringComparer.Ordinal);
            var links = materializedWorkspace.Instance.GetOrCreateEntityRecords("BusinessLink").ToDictionary(record => record.Id, StringComparer.Ordinal);
            var hubKeyParts = materializedWorkspace.Instance.GetOrCreateEntityRecords("BusinessHubKeyPart").ToDictionary(record => record.Id, StringComparer.Ordinal);
            var hubKeyPartDetails = materializedWorkspace.Instance.GetOrCreateEntityRecords("BusinessHubKeyPartDataTypeDetail").ToDictionary(record => record.Id, StringComparer.Ordinal);

            Assert.Equal("BH_Customer", hubs["Customer"].Values["Name"]);
            Assert.Equal("BH_Invoice", hubs["Invoice"].Values["Name"]);
            Assert.Equal("BL_CustomerOrder", links["CustomerOrder"].Values["Name"]);
            Assert.Equal("BL_CustomerInvoice", links["CustomerInvoice"].Values["Name"]);
            Assert.Equal("meta:type:String", hubKeyParts["CustomerIdentifier"].Values["DataTypeId"]);
            Assert.Equal("meta:type:String", hubKeyParts["OrderIdentifier"].Values["DataTypeId"]);
            Assert.Equal("Length", hubKeyPartDetails["CustomerIdentifierLength"].Values["Name"]);
            Assert.Equal("50", hubKeyPartDetails["CustomerIdentifierLength"].Values["Value"]);
        }
        finally
        {
            DeleteDirectoryIfExists(Path.GetDirectoryName(outputPath)!);
        }
    }
}
