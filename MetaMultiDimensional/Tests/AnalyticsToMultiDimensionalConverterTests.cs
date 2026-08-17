using Meta.Integration;
using Meta.Operations.Domain;
using Meta.TypedModels;
using MetaAnalytics;
using MetaBi.Tests.Common;
using MetaConvert.AnalyticsToMultiDimensional;
using MetaWeave.Core;
using MetaWeaveScript.Execution;
using AnalyticsAttribute = MetaAnalytics.Attribute;
using AnalyticsMeasure = MetaAnalytics.Measure;
using AnalyticsSecurityRole = MetaAnalytics.SecurityRole;
using AnalyticsTable = MetaAnalytics.Table;

namespace MetaMultiDimensional.Tests;

public sealed class AnalyticsToMultiDimensionalConverterTests
{
    [Fact]
    public async Task SanctionedWeave_MatchesEstablishedConverter_AndExercisesEveryPopulation()
    {
        var source = CreateConvertibleAnalyticsModel();
        var expected = TypedWorkspaceModelMapper.ToInMemoryWorkspace(
            AnalyticsToMultiDimensionalCSharpReference.Convert(source));
        var progress = new List<MetaWeaveScriptExecutionProgress>();
        var converted = TypedWorkspaceModelMapper.ToInMemoryWorkspace(
            AnalyticsToMultiDimensionalConverter.Convert(source, progress.Add));

        var actual = await ExecuteSanctionedWeaveAsync(source);

        Assert.True(actual.IsSuccess, FormatIssues(actual));
        var output = Assert.IsType<InMemoryWorkspace>(actual.OutputWorkspace);
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, converted));
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, output));
        Assert.Equal(progress[^1].TotalTaskCount, progress[^1].CompletedTaskCount);
        foreach (var targetEntity in LoadSanctionedDirection().Transformations
                     .Select(transformation => transformation.TargetEntityName))
        {
            Assert.True(
                output.Instance.RecordsByEntity.TryGetValue(targetEntity, out var records) && records.Count > 0,
                $"Transformation target '{targetEntity}' produced no witness rows.");
        }
    }

    [Fact]
    public void Convert_CopiesCommonAnalyticsIntent_ToMultidimensionalWorkspace()
    {
        var converted = AnalyticsToMultiDimensionalConverter.Convert(CreateConvertibleAnalyticsModel());

        var cube = Assert.Single(converted.CubeList);
        var dateDimension = Assert.Single(converted.DimensionList, row => row.Id == "Date");
        var salesGroup = Assert.Single(converted.MeasureGroupList, row => row.Id == "Sales:measure-group");
        var salesAmount = Assert.Single(converted.MeasureList, row => row.Id == "SalesAmount");
        var usage = Assert.Single(converted.DimensionUsageList);

        Assert.Equal("Commerce", cube.Name);
        Assert.Equal("Date", dateDimension.Name);
        Assert.Same(salesGroup, salesAmount.MeasureGroup);
        Assert.Same(salesGroup, usage.MeasureGroup);
        Assert.Equal("SalesAmount", salesAmount.SourceName);
        Assert.Equal("Sum", salesAmount.AggregateFunction);
        Assert.Equal("OrderDate", usage.RoleName);
    }

    [Theory]
    [InlineData(typeof(SumAggregateFunction), "Sum")]
    [InlineData(typeof(AverageAggregateFunction), "Average")]
    [InlineData(typeof(CountAggregateFunction), "Count")]
    [InlineData(typeof(DistinctCountAggregateFunction), "DistinctCount")]
    [InlineData(typeof(MinimumAggregateFunction), "Min")]
    [InlineData(typeof(MaximumAggregateFunction), "Max")]
    public void Convert_ProjectsEveryNeutralBaseMeasureAggregate(
        Type aggregateFunctionType,
        string targetFunction)
    {
        var source = CreateConvertibleAnalyticsModel();
        SetAggregateFunctionType(source, source.MeasureList.Single().AggregateFunction, aggregateFunctionType);

        var expected = AnalyticsToMultiDimensionalCSharpReference.Convert(source);
        var converted = AnalyticsToMultiDimensionalConverter.Convert(source);

        Assert.Equal(targetFunction, expected.MeasureList.Single().AggregateFunction);
        Assert.Equal(targetFunction, converted.MeasureList.Single().AggregateFunction);
    }

    [Fact]
    public void Convert_RejectsTabularSpecificSecurityClearly()
    {
        var source = CreateConvertibleAnalyticsModel();
        var role = source.SecurityRoleList.Single();
        var sales = source.TableList.Single(row => row.Id == "Sales");
        source.TablePermissionList.Add(new TablePermission
        {
            Id = "SalesTablePermission",
            SecurityRole = role,
            Table = sales,
            MetadataPermission = "None",
        });

        var ex = Assert.Throws<InvalidOperationException>(() => AnalyticsToMultiDimensionalConverter.Convert(source));
        Assert.Contains("row/object security", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SanctionedWeave_RejectsTabularSpecificSecurityClearly()
    {
        var source = CreateConvertibleAnalyticsModel();
        source.TablePermissionList.Add(new TablePermission
        {
            Id = "SalesTablePermission",
            SecurityRole = source.SecurityRoleList.Single(),
            Table = source.TableList.Single(row => row.Id == "Sales"),
            MetadataPermission = "None",
        });

        var result = await ExecuteSanctionedWeaveAsync(source);

        Assert.False(result.IsSuccess);
        Assert.Null(result.OutputWorkspace);
        var issue = Assert.Single(result.Issues);
        Assert.Equal("UnsupportedSecurity", issue.Code);
        Assert.Equal("UnsupportedSecurity", issue.RequirementName);
        Assert.Contains("row/object security", issue.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(false, "VariantCount=NULL")]
    [InlineData(true, "VariantCount=2")]
    public async Task SanctionedWeave_RejectsInvalidAggregateFunctionUnion(
        bool addOverlappingVariant,
        string expectedEvidence)
    {
        var source = CreateConvertibleAnalyticsModel();
        var aggregateFunction = source.MeasureList.Single().AggregateFunction;
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
            source.SumAggregateFunctionList.Clear();
        }

        var result = await ExecuteSanctionedWeaveAsync(source);

        Assert.False(result.IsSuccess);
        Assert.Null(result.OutputWorkspace);
        var issue = Assert.Single(result.Issues);
        Assert.Equal("MeasureAggregateFunctionInvalid", issue.Code);
        Assert.Equal("MeasureAggregateFunction", issue.RequirementName);
        Assert.Contains(expectedEvidence, issue.Message, StringComparison.Ordinal);
    }

    private static MetaAnalyticsModel CreateConvertibleAnalyticsModel()
    {
        var model = MetaAnalyticsModel.CreateEmpty();
        var analytics = Add(model.AnalyticsModelList, new AnalyticsModel
        {
            Id = "Commerce",
            Name = "Commerce",
            DefaultCulture = "en-US",
        });
        Add(model.DataSourceList, new MetaAnalytics.DataSource
        {
            Id = "CommerceSource",
            AnalyticsModel = analytics,
            Name = "CommerceSource",
            Provider = "SQL Server",
            ConnectionReference = "Commerce",
            SourceKind = "Relational",
        });

        var date = Add(model.TableList, new AnalyticsTable
        {
            Id = "Date",
            AnalyticsModel = analytics,
            Name = "Date",
            Kind = "Dimension",
            DataCategory = "Time",
        });
        var dateKey = Add(model.AttributeList, new AnalyticsAttribute
        {
            Id = "DateKey",
            Table = date,
            Name = "DateKey",
            DataTypeId = "meta:type:Int32",
            Ordinal = "10",
            IsKey = "true",
            IsHidden = "true",
        });
        var calendarYear = Add(model.AttributeList, new AnalyticsAttribute
        {
            Id = "CalendarYear",
            Table = date,
            Name = "CalendarYear",
            DataTypeId = "meta:type:Int32",
            Ordinal = "20",
        });
        Add(model.AttributeRelationshipList, new MetaAnalytics.AttributeRelationship
        {
            Id = "CalendarYearToDateKey",
            ChildAttribute = calendarYear,
            ParentAttribute = dateKey,
            RelationshipType = "Rigid",
        });
        var calendar = Add(model.HierarchyList, new Hierarchy
        {
            Id = "Calendar",
            Table = date,
            Name = "Calendar",
        });
        Add(model.HierarchyLevelList, new HierarchyLevel
        {
            Id = "CalendarYearLevel",
            Hierarchy = calendar,
            Attribute = calendarYear,
            Name = "Year",
            Ordinal = "10",
        });

        var sales = Add(model.TableList, new AnalyticsTable
        {
            Id = "Sales",
            AnalyticsModel = analytics,
            Name = "Sales",
            Kind = "Fact",
        });
        var orderDateKey = Add(model.AttributeList, new AnalyticsAttribute
        {
            Id = "OrderDateKey",
            Table = sales,
            Name = "OrderDateKey",
            DataTypeId = "meta:type:Int32",
            Ordinal = "10",
        });
        var salesAmountColumn = Add(model.AttributeList, new AnalyticsAttribute
        {
            Id = "SalesAmountColumn",
            Table = sales,
            Name = "SalesAmount",
            DataTypeId = "meta:type:Decimal",
            Ordinal = "20",
        });
        Add(model.RelationshipList, new Relationship
        {
            Id = "SalesOrderDate",
            FromTable = sales,
            FromAttribute = orderDateKey,
            ToTable = date,
            ToAttribute = dateKey,
            Name = "OrderDate",
            RoleName = "OrderDate",
            RelationshipKind = "Regular",
            Cardinality = "ManyToOne",
        });
        var aggregateFunction = Add(model.AggregateFunctionList, new AggregateFunction
        {
            Id = "SalesAmount:aggregate-function",
        });
        Add(model.SumAggregateFunctionList, new SumAggregateFunction
        {
            Id = "SalesAmount:aggregate-function:type",
            AggregateFunction = aggregateFunction,
        });
        var salesAmount = Add(model.MeasureList, new AnalyticsMeasure
        {
            Id = "SalesAmount",
            Table = sales,
            SourceAttribute = salesAmountColumn,
            Name = "Sales Amount",
            AggregateFunction = aggregateFunction,
            DataTypeId = "meta:type:Decimal",
            FormatString = "#,0.00",
        });
        var perspective = Add(model.PerspectiveList, new MetaAnalytics.Perspective
        {
            Id = "BusinessUsers",
            AnalyticsModel = analytics,
            Name = "Business Users",
        });
        Add(model.PerspectiveTableList, new MetaAnalytics.PerspectiveTable
        {
            Id = "BusinessUsersDate",
            Perspective = perspective,
            Table = date,
        });
        Add(model.PerspectiveTableList, new MetaAnalytics.PerspectiveTable
        {
            Id = "BusinessUsersSales",
            Perspective = perspective,
            Table = sales,
        });
        Add(model.PerspectiveMeasureList, new MetaAnalytics.PerspectiveMeasure
        {
            Id = "BusinessUsersSalesAmount",
            Perspective = perspective,
            Measure = salesAmount,
        });
        var role = Add(model.SecurityRoleList, new AnalyticsSecurityRole
        {
            Id = "Readers",
            AnalyticsModel = analytics,
            Name = "Readers",
            Permission = "Read",
        });
        Add(model.RoleMemberList, new MetaAnalytics.RoleMember
        {
            Id = "ReadersMember",
            SecurityRole = role,
            MemberName = "DOMAIN\\Readers",
        });
        var culture = Add(model.CultureList, new MetaAnalytics.Culture
        {
            Id = "en-US",
            AnalyticsModel = analytics,
            Name = "en-US",
            Description = "English",
        });
        Add(model.TableTranslationList, new MetaAnalytics.TableTranslation
        {
            Id = "DateTranslation",
            Culture = culture,
            Table = date,
            Caption = "Date",
        });
        Add(model.AttributeTranslationList, new MetaAnalytics.AttributeTranslation
        {
            Id = "CalendarYearTranslation",
            Culture = culture,
            Attribute = calendarYear,
            Caption = "Calendar year",
        });
        Add(model.MeasureTranslationList, new MetaAnalytics.MeasureTranslation
        {
            Id = "SalesAmountTranslation",
            Culture = culture,
            Measure = salesAmount,
            Caption = "Sales amount",
        });
        Add(model.PerspectiveTranslationList, new MetaAnalytics.PerspectiveTranslation
        {
            Id = "BusinessUsersTranslation",
            Culture = culture,
            Perspective = perspective,
            Caption = "Business users",
        });

        return model;
    }

    private static void SetAggregateFunctionType(
        MetaAnalyticsModel model,
        AggregateFunction aggregateFunction,
        Type aggregateFunctionType)
    {
        model.SumAggregateFunctionList.Clear();
        if (aggregateFunctionType == typeof(SumAggregateFunction)) model.SumAggregateFunctionList.Add(new SumAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else if (aggregateFunctionType == typeof(AverageAggregateFunction)) model.AverageAggregateFunctionList.Add(new AverageAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else if (aggregateFunctionType == typeof(CountAggregateFunction)) model.CountAggregateFunctionList.Add(new CountAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else if (aggregateFunctionType == typeof(DistinctCountAggregateFunction)) model.DistinctCountAggregateFunctionList.Add(new DistinctCountAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else if (aggregateFunctionType == typeof(MinimumAggregateFunction)) model.MinimumAggregateFunctionList.Add(new MinimumAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else if (aggregateFunctionType == typeof(MaximumAggregateFunction)) model.MaximumAggregateFunctionList.Add(new MaximumAggregateFunction { Id = aggregateFunction.Id + ":type", AggregateFunction = aggregateFunction });
        else throw new ArgumentOutOfRangeException(nameof(aggregateFunctionType));
    }

    private static T Add<T>(ICollection<T> rows, T row)
    {
        rows.Add(row);
        return row;
    }

    private static Task<MetaWeaveScriptApplicationResult> ExecuteSanctionedWeaveAsync(
        MetaAnalyticsModel source)
    {
        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            LoadSanctionedDirection(),
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(source),
            TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaMultiDimensionalModel.CreateEmpty()));
        return Task.FromResult(result);
    }

    private static string FormatIssues(MetaWeaveScriptApplicationResult result) =>
        string.Join(Environment.NewLine, result.Issues.Select(issue => $"{issue.Code}: {issue.Message}"));

    private static MetaWeaveScriptDirection LoadSanctionedDirection() =>
        new MetaWeaveScriptDirectionLoader().Load(
            Path.Combine(
                CliTestRunner.FindRepositoryRoot(),
                "MetaConvert",
                "Weaves",
                "AnalyticsToMultiDimensional"),
            "forward");
}
