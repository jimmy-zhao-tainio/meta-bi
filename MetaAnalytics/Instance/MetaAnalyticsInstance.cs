using AnalyticsAttribute = MetaAnalytics.Attribute;
using AnalyticsTable = MetaAnalytics.Table;

namespace MetaAnalytics.Instance;

public static class MetaAnalyticsInstance
{
    public static MetaAnalyticsModel SampleCommerce { get; } = CreateSampleCommerce();

    private static MetaAnalyticsModel CreateSampleCommerce()
    {
        var model = MetaAnalyticsModel.CreateEmpty();

        var analyticsModel = Add(model.AnalyticsModelList, new AnalyticsModel
        {
            Id = "model:commerce",
            Name = "Commerce Analytics",
            DefaultCulture = "en-US",
            Description = "Sample conceptual analytics model over the commerce warehouse.",
        });

        Add(model.DataSourceList, new DataSource
        {
            Id = "data-source:commerce-warehouse",
            AnalyticsModel = analyticsModel,
            Name = "Commerce Warehouse",
            Provider = "SqlServer",
            ConnectionReference = "COMMERCE_DW",
            SourceKind = "Relational",
        });

        var date = AddTable(model, analyticsModel, "table:date", "Date", "Dimension", dataCategory: "Time");
        var dateKey = AddAttribute(model, date, "DateKey", "DateKey", "meta:type:Int32", 10, isKey: true, isHidden: true);
        var calendarDate = AddAttribute(model, date, "CalendarDate", "CalendarDate", "meta:type:Date", 20);
        var calendarYear = AddAttribute(model, date, "CalendarYear", "CalendarYear", "meta:type:Int32", 30);
        var monthName = AddAttribute(model, date, "MonthName", "MonthName", "meta:type:String", 40);
        var calendar = Add(model.HierarchyList, new Hierarchy
        {
            Id = "hierarchy:date:calendar",
            Table = date,
            Name = "Calendar",
        });
        AddHierarchyLevel(model, calendar, calendarYear, "Year", 10);
        AddHierarchyLevel(model, calendar, monthName, "Month", 20);
        AddHierarchyLevel(model, calendar, calendarDate, "Date", 30);
        Add(model.SortByAttributeList, new SortByAttribute
        {
            Id = "sort-by:date:month-name",
            SourceAttribute = monthName,
            SortAttribute = calendarDate,
        });

        var customer = AddTable(model, analyticsModel, "table:customer", "Customer", "Dimension", dataCategory: "Customers");
        var customerKey = AddAttribute(model, customer, "CustomerKey", "CustomerKey", "meta:type:Int64", 10, isKey: true, isHidden: true);
        var customerName = AddAttribute(model, customer, "CustomerName", "Customer Name", "meta:type:String", 20);
        var customerTier = AddAttribute(model, customer, "CustomerTier", "Customer Tier", "meta:type:String", 30);
        var customerRegion = AddAttribute(model, customer, "Region", "Region", "meta:type:String", 40);

        var product = AddTable(model, analyticsModel, "table:product", "Product", "Dimension", dataCategory: "Products");
        var productKey = AddAttribute(model, product, "ProductKey", "ProductKey", "meta:type:Int64", 10, isKey: true, isHidden: true);
        var productName = AddAttribute(model, product, "ProductName", "Product Name", "meta:type:String", 20);
        var productCategory = AddAttribute(model, product, "ProductCategory", "Category", "meta:type:String", 30);
        var productHierarchy = Add(model.HierarchyList, new Hierarchy
        {
            Id = "hierarchy:product:category-product",
            Table = product,
            Name = "Category Product",
        });
        AddHierarchyLevel(model, productHierarchy, productCategory, "Category", 10);
        AddHierarchyLevel(model, productHierarchy, productName, "Product", 20);

        var sales = AddTable(model, analyticsModel, "table:sales", "Sales", "Fact", dataCategory: "Quantitative");
        var orderDateKey = AddAttribute(model, sales, "OrderDateKey", "OrderDateKey", "meta:type:Int64", 10, isHidden: true);
        var customerSalesKey = AddAttribute(model, sales, "CustomerKey", "CustomerKey", "meta:type:Int64", 20, isHidden: true);
        var productSalesKey = AddAttribute(model, sales, "ProductKey", "ProductKey", "meta:type:Int64", 30, isHidden: true);
        var quantityColumn = AddAttribute(model, sales, "Quantity", "Quantity", "meta:type:Int32", 40, isHidden: true, summarizeBy: "Sum");
        var salesAmountColumn = AddAttribute(model, sales, "SalesAmount", "Sales Amount", "meta:type:Decimal", 50, isHidden: true, summarizeBy: "Sum");
        var salesTargetColumn = AddAttribute(model, sales, "SalesTarget", "Sales Target", "meta:type:Decimal", 60, isHidden: true, summarizeBy: "Sum");

        AddRelationship(model, "relationship:sales:order-date", sales, orderDateKey, date, dateKey, "Order Date");
        AddRelationship(model, "relationship:sales:customer", sales, customerSalesKey, customer, customerKey, "Customer");
        AddRelationship(model, "relationship:sales:product", sales, productSalesKey, product, productKey, "Product");

        var salesAmount = AddMeasure(model, sales, salesAmountColumn, "measure:sales-amount", "Sales Amount", "meta:type:Decimal", "#,0.00", "Sales");
        Add(model.AggregationBehaviorList, new AggregationBehavior
        {
            Id = "aggregation:sales-amount",
            Measure = salesAmount,
            Function = "Sum",
        });

        var quantity = AddMeasure(model, sales, quantityColumn, "measure:quantity", "Quantity", "meta:type:Int32", "#,0", "Sales");
        Add(model.AggregationBehaviorList, new AggregationBehavior
        {
            Id = "aggregation:quantity",
            Measure = quantity,
            Function = "Sum",
        });

        var salesTarget = AddMeasure(model, sales, salesTargetColumn, "measure:sales-target", "Sales Target", "meta:type:Decimal", "#,0.00", "Sales");
        Add(model.AggregationBehaviorList, new AggregationBehavior
        {
            Id = "aggregation:sales-target",
            Measure = salesTarget,
            Function = "Sum",
        });

        var salesPerspective = Add(model.PerspectiveList, new Perspective
        {
            Id = "perspective:sales",
            AnalyticsModel = analyticsModel,
            Name = "Sales",
        });
        Add(model.PerspectiveTableList, new PerspectiveTable { Id = "perspective:sales:table:date", Perspective = salesPerspective, Table = date });
        Add(model.PerspectiveTableList, new PerspectiveTable { Id = "perspective:sales:table:customer", Perspective = salesPerspective, Table = customer });
        Add(model.PerspectiveTableList, new PerspectiveTable { Id = "perspective:sales:table:product", Perspective = salesPerspective, Table = product });
        Add(model.PerspectiveTableList, new PerspectiveTable { Id = "perspective:sales:table:sales", Perspective = salesPerspective, Table = sales });
        Add(model.PerspectiveHierarchyList, new PerspectiveHierarchy { Id = "perspective:sales:hierarchy:calendar", Perspective = salesPerspective, Hierarchy = calendar });
        Add(model.PerspectiveMeasureList, new PerspectiveMeasure { Id = "perspective:sales:measure:sales-amount", Perspective = salesPerspective, Measure = salesAmount });
        Add(model.PerspectiveMeasureList, new PerspectiveMeasure { Id = "perspective:sales:measure:quantity", Perspective = salesPerspective, Measure = quantity });
        Add(model.PerspectiveMeasureList, new PerspectiveMeasure { Id = "perspective:sales:measure:sales-target", Perspective = salesPerspective, Measure = salesTarget });

        var salesRole = Add(model.SecurityRoleList, new SecurityRole
        {
            Id = "role:sales-region-reader",
            AnalyticsModel = analyticsModel,
            Name = "Sales Region Reader",
            Permission = "Read",
        });
        Add(model.RoleMemberList, new RoleMember
        {
            Id = "role-member:sales-region-reader:analysts",
            SecurityRole = salesRole,
            MemberName = "CONTOSO\\SalesAnalysts",
            MemberKind = "WindowsGroup",
        });
        Add(model.RoleFilterList, new RoleFilter
        {
            Id = "role-filter:sales-region-reader:customer",
            SecurityRole = salesRole,
            Table = customer,
            ExpressionLanguage = "DAX",
            Expression = "Customer[Region] = LOOKUPVALUE(UserRegion[Region], UserRegion[UserName], USERNAME())",
        });
        Add(model.AttributePermissionList, new AttributePermission
        {
            Id = "attribute-permission:sales-region-reader:customer-tier",
            SecurityRole = salesRole,
            Attribute = customerTier,
            MetadataPermission = "None",
        });

        var sv = Add(model.CultureList, new Culture
        {
            Id = "culture:sv-SE",
            AnalyticsModel = analyticsModel,
            Name = "sv-SE",
        });
        Add(model.TableTranslationList, new TableTranslation { Id = "translation:sv-SE:table:sales", Culture = sv, Table = sales, Caption = "Forsaljning" });
        Add(model.MeasureTranslationList, new MeasureTranslation { Id = "translation:sv-SE:measure:sales-amount", Culture = sv, Measure = salesAmount, Caption = "Forsaljningsbelopp" });
        Add(model.HierarchyTranslationList, new HierarchyTranslation { Id = "translation:sv-SE:hierarchy:calendar", Culture = sv, Hierarchy = calendar, Caption = "Kalender" });

        _ = customerName;
        return model;
    }

    private static T Add<T>(ICollection<T> rows, T row)
    {
        rows.Add(row);
        return row;
    }

    private static AnalyticsTable AddTable(
        MetaAnalyticsModel model,
        AnalyticsModel analyticsModel,
        string id,
        string name,
        string kind,
        string? dataCategory = null)
    {
        return Add(model.TableList, new AnalyticsTable
        {
            Id = id,
            AnalyticsModel = analyticsModel,
            Name = name,
            Kind = kind,
            DataCategory = dataCategory,
        });
    }

    private static AnalyticsAttribute AddAttribute(
        MetaAnalyticsModel model,
        AnalyticsTable table,
        string idSegment,
        string name,
        string dataTypeId,
        int ordinal,
        bool isKey = false,
        bool isHidden = false,
        string? summarizeBy = null)
    {
        return Add(model.AttributeList, new AnalyticsAttribute
        {
            Id = $"{table.Id}:attribute:{idSegment}",
            Table = table,
            Name = name,
            DataTypeId = dataTypeId,
            Ordinal = ordinal.ToString(),
            IsKey = isKey ? "true" : "false",
            IsHidden = isHidden ? "true" : "false",
            SummarizeBy = summarizeBy,
        });
    }

    private static void AddHierarchyLevel(
        MetaAnalyticsModel model,
        Hierarchy hierarchy,
        AnalyticsAttribute attribute,
        string name,
        int ordinal)
    {
        Add(model.HierarchyLevelList, new HierarchyLevel
        {
            Id = $"{hierarchy.Id}:level:{ordinal}",
            Hierarchy = hierarchy,
            Attribute = attribute,
            Name = name,
            Ordinal = ordinal.ToString(),
        });
    }

    private static void AddRelationship(
        MetaAnalyticsModel model,
        string id,
        AnalyticsTable fromTable,
        AnalyticsAttribute fromAttribute,
        AnalyticsTable toTable,
        AnalyticsAttribute toAttribute,
        string roleName)
    {
        Add(model.RelationshipList, new Relationship
        {
            Id = id,
            FromTable = fromTable,
            FromAttribute = fromAttribute,
            ToTable = toTable,
            ToAttribute = toAttribute,
            Name = roleName,
            RoleName = roleName,
            RelationshipKind = "Regular",
            Cardinality = "ManyToOne",
            CrossFilterDirection = "Single",
            IsActive = "true",
            IsRequired = "true",
        });
    }

    private static Measure AddMeasure(
        MetaAnalyticsModel model,
        AnalyticsTable table,
        AnalyticsAttribute sourceAttribute,
        string id,
        string name,
        string dataTypeId,
        string formatString,
        string displayFolder)
    {
        return Add(model.MeasureList, new Measure
        {
            Id = id,
            Table = table,
            SourceAttribute = sourceAttribute,
            Name = name,
            DataTypeId = dataTypeId,
            FormatString = formatString,
            DisplayFolder = displayFolder,
        });
    }
}
