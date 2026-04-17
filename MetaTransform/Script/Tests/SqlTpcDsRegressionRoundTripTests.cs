using MetaTransformScript.Sql;
using MetaTransformScript.Sql.Parsing;

public sealed class SqlTpcDsRegressionRoundTripTests
{
    [Theory]
    [InlineData("072_q72", "tpcds.v_q72")]
    [InlineData("075_q75", "tpcds.v_q75")]
    [InlineData("078_q78", "tpcds.v_q78")]
    public void ParserAndEmitter_RoundTripsKnownTpcDsGapFiles(string sourceViewFolder, string bareSelectName)
    {
        var sqlPath = GetTpcDsSqlPath(sourceViewFolder);
        var sql = File.ReadAllText(sqlPath);

        var parser = new MetaTransformScriptSqlParser();
        var firstModel = parser.ParseSqlCode(sql, bareSelectName: bareSelectName);
        firstModel = MetaTransformScriptTestHelper.RoundTripWorkspace(firstModel, "first");

        var service = new MetaTransformScriptSqlService();
        var firstEmission = service.ExportToSqlCode(firstModel);
        var secondModel = parser.ParseSqlCode(firstEmission, bareSelectName: bareSelectName);
        secondModel = MetaTransformScriptTestHelper.RoundTripWorkspace(secondModel, "second");
        var secondEmission = service.ExportToSqlCode(secondModel);

        Assert.Equal(firstEmission, secondEmission);
    }

    private static string GetTpcDsSqlPath(string sourceViewFolder)
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !Directory.Exists(Path.Combine(root.FullName, "Samples")))
        {
            root = root.Parent;
        }

        if (root is null)
        {
            throw new DirectoryNotFoundException("Could not locate repository root containing the Samples directory.");
        }

        return Path.Combine(
            root.FullName,
            "Samples",
            "Demos",
            "MetaTransformScriptTpcDsCliIntegration",
            "SourceViews",
            sourceViewFolder,
            "view.sql");
    }
}
