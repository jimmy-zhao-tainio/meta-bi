namespace MetaTransform.Binding;

internal sealed record TransformBindingTargetResolution(
    string SqlIdentifier,
    string? TableId);
