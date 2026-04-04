using System.Collections;
using MetaTransformScript;
using MetaTransformScript.Instance;
using MetaTransformScript.Sql;
using MetaTransformScript.Sql.Parsing;

public sealed class SqlServiceOwnedImportTests
{
    [Theory]
    [InlineData("001_basic_select.sql")]
    [InlineData("017_cte.sql")]
    [InlineData("020_xml_namespaces_and_methods.sql")]
    [InlineData("044_window_frame_offsets.sql")]
    [InlineData("045_nested_subqueries.sql")]
    public void ImportFromSqlCode_MatchesDirectParser_OnSupportedInputs(string fileName)
    {
        var sql = LoadCorpus(fileName);

        var serviceModel = new MetaTransformScriptSqlService().ImportFromSqlCode(sql);
        var parserModel = new MetaTransformScriptSqlParser().ParseSqlCode(sql);

        AssertModelListCountsEqual(parserModel, serviceModel);

        serviceModel = RoundTripWorkspace(serviceModel, "service");
        parserModel = RoundTripWorkspace(parserModel, "parser");

        var service = new MetaTransformScriptSqlService();
        Assert.Equal(service.ExportToSqlCode(parserModel), service.ExportToSqlCode(serviceModel));
        Assert.Equal(parserModel.TransformScriptList.Single().Name, serviceModel.TransformScriptList.Single().Name);
    }

    [Fact]
    public void ImportFromSqlCode_MapsUnsupportedSyntax_ToUnsupportedSql()
    {
        var sql = LoadCorpus("040_view_column_list.sql");

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode(sql));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
    }

    [Fact]
    public void ImportFromSqlCode_MapsParseErrors_ToParseFailed()
    {
        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlCode("SELECT * FROM"));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.ParseFailed, exception.Kind);
    }

    [Theory]
    [InlineData("001_basic_select.sql")]
    [InlineData("017_cte.sql")]
    [InlineData("020_xml_namespaces_and_methods.sql")]
    [InlineData("044_window_frame_offsets.sql")]
    [InlineData("045_nested_subqueries.sql")]
    public void ImportFromSqlPath_UsesOwnedParser_OnSingleFileNoGoSupportedInputs(string fileName)
    {
        var sql = LoadCorpus(fileName);
        var tempFilePath = WriteTempSqlFile(fileName, sql);

        var serviceModel = new MetaTransformScriptSqlService().ImportFromSqlPath(tempFilePath);
        var parserModel = new MetaTransformScriptSqlParser().ParseSqlCode(
            sql,
            Path.GetFileName(tempFilePath),
            Path.GetFileNameWithoutExtension(tempFilePath));

        AssertModelListCountsEqual(parserModel, serviceModel);

        serviceModel = RoundTripWorkspace(serviceModel, "service-path");
        parserModel = RoundTripWorkspace(parserModel, "parser-path");

        var service = new MetaTransformScriptSqlService();
        Assert.Equal(service.ExportToSqlCode(parserModel), service.ExportToSqlCode(serviceModel));
        Assert.Equal(parserModel.TransformScriptList.Single().Name, serviceModel.TransformScriptList.Single().Name);
    }

    [Fact]
    public void ImportFromSqlPath_UsesLegacyPath_ForWrapperHeavySingleFiles()
    {
        var sql = LoadCorpus("040_view_column_list.sql");

        var model = new MetaTransformScriptSqlService().ImportFromSqlPath(
            WriteTempSqlFile("wrapper-heavy.sql", sql));

        var script = Assert.Single(model.TransformScriptList);
        Assert.Equal("dbo.v_view_column_list", script.Name);
        Assert.Equal(2, model.TransformScriptViewColumnsItemList.Count);
    }

    [Fact]
    public void ImportFromSqlPath_FailsExplicitly_ForUnsupportedSingleFileMainlineSql()
    {
        const string sql = """
SELECT
    c.CustomerId
FROM sales.Customer AS c
GROUP BY ALL c.CustomerId
""";

        var exception = Assert.Throws<MetaTransformScriptSqlImportException>(
            () => new MetaTransformScriptSqlService().ImportFromSqlPath(
                WriteTempSqlFile("group-by-all.sql", sql)));

        Assert.Equal(MetaTransformScriptSqlImportFailureKind.UnsupportedSql, exception.Kind);
    }

    private static string LoadCorpus(string fileName)
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "Reference",
            "Corpus",
            fileName));
        return File.ReadAllText(path);
    }

    private static void AssertModelListCountsEqual(
        MetaTransformScriptModel expected,
        MetaTransformScriptModel actual)
    {
        var listProperties = typeof(MetaTransformScriptModel)
            .GetProperties()
            .Where(static property => typeof(ICollection).IsAssignableFrom(property.PropertyType))
            .OrderBy(static property => property.Name, StringComparer.Ordinal);

        foreach (var property in listProperties)
        {
            var expectedCount = ((ICollection?)property.GetValue(expected))?.Count ?? 0;
            var actualCount = ((ICollection?)property.GetValue(actual))?.Count ?? 0;
            Assert.True(
                expectedCount == actualCount,
                $"{property.Name}: expected {expectedCount}, actual {actualCount}");
        }
    }

    private static MetaTransformScriptModel RoundTripWorkspace(MetaTransformScriptModel model, string label)
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "meta-bi", "metatransformscript-tests", Guid.NewGuid().ToString("N"), label);
        Directory.CreateDirectory(workspacePath);
        MetaTransformScriptInstance.SaveToWorkspace(model, workspacePath);
        return MetaTransformScriptInstance.LoadFromWorkspace(workspacePath, searchUpward: false);
    }

    private static string WriteTempSqlFile(string fileName, string sql)
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "meta-bi", "metatransformscript-tests", Guid.NewGuid().ToString("N"), "sql-path");
        Directory.CreateDirectory(directoryPath);
        var filePath = Path.Combine(directoryPath, fileName);
        File.WriteAllText(filePath, sql);
        return filePath;
    }
}
