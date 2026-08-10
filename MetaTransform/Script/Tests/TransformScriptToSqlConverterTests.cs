using MetaConvert.TransformScriptToSql;
using Meta.Core.Serialization;
using MetaSql;
using MetaTransformScript;
using MetaTransformScript.Sql;

public sealed class TransformScriptToSqlConverterTests
{
    [Fact]
    public async Task ConvertAsync_UsesModeledSchema_ForTwoPartViewIdentifier()
    {
        const string sql = """
CREATE VIEW stage.vCustomer
AS
SELECT
    1 AS CustomerId
""";

        var root = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(root, "TransformScriptWS");
        var metaSqlWorkspacePath = Path.Combine(root, "MetaSqlWS");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                sql,
                targetSqlIdentifier: null,
                transformWorkspacePath);

            await TransformScriptToSqlConverter.ConvertAsync(
                transformWorkspacePath,
                metaSqlWorkspacePath,
                "Staging",
                "xml");

            var model = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaSqlModel>(metaSqlWorkspacePath, searchUpward: false);
            var schema = Assert.Single(model.SchemaList);
            Assert.Equal("stage", schema.Name);

            var view = Assert.Single(model.ViewList);
            Assert.Same(schema, view.Schema);
            Assert.Equal("vCustomer", view.Name);
            Assert.StartsWith("CREATE VIEW stage.vCustomer", view.DefinitionSql, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("GO", view.DefinitionSql, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_OutputBuildsDeployManifestAgainstEmptyLiveMetaSql()
    {
        const string functionSql = """
CREATE FUNCTION deploy.fnCustomerOrders
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
        const string viewSql = """
CREATE VIEW deploy.vCustomerOrders
AS
SELECT
    1 AS CustomerId
""";

        var root = CreateTempRoot();
        var transformWorkspacePath = Path.Combine(root, "TransformScriptWS");
        var sourceMetaSqlWorkspacePath = Path.Combine(root, "SourceMetaSqlWS");
        var liveMetaSqlWorkspacePath = Path.Combine(root, "LiveMetaSqlWS");

        try
        {
            var transformService = new MetaTransformScriptSqlService();
            await transformService.ImportFromSqlCodeToXmlWorkspaceAsync(
                functionSql,
                targetSqlIdentifier: null,
                transformWorkspacePath);
            await transformService.AddSqlCodeToWorkspaceAsync(
                viewSql,
                targetSqlIdentifier: null,
                transformWorkspacePath);

            await TransformScriptToSqlConverter.ConvertAsync(
                transformWorkspacePath,
                sourceMetaSqlWorkspacePath,
                "SymmetryDb",
                "xml");
            SaveEmptyMetaSqlWorkspace(liveMetaSqlWorkspacePath, "SymmetryDb");

            var sourceWorkspace = await XmlWorkspaceReader.OpenAsync(sourceMetaSqlWorkspacePath);
            var liveWorkspace = await XmlWorkspaceReader.OpenAsync(liveMetaSqlWorkspacePath);

            var differences = new MetaSqlDifferenceService().BuildDifferences(
                sourceWorkspace.State,
                liveWorkspace.State);
            Assert.Contains(differences, row =>
                row.ObjectKind == MetaSqlObjectKind.Function &&
                row.DifferenceKind == MetaSqlDifferenceKind.MissingInLive &&
                string.Equals(row.DisplayName, "deploy.fnCustomerOrders", StringComparison.Ordinal));
            Assert.Contains(differences, row =>
                row.ObjectKind == MetaSqlObjectKind.View &&
                row.DifferenceKind == MetaSqlDifferenceKind.MissingInLive &&
                string.Equals(row.DisplayName, "deploy.vCustomerOrders", StringComparison.Ordinal));

            var manifest = new MetaSqlDeployManifestService().BuildManifest(
                sourceWorkspace.State,
                liveWorkspace.State,
                MetaSqlLiveDatabasePresence.Present,
                differences,
                manifestName: "TransformScriptToSqlManifest",
                targetDescription: "transform-script-to-sql symmetry witness");

            Assert.True(manifest.IsDeployable);
            Assert.Equal(3, manifest.AddCount);
            Assert.Equal(0, manifest.DropCount);
            Assert.Equal(0, manifest.AlterCount);
            Assert.Equal(0, manifest.ReplaceCount);
            Assert.Equal(0, manifest.BlockCount);
            Assert.Single(manifest.ManifestModel.AddSchemaList);
            Assert.Single(manifest.ManifestModel.AddFunctionList);
            Assert.Single(manifest.ManifestModel.AddViewList);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public void ConvertToMetaSql_MapsInlineTableValuedFunction()
    {
        const string sql = """
CREATE FUNCTION dbo.fnCustomerOrders
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

        var modules = new MetaTransformScriptSqlService()
            .ExportModuleDefinitions(new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        var model = TransformScriptToSqlConverter.ConvertToMetaSql(modules, "Warehouse");

        var function = Assert.Single(model.FunctionList);
        Assert.Equal("fnCustomerOrders", function.Name);
        Assert.Equal("InlineTableValuedFunction", function.FunctionKind);
        Assert.Equal("dbo", function.Schema.Name);
        Assert.StartsWith("CREATE FUNCTION dbo.fnCustomerOrders", function.DefinitionSql, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("GO", function.DefinitionSql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExportModuleDefinitions_UsesCreateObjectName_NotViewTargetIdentifier()
    {
        const string sql = """
CREATE VIEW dbo.vCustomerLoad
AS
SELECT
    1 AS CustomerId
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql);
        SetViewTargetSqlIdentifier(model, "dbo.Customer");

        var module = Assert.Single(service.ExportModuleDefinitions(model));

        Assert.Equal("dbo", module.SchemaName);
        Assert.Equal("vCustomerLoad", module.ObjectName);
        Assert.StartsWith("CREATE VIEW dbo.vCustomerLoad", module.DefinitionSql, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("CREATE VIEW dbo.Customer", module.DefinitionSql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ExportModuleDefinitions_RejectsModuleNameWithoutSchema()
    {
        const string sql = """
SELECT
    1 AS CustomerId
""";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql, "vCustomer");

        var exception = Assert.Throws<InvalidOperationException>(() => service.ExportModuleDefinitions(model));
        Assert.Contains("requires schema.object module names", exception.Message, StringComparison.OrdinalIgnoreCase);
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

    private static void SaveEmptyMetaSqlWorkspace(string workspacePath, string databaseName)
    {
        var model = MetaSqlModel.CreateEmpty();
        model.DatabaseList.Add(new Database
        {
            Id = databaseName,
            Name = databaseName,
        });
        MetaTransformScriptTestHelper.SaveXml(model, workspacePath);
    }

    private static void SetViewTargetSqlIdentifier(MetaTransformScriptModel model, string targetSqlIdentifier)
    {
        var script = Assert.Single(model.TransformScriptList);
        var scriptObjectView = model.ScriptObjectViewList.SingleOrDefault(item => item.TransformScript.Id == script.Id);
        if (scriptObjectView is null)
        {
            scriptObjectView = new ScriptObjectView
            {
                Id = Guid.NewGuid().ToString("N"),
                TransformScript = script,
            };
            model.ScriptObjectViewList.Add(scriptObjectView);
        }

        scriptObjectView.TargetSqlIdentifier = targetSqlIdentifier;
    }
}
