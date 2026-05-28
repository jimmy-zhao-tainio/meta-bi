using MetaDataWarehouse;

namespace MetaDataWarehouse.Instance;

public static class MetaDataWarehouseInstance
{
    public static MetaDataWarehouseModel SampleSales { get; } = CreateSampleSales();

    private static MetaDataWarehouseModel CreateSampleSales()
    {
        var model = MetaDataWarehouseModel.CreateEmpty();

        var warehouse = Add(model.WarehouseList, new Warehouse
        {
            Id = "warehouse:commerce",
            Name = "Commerce",
            Description = "Sample commerce dimensional warehouse.",
        });

        var date = AddDimension(model, warehouse, "dimension:date", "Date");
        AddDimensionKey(model, date, "CalendarDateKey", "meta:type:Int32");
        var calendarDate = AddAttribute(model, date, "Date", "CalendarDate", "meta:type:Date", 10);
        AddAttribute(model, date, "MonthName", "MonthName", "meta:type:String", 20);
        AddAttribute(model, date, "FiscalYear", "FiscalYear", "meta:type:Int32", 30);
        Add(model.ConformedDimensionList, new ConformedDimension
        {
            Id = "conformed:date",
            Dimension = date,
            ConformanceName = "EnterpriseDate",
            Description = "Shared calendar conformance dimension.",
        });

        var customer = AddDimension(model, warehouse, "dimension:customer", "Customer");
        AddDimensionKey(model, customer, "CustomerNumber", "meta:type:String");
        var customerName = AddAttribute(model, customer, "CustomerName", "CustomerName", "meta:type:String", 10);
        var customerTier = AddAttribute(model, customer, "CustomerTier", "CustomerTier", "meta:type:String", 20);
        var scd = Add(model.SlowlyChangingDimensionList, new SlowlyChangingDimension
        {
            Id = "scd:customer",
            Name = "CustomerHistory",
            Dimension = customer,
        });
        Add(model.Type2DimensionAttributeList, new Type2DimensionAttribute
        {
            Id = "scd:customer:customer-name",
            SlowlyChangingDimension = scd,
            DimensionAttribute = customerName,
        });
        Add(model.Type1DimensionAttributeList, new Type1DimensionAttribute
        {
            Id = "scd:customer:customer-tier",
            SlowlyChangingDimension = scd,
            DimensionAttribute = customerTier,
        });

        var product = AddDimension(model, warehouse, "dimension:product", "Product");
        AddDimensionKey(model, product, "ProductNumber", "meta:type:String");
        AddAttribute(model, product, "ProductName", "ProductName", "meta:type:String", 10);
        AddAttribute(model, product, "ProductCategory", "ProductCategory", "meta:type:String", 20);

        var demographics = AddDimension(model, warehouse, "dimension:customer-demographics", "CustomerDemographics");
        AddDimensionKey(model, demographics, "CustomerDemographicsCode", "meta:type:String");
        AddAttribute(model, demographics, "AgeBand", "AgeBand", "meta:type:String", 10);
        AddAttribute(model, demographics, "IncomeBand", "IncomeBand", "meta:type:String", 20);
        Add(model.MiniDimensionList, new MiniDimension
        {
            Id = "mini:customer-demographics",
            SourceDimension = customer,
            ProfileDimension = demographics,
            RoleName = "Demographics",
        });

        var fiscalCalendar = AddDimension(model, warehouse, "dimension:fiscal-calendar", "FiscalCalendar");
        AddDimensionKey(model, fiscalCalendar, "FiscalCalendarCode", "meta:type:String");
        Add(model.OutriggerDimensionList, new OutriggerDimension
        {
            Id = "outrigger:date:fiscal-calendar",
            ParentDimension = date,
            ChildDimension = fiscalCalendar,
            RoleName = "FiscalCalendar",
            Ordinal = "10",
            IsRequired = "false",
        });

        var salesOrder = AddFact(model, warehouse, "fact:sales-order", "SalesOrder");
        Add(model.TransactionFactList, new TransactionFact { Id = "transaction-fact:sales-order", Fact = salesOrder });
        Add(model.FactGrainList, new FactGrain
        {
            Id = "grain:sales-order-line",
            Fact = salesOrder,
            Name = "Sales order line",
            Description = "One row per sales order line.",
        });
        AddFactDimension(model, salesOrder, date, "OrderDate", 10);
        AddFactDimension(model, salesOrder, date, "ShipDate", 20, required: false);
        AddFactDimension(model, salesOrder, customer, "Customer", 30);
        AddFactDimension(model, salesOrder, product, "Product", 40);
        AddDegenerateDimension(model, salesOrder, "OrderNumber", "meta:type:String", 10);
        AddMeasure(model, salesOrder, "Quantity", "meta:type:Int32", 10);
        AddMeasure(model, salesOrder, "SalesAmount", "meta:type:Decimal", 20);

        var inventory = AddFact(model, warehouse, "fact:inventory-snapshot", "InventorySnapshot");
        Add(model.PeriodicSnapshotFactList, new PeriodicSnapshotFact
        {
            Id = "periodic-snapshot:inventory",
            Fact = inventory,
            PeriodName = "Day",
        });
        AddFactDimension(model, inventory, date, "SnapshotDate", 10);
        AddFactDimension(model, inventory, product, "Product", 20);
        AddMeasure(model, inventory, "OnHandQuantity", "meta:type:Int32", 10);

        var fulfillment = AddFact(model, warehouse, "fact:order-fulfillment", "OrderFulfillment");
        var accumulating = Add(model.AccumulatingSnapshotFactList, new AccumulatingSnapshotFact
        {
            Id = "accumulating-snapshot:order-fulfillment",
            Fact = fulfillment,
        });
        Add(model.AccumulatingSnapshotMilestoneList, new AccumulatingSnapshotMilestone
        {
            Id = "milestone:order-placed",
            AccumulatingSnapshotFact = accumulating,
            Name = "OrderPlaced",
            DateRoleName = "OrderPlaced",
            Ordinal = "10",
        });
        Add(model.AccumulatingSnapshotMilestoneList, new AccumulatingSnapshotMilestone
        {
            Id = "milestone:order-shipped",
            AccumulatingSnapshotFact = accumulating,
            Name = "OrderShipped",
            DateRoleName = "OrderShipped",
            Ordinal = "20",
        });
        AddFactDimension(model, fulfillment, customer, "Customer", 10);
        AddMeasure(model, fulfillment, "DaysToShip", "meta:type:Int32", 10);

        var promotionCoverage = AddFact(model, warehouse, "fact:promotion-coverage", "PromotionCoverage");
        Add(model.FactlessFactList, new FactlessFact
        {
            Id = "factless:promotion-coverage",
            Fact = promotionCoverage,
        });
        AddFactDimension(model, promotionCoverage, date, "PromotionDate", 10);
        AddFactDimension(model, promotionCoverage, product, "Product", 20);

        var monthlySales = AddFact(model, warehouse, "fact:monthly-sales-summary", "MonthlySalesSummary");
        Add(model.AggregateFactList, new AggregateFact
        {
            Id = "aggregate:monthly-sales-summary",
            AggregatedFact = monthlySales,
            SourceFact = salesOrder,
        });
        AddFactDimension(model, monthlySales, date, "Month", 10);
        AddFactDimension(model, monthlySales, product, "Product", 20);
        AddMeasure(model, monthlySales, "SalesAmount", "meta:type:Decimal", 10);

        var customerGroupBridge = Add(model.BridgeTableList, new BridgeTable
        {
            Id = "bridge:customer-group",
            Warehouse = warehouse,
            Name = "CustomerGroup",
        });
        Add(model.BridgeParticipantList, new BridgeParticipant
        {
            Id = "bridge-participant:customer-group:customer",
            BridgeTable = customerGroupBridge,
            Dimension = customer,
            RoleName = "Customer",
            Ordinal = "10",
            IsRequired = "true",
        });
        Add(model.BridgeWeightList, new BridgeWeight
        {
            Id = "bridge-weight:customer-group",
            BridgeTable = customerGroupBridge,
            Name = "AllocationWeight",
            DataTypeId = "meta:type:Decimal",
        });
        Add(model.FactBridgeList, new FactBridge
        {
            Id = "fact-bridge:sales-order:customer-group",
            Fact = salesOrder,
            BridgeTable = customerGroupBridge,
            RoleName = "CustomerGroup",
            Ordinal = "50",
        });

        return model;
    }

    private static T Add<T>(ICollection<T> rows, T row)
    {
        rows.Add(row);
        return row;
    }

    private static Dimension AddDimension(MetaDataWarehouseModel model, Warehouse warehouse, string id, string name)
    {
        return Add(model.DimensionList, new Dimension
        {
            Id = id,
            Warehouse = warehouse,
            Name = name,
        });
    }

    private static Fact AddFact(MetaDataWarehouseModel model, Warehouse warehouse, string id, string name)
    {
        return Add(model.FactList, new Fact
        {
            Id = id,
            Warehouse = warehouse,
            Name = name,
        });
    }

    private static void AddDimensionKey(
        MetaDataWarehouseModel model,
        Dimension dimension,
        string businessKeyPartName,
        string businessKeyType)
    {
        var keyAttribute = Add(model.DimensionAttributeList, new DimensionAttribute
        {
            Id = $"{dimension.Id}:attribute:{businessKeyPartName}",
            Dimension = dimension,
            Name = businessKeyPartName,
            DataTypeId = businessKeyType,
            Ordinal = "0",
            IsNullable = "false",
        });

        var key = Add(model.DimensionBusinessKeyList, new DimensionBusinessKey
        {
            Id = $"{dimension.Id}:business-key",
            Dimension = dimension,
            Name = $"{dimension.Name} business key",
        });

        Add(model.DimensionBusinessKeyPartList, new DimensionBusinessKeyPart
        {
            Id = $"{key.Id}:part:1",
            DimensionBusinessKey = key,
            DimensionAttribute = keyAttribute,
            Ordinal = "10",
        });
    }

    private static DimensionAttribute AddAttribute(
        MetaDataWarehouseModel model,
        Dimension dimension,
        string idSegment,
        string name,
        string dataTypeId,
        int ordinal)
    {
        return Add(model.DimensionAttributeList, new DimensionAttribute
        {
            Id = $"{dimension.Id}:attribute:{idSegment}",
            Dimension = dimension,
            Name = name,
            DataTypeId = dataTypeId,
            Ordinal = ordinal.ToString(),
            IsNullable = "true",
        });
    }

    private static void AddFactDimension(
        MetaDataWarehouseModel model,
        Fact fact,
        Dimension dimension,
        string roleName,
        int ordinal,
        bool required = true)
    {
        Add(model.FactDimensionList, new FactDimension
        {
            Id = $"{fact.Id}:dimension:{roleName}",
            Fact = fact,
            Dimension = dimension,
            RoleName = roleName,
            Ordinal = ordinal.ToString(),
            IsRequired = required ? "true" : "false",
        });
    }

    private static void AddDegenerateDimension(
        MetaDataWarehouseModel model,
        Fact fact,
        string name,
        string dataTypeId,
        int ordinal)
    {
        Add(model.DegenerateDimensionList, new DegenerateDimension
        {
            Id = $"{fact.Id}:degenerate:{name}",
            Fact = fact,
            Name = name,
            DataTypeId = dataTypeId,
            Ordinal = ordinal.ToString(),
        });
    }

    private static void AddMeasure(
        MetaDataWarehouseModel model,
        Fact fact,
        string name,
        string dataTypeId,
        int ordinal)
    {
        Add(model.FactMeasureList, new FactMeasure
        {
            Id = $"{fact.Id}:measure:{name}",
            Fact = fact,
            Name = name,
            DataTypeId = dataTypeId,
            Ordinal = ordinal.ToString(),
            IsNullable = "false",
        });
    }
}
