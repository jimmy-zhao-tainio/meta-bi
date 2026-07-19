using System.Reflection;
using Tom = Microsoft.AnalysisServices.Tabular;
using MetaTabular.Core.Deploy;

namespace MetaTabular.Tests;

public sealed class DeployShapeTests
{
    [Fact]
    public void BuildDatabase_EmitsSortByColumns()
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var monthName = model.TabularColumnList.Single();
        var monthNumber = Add(model.TabularColumnList, new TabularColumn
        {
            Id = "MonthNumber",
            TabularTable = table,
            Name = "MonthNumber",
            DataTypeId = "meta:type:Int32",
            Ordinal = "20",
        });
        Add(model.TabularSortByColumnList, new TabularSortByColumn
        {
            Id = "MonthNameSort",
            SourceColumn = monthName,
            SortColumn = monthNumber,
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetTable = Assert.Single(database.Model.Tables.Cast<Tom.Table>());
        var targetMonthName = Assert.Single(targetTable.Columns.Cast<Tom.Column>(), column => column.Name == "SalesAmount");
        var targetMonthNumber = Assert.Single(targetTable.Columns.Cast<Tom.Column>(), column => column.Name == "MonthNumber");
        Assert.Same(targetMonthNumber, targetMonthName.SortByColumn);
    }

    [Fact]
    public void BuildDatabase_EmitsHierarchies()
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var yearColumn = model.TabularColumnList.Single();
        var monthColumn = Add(model.TabularColumnList, new TabularColumn
        {
            Id = "MonthName",
            TabularTable = table,
            Name = "Month Name",
            DataTypeId = "meta:type:String",
            Ordinal = "20",
        });
        var hierarchy = Add(model.TabularHierarchyList, new TabularHierarchy
        {
            Id = "Calendar",
            TabularTable = table,
            Name = "Calendar",
            DisplayFolder = "Date",
            IsHidden = "true",
            Description = "Calendar navigation",
        });
        Add(model.TabularHierarchyLevelList, new TabularHierarchyLevel
        {
            Id = "CalendarMonth",
            TabularHierarchy = hierarchy,
            TabularColumn = monthColumn,
            Name = "Month",
            Ordinal = "20",
        });
        Add(model.TabularHierarchyLevelList, new TabularHierarchyLevel
        {
            Id = "CalendarYear",
            TabularHierarchy = hierarchy,
            TabularColumn = yearColumn,
            Name = "Year",
            Ordinal = "10",
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetTable = Assert.Single(database.Model.Tables.Cast<Tom.Table>());
        var targetHierarchy = Assert.Single(targetTable.Hierarchies.Cast<Tom.Hierarchy>());
        Assert.Equal("Calendar", targetHierarchy.Name);
        Assert.Equal("Date", targetHierarchy.DisplayFolder);
        Assert.True(targetHierarchy.IsHidden);
        Assert.Equal("Calendar navigation", targetHierarchy.Description);

        var targetLevels = targetHierarchy.Levels.Cast<Tom.Level>().ToArray();
        Assert.Collection(
            targetLevels,
            level =>
            {
                Assert.Equal("Year", level.Name);
                Assert.Equal(0, level.Ordinal);
                Assert.Equal("SalesAmount", level.Column.Name);
            },
            level =>
            {
                Assert.Equal("Month", level.Name);
                Assert.Equal(1, level.Ordinal);
                Assert.Equal("Month Name", level.Column.Name);
            });
    }

    [Fact]
    public void BuildDatabase_EmitsPerspectives()
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var salesTable = model.TabularTableList.Single();
        var salesAmountColumn = model.TabularColumnList.Single();
        var monthColumn = Add(model.TabularColumnList, new TabularColumn
        {
            Id = "MonthName",
            TabularTable = salesTable,
            Name = "Month",
            DataTypeId = "meta:type:String",
            Ordinal = "20",
        });
        var measure = Add(model.TabularMeasureList, new TabularMeasure
        {
            Id = "SalesAmountMeasure",
            TabularTable = salesTable,
            Name = "Sales Amount",
            Expression = "SUM('Sales'[SalesAmount])",
        });
        var hierarchy = Add(model.TabularHierarchyList, new TabularHierarchy
        {
            Id = "Calendar",
            TabularTable = salesTable,
            Name = "Calendar",
        });
        AddHierarchyLevel(model, hierarchy, monthColumn, "Month", "10");
        var productTable = Add(model.TabularTableList, new TabularTable
        {
            Id = "Product",
            TabularModel = root,
            Name = "Product",
        });
        var calculationGroup = Add(model.TabularCalculationGroupList, new TabularCalculationGroup
        {
            Id = "TimeIntelligence",
            TabularModel = root,
            Name = "Time Intelligence",
            Precedence = "10",
        });
        var perspective = Add(model.TabularPerspectiveList, new TabularPerspective
        {
            Id = "SalesPerspective",
            TabularModel = root,
            Name = "Sales",
            Description = "Sales browsing surface",
        });
        Add(model.TabularPerspectiveColumnList, new TabularPerspectiveColumn
        {
            Id = "SalesAmountColumnPerspective",
            TabularPerspective = perspective,
            TabularColumn = salesAmountColumn,
        });
        Add(model.TabularPerspectiveHierarchyList, new TabularPerspectiveHierarchy
        {
            Id = "CalendarPerspective",
            TabularPerspective = perspective,
            TabularHierarchy = hierarchy,
        });
        Add(model.TabularPerspectiveMeasureList, new TabularPerspectiveMeasure
        {
            Id = "SalesAmountMeasurePerspective",
            TabularPerspective = perspective,
            TabularMeasure = measure,
        });
        Add(model.TabularPerspectiveTableList, new TabularPerspectiveTable
        {
            Id = "ProductTablePerspective",
            TabularPerspective = perspective,
            TabularTable = productTable,
        });
        Add(model.TabularPerspectiveCalculationGroupList, new TabularPerspectiveCalculationGroup
        {
            Id = "TimeIntelligencePerspective",
            TabularPerspective = perspective,
            TabularCalculationGroup = calculationGroup,
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetPerspective = Assert.Single(database.Model.Perspectives.Cast<Tom.Perspective>());
        Assert.Equal("Sales", targetPerspective.Name);
        Assert.Equal("Sales browsing surface", targetPerspective.Description);

        var perspectiveTables = targetPerspective.PerspectiveTables.Cast<Tom.PerspectiveTable>().ToArray();
        Assert.Equal(3, perspectiveTables.Length);

        var salesPerspectiveTable = Assert.Single(perspectiveTables, table => table.Table.Name == "Sales");
        Assert.False(salesPerspectiveTable.IncludeAll);
        Assert.Same(
            database.Model.Tables["Sales"].Columns["SalesAmount"],
            Assert.Single(salesPerspectiveTable.PerspectiveColumns.Cast<Tom.PerspectiveColumn>()).Column);
        Assert.Same(
            database.Model.Tables["Sales"].Hierarchies["Calendar"],
            Assert.Single(salesPerspectiveTable.PerspectiveHierarchies.Cast<Tom.PerspectiveHierarchy>()).Hierarchy);
        Assert.Same(
            database.Model.Tables["Sales"].Measures["Sales Amount"],
            Assert.Single(salesPerspectiveTable.PerspectiveMeasures.Cast<Tom.PerspectiveMeasure>()).Measure);

        var productPerspectiveTable = Assert.Single(perspectiveTables, table => table.Table.Name == "Product");
        Assert.True(productPerspectiveTable.IncludeAll);

        var calculationGroupPerspectiveTable = Assert.Single(perspectiveTables, table => table.Table.Name == "Time Intelligence");
        Assert.True(calculationGroupPerspectiveTable.IncludeAll);
    }

    [Fact]
    public void BuildDatabase_EmitsKpis()
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var baseMeasure = Add(model.TabularMeasureList, new TabularMeasure
        {
            Id = "SalesAmountMeasure",
            TabularTable = table,
            Name = "Sales Amount",
            Expression = "SUM('Sales'[SalesAmount])",
        });
        var targetMeasure = Add(model.TabularMeasureList, new TabularMeasure
        {
            Id = "SalesTargetMeasure",
            TabularTable = table,
            Name = "Sales Target",
            Expression = "SUM('Sales'[SalesTarget])",
        });
        Add(model.TabularKpiList, new TabularKpi
        {
            Id = "SalesKpi",
            Description = "Sales health",
            BaseMeasure = baseMeasure,
            TargetMeasure = targetMeasure,
            StatusExpression = "IF([Sales Amount] >= [Sales Target], 1, -1)",
            StatusGraphic = "Traffic Lights",
            TrendExpression = "1",
            TrendGraphic = "Standard Arrow",
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetKpi = database.Model.Tables["Sales"].Measures["Sales Amount"].KPI;
        Assert.NotNull(targetKpi);
        Assert.Equal("Sales health", targetKpi.Description);
        Assert.Equal("[Sales Target]", targetKpi.TargetExpression);
        Assert.Equal("IF([Sales Amount] >= [Sales Target], 1, -1)", targetKpi.StatusExpression);
        Assert.Equal("Traffic Lights", targetKpi.StatusGraphic);
        Assert.Equal("1", targetKpi.TrendExpression);
        Assert.Equal("Standard Arrow", targetKpi.TrendGraphic);
    }

    [Fact]
    public void BuildDatabase_EmitsKpiPerspectiveMembership()
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var measure = Add(model.TabularMeasureList, new TabularMeasure
        {
            Id = "SalesAmountMeasure",
            TabularTable = table,
            Name = "Sales Amount",
            Expression = "SUM('Sales'[SalesAmount])",
        });
        var kpi = Add(model.TabularKpiList, new TabularKpi
        {
            Id = "SalesKpi",
            BaseMeasure = measure,
            StatusExpression = "1",
        });
        var perspective = Add(model.TabularPerspectiveList, new TabularPerspective
        {
            Id = "SalesPerspective",
            TabularModel = root,
            Name = "Sales",
        });
        Add(model.TabularPerspectiveKpiList, new TabularPerspectiveKpi
        {
            Id = "SalesKpiPerspective",
            TabularPerspective = perspective,
            TabularKpi = kpi,
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetPerspective = Assert.Single(database.Model.Perspectives.Cast<Tom.Perspective>());
        var perspectiveTable = Assert.Single(targetPerspective.PerspectiveTables.Cast<Tom.PerspectiveTable>());
        Assert.Same(
            database.Model.Tables["Sales"].Measures["Sales Amount"],
            Assert.Single(perspectiveTable.PerspectiveMeasures.Cast<Tom.PerspectiveMeasure>()).Measure);
    }

    [Fact]
    public void BuildDatabase_EmitsTranslations()
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var column = model.TabularColumnList.Single();
        var measure = Add(model.TabularMeasureList, new TabularMeasure
        {
            Id = "SalesAmountMeasure",
            TabularTable = table,
            Name = "Sales Amount",
            Expression = "SUM('Sales'[SalesAmount])",
        });
        var kpi = Add(model.TabularKpiList, new TabularKpi
        {
            Id = "SalesKpi",
            BaseMeasure = measure,
            StatusExpression = "1",
        });
        var hierarchy = Add(model.TabularHierarchyList, new TabularHierarchy
        {
            Id = "SalesHierarchy",
            TabularTable = table,
            Name = "Sales Hierarchy",
        });
        AddHierarchyLevel(model, hierarchy, column, "Amount", "10");
        var perspective = Add(model.TabularPerspectiveList, new TabularPerspective
        {
            Id = "SalesPerspective",
            TabularModel = root,
            Name = "Sales",
        });
        var culture = Add(model.TabularCultureList, new TabularCulture
        {
            Id = "Swedish",
            TabularModel = root,
            Name = "sv-SE",
        });
        Add(model.TabularTableTranslationList, new TabularTableTranslation
        {
            Id = "SalesTableSv",
            TabularCulture = culture,
            TabularTable = table,
            Caption = "Sales SV",
            Description = "Sales table SV",
        });
        Add(model.TabularColumnTranslationList, new TabularColumnTranslation
        {
            Id = "SalesAmountColumnSv",
            TabularCulture = culture,
            TabularColumn = column,
            Caption = "Amount SV",
        });
        Add(model.TabularHierarchyTranslationList, new TabularHierarchyTranslation
        {
            Id = "SalesHierarchySv",
            TabularCulture = culture,
            TabularHierarchy = hierarchy,
            Caption = "Hierarchy SV",
        });
        Add(model.TabularMeasureTranslationList, new TabularMeasureTranslation
        {
            Id = "SalesAmountMeasureSv",
            TabularCulture = culture,
            TabularMeasure = measure,
            Caption = "Measure SV",
        });
        Add(model.TabularPerspectiveTranslationList, new TabularPerspectiveTranslation
        {
            Id = "SalesPerspectiveSv",
            TabularCulture = culture,
            TabularPerspective = perspective,
            Caption = "Perspective SV",
        });
        Add(model.TabularKpiTranslationList, new TabularKpiTranslation
        {
            Id = "SalesKpiSv",
            TabularCulture = culture,
            TabularKpi = kpi,
            Description = "KPI description SV",
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetCulture = Assert.Single(database.Model.Cultures.Cast<Tom.Culture>());
        Assert.Equal("sv-SE", targetCulture.Name);
        AssertTranslation(targetCulture, database.Model.Tables["Sales"], Tom.TranslatedProperty.Caption, "Sales SV");
        AssertTranslation(targetCulture, database.Model.Tables["Sales"], Tom.TranslatedProperty.Description, "Sales table SV");
        AssertTranslation(targetCulture, database.Model.Tables["Sales"].Columns["SalesAmount"], Tom.TranslatedProperty.Caption, "Amount SV");
        AssertTranslation(targetCulture, database.Model.Tables["Sales"].Hierarchies["Sales Hierarchy"], Tom.TranslatedProperty.Caption, "Hierarchy SV");
        AssertTranslation(targetCulture, database.Model.Tables["Sales"].Measures["Sales Amount"], Tom.TranslatedProperty.Caption, "Measure SV");
        AssertTranslation(targetCulture, database.Model.Perspectives["Sales"], Tom.TranslatedProperty.Caption, "Perspective SV");
        AssertTranslation(targetCulture, database.Model.Tables["Sales"].Measures["Sales Amount"].KPI, Tom.TranslatedProperty.Description, "KPI description SV");
    }

    [Fact]
    public void BuildDatabase_EmitsObjectLevelSecurity()
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var column = model.TabularColumnList.Single();
        var role = Add(model.TabularSecurityRoleList, new TabularSecurityRole
        {
            Id = "Reader",
            TabularModel = root,
            Name = "Reader",
            Permission = "Read",
        });
        Add(model.TabularRoleMemberList, new TabularRoleMember
        {
            Id = "SalesReaders",
            TabularSecurityRole = role,
            MemberName = "CONTOSO\\SalesReaders",
            MemberId = "S-1-5-21-1000-2000-3000-4000",
        });
        Add(model.TabularRoleFilterList, new TabularRoleFilter
        {
            Id = "SalesFilter",
            TabularSecurityRole = role,
            TabularTable = table,
            Expression = "Sales[SalesAmount] >= 0",
        });
        Add(model.TabularTablePermissionList, new TabularTablePermission
        {
            Id = "SalesTablePermission",
            TabularSecurityRole = role,
            TabularTable = table,
            MetadataPermission = "Read",
        });
        Add(model.TabularColumnPermissionList, new TabularColumnPermission
        {
            Id = "SalesAmountPermission",
            TabularSecurityRole = role,
            TabularColumn = column,
            MetadataPermission = "None",
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetRole = Assert.Single(database.Model.Roles.Cast<Tom.ModelRole>());
        var targetMember = Assert.IsType<Tom.WindowsModelRoleMember>(Assert.Single(targetRole.Members.Cast<Tom.ModelRoleMember>()));
        Assert.Equal("CONTOSO\\SalesReaders", targetMember.MemberName);
        Assert.Equal("S-1-5-21-1000-2000-3000-4000", targetMember.MemberID);

        var tablePermission = Assert.Single(targetRole.TablePermissions.Cast<Tom.TablePermission>());
        Assert.Same(database.Model.Tables["Sales"], tablePermission.Table);
        Assert.Equal("Sales[SalesAmount] >= 0", tablePermission.FilterExpression);
        Assert.Equal(Tom.MetadataPermission.Read, tablePermission.MetadataPermission);

        var columnPermission = Assert.Single(tablePermission.ColumnPermissions.Cast<Tom.ColumnPermission>());
        Assert.Same(database.Model.Tables["Sales"].Columns["SalesAmount"], columnPermission.Column);
        Assert.Equal(Tom.MetadataPermission.None, columnPermission.MetadataPermission);
    }

    [Fact]
    public void BuildDatabase_EmitsPartitionQuerySources()
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var dataSource = Add(model.TabularDataSourceList, new TabularDataSource
        {
            Id = "Warehouse",
            TabularModel = root,
            Name = "Warehouse",
            Provider = "Custom.Provider",
            ConnectionReference = "META_BI_TEST_WAREHOUSE",
        });
        Add(model.TabularPartitionList, new TabularPartition
        {
            Id = "SalesPartition",
            TabularTable = table,
            TabularDataSource = dataSource,
            Name = "Sales Partition",
            Ordinal = "10",
            Mode = "Import",
            Expression = "SELECT * FROM mart.FactSales",
        });

        var database = BuildDatabase(model, root, "Commerce");

        var targetDataSource = Assert.IsType<Tom.ProviderDataSource>(Assert.Single(database.Model.DataSources.Cast<Tom.DataSource>()));
        Assert.Equal("Custom.Provider", targetDataSource.Provider);

        var targetTable = Assert.Single(database.Model.Tables.Cast<Tom.Table>());
        var partition = Assert.Single(targetTable.Partitions.Cast<Tom.Partition>());
        Assert.Equal("Sales Partition", partition.Name);
        var source = Assert.IsType<Tom.QueryPartitionSource>(partition.Source);
        Assert.Same(targetDataSource, source.DataSource);
        Assert.Equal("SELECT * FROM mart.FactSales", source.Query);
    }

    [Fact]
    public void BuildDatabase_EmitsRelationshipsWithDeclaredEndpoints()
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var salesTable = model.TabularTableList.Single();
        var salesCustomerKey = Add(model.TabularColumnList, new TabularColumn
        {
            Id = "SalesCustomerKey",
            TabularTable = salesTable,
            Name = "CustomerKey",
            DataTypeId = "meta:type:Int64",
            Ordinal = "20",
        });
        var customerTable = Add(model.TabularTableList, new TabularTable
        {
            Id = "Customer",
            TabularModel = root,
            Name = "Customer",
        });
        var customerKey = Add(model.TabularColumnList, new TabularColumn
        {
            Id = "CustomerKey",
            TabularTable = customerTable,
            Name = "CustomerKey",
            DataTypeId = "meta:type:Int64",
            Ordinal = "10",
        });
        Add(model.TabularRelationshipList, new TabularRelationship
        {
            Id = "SalesCustomer",
            Name = "Sales Customer",
            FromTable = salesTable,
            FromColumn = salesCustomerKey,
            ToTable = customerTable,
            ToColumn = customerKey,
            Cardinality = "ManyToOne",
            CrossFilterDirection = "Single",
            IsActive = "true",
            IsRequired = "true",
        });

        var database = BuildDatabase(model, root, "Commerce");

        var relationship = Assert.IsType<Tom.SingleColumnRelationship>(Assert.Single(database.Model.Relationships.Cast<Tom.Relationship>()));
        Assert.Equal("Sales Customer", relationship.Name);
        Assert.Same(database.Model.Tables["Sales"].Columns["CustomerKey"], relationship.FromColumn);
        Assert.Same(database.Model.Tables["Customer"].Columns["CustomerKey"], relationship.ToColumn);
        Assert.Equal(Tom.RelationshipEndCardinality.Many, relationship.FromCardinality);
        Assert.Equal(Tom.RelationshipEndCardinality.One, relationship.ToCardinality);
        Assert.True(relationship.IsActive);
        Assert.True(relationship.RelyOnReferentialIntegrity);
    }

    [Theory]
    [InlineData("DefaultDataView", "TabularModel.DefaultDataView")]
    [InlineData("SummarizeBy", "TabularColumn.SummarizeBy")]
    [InlineData("PartitionMode", "TabularPartition.Mode")]
    [InlineData("Cardinality", "TabularRelationship.Cardinality")]
    [InlineData("CrossFilterDirection", "TabularRelationship.CrossFilterDirection")]
    [InlineData("Permission", "TabularSecurityRole.Permission")]
    [InlineData("MetadataPermission", "MetadataPermission")]
    public void BuildDatabase_RejectsInvalidTargetValues(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var column = model.TabularColumnList.Single();

        switch (scenario)
        {
            case "DefaultDataView":
                root.DefaultDataView = "NotADataView";
                break;
            case "SummarizeBy":
                column.SummarizeBy = "Totalish";
                break;
            case "PartitionMode":
                Add(model.TabularDataSourceList, new TabularDataSource
                {
                    Id = "Warehouse",
                    TabularModel = root,
                    Name = "Warehouse",
                    Provider = "SqlServer",
                });
                Add(model.TabularPartitionList, new TabularPartition
                {
                    Id = "SalesPartition",
                    TabularTable = table,
                    Name = "Sales Partition",
                    Mode = "MaybeImport",
                });
                break;
            case "Cardinality":
                AddRelationship(model, table, column, "Sideways", "Single");
                break;
            case "CrossFilterDirection":
                AddRelationship(model, table, column, "ManyToOne", "Diagonal");
                break;
            case "Permission":
                Add(model.TabularSecurityRoleList, new TabularSecurityRole
                {
                    Id = "Reader",
                    TabularModel = root,
                    Name = "Reader",
                    Permission = "CanPeek",
                });
                break;
            case "MetadataPermission":
                var role = Add(model.TabularSecurityRoleList, new TabularSecurityRole
                {
                    Id = "Reader",
                    TabularModel = root,
                    Name = "Reader",
                    Permission = "Read",
                });
                Add(model.TabularTablePermissionList, new TabularTablePermission
                {
                    Id = "SalesTablePermission",
                    TabularSecurityRole = role,
                    TabularTable = table,
                    MetadataPermission = "CanPeek",
                });
                break;
        }

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("ExpressionWithoutDataSource", "defines Expression but no TabularDataSource")]
    [InlineData("DataSourceWithoutExpression", "defines TabularDataSource but no Expression")]
    [InlineData("MissingDataSource", "references data source")]
    public void BuildDatabase_RejectsInvalidPartitions(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var dataSource = Add(model.TabularDataSourceList, new TabularDataSource
        {
            Id = "Warehouse",
            TabularModel = root,
            Name = "Warehouse",
            Provider = "SqlServer",
        });

        if (scenario == "MissingDataSource")
        {
            dataSource = new TabularDataSource
            {
                Id = "DetachedWarehouse",
                TabularModel = root,
                Name = "Detached Warehouse",
            };
        }

        Add(model.TabularPartitionList, new TabularPartition
        {
            Id = "SalesPartition",
            TabularTable = table,
            TabularDataSource = scenario == "ExpressionWithoutDataSource" ? null : dataSource,
            Name = "Sales Partition",
            Ordinal = "10",
            Mode = "Import",
            Expression = scenario == "DataSourceWithoutExpression" ? null : "SELECT * FROM mart.FactSales",
        });

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("MissingFromTable", "references table")]
    [InlineData("MissingFromColumn", "references column")]
    [InlineData("FromColumnTableMismatch", "FromColumn must belong to FromTable")]
    [InlineData("ToColumnTableMismatch", "ToColumn must belong to ToTable")]
    public void BuildDatabase_RejectsInvalidRelationships(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var salesTable = model.TabularTableList.Single();
        var salesAmount = model.TabularColumnList.Single();
        var customerTable = Add(model.TabularTableList, new TabularTable
        {
            Id = "Customer",
            TabularModel = root,
            Name = "Customer",
        });
        var customerKey = Add(model.TabularColumnList, new TabularColumn
        {
            Id = "CustomerKey",
            TabularTable = customerTable,
            Name = "CustomerKey",
            DataTypeId = "meta:type:Int64",
            Ordinal = "10",
        });

        var fromTable = salesTable;
        var fromColumn = salesAmount;
        var toTable = customerTable;
        var toColumn = customerKey;

        switch (scenario)
        {
            case "MissingFromTable":
                fromTable = new TabularTable
                {
                    Id = "DetachedTable",
                    TabularModel = root,
                    Name = "Detached",
                };
                break;
            case "MissingFromColumn":
                fromColumn = new TabularColumn
                {
                    Id = "DetachedColumn",
                    TabularTable = salesTable,
                    Name = "Detached",
                    DataTypeId = "meta:type:Int64",
                };
                break;
            case "FromColumnTableMismatch":
                fromColumn = customerKey;
                break;
            case "ToColumnTableMismatch":
                toColumn = salesAmount;
                break;
        }

        Add(model.TabularRelationshipList, new TabularRelationship
        {
            Id = "SalesCustomer",
            Name = "Sales Customer",
            FromTable = fromTable,
            FromColumn = fromColumn,
            ToTable = toTable,
            ToColumn = toColumn,
            Cardinality = "ManyToOne",
            CrossFilterDirection = "Single",
        });

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("MissingBase", "references base measure")]
    [InlineData("MissingTarget", "references target measure")]
    [InlineData("DuplicateBase", "more than one TabularKpi row")]
    [InlineData("ConflictingTarget", "defines both TargetExpression and TargetMeasure")]
    public void BuildDatabase_RejectsInvalidKpis(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var baseMeasure = Add(model.TabularMeasureList, new TabularMeasure
        {
            Id = "SalesAmountMeasure",
            TabularTable = table,
            Name = "Sales Amount",
            Expression = "SUM('Sales'[SalesAmount])",
        });
        var targetMeasure = Add(model.TabularMeasureList, new TabularMeasure
        {
            Id = "SalesTargetMeasure",
            TabularTable = table,
            Name = "Sales Target",
            Expression = "SUM('Sales'[SalesTarget])",
        });

        switch (scenario)
        {
            case "MissingBase":
                baseMeasure = new TabularMeasure
                {
                    Id = "DetachedBase",
                    TabularTable = table,
                    Name = "Detached Base",
                    Expression = "BLANK()",
                };
                break;
            case "MissingTarget":
                targetMeasure = new TabularMeasure
                {
                    Id = "DetachedTarget",
                    TabularTable = table,
                    Name = "Detached Target",
                    Expression = "BLANK()",
                };
                break;
            case "DuplicateBase":
                Add(model.TabularKpiList, new TabularKpi
                {
                    Id = "FirstSalesKpi",
                    BaseMeasure = baseMeasure,
                    StatusExpression = "1",
                });
                break;
        }

        Add(model.TabularKpiList, new TabularKpi
        {
            Id = "SalesKpi",
            BaseMeasure = baseMeasure,
            TargetMeasure = targetMeasure,
            TargetExpression = scenario == "ConflictingTarget" ? "1" : null,
            StatusExpression = "1",
        });

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("DuplicateTable", "duplicate full-table membership")]
    [InlineData("DuplicateColumn", "duplicate column membership")]
    [InlineData("DuplicateHierarchy", "duplicate hierarchy membership")]
    [InlineData("DuplicateMeasure", "duplicate measure membership")]
    [InlineData("MissingTable", "references table")]
    [InlineData("MissingColumn", "references column")]
    [InlineData("MissingHierarchy", "references hierarchy")]
    [InlineData("MissingMeasure", "references measure")]
    [InlineData("MissingKpi", "references KPI")]
    [InlineData("DuplicateKpi", "duplicate measure membership")]
    public void BuildDatabase_RejectsInvalidPerspectives(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var column = model.TabularColumnList.Single();
        var perspective = Add(model.TabularPerspectiveList, new TabularPerspective
        {
            Id = "SalesPerspective",
            TabularModel = root,
            Name = "Sales",
        });
        var measure = Add(model.TabularMeasureList, new TabularMeasure
        {
            Id = "SalesAmountMeasure",
            TabularTable = table,
            Name = "Sales Amount",
            Expression = "SUM('Sales'[SalesAmount])",
        });
        var hierarchy = Add(model.TabularHierarchyList, new TabularHierarchy
        {
            Id = "Calendar",
            TabularTable = table,
            Name = "Calendar",
        });
        AddHierarchyLevel(model, hierarchy, column, "Amount", "10");

        switch (scenario)
        {
            case "DuplicateTable":
                AddPerspectiveTable(model, perspective, table, "First");
                AddPerspectiveTable(model, perspective, table, "Second");
                break;
            case "DuplicateColumn":
                AddPerspectiveColumn(model, perspective, column, "First");
                AddPerspectiveColumn(model, perspective, column, "Second");
                break;
            case "DuplicateHierarchy":
                AddPerspectiveHierarchy(model, perspective, hierarchy, "First");
                AddPerspectiveHierarchy(model, perspective, hierarchy, "Second");
                break;
            case "DuplicateMeasure":
                AddPerspectiveMeasure(model, perspective, measure, "First");
                AddPerspectiveMeasure(model, perspective, measure, "Second");
                break;
            case "MissingTable":
                AddPerspectiveTable(model, perspective, new TabularTable
                {
                    Id = "Detached",
                    TabularModel = root,
                    Name = "Detached",
                }, "Missing");
                break;
            case "MissingColumn":
                AddPerspectiveColumn(model, perspective, new TabularColumn
                {
                    Id = "DetachedColumn",
                    TabularTable = table,
                    Name = "Detached",
                    DataTypeId = "meta:type:String",
                }, "Missing");
                break;
            case "MissingHierarchy":
                AddPerspectiveHierarchy(model, perspective, new TabularHierarchy
                {
                    Id = "DetachedHierarchy",
                    TabularTable = table,
                    Name = "Detached",
                }, "Missing");
                break;
            case "MissingMeasure":
                AddPerspectiveMeasure(model, perspective, new TabularMeasure
                {
                    Id = "DetachedMeasure",
                    TabularTable = table,
                    Name = "Detached",
                    Expression = "BLANK()",
                }, "Missing");
                break;
            case "MissingKpi":
                Add(model.TabularPerspectiveKpiList, new TabularPerspectiveKpi
                {
                    Id = "MissingKpiPerspective",
                    TabularPerspective = perspective,
                    TabularKpi = new TabularKpi
                    {
                        Id = "DetachedKpi",
                        BaseMeasure = measure,
                        StatusExpression = "1",
                    },
                });
                break;
            case "DuplicateKpi":
                var kpi = Add(model.TabularKpiList, new TabularKpi
                {
                    Id = "SalesKpi",
                    BaseMeasure = measure,
                    StatusExpression = "1",
                });
                Add(model.TabularPerspectiveKpiList, new TabularPerspectiveKpi
                {
                    Id = "FirstSalesKpiPerspective",
                    TabularPerspective = perspective,
                    TabularKpi = kpi,
                });
                Add(model.TabularPerspectiveKpiList, new TabularPerspectiveKpi
                {
                    Id = "SecondSalesKpiPerspective",
                    TabularPerspective = perspective,
                    TabularKpi = kpi,
                });
                break;
        }

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("MissingCulture", "references culture")]
    [InlineData("MissingTable", "references table")]
    [InlineData("DuplicateCaption", "duplicates a Caption translation")]
    public void BuildDatabase_RejectsInvalidTranslations(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var culture = Add(model.TabularCultureList, new TabularCulture
        {
            Id = "Swedish",
            TabularModel = root,
            Name = "sv-SE",
        });

        switch (scenario)
        {
            case "MissingCulture":
                culture = new TabularCulture
                {
                    Id = "DetachedCulture",
                    TabularModel = root,
                    Name = "sv-SE",
                };
                break;
            case "MissingTable":
                table = new TabularTable
                {
                    Id = "DetachedTable",
                    TabularModel = root,
                    Name = "Detached",
                };
                break;
            case "DuplicateCaption":
                Add(model.TabularTableTranslationList, new TabularTableTranslation
                {
                    Id = "FirstTableTranslation",
                    TabularCulture = culture,
                    TabularTable = table,
                    Caption = "First",
                });
                break;
        }

        Add(model.TabularTableTranslationList, new TabularTableTranslation
        {
            Id = "TableTranslation",
            TabularCulture = culture,
            TabularTable = table,
            Caption = "Second",
        });

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("CrossTable", "must reference columns in the same TabularTable")]
    [InlineData("Duplicate", "has more than one TabularSortByColumn row")]
    public void BuildDatabase_RejectsInvalidSortByColumns(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var sourceColumn = model.TabularColumnList.Single();
        var sortColumn = Add(model.TabularColumnList, new TabularColumn
        {
            Id = "SortColumn",
            TabularTable = table,
            Name = "SortColumn",
            DataTypeId = "meta:type:Int32",
            Ordinal = "20",
        });

        switch (scenario)
        {
            case "CrossTable":
                var otherTable = Add(model.TabularTableList, new TabularTable
                {
                    Id = "Other",
                    TabularModel = root,
                    Name = "Other",
                });
                sortColumn.TabularTable = otherTable;
                break;
            case "Duplicate":
                var secondSortColumn = Add(model.TabularColumnList, new TabularColumn
                {
                    Id = "SecondSortColumn",
                    TabularTable = table,
                    Name = "SecondSortColumn",
                    DataTypeId = "meta:type:Int32",
                    Ordinal = "30",
                });
                Add(model.TabularSortByColumnList, new TabularSortByColumn
                {
                    Id = "DuplicateSort",
                    SourceColumn = sourceColumn,
                    SortColumn = secondSortColumn,
                });
                break;
        }

        Add(model.TabularSortByColumnList, new TabularSortByColumn
        {
            Id = "Sort",
            SourceColumn = sourceColumn,
            SortColumn = sortColumn,
        });

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("NoLevels", "requires at least one TabularHierarchyLevel row")]
    [InlineData("CrossTable", "must reference a column in the hierarchy table")]
    [InlineData("DuplicateOrdinal", "has more than one level with ordinal")]
    public void BuildDatabase_RejectsInvalidHierarchies(string scenario, string expectedMessage)
    {
        var model = CreateModel();
        var root = model.TabularModelList.Single();
        var table = model.TabularTableList.Single();
        var column = model.TabularColumnList.Single();
        var hierarchy = Add(model.TabularHierarchyList, new TabularHierarchy
        {
            Id = "Calendar",
            TabularTable = table,
            Name = "Calendar",
        });

        switch (scenario)
        {
            case "NoLevels":
                break;
            case "CrossTable":
                var otherTable = Add(model.TabularTableList, new TabularTable
                {
                    Id = "Other",
                    TabularModel = root,
                    Name = "Other",
                });
                var otherColumn = Add(model.TabularColumnList, new TabularColumn
                {
                    Id = "OtherColumn",
                    TabularTable = otherTable,
                    Name = "Other Column",
                    DataTypeId = "meta:type:String",
                });
                AddHierarchyLevel(model, hierarchy, otherColumn, "Other", "10");
                break;
            case "DuplicateOrdinal":
                AddHierarchyLevel(model, hierarchy, column, "First", "10");
                var secondColumn = Add(model.TabularColumnList, new TabularColumn
                {
                    Id = "SecondColumn",
                    TabularTable = table,
                    Name = "Second Column",
                    DataTypeId = "meta:type:String",
                });
                AddHierarchyLevel(model, hierarchy, secondColumn, "Second", "10");
                break;
        }

        var exception = AssertBuildFails(model, root);
        Assert.Contains(expectedMessage, exception.Message, StringComparison.Ordinal);
    }

    private static void AddRelationship(MetaTabularModel model, TabularTable table, TabularColumn column, string cardinality, string crossFilterDirection)
    {
        Add(model.TabularRelationshipList, new TabularRelationship
        {
            Id = "SalesSelf",
            Name = "Sales Self",
            FromTable = table,
            FromColumn = column,
            ToTable = table,
            ToColumn = column,
            Cardinality = cardinality,
            CrossFilterDirection = crossFilterDirection,
        });
    }

    private static void AddHierarchyLevel(MetaTabularModel model, TabularHierarchy hierarchy, TabularColumn column, string name, string ordinal)
    {
        Add(model.TabularHierarchyLevelList, new TabularHierarchyLevel
        {
            Id = $"{hierarchy.Id}:{name}",
            TabularHierarchy = hierarchy,
            TabularColumn = column,
            Name = name,
            Ordinal = ordinal,
        });
    }

    private static void AddPerspectiveTable(MetaTabularModel model, TabularPerspective perspective, TabularTable table, string id)
    {
        Add(model.TabularPerspectiveTableList, new TabularPerspectiveTable
        {
            Id = id,
            TabularPerspective = perspective,
            TabularTable = table,
        });
    }

    private static void AddPerspectiveColumn(MetaTabularModel model, TabularPerspective perspective, TabularColumn column, string id)
    {
        Add(model.TabularPerspectiveColumnList, new TabularPerspectiveColumn
        {
            Id = id,
            TabularPerspective = perspective,
            TabularColumn = column,
        });
    }

    private static void AddPerspectiveHierarchy(MetaTabularModel model, TabularPerspective perspective, TabularHierarchy hierarchy, string id)
    {
        Add(model.TabularPerspectiveHierarchyList, new TabularPerspectiveHierarchy
        {
            Id = id,
            TabularPerspective = perspective,
            TabularHierarchy = hierarchy,
        });
    }

    private static void AddPerspectiveMeasure(MetaTabularModel model, TabularPerspective perspective, TabularMeasure measure, string id)
    {
        Add(model.TabularPerspectiveMeasureList, new TabularPerspectiveMeasure
        {
            Id = id,
            TabularPerspective = perspective,
            TabularMeasure = measure,
        });
    }

    private static InvalidOperationException AssertBuildFails(MetaTabularModel model, TabularModel root)
    {
        var method = typeof(MetaTabularDeployService).GetMethod(
            "BuildDatabase",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var exception = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { model, root, "Commerce" }));
        return Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    private static Tom.Database BuildDatabase(MetaTabularModel model, TabularModel root, string databaseName)
    {
        var method = typeof(MetaTabularDeployService).GetMethod(
            "BuildDatabase",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        return Assert.IsType<Tom.Database>(method.Invoke(null, new object[] { model, root, databaseName }));
    }

    private static void AssertTranslation(
        Tom.Culture culture,
        Tom.MetadataObject targetObject,
        Tom.TranslatedProperty property,
        string expectedValue)
    {
        var translation = culture.ObjectTranslations[targetObject, property];
        Assert.NotNull(translation);
        Assert.Equal(expectedValue, translation.Value);
    }

    private static MetaTabularModel CreateModel()
    {
        var model = MetaTabularModel.CreateEmpty();
        var root = Add(model.TabularModelList, new TabularModel
        {
            Id = "Commerce",
            Name = "Commerce",
            DefaultCulture = "en-US",
        });
        var table = Add(model.TabularTableList, new TabularTable
        {
            Id = "Sales",
            TabularModel = root,
            Name = "Sales",
        });
        Add(model.TabularColumnList, new TabularColumn
        {
            Id = "SalesAmount",
            TabularTable = table,
            Name = "SalesAmount",
            DataTypeId = "meta:type:Decimal",
        });

        return model;
    }

    private static T Add<T>(ICollection<T> rows, T row)
    {
        rows.Add(row);
        return row;
    }
}
