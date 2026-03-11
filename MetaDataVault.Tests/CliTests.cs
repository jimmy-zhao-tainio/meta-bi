using System.Diagnostics;
using Meta.Core.Services;
using MetaSchema.Core;

namespace MetaDataVault.Tests;

public sealed class CliTests
{
    [Fact]
    public async Task BusinessAuthoringCommands_CoverAllAddCommands()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            var createResult = RunBusinessCli($"--new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            RunBusinessAdd(workspacePath, "add-hub --id Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-hub --id Order --name Order");
            RunBusinessAdd(workspacePath, "add-hub --id CustomerAlias --name CustomerAlias");
            RunBusinessAdd(workspacePath, "add-hub --id ParentNode --name ParentNode");
            RunBusinessAdd(workspacePath, "add-hub --id ChildNode --name ChildNode");

            RunBusinessAdd(workspacePath, "add-hub-key-part --id CustomerIdentifier --hub Customer --name Identifier --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-hub-key-part-data-type-detail --id CustomerIdentifierLength --hub-key-part CustomerIdentifier --name Length --value 50");
            RunBusinessAdd(workspacePath, "add-hub-key-part --id OrderIdentifier --hub Order --name Identifier --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-hub-key-part --id CustomerAliasIdentifier --hub CustomerAlias --name Identifier --data-type-id meta:type:String --ordinal 1");

            RunBusinessAdd(workspacePath, "add-link --id CustomerOrder --name CustomerOrder");
            RunBusinessAdd(workspacePath, "add-link-hub --id CustomerOrderCustomer --link CustomerOrder --hub Customer --ordinal 1 --role-name Customer");
            RunBusinessAdd(workspacePath, "add-link-hub --id CustomerOrderOrder --link CustomerOrder --hub Order --ordinal 2 --role-name Order");

            RunBusinessAdd(workspacePath, "add-same-as-link --id CustomerSameAsAlias --name CustomerSameAsAlias --primary-hub Customer --equivalent-hub CustomerAlias");
            RunBusinessAdd(workspacePath, "add-hierarchical-link --id ParentChild --name ParentChild --parent-hub ParentNode --child-hub ChildNode");

            RunBusinessAdd(workspacePath, "add-reference --id StatusCode --name StatusCode");
            RunBusinessAdd(workspacePath, "add-reference-key-part --id StatusCodeValue --reference StatusCode --name Code --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-reference-key-part-data-type-detail --id StatusCodeValueLength --reference-key-part StatusCodeValue --name Length --value 20");

            RunBusinessAdd(workspacePath, "add-hub-satellite --id CustomerProfile --hub Customer --name CustomerProfile --satellite-kind standard");
            RunBusinessAdd(workspacePath, "add-hub-satellite-key-part --id CustomerProfileVersion --hub-satellite CustomerProfile --name VersionId --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-hub-satellite-key-part-data-type-detail --id CustomerProfileVersionLength --hub-satellite-key-part CustomerProfileVersion --name Length --value 10");
            RunBusinessAdd(workspacePath, "add-hub-satellite-attribute --id CustomerName --hub-satellite CustomerProfile --name CustomerName --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-hub-satellite-attribute-data-type-detail --id CustomerNameLength --hub-satellite-attribute CustomerName --name Length --value 200");

            RunBusinessAdd(workspacePath, "add-link-satellite --id CustomerOrderStatus --link CustomerOrder --name CustomerOrderStatus --satellite-kind standard");
            RunBusinessAdd(workspacePath, "add-link-satellite-key-part --id CustomerOrderStatusVersion --link-satellite CustomerOrderStatus --name VersionId --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-link-satellite-key-part-data-type-detail --id CustomerOrderStatusVersionLength --link-satellite-key-part CustomerOrderStatusVersion --name Length --value 10");
            RunBusinessAdd(workspacePath, "add-link-satellite-attribute --id CustomerOrderStatusCode --link-satellite CustomerOrderStatus --name StatusCode --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-link-satellite-attribute-data-type-detail --id CustomerOrderStatusCodeLength --link-satellite-attribute CustomerOrderStatusCode --name Length --value 20");

            RunBusinessAdd(workspacePath, "add-same-as-link-satellite --id CustomerSameAsAliasAudit --same-as-link CustomerSameAsAlias --name CustomerSameAsAliasAudit --satellite-kind standard");
            RunBusinessAdd(workspacePath, "add-same-as-link-satellite-key-part --id CustomerSameAsAliasAuditVersion --same-as-link-satellite CustomerSameAsAliasAudit --name VersionId --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-same-as-link-satellite-key-part-data-type-detail --id CustomerSameAsAliasAuditVersionLength --same-as-link-satellite-key-part CustomerSameAsAliasAuditVersion --name Length --value 10");
            RunBusinessAdd(workspacePath, "add-same-as-link-satellite-attribute --id CustomerSameAsAliasReason --same-as-link-satellite CustomerSameAsAliasAudit --name ReasonCode --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-same-as-link-satellite-attribute-data-type-detail --id CustomerSameAsAliasReasonLength --same-as-link-satellite-attribute CustomerSameAsAliasReason --name Length --value 20");

            RunBusinessAdd(workspacePath, "add-hierarchical-link-satellite --id ParentChildAudit --hierarchical-link ParentChild --name ParentChildAudit --satellite-kind standard");
            RunBusinessAdd(workspacePath, "add-hierarchical-link-satellite-key-part --id ParentChildAuditVersion --hierarchical-link-satellite ParentChildAudit --name VersionId --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-hierarchical-link-satellite-key-part-data-type-detail --id ParentChildAuditVersionLength --hierarchical-link-satellite-key-part ParentChildAuditVersion --name Length --value 10");
            RunBusinessAdd(workspacePath, "add-hierarchical-link-satellite-attribute --id ParentChildRelationshipType --hierarchical-link-satellite ParentChildAudit --name RelationshipType --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-hierarchical-link-satellite-attribute-data-type-detail --id ParentChildRelationshipTypeLength --hierarchical-link-satellite-attribute ParentChildRelationshipType --name Length --value 30");

            RunBusinessAdd(workspacePath, "add-reference-satellite --id StatusCodeDescriptionSet --reference StatusCode --name StatusCodeDescriptionSet --satellite-kind standard");
            RunBusinessAdd(workspacePath, "add-reference-satellite-key-part --id StatusCodeDescriptionVersion --reference-satellite StatusCodeDescriptionSet --name VersionId --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-reference-satellite-key-part-data-type-detail --id StatusCodeDescriptionVersionLength --reference-satellite-key-part StatusCodeDescriptionVersion --name Length --value 10");
            RunBusinessAdd(workspacePath, "add-reference-satellite-attribute --id StatusCodeDescription --reference-satellite StatusCodeDescriptionSet --name Description --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-reference-satellite-attribute-data-type-detail --id StatusCodeDescriptionLength --reference-satellite-attribute StatusCodeDescription --name Length --value 100");

            RunBusinessAdd(workspacePath, "add-point-in-time --id CustomerSnapshot --hub Customer --name CustomerSnapshot");
            RunBusinessAdd(workspacePath, "add-point-in-time-stamp --id CustomerSnapshotBusinessDate --point-in-time CustomerSnapshot --name BusinessDate --data-type-id meta:type:DateTime --ordinal 1");
            RunBusinessAdd(workspacePath, "add-point-in-time-stamp-data-type-detail --id CustomerSnapshotBusinessDatePrecision --point-in-time-stamp CustomerSnapshotBusinessDate --name Precision --value 7");
            RunBusinessAdd(workspacePath, "add-point-in-time-hub-satellite --id CustomerSnapshotProfile --point-in-time CustomerSnapshot --hub-satellite CustomerProfile --ordinal 1");
            RunBusinessAdd(workspacePath, "add-point-in-time-link-satellite --id CustomerSnapshotOrderStatus --point-in-time CustomerSnapshot --link-satellite CustomerOrderStatus --ordinal 2");

            RunBusinessAdd(workspacePath, "add-bridge --id CustomerOrderTraversal --anchor-hub Customer --name CustomerOrderTraversal");
            RunBusinessAdd(workspacePath, "add-bridge-hub --id CustomerOrderTraversalCustomer --bridge CustomerOrderTraversal --hub Customer --ordinal 1 --role-name Customer");
            RunBusinessAdd(workspacePath, "add-bridge-hub --id CustomerOrderTraversalOrder --bridge CustomerOrderTraversal --hub Order --ordinal 2 --role-name Order");
            RunBusinessAdd(workspacePath, "add-bridge-link --id CustomerOrderTraversalLink --bridge CustomerOrderTraversal --link CustomerOrder --ordinal 1 --role-name CustomerOrder");
            RunBusinessAdd(workspacePath, "add-bridge-hub-key-part-projection --id CustomerOrderTraversalCustomerIdentifier --bridge CustomerOrderTraversal --hub-key-part CustomerIdentifier --name CustomerIdentifier --ordinal 1");
            RunBusinessAdd(workspacePath, "add-bridge-hub-satellite-attribute-projection --id CustomerOrderTraversalCustomerName --bridge CustomerOrderTraversal --hub-satellite-attribute CustomerName --name CustomerName --ordinal 2");
            RunBusinessAdd(workspacePath, "add-bridge-link-satellite-attribute-projection --id CustomerOrderTraversalStatusCode --bridge CustomerOrderTraversal --link-satellite-attribute CustomerOrderStatusCode --name StatusCode --ordinal 3");

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("BusinessBridge"));
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("BusinessPointInTime"));
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("BusinessSameAsLink"));
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("BusinessHierarchicalLink"));
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("BusinessReference"));
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }
    [Fact]
    public async Task NewWorkspace_CreatesMetaBusinessDataVaultWorkspace()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            var result = RunBusinessCli($"--new-workspace \"{workspacePath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("OK: metabusinessdatavault workspace created", result.Output, StringComparison.OrdinalIgnoreCase);

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            Assert.Equal("MetaBusinessDataVault", workspace.Model.Name);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task NewWorkspace_CreatesMetaRawDataVaultWorkspace()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");

        try
        {
            var result = RunRawCli($"--new-workspace \"{workspacePath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("OK: metarawdatavault workspace created", result.Output, StringComparison.OrdinalIgnoreCase);

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            Assert.Equal("MetaRawDataVault", workspace.Model.Name);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public void Help_ShowsFromMetaSchemaCommand()
    {
        var result = RunRawCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-datavault-raw", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("from-metaschema", result.Output);
        Assert.DoesNotContain("check-business-materialization", result.Output);
        Assert.DoesNotContain("generate-sql", result.Output);
    }


    [Fact]
    public void FromMetaSchema_Help_ShowsRequiredOptions()
    {
        var result = RunRawCli("from-metaschema --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--source-workspace <path>", result.Output);
        Assert.Contains("--business-workspace <path>", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--new-workspace <path>", result.Output);
        Assert.Contains("MetaBusiness", result.Output);
        Assert.Contains("MetaDataVaultImplementation", result.Output);
    }

    [Fact]
    public void CheckBusinessMaterialization_Help_ShowsRequiredOptions()
    {
        var result = RunBusinessCli("check-business-materialization --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--business-workspace <path>", result.Output);
        Assert.Contains("--bdv-workspace", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--weave-workspace <path>", result.Output);
        Assert.Contains("--fabric-workspace <path>", result.Output);
        Assert.Contains("MetaBusinessDataVault", result.Output);
        Assert.Contains("MetaFabric", result.Output);
    }

    [Fact]
    public void MaterializeBusiness_Help_ShowsRequiredOptions()
    {
        var result = RunBusinessCli("materialize-business --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--business-workspace <path>", result.Output);
        Assert.Contains("--bdv-workspace", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--weave-workspace <path>", result.Output);
        Assert.Contains("--fabric-workspace <path>", result.Output);
        Assert.Contains("--new-workspace <path>", result.Output);
        Assert.Contains("table name patterns", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GenerateSql_Help_ShowsRequiredOptions()
    {
        var result = RunBusinessCli("generate-sql --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace <path>", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--data-type-conversion-workspace <path>", result.Output);
        Assert.Contains("--out <path>", result.Output);
        Assert.Contains("hubs, links, same-as links, hierarchical links, references, satellites, point-in-time tables, and bridges", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FromMetaSchema_FailsWhenRequiredSanctionedWorkspacesAreMissing()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(root, "MetaSchemaSource");
        var targetPath = Path.Combine(root, "RawDataVault");

        try
        {
            Directory.CreateDirectory(sourcePath);
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace(sourcePath);
            SeedMetaSchema(source);
            await new WorkspaceService().SaveAsync(source);

            var result = RunRawCli($"from-metaschema --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\"");
            Assert.Equal(1, result.ExitCode);
            Assert.Contains("missing required option --business-workspace <path>", result.Output);
            Assert.False(File.Exists(Path.Combine(targetPath, "workspace.xml")));
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task FromMetaSchema_FailsUntilWeaveDrivenMaterializationExists()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(root, "MetaSchemaSource");
        var businessPath = Path.Combine(root, "MetaBusiness");
        var implementationPath = Path.Combine(root, "MetaDataVaultImplementation");
        var targetPath = Path.Combine(root, "RawDataVault");

        try
        {
            Directory.CreateDirectory(sourcePath);
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace(sourcePath);
            SeedMetaSchema(source);
            await new WorkspaceService().SaveAsync(source);

            var repoRoot = FindRepositoryRoot();
            CopyDirectory(Path.Combine(repoRoot, "MetaBusiness.Workspaces", "MetaBusiness"), businessPath);
            CopyDirectory(Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation"), implementationPath);

            var result = RunRawCli(
                $"from-metaschema --source-workspace \"{sourcePath}\" --business-workspace \"{businessPath}\" --implementation-workspace \"{implementationPath}\" --new-workspace \"{targetPath}\"");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("could not materialize raw datavault from sanctioned inputs", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("MetaBusiness", result.Output);
            Assert.Contains("MetaDataVaultImplementation", result.Output);
            Assert.Contains("weave bindings", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.False(File.Exists(Path.Combine(targetPath, "workspace.xml")));
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }


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

        [Fact]
    public async Task GenerateSql_EmitsHubLinkAndSatelliteScripts()
    {
        var repoRoot = FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceRepeatedKeyPart");
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            var result = RunBusinessCli(
                $"generate-sql --workspace \"{workspacePath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("OK: business datavault sql generated", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Files: 5", result.Output);

            var customerHubSql = await File.ReadAllTextAsync(Path.Combine(sqlOutputPath, "BH_Customer.sql"));
            Assert.Contains("CREATE TABLE [dbo].[BH_Customer]", customerHubSql);
            Assert.Contains("[HashKey] binary(16) NOT NULL", customerHubSql);
            Assert.Contains("[Identifier] nvarchar(50) NOT NULL", customerHubSql);
            Assert.Contains("[LoadTimestamp] datetime2(7) NOT NULL", customerHubSql);
            Assert.Contains("[RecordSource] nvarchar(256) NOT NULL", customerHubSql);
            Assert.Contains("[AuditId] int NOT NULL", customerHubSql);

            var customerOrderLinkSql = await File.ReadAllTextAsync(Path.Combine(sqlOutputPath, "BL_CustomerOrder.sql"));
            Assert.Contains("CREATE TABLE [dbo].[BL_CustomerOrder]", customerOrderLinkSql);
            Assert.Contains("[CustomerHashKey] binary(16) NOT NULL", customerOrderLinkSql);
            Assert.Contains("[OrderHashKey] binary(16) NOT NULL", customerOrderLinkSql);
            Assert.Contains("[AuditId] int NOT NULL", customerOrderLinkSql);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }
[Fact]
    public async Task GenerateSql_EmitsPointInTimeAndBridgeScripts()
    {
        var repoRoot = FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            var result = RunBusinessCli(
                $"generate-sql --workspace \"{workspacePath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("OK: business datavault sql generated", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Files: 11", result.Output);
            Assert.Contains("BusinessReferences: 1", result.Output);
            Assert.Contains("BusinessReferenceSatellites: 1", result.Output);
            Assert.Contains("BusinessPointInTimes: 1", result.Output);
            Assert.Contains("BusinessBridges: 1", result.Output);

            var pitSql = await File.ReadAllTextAsync(Path.Combine(sqlOutputPath, "PIT_CustomerSnapshot.sql"));
            Assert.Contains("CREATE TABLE [dbo].[PIT_CustomerSnapshot]", pitSql);
            Assert.Contains("[HubHashKey] binary(16) NOT NULL", pitSql);
            Assert.Contains("[SnapshotTimestamp] datetime2(7) NOT NULL", pitSql);
            Assert.Contains("[ProfileLoadTimestamp] datetime2(7) NOT NULL", pitSql);
            Assert.Contains("[StatusLoadTimestamp] datetime2(7) NOT NULL", pitSql);
            Assert.Contains("[AuditId] int NOT NULL", pitSql);

            var bridgeSql = await File.ReadAllTextAsync(Path.Combine(sqlOutputPath, "BR_CustomerOrderTraversal.sql"));
            Assert.Contains("CREATE TABLE [dbo].[BR_CustomerOrderTraversal]", bridgeSql);
            Assert.Contains("[RootHashKey] binary(16) NOT NULL", bridgeSql);
            Assert.Contains("[RelatedHashKey] binary(16) NOT NULL", bridgeSql);
            Assert.Contains("[Depth] int NOT NULL", bridgeSql);
            Assert.Contains("[Path] nvarchar(4000) NOT NULL", bridgeSql);
            Assert.Contains("[CustomerIdentifier] nvarchar(50) NOT NULL", bridgeSql);
            Assert.Contains("[OrderIdentifier] nvarchar(50) NOT NULL", bridgeSql);
            Assert.Contains("[CustomerName] nvarchar(200) NOT NULL", bridgeSql);
            Assert.Contains("[StatusCode] nvarchar(20) NOT NULL", bridgeSql);
            Assert.Contains("[EffectiveFrom] datetime2(7) NOT NULL", bridgeSql);
            Assert.Contains("[EffectiveTo] datetime2(7) NOT NULL", bridgeSql);
            Assert.Contains("[AuditId] int NOT NULL", bridgeSql);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task GenerateSql_EmitsSameAsAndHierarchicalLinkScripts()
    {
        var repoRoot = FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultLinkVariants");
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            var result = RunBusinessCli(
                $"generate-sql --workspace \"{workspacePath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("BusinessSameAsLinks: 1", result.Output);
            Assert.Contains("BusinessHierarchicalLinks: 1", result.Output);
            Assert.Contains("BusinessSameAsLinkSatellites: 1", result.Output);
            Assert.Contains("BusinessHierarchicalLinkSatellites: 1", result.Output);

            var sameAsSql = await File.ReadAllTextAsync(Path.Combine(sqlOutputPath, "BSAL_CustomerMatch.sql"));
            Assert.Contains("CREATE TABLE [dbo].[BSAL_CustomerMatch]", sameAsSql);
            Assert.Contains("[PrimaryHashKey] binary(16) NOT NULL", sameAsSql);
            Assert.Contains("[EquivalentHashKey] binary(16) NOT NULL", sameAsSql);
            Assert.Contains("[AuditId] int NOT NULL", sameAsSql);

            var hierarchySql = await File.ReadAllTextAsync(Path.Combine(sqlOutputPath, "BHAL_EmployeeManager.sql"));
            Assert.Contains("CREATE TABLE [dbo].[BHAL_EmployeeManager]", hierarchySql);
            Assert.Contains("[ParentHashKey] binary(16) NOT NULL", hierarchySql);
            Assert.Contains("[ChildHashKey] binary(16) NOT NULL", hierarchySql);
            Assert.Contains("[AuditId] int NOT NULL", hierarchySql);

            var sameAsSatelliteSql = await File.ReadAllTextAsync(Path.Combine(sqlOutputPath, "BSALS_CustomerMatch_Evidence.sql"));
            Assert.Contains("[MatchScore] nvarchar(20) NOT NULL", sameAsSatelliteSql);
            Assert.Contains("[AuditId] int NOT NULL", sameAsSatelliteSql);

            var hierarchySatelliteSql = await File.ReadAllTextAsync(Path.Combine(sqlOutputPath, "BHALS_EmployeeManager_Line.sql"));
            Assert.Contains("[LineType] nvarchar(20) NOT NULL", hierarchySatelliteSql);
            Assert.Contains("[AuditId] int NOT NULL", hierarchySatelliteSql);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }
    [Fact]
    public async Task GenerateSql_FailsWhenBridgePathIsInconsistent()
    {
        var repoRoot = FindRepositoryRoot();
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "Workspace");
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            CopyDirectory(Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceHelpers"), workspacePath);
            var bridgeHubPath = Path.Combine(workspacePath, "metadata", "instance", "BusinessBridgeHub.xml");
            var bridgeHubXml = await File.ReadAllTextAsync(bridgeHubPath);
            bridgeHubXml = bridgeHubXml.Replace("BusinessHubId=\"Order\"", "BusinessHubId=\"Invoice\"", StringComparison.Ordinal);
            await File.WriteAllTextAsync(bridgeHubPath, bridgeHubXml);

            var result = RunBusinessCli(
                $"generate-sql --workspace \"{workspacePath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("does not connect", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("OK: business datavault sql generated", result.Output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }


    [Fact]
    public async Task GenerateSql_FailsWhenBridgeProjectionFallsOutsidePath()
    {
        var repoRoot = FindRepositoryRoot();
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "Workspace");
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            CopyDirectory(Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceHelpers"), workspacePath);
            var projectionPath = Path.Combine(workspacePath, "metadata", "instance", "BusinessBridgeHubKeyPartProjection.xml");
            var projectionXml = await File.ReadAllTextAsync(projectionPath);
            projectionXml = projectionXml.Replace("BusinessHubKeyPartId=\"OrderIdentifier\"", "BusinessHubKeyPartId=\"InvoiceIdentifier\"", StringComparison.Ordinal);
            await File.WriteAllTextAsync(projectionPath, projectionXml);

            var result = RunBusinessCli(
                $"generate-sql --workspace \"{workspacePath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("ordered path", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("BusinessHubKeyPart", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task GenerateSql_FailsWhenMultiActiveHubSatelliteHasNoKeyParts()
    {
        var repoRoot = FindRepositoryRoot();
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "Workspace");
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            CopyDirectory(Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceHelpers"), workspacePath);
            var satellitePath = Path.Combine(workspacePath, "metadata", "instance", "BusinessHubSatellite.xml");
            var satelliteXml = await File.ReadAllTextAsync(satellitePath);
            satelliteXml = satelliteXml.Replace("<SatelliteKind>standard</SatelliteKind>", "<SatelliteKind>multi-active</SatelliteKind>", StringComparison.Ordinal);
            await File.WriteAllTextAsync(satellitePath, satelliteXml);

            var result = RunBusinessCli(
                $"generate-sql --workspace \"{workspacePath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("multi-active", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("BusinessHubSatelliteKeyPart", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task GenerateSql_FailsWhenPointInTimeReferencesMultiActiveSatellite()
    {
        var repoRoot = FindRepositoryRoot();
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "Workspace");
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            CopyDirectory(Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceHelpers"), workspacePath);
            var satellitePath = Path.Combine(workspacePath, "metadata", "instance", "BusinessHubSatellite.xml");
            var satelliteXml = await File.ReadAllTextAsync(satellitePath);
            satelliteXml = satelliteXml.Replace("<SatelliteKind>standard</SatelliteKind>", "<SatelliteKind>multi-active</SatelliteKind>", StringComparison.Ordinal);
            await File.WriteAllTextAsync(satellitePath, satelliteXml);

            var keyPartPath = Path.Combine(workspacePath, "metadata", "instance", "BusinessHubSatelliteKeyPart.xml");
            await File.WriteAllTextAsync(keyPartPath,
                """
                <?xml version="1.0" encoding="utf-8"?>
                <MetaBusinessDataVault>
                  <BusinessHubSatelliteKeyPartList>
                    <BusinessHubSatelliteKeyPart Id="CustomerProfileVersion" BusinessHubSatelliteId="CustomerProfile">
                      <Name>VersionId</Name>
                      <DataTypeId>meta:type:String</DataTypeId>
                      <Ordinal>1</Ordinal>
                    </BusinessHubSatelliteKeyPart>
                  </BusinessHubSatelliteKeyPartList>
                </MetaBusinessDataVault>
                """);

            var keyPartDetailPath = Path.Combine(workspacePath, "metadata", "instance", "BusinessHubSatelliteKeyPartDataTypeDetail.xml");
            await File.WriteAllTextAsync(keyPartDetailPath,
                """
                <?xml version="1.0" encoding="utf-8"?>
                <MetaBusinessDataVault>
                  <BusinessHubSatelliteKeyPartDataTypeDetailList>
                    <BusinessHubSatelliteKeyPartDataTypeDetail Id="CustomerProfileVersionLength" BusinessHubSatelliteKeyPartId="CustomerProfileVersion">
                      <Name>Length</Name>
                      <Value>50</Value>
                    </BusinessHubSatelliteKeyPartDataTypeDetail>
                  </BusinessHubSatelliteKeyPartDataTypeDetailList>
                </MetaBusinessDataVault>
                """);

            var result = RunBusinessCli(
                $"generate-sql --workspace \"{workspacePath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("point-in-time", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("multi-active", result.Output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task GenerateSql_EmitsPointInTimeExplicitStampColumns()
    {
        var repoRoot = FindRepositoryRoot();
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "Workspace");
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            CopyDirectory(Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceHelpers"), workspacePath);
            await File.WriteAllTextAsync(
                Path.Combine(workspacePath, "metadata", "instance", "BusinessPointInTimeStamp.xml"),
                """
<?xml version="1.0" encoding="utf-8"?>
<MetaBusinessDataVault>
  <BusinessPointInTimeStampList>
    <BusinessPointInTimeStamp Id="CustomerSnapshot-BusinessDate" BusinessPointInTimeId="CustomerSnapshot">
      <Name>BusinessDate</Name>
      <DataTypeId>meta:type:DateTime</DataTypeId>
      <Ordinal>10</Ordinal>
    </BusinessPointInTimeStamp>
  </BusinessPointInTimeStampList>
</MetaBusinessDataVault>
""");

            var result = RunBusinessCli(
                $"generate-sql --workspace \"{workspacePath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(0, result.ExitCode);
            var pitSql = await File.ReadAllTextAsync(Path.Combine(sqlOutputPath, "PIT_CustomerSnapshot.sql"));
            Assert.Contains("[BusinessDate] datetime NOT NULL", pitSql);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }
    [Fact]
    public async Task GenerateSql_FailsWhenPointInTimeReferencesHubSatelliteOutsideParentHub()
    {
        var repoRoot = FindRepositoryRoot();
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "Workspace");
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            CopyDirectory(Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceHelpers"), workspacePath);
            var pitPath = Path.Combine(workspacePath, "metadata", "instance", "BusinessPointInTime.xml");
            var pitXml = await File.ReadAllTextAsync(pitPath);
            pitXml = pitXml.Replace("BusinessHubId=\"Customer\"", "BusinessHubId=\"Order\"", StringComparison.Ordinal);
            await File.WriteAllTextAsync(pitPath, pitXml);

            var result = RunBusinessCli(
                $"generate-sql --workspace \"{workspacePath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("BusinessPointInTime", result.Output, StringComparison.Ordinal);
            Assert.Contains("BusinessHubSatellite", result.Output, StringComparison.Ordinal);
            Assert.Contains("belonging to hub", result.Output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task GenerateSql_FailsWhenPointInTimeOrdinalsAreDuplicated()
    {
        var repoRoot = FindRepositoryRoot();
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "Workspace");
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            CopyDirectory(Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceHelpers"), workspacePath);
            var pitLinkPath = Path.Combine(workspacePath, "metadata", "instance", "BusinessPointInTimeLinkSatellite.xml");
            var pitLinkXml = await File.ReadAllTextAsync(pitLinkPath);
            pitLinkXml = pitLinkXml.Replace("<Ordinal>2</Ordinal>", "<Ordinal>1</Ordinal>", StringComparison.Ordinal);
            await File.WriteAllTextAsync(pitLinkPath, pitLinkXml);

            var result = RunBusinessCli(
                $"generate-sql --workspace \"{workspacePath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("duplicate Ordinal", result.Output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task GenerateSql_FailsWhenRequiredImplementationPropertyIsMissing()
    {
        var repoRoot = FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var implementationPath = Path.Combine(root, "Implementation");
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            CopyDirectory(Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation"), implementationPath);
            var implementationFile = Path.Combine(implementationPath, "metadata", "instance", "BusinessHubImplementation.xml");
            var implementationXml = await File.ReadAllTextAsync(implementationFile);
            implementationXml = implementationXml.Replace("<RecordSourceColumnName>RecordSource</RecordSourceColumnName>", string.Empty, StringComparison.Ordinal);
            await File.WriteAllTextAsync(implementationFile, implementationXml);

            var result = RunBusinessCli(
                $"generate-sql --workspace \"{workspacePath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(4, result.ExitCode);
            Assert.Contains("MetaDataVaultImplementation", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("RecordSourceColumnName", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    private static void SeedMetaSchema(Meta.Core.Domain.Workspace workspace)
    {
        var systems = workspace.Instance.GetOrCreateEntityRecords("System");
        systems.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "1",
            SourceShardFileName = "System.xml",
            Values =
            {
                ["Name"] = "Sales"
            }
        });

        var schemas = workspace.Instance.GetOrCreateEntityRecords("Schema");
        schemas.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "1",
            SourceShardFileName = "Schema.xml",
            Values =
            {
                ["Name"] = "dbo"
            },
            RelationshipIds =
            {
                ["SystemId"] = "1"
            }
        });

        var tables = workspace.Instance.GetOrCreateEntityRecords("Table");
        tables.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "1",
            SourceShardFileName = "Table.xml",
            Values =
            {
                ["Name"] = "Order"
            },
            RelationshipIds =
            {
                ["SchemaId"] = "1"
            }
        });
        tables.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "2",
            SourceShardFileName = "Table.xml",
            Values =
            {
                ["Name"] = "Customer"
            },
            RelationshipIds =
            {
                ["SchemaId"] = "1"
            }
        });

        var fields = workspace.Instance.GetOrCreateEntityRecords("Field");
        fields.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "1",
            SourceShardFileName = "Field.xml",
            Values =
            {
                ["Name"] = "OrderId",
                ["DataTypeId"] = "sqlserver:type:int",
                ["Ordinal"] = "1"
            },
            RelationshipIds =
            {
                ["TableId"] = "1"
            }
        });
        fields.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "2",
            SourceShardFileName = "Field.xml",
            Values =
            {
                ["Name"] = "OrderNumber",
                ["DataTypeId"] = "sqlserver:type:nvarchar",
                ["Ordinal"] = "2"
            },
            RelationshipIds =
            {
                ["TableId"] = "1"
            }
        });
        fields.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "3",
            SourceShardFileName = "Field.xml",
            Values =
            {
                ["Name"] = "CustomerId",
                ["DataTypeId"] = "sqlserver:type:int",
                ["Ordinal"] = "3"
            },
            RelationshipIds =
            {
                ["TableId"] = "1"
            }
        });
        fields.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "4",
            SourceShardFileName = "Field.xml",
            Values =
            {
                ["Name"] = "CustomerId",
                ["DataTypeId"] = "sqlserver:type:int",
                ["Ordinal"] = "1"
            },
            RelationshipIds =
            {
                ["TableId"] = "2"
            }
        });
        fields.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "5",
            SourceShardFileName = "Field.xml",
            Values =
            {
                ["Name"] = "CustomerName",
                ["DataTypeId"] = "sqlserver:type:nvarchar",
                ["Ordinal"] = "2"
            },
            RelationshipIds =
            {
                ["TableId"] = "2"
            }
        });

        var tableRelationships = workspace.Instance.GetOrCreateEntityRecords("TableRelationship");
        tableRelationships.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "rel:1",
            SourceShardFileName = "TableRelationship.xml",
            Values =
            {
                ["Name"] = "FK_Order_Customer",
                ["TargetSchemaName"] = "dbo",
                ["TargetTableName"] = "Customer"
            },
            RelationshipIds =
            {
                ["SourceTableId"] = "1"
            }
        });

        var tableRelationshipFields = workspace.Instance.GetOrCreateEntityRecords("TableRelationshipField");
        tableRelationshipFields.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "relf:1",
            SourceShardFileName = "TableRelationshipField.xml",
            Values =
            {
                ["Ordinal"] = "1",
                ["SourceFieldName"] = "CustomerId",
                ["TargetFieldName"] = "CustomerId"
            },
            RelationshipIds =
            {
                ["TableRelationshipId"] = "rel:1",
                ["SourceFieldId"] = "3"
            }
        });
    }

    private static (int ExitCode, string Output) RunRawCli(string arguments)
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaDataVault.Raw.Cli", "meta-datavault-raw.exe");
        var startInfo = new ProcessStartInfo
        {
            FileName = cliPath,
            Arguments = arguments,
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        return RunProcess(startInfo, "Could not start DataVault CLI process.");
    }

    private static (int ExitCode, string Output) RunProcess(ProcessStartInfo startInfo, string errorMessage)
    {
        using var process = Process.Start(startInfo)
                            ?? throw new InvalidOperationException(errorMessage);
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            try
            {
                process.WaitForExitAsync(timeout.Token).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException exception)
            {
                TryKillProcessTree(process);
                process.WaitForExit();
                throw new TimeoutException($"Timed out waiting for process: {startInfo.FileName} {startInfo.Arguments}", exception);
            }

            var stdout = stdoutTask.GetAwaiter().GetResult();
            var stderr = stderrTask.GetAwaiter().GetResult();
            return (process.ExitCode, stdout + stderr);
        }
        finally
        {
            if (!process.HasExited)
            {
                TryKillProcessTree(process);
                process.WaitForExit();
            }
        }
    }

    private static void RunBusinessAdd(string workspacePath, string command)
    {
        var result = RunBusinessCli($"{command} --workspace \"{workspacePath}\"");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("OK: business datavault row added", result.Output, StringComparison.OrdinalIgnoreCase);
    }
    private static (int ExitCode, string Output) RunBusinessCli(string arguments)
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot, "MetaDataVault.Business.Cli", "meta-datavault-business.exe");
        var startInfo = new ProcessStartInfo
        {
            FileName = cliPath,
            Arguments = arguments,
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        return RunProcess(startInfo, "Could not start DataVault CLI process.");
    }

    private static string ResolveCliPath(string repoRoot, string projectDirectory, string executableName)
    {
        var cliPath = Path.Combine(repoRoot, projectDirectory, "bin", "Debug", "net8.0", executableName);
        if (!File.Exists(cliPath))
        {
            throw new FileNotFoundException($"Could not find compiled DataVault CLI at '{cliPath}'. Build the requested DataVault CLI before running tests.");
        }

        return cliPath;
    }

    private static void TryKillProcessTree(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = AppContext.BaseDirectory;
        while (!string.IsNullOrWhiteSpace(directory))
        {
            if (File.Exists(Path.Combine(directory, "README.md")) && (Directory.Exists(Path.Combine(directory, "MetaDataVault.Raw.Cli")) || Directory.Exists(Path.Combine(directory, "MetaDataVault.Business.Cli"))))
            {
                return directory;
            }

            var parent = Directory.GetParent(directory);
            if (parent == null)
            {
                break;
            }

            directory = parent.FullName;
        }

        throw new InvalidOperationException("Could not locate meta-bi repository root from test base directory.");
    }

    private static void CopyDirectory(string sourcePath, string targetPath)
    {
        Directory.CreateDirectory(targetPath);

        foreach (var directory in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(Path.Combine(targetPath, Path.GetRelativePath(sourcePath, directory)));
        }

        foreach (var file in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourcePath, file);
            var targetFile = Path.Combine(targetPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
            File.Copy(file, targetFile, overwrite: true);
        }
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}









