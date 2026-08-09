using MetaDataQuality;
using MetaTransformBinding;
using MetaTransformScript;

namespace MetaDataQuality.Core;

public sealed partial class MetaDataQualityCandidateDiscoveryService
{
    public MetaDataQualityDiscoveryResult DiscoverFromTransformWorkspace(string transformWorkspacePath)
    {
        return DiscoverFromTransformWorkspace(transformWorkspacePath, bindingWorkspacePath: null);
    }

    public MetaDataQualityDiscoveryResult DiscoverFromTransformWorkspace(
        string transformWorkspacePath,
        string? bindingWorkspacePath)
    {
        var model = Meta.Core.Serialization.TypedWorkspaceModelMapper.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
        var includedTransformScriptIds = LoadValidatedTransformScriptIds(bindingWorkspacePath);
        var dataQualityModel = Discover(model, includedTransformScriptIds);
        var analyzedTransformScriptCount = includedTransformScriptIds is null
            ? model.TransformScriptList.Count
            : model.TransformScriptList.Count(item => includedTransformScriptIds.Contains(item.Id));

        return new MetaDataQualityDiscoveryResult
        {
            Model = dataQualityModel,
            TransformScriptCount = model.TransformScriptList.Count,
            AnalyzedTransformScriptCount = analyzedTransformScriptCount,
            BindingSkippedTransformScriptCount = model.TransformScriptList.Count - analyzedTransformScriptCount,
        };
    }

    public MetaDataQualityModel Discover(MetaTransformScriptModel transformModel)
    {
        return Discover(transformModel, includedTransformScriptIds: null);
    }

    public MetaDataQualityModel Discover(
        MetaTransformScriptModel transformModel,
        IReadOnlySet<string>? includedTransformScriptIds)
    {
        // Phase 1: Extract minimal, reusable DQ evidence from TransformScript instances.
        var extracted = ExtractWorkspaceEvidence(transformModel, includedTransformScriptIds);

        // Phase 2: Iterate extracted evidence and project sanctioned MetaDataQuality entities.
        var model = MaterializeDataQualityModel(extracted);

        // Phase 3: Build corpus-level relationship inference and candidate evidence.
        new MetaDataQualityCorpusInferenceService().Apply(model);
        return model;
    }

    private static IReadOnlySet<string>? LoadValidatedTransformScriptIds(string? bindingWorkspacePath)
    {
        if (string.IsNullOrWhiteSpace(bindingWorkspacePath))
        {
            return null;
        }

        var bindingModel = Meta.Core.Serialization.TypedWorkspaceModelMapper.Load<MetaTransformBindingModel>(bindingWorkspacePath, searchUpward: false);
        var validatedBindingIds = bindingModel.ValidationList
            .Select(static item => item.TransformBinding.Id)
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .ToHashSet(StringComparer.Ordinal);

        return bindingModel.TransformBindingList
            .Where(item => validatedBindingIds.Contains(item.Id))
            .Select(static item => item.MetaTransformScriptTransformScriptId)
            .Where(static item => !string.IsNullOrWhiteSpace(item))
            .ToHashSet(StringComparer.Ordinal);
    }
}
