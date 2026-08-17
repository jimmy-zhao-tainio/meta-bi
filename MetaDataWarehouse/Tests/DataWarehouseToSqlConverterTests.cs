using Meta.Integration;
using Meta.Operations.Domain;
using Meta.TypedModels;
using MetaConvert.DataWarehouseToSql;
using MetaDataType;
using MetaDataTypeConversion;
using MetaBi.Tests.Common;
using MetaWeave.Core;
using MetaWeaveScript.Execution;

namespace MetaDataWarehouse.Tests;

public sealed class DataWarehouseToSqlConverterTests
{
    [Fact]
    public async Task SanctionedWeave_MatchesEstablishedConverter()
    {
        var warehouse = TestModels.LoadSampleSales();
        var implementation = MetaDataWarehouseImplementation.MetaDataWarehouseImplementationInstance.BuiltIn;
        var expected = TypedWorkspaceModelMapper.ToInMemoryWorkspace(
            DataWarehouseToSqlConverter.ConvertToMetaSql(warehouse, implementation, "CommerceDw"));

        var actual = await ExecuteSanctionedWeaveAsync(warehouse, implementation, "CommerceDw");

        Assert.True(actual.IsSuccess, FormatIssues(actual));
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, actual.OutputWorkspace!));
    }

    [Fact]
    public async Task SanctionedWeave_MatchesEstablishedConverter_WhenOrdinalsAreNotNumeric()
    {
        var warehouse = TestModels.LoadSampleSales();
        var implementation = MetaDataWarehouseImplementation.MetaDataWarehouseImplementationInstance.BuiltIn;
        warehouse.DimensionAttributeList[0].Ordinal = "not-an-integer";
        warehouse.DimensionAttributeList[1].Ordinal = "also-not-an-integer";
        var expected = TypedWorkspaceModelMapper.ToInMemoryWorkspace(
            DataWarehouseToSqlConverter.ConvertToMetaSql(warehouse, implementation, "CommerceDw"));

        var actual = await ExecuteSanctionedWeaveAsync(warehouse, implementation, "CommerceDw");

        Assert.True(actual.IsSuccess, FormatIssues(actual));
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(expected, actual.OutputWorkspace!));
    }

    [Fact]
    public async Task SanctionedWeave_RejectsBlankDatabaseName()
    {
        var result = await ExecuteSanctionedWeaveAsync(
            TestModels.LoadSampleSales(),
            MetaDataWarehouseImplementation.MetaDataWarehouseImplementationInstance.BuiltIn,
            " ");

        Assert.False(result.IsSuccess);
        Assert.Null(result.OutputWorkspace);
        Assert.Contains(result.Issues, issue => issue.Code == "DatabaseNameRequired");
    }

    [Fact]
    public async Task SanctionedWeave_RejectsAnUnlowerableUsedType()
    {
        var warehouse = TestModels.LoadSampleSales();
        warehouse.DimensionAttributeList[0].DataTypeId = "unsanctioned:type";

        var result = await ExecuteSanctionedWeaveAsync(
            warehouse,
            MetaDataWarehouseImplementation.MetaDataWarehouseImplementationInstance.BuiltIn,
            "CommerceDw");

        Assert.False(result.IsSuccess);
        Assert.Null(result.OutputWorkspace);
        Assert.Contains(result.Issues, issue => issue.Code == "SqlServerTypeLoweringInvalid");
    }

    [Fact]
    public async Task SanctionedWeave_RejectsMissingImplementationContract()
    {
        var implementation = TypedModelMapper.FromWorkspace(
            TypedModelMapper.ToWorkspace(
                MetaDataWarehouseImplementation.MetaDataWarehouseImplementationInstance.BuiltIn),
            MetaDataWarehouseImplementation.MetaDataWarehouseImplementationModel.CreateEmpty);
        implementation.FactTableImplementationList.Clear();

        var result = await ExecuteSanctionedWeaveAsync(
            TestModels.LoadSampleSales(),
            implementation,
            "CommerceDw");

        Assert.False(result.IsSuccess);
        Assert.Null(result.OutputWorkspace);
        Assert.Contains(result.Issues, issue => issue.Code == "ImplementationCardinalityInvalid");
    }

    [Fact]
    public void ConvertToMetaSql_ProjectsDimensionsFactsRolesAndPlatformColumns()
    {
        var sql = DataWarehouseToSqlConverter.ConvertToMetaSql(
            TestModels.LoadSampleSales(),
            MetaDataWarehouseImplementation.MetaDataWarehouseImplementationInstance.BuiltIn,
            "CommerceDw");

        var customerTable = Assert.Single(sql.TableList, row => row.Name == "Dim_Customer");
        var salesOrderTable = Assert.Single(sql.TableList, row => row.Name == "Fact_SalesOrder");
        Assert.Contains(sql.TableColumnList, row => row.Table == customerTable && row.Name == "CustomerKey");
        Assert.Contains(sql.TableColumnList, row => row.Table == customerTable && row.Name == "AuditId" && row.DefaultExpressionSql == "CONVERT(bigint, SESSION_CONTEXT(N'MetaPipeline.AuditId'))");
        var orderDateKey = Assert.Single(sql.TableColumnList, row => row.Table == salesOrderTable && row.Name == "OrderDateKey");
        var shipDateKey = Assert.Single(sql.TableColumnList, row => row.Table == salesOrderTable && row.Name == "ShipDateKey");
        Assert.Equal("sqlserver:type:bigint", orderDateKey.MetaDataTypeId);
        Assert.Equal("false", orderDateKey.IsNullable);
        Assert.Equal("true", shipDateKey.IsNullable);
        var customerNumber = Assert.Single(sql.TableColumnList, row => row.Table == customerTable && row.Name == "CustomerNumber");
        Assert.Equal("sqlserver:type:nvarchar", customerNumber.MetaDataTypeId);
        Assert.Contains(sql.TableColumnDataTypeDetailList, row => row.TableColumn == customerNumber && row.Name == "Length" && row.Value == "256");
        Assert.Contains(sql.ForeignKeyList, row => row.SourceTable == salesOrderTable && row.TargetTable.Name == "Dim_Date" && row.Name == "FK_Fact_SalesOrder_OrderDate");
        Assert.Contains(sql.ForeignKeyList, row => row.SourceTable == salesOrderTable && row.TargetTable.Name == "Dim_Date" && row.Name == "FK_Fact_SalesOrder_ShipDate");

        var salesOrderPrimaryKey = Assert.Single(sql.PrimaryKeyList, row => row.Table == salesOrderTable);
        Assert.Contains(sql.PrimaryKeyColumnList, row => row.PrimaryKey == salesOrderPrimaryKey && row.TableColumn == orderDateKey);
        Assert.DoesNotContain(sql.PrimaryKeyColumnList, row => row.PrimaryKey == salesOrderPrimaryKey && row.TableColumn == shipDateKey);
    }

    private static async Task<MetaWeaveScriptApplicationResult> ExecuteSanctionedWeaveAsync(
        MetaDataWarehouseModel warehouse,
        MetaDataWarehouseImplementation.MetaDataWarehouseImplementationModel implementation,
        string databaseName)
    {
        var repositoryRoot = CliTestRunner.FindRepositoryRoot();
        var targetContract = await TypedWorkspaceModelMapper.LoadStateAsync(
            Path.Combine(repositoryRoot, "MetaSql", "Workspace"));
        var emptyTarget = new InMemoryWorkspace(
            targetContract.Model.Clone(),
            new GenericInstance { ModelName = targetContract.Model.Name });
        var sources = new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
        {
            ["warehouse"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(warehouse),
            ["implementation"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(implementation),
            ["dataTypes"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeInstance.BuiltIn),
            ["typeConversions"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(MetaDataTypeConversionInstance.BuiltIn),
        };

        return new MetaWeaveScriptExecutionService().ExecuteDirection(
            LoadSanctionedDirection(),
            sources,
            emptyTarget,
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["databaseName"] = databaseName,
            });
    }

    private static string FormatIssues(MetaWeaveScriptApplicationResult result) =>
        string.Join(Environment.NewLine, result.Issues.Select(issue => $"{issue.Code}: {issue.Message}"));

    private static MetaWeaveScriptDirection LoadSanctionedDirection() =>
        new MetaWeaveScriptDirectionLoader().Load(
            Path.Combine(
                CliTestRunner.FindRepositoryRoot(),
                "MetaConvert",
                "Weaves",
                "DataWarehouseToSql"),
            "forward");
}
