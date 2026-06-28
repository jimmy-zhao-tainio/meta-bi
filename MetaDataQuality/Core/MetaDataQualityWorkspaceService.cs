namespace MetaDataQuality.Core;

public sealed class MetaDataQualityWorkspaceService
{
    public MetaDataQualityWorkspaceCreationResult CreateFromTransformWorkspace(
        string transformWorkspacePath,
        string? bindingWorkspacePath,
        string workspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transformWorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);

        var transformFullPath = Path.GetFullPath(transformWorkspacePath);
        var bindingFullPath = string.IsNullOrWhiteSpace(bindingWorkspacePath)
            ? null
            : Path.GetFullPath(bindingWorkspacePath);
        var workspaceFullPath = Path.GetFullPath(workspacePath);

        var discovery = new MetaDataQualityCandidateDiscoveryService()
            .DiscoverFromTransformWorkspace(transformFullPath, bindingFullPath);

        discovery.Model.SaveToXmlWorkspace(workspaceFullPath);

        return new MetaDataQualityWorkspaceCreationResult(
            workspaceFullPath,
            discovery.Model.DataQualityCandidateList.Count,
            discovery.Model.JoinPatternOccurrenceList.Count,
            discovery.TransformScriptCount,
            discovery.AnalyzedTransformScriptCount,
            discovery.BindingSkippedTransformScriptCount,
            bindingFullPath is not null);
    }
}

public sealed record MetaDataQualityWorkspaceCreationResult(
    string WorkspacePath,
    int DataQualityCandidateCount,
    int JoinPatternOccurrenceCount,
    int TransformScriptCount,
    int AnalyzedTransformScriptCount,
    int BindingSkippedTransformScriptCount,
    bool BindingWorkspaceProvided);
