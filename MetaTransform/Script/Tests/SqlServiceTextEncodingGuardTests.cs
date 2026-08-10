using System.Text;
using MetaTransformScript.Instance;
using MetaTransformScript.Sql;

public sealed class SqlServiceTextEncodingGuardTests
{
    [Fact]
    public void ImportFromSqlCode_RejectsLikelyMojibakeBeforeParsing()
    {
        var sql = "CREATE VIEW dbo.BadText AS" + Environment.NewLine +
            "SELECT N'M\u00c3\u00a5nadsl\u00c3\u00b6n' AS Name" + Environment.NewLine;

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch, exception.Kind);
        Assert.Equal(2, exception.Line);
        Assert.True(exception.Column > 0);
        Assert.Contains("mojibake", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Unicode", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportFromSqlCode_RejectsLikelyMojibakeInsideComments()
    {
        var sql = "CREATE VIEW dbo.BadComment AS" + Environment.NewLine +
            "SELECT 1 AS A" + Environment.NewLine +
            "-- exported comment with M\u00c3\u00a5nad" + Environment.NewLine;

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch, exception.Kind);
        Assert.Equal(3, exception.Line);
    }

    [Fact]
    public void ImportFromSqlCode_RejectsLikelyMojibakeInsideIdentifiers()
    {
        var sql = "CREATE VIEW dbo.BadIdentifier AS" + Environment.NewLine +
            "SELECT s.N\u00c3\u00a4raKod AS NaraKod" + Environment.NewLine +
            "FROM dbo.Source AS s" + Environment.NewLine;

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch, exception.Kind);
        Assert.Equal(2, exception.Line);
    }

    [Fact]
    public void ImportFromSqlCode_AllowsIntentionalUnicodeText()
    {
        const string value = "\u00c5\u00c4\u00d6 \u00e5\u00e4\u00f6";
        const string columnName = "\u00c5rsbelopp";
        var sql = $"SELECT N'{value}' AS [{columnName}]";

        var service = new MetaTransformScriptSqlService();
        var model = service.ImportFromSqlCode(sql, "dbo.v_unicode");
        var exported = service.ExportToSqlCode(model);

        Assert.Single(model.TransformScriptList);
        Assert.Contains(value, exported, StringComparison.Ordinal);
        Assert.Contains(columnName, exported, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ImportSqlFilesToNewWorkspace_ReportsLikelyMojibakeAndContinues()
    {
        var root = CreateTempDirectory();
        var sourceDirectory = Path.Combine(root, "sql");
        var workspacePath = Path.Combine(root, "TransformWS");
        Directory.CreateDirectory(sourceDirectory);

        var goodPath = WriteSql(
            sourceDirectory,
            "dbo.Good.sql",
            """
CREATE VIEW dbo.Good AS
SELECT 1 AS A
GO
""");
        var damagedPath = WriteSql(
            sourceDirectory,
            "dbo.Damaged.sql",
            "CREATE VIEW dbo.Damaged AS" + Environment.NewLine +
            "SELECT N'M\u00c3\u00a5nad' AS Name" + Environment.NewLine +
            "GO" + Environment.NewLine);

        var result = await new MetaTransformScriptSqlService().ImportSqlFilesToNewXmlWorkspaceAsync(
            new[]
            {
                new SqlFileImportRequest(goodPath, "dbo.TargetGood"),
                new SqlFileImportRequest(damagedPath, "dbo.TargetDamaged")
            },
            workspacePath);

        Assert.Single(result.Successes);
        var failure = Assert.Single(result.Failures);
        Assert.Equal(MetaTransformScriptSqlImportFailureKind.LikelyTextEncodingMismatch, failure.Kind);
        Assert.Equal(2, failure.Line);
        Assert.True(failure.Column > 0);

        var loaded = MetaTransformScriptInstance.LoadFromWorkspace(workspacePath, searchUpward: false);
        Assert.Single(loaded.TransformScriptList);
        Assert.Contains(loaded.TransformScriptList, script => string.Equals(script.Name, "dbo.Good", StringComparison.Ordinal));
        Assert.DoesNotContain(loaded.TransformScriptList, script => string.Equals(script.Name, "dbo.Damaged", StringComparison.Ordinal));
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
        File.WriteAllText(path, sql, Encoding.UTF8);
        return path;
    }
}
