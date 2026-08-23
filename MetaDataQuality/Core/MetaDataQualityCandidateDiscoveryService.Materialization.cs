using MetaDataQuality;
using System.Globalization;
using MetaTransformScript;

namespace MetaDataQuality.Core;

public sealed partial class MetaDataQualityCandidateDiscoveryService
{
    private static MetaDataQualityModel MaterializeDataQualityModel(
        IReadOnlyList<ExtractedScriptEvidence> extracted)
    {
        var model = MetaDataQualityModel.CreateEmpty();
        var joinPatternIds = new HashSet<string>(StringComparer.Ordinal);
        var occurrenceIds = new HashSet<string>(StringComparer.Ordinal);
        var occurrenceBaseTableIds = new HashSet<string>(StringComparer.Ordinal);
        var occurrenceSignalIds = new HashSet<string>(StringComparer.Ordinal);
        var filterPredicateObservationIds = new HashSet<string>(StringComparer.Ordinal);
        var keyPartIds = new HashSet<string>(StringComparer.Ordinal);
        var joinPatternBySignature = new Dictionary<string, JoinPatternAggregate>(StringComparer.Ordinal);

        var joinPatternCounter = 0;
        var occurrenceCounter = 0;
        var occurrenceBaseTableCounter = 0;
        var occurrenceSignalCounter = 0;
        var filterPredicateObservationCounter = 0;
        var keyPartCounter = 0;

        foreach (var scriptEvidence in extracted)
        {
            MaterializeJoinPatternsForScript(
                model,
                scriptEvidence,
                joinPatternBySignature,
                joinPatternIds,
                occurrenceIds,
                occurrenceBaseTableIds,
                occurrenceSignalIds,
                filterPredicateObservationIds,
                keyPartIds,
                ref joinPatternCounter,
                ref occurrenceCounter,
                ref occurrenceBaseTableCounter,
                ref occurrenceSignalCounter,
                ref filterPredicateObservationCounter,
                ref keyPartCounter);
        }

        var candidateIds = new HashSet<string>(StringComparer.Ordinal);
        var candidateJoinPatternLinkIds = new HashSet<string>(StringComparer.Ordinal);
        var candidateByFingerprint = new Dictionary<string, string>(StringComparer.Ordinal);
        var candidateById = new Dictionary<string, DataQualityCandidate>(StringComparer.Ordinal);
        var candidateCounter = 0;

        foreach (var aggregate in joinPatternBySignature.Values
                     .OrderBy(static item => item.Pattern.CanonicalSignature, StringComparer.Ordinal))
        {
            AddCandidatesForPattern(
                model,
                aggregate,
                candidateIds,
                candidateJoinPatternLinkIds,
                candidateByFingerprint,
                candidateById,
                ref candidateCounter);
        }

        return model;
    }

    private static void MaterializeJoinPatternsForScript(
        MetaDataQualityModel model,
        ExtractedScriptEvidence scriptEvidence,
        IDictionary<string, JoinPatternAggregate> joinPatternBySignature,
        ISet<string> seenJoinPatternIds,
        ISet<string> seenOccurrenceIds,
        ISet<string> seenOccurrenceBaseTableIds,
        ISet<string> seenOccurrenceSignalIds,
        ISet<string> seenFilterPredicateObservationIds,
        ISet<string> seenKeyPartIds,
        ref int joinPatternCounter,
        ref int occurrenceCounter,
        ref int occurrenceBaseTableCounter,
        ref int occurrenceSignalCounter,
        ref int filterPredicateObservationCounter,
        ref int keyPartCounter)
    {
        var script = scriptEvidence.TransformScript;
        var scan = scriptEvidence.Scan;
        if (scan.JoinLocations.Count == 0)
        {
            return;
        }

        foreach (var joinEvidence in scan.JoinLocations.OrderBy(static row => row.QualifiedJoinId, StringComparer.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(joinEvidence.QualifiedJoinId))
            {
                continue;
            }

            var canonicalSignature = BuildJoinPatternSignature(joinEvidence);
            if (!joinPatternBySignature.TryGetValue(canonicalSignature, out var aggregate))
            {
                var joinPatternId = BuildUniqueId(
                    seenJoinPatternIds,
                    "JoinPattern",
                    ref joinPatternCounter);
                var joinPattern = new JoinPattern
                {
                    Id = joinPatternId,
                    CanonicalSignature = canonicalSignature,
                    QualifiedJoinType = joinEvidence.QualifiedJoinType,
                    ContainsEqualityPredicate = joinEvidence.ContainsEqualityPredicate ? "true" : "false",
                    EqualityPredicateCount = joinEvidence.EqualityPredicateCount.ToString(),
                };
                model.JoinPatternList.Add(joinPattern);
                aggregate = new JoinPatternAggregate(joinPattern);
                joinPatternBySignature.Add(canonicalSignature, aggregate);

                AddJoinPatternKeyParts(
                    model,
                    joinPattern,
                    joinEvidence.EqualityPredicates,
                    seenKeyPartIds,
                    ref keyPartCounter);
            }

            var occurrenceId = BuildUniqueId(
                seenOccurrenceIds,
                $"{script.Id}.JoinPatternOccurrence.{joinEvidence.QualifiedJoinId}",
                ref occurrenceCounter);
            var occurrence = new JoinPatternOccurrence
            {
                Id = occurrenceId,
                JoinPattern = aggregate.Pattern,
                TransformScriptId = script.Id,
                TransformScriptName = script.Name,
                QueryExpressionId = joinEvidence.QueryExpressionId,
                QuerySpecificationId = joinEvidence.QuerySpecificationId,
                JoinTableReferenceId = joinEvidence.JoinTableReferenceId,
                QualifiedJoinId = joinEvidence.QualifiedJoinId,
                SearchConditionBooleanExpressionId = joinEvidence.SearchConditionBooleanExpressionId,
                FirstTableReferenceId = joinEvidence.FirstTableReferenceId,
                SecondTableReferenceId = joinEvidence.SecondTableReferenceId,
                ScopePath = joinEvidence.ScopePath,
                CteId = joinEvidence.CteId,
                CteName = joinEvidence.CteName,
            };
            model.JoinPatternOccurrenceList.Add(occurrence);

            AddJoinPatternOccurrenceBaseTables(
                model,
                occurrence,
                joinEvidence.BaseTables,
                seenOccurrenceBaseTableIds,
                ref occurrenceBaseTableCounter);

            AddJoinPatternOccurrenceSignals(
                model,
                occurrence,
                joinEvidence,
                scan.HasGroupBy,
                scan.HasDistinct,
                seenOccurrenceSignalIds,
                ref occurrenceSignalCounter);
            AddFilterPredicateObservations(
                model,
                occurrence,
                joinEvidence.FilterPredicates,
                seenFilterPredicateObservationIds,
                ref filterPredicateObservationCounter);

            aggregate.RegisterOccurrence(
                joinEvidence,
                scan.HasGroupBy,
                scan.HasDistinct);
        }
    }

    private static void AddCandidatesForPattern(
        MetaDataQualityModel model,
        JoinPatternAggregate aggregate,
        ISet<string> seenCandidateIds,
        ISet<string> seenCandidateJoinPatternLinkIds,
        IDictionary<string, string> candidateByFingerprint,
        IDictionary<string, DataQualityCandidate> candidateById,
        ref int candidateCounter)
    {
        var joinPatternIds = new[] { aggregate.Pattern.Id };
        if (aggregate.HasExecutableRelationshipKey)
        {
            AddJoinOrphanCandidate(
                model,
                aggregate,
                joinPatternIds,
                seenCandidateIds,
                seenCandidateJoinPatternLinkIds,
                candidateByFingerprint,
                candidateById,
                ref candidateCounter);

            AddJoinMultiplicityExplosionCandidate(
                model,
                aggregate,
                joinPatternIds,
                seenCandidateIds,
                seenCandidateJoinPatternLinkIds,
                candidateByFingerprint,
                candidateById,
                ref candidateCounter);
        }

        if (aggregate.HasExecutableRelationshipKey && aggregate.OuterJoinOccurrenceCount > 0)
        {
            AddOuterJoinNullExpansionCandidate(
                model,
                aggregate,
                joinPatternIds,
                seenCandidateIds,
                seenCandidateJoinPatternLinkIds,
                candidateByFingerprint,
                candidateById,
                ref candidateCounter);
        }

        if (aggregate.HasExecutableRelationshipKey && aggregate.HasOccurrenceWithoutDistinctOrGroupBy)
        {
            AddOutputDuplicateRiskCandidate(
                model,
                aggregate,
                joinPatternIds,
                seenCandidateIds,
                seenCandidateJoinPatternLinkIds,
                candidateByFingerprint,
                candidateById,
                ref candidateCounter);
        }
    }

    private static void AddJoinOrphanCandidate(
        MetaDataQualityModel model,
        JoinPatternAggregate aggregate,
        IReadOnlyList<string> joinPatternIds,
        ISet<string> seenCandidateIds,
        ISet<string> seenCandidateJoinPatternLinkIds,
        IDictionary<string, string> candidateByFingerprint,
        IDictionary<string, DataQualityCandidate> candidateById,
        ref int candidateCounter)
    {
        var upsert = AddCandidate(
            model,
            aggregate,
            CandidateKinds.JoinOrphan,
            "Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.",
            "Relationship direction is inferred from syntax and may require user correction.",
            joinPatternIds,
            seenCandidateIds,
            seenCandidateJoinPatternLinkIds,
            candidateByFingerprint,
            candidateById,
            ref candidateCounter);
        if (!upsert.IsNew)
        {
            return;
        }

        model.JoinOrphanList.Add(new JoinOrphan
        {
            Id = $"{upsert.Candidate.Id}.JoinOrphan",
            DataQualityCandidate = upsert.Candidate,
            EqualityPredicateCount = aggregate.MaxEqualityPredicateCount.ToString(),
        });
    }

    private static void AddOuterJoinNullExpansionCandidate(
        MetaDataQualityModel model,
        JoinPatternAggregate aggregate,
        IReadOnlyList<string> joinPatternIds,
        ISet<string> seenCandidateIds,
        ISet<string> seenCandidateJoinPatternLinkIds,
        IDictionary<string, string> candidateByFingerprint,
        IDictionary<string, DataQualityCandidate> candidateById,
        ref int candidateCounter)
    {
        var upsert = AddCandidate(
            model,
            aggregate,
            CandidateKinds.OuterJoinNullExpansion,
            "Outer join usage was found; null-expansion checks are likely relevant.",
            "Optional-side semantics depend on business intent and may require user adjustment.",
            joinPatternIds,
            seenCandidateIds,
            seenCandidateJoinPatternLinkIds,
            candidateByFingerprint,
            candidateById,
            ref candidateCounter);
        if (!upsert.IsNew)
        {
            return;
        }

        model.OuterJoinNullExpansionList.Add(new OuterJoinNullExpansion
        {
            Id = $"{upsert.Candidate.Id}.OuterJoinNullExpansion",
            DataQualityCandidate = upsert.Candidate,
            OuterJoinCount = aggregate.OuterJoinOccurrenceCount.ToString(),
        });
    }

    private static void AddJoinMultiplicityExplosionCandidate(
        MetaDataQualityModel model,
        JoinPatternAggregate aggregate,
        IReadOnlyList<string> joinPatternIds,
        ISet<string> seenCandidateIds,
        ISet<string> seenCandidateJoinPatternLinkIds,
        IDictionary<string, string> candidateByFingerprint,
        IDictionary<string, DataQualityCandidate> candidateById,
        ref int candidateCounter)
    {
        if (!aggregate.HasOccurrenceWithoutRightDetailProjection)
        {
            return;
        }

        var upsert = AddCandidate(
            model,
            aggregate,
            CandidateKinds.JoinMultiplicityExplosion,
            "Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.",
            "Expected multiplicity (1:1, 1:N, N:1) is not inferable from syntax alone.",
            joinPatternIds,
            seenCandidateIds,
            seenCandidateJoinPatternLinkIds,
            candidateByFingerprint,
            candidateById,
            ref candidateCounter);
        if (!upsert.IsNew)
        {
            return;
        }

        model.JoinMultiplicityExplosionList.Add(new JoinMultiplicityExplosion
        {
            Id = $"{upsert.Candidate.Id}.JoinMultiplicityExplosion",
            DataQualityCandidate = upsert.Candidate,
            EqualityPredicateCount = aggregate.MaxEqualityPredicateCount.ToString(),
        });
    }

    private static void AddOutputDuplicateRiskCandidate(
        MetaDataQualityModel model,
        JoinPatternAggregate aggregate,
        IReadOnlyList<string> joinPatternIds,
        ISet<string> seenCandidateIds,
        ISet<string> seenCandidateJoinPatternLinkIds,
        IDictionary<string, string> candidateByFingerprint,
        IDictionary<string, DataQualityCandidate> candidateById,
        ref int candidateCounter)
    {
        var upsert = AddCandidate(
            model,
            aggregate,
            CandidateKinds.OutputDuplicateRisk,
            "Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.",
            "Candidate key selection requires user confirmation.",
            joinPatternIds,
            seenCandidateIds,
            seenCandidateJoinPatternLinkIds,
            candidateByFingerprint,
            candidateById,
            ref candidateCounter);
        if (!upsert.IsNew)
        {
            return;
        }

        model.OutputDuplicateRiskList.Add(new OutputDuplicateRisk
        {
            Id = $"{upsert.Candidate.Id}.OutputDuplicateRisk",
            DataQualityCandidate = upsert.Candidate,
            QualifiedJoinCount = aggregate.QualifiedJoinOccurrenceCount.ToString(),
            HasDistinct = aggregate.AnyOccurrenceHasDistinct ? "true" : "false",
            HasGroupBy = aggregate.AnyOccurrenceHasGroupBy ? "true" : "false",
        });
    }

    private static CandidateUpsertResult AddCandidate(
        MetaDataQualityModel model,
        JoinPatternAggregate aggregate,
        string kind,
        string rationale,
        string assumptions,
        IReadOnlyList<string> joinPatternIds,
        ISet<string> seenCandidateIds,
        ISet<string> seenCandidateJoinPatternLinkIds,
        IDictionary<string, string> candidateByFingerprint,
        IDictionary<string, DataQualityCandidate> candidateById,
        ref int candidateCounter)
    {
        var fingerprint = BuildCandidateFingerprint(kind, joinPatternIds);
        if (candidateByFingerprint.TryGetValue(fingerprint, out var existingCandidateId)
            && candidateById.TryGetValue(existingCandidateId, out var existingCandidate))
        {
            AddCandidateJoinPatternLinks(
                model,
                existingCandidate,
                joinPatternIds,
                seenCandidateJoinPatternLinkIds);
            return new CandidateUpsertResult(existingCandidate, IsNew: false);
        }

        candidateCounter++;
        var candidateId = BuildUniqueId(
            seenCandidateIds,
            $"{kind}.{aggregate.Pattern.Id}.{candidateCounter}",
            ref candidateCounter);
        var candidate = new DataQualityCandidate
        {
            Id = candidateId,
            Name = $"{kind}:{aggregate.Pattern.Id}",
            Status = CandidateStatuses.Discovered,
            Rationale = rationale,
            Assumptions = assumptions,
            SqlTemplate = BuildSqlTemplate(kind, aggregate.Pattern),
        };
        model.DataQualityCandidateList.Add(candidate);
        candidateByFingerprint[fingerprint] = candidate.Id;
        candidateById[candidate.Id] = candidate;
        AddCandidateJoinPatternLinks(
            model,
            candidate,
            joinPatternIds,
            seenCandidateJoinPatternLinkIds);

        return new CandidateUpsertResult(candidate, IsNew: true);
    }

    private static void AddJoinPatternKeyParts(
        MetaDataQualityModel model,
        JoinPattern joinPattern,
        IReadOnlyList<EqualityPredicateEvidence> equalityPredicates,
        ISet<string> seenKeyPartIds,
        ref int keyPartCounter)
    {
        var dedupe = new HashSet<string>(StringComparer.Ordinal);
        for (var i = 0; i < equalityPredicates.Count; i++)
        {
            var predicate = equalityPredicates[i];
            if (string.IsNullOrWhiteSpace(predicate.BooleanComparisonExpressionId)
                || string.IsNullOrWhiteSpace(predicate.FirstExpressionId)
                || string.IsNullOrWhiteSpace(predicate.SecondExpressionId))
            {
                continue;
            }

            var dedupeKey = string.Join(
                "|",
                predicate.BooleanComparisonExpressionId,
                NormalizeFingerprintPart(predicate.FirstExpressionDisplay),
                NormalizeFingerprintPart(predicate.SecondExpressionDisplay),
                NormalizeFingerprintPart(predicate.FirstExpressionId),
                NormalizeFingerprintPart(predicate.SecondExpressionId),
                (i + 1).ToString());
            if (!dedupe.Add(dedupeKey))
            {
                continue;
            }

            var keyPartId = BuildUniqueId(
                seenKeyPartIds,
                $"{joinPattern.Id}.KeyPart.{i + 1}",
                ref keyPartCounter);
            var keyPart = new JoinPatternKeyPart
            {
                Id = keyPartId,
                JoinPattern = joinPattern,
                Ordinal = (i + 1).ToString(),
                BooleanComparisonExpressionId = predicate.BooleanComparisonExpressionId,
                FirstExpressionId = predicate.FirstExpressionId,
                SecondExpressionId = predicate.SecondExpressionId,
                FirstExpressionDisplay = predicate.FirstExpressionDisplay,
                SecondExpressionDisplay = predicate.SecondExpressionDisplay,
                FirstJoinInputColumnName = predicate.FirstJoinInputColumnName,
                SecondJoinInputColumnName = predicate.SecondJoinInputColumnName,
                FirstJoinInputObjectName = predicate.FirstJoinInputObjectName,
                SecondJoinInputObjectName = predicate.SecondJoinInputObjectName,
            };
            model.JoinPatternKeyPartList.Add(keyPart);
            AddJoinInputObjectIdentifierParts(
                model,
                keyPart,
                "First",
                predicate.FirstJoinInputObjectIdentifierParts);
            AddJoinInputObjectIdentifierParts(
                model,
                keyPart,
                "Second",
                predicate.SecondJoinInputObjectIdentifierParts);
        }
    }

    private static void AddJoinInputObjectIdentifierParts(
        MetaDataQualityModel model,
        JoinPatternKeyPart keyPart,
        string inputSide,
        IReadOnlyList<string> values)
    {
        for (var index = 0; index < values.Count; index++)
        {
            model.JoinPatternKeyPartInputObjectIdentifierPartList.Add(
                new JoinPatternKeyPartInputObjectIdentifierPart
                {
                    Id = $"{keyPart.Id}.{inputSide}ObjectPart.{index + 1}",
                    JoinPatternKeyPart = keyPart,
                    InputSide = inputSide,
                    Ordinal = index.ToString(CultureInfo.InvariantCulture),
                    Value = values[index],
                });
        }
    }

    private static void AddJoinPatternOccurrenceBaseTables(
        MetaDataQualityModel model,
        JoinPatternOccurrence occurrence,
        IReadOnlyList<BaseTableEvidence> baseTables,
        ISet<string> seenOccurrenceBaseTableIds,
        ref int occurrenceBaseTableCounter)
    {
        var dedupe = new HashSet<string>(StringComparer.Ordinal);
        foreach (var baseTable in baseTables)
        {
            var dedupeKey = string.Join(
                "|",
                baseTable.JoinInputTableReferenceId,
                baseTable.BaseTableReferenceId,
                baseTable.BaseNamedTableReferenceId,
                baseTable.BaseSchemaObjectNameId,
                baseTable.BaseObjectName,
                baseTable.ResolutionPath);
            if (!dedupe.Add(dedupeKey))
            {
                continue;
            }

            var baseTableId = BuildUniqueId(
                seenOccurrenceBaseTableIds,
                $"{occurrence.Id}.BaseTable.{baseTable.JoinInputTableReferenceId}.{baseTable.BaseTableReferenceId}",
                ref occurrenceBaseTableCounter);
            model.JoinPatternOccurrenceBaseTableList.Add(new JoinPatternOccurrenceBaseTable
            {
                Id = baseTableId,
                JoinPatternOccurrence = occurrence,
                JoinInputTableReferenceId = baseTable.JoinInputTableReferenceId,
                BaseTableReferenceId = baseTable.BaseTableReferenceId,
                BaseNamedTableReferenceId = baseTable.BaseNamedTableReferenceId,
                BaseSchemaObjectNameId = baseTable.BaseSchemaObjectNameId,
                BaseObjectName = baseTable.BaseObjectName,
                ResolutionDepth = baseTable.ResolutionDepth.ToString(),
                ResolutionPath = baseTable.ResolutionPath,
                ResolvedInCteId = baseTable.ResolvedInCteId,
                ResolvedInCteName = baseTable.ResolvedInCteName,
            });
        }
    }

    private static void AddJoinPatternOccurrenceSignals(
        MetaDataQualityModel model,
        JoinPatternOccurrence occurrence,
        JoinLocationEvidence joinEvidence,
        bool hasGroupBy,
        bool hasDistinct,
        ISet<string> seenOccurrenceSignalIds,
        ref int occurrenceSignalCounter)
    {
        var riskSignals = ResolveUnsuppressedRiskSignals(joinEvidence, hasGroupBy, hasDistinct);
        if (!riskSignals.HasAny)
        {
            return;
        }

        if (riskSignals.JoinMultiplicityExplosion)
        {
            AddJoinPatternOccurrenceSignal(
                model,
                occurrence,
                CandidateKinds.JoinMultiplicityExplosion,
                "Unsuppressed transform-scope row-multiplication risk signal.",
                seenOccurrenceSignalIds,
                ref occurrenceSignalCounter);
        }

        if (riskSignals.OutputDuplicateRisk)
        {
            AddJoinPatternOccurrenceSignal(
                model,
                occurrence,
                CandidateKinds.OutputDuplicateRisk,
                "Unsuppressed transform-scope duplicate-output risk signal.",
                seenOccurrenceSignalIds,
                ref occurrenceSignalCounter);
        }
    }

    private static void AddJoinPatternOccurrenceSignal(
        MetaDataQualityModel model,
        JoinPatternOccurrence occurrence,
        string signalKind,
        string explanation,
        ISet<string> seenOccurrenceSignalIds,
        ref int occurrenceSignalCounter)
    {
        var signalId = BuildUniqueId(
            seenOccurrenceSignalIds,
            $"{occurrence.Id}.Signal.{signalKind}",
            ref occurrenceSignalCounter);
        model.JoinPatternOccurrenceSignalList.Add(new JoinPatternOccurrenceSignal
        {
            Id = signalId,
            JoinPatternOccurrence = occurrence,
            SignalKind = signalKind,
            SourceCandidateKind = signalKind,
            Explanation = explanation,
        });
    }

    private static void AddFilterPredicateObservations(
        MetaDataQualityModel model,
        JoinPatternOccurrence occurrence,
        IReadOnlyList<FilterPredicateEvidence> filterPredicates,
        ISet<string> seenFilterPredicateObservationIds,
        ref int filterPredicateObservationCounter)
    {
        if (filterPredicates.Count == 0)
        {
            return;
        }

        var dedupe = new HashSet<string>(StringComparer.Ordinal);
        foreach (var predicate in filterPredicates
                     .Where(static row => !string.IsNullOrWhiteSpace(row.BaseObjectName)
                                          && !string.IsNullOrWhiteSpace(row.PredicateSignature)
                                          && !string.IsNullOrWhiteSpace(row.PredicateDisplay))
                     .OrderBy(static row => row.BaseObjectName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static row => row.PredicateSignature, StringComparer.Ordinal))
        {
            var dedupeKey = $"{predicate.BaseObjectName}|{predicate.PredicateSignature}";
            if (!dedupe.Add(dedupeKey))
            {
                continue;
            }

            var rowId = BuildUniqueId(
                seenFilterPredicateObservationIds,
                $"{occurrence.Id}.Filter.{CorpusInferenceNormalization.NormalizeSignaturePart(predicate.BaseObjectName)}.{CorpusInferenceNormalization.NormalizeSignaturePart(predicate.PredicateSignature)}",
                ref filterPredicateObservationCounter);
            model.FilterPredicateObservationList.Add(new FilterPredicateObservation
            {
                Id = rowId,
                JoinPatternOccurrence = occurrence,
                BaseObjectName = predicate.BaseObjectName,
                PredicateSignature = predicate.PredicateSignature,
                PredicateDisplay = predicate.PredicateDisplay,
            });
        }
    }

    private static OccurrenceRiskSignals ResolveUnsuppressedRiskSignals(
        JoinLocationEvidence evidence,
        bool hasGroupBy,
        bool hasDistinct)
    {
        var unsuppressedJoinMultiplicity =
            evidence.ContainsEqualityPredicate
            && !evidence.ProjectsRightDetailColumn;
        var unsuppressedOutputDuplicate =
            unsuppressedJoinMultiplicity
            && !hasGroupBy
            && !hasDistinct;

        return new OccurrenceRiskSignals(
            unsuppressedJoinMultiplicity,
            unsuppressedOutputDuplicate);
    }

    private static void AddCandidateJoinPatternLinks(
        MetaDataQualityModel model,
        DataQualityCandidate candidate,
        IReadOnlyList<string> joinPatternIds,
        ISet<string> seenCandidateJoinPatternLinkIds)
    {
        var existingPairs = model.DataQualityCandidateJoinPatternLinkList
            .Select(static row => $"{row.DataQualityCandidate.Id}|{row.JoinPattern.Id}")
            .ToHashSet(StringComparer.Ordinal);

        var linkCounter = 0;
        foreach (var joinPatternId in joinPatternIds
                     .Where(static value => !string.IsNullOrWhiteSpace(value))
                     .Distinct(StringComparer.Ordinal))
        {
            var pairKey = $"{candidate.Id}|{joinPatternId}";
            if (existingPairs.Contains(pairKey))
            {
                continue;
            }

            var joinPattern = model.JoinPatternList.FirstOrDefault(row => string.Equals(row.Id, joinPatternId, StringComparison.Ordinal))
                ?? throw new InvalidOperationException($"Join pattern '{joinPatternId}' was not found while creating candidate links.");
            var linkId = BuildUniqueId(
                seenCandidateJoinPatternLinkIds,
                $"{candidate.Id}.JoinPatternLink.{joinPatternId}",
                ref linkCounter);
            model.DataQualityCandidateJoinPatternLinkList.Add(new DataQualityCandidateJoinPatternLink
            {
                Id = linkId,
                DataQualityCandidate = candidate,
                JoinPattern = joinPattern,
            });
            existingPairs.Add(pairKey);
        }
    }

    private static string BuildCandidateFingerprint(
        string kind,
        IReadOnlyList<string> joinPatternIds)
    {
        var patternPart = joinPatternIds
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static value => value, StringComparer.Ordinal)
            .ToArray();
        var anchors = patternPart.Length == 0
            ? "(none)"
            : string.Join("||", patternPart);
        return $"{kind}|{anchors}";
    }

    private static string BuildSqlTemplate(string kind, JoinPattern pattern)
    {
        return $$"""
/*
CandidateType: {{kind}}
JoinPatternId: {{pattern.Id}}
JoinPatternSignature: {{pattern.CanonicalSignature}}
*/
SELECT
    CAST(NULL AS bigint) AS SuspectRowCount
WHERE 1 = 0;
""";
    }

    private static string BuildJoinPatternSignature(JoinLocationEvidence evidence)
    {
        var joinType = NormalizeFingerprintPart(evidence.QualifiedJoinType);
        var leftTables = BuildJoinSideTableSignature(evidence.BaseTables, evidence.FirstTableReferenceId);
        var rightTables = BuildJoinSideTableSignature(evidence.BaseTables, evidence.SecondTableReferenceId);
        var keyPartSignature = BuildKeyPartSignature(evidence.EqualityPredicates);
        var equalityPart = evidence.EqualityPredicateCount.ToString();
        var hasEqualityPart = evidence.ContainsEqualityPredicate ? "1" : "0";
        return $"type={joinType};left={leftTables};right={rightTables};eq={equalityPart};hasEq={hasEqualityPart};keys={keyPartSignature}";
    }

    private static string BuildJoinSideTableSignature(
        IReadOnlyList<BaseTableEvidence> baseTables,
        string joinInputTableReferenceId)
    {
        var tables = baseTables
            .Where(row => string.Equals(row.JoinInputTableReferenceId, joinInputTableReferenceId, StringComparison.Ordinal))
            .Select(row => NormalizeFingerprintPart(row.BaseObjectName))
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static value => value, StringComparer.Ordinal)
            .ToArray();
        return tables.Length == 0
            ? "(none)"
            : string.Join(",", tables);
    }

    private static string BuildKeyPartSignature(IReadOnlyList<EqualityPredicateEvidence> predicates)
    {
        var parts = predicates
            .Select(static row =>
            {
                var left = string.IsNullOrWhiteSpace(row.FirstExpressionDisplay)
                    ? row.FirstExpressionId
                    : row.FirstExpressionDisplay;
                var right = string.IsNullOrWhiteSpace(row.SecondExpressionDisplay)
                    ? row.SecondExpressionId
                    : row.SecondExpressionDisplay;
                return BuildCanonicalEqualityPart(left, right);
            })
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static value => value, StringComparer.Ordinal)
            .ToArray();
        return parts.Length == 0
            ? "(none)"
            : string.Join("&", parts);
    }

    private static string BuildCanonicalEqualityPart(string first, string second)
    {
        var left = NormalizeFingerprintPart(first);
        var right = NormalizeFingerprintPart(second);
        if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
        {
            return string.Empty;
        }

        return string.Compare(left, right, StringComparison.Ordinal) <= 0
            ? $"{left}={right}"
            : $"{right}={left}";
    }

    private static string NormalizeFingerprintPart(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim().ToLowerInvariant();

    private static string BuildUniqueId(
        ISet<string> seenIds,
        string baseId,
        ref int counter)
    {
        if (seenIds.Add(baseId))
        {
            return baseId;
        }

        while (true)
        {
            counter++;
            var withSuffix = $"{baseId}.{counter}";
            if (seenIds.Add(withSuffix))
            {
                return withSuffix;
            }
        }
    }

    private static bool IsOuterJoin(string qualifiedJoinType)
    {
        return string.Equals(qualifiedJoinType, "LeftOuter", StringComparison.OrdinalIgnoreCase)
               || string.Equals(qualifiedJoinType, "RightOuter", StringComparison.OrdinalIgnoreCase)
               || string.Equals(qualifiedJoinType, "FullOuter", StringComparison.OrdinalIgnoreCase);
    }

    private sealed class JoinPatternAggregate
    {
        public JoinPatternAggregate(JoinPattern pattern)
        {
            Pattern = pattern;
            ContainsEqualityPredicate = string.Equals(
                pattern.ContainsEqualityPredicate,
                "true",
                StringComparison.OrdinalIgnoreCase);
            MaxEqualityPredicateCount = int.TryParse(pattern.EqualityPredicateCount, out var parsed)
                ? parsed
                : 0;
        }

        public JoinPattern Pattern { get; }

        public bool ContainsEqualityPredicate { get; }

        public int QualifiedJoinOccurrenceCount { get; private set; }

        public int OuterJoinOccurrenceCount { get; private set; }

        public int MaxEqualityPredicateCount { get; private set; }

        public bool AnyOccurrenceHasGroupBy { get; private set; }

        public bool AnyOccurrenceHasDistinct { get; private set; }

        public bool HasOccurrenceWithoutDistinctOrGroupBy { get; private set; }

        public bool HasOccurrenceWithoutRightDetailProjection { get; private set; }

        public bool HasExecutableRelationshipKey { get; private set; }

        public void RegisterOccurrence(
            JoinLocationEvidence evidence,
            bool hasGroupBy,
            bool hasDistinct)
        {
            QualifiedJoinOccurrenceCount++;
            if (IsOuterJoin(evidence.QualifiedJoinType))
            {
                OuterJoinOccurrenceCount++;
            }

            if (evidence.EqualityPredicateCount > MaxEqualityPredicateCount)
            {
                MaxEqualityPredicateCount = evidence.EqualityPredicateCount;
            }

            if (evidence.EqualityPredicates.Any(static predicate =>
                    !string.IsNullOrWhiteSpace(predicate.FirstJoinInputObjectName)
                    && !string.IsNullOrWhiteSpace(predicate.FirstJoinInputColumnName)
                    && !string.IsNullOrWhiteSpace(predicate.SecondJoinInputObjectName)
                    && !string.IsNullOrWhiteSpace(predicate.SecondJoinInputColumnName)))
            {
                HasExecutableRelationshipKey = true;
            }

            if (hasGroupBy)
            {
                AnyOccurrenceHasGroupBy = true;
            }

            if (hasDistinct)
            {
                AnyOccurrenceHasDistinct = true;
            }

            var riskSignals = ResolveUnsuppressedRiskSignals(evidence, hasGroupBy, hasDistinct);
            if (riskSignals.JoinMultiplicityExplosion)
            {
                HasOccurrenceWithoutRightDetailProjection = true;
            }

            if (riskSignals.OutputDuplicateRisk)
            {
                HasOccurrenceWithoutDistinctOrGroupBy = true;
            }
        }
    }

    private readonly record struct OccurrenceRiskSignals(
        bool JoinMultiplicityExplosion,
        bool OutputDuplicateRisk)
    {
        public bool HasAny => JoinMultiplicityExplosion || OutputDuplicateRisk;
    }

    private readonly record struct CandidateUpsertResult(
        DataQualityCandidate Candidate,
        bool IsNew);
}
