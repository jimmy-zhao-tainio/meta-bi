namespace MetaTransform.Binding;

public sealed class TransformBindingValidationOptions
{
    public static readonly TransformBindingValidationOptions Default = new();

    public IReadOnlySet<string> IgnoredTargetColumnNames { get; init; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public static TransformBindingValidationOptions Create(
        IEnumerable<string>? ignoredTargetColumnNames)
    {
        if (ignoredTargetColumnNames is null)
        {
            return Default;
        }

        var normalized = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in ignoredTargetColumnNames)
        {
            var trimmed = name?.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                normalized.Add(trimmed);
            }
        }

        return normalized.Count == 0
            ? Default
            : new TransformBindingValidationOptions
            {
                IgnoredTargetColumnNames = normalized
            };
    }
}
