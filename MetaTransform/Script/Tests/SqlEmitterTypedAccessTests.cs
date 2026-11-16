using MetaTransformScript.Sql;
using MetaTransformScript.Sql.Parsing;

public sealed class SqlEmitterTypedAccessTests
{
    [Fact]
    public void QualifiedFunctionOwnersAndOrderedChildrenStaySeparateAcrossCalls()
    {
        var parser = new MetaTransformScriptSqlParser();
        var service = new MetaTransformScriptSqlService();
        var model = parser.ParseSqlCode(
            "SELECT dbo.MakeValue('left', 'right') AS FirstValue, dbo.MakeValue('up', 'down') AS SecondValue FROM dbo.Input AS src;",
            bareSelectName: "dbo.v_typed_owners");
        var expected = service.ExportToSqlCode(model);

        // Collections are unordered storage; modeled ordinals and owners determine each output sequence.
        model.FunctionCallParametersItemList.Reverse();
        model.QuerySpecificationSelectElementsItemList.Reverse();
        model.MultiPartIdentifierIdentifiersItemList.Reverse();
        var emitted = service.ExportToSqlCode(model);

        Assert.Equal(expected, emitted);
        Assert.Contains("dbo.MakeValue('left', 'right') AS FirstValue", emitted, StringComparison.Ordinal);
        Assert.Contains("dbo.MakeValue('up', 'down') AS SecondValue", emitted, StringComparison.Ordinal);
        Assert.Equal(emitted, service.ExportToSqlCode(parser.ParseSqlCode(emitted, bareSelectName: "dbo.v_typed_owners")));
    }

    [Fact]
    public void TableSampleUsesItsAuthoredSystemFlagAndPreservesOptionalLinks()
    {
        var parser = new MetaTransformScriptSqlParser();
        var service = new MetaTransformScriptSqlService();
        var model = parser.ParseSqlCode(
            "SELECT src.Value FROM dbo.Input AS src TABLESAMPLE (10 PERCENT) REPEATABLE (17);",
            bareSelectName: "dbo.v_typed_sample");
        var sample = Assert.Single(model.TableSampleClauseList);
        sample.System = "true";

        var emitted = service.ExportToSqlCode(model);

        Assert.Contains("TABLESAMPLE SYSTEM (10 PERCENT) REPEATABLE (17)", emitted, StringComparison.Ordinal);
        sample.System = null;
        model.TableSampleClauseRepeatSeedLinkList.Clear();
        var withoutOptions = service.ExportToSqlCode(model);
        Assert.Contains("TABLESAMPLE (10 PERCENT)", withoutOptions, StringComparison.Ordinal);
        Assert.DoesNotContain("SYSTEM", withoutOptions, StringComparison.Ordinal);
        Assert.DoesNotContain("REPEATABLE", withoutOptions, StringComparison.Ordinal);
    }

    [Fact]
    public void RequiredOwnerLinkStillReportsItsSpecificLookupFailure()
    {
        var model = new MetaTransformScriptSqlParser().ParseSqlCode(
            "SELECT CAST(src.Value AS int) AS Value FROM dbo.Input AS src;",
            bareSelectName: "dbo.v_missing_parameter");
        var cast = Assert.Single(model.CastCallList);
        model.CastCallParameterLinkList.Clear();

        var error = Assert.Throws<InvalidOperationException>(() => new MetaTransformScriptSqlService().ExportToSqlCode(model));

        Assert.Equal($"Required MetaTransformScript link 'CastCall.Parameter' with owner id '{cast.Id}' was not found.", error.Message);
    }

    [Fact]
    public void UnsupportedBaseExpressionStillReportsTheAuthoredExpressionId()
    {
        var model = new MetaTransformScriptSqlParser().ParseSqlCode("SELECT 1 AS Value;", bareSelectName: "dbo.v_missing_base");
        var expression = Assert.Single(model.ScalarExpressionList);
        model.PrimaryExpressionList.Clear();

        var error = Assert.Throws<InvalidOperationException>(() => new MetaTransformScriptSqlService().ExportToSqlCode(model));

        Assert.Equal($"Unsupported MetaTransformScript ScalarExpression id '{expression.Id}'.", error.Message);
    }
}
