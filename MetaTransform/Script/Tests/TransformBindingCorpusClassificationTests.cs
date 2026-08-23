using MetaTransform.Binding;
using MetaTransformScript.Sql.Parsing;

public sealed class TransformBindingCorpusClassificationTests
{
    [Fact]
    public void CorpusBinding_IsBoundOrExpectedGap()
    {
        var corpusPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "Reference",
            "Corpus"));

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
            "ColumnQualifierNotFound",
            "SelectStarQualifierNotFound",
            "SetOperationColumnCountMismatch",
            "SetOperationColumnNameMismatch",
            "SubqueryOutputColumnCountMismatch"
        };
        var expectedParseGapFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "032_openrowset.sql",
            "034_changetable.sql",
            "035_odbc_surface.sql",
            "038_adhoc_datasource.sql",
            "039_changetable_version.sql"
        };

        var unknownFailures = new List<string>();
        var boundCount = 0;
        var expectedGapCount = 0;

        foreach (var filePath in Directory.GetFiles(corpusPath, "*.sql").OrderBy(static item => item, StringComparer.OrdinalIgnoreCase))
        {
            var fileName = Path.GetFileName(filePath);
            MetaTransformScript.MetaTransformScriptModel model;
            try
            {
                model = new MetaTransformScriptSqlParser().ParseSqlCode(File.ReadAllText(filePath));
            }
            catch (Exception ex)
            {
                if (expectedParseGapFiles.Contains(fileName))
                {
                    expectedGapCount++;
                    continue;
                }

                unknownFailures.Add($"{fileName}: parser failed with {ex.GetType().Name}: {ex.Message}");
                continue;
            }

            if (model.TransformScriptList.Count != 1)
            {
                unknownFailures.Add($"{fileName}: parser produced {model.TransformScriptList.Count} transform scripts.");
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
                $"{fileName}: unexpected issue codes [{string.Join(", ", unexpected)}], all codes [{string.Join(", ", issueCodes)}]");
        }

        Assert.True(boundCount > 0, "Expected at least one bound corpus file.");
        Assert.True(expectedGapCount > 0, "Expected at least one expected-gap corpus file.");
        Assert.True(
            unknownFailures.Count == 0,
            "Corpus binding classification has unknown failures:\n" + string.Join("\n", unknownFailures));
    }
}
