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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaDataVault", "Cli", "Raw"), "meta-datavault-raw.dll");
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{cliPath}\" {arguments}",
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
        Assert.Contains("OK: raw datavault row added", result.Output, StringComparison.OrdinalIgnoreCase);
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
        var cliPath = ResolveCliPath(repoRoot, Path.Combine("MetaDataVault", "Cli", "Business"), "meta-datavault-business.dll");
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{cliPath}\" {arguments}",
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













