using MetaDataQuality;

namespace MetaDataQuality.Core;

public sealed class MetaDataQualityInspectionService
{
    public MetaDataQualityInspectionResult Inspect(MetaDataQualityModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

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
            .Select(group => new MetaDataQualityCheckFamily(group.Key, group.Count()))
            .ToArray();
        var dominantCorpusPatternCount = model.CorpusRelationshipPatternList.Count(static row => IsTrueFlag(row.IsDominant));
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

        return new MetaDataQualityInspectionResult(
            model.DataQualityCandidateList.Count,
            model.DataQualityCandidateList.Count(item =>
                string.Equals(item.Status, CandidateStatuses.Promoted, StringComparison.OrdinalIgnoreCase)),
            model.DataQualityCandidateList.Count(item =>
                string.Equals(item.Status, CandidateStatuses.Discovered, StringComparison.OrdinalIgnoreCase)),
            viewFamilies,
            situations.Count,
            new MetaDataQualityCorpusInferenceSummary(
                model.CorpusRelationshipList.Count,
                dominantCorpusPatternCount,
                model.CorpusColumnEquivalenceList.Count,
                minoritySummary,
                incompleteSummary,
                extraSummary,
                missingCommonFilterSummary,
                minorityColumnEquivalenceSummary,
                innerAgainstOptionalSummary,
                leftAgainstMandatorySummary,
                impliedJoinFanoutSummary,
                impliedOutputDuplicateSummary),
            pendingSituations);
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

    private static List<MetaDataQualityJoinSituation> BuildJoinSituations(
        MetaDataQualityModel model,
        IReadOnlyDictionary<string, DataQualityCandidate> candidateById,
        IReadOnlyDictionary<string, string> candidateTypes,
        IReadOnlyDictionary<string, JoinPatternOccurrence[]> occurrencesByPatternId,
        IReadOnlyDictionary<string, JoinPatternOccurrenceBaseTable[]> baseTablesByOccurrenceId,
        IReadOnlyDictionary<string, JoinPatternKeyPart[]> keyPartsByPatternId,
        IReadOnlyDictionary<string, string[]> candidateIdsByPattern)
    {
        var result = new List<MetaDataQualityJoinSituation>();
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

            result.Add(new MetaDataQualityJoinSituation(
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

    private static MetaDataQualityCandidateStatusSummary SummarizeCandidateStatuses(
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

        return new MetaDataQualityCandidateStatusSummary(total, promoted);
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
}

public sealed record MetaDataQualityInspectionResult(
    int CandidateCount,
    int PromotedCount,
    int PendingCount,
    IReadOnlyList<MetaDataQualityCheckFamily> CheckFamilies,
    int RelationshipSituationCount,
    MetaDataQualityCorpusInferenceSummary CorpusInference,
    IReadOnlyList<MetaDataQualityJoinSituation> PendingSituations);

public sealed record MetaDataQualityCheckFamily(
    string Label,
    int Count);

public sealed record MetaDataQualityCorpusInferenceSummary(
    int RelationshipCount,
    int DominantRelationshipPatternCount,
    int ColumnEquivalenceEdgeCount,
    MetaDataQualityCandidateStatusSummary MinorityJoinPattern,
    MetaDataQualityCandidateStatusSummary IncompleteComposite,
    MetaDataQualityCandidateStatusSummary SuspiciousExtraPredicate,
    MetaDataQualityCandidateStatusSummary MissingCommonFilter,
    MetaDataQualityCandidateStatusSummary MinorityColumnEquivalence,
    MetaDataQualityCandidateStatusSummary InnerAgainstUsuallyOptional,
    MetaDataQualityCandidateStatusSummary LeftAgainstUsuallyMandatory,
    MetaDataQualityCandidateStatusSummary ImpliedJoinFanout,
    MetaDataQualityCandidateStatusSummary ImpliedOutputDuplicate);

public sealed record MetaDataQualityJoinSituation(
    string JoinDescription,
    string JoinCondition,
    string JoinType,
    string[] ViewLabels,
    int WaitingCount,
    string[] PendingCandidateIds);

public sealed record MetaDataQualityCandidateStatusSummary(
    int Total,
    int Promoted)
{
    public int Waiting => Math.Max(Total - Promoted, 0);
}
