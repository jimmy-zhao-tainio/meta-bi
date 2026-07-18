using System.Diagnostics;
using Meta.Core.Services;
using MetaSchema.Core;

namespace MetaDataVault.Tests;

public sealed partial class CliTests
{
    [Fact]
    public async Task BusinessAuthoringCommands_CoverAllAddCommands()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            var createResult = RunBusinessCli($"new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            RunBusinessAdd(workspacePath, "add-hub --id Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-hub --id Order --name Order");
            RunBusinessAdd(workspacePath, "add-hub --id CustomerAlias --name CustomerAlias");
            RunBusinessAdd(workspacePath, "add-hub --id ParentNode --name ParentNode");
            RunBusinessAdd(workspacePath, "add-hub --id ChildNode --name ChildNode");

            RunBusinessAdd(workspacePath, "add-hub-key-part --id CustomerIdentifier --hub Customer --name Identifier --data-type-id meta:type:String --length 50");
            RunBusinessAdd(workspacePath, "add-hub-key-part --id CustomerSource --hub Customer --name Source --data-type-id meta:type:String --previous-key-part CustomerIdentifier");
            RunBusinessAdd(workspacePath, "add-hub-key-part --id OrderIdentifier --hub Order --name Identifier --data-type-id meta:type:String");
            RunBusinessAdd(workspacePath, "add-hub-key-part --id CustomerAliasIdentifier --hub CustomerAlias --name Identifier --data-type-id meta:type:String");

            RunBusinessAdd(workspacePath, "add-link --id CustomerOrder --name CustomerOrder");
            RunBusinessAdd(workspacePath, "add-link-role --id CustomerOrderCustomer --link CustomerOrder --hub Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-link-role --id CustomerOrderOrder --link CustomerOrder --hub Order --name Order");

            RunBusinessAdd(workspacePath, "add-same-as-link --id CustomerSameAsAlias --name CustomerSameAsAlias --primary-hub Customer --equivalent-hub CustomerAlias");
            RunBusinessAdd(workspacePath, "add-hierarchical-link --id ParentChild --name ParentChild --parent-hub ParentNode --child-hub ChildNode");

            RunBusinessAdd(workspacePath, "add-reference --id StatusCode --name StatusCode");
            RunBusinessAdd(workspacePath, "add-reference-key-part --id StatusCodeValue --reference StatusCode --name Code --data-type-id meta:type:String --length 20");
            RunBusinessAdd(workspacePath, "add-reference-key-part --id StatusCodeSource --reference StatusCode --name Source --data-type-id meta:type:String --previous-key-part StatusCodeValue");

            RunBusinessAdd(workspacePath, "add-hub-satellite --id CustomerProfile --hub Customer --name CustomerProfile");
            RunBusinessAdd(workspacePath, "add-hub-satellite-attribute --id CustomerName --hub-satellite CustomerProfile --name CustomerName --data-type-id meta:type:String --ordinal 1 --length 200");

            RunBusinessAdd(workspacePath, "add-link-satellite --id CustomerOrderStatus --link CustomerOrder --name CustomerOrderStatus");
            RunBusinessAdd(workspacePath, "add-link-satellite-attribute --id CustomerOrderStatusCode --link-satellite CustomerOrderStatus --name StatusCode --data-type-id meta:type:String --ordinal 1 --length 20");

            RunBusinessAdd(workspacePath, "add-same-as-link-satellite --id CustomerSameAsAliasAudit --same-as-link CustomerSameAsAlias --name CustomerSameAsAliasAudit");
            RunBusinessAdd(workspacePath, "add-same-as-link-satellite-attribute --id CustomerSameAsAliasReason --same-as-link-satellite CustomerSameAsAliasAudit --name ReasonCode --data-type-id meta:type:String --ordinal 1 --length 20");

            RunBusinessAdd(workspacePath, "add-hierarchical-link-satellite --id ParentChildAudit --hierarchical-link ParentChild --name ParentChildAudit");
            RunBusinessAdd(workspacePath, "add-hierarchical-link-satellite-attribute --id ParentChildRelationshipType --hierarchical-link-satellite ParentChildAudit --name RelationshipType --data-type-id meta:type:String --ordinal 1 --length 30");

            RunBusinessAdd(workspacePath, "add-reference-satellite --id StatusCodeDescriptionSet --reference StatusCode --name StatusCodeDescriptionSet");
            RunBusinessAdd(workspacePath, "add-reference-satellite-attribute --id StatusCodeDescription --reference-satellite StatusCodeDescriptionSet --name Description --data-type-id meta:type:String --ordinal 1 --length 100");

            RunBusinessAdd(workspacePath, "add-point-in-time --id CustomerSnapshot --hub Customer --name CustomerSnapshot");
            RunBusinessAdd(workspacePath, "add-point-in-time-stamp --id CustomerSnapshotBusinessDate --point-in-time CustomerSnapshot --name BusinessDate --data-type-id meta:type:DateTime --ordinal 1 --precision 7");
            RunBusinessAdd(workspacePath, "add-point-in-time-hub-satellite --id CustomerSnapshotProfile --point-in-time CustomerSnapshot --hub-satellite CustomerProfile --ordinal 1");
            RunBusinessAdd(workspacePath, "add-point-in-time-link-satellite --id CustomerSnapshotOrderStatus --point-in-time CustomerSnapshot --link-satellite CustomerOrderStatus --ordinal 2");
            RunBusinessAdd(workspacePath, "add-bridge --id CustomerOrderTraversal --hub Customer --name CustomerOrderTraversal");
            RunBusinessAdd(workspacePath, "add-bridge-traversal --id CustomerOrderTraversalCustomerOrder --bridge CustomerOrderTraversal --source-role CustomerOrderCustomer --target-role CustomerOrderOrder");

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("BusinessPointInTime"));
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("BusinessSameAsLink"));
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("BusinessHierarchicalLink"));
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("BusinessReference"));
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("BusinessBridge"));

            var hubKeyPartDetails = workspace.Instance.GetOrCreateEntityRecords("BusinessHubKeyPartDataTypeDetail");
            Assert.Contains(hubKeyPartDetails, record =>
                string.Equals(record.RelationshipIds.GetValueOrDefault("BusinessHubKeyPartId"), "CustomerIdentifier", StringComparison.Ordinal) &&
                string.Equals(record.Values.GetValueOrDefault("Name"), "Length", StringComparison.Ordinal) &&
                string.Equals(record.Values.GetValueOrDefault("Value"), "50", StringComparison.Ordinal));

            var hubKeyParts = workspace.Instance.GetOrCreateEntityRecords("BusinessHubKeyPart").ToDictionary(record => record.Id, StringComparer.Ordinal);
            var referenceKeyParts = workspace.Instance.GetOrCreateEntityRecords("BusinessReferenceKeyPart").ToDictionary(record => record.Id, StringComparer.Ordinal);
            Assert.False(hubKeyParts["CustomerIdentifier"].Values.ContainsKey("Ordinal"));
            Assert.Equal("CustomerIdentifier", hubKeyParts["CustomerSource"].RelationshipIds["PreviousKeyPartId"]);
            Assert.Equal("StatusCodeValue", referenceKeyParts["StatusCodeSource"].RelationshipIds["PreviousKeyPartId"]);

            var pointInTimeStampDetails = workspace.Instance.GetOrCreateEntityRecords("BusinessPointInTimeStampDataTypeDetail");
            Assert.Contains(pointInTimeStampDetails, record =>
                string.Equals(record.RelationshipIds.GetValueOrDefault("BusinessPointInTimeStampId"), "CustomerSnapshotBusinessDate", StringComparison.Ordinal) &&
                string.Equals(record.Values.GetValueOrDefault("Name"), "Precision", StringComparison.Ordinal) &&
                string.Equals(record.Values.GetValueOrDefault("Value"), "7", StringComparison.Ordinal));
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public void BusinessAuthoringRejectsSameAsLinkWithIdenticalHubs()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            var createResult = RunBusinessCli($"new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            RunBusinessAdd(workspacePath, "add-hub --id Customer --name Customer");

            var result = RunBusinessCli($"add-same-as-link --workspace \"{workspacePath}\" --id CustomerSameAsCustomer --name CustomerSameAsCustomer --primary-hub Customer --equivalent-hub Customer");
            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("distinct PrimaryHubId and EquivalentHubId", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public void BusinessAuthoringRejectsDuplicateLinkRoleName()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            var createResult = RunBusinessCli($"new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            RunBusinessAdd(workspacePath, "add-hub --id Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-hub --id Order --name Order");
            RunBusinessAdd(workspacePath, "add-link --id CustomerOrder --name CustomerOrder");
            RunBusinessAdd(workspacePath, "add-link-role --id CustomerOrderCustomer --link CustomerOrder --hub Customer --name Customer");

            var result = RunBusinessCli($"add-link-role --workspace \"{workspacePath}\" --id CustomerOrderCustomerAgain --link CustomerOrder --hub Order --name Customer");
            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("Business link 'CustomerOrder' already has a role named 'Customer'", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public void BusinessAuthoringRejectsBridgeTraversalOutsideAnchor()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            Assert.Equal(0, RunBusinessCli($"new-workspace \"{workspacePath}\"").ExitCode);

            RunBusinessAdd(workspacePath, "add-hub --id Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-hub --id Order --name Order");
            RunBusinessAdd(workspacePath, "add-link --id CustomerOrder --name CustomerOrder");
            RunBusinessAdd(workspacePath, "add-link-role --id CustomerOrderCustomer --link CustomerOrder --hub Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-link-role --id CustomerOrderOrder --link CustomerOrder --hub Order --name Order");
            RunBusinessAdd(workspacePath, "add-bridge --id CustomerOrderTraversal --hub Customer --name CustomerOrderTraversal");

            var result = RunBusinessCli($"add-bridge-traversal --workspace \"{workspacePath}\" --id CustomerOrderTraversalOrderCustomer --bridge CustomerOrderTraversal --source-role CustomerOrderOrder --target-role CustomerOrderCustomer");
            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("must start from its anchor hub 'Customer'", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public void BusinessAuthoringRejectsSecondKeyPartWithoutPrecedence()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            Assert.Equal(0, RunBusinessCli($"new-workspace \"{workspacePath}\"").ExitCode);
            RunBusinessAdd(workspacePath, "add-hub --id Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-hub-key-part --id CustomerCountry --hub Customer --name CountryCode --data-type-id meta:type:String");

            var result = RunBusinessCli($"add-hub-key-part --workspace \"{workspacePath}\" --id CustomerNumber --hub Customer --name CustomerNumber --data-type-id meta:type:String");

            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("must have exactly one starting key part", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task BusinessAuthoringAppendsExistingOrdinalsWhenOmitted()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            Assert.Equal(0, RunBusinessCli($"new-workspace \"{workspacePath}\"").ExitCode);

            RunBusinessAdd(workspacePath, "add-hub --id Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-hub --id Order --name Order");
            RunBusinessAdd(workspacePath, "add-link --id CustomerOrder --name CustomerOrder");
            RunBusinessAdd(workspacePath, "add-link-role --id CustomerOrderCustomer --link CustomerOrder --hub Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-link-role --id CustomerOrderOrder --link CustomerOrder --hub Order --name Order");
            RunBusinessAdd(workspacePath, "add-hub-satellite --id CustomerProfile --hub Customer --name CustomerProfile");
            RunBusinessAdd(workspacePath, "add-link-satellite --id CustomerOrderStatus --link CustomerOrder --name CustomerOrderStatus");
            RunBusinessAdd(workspacePath, "add-link-satellite-attribute --id CustomerOrderStatusCode --link-satellite CustomerOrderStatus --name StatusCode --data-type-id meta:type:String");
            RunBusinessAdd(workspacePath, "add-link-satellite-attribute --id CustomerOrderStatusReason --link-satellite CustomerOrderStatus --name StatusReason --data-type-id meta:type:String");
            RunBusinessAdd(workspacePath, "add-point-in-time --id CustomerSnapshot --hub Customer --name CustomerSnapshot");
            RunBusinessAdd(workspacePath, "add-point-in-time-hub-satellite --id CustomerSnapshotProfile --point-in-time CustomerSnapshot --hub-satellite CustomerProfile");
            RunBusinessAdd(workspacePath, "add-point-in-time-link-satellite --id CustomerSnapshotOrderStatus --point-in-time CustomerSnapshot --link-satellite CustomerOrderStatus");
            RunBusinessAdd(workspacePath, "add-bridge --id CustomerOrderTraversal --hub Customer --name CustomerOrderTraversal");
            RunBusinessAdd(workspacePath, "add-bridge-traversal --id CustomerOrderTraversalCustomerOrder --bridge CustomerOrderTraversal --source-role CustomerOrderCustomer --target-role CustomerOrderOrder");

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            var linkRoles = workspace.Instance.GetOrCreateEntityRecords("BusinessLinkRole").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var linkSatelliteAttributes = workspace.Instance.GetOrCreateEntityRecords("BusinessLinkSatelliteAttribute").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var pointInTimeHubSatellites = workspace.Instance.GetOrCreateEntityRecords("BusinessPointInTimeHubSatellite").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var pointInTimeLinkSatellites = workspace.Instance.GetOrCreateEntityRecords("BusinessPointInTimeLinkSatellite").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var bridgeTraversals = workspace.Instance.GetOrCreateEntityRecords("BusinessBridgeTraversal").ToDictionary(row => row.Id, StringComparer.Ordinal);

            Assert.Equal("Customer", linkRoles["CustomerOrderCustomer"].Values["Name"]);
            Assert.Equal("CustomerOrderCustomer", bridgeTraversals["CustomerOrderTraversalCustomerOrder"].RelationshipIds["SourceRoleId"]);
            Assert.Equal("CustomerOrderOrder", bridgeTraversals["CustomerOrderTraversalCustomerOrder"].RelationshipIds["TargetRoleId"]);
            Assert.Equal("1", linkSatelliteAttributes["CustomerOrderStatusCode"].Values["Ordinal"]);
            Assert.Equal("2", linkSatelliteAttributes["CustomerOrderStatusReason"].Values["Ordinal"]);
            Assert.Equal("1", pointInTimeHubSatellites["CustomerSnapshotProfile"].Values["Ordinal"]);
            Assert.Equal("2", pointInTimeLinkSatellites["CustomerSnapshotOrderStatus"].Values["Ordinal"]);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task BusinessAuthoringCommandWithoutWorkspace_UsesCurrentDirectoryWorkspace()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            Assert.Equal(0, RunBusinessCli($"new-workspace \"{workspacePath}\"").ExitCode);

            var result = RunBusinessCli("add-hub --id Customer --name Customer", workspacePath);

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("Ok", result.Output, StringComparison.Ordinal);

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            var hubs = workspace.Instance.GetOrCreateEntityRecords("BusinessHub");
            Assert.Single(hubs);
            Assert.Equal("Customer", hubs[0].Id);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task RawAuthoringCommands_CoverBaselineAddCommands()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");

        try
        {
            var createResult = RunRawCli($"new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            RunRawAdd(workspacePath, "add-field --id CustomerIdField --name CustomerId --data-type-id sqlserver:type:nvarchar");
            RunRawAdd(workspacePath, "add-field-data-type-detail --id CustomerIdFieldLength --field CustomerIdField --name Length --value 50");
            RunRawAdd(workspacePath, "add-field --id CustomerNameField --name CustomerName --data-type-id sqlserver:type:nvarchar");
            RunRawAdd(workspacePath, "add-field-data-type-detail --id CustomerNameFieldLength --field CustomerNameField --name Length --value 200");
            RunRawAdd(workspacePath, "add-field --id OrderIdField --name OrderId --data-type-id sqlserver:type:nvarchar");
            RunRawAdd(workspacePath, "add-field-data-type-detail --id OrderIdFieldLength --field OrderIdField --name Length --value 50");
            RunRawAdd(workspacePath, "add-field --id OrderStatusField --name StatusCode --data-type-id sqlserver:type:nvarchar");
            RunRawAdd(workspacePath, "add-field-data-type-detail --id OrderStatusFieldLength --field OrderStatusField --name Length --value 20");
            RunRawAdd(workspacePath, "add-hub --id CustomerHub --name Customer");
            RunRawAdd(workspacePath, "add-hub --id OrderHub --name Order");
            RunRawAdd(workspacePath, "add-hub-key-part --id CustomerHubKey --hub CustomerHub --field CustomerIdField --name CustomerId");
            RunRawAdd(workspacePath, "add-hub-key-part --id OrderHubKey --hub OrderHub --field OrderIdField --name OrderId");
            RunRawAdd(workspacePath, "add-hub-satellite --id CustomerProfileSat --hub CustomerHub --name CustomerProfile --satellite-kind standard");
            RunRawAdd(workspacePath, "add-hub-satellite-attribute --id CustomerNameAttr --hub-satellite CustomerProfileSat --field CustomerNameField --name CustomerName");
            RunRawAdd(workspacePath, "add-link --id OrderCustomerLink --name OrderCustomer --link-kind standard");
            RunRawAdd(workspacePath, "add-link-role --id OrderCustomerLinkOrder --link OrderCustomerLink --hub OrderHub --name Order");
            RunRawAdd(workspacePath, "add-link-role --id OrderCustomerLinkCustomer --link OrderCustomerLink --hub CustomerHub --name Customer");
            RunRawAdd(workspacePath, "add-link-satellite --id OrderCustomerStatusSat --link OrderCustomerLink --name OrderCustomerStatus --satellite-kind standard");
            RunRawAdd(workspacePath, "add-link-satellite-attribute --id OrderCustomerStatusCodeAttr --link-satellite OrderCustomerStatusSat --field OrderStatusField --name StatusCode");

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawHub").Count);
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("RawLink"));
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawLinkRole").Count);
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("RawHubSatellite"));
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("RawLinkSatellite"));
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task RawAuthoringCreatesUnorderedStructuralMembers()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");

        try
        {
            Assert.Equal(0, RunRawCli($"new-workspace \"{workspacePath}\"").ExitCode);

            RunRawAdd(workspacePath, "add-field --id CustomerIdField --name CustomerId --data-type-id sqlserver:type:nvarchar");
            RunRawAdd(workspacePath, "add-field --id CustomerNameField --name CustomerName --data-type-id sqlserver:type:nvarchar");
            RunRawAdd(workspacePath, "add-field --id OrderIdField --name OrderId --data-type-id sqlserver:type:nvarchar");
            RunRawAdd(workspacePath, "add-field --id OrderStatusField --name StatusCode --data-type-id sqlserver:type:nvarchar");
            RunRawAdd(workspacePath, "add-hub --id CustomerHub --name Customer");
            RunRawAdd(workspacePath, "add-hub --id OrderHub --name Order");
            RunRawAdd(workspacePath, "add-hub-key-part --id CustomerHubKey --hub CustomerHub --field CustomerIdField --name CustomerId");
            RunRawAdd(workspacePath, "add-hub-satellite --id CustomerProfileSat --hub CustomerHub --name CustomerProfile --satellite-kind standard");
            RunRawAdd(workspacePath, "add-hub-satellite-attribute --id CustomerNameAttr --hub-satellite CustomerProfileSat --field CustomerNameField --name CustomerName");
            RunRawAdd(workspacePath, "add-link --id OrderCustomerLink --name OrderCustomer --link-kind standard");
            RunRawAdd(workspacePath, "add-link-role --id OrderCustomerLinkOrder --link OrderCustomerLink --hub OrderHub --name Order");
            RunRawAdd(workspacePath, "add-link-role --id OrderCustomerLinkCustomer --link OrderCustomerLink --hub CustomerHub --name Customer");
            RunRawAdd(workspacePath, "add-link-satellite --id OrderCustomerStatusSat --link OrderCustomerLink --name OrderCustomerStatus --satellite-kind standard");
            RunRawAdd(workspacePath, "add-link-satellite-attribute --id OrderCustomerStatusCodeAttr --link-satellite OrderCustomerStatusSat --field OrderStatusField --name StatusCode");

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            var hubKeyParts = workspace.Instance.GetOrCreateEntityRecords("RawHubKeyPart").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var hubSatelliteAttributes = workspace.Instance.GetOrCreateEntityRecords("RawHubSatelliteAttribute").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var linkSatelliteAttributes = workspace.Instance.GetOrCreateEntityRecords("RawLinkSatelliteAttribute").ToDictionary(row => row.Id, StringComparer.Ordinal);

            Assert.DoesNotContain("Ordinal", hubKeyParts["CustomerHubKey"].Values.Keys);
            Assert.DoesNotContain("Ordinal", hubSatelliteAttributes["CustomerNameAttr"].Values.Keys);
            Assert.DoesNotContain("Ordinal", linkSatelliteAttributes["OrderCustomerStatusCodeAttr"].Values.Keys);
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

        AddMetaSchemaTable(workspace, "1", "Order", "1");
        AddMetaSchemaTable(workspace, "2", "Customer", "1");
        AddMetaSchemaField(workspace, "1", "1", "OrderId", "sqlserver:type:int", "1", "false");
        AddMetaSchemaField(workspace, "2", "1", "OrderNumber", "sqlserver:type:nvarchar", "2", "false");
        AddMetaSchemaField(workspace, "3", "1", "CustomerId", "sqlserver:type:int", "3", "false");
        AddMetaSchemaField(workspace, "4", "2", "CustomerId", "sqlserver:type:int", "1", "false");
        AddMetaSchemaField(workspace, "5", "2", "CustomerName", "sqlserver:type:nvarchar", "2", "true");
        AddMetaSchemaPrimaryKey(workspace, "key:1", "PK_Order", "1");
        AddMetaSchemaPrimaryKey(workspace, "key:2", "PK_Customer", "2");
        AddMetaSchemaKeyField(workspace, "keyf:1", "key:1", "1", "1");
        AddMetaSchemaKeyField(workspace, "keyf:2", "key:2", "4", "1");

        var tableRelationships = workspace.Instance.GetOrCreateEntityRecords("TableRelationship");
        tableRelationships.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "rel:1",
            SourceShardFileName = "TableRelationship.xml",
            Values =
            {
                ["Name"] = "FK_Order_Customer"
            },
            RelationshipIds =
            {
                ["SourceTableId"] = "1",
                ["TargetTableId"] = "2"
            }
        });

        var tableRelationshipFields = workspace.Instance.GetOrCreateEntityRecords("TableRelationshipField");
        tableRelationshipFields.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "relf:1",
            SourceShardFileName = "TableRelationshipField.xml",
            Values =
            {
                ["Ordinal"] = "1"
            },
            RelationshipIds =
            {
                ["TableRelationshipId"] = "rel:1",
                ["SourceFieldId"] = "3",
                ["TargetFieldId"] = "4"
            }
        });
    }

    private static void AddMetaSchemaTable(Meta.Core.Domain.Workspace workspace, string id, string name, string schemaId)
    {
        workspace.Instance.GetOrCreateEntityRecords("SchemaObject").Add(new Meta.Core.Domain.GenericRecord
        {
            Id = id,
            SourceShardFileName = "SchemaObject.xml",
            Values = { ["Name"] = name },
            RelationshipIds = { ["SchemaId"] = schemaId }
        });
        workspace.Instance.GetOrCreateEntityRecords("Table").Add(new Meta.Core.Domain.GenericRecord
        {
            Id = id,
            SourceShardFileName = "Table.xml",
            RelationshipIds = { ["SchemaObjectId"] = id }
        });
    }

    private static void AddMetaSchemaView(Meta.Core.Domain.Workspace workspace, string id, string name, string schemaId)
    {
        workspace.Instance.GetOrCreateEntityRecords("SchemaObject").Add(new Meta.Core.Domain.GenericRecord
        {
            Id = id,
            SourceShardFileName = "SchemaObject.xml",
            Values = { ["Name"] = name },
            RelationshipIds = { ["SchemaId"] = schemaId }
        });
        workspace.Instance.GetOrCreateEntityRecords("View").Add(new Meta.Core.Domain.GenericRecord
        {
            Id = id,
            SourceShardFileName = "View.xml",
            RelationshipIds = { ["SchemaObjectId"] = id }
        });
    }

    private static void AddMetaSchemaPrimaryKey(Meta.Core.Domain.Workspace workspace, string id, string name, string tableId)
    {
        workspace.Instance.GetOrCreateEntityRecords("Key").Add(new Meta.Core.Domain.GenericRecord
        {
            Id = id,
            SourceShardFileName = "Key.xml",
            Values = { ["Name"] = name },
            RelationshipIds = { ["TableId"] = tableId }
        });
        workspace.Instance.GetOrCreateEntityRecords("PrimaryKey").Add(new Meta.Core.Domain.GenericRecord
        {
            Id = id,
            SourceShardFileName = "PrimaryKey.xml",
            RelationshipIds = { ["KeyId"] = id }
        });
    }

    private static void AddMetaSchemaKeyField(Meta.Core.Domain.Workspace workspace, string id, string keyId, string fieldId, string ordinal)
    {
        workspace.Instance.GetOrCreateEntityRecords("KeyField").Add(new Meta.Core.Domain.GenericRecord
        {
            Id = id,
            SourceShardFileName = "KeyField.xml",
            Values = { ["Ordinal"] = ordinal },
            RelationshipIds =
            {
                ["KeyId"] = keyId,
                ["FieldId"] = fieldId
            }
        });
    }

    private static void AddMetaSchemaField(Meta.Core.Domain.Workspace workspace, string id, string schemaObjectId, string name, string dataTypeId, string ordinal, string? isNullable)
    {
        var values = new Dictionary<string, string>
        {
            ["Name"] = name,
            ["MetaDataTypeId"] = dataTypeId,
            ["Ordinal"] = ordinal,
        };
        if (isNullable != null)
        {
            values["IsNullable"] = isNullable;
        }

        var field = new Meta.Core.Domain.GenericRecord
        {
            Id = id,
            SourceShardFileName = "Field.xml",
            RelationshipIds =
            {
                ["SchemaObjectId"] = schemaObjectId
            }
        };
        foreach (var value in values)
        {
            field.Values[value.Key] = value.Value;
        }

        workspace.Instance.GetOrCreateEntityRecords("Field").Add(field);
    }

    private static (int ExitCode, string Output) RunRawCli(string arguments)
    {
        var repoRoot = FindRepositoryRoot();
        var startInfo = new ProcessStartInfo
        {
            FileName = CliTestSupport.RequireBuiltCli(repoRoot, "MetaDataVault", "Cli", "Raw", "bin", "Debug", "net8.0", "meta-datavault-raw.exe"),
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

    private static void RunRawAdd(string workspacePath, string command)
    {
        var result = RunRawCli($"{command} --workspace \"{workspacePath}\"");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Ok", result.Output, StringComparison.Ordinal);
    }
    private static void RunBusinessAdd(string workspacePath, string command)
    {
        var result = RunBusinessCli($"{command} --workspace \"{workspacePath}\"");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Ok", result.Output, StringComparison.Ordinal);
    }
    private static (int ExitCode, string Output) RunBusinessCli(string arguments, string? workingDirectory = null)
    {
        var repoRoot = FindRepositoryRoot();
        var startInfo = new ProcessStartInfo
        {
            FileName = CliTestSupport.RequireBuiltCli(repoRoot, "MetaDataVault", "Cli", "Business", "bin", "Debug", "net8.0", "meta-datavault-business.exe"),
            Arguments = arguments,
            WorkingDirectory = workingDirectory ?? repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        return RunProcess(startInfo, "Could not start DataVault CLI process.");
    }

    private static (int ExitCode, string Output) RunMetaConvertCli(string arguments)
    {
        var repoRoot = FindRepositoryRoot();
        var startInfo = new ProcessStartInfo
        {
            FileName = CliTestSupport.RequireBuiltCli(repoRoot, "MetaConvert", "Cli", "bin", "Debug", "net8.0", "meta-convert.exe"),
            Arguments = arguments,
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        return RunProcess(startInfo, "Could not start MetaConvert CLI process.");
    }

    private static string GetRawImplementationWorkspacePath()
    {
        return Path.Combine(FindRepositoryRoot(), "MetaDataVault", "Workspaces", "MetaDataVaultImplementation");
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
            if (File.Exists(Path.Combine(directory, "README.md")) && (Directory.Exists(Path.Combine(directory, Path.Combine("MetaDataVault", "Cli", "Raw"))) || Directory.Exists(Path.Combine(directory, Path.Combine("MetaDataVault", "Cli", "Business")))))
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














