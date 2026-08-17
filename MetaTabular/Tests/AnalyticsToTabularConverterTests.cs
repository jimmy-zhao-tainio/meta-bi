using Meta.Integration;
using Meta.Operations.Domain;
using MetaAnalytics;
using MetaConvert.AnalyticsToTabular;
using Meta.Surfaces.Xml;
using Meta.TypedModels;
using MetaBi.Tests.Common;
using MetaWeave.Core;
using MetaWeaveScript.Execution;

namespace MetaTabular.Tests;

public sealed class AnalyticsToTabularConverterTests
{
    [Fact]
    public async Task SanctionedWeave_MatchesEstablishedConverter()
    {
        var source = LoadSampleCommerce();
        var expected = TypedModelMapper.ToWorkspace(AnalyticsToTabularCSharpReference.Convert(source));
        var progress = new List<MetaWeaveScriptExecutionProgress>();
        var converted = TypedModelMapper.ToWorkspace(
            AnalyticsToTabularConverter.Convert(source, progress.Add));

        var actual = await ExecuteSanctionedWeaveAsync(TypedModelMapper.ToWorkspace(source));

        Assert.True(actual.IsSuccess, FormatIssues(actual));
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, converted));
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, actual.OutputWorkspace!));
        Assert.Equal(progress[^1].TotalTaskCount, progress[^1].CompletedTaskCount);
    }

    [Fact]
    public async Task SanctionedWeave_ExercisesEveryTransformationPopulation()
    {
        var source = CloneSample();
        source.AttributeTranslationList.Add(new AttributeTranslation
        {
            Id = "translation:attribute",
            Attribute = source.AttributeList[0],
            Culture = source.CultureList[0],
            Caption = "Translated attribute",
        });
        source.PerspectiveAttributeList.Add(new PerspectiveAttribute
        {
            Id = "perspective:attribute",
            Attribute = source.AttributeList[0],
            Perspective = source.PerspectiveList[0],
        });
        source.PerspectiveTranslationList.Add(new PerspectiveTranslation
        {
            Id = "translation:perspective",
            Culture = source.CultureList[0],
            Perspective = source.PerspectiveList[0],
            Caption = "Translated perspective",
        });
        source.TablePermissionList.Add(new TablePermission
        {
            Id = "permission:table",
            SecurityRole = source.SecurityRoleList[0],
            Table = source.TableList[0],
            MetadataPermission = "Read",
        });
        var expected = TypedModelMapper.ToWorkspace(AnalyticsToTabularCSharpReference.Convert(source));
        var converted = TypedModelMapper.ToWorkspace(AnalyticsToTabularConverter.Convert(source));

        var actual = await ExecuteSanctionedWeaveAsync(TypedModelMapper.ToWorkspace(source));

        Assert.True(actual.IsSuccess, FormatIssues(actual));
        var output = Assert.IsType<InMemoryWorkspace>(actual.OutputWorkspace);
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, converted));
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, output));
        foreach (var targetEntity in LoadSanctionedDirection().Transformations
                     .Select(transformation => transformation.TargetEntityName))
        {
            Assert.True(
                output.Instance.RecordsByEntity.TryGetValue(targetEntity, out var records) && records.Count > 0,
                $"Transformation target '{targetEntity}' produced no witness rows.");
        }
    }

    [Theory]
    [InlineData(false, "VariantCount=NULL")]
    [InlineData(true, "VariantCount=2")]
    public async Task SanctionedWeave_RejectsInvalidAggregateFunctionUnion(
        bool addOverlappingVariant,
        string expectedEvidence)
    {
        var source = CloneSample();
        var aggregateFunction = source.MeasureList[0].AggregateFunction;
        if (addOverlappingVariant)
        {
            source.AverageAggregateFunctionList.Add(new AverageAggregateFunction
            {
                Id = aggregateFunction.Id + ":average-type",
                AggregateFunction = aggregateFunction,
            });
        }
        else
        {
            source.SumAggregateFunctionList.RemoveAll(
                row => ReferenceEquals(row.AggregateFunction, aggregateFunction));
        }

        var result = await ExecuteSanctionedWeaveAsync(TypedModelMapper.ToWorkspace(source));

        Assert.False(result.IsSuccess);
        Assert.Null(result.OutputWorkspace);
        var issue = Assert.Single(result.Issues);
        Assert.Equal("MeasureAggregateFunctionInvalid", issue.Code);
        Assert.Equal("MeasureAggregateFunction", issue.RequirementName);
        Assert.Contains(expectedEvidence, issue.Message, StringComparison.Ordinal);
        Assert.Contains("exactly one", issue.Message, StringComparison.Ordinal);
    }

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
    public async Task Convert_ProjectsEveryNeutralBaseMeasureAggregate(
        Type aggregateFunctionType,
        string daxFunction)
    {
        var source = CloneSample();
        SetAggregateFunctionType(source, source.MeasureList[0].AggregateFunction, aggregateFunctionType);

        var expected = AnalyticsToTabularCSharpReference.Convert(source);
        var converted = AnalyticsToTabularConverter.Convert(source);
        var woven = await ExecuteSanctionedWeaveAsync(TypedModelMapper.ToWorkspace(source));

        Assert.StartsWith(
            daxFunction + "(",
            converted.TabularMeasureList.Single(row => row.Id == source.MeasureList[0].Id).Expression,
            StringComparison.Ordinal);
        Assert.True(woven.IsSuccess, FormatIssues(woven));
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(
            TypedModelMapper.ToWorkspace(expected),
            TypedModelMapper.ToWorkspace(converted)));
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(
            TypedModelMapper.ToWorkspace(converted),
            woven.OutputWorkspace!));
    }

    [Fact]
    public void Convert_RejectsAnAggregateWithoutAConcreteType()
    {
        var unsupportedSource = CloneSample();
        unsupportedSource.SumAggregateFunctionList.RemoveAll(
            row => ReferenceEquals(row.AggregateFunction, unsupportedSource.MeasureList[0].AggregateFunction));
        Assert.Contains(
            "exactly one concrete aggregate-function entity",
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
            "exactly one concrete aggregate-function entity",
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

    private static async Task<MetaWeaveScriptApplicationResult> ExecuteSanctionedWeaveAsync(
        InMemoryWorkspace source)
    {
        var repositoryRoot = CliTestRunner.FindRepositoryRoot();
        var direction = LoadSanctionedDirection();
        var targetContract = await TypedWorkspaceModelMapper.LoadStateAsync(
            Path.Combine(repositoryRoot, "MetaTabular", "Workspaces", "MetaTabular"));
        var emptyTarget = new InMemoryWorkspace(
            targetContract.Model.Clone(),
            new GenericInstance { ModelName = targetContract.Model.Name });

        return new MetaWeaveScriptExecutionService().ExecuteDirection(
            direction,
            source,
            emptyTarget);
    }

    private static string FormatIssues(MetaWeaveScriptApplicationResult result) =>
        string.Join(Environment.NewLine, result.Issues.Select(issue => $"{issue.Code}: {issue.Message}"));

    private static MetaWeaveScriptDirection LoadSanctionedDirection() =>
        new MetaWeaveScriptDirectionLoader().Load(
            Path.Combine(
                CliTestRunner.FindRepositoryRoot(),
                "MetaConvert",
                "Weaves",
                "AnalyticsToTabular"),
            "forward");
}
