using Meta.Core.Domain;
using Meta.Core.Serialization;

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

    [Fact]
    public void BuildDifferences_MatchesByScopeAndName_WhenSourceAndLiveIdsDiffer()
    {
        var sourceWorkspace = CreateWorkspace(
            CreateCustomerModel(includeExtraLiveColumn: false, sourceIndexUnique: true, idPrefix: "SourceDb"),
            "source");
        var liveWorkspace = CreateWorkspace(
            CreateCustomerModel(includeExtraLiveColumn: false, sourceIndexUnique: false, idPrefix: "LiveDb"),
            "live");

        var service = new MetaSqlDifferenceService();
        var differences = service.BuildDifferences(sourceWorkspace, liveWorkspace);

        var difference = Assert.Single(differences, row => row.ObjectKind == MetaSqlObjectKind.Index);
        Assert.Equal(MetaSqlObjectKind.Index, difference.ObjectKind);
        Assert.Equal(MetaSqlDifferenceKind.Different, difference.DifferenceKind);
        Assert.Equal("dbo.Customer", difference.ScopeDisplayName);
        Assert.Equal("IX_Customer_Name", difference.DisplayName);
        Assert.Equal("SourceDb.dbo.Customer.index.IX_Customer_Name", difference.SourceId);
        Assert.Equal("LiveDb.dbo.Customer.index.IX_Customer_Name", difference.LiveId);
        Assert.DoesNotContain(differences, row => row.ObjectKind == MetaSqlObjectKind.Index && row.DifferenceKind == MetaSqlDifferenceKind.MissingInLive);
        Assert.DoesNotContain(differences, row => row.ObjectKind == MetaSqlObjectKind.Index && row.DifferenceKind == MetaSqlDifferenceKind.ExtraInLive);
    }

    [Fact]
    public void BuildDifferences_ReturnsSqlModuleDifferences()
    {
        var sourceModel = CreateCustomerModel(includeExtraLiveColumn: false);
        AddView(sourceModel, "vCustomerReview", "CREATE OR ALTER VIEW [dbo].[vCustomerReview] AS SELECT [CustomerId] FROM [dbo].[Customer];");
        AddFunction(sourceModel, "fnCustomerScore", "ScalarFunction", "CREATE FUNCTION [dbo].[fnCustomerScore](@CustomerId int) RETURNS int AS BEGIN RETURN @CustomerId END;");
        AddStoredProcedure(sourceModel, "RunReview", "CREATE OR ALTER PROCEDURE [dbo].[RunReview] AS SELECT 1 AS [Result];");

        var liveModel = CreateCustomerModel(includeExtraLiveColumn: false);
        AddView(liveModel, "vCustomerReview", "CREATE OR ALTER VIEW [dbo].[vCustomerReview] AS SELECT [CustomerName] FROM [dbo].[Customer];");
        AddFunction(liveModel, "fnCustomerScore", "InlineTableValuedFunction", "CREATE FUNCTION [dbo].[fnCustomerScore](@CustomerId int) RETURNS TABLE AS RETURN SELECT @CustomerId AS CustomerId;");

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(liveModel, "live");

        var service = new MetaSqlDifferenceService();
        var differences = service.BuildDifferences(sourceWorkspace, liveWorkspace);

        var viewDifference = Assert.Single(differences, row => row.ObjectKind == MetaSqlObjectKind.View);
        Assert.Equal(MetaSqlDifferenceKind.Different, viewDifference.DifferenceKind);
        Assert.Equal("dbo", viewDifference.ScopeDisplayName);
        Assert.Equal("dbo.vCustomerReview", viewDifference.DisplayName);

        var procedureDifference = Assert.Single(differences, row => row.ObjectKind == MetaSqlObjectKind.StoredProcedure);
        Assert.Equal(MetaSqlDifferenceKind.MissingInLive, procedureDifference.DifferenceKind);
        Assert.Equal("dbo.RunReview", procedureDifference.DisplayName);

        var functionDifference = Assert.Single(differences, row => row.ObjectKind == MetaSqlObjectKind.Function);
        Assert.Equal(MetaSqlDifferenceKind.Different, functionDifference.DifferenceKind);
        Assert.Equal("dbo.fnCustomerScore", functionDifference.DisplayName);
    }

    [Fact]
    public void BuildDifferences_ThrowsOnAmbiguousTableScopeIdentity()
    {
        var sourceModel = CreateCustomerModel(includeExtraLiveColumn: false);
        var schema = Assert.Single(sourceModel.SchemaList);
        sourceModel.TableList.Add(new Table
        {
            Id = "SalesDb.dbo.Customer.Duplicate",
            Name = "Customer",
            Schema = schema,
        });

        var sourceWorkspace = CreateWorkspace(sourceModel, "source");
        var liveWorkspace = CreateWorkspace(CreateCustomerModel(includeExtraLiveColumn: false), "live");

        var service = new MetaSqlDifferenceService();
        var exception = Assert.Throws<InvalidOperationException>(() => service.BuildDifferences(sourceWorkspace, liveWorkspace));
        Assert.Contains("Ambiguous source table identity key", exception.Message, StringComparison.Ordinal);
    }

    private static MetaSqlModel CreateCustomerModel(bool includeExtraLiveColumn, bool sourceIndexUnique = false, string idPrefix = "SalesDb")
    {
        var model = MetaSqlModel.CreateEmpty();

        var database = new Database
        {
            Id = idPrefix,
            Name = "SalesDb",
        };
        var schema = new Schema
        {
            Id = $"{idPrefix}.dbo",
            Name = "dbo",
            Database = database,
        };
        var table = new Table
        {
            Id = $"{idPrefix}.dbo.Customer",
            Name = "Customer",
            Schema = schema,
        };
        var idColumn = new TableColumn
        {
            Id = $"{idPrefix}.dbo.Customer.CustomerId",
            Name = "CustomerId",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsNullable = "false",
            Table = table,
        };
        var nameColumn = new TableColumn
        {
            Id = $"{idPrefix}.dbo.Customer.CustomerName",
            Name = "CustomerName",
            Ordinal = "2",
            MetaDataTypeId = "sqlserver:type:nvarchar",
            IsNullable = "true",
            Table = table,
        };

        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        model.TableList.Add(table);
        model.TableColumnList.Add(idColumn);
        model.TableColumnList.Add(nameColumn);
        var primaryKeyId = $"{idPrefix}.dbo.Customer.pk.PK_Customer";
        var primaryKey = new PrimaryKey
        {
            Id = primaryKeyId,
            Name = "PK_Customer",
            Table = table,
        };
        model.PrimaryKeyList.Add(primaryKey);
        model.PrimaryKeyColumnList.Add(new PrimaryKeyColumn
        {
            Id = $"{primaryKeyId}.column.1",
            PrimaryKey = primaryKey,
            TableColumn = idColumn,
            Ordinal = "1",
        });
        var indexId = $"{idPrefix}.dbo.Customer.index.IX_Customer_Name";
        var index = new Index
        {
            Id = indexId,
            Name = "IX_Customer_Name",
            Table = table,
            IsUnique = sourceIndexUnique ? "true" : "false",
        };
        model.IndexList.Add(index);
        model.IndexColumnList.Add(new IndexColumn
        {
            Id = $"{indexId}.column.1",
            Index = index,
            TableColumn = nameColumn,
            Ordinal = "1",
        });

        if (includeExtraLiveColumn)
        {
            model.TableColumnList.Add(new TableColumn
            {
                Id = $"{idPrefix}.dbo.Customer.LegacyCode",
                Name = "LegacyCode",
                Ordinal = "3",
                MetaDataTypeId = "sqlserver:type:nvarchar",
                IsNullable = "true",
                Table = table,
            });
        }

        return model;
    }

    private static void AddView(MetaSqlModel model, string name, string definitionSql)
    {
        var schema = Assert.Single(model.SchemaList);
        model.ViewList.Add(new View
        {
            Id = $"{schema.Id}.view.{name}",
            Name = name,
            DefinitionSql = definitionSql,
            Schema = schema,
        });
    }

    private static void AddStoredProcedure(MetaSqlModel model, string name, string definitionSql)
    {
        var schema = Assert.Single(model.SchemaList);
        model.StoredProcedureList.Add(new StoredProcedure
        {
            Id = $"{schema.Id}.procedure.{name}",
            Name = name,
            DefinitionSql = definitionSql,
            Schema = schema,
        });
    }

    private static void AddFunction(MetaSqlModel model, string name, string functionKind, string definitionSql)
    {
        var schema = Assert.Single(model.SchemaList);
        model.FunctionList.Add(new Function
        {
            Id = $"{schema.Id}.function.{name}",
            Name = name,
            FunctionKind = functionKind,
            DefinitionSql = definitionSql,
            Schema = schema,
        });
    }

    private static InMemoryWorkspace CreateWorkspace(MetaSqlModel model, string leafName)
    {
        var workspacePath = Path.Combine(Path.GetTempPath(), "MetaSql.Tests", Guid.NewGuid().ToString("N"), leafName);
        model.SaveToXmlWorkspace(workspacePath);
        return XmlWorkspaceReader.OpenAsync(workspacePath).GetAwaiter().GetResult().State;
    }
}
