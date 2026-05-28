using MetaConvert.DataWarehouseToSql;
using MetaDataWarehouse.Instance;

namespace MetaDataWarehouse.Tests;

public sealed class DataWarehouseToSqlConverterTests
{
    [Fact]
    public void ConvertToMetaSql_ProjectsDimensionsFactsRolesAndPlatformColumns()
    {
        var sql = DataWarehouseToSqlConverter.ConvertToMetaSql(
            MetaDataWarehouseInstance.SampleSales,
            MetaDataWarehouseImplementationInstance.Default,
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
}
