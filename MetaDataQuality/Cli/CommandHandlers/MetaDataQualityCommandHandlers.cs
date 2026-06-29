using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataQuality;
using MetaDataQuality.Core;

internal sealed class MetaDataQualityCommandHandlers
{
    private readonly ConsolePresenter presenter;
    private readonly MetaDataQualityWorkspaceService workspaceService;
    private readonly MetaDataQualityInspectionService inspectionService;
    private readonly MetaDataQualityPromotionService promotionService;

    public MetaDataQualityCommandHandlers(
        ConsolePresenter presenter,
        MetaDataQualityWorkspaceService workspaceService,
        MetaDataQualityInspectionService inspectionService,
        MetaDataQualityPromotionService promotionService)
    {
        this.presenter = presenter;
        this.workspaceService = workspaceService;
        this.inspectionService = inspectionService;
        this.promotionService = promotionService;
    }

    public void RunFromTransformWorkspace(MetaCliInvocation invocation)
    {
        var transformWorkspacePathValue = invocation.Required("transform-workspace");
        var newWorkspacePathValue = invocation.Required("new-workspace");
        var bindingWorkspacePathValue = invocation.Optional("binding-workspace");

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(newWorkspacePathValue);
        if (!targetValidation.Ok)
        {
            Fail(
                targetValidation.ErrorMessage,
                "choose a new folder or empty the target directory and retry.",
                4,
                targetValidation.Details);
        }

        try
        {
            var result = workspaceService.CreateFromTransformWorkspace(
                transformWorkspacePathValue,
                bindingWorkspacePathValue,
                targetValidation.FullPath);

            presenter.WriteInfo($"Workspace: {result.WorkspacePath}");
            presenter.WriteInfo($"Views ready to create: {result.DataQualityCandidateCount}");
            presenter.WriteInfo($"Relationships captured: {result.JoinPatternOccurrenceCount}");
            if (result.BindingWorkspaceProvided)
            {
                presenter.WriteInfo($"Transform scripts scanned: {result.AnalyzedTransformScriptCount}/{result.TransformScriptCount}");
                presenter.WriteInfo($"Transform scripts skipped by BindingWS: {result.BindingSkippedTransformScriptCount}");
            }
        }
        catch (Exception ex) when (ex is not MetaCliExitException)
        {
            Fail(
                "Cannot create data-quality workspace.",
                "check the transform workspace and retry.",
                4,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(transformWorkspacePathValue)}",
                    $"  BindingWorkspace: {(string.IsNullOrWhiteSpace(bindingWorkspacePathValue) ? "<none>" : Path.GetFullPath(bindingWorkspacePathValue))}",
                    $"  {ex.Message}",
                });
        }
    }

    public void RunInspect(MetaCliInvocation invocation, MetaDataQualityModel model)
    {
        var workspacePath = ResolveWorkspacePath(invocation);
        var topCases = ReadPositiveInt(invocation.Required("top-cases"), "--top-cases");
        var showCases = invocation.Flag("show-cases")
                        || invocation.IsPresent("top-cases")
                        || invocation.Flag("show-candidate-ids");
        var showCandidateIds = invocation.Flag("show-candidate-ids");

        try
        {
            var result = inspectionService.Inspect(model);
            presenter.WriteInfo($"Views ready to create: {result.CandidateCount}");
            if (result.PromotedCount > 0)
            {
                presenter.WriteInfo($"Promoted for SQL: {result.PromotedCount}");
            }

            if (result.CheckFamilies.Count > 0)
            {
                presenter.WriteInfo(string.Empty);
                presenter.WriteInfo("Checks:");
                foreach (var family in result.CheckFamilies)
                {
                    presenter.WriteInfo($"  {family.Label}: {family.Count}");
                }
            }

            if (result.RelationshipSituationCount > 0)
            {
                presenter.WriteInfo(string.Empty);
                presenter.WriteInfo($"Relationships captured: {result.RelationshipSituationCount}");
            }

            if (result.CorpusInference.RelationshipCount > 0)
            {
                presenter.WriteInfo(string.Empty);
                presenter.WriteInfo("Corpus Inference:");
                presenter.WriteInfo($"  Relationships observed: {result.CorpusInference.RelationshipCount}");
                presenter.WriteInfo($"  Dominant relationship patterns: {result.CorpusInference.DominantRelationshipPatternCount}");
                presenter.WriteInfo($"  Column equivalence edges: {result.CorpusInference.ColumnEquivalenceEdgeCount}");
                presenter.WriteInfo($"  Minority join-pattern candidates: {FormatStatusSummary(result.CorpusInference.MinorityJoinPattern)}");
                presenter.WriteInfo($"  Incomplete-composite candidates: {FormatStatusSummary(result.CorpusInference.IncompleteComposite)}");
                presenter.WriteInfo($"  Suspicious-extra-predicate candidates: {FormatStatusSummary(result.CorpusInference.SuspiciousExtraPredicate)}");
                presenter.WriteInfo($"  Missing-common-filter candidates: {FormatStatusSummary(result.CorpusInference.MissingCommonFilter)}");
                presenter.WriteInfo($"  Minority column-equivalence candidates: {FormatStatusSummary(result.CorpusInference.MinorityColumnEquivalence)}");
                presenter.WriteInfo($"  Optionality-drift (inner vs usually optional): {FormatStatusSummary(result.CorpusInference.InnerAgainstUsuallyOptional)}");
                presenter.WriteInfo($"  Optionality-drift (left vs usually mandatory): {FormatStatusSummary(result.CorpusInference.LeftAgainstUsuallyMandatory)}");
                presenter.WriteInfo($"  Implied fanout-risk candidates: {FormatStatusSummary(result.CorpusInference.ImpliedJoinFanout)}");
                presenter.WriteInfo($"  Implied output-duplicate-risk candidates: {FormatStatusSummary(result.CorpusInference.ImpliedOutputDuplicate)}");
            }

            if (result.PendingCount > 0)
            {
                presenter.WriteInfo(string.Empty);
                presenter.WriteInfo($"Pending candidates: {result.PendingCount}");
            }

            if (showCases && result.PendingSituations.Count > 0)
            {
                presenter.WriteInfo(string.Empty);
                presenter.WriteInfo("Relationship Cases:");
                var visible = result.PendingSituations.Take(topCases).ToArray();
                for (var i = 0; i < visible.Length; i++)
                {
                    var situation = visible[i];
                    presenter.WriteInfo($"  {i + 1}. {situation.JoinDescription}");
                    presenter.WriteInfo($"     Keys: {situation.JoinCondition}");
                    presenter.WriteInfo($"     SQL join: {situation.JoinType}");
                    presenter.WriteInfo($"     Checks: {string.Join(", ", situation.ViewLabels)}");
                    if (showCandidateIds)
                    {
                        presenter.WriteInfo($"     Candidate ids: {string.Join(", ", situation.PendingCandidateIds)}");
                    }
                }

                if (result.PendingSituations.Count > visible.Length)
                {
                    presenter.WriteInfo($"  Showing {visible.Length} of {result.PendingSituations.Count} relationships. Increase with --top-cases.");
                }
            }
            else if (showCases && result.PendingSituations.Count == 0)
            {
                presenter.WriteInfo(string.Empty);
                presenter.WriteInfo("  No generated candidates remain to promote.");
            }
        }
        catch (Exception ex) when (ex is not MetaCliExitException)
        {
            Fail(
                "Cannot inspect data-quality workspace.",
                "check the workspace path and retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  {ex.Message}",
                });
        }
    }

    public void RunPromote(MetaCliInvocation invocation, MetaDataQualityModel model)
    {
        var workspacePath = ResolveWorkspacePath(invocation);
        var candidateIds = invocation.Values("candidate-id");
        var promoteAll = invocation.Flag("all");

        try
        {
            var result = promotionService.PromoteWorkspace(model, workspacePath, candidateIds, promoteAll);

            presenter.WriteInfo($"Candidates promoted this run: {result.PromotedThisRunCount}");
            presenter.WriteInfo($"Candidates promoted for SQL: {result.TotalPromotedCount}");
        }
        catch (MetaDataQualityCandidateNotFoundException ex)
        {
            Fail(
                ex.Message,
                "run meta-data-quality inspect --workspace <path> and retry.",
                4,
                new[] { $"  Workspace: {workspacePath}" });
        }
        catch (Exception ex) when (ex is not MetaCliExitException)
        {
            Fail(
                "Cannot promote data-quality candidates.",
                "check the workspace and candidate ids, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {workspacePath}",
                    $"  {ex.Message}",
                });
        }
    }

    private static string ResolveWorkspacePath(MetaCliInvocation invocation) =>
        Path.GetFullPath(invocation.Optional("workspace") ?? Directory.GetCurrentDirectory());

    private void Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details.Where(static item => !string.IsNullOrWhiteSpace(item)));
        }

        renderedDetails.Add($"Next: {next}");
        presenter.WriteFailure(message, renderedDetails);
        throw new MetaCliExitException(exitCode);
    }

    private int ReadPositiveInt(string value, string parameterName)
    {
        if (int.TryParse(value, out var result) && result > 0)
        {
            return result;
        }

        Fail($"{parameterName} must be a positive integer.", "run meta-data-quality help inspect and retry.");
        throw new MetaCliExitException(2);
    }

    private static string FormatStatusSummary(MetaDataQualityCandidateStatusSummary summary)
    {
        if (summary.Total == 0)
        {
            return "0";
        }

        return $"{summary.Total} (waiting {summary.Waiting}, promoted {summary.Promoted})";
    }
}
