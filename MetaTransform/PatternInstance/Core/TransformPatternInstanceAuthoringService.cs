using MTP = global::MetaTransformPattern;
using MTPI = global::MetaTransformPatternInstance;

namespace MetaTransformPatternInstance.Core;

public sealed class TransformPatternInstanceAuthoringService
{
    private static readonly StringComparer IdentityComparer = StringComparer.OrdinalIgnoreCase;
    private readonly MetaTransformPattern.Core.TransformPatternAuthoringService patternService = new();

    public MTPI.MetaTransformPatternInstanceModel CreateWorkspace() =>
        MTPI.MetaTransformPatternInstanceModel.CreateEmpty();

    public MTPI.TransformPatternInstance AddInstance(
        MTPI.MetaTransformPatternInstanceModel instanceModel,
        MTP.MetaTransformPatternModel patternModel,
        string id,
        string name,
        string patternId)
    {
        ArgumentNullException.ThrowIfNull(instanceModel);
        ArgumentNullException.ThrowIfNull(patternModel);
        var normalizedId = RequireText(id, "Instance id");
        var normalizedName = RequireText(name, "Instance name");
        var pattern = patternService.RequirePattern(patternModel, patternId);
        if (instanceModel.TransformPatternInstanceList.Any(instance =>
                IdentityComparer.Equals(instance.Id, normalizedId)))
        {
            throw new InvalidOperationException($"Transform-pattern instance '{normalizedId}' already exists.");
        }

        var instance = new MTPI.TransformPatternInstance
        {
            Id = normalizedId,
            Name = normalizedName,
            TransformPatternId = pattern.Id,
        };
        instanceModel.TransformPatternInstanceList.Add(instance);
        return instance;
    }

    public MTPI.TransformPatternInstancePlaceholder SetPlaceholderValue(
        MTPI.MetaTransformPatternInstanceModel instanceModel,
        MTP.MetaTransformPatternModel patternModel,
        string instanceId,
        string placeholderIdentity,
        string sqlText)
    {
        var placeholder = RequireInstancePlaceholder(
            instanceModel,
            patternModel,
            instanceId,
            placeholderIdentity);
        ArgumentNullException.ThrowIfNull(sqlText);
        placeholder.SqlText = sqlText;
        return placeholder;
    }

    public MTPI.TransformPatternInstance RequireInstance(
        MTPI.MetaTransformPatternInstanceModel model,
        string instanceId)
    {
        ArgumentNullException.ThrowIfNull(model);
        var normalizedId = RequireText(instanceId, "Instance id");
        return model.TransformPatternInstanceList.SingleOrDefault(instance =>
                   IdentityComparer.Equals(instance.Id, normalizedId))
               ?? throw new InvalidOperationException(
                   $"Transform-pattern instance '{normalizedId}' was not found.");
    }

    private MTPI.TransformPatternInstancePlaceholder RequireInstancePlaceholder(
        MTPI.MetaTransformPatternInstanceModel instanceModel,
        MTP.MetaTransformPatternModel patternModel,
        string instanceId,
        string placeholderIdentity)
    {
        ArgumentNullException.ThrowIfNull(instanceModel);
        ArgumentNullException.ThrowIfNull(patternModel);
        var instance = RequireInstance(instanceModel, instanceId);
        var placeholder = patternService.RequirePlaceholder(
            patternModel,
            instance.TransformPatternId,
            placeholderIdentity);
        var existing = instanceModel.TransformPatternInstancePlaceholderList.SingleOrDefault(candidate =>
            ReferenceEquals(candidate.TransformPatternInstance, instance) &&
            IdentityComparer.Equals(candidate.TransformPatternPlaceholderId, placeholder.Id));
        if (existing is not null)
        {
            return existing;
        }

        var created = new MTPI.TransformPatternInstancePlaceholder
        {
            Id = $"{instance.Id}:placeholder:{placeholder.Id}",
            SqlText = string.Empty,
            TransformPatternInstance = instance,
            TransformPatternPlaceholderId = placeholder.Id,
        };
        instanceModel.TransformPatternInstancePlaceholderList.Add(created);
        return created;
    }

    private static string RequireText(string value, string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }
}
