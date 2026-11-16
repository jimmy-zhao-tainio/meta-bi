using MetaDataType;
using Meta.Operations.Domain;

namespace MetaSql.Extractors.SqlServer;

/// <summary>Resolves SQL Server type identities through one selected modeled catalog.</summary>
public sealed class SqlServerDataTypeCatalog(MetaDataTypeModel catalog)
{
    private readonly MetaDataTypeModel catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));

    public DataType ResolveId(string? id, string description)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new InvalidOperationException($"{description} is missing MetaDataTypeId.");
        var matches = catalog.DataTypeList.Where(type => type.Id == id).ToArray();
        if (matches.Length != 1)
            throw new InvalidOperationException($"{description} has MetaDataTypeId '{id}', which resolves to {matches.Length} types in the selected MetaDataType catalog; expected one.");
        if (!IsSqlServer(matches[0]) || string.IsNullOrWhiteSpace(matches[0].Name))
            throw new InvalidOperationException($"{description} has MetaDataTypeId '{id}', which does not resolve to a named SQL Server type in the selected MetaDataType catalog.");
        return matches[0];
    }

    public DataType ResolveName(string name, string description)
    {
        var matches = catalog.DataTypeList.Where(type => IsSqlServer(type) &&
            string.Equals(type.Name, name, StringComparison.OrdinalIgnoreCase)).ToArray();
        if (matches.Length != 1)
            throw new InvalidOperationException($"{description} has SQL Server type name '{name}', which resolves to {matches.Length} types in the selected MetaDataType catalog; expected one.");
        return ResolveId(matches[0].Id, description);
    }

    public bool ContainsSqlServerId(string? id) => !string.IsNullOrWhiteSpace(id) &&
        catalog.DataTypeList.Count(type => type.Id == id) == 1 &&
        catalog.DataTypeList.Any(type => type.Id == id && IsSqlServer(type));

    public void ValidateColumns(InMemoryWorkspace workspace, string description)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        if (!workspace.Instance.RecordsByEntity.TryGetValue("TableColumn", out var columns)) return;
        foreach (var row in columns)
            ResolveId(row.Values.GetValueOrDefault("MetaDataTypeId"), $"{description} column '{row.Id}'");
    }

    private bool IsSqlServer(DataType type)
    {
        var systems = catalog.DataTypeSystemList.Where(system => system.Id == type.DataTypeSystem?.Id).ToArray();
        return systems.Length == 1 && string.Equals(systems[0].Name, "SqlServer", StringComparison.OrdinalIgnoreCase);
    }
}
