using MetaTransform.Binding;
using MetaTransformScript;
using MetaTransformScript.Sql;

[Trait("Category", "Integration")]
public sealed class TransformBindingTpcDsValidatedCorpusTests
{
    public static IEnumerable<object[]> Queries()
    {
        var demo = Path.Combine(RepositoryRoot(), "Demos", "MetaTransformScriptTpcDsCliIntegration");
        var sourceRoot = Path.Combine(demo, "SourceViews");
        var paths = Directory.GetFiles(sourceRoot, "view.sql", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(99, paths.Length);
        var manifest = File.ReadAllLines(Path.Combine(demo, "SourceViews.manifest.tsv"));
        Assert.Equal("Path\tTarget", manifest[0]);
        var entries = manifest.Skip(1).Select(line => line.Split('\t')).ToArray();
        Assert.All(entries, entry => Assert.Equal(2, entry.Length));
        var targets = entries.ToDictionary(
            entry => Path.GetFullPath(Path.Combine(demo, entry[0].Replace('\\', Path.DirectorySeparatorChar))),
            entry => entry[1], StringComparer.Ordinal);
        Assert.Equal(paths, targets.Keys.OrderBy(path => path, StringComparer.Ordinal));
        foreach (var path in paths)
        {
            yield return new object[] { Path.GetFileName(Path.GetDirectoryName(path))!, targets[path] };
        }
    }

    [Theory]
    [MemberData(nameof(Queries))]
    public void Query_BindsAndValidatesAgainstSavedSchema(string queryFolder, string targetSqlIdentifier)
    {
        var repository = RepositoryRoot();
        var demo = Path.Combine(repository, "Demos", "MetaTransformScriptTpcDsCliIntegration");
        var sourcePath = Path.Combine(demo, "SourceViews", queryFolder, "view.sql");
        var schemaPath = Path.Combine(demo, "SchemaWS");
        var catalogPath = Path.Combine(repository, "MetaDataType", "Workspace");
        var root = Path.Combine(Path.GetTempPath(), "MetaTransform.TpcDs.Binding", Guid.NewGuid().ToString("N"));
        try
        {
            var scriptModel = MetaTransformScriptModel.CreateEmpty();
            new MetaTransformScriptSqlService().ImportSqlFile(scriptModel, sourcePath, targetSqlIdentifier);
            Assert.Single(scriptModel.TransformScriptList);
            var transformPath = Path.Combine(root, "TransformWS");
            MetaTransformScriptTestHelper.SaveXml(scriptModel, transformPath);

            var result = new TransformBindingWorkspaceService().BindValidatedToXmlWorkspace(
                transformPath,
                new[] { schemaPath },
                schemaPath,
                executeSystemName: "TpcDsSnapshot",
                executeSystemDefaultSchemaName: "dbo",
                newWorkspacePath: Path.Combine(root, "BindingWS"),
                allowPartial: false,
                dataTypeWorkspacePath: catalogPath);

            Assert.Equal(1, result.TransformBindingCount);
            var validatedColumns = result.Model.ValidationTargetColumnLinkList.Count;
            var exactAssessments = result.Model.ValidationTargetColumnTypeExactList.Count;
            Assert.True(validatedColumns > 0 && exactAssessments == validatedColumns,
                $"{queryFolder}: MissingOutputTypeAssessment: expected exact type evidence for every output column; " +
                $"found {validatedColumns} validated column links and {exactAssessments} exact assessments.");
        }
        catch (TransformBindingValidationException exception)
        {
            Assert.Fail($"{queryFolder}: {exception.Code}: {exception.Message}");
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }
    }

    private static string RepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "Demos", "MetaTransformScriptTpcDsCliIntegration")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not locate the repository containing the TPC-DS corpus.");
    }
}
