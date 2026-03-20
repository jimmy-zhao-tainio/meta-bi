using Meta.Core.Domain;
using Meta.Core.Services;

namespace MetaSql.Tests;

public sealed class MetaSqlDifferenceServiceTests
{
    [Fact]
    public void BuildDifferences_ReturnsExtraLiveColumnDifference()
    {
        var sourceWorkspace = CreateWorkspace(CreateCustomerModel(includeExtraLiveColumn: false), "source");
        var liveWorkspace = CreateWorkspace(CreateCustomerModel(includeExtraLiveColumn: true), "live");

        var service = new MetaSqlDifferenceService();
        var differences = service.BuildDifferences(sourceWorkspace, liveWorkspace);

        var difference = Assert.Single(differences);
        Assert.Equal(MetaSqlObjectKind.TableColumn, difference.ObjectKind);
        Assert.Equal(MetaSqlDifferenceKind.ExtraInLive, difference.DifferenceKind);
        Assert.Equal("dbo.Customer", difference.ScopeDisplayName);
        Assert.Equal("dbo.Customer.LegacyCode", difference.DisplayName);
        Assert.Equal("SalesDb.dbo.Customer.LegacyCode", difference.LiveId);
    }

    [Fact]
    public void BuildDifferences_ReturnsChangedIndexDifference()
    {
        var sourceWorkspace = CreateWorkspace(CreateCustomerModel(includeExtraLiveColumn: false, sourceIndexUnique: true), "source");
        var liveWorkspace = CreateWorkspace(CreateCustomerModel(includeExtraLiveColumn: false, sourceIndexUnique: false), "live");

        var service = new MetaSqlDifferenceService();
        var differences = service.BuildDifferences(sourceWorkspace, liveWorkspace);

        var difference = Assert.Single(differences);
        Assert.Equal(MetaSqlObjectKind.Index, difference.ObjectKind);
        Assert.Equal(MetaSqlDifferenceKind.Different, difference.DifferenceKind);
        Assert.Equal("dbo.Customer", difference.ScopeDisplayName);
        Assert.Equal("IX_Customer_Name", difference.DisplayName);
        Assert.Equal("SalesDb.dbo.Customer.index.IX_Customer_Name", difference.SourceId);
        Assert.Equal("SalesDb.dbo.Customer.index.IX_Customer_Name", difference.LiveId);
    }

    private static MetaSqlModel CreateCustomerModel(bool includeExtraLiveColumn, bool sourceIndexUnique = false)
    {
        var model = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = "SalesDb",
            Name = "SalesDb",
            Platform = "sqlserver",
        };
        var schema = new Schema
        {
            Id = "SalesDb.dbo",
            Name = "dbo",
            DatabaseId = database.Id,
            Database = database,
        };
        var table = new Table
        {
            Id = "SalesDb.dbo.Customer",
            Name = "Customer",
            SchemaId = schema.Id,
            Schema = schema,
        };
        var idColumn = new TableColumn
        {
            Id = "SalesDb.dbo.Customer.CustomerId",
            Name = "CustomerId",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = table.Id,
            Table = table,
        };
        var nameColumn = new TableColumn
        {
            Id = "SalesDb.dbo.Customer.CustomerName",
            Name = "CustomerName",
            Ordinal = "2",
            MetaDataTypeId = "sqlserver:type:nvarchar",
            IsNullable = "true",
            TableId = table.Id,
            Table = table,
        };

        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        model.TableList.Add(table);
        model.TableColumnList.Add(idColumn);
        model.TableColumnList.Add(nameColumn);
        model.PrimaryKeyList.Add(new PrimaryKey
        {
            Id = "SalesDb.dbo.Customer.pk.PK_Customer",
            Name = "PK_Customer",
            TableId = table.Id,
            Table = table,
        });
        model.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
        {
            Id = "SalesDb.dbo.Customer.pk.PK_Customer.column.1",
            PrimaryKeyId = "SalesDb.dbo.Customer.pk.PK_Customer",
            TableColumnId = idColumn.Id,
            TableColumn = idColumn,
            Ordinal = "1",
        });
        model.IndexList.Add(new Index
        {
            Id = "SalesDb.dbo.Customer.index.IX_Customer_Name",
            Name = "IX_Customer_Name",
            TableId = table.Id,
            Table = table,
            IsUnique = sourceIndexUnique ? "true" : "false",
        });
        model.IndexColumnList.Add(new IndexColumn
        {
            Id = "SalesDb.dbo.Customer.index.IX_Customer_Name.column.1",
            IndexId = "SalesDb.dbo.Customer.index.IX_Customer_Name",
            TableColumnId = nameColumn.Id,
            TableColumn = nameColumn,
            Ordinal = "1",
        });

        if (includeExtraLiveColumn)
        {
            model.TableColumnList.Add(new TableColumn
            {
                Id = "SalesDb.dbo.Customer.LegacyCode",
                Name = "LegacyCode",
                Ordinal = "3",
                MetaDataTypeId = "sqlserver:type:nvarchar",
                IsNullable = "true",
                TableId = table.Id,
                Table = table,
            });
        }

        return model;
    }

    private static Workspace CreateWorkspace(MetaSqlModel model, string leafName)
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), leafName);
        model.SaveToXmlWorkspace(workspacePath);
        return new WorkspaceService().LoadAsync(workspacePath, searchUpward: false).GetAwaiter().GetResult();
    }
}
