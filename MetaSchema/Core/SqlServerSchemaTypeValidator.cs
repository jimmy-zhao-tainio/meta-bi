using MetaDataType;

namespace MetaSchema;

/// <summary>Resolves schema types through a catalog and enforces the SQL Server schema contract.</summary>
public sealed class SqlServerSchemaTypeValidator(MetaDataTypeModel catalog)
{
    private readonly MetaDataTypeModel catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));

    public DataType Resolve(string? typeId, string fieldDescription)
    {
        if (string.IsNullOrWhiteSpace(typeId))
            throw new SchemaTypeValidationException("Missing", $"{fieldDescription} is missing MetaDataTypeId.");
        var matches = catalog.DataTypeList.Where(type => type.Id == typeId).ToArray();
        if (matches.Length != 1)
            throw new SchemaTypeValidationException("Unknown", $"{fieldDescription} has MetaDataTypeId '{typeId}', which resolves to {matches.Length} types in the selected MetaDataType catalog; expected one.");
        if (!IsSqlServer(matches[0]))
            throw new SchemaTypeValidationException("NonSqlServer", $"{fieldDescription} has MetaDataTypeId '{typeId}', which does not belong to the SQL Server type system in the selected MetaDataType catalog.");
        return matches[0];
    }

    public string ResolveSqlTypeName(string name)
    {
        var matches = catalog.DataTypeList.Where(type => IsSqlServer(type) &&
            string.Equals(type.Name, name, StringComparison.OrdinalIgnoreCase)).ToArray();
        if (matches.Length != 1)
            throw new SchemaTypeValidationException("SqlTypeName", $"SQL expression type '{name}' resolves to {matches.Length} SQL Server types in the selected MetaDataType catalog; expected one.");
        return Resolve(matches[0].Id, $"SQL expression type '{name}'").Id;
    }

    public void Validate(MetaSchemaModel schema)
    {
        ArgumentNullException.ThrowIfNull(schema);
        foreach (var field in schema.FieldList)
        {
            var obj = field.SchemaObject;
            Resolve(field.MetaDataTypeId, $"Schema field '{obj?.Schema?.System?.Name}.{obj?.Schema?.Name}.{obj?.Name}.{field.Name}' (field ID '{field.Id}')");
        }
    }

    private bool IsSqlServer(DataType type)
    {
        var systems = catalog.DataTypeSystemList.Where(system => system.Id == type.DataTypeSystem?.Id).ToArray();
        return systems.Length == 1 && string.Equals(systems[0].Name, "SqlServer", StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class SchemaTypeValidationException(string code, string message) : InvalidOperationException(message)
{
    public string Code { get; } = code;
}
