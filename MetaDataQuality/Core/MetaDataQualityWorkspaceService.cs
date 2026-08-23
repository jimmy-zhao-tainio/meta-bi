namespace MetaDataQuality.Core;

public sealed class MetaDataQualityWorkspaceService
{
    public MetaDataQualityWorkspaceCreationResult CreateFromTransformWorkspace(
        string transformWorkspacePath,
        string? bindingWorkspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);

        var transformFullPath = Path.GetFullPath(transformWorkspacePath);
        var bindingFullPath = string.IsNullOrWhiteSpace(bindingWorkspacePath)
            ? null
            : Path.GetFullPath(bindingWorkspacePath);
        var discovery = new MetaDataQualityCandidateDiscoveryService()
            .DiscoverFromTransformWorkspace(transformFullPath, bindingFullPath);

        return new MetaDataQualityWorkspaceCreationResult(
            discovery.Model,
            discovery.Model.DataQualityCandidateList.Count,
            discovery.Model.JoinPatternOccurrenceList.Count,
            discovery.TransformScriptCount,
            discovery.AnalyzedTransformScriptCount,
            discovery.BindingSkippedTransformScriptCount,
            bindingFullPath is not null);
    }
}

public sealed record MetaDataQualityWorkspaceCreationResult(
    MetaDataQualityModel Model,
    int DataQualityCandidateCount,
    int JoinPatternOccurrenceCount,
    int TransformScriptCount,
    int AnalyzedTransformScriptCount,
    int BindingSkippedTransformScriptCount,
    bool BindingWorkspaceProvided);
