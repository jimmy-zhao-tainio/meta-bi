using MetaBusinessDataVault;
using MetaDataVaultImplementation;
using MetaDataType;
using MetaDataTypeConversion;
using MetaSql;

namespace MetaConvert.DataVaultToSql;

// Frozen reference for proving the sanctioned weave. Product execution does not compile this code.
internal static partial class BusinessDataVaultToSqlCSharpReference
{
    public static MetaSqlModel ConvertToMetaSql(
        MetaBusinessDataVaultModel model,
        MetaDataVaultImplementationModel implementation,
        string databaseName)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(implementation);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        var context = CreateContext(
            databaseName,
            implementation,
            SqlServerBusinessTypeLowering.Create(
                MetaDataTypeInstance.BuiltIn,
                MetaDataTypeConversionInstance.BuiltIn));
        return ConvertBusiness(model, context);
    }

    private static ConversionContext CreateContext(
        string databaseName,
        MetaDataVaultImplementationModel implementationModel,
        SqlServerBusinessTypeLowering businessTypeLowering)
    {
        RequireSqlServerIdentifier(databaseName, "database");
        var metaSqlModel = MetaSqlModel.CreateEmpty();
        var database = new Database
        {
            Id = databaseName,
            Name = databaseName,
        };
        metaSqlModel.DatabaseList.Add(database);

        var schemasByName = new Dictionary<string, Schema>(StringComparer.OrdinalIgnoreCase);
        foreach (var schemaName in GetSchemaNames(implementationModel))
        {
            RequireSqlServerIdentifier(schemaName, "schema");
            if (schemasByName.ContainsKey(schemaName))
            {
                continue;
            }

            var schema = new Schema
            {
                Id = $"{database.Id}.{schemaName}",
                Name = schemaName,
                Database = database,
            };
            metaSqlModel.SchemaList.Add(schema);
            schemasByName[schemaName] = schema;
        }

        return new ConversionContext
        {
            DatabaseName = databaseName,
            ImplementationModel = implementationModel,
            BusinessTypeLowering = businessTypeLowering,
            MetaSql = metaSqlModel,
            Database = database,
            SchemasByName = schemasByName,
        };
    }

    private static IEnumerable<string> GetSchemaNames(MetaDataVaultImplementationModel implementationModel)
    {
        return implementationModel.BusinessHierarchicalLinkImplementationList.Select(row => row.SchemaName)
            .Concat(implementationModel.BusinessHierarchicalLinkSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessBridgeImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessHubImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessHubSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessLinkImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessLinkSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessPointInTimeImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessReferenceImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessReferenceSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessSameAsLinkImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.BusinessSameAsLinkSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.RawHubImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.RawHubSatelliteImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.RawLinkImplementationList.Select(row => row.SchemaName))
            .Concat(implementationModel.RawLinkSatelliteImplementationList.Select(row => row.SchemaName))
            .Where(row => !string.IsNullOrWhiteSpace(row))
            .Select(row => row.Trim());
    }

    private static Dictionary<string, List<T>> GroupById<T>(
        IEnumerable<T> rows,
        Func<T, string> keySelector)
        where T : class
    {
        var groups = new Dictionary<string, List<T>>(StringComparer.Ordinal);
        foreach (var row in rows)
        {
            var key = keySelector(row);
            if (!groups.TryGetValue(key, out var bucket))
            {
                bucket = new List<T>();
                groups[key] = bucket;
            }

            bucket.Add(row);
        }

        return groups;
    }

    private static IReadOnlyList<T> GetGroup<T>(
        IReadOnlyDictionary<string, List<T>> groups,
        string key)
        where T : class
    {
        return groups.TryGetValue(key, out var bucket)
            ? bucket
            : Array.Empty<T>();
    }
}
