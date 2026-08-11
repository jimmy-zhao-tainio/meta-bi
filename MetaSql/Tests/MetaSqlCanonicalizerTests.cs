using Meta.Integration;
using Meta.Operations.Domain;

namespace MetaSql.Tests;

public sealed class MetaSqlCanonicalizerTests
{
    [Fact]
    public void Canonicalize_NormalizesMetaSqlDefaultsWithoutMutatingTheSource()
    {
        var source = CreateWorkspace();

        var canonical = MetaSqlCanonicalizer.Canonicalize(source);

        var sourceColumn = GetRecord(source, "TableColumn", "SalesDb.dbo.Customer.CustomerId");
        Assert.Equal(string.Empty, sourceColumn.Values["IsIdentity"]);
        Assert.Equal("   ", sourceColumn.Values["ExpressionSql"]);

        var customerId = GetRecord(canonical, "TableColumn", "SalesDb.dbo.Customer.CustomerId");
        Assert.Equal("false", customerId.Values["IsNullable"]);
        Assert.DoesNotContain("IsIdentity", customerId.Values.Keys);
        Assert.DoesNotContain("IdentitySeed", customerId.Values.Keys);
        Assert.DoesNotContain("ExpressionSql", customerId.Values.Keys);
        Assert.DoesNotContain("DefaultExpressionSql", customerId.Values.Keys);

        var displayName = GetRecord(canonical, "TableColumn", "SalesDb.dbo.Customer.DisplayName");
        Assert.Equal("true", displayName.Values["IsNullable"]);
        Assert.Equal("true", displayName.Values["IsIdentity"]);
        Assert.Equal("2", displayName.Values["IdentitySeed"]);
        Assert.Equal("([FirstName] + [LastName])", displayName.Values["ExpressionSql"]);
        Assert.Equal("('unknown')", displayName.Values["DefaultExpressionSql"]);

        var primaryKey = GetRecord(canonical, "PrimaryKey", "SalesDb.dbo.Customer.pk.PK_Customer");
        var primaryKeyColumn = GetRecord(canonical, "PrimaryKeyColumn", "SalesDb.dbo.Customer.pk.PK_Customer.column.1");
        Assert.DoesNotContain("IsClustered", primaryKey.Values.Keys);
        Assert.Equal("true", primaryKeyColumn.Values["IsDescending"]);

        var index = GetRecord(canonical, "Index", "SalesDb.dbo.Customer.index.IX_Customer");
        var indexColumn = GetRecord(canonical, "IndexColumn", "SalesDb.dbo.Customer.index.IX_Customer.column.1");
        Assert.Equal("true", index.Values["IsUnique"]);
        Assert.DoesNotContain("IsClustered", index.Values.Keys);
        Assert.Equal("[DisplayName] IS NOT NULL", index.Values["FilterSql"]);
        Assert.DoesNotContain("IsIncluded", indexColumn.Values.Keys);
        Assert.Equal("true", indexColumn.Values["IsDescending"]);

        var canonicalAgain = MetaSqlCanonicalizer.Canonicalize(canonical);
        Assert.Null(InMemoryWorkspaceComparer.FindDifference(canonical, canonicalAgain));
    }

    [Fact]
    public void DiffService_TreatsCanonicalDefaultsAsEqualAndReportsMeaningfulChanges()
    {
        var authored = CreateWorkspace();
        var canonical = MetaSqlCanonicalizer.Canonicalize(authored);
        var service = new MetaSqlDiffService();

        Assert.False(service.BuildEqualDiffWorkspace(authored, canonical).HasDifferences);
        Assert.Empty(new MetaSqlDifferenceService().BuildDifferences(authored, canonical));
        Assert.Equal(
            MetaSqlInstanceFingerprint.Compute(authored),
            MetaSqlInstanceFingerprint.Compute(canonical));

        var changed = canonical.Clone();
        GetRecord(changed, "TableColumn", "SalesDb.dbo.Customer.DisplayName").Values["IsNullable"] = "false";
        Assert.True(service.BuildEqualDiffWorkspace(canonical, changed).HasDifferences);
    }

    [Fact]
    public void GenericWorkspaceComparison_StillDistinguishesMissingFromExplicitEmpty()
    {
        var missing = CreateWorkspace();
        var empty = missing.Clone();
        GetRecord(empty, "PrimaryKey", "SalesDb.dbo.Customer.pk.PK_Customer").Values["IsClustered"] = string.Empty;

        Assert.NotNull(InMemoryWorkspaceComparer.FindDifference(missing, empty));
    }

    [Fact]
    public void Canonicalize_RejectsMalformedBooleanValues()
    {
        var workspace = CreateWorkspace();
        GetRecord(workspace, "Index", "SalesDb.dbo.Customer.index.IX_Customer").Values["IsUnique"] = "sometimes";

        var exception = Assert.Throws<InvalidOperationException>(() => MetaSqlCanonicalizer.Canonicalize(workspace));

        Assert.Contains("IX_Customer", exception.Message, StringComparison.Ordinal);
        Assert.Contains("IsUnique", exception.Message, StringComparison.Ordinal);
    }

    private static InMemoryWorkspace CreateWorkspace()
    {
        var model = MetaSqlModel.CreateEmpty();
        var database = new Database { Id = "SalesDb", Name = "SalesDb", Collation = " " };
        var schema = new Schema { Id = "SalesDb.dbo", Name = "dbo", Database = database };
        var table = new Table { Id = "SalesDb.dbo.Customer", Name = "Customer", Schema = schema };
        var customerId = new TableColumn
        {
            Id = "SalesDb.dbo.Customer.CustomerId",
            Name = "CustomerId",
            Ordinal = "1",
            MetaDataTypeId = "sqlserver:type:int",
            IsIdentity = string.Empty,
            IdentitySeed = " ",
            ExpressionSql = "   ",
            DefaultExpressionSql = string.Empty,
            Table = table,
        };
        var displayName = new TableColumn
        {
            Id = "SalesDb.dbo.Customer.DisplayName",
            Name = "DisplayName",
            Ordinal = "2",
            MetaDataTypeId = "sqlserver:type:nvarchar",
            IsNullable = " TRUE ",
            IsIdentity = "true",
            IdentitySeed = "2",
            ExpressionSql = "([FirstName] + [LastName])",
            DefaultExpressionSql = "('unknown')",
            Table = table,
        };
        var primaryKey = new PrimaryKey
        {
            Id = "SalesDb.dbo.Customer.pk.PK_Customer",
            Name = "PK_Customer",
            IsClustered = " false ",
            Table = table,
        };
        var primaryKeyColumn = new PrimaryKeyColumn
        {
            Id = "SalesDb.dbo.Customer.pk.PK_Customer.column.1",
            Ordinal = "1",
            IsDescending = " TRUE ",
            PrimaryKey = primaryKey,
            TableColumn = customerId,
        };
        var index = new Index
        {
            Id = "SalesDb.dbo.Customer.index.IX_Customer",
            Name = "IX_Customer",
            IsUnique = "true",
            IsClustered = " ",
            FilterSql = "[DisplayName] IS NOT NULL",
            Table = table,
        };
        var indexColumn = new IndexColumn
        {
            Id = "SalesDb.dbo.Customer.index.IX_Customer.column.1",
            Ordinal = "1",
            IsDescending = "true",
            IsIncluded = "false",
            Index = index,
            TableColumn = displayName,
        };
        var view = new View
        {
            Id = "SalesDb.dbo.view.v_Customer",
            Name = "v_Customer",
            DefinitionSql = "select CustomerId from dbo.Customer",
            DeployOrdinal = " ",
            Schema = schema,
        };

        model.DatabaseList.Add(database);
        model.SchemaList.Add(schema);
        model.TableList.Add(table);
        model.TableColumnList.AddRange([customerId, displayName]);
        model.PrimaryKeyList.Add(primaryKey);
        model.PrimaryKeyColumnList.Add(primaryKeyColumn);
        model.IndexList.Add(index);
        model.IndexColumnList.Add(indexColumn);
        model.ViewList.Add(view);
        return TypedWorkspaceModelMapper.ToInMemoryWorkspace(model);
    }

    private static GenericRecord GetRecord(InMemoryWorkspace workspace, string entityName, string id)
    {
        return workspace.Instance.RecordsByEntity[entityName].Single(record => record.Id == id);
    }
}
