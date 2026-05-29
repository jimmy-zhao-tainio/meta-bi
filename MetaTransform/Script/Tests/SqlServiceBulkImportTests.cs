using MetaTransformScript;
using MetaTransformScript.Instance;
using MetaTransformScript.Sql;

public sealed class SqlServiceBulkImportTests
{
    [Fact]
    public async Task ImportSqlFilesToNewWorkspace_ContinuesAfterFailures_AndSavesSuccessfulImports()
    {
        var root = CreateTempDirectory();
        var sourceDirectory = Path.Combine(root, "sql");
        var workspacePath = Path.Combine(root, "TransformWS");
        Directory.CreateDirectory(sourceDirectory);

        var firstPath = WriteSql(
            sourceDirectory,
            "dbo.First.sql",
            """
CREATE VIEW dbo.First AS
SELECT 1 AS A
GO
""");
        var badPath = WriteSql(
            sourceDirectory,
            "dbo.Bad.sql",
            """
CREATE VIEW dbo.Bad AS
SELECT FROM dbo.Source
GO
""");
        var secondPath = WriteSql(
            sourceDirectory,
            "dbo.Second.sql",
            """
CREATE VIEW dbo.Second AS
SELECT 2 AS B
GO
""");

        var progress = new List<SqlFileImportProgress>();
        var result = await new MetaTransformScriptSqlService().ImportSqlFilesToNewWorkspaceAsync(
            new[]
            {
                new SqlFileImportRequest(firstPath, "dbo.TargetFirst"),
                new SqlFileImportRequest(badPath, "dbo.TargetBad"),
                new SqlFileImportRequest(secondPath, "dbo.TargetSecond")
            },
            workspacePath,
            progress.Add);

        Assert.Equal(2, result.Successes.Count);
        var failure = Assert.Single(result.Failures);
        Assert.Equal(MetaTransformScriptSqlImportFailureKind.ParseFailed, failure.Kind);
        Assert.Equal(2, failure.Line);
        Assert.Equal(16, failure.Column);
        Assert.Equal(3, progress.Count);
        Assert.False(progress[1].Success);
        Assert.Equal(2, progress[1].Line);
        Assert.Equal(16, progress[1].Column);

        var loaded = MetaTransformScriptInstance.LoadFromWorkspace(workspacePath, searchUpward: false);
        Assert.Equal(2, loaded.TransformScriptList.Count);
        Assert.Contains(loaded.TransformScriptList, script => string.Equals(script.Name, "dbo.First", StringComparison.Ordinal));
        Assert.Contains(loaded.TransformScriptList, script => string.Equals(script.Name, "dbo.Second", StringComparison.Ordinal));
        Assert.DoesNotContain(loaded.TransformScriptList, script => string.Equals(script.Name, "dbo.Bad", StringComparison.Ordinal));
        Assert.Contains("dbo.TargetFirst", GetViewTargets(loaded));
        Assert.Contains("dbo.TargetSecond", GetViewTargets(loaded));
    }

    [Fact]
    public async Task ImportSqlFilesToNewWorkspace_ImportsViewsAndInlineTvfs_InOneSave()
    {
        var root = CreateTempDirectory();
        var sourceDirectory = Path.Combine(root, "sql");
        var workspacePath = Path.Combine(root, "TransformWS");
        Directory.CreateDirectory(sourceDirectory);

        var viewPath = WriteSql(
            sourceDirectory,
            "dbo.CustomerView.sql",
            """
CREATE VIEW dbo.CustomerView AS
SELECT CustomerId
FROM dbo.Customer
GO
""");
        var helperViewPath = WriteSql(
            sourceDirectory,
            "dbo.HelperView.sql",
            """
CREATE VIEW dbo.HelperView AS
SELECT CustomerId
FROM dbo.Customer
GO
""");
        var tvfPath = WriteSql(
            sourceDirectory,
            "dbo.fnCustomer.sql",
            """
CREATE FUNCTION dbo.fnCustomer(@customerId int)
RETURNS TABLE
AS
RETURN
(
    SELECT @customerId AS CustomerId
)
GO
""");

        var result = await new MetaTransformScriptSqlService().ImportSqlFilesToNewWorkspaceAsync(
            new[]
            {
                new SqlFileImportRequest(viewPath, "dbo.TargetCustomer"),
                new SqlFileImportRequest(helperViewPath, null),
                new SqlFileImportRequest(tvfPath, null)
            },
            workspacePath);

        Assert.Empty(result.Failures);
        Assert.Equal(3, result.Successes.Count);

        var loaded = MetaTransformScriptInstance.LoadFromWorkspace(workspacePath, searchUpward: false);
        Assert.Equal(3, loaded.TransformScriptList.Count);
        Assert.Single(loaded.ScriptObjectViewList);
        Assert.Single(loaded.ScriptObjectTVFList);
        Assert.Equal("dbo.TargetCustomer", loaded.ScriptObjectViewList[0].TargetSqlIdentifier);
        Assert.Contains(loaded.TransformScriptList, script => string.Equals(script.Name, "dbo.HelperView", StringComparison.Ordinal));
    }

    [Fact]
    public async Task ImportSqlFilesToNewWorkspace_DoesNotCreateWorkspace_WhenEveryImportFails()
    {
        var root = CreateTempDirectory();
        var sourceDirectory = Path.Combine(root, "sql");
        var workspacePath = Path.Combine(root, "TransformWS");
        Directory.CreateDirectory(sourceDirectory);

        var badPath = WriteSql(
            sourceDirectory,
            "dbo.Bad.sql",
            """
CREATE VIEW dbo.Bad AS
SELECT FROM dbo.Source
GO
""");

        var result = await new MetaTransformScriptSqlService().ImportSqlFilesToNewWorkspaceAsync(
            new[]
            {
                new SqlFileImportRequest(badPath, "dbo.TargetBad")
            },
            workspacePath);

        Assert.Empty(result.Successes);
        Assert.Single(result.Failures);
        Assert.False(Directory.Exists(workspacePath));
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            "meta-bi",
            "metatransformscript-tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static string WriteSql(string directory, string fileName, string sql)
    {
        var path = Path.Combine(directory, fileName);
        File.WriteAllText(path, sql);
        return path;
    }

    private static IReadOnlyList<string> GetViewTargets(MetaTransformScriptModel model) =>
        model.ScriptObjectViewList
            .Select(static view => view.TargetSqlIdentifier)
            .ToArray();
}
