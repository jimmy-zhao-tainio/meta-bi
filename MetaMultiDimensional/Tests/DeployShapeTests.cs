using System.Data;
using System.Globalization;
using System.Reflection;
using Amo = Microsoft.AnalysisServices;
using MetaMultiDimensional.Core.Deploy;

namespace MetaMultiDimensional.Tests;

public sealed class DeployShapeTests
{
    [Fact]
    public void BuildDatabase_AddsDsvRelationsAndDefaultMeasure()
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();

        var database = BuildDatabase(model, root, "Commerce");

        var dataSourceView = Assert.Single(database.DataSourceViews.Cast<Amo.DataSourceView>());
        var relation = Assert.Single(dataSourceView.Schema.Relations.Cast<DataRelation>());
        Assert.Equal("Date", relation.ParentTable.TableName);
        Assert.Equal("DateKey", Assert.Single(relation.ParentColumns).ColumnName);
        Assert.Equal("Sales", relation.ChildTable.TableName);
        Assert.Equal("DateKey", Assert.Single(relation.ChildColumns).ColumnName);

        var cube = Assert.Single(database.Cubes.Cast<Amo.Cube>());
        Assert.Equal("[Measures].[Sales Amount]", cube.DefaultMeasure);
        var measure = Assert.Single(cube.MeasureGroups.Cast<Amo.MeasureGroup>()).Measures.Cast<Amo.Measure>().Single();
        Assert.Equal(Amo.MeasureDataType.Double, measure.DataType);
    }

    [Fact]
    public void BuildDatabase_EmitsModeSettings()
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var dimension = model.DimensionList.Single();
        var cube = model.CubeList.Single();
        var measureGroup = model.MeasureGroupList.Single();

        dimension.StorageMode = "Rolap";
        dimension.ProcessingMode = "LazyAggregations";
        dimension.ProcessingGroup = "ByTable";
        cube.StorageMode = "Rolap";
        cube.ProcessingMode = "LazyAggregations";
        measureGroup.StorageMode = "Rolap";
        measureGroup.ProcessingMode = "LazyAggregations";

        var database = BuildDatabase(model, root, "Commerce");

        var targetDimension = Assert.Single(database.Dimensions.Cast<Amo.Dimension>());
        Assert.Equal(Amo.DimensionStorageMode.Rolap, targetDimension.StorageMode);
        Assert.Equal(Amo.ProcessingMode.LazyAggregations, targetDimension.ProcessingMode);
        Assert.Equal(Amo.ProcessingGroup.ByTable, targetDimension.ProcessingGroup);

        var targetCube = Assert.Single(database.Cubes.Cast<Amo.Cube>());
        Assert.Equal(Amo.StorageMode.Rolap, targetCube.StorageMode);
        Assert.Equal(Amo.ProcessingMode.LazyAggregations, targetCube.ProcessingMode);

        var targetMeasureGroup = Assert.Single(targetCube.MeasureGroups.Cast<Amo.MeasureGroup>());
        Assert.Equal(Amo.StorageMode.Rolap, targetMeasureGroup.StorageMode);
        Assert.Equal(Amo.ProcessingMode.LazyAggregations, targetMeasureGroup.ProcessingMode);
    }

    [Fact]
    public void BuildDatabase_AppliesLargeStringStoresPolicy()
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();

        var database = BuildDatabase(model, root, "Commerce");

        Assert.Equal(1100, database.CompatibilityLevel);
        var targetDimension = Assert.Single(database.Dimensions.Cast<Amo.Dimension>());
        Assert.Equal(1100, targetDimension.StringStoresCompatibilityLevel);

        var targetPartition = database.Cubes
            .Cast<Amo.Cube>()
            .Single()
            .MeasureGroups
            .Cast<Amo.MeasureGroup>()
            .Single()
            .Partitions
            .Cast<Amo.Partition>()
            .Single();
        Assert.Equal(1100, targetPartition.StringStoresCompatibilityLevel);
    }

    [Fact]
    public void BuildDatabase_EmitsRoleMemberSids()
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var role = Add(model.SecurityRoleList, new SecurityRole
        {
            Id = "Reader",
            MultiDimensionalDatabase = root,
            Name = "Reader",
            Permission = "Allowed",
        });
        Add(model.RoleMemberList, new RoleMember
        {
            Id = "SalesReaders",
            SecurityRole = role,
            MemberName = "CONTOSO\\SalesReaders",
            MemberSid = "S-1-5-21-1000-2000-3000-4000",
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetRole = Assert.Single(database.Roles.Cast<Amo.Role>());
        var targetMember = Assert.Single(targetRole.Members.Cast<Amo.RoleMember>());
        Assert.Equal("CONTOSO\\SalesReaders", targetMember.Name);
        Assert.Equal("S-1-5-21-1000-2000-3000-4000", targetMember.Sid);
    }

    [Fact]
    public void BuildDatabase_EmitsHierarchiesAndAttributeRelationships()
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var dimension = model.DimensionList.Single();
        var year = AddDimensionAttribute(model, dimension, "CalendarYear", "Calendar Year");
        var month = AddDimensionAttribute(model, dimension, "MonthName", "Month");
        Add(model.AttributeRelationshipList, new AttributeRelationship
        {
            Id = "MonthToYear",
            ChildAttribute = month,
            ParentAttribute = year,
            RelationshipType = "Rigid",
        });
        var hierarchy = Add(model.DimensionHierarchyList, new DimensionHierarchy
        {
            Id = "Calendar",
            Dimension = dimension,
            Name = "Calendar",
            HierarchyType = "Natural",
        });
        Add(model.DimensionHierarchyLevelList, new DimensionHierarchyLevel
        {
            Id = "CalendarYearLevel",
            DimensionHierarchy = hierarchy,
            DimensionAttribute = year,
            Name = "Year",
            Ordinal = "10",
        });
        Add(model.DimensionHierarchyLevelList, new DimensionHierarchyLevel
        {
            Id = "CalendarMonthLevel",
            DimensionHierarchy = hierarchy,
            DimensionAttribute = month,
            Name = "Month",
            Ordinal = "20",
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetDimension = Assert.Single(database.Dimensions.Cast<Amo.Dimension>());
        var targetHierarchy = Assert.Single(targetDimension.Hierarchies.Cast<Amo.Hierarchy>());
        Assert.Equal("Calendar", targetHierarchy.ID);
        Assert.Equal("Calendar", targetHierarchy.Name);
        Assert.Equal(Amo.HierarchyStructureType.Natural, targetHierarchy.StructureType);

        var levels = targetHierarchy.Levels.Cast<Amo.Level>().ToArray();
        Assert.Equal(["CalendarYearLevel", "CalendarMonthLevel"], levels.Select(level => level.ID).ToArray());
        Assert.Equal(["CalendarYear", "MonthName"], levels.Select(level => level.SourceAttributeID).ToArray());

        var targetMonth = targetDimension.Attributes.Cast<Amo.DimensionAttribute>().Single(attribute => attribute.ID == "MonthName");
        var relationship = Assert.Single(targetMonth.AttributeRelationships.Cast<Amo.AttributeRelationship>());
        Assert.Equal("CalendarYear", relationship.AttributeID);
        Assert.Equal(Amo.RelationshipType.Rigid, relationship.RelationshipType);
    }

    [Fact]
    public void BuildDatabase_EmitsKpis()
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var cube = model.CubeList.Single();
        var measure = model.MeasureList.Single();
        Add(model.KpiList, new Kpi
        {
            Id = "SalesHealth",
            Cube = cube,
            AssociatedMeasure = measure,
            Name = "Sales Health",
            ValueExpression = "[Measures].[Sales Amount]",
            GoalExpression = "1000",
            StatusExpression = "IIF([Measures].[Sales Amount] >= 1000, 1, -1)",
            TrendExpression = "0",
            StatusGraphic = "Traffic Light",
            TrendGraphic = "Standard Arrow",
            Description = "Sales health KPI",
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetCube = Assert.Single(database.Cubes.Cast<Amo.Cube>());
        var targetKpi = Assert.Single(targetCube.Kpis.Cast<Amo.Kpi>());
        Assert.Equal("SalesHealth", targetKpi.ID);
        Assert.Equal("Sales Health", targetKpi.Name);
        Assert.Equal("Sales_measure_group", targetKpi.AssociatedMeasureGroupID);
        Assert.Equal("[Measures].[Sales Amount]", targetKpi.Value);
        Assert.Equal("1000", targetKpi.Goal);
        Assert.Equal("IIF([Measures].[Sales Amount] >= 1000, 1, -1)", targetKpi.Status);
        Assert.Equal("0", targetKpi.Trend);
        Assert.Equal("Traffic Light", targetKpi.StatusGraphic);
        Assert.Equal("Standard Arrow", targetKpi.TrendGraphic);
        Assert.Equal("Sales health KPI", targetKpi.Description);
    }

    [Fact]
    public void BuildDatabase_EmitsActionsWithAuthoredTarget()
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var cube = model.CubeList.Single();
        Add(model.CubeActionList, new CubeAction
        {
            Id = "SalesDetails",
            Cube = cube,
            Name = "Sales Details",
            ActionType = "Statement",
            TargetKind = "Cells",
            Target = "{[Measures].[Sales Amount]}",
            Expression = "1",
            Caption = "Details",
            Description = "Cell details",
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetCube = Assert.Single(database.Cubes.Cast<Amo.Cube>());
        var targetAction = Assert.Single(targetCube.Actions.Cast<Amo.Action>());
        Assert.Equal("SalesDetails", targetAction.ID);
        Assert.Equal("Sales Details", targetAction.Name);
        Assert.Equal(Amo.ActionType.Statement, targetAction.Type);
        Assert.Equal(Amo.ActionTargetType.Cells, targetAction.TargetType);
        Assert.Equal("{[Measures].[Sales Amount]}", targetAction.Target);
        Assert.Equal("1", Assert.IsType<Amo.StandardAction>(targetAction).Expression);
        Assert.Equal("Details", targetAction.Caption);
        Assert.Equal("Cell details", targetAction.Description);
    }

    [Fact]
    public void BuildDatabase_EmitsPerspectives()
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var cube = model.CubeList.Single();
        var cubeDimension = model.CubeDimensionList.Single();
        var salesMeasureGroup = model.MeasureGroupList.Single();
        var salesMeasure = model.MeasureList.Single();
        var inventoryMeasureGroup = Add(model.MeasureGroupList, new MeasureGroup
        {
            Id = "Inventory:measure-group",
            Cube = cube,
            Name = "Inventory",
            SourceName = "Inventory",
        });
        var quantityMeasure = Add(model.MeasureList, new Measure
        {
            Id = "Quantity",
            MeasureGroup = inventoryMeasureGroup,
            Name = "Quantity",
            SourceName = "Quantity",
            DataTypeId = "meta:type:Int32",
            AggregateFunction = "Sum",
        });
        var kpi = Add(model.KpiList, new Kpi
        {
            Id = "SalesHealth",
            Cube = cube,
            AssociatedMeasure = salesMeasure,
            Name = "Sales Health",
            ValueExpression = "[Measures].[Sales Amount]",
        });
        var calculation = Add(model.MdxCalculationList, new MdxCalculation
        {
            Id = "MarginCalculation",
            Cube = cube,
            Name = "[Measures].[Margin]",
            CalculationKind = "CalculatedMember",
            Expression = "CREATE MEMBER CURRENTCUBE.[Measures].[Margin] AS 1",
        });
        var namedSet = Add(model.NamedSetList, new NamedSet
        {
            Id = "TopDates",
            Cube = cube,
            Name = "Top Dates",
            Expression = "{[Date].[DateKey].Members}",
        });
        var action = Add(model.CubeActionList, new CubeAction
        {
            Id = "Details",
            Cube = cube,
            Name = "Details",
            ActionType = "Statement",
            TargetKind = "Cells",
            Expression = "1",
        });
        var perspective = Add(model.PerspectiveList, new Perspective
        {
            Id = "Executive",
            Cube = cube,
            Name = "Executive",
            DefaultMeasureName = "[Measures].[Sales Amount]",
            Description = "Executive slice",
        });
        Add(model.PerspectiveDimensionList, new PerspectiveDimension
        {
            Id = "ExecutiveDate",
            Perspective = perspective,
            CubeDimension = cubeDimension,
        });
        Add(model.PerspectiveMeasureGroupList, new PerspectiveMeasureGroup
        {
            Id = "ExecutiveSales",
            Perspective = perspective,
            MeasureGroup = salesMeasureGroup,
        });
        Add(model.PerspectiveMeasureList, new PerspectiveMeasure
        {
            Id = "ExecutiveQuantity",
            Perspective = perspective,
            Measure = quantityMeasure,
        });
        Add(model.PerspectiveCalculationList, new PerspectiveCalculation
        {
            Id = "ExecutiveMargin",
            Perspective = perspective,
            MdxCalculation = calculation,
        });
        Add(model.PerspectiveNamedSetList, new PerspectiveNamedSet
        {
            Id = "ExecutiveTopDates",
            Perspective = perspective,
            NamedSet = namedSet,
        });
        Add(model.PerspectiveKpiList, new PerspectiveKpi
        {
            Id = "ExecutiveSalesHealth",
            Perspective = perspective,
            Kpi = kpi,
        });
        Add(model.PerspectiveActionList, new PerspectiveAction
        {
            Id = "ExecutiveDetails",
            Perspective = perspective,
            CubeAction = action,
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetCube = Assert.Single(database.Cubes.Cast<Amo.Cube>());
        var targetPerspective = Assert.Single(targetCube.Perspectives.Cast<Amo.Perspective>());
        Assert.Equal("Executive", targetPerspective.ID);
        Assert.Equal("Executive", targetPerspective.Name);
        Assert.Equal("Executive slice", targetPerspective.Description);
        Assert.Equal("[Measures].[Sales Amount]", targetPerspective.DefaultMeasure);
        Assert.Equal("Date_cube_dimension", Assert.Single(targetPerspective.Dimensions.Cast<Amo.PerspectiveDimension>()).CubeDimensionID);

        var targetSalesGroup = targetPerspective.MeasureGroups
            .Cast<Amo.PerspectiveMeasureGroup>()
            .Single(group => group.MeasureGroupID == "Sales_measure_group");
        Assert.Empty(targetSalesGroup.Measures.Cast<Amo.PerspectiveMeasure>());

        var targetInventoryGroup = targetPerspective.MeasureGroups
            .Cast<Amo.PerspectiveMeasureGroup>()
            .Single(group => group.MeasureGroupID == "Inventory_measure_group");
        Assert.Equal("Quantity", Assert.Single(targetInventoryGroup.Measures.Cast<Amo.PerspectiveMeasure>()).MeasureID);

        Assert.Equal("SalesHealth", Assert.Single(targetPerspective.Kpis.Cast<Amo.PerspectiveKpi>()).KpiID);
        Assert.Equal("Details", Assert.Single(targetPerspective.Actions.Cast<Amo.PerspectiveAction>()).ActionID);

        var targetCalculations = targetPerspective.Calculations.Cast<Amo.PerspectiveCalculation>().ToArray();
        var targetMember = Assert.Single(targetCalculations, item => item.Name == "[Measures].[Margin]");
        Assert.Equal(Amo.PerspectiveCalculationType.Member, targetMember.Type);
        var targetSet = Assert.Single(targetCalculations, item => item.Name == "Top Dates");
        Assert.Equal(Amo.PerspectiveCalculationType.Set, targetSet.Type);
    }

    [Fact]
    public void BuildDatabase_EmitsTranslations()
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var cube = model.CubeList.Single();
        var dimension = model.DimensionList.Single();
        var attribute = model.DimensionAttributeList.Single();
        var measure = model.MeasureList.Single();
        var language = CultureInfo.GetCultureInfo("sv-SE").LCID;
        var culture = Add(model.CultureList, new Culture
        {
            Id = "sv-SE",
            MultiDimensionalDatabase = root,
            Name = "sv-SE",
        });
        var perspective = Add(model.PerspectiveList, new Perspective
        {
            Id = "Executive",
            Cube = cube,
            Name = "Executive",
        });
        var kpi = Add(model.KpiList, new Kpi
        {
            Id = "SalesHealth",
            Cube = cube,
            AssociatedMeasure = measure,
            Name = "Sales Health",
            ValueExpression = "[Measures].[Sales Amount]",
        });
        var action = Add(model.CubeActionList, new CubeAction
        {
            Id = "Details",
            Cube = cube,
            Name = "Details",
            ActionType = "Statement",
            TargetKind = "Cells",
            Expression = "1",
        });
        var namedSet = Add(model.NamedSetList, new NamedSet
        {
            Id = "TopDates",
            Cube = cube,
            Name = "Top Dates",
            Expression = "{[Date].[DateKey].Members}",
        });

        Add(model.CubeTranslationList, new CubeTranslation { Id = "CubeSv", Culture = culture, Cube = cube, Caption = "Handel", Description = "Cube" });
        Add(model.DimensionTranslationList, new DimensionTranslation { Id = "DimensionSv", Culture = culture, Dimension = dimension, Caption = "Datum", Description = "Dimension" });
        Add(model.AttributeTranslationList, new AttributeTranslation { Id = "AttributeSv", Culture = culture, DimensionAttribute = attribute, Caption = "Datumnyckel", Description = "Attribut" });
        Add(model.MeasureTranslationList, new MeasureTranslation { Id = "MeasureSv", Culture = culture, Measure = measure, Caption = "Forsaljning", Description = "Matt" });
        Add(model.PerspectiveTranslationList, new PerspectiveTranslation { Id = "PerspectiveSv", Culture = culture, Perspective = perspective, Caption = "Ledning", Description = "Perspektiv" });
        Add(model.KpiTranslationList, new KpiTranslation { Id = "KpiSv", Culture = culture, Kpi = kpi, Caption = "Forsaljningshalsa", Description = "KPI" });
        Add(model.ActionTranslationList, new ActionTranslation { Id = "ActionSv", Culture = culture, CubeAction = action, Caption = "Detaljer", Description = "Atgard" });
        Add(model.NamedSetTranslationList, new NamedSetTranslation { Id = "NamedSetSv", Culture = culture, NamedSet = namedSet, Caption = "Toppdatum", Description = "Namngiven mangd" });

        var database = BuildDatabase(model, root, "Commerce");

        var targetCube = Assert.Single(database.Cubes.Cast<Amo.Cube>());
        AssertTranslation(targetCube.Translations, language, "Handel", "Cube");

        var targetDimension = Assert.Single(database.Dimensions.Cast<Amo.Dimension>());
        AssertTranslation(targetDimension.Translations, language, "Datum", "Dimension");

        var targetAttribute = Assert.Single(targetDimension.Attributes.Cast<Amo.DimensionAttribute>());
        AssertAttributeTranslation(targetAttribute.Translations, language, "Datumnyckel", "Attribut");

        var targetMeasure = Assert.Single(targetCube.MeasureGroups.Cast<Amo.MeasureGroup>()).Measures.Cast<Amo.Measure>().Single();
        AssertTranslation(targetMeasure.Translations, language, "Forsaljning", "Matt");

        var targetPerspective = Assert.Single(targetCube.Perspectives.Cast<Amo.Perspective>());
        AssertTranslation(targetPerspective.Translations, language, "Ledning", "Perspektiv");

        var targetKpi = Assert.Single(targetCube.Kpis.Cast<Amo.Kpi>());
        AssertTranslation(targetKpi.Translations, language, "Forsaljningshalsa", "KPI");

        var targetAction = Assert.Single(targetCube.Actions.Cast<Amo.Action>());
        AssertTranslation(targetAction.Translations, language, "Detaljer", "Atgard");

        var targetNamedSet = Assert.Single(
            Assert.Single(targetCube.MdxScripts.Cast<Amo.MdxScript>()).CalculationProperties.Cast<Amo.CalculationProperty>(),
            property => property.CalculationReference == "Top Dates");
        AssertTranslation(targetNamedSet.Translations, language, "Toppdatum", "Namngiven mangd");
    }

    [Fact]
    public void BuildDatabase_EmitsDimensionPermissions()
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var dimension = model.DimensionList.Single();
        var attribute = model.DimensionAttributeList.Single();
        var role = Add(model.SecurityRoleList, new SecurityRole
        {
            Id = "Reader",
            MultiDimensionalDatabase = root,
            Name = "Reader",
            Permission = "Allowed",
        });
        Add(model.DimensionPermissionList, new DimensionPermission
        {
            Id = "ReaderDate",
            SecurityRole = role,
            Dimension = dimension,
            DimensionAttribute = attribute,
            AllowedSetExpression = "{[Date].[DateKey].Members}",
            DeniedSetExpression = "{}",
            DefaultMemberExpression = "[Date].[DateKey].DefaultMember",
            VisualTotals = "1",
            Description = "Date member security",
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetDimension = Assert.Single(database.Dimensions.Cast<Amo.Dimension>());
        var permission = Assert.Single(targetDimension.DimensionPermissions.Cast<Amo.DimensionPermission>());
        Assert.Equal("Reader", permission.RoleID);

        var attributePermission = Assert.Single(permission.AttributePermissions.Cast<Amo.AttributePermission>());
        Assert.Equal("DateKey", attributePermission.AttributeID);
        Assert.Equal("{[Date].[DateKey].Members}", attributePermission.AllowedSet);
        Assert.Equal("{}", attributePermission.DeniedSet);
        Assert.Equal("[Date].[DateKey].DefaultMember", attributePermission.DefaultMember);
        Assert.Equal("1", attributePermission.VisualTotals);
        Assert.Equal("Date member security", attributePermission.Description);
    }

    [Theory]
    [InlineData("DimensionType", "Dimension.DimensionType")]
    [InlineData("DimensionStorageMode", "Dimension.StorageMode")]
    [InlineData("DimensionProcessingMode", "Dimension.ProcessingMode")]
    [InlineData("DimensionProcessingGroup", "Dimension.ProcessingGroup")]
    [InlineData("CubeStorageMode", "Cube.StorageMode")]
    [InlineData("CubeProcessingMode", "Cube.ProcessingMode")]
    [InlineData("MeasureGroupStorageMode", "MeasureGroup.StorageMode")]
    [InlineData("MeasureGroupProcessingMode", "MeasureGroup.ProcessingMode")]
    [InlineData("AttributeUsage", "DimensionAttribute.Usage")]
    [InlineData("AggregateFunction", "Measure.AggregateFunction")]
    [InlineData("PartitionStorageMode", "Partition.StorageMode")]
    [InlineData("PartitionProcessingMode", "Partition.ProcessingMode")]
    [InlineData("ActionType", "CubeAction.ActionType")]
    [InlineData("ActionTargetKind", "CubeAction.TargetKind")]
    [InlineData("Permission", "SecurityRole.Permission")]
    [InlineData("AttributeRelationshipType", "AttributeRelationship.RelationshipType")]
    [InlineData("DimensionHierarchyType", "DimensionHierarchy.HierarchyType")]
    public void BuildDatabase_RejectsInvalidTargetValues(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var dimension = model.DimensionList.Single();
        var attribute = model.DimensionAttributeList.Single();
        var measure = model.MeasureList.Single();
        var measureGroup = model.MeasureGroupList.Single();
        var partition = model.PartitionList.Single();
        var cube = model.CubeList.Single();

        switch (scenario)
        {
            case "DimensionType":
                dimension.DimensionType = "Calendarish";
                break;
            case "DimensionStorageMode":
                dimension.StorageMode = "MemoryOnly";
                break;
            case "DimensionProcessingMode":
                dimension.ProcessingMode = "Whenever";
                break;
            case "DimensionProcessingGroup":
                dimension.ProcessingGroup = "ByMood";
                break;
            case "CubeStorageMode":
                cube.StorageMode = "MemoryOnly";
                break;
            case "CubeProcessingMode":
                cube.ProcessingMode = "Whenever";
                break;
            case "MeasureGroupStorageMode":
                measureGroup.StorageMode = "MemoryOnly";
                break;
            case "MeasureGroupProcessingMode":
                measureGroup.ProcessingMode = "Whenever";
                break;
            case "AttributeUsage":
                attribute.Usage = "AlmostKey";
                attribute.IsKey = "false";
                break;
            case "AggregateFunction":
                measure.AggregateFunction = "RollupMagic";
                break;
            case "PartitionStorageMode":
                partition.StorageMode = "MemoryOnly";
                break;
            case "PartitionProcessingMode":
                partition.ProcessingMode = "Whenever";
                break;
            case "ActionType":
                Add(model.CubeActionList, new CubeAction
                {
                    Id = "Details",
                    Cube = cube,
                    Name = "Details",
                    ActionType = "Teleport",
                    TargetKind = "Cells",
                    Expression = "1",
                });
                break;
            case "ActionTargetKind":
                Add(model.CubeActionList, new CubeAction
                {
                    Id = "Details",
                    Cube = cube,
                    Name = "Details",
                    ActionType = "Statement",
                    TargetKind = "Somewhere",
                    Expression = "1",
                });
                break;
            case "Permission":
                Add(model.SecurityRoleList, new SecurityRole
                {
                    Id = "Reader",
                    MultiDimensionalDatabase = root,
                    Name = "Reader",
                    Permission = "CanPeek",
                });
                break;
            case "AttributeRelationshipType":
                var parentAttribute = AddDimensionAttribute(model, dimension, "CalendarYear", "Calendar Year");
                Add(model.AttributeRelationshipList, new AttributeRelationship
                {
                    Id = "BadRelationshipType",
                    ChildAttribute = attribute,
                    ParentAttribute = parentAttribute,
                    RelationshipType = "Sticky",
                });
                break;
            case "DimensionHierarchyType":
                var hierarchyAttribute = AddDimensionAttribute(model, dimension, "CalendarYear", "Calendar Year");
                var hierarchy = Add(model.DimensionHierarchyList, new DimensionHierarchy
                {
                    Id = "BadHierarchyType",
                    Dimension = dimension,
                    Name = "Calendar",
                    HierarchyType = "Pretty",
                });
                Add(model.DimensionHierarchyLevelList, new DimensionHierarchyLevel
                {
                    Id = "CalendarYearLevel",
                    DimensionHierarchy = hierarchy,
                    DimensionAttribute = hierarchyAttribute,
                    Name = "Year",
                    Ordinal = "10",
                });
                break;
        }

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("EmptyHierarchy", "must contain at least one level")]
    [InlineData("CrossDimensionHierarchyLevel", "outside hierarchy dimension")]
    [InlineData("DuplicateHierarchyLevelOrdinal", "duplicate level ordinal")]
    [InlineData("CrossDimensionAttributeRelationship", "attributes from different dimensions")]
    [InlineData("SelfAttributeRelationship", "same child and parent attribute")]
    [InlineData("DuplicateAttributeRelationship", "already has an attribute relationship")]
    public void BuildDatabase_RejectsInvalidHierarchyAndRelationshipShape(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var dimension = model.DimensionList.Single();
        var dateKey = model.DimensionAttributeList.Single();

        switch (scenario)
        {
            case "EmptyHierarchy":
                Add(model.DimensionHierarchyList, new DimensionHierarchy
                {
                    Id = "Calendar",
                    Dimension = dimension,
                    Name = "Calendar",
                });
                break;
            case "CrossDimensionHierarchyLevel":
                var productAttribute = AddProductAttribute(model, root);
                var crossHierarchy = Add(model.DimensionHierarchyList, new DimensionHierarchy
                {
                    Id = "Calendar",
                    Dimension = dimension,
                    Name = "Calendar",
                });
                Add(model.DimensionHierarchyLevelList, new DimensionHierarchyLevel
                {
                    Id = "ProductLevel",
                    DimensionHierarchy = crossHierarchy,
                    DimensionAttribute = productAttribute,
                    Name = "Product",
                    Ordinal = "10",
                });
                break;
            case "DuplicateHierarchyLevelOrdinal":
                var year = AddDimensionAttribute(model, dimension, "CalendarYear", "Calendar Year");
                var month = AddDimensionAttribute(model, dimension, "MonthName", "Month");
                var duplicateHierarchy = Add(model.DimensionHierarchyList, new DimensionHierarchy
                {
                    Id = "Calendar",
                    Dimension = dimension,
                    Name = "Calendar",
                });
                Add(model.DimensionHierarchyLevelList, new DimensionHierarchyLevel
                {
                    Id = "CalendarYearLevel",
                    DimensionHierarchy = duplicateHierarchy,
                    DimensionAttribute = year,
                    Name = "Year",
                    Ordinal = "10",
                });
                Add(model.DimensionHierarchyLevelList, new DimensionHierarchyLevel
                {
                    Id = "CalendarMonthLevel",
                    DimensionHierarchy = duplicateHierarchy,
                    DimensionAttribute = month,
                    Name = "Month",
                    Ordinal = "10",
                });
                break;
            case "CrossDimensionAttributeRelationship":
                Add(model.AttributeRelationshipList, new AttributeRelationship
                {
                    Id = "DateToProduct",
                    ChildAttribute = dateKey,
                    ParentAttribute = AddProductAttribute(model, root),
                });
                break;
            case "SelfAttributeRelationship":
                Add(model.AttributeRelationshipList, new AttributeRelationship
                {
                    Id = "DateToDate",
                    ChildAttribute = dateKey,
                    ParentAttribute = dateKey,
                });
                break;
            case "DuplicateAttributeRelationship":
                var parent = AddDimensionAttribute(model, dimension, "CalendarYear", "Calendar Year");
                Add(model.AttributeRelationshipList, new AttributeRelationship
                {
                    Id = "DateToYear1",
                    ChildAttribute = dateKey,
                    ParentAttribute = parent,
                });
                Add(model.AttributeRelationshipList, new AttributeRelationship
                {
                    Id = "DateToYear2",
                    ChildAttribute = dateKey,
                    ParentAttribute = parent,
                });
                break;
        }

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("MissingCube", "references cube")]
    [InlineData("CrossCubeAssociatedMeasure", "must belong to the KPI cube")]
    public void BuildDatabase_RejectsInvalidKpis(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var cube = model.CubeList.Single();
        var measure = model.MeasureList.Single();

        switch (scenario)
        {
            case "MissingCube":
                cube = new Cube
                {
                    Id = "DetachedCube",
                    MultiDimensionalDatabase = root,
                    Name = "Detached",
                };
                break;
            case "CrossCubeAssociatedMeasure":
                var otherCube = Add(model.CubeList, new Cube
                {
                    Id = "OtherCube",
                    MultiDimensionalDatabase = root,
                    Name = "Other",
                });
                var otherGroup = Add(model.MeasureGroupList, new MeasureGroup
                {
                    Id = "OtherGroup",
                    Cube = otherCube,
                    Name = "Other",
                });
                measure = Add(model.MeasureList, new Measure
                {
                    Id = "OtherMeasure",
                    MeasureGroup = otherGroup,
                    Name = "Other Measure",
                    DataTypeId = "meta:type:Decimal",
                    AggregateFunction = "Sum",
                });
                break;
        }

        Add(model.KpiList, new Kpi
        {
            Id = "SalesHealth",
            Cube = cube,
            AssociatedMeasure = measure,
            Name = "Sales Health",
            ValueExpression = "[Measures].[Sales Amount]",
        });

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("MissingPerspectiveCube", "references cube")]
    [InlineData("CrossCubeDimension", "outside perspective cube")]
    [InlineData("MissingMeasure", "references measure")]
    [InlineData("DuplicateDimension", "duplicate dimension membership")]
    [InlineData("DuplicateMeasure", "duplicate measure membership")]
    [InlineData("CrossCubeKpi", "outside perspective cube")]
    [InlineData("MissingAction", "references action")]
    [InlineData("InvalidCalculationKind", "MdxCalculation.CalculationKind")]
    public void BuildDatabase_RejectsInvalidPerspectives(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var cube = model.CubeList.Single();
        var cubeDimension = model.CubeDimensionList.Single();
        var measure = model.MeasureList.Single();
        var perspectiveCube = cube;

        if (scenario == "MissingPerspectiveCube")
        {
            perspectiveCube = new Cube
            {
                Id = "DetachedCube",
                MultiDimensionalDatabase = root,
                Name = "Detached",
            };
        }

        var perspective = Add(model.PerspectiveList, new Perspective
        {
            Id = "Executive",
            Cube = perspectiveCube,
            Name = "Executive",
        });

        switch (scenario)
        {
            case "CrossCubeDimension":
                var otherCube = Add(model.CubeList, new Cube
                {
                    Id = "OtherCube",
                    MultiDimensionalDatabase = root,
                    Name = "Other",
                });
                var otherCubeDimension = Add(model.CubeDimensionList, new CubeDimension
                {
                    Id = "OtherDate",
                    Cube = otherCube,
                    Dimension = model.DimensionList.Single(),
                    Name = "Other Date",
                });
                Add(model.PerspectiveDimensionList, new PerspectiveDimension
                {
                    Id = "CrossDate",
                    Perspective = perspective,
                    CubeDimension = otherCubeDimension,
                });
                break;
            case "MissingMeasure":
                Add(model.PerspectiveMeasureList, new PerspectiveMeasure
                {
                    Id = "DetachedMeasureMembership",
                    Perspective = perspective,
                    Measure = new Measure
                    {
                        Id = "DetachedMeasure",
                        MeasureGroup = model.MeasureGroupList.Single(),
                        Name = "Detached",
                    },
                });
                break;
            case "DuplicateDimension":
                Add(model.PerspectiveDimensionList, new PerspectiveDimension
                {
                    Id = "ExecutiveDate1",
                    Perspective = perspective,
                    CubeDimension = cubeDimension,
                });
                Add(model.PerspectiveDimensionList, new PerspectiveDimension
                {
                    Id = "ExecutiveDate2",
                    Perspective = perspective,
                    CubeDimension = cubeDimension,
                });
                break;
            case "DuplicateMeasure":
                Add(model.PerspectiveMeasureList, new PerspectiveMeasure
                {
                    Id = "ExecutiveMeasure1",
                    Perspective = perspective,
                    Measure = measure,
                });
                Add(model.PerspectiveMeasureList, new PerspectiveMeasure
                {
                    Id = "ExecutiveMeasure2",
                    Perspective = perspective,
                    Measure = measure,
                });
                break;
            case "CrossCubeKpi":
                var kpiCube = Add(model.CubeList, new Cube
                {
                    Id = "KpiCube",
                    MultiDimensionalDatabase = root,
                    Name = "KPI Cube",
                });
                var kpi = Add(model.KpiList, new Kpi
                {
                    Id = "OtherHealth",
                    Cube = kpiCube,
                    Name = "Other Health",
                    ValueExpression = "1",
                });
                Add(model.PerspectiveKpiList, new PerspectiveKpi
                {
                    Id = "CrossKpi",
                    Perspective = perspective,
                    Kpi = kpi,
                });
                break;
            case "MissingAction":
                Add(model.PerspectiveActionList, new PerspectiveAction
                {
                    Id = "MissingActionMembership",
                    Perspective = perspective,
                    CubeAction = new CubeAction
                    {
                        Id = "DetachedAction",
                        Cube = cube,
                        Name = "Detached",
                        ActionType = "Statement",
                        TargetKind = "Cells",
                        Expression = "1",
                    },
                });
                break;
            case "InvalidCalculationKind":
                var calculation = Add(model.MdxCalculationList, new MdxCalculation
                {
                    Id = "BadCalculation",
                    Cube = cube,
                    Name = "[Measures].[Bad]",
                    CalculationKind = "Magic",
                    Expression = "CREATE MEMBER CURRENTCUBE.[Measures].[Bad] AS 1",
                });
                Add(model.PerspectiveCalculationList, new PerspectiveCalculation
                {
                    Id = "BadCalculationMembership",
                    Perspective = perspective,
                    MdxCalculation = calculation,
                });
                break;
        }

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("MissingCulture", "references culture")]
    [InlineData("DuplicateCulture", "more than one Culture row")]
    [InlineData("DuplicateMeasureTranslation", "duplicates translation language")]
    [InlineData("MissingMeasure", "references measure")]
    [InlineData("InvalidCultureLanguageId", "Culture.LanguageId")]
    public void BuildDatabase_RejectsInvalidTranslations(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var measure = model.MeasureList.Single();
        var culture = Add(model.CultureList, new Culture
        {
            Id = "sv-SE",
            MultiDimensionalDatabase = root,
            Name = "sv-SE",
        });

        switch (scenario)
        {
            case "MissingCulture":
                culture = new Culture
                {
                    Id = "DetachedCulture",
                    MultiDimensionalDatabase = root,
                    Name = "sv-SE",
                };
                Add(model.MeasureTranslationList, new MeasureTranslation
                {
                    Id = "MissingCultureTranslation",
                    Culture = culture,
                    Measure = measure,
                    Caption = "Forsaljning",
                });
                break;
            case "DuplicateCulture":
                Add(model.CultureList, new Culture
                {
                    Id = "sv-SE-duplicate",
                    MultiDimensionalDatabase = root,
                    Name = "Swedish",
                    LanguageId = "1053",
                });
                break;
            case "DuplicateMeasureTranslation":
                Add(model.MeasureTranslationList, new MeasureTranslation
                {
                    Id = "MeasureSv1",
                    Culture = culture,
                    Measure = measure,
                    Caption = "Forsaljning",
                });
                Add(model.MeasureTranslationList, new MeasureTranslation
                {
                    Id = "MeasureSv2",
                    Culture = culture,
                    Measure = measure,
                    Caption = "Omsattning",
                });
                break;
            case "MissingMeasure":
                Add(model.MeasureTranslationList, new MeasureTranslation
                {
                    Id = "MissingMeasureTranslation",
                    Culture = culture,
                    Measure = new Measure
                    {
                        Id = "DetachedMeasure",
                        MeasureGroup = model.MeasureGroupList.Single(),
                        Name = "Detached",
                    },
                    Caption = "Saknas",
                });
                break;
            case "InvalidCultureLanguageId":
                culture.LanguageId = "-1";
                Add(model.MeasureTranslationList, new MeasureTranslation
                {
                    Id = "InvalidCultureTranslation",
                    Culture = culture,
                    Measure = measure,
                    Caption = "Forsaljning",
                });
                break;
        }

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("MissingRole", "references role")]
    [InlineData("MissingDimension", "references dimension")]
    [InlineData("MissingAttribute", "references attribute")]
    [InlineData("CrossDimensionAttribute", "outside dimension")]
    [InlineData("DuplicateAttribute", "duplicates attribute permission")]
    public void BuildDatabase_RejectsInvalidDimensionPermissions(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.MultiDimensionalDatabaseList.Single();
        var role = Add(model.SecurityRoleList, new SecurityRole
        {
            Id = "Reader",
            MultiDimensionalDatabase = root,
            Name = "Reader",
            Permission = "Allowed",
        });
        var dimension = model.DimensionList.Single();
        var attribute = model.DimensionAttributeList.Single();

        switch (scenario)
        {
            case "MissingRole":
                role = new SecurityRole
                {
                    Id = "DetachedRole",
                    MultiDimensionalDatabase = root,
                    Name = "Detached",
                    Permission = "Allowed",
                };
                break;
            case "MissingDimension":
                dimension = new Dimension
                {
                    Id = "DetachedDimension",
                    MultiDimensionalDatabase = root,
                    Name = "Detached",
                };
                break;
            case "MissingAttribute":
                attribute = new DimensionAttribute
                {
                    Id = "DetachedAttribute",
                    Dimension = dimension,
                    Name = "Detached",
                };
                break;
            case "CrossDimensionAttribute":
                attribute = AddProductAttribute(model, root);
                break;
            case "DuplicateAttribute":
                Add(model.DimensionPermissionList, new DimensionPermission
                {
                    Id = "ReaderDate1",
                    SecurityRole = role,
                    Dimension = dimension,
                    DimensionAttribute = attribute,
                    AllowedSetExpression = "{[Date].[DateKey].Members}",
                });
                break;
        }

        Add(model.DimensionPermissionList, new DimensionPermission
        {
            Id = "ReaderDate2",
            SecurityRole = role,
            Dimension = dimension,
            DimensionAttribute = attribute,
            AllowedSetExpression = "{[Date].[DateKey].Members}",
        });

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    private static Amo.Database BuildDatabase(MetaMultiDimensionalModel model, MultiDimensionalDatabase root, string databaseName)
    {
        var method = typeof(MetaMultiDimensionalDeployService).GetMethod(
            "BuildDatabase",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        return Assert.IsType<Amo.Database>(method.Invoke(null, new object[] { model, root, databaseName }));
    }

    private static InvalidOperationException AssertBuildFails(MetaMultiDimensionalModel model, MultiDimensionalDatabase root)
    {
        var method = typeof(MetaMultiDimensionalDeployService).GetMethod(
            "BuildDatabase",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var exception = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { model, root, "Commerce" }));
        return Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    private static void AssertTranslation(
        Amo.TranslationCollection translations,
        int language,
        string caption,
        string description)
    {
        var translation = Assert.Single(translations.Cast<Amo.Translation>());
        Assert.Equal(language, translation.Language);
        Assert.Equal(caption, translation.Caption);
        Assert.Equal(description, translation.Description);
    }

    private static void AssertAttributeTranslation(
        Amo.AttributeTranslationCollection translations,
        int language,
        string caption,
        string description)
    {
        var translation = Assert.Single(translations.Cast<Amo.AttributeTranslation>());
        Assert.Equal(language, translation.Language);
        Assert.Equal(caption, translation.Caption);
        Assert.Equal(description, translation.Description);
    }

    private static MetaMultiDimensionalModel CreateModel()
    {
        var model = MetaMultiDimensionalModel.CreateEmpty();
        var database = Add(model.MultiDimensionalDatabaseList, new MultiDimensionalDatabase
        {
            Id = "Commerce",
            Name = "Commerce",
            DefaultLanguage = "en-US",
        });
        var dataSource = Add(model.MultiDimensionalDataSourceList, new MultiDimensionalDataSource
        {
            Id = "Warehouse",
            MultiDimensionalDatabase = database,
            Name = "Warehouse",
            Provider = "SqlServer",
        });
        var dimension = Add(model.DimensionList, new Dimension
        {
            Id = "Date",
            MultiDimensionalDatabase = database,
            Name = "Date",
            SourceName = "Date",
        });
        var dateKey = Add(model.DimensionAttributeList, new DimensionAttribute
        {
            Id = "DateKey",
            Dimension = dimension,
            Name = "DateKey",
            SourceName = "DateKey",
            DataTypeId = "meta:type:String",
            IsKey = "true",
        });
        var cube = Add(model.CubeList, new Cube
        {
            Id = "Commerce:cube",
            MultiDimensionalDatabase = database,
            Name = "Commerce",
        });
        var cubeDimension = Add(model.CubeDimensionList, new CubeDimension
        {
            Id = "Date:cube-dimension",
            Cube = cube,
            Dimension = dimension,
            Name = "Date",
        });
        var measureGroup = Add(model.MeasureGroupList, new MeasureGroup
        {
            Id = "Sales:measure-group",
            Cube = cube,
            Name = "Sales",
            SourceName = "Sales",
        });
        Add(model.MeasureList, new Measure
        {
            Id = "SalesAmount",
            MeasureGroup = measureGroup,
            Name = "Sales Amount",
            SourceName = "SalesAmount",
            DataTypeId = "meta:type:Decimal",
            AggregateFunction = "Sum",
        });
        Add(model.DimensionUsageList, new DimensionUsage
        {
            Id = "SalesOrderDate",
            MeasureGroup = measureGroup,
            CubeDimension = cubeDimension,
            GranularityAttribute = dateKey,
            RoleName = "OrderDate",
        });
        Add(model.PartitionList, new Partition
        {
            Id = "SalesCurrent",
            MeasureGroup = measureGroup,
            MultiDimensionalDataSource = dataSource,
            Name = "Sales Current",
            SourceExpression = "SELECT DateKey, SalesAmount FROM dbo.Sales",
        });

        return model;
    }

    private static DimensionAttribute AddDimensionAttribute(
        MetaMultiDimensionalModel model,
        Dimension dimension,
        string id,
        string name)
    {
        return Add(model.DimensionAttributeList, new DimensionAttribute
        {
            Id = id,
            Dimension = dimension,
            Name = name,
            SourceName = id,
            DataTypeId = "meta:type:Int32",
            IsKey = "false",
        });
    }

    private static DimensionAttribute AddProductAttribute(MetaMultiDimensionalModel model, MultiDimensionalDatabase root)
    {
        var dimension = Add(model.DimensionList, new Dimension
        {
            Id = "Product",
            MultiDimensionalDatabase = root,
            Name = "Product",
            SourceName = "Product",
        });

        return AddDimensionAttribute(model, dimension, "ProductKey", "Product Key");
    }

    private static T Add<T>(ICollection<T> rows, T row)
    {
        rows.Add(row);
        return row;
    }
}
