using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaCli.Core;
using MetaDataQuality;
using MetaDataQuality.Core;

internal static class Program
{
    private const string AppName = "meta-data-quality";
    private const string ApplicationId = "app-meta-data-quality";
    private const string CommandWorkspaceDirectoryName = "meta-data-quality.MetaCli";

    private static readonly ConsolePresenter Presenter = new();

    static int Main(string[] args)
    {
        if (CliVersion.TryWriteVersion(Presenter, AppName, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        Environment.ExitCode = 0;
        var runtime = new MetaCliRuntime<MetaDataQualityModel>(CommandWorkspacePath, ApplicationId)
            .UseDefaultHelp()
            .Bind("exec-from-transform-workspace", RunFromTransformWorkspace)
            .Bind("exec-inspect", RunInspect)
            .Bind("exec-promote", RunPromote);

        runtime.Run(args);
        return Environment.ExitCode;
    }

    private static string CommandWorkspacePath
    {
        get
        {
            return Path.Combine(AppContext.BaseDirectory, CommandWorkspaceDirectoryName);
        }
    }

    private static void RunFromTransformWorkspace(MetaCliInvocation invocation)
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
            var transformWorkspacePath = Path.GetFullPath(transformWorkspacePathValue);
            var discovery = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(
                    transformWorkspacePath,
                    string.IsNullOrWhiteSpace(bindingWorkspacePathValue)
                        ? null
                        : Path.GetFullPath(bindingWorkspacePathValue));

            var model = discovery.Model;
            model.SaveToXmlWorkspace(targetValidation.FullPath);

            Presenter.WriteInfo($"Workspace: {targetValidation.FullPath}");
            Presenter.WriteInfo($"Views ready to create: {model.DataQualityCandidateList.Count}");
            Presenter.WriteInfo($"Relationships captured: {model.JoinPatternOccurrenceList.Count}");
            if (!string.IsNullOrWhiteSpace(bindingWorkspacePathValue))
            {
                Presenter.WriteInfo($"Transform scripts scanned: {discovery.AnalyzedTransformScriptCount}/{discovery.TransformScriptCount}");
                Presenter.WriteInfo($"Transform scripts skipped by BindingWS: {discovery.BindingSkippedTransformScriptCount}");
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

    private static void RunInspect(MetaCliInvocation invocation, MetaDataQualityModel model)
    {
        var workspacePath = ResolveWorkspacePath(invocation);
        var topCases = ReadPositiveInt(invocation.Required("top-cases"), "--top-cases");
        var showCases = invocation.Flag("show-cases")
                        || invocation.IsPresent("top-cases")
                        || invocation.Flag("show-candidate-ids");
        var showCandidateIds = invocation.Flag("show-candidate-ids");

        try
        {
            var candidateCount = model.DataQualityCandidateList.Count;
            var promoted = model.DataQualityCandidateList.Count(item =>
                string.Equals(item.Status, CandidateStatuses.Promoted, StringComparison.OrdinalIgnoreCase));

            var candidateTypes = ResolveCandidateTypeMap(model);
            var candidateById = model.DataQualityCandidateList.ToDictionary(item => item.Id, StringComparer.Ordinal);
            var occurrencesByPatternId = model.JoinPatternOccurrenceList
                .GroupBy(item => item.JoinPattern.Id, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
            var baseTablesByOccurrenceId = model.JoinPatternOccurrenceBaseTableList
                .GroupBy(item => item.JoinPatternOccurrence.Id, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
            var keyPartsByPatternId = model.JoinPatternKeyPartList
                .GroupBy(item => item.JoinPattern.Id, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
            var candidateIdsByPattern = model.DataQualityCandidateJoinPatternLinkList
                .GroupBy(item => item.JoinPattern.Id, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.Select(item => item.DataQualityCandidate.Id).Distinct(StringComparer.Ordinal).ToArray(), StringComparer.Ordinal);
            var pending = model.DataQualityCandidateList.Count(item =>
                string.Equals(item.Status, CandidateStatuses.Discovered, StringComparison.OrdinalIgnoreCase));
            var situations = BuildJoinSituations(
                model,
                candidateById,
                candidateTypes,
                occurrencesByPatternId,
                baseTablesByOccurrenceId,
                keyPartsByPatternId,
                candidateIdsByPattern);
            var pendingSituations = situations
                .Where(static item => item.WaitingCount > 0)
                .OrderByDescending(static item => item.WaitingCount)
                .ThenBy(static item => item.JoinDescription, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var viewFamilies = model.DataQualityCandidateList
                .Select(item => candidateTypes.TryGetValue(item.Id, out var kind) ? kind : "(untyped)")
                .Select(ToHumanCheckLabel)
                .GroupBy(static label => label, StringComparer.OrdinalIgnoreCase)
                .OrderBy(group => CheckLabelSortOrder(group.Key))
                .ThenBy(static group => group.Key, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var dominantCorpusPatterns = model.CorpusRelationshipPatternList
                .Where(static row => IsTrueFlag(row.IsDominant))
                .Select(row =>
                {
                    var relationship = row.CorpusRelationship;
                    return new DominantCorpusPatternView(
                        relationship.CanonicalSideAObjectName,
                        relationship.CanonicalSideBObjectName,
                        row.CanonicalKeyPartSetSignature,
                        row.OccurrenceRatio,
                        row.OccurrenceCount,
                        relationship.OccurrenceCount);
                })
                .OrderByDescending(static row => ParseIntOrZero(row.OccurrenceCount))
                .ThenBy(static row => row.LeftObjectName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static row => row.RightObjectName, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var minoritySummary = SummarizeCandidateStatuses(
                model.MinorityJoinPatternList.Select(static row => row.DataQualityCandidate.Id),
                candidateById);
            var incompleteSummary = SummarizeCandidateStatuses(
                model.IncompleteCompositeJoinList.Select(static row => row.DataQualityCandidate.Id),
                candidateById);
            var extraSummary = SummarizeCandidateStatuses(
                model.SuspiciousExtraJoinPredicateList.Select(static row => row.DataQualityCandidate.Id),
                candidateById);
            var missingCommonFilterSummary = SummarizeCandidateStatuses(
                model.MissingCommonFilterList.Select(static row => row.DataQualityCandidate.Id),
                candidateById);
            var minorityColumnEquivalenceSummary = SummarizeCandidateStatuses(
                model.MinorityColumnEquivalenceList.Select(static row => row.DataQualityCandidate.Id),
                candidateById);
            var innerAgainstOptionalSummary = SummarizeCandidateStatuses(
                model.InnerJoinAgainstUsuallyOptionalRelationshipList.Select(static row => row.DataQualityCandidate.Id),
                candidateById);
            var leftAgainstMandatorySummary = SummarizeCandidateStatuses(
                model.LeftJoinAgainstUsuallyMandatoryRelationshipList.Select(static row => row.DataQualityCandidate.Id),
                candidateById);
            var impliedJoinFanoutSummary = SummarizeCandidateStatuses(
                model.ImpliedJoinFanoutRiskList.Select(static row => row.DataQualityCandidate.Id),
                candidateById);
            var impliedOutputDuplicateSummary = SummarizeCandidateStatuses(
                model.ImpliedOutputDuplicateRiskList.Select(static row => row.DataQualityCandidate.Id),
                candidateById);
            Presenter.WriteInfo($"Views ready to create: {candidateCount}");
            if (promoted > 0)
            {
                Presenter.WriteInfo($"Promoted for SQL: {promoted}");
            }

            if (viewFamilies.Length > 0)
            {
                Presenter.WriteInfo(string.Empty);
                Presenter.WriteInfo("Checks:");
                foreach (var family in viewFamilies)
                {
                    Presenter.WriteInfo($"  {family.Key}: {family.Count()}");
                }
            }

            if (situations.Count > 0)
            {
                Presenter.WriteInfo(string.Empty);
                Presenter.WriteInfo($"Relationships captured: {situations.Count}");
            }

            if (model.CorpusRelationshipList.Count > 0)
            {
                Presenter.WriteInfo(string.Empty);
                Presenter.WriteInfo("Corpus Inference:");
                Presenter.WriteInfo($"  Relationships observed: {model.CorpusRelationshipList.Count}");
                Presenter.WriteInfo($"  Dominant relationship patterns: {dominantCorpusPatterns.Length}");
                Presenter.WriteInfo($"  Column equivalence edges: {model.CorpusColumnEquivalenceList.Count}");
                Presenter.WriteInfo($"  Minority join-pattern candidates: {FormatStatusSummary(minoritySummary)}");
                Presenter.WriteInfo($"  Incomplete-composite candidates: {FormatStatusSummary(incompleteSummary)}");
                Presenter.WriteInfo($"  Suspicious-extra-predicate candidates: {FormatStatusSummary(extraSummary)}");
                Presenter.WriteInfo($"  Missing-common-filter candidates: {FormatStatusSummary(missingCommonFilterSummary)}");
                Presenter.WriteInfo($"  Minority column-equivalence candidates: {FormatStatusSummary(minorityColumnEquivalenceSummary)}");
                Presenter.WriteInfo($"  Optionality-drift (inner vs usually optional): {FormatStatusSummary(innerAgainstOptionalSummary)}");
                Presenter.WriteInfo($"  Optionality-drift (left vs usually mandatory): {FormatStatusSummary(leftAgainstMandatorySummary)}");
                Presenter.WriteInfo($"  Implied fanout-risk candidates: {FormatStatusSummary(impliedJoinFanoutSummary)}");
                Presenter.WriteInfo($"  Implied output-duplicate-risk candidates: {FormatStatusSummary(impliedOutputDuplicateSummary)}");
            }

            if (pending > 0)
            {
                Presenter.WriteInfo(string.Empty);
                Presenter.WriteInfo($"Pending candidates: {pending}");
            }

            if (showCases && pendingSituations.Length > 0)
            {
                Presenter.WriteInfo(string.Empty);
                Presenter.WriteInfo("Relationship Cases:");
                var visible = pendingSituations.Take(topCases).ToArray();
                for (var i = 0; i < visible.Length; i++)
                {
                    var situation = visible[i];
                    Presenter.WriteInfo($"  {i + 1}. {situation.JoinDescription}");
                    Presenter.WriteInfo($"     Keys: {situation.JoinCondition}");
                    Presenter.WriteInfo($"     SQL join: {situation.JoinType}");
                    Presenter.WriteInfo($"     Checks: {string.Join(", ", situation.ViewLabels)}");
                    if (showCandidateIds)
                    {
                        Presenter.WriteInfo($"     Candidate ids: {string.Join(", ", situation.PendingCandidateIds)}");
                    }
                }

                if (pendingSituations.Length > visible.Length)
                {
                    Presenter.WriteInfo($"  Showing {visible.Length} of {pendingSituations.Length} relationships. Increase with --top-cases.");
                }
            }
            else if (showCases && pendingSituations.Length == 0)
            {
                Presenter.WriteInfo(string.Empty);
                Presenter.WriteInfo("  No generated candidates remain to promote.");
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

    private static void RunPromote(MetaCliInvocation invocation, MetaDataQualityModel model)
    {
        var workspacePath = ResolveWorkspacePath(invocation);
        var candidateIds = invocation.Values("candidate-id");
        var promoteAll = invocation.Flag("all");

        try
        {
            var promotedCount = 0;
            if (promoteAll)
            {
                foreach (var candidate in model.DataQualityCandidateList)
                {
                    if (!string.Equals(candidate.Status, CandidateStatuses.Promoted, StringComparison.OrdinalIgnoreCase))
                    {
                        candidate.Status = CandidateStatuses.Promoted;
                        promotedCount++;
                    }
                }
            }
            else
            {
                var byId = model.DataQualityCandidateList
                    .ToDictionary(item => item.Id, StringComparer.Ordinal);
                foreach (var candidateId in candidateIds.Distinct(StringComparer.Ordinal))
                {
                    if (!byId.TryGetValue(candidateId, out var candidate) || candidate is null)
                    {
                        Fail(
                            $"Data quality candidate id '{candidateId}' was not found.",
                            "run meta-data-quality inspect --workspace <path> and retry.",
                            4,
                            new[] { $"  Workspace: {workspacePath}" });
                        continue;
                    }

                    if (!string.Equals(candidate.Status, CandidateStatuses.Promoted, StringComparison.OrdinalIgnoreCase))
                    {
                        candidate.Status = CandidateStatuses.Promoted;
                        promotedCount++;
                    }
                }
            }

            model.SaveToXmlWorkspace(workspacePath);
            var totalPromoted = model.DataQualityCandidateList.Count(item =>
                string.Equals(item.Status, CandidateStatuses.Promoted, StringComparison.OrdinalIgnoreCase));
            Presenter.WriteInfo($"Candidates promoted this run: {promotedCount}");
            Presenter.WriteInfo($"Candidates promoted for SQL: {totalPromoted}");
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

    private static int ReadPositiveInt(string value, string parameterName)
    {
        if (int.TryParse(value, out var result) && result > 0)
        {
            return result;
        }

        Fail($"{parameterName} must be a positive integer.", "run meta-data-quality help inspect and retry.");
        throw new MetaCliExitException(2);
    }

    private static void Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details.Where(static item => !string.IsNullOrWhiteSpace(item)));
        }

        renderedDetails.Add($"Next: {next}");
        Presenter.WriteFailure(message, renderedDetails);
        throw new MetaCliExitException(exitCode);
    }

    private static IReadOnlyDictionary<string, string> ResolveCandidateTypeMap(MetaDataQualityModel model)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        AddType(map, model.JoinOrphanList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.JoinOrphan);
        AddType(map, model.OuterJoinNullExpansionList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.OuterJoinNullExpansion);
        AddType(map, model.JoinMultiplicityExplosionList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.JoinMultiplicityExplosion);
        AddType(map, model.OutputDuplicateRiskList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.OutputDuplicateRisk);
        AddType(map, model.MinorityJoinPatternList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.MinorityJoinPattern);
        AddType(map, model.IncompleteCompositeJoinList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.IncompleteCompositeJoin);
        AddType(map, model.SuspiciousExtraJoinPredicateList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.SuspiciousExtraJoinPredicate);
        AddType(map, model.MissingCommonFilterList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.MissingCommonFilter);
        AddType(map, model.MinorityColumnEquivalenceList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.MinorityColumnEquivalence);
        AddType(map, model.InnerJoinAgainstUsuallyOptionalRelationshipList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship);
        AddType(map, model.LeftJoinAgainstUsuallyMandatoryRelationshipList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship);
        AddType(map, model.ImpliedForeignKeyMissingReferenceList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedForeignKeyMissingReference);
        AddType(map, model.ImpliedUniqueKeyViolationList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedUniqueKeyViolation);
        AddType(map, model.ImpliedJoinFanoutRiskList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedJoinFanoutRisk);
        AddType(map, model.ImpliedOutputDuplicateRiskList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedOutputDuplicateRisk);
        return map;
    }

    private static void AddType(
        IDictionary<string, string> map,
        IEnumerable<string> candidateIds,
        string candidateType)
    {
        foreach (var candidateId in candidateIds.Where(static id => !string.IsNullOrWhiteSpace(id)))
        {
            if (!map.TryAdd(candidateId, candidateType))
            {
                map[candidateId] = candidateType;
            }
        }
    }

    private static string FormatQualifiedJoinType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "(unspecified)";
        }

        return value switch
        {
            "Inner" => "Inner",
            "LeftOuter" => "Left Outer",
            "RightOuter" => "Right Outer",
            "FullOuter" => "Full Outer",
            "Cross" => "Cross",
            _ => value,
        };
    }

    private static string[] ResolveSideTableNames(
        IReadOnlyDictionary<string, JoinPatternOccurrenceBaseTable[]> baseTablesByOccurrenceId,
        string joinPatternOccurrenceId,
        string? joinInputTableReferenceId)
    {
        if (!baseTablesByOccurrenceId.TryGetValue(joinPatternOccurrenceId, out var rows))
        {
            return [];
        }

        return rows
            .Where(row => string.Equals(row.JoinInputTableReferenceId, joinInputTableReferenceId, StringComparison.Ordinal))
            .Select(static row => row.BaseObjectName)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string ResolveJoinOnText(
        IReadOnlyDictionary<string, JoinPatternKeyPart[]> keyPartsByPatternId,
        string joinPatternId)
    {
        if (!keyPartsByPatternId.TryGetValue(joinPatternId, out var keyParts) || keyParts.Length == 0)
        {
            return "(no equality predicates captured)";
        }

        var parts = keyParts
            .OrderBy(static row => ParseOrdinalOrMax(row.Ordinal))
            .Select(static row =>
            {
                var left = FormatExpressionDisplay(string.IsNullOrWhiteSpace(row.FirstExpressionDisplay) ? row.FirstExpressionId : row.FirstExpressionDisplay);
                var right = FormatExpressionDisplay(string.IsNullOrWhiteSpace(row.SecondExpressionDisplay) ? row.SecondExpressionId : row.SecondExpressionDisplay);
                return $"{left} = {right}";
            })
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
        return parts.Length == 0
            ? "(no equality predicates captured)"
            : string.Join("; ", parts);
    }

    private static int ParseOrdinalOrMax(string ordinal)
    {
        return int.TryParse(ordinal, out var parsed)
            ? parsed
            : int.MaxValue;
    }

    private static List<JoinSituationView> BuildJoinSituations(
        MetaDataQualityModel model,
        IReadOnlyDictionary<string, DataQualityCandidate> candidateById,
        IReadOnlyDictionary<string, string> candidateTypes,
        IReadOnlyDictionary<string, JoinPatternOccurrence[]> occurrencesByPatternId,
        IReadOnlyDictionary<string, JoinPatternOccurrenceBaseTable[]> baseTablesByOccurrenceId,
        IReadOnlyDictionary<string, JoinPatternKeyPart[]> keyPartsByPatternId,
        IReadOnlyDictionary<string, string[]> candidateIdsByPattern)
    {
        var result = new List<JoinSituationView>();
        foreach (var pattern in model.JoinPatternList
                     .OrderBy(item => FormatQualifiedJoinType(item.QualifiedJoinType), StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static item => item.CanonicalSignature, StringComparer.Ordinal))
        {
            var patternId = pattern.Id;
            var occurrences = occurrencesByPatternId.TryGetValue(patternId, out var groupedOccurrences)
                ? groupedOccurrences
                : [];
            var anchor = occurrences
                .OrderBy(static item => item.TransformScriptName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static item => item.QualifiedJoinId, StringComparer.Ordinal)
                .FirstOrDefault();

            var joinDescription = "Unknown table joined with unknown table";
            if (anchor != null)
            {
                var leftTables = ResolveSideTableNames(
                    baseTablesByOccurrenceId,
                    anchor.Id,
                    anchor.FirstTableReferenceId);
                var rightTables = ResolveSideTableNames(
                    baseTablesByOccurrenceId,
                    anchor.Id,
                    anchor.SecondTableReferenceId);
                var left = FormatTableSide(leftTables, "(unknown left)");
                var right = FormatTableSide(rightTables, "(unknown right)");
                joinDescription = $"{left} joined with {right}";
            }

            var candidateIds = candidateIdsByPattern.TryGetValue(patternId, out var links)
                ? links.Where(candidateById.ContainsKey).Distinct(StringComparer.Ordinal).OrderBy(static id => id, StringComparer.Ordinal).ToArray()
                : [];
            var viewLabels = candidateIds
                .Select(id => candidateTypes.TryGetValue(id, out var type) ? type : "(untyped)")
                .Select(ToHumanCheckLabel)
                .Where(static label => !string.IsNullOrWhiteSpace(label))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(CheckLabelSortOrder)
                .ThenBy(static label => label, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var pendingCandidateIds = candidateIds
                .Where(id => candidateById.TryGetValue(id, out var candidate)
                             && string.Equals(candidate.Status, CandidateStatuses.Discovered, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            var waitingCount = pendingCandidateIds.Length;

            result.Add(new JoinSituationView(
                joinDescription,
                ResolveJoinOnText(keyPartsByPatternId, patternId),
                FormatQualifiedJoinType(pattern.QualifiedJoinType),
                viewLabels.Length == 0 ? ["(none)"] : viewLabels,
                waitingCount,
                pendingCandidateIds));
        }

        return result;
    }

    private static string FormatExpressionDisplay(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "(scalar expression)";
        }

        var trimmed = value.Trim();
        if (trimmed.StartsWith("ScalarExpression:", StringComparison.OrdinalIgnoreCase))
        {
            return "(scalar expression)";
        }

        const int maxLength = 120;
        if (trimmed.Length <= maxLength)
        {
            return trimmed;
        }

        return $"{trimmed[..58]}...{trimmed[^58..]}";
    }

    private static string FormatTableSide(IReadOnlyList<string> tableNames, string emptyText)
    {
        if (tableNames.Count == 0)
        {
            return emptyText;
        }

        const int maxVisibleTables = 3;
        if (tableNames.Count <= maxVisibleTables)
        {
            return string.Join(", ", tableNames);
        }

        return $"{string.Join(", ", tableNames.Take(maxVisibleTables))}, ... (+{tableNames.Count - maxVisibleTables})";
    }

    private static bool IsTrueFlag(string value) =>
        string.Equals(value?.Trim(), "true", StringComparison.OrdinalIgnoreCase);

    private static int ParseIntOrZero(string value)
    {
        return int.TryParse(value, out var parsed)
            ? parsed
            : 0;
    }

    private static CandidateStatusSummary SummarizeCandidateStatuses(
        IEnumerable<string> candidateIds,
        IReadOnlyDictionary<string, DataQualityCandidate> candidateById)
    {
        var total = 0;
        var promoted = 0;
        foreach (var candidateId in candidateIds
                     .Where(static id => !string.IsNullOrWhiteSpace(id))
                     .Distinct(StringComparer.Ordinal))
        {
            if (!candidateById.TryGetValue(candidateId, out var candidate))
            {
                continue;
            }

            total++;
            if (string.Equals(candidate.Status, CandidateStatuses.Promoted, StringComparison.OrdinalIgnoreCase))
            {
                promoted++;
            }
        }

        return new CandidateStatusSummary(total, promoted);
    }

    private static string FormatStatusSummary(CandidateStatusSummary summary)
    {
        if (summary.Total == 0)
        {
            return "0";
        }

        return $"{summary.Total} (waiting {summary.Waiting}, promoted {summary.Promoted})";
    }

    private static string ToHumanCheckLabel(string candidateType)
    {
        return candidateType switch
        {
            CandidateKinds.JoinOrphan => "Missing referenced rows",
            CandidateKinds.OuterJoinNullExpansion => "Unexpected NULLs from outer joins",
            CandidateKinds.JoinMultiplicityExplosion => "Row multiplication",
            CandidateKinds.OutputDuplicateRisk => "Duplicate output rows",
            CandidateKinds.MinorityJoinPattern => "Minority join pattern",
            CandidateKinds.IncompleteCompositeJoin => "Incomplete composite join",
            CandidateKinds.SuspiciousExtraJoinPredicate => "Suspicious extra join predicate",
            CandidateKinds.MissingCommonFilter => "Missing common filter",
            CandidateKinds.MinorityColumnEquivalence => "Minority column equivalence",
            CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship => "Inner join against usually optional side",
            CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship => "Left join against usually mandatory side",
            CandidateKinds.ImpliedForeignKeyMissingReference => "Implied missing referenced rows",
            CandidateKinds.ImpliedUniqueKeyViolation => "Implied unique-key violation",
            CandidateKinds.ImpliedJoinFanoutRisk => "Implied join fanout risk",
            CandidateKinds.ImpliedOutputDuplicateRisk => "Implied output duplicate risk",
            _ => candidateType,
        };
    }

    private static int CheckLabelSortOrder(string label)
    {
        return label switch
        {
            "Row multiplication" => 0,
            "Missing referenced rows" => 1,
            "Duplicate output rows" => 2,
            "Unexpected NULLs from outer joins" => 3,
            "Minority join pattern" => 4,
            "Incomplete composite join" => 5,
            "Suspicious extra join predicate" => 6,
            "Missing common filter" => 7,
            "Minority column equivalence" => 8,
            "Inner join against usually optional side" => 9,
            "Left join against usually mandatory side" => 10,
            "Implied missing referenced rows" => 11,
            "Implied unique-key violation" => 12,
            "Implied join fanout risk" => 13,
            "Implied output duplicate risk" => 14,
            _ => 100,
        };
    }

    private readonly record struct JoinSituationView(
        string JoinDescription,
        string JoinCondition,
        string JoinType,
        string[] ViewLabels,
        int WaitingCount,
        string[] PendingCandidateIds);

    private readonly record struct CandidateStatusSummary(
        int Total,
        int Promoted)
    {
        public int Waiting => Math.Max(Total - Promoted, 0);
    }

    private readonly record struct DominantCorpusPatternView(
        string LeftObjectName,
        string RightObjectName,
        string KeySignature,
        string OccurrenceRatio,
        string OccurrenceCount,
        string RelationshipOccurrenceCount);

}
