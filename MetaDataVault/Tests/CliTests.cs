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
            var createResult = RunBusinessCli($"--new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            RunBusinessAdd(workspacePath, "add-hub --id Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-hub --id Order --name Order");
            RunBusinessAdd(workspacePath, "add-hub --id CustomerAlias --name CustomerAlias");
            RunBusinessAdd(workspacePath, "add-hub --id ParentNode --name ParentNode");
            RunBusinessAdd(workspacePath, "add-hub --id ChildNode --name ChildNode");

            RunBusinessAdd(workspacePath, "add-hub-key-part --id CustomerIdentifier --hub Customer --name Identifier --data-type-id meta:type:String --ordinal 1 --length 50");
            RunBusinessAdd(workspacePath, "add-hub-key-part --id OrderIdentifier --hub Order --name Identifier --data-type-id meta:type:String --ordinal 1");
            RunBusinessAdd(workspacePath, "add-hub-key-part --id CustomerAliasIdentifier --hub CustomerAlias --name Identifier --data-type-id meta:type:String --ordinal 1");

            RunBusinessAdd(workspacePath, "add-link --id CustomerOrder --name CustomerOrder");
            RunBusinessAdd(workspacePath, "add-link-hub --id CustomerOrderCustomer --link CustomerOrder --hub Customer --ordinal 1 --role-name Customer");
            RunBusinessAdd(workspacePath, "add-link-hub --id CustomerOrderOrder --link CustomerOrder --hub Order --ordinal 2 --role-name Order");

            RunBusinessAdd(workspacePath, "add-same-as-link --id CustomerSameAsAlias --name CustomerSameAsAlias --primary-hub Customer --equivalent-hub CustomerAlias");
            RunBusinessAdd(workspacePath, "add-hierarchical-link --id ParentChild --name ParentChild --parent-hub ParentNode --child-hub ChildNode");

            RunBusinessAdd(workspacePath, "add-reference --id StatusCode --name StatusCode");
            RunBusinessAdd(workspacePath, "add-reference-key-part --id StatusCodeValue --reference StatusCode --name Code --data-type-id meta:type:String --ordinal 1 --length 20");

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
            RunBusinessAdd(workspacePath, "add-bridge --id CustomerOrderTraversal --anchor-hub Customer --name CustomerOrderTraversal");
            RunBusinessAdd(workspacePath, "add-bridge-link --id CustomerOrderTraversalCustomerOrder --bridge CustomerOrderTraversal --link CustomerOrder --ordinal 1 --role-name CustomerOrder");
            RunBusinessAdd(workspacePath, "add-bridge-hub --id CustomerOrderTraversalOrder --bridge CustomerOrderTraversal --hub Order --ordinal 2 --role-name Order");

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
            var createResult = RunBusinessCli($"--new-workspace \"{workspacePath}\"");
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
    public void BusinessAuthoringRejectsDuplicateBridgeOrdinal()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            var createResult = RunBusinessCli($"--new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            RunBusinessAdd(workspacePath, "add-hub --id Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-hub --id Order --name Order");
            RunBusinessAdd(workspacePath, "add-link --id CustomerOrder --name CustomerOrder");
            RunBusinessAdd(workspacePath, "add-link-hub --id CustomerOrderCustomer --link CustomerOrder --hub Customer --ordinal 1 --role-name Customer");
            RunBusinessAdd(workspacePath, "add-link-hub --id CustomerOrderOrder --link CustomerOrder --hub Order --ordinal 2 --role-name Order");
            RunBusinessAdd(workspacePath, "add-bridge --id CustomerOrderTraversal --anchor-hub Customer --name CustomerOrderTraversal");
            RunBusinessAdd(workspacePath, "add-bridge-link --id CustomerOrderTraversalCustomerOrder --bridge CustomerOrderTraversal --link CustomerOrder --ordinal 1 --role-name CustomerOrder");

            var result = RunBusinessCli($"add-bridge-hub --workspace \"{workspacePath}\" --id CustomerOrderTraversalOrder --bridge CustomerOrderTraversal --hub Order --ordinal 1 --role-name Order");
            Assert.NotEqual(0, result.ExitCode);
            Assert.Contains("Bridge 'CustomerOrderTraversal' already contains ordinal '1'", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task BusinessAuthoringAppendsOrdinalWhenOmitted()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");

        try
        {
            Assert.Equal(0, RunBusinessCli($"--new-workspace \"{workspacePath}\"").ExitCode);

            RunBusinessAdd(workspacePath, "add-hub --id Customer --name Customer");
            RunBusinessAdd(workspacePath, "add-hub --id Order --name Order");
            RunBusinessAdd(workspacePath, "add-link --id CustomerOrder --name CustomerOrder");
            RunBusinessAdd(workspacePath, "add-link-hub --id CustomerOrderCustomer --link CustomerOrder --hub Customer --role-name Customer");
            RunBusinessAdd(workspacePath, "add-link-hub --id CustomerOrderOrder --link CustomerOrder --hub Order --role-name Order");
            RunBusinessAdd(workspacePath, "add-hub-satellite --id CustomerProfile --hub Customer --name CustomerProfile");
            RunBusinessAdd(workspacePath, "add-link-satellite --id CustomerOrderStatus --link CustomerOrder --name CustomerOrderStatus");
            RunBusinessAdd(workspacePath, "add-link-satellite-attribute --id CustomerOrderStatusCode --link-satellite CustomerOrderStatus --name StatusCode --data-type-id meta:type:String");
            RunBusinessAdd(workspacePath, "add-link-satellite-attribute --id CustomerOrderStatusReason --link-satellite CustomerOrderStatus --name StatusReason --data-type-id meta:type:String");
            RunBusinessAdd(workspacePath, "add-point-in-time --id CustomerSnapshot --hub Customer --name CustomerSnapshot");
            RunBusinessAdd(workspacePath, "add-point-in-time-hub-satellite --id CustomerSnapshotProfile --point-in-time CustomerSnapshot --hub-satellite CustomerProfile");
            RunBusinessAdd(workspacePath, "add-point-in-time-link-satellite --id CustomerSnapshotOrderStatus --point-in-time CustomerSnapshot --link-satellite CustomerOrderStatus");
            RunBusinessAdd(workspacePath, "add-bridge --id CustomerOrderTraversal --anchor-hub Customer --name CustomerOrderTraversal");
            RunBusinessAdd(workspacePath, "add-bridge-link --id CustomerOrderTraversalCustomerOrder --bridge CustomerOrderTraversal --link CustomerOrder --role-name CustomerOrder");
            RunBusinessAdd(workspacePath, "add-bridge-hub --id CustomerOrderTraversalOrder --bridge CustomerOrderTraversal --hub Order --role-name Order");

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            var linkHubs = workspace.Instance.GetOrCreateEntityRecords("BusinessLinkHub").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var linkSatelliteAttributes = workspace.Instance.GetOrCreateEntityRecords("BusinessLinkSatelliteAttribute").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var pointInTimeHubSatellites = workspace.Instance.GetOrCreateEntityRecords("BusinessPointInTimeHubSatellite").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var pointInTimeLinkSatellites = workspace.Instance.GetOrCreateEntityRecords("BusinessPointInTimeLinkSatellite").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var bridgeLinks = workspace.Instance.GetOrCreateEntityRecords("BusinessBridgeLink").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var bridgeHubs = workspace.Instance.GetOrCreateEntityRecords("BusinessBridgeHub").ToDictionary(row => row.Id, StringComparer.Ordinal);

            Assert.Equal("1", linkHubs["CustomerOrderCustomer"].Values["Ordinal"]);
            Assert.Equal("2", linkHubs["CustomerOrderOrder"].Values["Ordinal"]);
            Assert.Equal("1", linkSatelliteAttributes["CustomerOrderStatusCode"].Values["Ordinal"]);
            Assert.Equal("2", linkSatelliteAttributes["CustomerOrderStatusReason"].Values["Ordinal"]);
            Assert.Equal("1", pointInTimeHubSatellites["CustomerSnapshotProfile"].Values["Ordinal"]);
            Assert.Equal("2", pointInTimeLinkSatellites["CustomerSnapshotOrderStatus"].Values["Ordinal"]);
            Assert.Equal("1", bridgeLinks["CustomerOrderTraversalCustomerOrder"].Values["Ordinal"]);
            Assert.Equal("2", bridgeHubs["CustomerOrderTraversalOrder"].Values["Ordinal"]);
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
            var createResult = RunRawCli($"--new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            RunRawAdd(workspacePath, "add-source-system --id Sales --name Sales");
            RunRawAdd(workspacePath, "add-source-schema --id dbo --system Sales --name dbo");
            RunRawAdd(workspacePath, "add-source-table --id CustomerTable --schema dbo --name Customer");
            RunRawAdd(workspacePath, "add-source-table --id OrderTable --schema dbo --name [Order]");
            RunRawAdd(workspacePath, "add-source-field --id CustomerIdField --table CustomerTable --name CustomerId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false");
            RunRawAdd(workspacePath, "add-source-field-data-type-detail --id CustomerIdFieldLength --field CustomerIdField --name Length --value 50");
            RunRawAdd(workspacePath, "add-source-field --id CustomerNameField --table CustomerTable --name CustomerName --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable true");
            RunRawAdd(workspacePath, "add-source-field-data-type-detail --id CustomerNameFieldLength --field CustomerNameField --name Length --value 200");
            RunRawAdd(workspacePath, "add-source-field --id OrderIdField --table OrderTable --name OrderId --data-type-id sqlserver:type:nvarchar --ordinal 1 --is-nullable false");
            RunRawAdd(workspacePath, "add-source-field-data-type-detail --id OrderIdFieldLength --field OrderIdField --name Length --value 50");
            RunRawAdd(workspacePath, "add-source-field --id OrderCustomerIdField --table OrderTable --name CustomerId --data-type-id sqlserver:type:nvarchar --ordinal 2 --is-nullable false");
            RunRawAdd(workspacePath, "add-source-field-data-type-detail --id OrderCustomerIdFieldLength --field OrderCustomerIdField --name Length --value 50");
            RunRawAdd(workspacePath, "add-source-field --id OrderStatusField --table OrderTable --name StatusCode --data-type-id sqlserver:type:nvarchar --ordinal 3 --is-nullable false");
            RunRawAdd(workspacePath, "add-source-field-data-type-detail --id OrderStatusFieldLength --field OrderStatusField --name Length --value 20");
            RunRawAdd(workspacePath, "add-source-table-relationship --id OrderCustomerRelationship --source-table OrderTable --target-table CustomerTable --name FK_Order_Customer");
            RunRawAdd(workspacePath, "add-source-table-relationship-field --id OrderCustomerRelationshipField --relationship OrderCustomerRelationship --source-field OrderCustomerIdField --target-field CustomerIdField --ordinal 1");
            RunRawAdd(workspacePath, "add-hub --id CustomerHub --source-table CustomerTable --name Customer");
            RunRawAdd(workspacePath, "add-hub --id OrderHub --source-table OrderTable --name Order");
            RunRawAdd(workspacePath, "add-hub-key-part --id CustomerHubKey --hub CustomerHub --source-field CustomerIdField --name CustomerId --ordinal 1");
            RunRawAdd(workspacePath, "add-hub-key-part --id OrderHubKey --hub OrderHub --source-field OrderIdField --name OrderId --ordinal 1");
            RunRawAdd(workspacePath, "add-hub-satellite --id CustomerProfileSat --hub CustomerHub --source-table CustomerTable --name CustomerProfile --satellite-kind standard");
            RunRawAdd(workspacePath, "add-hub-satellite-attribute --id CustomerNameAttr --hub-satellite CustomerProfileSat --source-field CustomerNameField --name CustomerName --ordinal 1");
            RunRawAdd(workspacePath, "add-link --id OrderCustomerLink --source-relationship OrderCustomerRelationship --name OrderCustomer --link-kind standard");
            RunRawAdd(workspacePath, "add-link-hub --id OrderCustomerLinkOrder --link OrderCustomerLink --hub OrderHub --ordinal 1 --role-name Order");
            RunRawAdd(workspacePath, "add-link-hub --id OrderCustomerLinkCustomer --link OrderCustomerLink --hub CustomerHub --ordinal 2 --role-name Customer");
            RunRawAdd(workspacePath, "add-link-satellite --id OrderCustomerStatusSat --link OrderCustomerLink --source-table OrderTable --name OrderCustomerStatus --satellite-kind standard");
            RunRawAdd(workspacePath, "add-link-satellite-attribute --id OrderCustomerStatusCodeAttr --link-satellite OrderCustomerStatusSat --source-field OrderStatusField --name StatusCode --ordinal 1");

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawHub").Count);
            Assert.Equal(1, workspace.Instance.GetOrCreateEntityRecords("RawLink").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawLinkHub").Count);
            Assert.Equal(1, workspace.Instance.GetOrCreateEntityRecords("RawHubSatellite").Count);
            Assert.Equal(1, workspace.Instance.GetOrCreateEntityRecords("RawLinkSatellite").Count);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task RawAuthoringAppendsOrdinalWhenOmitted()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");

        try
        {
            Assert.Equal(0, RunRawCli($"--new-workspace \"{workspacePath}\"").ExitCode);

            RunRawAdd(workspacePath, "add-source-system --id Sales --name Sales");
            RunRawAdd(workspacePath, "add-source-schema --id dbo --system Sales --name dbo");
            RunRawAdd(workspacePath, "add-source-table --id CustomerTable --schema dbo --name Customer");
            RunRawAdd(workspacePath, "add-source-table --id OrderTable --schema dbo --name [Order]");
            RunRawAdd(workspacePath, "add-source-field --id CustomerIdField --table CustomerTable --name CustomerId --data-type-id sqlserver:type:nvarchar --is-nullable false");
            RunRawAdd(workspacePath, "add-source-field --id CustomerNameField --table CustomerTable --name CustomerName --data-type-id sqlserver:type:nvarchar --is-nullable true");
            RunRawAdd(workspacePath, "add-source-field --id OrderIdField --table OrderTable --name OrderId --data-type-id sqlserver:type:nvarchar --is-nullable false");
            RunRawAdd(workspacePath, "add-source-field --id OrderCustomerIdField --table OrderTable --name CustomerId --data-type-id sqlserver:type:nvarchar --is-nullable false");
            RunRawAdd(workspacePath, "add-source-field --id OrderStatusField --table OrderTable --name StatusCode --data-type-id sqlserver:type:nvarchar --is-nullable false");
            RunRawAdd(workspacePath, "add-source-table-relationship --id OrderCustomerRelationship --source-table OrderTable --target-table CustomerTable --name FK_Order_Customer");
            RunRawAdd(workspacePath, "add-source-table-relationship-field --id OrderCustomerRelationshipField --relationship OrderCustomerRelationship --source-field OrderCustomerIdField --target-field CustomerIdField");
            RunRawAdd(workspacePath, "add-hub --id CustomerHub --source-table CustomerTable --name Customer");
            RunRawAdd(workspacePath, "add-hub --id OrderHub --source-table OrderTable --name Order");
            RunRawAdd(workspacePath, "add-hub-key-part --id CustomerHubKey --hub CustomerHub --source-field CustomerIdField --name CustomerId");
            RunRawAdd(workspacePath, "add-hub-satellite --id CustomerProfileSat --hub CustomerHub --source-table CustomerTable --name CustomerProfile --satellite-kind standard");
            RunRawAdd(workspacePath, "add-hub-satellite-attribute --id CustomerNameAttr --hub-satellite CustomerProfileSat --source-field CustomerNameField --name CustomerName");
            RunRawAdd(workspacePath, "add-link --id OrderCustomerLink --source-relationship OrderCustomerRelationship --name OrderCustomer --link-kind standard");
            RunRawAdd(workspacePath, "add-link-hub --id OrderCustomerLinkOrder --link OrderCustomerLink --hub OrderHub --role-name Order");
            RunRawAdd(workspacePath, "add-link-hub --id OrderCustomerLinkCustomer --link OrderCustomerLink --hub CustomerHub --role-name Customer");
            RunRawAdd(workspacePath, "add-link-satellite --id OrderCustomerStatusSat --link OrderCustomerLink --source-table OrderTable --name OrderCustomerStatus --satellite-kind standard");
            RunRawAdd(workspacePath, "add-link-satellite-attribute --id OrderCustomerStatusCodeAttr --link-satellite OrderCustomerStatusSat --source-field OrderStatusField --name StatusCode");

            var workspace = await new WorkspaceService().LoadAsync(workspacePath, searchUpward: false);
            var sourceFields = workspace.Instance.GetOrCreateEntityRecords("SourceField").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var relationshipFields = workspace.Instance.GetOrCreateEntityRecords("SourceTableRelationshipField").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var hubKeyParts = workspace.Instance.GetOrCreateEntityRecords("RawHubKeyPart").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var linkHubs = workspace.Instance.GetOrCreateEntityRecords("RawLinkHub").ToDictionary(row => row.Id, StringComparer.Ordinal);
            var linkSatelliteAttributes = workspace.Instance.GetOrCreateEntityRecords("RawLinkSatelliteAttribute").ToDictionary(row => row.Id, StringComparer.Ordinal);

            Assert.Equal("1", sourceFields["CustomerIdField"].Values["Ordinal"]);
            Assert.Equal("2", sourceFields["CustomerNameField"].Values["Ordinal"]);
            Assert.Equal("1", sourceFields["OrderIdField"].Values["Ordinal"]);
            Assert.Equal("2", sourceFields["OrderCustomerIdField"].Values["Ordinal"]);
            Assert.Equal("3", sourceFields["OrderStatusField"].Values["Ordinal"]);
            Assert.Equal("1", relationshipFields["OrderCustomerRelationshipField"].Values["Ordinal"]);
            Assert.Equal("1", hubKeyParts["CustomerHubKey"].Values["Ordinal"]);
            Assert.Equal("1", linkHubs["OrderCustomerLinkOrder"].Values["Ordinal"]);
            Assert.Equal("2", linkHubs["OrderCustomerLinkCustomer"].Values["Ordinal"]);
            Assert.Equal("1", linkSatelliteAttributes["OrderCustomerStatusCodeAttr"].Values["Ordinal"]);
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
                ["MetaDataTypeId"] = "sqlserver:type:int",
                ["Ordinal"] = "1",
                ["IsNullable"] = "false"
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
                ["MetaDataTypeId"] = "sqlserver:type:nvarchar",
                ["Ordinal"] = "2",
                ["IsNullable"] = "false"
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
                ["MetaDataTypeId"] = "sqlserver:type:int",
                ["Ordinal"] = "3",
                ["IsNullable"] = "false"
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
                ["MetaDataTypeId"] = "sqlserver:type:int",
                ["Ordinal"] = "1",
                ["IsNullable"] = "false"
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
                ["MetaDataTypeId"] = "sqlserver:type:nvarchar",
                ["Ordinal"] = "2",
                ["IsNullable"] = "true"
            },
            RelationshipIds =
            {
                ["TableId"] = "2"
            }
        });

        var tableKeys = workspace.Instance.GetOrCreateEntityRecords("TableKey");
        tableKeys.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "key:1",
            SourceShardFileName = "TableKey.xml",
            Values =
            {
                ["Name"] = "PK_Order",
                ["KeyType"] = "primary"
            },
            RelationshipIds =
            {
                ["TableId"] = "1"
            }
        });
        tableKeys.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "key:2",
            SourceShardFileName = "TableKey.xml",
            Values =
            {
                ["Name"] = "PK_Customer",
                ["KeyType"] = "primary"
            },
            RelationshipIds =
            {
                ["TableId"] = "2"
            }
        });

        var tableKeyFields = workspace.Instance.GetOrCreateEntityRecords("TableKeyField");
        tableKeyFields.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "keyf:1",
            SourceShardFileName = "TableKeyField.xml",
            Values =
            {
                ["Ordinal"] = "1",
                ["FieldName"] = "OrderId"
            },
            RelationshipIds =
            {
                ["TableKeyId"] = "key:1",
                ["FieldId"] = "1"
            }
        });
        tableKeyFields.Add(new Meta.Core.Domain.GenericRecord
        {
            Id = "keyf:2",
            SourceShardFileName = "TableKeyField.xml",
            Values =
            {
                ["Ordinal"] = "1",
                ["FieldName"] = "CustomerId"
            },
            RelationshipIds =
            {
                ["TableKeyId"] = "key:2",
                ["FieldId"] = "4"
            }
        });

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

    private static void AddMetaSchemaField(Meta.Core.Domain.Workspace workspace, string id, string tableId, string name, string dataTypeId, string ordinal, string isNullable)
    {
        workspace.Instance.GetOrCreateEntityRecords("Field").Add(new Meta.Core.Domain.GenericRecord
        {
            Id = id,
            SourceShardFileName = "Field.xml",
            Values =
            {
                ["Name"] = name,
                ["MetaDataTypeId"] = dataTypeId,
                ["Ordinal"] = ordinal,
                ["IsNullable"] = isNullable
            },
            RelationshipIds =
            {
                ["TableId"] = tableId
            }
        });
    }

    private static (int ExitCode, string Output) RunRawCli(string arguments)
    {
        var repoRoot = FindRepositoryRoot();
        var startInfo = new ProcessStartInfo
        {
            FileName = "meta-datavault-raw",
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
        Assert.Contains("OK: Added ", result.Output, StringComparison.OrdinalIgnoreCase);
    }
    private static void RunBusinessAdd(string workspacePath, string command)
    {
        var result = RunBusinessCli($"{command} --workspace \"{workspacePath}\"");
        Assert.Equal(0, result.ExitCode);
        Assert.Contains("OK: Added ", result.Output, StringComparison.OrdinalIgnoreCase);
    }
    private static (int ExitCode, string Output) RunBusinessCli(string arguments)
    {
        var repoRoot = FindRepositoryRoot();
        var startInfo = new ProcessStartInfo
        {
            FileName = "meta-datavault-business",
            Arguments = arguments,
            WorkingDirectory = repoRoot,
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
        var localExePath = Path.Combine(repoRoot, "MetaConvert", "Cli", "bin", "Debug", "net8.0", "meta-convert.exe");
        var fileName = File.Exists(localExePath) ? localExePath : "meta-convert";
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
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














