using System.Globalization;
using MetaDataQuality;

namespace MetaDataQuality.Core;

internal static class CorpusCandidateEmitter
{
    public static void Emit(
        MetaDataQualityModel model,
        IReadOnlyList<MaterializedRelationship> materializedRelationships,
        IReadOnlyList<MaterializedColumnEquivalence> materializedColumnEquivalences,
        CorpusInferenceOptions effectiveOptions,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var joinFanoutSignalOccurrenceIds = BuildSignalOccurrenceIds(
            model,
            CandidateKinds.JoinMultiplicityExplosion);
        var outputDuplicateSignalOccurrenceIds = BuildSignalOccurrenceIds(
            model,
            CandidateKinds.OutputDuplicateRisk);
        var filterPredicatesByOccurrenceId = BuildFilterPredicatesByOccurrenceId(model);

        foreach (var relationship in materializedRelationships)
        {
            if (relationship.Patterns.Count == 0)
            {
                continue;
            }

            var dominant = relationship.Patterns
                .OrderByDescending(static item => item.Aggregate.OccurrenceIds.Count)
                .ThenByDescending(static item => item.Aggregate.TransformScriptIds.Count)
                .ThenBy(static item => item.Row.CanonicalKeyPartSetSignature, StringComparer.Ordinal)
                .First();

            var relationOccurrenceCount = relationship.Aggregate.OccurrenceIds.Count;
            var relationTransformCount = relationship.Aggregate.TransformScriptIds.Count;
            var dominantOccurrenceCount = dominant.Aggregate.OccurrenceIds.Count;
            var dominantRatio = relationOccurrenceCount == 0
                ? 0d
                : dominantOccurrenceCount / (double)relationOccurrenceCount;
            var isDominantQualified =
                relationOccurrenceCount >= effectiveOptions.MinTablePairOccurrenceCount
                && relationTransformCount >= effectiveOptions.MinTablePairTransformCount
                && dominantOccurrenceCount >= effectiveOptions.DominantPatternMinOccurrenceCount
                && dominantRatio >= effectiveOptions.DominantPatternMinRatio;

            if (isDominantQualified)
            {
                dominant.Row.IsDominant = "true";
            }

            if (isDominantQualified)
            {
                foreach (var outlier in relationship.Patterns
                             .Where(item => !ReferenceEquals(item, dominant))
                             .OrderBy(static item => item.Row.CanonicalKeyPartSetSignature, StringComparer.Ordinal))
                {
                    var outlierOccurrenceCount = outlier.Aggregate.OccurrenceIds.Count;
                    var outlierRatio = relationOccurrenceCount == 0
                        ? 0d
                        : outlierOccurrenceCount / (double)relationOccurrenceCount;
                    if (outlierOccurrenceCount == 0
                        || outlierRatio > effectiveOptions.MinorityPatternMaxRatio)
                    {
                        continue;
                    }

                    AddMinorityCandidate(
                        model,
                        relationship.Row,
                        dominant,
                        outlier,
                        dominantRatio,
                        outlierRatio,
                        effectiveOptions,
                        usedIds,
                        countersByPrefix);

                    if (dominant.Aggregate.KeyParts.Count >= 2
                        && CorpusInferenceNormalization.IsStrictSubset(outlier.Aggregate.KeyParts, dominant.Aggregate.KeyParts))
                    {
                        AddIncompleteCompositeCandidate(
                            model,
                            relationship.Row,
                            dominant,
                            outlier,
                            dominantRatio,
                            outlierRatio,
                            effectiveOptions,
                            usedIds,
                            countersByPrefix);
                    }

                    if (CorpusInferenceNormalization.IsStrictSubset(dominant.Aggregate.KeyParts, outlier.Aggregate.KeyParts))
                    {
                        AddSuspiciousExtraCandidate(
                            model,
                            relationship.Row,
                            dominant,
                            outlier,
                            dominantRatio,
                            outlierRatio,
                            effectiveOptions,
                            usedIds,
                            countersByPrefix);
                    }

                    AddMissingCommonFilterCandidates(
                        model,
                        relationship.Row,
                        dominant,
                        outlier,
                        filterPredicatesByOccurrenceId,
                        effectiveOptions,
                        usedIds,
                        countersByPrefix);
                }

                var canInferImpliedRelationship =
                    relationOccurrenceCount >= effectiveOptions.MinRelationshipOccurrenceCount
                    && relationTransformCount >= effectiveOptions.MinRelationshipTransformCount
                    && dominantRatio >= effectiveOptions.MinConsensusRatio
                    && dominantOccurrenceCount >= effectiveOptions.MinDominantPatternOccurrenceCount;
                if (canInferImpliedRelationship)
                {
                    AddImpliedForeignKeyCandidate(
                        model,
                        relationship.Row,
                        dominant,
                        dominantRatio,
                        effectiveOptions,
                        usedIds,
                        countersByPrefix);

                    var directionCounts = CountDirectionalEvidence(dominant.Aggregate.Observations);
                    var lookupConsistencyRatio = directionCounts.TotalCount == 0
                        ? 0d
                        : Math.Max(directionCounts.SideAToSideBCount, directionCounts.SideBToSideACount) / (double)directionCounts.TotalCount;
                    var canInferUniqueKey =
                        dominantOccurrenceCount >= effectiveOptions.MinLookupSideOccurrenceCount
                        && dominant.Aggregate.TransformScriptIds.Count >= effectiveOptions.MinLookupSideTransformCount
                        && lookupConsistencyRatio >= effectiveOptions.MinLookupSideConsistencyRatio
                        && dominantOccurrenceCount >= effectiveOptions.MinKeyPartOccurrenceCount;
                    if (canInferUniqueKey)
                    {
                        AddImpliedUniqueKeyCandidate(
                            model,
                            relationship.Row,
                            dominant,
                            dominantRatio,
                            1d - lookupConsistencyRatio,
                            effectiveOptions,
                            usedIds,
                            countersByPrefix);
                    }

                    AddImpliedJoinFanoutRiskCandidateIfQualified(
                        model,
                        relationship.Row,
                        dominant,
                        dominantRatio,
                        joinFanoutSignalOccurrenceIds,
                        effectiveOptions,
                        usedIds,
                        countersByPrefix);

                    AddImpliedOutputDuplicateRiskCandidateIfQualified(
                        model,
                        relationship.Row,
                        dominant,
                        dominantRatio,
                        outputDuplicateSignalOccurrenceIds,
                        effectiveOptions,
                        usedIds,
                        countersByPrefix);
                }
            }

            AddOptionalityDriftCandidatesForRelationship(
                model,
                relationship,
                effectiveOptions,
                usedIds,
                countersByPrefix);
        }

        AddColumnEquivalenceCandidates(
            model,
            materializedRelationships,
            materializedColumnEquivalences,
            effectiveOptions,
            usedIds,
            countersByPrefix);
    }

    private static DirectionCounts CountDirectionalEvidence(IReadOnlyList<OccurrenceObservation> observations)
    {
        var sideAToSideBCount = 0;
        var sideBToSideACount = 0;
        foreach (var observation in observations)
        {
            if (observation.LeftObjectName.Equals(observation.CanonicalSideAObjectName, StringComparison.OrdinalIgnoreCase)
                && observation.RightObjectName.Equals(observation.CanonicalSideBObjectName, StringComparison.OrdinalIgnoreCase))
            {
                sideAToSideBCount++;
                continue;
            }

            if (observation.LeftObjectName.Equals(observation.CanonicalSideBObjectName, StringComparison.OrdinalIgnoreCase)
                && observation.RightObjectName.Equals(observation.CanonicalSideAObjectName, StringComparison.OrdinalIgnoreCase))
            {
                sideBToSideACount++;
            }
        }

        return new DirectionCounts(sideAToSideBCount, sideBToSideACount);
    }

    private static HashSet<string> BuildSignalOccurrenceIds(
        MetaDataQualityModel model,
        string signalKind)
    {
        return model.JoinPatternOccurrenceSignalList
            .Where(row => string.Equals(row.SignalKind, signalKind, StringComparison.Ordinal)
                          && !string.IsNullOrWhiteSpace(row.JoinPatternOccurrence.Id))
            .Select(static row => row.JoinPatternOccurrence.Id)
            .ToHashSet(StringComparer.Ordinal);
    }

    private static IReadOnlyDictionary<string, FilterPredicateObservation[]> BuildFilterPredicatesByOccurrenceId(
        MetaDataQualityModel model)
    {
        return model.FilterPredicateObservationList
            .Where(row => !string.IsNullOrWhiteSpace(row.JoinPatternOccurrence.Id)
                          && !string.IsNullOrWhiteSpace(row.BaseObjectName)
                          && !string.IsNullOrWhiteSpace(row.PredicateSignature))
            .GroupBy(static row => row.JoinPatternOccurrence.Id, StringComparer.Ordinal)
            .ToDictionary(
                static group => group.Key,
                static group => group
                    .OrderBy(static row => row.BaseObjectName, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(static row => row.PredicateSignature, StringComparer.Ordinal)
                    .ThenBy(static row => row.Id, StringComparer.Ordinal)
                    .ToArray(),
                StringComparer.Ordinal);
    }

    private static SignalCounts CountSignalEvidence(
        IReadOnlyList<OccurrenceObservation> observations,
        IReadOnlySet<string> signaledOccurrenceIds)
    {
        var occurrenceIds = new HashSet<string>(StringComparer.Ordinal);
        var transformScriptIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var observation in observations)
        {
            if (!signaledOccurrenceIds.Contains(observation.OccurrenceId))
            {
                continue;
            }

            occurrenceIds.Add(observation.OccurrenceId);
            if (!string.IsNullOrWhiteSpace(observation.TransformScriptId))
            {
                transformScriptIds.Add(observation.TransformScriptId);
            }
        }

        return new SignalCounts(
            occurrenceIds.Count,
            transformScriptIds.Count,
            occurrenceIds.OrderBy(static value => value, StringComparer.Ordinal).ToArray());
    }

    private static void AddImpliedJoinFanoutRiskCandidateIfQualified(
        MetaDataQualityModel model,
        CorpusRelationship relationship,
        MaterializedPattern dominant,
        double consensusRatio,
        IReadOnlySet<string> joinFanoutSignalOccurrenceIds,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        if (joinFanoutSignalOccurrenceIds.Count == 0)
        {
            return;
        }

        var signalCounts = CountSignalEvidence(dominant.Aggregate.Observations, joinFanoutSignalOccurrenceIds);
        if (signalCounts.OccurrenceCount < options.MinFanoutSignalOccurrenceCount
            || signalCounts.TransformCount < options.MinFanoutSignalTransformCount)
        {
            return;
        }

        var dominantOccurrenceCount = dominant.Aggregate.OccurrenceIds.Count;
        var signalRatio = dominantOccurrenceCount == 0
            ? 0d
            : signalCounts.OccurrenceCount / (double)dominantOccurrenceCount;
        if (signalRatio < options.MinFanoutSignalRatio)
        {
            return;
        }

        var candidate = AddCandidate(
            model,
            CandidateKinds.ImpliedJoinFanoutRisk,
            $"ImpliedJoinFanoutRisk:{relationship.Id}:{dominant.Row.Id}",
            "High-consensus relationship usage repeatedly carries row-multiplication risk signals.",
            "Fanout risk can be intentional for detail-grain outputs and should be validated by data owners.",
            usedIds,
            countersByPrefix);

        model.ImpliedJoinFanoutRiskList.Add(new ImpliedJoinFanoutRisk
        {
            Id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "ImpliedJoinFanoutRisk"),
            DataQualityCandidate = candidate,
            DominantPattern = dominant.Row,
        });

        AddEvidence(
            model,
            candidate,
            relationship,
            dominant.Row,
            "ImpliedJoinFanoutRiskConsensus",
            signalCounts.OccurrenceCount,
            signalCounts.TransformCount,
            consensusRatio,
            1d - signalRatio,
            $"Dominant relationship pattern '{dominant.Row.CanonicalKeyPartSetSignature}' repeatedly maps to row-multiplication risk signals in transform-level analysis. Signaled occurrences: {FormatOccurrenceIdPreview(signalCounts.OccurrenceIds)}.",
            ResolveDistinctSourceObjectCount(relationship),
            1,
            options,
            usedIds,
            countersByPrefix);
    }

    private static void AddImpliedOutputDuplicateRiskCandidateIfQualified(
        MetaDataQualityModel model,
        CorpusRelationship relationship,
        MaterializedPattern dominant,
        double consensusRatio,
        IReadOnlySet<string> outputDuplicateSignalOccurrenceIds,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        if (outputDuplicateSignalOccurrenceIds.Count == 0)
        {
            return;
        }

        var signalCounts = CountSignalEvidence(dominant.Aggregate.Observations, outputDuplicateSignalOccurrenceIds);
        if (signalCounts.OccurrenceCount < options.MinOutputDuplicateSignalOccurrenceCount
            || signalCounts.TransformCount < options.MinOutputDuplicateSignalTransformCount)
        {
            return;
        }

        var dominantOccurrenceCount = dominant.Aggregate.OccurrenceIds.Count;
        var signalRatio = dominantOccurrenceCount == 0
            ? 0d
            : signalCounts.OccurrenceCount / (double)dominantOccurrenceCount;
        if (signalRatio < options.MinOutputDuplicateSignalRatio)
        {
            return;
        }

        var candidate = AddCandidate(
            model,
            CandidateKinds.ImpliedOutputDuplicateRisk,
            $"ImpliedOutputDuplicateRisk:{relationship.Id}:{dominant.Row.Id}",
            "High-consensus relationship usage repeatedly carries duplicate-output risk signals.",
            "Duplicate-output risk can be intentional for detail-grain outputs and should be validated by data owners.",
            usedIds,
            countersByPrefix);

        model.ImpliedOutputDuplicateRiskList.Add(new ImpliedOutputDuplicateRisk
        {
            Id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "ImpliedOutputDuplicateRisk"),
            DataQualityCandidate = candidate,
            DominantPattern = dominant.Row,
        });

        AddEvidence(
            model,
            candidate,
            relationship,
            dominant.Row,
            "ImpliedOutputDuplicateRiskConsensus",
            signalCounts.OccurrenceCount,
            signalCounts.TransformCount,
            consensusRatio,
            1d - signalRatio,
            $"Dominant relationship pattern '{dominant.Row.CanonicalKeyPartSetSignature}' repeatedly maps to duplicate-output risk signals in transform-level analysis. Signaled occurrences: {FormatOccurrenceIdPreview(signalCounts.OccurrenceIds)}.",
            ResolveDistinctSourceObjectCount(relationship),
            1,
            options,
            usedIds,
            countersByPrefix);
    }

    private static void AddOptionalityDriftCandidatesForRelationship(
        MetaDataQualityModel model,
        MaterializedRelationship relationship,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        foreach (var pattern in relationship.Patterns
                     .OrderBy(static item => item.Row.CanonicalKeyPartSetSignature, StringComparer.Ordinal))
        {
            var optionality = CountOptionalityEvidence(pattern.Aggregate.Observations);
            if (pattern.Aggregate.OccurrenceIds.Count < options.MinPatternOccurrenceCount
                || pattern.Aggregate.TransformScriptIds.Count < options.MinPatternTransformCount)
            {
                continue;
            }

            var dominant = ResolveDominantOptionality(optionality);
            if (dominant.Kind == JoinOptionality.Other
                || dominant.OccurrenceCount < options.DominantOptionalityMinOccurrenceCount)
            {
                continue;
            }

            var dominantRatio = optionality.TotalOccurrenceCount == 0
                ? 0d
                : dominant.OccurrenceCount / (double)optionality.TotalOccurrenceCount;
            if (dominantRatio < options.DominantOptionalityMinRatio)
            {
                continue;
            }

            if (dominant.Kind is JoinOptionality.LeftOuterSideAOptional or JoinOptionality.LeftOuterSideBOptional)
            {
                var outlier = optionality.Inner;
                var outlierRatio = optionality.TotalOccurrenceCount == 0
                    ? 0d
                    : outlier.OccurrenceCount / (double)optionality.TotalOccurrenceCount;
                if (outlier.OccurrenceCount >= options.OutlierOptionalityMinOccurrenceCount
                    && outlierRatio <= options.OutlierOptionalityMaxRatio)
                {
                    AddInnerJoinAgainstUsuallyOptionalRelationshipCandidate(
                        model,
                        relationship.Row,
                        pattern,
                        dominantRatio,
                        outlierRatio,
                        outlier,
                        dominant.Kind,
                        options,
                        usedIds,
                        countersByPrefix);
                }

                continue;
            }

            if (dominant.Kind == JoinOptionality.Inner)
            {
                var outlier = ResolveInnerDominantLeftOutlier(optionality);
                if (outlier != null)
                {
                    var outlierRatio = optionality.TotalOccurrenceCount == 0
                        ? 0d
                        : outlier.OccurrenceCount / (double)optionality.TotalOccurrenceCount;
                    if (outlier.OccurrenceCount >= options.OutlierOptionalityMinOccurrenceCount
                        && outlierRatio <= options.OutlierOptionalityMaxRatio)
                    {
                        AddLeftJoinAgainstUsuallyMandatoryRelationshipCandidate(
                            model,
                            relationship.Row,
                            pattern,
                            dominantRatio,
                            outlierRatio,
                            outlier,
                            options,
                            usedIds,
                            countersByPrefix);
                    }
                }
            }
        }
    }

    private static OptionalityCounts CountOptionalityEvidence(IReadOnlyList<OccurrenceObservation> observations)
    {
        var inner = new OptionalityBucket(JoinOptionality.Inner);
        var leftOuterSideAOptional = new OptionalityBucket(JoinOptionality.LeftOuterSideAOptional);
        var leftOuterSideBOptional = new OptionalityBucket(JoinOptionality.LeftOuterSideBOptional);
        var other = new OptionalityBucket(JoinOptionality.Other);
        foreach (var observation in observations)
        {
            var optionality = ClassifyOptionality(observation);
            switch (optionality)
            {
                case JoinOptionality.Inner:
                    inner.Register(observation.TransformScriptId);
                    break;
                case JoinOptionality.LeftOuterSideAOptional:
                    leftOuterSideAOptional.Register(observation.TransformScriptId);
                    break;
                case JoinOptionality.LeftOuterSideBOptional:
                    leftOuterSideBOptional.Register(observation.TransformScriptId);
                    break;
                default:
                    other.Register(observation.TransformScriptId);
                    break;
            }
        }

        return new OptionalityCounts(inner, leftOuterSideAOptional, leftOuterSideBOptional, other);
    }

    private static OptionalityBucket ResolveDominantOptionality(OptionalityCounts counts)
    {
        return new[] { counts.Inner, counts.LeftOuterSideAOptional, counts.LeftOuterSideBOptional, counts.Other }
            .OrderByDescending(static item => item.OccurrenceCount)
            .ThenByDescending(static item => item.TransformCount)
            .ThenBy(static item => item.Kind, StringComparer.Ordinal)
            .First();
    }

    private static string ClassifyOptionality(OccurrenceObservation observation)
    {
        var qualifiedJoinType = observation.QualifiedJoinType;
        if (string.Equals(qualifiedJoinType, "Inner", StringComparison.OrdinalIgnoreCase))
        {
            return JoinOptionality.Inner;
        }

        if (string.Equals(qualifiedJoinType, "LeftOuter", StringComparison.OrdinalIgnoreCase))
        {
            if (observation.CanonicalSideAObjectName.Equals(observation.CanonicalSideBObjectName, StringComparison.OrdinalIgnoreCase))
            {
                return JoinOptionality.Other;
            }

            if (observation.LeftObjectName.Equals(observation.CanonicalSideAObjectName, StringComparison.OrdinalIgnoreCase)
                && observation.RightObjectName.Equals(observation.CanonicalSideBObjectName, StringComparison.OrdinalIgnoreCase))
            {
                return JoinOptionality.LeftOuterSideBOptional;
            }

            if (observation.LeftObjectName.Equals(observation.CanonicalSideBObjectName, StringComparison.OrdinalIgnoreCase)
                && observation.RightObjectName.Equals(observation.CanonicalSideAObjectName, StringComparison.OrdinalIgnoreCase))
            {
                return JoinOptionality.LeftOuterSideAOptional;
            }
        }

        return JoinOptionality.Other;
    }

    private static OptionalityBucket? ResolveInnerDominantLeftOutlier(OptionalityCounts counts)
    {
        var sorted = new[] { counts.LeftOuterSideAOptional, counts.LeftOuterSideBOptional }
            .OrderByDescending(static item => item.OccurrenceCount)
            .ThenByDescending(static item => item.TransformCount)
            .ThenBy(static item => item.Kind, StringComparer.Ordinal)
            .ToArray();

        var top = sorted[0];
        var second = sorted[1];
        if (top.OccurrenceCount == 0)
        {
            return null;
        }

        if (second.OccurrenceCount > 0
            && second.OccurrenceCount == top.OccurrenceCount
            && second.TransformCount == top.TransformCount)
        {
            return null;
        }

        return top;
    }

    private static void AddInnerJoinAgainstUsuallyOptionalRelationshipCandidate(
        MetaDataQualityModel model,
        CorpusRelationship relationship,
        MaterializedPattern pattern,
        double consensusRatio,
        double outlierRatio,
        OptionalityBucket innerOutlier,
        string dominantOptionalityKind,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var candidate = AddCandidate(
            model,
            CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship,
            $"InnerJoinAgainstUsuallyOptionalRelationship:{relationship.Id}:{pattern.Row.Id}",
            "A relationship pattern is usually joined as optional, but a minority inner-join usage was detected.",
            "Join optionality can be intentional by transform purpose and should be validated by data owners.",
            usedIds,
            countersByPrefix);

        model.InnerJoinAgainstUsuallyOptionalRelationshipList.Add(new InnerJoinAgainstUsuallyOptionalRelationship
        {
            Id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "InnerJoinAgainstUsuallyOptionalRelationship"),
            DataQualityCandidate = candidate,
            CorpusRelationshipPattern = pattern.Row,
        });

        AddEvidence(
            model,
            candidate,
            relationship,
            pattern.Row,
            "OptionalityDriftInnerAgainstUsuallyOptional",
            innerOutlier.OccurrenceCount,
            innerOutlier.TransformCount,
            consensusRatio,
            outlierRatio,
            $"Pattern '{pattern.Row.CanonicalKeyPartSetSignature}' is usually LEFT OUTER joined ({DescribeLeftOptionality(dominantOptionalityKind, relationship)}) but has INNER outlier usage.",
            ResolveDistinctSourceObjectCount(relationship),
            1,
            options,
            usedIds,
            countersByPrefix);
    }

    private static void AddLeftJoinAgainstUsuallyMandatoryRelationshipCandidate(
        MetaDataQualityModel model,
        CorpusRelationship relationship,
        MaterializedPattern pattern,
        double consensusRatio,
        double outlierRatio,
        OptionalityBucket leftOutlier,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var candidate = AddCandidate(
            model,
            CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship,
            $"LeftJoinAgainstUsuallyMandatoryRelationship:{relationship.Id}:{pattern.Row.Id}",
            "A relationship pattern is usually joined as mandatory, but a minority left-join usage was detected.",
            "Join optionality can be intentional by transform purpose and should be validated by data owners.",
            usedIds,
            countersByPrefix);

        model.LeftJoinAgainstUsuallyMandatoryRelationshipList.Add(new LeftJoinAgainstUsuallyMandatoryRelationship
        {
            Id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "LeftJoinAgainstUsuallyMandatoryRelationship"),
            DataQualityCandidate = candidate,
            CorpusRelationshipPattern = pattern.Row,
        });

        AddEvidence(
            model,
            candidate,
            relationship,
            pattern.Row,
            "OptionalityDriftLeftAgainstUsuallyMandatory",
            leftOutlier.OccurrenceCount,
            leftOutlier.TransformCount,
            consensusRatio,
            outlierRatio,
            $"Pattern '{pattern.Row.CanonicalKeyPartSetSignature}' is usually INNER joined but has LEFT OUTER outlier usage ({DescribeLeftOptionality(leftOutlier.Kind, relationship)}).",
            ResolveDistinctSourceObjectCount(relationship),
            1,
            options,
            usedIds,
            countersByPrefix);
    }

    private static string DescribeLeftOptionality(string kind, CorpusRelationship relationship)
    {
        if (kind == JoinOptionality.LeftOuterSideAOptional)
        {
            return $"nullable side is '{relationship.CanonicalSideAObjectName}'";
        }

        if (kind == JoinOptionality.LeftOuterSideBOptional)
        {
            return $"nullable side is '{relationship.CanonicalSideBObjectName}'";
        }

        return "nullable side is mixed/unknown";
    }

    private static void AddColumnEquivalenceCandidates(
        MetaDataQualityModel model,
        IReadOnlyList<MaterializedRelationship> materializedRelationships,
        IReadOnlyList<MaterializedColumnEquivalence> materializedColumnEquivalences,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        if (materializedColumnEquivalences.Count == 0)
        {
            return;
        }

        var relationshipBySignature = materializedRelationships
            .Select(item => item.Row)
            .Where(static row => !string.IsNullOrWhiteSpace(row.CanonicalUndirectedSignature))
            .ToDictionary(
                row => row.CanonicalUndirectedSignature,
                row => row,
                StringComparer.Ordinal);
        var anchorGroups = BuildColumnAnchorGroups(materializedColumnEquivalences);
        foreach (var anchor in anchorGroups.Values
                     .OrderBy(static item => item.AnchorColumnName, StringComparer.Ordinal))
        {
            var usageRows = anchor.Usages
                .OrderByDescending(static item => item.OccurrenceCount)
                .ThenByDescending(static item => item.TransformCount)
                .ThenBy(static item => item.CounterpartColumnName, StringComparer.Ordinal)
                .ToArray();
            if (usageRows.Length == 0)
            {
                continue;
            }

            var totalOccurrenceCount = usageRows.Sum(static item => item.OccurrenceCount);
            var totalTransformCount = usageRows
                .SelectMany(static item => item.CorpusColumnEquivalence.Aggregate.TransformScriptIds)
                .Where(static value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.Ordinal)
                .Count();
            var dominant = usageRows[0];
            var dominantRatio = totalOccurrenceCount == 0
                ? 0d
                : dominant.OccurrenceCount / (double)totalOccurrenceCount;
            if (totalOccurrenceCount < options.MinColumnAnchorOccurrenceCount
                || totalTransformCount < options.MinColumnAnchorTransformCount
                || dominant.OccurrenceCount < options.DominantColumnEquivalenceMinOccurrenceCount
                || dominantRatio < options.DominantColumnEquivalenceMinRatio)
            {
                continue;
            }

            foreach (var outlier in usageRows
                         .Skip(1)
                         .OrderBy(static item => item.CounterpartColumnName, StringComparer.Ordinal))
            {
                var outlierRatio = totalOccurrenceCount == 0
                    ? 0d
                    : outlier.OccurrenceCount / (double)totalOccurrenceCount;
                if (outlier.OccurrenceCount == 0
                    || outlierRatio > options.MinorityColumnEquivalenceMaxRatio)
                {
                    continue;
                }

                var evidenceRelationship = TryResolveEvidenceRelationship(
                    outlier.CorpusColumnEquivalence,
                    relationshipBySignature);
                if (evidenceRelationship is null)
                {
                    evidenceRelationship = TryResolveEvidenceRelationship(
                        dominant.CorpusColumnEquivalence,
                        relationshipBySignature);
                }

                if (evidenceRelationship is null)
                {
                    continue;
                }

                AddMinorityColumnEquivalenceCandidate(
                    model,
                    anchor.AnchorColumnName,
                    dominant,
                    outlier,
                    evidenceRelationship,
                    dominantRatio,
                    outlierRatio,
                    options,
                    usedIds,
                    countersByPrefix);
            }
        }
    }

    private static Dictionary<string, ColumnAnchorGroup> BuildColumnAnchorGroups(
        IReadOnlyList<MaterializedColumnEquivalence> materializedColumnEquivalences)
    {
        var groups = new Dictionary<string, ColumnAnchorGroup>(StringComparer.Ordinal);
        foreach (var equivalence in materializedColumnEquivalences
                     .OrderBy(static item => item.Row.CanonicalUndirectedSignature, StringComparer.Ordinal))
        {
            RegisterColumnAnchorUsage(
                groups,
                equivalence.Row.CanonicalSideAColumnName,
                equivalence.Row.CanonicalSideBColumnName,
                equivalence);
            if (!string.Equals(equivalence.Row.CanonicalSideAColumnName, equivalence.Row.CanonicalSideBColumnName, StringComparison.Ordinal))
            {
                RegisterColumnAnchorUsage(
                    groups,
                    equivalence.Row.CanonicalSideBColumnName,
                    equivalence.Row.CanonicalSideAColumnName,
                    equivalence);
            }
        }

        return groups;
    }

    private static void RegisterColumnAnchorUsage(
        IDictionary<string, ColumnAnchorGroup> groups,
        string anchorColumnName,
        string counterpartColumnName,
        MaterializedColumnEquivalence equivalence)
    {
        if (string.IsNullOrWhiteSpace(anchorColumnName) || string.IsNullOrWhiteSpace(counterpartColumnName))
        {
            return;
        }

        if (!groups.TryGetValue(anchorColumnName, out var group))
        {
            group = new ColumnAnchorGroup(anchorColumnName);
            groups.Add(anchorColumnName, group);
        }

        group.Usages.Add(new ColumnAnchorUsage(
            counterpartColumnName,
            equivalence,
            equivalence.Aggregate.OccurrenceIds.Count,
            equivalence.Aggregate.TransformScriptIds.Count));
    }

    private static CorpusRelationship? TryResolveEvidenceRelationship(
        MaterializedColumnEquivalence equivalence,
        IReadOnlyDictionary<string, CorpusRelationship> relationshipBySignature)
    {
        return equivalence.Aggregate.Observations
            .Where(static item => !string.IsNullOrWhiteSpace(item.RelationshipSignature))
            .GroupBy(static item => item.RelationshipSignature, StringComparer.Ordinal)
            .Select(group => new
            {
                RelationshipSignature = group.Key,
                OccurrenceCount = group
                    .Select(static item => item.OccurrenceId)
                    .Distinct(StringComparer.Ordinal)
                    .Count(),
            })
            .OrderByDescending(static item => item.OccurrenceCount)
            .ThenBy(static item => item.RelationshipSignature, StringComparer.Ordinal)
            .Select(static item => item.RelationshipSignature)
            .Where(relationshipBySignature.ContainsKey)
            .Select(signature => relationshipBySignature[signature])
            .FirstOrDefault();
    }

    private static void AddMinorityColumnEquivalenceCandidate(
        MetaDataQualityModel model,
        string anchorColumnName,
        ColumnAnchorUsage dominant,
        ColumnAnchorUsage outlier,
        CorpusRelationship evidenceRelationship,
        double consensusRatio,
        double outlierRatio,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var candidate = AddCandidate(
            model,
            CandidateKinds.MinorityColumnEquivalence,
            $"MinorityColumnEquivalence:{anchorColumnName}:{outlier.CorpusColumnEquivalence.Row.Id}",
            "A column is usually joined to one counterpart column, but minority equivalence usage points to a different counterpart.",
            "Minority equivalence can be intentional when semantics differ by transform purpose and should be reviewed by domain owners.",
            usedIds,
            countersByPrefix);

        model.MinorityColumnEquivalenceList.Add(new MinorityColumnEquivalence
        {
            Id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "MinorityColumnEquivalence"),
            DataQualityCandidate = candidate,
            DominantEquivalence = dominant.CorpusColumnEquivalence.Row,
            OutlierEquivalence = outlier.CorpusColumnEquivalence.Row,
        });

        AddEvidence(
            model,
            candidate,
            evidenceRelationship,
            null,
            "ColumnEquivalenceConsensusMinority",
            outlier.OccurrenceCount,
            outlier.TransformCount,
            consensusRatio,
            outlierRatio,
            $"Column '{anchorColumnName}' is usually equated with '{dominant.CounterpartColumnName}', but minority usage equates it with '{outlier.CounterpartColumnName}'.",
            ResolveDistinctSourceObjectCount(evidenceRelationship),
            2,
            options,
            usedIds,
            countersByPrefix);
    }

    private static void AddMinorityCandidate(
        MetaDataQualityModel model,
        CorpusRelationship relationship,
        MaterializedPattern dominant,
        MaterializedPattern outlier,
        double consensusRatio,
        double outlierRatio,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var candidate = AddCandidate(
            model,
            CandidateKinds.MinorityJoinPattern,
            $"MinorityJoinPattern:{relationship.Id}:{outlier.Row.Id}",
            "A minority join pattern was found for a relationship with strong corpus consensus.",
            "Consensus reflects observed transform usage and may still include intentional exceptions.",
            usedIds,
            countersByPrefix);

        model.MinorityJoinPatternList.Add(new MinorityJoinPattern
        {
            Id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "MinorityJoinPattern"),
            DataQualityCandidate = candidate,
            DominantPattern = dominant.Row,
            OutlierPattern = outlier.Row,
        });

        AddEvidence(
            model,
            candidate,
            relationship,
            outlier.Row,
            "RelationshipConsensusMinority",
            outlier.Aggregate.OccurrenceIds.Count,
            outlier.Aggregate.TransformScriptIds.Count,
            consensusRatio,
            outlierRatio,
            $"Outlier pattern '{outlier.Row.CanonicalKeyPartSetSignature}' differs from dominant pattern '{dominant.Row.CanonicalKeyPartSetSignature}'.",
            ResolveDistinctSourceObjectCount(relationship),
            2,
            options,
            usedIds,
            countersByPrefix);
    }

    private static void AddIncompleteCompositeCandidate(
        MetaDataQualityModel model,
        CorpusRelationship relationship,
        MaterializedPattern dominant,
        MaterializedPattern outlier,
        double consensusRatio,
        double outlierRatio,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var candidate = AddCandidate(
            model,
            CandidateKinds.IncompleteCompositeJoin,
            $"IncompleteCompositeJoin:{relationship.Id}:{outlier.Row.Id}",
            "A join pattern omits key parts from the dominant composite relationship pattern.",
            "Missing key parts can be intentional only when grain semantics differ by design.",
            usedIds,
            countersByPrefix);

        model.IncompleteCompositeJoinList.Add(new IncompleteCompositeJoin
        {
            Id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "IncompleteCompositeJoin"),
            DataQualityCandidate = candidate,
            DominantPattern = dominant.Row,
            OutlierPattern = outlier.Row,
        });

        AddEvidence(
            model,
            candidate,
            relationship,
            outlier.Row,
            "RelationshipConsensusIncompleteComposite",
            outlier.Aggregate.OccurrenceIds.Count,
            outlier.Aggregate.TransformScriptIds.Count,
            consensusRatio,
            outlierRatio,
            $"Outlier pattern '{outlier.Row.CanonicalKeyPartSetSignature}' is a strict subset of dominant composite pattern '{dominant.Row.CanonicalKeyPartSetSignature}'.",
            ResolveDistinctSourceObjectCount(relationship),
            2,
            options,
            usedIds,
            countersByPrefix);
    }

    private static void AddSuspiciousExtraCandidate(
        MetaDataQualityModel model,
        CorpusRelationship relationship,
        MaterializedPattern dominant,
        MaterializedPattern outlier,
        double consensusRatio,
        double outlierRatio,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var candidate = AddCandidate(
            model,
            CandidateKinds.SuspiciousExtraJoinPredicate,
            $"SuspiciousExtraJoinPredicate:{relationship.Id}:{outlier.Row.Id}",
            "A join pattern adds predicates beyond the dominant relationship pattern.",
            "Extra predicates can be intentional filters and should be reviewed for accidental row loss.",
            usedIds,
            countersByPrefix);

        model.SuspiciousExtraJoinPredicateList.Add(new SuspiciousExtraJoinPredicate
        {
            Id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "SuspiciousExtraJoinPredicate"),
            DataQualityCandidate = candidate,
            DominantPattern = dominant.Row,
            OutlierPattern = outlier.Row,
        });

        AddEvidence(
            model,
            candidate,
            relationship,
            outlier.Row,
            "RelationshipConsensusSuspiciousExtra",
            outlier.Aggregate.OccurrenceIds.Count,
            outlier.Aggregate.TransformScriptIds.Count,
            consensusRatio,
            outlierRatio,
            $"Outlier pattern '{outlier.Row.CanonicalKeyPartSetSignature}' is a strict superset of dominant pattern '{dominant.Row.CanonicalKeyPartSetSignature}'.",
            ResolveDistinctSourceObjectCount(relationship),
            2,
            options,
            usedIds,
            countersByPrefix);
    }

    private static void AddMissingCommonFilterCandidates(
        MetaDataQualityModel model,
        CorpusRelationship relationship,
        MaterializedPattern dominant,
        MaterializedPattern outlier,
        IReadOnlyDictionary<string, FilterPredicateObservation[]> filterPredicatesByOccurrenceId,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        if (dominant.Aggregate.OccurrenceIds.Count == 0
            || outlier.Aggregate.OccurrenceIds.Count == 0)
        {
            return;
        }

        var dominantFilterBuckets = BuildFilterBuckets(dominant.Aggregate.Observations, filterPredicatesByOccurrenceId);
        foreach (var bucket in dominantFilterBuckets.Values
                     .OrderByDescending(static item => item.OccurrenceIds.Count)
                     .ThenByDescending(static item => item.TransformScriptIds.Count)
                     .ThenBy(static item => item.BaseObjectName, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(static item => item.PredicateSignature, StringComparer.Ordinal))
        {
            if (bucket.OccurrenceIds.Count < options.MinCommonFilterOccurrenceCount
                || bucket.TransformScriptIds.Count < options.MinCommonFilterTransformCount)
            {
                continue;
            }

            var dominantFilterRatio = bucket.OccurrenceIds.Count / (double)dominant.Aggregate.OccurrenceIds.Count;
            if (dominantFilterRatio < options.MinCommonFilterConsensusRatio)
            {
                continue;
            }

            var outlierFilterOccurrences = CountFilterUsage(
                outlier.Aggregate.Observations,
                filterPredicatesByOccurrenceId,
                bucket.BaseObjectName,
                bucket.PredicateSignature);
            var outlierFilterRatio = outlierFilterOccurrences / (double)outlier.Aggregate.OccurrenceIds.Count;
            if (outlierFilterRatio > options.MissingCommonFilterOutlierMaxRatio)
            {
                continue;
            }

            var candidate = AddCandidate(
                model,
                CandidateKinds.MissingCommonFilter,
                $"MissingCommonFilter:{relationship.Id}:{outlier.Row.Id}:{CorpusInferenceNormalization.NormalizeSignaturePart(bucket.BaseObjectName)}",
                "A dominant relationship pattern consistently uses a filter predicate that an outlier pattern omits.",
                "Common filters can be intentionally omitted for historical/full extracts and should be validated by domain owners.",
                usedIds,
                countersByPrefix);

            model.MissingCommonFilterList.Add(new MissingCommonFilter
            {
                Id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "MissingCommonFilter"),
                DataQualityCandidate = candidate,
                DominantPattern = dominant.Row,
                OutlierPattern = outlier.Row,
                BaseObjectName = bucket.BaseObjectName,
                CommonPredicateSignature = bucket.PredicateSignature,
                CommonPredicateDisplay = bucket.PredicateDisplay,
            });

            AddEvidence(
                model,
                candidate,
                relationship,
                outlier.Row,
                "CommonFilterConsensusMissing",
                outlierFilterOccurrences,
                outlier.Aggregate.TransformScriptIds.Count,
                dominantFilterRatio,
                outlierFilterRatio,
                $"Dominant pattern '{dominant.Row.CanonicalKeyPartSetSignature}' usually applies '{bucket.PredicateDisplay}' on '{bucket.BaseObjectName}', but outlier pattern '{outlier.Row.CanonicalKeyPartSetSignature}' mostly omits it.",
                ResolveDistinctSourceObjectCount(relationship),
                2,
                options,
                usedIds,
                countersByPrefix);
        }
    }

    private static Dictionary<string, FilterBucket> BuildFilterBuckets(
        IReadOnlyList<OccurrenceObservation> observations,
        IReadOnlyDictionary<string, FilterPredicateObservation[]> filterPredicatesByOccurrenceId)
    {
        var buckets = new Dictionary<string, FilterBucket>(StringComparer.Ordinal);
        foreach (var observation in observations)
        {
            if (!filterPredicatesByOccurrenceId.TryGetValue(observation.OccurrenceId, out var predicates) || predicates.Length == 0)
            {
                continue;
            }

            foreach (var predicate in predicates)
            {
                var key = $"{predicate.BaseObjectName}|{predicate.PredicateSignature}";
                if (!buckets.TryGetValue(key, out var bucket))
                {
                    bucket = new FilterBucket(predicate.BaseObjectName, predicate.PredicateSignature, predicate.PredicateDisplay);
                    buckets.Add(key, bucket);
                }

                bucket.OccurrenceIds.Add(observation.OccurrenceId);
                if (!string.IsNullOrWhiteSpace(observation.TransformScriptId))
                {
                    bucket.TransformScriptIds.Add(observation.TransformScriptId);
                }
            }
        }

        return buckets;
    }

    private static int CountFilterUsage(
        IReadOnlyList<OccurrenceObservation> observations,
        IReadOnlyDictionary<string, FilterPredicateObservation[]> filterPredicatesByOccurrenceId,
        string baseObjectName,
        string predicateSignature)
    {
        var count = 0;
        foreach (var observation in observations)
        {
            if (!filterPredicatesByOccurrenceId.TryGetValue(observation.OccurrenceId, out var predicates))
            {
                continue;
            }

            if (predicates.Any(row =>
                    string.Equals(row.BaseObjectName, baseObjectName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(row.PredicateSignature, predicateSignature, StringComparison.Ordinal)))
            {
                count++;
            }
        }

        return count;
    }

    private static void AddImpliedForeignKeyCandidate(
        MetaDataQualityModel model,
        CorpusRelationship relationship,
        MaterializedPattern dominant,
        double consensusRatio,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var candidate = AddCandidate(
            model,
            CandidateKinds.ImpliedForeignKeyMissingReference,
            $"ImpliedForeignKeyMissingReference:{relationship.Id}:{dominant.Row.Id}",
            "High-consensus join behavior implies a relationship contract suitable for missing-reference checks.",
            "Implied FK direction is inferred from dominant directional occurrences and should be validated by domain owners.",
            usedIds,
            countersByPrefix);

        model.ImpliedForeignKeyMissingReferenceList.Add(new ImpliedForeignKeyMissingReference
        {
            Id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "ImpliedForeignKeyMissingReference"),
            DataQualityCandidate = candidate,
            DominantPattern = dominant.Row,
        });

        AddEvidence(
            model,
            candidate,
            relationship,
            dominant.Row,
            "ImpliedForeignKeyConsensus",
            dominant.Aggregate.OccurrenceIds.Count,
            dominant.Aggregate.TransformScriptIds.Count,
            consensusRatio,
            1d - consensusRatio,
            $"Dominant relationship pattern '{dominant.Row.CanonicalKeyPartSetSignature}' reached consensus ratio {CorpusInferenceNormalization.FormatRatio(consensusRatio)}.",
            ResolveDistinctSourceObjectCount(relationship),
            1,
            options,
            usedIds,
            countersByPrefix);
    }

    private static void AddImpliedUniqueKeyCandidate(
        MetaDataQualityModel model,
        CorpusRelationship relationship,
        MaterializedPattern dominant,
        double consensusRatio,
        double outlierRatio,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var candidate = AddCandidate(
            model,
            CandidateKinds.ImpliedUniqueKeyViolation,
            $"ImpliedUniqueKeyViolation:{relationship.Id}:{dominant.Row.Id}",
            "High-consensus join behavior implies a unique-key expectation on the lookup side.",
            "Unique-side inference is based on dominant directional usage and should be validated against business constraints.",
            usedIds,
            countersByPrefix);

        model.ImpliedUniqueKeyViolationList.Add(new ImpliedUniqueKeyViolation
        {
            Id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "ImpliedUniqueKeyViolation"),
            DataQualityCandidate = candidate,
            DominantPattern = dominant.Row,
        });

        AddEvidence(
            model,
            candidate,
            relationship,
            dominant.Row,
            "ImpliedUniqueKeyConsensus",
            dominant.Aggregate.OccurrenceIds.Count,
            dominant.Aggregate.TransformScriptIds.Count,
            consensusRatio,
            outlierRatio,
            $"Dominant relationship pattern '{dominant.Row.CanonicalKeyPartSetSignature}' reached consensus ratio {CorpusInferenceNormalization.FormatRatio(consensusRatio)} with directional outlier ratio {CorpusInferenceNormalization.FormatRatio(outlierRatio)}.",
            ResolveDistinctSourceObjectCount(relationship),
            1,
            options,
            usedIds,
            countersByPrefix);
    }

    private static DataQualityCandidate AddCandidate(
        MetaDataQualityModel model,
        string kind,
        string name,
        string rationale,
        string assumptions,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, $"DataQualityCandidate.{kind}");
        var candidate = new DataQualityCandidate
        {
            Id = id,
            Name = name,
            Status = CandidateStatuses.Discovered,
            Rationale = rationale,
            Assumptions = assumptions,
            SqlTemplate = string.Empty,
        };
        model.DataQualityCandidateList.Add(candidate);
        return candidate;
    }

    private static void AddEvidence(
        MetaDataQualityModel model,
        DataQualityCandidate dataQualityCandidate,
        CorpusRelationship corpusRelationship,
        CorpusRelationshipPattern? corpusRelationshipPattern,
        string evidenceType,
        int occurrenceCount,
        int transformCount,
        double consensusRatio,
        double outlierRatio,
        string explanation,
        int distinctSourceObjectCount,
        int distinctRelationshipPatternCount,
        CorpusInferenceOptions options,
        ISet<string> usedIds,
        IDictionary<string, int> countersByPrefix)
    {
        var calibration = BuildCalibrationMetadata(
            transformCount,
            distinctSourceObjectCount,
            distinctRelationshipPatternCount,
            consensusRatio,
            options);
        var id = CorpusInferenceIdAllocator.BuildUniqueId(usedIds, countersByPrefix, "DataQualityCandidateEvidence");
        model.DataQualityCandidateEvidenceList.Add(new DataQualityCandidateEvidence
        {
            Id = id,
            DataQualityCandidate = dataQualityCandidate,
            CorpusRelationship = corpusRelationship,
            CorpusRelationshipPattern = corpusRelationshipPattern,
            EvidenceType = evidenceType,
            OccurrenceCount = occurrenceCount.ToString(CultureInfo.InvariantCulture),
            TransformCount = transformCount.ToString(CultureInfo.InvariantCulture),
            DistinctTransformCount = calibration.DistinctTransformCount.ToString(CultureInfo.InvariantCulture),
            DistinctSourceTransformCount = calibration.DistinctSourceTransformCount.ToString(CultureInfo.InvariantCulture),
            DistinctSourceObjectCount = calibration.DistinctSourceObjectCount.ToString(CultureInfo.InvariantCulture),
            DistinctRelationshipPatternCount = calibration.DistinctRelationshipPatternCount.ToString(CultureInfo.InvariantCulture),
            EffectiveTransformCount = calibration.EffectiveTransformCount.ToString(CultureInfo.InvariantCulture),
            ConsensusRatio = CorpusInferenceNormalization.FormatRatio(consensusRatio),
            OutlierRatio = CorpusInferenceNormalization.FormatRatio(outlierRatio),
            EvidenceQuality = calibration.EvidenceQuality,
            ConfidenceBand = calibration.ConfidenceBand,
            ConfidenceReason = calibration.ConfidenceReason,
            EvidenceDiversitySummary = calibration.EvidenceDiversitySummary,
            Explanation = explanation,
        });
    }

    private static EvidenceCalibrationMetadata BuildCalibrationMetadata(
        int evidenceTransformCount,
        int distinctSourceObjectCount,
        int distinctRelationshipPatternCount,
        double consensusRatio,
        CorpusInferenceOptions options)
    {
        var distinctTransformCount = Math.Max(evidenceTransformCount, 0);
        var distinctSourceTransformCount = distinctTransformCount;
        var effectiveTransformCount = distinctTransformCount;
        var sourceObjectCount = Math.Max(distinctSourceObjectCount, 1);
        var relationshipPatternCount = Math.Max(distinctRelationshipPatternCount, 1);

        var evidenceQuality = distinctTransformCount >= options.MinHighConfidenceDistinctTransformCount
            ? "High"
            : distinctTransformCount >= options.MinMediumConfidenceDistinctTransformCount
                ? "Medium"
                : "Low";

        var confidenceBand = ResolveConfidenceBand(
            distinctTransformCount,
            sourceObjectCount,
            consensusRatio,
            options);
        var confidenceReason = ResolveConfidenceReason(
            confidenceBand,
            distinctTransformCount,
            sourceObjectCount,
            consensusRatio);
        var diversitySummary =
            $"Distinct transforms: {distinctTransformCount}; source objects: {sourceObjectCount}; relationship patterns: {relationshipPatternCount}; effective transforms: {effectiveTransformCount}.";

        return new EvidenceCalibrationMetadata(
            distinctTransformCount,
            distinctSourceTransformCount,
            sourceObjectCount,
            relationshipPatternCount,
            effectiveTransformCount,
            evidenceQuality,
            confidenceBand,
            confidenceReason,
            diversitySummary);
    }

    private static string ResolveConfidenceBand(
        int distinctTransformCount,
        int distinctSourceObjectCount,
        double consensusRatio,
        CorpusInferenceOptions options)
    {
        if (distinctTransformCount >= options.MinHighConfidenceDistinctTransformCount
            && consensusRatio >= options.MinHighConfidenceConsensusRatio
            && distinctSourceObjectCount >= 2)
        {
            return "High";
        }

        if (distinctTransformCount >= options.MinMediumConfidenceDistinctTransformCount
            && consensusRatio >= options.MinMediumConfidenceConsensusRatio)
        {
            return "Medium";
        }

        return "Low";
    }

    private static string ResolveConfidenceReason(
        string confidenceBand,
        int distinctTransformCount,
        int distinctSourceObjectCount,
        double consensusRatio)
    {
        return confidenceBand switch
        {
            "High" => $"Strong diversity and consensus ({distinctTransformCount} transforms, {distinctSourceObjectCount} source objects, consensus {CorpusInferenceNormalization.FormatRatio(consensusRatio)}).",
            "Medium" => $"Adequate corpus support but limited diversity or consensus ({distinctTransformCount} transforms, {distinctSourceObjectCount} source objects, consensus {CorpusInferenceNormalization.FormatRatio(consensusRatio)}).",
            _ => $"Low diversity or borderline corpus support ({distinctTransformCount} transforms, {distinctSourceObjectCount} source objects, consensus {CorpusInferenceNormalization.FormatRatio(consensusRatio)}).",
        };
    }

    private static int ResolveDistinctSourceObjectCount(CorpusRelationship relationship)
    {
        if (string.Equals(
                relationship.CanonicalSideAObjectName,
                relationship.CanonicalSideBObjectName,
                StringComparison.OrdinalIgnoreCase))
        {
            return 1;
        }

        return 2;
    }

    private static string FormatOccurrenceIdPreview(IReadOnlyList<string> occurrenceIds)
    {
        if (occurrenceIds.Count == 0)
        {
            return "(none)";
        }

        const int previewLimit = 10;
        var preview = occurrenceIds
            .Take(previewLimit)
            .ToArray();
        return occurrenceIds.Count <= previewLimit
            ? string.Join(", ", preview)
            : $"{string.Join(", ", preview)} (+{occurrenceIds.Count - previewLimit} more)";
    }

    private sealed class OptionalityBucket
    {
        private readonly HashSet<string> transformScriptIds = new(StringComparer.Ordinal);

        public OptionalityBucket(string kind)
        {
            Kind = kind;
        }

        public string Kind { get; }

        public int OccurrenceCount { get; private set; }

        public int TransformCount => transformScriptIds.Count;

        public void Register(string transformScriptId)
        {
            OccurrenceCount++;
            if (!string.IsNullOrWhiteSpace(transformScriptId))
            {
                transformScriptIds.Add(transformScriptId);
            }
        }
    }

    private readonly record struct OptionalityCounts(
        OptionalityBucket Inner,
        OptionalityBucket LeftOuterSideAOptional,
        OptionalityBucket LeftOuterSideBOptional,
        OptionalityBucket Other)
    {
        public int TotalOccurrenceCount => Inner.OccurrenceCount + LeftOuterSideAOptional.OccurrenceCount + LeftOuterSideBOptional.OccurrenceCount + Other.OccurrenceCount;
    }

    private readonly record struct SignalCounts(
        int OccurrenceCount,
        int TransformCount,
        string[] OccurrenceIds);

    private readonly record struct EvidenceCalibrationMetadata(
        int DistinctTransformCount,
        int DistinctSourceTransformCount,
        int DistinctSourceObjectCount,
        int DistinctRelationshipPatternCount,
        int EffectiveTransformCount,
        string EvidenceQuality,
        string ConfidenceBand,
        string ConfidenceReason,
        string EvidenceDiversitySummary);

    private sealed class FilterBucket
    {
        public FilterBucket(string baseObjectName, string predicateSignature, string predicateDisplay)
        {
            BaseObjectName = baseObjectName;
            PredicateSignature = predicateSignature;
            PredicateDisplay = predicateDisplay;
        }

        public string BaseObjectName { get; }

        public string PredicateSignature { get; }

        public string PredicateDisplay { get; }

        public HashSet<string> OccurrenceIds { get; } = new(StringComparer.Ordinal);

        public HashSet<string> TransformScriptIds { get; } = new(StringComparer.Ordinal);
    }

    private sealed class ColumnAnchorGroup
    {
        public ColumnAnchorGroup(string anchorColumnName)
        {
            AnchorColumnName = anchorColumnName;
        }

        public string AnchorColumnName { get; }

        public List<ColumnAnchorUsage> Usages { get; } = [];
    }

    private readonly record struct ColumnAnchorUsage(
        string CounterpartColumnName,
        MaterializedColumnEquivalence CorpusColumnEquivalence,
        int OccurrenceCount,
        int TransformCount);

    private static class JoinOptionality
    {
        public const string Inner = "Inner";
        public const string LeftOuterSideAOptional = "LeftOuterSideAOptional";
        public const string LeftOuterSideBOptional = "LeftOuterSideBOptional";
        public const string Other = "Other";
    }
}
