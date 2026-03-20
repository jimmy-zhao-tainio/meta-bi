using Meta.Core.Services;

namespace MetaSql.Tests;

public sealed class MetaSqlDeployManifestServiceTests
{
    [Fact]
    public void BuildManifest_MapsAddDropAndBlockEntries()
    {
        var sourceWorkspace = CreateWorkspace(
            CreateModel(
                includeSourceOnlyColumn: true,
                includeLiveOnlyColumn: false,
                sourceNameLength: 50),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreateModel(
                includeSourceOnlyColumn: false,
                includeLiveOnlyColumn: true,
                sourceNameLength: 100),
            "live");

        var differenceService = new MetaSqlDifferenceService();
        var differences = differenceService.BuildDifferences(sourceWorkspace, liveWorkspace);

        var service = new MetaSqlDeployManifestService();
        var manifest = service.BuildManifest(
            sourceWorkspace,
            liveWorkspace,
            differences,
            manifestName: "TestManifest",
            targetDescription: "Scope=raw:*");

        Assert.True(manifest.BlockCount > 0);
        Assert.Equal(1, manifest.AddCount);
        Assert.Equal(1, manifest.DropCount);
        Assert.Equal(1, manifest.BlockCount);
        Assert.False(manifest.IsDeployable);

        var root = Assert.Single(manifest.ManifestModel.DeployManifestList);
        Assert.False(string.IsNullOrWhiteSpace(root.SourceInstanceFingerprint));
        Assert.False(string.IsNullOrWhiteSpace(root.LiveInstanceFingerprint));
        Assert.Equal("Scope=raw:*", root.TargetDescription);
        Assert.Single(manifest.ManifestModel.AddTableColumnList);
        Assert.Single(manifest.ManifestModel.DropTableColumnList);
        Assert.Single(manifest.ManifestModel.BlockTableColumnDifferenceList);
    }

    private static MetaSqlModel CreateModel(bool includeSourceOnlyColumn, bool includeLiveOnlyColumn, int sourceNameLength)
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

        var customerId = new TableColumn
        {
            Id = "SalesDb.dbo.Customer.CustomerId",
            Name = "CustomerId",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            TableId = table.Id,
            Table = table,
        };
        var customerName = new TableColumn
        {
            Id = "SalesDb.dbo.Customer.CustomerName",
            Name = "CustomerName",
            Ordinal = "2",
            MetaDataTypeId = "sqlserver:type:nvarchar",
            IsNullable = "true",
            TableId = table.Id,
            Table = table,
        };
        var customerNameLength = new TableColumnDataTypeDetail
        {
            Id = $"SalesDb.dbo.Customer.CustomerName.detail.Length.{sourceNameLength}",
            Name = "Length",
            Value = sourceNameLength.ToString(),
            TableColumnId = customerName.Id,
            TableColumn = customerName,
        };

        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        model.TableList.Add(table);
        model.TableColumnList.Add(customerId);
        model.TableColumnList.Add(customerName);
        model.TableColumnDataTypeDetailList.Add(customerNameLength);

        if (includeSourceOnlyColumn)
        {
            model.TableColumnList.Add(new TableColumn
            {
                Id = "SalesDb.dbo.Customer.NewCode",
                Name = "NewCode",
                Ordinal = "3",
                MetaDataTypeId = "sqlserver:type:nvarchar",
                IsNullable = "true",
                TableId = table.Id,
                Table = table,
            });
        }

        if (includeLiveOnlyColumn)
        {
            model.TableColumnList.Add(new TableColumn
            {
                Id = "SalesDb.dbo.Customer.LegacyCode",
                Name = "LegacyCode",
                Ordinal = "4",
                MetaDataTypeId = "sqlserver:type:nvarchar",
                IsNullable = "true",
                TableId = table.Id,
                Table = table,
            });
        }

        return model;
    }

    private static Meta.Core.Domain.Workspace CreateWorkspace(MetaSqlModel model, string leafName)
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), leafName);
        model.SaveToXmlWorkspace(workspacePath);
        return new WorkspaceService().LoadAsync(workspacePath, searchUpward: false).GetAwaiter().GetResult();
    }
}
