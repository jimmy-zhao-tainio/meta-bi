using MetaAnalytics;
using MetaAnalytics.Instance;
using MetaConvert.AnalyticsToTabular;
using Meta.TypedModels;

namespace MetaTabular.Tests;

public sealed class AnalyticsToTabularConverterTests
{
    [Fact]
    public void Convert_CopiesCommonAnalyticsIntent_ToTabularWorkspace()
    {
        var converted = AnalyticsToTabularConverter.Convert(MetaAnalyticsInstance.SampleCommerce);

        Assert.Equal(MetaAnalyticsInstance.SampleCommerce.TableList.Count, converted.TabularTableList.Count);
        Assert.Equal(MetaAnalyticsInstance.SampleCommerce.AttributeList.Count, converted.TabularColumnList.Count);
        Assert.Equal(MetaAnalyticsInstance.SampleCommerce.MeasureList.Count, converted.TabularMeasureList.Count);

        var sales = Assert.Single(converted.TabularTableList, row => row.Id == "table:sales");
        var salesAmount = Assert.Single(converted.TabularMeasureList, row => row.Id == "measure:sales-amount");
        var relationship = Assert.Single(converted.TabularRelationshipList, row => row.Id == "relationship:sales:customer");
        var roleFilter = Assert.Single(converted.TabularRoleFilterList);

        Assert.Same(sales, salesAmount.TabularTable);
        Assert.Same(sales, relationship.FromTable);
        Assert.Contains("SUM", salesAmount.Expression, StringComparison.Ordinal);
        Assert.Contains("Customer[Region]", roleFilter.Expression, StringComparison.Ordinal);
    }

    [Fact]
    public void Convert_RejectsNonDaxAttributeAndRoleFilterExpressions()
    {
        var attributeSource = CloneSample();
        var attribute = attributeSource.AttributeList[0];
        attribute.Expression = "1 + 1";
        attribute.ExpressionLanguage = "MDX";

        var attributeError = Assert.Throws<InvalidOperationException>(
            () => AnalyticsToTabularConverter.Convert(attributeSource));
        Assert.Contains("requires DAX", attributeError.Message, StringComparison.Ordinal);

        var roleSource = CloneSample();
        roleSource.RoleFilterList[0].ExpressionLanguage = "MDX";

        var roleError = Assert.Throws<InvalidOperationException>(
            () => AnalyticsToTabularConverter.Convert(roleSource));
        Assert.Contains("requires DAX", roleError.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Convert_NormalizesBlankAttributeExpressionsToNull()
    {
        var source = CloneSample();
        var attribute = source.AttributeList[0];
        attribute.Expression = "   ";
        attribute.ExpressionLanguage = "MDX";

        var converted = AnalyticsToTabularConverter.Convert(source);

        Assert.Null(converted.TabularColumnList.Single(row => row.Id == attribute.Id).Expression);
    }

    [Fact]
    public void Convert_RequiresExactlyOneSupportedAggregationBehaviorPerMeasure()
    {
        var missingSource = CloneSample();
        var missingMeasure = missingSource.MeasureList[0];
        missingSource.AggregationBehaviorList.RemoveAll(row =>
            ReferenceEquals(row.Measure, missingMeasure));
        Assert.Contains(
            "does not define an aggregation behavior",
            Assert.Throws<InvalidOperationException>(
                () => AnalyticsToTabularConverter.Convert(missingSource)).Message,
            StringComparison.Ordinal);

        var multipleSource = CloneSample();
        var existing = multipleSource.AggregationBehaviorList[0];
        multipleSource.AggregationBehaviorList.Add(new AggregationBehavior
        {
            Id = existing.Id + ":duplicate",
            Measure = existing.Measure,
            Function = existing.Function,
        });
        Assert.Contains(
            "defines multiple aggregation behaviors",
            Assert.Throws<InvalidOperationException>(
                () => AnalyticsToTabularConverter.Convert(multipleSource)).Message,
            StringComparison.Ordinal);

        var unsupportedSource = CloneSample();
        unsupportedSource.AggregationBehaviorList[0].Function = "MEDIAN";
        Assert.Contains(
            "does not have a supported DAX base-measure projection",
            Assert.Throws<InvalidOperationException>(
                () => AnalyticsToTabularConverter.Convert(unsupportedSource)).Message,
            StringComparison.Ordinal);
    }

    private static MetaAnalyticsModel CloneSample() =>
        TypedModelMapper.FromWorkspace(
            TypedModelMapper.ToWorkspace(MetaAnalyticsInstance.SampleCommerce),
            MetaAnalyticsModel.CreateEmpty);
}
