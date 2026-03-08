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




