using System.Diagnostics;
using Meta.Core.Services;
using MetaSchema.Core;

namespace MetaDataVault.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsFromMetaSchemaCommand()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("meta-datavault", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("from-metaschema", result.Output);
        Assert.Contains("check-business-materialization", result.Output);
        Assert.Contains("generate-sql", result.Output);
    }

    [Fact]
    public void FromMetaSchema_Help_ShowsRequiredOptions()
    {
        var result = RunCli("from-metaschema --help");

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
        var result = RunCli("check-business-materialization --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--business-workspace <path>", result.Output);
        Assert.Contains("--bdv-workspace <path>", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--weave-workspace <path>", result.Output);
        Assert.Contains("--fabric-workspace <path>", result.Output);
        Assert.Contains("MetaBusinessDataVault", result.Output);
        Assert.Contains("MetaFabric", result.Output);
    }

    [Fact]
    public void MaterializeBusiness_Help_ShowsRequiredOptions()
    {
        var result = RunCli("materialize-business --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--business-workspace <path>", result.Output);
        Assert.Contains("--bdv-workspace <path>", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--weave-workspace <path>", result.Output);
        Assert.Contains("--fabric-workspace <path>", result.Output);
        Assert.Contains("--new-workspace <path>", result.Output);
        Assert.Contains("table name patterns", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GenerateSql_Help_ShowsRequiredOptions()
    {
        var result = RunCli("generate-sql --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("--workspace <path>", result.Output);
        Assert.Contains("--implementation-workspace <path>", result.Output);
        Assert.Contains("--data-type-conversion-workspace <path>", result.Output);
        Assert.Contains("--out <path>", result.Output);
        Assert.Contains("hubs, links, and satellites", result.Output, StringComparison.OrdinalIgnoreCase);
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

            var result = RunCli($"from-metaschema --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\"");
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

            var result = RunCli(
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
        var linkEndWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart");
        var hubKeyPartFabricPath = Path.Combine(repoRoot, "Fabrics", "Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce");
        var linkEndFabricPath = Path.Combine(repoRoot, "Fabrics", "Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart");

        var result = RunCli(
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
        var linkEndWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart");
        var hubKeyPartFabricPath = Path.Combine(repoRoot, "Fabrics", "Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce");
        var linkEndFabricPath = Path.Combine(repoRoot, "Fabrics", "Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart");
        var outputPath = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"), "MaterializedBusinessDataVault");

        try
        {
            var result = RunCli(
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
        var businessPath = Path.Combine(repoRoot, "MetaBusiness.Workspaces", "SampleBusinessCommerceRepeatedKeyPart");
        var bdvPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "SampleBusinessDataVaultCommerceRepeatedKeyPart");
        var implementationPath = Path.Combine(repoRoot, "MetaDataVault.Workspaces", "MetaDataVaultImplementation");
        var conversionPath = Path.Combine(repoRoot, "MetaDataTypeConversion.Workspaces", "MetaDataTypeConversion");
        var hubObjectWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-HubObject-Commerce-RepeatedKeyPart");
        var hubKeyPartWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce");
        var linkRelationshipWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-LinkRelationship-Commerce-RepeatedKeyPart");
        var linkEndWeavePath = Path.Combine(repoRoot, "Weaves", "Weave-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart");
        var hubKeyPartFabricPath = Path.Combine(repoRoot, "Fabrics", "Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-HubKeyPart-KeyPart-Commerce");
        var linkEndFabricPath = Path.Combine(repoRoot, "Fabrics", "Fabric-Scoped-MetaBusiness-MetaBusinessDataVault-LinkEndParticipant-Commerce-RepeatedKeyPart");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var materializedPath = Path.Combine(root, "MaterializedBusinessDataVault");
        var sqlOutputPath = Path.Combine(root, "Sql");

        try
        {
            var materializeResult = RunCli(
                $"materialize-business --business-workspace \"{businessPath}\" --bdv-workspace \"{bdvPath}\" --implementation-workspace \"{implementationPath}\" --weave-workspace \"{hubObjectWeavePath}\" --weave-workspace \"{hubKeyPartWeavePath}\" --weave-workspace \"{linkRelationshipWeavePath}\" --weave-workspace \"{linkEndWeavePath}\" --fabric-workspace \"{hubKeyPartFabricPath}\" --fabric-workspace \"{linkEndFabricPath}\" --new-workspace \"{materializedPath}\"");

            Assert.Equal(0, materializeResult.ExitCode);

            var result = RunCli(
                $"generate-sql --workspace \"{materializedPath}\" --implementation-workspace \"{implementationPath}\" --data-type-conversion-workspace \"{conversionPath}\" --out \"{sqlOutputPath}\"");

            Assert.Equal(0, result.ExitCode);
            Assert.Contains("OK: business datavault sql generated", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Files: 5", result.Output);

            var customerHubSql = await File.ReadAllTextAsync(Path.Combine(sqlOutputPath, "BH_Customer.sql"));
            Assert.Contains("CREATE TABLE [BH_Customer]", customerHubSql);
            Assert.Contains("[HashKey] binary(16) NOT NULL", customerHubSql);
            Assert.Contains("[Identifier] nvarchar(50) NOT NULL", customerHubSql);
            Assert.Contains("[LoadTimestamp] datetime2(7) NOT NULL", customerHubSql);
            Assert.Contains("[RecordSource] nvarchar(256) NOT NULL", customerHubSql);

            var customerOrderLinkSql = await File.ReadAllTextAsync(Path.Combine(sqlOutputPath, "BL_CustomerOrder.sql"));
            Assert.Contains("CREATE TABLE [BL_CustomerOrder]", customerOrderLinkSql);
            Assert.Contains("[CustomerHashKey] binary(16) NOT NULL", customerOrderLinkSql);
            Assert.Contains("[OrderHashKey] binary(16) NOT NULL", customerOrderLinkSql);
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

    private static (int ExitCode, string Output) RunCli(string arguments)
    {
        var repoRoot = FindRepositoryRoot();
        var cliPath = ResolveCliPath(repoRoot);
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

        return RunProcess(startInfo, "Could not start meta-datavault CLI process.");
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

    private static string ResolveCliPath(string repoRoot)
    {
        var cliPath = Path.Combine(repoRoot, "MetaDataVault.Cli", "bin", "Debug", "net8.0", "meta-datavault.exe");
        if (!File.Exists(cliPath))
        {
            throw new FileNotFoundException($"Could not find compiled MetaDataVault CLI at '{cliPath}'. Build MetaDataVault.Cli before running tests.");
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
            if (File.Exists(Path.Combine(directory, "README.md")) && Directory.Exists(Path.Combine(directory, "MetaDataVault.Cli")))
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





