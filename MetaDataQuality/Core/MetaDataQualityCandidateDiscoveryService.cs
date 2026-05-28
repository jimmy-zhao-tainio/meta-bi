using MetaDataQuality;
using MetaTransformScript;

namespace MetaDataQuality.Core;

public sealed partial class MetaDataQualityCandidateDiscoveryService
{
    public MetaDataQualityDiscoveryResult DiscoverFromTransformWorkspace(string transformWorkspacePath)
    {
        var model = MetaTransformScriptModel.LoadFromXmlWorkspace(transformWorkspacePath, searchUpward: false);
        var dataQualityModel = Discover(model);
        return new MetaDataQualityDiscoveryResult
        {
            Model = dataQualityModel,
            TransformScriptCount = model.TransformScriptList.Count,
        };
    }

    public MetaDataQualityModel Discover(MetaTransformScriptModel transformModel)
    {
        // Phase 1: Extract minimal, reusable DQ evidence from TransformScript instances.
        var extracted = ExtractWorkspaceEvidence(transformModel);

        // Phase 2: Iterate extracted evidence and project sanctioned MetaDataQuality entities.
        var model = MaterializeDataQualityModel(extracted);

        // Phase 3: Build corpus-level relationship inference and candidate evidence.
        new MetaDataQualityCorpusInferenceService().Apply(model);
        return model;
    }
}
