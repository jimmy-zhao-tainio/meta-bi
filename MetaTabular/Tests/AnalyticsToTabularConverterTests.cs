using MetaAnalytics.Instance;
using MetaConvert.AnalyticsToTabular;

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
}
