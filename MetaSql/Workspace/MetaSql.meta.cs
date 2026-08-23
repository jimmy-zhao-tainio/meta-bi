#nullable enable
using System;
using System.Collections.Generic;

namespace MetaSql;
public sealed partial class Database
{
    public string Id { get; set; } = null !;
    public string? Collation { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class ForeignKey
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Table SourceTable { get; set; } = null !;
    public Table TargetTable { get; set; } = null !;
}

public sealed partial class ForeignKeyColumn
{
    public string Id { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public ForeignKey ForeignKey { get; set; } = null !;
    public TableColumn SourceColumn { get; set; } = null !;
    public TableColumn TargetColumn { get; set; } = null !;
}

public sealed partial class Function
{
    public string Id { get; set; } = null !;
    public string DefinitionSql { get; set; } = null !;
    public string? DeployOrdinal { get; set; }
    public string FunctionKind { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Schema Schema { get; set; } = null !;
}

public sealed partial class Index
{
    public string Id { get; set; } = null !;
    public string? FilterSql { get; set; }
    public string? IsClustered { get; set; }
    public string? IsUnique { get; set; }
    public string Name { get; set; } = null !;
    public Table Table { get; set; } = null !;
}

public sealed partial class IndexColumn
{
    public string Id { get; set; } = null !;
    public string? IsDescending { get; set; }
    public string? IsIncluded { get; set; }
    public string Ordinal { get; set; } = null !;
    public Index Index { get; set; } = null !;
    public TableColumn TableColumn { get; set; } = null !;
}

public sealed partial class PrimaryKey
{
    public string Id { get; set; } = null !;
    public string? IsClustered { get; set; }
    public string Name { get; set; } = null !;
    public Table Table { get; set; } = null !;
}

public sealed partial class PrimaryKeyColumn
{
    public string Id { get; set; } = null !;
    public string? IsDescending { get; set; }
    public string Ordinal { get; set; } = null !;
    public PrimaryKey PrimaryKey { get; set; } = null !;
    public TableColumn TableColumn { get; set; } = null !;
}

public sealed partial class Schema
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Database Database { get; set; } = null !;
}

public sealed partial class StoredProcedure
{
    public string Id { get; set; } = null !;
    public string DefinitionSql { get; set; } = null !;
    public string? DeployOrdinal { get; set; }
    public string Name { get; set; } = null !;
    public Schema Schema { get; set; } = null !;
}

public sealed partial class Table
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Schema Schema { get; set; } = null !;
}

public sealed partial class TableColumn
{
    public string Id { get; set; } = null !;
    public string? DefaultExpressionSql { get; set; }
    public string? ExpressionSql { get; set; }
    public string? IdentityIncrement { get; set; }
    public string? IdentitySeed { get; set; }
    public string? IsIdentity { get; set; }
    public string? IsNullable { get; set; }
    public string MetaDataTypeId { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string? Ordinal { get; set; }
    public Table Table { get; set; } = null !;
}

public sealed partial class TableColumnDataTypeDetail
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Value { get; set; } = null !;
    public TableColumn TableColumn { get; set; } = null !;
}

public sealed partial class View
{
    public string Id { get; set; } = null !;
    public string DefinitionSql { get; set; } = null !;
    public string? DeployOrdinal { get; set; }
    public string Name { get; set; } = null !;
    public Schema Schema { get; set; } = null !;
}

public sealed partial class MetaSqlModel
{
    public static MetaSqlModel CreateEmpty() => new();
    public List<Database> DatabaseList { get; set; } = new();
    public List<ForeignKey> ForeignKeyList { get; set; } = new();
    public List<ForeignKeyColumn> ForeignKeyColumnList { get; set; } = new();
    public List<Function> FunctionList { get; set; } = new();
    public List<Index> IndexList { get; set; } = new();
    public List<IndexColumn> IndexColumnList { get; set; } = new();
    public List<PrimaryKey> PrimaryKeyList { get; set; } = new();
    public List<PrimaryKeyColumn> PrimaryKeyColumnList { get; set; } = new();
    public List<Schema> SchemaList { get; set; } = new();
    public List<StoredProcedure> StoredProcedureList { get; set; } = new();
    public List<Table> TableList { get; set; } = new();
    public List<TableColumn> TableColumnList { get; set; } = new();
    public List<TableColumnDataTypeDetail> TableColumnDataTypeDetailList { get; set; } = new();
    public List<View> ViewList { get; set; } = new();
}

public static partial class MetaSqlInstance
{
    private static readonly MetaSqlModel _builtIn = CreateBuiltIn();
    public static MetaSqlModel BuiltIn => _builtIn;

    public static MetaSqlModel CreateBuiltIn()
    {
        var model = MetaSqlModel.CreateEmpty();
        return model;
    }
}