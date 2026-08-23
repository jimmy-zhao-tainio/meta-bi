using System.Globalization;
using MetaConvert.DataVaultToSql;
using MetaBusinessDataVault;
using MetaRawDataVault;
using Meta.Integration;
using Meta.Operations.Domain;
using Meta.Surfaces.Xml;
using Meta.TypedModels;
using MetaBi.Tests.Common;
using MetaDataVaultImplementation;
using MetaDataType;
using MetaDataTypeConversion;
using Meta.Surfaces;
using MetaSql;
using MetaWeave.Core;
using MetaWeaveScript.Execution;

namespace MetaDataVault.Tests;

public sealed class ConvertToMetaSqlTests
{
    [Fact]
    public async Task RawProductConverter_ExecutesSanctionedWeave_ForCompleteSample()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(repoRoot, "Demos", "RawDataVaultCliIntegration", "Workspace");
        var implementationPath = GetImplementationWorkspacePath(repoRoot);
        var actual = await Converter.ConvertAsync(
            workspacePath,
            implementationPath,
            databaseName: "RawVault");
        var raw = await TypedWorkspaceModelMapper.LoadAsync<MetaRawDataVaultModel>(
            workspacePath,
            searchUpward: false);
        var implementation = await TypedWorkspaceModelMapper.LoadAsync<MetaDataVaultImplementationModel>(
            implementationPath,
            searchUpward: false);
        var direction = new MetaWeaveScriptDirectionLoader().Load(
            Path.Combine(repoRoot, "MetaConvert", "Weaves", "RawDataVaultToSql"),
            "forward");

        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            direction,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["raw"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(raw),
                ["implementation"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(implementation),
                ["dataTypes"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeInstance.BuiltIn),
                ["typeConversions"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeConversionInstance.BuiltIn),
            },
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaSqlModel.CreateEmpty()),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["databaseName"] = "RawVault",
            });

        Assert.True(
            result.IsSuccess,
            string.Join(Environment.NewLine, result.Issues.Select(issue => $"{issue.Code}: {issue.Message}")));
        var expected = Assert.IsType<InMemoryWorkspace>(result.OutputWorkspace);
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, actual));
        foreach (var targetEntity in direction.Transformations.Select(row => row.TargetEntityName))
        {
            Assert.True(
                actual.Instance.RecordsByEntity.TryGetValue(targetEntity, out var records) && records.Count > 0,
                $"Transformation target '{targetEntity}' produced no witness rows.");
        }
    }

    [Theory]
    [InlineData("MetaDataVault/Workspaces/SampleBusinessDataVaultCommerceHelpers")]
    [InlineData("MetaDataVault/Workspaces/SampleBusinessDataVaultLinkVariants")]
    [InlineData("Demos/AdventureWorksBIStackDemo/Runs/bdv/BusinessVault")]
    public async Task BusinessSanctionedWeave_MatchesCSharpReference_ForCompleteSamples(string relativeWorkspacePath)
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(
            repoRoot,
            relativeWorkspacePath.Replace('/', Path.DirectorySeparatorChar));
        var implementationPath = GetImplementationWorkspacePath(repoRoot);
        var business = await TypedWorkspaceModelMapper.LoadAsync<MetaBusinessDataVaultModel>(
            workspacePath,
            searchUpward: false);
        var implementation = await TypedWorkspaceModelMapper.LoadAsync<MetaDataVaultImplementationModel>(
            implementationPath,
            searchUpward: false);
        var expected = TypedWorkspaceModelMapper.ToInMemoryWorkspace(
            BusinessDataVaultToSqlCSharpReference.ConvertToMetaSql(
                business,
                implementation,
                "BusinessVault"));
        var direction = new MetaWeaveScriptDirectionLoader().Load(
            Path.Combine(repoRoot, "MetaConvert", "Weaves", "BusinessDataVaultToSql"),
            "forward");

        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            direction,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["business"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(business),
                ["implementation"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(implementation),
                ["dataTypes"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeInstance.BuiltIn),
                ["typeConversions"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeConversionInstance.BuiltIn),
            },
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaSqlModel.CreateEmpty()),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["databaseName"] = "BusinessVault",
            });

        Assert.True(
            result.IsSuccess,
            string.Join(Environment.NewLine, result.Issues.Select(issue => $"{issue.Code}: {issue.Message}")));
        var actual = Assert.IsType<InMemoryWorkspace>(result.OutputWorkspace);
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, actual));
    }

    [Fact]
    public async Task BusinessSanctionedWeave_ReproducesRepeatedUnderscoreColumnAllocation()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var sourceWorkspacePath = Path.Combine(
            repoRoot,
            "MetaDataVault",
            "Workspaces",
            "SampleBusinessDataVaultCommerceHelpers");
        var implementationPath = GetImplementationWorkspacePath(repoRoot);
        var business = await TypedWorkspaceModelMapper.LoadAsync<MetaBusinessDataVaultModel>(
            sourceWorkspacePath,
            searchUpward: false);
        var firstKeyPart = business.BusinessHubKeyPartList.Single(row => row.Id == "CustomerIdentifier");
        firstKeyPart.Name = "HashKey";
        business.BusinessHubKeyPartList.Add(new BusinessHubKeyPart
        {
            Id = "CustomerSecondaryIdentifier",
            Name = "_HashKey",
            DataTypeId = firstKeyPart.DataTypeId,
            BusinessHub = firstKeyPart.BusinessHub,
            PreviousKeyPart = firstKeyPart,
        });
        var implementation = await TypedWorkspaceModelMapper.LoadAsync<MetaDataVaultImplementationModel>(
            implementationPath,
            searchUpward: false);
        var expected = TypedWorkspaceModelMapper.ToInMemoryWorkspace(
            BusinessDataVaultToSqlCSharpReference.ConvertToMetaSql(
                business,
                implementation,
                "BusinessVault"));
        var direction = new MetaWeaveScriptDirectionLoader().Load(
            Path.Combine(repoRoot, "MetaConvert", "Weaves", "BusinessDataVaultToSql"),
            "forward");
        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            direction,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["business"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(business),
                ["implementation"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(implementation),
                ["dataTypes"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeInstance.BuiltIn),
                ["typeConversions"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeConversionInstance.BuiltIn),
            },
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaSqlModel.CreateEmpty()),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["databaseName"] = "BusinessVault",
            });

        Assert.True(
            result.IsSuccess,
            string.Join(Environment.NewLine, result.Issues.Select(issue => $"{issue.Code}: {issue.Message}")));
        var actual = Assert.IsType<InMemoryWorkspace>(result.OutputWorkspace);
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, actual));
        var customerHubColumns = actual.Instance.GetOrCreateEntityRecords("TableColumn")
            .Where(row => row.RelationshipIds.TryGetValue("TableId", out var tableId) &&
                          tableId == "BusinessVault.dbo.BH_Customer")
            .Select(row => row.Values["Name"])
            .ToList();
        Assert.Contains("HashKey", customerHubColumns);
        Assert.Contains("_HashKey", customerHubColumns);
        Assert.Contains("__HashKey", customerHubColumns);
    }

    [Fact]
    public async Task ConvertAsync_LoadsRawWorkspaceAndCreatesSqlWorkspaceRootInMemory()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var createResult = RunRawCli($"create --xml \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "RawVault");

            Assert.False(Directory.Exists(targetPath));
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
            var createResult = RunBusinessCli($"create --xml \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            Assert.False(Directory.Exists(targetPath));
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
        var workspacePath = Path.Combine(repoRoot, "Demos", "RawDataVaultCliIntegration", "Workspace");
        var targetPath = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"), "MetaSql");

        try
        {
            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "RawVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var primaryKeys = sqlWorkspace.Instance.GetOrCreateEntityRecords("PrimaryKey");
            var primaryKeyColumns = sqlWorkspace.Instance.GetOrCreateEntityRecords("PrimaryKeyColumn");
            var foreignKeys = sqlWorkspace.Instance.GetOrCreateEntityRecords("ForeignKey");

            Assert.Equal(27, tables.Count);
            Assert.Equal(27, primaryKeys.Count);
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
            var loadTimestampColumn = GetColumn(columns, customerHub.Id, "LoadTimestamp");
            var auditIdColumn = GetColumn(columns, customerHub.Id, "AuditId");
            Assert.Equal("CONVERT(datetime2(7), SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'))", loadTimestampColumn.Values["DefaultExpressionSql"]);
            Assert.Equal("sqlserver:type:bigint", auditIdColumn.Values["MetaDataTypeId"]);
            Assert.Equal("CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))", auditIdColumn.Values["DefaultExpressionSql"]);
            Assert.Contains(primaryKeys, row => row.Id == "RawVault.dbo.H_Customer.pk.PK_H_Customer" && string.Equals(row.Values["Name"], "PK_H_Customer", StringComparison.Ordinal));

            var orderCustomerLink = tables.Single(row => row.Values.TryGetValue("Name", out var name) && string.Equals(name, "L_OrderCustomer", StringComparison.Ordinal));
            Assert.Equal("RawVault.dbo.L_OrderCustomer", orderCustomerLink.Id);
            Assert.Contains(columns, row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == orderCustomerLink.Id && row.Values.TryGetValue("Name", out var name) && string.Equals(name, "OrderHashKey", StringComparison.Ordinal));
            Assert.Contains(columns, row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == orderCustomerLink.Id && row.Values.TryGetValue("Name", out var name) && string.Equals(name, "CustomerHashKey", StringComparison.Ordinal));
            Assert.Contains(foreignKeys, row => row.Id == "RawVault.dbo.L_OrderCustomer.fk.FK_L_OrderCustomer_OrderHashKey");

            var satelliteTables = tables
                .Where(row => row.Values["Name"].StartsWith("HS_", StringComparison.Ordinal) || row.Values["Name"].StartsWith("LS_", StringComparison.Ordinal))
                .ToArray();
            Assert.Equal(13, satelliteTables.Length);
            foreach (var satelliteTable in satelliteTables)
            {
                var parentHashKeyName = satelliteTable.Values["Name"].StartsWith("HS_", StringComparison.Ordinal)
                    ? "HubHashKey"
                    : "LinkHashKey";
                AssertSatellitePrimaryKey(primaryKeys, primaryKeyColumns, columns, satelliteTable, parentHashKeyName, "LoadTimestamp");
            }
        }
        finally
        {
            DeleteDirectoryIfExists(Path.GetDirectoryName(targetPath)!);
        }
    }

    [Fact]
    public async Task ConvertAsync_OmitsOptionalBusinessImplementationColumnWhenNameAndTypeAreAbsent()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var implementationPath = Path.Combine(root, "Implementation");
        var targetPath = Path.Combine(root, "MetaSql");
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");

        try
        {
            var implementation = await TypedWorkspaceModelMapper.LoadAsync<MetaDataVaultImplementationModel>(
                GetImplementationWorkspacePath(repoRoot),
                searchUpward: false);
            var hubImplementation = Assert.Single(implementation.BusinessHubImplementationList);
            hubImplementation.LoadTimestampColumnName = null;
            hubImplementation.LoadTimestampDataTypeId = null;
            await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.SaveAsync(implementation, implementationPath);

            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                                implementationPath,
                databaseName: "BusinessVault");

            var customerHub = GetTable(sqlWorkspace.Instance.GetOrCreateEntityRecords("Table"), "BH_Customer");
            Assert.DoesNotContain(
                sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn"),
                row => row.RelationshipIds.TryGetValue("TableId", out var tableId) &&
                       tableId == customerHub.Id &&
                       row.Values.TryGetValue("Name", out var name) &&
                       string.Equals(name, "LoadTimestamp", StringComparison.Ordinal));
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_RejectsPartiallySpecifiedOptionalBusinessImplementationColumn()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var implementationPath = Path.Combine(root, "Implementation");
        var targetPath = Path.Combine(root, "MetaSql");
        var workspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");

        try
        {
            var implementation = await TypedWorkspaceModelMapper.LoadAsync<MetaDataVaultImplementationModel>(
                GetImplementationWorkspacePath(repoRoot),
                searchUpward: false);
            var hubImplementation = Assert.Single(implementation.BusinessHubImplementationList);
            hubImplementation.LoadTimestampDataTypeId = null;
            await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.SaveAsync(implementation, implementationPath);

            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                                implementationPath,
                databaseName: "BusinessVault"));

            Assert.Contains("BusinessOptionalColumnInvalid", error.Message, StringComparison.Ordinal);
            Assert.Contains("ColumnKind=LoadTimestamp", error.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_ProjectsRawLinkRolesInDeterministicNameOrder()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var model = MetaRawDataVaultModel.CreateEmpty();
            var employee = new RawHub { Id = "RawHub:Employee", Name = "Employee" };
            var department = new RawHub { Id = "RawHub:Department", Name = "Department" };
            var project = new RawHub { Id = "RawHub:Project", Name = "Project" };
            var assignment = new RawLink { Id = "RawLink:Assignment", Name = "Assignment", LinkKind = "standard" };

            model.RawHubList.Add(employee);
            model.RawHubList.Add(department);
            model.RawHubList.Add(project);
            model.RawLinkList.Add(assignment);
            model.RawLinkRoleList.Add(new RawLinkRole
            {
                Id = "RawLinkRole:Assignment:Employee",
                Name = "Employee",
                RawHub = employee,
                RawLink = assignment,
            });
            model.RawLinkRoleList.Add(new RawLinkRole
            {
                Id = "RawLinkRole:Assignment:Department",
                Name = "Department",
                RawHub = department,
                RawLink = assignment,
            });
            model.RawLinkRoleList.Add(new RawLinkRole
            {
                Id = "RawLinkRole:Assignment:AssignedProject",
                Name = "AssignedProject",
                RawHub = project,
                RawLink = assignment,
            });

            await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath);

            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "RawVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var assignmentTable = GetTable(tables, "L_Assignment");

            var columnNames = columns
                .Where(row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == assignmentTable.Id)
                .OrderBy(row => int.Parse(row.Values["Ordinal"], CultureInfo.InvariantCulture))
                .Select(row => row.Values["Name"])
                .ToList();

            Assert.Equal(
                new[]
                {
                    "HashKey",
                    "AssignedProjectHashKey",
                    "DepartmentHashKey",
                    "EmployeeHashKey",
                    "LoadTimestamp",
                    "RecordSource",
                    "AuditId",
                },
                columnNames);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_RejectsDuplicateRawLinkRoleNamesWithinOneLink()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var model = MetaRawDataVaultModel.CreateEmpty();
            var employee = new RawHub { Id = "RawHub:Employee", Name = "Employee" };
            var manager = new RawHub { Id = "RawHub:Manager", Name = "Manager" };
            var assignment = new RawLink { Id = "RawLink:Assignment", Name = "Assignment", LinkKind = "standard" };

            model.RawHubList.Add(employee);
            model.RawHubList.Add(manager);
            model.RawLinkList.Add(assignment);
            model.RawLinkRoleList.Add(new RawLinkRole
            {
                Id = "RawLinkRole:Assignment:Employee",
                Name = "Participant",
                RawHub = employee,
                RawLink = assignment,
            });
            model.RawLinkRoleList.Add(new RawLinkRole
            {
                Id = "RawLinkRole:Assignment:Manager",
                Name = "Participant",
                RawHub = manager,
                RawLink = assignment,
            });

            await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath);

            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "RawVault"));

            Assert.Contains("RawLinkRoleNameDuplicate", error.Message, StringComparison.Ordinal);
            Assert.Contains("RawLinkId=RawLink:Assignment", error.Message, StringComparison.Ordinal);
            Assert.Contains("RoleName=Participant", error.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_UsesUnderscorePrefixWhenFieldCollidesWithTechnicalName()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var model = MetaRawDataVaultModel.CreateEmpty();

            var field = new Field
            {
                Id = "Field:Customer:HashKey",
                Name = "HashKey",
                DataTypeId = "sqlserver:type:nvarchar",
            };
            var fieldDetail = new FieldDataTypeDetail
            {
                Id = "FieldDetail:Customer:HashKey:Length",
                Name = "Length",
                Value = "50",
                Field = field,
            };
            var rawHub = new RawHub
            {
                Id = "RawHub:Customer",
                Name = "Customer",
            };
            var rawHubKeyPart = new RawHubKeyPart
            {
                Id = "RawHubKeyPart:Customer:HashKey",
                Name = "HashKey",
                RawHub = rawHub,
                Field = field,
            };

            model.FieldList.Add(field);
            model.FieldDataTypeDetailList.Add(fieldDetail);
            model.RawHubList.Add(rawHub);
            model.RawHubKeyPartList.Add(rawHubKeyPart);

            await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath);

            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
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

            var businessKeyColumn = columns.Single(row =>
                row.RelationshipIds.TryGetValue("TableId", out var tableId) &&
                tableId == customerHub.Id &&
                string.Equals(row.Values["Name"], "_HashKey", StringComparison.Ordinal));
            Assert.Equal("true", businessKeyColumn.Values["IsNullable"]);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_LowersRawFieldSqlAliasesToDeployableSqlServerTypes()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var model = MetaRawDataVaultModel.CreateEmpty();

            var field = new Field
            {
                Id = "Field:Customer:CustomerName",
                Name = "CustomerName",
                DataTypeId = "sqlserver:type:Name",
            };
            var fieldDetail = new FieldDataTypeDetail
            {
                Id = "FieldDetail:Customer:CustomerName:Length",
                Name = "Length",
                Value = "50",
                Field = field,
            };
            var systemNameField = new Field
            {
                Id = "Field:Customer:SystemName",
                Name = "SystemName",
                DataTypeId = "sqlserver:type:sysname",
            };
            var systemNameFieldDetail = new FieldDataTypeDetail
            {
                Id = "FieldDetail:Customer:SystemName:Length",
                Name = "Length",
                Value = "128",
                Field = systemNameField,
            };
            var rawHub = new RawHub
            {
                Id = "RawHub:Customer",
                Name = "Customer",
            };
            var rawHubKeyPart = new RawHubKeyPart
            {
                Id = "RawHubKeyPart:Customer:ZCustomerName",
                Name = "CustomerName",
                RawHub = rawHub,
                Field = field,
            };
            var sysnameRawHubKeyPart = new RawHubKeyPart
            {
                Id = "RawHubKeyPart:Customer:ASystemName",
                Name = "SystemName",
                RawHub = rawHub,
                Field = systemNameField,
            };

            model.FieldList.Add(field);
            model.FieldList.Add(systemNameField);
            model.FieldDataTypeDetailList.Add(fieldDetail);
            model.FieldDataTypeDetailList.Add(systemNameFieldDetail);
            model.RawHubList.Add(rawHub);
            model.RawHubKeyPartList.Add(rawHubKeyPart);
            model.RawHubKeyPartList.Add(sysnameRawHubKeyPart);

            await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath);

            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "RawVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var details = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumnDataTypeDetail");
            var customerHub = GetTable(tables, "H_Customer");
            var projectedBusinessKeyColumns = columns
                .Where(row => row.RelationshipIds.TryGetValue("TableId", out var tableId) && tableId == customerHub.Id)
                .Where(row => row.Values["Name"] is "CustomerName" or "SystemName")
                .OrderBy(row => int.Parse(row.Values["Ordinal"], CultureInfo.InvariantCulture))
                .Select(row => row.Values["Name"])
                .ToArray();
            var customerName = GetColumn(columns, customerHub.Id, "CustomerName");

            Assert.Equal(["CustomerName", "SystemName"], projectedBusinessKeyColumns);
            Assert.Equal("sqlserver:type:nvarchar", customerName.Values["MetaDataTypeId"]);
            Assert.Equal("50", GetDetailValue(details, customerName.Id, "Length"));

            var systemName = GetColumn(columns, customerHub.Id, "SystemName");
            Assert.Equal("sqlserver:type:nvarchar", systemName.Values["MetaDataTypeId"]);
            Assert.Equal("128", GetDetailValue(details, systemName.Id, "Length"));
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_RejectsSqlServerPhysicalIdentifiersLongerThan128Characters()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "RawDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var model = MetaRawDataVaultModel.CreateEmpty();
            var longChildName = "SalesOrderDetailProductModelProductDescriptionCultureVersionHistoryRelationship";
            var longParentName = "ProductModelProductDescriptionCultureLocalizationHistoryReference";
            var longLinkName = longChildName + longParentName;

            var childField = new Field
            {
                Id = "Field:" + longChildName + ":Id",
                Name = longChildName + "IdentifierForAuditableRelationshipReplay",
                DataTypeId = "sqlserver:type:nvarchar",
            };
            var parentField = new Field
            {
                Id = "Field:" + longParentName + ":Id",
                Name = longParentName + "IdentifierForAuditableRelationshipReplay",
                DataTypeId = "sqlserver:type:nvarchar",
            };
            var childFieldDetail = new FieldDataTypeDetail
            {
                Id = childField.Id + ":Length",
                Name = "Length",
                Value = "50",
                Field = childField,
            };
            var parentFieldDetail = new FieldDataTypeDetail
            {
                Id = parentField.Id + ":Length",
                Name = "Length",
                Value = "50",
                Field = parentField,
            };
            var childHub = new RawHub
            {
                Id = "RawHub:" + longChildName,
                Name = longChildName,
            };
            var parentHub = new RawHub
            {
                Id = "RawHub:" + longParentName,
                Name = longParentName,
            };
            var childHubKeyPart = new RawHubKeyPart
            {
                Id = "RawHubKeyPart:" + longChildName,
                Name = "Identifier",
                RawHub = childHub,
                Field = childField,
            };
            var parentHubKeyPart = new RawHubKeyPart
            {
                Id = "RawHubKeyPart:" + longParentName,
                Name = "Identifier",
                RawHub = parentHub,
                Field = parentField,
            };
            var rawLink = new RawLink
            {
                Id = "RawLink:" + longLinkName,
                Name = longLinkName,
                LinkKind = "standard",
            };
            var childLinkRole = new RawLinkRole
            {
                Id = "RawLinkRole:" + longChildName,
                Name = longChildName + "Role",
                RawHub = childHub,
                RawLink = rawLink,
            };
            var parentLinkRole = new RawLinkRole
            {
                Id = "RawLinkRole:" + longParentName,
                Name = longParentName + "Role",
                RawHub = parentHub,
                RawLink = rawLink,
            };

            model.FieldList.Add(childField);
            model.FieldList.Add(parentField);
            model.FieldDataTypeDetailList.Add(childFieldDetail);
            model.FieldDataTypeDetailList.Add(parentFieldDetail);
            model.RawHubList.Add(childHub);
            model.RawHubList.Add(parentHub);
            model.RawHubKeyPartList.Add(childHubKeyPart);
            model.RawHubKeyPartList.Add(parentHubKeyPart);
            model.RawLinkList.Add(rawLink);
            model.RawLinkRoleList.Add(childLinkRole);
            model.RawLinkRoleList.Add(parentLinkRole);

            await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath);

            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "RawVault"));

            Assert.Contains("SqlServerIdentifierTooLong", error.Message, StringComparison.Ordinal);
            Assert.Contains("IdentifierKind=Table", error.Message, StringComparison.Ordinal);
            Assert.Contains("SQL Server identifiers must contain at most 128 characters.", error.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_RejectsLongSqlServerDatabaseIdentifierForBusinessProjection()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var workspacePath = Path.Combine(
            repoRoot,
            "MetaDataVault",
            "Workspaces",
            "SampleBusinessDataVaultCommerceHelpers");
        var databaseName = new string('D', 129);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
            workspacePath,
            GetImplementationWorkspacePath(repoRoot),
            databaseName));

        Assert.Contains("SqlServerIdentifierTooLong", error.Message, StringComparison.Ordinal);
        Assert.Contains("IdentifierKind=Database", error.Message, StringComparison.Ordinal);
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
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var primaryKeys = sqlWorkspace.Instance.GetOrCreateEntityRecords("PrimaryKey");
            var primaryKeyColumns = sqlWorkspace.Instance.GetOrCreateEntityRecords("PrimaryKeyColumn");
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

            AssertSatellitePrimaryKey(primaryKeys, primaryKeyColumns, columns, GetTable(tables, "BHS_Customer_Profile"), "HubHashKey", "LoadTimestamp");
            AssertSatellitePrimaryKey(primaryKeys, primaryKeyColumns, columns, GetTable(tables, "BLS_CustomerOrder_Status"), "LinkHashKey", "LoadTimestamp");
            AssertSatellitePrimaryKey(primaryKeys, primaryKeyColumns, columns, GetTable(tables, "RSAT_Status_Current"), "ReferenceHashKey", "LoadTimestamp");
            Assert.Equal("true", GetColumn(columns, GetTable(tables, "BHS_Customer_Profile").Id, "CustomerName").Values["IsNullable"]);
            Assert.Equal("true", GetColumn(columns, GetTable(tables, "BLS_CustomerOrder_Status").Id, "StatusCode").Values["IsNullable"]);
            Assert.Equal("true", GetColumn(columns, GetTable(tables, "RSAT_Status_Current").Id, "StatusName").Values["IsNullable"]);
            Assert.Equal("false", GetColumn(columns, customerSnapshotPit.Id, "ProfileLoadTimestamp").Values["IsNullable"]);
        }
        finally
        {
            DeleteDirectoryIfExists(Path.GetDirectoryName(targetPath)!);
        }
    }

    [Fact]
    public async Task ConvertAsync_ProjectsBusinessPayloadAndPointInTimeReferencesInNameOrder()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var sourceWorkspacePath = Path.Combine(repoRoot, "MetaDataVault", "Workspaces", "SampleBusinessDataVaultCommerceHelpers");
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var model = await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.LoadAsync<MetaBusinessDataVaultModel>(sourceWorkspacePath, searchUpward: false);
            var customerProfile = model.BusinessHubSatelliteList.Single(row => row.Id == "CustomerProfile");
            var customerOrderStatus = model.BusinessLinkSatelliteList.Single(row => row.Id == "CustomerOrderStatus");

            customerProfile.BusinessSatellite.Name = "ZuluProfile";
            customerOrderStatus.BusinessSatellite.Name = "AlphaStatus";
            model.BusinessSatelliteAttributeList.Add(new BusinessSatelliteAttribute
            {
                Id = "zulu-payload-id",
                BusinessSatellite = customerProfile.BusinessSatellite,
                DataTypeId = "meta:type:String",
                Name = "AlphaPayload",
            });
            model.BusinessSatelliteAttributeList.Add(new BusinessSatelliteAttribute
            {
                Id = "alpha-payload-id",
                BusinessSatellite = customerProfile.BusinessSatellite,
                DataTypeId = "meta:type:String",
                Name = "ZuluPayload",
            });
            await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath);

            var sqlWorkspace = await Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var profileTable = GetTable(tables, "BHS_Customer_ZuluProfile");
            var orderedPayloadColumns = columns
                .Where(row => row.RelationshipIds.GetValueOrDefault("TableId") == profileTable.Id)
                .Where(row => row.Values["Name"] is "AlphaPayload" or "CustomerName" or "ZuluPayload")
                .OrderBy(row => int.Parse(row.Values["Ordinal"], CultureInfo.InvariantCulture))
                .Select(row => row.Values["Name"])
                .ToArray();
            var pointInTimeTable = GetTable(tables, "PIT_CustomerSnapshot");
            var orderedSatelliteReferences = columns
                .Where(row => row.RelationshipIds.GetValueOrDefault("TableId") == pointInTimeTable.Id)
                .Where(row => row.Values["Name"] is "AlphaStatusLoadTimestamp" or "ZuluProfileLoadTimestamp")
                .OrderBy(row => int.Parse(row.Values["Ordinal"], CultureInfo.InvariantCulture))
                .Select(row => row.Values["Name"])
                .ToArray();

            Assert.Equal(["AlphaPayload", "CustomerName", "ZuluPayload"], orderedPayloadColumns);
            Assert.Equal(["AlphaStatusLoadTimestamp", "ZuluProfileLoadTimestamp"], orderedSatelliteReferences);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
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
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            var tables = sqlWorkspace.Instance.GetOrCreateEntityRecords("Table");
            var columns = sqlWorkspace.Instance.GetOrCreateEntityRecords("TableColumn");
            var primaryKeys = sqlWorkspace.Instance.GetOrCreateEntityRecords("PrimaryKey");
            var primaryKeyColumns = sqlWorkspace.Instance.GetOrCreateEntityRecords("PrimaryKeyColumn");
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

            AssertSatellitePrimaryKey(primaryKeys, primaryKeyColumns, columns, GetTable(tables, "BSALS_CustomerMatch_Evidence"), "LinkHashKey", "LoadTimestamp");
            AssertSatellitePrimaryKey(primaryKeys, primaryKeyColumns, columns, GetTable(tables, "BHALS_EmployeeManager_Line"), "LinkHashKey", "LoadTimestamp");
            Assert.Equal("true", GetColumn(columns, GetTable(tables, "BSALS_CustomerMatch_Evidence").Id, "MatchScore").Values["IsNullable"]);
            Assert.Equal("true", GetColumn(columns, GetTable(tables, "BHALS_EmployeeManager_Line").Id, "LineType").Values["IsNullable"]);
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
    public async Task ConvertAsync_RejectsBusinessBridgeTraversalOutsideAnchor()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            var createResult = RunBusinessCli($"create --xml \"{workspacePath}\"");
            Assert.Equal(0, createResult.ExitCode);

            Assert.Equal(0, RunBusinessCli($"add-hub --workspace \"{workspacePath}\" --id Customer --name Customer").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-hub --workspace \"{workspacePath}\" --id Order --name Order").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-link --workspace \"{workspacePath}\" --id CustomerOrder --name CustomerOrder").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-link-role --workspace \"{workspacePath}\" --id CustomerOrderCustomer --link CustomerOrder --hub Customer --name Customer").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-link-role --workspace \"{workspacePath}\" --id CustomerOrderOrder --link CustomerOrder --hub Order --name Order").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-bridge --workspace \"{workspacePath}\" --id CustomerShipmentTraversal --hub Customer --name CustomerShipmentTraversal").ExitCode);

            var model = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaBusinessDataVaultModel>(workspacePath);
            var bridge = Assert.Single(model.BusinessBridgeList);
            var customerRole = model.BusinessLinkRoleList.Single(row => row.Id == "CustomerOrderCustomer");
            var orderRole = model.BusinessLinkRoleList.Single(row => row.Id == "CustomerOrderOrder");
            model.BusinessBridgeTraversalList.Add(new BusinessBridgeTraversal
            {
                Id = "CustomerShipmentTraversalOrderCustomer",
                BusinessBridge = bridge,
                SourceRole = orderRole,
                TargetRole = customerRole,
            });
            Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(model, workspacePath);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault"));
            Assert.Contains("BusinessBridgeTraversalInvalid", exception.Message, StringComparison.Ordinal);
            Assert.Contains("Failure=DoesNotStartAtAnchor", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_RejectsBusinessHubKeyPartPrecedenceBranch()
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var root = Path.Combine(Path.GetTempPath(), "metadatavault-tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(root, "BusinessDataVault");
        var targetPath = Path.Combine(root, "MetaSql");

        try
        {
            Assert.Equal(0, RunBusinessCli($"create --xml \"{workspacePath}\"").ExitCode);
            Assert.Equal(0, RunBusinessCli($"add-hub --workspace \"{workspacePath}\" --id Customer --name Customer").ExitCode);

            var model = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaBusinessDataVaultModel>(workspacePath);
            var customer = Assert.Single(model.BusinessHubList);
            var countryCode = new BusinessHubKeyPart
            {
                Id = "CustomerCountryCode",
                BusinessHub = customer,
                DataTypeId = "meta:type:String",
                Name = "CountryCode",
            };
            model.BusinessHubKeyPartList.Add(countryCode);
            model.BusinessHubKeyPartList.Add(new BusinessHubKeyPart
            {
                Id = "CustomerNumber",
                BusinessHub = customer,
                DataTypeId = "meta:type:String",
                Name = "CustomerNumber",
                PreviousKeyPart = countryCode,
            });
            model.BusinessHubKeyPartList.Add(new BusinessHubKeyPart
            {
                Id = "CustomerSource",
                BusinessHub = customer,
                DataTypeId = "meta:type:String",
                Name = "SourceSystem",
                PreviousKeyPart = countryCode,
            });
            Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(model, workspacePath);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault"));
            Assert.Contains("BusinessKeyPartChainInvalid", exception.Message, StringComparison.Ordinal);
            Assert.Contains("Failure=Branch", exception.Message, StringComparison.Ordinal);
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
                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault");

            await WorkspaceSurface.CreateAsync(sqlWorkspace, targetPath, "xml");
            var reloaded = await XmlWorkspaceReader.OpenAsync(targetPath);

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
            var model = await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.LoadAsync<MetaBusinessDataVaultModel>(sourceWorkspacePath, searchUpward: false);

            foreach (var row in model.BusinessHubKeyPartList)
            {
                row.DataTypeId = "sqlserver:type:nvarchar";
            }

            foreach (var row in model.BusinessReferenceKeyPartList)
            {
                row.DataTypeId = "sqlserver:type:nvarchar";
            }

            foreach (var row in model.BusinessSatelliteAttributeList)
            {
                row.DataTypeId = "sqlserver:type:nvarchar";
            }

            foreach (var row in model.BusinessPointInTimeStampList)
            {
                row.DataTypeId = "sqlserver:type:nvarchar";
            }

            await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath);

            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault"));

            Assert.Contains("sqlserver:type:nvarchar", error.Message, StringComparison.Ordinal);
            Assert.Contains("BusinessColumnTypeLoweringInvalid", error.Message, StringComparison.Ordinal);
            Assert.Contains("SourceSystemId=SqlServer", error.Message, StringComparison.Ordinal);
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
            var model = await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.LoadAsync<MetaBusinessDataVaultModel>(sourceWorkspacePath, searchUpward: false);
            model.BusinessHubKeyPartList[0].DataTypeId = "meta:type:Xml";
            await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath);

            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault"));

            Assert.Contains("meta:type:Xml", error.Message, StringComparison.Ordinal);
            Assert.Contains("BusinessColumnTypeLoweringInvalid", error.Message, StringComparison.Ordinal);
            Assert.Contains("SourceSystemId=Meta", error.Message, StringComparison.Ordinal);
            Assert.Contains("MappingCount=0", error.Message, StringComparison.Ordinal);
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
            var model = await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.LoadAsync<MetaBusinessDataVaultModel>(sourceWorkspacePath, searchUpward: false);
            model.BusinessHubKeyPartList[0].DataTypeId = "sqlserver:type:not-real";
            await Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.SaveAsync(model, workspacePath);

            var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Converter.ConvertAsync(
                workspacePath,
                                GetImplementationWorkspacePath(repoRoot),
                databaseName: "BusinessVault"));

            Assert.Contains("sqlserver:type:not-real", error.Message, StringComparison.Ordinal);
            Assert.Contains("BusinessColumnTypeLoweringInvalid", error.Message, StringComparison.Ordinal);
            Assert.Contains("SourceSystemId=NULL", error.Message, StringComparison.Ordinal);
            Assert.Contains("MappingCount=0", error.Message, StringComparison.Ordinal);
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
        var startInfo = CliTestSupport.CreateManagedCliStartInfo("meta-datavault-raw", arguments, repoRoot);

        return CliTestSupport.RunProcess(startInfo, "Could not start DataVault CLI process.");
    }

    private static (int ExitCode, string Output) RunBusinessCli(string arguments)
    {
        var repoRoot = CliTestSupport.FindRepositoryRoot();
        var startInfo = CliTestSupport.CreateManagedCliStartInfo("meta-datavault-business", arguments, repoRoot);

        return CliTestSupport.RunProcess(startInfo, "Could not start DataVault CLI process.");
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }

    private static Meta.Operations.Domain.GenericRecord GetTable(IReadOnlyList<Meta.Operations.Domain.GenericRecord> tables, string tableName)
    {
        return tables.Single(row => row.Values.TryGetValue("Name", out var name) && string.Equals(name, tableName, StringComparison.Ordinal));
    }

    private static List<string> GetColumnNames(IReadOnlyList<Meta.Operations.Domain.GenericRecord> columns, string tableId)
    {
        return columns
            .Where(row => row.RelationshipIds.TryGetValue("TableId", out var currentTableId) && currentTableId == tableId)
            .Select(row => row.Values["Name"])
            .ToList();
    }

    private static Meta.Operations.Domain.GenericRecord GetColumn(IReadOnlyList<Meta.Operations.Domain.GenericRecord> columns, string tableId, string columnName)
    {
        return columns.Single(row =>
            row.RelationshipIds.TryGetValue("TableId", out var currentTableId) &&
            currentTableId == tableId &&
            row.Values.TryGetValue("Name", out var currentName) &&
            string.Equals(currentName, columnName, StringComparison.Ordinal));
    }

    private static void AssertSatellitePrimaryKey(
        IReadOnlyList<Meta.Operations.Domain.GenericRecord> primaryKeys,
        IReadOnlyList<Meta.Operations.Domain.GenericRecord> primaryKeyColumns,
        IReadOnlyList<Meta.Operations.Domain.GenericRecord> columns,
        Meta.Operations.Domain.GenericRecord table,
        params string[] expectedColumnNames)
    {
        var primaryKey = primaryKeys.Single(row =>
            row.RelationshipIds.TryGetValue("TableId", out var tableId) &&
            tableId == table.Id);
        Assert.Equal("PK_" + table.Values["Name"], primaryKey.Values["Name"]);

        var actualColumnNames = primaryKeyColumns
            .Where(row => row.RelationshipIds.TryGetValue("PrimaryKeyId", out var primaryKeyId) && primaryKeyId == primaryKey.Id)
            .OrderBy(row => int.Parse(row.Values["Ordinal"], CultureInfo.InvariantCulture))
            .Select(row => columns.Single(column => column.Id == row.RelationshipIds["TableColumnId"]).Values["Name"])
            .ToArray();
        Assert.Equal(expectedColumnNames, actualColumnNames);
    }

    private static string GetDetailValue(IReadOnlyList<Meta.Operations.Domain.GenericRecord> details, string tableColumnId, string detailName)
    {
        return details.Single(row =>
            row.RelationshipIds.TryGetValue("TableColumnId", out var currentTableColumnId) &&
            currentTableColumnId == tableColumnId &&
            row.Values.TryGetValue("Name", out var currentName) &&
            string.Equals(currentName, detailName, StringComparison.Ordinal)).Values["Value"];
    }

}
