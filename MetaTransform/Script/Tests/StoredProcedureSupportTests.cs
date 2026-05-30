using MetaTransform.Binding;
using MetaTransformScript;
using MetaTransformScript.Sql;

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
            await service.ImportFromSqlCodeToWorkspaceAsync(
                """
                CREATE PROCEDURE dq.RunReview
                AS
                BEGIN
                    SELECT 1 AS ReviewRunId;
                END
                """,
                targetSqlIdentifier: null,
                newWorkspacePath: workspacePath);

            var model = MetaTransformScriptModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
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

        var statementKind = new TransformScriptStatementKindService().GetStatementKind(model, script);
        Assert.Equal(BoundStatementKind.StoredProcedure, statementKind);

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
    public async Task StoredProcedureContractService_AddsViewsAndRemovesContract()
    {
        var root = CreateTempRoot();
        var workspacePath = Path.Combine(root, "TransformScriptWS");

        try
        {
            var service = new MetaTransformScriptSqlService();
            await service.ImportFromSqlCodeToWorkspaceAsync(
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
            await service.ImportFromSqlCodeToWorkspaceAsync(
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
}
