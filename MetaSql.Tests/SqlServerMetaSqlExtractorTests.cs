using MetaSql.Extractors.SqlServer;

namespace MetaSql.Tests;

public sealed class SqlServerMetaSqlExtractorTests
{
    [Fact]
    public void Project_CreatesPhysicalIdsForSupportedObjects()
    {
        var workspace = SqlServerMetaSqlProjector.Project(
            newWorkspacePath: "C:\\tmp\\MetaSql",
            databaseName: "SalesDb",
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("dbo", "Customer"),
                new SqlServerMetaSqlProjector.TableRow("dbo", "Order")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Customer"] =
                [
                    new("dbo", "Customer", "CustomerId", 1, false, "int", null, 10, 0),
                    new("dbo", "Customer", "CustomerName", 2, true, "nvarchar", 200, null, null)
                ],
                ["dbo.Order"] =
                [
                    new("dbo", "Order", "OrderId", 1, false, "int", null, 10, 0),
                    new("dbo", "Order", "CustomerId", 2, false, "int", null, 10, 0)
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Customer"] = [new("PK_Customer", true)],
                ["dbo.Order"] = [new("PK_Order", true)],
            },
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Customer"] = [new("PK_Customer", 1, "CustomerId", false)],
                ["dbo.Order"] = [new("PK_Order", 1, "OrderId", false)],
            },
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Order"] = [new("FK_Order_Customer", "dbo", "Customer")],
            },
            foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Order"] = [new("FK_Order_Customer", 1, "CustomerId", "CustomerId")],
            },
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Customer"] = [new("IX_Customer_Name", false, false)],
            },
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Customer"] = [new("IX_Customer_Name", 1, "CustomerName", false, false)],
            });

        Assert.Equal("MetaSql", workspace.Model.Name);

        var databases = workspace.Instance.GetOrCreateEntityRecords("Database");
        var schemas = workspace.Instance.GetOrCreateEntityRecords("Schema");
        var tables = workspace.Instance.GetOrCreateEntityRecords("Table");
        var columns = workspace.Instance.GetOrCreateEntityRecords("TableColumn");
        var primaryKeys = workspace.Instance.GetOrCreateEntityRecords("PrimaryKey");
        var foreignKeys = workspace.Instance.GetOrCreateEntityRecords("ForeignKey");
        var indexes = workspace.Instance.GetOrCreateEntityRecords("Index");

        Assert.Single(databases);
        Assert.Equal("SalesDb", databases[0].Id);
        Assert.Single(schemas);
        Assert.Equal("SalesDb.dbo", schemas[0].Id);

        Assert.Contains(tables, row => row.Id == "SalesDb.dbo.Customer");
        Assert.Contains(tables, row => row.Id == "SalesDb.dbo.Order");
        Assert.Contains(columns, row => row.Id == "SalesDb.dbo.Customer.CustomerId");
        Assert.Contains(columns, row => row.Id == "SalesDb.dbo.Customer.CustomerName");
        Assert.Contains(primaryKeys, row => row.Id == "SalesDb.dbo.Customer.pk.PK_Customer");
        Assert.Contains(foreignKeys, row => row.Id == "SalesDb.dbo.Order.fk.FK_Order_Customer");
        Assert.Contains(indexes, row => row.Id == "SalesDb.dbo.Customer.index.IX_Customer_Name");
    }

    [Fact]
    public void Project_PreservesColumnTypeDetailsAndIndexFlags()
    {
        var workspace = SqlServerMetaSqlProjector.Project(
            newWorkspacePath: "C:\\tmp\\MetaSql",
            databaseName: "SalesDb",
            tableRows: [new SqlServerMetaSqlProjector.TableRow("dbo", "Customer")],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Customer"] =
                [
                    new("dbo", "Customer", "CustomerName", 1, true, "nvarchar", 200, null, null)
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase),
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Customer"] = [new("IX_Customer_Name", true, false)],
            },
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Customer"] = [new("IX_Customer_Name", 1, "CustomerName", true, true)],
            });

        var columns = workspace.Instance.GetOrCreateEntityRecords("TableColumn");
        var details = workspace.Instance.GetOrCreateEntityRecords("TableColumnDataTypeDetail");
        var indexes = workspace.Instance.GetOrCreateEntityRecords("Index");
        var indexColumns = workspace.Instance.GetOrCreateEntityRecords("IndexColumn");

        var customerName = columns.Single(row => row.Id == "SalesDb.dbo.Customer.CustomerName");
        var length = details.Single(row => row.RelationshipIds["TableColumnId"] == customerName.Id && row.Values["Name"] == "Length");
        var index = indexes.Single();
        var indexColumn = indexColumns.Single();

        Assert.Equal("200", length.Values["Value"]);
        Assert.Equal("true", index.Values["IsUnique"]);
        Assert.Equal("true", indexColumn.Values["IsDescending"]);
        Assert.Equal("true", indexColumn.Values["IsIncluded"]);
    }

    [Fact]
    public void ExtractMetaSqlWorkspace_RequiresConnectionString()
    {
        var extractor = new SqlServerMetaSqlExtractor();
        var exception = Assert.Throws<InvalidOperationException>(() => extractor.ExtractMetaSqlWorkspace(new SqlServerExtractRequest
        {
            NewWorkspacePath = "C:\\tmp\\MetaSql",
        }));

        Assert.Contains("connection string", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Project_CreatesForeignKeysEvenWhenTargetTableAppearsLater()
    {
        var workspace = SqlServerMetaSqlProjector.Project(
            newWorkspacePath: "C:\\tmp\\MetaSql",
            databaseName: "SalesDb",
            tableRows:
            [
                new SqlServerMetaSqlProjector.TableRow("dbo", "Order"),
                new SqlServerMetaSqlProjector.TableRow("dbo", "Customer")
            ],
            columnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Order"] =
                [
                    new("dbo", "Order", "OrderId", 1, false, "int", null, null, null),
                    new("dbo", "Order", "CustomerId", 2, false, "int", null, null, null)
                ],
                ["dbo.Customer"] =
                [
                    new("dbo", "Customer", "CustomerId", 1, false, "int", null, null, null)
                ],
            },
            primaryKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyRow>>(StringComparer.OrdinalIgnoreCase),
            primaryKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.PrimaryKeyColumnRow>>(StringComparer.OrdinalIgnoreCase),
            foreignKeysByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Order"] = [new("FK_Order_Customer", "dbo", "Customer")],
            },
            foreignKeyColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.ForeignKeyColumnRow>>(StringComparer.OrdinalIgnoreCase)
            {
                ["dbo.Order"] = [new("FK_Order_Customer", 1, "CustomerId", "CustomerId")],
            },
            indexesByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexRow>>(StringComparer.OrdinalIgnoreCase),
            indexColumnsByTableKey: new Dictionary<string, List<SqlServerMetaSqlProjector.IndexColumnRow>>(StringComparer.OrdinalIgnoreCase));

        var foreignKeys = workspace.Instance.GetOrCreateEntityRecords("ForeignKey");
        var foreignKeyColumns = workspace.Instance.GetOrCreateEntityRecords("ForeignKeyColumn");

        Assert.Contains(foreignKeys, row => row.Id == "SalesDb.dbo.Order.fk.FK_Order_Customer");
        Assert.Contains(foreignKeyColumns, row => row.Id == "SalesDb.dbo.Order.fk.FK_Order_Customer.column.1");
    }
}
