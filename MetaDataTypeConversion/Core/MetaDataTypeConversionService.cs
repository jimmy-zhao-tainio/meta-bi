using Meta.Core.Domain;

namespace MetaDataTypeConversion.Core;

public sealed record DataTypeMappingResolution(
    string MappingId,
    string SourceDataTypeId,
    string TargetDataTypeId,
    string TargetDataTypeSystemName,
    string ConversionImplementationId,
    string ConversionImplementationName,
    string? Notes);

public sealed record DataTypeCompatibilityResolution(
    string SourceDataTypeId,
    string TargetDataTypeId,
    IReadOnlyList<DataTypeMappingResolution> Path)
{
    public bool IsExact => string.Equals(SourceDataTypeId, TargetDataTypeId, StringComparison.Ordinal);
}

public sealed record MetaDataTypeConversionCheckResult(
    int MappingCount,
    int ImplementationCount,
    IReadOnlyList<string> Errors)
{
    public bool HasErrors => Errors.Count > 0;
}

public interface IMetaDataTypeConversionService
{
    MetaDataTypeConversionCheckResult Check(Workspace workspace);
    DataTypeMappingResolution Resolve(Workspace workspace, string sourceDataTypeId);
    DataTypeMappingResolution Resolve(Workspace workspace, string sourceDataTypeId, string targetDataTypeSystemName);
    DataTypeCompatibilityResolution ResolveCompatibility(Workspace workspace, string sourceDataTypeId, string targetDataTypeId);
}

public sealed class MetaDataTypeConversionService : IMetaDataTypeConversionService
{
    public MetaDataTypeConversionCheckResult Check(Workspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        var implementations = workspace.Instance.GetOrCreateEntityRecords("ConversionImplementation")
            .OrderBy(record => record.Id, StringComparer.Ordinal)
            .ToList();
        var mappings = workspace.Instance.GetOrCreateEntityRecords("DataTypeMapping")
            .OrderBy(record => record.Id, StringComparer.Ordinal)
            .ToList();

        var implementationById = implementations.ToDictionary(record => record.Id, StringComparer.Ordinal);
        var errors = new List<string>();

        foreach (var mapping in mappings)
        {
            var sourceDataTypeId = RequireValue(mapping, "SourceDataTypeId");
            var targetDataTypeId = RequireValue(mapping, "TargetDataTypeId");
            if (string.IsNullOrWhiteSpace(TryGetDataTypeSystemName(targetDataTypeId)))
            {
                errors.Add($"DataTypeMapping '{mapping.Id}' has TargetDataTypeId '{targetDataTypeId}' with an unsupported data type id shape.");
            }


            if (!mapping.RelationshipIds.TryGetValue("ConversionImplementationId", out var implementationId) ||
                string.IsNullOrWhiteSpace(implementationId))
            {
                errors.Add($"DataTypeMapping '{mapping.Id}' is missing required relationship 'ConversionImplementationId'.");
                continue;
            }

            if (!implementationById.ContainsKey(implementationId))
            {
                errors.Add($"DataTypeMapping '{mapping.Id}' references missing ConversionImplementation '{implementationId}'.");
            }
        }

        var duplicateSources = mappings
            .GroupBy(
                record => new DataTypeMappingKey(
                    RequireValue(record, "SourceDataTypeId"),
                    NormalizeDataTypeSystemName(TryGetDataTypeSystemName(RequireValue(record, "TargetDataTypeId")) ?? string.Empty)),
                DataTypeMappingKeyComparer.Instance)
            .Where(group => group.Count() > 1)
            .OrderBy(group => group.Key.SourceDataTypeId, StringComparer.Ordinal)
            .ThenBy(group => group.Key.TargetDataTypeSystemName, StringComparer.Ordinal);

        foreach (var duplicateSource in duplicateSources)
        {
            var ids = string.Join(", ", duplicateSource.Select(record => record.Id).OrderBy(id => id, StringComparer.Ordinal));
            errors.Add($"SourceDataTypeId '{duplicateSource.Key.SourceDataTypeId}' is mapped more than once for target data type system '{duplicateSource.Key.TargetDataTypeSystemName}' ({ids}).");
        }

        return new MetaDataTypeConversionCheckResult(mappings.Count, implementations.Count, errors);
    }

    public DataTypeMappingResolution Resolve(Workspace workspace, string sourceDataTypeId)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDataTypeId);

        var check = Check(workspace);
        if (check.HasErrors)
        {
            throw new InvalidOperationException("MetaDataTypeConversion workspace is invalid. Run 'meta-data-type-conversion check' first.");
        }

        var mappings = workspace.Instance.GetOrCreateEntityRecords("DataTypeMapping")
            .Where(record => string.Equals(RequireValue(record, "SourceDataTypeId"), sourceDataTypeId, StringComparison.Ordinal))
            .ToList();

        if (mappings.Count == 0)
        {
            throw new InvalidOperationException($"No DataTypeMapping exists for source data type '{sourceDataTypeId}'.");
        }

        if (mappings.Count > 1)
        {
            var targetSystems = string.Join(
                ", ",
                mappings
                    .Select(record => TryGetDataTypeSystemName(RequireValue(record, "TargetDataTypeId")) ?? "<unknown>")
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(value => value, StringComparer.OrdinalIgnoreCase));
            throw new InvalidOperationException(
                $"Source data type '{sourceDataTypeId}' resolves ambiguously to {mappings.Count} DataTypeMappings. Specify a target data type system. Available target systems: {targetSystems}.");
        }

        return CreateResolution(workspace, mappings[0]);
    }

    public DataTypeMappingResolution Resolve(
        Workspace workspace,
        string sourceDataTypeId,
        string targetDataTypeSystemName)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDataTypeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetDataTypeSystemName);

        var check = Check(workspace);
        if (check.HasErrors)
        {
            throw new InvalidOperationException("MetaDataTypeConversion workspace is invalid. Run 'meta-data-type-conversion check' first.");
        }

        var normalizedTargetSystem = NormalizeDataTypeSystemName(targetDataTypeSystemName);
        var mappings = workspace.Instance.GetOrCreateEntityRecords("DataTypeMapping")
            .Where(record => string.Equals(RequireValue(record, "SourceDataTypeId"), sourceDataTypeId, StringComparison.Ordinal))
            .Where(record => string.Equals(
                NormalizeDataTypeSystemName(TryGetDataTypeSystemName(RequireValue(record, "TargetDataTypeId")) ?? string.Empty),
                normalizedTargetSystem,
                StringComparison.Ordinal))
            .ToList();

        if (mappings.Count == 0)
        {
            throw new InvalidOperationException($"No DataTypeMapping exists for source data type '{sourceDataTypeId}' to target data type system '{targetDataTypeSystemName}'.");
        }

        if (mappings.Count > 1)
        {
            throw new InvalidOperationException($"Source data type '{sourceDataTypeId}' resolves ambiguously to {mappings.Count} DataTypeMappings for target data type system '{targetDataTypeSystemName}'.");
        }

        return CreateResolution(workspace, mappings[0]);
    }

    public DataTypeCompatibilityResolution ResolveCompatibility(
        Workspace workspace,
        string sourceDataTypeId,
        string targetDataTypeId)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDataTypeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetDataTypeId);

        var check = Check(workspace);
        if (check.HasErrors)
        {
            throw new InvalidOperationException("MetaDataTypeConversion workspace is invalid. Run 'meta-data-type-conversion check' first.");
        }

        var source = sourceDataTypeId.Trim();
        var target = targetDataTypeId.Trim();
        var mappings = workspace.Instance.GetOrCreateEntityRecords("DataTypeMapping")
            .Select(record => CreateResolution(workspace, record))
            .ToArray();

        if (string.Equals(source, target, StringComparison.Ordinal))
        {
            if (!IsKnownDataType(mappings, source))
            {
                throw new InvalidOperationException($"Data type '{source}' is not present in the MetaDataTypeConversion workspace.");
            }

            return new DataTypeCompatibilityResolution(source, target, []);
        }

        var mappingsBySource = mappings
            .GroupBy(mapping => mapping.SourceDataTypeId, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(item => item.TargetDataTypeId, StringComparer.Ordinal).ThenBy(item => item.MappingId, StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal);

        var queue = new Queue<(string CurrentDataTypeId, IReadOnlyList<DataTypeMappingResolution> Path)>();
        var seen = new HashSet<string>(StringComparer.Ordinal) { source };
        queue.Enqueue((source, []));

        while (queue.Count > 0)
        {
            var (currentDataTypeId, path) = queue.Dequeue();
            if (!mappingsBySource.TryGetValue(currentDataTypeId, out var outgoing))
            {
                continue;
            }

            foreach (var edge in outgoing)
            {
                var nextPath = path.Concat([edge]).ToArray();
                if (string.Equals(edge.TargetDataTypeId, target, StringComparison.Ordinal))
                {
                    return new DataTypeCompatibilityResolution(source, target, nextPath);
                }

                if (seen.Add(edge.TargetDataTypeId))
                {
                    queue.Enqueue((edge.TargetDataTypeId, nextPath));
                }
            }
        }

        throw new InvalidOperationException($"No sanctioned data type conversion path exists from '{source}' to '{target}'.");
    }

    private static DataTypeMappingResolution CreateResolution(Workspace workspace, GenericRecord mapping)
    {
        var implementations = workspace.Instance.GetOrCreateEntityRecords("ConversionImplementation")
            .ToDictionary(record => record.Id, StringComparer.Ordinal);

        if (!mapping.RelationshipIds.TryGetValue("ConversionImplementationId", out var implementationId) ||
            string.IsNullOrWhiteSpace(implementationId))
        {
            throw new InvalidOperationException($"DataTypeMapping '{mapping.Id}' is missing required relationship 'ConversionImplementationId'.");
        }

        if (!implementations.TryGetValue(implementationId, out var implementation))
        {
            throw new InvalidOperationException($"DataTypeMapping '{mapping.Id}' references missing ConversionImplementation '{implementationId}'.");
        }

        var targetDataTypeId = RequireValue(mapping, "TargetDataTypeId");

        return new DataTypeMappingResolution(
            mapping.Id,
            RequireValue(mapping, "SourceDataTypeId"),
            targetDataTypeId,
            TryGetDataTypeSystemName(targetDataTypeId) ?? string.Empty,
            implementationId,
            RequireValue(implementation, "Name"),
            mapping.Values.TryGetValue("Notes", out var notes) ? notes : null);
    }

    private static bool IsKnownDataType(IEnumerable<DataTypeMappingResolution> mappings, string dataTypeId) =>
        mappings.Any(mapping =>
            string.Equals(mapping.SourceDataTypeId, dataTypeId, StringComparison.Ordinal) ||
            string.Equals(mapping.TargetDataTypeId, dataTypeId, StringComparison.Ordinal));

    public static string? TryGetDataTypeSystemName(string dataTypeId)
    {
        if (string.IsNullOrWhiteSpace(dataTypeId))
        {
            return null;
        }

        var markerIndex = dataTypeId.IndexOf(":type:", StringComparison.Ordinal);
        if (markerIndex <= 0)
        {
            return null;
        }

        return dataTypeId[..markerIndex];
    }

    public static bool BelongsToDataTypeSystem(string dataTypeId, string dataTypeSystemName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataTypeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(dataTypeSystemName);

        return string.Equals(
            NormalizeDataTypeSystemName(TryGetDataTypeSystemName(dataTypeId) ?? string.Empty),
            NormalizeDataTypeSystemName(dataTypeSystemName),
            StringComparison.Ordinal);
    }

    private static string NormalizeDataTypeSystemName(string value) => value.Trim().ToLowerInvariant();

    private static string RequireValue(GenericRecord record, string propertyName)
    {
        if (!record.Values.TryGetValue(propertyName, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Record '{record.Id}' is missing required property '{propertyName}'.");
        }

        return value;
    }

    private readonly record struct DataTypeMappingKey(string SourceDataTypeId, string TargetDataTypeSystemName);

    private sealed class DataTypeMappingKeyComparer : IEqualityComparer<DataTypeMappingKey>
    {
        public static DataTypeMappingKeyComparer Instance { get; } = new();

        public bool Equals(DataTypeMappingKey x, DataTypeMappingKey y) =>
            string.Equals(x.SourceDataTypeId, y.SourceDataTypeId, StringComparison.Ordinal) &&
            string.Equals(x.TargetDataTypeSystemName, y.TargetDataTypeSystemName, StringComparison.Ordinal);

        public int GetHashCode(DataTypeMappingKey obj) =>
            HashCode.Combine(
                StringComparer.Ordinal.GetHashCode(obj.SourceDataTypeId),
                StringComparer.Ordinal.GetHashCode(obj.TargetDataTypeSystemName));
    }
}
