using Meta.Core.Presentation;
using Meta.Core.Presentation.Cli;
using MetaDataQuality;
using MetaDataQuality.Core;

internal static class Program
{
    private const string AppName = "meta-data-quality";

    private static readonly ConsolePresenter Presenter = new();
    private static readonly IReadOnlyList<CliCommandRoute> CommandRoutes = BuildCommandRoutes();
    private static readonly IReadOnlyDictionary<string, CliCommandRoute> CommandRoutesByName = CommandRoutes
        .ToDictionary(route => route.Definition.Name, StringComparer.OrdinalIgnoreCase);
    private static readonly CliAppDefinition Cli = new(
        AppName,
        new[]
        {
            "meta-data-quality <command> [options]"
        },
        CommandRoutes.Select(route => route.Definition).ToArray(),
        Next: "meta-data-quality from-transform-workspace --help");

    private static IReadOnlyList<CliCommandRoute> BuildCommandRoutes() =>
        new[]
        {
            new CliCommandRoute(
                new CliCommandDefinition(
                    "help",
                    "Show this help.",
                    new[] { "meta-data-quality help" }),
                _ =>
                {
                    PrintHelp();
                    return Task.FromResult(0);
                }),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "from-transform-workspace",
                    "Create generated DQ views from a full MetaTransformScript workspace.",
                    new[] { "meta-data-quality from-transform-workspace --transform-workspace <path> --new-workspace <path>" },
                    new[]
                    {
                        new CliOptionDefinition("--transform-workspace <path>", "Required. MetaTransformScript workspace to analyze."),
                        new CliOptionDefinition("--new-workspace <path>", "Required. Directory where the generated MetaDataQuality workspace will be created.")
                    },
                    new[]
                    {
                        "Scans all TransformScript instances in one workspace.",
                        "Creates one MetaDataQuality workspace with generated DQ views."
                    }),
                args => Task.FromResult(RunCommandWithHelp(args, "from-transform-workspace", commandArgs => RunFromTransformWorkspace(commandArgs, startIndex: 1)))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "inspect",
                    "Review the generated DQ pack and optional adjustments.",
                    new[] { "meta-data-quality inspect --workspace <path> [--show-cases] [--top-cases <n>] [--show-candidate-ids]" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaDataQuality workspace to inspect."),
                        new CliOptionDefinition("--show-cases", "Optional. Show candidate adjustment cases."),
                        new CliOptionDefinition("--top-cases <n>", "Optional. Show up to n candidate cases. Implies --show-cases. Default: 20."),
                        new CliOptionDefinition("--show-candidate-ids", "Optional. Include candidate ids in adjustment output. Implies --show-cases.")
                    },
                    new[]
                    {
                        "Default output guides the full-pack-first path.",
                        "Use --show-cases when you want to make small adjustments before SQL generation.",
                        "Use --show-candidate-ids when promoting individual generated candidates."
                    }),
                args => Task.FromResult(RunCommandWithHelp(args, "inspect", commandArgs => RunInspect(commandArgs, startIndex: 1)))),
            new CliCommandRoute(
                new CliCommandDefinition(
                    "promote",
                    "Promote generated DQ candidates for SQL output.",
                    new[] { "meta-data-quality promote --workspace <path> (--all | --candidate-id <id> [--candidate-id <id> ...])" },
                    new[]
                    {
                        new CliOptionDefinition("--workspace <path>", "Required. MetaDataQuality workspace to update."),
                        new CliOptionDefinition("--all", "Promote every generated data-quality candidate."),
                        new CliOptionDefinition("--candidate-id <id>", "Promote one generated candidate. May be provided more than once.")
                    },
                    new[]
                    {
                        "Promotes generated DQ candidates for data-quality-to-sql output."
                    }),
                args => Task.FromResult(RunCommandWithHelp(args, "promote", commandArgs => RunPromote(commandArgs, startIndex: 1))))
        };

    static int Main(string[] args)
    {
        if (Meta.Core.Presentation.Cli.CliVersion.TryWriteVersion(Presenter, Cli.Name, args, out var versionExitCode))
        {
            return versionExitCode;
        }

        if (args.Length == 0 || IsHelpToken(args[0]))
        {
            PrintHelp();
            return 0;
        }

        if (CommandRoutesByName.TryGetValue(args[0], out var route))
        {
            return route.ExecuteAsync(args).GetAwaiter().GetResult();
        }

        return Fail($"unknown command '{args[0]}'.", $"{AppName} help");
    }

    private static int RunFromTransformWorkspace(string[] args, int startIndex)
    {
        var parse = ParseFromTransformWorkspaceArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("from-transform-workspace"));
        }

        var targetValidation = CliNewWorkspaceTargetValidator.Validate(parse.NewWorkspacePath);
        if (!targetValidation.Ok)
        {
            return Fail(
                targetValidation.ErrorMessage,
                "choose a new folder or empty the target directory and retry.",
                4,
                targetValidation.Details);
        }

        try
        {
            var transformWorkspacePath = Path.GetFullPath(parse.TransformWorkspacePath);
            var discovery = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);

            var model = discovery.Model;
            model.SaveToXmlWorkspace(targetValidation.FullPath);

            Presenter.WriteOk($"Created {Path.GetFileName(targetValidation.FullPath)}");
            Presenter.WriteInfo("MetaDataQuality can create SQL views that return rows to investigate.");
            Presenter.WriteInfo($"  Views ready to create: {model.DataQualityCandidateList.Count}");
            Presenter.WriteInfo(string.Empty);
            Presenter.WriteInfo("Run This First:");
            Presenter.WriteInfo($"  meta-data-quality promote --workspace \"{targetValidation.FullPath}\" --all");
            Presenter.WriteInfo($"  meta-convert data-quality-to-sql --workspace \"{targetValidation.FullPath}\" --out DataQualityViews.sql");
            Presenter.WriteInfo(string.Empty);
            Presenter.WriteInfo("Review The Results:");
            Presenter.WriteInfo("  Run the generated views and decide whether the reported rows are real data-quality issues.");
            Presenter.WriteInfo("  Some rows may be valid for your business rules; keep the views that reveal useful problems.");
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot create data-quality workspace.",
                "check the transform workspace and retry.",
                4,
                new[]
                {
                    $"  TransformWorkspace: {Path.GetFullPath(parse.TransformWorkspacePath)}",
                    $"  {ex.Message}",
                });
        }
    }

    private static int RunInspect(string[] args, int startIndex)
    {
        var parse = ParseInspectArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("inspect"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MetaDataQualityModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
            var candidateCount = model.DataQualityCandidateList.Count;
            var promoted = model.DataQualityCandidateList.Count(item =>
                string.Equals(item.Status, CandidateStatuses.Promoted, StringComparison.OrdinalIgnoreCase));

            Presenter.WriteOk("Loaded MetaDataQuality workspace");

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
            var relationshipPreview = situations
                .OrderByDescending(static item => item.WaitingCount)
                .ThenBy(static item => item.JoinDescription, StringComparer.OrdinalIgnoreCase)
                .Take(5)
                .ToArray();
            var corpusRelationshipById = model.CorpusRelationshipList
                .ToDictionary(item => item.Id, StringComparer.Ordinal);
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
            var impliedSummaryRows = BuildImpliedEvidenceSummaryRows(
                model,
                candidateById,
                candidateTypes,
                corpusRelationshipById);
            var optionalitySummaryRows = BuildOptionalityEvidenceSummaryRows(
                model,
                candidateById,
                candidateTypes,
                corpusRelationshipById);

            Presenter.WriteInfo(string.Empty);
            Presenter.WriteInfo("MetaDataQuality can create SQL views that return rows to investigate.");
            Presenter.WriteInfo($"  Views ready to create: {candidateCount}");
            if (promoted > 0)
            {
                Presenter.WriteInfo($"  Promoted for SQL: {promoted}");
            }

            Presenter.WriteInfo(string.Empty);
            Presenter.WriteInfo("Why These Views Exist:");
            Presenter.WriteInfo("  MetaDataQuality found joins in the transform workspace.");
            Presenter.WriteInfo("  The generated views check whether the data behind those joins behaves as expected.");

            if (viewFamilies.Length > 0)
            {
                Presenter.WriteInfo(string.Empty);
                Presenter.WriteInfo("The Views Check For:");
                foreach (var family in viewFamilies)
                {
                    Presenter.WriteInfo($"  {family.Key}: {DescribeCheckLabel(family.Key)}");
                }
            }

            if (relationshipPreview.Length > 0)
            {
                Presenter.WriteInfo(string.Empty);
                Presenter.WriteInfo("Relationships Checked:");
                for (var i = 0; i < relationshipPreview.Length; i++)
                {
                    var situation = relationshipPreview[i];
                    Presenter.WriteInfo($"  {i + 1}. {situation.JoinDescription}");
                    Presenter.WriteInfo($"     Keys: {situation.JoinCondition}");
                    Presenter.WriteInfo($"     Views: {string.Join(", ", situation.ViewLabels)}");
                }

                if (situations.Count > relationshipPreview.Length)
                {
                    Presenter.WriteInfo($"  Showing {relationshipPreview.Length} of {situations.Count} relationships. Use --show-cases to see more.");
                }
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

                if (dominantCorpusPatterns.Length > 0)
                {
                    Presenter.WriteInfo("  Dominant relationship preview:");
                    foreach (var item in dominantCorpusPatterns.Take(5))
                    {
                        Presenter.WriteInfo(
                            $"    {item.LeftObjectName} <-> {item.RightObjectName} | Ratio {item.OccurrenceRatio} ({item.OccurrenceCount}/{item.RelationshipOccurrenceCount})");
                    }
                }

                if (impliedSummaryRows.Length > 0)
                {
                    Presenter.WriteInfo("  Implied relationship checks:");
                    foreach (var row in impliedSummaryRows)
                    {
                        Presenter.WriteInfo(
                            $"    {ToHumanCheckLabel(row.CandidateType)} | {row.LeftObjectName} <-> {row.RightObjectName} | Consensus {row.ConsensusRatio}, Outlier {row.OutlierRatio} | Confidence {row.ConfidenceBand} | {row.Status}");
                        Presenter.WriteInfo($"      Diversity: {row.EvidenceDiversitySummary}");
                    }
                }

                if (optionalitySummaryRows.Length > 0)
                {
                    Presenter.WriteInfo("  Optionality drift checks:");
                    foreach (var row in optionalitySummaryRows)
                    {
                        Presenter.WriteInfo(
                            $"    {ToHumanCheckLabel(row.CandidateType)} | {row.LeftObjectName} <-> {row.RightObjectName} | {row.Explanation} | Consensus {row.ConsensusRatio}, Outlier {row.OutlierRatio} | Confidence {row.ConfidenceBand} | {row.Status}");
                        Presenter.WriteInfo($"      Diversity: {row.EvidenceDiversitySummary}");
                    }
                }
            }

            Presenter.WriteInfo(string.Empty);
            Presenter.WriteInfo("Run This First:");
            if (pending > 0)
            {
                Presenter.WriteInfo($"  meta-data-quality promote --workspace \"{workspacePath}\" --all");
                Presenter.WriteInfo($"  meta-convert data-quality-to-sql --workspace \"{workspacePath}\" --out DataQualityViews.sql");
            }
            else
            {
                Presenter.WriteInfo($"  meta-convert data-quality-to-sql --workspace \"{workspacePath}\" --out DataQualityViews.sql");
            }
            Presenter.WriteInfo(string.Empty);
            Presenter.WriteInfo("Review The Results:");
            Presenter.WriteInfo("  Run the generated views and decide whether the reported rows are real data-quality issues.");
            Presenter.WriteInfo("  Some rows may be valid for your business rules; keep the views that reveal useful problems.");
            if (pending > 0)
            {
                Presenter.WriteInfo(string.Empty);
                Presenter.WriteInfo($"Need to exclude a few candidates? meta-data-quality inspect --workspace \"{workspacePath}\" --show-cases --show-candidate-ids");
            }

            if (parse.ShowCases && pendingSituations.Length > 0)
            {
                Presenter.WriteInfo(string.Empty);
                Presenter.WriteInfo("Adjustment View:");
                Presenter.WriteInfo("  Use this when you want to promote or skip a few generated candidates before creating SQL.");
                var visible = pendingSituations.Take(parse.TopCases).ToArray();
                for (var i = 0; i < visible.Length; i++)
                {
                    var situation = visible[i];
                    Presenter.WriteInfo($"  {i + 1}. {situation.JoinDescription}");
                    Presenter.WriteInfo($"     Keys: {situation.JoinCondition}");
                    Presenter.WriteInfo($"     SQL join: {situation.JoinType}");
                    Presenter.WriteInfo($"     Views: {string.Join(", ", situation.ViewLabels)}");
                    if (parse.ShowCandidateIds)
                    {
                        Presenter.WriteInfo($"     Candidate ids: {string.Join(", ", situation.PendingCandidateIds)}");
                    }
                }

                if (pendingSituations.Length > visible.Length)
                {
                    Presenter.WriteInfo($"  Showing {visible.Length} of {pendingSituations.Length} relationships. Increase with --top-cases.");
                }
            }
            else if (parse.ShowCases && pendingSituations.Length == 0)
            {
                Presenter.WriteInfo(string.Empty);
                Presenter.WriteInfo("Adjustment View:");
                Presenter.WriteInfo("  No generated candidates remain to promote.");
            }

            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot inspect data-quality workspace.",
                "check the workspace path and retry.",
                4,
                new[]
                {
                    $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}",
                    $"  {ex.Message}",
                });
        }
    }

    private static int RunPromote(string[] args, int startIndex)
    {
        var parse = ParsePromoteArgs(args, startIndex);
        if (!parse.Ok)
        {
            return Fail(parse.ErrorMessage, HelpCommand("promote"));
        }

        try
        {
            var workspacePath = Path.GetFullPath(parse.WorkspacePath);
            var model = MetaDataQualityModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);

            var promotedCount = 0;
            if (parse.PromoteAll)
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
                foreach (var candidateId in parse.CandidateIds.Distinct(StringComparer.Ordinal))
                {
                    if (!byId.TryGetValue(candidateId, out var candidate))
                    {
                        return Fail(
                            $"Data quality candidate id '{candidateId}' was not found.",
                            "run meta-data-quality inspect --workspace <path> and retry.",
                            4,
                            new[] { $"  Workspace: {workspacePath}" });
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
            Presenter.WriteOk($"Promoted {promotedCount} data quality candidates");
            Presenter.WriteInfo($"  Candidates promoted for SQL: {totalPromoted}");
            Presenter.WriteInfo(string.Empty);
            Presenter.WriteInfo("Next:");
            Presenter.WriteInfo($"  meta-convert data-quality-to-sql --workspace \"{workspacePath}\" --out DataQualityViews.sql");
            return 0;
        }
        catch (Exception ex)
        {
            return Fail(
                "Cannot promote data-quality candidates.",
                "check the workspace and candidate ids, then retry.",
                4,
                new[]
                {
                    $"  Workspace: {Path.GetFullPath(parse.WorkspacePath)}",
                    $"  {ex.Message}",
                });
        }
    }

    private static (bool Ok, string TransformWorkspacePath, string NewWorkspacePath, string ErrorMessage) ParseFromTransformWorkspaceArgs(string[] args, int startIndex)
    {
        var transformWorkspacePath = string.Empty;
        var newWorkspacePath = string.Empty;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--transform-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, newWorkspacePath, "missing value for --transform-workspace.");
                if (!string.IsNullOrWhiteSpace(transformWorkspacePath)) return (false, transformWorkspacePath, newWorkspacePath, "--transform-workspace can only be provided once.");
                transformWorkspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--new-workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, transformWorkspacePath, newWorkspacePath, "missing value for --new-workspace.");
                if (!string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, transformWorkspacePath, newWorkspacePath, "--new-workspace can only be provided once.");
                newWorkspacePath = args[++i];
                continue;
            }

            return (false, transformWorkspacePath, newWorkspacePath, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(transformWorkspacePath)) return (false, transformWorkspacePath, newWorkspacePath, "missing required option --transform-workspace <path>.");
        if (string.IsNullOrWhiteSpace(newWorkspacePath)) return (false, transformWorkspacePath, newWorkspacePath, "missing required option --new-workspace <path>.");
        return (true, transformWorkspacePath, newWorkspacePath, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, string ErrorMessage) ParseWorkspaceOnlyArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, "missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return (false, workspacePath, "--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }

            return (false, workspacePath, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return (false, workspacePath, "missing required option --workspace <path>.");
        }

        return (true, workspacePath, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, bool ShowCases, int TopCases, bool ShowCandidateIds, string ErrorMessage) ParseInspectArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var showCases = false;
        var topCases = 20;
        var showCandidateIds = false;
        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, showCases, topCases, showCandidateIds, "missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return (false, workspacePath, showCases, topCases, showCandidateIds, "--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--show-cases", StringComparison.OrdinalIgnoreCase))
            {
                showCases = true;
                continue;
            }

            if (string.Equals(arg, "--top-cases", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, showCases, topCases, showCandidateIds, "missing value for --top-cases.");
                var value = args[++i];
                if (!int.TryParse(value, out topCases) || topCases <= 0)
                {
                    return (false, workspacePath, showCases, topCases, showCandidateIds, "--top-cases must be a positive integer.");
                }

                showCases = true;
                continue;
            }

            if (string.Equals(arg, "--show-candidate-ids", StringComparison.OrdinalIgnoreCase))
            {
                showCandidateIds = true;
                showCases = true;
                continue;
            }

            return (false, workspacePath, showCases, topCases, showCandidateIds, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return (false, workspacePath, showCases, topCases, showCandidateIds, "missing required option --workspace <path>.");
        }

        return (true, workspacePath, showCases, topCases, showCandidateIds, string.Empty);
    }

    private static (bool Ok, string WorkspacePath, IReadOnlyList<string> CandidateIds, bool PromoteAll, string ErrorMessage) ParsePromoteArgs(string[] args, int startIndex)
    {
        var workspacePath = string.Empty;
        var candidateIds = new List<string>();
        var promoteAll = false;

        for (var i = startIndex; i < args.Length; i++)
        {
            var arg = args[i];
            if (string.Equals(arg, "--workspace", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, candidateIds, promoteAll, "missing value for --workspace.");
                if (!string.IsNullOrWhiteSpace(workspacePath)) return (false, workspacePath, candidateIds, promoteAll, "--workspace can only be provided once.");
                workspacePath = args[++i];
                continue;
            }

            if (string.Equals(arg, "--candidate-id", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 >= args.Length) return (false, workspacePath, candidateIds, promoteAll, "missing value for --candidate-id.");
                candidateIds.Add(args[++i]);
                continue;
            }

            if (string.Equals(arg, "--all", StringComparison.OrdinalIgnoreCase))
            {
                promoteAll = true;
                continue;
            }

            return (false, workspacePath, candidateIds, promoteAll, $"unknown option '{arg}'.");
        }

        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            return (false, workspacePath, candidateIds, promoteAll, "missing required option --workspace <path>.");
        }

        if (promoteAll && candidateIds.Count > 0)
        {
            return (false, workspacePath, candidateIds, promoteAll, "use either --all or one-or-more --candidate-id values, not both.");
        }

        if (!promoteAll && candidateIds.Count == 0)
        {
            return (false, workspacePath, candidateIds, promoteAll, "specify --all or one-or-more --candidate-id values.");
        }

        return (true, workspacePath, candidateIds, promoteAll, string.Empty);
    }

    private static bool IsHelpToken(string value)
    {
        return string.Equals(value, "help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        CliHelpRenderer.WriteAppHelp(Presenter, Cli);
    }

    private static void PrintFromTransformWorkspaceHelp()
    {
        PrintCommandHelp("from-transform-workspace");
    }

    private static void PrintInspectHelp()
    {
        PrintCommandHelp("inspect");
    }

    private static void PrintPromoteHelp()
    {
        PrintCommandHelp("promote");
    }

    private static int RunCommandWithHelp(string[] args, string commandName, Func<string[], int> execute)
    {
        if (args.Length >= 2 && IsHelpToken(args[1]))
        {
            PrintCommandHelp(commandName);
            return 0;
        }

        return execute(args);
    }

    private static void PrintCommandHelp(string commandName)
    {
        CliHelpRenderer.WriteCommandHelp(Presenter, Cli, Cli.GetCommand(commandName));
    }

    private static string HelpCommand(string commandName) => Cli.GetCommand(commandName).HelpCommand(Cli.Name);

    private static int Fail(string message, string next, int exitCode = 1, IEnumerable<string>? details = null)
    {
        var renderedDetails = new List<string>();
        if (details != null)
        {
            renderedDetails.AddRange(details.Where(static item => !string.IsNullOrWhiteSpace(item)));
        }

        renderedDetails.Add($"Next: {next}");
        Presenter.WriteFailure(message, renderedDetails);
        return exitCode;
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

    private static string FormatQualifiedJoinType(string value)
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
        string joinInputTableReferenceId)
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
                var left = string.IsNullOrWhiteSpace(row.FirstExpressionDisplay) ? row.FirstExpressionId : row.FirstExpressionDisplay;
                var right = string.IsNullOrWhiteSpace(row.SecondExpressionDisplay) ? row.SecondExpressionId : row.SecondExpressionDisplay;
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
                var left = leftTables.Length == 0 ? "(unknown left)" : string.Join(", ", leftTables);
                var right = rightTables.Length == 0 ? "(unknown right)" : string.Join(", ", rightTables);
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

    private static ImpliedEvidenceSummaryRow[] BuildImpliedEvidenceSummaryRows(
        MetaDataQualityModel model,
        IReadOnlyDictionary<string, DataQualityCandidate> candidateById,
        IReadOnlyDictionary<string, string> candidateTypes,
        IReadOnlyDictionary<string, CorpusRelationship> corpusRelationshipById)
    {
        var rows = new List<ImpliedEvidenceSummaryRow>();
        foreach (var evidence in model.DataQualityCandidateEvidenceList)
        {
            if (!candidateById.TryGetValue(evidence.DataQualityCandidate.Id, out var candidate))
            {
                continue;
            }

            if (!candidateTypes.TryGetValue(candidate.Id, out var candidateType))
            {
                continue;
            }

            if (!string.Equals(candidateType, CandidateKinds.ImpliedForeignKeyMissingReference, StringComparison.Ordinal)
                && !string.Equals(candidateType, CandidateKinds.ImpliedUniqueKeyViolation, StringComparison.Ordinal)
                && !string.Equals(candidateType, CandidateKinds.ImpliedJoinFanoutRisk, StringComparison.Ordinal)
                && !string.Equals(candidateType, CandidateKinds.ImpliedOutputDuplicateRisk, StringComparison.Ordinal))
            {
                continue;
            }

            if (!corpusRelationshipById.TryGetValue(evidence.CorpusRelationship.Id, out var relationship))
            {
                continue;
            }

            rows.Add(new ImpliedEvidenceSummaryRow(
                relationship.CanonicalSideAObjectName,
                relationship.CanonicalSideBObjectName,
                candidateType,
                evidence.ConsensusRatio,
                evidence.OutlierRatio,
                evidence.ConfidenceBand,
                evidence.EvidenceDiversitySummary,
                candidate.Status));
        }

        return rows
            .OrderByDescending(static row => ParseRatioOrZero(row.ConsensusRatio))
            .ThenBy(static row => row.LeftObjectName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static row => row.RightObjectName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static row => row.CandidateType, StringComparer.Ordinal)
            .Take(5)
            .ToArray();
    }

    private static OptionalityEvidenceSummaryRow[] BuildOptionalityEvidenceSummaryRows(
        MetaDataQualityModel model,
        IReadOnlyDictionary<string, DataQualityCandidate> candidateById,
        IReadOnlyDictionary<string, string> candidateTypes,
        IReadOnlyDictionary<string, CorpusRelationship> corpusRelationshipById)
    {
        var rows = new List<OptionalityEvidenceSummaryRow>();
        foreach (var evidence in model.DataQualityCandidateEvidenceList)
        {
            if (!candidateById.TryGetValue(evidence.DataQualityCandidate.Id, out var candidate))
            {
                continue;
            }

            if (!candidateTypes.TryGetValue(candidate.Id, out var candidateType))
            {
                continue;
            }

            if (!string.Equals(candidateType, CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship, StringComparison.Ordinal)
                && !string.Equals(candidateType, CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship, StringComparison.Ordinal))
            {
                continue;
            }

            if (!corpusRelationshipById.TryGetValue(evidence.CorpusRelationship.Id, out var relationship))
            {
                continue;
            }

            rows.Add(new OptionalityEvidenceSummaryRow(
                relationship.CanonicalSideAObjectName,
                relationship.CanonicalSideBObjectName,
                candidateType,
                evidence.Explanation,
                evidence.ConsensusRatio,
                evidence.OutlierRatio,
                evidence.ConfidenceBand,
                evidence.EvidenceDiversitySummary,
                candidate.Status));
        }

        return rows
            .OrderByDescending(static row => ParseRatioOrZero(row.ConsensusRatio))
            .ThenBy(static row => row.LeftObjectName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static row => row.RightObjectName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static row => row.CandidateType, StringComparer.Ordinal)
            .Take(5)
            .ToArray();
    }

    private static double ParseRatioOrZero(string value)
    {
        return double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0d;
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

    private static string DescribeCheckLabel(string label)
    {
        return label switch
        {
            "Row multiplication" => "a join may turn one business row into several rows.",
            "Missing referenced rows" => "a row may point to related data that is not present.",
            "Duplicate output rows" => "a transform may produce the same business row more than once.",
            "Unexpected NULLs from outer joins" => "an optional join may leave related columns empty.",
            "Minority join pattern" => "a relationship is joined using a rare pattern compared to corpus consensus.",
            "Incomplete composite join" => "an outlier join omits key parts from a dominant composite pattern.",
            "Suspicious extra join predicate" => "an outlier join adds predicates beyond the dominant relationship pattern.",
            "Missing common filter" => "most occurrences apply a common source filter, while the outlier pattern omits it.",
            "Minority column equivalence" => "a column is usually joined to one counterpart column, but minority joins map it to a different counterpart.",
            "Inner join against usually optional side" => "an INNER join appears where the relationship is usually optional on one canonical side.",
            "Left join against usually mandatory side" => "a LEFT join appears where the relationship is usually mandatory in most occurrences.",
            "Implied missing referenced rows" => "high-consensus relationship evidence suggests reference misses should be checked globally.",
            "Implied unique-key violation" => "high-consensus relationship evidence suggests duplicate lookup keys should be checked globally.",
            "Implied join fanout risk" => "high-consensus relationship evidence repeatedly maps to row-multiplication risk signals.",
            "Implied output duplicate risk" => "high-consensus relationship evidence repeatedly maps to duplicate-output risk signals.",
            _ => "review rows returned by the generated view.",
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

    private readonly record struct ImpliedEvidenceSummaryRow(
        string LeftObjectName,
        string RightObjectName,
        string CandidateType,
        string ConsensusRatio,
        string OutlierRatio,
        string ConfidenceBand,
        string EvidenceDiversitySummary,
        string Status);

    private readonly record struct OptionalityEvidenceSummaryRow(
        string LeftObjectName,
        string RightObjectName,
        string CandidateType,
        string Explanation,
        string ConsensusRatio,
        string OutlierRatio,
        string ConfidenceBand,
        string EvidenceDiversitySummary,
        string Status);
}
