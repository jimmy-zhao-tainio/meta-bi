using MetaTransform.Binding;
using MetaTransformScript.Sql.Parsing;

public sealed class TransformBindingTpcDsCorpusTests
{
    [Fact]
    public void TpcDsBinding_IsBoundOrExpectedGap()
    {
        var sourceViewsRoot = GetTpcDsSourceViewsRoot();
        var sqlFiles = Directory.GetFiles(sourceViewsRoot, "view.sql", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        Assert.Equal(99, sqlFiles.Length);

        var expectedGapCodes = new HashSet<string>(StringComparer.Ordinal)
        {
            "SelectStarRequiresValidationSchema",
            "GroupedSelectStarNotSupported",
            "FunctionTableReferenceColumnAliasesRequired",
            "GlobalFunctionTableReferenceOutputShapeNotSupported",
            "UnsupportedTableReferenceShape",
            "UnsupportedScalarExpressionShape",
            "UnsupportedBooleanExpressionShape",
            "UnsupportedGroupingExpressionShape",
            "UnsupportedGroupingSpecificationShape",
            "UngroupedColumnReference",
            "ColumnReferenceRequiresValidationSchema",
            "ColumnReferenceNotFound",
            "ColumnReferenceAmbiguous",
            "ColumnQualifierNotFound",
            "SelectStarQualifierNotFound",
            "SetOperationColumnCountMismatch",
            "SetOperationColumnNameMismatch",
            "SubqueryOutputColumnCountMismatch",
            "UnsupportedSelectOutputName"
        };

        var unknownFailures = new List<string>();
        var boundCount = 0;
        var expectedGapCount = 0;

        foreach (var sqlPath in sqlFiles)
        {
            var scenario = Path.GetFileName(Path.GetDirectoryName(sqlPath)) ?? sqlPath;
            MetaTransformScript.MetaTransformScriptModel model;
            try
            {
                model = new MetaTransformScriptSqlParser().ParseSqlCode(File.ReadAllText(sqlPath));
            }
            catch (Exception ex)
            {
                unknownFailures.Add($"{scenario}: parser failed with {ex.GetType().Name}: {ex.Message}");
                continue;
            }

            if (model.TransformScriptList.Count != 1)
            {
                unknownFailures.Add($"{scenario}: parser produced {model.TransformScriptList.Count} transform scripts.");
                continue;
            }

            var result = new TransformBindingService().BindSingleTransform(model);
            if (!result.HasErrors)
            {
                boundCount++;
                continue;
            }

            var issueCodes = result.Issues
                .Select(item => item.Code)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(static item => item, StringComparer.Ordinal)
                .ToArray();

            var unexpected = issueCodes
                .Where(code => !expectedGapCodes.Contains(code))
                .ToArray();

            if (unexpected.Length == 0)
            {
                expectedGapCount++;
                continue;
            }

            unknownFailures.Add(
                $"{scenario}: unexpected issue codes [{string.Join(", ", unexpected)}], all codes [{string.Join(", ", issueCodes)}]");
        }

        Assert.True(boundCount > 0, "Expected at least one bound TPC-DS script.");
        Assert.True(expectedGapCount > 0, "Expected at least one expected-gap TPC-DS script.");
        Assert.True(
            unknownFailures.Count == 0,
            "TPC-DS binding classification has unknown failures:\n" + string.Join("\n", unknownFailures));
    }

    private static string GetTpcDsSourceViewsRoot()
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
            "SourceViews");
    }
}
