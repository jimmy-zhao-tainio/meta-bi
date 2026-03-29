using Meta.Core.Services;
using MetaSql;
using MetaSchema.Core;

namespace MetaDataVault.Tests;

public sealed partial class CliTests
{
    [Fact]
    public async Task FromMetaSchema_MaterializesRawDataVault()
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

            var result = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\" --verbose");
            Assert.Equal(0, result.ExitCode);
            Assert.Contains("OK: Created RawDataVault", result.Output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Summary", result.Output, StringComparison.OrdinalIgnoreCase);

            var workspace = await new WorkspaceService().LoadAsync(targetPath, searchUpward: false);
            Assert.Equal("MetaRawDataVault", workspace.Model.Name);

            Assert.Equal(1, workspace.Instance.GetOrCreateEntityRecords("SourceSystem").Count);
            Assert.Equal(1, workspace.Instance.GetOrCreateEntityRecords("SourceSchema").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("SourceTable").Count);
            Assert.Equal(5, workspace.Instance.GetOrCreateEntityRecords("SourceField").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawHub").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawHubKeyPart").Count);
            Assert.Equal(1, workspace.Instance.GetOrCreateEntityRecords("RawLink").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawLinkHub").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawHubSatellite").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawHubSatelliteAttribute").Count);
            Assert.Empty(workspace.Instance.GetOrCreateEntityRecords("RawLinkSatellite"));

            var rawLinks = workspace.Instance.GetOrCreateEntityRecords("RawLink").ToDictionary(record => record.Id, StringComparer.Ordinal);
            Assert.Equal("OrderCustomer", rawLinks["rawlink:rel:1"].Values["Name"]);

            var rawLinkHubs = workspace.Instance.GetOrCreateEntityRecords("RawLinkHub").ToDictionary(record => record.Id, StringComparer.Ordinal);
            Assert.Equal("Order", rawLinkHubs["rawlink:rel:1:source"].Values["RoleName"]);
            Assert.Equal("Customer", rawLinkHubs["rawlink:rel:1:target"].Values["RoleName"]);

            var rawHubSatellites = workspace.Instance.GetOrCreateEntityRecords("RawHubSatellite").ToDictionary(record => record.Id, StringComparer.Ordinal);
            Assert.Equal("Order", rawHubSatellites["rawhub:1:sat"].Values["Name"]);
            Assert.Equal("Customer", rawHubSatellites["rawhub:2:sat"].Values["Name"]);

            var hubSatelliteAttributes = workspace.Instance.GetOrCreateEntityRecords("RawHubSatelliteAttribute").ToDictionary(record => record.Id, StringComparer.Ordinal);
            Assert.Equal("OrderNumber", hubSatelliteAttributes["rawhub:1:sat:attr:2"].Values["Name"]);
            Assert.Equal("CustomerName", hubSatelliteAttributes["rawhub:2:sat:attr:5"].Values["Name"]);

            var reportPath = Path.Combine(targetPath, "materialization-report.md");
            Assert.False(File.Exists(reportPath));
            Assert.Contains("Summary", result.Output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task FromMetaSchema_IncludesViewsOnlyWhenRequested()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(root, "MetaSchemaSource");
        var defaultTargetPath = Path.Combine(root, "RawDataVault_Default");
        var includeViewsTargetPath = Path.Combine(root, "RawDataVault_WithViews");

        try
        {
            Directory.CreateDirectory(sourcePath);
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace(sourcePath);
            SeedMetaSchema(source);

            source.Instance.GetOrCreateEntityRecords("Table").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "view:1",
                SourceShardFileName = "Table.xml",
                Values =
                {
                    ["Name"] = "CustomerView",
                    ["ObjectType"] = "View"
                },
                RelationshipIds =
                {
                    ["SchemaId"] = "1"
                }
            });
            AddMetaSchemaField(source, "view-field:1", "view:1", "CustomerViewId", "sqlserver:type:int", "1", "false");
            AddMetaSchemaField(source, "view-field:2", "view:1", "CustomerViewName", "sqlserver:type:nvarchar", "2", "true");
            source.Instance.GetOrCreateEntityRecords("TableKey").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "view-key:1",
                SourceShardFileName = "TableKey.xml",
                Values =
                {
                    ["Name"] = "PK_CustomerView",
                    ["KeyType"] = "primary"
                },
                RelationshipIds =
                {
                    ["TableId"] = "view:1"
                }
            });
            source.Instance.GetOrCreateEntityRecords("TableKeyField").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "view-keyf:1",
                SourceShardFileName = "TableKeyField.xml",
                Values =
                {
                    ["Ordinal"] = "1",
                    ["FieldName"] = "CustomerViewId"
                },
                RelationshipIds =
                {
                    ["TableKeyId"] = "view-key:1",
                    ["FieldId"] = "view-field:1"
                }
            });

            await new WorkspaceService().SaveAsync(source);

            var defaultResult = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{defaultTargetPath}\"");
            Assert.Equal(0, defaultResult.ExitCode);

            var includeViewsResult = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{includeViewsTargetPath}\" --include-views");
            Assert.Equal(0, includeViewsResult.ExitCode);
            Assert.DoesNotContain("Error:", includeViewsResult.Output, StringComparison.OrdinalIgnoreCase);

            var defaultWorkspace = await new WorkspaceService().LoadAsync(defaultTargetPath, searchUpward: false);
            var includeViewsWorkspace = await new WorkspaceService().LoadAsync(includeViewsTargetPath, searchUpward: false);

            Assert.Equal(2, defaultWorkspace.Instance.GetOrCreateEntityRecords("SourceTable").Count);
            Assert.Equal(3, includeViewsWorkspace.Instance.GetOrCreateEntityRecords("SourceTable").Count);
            Assert.Equal(2, defaultWorkspace.Instance.GetOrCreateEntityRecords("RawHub").Count);
            Assert.Equal(3, includeViewsWorkspace.Instance.GetOrCreateEntityRecords("RawHub").Count);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task FromMetaSchema_KeepsRecognizedTechnicalFieldsWithoutExplicitIgnoreSwitches()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(root, "MetaSchemaSource");
        var targetPath = Path.Combine(root, "RawDataVault");

        try
        {
            Directory.CreateDirectory(sourcePath);
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace(sourcePath);
            SeedMetaSchema(source);
            AddMetaSchemaField(source, "6", "1", "AuditId", "sqlserver:type:uniqueidentifier", "4", "false");
            await new WorkspaceService().SaveAsync(source);

            var result = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\"");

            Assert.Equal(0, result.ExitCode);

            var workspace = await new WorkspaceService().LoadAsync(targetPath, searchUpward: false);
            Assert.Equal(6, workspace.Instance.GetOrCreateEntityRecords("SourceField").Count);
            Assert.Equal(3, workspace.Instance.GetOrCreateEntityRecords("RawHubSatelliteAttribute").Count);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task FromMetaSchema_AllowsExplicitIgnoreSwitchesForRecognizedTechnicalFields()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(root, "MetaSchemaSource");
        var targetPath = Path.Combine(root, "RawDataVault");

        try
        {
            Directory.CreateDirectory(sourcePath);
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace(sourcePath);
            SeedMetaSchema(source);
            AddMetaSchemaField(source, "6", "1", "OrderHashKey", "sqlserver:type:varbinary", "4", "false");
            await new WorkspaceService().SaveAsync(source);

            var result = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\" --ignore-field-suffix HashKey");

            Assert.Equal(0, result.ExitCode);
            Assert.DoesNotContain("Error:", result.Output, StringComparison.OrdinalIgnoreCase);

            var workspace = await new WorkspaceService().LoadAsync(targetPath, searchUpward: false);
            Assert.Equal(6, workspace.Instance.GetOrCreateEntityRecords("SourceField").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawHubSatelliteAttribute").Count);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task FromMetaSchema_UsesSourceKeysEvenWhenFieldNameLooksTechnical()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(root, "MetaSchemaSource");
        var targetPath = Path.Combine(root, "RawDataVault");

        try
        {
            Directory.CreateDirectory(sourcePath);
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace(sourcePath);
            SeedMetaSchema(source);

            source.Instance.GetOrCreateEntityRecords("Table").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "3",
                SourceShardFileName = "Table.xml",
                Values =
                {
                    ["Name"] = "HashDriven"
                },
                RelationshipIds =
                {
                    ["SchemaId"] = "1"
                }
            });

            AddMetaSchemaField(source, "6", "3", "OrderHashKey", "sqlserver:type:varbinary", "1", "false");
            AddMetaSchemaField(source, "7", "3", "Description", "sqlserver:type:nvarchar", "2", "true");

            source.Instance.GetOrCreateEntityRecords("TableKey").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "key:3",
                SourceShardFileName = "TableKey.xml",
                Values =
                {
                    ["Name"] = "PK_HashDriven",
                    ["KeyType"] = "primary"
                },
                RelationshipIds =
                {
                    ["TableId"] = "3"
                }
            });

            source.Instance.GetOrCreateEntityRecords("TableKeyField").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "keyf:3",
                SourceShardFileName = "TableKeyField.xml",
                Values =
                {
                    ["Ordinal"] = "1",
                    ["FieldName"] = "OrderHashKey"
                },
                RelationshipIds =
                {
                    ["TableKeyId"] = "key:3",
                    ["FieldId"] = "6"
                }
            });

            await new WorkspaceService().SaveAsync(source);

            var result = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\"");

            Assert.Equal(0, result.ExitCode);

            var workspace = await new WorkspaceService().LoadAsync(targetPath, searchUpward: false);
            Assert.Equal(3, workspace.Instance.GetOrCreateEntityRecords("RawHub").Count);

            var rawHubKeyParts = workspace.Instance.GetOrCreateEntityRecords("RawHubKeyPart").ToDictionary(record => record.Id, StringComparer.Ordinal);
            Assert.Equal("OrderHashKey", rawHubKeyParts["rawhub:3:key:6"].Values["Name"]);

            var rawHubSatelliteAttributes = workspace.Instance.GetOrCreateEntityRecords("RawHubSatelliteAttribute").ToDictionary(record => record.Id, StringComparer.Ordinal);
            Assert.Equal("Description", rawHubSatelliteAttributes["rawhub:3:sat:attr:7"].Values["Name"]);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task FromMetaSchema_DisambiguatesMultipleRelationshipsBetweenSameSourceAndTargetTables()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(root, "MetaSchemaSource");
        var targetPath = Path.Combine(root, "RawDataVault");
        var currentMetaSqlPath = Path.Combine(root, "CurrentMetaSql");
        var implementationPath = GetRawImplementationWorkspacePath();

        try
        {
            Directory.CreateDirectory(sourcePath);
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace(sourcePath);

            source.Instance.GetOrCreateEntityRecords("System").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "1",
                SourceShardFileName = "System.xml",
                Values = { ["Name"] = "Sales" }
            });

            source.Instance.GetOrCreateEntityRecords("Schema").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "1",
                SourceShardFileName = "Schema.xml",
                Values = { ["Name"] = "dbo" },
                RelationshipIds = { ["SystemId"] = "1" }
            });

            source.Instance.GetOrCreateEntityRecords("Table").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "1",
                SourceShardFileName = "Table.xml",
                Values = { ["Name"] = "DepartmentHierarchy" },
                RelationshipIds = { ["SchemaId"] = "1" }
            });
            source.Instance.GetOrCreateEntityRecords("Table").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "2",
                SourceShardFileName = "Table.xml",
                Values = { ["Name"] = "Department" },
                RelationshipIds = { ["SchemaId"] = "1" }
            });

            AddMetaSchemaField(source, "1", "1", "DepartmentHierarchyId", "sqlserver:type:int", "1", "false");
            AddMetaSchemaField(source, "2", "1", "ParentDepartmentId", "sqlserver:type:int", "2", "false");
            AddMetaSchemaField(source, "3", "1", "ChildDepartmentId", "sqlserver:type:int", "3", "false");
            AddMetaSchemaField(source, "4", "2", "DepartmentId", "sqlserver:type:int", "1", "false");
            AddMetaSchemaField(source, "5", "2", "DepartmentName", "sqlserver:type:nvarchar", "2", "true");

            source.Instance.GetOrCreateEntityRecords("TableKey").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "key:1",
                SourceShardFileName = "TableKey.xml",
                Values =
                {
                    ["Name"] = "PK_DepartmentHierarchy",
                    ["KeyType"] = "primary"
                },
                RelationshipIds = { ["TableId"] = "1" }
            });
            source.Instance.GetOrCreateEntityRecords("TableKey").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "key:2",
                SourceShardFileName = "TableKey.xml",
                Values =
                {
                    ["Name"] = "PK_Department",
                    ["KeyType"] = "primary"
                },
                RelationshipIds = { ["TableId"] = "2" }
            });

            source.Instance.GetOrCreateEntityRecords("TableKeyField").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "keyf:1",
                SourceShardFileName = "TableKeyField.xml",
                Values =
                {
                    ["Ordinal"] = "1",
                    ["FieldName"] = "DepartmentHierarchyId"
                },
                RelationshipIds =
                {
                    ["TableKeyId"] = "key:1",
                    ["FieldId"] = "1"
                }
            });
            source.Instance.GetOrCreateEntityRecords("TableKeyField").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "keyf:2",
                SourceShardFileName = "TableKeyField.xml",
                Values =
                {
                    ["Ordinal"] = "1",
                    ["FieldName"] = "DepartmentId"
                },
                RelationshipIds =
                {
                    ["TableKeyId"] = "key:2",
                    ["FieldId"] = "4"
                }
            });

            source.Instance.GetOrCreateEntityRecords("TableRelationship").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "rel:parent",
                SourceShardFileName = "TableRelationship.xml",
                Values = { ["Name"] = "FK_DepartmentHierarchy_Department_ParentDepartmentId" },
                RelationshipIds =
                {
                    ["SourceTableId"] = "1",
                    ["TargetTableId"] = "2"
                }
            });
            source.Instance.GetOrCreateEntityRecords("TableRelationship").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "rel:child",
                SourceShardFileName = "TableRelationship.xml",
                Values = { ["Name"] = "FK_DepartmentHierarchy_Department_ChildDepartmentId" },
                RelationshipIds =
                {
                    ["SourceTableId"] = "1",
                    ["TargetTableId"] = "2"
                }
            });

            source.Instance.GetOrCreateEntityRecords("TableRelationshipField").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "relf:parent",
                SourceShardFileName = "TableRelationshipField.xml",
                Values = { ["Ordinal"] = "1" },
                RelationshipIds =
                {
                    ["TableRelationshipId"] = "rel:parent",
                    ["SourceFieldId"] = "2",
                    ["TargetFieldId"] = "4"
                }
            });
            source.Instance.GetOrCreateEntityRecords("TableRelationshipField").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "relf:child",
                SourceShardFileName = "TableRelationshipField.xml",
                Values = { ["Ordinal"] = "1" },
                RelationshipIds =
                {
                    ["TableRelationshipId"] = "rel:child",
                    ["SourceFieldId"] = "3",
                    ["TargetFieldId"] = "4"
                }
            });

            await new WorkspaceService().SaveAsync(source);

            var fromMetaSchemaResult = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\"");
            Assert.Equal(0, fromMetaSchemaResult.ExitCode);

            var workspace = await new WorkspaceService().LoadAsync(targetPath, searchUpward: false);
            var rawLinks = workspace.Instance.GetOrCreateEntityRecords("RawLink").ToDictionary(record => record.Id, StringComparer.Ordinal);

            Assert.Equal("DepartmentHierarchyDepartment_ParentDepartmentId", rawLinks["rawlink:rel:parent"].Values["Name"]);
            Assert.Equal("DepartmentHierarchyDepartment_ChildDepartmentId", rawLinks["rawlink:rel:child"].Values["Name"]);

            var generateMetaSqlResult = RunRawCli(
                $"generate-metasql --workspace \"{targetPath}\" --implementation-workspace \"{implementationPath}\" --database-name \"DisambiguatedLinkNaming\" --out \"{currentMetaSqlPath}\"");

            Assert.Equal(0, generateMetaSqlResult.ExitCode);
            Assert.Contains("OK: Generated CurrentMetaSql", generateMetaSqlResult.Output, StringComparison.OrdinalIgnoreCase);
            Assert.True(Directory.Exists(currentMetaSqlPath));
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task GenerateMetaSql_PrependsUnderscoreWhenSourceColumnsCollideWithRawTechnicalColumns()
    {
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(root, "MetaSchemaSource");
        var targetPath = Path.Combine(root, "RawDataVault");
        var currentMetaSqlPath = Path.Combine(root, "CurrentMetaSql");
        var implementationPath = GetRawImplementationWorkspacePath();

        try
        {
            Directory.CreateDirectory(sourcePath);
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace(sourcePath);
            SeedMetaSchema(source);
            AddMetaSchemaField(source, "6", "2", "AuditId", "sqlserver:type:int", "3", "false");
            AddMetaSchemaField(source, "7", "2", "LoadTimestamp", "sqlserver:type:datetime2", "4", "false");
            await new WorkspaceService().SaveAsync(source);

            var fromMetaSchemaResult = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\"");
            Assert.Equal(0, fromMetaSchemaResult.ExitCode);

            var generateMetaSqlResult = RunRawCli(
                $"generate-metasql --workspace \"{targetPath}\" --implementation-workspace \"{implementationPath}\" --database-name \"ReservedRawColumnNames\" --out \"{currentMetaSqlPath}\"");

            Assert.Equal(0, generateMetaSqlResult.ExitCode);

            var model = await MetaSqlModel.LoadFromXmlWorkspaceAsync(currentMetaSqlPath, searchUpward: false);
            var customerSatellite = Assert.Single(model.TableList, row => string.Equals(row.Name, "HS_Customer_Customer", StringComparison.Ordinal));
            var satelliteColumns = model.TableColumnList
                .Where(row => string.Equals(row.TableId, customerSatellite.Id, StringComparison.Ordinal))
                .Select(row => row.Name)
                .ToHashSet(StringComparer.Ordinal);

            Assert.Contains("AuditId", satelliteColumns);
            Assert.Contains("_AuditId", satelliteColumns);
            Assert.DoesNotContain("AuditId_", satelliteColumns);

            Assert.Contains("LoadTimestamp", satelliteColumns);
            Assert.Contains("_LoadTimestamp", satelliteColumns);
            Assert.DoesNotContain("LoadTimestamp_", satelliteColumns);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }
}
