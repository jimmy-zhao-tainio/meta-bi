using MetaAnalytics;
using MetaConvert.AnalyticsToMultiDimensional;
using AnalyticsAttribute = MetaAnalytics.Attribute;
using AnalyticsMeasure = MetaAnalytics.Measure;
using AnalyticsSecurityRole = MetaAnalytics.SecurityRole;
using AnalyticsTable = MetaAnalytics.Table;

namespace MetaMultiDimensional.Tests;

public sealed class AnalyticsToMultiDimensionalConverterTests
{
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

    [Fact]
    public void Convert_RejectsTabularSpecificSecurityClearly()
    {
        var source = CreateConvertibleAnalyticsModel();
        var role = source.SecurityRoleList.Single();
        var sales = source.TableList.Single(row => row.Id == "Sales");
        source.RoleFilterList.Add(new RoleFilter
        {
            Id = "SalesRoleFilter",
            SecurityRole = role,
            Table = sales,
            ExpressionLanguage = "DAX",
            Expression = "Sales[Region] = USERNAME()",
        });

        var ex = Assert.Throws<InvalidOperationException>(() => AnalyticsToMultiDimensionalConverter.Convert(source));
        Assert.Contains("row/object security", ex.Message, StringComparison.OrdinalIgnoreCase);
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
        var salesAmount = Add(model.MeasureList, new AnalyticsMeasure
        {
            Id = "SalesAmount",
            Table = sales,
            SourceAttribute = salesAmountColumn,
            Name = "Sales Amount",
            DataTypeId = "meta:type:Decimal",
            FormatString = "#,0.00",
        });
        Add(model.AggregationBehaviorList, new AggregationBehavior
        {
            Id = "SalesAmountAggregation",
            Measure = salesAmount,
            Function = "Sum",
        });
        Add(model.SecurityRoleList, new AnalyticsSecurityRole
        {
            Id = "Readers",
            AnalyticsModel = analytics,
            Name = "Readers",
            Permission = "Read",
        });

        return model;
    }

    private static T Add<T>(ICollection<T> rows, T row)
    {
        rows.Add(row);
        return row;
    }
}
