using MetaConvert.SqlToTransformScript;
using MetaConvert.TransformScriptToSql;
using MetaSql;

public sealed class SqlToTransformScriptConverterTests
{
    [Fact]
    public async Task ConvertAsync_RoundTripsViewAndFunctionModules_FromMetaSql()
    {
        var root = CreateTempRoot();
        var sourceMetaSqlWorkspacePath = Path.Combine(root, "SourceMetaSqlWS");
        var transformScriptWorkspacePath = Path.Combine(root, "TransformScriptWS");
        var roundTripMetaSqlWorkspacePath = Path.Combine(root, "RoundTripMetaSqlWS");

        try
        {
            SaveMetaSqlWithViewAndFunction(sourceMetaSqlWorkspacePath);

            var result = await SqlToTransformScriptConverter.ConvertAsync(
                sourceMetaSqlWorkspacePath,
                transformScriptWorkspacePath);

            Assert.Equal(1, result.ViewCount);
            Assert.Equal(1, result.FunctionCount);
            Assert.Equal(0, result.StoredProcedureCount);

            await TransformScriptToSqlConverter.ConvertAsync(
                transformScriptWorkspacePath,
                roundTripMetaSqlWorkspacePath,
                "SymmetryDb",
                "xml");

            var roundTrip = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaSqlModel>(roundTripMetaSqlWorkspacePath, searchUpward: false);
            var view = Assert.Single(roundTrip.ViewList);
            Assert.Equal("dq", view.Schema.Name);
            Assert.Equal("vCustomerScore", view.Name);
            Assert.StartsWith("CREATE VIEW dq.vCustomerScore", view.DefinitionSql, StringComparison.OrdinalIgnoreCase);

            var function = Assert.Single(roundTrip.FunctionList);
            Assert.Equal("dq", function.Schema.Name);
            Assert.Equal("fnCustomerScore", function.Name);
            Assert.Equal("InlineTableValuedFunction", function.FunctionKind);
            Assert.StartsWith("CREATE FUNCTION dq.fnCustomerScore", function.DefinitionSql, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_RoundTripsStoredProcedures_FromMetaSql()
    {
        var root = CreateTempRoot();
        var sourceMetaSqlWorkspacePath = Path.Combine(root, "SourceMetaSqlWS");
        var transformScriptWorkspacePath = Path.Combine(root, "TransformScriptWS");
        var roundTripMetaSqlWorkspacePath = Path.Combine(root, "RoundTripMetaSqlWS");

        try
        {
            SaveMetaSqlWithStoredProcedure(sourceMetaSqlWorkspacePath);

            var result = await SqlToTransformScriptConverter.ConvertAsync(
                sourceMetaSqlWorkspacePath,
                transformScriptWorkspacePath);

            Assert.Equal(0, result.ViewCount);
            Assert.Equal(0, result.FunctionCount);
            Assert.Equal(1, result.StoredProcedureCount);

            var transformModel = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaTransformScript.MetaTransformScriptModel>(
                transformScriptWorkspacePath,
                searchUpward: false);
            var procedureScript = Assert.Single(transformModel.TransformScriptList);
            Assert.Equal("dq.RunReview", procedureScript.Name);
            var procedureObject = Assert.Single(transformModel.ScriptObjectStoredProcedureList);
            Assert.Equal(procedureScript.Id, procedureObject.TransformScript.Id);
            Assert.Contains("CREATE PROCEDURE dq.RunReview", procedureObject.DefinitionSql, StringComparison.OrdinalIgnoreCase);

            await TransformScriptToSqlConverter.ConvertAsync(
                transformScriptWorkspacePath,
                roundTripMetaSqlWorkspacePath,
                "SymmetryDb",
                "xml");

            var roundTrip = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaSqlModel>(roundTripMetaSqlWorkspacePath, searchUpward: false);
            var procedure = Assert.Single(roundTrip.StoredProcedureList);
            Assert.Equal("dq", procedure.Schema.Name);
            Assert.Equal("RunReview", procedure.Name);
            Assert.StartsWith("CREATE PROCEDURE dq.RunReview", procedure.DefinitionSql, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_ConvertsSelectedViewsAndFunctions_WhenMetaSqlAlsoHasStoredProcedures()
    {
        var root = CreateTempRoot();
        var sourceMetaSqlWorkspacePath = Path.Combine(root, "SourceMetaSqlWS");
        var transformScriptWorkspacePath = Path.Combine(root, "TransformScriptWS");

        try
        {
            SaveMetaSqlWithViewFunctionAndStoredProcedure(sourceMetaSqlWorkspacePath);

            var result = await SqlToTransformScriptConverter.ConvertAsync(
                sourceMetaSqlWorkspacePath,
                transformScriptWorkspacePath,
                new SqlToTransformScriptConversionOptions
                {
                    ModuleKinds = SqlToTransformScriptModuleKinds.Views | SqlToTransformScriptModuleKinds.Functions,
                });

            Assert.Equal(1, result.ViewCount);
            Assert.Equal(1, result.FunctionCount);
            Assert.Equal(0, result.StoredProcedureCount);
            Assert.Equal(2, result.Workspace.Instance.GetOrCreateEntityRecords("TransformScript").Count);
        }
        finally
        {
            DeleteTempRoot(root);
        }
    }

    [Fact]
    public async Task ConvertAsync_CreatesEmptyWorkspace_WhenSelectedKindsAreEmptyAndAllowed()
    {
        var root = CreateTempRoot();
        var sourceMetaSqlWorkspacePath = Path.Combine(root, "SourceMetaSqlWS");
        var transformScriptWorkspacePath = Path.Combine(root, "TransformScriptWS");

        try
        {
            SaveMetaSqlWithStoredProcedure(sourceMetaSqlWorkspacePath);

            var result = await SqlToTransformScriptConverter.ConvertAsync(
                sourceMetaSqlWorkspacePath,
                transformScriptWorkspacePath,
                new SqlToTransformScriptConversionOptions
                {
                    ModuleKinds = SqlToTransformScriptModuleKinds.Views | SqlToTransformScriptModuleKinds.Functions,
                    AllowEmpty = true,
                });

            Assert.Equal(0, result.ViewCount);
            Assert.Equal(0, result.FunctionCount);
            Assert.Equal(0, result.StoredProcedureCount);
            Assert.Empty(result.Workspace.Instance.GetOrCreateEntityRecords("TransformScript"));
            Assert.True(Directory.Exists(transformScriptWorkspacePath));
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

    private static void SaveMetaSqlWithViewAndFunction(string workspacePath)
    {
        var model = CreateBaseMetaSql(out var schema);
        model.FunctionList.Add(new Function
        {
            Id = "SymmetryDb.dq.fnCustomerScore",
            Schema = schema,
            Name = "fnCustomerScore",
            FunctionKind = "InlineTableValuedFunction",
            DeployOrdinal = "1",
            DefinitionSql = """
CREATE FUNCTION dq.fnCustomerScore
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
""",
        });
        model.ViewList.Add(new View
        {
            Id = "SymmetryDb.dq.vCustomerScore",
            Schema = schema,
            Name = "vCustomerScore",
            DeployOrdinal = "2",
            DefinitionSql = """
CREATE VIEW dq.vCustomerScore
AS
SELECT
    CustomerId
FROM dq.fnCustomerScore(1)
""",
        });

        Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(model, workspacePath);
    }

    private static void SaveMetaSqlWithViewFunctionAndStoredProcedure(string workspacePath)
    {
        SaveMetaSqlWithViewAndFunction(workspacePath);
        var model = Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Load<MetaSqlModel>(workspacePath, searchUpward: false);
        var schema = model.SchemaList.Single(row => string.Equals(row.Name, "dq", StringComparison.Ordinal));
        model.StoredProcedureList.Add(new StoredProcedure
        {
            Id = "SymmetryDb.dq.RunReview",
            Schema = schema,
            Name = "RunReview",
            DeployOrdinal = "3",
            DefinitionSql = """
CREATE PROCEDURE dq.RunReview
AS
BEGIN
    SELECT 1 AS ReviewRunId;
END
""",
        });

        Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(model, workspacePath);
    }

    private static void SaveMetaSqlWithStoredProcedure(string workspacePath)
    {
        var model = CreateBaseMetaSql(out var schema);
        model.StoredProcedureList.Add(new StoredProcedure
        {
            Id = "SymmetryDb.dq.RunReview",
            Schema = schema,
            Name = "RunReview",
            DeployOrdinal = "1",
            DefinitionSql = """
CREATE PROCEDURE dq.RunReview
AS
BEGIN
    SELECT 1 AS ReviewRunId;
END
""",
        });

        Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(model, workspacePath);
    }

    private static MetaSqlModel CreateBaseMetaSql(out Schema schema)
    {
        var model = MetaSqlModel.CreateEmpty();
        var database = new Database
        {
            Id = "SymmetryDb",
            Name = "SymmetryDb",
        };
        schema = new Schema
        {
            Id = "SymmetryDb.dq",
            Database = database,
            Name = "dq",
        };

        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        return model;
    }
}
