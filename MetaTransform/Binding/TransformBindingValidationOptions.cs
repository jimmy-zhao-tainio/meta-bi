namespace MetaTransform.Binding;

public sealed class TransformBindingValidationOptions
{
    public static readonly TransformBindingValidationOptions Default = new();

    public IReadOnlySet<string> IgnoredTargetColumnNames { get; init; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public IReadOnlySet<string> IgnoredTargetColumnNamesIfPresent { get; init; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public string ExecuteSystemName { get; init; } = string.Empty;

    public string ExecuteSystemDefaultSchemaName { get; init; } = string.Empty;

    public static TransformBindingValidationOptions Create(
        IEnumerable<string>? ignoredTargetColumnNames)
    {
        return Create(
            ignoredTargetColumnNames,
            ignoredTargetColumnNamesIfPresent: null,
            executeSystemName: null,
            executeSystemDefaultSchemaName: null);
    }

    public static TransformBindingValidationOptions Create(
        IEnumerable<string>? ignoredTargetColumnNames,
        string? executeSystemName,
        string? executeSystemDefaultSchemaName)
    {
        return Create(
            ignoredTargetColumnNames,
            ignoredTargetColumnNamesIfPresent: null,
            executeSystemName,
            executeSystemDefaultSchemaName);
    }

    public static TransformBindingValidationOptions Create(
        IEnumerable<string>? ignoredTargetColumnNames,
        IEnumerable<string>? ignoredTargetColumnNamesIfPresent,
        string? executeSystemName,
        string? executeSystemDefaultSchemaName)
    {
        var normalizedExecuteSystemName = executeSystemName?.Trim() ?? string.Empty;
        var normalizedDefaultSchemaName = executeSystemDefaultSchemaName?.Trim() ?? string.Empty;

        var normalized = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (ignoredTargetColumnNames is not null)
        {
            foreach (var name in ignoredTargetColumnNames)
            {
                var trimmed = name?.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    normalized.Add(trimmed);
                }
            }
        }

        var normalizedIfPresent = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (ignoredTargetColumnNamesIfPresent is not null)
        {
            foreach (var name in ignoredTargetColumnNamesIfPresent)
            {
                var trimmed = name?.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    normalizedIfPresent.Add(trimmed);
                }
            }
        }

        if (normalized.Count == 0 &&
            normalizedIfPresent.Count == 0 &&
            string.IsNullOrWhiteSpace(normalizedExecuteSystemName) &&
            string.IsNullOrWhiteSpace(normalizedDefaultSchemaName))
        {
            return Default;
        }

        return new TransformBindingValidationOptions
        {
            IgnoredTargetColumnNames = normalized,
            IgnoredTargetColumnNamesIfPresent = normalizedIfPresent,
            ExecuteSystemName = normalizedExecuteSystemName,
            ExecuteSystemDefaultSchemaName = normalizedDefaultSchemaName
        };
    }
}
