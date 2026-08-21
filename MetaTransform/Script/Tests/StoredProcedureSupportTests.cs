using MetaTransform.Binding;
using MetaTransformBinding;
using MetaTransformScript;
using MetaTransformScript.Sql;
using MS = MetaSchema;

public sealed class StoredProcedureSupportTests
{
    [Fact]
    public async Task SqlImport_ModelsStoredProcedureAsBlobModule()
    {
        var root = CreateTempRoot();
        var workspacePath = Path.Combine(root, "TransformScriptWS");

        try
        {
            var service = new MetaTransformScriptSqlService();
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                CREATE PROCEDURE dq.RunReview
                AS
                BEGIN
                    SELECT 1 AS ReviewRunId;
                END
                """,
                targetSqlIdentifier: null,
                newWorkspacePath: workspacePath);

            var model = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(workspacePath, searchUpward: false);
            var script = Assert.Single(model.TransformScriptList);
            Assert.Equal("dq.RunReview", script.Name);
            Assert.Empty(model.TransformScriptStatementLinkList);

            var storedProcedure = Assert.Single(model.ScriptObjectStoredProcedureList);
            Assert.Equal(script.Id, storedProcedure.TransformScript.Id);
            Assert.Contains("SELECT 1 AS ReviewRunId", storedProcedure.DefinitionSql, StringComparison.OrdinalIgnoreCase);

            var modules = service.ExportModuleDefinitions(model);
            var module = Assert.Single(modules);
            Assert.Equal(MetaTransformScriptSqlModuleKind.StoredProcedure, module.ModuleKind);
            Assert.Equal("dq", module.SchemaName);
            Assert.Equal("RunReview", module.ObjectName);
            Assert.StartsWith("CREATE PROCEDURE dq.RunReview", module.DefinitionSql, StringComparison.OrdinalIgnoreCase);

            Assert.Equal("EXEC dq.RunReview;", service.ExportToSqlCode(model));
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public void Binding_UsesStoredProcedureDeclarationRows()
    {
        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(
            """
            CREATE PROCEDURE dq.RunReview
            AS
            BEGIN
                EXEC audit.MarkStarted;
            END
            """);
        var script = Assert.Single(model.TransformScriptList);
        var storedProcedure = Assert.Single(model.ScriptObjectStoredProcedureList);

        var contract = CreateContract(storedProcedure);
        model.StoredProcedureContractList.Add(contract);
        model.StoredProcedureContractOperationList.Add(new StoredProcedureContractOperation
        {
            Id = "stored-procedure-operation:1",
            StoredProcedureContract = contract,
            Ordinal = "10",
            OperationKind = "Read",
            SqlIdentifier = "src.Customer",
            AccessRole = "CustomerInput"
        });
        model.StoredProcedureContractOperationList.Add(new StoredProcedureContractOperation
        {
            Id = "stored-procedure-operation:2",
            StoredProcedureContract = contract,
            Ordinal = "20",
            OperationKind = "Mutation",
            SqlIdentifier = "dq.CustomerReview"
        });
        model.StoredProcedureContractOperationList.Add(new StoredProcedureContractOperation
        {
            Id = "stored-procedure-operation:3",
            StoredProcedureContract = contract,
            Ordinal = "30",
            OperationKind = "Call",
            SqlIdentifier = "audit.MarkStarted"
        });

        var statementKind = new TransformScriptNavigator(model).GetTransformScriptStatementKind(script);
        Assert.Equal(TransformScriptStatementKind.StoredProcedure, statementKind);

        var bound = new TransformBindingService().BindTransform(model, script);
        Assert.False(bound.HasErrors);
        Assert.Contains(bound.Rowsets, rowset =>
            string.Equals(rowset.DerivationKind, "Source", StringComparison.Ordinal) &&
            string.Equals(rowset.SqlIdentifier, "src.Customer", StringComparison.Ordinal));
        Assert.Contains(bound.Rowsets, rowset =>
            string.Equals(rowset.DerivationKind, "Target", StringComparison.Ordinal) &&
            string.Equals(rowset.SqlIdentifier, "dq.CustomerReview", StringComparison.Ordinal));
    }

    [Fact]
    public void Binding_RejectsStoredProcedureWithoutContract()
    {
        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(
            """
            CREATE PROCEDURE dq.RunReview
            AS
            BEGIN
                SELECT 1 AS ReviewRunId;
            END
            """);
        var script = Assert.Single(model.TransformScriptList);

        var bound = new TransformBindingService().BindTransform(model, script);

        Assert.True(bound.HasErrors);
        Assert.Contains(bound.Issues, issue =>
            string.Equals(issue.Code, "StoredProcedureContractMissing", StringComparison.Ordinal));
    }

    [Fact]
    public void Binding_RejectsStoredProcedureWithMultipleContracts()
    {
        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(
            """
            CREATE PROCEDURE dq.RunReview
            AS
            BEGIN
                SELECT 1 AS ReviewRunId;
            END
            """);
        var script = Assert.Single(model.TransformScriptList);
        var storedProcedure = Assert.Single(model.ScriptObjectStoredProcedureList);

        model.StoredProcedureContractList.Add(new StoredProcedureContract
        {
            Id = "stored-procedure-contract:1",
            ScriptObjectStoredProcedure = storedProcedure
        });
        model.StoredProcedureContractList.Add(new StoredProcedureContract
        {
            Id = "stored-procedure-contract:2",
            ScriptObjectStoredProcedure = storedProcedure
        });

        var bound = new TransformBindingService().BindTransform(model, script);

        Assert.True(bound.HasErrors);
        Assert.Contains(bound.Issues, issue =>
            string.Equals(issue.Code, "StoredProcedureContractInvalid", StringComparison.Ordinal));
    }

    [Fact]
    public void Binding_RejectsStoredProcedureWithMultipleResultRowsets()
    {
        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(
            """
            CREATE PROCEDURE dq.ExportReviewRows
            AS
            BEGIN
                SELECT 1 AS ReviewRunId;
                SELECT 2 AS ReviewIssueId;
            END
            """);
        var script = Assert.Single(model.TransformScriptList);
        var storedProcedure = Assert.Single(model.ScriptObjectStoredProcedureList);
        var contract = CreateContract(storedProcedure);
        model.StoredProcedureContractList.Add(contract);
        model.StoredProcedureResultRowsetItemList.Add(new StoredProcedureResultRowsetItem
        {
            Id = "stored-procedure-result-rowset:1",
            StoredProcedureContract = contract,
            Name = "ReviewRuns",
            Ordinal = "0"
        });
        model.StoredProcedureResultRowsetItemList.Add(new StoredProcedureResultRowsetItem
        {
            Id = "stored-procedure-result-rowset:2",
            StoredProcedureContract = contract,
            Name = "ReviewIssues",
            Ordinal = "1"
        });

        var bound = new TransformBindingService().BindTransform(model, script);

        Assert.True(bound.HasErrors);
        Assert.Contains(bound.Issues, issue =>
            string.Equals(issue.Code, "StoredProcedureResultRowsetInvalid", StringComparison.Ordinal));
    }

    [Fact]
    public void Binding_AcceptsStoredProcedureContractWithNoDeclaredEffects()
    {
        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(
            """
            CREATE PROCEDURE audit.MarkHeartbeat
            AS
            BEGIN
                SELECT 1 AS IsAlive;
            END
            """);
        var script = Assert.Single(model.TransformScriptList);
        var storedProcedure = Assert.Single(model.ScriptObjectStoredProcedureList);
        model.StoredProcedureContractList.Add(CreateContract(storedProcedure));

        var bound = new TransformBindingService().BindTransform(model, script);

        Assert.False(bound.HasErrors);
        Assert.Empty(bound.Rowsets);
    }

    [Fact]
    public async Task BindingWorkspace_ScopesStoredProcedureDeclaredColumnsPerOperationRowset()
    {
        var root = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(root, "TransformWS");
        var schemaWorkspacePath = Path.Combine(root, "SchemaWS");
        var bindingWorkspacePath = Path.Combine(root, "BindingWS");

        try
        {
            var service = new MetaTransformScriptSqlService();
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                CREATE PROCEDURE etl.FirstLoad
                AS
                BEGIN
                    SELECT 1 AS Marker;
                END
                """,
                targetSqlIdentifier: null,
                newWorkspacePath: transformWorkspacePath);
            await service.AddSqlCodeToWorkspaceAsync(
                """
                CREATE PROCEDURE etl.SecondLoad
                AS
                BEGIN
                    SELECT 1 AS Marker;
                END
                """,
                targetSqlIdentifier: null,
                workspacePath: transformWorkspacePath);

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            AddSingleReadContract(transformModel, "etl.FirstLoad", "src.SharedSource");
            AddSingleReadContract(transformModel, "etl.SecondLoad", "src.SharedSource");
            MetaTransformScriptTestHelper.SaveXml(transformModel, transformWorkspacePath);
            SaveSchemaWorkspace(schemaWorkspacePath, "Hairball", "src.SharedSource");

            var result = new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(
                transformWorkspacePath,
                [schemaWorkspacePath],
                schemaWorkspacePath,
                executeSystemName: "Hairball",
                executeSystemDefaultSchemaName: null,
                newWorkspacePath: bindingWorkspacePath);

            var bindingModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformBindingModel>(bindingWorkspacePath, searchUpward: false);
            Assert.Equal(2, result.TransformBindingCount);
            Assert.Equal(bindingModel.ColumnList.Count, bindingModel.ColumnList.Select(static item => item.Id).Distinct(StringComparer.Ordinal).Count());
            Assert.Equal(2, bindingModel.RowsetList.Count(static item =>
                string.Equals(item.SqlIdentifier, "src.SharedSource", StringComparison.Ordinal)));
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public async Task BindingWorkspace_PersistsResolvedIdentityForEachNonCallStoredProcedureOperation()
    {
        var root = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(root, "TransformWS");
        var sourceSchemaWorkspacePath = Path.Combine(root, "SourceSchemaWS");
        var targetSchemaWorkspacePath = Path.Combine(root, "TargetSchemaWS");
        var bindingWorkspacePath = Path.Combine(root, "BindingWS");

        try
        {
            var service = new MetaTransformScriptSqlService();
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                CREATE PROCEDURE etl.RefreshStage
                AS
                BEGIN
                    SELECT 1 AS Marker;
                END
                """,
                targetSqlIdentifier: null,
                newWorkspacePath: transformWorkspacePath);

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var storedProcedure = Assert.Single(transformModel.ScriptObjectStoredProcedureList);
            var contract = CreateContract(storedProcedure);
            contract.Id = $"{storedProcedure.Id}:contract";
            transformModel.StoredProcedureContractList.Add(contract);
            transformModel.StoredProcedureContractOperationList.Add(new StoredProcedureContractOperation
            {
                Id = $"{contract.Id}:read",
                StoredProcedureContract = contract,
                Ordinal = "10",
                OperationKind = "Read",
                SqlIdentifier = "src.SourceCustomer"
            });
            transformModel.StoredProcedureContractOperationList.Add(new StoredProcedureContractOperation
            {
                Id = $"{contract.Id}:append",
                StoredProcedureContract = contract,
                Ordinal = "20",
                OperationKind = "Append",
                SqlIdentifier = "dbo.StageCustomer"
            });
            transformModel.StoredProcedureContractOperationList.Add(new StoredProcedureContractOperation
            {
                Id = $"{contract.Id}:call",
                StoredProcedureContract = contract,
                Ordinal = "30",
                OperationKind = "Call",
                SqlIdentifier = "audit.MarkRefresh"
            });
            MetaTransformScriptTestHelper.SaveXml(transformModel, transformWorkspacePath);
            SaveSchemaWorkspace(sourceSchemaWorkspacePath, "ExecutionDb", "src.SourceCustomer");
            SaveSchemaWorkspace(targetSchemaWorkspacePath, "WarehouseDb", "dbo.StageCustomer");

            var result = new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(
                transformWorkspacePath,
                [sourceSchemaWorkspacePath],
                targetSchemaWorkspacePath,
                executeSystemName: "ExecutionDb",
                executeSystemDefaultSchemaName: null,
                newWorkspacePath: bindingWorkspacePath);

            Assert.Equal(0, result.ErrorCount);
            var bindingModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformBindingModel>(bindingWorkspacePath, searchUpward: false);
            var operationBindings = bindingModel.StoredProcedureOperationBindingList
                .OrderBy(static item => item.MetaTransformScriptStoredProcedureContractOperationId, StringComparer.Ordinal)
                .ToArray();
            Assert.Equal(2, operationBindings.Length);
            Assert.DoesNotContain(operationBindings, item =>
                string.Equals(item.MetaTransformScriptStoredProcedureContractOperationId, $"{contract.Id}:call", StringComparison.Ordinal));

            var resolvedOperations = bindingModel.ValidationStoredProcedureOperationLinkList
                .ToDictionary(
                    item => item.StoredProcedureOperationBinding.MetaTransformScriptStoredProcedureContractOperationId,
                    item => item.ResolvedSqlIdentifier,
                    StringComparer.Ordinal);
            Assert.Equal("ExecutionDb.src.SourceCustomer", resolvedOperations[$"{contract.Id}:read"]);
            Assert.Equal("WarehouseDb.dbo.StageCustomer", resolvedOperations[$"{contract.Id}:append"]);

            var sourceSchema = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MS.MetaSchemaModel>(sourceSchemaWorkspacePath, searchUpward: false);
            var targetSchema = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MS.MetaSchemaModel>(targetSchemaWorkspacePath, searchUpward: false);
            var revalidated = new TransformBindingValidationService().ApplyValidation(
                bindingModel,
                sourceSchema,
                targetSchema,
                TransformBindingValidationOptions.Create(
                    ignoredTargetColumnNames: null,
                    executeSystemName: "ExecutionDb",
                    executeSystemDefaultSchemaName: null));
            Assert.Equal(2, revalidated.ValidationStoredProcedureOperationLinkList.Count);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public async Task BindingWorkspace_RejectsStoredProcedureWriteOperationAgainstView()
    {
        var root = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(root, "TransformWS");
        var targetSchemaWorkspacePath = Path.Combine(root, "TargetSchemaWS");
        var bindingWorkspacePath = Path.Combine(root, "BindingWS");

        try
        {
            var service = new MetaTransformScriptSqlService();
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(
                "CREATE PROCEDURE etl.RefreshStage AS SELECT 1 AS Marker;",
                targetSqlIdentifier: null,
                newWorkspacePath: transformWorkspacePath);

            var transformModel = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var storedProcedure = Assert.Single(transformModel.ScriptObjectStoredProcedureList);
            var contract = CreateContract(storedProcedure);
            contract.Id = $"{storedProcedure.Id}:contract";
            transformModel.StoredProcedureContractList.Add(contract);
            transformModel.StoredProcedureContractOperationList.Add(new StoredProcedureContractOperation
            {
                Id = $"{contract.Id}:append",
                StoredProcedureContract = contract,
                Ordinal = "10",
                OperationKind = "Append",
                SqlIdentifier = "dbo.StageCustomer"
            });
            MetaTransformScriptTestHelper.SaveXml(transformModel, transformWorkspacePath);

            SaveSchemaWorkspace(targetSchemaWorkspacePath, "WarehouseDb", "dbo.StageCustomer");
            var targetSchema = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MS.MetaSchemaModel>(targetSchemaWorkspacePath, searchUpward: false);
            var table = Assert.Single(targetSchema.TableList);
            targetSchema.TableList.Clear();
            targetSchema.ViewList.Add(new MS.View
            {
                Id = table.Id,
                SchemaObject = table.SchemaObject
            });
            MetaTransformScriptTestHelper.SaveXml(targetSchema, targetSchemaWorkspacePath);

            var exception = Assert.Throws<TransformBindingValidationException>(() =>
                new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(
                    transformWorkspacePath,
                    sourceSchemaWorkspacePaths: [targetSchemaWorkspacePath],
                    targetSchemaWorkspacePath,
                    executeSystemName: "WarehouseDb",
                    executeSystemDefaultSchemaName: null,
                    newWorkspacePath: bindingWorkspacePath));

            Assert.Equal("TargetSchemaObjectNotWritable", exception.Code);
            Assert.Contains("read-only view", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("writable table contracts", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public async Task StoredProcedureContractService_AddsViewsAndRemovesContract()
    {
        var root = CreateTempRoot();
        var workspacePath = Path.Combine(root, "TransformScriptWS");

        try
        {
            var service = new MetaTransformScriptSqlService();
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                CREATE PROCEDURE dq.RunReview
                AS
                BEGIN
                    EXEC audit.MarkStarted;
                END
                """,
                targetSqlIdentifier: null,
                newWorkspacePath: workspacePath);

            var declaration = new StoredProcedureContractDeclaration(
                Operations:
                [
                    new StoredProcedureContractOperationDeclaration(10, "Read", "src.Customer", "CustomerInput"),
                    new StoredProcedureContractOperationDeclaration(20, "Mutation", "dq.CustomerReview"),
                    new StoredProcedureContractOperationDeclaration(30, "Call", "audit.MarkStarted")
                ],
                ResultRowsets: []);

            var result = await service.AddStoredProcedureContractAsync(
                workspacePath,
                "dq.RunReview",
                declaration);

            Assert.Equal(StoredProcedureContractState.Present, result.Item.ContractState);
            Assert.Equal(3, result.Item.OperationCount);
            Assert.Equal(1, result.Item.ReadOperationCount);
            Assert.Equal(1, result.Item.WriteOperationCount);
            Assert.Equal(1, result.Item.CallOperationCount);

            var inspect = await service.InspectStoredProcedureContractsAsync(workspacePath);
            var item = Assert.Single(inspect.Items);
            Assert.Equal(StoredProcedureContractState.Present, item.ContractState);
            Assert.Equal(0, inspect.MissingContractCount);

            var removal = await service.RemoveStoredProcedureContractAsync(workspacePath, "dq.RunReview");
            Assert.Equal(1, removal.ContractCount);
            Assert.Equal(3, removal.OperationCount);

            var afterRemoval = await service.InspectStoredProcedureContractsAsync(workspacePath);
            Assert.Equal(StoredProcedureContractState.Missing, Assert.Single(afterRemoval.Items).ContractState);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public async Task StoredProcedureContractService_RejectsMultipleResultRowsets()
    {
        var root = CreateTempRoot();
        var workspacePath = Path.Combine(root, "TransformScriptWS");

        try
        {
            var service = new MetaTransformScriptSqlService();
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                CREATE PROCEDURE dq.ExportReviewRows
                AS
                BEGIN
                    SELECT 1 AS ReviewRunId;
                    SELECT 2 AS ReviewIssueId;
                END
                """,
                targetSqlIdentifier: null,
                newWorkspacePath: workspacePath);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.AddStoredProcedureContractAsync(
                    workspacePath,
                    "dq.ExportReviewRows",
                    new StoredProcedureContractDeclaration(
                        Operations: [],
                        ResultRowsets:
                        [
                            new StoredProcedureResultRowsetDeclaration(
                                "ReviewRuns",
                                [new StoredProcedureResultColumnDeclaration("ReviewRunId", null, null)]),
                            new StoredProcedureResultRowsetDeclaration(
                                "ReviewIssues",
                                [new StoredProcedureResultColumnDeclaration("ReviewIssueId", null, null)])
                        ])));

            Assert.Contains("at most one result rowset", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    private static string CreateTempRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "MetaTransform.Script.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    private static void DeleteTempRoot(string root)
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static StoredProcedureContract CreateContract(
        ScriptObjectStoredProcedure storedProcedure) =>
        new()
        {
            Id = "stored-procedure-contract:1",
            ScriptObjectStoredProcedure = storedProcedure
        };

    private static void AddSingleReadContract(
        MetaTransformScriptModel model,
        string transformScriptName,
        string sourceSqlIdentifier)
    {
        var transformScript = model.TransformScriptList.Single(item =>
            string.Equals(item.Name, transformScriptName, StringComparison.Ordinal));
        var storedProcedure = model.ScriptObjectStoredProcedureList.Single(item =>
            string.Equals(item.TransformScript.Id, transformScript.Id, StringComparison.Ordinal));
        var contract = new StoredProcedureContract
        {
            Id = $"{storedProcedure.Id}:contract",
            ScriptObjectStoredProcedure = storedProcedure
        };
        model.StoredProcedureContractList.Add(contract);
        model.StoredProcedureContractOperationList.Add(new StoredProcedureContractOperation
        {
            Id = $"{contract.Id}:operation:1",
            StoredProcedureContract = contract,
            Ordinal = "10",
            OperationKind = "Read",
            SqlIdentifier = sourceSqlIdentifier,
            AccessRole = "Source"
        });
    }

    private static void SaveSchemaWorkspace(
        string workspacePath,
        string systemName,
        params string[] tableSqlIdentifiers)
    {
        var model = MS.MetaSchemaModel.CreateEmpty();
        var system = new MS.System
        {
            Id = $"{systemName}:system",
            Name = systemName
        };
        model.SystemList.Add(system);

        foreach (var tableSqlIdentifier in tableSqlIdentifiers)
        {
            var parts = tableSqlIdentifier.Split('.', 2);
            Assert.Equal(2, parts.Length);
            var schema = model.SchemaList.SingleOrDefault(item =>
                string.Equals(item.Name, parts[0], StringComparison.Ordinal)) ?? new MS.Schema
            {
                Id = $"{systemName}:schema:{parts[0]}",
                System = system,
                Name = parts[0]
            };
            if (!model.SchemaList.Contains(schema))
            {
                model.SchemaList.Add(schema);
            }

            var schemaObject = new MS.SchemaObject
            {
                Id = $"{systemName}:table:{parts[0]}:{parts[1]}",
                Schema = schema,
                Name = parts[1]
            };
            model.SchemaObjectList.Add(schemaObject);
            model.TableList.Add(new MS.Table
            {
                Id = schemaObject.Id,
                SchemaObject = schemaObject
            });
            model.FieldList.Add(new MS.Field
            {
                Id = $"{systemName}:field:{parts[0]}:{parts[1]}:Value",
                SchemaObject = schemaObject,
                MetaDataTypeId = "sqlserver:type:int",
                Name = "Value",
                Ordinal = "0",
                IsNullable = "false"
            });
        }

        MetaTransformScriptTestHelper.SaveXml(model, workspacePath);
    }
}
