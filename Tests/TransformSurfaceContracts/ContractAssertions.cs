using MetaDataQuality;
using MetaOrchestration.Core;
using MetaTransform.Binding;
using MetaTransformBinding;

namespace MetaBi.TransformSurfaceContracts.Tests;

internal static class ContractAssertions
{
    public static void AssertBoundWithoutErrors(BindToWorkspaceResult result)
    {
        Assert.Equal(0, result.ErrorCount);
        Assert.Equal(0, result.IssueCount);
    }

    public static void AssertStatementKind(
        ContractWorkspace workspace,
        string scriptName,
        BoundStatementKind expected)
    {
        Assert.Equal(expected, workspace.GetStatementKind(workspace.ResolveScript(scriptName)));
    }

    public static void AssertDataQualitySawJoin(
        MetaDataQualityModel model,
        string scriptName)
    {
        Assert.NotEmpty(model.JoinPatternList);
        Assert.Contains(
            model.JoinPatternOccurrenceList,
            item => string.Equals(item.TransformScriptName, scriptName, StringComparison.OrdinalIgnoreCase));
        Assert.NotEmpty(model.DataQualityCandidateList);
    }

    public static void AssertNoDataQualityCandidates(MetaDataQualityModel model)
    {
        Assert.Empty(model.JoinPatternList);
        Assert.Empty(model.JoinPatternOccurrenceList);
        Assert.Empty(model.DataQualityCandidateList);
    }

    public static void AssertAccess(
        OrchestrationAnalysisResult result,
        string sqlIdentifier,
        OrchestrationObjectAccessKind accessKind,
        string accessRole)
    {
        Assert.Contains(
            result.Pipelines.SelectMany(pipeline => pipeline.Tasks).SelectMany(task => task.ObjectAccesses),
            item => string.Equals(item.SqlIdentifier, sqlIdentifier, StringComparison.OrdinalIgnoreCase)
                    && item.AccessKind == accessKind
                    && string.Equals(item.AccessRole, accessRole, StringComparison.OrdinalIgnoreCase));
    }

    public static void AssertOnlyIssue(
        OrchestrationAnalysisResult result,
        OrchestrationIssueCode code)
    {
        var issue = Assert.Single(result.Issues);
        Assert.Equal(code, issue.Code);
        Assert.True(issue.BlocksDag);
    }
}
