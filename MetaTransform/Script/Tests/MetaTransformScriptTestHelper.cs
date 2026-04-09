using System.Collections;
using MetaTransformScript;
using MetaTransformScript.Instance;
using MetaTransformScript.Sql;
using MetaTransformScript.Sql.Parsing;

internal static class MetaTransformScriptTestHelper
{
    public static void AssertParserRoundTripsCorpusFile(string fileName, string bareSelectName = "dbo.v_test")
    {
        var sql = LoadCorpus(fileName);
        var parser = new MetaTransformScriptSqlParser();
        var firstModel = parser.ParseSqlCode(sql, bareSelectName: bareSelectName);
        firstModel = RoundTripWorkspace(firstModel, "first");

        var service = new MetaTransformScriptSqlService();
        var firstEmission = service.ExportToSqlCode(firstModel);
        var secondModel = parser.ParseSqlCode(firstEmission, bareSelectName: bareSelectName);
        secondModel = RoundTripWorkspace(secondModel, "second");
        var secondEmission = service.ExportToSqlCode(secondModel);

        Assert.Equal(firstEmission, secondEmission);
    }

    public static string LoadCorpus(string fileName)
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

    public static void AssertModelListCountsEqual(
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

    public static MetaTransformScriptModel RoundTripWorkspace(MetaTransformScriptModel model, string label)
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "meta-bi", "metatransformscript-tests", Guid.NewGuid().ToString("N"), label);
        Directory.CreateDirectory(workspacePath);
        MetaTransformScriptInstance.SaveToWorkspace(model, workspacePath);
        return MetaTransformScriptInstance.LoadFromWorkspace(workspacePath, searchUpward: false);
    }

    public static string WriteTempSqlFile(string fileName, string sql)
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "meta-bi", "metatransformscript-tests", Guid.NewGuid().ToString("N"), "sql-path");
        Directory.CreateDirectory(directoryPath);
        var filePath = Path.Combine(directoryPath, fileName);
        File.WriteAllText(filePath, sql);
        return filePath;
    }

    public static string WriteTempSqlFolder(params (string FileName, string Sql)[] files)
    {
        var directoryPath = Path.Combine(Path.GetTempPath(), "meta-bi", "metatransformscript-tests", Guid.NewGuid().ToString("N"), "sql-folder");
        Directory.CreateDirectory(directoryPath);

        foreach (var (fileName, sql) in files)
        {
            File.WriteAllText(Path.Combine(directoryPath, fileName), sql);
        }

        return directoryPath;
    }
}
