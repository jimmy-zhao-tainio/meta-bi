using Meta.Core.Domain;

namespace MetaSql.Tests;

public sealed class MetaSqlDiffServiceTests
{
    [Fact]
    public async Task BuildEqualDiffWorkspaceAsync_ReturnsNoDifferencesForMatchingMetaSqlWorkspaces()
    {
        var tempRoot = CreateTempRoot();
        try
        {
            var desiredPath = Path.Combine(tempRoot, "desired");
            var livePath = Path.Combine(tempRoot, "live");

            await SaveWorkspaceAsync(desiredPath, CreateCustomerModel(includeExtraLiveColumn: false));
            await SaveWorkspaceAsync(livePath, CreateCustomerModel(includeExtraLiveColumn: false));

            var service = new MetaSqlDiffService();
            var result = await service.BuildEqualDiffWorkspaceAsync(desiredPath, livePath);

            Assert.False(result.HasDifferences);
            Assert.Equal(0, result.LeftNotInRightCount);
            Assert.Equal(0, result.RightNotInLeftCount);
            Assert.Equal(livePath + ".instance-diff", result.DiffWorkspacePath);
        }
        finally
        {
            DeleteIfExists(tempRoot);
        }
    }

    [Fact]
    public void BuildEqualDiffWorkspace_ReturnsDifferencesForLiveOnlyColumn()
    {
        var desiredWorkspace = CreateCustomerModel(includeExtraLiveColumn: false)
            .ToXmlWorkspace(Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), "desired"));
        var liveWorkspace = CreateCustomerModel(includeExtraLiveColumn: true)
            .ToXmlWorkspace(Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), "live"));

        var service = new MetaSqlDiffService();
        var result = service.BuildEqualDiffWorkspace(desiredWorkspace, liveWorkspace, liveWorkspace.WorkspaceRootPath!);

        Assert.True(result.HasDifferences);
        Assert.True(result.RightRowCount > result.LeftRowCount);
    }

    [Fact]
    public void BuildEqualDiffWorkspace_RejectsNonMetaSqlWorkspace()
    {
        var desiredWorkspace = CreateCustomerModel(includeExtraLiveColumn: false)
            .ToXmlWorkspace(Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), "desired"));
        var liveWorkspace = CreateCustomerModel(includeExtraLiveColumn: false)
            .ToXmlWorkspace(Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), "live"));
        liveWorkspace.Model.Name = "OtherModel";

        var service = new MetaSqlDiffService();
        var exception = Assert.Throws<InvalidOperationException>(() =>
            service.BuildEqualDiffWorkspace(desiredWorkspace, liveWorkspace, liveWorkspace.WorkspaceRootPath!));

        Assert.Contains("MetaSql model", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static MetaSqlModel CreateCustomerModel(bool includeExtraLiveColumn)
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

    private static async Task SaveWorkspaceAsync(string workspacePath, MetaSqlModel model)
    {
        var workspace = model.ToXmlWorkspace(workspacePath);
        await MetaSqlTooling.SaveWorkspaceAsync(workspace);
    }

    private static string CreateTempRoot() =>
        Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"));

    private static void DeleteIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
