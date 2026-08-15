using MetaAnalytics;
using MetaConvert.AnalyticsToTabular;
using Meta.Surfaces.Xml;
using Meta.TypedModels;
using MetaBi.Tests.Common;

namespace MetaTabular.Tests;

public sealed class AnalyticsToTabularConverterTests
{
    [Fact]
    public void Convert_CopiesCommonAnalyticsIntent_ToTabularWorkspace()
    {
        var source = LoadSampleCommerce();
        var converted = AnalyticsToTabularConverter.Convert(source);

        Assert.Equal(source.TableList.Count, converted.TabularTableList.Count);
        Assert.Equal(source.AttributeList.Count, converted.TabularColumnList.Count);
        Assert.Equal(source.MeasureList.Count, converted.TabularMeasureList.Count);

        var sales = Assert.Single(converted.TabularTableList, row => row.Id == "table:sales");
        var salesAmount = Assert.Single(converted.TabularMeasureList, row => row.Id == "measure:sales-amount");
        var relationship = Assert.Single(converted.TabularRelationshipList, row => row.Id == "relationship:sales:customer");

        Assert.Same(sales, salesAmount.TabularTable);
        Assert.Same(sales, relationship.FromTable);
        Assert.Contains("SUM", salesAmount.Expression, StringComparison.Ordinal);
        Assert.Empty(converted.TabularRoleFilterList);
    }

    [Theory]
    [InlineData(typeof(SumAggregateFunction), "SUM")]
    [InlineData(typeof(AverageAggregateFunction), "AVERAGE")]
    [InlineData(typeof(CountAggregateFunction), "COUNT")]
    [InlineData(typeof(DistinctCountAggregateFunction), "DISTINCTCOUNT")]
    [InlineData(typeof(MinimumAggregateFunction), "MIN")]
    [InlineData(typeof(MaximumAggregateFunction), "MAX")]
    public void Convert_ProjectsEveryNeutralBaseMeasureAggregate(Type aggregateFunctionType, string daxFunction)
    {
        var source = CloneSample();
        SetAggregateFunctionType(source, source.MeasureList[0].AggregateFunction, aggregateFunctionType);

        var converted = AnalyticsToTabularConverter.Convert(source);

        Assert.StartsWith(
            daxFunction + "(",
            converted.TabularMeasureList.Single(row => row.Id == source.MeasureList[0].Id).Expression,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Convert_RejectsAnAggregateWithoutAConcreteType()
    {
        var unsupportedSource = CloneSample();
        unsupportedSource.SumAggregateFunctionList.RemoveAll(
            row => ReferenceEquals(row.AggregateFunction, unsupportedSource.MeasureList[0].AggregateFunction));
        Assert.Contains(
            "must reference one concrete aggregate-function entity; found 0",
            Assert.Throws<InvalidOperationException>(
                () => AnalyticsToTabularConverter.Convert(unsupportedSource)).Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Convert_RejectsAnAggregateWithOverlappingConcreteTypes()
    {
        var unsupportedSource = CloneSample();
        var aggregateFunction = unsupportedSource.MeasureList[0].AggregateFunction;
        unsupportedSource.AverageAggregateFunctionList.Add(new AverageAggregateFunction
        {
            Id = aggregateFunction.Id + ":average-type",
            AggregateFunction = aggregateFunction,
        });

        Assert.Contains(
            "must reference one concrete aggregate-function entity; found 2",
            Assert.Throws<InvalidOperationException>(
                () => AnalyticsToTabularConverter.Convert(unsupportedSource)).Message,
            StringComparison.Ordinal);
    }

    private static void SetAggregateFunctionType(
        MetaAnalyticsModel model,
        AggregateFunction aggregateFunction,
        Type aggregateFunctionType)
    {
        model.SumAggregateFunctionList.RemoveAll(row => ReferenceEquals(row.AggregateFunction, aggregateFunction));
        if (aggregateFunctionType == typeof(SumAggregateFunction)) model.SumAggregateFunctionList.Add(new SumAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else if (aggregateFunctionType == typeof(AverageAggregateFunction)) model.AverageAggregateFunctionList.Add(new AverageAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else if (aggregateFunctionType == typeof(CountAggregateFunction)) model.CountAggregateFunctionList.Add(new CountAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else if (aggregateFunctionType == typeof(DistinctCountAggregateFunction)) model.DistinctCountAggregateFunctionList.Add(new DistinctCountAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else if (aggregateFunctionType == typeof(MinimumAggregateFunction)) model.MinimumAggregateFunctionList.Add(new MinimumAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else if (aggregateFunctionType == typeof(MaximumAggregateFunction)) model.MaximumAggregateFunctionList.Add(new MaximumAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else throw new ArgumentOutOfRangeException(nameof(aggregateFunctionType));
    }

    private static MetaAnalyticsModel CloneSample() =>
        TypedModelMapper.FromWorkspace(
            TypedModelMapper.ToWorkspace(LoadSampleCommerce()),
            MetaAnalyticsModel.CreateEmpty);

    private static MetaAnalyticsModel LoadSampleCommerce()
    {
        var workspacePath = Path.Combine(
            CliTestRunner.FindRepositoryRoot(),
            "MetaAnalytics",
            "Workspaces",
            "SampleAnalyticsCommerce");
        return TypedWorkspaceXmlSerializer.Load<MetaAnalyticsModel>(workspacePath, searchUpward: false);
    }
}
