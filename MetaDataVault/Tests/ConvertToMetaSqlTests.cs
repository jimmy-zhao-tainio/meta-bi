using MetaDataVault.ToMetaSql;
using MetaBusinessDataVault;
using MetaRawDataVault;
using Meta.Core.Services;

namespace MetaDataVault.Tests;

public sealed class ConvertToMetaSqlTests
{
    [Fact]
    public async Task ConvertAsync_LoadsRawWorkspaceAndCreatesSqlWorkspaceRootInMemory()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var createResult = RunRawCli($"--new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "RawVault");

            Assert.Equal(targetPath, sqlWorkspace.WorkspaceRootPath);
            Assert.Equal("MetaSql", sqlWorkspace.Model.Name);
            var databases = sqlWorkspace.Instance.GetOrCreateEntityRecords("Database");
            var schemas = sqlWorkspace.Instance.GetOrCreateEntityRecords("Schema");
            Assert.Single(databases);
            Assert.Single(schemas);
            Assert.Equal("RawVault", databases[0].Values["Name"]);
            Assert.Equal("RawVault", databases[0].Id);
            Assert.Equal("dbo", schemas[0].Values["Name"]);
            Assert.Equal("RawVault.dbo", schemas[0].Id);
            Assert.Empty(sqlWorkspace.Instance.GetOrCreateEntityRecords("Table"));
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_LoadsBusinessWorkspaceAndCreatesSqlWorkspaceRootInMemory()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var createResult = RunBusinessCli($"--new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            Assert.Equal(targetPath, sqlWorkspace.WorkspaceRootPath);
            Assert.Equal("MetaSql", sqlWorkspace.Model.Name);
            var databases = sqlWorkspace.Instance.GetOrCreateEntityRecords("Database");
            var schemas = sqlWorkspace.Instance.GetOrCreateEntityRecords("Schema");
            Assert.Single(databases);
            Assert.Single(schemas);
            Assert.Equal("BusinessVault", databases[0].Values["Name"]);
            Assert.Equal("BusinessVault", databases[0].Id);
            Assert.Equal("dbo", schemas[0].Values["Name"]);
            Assert.Equal("BusinessVault.dbo", schemas[0].Id);
            Assert.Empty(sqlWorkspace.Instance.GetOrCreateEntityRecords("Table"));
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_ProjectsRawSampleWorkspaceIntoSqlTables()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "Samples", "Demos", "RawDataVaultCliIntegration", "Workspace");
        var targetPath = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"), "MetaSql");

        try
        {
            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "RawVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var primaryKeys = sqlWorkspace.Instance.GetOrCreateEntityRecords("PrimaryKey");
            var foreignKeys = sqlWorkspace.Instance.GetOrCreateEntityRecords("ForeignKey");

            Assert.Equal(27, tables.Count);
            Assert.Equal(14, primaryKeys.Count);
            Assert.Equal(25, foreignKeys.Count);
            Assert.Contains(tables, row => row.Values.TryGetValue("Name", out var name) && string.Equals(name, "H_Customer", StringComparison.Ordinal));
            Assert.Contains(tables, row => row.Values.TryGetValue("Name", out var name) && string.Equals(name, "HS_Customer_CustomerProfile", StringComparison.Ordinal));
            Assert.Contains(tables, row => row.Values.TryGetValue("Name", out var name) && string.Equals(name, "L_OrderCustomer", StringComparison.Ordinal));
            Assert.Contains(tables, row => row.Values.TryGetValue("Name", out var name) && string.Equals(name, "LS_OrderCustomer_OrderCustomerStatus", StringComparison.Ordinal));

            var customerHub = tables.Single(row => row.Values.TryGetValue("Name", out var name) && string.Equals(name, "H_Customer", StringComparison.Ordinal));
            Assert.Equal("RawVault.dbo.H_Customer", customerHub.Id);
            Assert.Contains(columns, row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == customerHub.Id && row.Values.TryGetValue("Name", out var name) && string.Equals(name, "HashKey", StringComparison.Ordinal));
            Assert.Contains(columns, row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == customerHub.Id && row.Values.TryGetValue("Name", out var name) && string.Equals(name, "CustomerId", StringComparison.Ordinal));
            Assert.Contains(columns, row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == customerHub.Id && row.Values.TryGetValue("Name", out var name) && string.Equals(name, "LoadTimestamp", StringComparison.Ordinal));
            Assert.Contains(columns, row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == customerHub.Id && row.Values.TryGetValue("Name", out var name) && string.Equals(name, "RecordSource", StringComparison.Ordinal));
            Assert.Contains(columns, row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == customerHub.Id && row.Values.TryGetValue("Name", out var name) && string.Equals(name, "AuditId", StringComparison.Ordinal));
            Assert.Contains(columns, row => row.Id == "RawVault.dbo.H_Customer.HashKey");
            Assert.Contains(primaryKeys, row => row.Id == "RawVault.dbo.H_Customer.pk.PK_H_Customer" && string.Equals(row.Values["Name"], "PK_H_Customer", StringComparison.Ordinal));

            var orderCustomerLink = tables.Single(row => row.Values.TryGetValue("Name", out var name) && string.Equals(name, "L_OrderCustomer", StringComparison.Ordinal));
            Assert.Equal("RawVault.dbo.L_OrderCustomer", orderCustomerLink.Id);
            Assert.Contains(columns, row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == orderCustomerLink.Id && row.Values.TryGetValue("Name", out var name) && string.Equals(name, "OrderHashKey", StringComparison.Ordinal));
            Assert.Contains(columns, row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == orderCustomerLink.Id && row.Values.TryGetValue("Name", out var name) && string.Equals(name, "CustomerHashKey", StringComparison.Ordinal));
            Assert.Contains(foreignKeys, row => row.Id == "RawVault.dbo.L_OrderCustomer.fk.FK_L_OrderCustomer_H_Order_OrderHashKey");
        }
        finally
        {
            DeleteDirectoryIfExists(Path.GetDirectoryName(targetPath)!);
        }
    }

    [Fact]
    public async Task ConvertAsync_UsesUnderscorePrefixWhenSourceColumnCollidesWithTechnicalName()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var model = MetaRawDataVaultModel.CreateEmpty();

            var sourceSystem = new SourceSystem
            {
                Id = "SourceSystem:CRM",
                Name = "CRM",
            };
            var sourceSchema = new SourceSchema
            {
                Id = "SourceSchema:CRM:dbo",
                Name = "dbo",
                SourceSystemId = sourceSystem.Id,
                SourceSystem = sourceSystem,
            };
            var sourceTable = new SourceTable
            {
                Id = "SourceTable:Customer",
                Name = "Customer",
                SourceSchemaId = sourceSchema.Id,
                SourceSchema = sourceSchema,
            };
            var sourceField = new SourceField
            {
                Id = "SourceField:Customer:HashKey",
                Name = "HashKey",
                Ordinal = "1",
                DataTypeId = "sqlserver:type:nvarchar",
                IsNullable = "false",
                SourceTableId = sourceTable.Id,
                SourceTable = sourceTable,
            };
            var sourceFieldDetail = new SourceFieldDataTypeDetail
            {
                Id = "SourceFieldDetail:Customer:HashKey:Length",
                Name = "Length",
                Value = "50",
                SourceFieldId = sourceField.Id,
                SourceField = sourceField,
            };
            var rawHub = new RawHub
            {
                Id = "RawHub:Customer",
                Name = "Customer",
                SourceTableId = sourceTable.Id,
                SourceTable = sourceTable,
            };
            var rawHubKeyPart = new RawHubKeyPart
            {
                Id = "RawHubKeyPart:Customer:HashKey",
                Name = "HashKey",
                Ordinal = "1",
                RawHubId = rawHub.Id,
                RawHub = rawHub,
                SourceFieldId = sourceField.Id,
                SourceField = sourceField,
            };

            model.SourceSystemList.Add(sourceSystem);
            model.SourceSchemaList.Add(sourceSchema);
            model.SourceTableList.Add(sourceTable);
            model.SourceFieldList.Add(sourceField);
            model.SourceFieldDataTypeDetailList.Add(sourceFieldDetail);
            model.RawHubList.Add(rawHub);
            model.RawHubKeyPartList.Add(rawHubKeyPart);

            await model.SaveToXmlWorkspaceAsync(workspacePath);

            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "RawVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var customerHub = tables.Single(row => row.Values.TryGetValue("Name", out var name) && string.Equals(name, "H_Customer", StringComparison.Ordinal));
            var customerHubColumnNames = columns
                .Where(row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == customerHub.Id)
                .Select(row => row.Values["Name"])
                .ToList();

            Assert.Contains("HashKey", customerHubColumnNames);
            Assert.Contains("_HashKey", customerHubColumnNames);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_ProjectsBusinessCommerceHelpersWorkspaceIntoSqlTables()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var targetPath = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"), "MetaSql");

        try
        {
            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var foreignKeys = sqlWorkspace.Instance.GetOrCreateEntityRecords("ForeignKey");

            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "BH_Customer", StringComparison.Ordinal));
            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "BL_CustomerOrder", StringComparison.Ordinal));
            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "REF_Status", StringComparison.Ordinal));
            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "BHS_Customer_Profile", StringComparison.Ordinal));
            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "BLS_CustomerOrder_Status", StringComparison.Ordinal));
            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "RSAT_Status_Current", StringComparison.Ordinal));
            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "PIT_CustomerSnapshot", StringComparison.Ordinal));
            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "BR_CustomerOrderTraversal", StringComparison.Ordinal));

            var customerHub = GetTable(tables, "BH_Customer");
            Assert.Equal("BusinessVault.dbo.BH_Customer", customerHub.Id);
            var customerHubColumns = GetColumnNames(columns, customerHub.Id);
            Assert.Contains("HashKey", customerHubColumns);
            Assert.Contains("Identifier", customerHubColumns);
            Assert.Contains("LoadTimestamp", customerHubColumns);
            Assert.Contains("RecordSource", customerHubColumns);
            Assert.Contains("AuditId", customerHubColumns);

            var customerOrderLink = GetTable(tables, "BL_CustomerOrder");
            var customerOrderLinkColumns = GetColumnNames(columns, customerOrderLink.Id);
            Assert.Contains("CustomerHashKey", customerOrderLinkColumns);
            Assert.Contains("OrderHashKey", customerOrderLinkColumns);

            var customerSnapshotPit = GetTable(tables, "PIT_CustomerSnapshot");
            Assert.Equal("BusinessVault.dbo.PIT_CustomerSnapshot", customerSnapshotPit.Id);
            var customerSnapshotColumns = GetColumnNames(columns, customerSnapshotPit.Id);
            Assert.Contains("HubHashKey", customerSnapshotColumns);
            Assert.Contains("SnapshotTimestamp", customerSnapshotColumns);
            Assert.Contains("ProfileLoadTimestamp", customerSnapshotColumns);
            Assert.Contains("StatusLoadTimestamp", customerSnapshotColumns);
            Assert.Contains("AuditId", customerSnapshotColumns);

            var customerOrderBridge = GetTable(tables, "BR_CustomerOrderTraversal");
            var customerOrderBridgeColumns = GetColumnNames(columns, customerOrderBridge.Id);
            Assert.Contains("RootHashKey", customerOrderBridgeColumns);
            Assert.Contains("RelatedHashKey", customerOrderBridgeColumns);
            Assert.Contains("Depth", customerOrderBridgeColumns);
            Assert.Contains("Path", customerOrderBridgeColumns);
            Assert.Contains("EffectiveFrom", customerOrderBridgeColumns);
            Assert.Contains("EffectiveTo", customerOrderBridgeColumns);
            Assert.Contains("AuditId", customerOrderBridgeColumns);

            Assert.Contains(foreignKeys, row => string.Equals(row.Values["Name"], "FK_BHS_Customer_Profile_BH_Customer", StringComparison.Ordinal));
            Assert.Contains(foreignKeys, row => string.Equals(row.Values["Name"], "FK_BLS_CustomerOrder_Status_BL_CustomerOrder", StringComparison.Ordinal));
            Assert.Contains(foreignKeys, row => string.Equals(row.Values["Name"], "FK_PIT_CustomerSnapshot_BH_Customer", StringComparison.Ordinal));
            Assert.Contains(foreignKeys, row => string.Equals(row.Values["Name"], "FK_BR_CustomerOrderTraversal_BH_Customer", StringComparison.Ordinal));
            Assert.Contains(foreignKeys, row => string.Equals(row.Values["Name"], "FK_BR_CustomerOrderTraversal_BH_Order_Related", StringComparison.Ordinal));
            Assert.Contains(foreignKeys, row => row.Id == "BusinessVault.dbo.PIT_CustomerSnapshot.fk.FK_PIT_CustomerSnapshot_BH_Customer");
        }
        finally
        {
            DeleteDirectoryIfExists(Path.GetDirectoryName(targetPath)!);
        }
    }

    [Fact]
    public async Task ConvertAsync_ProjectsBusinessLinkVariantsWorkspaceIntoSqlTables()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultLinkVariants");
        var targetPath = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"), "MetaSql");

        try
        {
            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var foreignKeys = sqlWorkspace.Instance.GetOrCreateEntityRecords("ForeignKey");

            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "BSAL_CustomerMatch", StringComparison.Ordinal));
            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "BSALS_CustomerMatch_Evidence", StringComparison.Ordinal));
            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "BHAL_EmployeeManager", StringComparison.Ordinal));
            Assert.Contains(tables, row => string.Equals(row.Values["Name"], "BHALS_EmployeeManager_Line", StringComparison.Ordinal));

            var customerMatch = GetTable(tables, "BSAL_CustomerMatch");
            Assert.Equal("BusinessVault.dbo.BSAL_CustomerMatch", customerMatch.Id);
            var customerMatchColumns = GetColumnNames(columns, customerMatch.Id);
            Assert.Contains("HashKey", customerMatchColumns);
            Assert.Contains("PrimaryHashKey", customerMatchColumns);
            Assert.Contains("EquivalentHashKey", customerMatchColumns);

            var employeeManager = GetTable(tables, "BHAL_EmployeeManager");
            Assert.Equal("BusinessVault.dbo.BHAL_EmployeeManager", employeeManager.Id);
            var employeeManagerColumns = GetColumnNames(columns, employeeManager.Id);
            Assert.Contains("HashKey", employeeManagerColumns);
            Assert.Contains("ParentHashKey", employeeManagerColumns);
            Assert.Contains("ChildHashKey", employeeManagerColumns);

            Assert.Contains(foreignKeys, row => string.Equals(row.Values["Name"], "FK_BSAL_CustomerMatch_BH_Customer_PrimaryHashKey", StringComparison.Ordinal));
            Assert.Contains(foreignKeys, row => string.Equals(row.Values["Name"], "FK_BHAL_EmployeeManager_BH_Employee_ParentHashKey", StringComparison.Ordinal));
            Assert.Contains(foreignKeys, row => string.Equals(row.Values["Name"], "FK_BHALS_EmployeeManager_Line_BHAL_EmployeeManager", StringComparison.Ordinal));
            Assert.Contains(foreignKeys, row => row.Id == "BusinessVault.dbo.BSAL_CustomerMatch.fk.FK_BSAL_CustomerMatch_BH_Customer_PrimaryHashKey");
        }
        finally
        {
            DeleteDirectoryIfExists(Path.GetDirectoryName(targetPath)!);
        }
    }

    [Fact]
    public async Task ConvertAsync_PreservesBusinessDataTypeDetailsInProjectedColumns()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var targetPath = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"), "MetaSql");

        try
        {
            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var details = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumnDataTypeDetail");

            Assert.Equal("50", GetDetailValue(details, GetColumn(columns, GetTable(tables, "BH_Customer").Id, "Identifier").Id, "Length"));
            Assert.Equal("200", GetDetailValue(details, GetColumn(columns, GetTable(tables, "BHS_Customer_Profile").Id, "CustomerName").Id, "Length"));
            Assert.Equal("20", GetDetailValue(details, GetColumn(columns, GetTable(tables, "BLS_CustomerOrder_Status").Id, "StatusCode").Id, "Length"));
        }
        finally
        {
            DeleteDirectoryIfExists(Path.GetDirectoryName(targetPath)!);
        }
    }

    [Fact]
    public async Task ConvertAsync_PreservesBusinessLinkVariantSatelliteAttributeDetails()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultLinkVariants");
        var targetPath = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"), "MetaSql");

        try
        {
            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var details = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumnDataTypeDetail");

            Assert.Equal("20", GetDetailValue(details, GetColumn(columns, GetTable(tables, "BSALS_CustomerMatch_Evidence").Id, "MatchScore").Id, "Length"));
            Assert.Equal("20", GetDetailValue(details, GetColumn(columns, GetTable(tables, "BHALS_EmployeeManager_Line").Id, "LineType").Id, "Length"));
        }
        finally
        {
            DeleteDirectoryIfExists(Path.GetDirectoryName(targetPath)!);
        }
    }

    [Fact]
    public async Task ConvertAsync_RejectsBusinessBridgeThatDoesNotAlternateLinkAndHub()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var createResult = RunBusinessCli($"--new-workspace \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            Assert.Equal(0, RunBusinessCli($"add-hub --workspace \"{workspacePath}\" --id Customer --name Customer").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-hub --workspace \"{workspacePath}\" --id Order --name Order").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-hub --workspace \"{workspacePath}\" --id Shipment --name Shipment").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-link --workspace \"{workspacePath}\" --id CustomerOrder --name CustomerOrder").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-link-hub --workspace \"{workspacePath}\" --id CustomerOrderCustomer --link CustomerOrder --hub Customer --ordinal 1 --role-name Customer").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-link-hub --workspace \"{workspacePath}\" --id CustomerOrderOrder --link CustomerOrder --hub Order --ordinal 2 --role-name Order").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-link --workspace \"{workspacePath}\" --id ShipmentOrder --name ShipmentOrder").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-link-hub --workspace \"{workspacePath}\" --id ShipmentOrderShipment --link ShipmentOrder --hub Shipment --ordinal 1 --role-name Shipment").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-link-hub --workspace \"{workspacePath}\" --id ShipmentOrderOrder --link ShipmentOrder --hub Order --ordinal 2 --role-name Order").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-bridge --workspace \"{workspacePath}\" --id CustomerShipmentTraversal --anchor-hub Customer --name CustomerShipmentTraversal").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-bridge-link --workspace \"{workspacePath}\" --id CustomerShipmentTraversalCustomerOrder --bridge CustomerShipmentTraversal --link CustomerOrder --ordinal 1 --role-name CustomerOrder").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-bridge-link --workspace \"{workspacePath}\" --id CustomerShipmentTraversalShipmentOrder --bridge CustomerShipmentTraversal --link ShipmentOrder --ordinal 2 --role-name ShipmentOrder").ExitCode);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault"));
            Assert.Contains("must end with a BusinessBridgeHub", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_CanSaveAndReloadProjectedSqlWorkspace()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            var workspaceService = new WorkspaceService();
            await workspaceService.SaveAsync(sqlWorkspace);
            var reloaded = await workspaceService.LoadAsync(targetPath, searchUpward: false);

            Assert.Equal("MetaSql", reloaded.Model.Name);
            Assert.NotEmpty(reloaded.Instance.GetOrCreateEntityRecords("Table"));
            Assert.Contains(reloaded.Instance.GetOrCreateEntityRecords("Table"), row => string.Equals(row.Values["Name"], "BH_Customer", StringComparison.Ordinal));
            Assert.Contains(reloaded.Instance.GetOrCreateEntityRecords("Table"), row => string.Equals(row.Values["Name"], "PIT_CustomerSnapshot", StringComparison.Ordinal));
            Assert.Contains(reloaded.Instance.GetOrCreateEntityRecords("Table"), row => row.Id == "BusinessVault.dbo.BH_Customer");
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_LowersBusinessLogicalTypesToSanctionedSqlServerTypes()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var targetPath = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"), "MetaSql");

        try
        {
            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");

            Assert.Equal("sqlserver:type:nvarchar", GetColumn(columns, GetTable(tables, "BH_Customer").Id, "Identifier").Values["MetaDataTypeId"]);
            Assert.Equal("sqlserver:type:nvarchar", GetColumn(columns, GetTable(tables, "BHS_Customer_Profile").Id, "CustomerName").Values["MetaDataTypeId"]);
            Assert.Equal("sqlserver:type:nvarchar", GetColumn(columns, GetTable(tables, "BLS_CustomerOrder_Status").Id, "StatusCode").Values["MetaDataTypeId"]);
            Assert.Equal("sqlserver:type:nvarchar", GetColumn(columns, GetTable(tables, "REF_Status").Id, "StatusCode").Values["MetaDataTypeId"]);
            Assert.Equal("sqlserver:type:nvarchar", GetColumn(columns, GetTable(tables, "RSAT_Status_Current").Id, "StatusName").Values["MetaDataTypeId"]);
        }
        finally
        {
            DeleteDirectoryIfExists(Path.GetDirectoryName(targetPath)!);
        }
    }

    [Fact]
    public async Task ConvertAsync_BusinessLogicalTypesUseSanctionedStaticTypeConversionInstance()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var targetPath = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"), "MetaSql");

        try
        {
            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            Assert.Equal("sqlserver:type:nvarchar", GetColumn(columns, GetTable(tables, "BH_Customer").Id, "Identifier").Values["MetaDataTypeId"]);
            Assert.Equal("sqlserver:type:nvarchar", GetColumn(columns, GetTable(tables, "BHS_Customer_Profile").Id, "CustomerName").Values["MetaDataTypeId"]);
        }
        finally
        {
            DeleteDirectoryIfExists(Path.GetDirectoryName(targetPath)!);
        }
    }

    [Fact]
    public async Task ConvertAsync_BusinessFailsWhenBusinessTypesDoNotBelongToMetaDataTypeSystem()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var sourceWorkspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var model = await MetaBusinessDataVaultModel.LoadFromXmlWorkspaceAsync(sourceWorkspacePath, searchUpward: false);

            foreach (var row in model.BusinessHubKeyPartList)
            {
                row.DataTypeId = "sqlserver:type:nvarchar";
            }

            foreach (var row in model.BusinessHubSatelliteAttributeList)
            {
                row.DataTypeId = "sqlserver:type:nvarchar";
            }

            foreach (var row in model.BusinessLinkSatelliteAttributeList)
            {
                row.DataTypeId = "sqlserver:type:nvarchar";
            }

            foreach (var row in model.BusinessReferenceKeyPartList)
            {
                row.DataTypeId = "sqlserver:type:nvarchar";
            }

            foreach (var row in model.BusinessReferenceSatelliteAttributeList)
            {
                row.DataTypeId = "sqlserver:type:nvarchar";
            }

            foreach (var row in model.BusinessPointInTimeStampList)
            {
                row.DataTypeId = "sqlserver:type:nvarchar";
            }

            await model.SaveToXmlWorkspaceAsync(workspacePath);

            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault"));

            Assert.Contains("sqlserver:type:nvarchar", error.Message, StringComparison.Ordinal);
            Assert.Contains("must belong to DataTypeSystem 'Meta'", error.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_BusinessFailsWhenLogicalTypeHasNoSanctionedDirectSqlServerLowering()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var sourceWorkspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var model = await MetaBusinessDataVaultModel.LoadFromXmlWorkspaceAsync(sourceWorkspacePath, searchUpward: false);
            model.BusinessHubKeyPartList[0].DataTypeId = "meta:type:Xml";
            await model.SaveToXmlWorkspaceAsync(workspacePath);

            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault"));

            Assert.Contains("meta:type:Xml", error.Message, StringComparison.Ordinal);
            Assert.Contains("no sanctioned direct SqlServer lowering", error.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_BusinessFailsWhenSqlServerTypedValueIsNotSanctionedInMetaDataType()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var sourceWorkspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var model = await MetaBusinessDataVaultModel.LoadFromXmlWorkspaceAsync(sourceWorkspacePath, searchUpward: false);
            model.BusinessHubKeyPartList[0].DataTypeId = "sqlserver:type:not-real";
            await model.SaveToXmlWorkspaceAsync(workspacePath);

            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                targetPath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault"));

            Assert.Contains("sqlserver:type:not-real", error.Message, StringComparison.Ordinal);
            Assert.Contains("not sanctioned in MetaDataType", error.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    private static string GetImplementationWorkspacePath(string repoRoot)
    {
        return Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "MetaDataVaultImplementation");
    }

    private static (int ExitCode, string Output) RunRawCli(string arguments)
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "meta-datavault-raw",
            Arguments = arguments,
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        return CliTestSupport.RunProcess(startInfo, "Could not start DataVault CLI process.");
    }

    private static (int ExitCode, string Output) RunBusinessCli(string arguments)
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "meta-datavault-business",
            Arguments = arguments,
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        return CliTestSupport.RunProcess(startInfo, "Could not start DataVault CLI process.");
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }

    private static Meta.Core.Domain.GenericRecord GetTable(IReadOnlyList<Meta.Core.Domain.GenericRecord> tables, string tableName)
    {
        return tables.Single(row => row.Values.TryGetValue("Name", out var name) && string.Equals(name, tableName, StringComparison.Ordinal));
    }

    private static List<string> GetColumnNames(IReadOnlyList<Meta.Core.Domain.GenericRecord> columns, string tableId)
    {
        return columns
            .Where(row => row.RelationshipIds.TryGetValue("TableId", out var currentTableId) && currentTableId == tableId)
            .Select(row => row.Values["Name"])
            .ToList();
    }

    private static Meta.Core.Domain.GenericRecord GetColumn(IReadOnlyList<Meta.Core.Domain.GenericRecord> columns, string tableId, string columnName)
    {
        return columns.Single(row =>
            row.RelationshipIds.TryGetValue("TableId", out var currentTableId) &&
            currentTableId == tableId &&
            row.Values.TryGetValue("Name", out var currentName) &&
            string.Equals(currentName, columnName, StringComparison.Ordinal));
    }

    private static string GetDetailValue(IReadOnlyList<Meta.Core.Domain.GenericRecord> details, string tableColumnId, string detailName)
    {
        return details.Single(row =>
            row.RelationshipIds.TryGetValue("TableColumnId", out var currentTableColumnId) &&
            currentTableColumnId == tableColumnId &&
            row.Values.TryGetValue("Name", out var currentName) &&
            string.Equals(currentName, detailName, StringComparison.Ordinal)).Values["Value"];
    }
}


