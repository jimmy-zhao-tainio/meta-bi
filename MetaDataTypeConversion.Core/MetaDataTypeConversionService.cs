using Meta.Core.Domain;

namespace MetaDataTypeConversion.Core;

public sealed record DataTypeMappingResolution(
    string MappingId,
    string SourceDataTypeId,
    string TargetDataTypeId,
    string ConversionImplementationId,
    string ConversionImplementationName,
    string? Notes);

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
            _ = RequireValue(mapping, "TargetDataTypeId");


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
            .GroupBy(record => RequireValue(record, "SourceDataTypeId"), StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .OrderBy(group => group.Key, StringComparer.Ordinal);

        foreach (var duplicateSource in duplicateSources)
        {
            var ids = string.Join(", ", duplicateSource.Select(record => record.Id).OrderBy(id => id, StringComparer.Ordinal));
            errors.Add($"SourceDataTypeId '{duplicateSource.Key}' is mapped more than once ({ids}).");
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
            throw new InvalidOperationException($"Source data type '{sourceDataTypeId}' resolves ambiguously to {mappings.Count} DataTypeMappings.");
        }

        var mapping = mappings[0];
        var implementations = workspace.Instance.GetOrCreateEntityRecords("ConversionImplementation")
            .ToDictionary(record => record.Id, StringComparer.Ordinal);

        var implementationId = mapping.RelationshipIds["ConversionImplementationId"];
        var implementation = implementations[implementationId];

        return new DataTypeMappingResolution(
            mapping.Id,
            RequireValue(mapping, "SourceDataTypeId"),
            RequireValue(mapping, "TargetDataTypeId"),
            implementationId,
            RequireValue(implementation, "Name"),
            mapping.Values.TryGetValue("Notes", out var notes) ? notes : null);
    }

    private static string RequireValue(GenericRecord record, string propertyName)
    {
        if (!record.Values.TryGetValue(propertyName, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Record '{record.Id}' is missing required property '{propertyName}'.");
        }

        return value;
    }
}
