using MetaDataTypeConversion;

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
    MetaDataTypeConversionModel CreateWorkspace();
    MetaDataTypeConversionCheckResult Check(MetaDataTypeConversionModel model);
    DataTypeMappingResolution Resolve(MetaDataTypeConversionModel model, string sourceDataTypeId);
    DataTypeMappingResolution Resolve(MetaDataTypeConversionModel model, string sourceDataTypeId, string targetDataTypeSystemName);
    DataTypeCompatibilityResolution ResolveCompatibility(MetaDataTypeConversionModel model, string sourceDataTypeId, string targetDataTypeId);
}

public sealed class MetaDataTypeConversionService : IMetaDataTypeConversionService
{
    public MetaDataTypeConversionModel CreateWorkspace() =>
        MetaDataTypeConversion.Instance.MetaDataTypeConversionInstance.Default;

    public MetaDataTypeConversionCheckResult Check(MetaDataTypeConversionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var implementations = model.ConversionImplementationList
            .OrderBy(row => row.Id, StringComparer.Ordinal)
            .ToList();
        var mappings = model.DataTypeMappingList
            .OrderBy(row => row.Id, StringComparer.Ordinal)
            .ToList();

        var implementationById = implementations.ToDictionary(row => row.Id, StringComparer.Ordinal);
        var errors = new List<string>();

        foreach (var mapping in mappings)
        {
            var sourceDataTypeId = RequireValue(mapping.Id, mapping.SourceDataTypeId, nameof(DataTypeMapping.SourceDataTypeId));
            var targetDataTypeId = RequireValue(mapping.Id, mapping.TargetDataTypeId, nameof(DataTypeMapping.TargetDataTypeId));
            if (string.IsNullOrWhiteSpace(TryGetDataTypeSystemName(targetDataTypeId)))
            {
                errors.Add($"DataTypeMapping '{mapping.Id}' has TargetDataTypeId '{targetDataTypeId}' with an unsupported data type id shape.");
            }

            if (mapping.ConversionImplementation is null ||
                string.IsNullOrWhiteSpace(mapping.ConversionImplementation.Id))
            {
                errors.Add($"DataTypeMapping '{mapping.Id}' is missing required relationship 'ConversionImplementation'.");
                continue;
            }

            if (!implementationById.ContainsKey(mapping.ConversionImplementation.Id))
            {
                errors.Add($"DataTypeMapping '{mapping.Id}' references missing ConversionImplementation '{mapping.ConversionImplementation.Id}'.");
            }
        }

        var duplicateSources = mappings
            .GroupBy(
                row => new DataTypeMappingKey(
                    RequireValue(row.Id, row.SourceDataTypeId, nameof(DataTypeMapping.SourceDataTypeId)),
                    NormalizeDataTypeSystemName(TryGetDataTypeSystemName(RequireValue(row.Id, row.TargetDataTypeId, nameof(DataTypeMapping.TargetDataTypeId))) ?? string.Empty)),
                DataTypeMappingKeyComparer.Instance)
            .Where(group => group.Count() > 1)
            .OrderBy(group => group.Key.SourceDataTypeId, StringComparer.Ordinal)
            .ThenBy(group => group.Key.TargetDataTypeSystemName, StringComparer.Ordinal);

        foreach (var duplicateSource in duplicateSources)
        {
            var ids = string.Join(", ", duplicateSource.Select(row => row.Id).OrderBy(id => id, StringComparer.Ordinal));
            errors.Add($"SourceDataTypeId '{duplicateSource.Key.SourceDataTypeId}' is mapped more than once for target data type system '{duplicateSource.Key.TargetDataTypeSystemName}' ({ids}).");
        }

        return new MetaDataTypeConversionCheckResult(mappings.Count, implementations.Count, errors);
    }

    public DataTypeMappingResolution Resolve(MetaDataTypeConversionModel model, string sourceDataTypeId)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDataTypeId);

        var check = Check(model);
        if (check.HasErrors)
        {
            throw new InvalidOperationException("MetaDataTypeConversion workspace is invalid. Run 'meta-data-type-conversion check' first.");
        }

        var mappings = model.DataTypeMappingList
            .Where(row => string.Equals(RequireValue(row.Id, row.SourceDataTypeId, nameof(DataTypeMapping.SourceDataTypeId)), sourceDataTypeId, StringComparison.Ordinal))
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
                    .Select(row => TryGetDataTypeSystemName(RequireValue(row.Id, row.TargetDataTypeId, nameof(DataTypeMapping.TargetDataTypeId))) ?? "<unknown>")
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(value => value, StringComparer.OrdinalIgnoreCase));
            throw new InvalidOperationException(
                $"Source data type '{sourceDataTypeId}' resolves ambiguously to {mappings.Count} DataTypeMappings. Specify a target data type system. Available target systems: {targetSystems}.");
        }

        return CreateResolution(model, mappings[0]);
    }

    public DataTypeMappingResolution Resolve(
        MetaDataTypeConversionModel model,
        string sourceDataTypeId,
        string targetDataTypeSystemName)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDataTypeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetDataTypeSystemName);

        var check = Check(model);
        if (check.HasErrors)
        {
            throw new InvalidOperationException("MetaDataTypeConversion workspace is invalid. Run 'meta-data-type-conversion check' first.");
        }

        var normalizedTargetSystem = NormalizeDataTypeSystemName(targetDataTypeSystemName);
        var mappings = model.DataTypeMappingList
            .Where(row => string.Equals(RequireValue(row.Id, row.SourceDataTypeId, nameof(DataTypeMapping.SourceDataTypeId)), sourceDataTypeId, StringComparison.Ordinal))
            .Where(row => string.Equals(
                NormalizeDataTypeSystemName(TryGetDataTypeSystemName(RequireValue(row.Id, row.TargetDataTypeId, nameof(DataTypeMapping.TargetDataTypeId))) ?? string.Empty),
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

        return CreateResolution(model, mappings[0]);
    }

    public DataTypeCompatibilityResolution ResolveCompatibility(
        MetaDataTypeConversionModel model,
        string sourceDataTypeId,
        string targetDataTypeId)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDataTypeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetDataTypeId);

        var check = Check(model);
        if (check.HasErrors)
        {
            throw new InvalidOperationException("MetaDataTypeConversion workspace is invalid. Run 'meta-data-type-conversion check' first.");
        }

        var source = sourceDataTypeId.Trim();
        var target = targetDataTypeId.Trim();
        var mappings = model.DataTypeMappingList
            .Select(row => CreateResolution(model, row))
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

    private static DataTypeMappingResolution CreateResolution(MetaDataTypeConversionModel model, DataTypeMapping mapping)
    {
        var implementations = model.ConversionImplementationList
            .ToDictionary(row => row.Id, StringComparer.Ordinal);

        if (mapping.ConversionImplementation is null ||
            string.IsNullOrWhiteSpace(mapping.ConversionImplementation.Id))
        {
            throw new InvalidOperationException($"DataTypeMapping '{mapping.Id}' is missing required relationship 'ConversionImplementation'.");
        }

        if (!implementations.TryGetValue(mapping.ConversionImplementation.Id, out var implementation))
        {
            throw new InvalidOperationException($"DataTypeMapping '{mapping.Id}' references missing ConversionImplementation '{mapping.ConversionImplementation.Id}'.");
        }

        var targetDataTypeId = RequireValue(mapping.Id, mapping.TargetDataTypeId, nameof(DataTypeMapping.TargetDataTypeId));

        return new DataTypeMappingResolution(
            mapping.Id,
            RequireValue(mapping.Id, mapping.SourceDataTypeId, nameof(DataTypeMapping.SourceDataTypeId)),
            targetDataTypeId,
            TryGetDataTypeSystemName(targetDataTypeId) ?? string.Empty,
            implementation.Id,
            RequireValue(implementation.Id, implementation.Name, nameof(ConversionImplementation.Name)),
            mapping.Notes);
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

    private static string RequireValue(string recordId, string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Record '{recordId}' is missing required property '{propertyName}'.");
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
