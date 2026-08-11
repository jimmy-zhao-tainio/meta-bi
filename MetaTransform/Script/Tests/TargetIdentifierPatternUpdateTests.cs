using MetaTransformScript;
using MetaTransformScript.Sql;

public sealed class TargetIdentifierPatternUpdateTests
{
    [Fact]
    public async Task UpdateTargetIdentifiersFromPatternAsync_DerivesTargetFromScriptName()
    {
        const string targetViewSql = """
CREATE VIEW dbo.FactCustomer_TargetView
AS
SELECT
    1 AS CustomerId
""";
        const string ordinaryViewSql = """
CREATE VIEW dbo.vCustomerAudit
AS
SELECT
    1 AS CustomerId
""";

        var root = CreateTempRoot();
        var workspacePath = Path.Combine(root, "TransformWS");

        try
        {
            var service = new MetaTransformScriptSqlService();
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(
                targetViewSql,
                targetSqlIdentifier: null,
                workspacePath);
            await service.AddSqlCodeToWorkspaceAsync(
                ordinaryViewSql,
                targetSqlIdentifier: null,
                workspacePath);

            var result = await service.UpdateTargetIdentifiersFromPatternAsync(
                workspacePath,
                sourcePattern: "{schema}.{object}_TargetView",
                targetPattern: "Warehouse.{schema}.{object}");

            Assert.Equal(2, result.ScriptCount);
            Assert.Equal(1, result.MatchedCount);
            Assert.Equal(1, result.UpdatedCount);
            Assert.Equal(0, result.SkippedExistingCount);
            Assert.Equal(0, result.UnchangedCount);

            var model = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(workspacePath, searchUpward: false);
            Assert.Equal(
                "Warehouse.dbo.FactCustomer",
                GetTargetSqlIdentifier(model, "dbo.FactCustomer_TargetView"));
            Assert.True(string.IsNullOrWhiteSpace(GetTargetSqlIdentifier(model, "dbo.vCustomerAudit")));
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public async Task UpdateTargetIdentifiersFromPatternAsync_OnlyMissing_PreservesExistingTarget()
    {
        const string sql = """
CREATE VIEW dbo.FactCustomer_TargetView
AS
SELECT
    1 AS CustomerId
""";

        var root = CreateTempRoot();
        var workspacePath = Path.Combine(root, "TransformWS");

        try
        {
            var service = new MetaTransformScriptSqlService();
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(
                sql,
                targetSqlIdentifier: "ExistingWarehouse.dbo.FactCustomer",
                workspacePath);

            var result = await service.UpdateTargetIdentifiersFromPatternAsync(
                workspacePath,
                sourcePattern: "{schema}.{object}_TargetView",
                targetPattern: "Warehouse.{schema}.{object}",
                onlyMissing: true);

            Assert.Equal(1, result.MatchedCount);
            Assert.Equal(0, result.UpdatedCount);
            Assert.Equal(1, result.SkippedExistingCount);

            var model = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(workspacePath, searchUpward: false);
            Assert.Equal(
                "ExistingWarehouse.dbo.FactCustomer",
                GetTargetSqlIdentifier(model, "dbo.FactCustomer_TargetView"));
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public async Task UpdateTargetIdentifiersFromPatternAsync_DryRun_DoesNotPersistTarget()
    {
        const string sql = """
CREATE VIEW dbo.FactCustomer_TargetView
AS
SELECT
    1 AS CustomerId
""";

        var root = CreateTempRoot();
        var workspacePath = Path.Combine(root, "TransformWS");

        try
        {
            var service = new MetaTransformScriptSqlService();
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(
                sql,
                targetSqlIdentifier: null,
                workspacePath);

            var result = await service.UpdateTargetIdentifiersFromPatternAsync(
                workspacePath,
                sourcePattern: "{schema}.{object}_TargetView",
                targetPattern: "Warehouse.{schema}.{object}",
                dryRun: true);

            Assert.Equal(1, result.UpdatedCount);
            Assert.True(result.DryRun);

            var model = Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(workspacePath, searchUpward: false);
            Assert.True(string.IsNullOrWhiteSpace(GetTargetSqlIdentifier(model, "dbo.FactCustomer_TargetView")));
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public async Task UpdateTargetIdentifiersFromPatternAsync_RejectsMatchedFunction()
    {
        const string sql = """
CREATE FUNCTION dbo.fnCustomer_TargetView
(
    @CustomerId int
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        @CustomerId AS CustomerId
)
""";

        var root = CreateTempRoot();
        var workspacePath = Path.Combine(root, "TransformWS");

        try
        {
            var service = new MetaTransformScriptSqlService();
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(
                sql,
                targetSqlIdentifier: null,
                workspacePath);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.UpdateTargetIdentifiersFromPatternAsync(
                    workspacePath,
                    sourcePattern: "{schema}.{object}_TargetView",
                    targetPattern: "Warehouse.{schema}.{object}"));

            Assert.Contains("target identifiers can only be set on view scripts", exception.Message, StringComparison.OrdinalIgnoreCase);
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

    private static string? GetTargetSqlIdentifier(
        MetaTransformScriptModel model,
        string transformScriptName)
    {
        var script = model.TransformScriptList.Single(item =>
            string.Equals(item.Name, transformScriptName, StringComparison.Ordinal));
        return model.ScriptObjectViewList
            .SingleOrDefault(item => string.Equals(item.TransformScript.Id, script.Id, StringComparison.Ordinal))
            ?.TargetSqlIdentifier;
    }
}
