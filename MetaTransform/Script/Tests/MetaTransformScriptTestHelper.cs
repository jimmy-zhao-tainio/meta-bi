using System.Collections;
using MetaConvert.TransformScriptToSql;
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
        var roundTripModuleName = GetPrimaryModuleName(service, firstModel, bareSelectName);
        var secondModel = parser.ParseSqlCode(firstEmission, bareSelectName: roundTripModuleName);
        secondModel = RoundTripWorkspace(secondModel, "second");
        var secondEmission = service.ExportToSqlCode(secondModel);

        Assert.Equal(firstEmission, secondEmission);
        AssertMetaSqlProjectionRoundTrips(firstModel);
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
        var directoryPath = Path.Combine(Path.GetTempPath(), "meta-bi", "metatransformscript-tests", Guid.NewGuid().ToString("N"), "sql-file");
        Directory.CreateDirectory(directoryPath);
        var filePath = Path.Combine(directoryPath, fileName);
        File.WriteAllText(filePath, sql);
        return filePath;
    }

    public static void AssertMetaSqlProjectionEqual(MetaTransformScriptModel expectedSource, MetaTransformScriptModel actualSource)
    {
        var service = new MetaTransformScriptSqlService();
        var expected = TransformScriptToSqlConverter.ConvertToMetaSql(
            service.ExportModuleDefinitions(expectedSource),
            "RoundTripDb");
        var actual = TransformScriptToSqlConverter.ConvertToMetaSql(
            service.ExportModuleDefinitions(actualSource),
            "RoundTripDb");

        Assert.Equal(expected.DatabaseList.Count, actual.DatabaseList.Count);
        Assert.Equal(
            expected.SchemaList.Select(static item => item.Name).OrderBy(static item => item, StringComparer.Ordinal),
            actual.SchemaList.Select(static item => item.Name).OrderBy(static item => item, StringComparer.Ordinal));
        Assert.Equal(
            expected.ViewList.Select(static item => $"{item.Schema.Name}.{item.Name}:{item.DefinitionSql}").OrderBy(static item => item, StringComparer.Ordinal),
            actual.ViewList.Select(static item => $"{item.Schema.Name}.{item.Name}:{item.DefinitionSql}").OrderBy(static item => item, StringComparer.Ordinal));
        Assert.Equal(
            expected.FunctionList.Select(static item => $"{item.Schema.Name}.{item.Name}:{item.FunctionKind}:{item.DefinitionSql}").OrderBy(static item => item, StringComparer.Ordinal),
            actual.FunctionList.Select(static item => $"{item.Schema.Name}.{item.Name}:{item.FunctionKind}:{item.DefinitionSql}").OrderBy(static item => item, StringComparer.Ordinal));
    }

    public static void AssertMetaSqlProjectionRoundTrips(MetaTransformScriptModel source)
    {
        var service = new MetaTransformScriptSqlService();
        var module = Assert.Single(service.ExportModuleDefinitions(source));
        var parsed = new MetaTransformScriptSqlParser().ParseSqlCode(module.DefinitionSql);
        parsed = RoundTripWorkspace(parsed, "meta-sql-projection");

        AssertMetaSqlProjectionEqual(source, parsed);
    }

    private static string GetPrimaryModuleName(
        MetaTransformScriptSqlService service,
        MetaTransformScriptModel model,
        string fallbackName)
    {
        var module = service
            .ExportModuleDefinitions(model)
            .OrderBy(static item => item.DeployOrdinal)
            .ThenBy(static item => item.SchemaName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static item => item.ObjectName, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        return module is null
            ? fallbackName
            : $"{module.SchemaName}.{module.ObjectName}";
    }
}
