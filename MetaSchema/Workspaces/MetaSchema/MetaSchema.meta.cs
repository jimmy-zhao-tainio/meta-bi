#nullable enable
using System;
using System.Collections.Generic;

namespace MetaSchema;
public sealed partial class Field
{
    public string Id { get; set; } = null !;
    public string? IdentityIncrement { get; set; }
    public string? IdentitySeed { get; set; }
    public string? IsIdentity { get; set; }
    public string? IsNullable { get; set; }
    public string MetaDataTypeId { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string? Ordinal { get; set; }
    public SchemaObject SchemaObject { get; set; } = null !;
}

public sealed partial class FieldDataTypeDetail
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public string Value { get; set; } = null !;
    public Field Field { get; set; } = null !;
}

public sealed partial class Key
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Table Table { get; set; } = null !;
}

public sealed partial class KeyField
{
    public string Id { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public Field Field { get; set; } = null !;
    public Key Key { get; set; } = null !;
}

public sealed partial class PrimaryKey
{
    public string Id { get; set; } = null !;
    public Key Key { get; set; } = null !;
}

public sealed partial class Schema
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public System System { get; set; } = null !;
}

public sealed partial class SchemaObject
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Schema Schema { get; set; } = null !;
}

public sealed partial class System
{
    public string Id { get; set; } = null !;
    public string? Description { get; set; }
    public string Name { get; set; } = null !;
}

public sealed partial class Table
{
    public string Id { get; set; } = null !;
    public SchemaObject SchemaObject { get; set; } = null !;
}

public sealed partial class TableRelationship
{
    public string Id { get; set; } = null !;
    public string Name { get; set; } = null !;
    public Table SourceTable { get; set; } = null !;
    public Table TargetTable { get; set; } = null !;
}

public sealed partial class TableRelationshipField
{
    public string Id { get; set; } = null !;
    public string Ordinal { get; set; } = null !;
    public Field SourceField { get; set; } = null !;
    public TableRelationship TableRelationship { get; set; } = null !;
    public Field TargetField { get; set; } = null !;
}

public sealed partial class UniqueKey
{
    public string Id { get; set; } = null !;
    public Key Key { get; set; } = null !;
}

public sealed partial class View
{
    public string Id { get; set; } = null !;
    public SchemaObject SchemaObject { get; set; } = null !;
}

public sealed partial class MetaSchemaModel
{
    public static MetaSchemaModel CreateEmpty() => new();
    public List<Field> FieldList { get; set; } = new();
    public List<FieldDataTypeDetail> FieldDataTypeDetailList { get; set; } = new();
    public List<Key> KeyList { get; set; } = new();
    public List<KeyField> KeyFieldList { get; set; } = new();
    public List<PrimaryKey> PrimaryKeyList { get; set; } = new();
    public List<Schema> SchemaList { get; set; } = new();
    public List<SchemaObject> SchemaObjectList { get; set; } = new();
    public List<System> SystemList { get; set; } = new();
    public List<Table> TableList { get; set; } = new();
    public List<TableRelationship> TableRelationshipList { get; set; } = new();
    public List<TableRelationshipField> TableRelationshipFieldList { get; set; } = new();
    public List<UniqueKey> UniqueKeyList { get; set; } = new();
    public List<View> ViewList { get; set; } = new();
}

public static partial class MetaSchemaInstance
{
    private static readonly MetaSchemaModel _builtIn = CreateBuiltIn();
    public static MetaSchemaModel BuiltIn => _builtIn;

    public static MetaSchemaModel CreateBuiltIn()
    {
        var model = MetaSchemaModel.CreateEmpty();
        return model;
    }
}