namespace MetaTransform.Binding;

public sealed class TransformBindingValidationOptions
{
    public static readonly TransformBindingValidationOptions Default = new();

    public IReadOnlySet<string> IgnoredTargetColumnNames { get; init; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public string ExecuteSystemName { get; init; } = string.Empty;

    public string ExecuteSystemDefaultSchemaName { get; init; } = string.Empty;

    public static TransformBindingValidationOptions Create(
        IEnumerable<string>? ignoredTargetColumnNames)
    {
        return Create(
            ignoredTargetColumnNames,
            executeSystemName: null,
            executeSystemDefaultSchemaName: null);
    }

    public static TransformBindingValidationOptions Create(
        IEnumerable<string>? ignoredTargetColumnNames,
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

        if (normalized.Count == 0 &&
            string.IsNullOrWhiteSpace(normalizedExecuteSystemName) &&
            string.IsNullOrWhiteSpace(normalizedDefaultSchemaName))
        {
            return Default;
        }

        return new TransformBindingValidationOptions
        {
            IgnoredTargetColumnNames = normalized,
            ExecuteSystemName = normalizedExecuteSystemName,
            ExecuteSystemDefaultSchemaName = normalizedDefaultSchemaName
        };
    }
}
