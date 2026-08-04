using Meta.Core.Serialization;
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
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace();
            SeedMetaSchema(source);
            await XmlWorkspaceWriter.WriteNewAsync(source, sourcePath);

            var result = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\" --verbose");
            Assert.Equal(0, result.ExitCode);
            Assert.Contains("Ok", result.Output, StringComparison.Ordinal);
            Assert.Contains("Summary", result.Output, StringComparison.OrdinalIgnoreCase);

            var workspace = await XmlWorkspaceReader.OpenAsync(targetPath);
            Assert.Equal("MetaRawDataVault", workspace.Model.Name);

            Assert.Null(workspace.Model.FindEntity("SourceSystem"));
            Assert.Null(workspace.Model.FindEntity("SourceSchema"));
            Assert.Null(workspace.Model.FindEntity("SourceTable"));
            Assert.Null(workspace.Model.FindEntity("SourceField"));
            Assert.Equal(5, workspace.Instance.GetOrCreateEntityRecords("Field").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawHub").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawHubKeyPart").Count);
            Assert.Single(workspace.Instance.GetOrCreateEntityRecords("RawLink"));
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawLinkRole").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawHubSatellite").Count);
            Assert.Equal(2, workspace.Instance.GetOrCreateEntityRecords("RawHubSatelliteAttribute").Count);
            Assert.Empty(workspace.Instance.GetOrCreateEntityRecords("RawLinkSatellite"));

            var rawLinks = workspace.Instance.GetOrCreateEntityRecords("RawLink").ToDictionary(record => record.Id, StringComparer.Ordinal);
            Assert.Equal("OrderCustomer", rawLinks["rawlink:rel:1"].Values["Name"]);

            var rawLinkRoles = workspace.Instance.GetOrCreateEntityRecords("RawLinkRole").ToDictionary(record => record.Id, StringComparer.Ordinal);
            Assert.Equal("Order", rawLinkRoles["rawlink:rel:1:source"].Values["Name"]);
            Assert.Equal("Customer", rawLinkRoles["rawlink:rel:1:target"].Values["Name"]);

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
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace();
            SeedMetaSchema(source);

            AddMetaSchemaView(source, "view:1", "CustomerView", "1");
            AddMetaSchemaField(source, "view-field:1", "view:1", "CustomerViewId", "sqlserver:type:int", "1", null);
            AddMetaSchemaField(source, "view-field:2", "view:1", "CustomerViewName", "sqlserver:type:nvarchar", "2", null);

            await XmlWorkspaceWriter.WriteNewAsync(source, sourcePath);

            var defaultResult = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{defaultTargetPath}\"");
            Assert.Equal(0, defaultResult.ExitCode);

            var includeViewsResult = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{includeViewsTargetPath}\" --include-views");
            Assert.Equal(0, includeViewsResult.ExitCode);
            Assert.DoesNotContain("Error:", includeViewsResult.Output, StringComparison.OrdinalIgnoreCase);

            var defaultWorkspace = await XmlWorkspaceReader.OpenAsync(defaultTargetPath);
            var includeViewsWorkspace = await XmlWorkspaceReader.OpenAsync(includeViewsTargetPath);

            Assert.Equal(5, defaultWorkspace.Instance.GetOrCreateEntityRecords("Field").Count);
            Assert.Equal(7, includeViewsWorkspace.Instance.GetOrCreateEntityRecords("Field").Count);
            Assert.Equal(2, defaultWorkspace.Instance.GetOrCreateEntityRecords("RawHub").Count);
            Assert.Equal(2, includeViewsWorkspace.Instance.GetOrCreateEntityRecords("RawHub").Count);
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
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace();
            SeedMetaSchema(source);
            AddMetaSchemaField(source, "6", "1", "AuditId", "sqlserver:type:uniqueidentifier", "4", "false");
            await XmlWorkspaceWriter.WriteNewAsync(source, sourcePath);

            var result = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\"");

            Assert.Equal(0, result.ExitCode);

            var workspace = await XmlWorkspaceReader.OpenAsync(targetPath);
            Assert.Equal(6, workspace.Instance.GetOrCreateEntityRecords("Field").Count);
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
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace();
            SeedMetaSchema(source);
            AddMetaSchemaField(source, "6", "1", "OrderHashKey", "sqlserver:type:varbinary", "4", "false");
            await XmlWorkspaceWriter.WriteNewAsync(source, sourcePath);

            var result = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\" --ignore-field-suffix HashKey");

            Assert.Equal(0, result.ExitCode);
            Assert.DoesNotContain("Error:", result.Output, StringComparison.OrdinalIgnoreCase);

            var workspace = await XmlWorkspaceReader.OpenAsync(targetPath);
            Assert.Equal(6, workspace.Instance.GetOrCreateEntityRecords("Field").Count);
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
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace();
            SeedMetaSchema(source);

            AddMetaSchemaTable(source, "3", "HashDriven", "1");

            AddMetaSchemaField(source, "6", "3", "OrderHashKey", "sqlserver:type:varbinary", "1", "false");
            AddMetaSchemaField(source, "7", "3", "Description", "sqlserver:type:nvarchar", "2", "true");

            AddMetaSchemaPrimaryKey(source, "key:3", "PK_HashDriven", "3");
            AddMetaSchemaKeyField(source, "keyf:3", "key:3", "6", "1");

            await XmlWorkspaceWriter.WriteNewAsync(source, sourcePath);

            var result = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\"");

            Assert.Equal(0, result.ExitCode);

            var workspace = await XmlWorkspaceReader.OpenAsync(targetPath);
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
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace();

            source.Instance.GetOrCreateEntityRecords("System").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "1",
                Values = { ["Name"] = "Sales" }
            });

            source.Instance.GetOrCreateEntityRecords("Schema").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "1",
                Values = { ["Name"] = "dbo" },
                RelationshipIds = { ["SystemId"] = "1" }
            });

            AddMetaSchemaTable(source, "1", "DepartmentHierarchy", "1");
            AddMetaSchemaTable(source, "2", "Department", "1");

            AddMetaSchemaField(source, "1", "1", "DepartmentHierarchyId", "sqlserver:type:int", "1", "false");
            AddMetaSchemaField(source, "2", "1", "ParentDepartmentId", "sqlserver:type:int", "2", "false");
            AddMetaSchemaField(source, "3", "1", "ChildDepartmentId", "sqlserver:type:int", "3", "false");
            AddMetaSchemaField(source, "4", "2", "DepartmentId", "sqlserver:type:int", "1", "false");
            AddMetaSchemaField(source, "5", "2", "DepartmentName", "sqlserver:type:nvarchar", "2", "true");

            AddMetaSchemaPrimaryKey(source, "key:1", "PK_DepartmentHierarchy", "1");
            AddMetaSchemaPrimaryKey(source, "key:2", "PK_Department", "2");
            AddMetaSchemaKeyField(source, "keyf:1", "key:1", "1", "1");
            AddMetaSchemaKeyField(source, "keyf:2", "key:2", "4", "1");

            source.Instance.GetOrCreateEntityRecords("TableRelationship").Add(new Meta.Core.Domain.GenericRecord
            {
                Id = "rel:parent",
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
                Values = { ["Ordinal"] = "1" },
                RelationshipIds =
                {
                    ["TableRelationshipId"] = "rel:child",
                    ["SourceFieldId"] = "3",
                    ["TargetFieldId"] = "4"
                }
            });

            await XmlWorkspaceWriter.WriteNewAsync(source, sourcePath);

            var fromMetaSchemaResult = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\"");
            Assert.Equal(0, fromMetaSchemaResult.ExitCode);

            var workspace = await XmlWorkspaceReader.OpenAsync(targetPath);
            var rawLinks = workspace.Instance.GetOrCreateEntityRecords("RawLink").ToDictionary(record => record.Id, StringComparer.Ordinal);

            Assert.Equal("DepartmentHierarchyDepartment_ParentDepartmentId", rawLinks["rawlink:rel:parent"].Values["Name"]);
            Assert.Equal("DepartmentHierarchyDepartment_ChildDepartmentId", rawLinks["rawlink:rel:child"].Values["Name"]);

            var generateMetaSqlResult = RunMetaConvertCli(
                $"raw-datavault-to-sql --workspace \"{targetPath}\" --implementation-workspace \"{implementationPath}\" --database-name \"DisambiguatedLinkNaming\" --out \"{currentMetaSqlPath}\"");

            Assert.Equal(0, generateMetaSqlResult.ExitCode);
            Assert.Contains("Ok", generateMetaSqlResult.Output, StringComparison.Ordinal);
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
            var source = MetaSchemaWorkspaces.CreateEmptyMetaSchemaWorkspace();
            SeedMetaSchema(source);
            AddMetaSchemaField(source, "6", "2", "AuditId", "sqlserver:type:int", "3", "false");
            AddMetaSchemaField(source, "7", "2", "LoadTimestamp", "sqlserver:type:datetime2", "4", "false");
            await XmlWorkspaceWriter.WriteNewAsync(source, sourcePath);

            var fromMetaSchemaResult = RunMetaConvertCli($"schema-to-raw-datavault --source-workspace \"{sourcePath}\" --new-workspace \"{targetPath}\"");
            Assert.Equal(0, fromMetaSchemaResult.ExitCode);

            var generateMetaSqlResult = RunMetaConvertCli(
                $"raw-datavault-to-sql --workspace \"{targetPath}\" --implementation-workspace \"{implementationPath}\" --database-name \"ReservedRawColumnNames\" --out \"{currentMetaSqlPath}\"");

            Assert.Equal(0, generateMetaSqlResult.ExitCode);

            var model = await MetaSqlModel.LoadFromXmlWorkspaceAsync(currentMetaSqlPath, searchUpward: false);
            var customerSatellite = Assert.Single(model.TableList, row => string.Equals(row.Name, "HS_Customer_Customer", StringComparison.Ordinal));
            var satelliteColumns = model.TableColumnList
                .Where(row => string.Equals(row.Table.Id, customerSatellite.Id, StringComparison.Ordinal))
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
