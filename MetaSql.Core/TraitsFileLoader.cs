using System.Text.Json;

namespace MetaSql.Core;

public sealed class TraitsFileLoader
{
    public IReadOnlyDictionary<string, SqlObjectTraits> Load(string? traitsFilePath)
    {
        if (string.IsNullOrWhiteSpace(traitsFilePath))
        {
            return new Dictionary<string, SqlObjectTraits>(StringComparer.OrdinalIgnoreCase);
        }

        var fullPath = Path.GetFullPath(traitsFilePath);
        if (!File.Exists(fullPath))
        {
            throw new InvalidOperationException($"traits file '{fullPath}' does not exist.");
        }

        var raw = JsonSerializer.Deserialize<Dictionary<string, TraitsRecord>>(
            File.ReadAllText(fullPath),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        if (raw == null)
        {
            return new Dictionary<string, SqlObjectTraits>(StringComparer.OrdinalIgnoreCase);
        }

        return raw.ToDictionary(
            item => item.Key,
            item => new SqlObjectTraits(
                ParseStateClass(item.Value.StateClass),
                ParseAutoPolicy(item.Value.AutoPolicy),
                item.Value.ValidationProfile,
                item.Value.DependencyGroup),
            StringComparer.OrdinalIgnoreCase);
    }

    private static SqlObjectStateClass ParseStateClass(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            null or "" or "persistent" => SqlObjectStateClass.Persistent,
            "replaceable" => SqlObjectStateClass.Replaceable,
            _ => throw new InvalidOperationException($"unsupported stateClass '{value}'.")
        };
    }

    private static SqlObjectAutoPolicy ParseAutoPolicy(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            null or "" or "additive-only" => SqlObjectAutoPolicy.AdditiveOnly,
            "additive-plus-empty-drop" => SqlObjectAutoPolicy.AdditivePlusEmptyDrop,
            _ => throw new InvalidOperationException($"unsupported autoPolicy '{value}'.")
        };
    }

    private sealed record TraitsRecord(
        string? StateClass,
        string? AutoPolicy,
        string? ValidationProfile,
        string? DependencyGroup);
}
