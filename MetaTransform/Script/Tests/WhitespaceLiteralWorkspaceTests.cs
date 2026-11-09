using MetaTransformScript.Sql;

namespace MetaTransformScript.Tests;

public sealed class WhitespaceLiteralWorkspaceTests
{
    [Theory]
    [InlineData("CREATE VIEW dbo.WhitespaceProbe AS SELECT {0} AS [Value];")]
    [InlineData("CREATE FUNCTION dbo.WhitespaceProbe() RETURNS TABLE AS RETURN (SELECT {0} AS [Value]);")]
    [InlineData("CREATE FUNCTION dbo.WhitespaceProbe() RETURNS nvarchar(50) AS BEGIN RETURN {0}; END;")]
    public async Task XmlWorkspace_ModuleExportPreservesMultilineLiteral(string template)
    {
        const string value = "first\r\nsecond\nthird";
        var root = Path.Combine(Path.GetTempPath(), "MetaTransformScript.WhitespaceTests", Guid.NewGuid().ToString("N"));
        try
        {
            var service = new MetaTransformScriptSqlService();
            var sql = string.Format(System.Globalization.CultureInfo.InvariantCulture, template, "N'" + value + "'");
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(sql, null, root);
            var exported = Assert.Single(service.ExportModuleDefinitions(root)).DefinitionSql;
            var reimported = service.ImportFromSqlCode(exported);
            Assert.Equal(value, Assert.Single(reimported.LiteralList, literal => literal.LiteralType == "String").Value);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r")]
    [InlineData("\r\n")]
    [InlineData(" \t\r\n ")]
    [InlineData(" leading and trailing ")]
    [InlineData("first\r\nsecond")]
    [InlineData("Åsa äter räksmörgås.")]
    public async Task XmlWorkspace_ImportReloadAndExportPreservesLiteralValue(string value)
    {
        var root = Path.Combine(Path.GetTempPath(), "MetaTransformScript.WhitespaceTests", Guid.NewGuid().ToString("N"));
        try
        {
            var service = new MetaTransformScriptSqlService();
            var sql = "SELECT N'" + value.Replace("'", "''", StringComparison.Ordinal) + "' AS [Value];";
            var parsed = service.ImportFromSqlCode(sql, "WhitespaceProbe");
            Assert.Equal(value, Assert.Single(parsed.LiteralList).Value);
            var expectedSql = service.ExportToSqlCode(parsed);

            var workspace = Path.Combine(root, "workspace");
            await service.ImportFromSqlCodeToXmlWorkspaceAsync(sql, "dbo.WhitespaceProbe", workspace, "WhitespaceProbe");
            var exported = service.ExportToSqlCode(workspace);
            Assert.Equal(expectedSql, exported);
            Assert.Equal(value, Assert.Single(service.ImportFromSqlCode(exported, "ReloadedProbe").LiteralList).Value);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }
}
