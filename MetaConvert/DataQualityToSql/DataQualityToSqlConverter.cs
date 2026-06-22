using System.Text;
using System.Globalization;
using MetaDataQuality;
using MetaDataQuality.Core;

namespace MetaConvert.DataQualityToSql;

public sealed class DataQualityToSqlConverter
{
    private const string OutputModeRuntimeCheck = "RuntimeCheck";
    private const string OutputModeSemanticReviewFinding = "SemanticReviewFinding";

    public DataQualityToSqlResult Convert(string workspacePath, string outputPath)
    {
        var model = MetaDataQualityModel.LoadFromXmlWorkspace(workspacePath, searchUpward: false);
        var candidateTypes = ResolveCandidateTypeMap(model);
        var candidateEvidenceByCandidateId = model.DataQualityCandidateEvidenceList
            .GroupBy(item => item.DataQualityCandidate.Id, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(static item => item.Id, StringComparer.Ordinal)
                    .ToArray(),
                StringComparer.Ordinal);
        var corpusRelationshipById = model.CorpusRelationshipList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var corpusRelationshipPatternById = model.CorpusRelationshipPatternList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var comparisonPatternByCandidateId = BuildComparisonPatternByCandidateId(model);
        var optionalityPatternByCandidateId = BuildOptionalityPatternByCandidateId(model);
        var joinPatternById = model.JoinPatternList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var occurrenceById = model.JoinPatternOccurrenceList.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var occurrencesByPatternId = model.JoinPatternOccurrenceList
            .GroupBy(item => item.JoinPattern.Id, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
        var baseTablesByOccurrenceId = model.JoinPatternOccurrenceBaseTableList
            .GroupBy(item => item.JoinPatternOccurrence.Id, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
        var keyPartsByPatternId = model.JoinPatternKeyPartList
            .GroupBy(item => item.JoinPattern.Id, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
        var candidateJoinPatternIds = ResolveCandidateJoinPatternIds(model, occurrenceById);
        var promoted = model.DataQualityCandidateList
            .Where(candidate => string.Equals(candidate.Status, CandidateStatuses.Promoted, StringComparison.OrdinalIgnoreCase))
            .OrderBy(candidate => candidateTypes.TryGetValue(candidate.Id, out var type) ? type : string.Empty, StringComparer.OrdinalIgnoreCase)
            .ThenBy(candidate => candidate.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        ValidatePromotedCandidateTypes(promoted, candidateTypes);

        if (IsSqlFilePath(outputPath))
        {
            var fullPath = Path.GetFullPath(outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            var sql = RenderCombinedSql(
                promoted,
                candidateTypes,
                candidateEvidenceByCandidateId,
                corpusRelationshipById,
                corpusRelationshipPatternById,
                comparisonPatternByCandidateId,
                optionalityPatternByCandidateId,
                candidateJoinPatternIds,
                joinPatternById,
                occurrencesByPatternId,
                baseTablesByOccurrenceId,
                keyPartsByPatternId);
            File.WriteAllText(fullPath, sql, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            return new DataQualityToSqlResult
            {
                OutputPath = fullPath,
                CandidateViewCount = promoted.Length,
                DashboardViewCount = 1,
                OperationalTableCount = 2,
                OperationalProcedureCount = 2,
            };
        }

        var outputDirectory = Path.GetFullPath(outputPath);
        Directory.CreateDirectory(outputDirectory);
        foreach (var candidate in promoted)
        {
            var filePath = Path.Combine(outputDirectory, $"{SanitizeFileName(candidate.Name)}.sql");
            var candidateType = ResolveCandidateType(candidateTypes, candidate.Id);
            var sql = RenderSingleViewSql(
                candidate,
                candidateType,
                includeSchemaGuard: true,
                candidateEvidenceByCandidateId,
                corpusRelationshipById,
                corpusRelationshipPatternById,
                comparisonPatternByCandidateId,
                optionalityPatternByCandidateId,
                candidateJoinPatternIds,
                joinPatternById,
                occurrencesByPatternId,
                baseTablesByOccurrenceId,
                keyPartsByPatternId);
            File.WriteAllText(filePath, sql, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }

        var dashboardPath = Path.Combine(outputDirectory, "v_DataQualityReview.sql");
        var dashboardSql = RenderDashboardViewSql(
            promoted,
            candidateTypes,
            includeSchemaGuard: true,
            candidateEvidenceByCandidateId,
            corpusRelationshipById,
            corpusRelationshipPatternById,
            comparisonPatternByCandidateId,
            optionalityPatternByCandidateId,
            candidateJoinPatternIds,
            joinPatternById,
            occurrencesByPatternId,
            baseTablesByOccurrenceId,
            keyPartsByPatternId);
        File.WriteAllText(dashboardPath, dashboardSql, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        var operationalPath = Path.Combine(outputDirectory, "MetaDQ.Operational.sql");
        var operationalSql = RenderMetaDqOperationalSql();
        File.WriteAllText(operationalPath, operationalSql, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        return new DataQualityToSqlResult
        {
            OutputPath = outputDirectory,
            CandidateViewCount = promoted.Length,
            DashboardViewCount = 1,
            OperationalTableCount = 2,
            OperationalProcedureCount = 2,
        };
    }

    private static bool IsSqlFilePath(string path) =>
        string.Equals(Path.GetExtension(path), ".sql", StringComparison.OrdinalIgnoreCase);

    private static IReadOnlyDictionary<string, string[]> ResolveCandidateJoinPatternIds(
        MetaDataQualityModel model,
        IReadOnlyDictionary<string, JoinPatternOccurrence> occurrenceById)
    {
        var byCandidate = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        foreach (var link in model.DataQualityCandidateJoinPatternLinkList)
        {
            AddCandidateJoinPatternId(byCandidate, link.DataQualityCandidate.Id, link.JoinPattern.Id);
        }

        var joinPatternIdsByCorpusPatternId = model.CorpusRelationshipPatternOccurrenceLinkList
            .GroupBy(link => link.CorpusRelationshipPattern.Id, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(link => occurrenceById.TryGetValue(link.JoinPatternOccurrence.Id, out var occurrence)
                        ? occurrence.JoinPattern.Id
                        : string.Empty)
                    .Where(static value => !string.IsNullOrWhiteSpace(value))
                    .Distinct(StringComparer.Ordinal)
                    .ToArray(),
                StringComparer.Ordinal);

        foreach (var row in model.ImpliedForeignKeyMissingReferenceList)
        {
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.DominantPattern.Id,
                joinPatternIdsByCorpusPatternId);
        }

        foreach (var row in model.ImpliedUniqueKeyViolationList)
        {
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.DominantPattern.Id,
                joinPatternIdsByCorpusPatternId);
        }

        foreach (var row in model.ImpliedJoinFanoutRiskList)
        {
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.DominantPattern.Id,
                joinPatternIdsByCorpusPatternId);
        }

        foreach (var row in model.ImpliedOutputDuplicateRiskList)
        {
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.DominantPattern.Id,
                joinPatternIdsByCorpusPatternId);
        }

        foreach (var row in model.MinorityJoinPatternList)
        {
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.DominantPattern.Id,
                joinPatternIdsByCorpusPatternId);
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.OutlierPattern.Id,
                joinPatternIdsByCorpusPatternId);
        }

        foreach (var row in model.IncompleteCompositeJoinList)
        {
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.DominantPattern.Id,
                joinPatternIdsByCorpusPatternId);
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.OutlierPattern.Id,
                joinPatternIdsByCorpusPatternId);
        }

        foreach (var row in model.SuspiciousExtraJoinPredicateList)
        {
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.DominantPattern.Id,
                joinPatternIdsByCorpusPatternId);
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.OutlierPattern.Id,
                joinPatternIdsByCorpusPatternId);
        }

        foreach (var row in model.InnerJoinAgainstUsuallyOptionalRelationshipList)
        {
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.CorpusRelationshipPattern.Id,
                joinPatternIdsByCorpusPatternId);
        }

        foreach (var row in model.LeftJoinAgainstUsuallyMandatoryRelationshipList)
        {
            AddCandidateJoinPatternIdsFromCorpusPattern(
                byCandidate,
                row.DataQualityCandidate.Id,
                row.CorpusRelationshipPattern.Id,
                joinPatternIdsByCorpusPatternId);
        }

        return byCandidate.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.OrderBy(static item => item, StringComparer.Ordinal).ToArray(),
            StringComparer.Ordinal);
    }

    private static void AddCandidateJoinPatternIdsFromCorpusPattern(
        IDictionary<string, HashSet<string>> byCandidate,
        string candidateId,
        string corpusRelationshipPatternId,
        IReadOnlyDictionary<string, string[]> joinPatternIdsByCorpusPatternId)
    {
        if (string.IsNullOrWhiteSpace(candidateId)
            || string.IsNullOrWhiteSpace(corpusRelationshipPatternId)
            || !joinPatternIdsByCorpusPatternId.TryGetValue(corpusRelationshipPatternId, out var joinPatternIds))
        {
            return;
        }

        foreach (var joinPatternId in joinPatternIds)
        {
            AddCandidateJoinPatternId(byCandidate, candidateId, joinPatternId);
        }
    }

    private static void AddCandidateJoinPatternId(
        IDictionary<string, HashSet<string>> byCandidate,
        string candidateId,
        string joinPatternId)
    {
        if (string.IsNullOrWhiteSpace(candidateId) || string.IsNullOrWhiteSpace(joinPatternId))
        {
            return;
        }

        if (!byCandidate.TryGetValue(candidateId, out var joinPatternIds))
        {
            joinPatternIds = new HashSet<string>(StringComparer.Ordinal);
            byCandidate.Add(candidateId, joinPatternIds);
        }

        joinPatternIds.Add(joinPatternId);
    }

    private static void ValidatePromotedCandidateTypes(
        IReadOnlyList<DataQualityCandidate> promotedCandidates,
        IReadOnlyDictionary<string, string> candidateTypes)
    {
        foreach (var candidate in promotedCandidates)
        {
            var candidateType = ResolveCandidateType(candidateTypes, candidate.Id);
            if (ResolveCandidateSqlOutputMode(candidateType) != CandidateSqlOutputMode.Unsupported)
            {
                continue;
            }

            throw new InvalidOperationException(
                $"Promoted DataQualityCandidate '{candidate.Id}' has unsupported type '{candidateType}' for data-quality-to-sql. Supported output modes are RuntimeCheck and SemanticReviewFinding.");
        }
    }

    private static CandidateSqlOutputMode ResolveCandidateSqlOutputMode(string candidateType)
    {
        if (candidateType is CandidateKinds.JoinOrphan
            or CandidateKinds.OuterJoinNullExpansion
            or CandidateKinds.JoinMultiplicityExplosion
            or CandidateKinds.OutputDuplicateRisk
            or CandidateKinds.ImpliedForeignKeyMissingReference
            or CandidateKinds.ImpliedUniqueKeyViolation)
        {
            return CandidateSqlOutputMode.RuntimeCheck;
        }

        if (candidateType is CandidateKinds.MinorityJoinPattern
            or CandidateKinds.IncompleteCompositeJoin
            or CandidateKinds.SuspiciousExtraJoinPredicate
            or CandidateKinds.MissingCommonFilter
            or CandidateKinds.MinorityColumnEquivalence
            or CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship
            or CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship
            or CandidateKinds.ImpliedJoinFanoutRisk
            or CandidateKinds.ImpliedOutputDuplicateRisk)
        {
            return CandidateSqlOutputMode.SemanticReviewFinding;
        }

        return CandidateSqlOutputMode.Unsupported;
    }

    private static string RenderCombinedSql(
        IReadOnlyList<DataQualityCandidate> candidates,
        IReadOnlyDictionary<string, string> candidateTypes,
        IReadOnlyDictionary<string, DataQualityCandidateEvidence[]> candidateEvidenceByCandidateId,
        IReadOnlyDictionary<string, CorpusRelationship> corpusRelationshipById,
        IReadOnlyDictionary<string, CorpusRelationshipPattern> corpusRelationshipPatternById,
        IReadOnlyDictionary<string, PatternComparison> comparisonPatternByCandidateId,
        IReadOnlyDictionary<string, string> optionalityPatternByCandidateId,
        IReadOnlyDictionary<string, string[]> candidateJoinPatternIds,
        IReadOnlyDictionary<string, JoinPattern> joinPatternById,
        IReadOnlyDictionary<string, JoinPatternOccurrence[]> occurrencesByPatternId,
        IReadOnlyDictionary<string, JoinPatternOccurrenceBaseTable[]> baseTablesByOccurrenceId,
        IReadOnlyDictionary<string, JoinPatternKeyPart[]> keyPartsByPatternId)
    {
        var builder = new StringBuilder();
        builder.AppendLine("IF SCHEMA_ID(N'dq') IS NULL EXEC(N'CREATE SCHEMA [dq]');");
        builder.AppendLine("GO");
        builder.AppendLine();

        for (var i = 0; i < candidates.Count; i++)
        {
            var candidateType = ResolveCandidateType(candidateTypes, candidates[i].Id);
            builder.Append(RenderSingleViewSql(
                candidates[i],
                candidateType,
                includeSchemaGuard: false,
                candidateEvidenceByCandidateId,
                corpusRelationshipById,
                corpusRelationshipPatternById,
                comparisonPatternByCandidateId,
                optionalityPatternByCandidateId,
                candidateJoinPatternIds,
                joinPatternById,
                occurrencesByPatternId,
                baseTablesByOccurrenceId,
                keyPartsByPatternId));
            if (i < candidates.Count - 1)
            {
                builder.AppendLine();
            }
        }

        builder.AppendLine();
        builder.Append(RenderDashboardViewSql(
            candidates,
            candidateTypes,
            includeSchemaGuard: false,
            candidateEvidenceByCandidateId,
            corpusRelationshipById,
            corpusRelationshipPatternById,
            comparisonPatternByCandidateId,
            optionalityPatternByCandidateId,
            candidateJoinPatternIds,
            joinPatternById,
            occurrencesByPatternId,
            baseTablesByOccurrenceId,
            keyPartsByPatternId));
        builder.AppendLine();
        builder.Append(RenderMetaDqOperationalSql());

        return builder.ToString();
    }

    private static string RenderMetaDqOperationalSql()
    {
        var builder = new StringBuilder();
        builder.AppendLine("/* MetaDataQuality operational store */");
        builder.AppendLine("IF DB_ID(N'MetaDQ') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    CREATE DATABASE [MetaDQ];");
        builder.AppendLine("END");
        builder.AppendLine("GO");
        builder.AppendLine();
        builder.AppendLine("EXEC [MetaDQ].sys.sp_executesql N'");
        builder.AppendLine("IF OBJECT_ID(N''[dbo].[RunLog]'', N''U'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    CREATE TABLE [dbo].[RunLog]");
        builder.AppendLine("    (");
        builder.AppendLine("        [RunId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_dbo_RunLog] PRIMARY KEY,");
        builder.AppendLine("        [StartedAtUtc] datetime2(3) NOT NULL CONSTRAINT [DF_dbo_RunLog_StartedAtUtc] DEFAULT (SYSUTCDATETIME()),");
        builder.AppendLine("        [CompletedAtUtc] datetime2(3) NULL,");
        builder.AppendLine("        [SourceDatabaseName] sysname NOT NULL,");
        builder.AppendLine("        [Status] nvarchar(32) NOT NULL,");
        builder.AppendLine("        [ErrorMessage] nvarchar(4000) NULL");
        builder.AppendLine("    );");
        builder.AppendLine("END;");
        builder.AppendLine();
        builder.AppendLine("IF COL_LENGTH(N''dbo.RunLog'', N''SourceDatabaseName'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[RunLog] ADD [SourceDatabaseName] sysname NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.RunLog'', N''Status'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[RunLog] ADD [Status] nvarchar(32) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.RunLog'', N''ErrorMessage'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[RunLog] ADD [ErrorMessage] nvarchar(4000) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine();
        builder.AppendLine("IF OBJECT_ID(N''[dbo].[FindingLog]'', N''U'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    CREATE TABLE [dbo].[FindingLog]");
        builder.AppendLine("    (");
        builder.AppendLine("        [FindingId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_dbo_FindingLog] PRIMARY KEY,");
        builder.AppendLine("        [RunId] bigint NOT NULL,");
        builder.AppendLine("        [DQView] nvarchar(128) NOT NULL,");
        builder.AppendLine("        [Issue] nvarchar(128) NOT NULL,");
        builder.AppendLine("        [FindingTitle] nvarchar(128) NULL,");
        builder.AppendLine("        [FindingCategory] nvarchar(128) NULL,");
        builder.AppendLine("        [OutputMode] nvarchar(64) NULL,");
        builder.AppendLine("        [CandidateId] nvarchar(256) NULL,");
        builder.AppendLine("        [CandidateKind] nvarchar(128) NULL,");
        builder.AppendLine("        [Relationship] nvarchar(512) NOT NULL,");
        builder.AppendLine("        [RelationshipLabel] nvarchar(512) NULL,");
        builder.AppendLine("        [ReferencingObject] nvarchar(512) NULL,");
        builder.AppendLine("        [ReferencedObject] nvarchar(512) NULL,");
        builder.AppendLine("        [CheckedObject] nvarchar(512) NULL,");
        builder.AppendLine("        [SuspectSide] nvarchar(512) NULL,");
        builder.AppendLine("        [SuspectObject] nvarchar(512) NULL,");
        builder.AppendLine("        [LookupObject] nvarchar(512) NULL,");
        builder.AppendLine("        [RelatedObject] nvarchar(512) NULL,");
        builder.AppendLine("        [CorpusRelationship] nvarchar(512) NULL,");
        builder.AppendLine("        [CorpusRelationshipPattern] nvarchar(max) NULL,");
        builder.AppendLine("        [DominantPattern] nvarchar(max) NULL,");
        builder.AppendLine("        [OutlierPattern] nvarchar(max) NULL,");
        builder.AppendLine("        [TransformViews] nvarchar(max) NOT NULL,");
        builder.AppendLine("        [GeneratedView] nvarchar(512) NOT NULL,");
        builder.AppendLine("        [RowsReturned] bigint NULL,");
        builder.AppendLine("        [ResultRowCount] bigint NULL,");
        builder.AppendLine("        [FindingGroupCount] bigint NULL,");
        builder.AppendLine("        [TotalSuspectCount] bigint NULL,");
        builder.AppendLine("        [SuspectRowCount] bigint NULL,");
        builder.AppendLine("        [Explanation] nvarchar(max) NULL,");
        builder.AppendLine("        [FindingExplanation] nvarchar(max) NULL,");
        builder.AppendLine("        [EvidenceSummary] nvarchar(max) NULL,");
        builder.AppendLine("        [EvidenceOccurrenceCount] bigint NULL,");
        builder.AppendLine("        [OutlierOccurrenceCount] bigint NULL,");
        builder.AppendLine("        [EvidenceTransformCount] bigint NULL,");
        builder.AppendLine("        [OutlierTransformCount] bigint NULL,");
        builder.AppendLine("        [EvidenceConsensusRatio] decimal(18,6) NULL,");
        builder.AppendLine("        [DominantConsensusRatio] decimal(18,6) NULL,");
        builder.AppendLine("        [EvidenceOutlierRatio] decimal(18,6) NULL,");
        builder.AppendLine("        [OutlierRatio] decimal(18,6) NULL,");
        builder.AppendLine("        [EvidenceQuality] nvarchar(16) NULL,");
        builder.AppendLine("        [ConfidenceBand] nvarchar(16) NULL,");
        builder.AppendLine("        [ConfidenceReason] nvarchar(max) NULL,");
        builder.AppendLine("        [EvidenceDiversitySummary] nvarchar(max) NULL,");
        builder.AppendLine("        [ConfidenceSummary] nvarchar(max) NULL,");
        builder.AppendLine("        [DistinctTransformCount] bigint NULL,");
        builder.AppendLine("        [DistinctSourceTransformCount] bigint NULL,");
        builder.AppendLine("        [DistinctSourceObjectCount] bigint NULL,");
        builder.AppendLine("        [DistinctRelationshipPatternCount] bigint NULL,");
        builder.AppendLine("        [EffectiveTransformCount] bigint NULL,");
        builder.AppendLine("        [RecommendedAction] nvarchar(128) NOT NULL,");
        builder.AppendLine("        [RuntimeCountStatus] nvarchar(64) NULL,");
        builder.AppendLine("        [ReviewQuery] nvarchar(max) NULL,");
        builder.AppendLine("        [DetailQuery] nvarchar(max) NULL,");
        builder.AppendLine("        [TransformViewQuery] nvarchar(max) NULL,");
        builder.AppendLine("        [SupportingTransformQuery] nvarchar(max) NULL,");
        builder.AppendLine("        [CapturedAtUtc] datetime2(3) NOT NULL CONSTRAINT [DF_dbo_FindingLog_CapturedAtUtc] DEFAULT (SYSUTCDATETIME()),");
        builder.AppendLine("        CONSTRAINT [FK_dbo_FindingLog_RunId] FOREIGN KEY ([RunId]) REFERENCES [dbo].[RunLog]([RunId])");
        builder.AppendLine("    );");
        builder.AppendLine("END;");
        builder.AppendLine();
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''Issue'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [Issue] nvarchar(128) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''FindingTitle'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [FindingTitle] nvarchar(128) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''FindingCategory'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [FindingCategory] nvarchar(128) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''ReviewQuery'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [ReviewQuery] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''DetailQuery'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [DetailQuery] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''TransformViewQuery'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [TransformViewQuery] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''SupportingTransformQuery'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [SupportingTransformQuery] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''OutputMode'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [OutputMode] nvarchar(64) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''CandidateId'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [CandidateId] nvarchar(256) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''CandidateKind'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [CandidateKind] nvarchar(128) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''RelationshipLabel'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [RelationshipLabel] nvarchar(512) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''ReferencingObject'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [ReferencingObject] nvarchar(512) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''ReferencedObject'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [ReferencedObject] nvarchar(512) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''CheckedObject'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [CheckedObject] nvarchar(512) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectSide'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [SuspectSide] nvarchar(512) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectObject'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [SuspectObject] nvarchar(512) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''LookupObject'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [LookupObject] nvarchar(512) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''RelatedObject'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [RelatedObject] nvarchar(512) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''CorpusRelationship'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [CorpusRelationship] nvarchar(512) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''CorpusRelationshipPattern'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [CorpusRelationshipPattern] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''DominantPattern'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [DominantPattern] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierPattern'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [OutlierPattern] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''ResultRowCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [ResultRowCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''FindingGroupCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [FindingGroupCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectRowCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [SuspectRowCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''Explanation'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [Explanation] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''FindingExplanation'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [FindingExplanation] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceSummary'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceSummary] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceOccurrenceCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceOccurrenceCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierOccurrenceCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [OutlierOccurrenceCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceTransformCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceTransformCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierTransformCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [OutlierTransformCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceConsensusRatio'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceConsensusRatio] decimal(18,6) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''DominantConsensusRatio'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [DominantConsensusRatio] decimal(18,6) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceOutlierRatio'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceOutlierRatio] decimal(18,6) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierRatio'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [OutlierRatio] decimal(18,6) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceQuality'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceQuality] nvarchar(16) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceBand'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceBand] nvarchar(16) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceReason'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceReason] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceDiversitySummary'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceDiversitySummary] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceSummary'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceSummary] nvarchar(max) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctTransformCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [DistinctTransformCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctSourceTransformCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [DistinctSourceTransformCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctSourceObjectCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [DistinctSourceObjectCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctRelationshipPatternCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [DistinctRelationshipPatternCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''EffectiveTransformCount'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [EffectiveTransformCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''RuntimeCountStatus'') IS NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ADD [RuntimeCountStatus] nvarchar(64) NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''RowsReturned'') IS NOT NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ALTER COLUMN [RowsReturned] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine("IF COL_LENGTH(N''dbo.FindingLog'', N''TotalSuspectCount'') IS NOT NULL");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    ALTER TABLE [dbo].[FindingLog] ALTER COLUMN [TotalSuspectCount] bigint NULL;");
        builder.AppendLine("END;");
        builder.AppendLine();
        builder.AppendLine("IF NOT EXISTS");
        builder.AppendLine("(");
        builder.AppendLine("    SELECT 1");
        builder.AppendLine("    FROM sys.indexes");
        builder.AppendLine("    WHERE [name] = N''IX_dbo_FindingLog_RunId''");
        builder.AppendLine("      AND [object_id] = OBJECT_ID(N''[dbo].[FindingLog]'')");
        builder.AppendLine(")");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    CREATE INDEX [IX_dbo_FindingLog_RunId] ON [dbo].[FindingLog]([RunId]);");
        builder.AppendLine("END;");
        builder.AppendLine("';");
        builder.AppendLine("GO");
        builder.AppendLine();
        builder.AppendLine(RenderMetaDqRunProcedureSql());
        builder.AppendLine();
        builder.AppendLine(RenderMetaDqFindingsProcedureSql());
        return builder.ToString();
    }

    private static string RenderMetaDqRunProcedureSql()
    {
        var builder = new StringBuilder();
        builder.AppendLine("EXEC [MetaDQ].sys.sp_executesql N'");
        builder.AppendLine("CREATE OR ALTER PROCEDURE [dbo].[Run]");
        builder.AppendLine("    @SourceDatabaseName sysname,");
        builder.AppendLine("    @RunId bigint OUTPUT");
        builder.AppendLine("AS");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    SET NOCOUNT ON;");
        builder.AppendLine("    SET XACT_ABORT ON;");
        builder.AppendLine();
        builder.AppendLine("    IF NULLIF(LTRIM(RTRIM(@SourceDatabaseName)), N'''') IS NULL");
        builder.AppendLine("    BEGIN");
        builder.AppendLine("        THROW 51000, N''@SourceDatabaseName is required.'', 1;");
        builder.AppendLine("    END;");
        builder.AppendLine();
        builder.AppendLine("    IF DB_ID(@SourceDatabaseName) IS NULL");
        builder.AppendLine("    BEGIN");
        builder.AppendLine("        THROW 51000, N''Source database was not found.'', 1;");
        builder.AppendLine("    END;");
        builder.AppendLine();
        builder.AppendLine("    DECLARE @runIdLocal bigint;");
        builder.AppendLine("    INSERT INTO [dbo].[RunLog] ([SourceDatabaseName], [Status])");
        builder.AppendLine("    VALUES (@SourceDatabaseName, N''Running'');");
        builder.AppendLine("    SET @runIdLocal = SCOPE_IDENTITY();");
        builder.AppendLine("    SET @RunId = @runIdLocal;");
        builder.AppendLine();
        builder.AppendLine("    BEGIN TRY");
        builder.AppendLine("        DECLARE @sql nvarchar(max) =");
        builder.AppendLine("            N''INSERT INTO [dbo].[FindingLog] ([RunId], [DQView], [Issue], [FindingTitle], [FindingCategory], [OutputMode], [CandidateId], [CandidateKind], [Relationship], [RelationshipLabel], [ReferencingObject], [ReferencedObject], [CheckedObject], [SuspectSide], [SuspectObject], [LookupObject], [RelatedObject], [CorpusRelationship], [CorpusRelationshipPattern], [DominantPattern], [OutlierPattern], [TransformViews], [GeneratedView], [RowsReturned], [ResultRowCount], [FindingGroupCount], [TotalSuspectCount], [SuspectRowCount], [Explanation], [FindingExplanation], [EvidenceSummary], [EvidenceOccurrenceCount], [OutlierOccurrenceCount], [EvidenceTransformCount], [OutlierTransformCount], [EvidenceConsensusRatio], [DominantConsensusRatio], [EvidenceOutlierRatio], [OutlierRatio], [EvidenceQuality], [ConfidenceBand], [ConfidenceReason], [EvidenceDiversitySummary], [ConfidenceSummary], [DistinctTransformCount], [DistinctSourceTransformCount], [DistinctSourceObjectCount], [DistinctRelationshipPatternCount], [EffectiveTransformCount], [RecommendedAction], [RuntimeCountStatus], [ReviewQuery], [DetailQuery], [TransformViewQuery], [SupportingTransformQuery]) '' +");
        builder.AppendLine("            N''SELECT @RunId, [DQView], [Issue], [FindingTitle], [FindingCategory], [OutputMode], [CandidateId], [CandidateKind], [Relationship], [RelationshipLabel], [ReferencingObject], [ReferencedObject], [CheckedObject], [SuspectSide], [SuspectObject], [LookupObject], [RelatedObject], [CorpusRelationship], [CorpusRelationshipPattern], [DominantPattern], [OutlierPattern], [TransformViews], [GeneratedView], [RowsReturned], [ResultRowCount], [FindingGroupCount], [TotalSuspectCount], [SuspectRowCount], [Explanation], [FindingExplanation], [EvidenceSummary], [EvidenceOccurrenceCount], [OutlierOccurrenceCount], [EvidenceTransformCount], [OutlierTransformCount], [EvidenceConsensusRatio], [DominantConsensusRatio], [EvidenceOutlierRatio], [OutlierRatio], [EvidenceQuality], [ConfidenceBand], [ConfidenceReason], [EvidenceDiversitySummary], [ConfidenceSummary], [DistinctTransformCount], [DistinctSourceTransformCount], [DistinctSourceObjectCount], [DistinctRelationshipPatternCount], [EffectiveTransformCount], [RecommendedAction], [RuntimeCountStatus], [ReviewQuery], [DetailQuery], [TransformViewQuery], [SupportingTransformQuery] '' +");
        builder.AppendLine("            N''FROM '' + QUOTENAME(@SourceDatabaseName) + N''.[dq].[v_DataQualityReview];'';");
        builder.AppendLine();
        builder.AppendLine("        EXEC sys.sp_executesql");
        builder.AppendLine("            @sql,");
        builder.AppendLine("            N''@RunId bigint'',");
        builder.AppendLine("            @RunId = @runIdLocal;");
        builder.AppendLine();
        builder.AppendLine("        UPDATE [dbo].[RunLog]");
        builder.AppendLine("        SET [CompletedAtUtc] = SYSUTCDATETIME(),");
        builder.AppendLine("            [Status] = N''Completed''");
        builder.AppendLine("        WHERE [RunId] = @runIdLocal;");
        builder.AppendLine("    END TRY");
        builder.AppendLine("    BEGIN CATCH");
        builder.AppendLine("        UPDATE [dbo].[RunLog]");
        builder.AppendLine("        SET [CompletedAtUtc] = SYSUTCDATETIME(),");
        builder.AppendLine("            [Status] = N''Failed'',");
        builder.AppendLine("            [ErrorMessage] = ERROR_MESSAGE()");
        builder.AppendLine("        WHERE [RunId] = @runIdLocal;");
        builder.AppendLine("        THROW;");
        builder.AppendLine("    END CATCH;");
        builder.AppendLine();
        builder.AppendLine("    SELECT");
        builder.AppendLine("        r.[RunId],");
        builder.AppendLine("        r.[SourceDatabaseName],");
        builder.AppendLine("        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [RowsReturned],");
        builder.AppendLine("        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [ResultRowCount],");
        builder.AppendLine("        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [FindingGroupCount],");
        builder.AppendLine("        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [TotalSuspectCount],");
        builder.AppendLine("        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [SuspectRowCount],");
        builder.AppendLine("        COUNT(f.[FindingId]) AS [ChecksExecuted],");
        builder.AppendLine("        COUNT(f.[FindingId]) AS [FindingsExecuted],");
        builder.AppendLine("        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [RuntimeFindingGroupCount],");
        builder.AppendLine("        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [RuntimeSuspectRowCount]");
        builder.AppendLine("    FROM [dbo].[RunLog] AS r");
        builder.AppendLine("    LEFT JOIN [dbo].[FindingLog] AS f");
        builder.AppendLine("      ON f.[RunId] = r.[RunId]");
        builder.AppendLine("    WHERE r.[RunId] = @runIdLocal");
        builder.AppendLine("    GROUP BY r.[RunId], r.[SourceDatabaseName];");
        builder.AppendLine();
        builder.AppendLine("    SELECT");
        builder.AppendLine("        [DQView],");
        builder.AppendLine("        [Issue],");
        builder.AppendLine("        [FindingTitle],");
        builder.AppendLine("        [FindingCategory],");
        builder.AppendLine("        [OutputMode],");
        builder.AppendLine("        [CandidateId],");
        builder.AppendLine("        [CandidateKind],");
        builder.AppendLine("        [Relationship],");
        builder.AppendLine("        [RelationshipLabel],");
        builder.AppendLine("        [ReferencingObject],");
        builder.AppendLine("        [ReferencedObject],");
        builder.AppendLine("        [CheckedObject],");
        builder.AppendLine("        [SuspectSide],");
        builder.AppendLine("        [SuspectObject],");
        builder.AppendLine("        [LookupObject],");
        builder.AppendLine("        [RelatedObject],");
        builder.AppendLine("        [CorpusRelationship],");
        builder.AppendLine("        [CorpusRelationshipPattern],");
        builder.AppendLine("        [DominantPattern],");
        builder.AppendLine("        [OutlierPattern],");
        builder.AppendLine("        [RowsReturned],");
        builder.AppendLine("        [ResultRowCount],");
        builder.AppendLine("        [FindingGroupCount],");
        builder.AppendLine("        [TotalSuspectCount],");
        builder.AppendLine("        [SuspectRowCount],");
        builder.AppendLine("        [Explanation],");
        builder.AppendLine("        [FindingExplanation],");
        builder.AppendLine("        [EvidenceSummary],");
        builder.AppendLine("        [EvidenceOccurrenceCount],");
        builder.AppendLine("        [OutlierOccurrenceCount],");
        builder.AppendLine("        [EvidenceTransformCount],");
        builder.AppendLine("        [OutlierTransformCount],");
        builder.AppendLine("        [EvidenceConsensusRatio],");
        builder.AppendLine("        [DominantConsensusRatio],");
        builder.AppendLine("        [EvidenceOutlierRatio],");
        builder.AppendLine("        [OutlierRatio],");
        builder.AppendLine("        [EvidenceQuality],");
        builder.AppendLine("        [ConfidenceBand],");
        builder.AppendLine("        [ConfidenceReason],");
        builder.AppendLine("        [EvidenceDiversitySummary],");
        builder.AppendLine("        [ConfidenceSummary],");
        builder.AppendLine("        [DistinctTransformCount],");
        builder.AppendLine("        [DistinctSourceTransformCount],");
        builder.AppendLine("        [DistinctSourceObjectCount],");
        builder.AppendLine("        [DistinctRelationshipPatternCount],");
        builder.AppendLine("        [EffectiveTransformCount],");
        builder.AppendLine("        [RecommendedAction],");
        builder.AppendLine("        [RuntimeCountStatus],");
        builder.AppendLine("        [GeneratedView],");
        builder.AppendLine("        [ReviewQuery],");
        builder.AppendLine("        [DetailQuery],");
        builder.AppendLine("        [TransformViewQuery],");
        builder.AppendLine("        [SupportingTransformQuery]");
        builder.AppendLine("    FROM [dbo].[FindingLog]");
        builder.AppendLine("    WHERE [RunId] = @runIdLocal");
        builder.AppendLine("      AND ([RowsReturned] > 0 OR [RowsReturned] IS NULL)");
        builder.AppendLine("    ORDER BY CASE WHEN [RowsReturned] IS NULL THEN 1 ELSE 0 END, [RowsReturned] DESC, [DQView] ASC;");
        builder.AppendLine("END;");
        builder.AppendLine("';");
        builder.AppendLine("GO");
        return builder.ToString();
    }

    private static string RenderMetaDqFindingsProcedureSql()
    {
        var builder = new StringBuilder();
        builder.AppendLine("EXEC [MetaDQ].sys.sp_executesql N'");
        builder.AppendLine("CREATE OR ALTER PROCEDURE [dbo].[Findings]");
        builder.AppendLine("    @RunId bigint = NULL");
        builder.AppendLine("AS");
        builder.AppendLine("BEGIN");
        builder.AppendLine("    SET NOCOUNT ON;");
        builder.AppendLine();
        builder.AppendLine("    IF @RunId IS NULL");
        builder.AppendLine("    BEGIN");
        builder.AppendLine("        SELECT TOP (1) @RunId = [RunId]");
        builder.AppendLine("        FROM [dbo].[RunLog]");
        builder.AppendLine("        ORDER BY [RunId] DESC;");
        builder.AppendLine("    END;");
        builder.AppendLine();
        builder.AppendLine("    IF @RunId IS NULL");
        builder.AppendLine("    BEGIN");
        builder.AppendLine("        SELECT");
        builder.AppendLine("            CAST(NULL AS bigint) AS [RunId],");
        builder.AppendLine("            CAST(NULL AS nvarchar(128)) AS [DQView],");
        builder.AppendLine("            CAST(NULL AS nvarchar(128)) AS [Issue],");
        builder.AppendLine("            CAST(NULL AS nvarchar(128)) AS [FindingTitle],");
        builder.AppendLine("            CAST(NULL AS nvarchar(128)) AS [FindingCategory],");
        builder.AppendLine("            CAST(NULL AS nvarchar(64)) AS [OutputMode],");
        builder.AppendLine("            CAST(NULL AS nvarchar(256)) AS [CandidateId],");
        builder.AppendLine("            CAST(NULL AS nvarchar(128)) AS [CandidateKind],");
        builder.AppendLine("            CAST(NULL AS nvarchar(512)) AS [Relationship],");
        builder.AppendLine("            CAST(NULL AS nvarchar(512)) AS [RelationshipLabel],");
        builder.AppendLine("            CAST(NULL AS nvarchar(512)) AS [ReferencingObject],");
        builder.AppendLine("            CAST(NULL AS nvarchar(512)) AS [ReferencedObject],");
        builder.AppendLine("            CAST(NULL AS nvarchar(512)) AS [CheckedObject],");
        builder.AppendLine("            CAST(NULL AS nvarchar(512)) AS [SuspectSide],");
        builder.AppendLine("            CAST(NULL AS nvarchar(512)) AS [SuspectObject],");
        builder.AppendLine("            CAST(NULL AS nvarchar(512)) AS [LookupObject],");
        builder.AppendLine("            CAST(NULL AS nvarchar(512)) AS [RelatedObject],");
        builder.AppendLine("            CAST(NULL AS nvarchar(512)) AS [CorpusRelationship],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [CorpusRelationshipPattern],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [DominantPattern],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [OutlierPattern],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [RowsReturned],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [ResultRowCount],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [FindingGroupCount],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [TotalSuspectCount],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [SuspectRowCount],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [Explanation],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [FindingExplanation],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [EvidenceSummary],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [OutlierOccurrenceCount],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [EvidenceTransformCount],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [OutlierTransformCount],");
        builder.AppendLine("            CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],");
        builder.AppendLine("            CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],");
        builder.AppendLine("            CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],");
        builder.AppendLine("            CAST(NULL AS decimal(18,6)) AS [OutlierRatio],");
        builder.AppendLine("            CAST(NULL AS nvarchar(16)) AS [EvidenceQuality],");
        builder.AppendLine("            CAST(NULL AS nvarchar(16)) AS [ConfidenceBand],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [ConfidenceReason],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [EvidenceDiversitySummary],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [ConfidenceSummary],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [DistinctTransformCount],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [DistinctSourceTransformCount],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [DistinctSourceObjectCount],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],");
        builder.AppendLine("            CAST(NULL AS bigint) AS [EffectiveTransformCount],");
        builder.AppendLine("            CAST(NULL AS nvarchar(128)) AS [RecommendedAction],");
        builder.AppendLine("            CAST(NULL AS nvarchar(64)) AS [RuntimeCountStatus],");
        builder.AppendLine("            CAST(NULL AS nvarchar(512)) AS [GeneratedView],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [ReviewQuery],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [DetailQuery],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [TransformViewQuery],");
        builder.AppendLine("            CAST(NULL AS nvarchar(max)) AS [SupportingTransformQuery]");
        builder.AppendLine("        WHERE 1 = 0;");
        builder.AppendLine("        RETURN;");
        builder.AppendLine("    END;");
        builder.AppendLine();
        builder.AppendLine("    SELECT");
        builder.AppendLine("        [RunId],");
        builder.AppendLine("        [DQView],");
        builder.AppendLine("        [Issue],");
        builder.AppendLine("        [FindingTitle],");
        builder.AppendLine("        [FindingCategory],");
        builder.AppendLine("        [OutputMode],");
        builder.AppendLine("        [CandidateId],");
        builder.AppendLine("        [CandidateKind],");
        builder.AppendLine("        [Relationship],");
        builder.AppendLine("        [RelationshipLabel],");
        builder.AppendLine("        [ReferencingObject],");
        builder.AppendLine("        [ReferencedObject],");
        builder.AppendLine("        [CheckedObject],");
        builder.AppendLine("        [SuspectSide],");
        builder.AppendLine("        [SuspectObject],");
        builder.AppendLine("        [LookupObject],");
        builder.AppendLine("        [RelatedObject],");
        builder.AppendLine("        [CorpusRelationship],");
        builder.AppendLine("        [CorpusRelationshipPattern],");
        builder.AppendLine("        [DominantPattern],");
        builder.AppendLine("        [OutlierPattern],");
        builder.AppendLine("        [RowsReturned],");
        builder.AppendLine("        [ResultRowCount],");
        builder.AppendLine("        [FindingGroupCount],");
        builder.AppendLine("        [TotalSuspectCount],");
        builder.AppendLine("        [SuspectRowCount],");
        builder.AppendLine("        [Explanation],");
        builder.AppendLine("        [FindingExplanation],");
        builder.AppendLine("        [EvidenceSummary],");
        builder.AppendLine("        [EvidenceOccurrenceCount],");
        builder.AppendLine("        [OutlierOccurrenceCount],");
        builder.AppendLine("        [EvidenceTransformCount],");
        builder.AppendLine("        [OutlierTransformCount],");
        builder.AppendLine("        [EvidenceConsensusRatio],");
        builder.AppendLine("        [DominantConsensusRatio],");
        builder.AppendLine("        [EvidenceOutlierRatio],");
        builder.AppendLine("        [OutlierRatio],");
        builder.AppendLine("        [EvidenceQuality],");
        builder.AppendLine("        [ConfidenceBand],");
        builder.AppendLine("        [ConfidenceReason],");
        builder.AppendLine("        [EvidenceDiversitySummary],");
        builder.AppendLine("        [ConfidenceSummary],");
        builder.AppendLine("        [DistinctTransformCount],");
        builder.AppendLine("        [DistinctSourceTransformCount],");
        builder.AppendLine("        [DistinctSourceObjectCount],");
        builder.AppendLine("        [DistinctRelationshipPatternCount],");
        builder.AppendLine("        [EffectiveTransformCount],");
        builder.AppendLine("        [RecommendedAction],");
        builder.AppendLine("        [RuntimeCountStatus],");
        builder.AppendLine("        [GeneratedView],");
        builder.AppendLine("        [ReviewQuery],");
        builder.AppendLine("        [DetailQuery],");
        builder.AppendLine("        [TransformViewQuery],");
        builder.AppendLine("        [SupportingTransformQuery]");
        builder.AppendLine("    FROM [dbo].[FindingLog]");
        builder.AppendLine("    WHERE [RunId] = @RunId");
        builder.AppendLine("      AND ([RowsReturned] > 0 OR [RowsReturned] IS NULL)");
        builder.AppendLine("    ORDER BY CASE WHEN [RowsReturned] IS NULL THEN 1 ELSE 0 END, [RowsReturned] DESC, [DQView] ASC;");
        builder.AppendLine("END;");
        builder.AppendLine("';");
        builder.AppendLine("GO");
        return builder.ToString();
    }

    private static string RenderDashboardViewSql(
        IReadOnlyList<DataQualityCandidate> candidates,
        IReadOnlyDictionary<string, string> candidateTypes,
        bool includeSchemaGuard,
        IReadOnlyDictionary<string, DataQualityCandidateEvidence[]> candidateEvidenceByCandidateId,
        IReadOnlyDictionary<string, CorpusRelationship> corpusRelationshipById,
        IReadOnlyDictionary<string, CorpusRelationshipPattern> corpusRelationshipPatternById,
        IReadOnlyDictionary<string, PatternComparison> comparisonPatternByCandidateId,
        IReadOnlyDictionary<string, string> optionalityPatternByCandidateId,
        IReadOnlyDictionary<string, string[]> candidateJoinPatternIds,
        IReadOnlyDictionary<string, JoinPattern> joinPatternById,
        IReadOnlyDictionary<string, JoinPatternOccurrence[]> occurrencesByPatternId,
        IReadOnlyDictionary<string, JoinPatternOccurrenceBaseTable[]> baseTablesByOccurrenceId,
        IReadOnlyDictionary<string, JoinPatternKeyPart[]> keyPartsByPatternId)
    {
        var builder = new StringBuilder();
        if (includeSchemaGuard)
        {
            builder.AppendLine("IF SCHEMA_ID(N'dq') IS NULL EXEC(N'CREATE SCHEMA [dq]');");
            builder.AppendLine("GO");
            builder.AppendLine();
        }

        builder.AppendLine("/* MetaDataQuality: Review dashboard */");
        builder.AppendLine("CREATE OR ALTER VIEW [dq].[v_DataQualityReview]");
        builder.AppendLine("AS");

        if (candidates.Count == 0)
        {
            builder.Append(RenderEmptyDashboardSelect());
            builder.AppendLine();
            builder.AppendLine("GO");
            return builder.ToString();
        }

        for (var i = 0; i < candidates.Count; i++)
        {
            var candidate = candidates[i];
            var candidateType = ResolveCandidateType(candidateTypes, candidate.Id);
            var outputMode = ResolveCandidateSqlOutputMode(candidateType);
            if (outputMode == CandidateSqlOutputMode.Unsupported)
            {
                throw new InvalidOperationException(
                    $"Unsupported generated view type '{candidateType}' for data-quality-to-sql.");
            }

            var (contexts, _) = ResolveJoinContexts(
                candidate,
                candidateJoinPatternIds,
                joinPatternById,
                occurrencesByPatternId,
                baseTablesByOccurrenceId,
                keyPartsByPatternId);
            contexts = FilterContextsForImpliedCandidateTypes(candidateType, contexts);
            var viewName = contexts.Count == 1
                ? BuildViewName(candidateType, contexts[0])
                : BuildViewName(candidate, candidateType);

            builder.Append(RenderDashboardSelect(
                candidate,
                candidateType,
                outputMode,
                viewName,
                contexts,
                candidateEvidenceByCandidateId,
                corpusRelationshipById,
                corpusRelationshipPatternById,
                comparisonPatternByCandidateId,
                optionalityPatternByCandidateId));
            if (i < candidates.Count - 1)
            {
                builder.AppendLine();
                builder.AppendLine("UNION ALL");
            }
        }

        builder.AppendLine();
        builder.AppendLine("GO");
        return builder.ToString();
    }

    private static string RenderEmptyDashboardSelect()
    {
        var builder = new StringBuilder();
        builder.AppendLine("SELECT");
        builder.AppendLine("    CAST(NULL AS nvarchar(128)) AS [DQView],");
        builder.AppendLine("    CAST(NULL AS nvarchar(128)) AS [Issue],");
        builder.AppendLine("    CAST(NULL AS nvarchar(128)) AS [FindingTitle],");
        builder.AppendLine("    CAST(NULL AS nvarchar(128)) AS [FindingCategory],");
        builder.AppendLine("    CAST(NULL AS nvarchar(64)) AS [OutputMode],");
        builder.AppendLine("    CAST(NULL AS nvarchar(256)) AS [CandidateId],");
        builder.AppendLine("    CAST(NULL AS nvarchar(128)) AS [CandidateKind],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [Relationship],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [RelationshipLabel],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [CheckedObject],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [SuspectSide],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [SuspectObject],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [LookupObject],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [RelatedObject],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [CorpusRelationship],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [CorpusRelationshipPattern],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [DominantPattern],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [OutlierPattern],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [TransformViews],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [RowsReturned],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [ResultRowCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [FindingGroupCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [TotalSuspectCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [SuspectRowCount],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [Explanation],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [FindingExplanation],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [EvidenceSummary],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [EvidenceTransformCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [OutlierTransformCount],");
        builder.AppendLine("    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],");
        builder.AppendLine("    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],");
        builder.AppendLine("    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],");
        builder.AppendLine("    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],");
        builder.AppendLine("    CAST(NULL AS nvarchar(16)) AS [EvidenceQuality],");
        builder.AppendLine("    CAST(NULL AS nvarchar(16)) AS [ConfidenceBand],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [ConfidenceReason],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [EvidenceDiversitySummary],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [ConfidenceSummary],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [DistinctTransformCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [EffectiveTransformCount],");
        builder.AppendLine("    CAST(NULL AS nvarchar(128)) AS [RecommendedAction],");
        builder.AppendLine("    CAST(NULL AS nvarchar(64)) AS [RuntimeCountStatus],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [GeneratedView],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [ReviewQuery],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [DetailQuery],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [TransformViewQuery],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [SupportingTransformQuery]");
        builder.Append("WHERE 1 = 0;");
        return builder.ToString();
    }

    private static string RenderDashboardSelect(
        DataQualityCandidate candidate,
        string candidateType,
        CandidateSqlOutputMode outputMode,
        string viewName,
        IReadOnlyList<JoinRenderContext> contexts,
        IReadOnlyDictionary<string, DataQualityCandidateEvidence[]> candidateEvidenceByCandidateId,
        IReadOnlyDictionary<string, CorpusRelationship> corpusRelationshipById,
        IReadOnlyDictionary<string, CorpusRelationshipPattern> corpusRelationshipPatternById,
        IReadOnlyDictionary<string, PatternComparison> comparisonPatternByCandidateId,
        IReadOnlyDictionary<string, string> optionalityPatternByCandidateId)
    {
        return outputMode switch
        {
            CandidateSqlOutputMode.RuntimeCheck => RenderRuntimeDashboardSelect(
                candidate,
                candidateType,
                viewName,
                contexts,
                candidateEvidenceByCandidateId,
                corpusRelationshipById,
                corpusRelationshipPatternById,
                comparisonPatternByCandidateId,
                optionalityPatternByCandidateId),
            CandidateSqlOutputMode.SemanticReviewFinding => RenderSemanticDashboardSelect(
                candidate,
                candidateType,
                viewName,
                contexts),
            _ => throw new InvalidOperationException(
                $"Unsupported generated view type '{candidateType}' for data-quality-to-sql."),
        };
    }

    private static string RenderRuntimeDashboardSelect(
        DataQualityCandidate candidate,
        string candidateType,
        string viewName,
        IReadOnlyList<JoinRenderContext> contexts,
        IReadOnlyDictionary<string, DataQualityCandidateEvidence[]> candidateEvidenceByCandidateId,
        IReadOnlyDictionary<string, CorpusRelationship> corpusRelationshipById,
        IReadOnlyDictionary<string, CorpusRelationshipPattern> corpusRelationshipPatternById,
        IReadOnlyDictionary<string, PatternComparison> comparisonPatternByCandidateId,
        IReadOnlyDictionary<string, string> optionalityPatternByCandidateId)
    {
        var generatedView = $"dq.{viewName}";
        var relationship = contexts.Count == 0
            ? "(unresolved relationship)"
            : string.Join(
                "; ",
                contexts
                    .Select(static context => $"{context.LeftTable} -> {context.RightTable}")
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase));
        var transformViews = contexts.Count == 0
            ? "(unknown transform view)"
            : string.Join(
                ", ",
                contexts
                    .SelectMany(static context => context.TransformViewNames)
                    .Where(static value => !string.IsNullOrWhiteSpace(value))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase));
        if (string.IsNullOrWhiteSpace(transformViews))
        {
            transformViews = "(unknown transform view)";
        }
        var sideInfo = ResolveRuntimeSideInfo(candidateType, contexts);

        var reviewQuery = $"SELECT * FROM {QuoteMultipartIdentifier(generatedView)} ORDER BY [Relationship], [KeyValues];";
        var transformViewQuery = BuildTransformViewQuery(contexts);
        var metadata = ResolveCandidateMetadata(
            candidate,
            candidateType,
            contexts,
            candidateEvidenceByCandidateId,
            corpusRelationshipById,
            corpusRelationshipPatternById,
            comparisonPatternByCandidateId,
            optionalityPatternByCandidateId);

        var builder = new StringBuilder();
        builder.AppendLine("SELECT");
        builder.AppendLine($"    CAST({SqlStringLiteral(BuildViewNameLabel(candidateType))} AS nvarchar(128)) AS [DQView],");
        builder.AppendLine($"    CAST({SqlStringLiteral(ToIssueLabel(candidateType))} AS nvarchar(128)) AS [Issue],");
        builder.AppendLine($"    CAST({SqlStringLiteral(ToFindingTitle(candidateType))} AS nvarchar(128)) AS [FindingTitle],");
        builder.AppendLine($"    CAST({SqlStringLiteral(ToFindingCategory(candidateType))} AS nvarchar(128)) AS [FindingCategory],");
        builder.AppendLine($"    CAST({SqlStringLiteral(OutputModeRuntimeCheck)} AS nvarchar(64)) AS [OutputMode],");
        builder.AppendLine($"    CAST({SqlStringLiteral(candidate.Id)} AS nvarchar(256)) AS [CandidateId],");
        builder.AppendLine($"    CAST({SqlStringLiteral(candidateType)} AS nvarchar(128)) AS [CandidateKind],");
        builder.AppendLine($"    CAST({SqlStringLiteral(sideInfo.RelationshipText ?? relationship)} AS nvarchar(512)) AS [Relationship],");
        builder.AppendLine($"    CAST({SqlStringLiteral(sideInfo.RelationshipText ?? relationship)} AS nvarchar(512)) AS [RelationshipLabel],");
        builder.AppendLine($"    {SqlNullableStringLiteral(sideInfo.ReferencingObject, 512)} AS [ReferencingObject],");
        builder.AppendLine($"    {SqlNullableStringLiteral(sideInfo.ReferencedObject, 512)} AS [ReferencedObject],");
        builder.AppendLine($"    {SqlNullableStringLiteral(sideInfo.CheckedObject, 512)} AS [CheckedObject],");
        builder.AppendLine($"    {SqlNullableStringLiteral(sideInfo.SuspectSide, 512)} AS [SuspectSide],");
        builder.AppendLine($"    {SqlNullableStringLiteral(sideInfo.SuspectObject, 512)} AS [SuspectObject],");
        builder.AppendLine($"    {SqlNullableStringLiteral(sideInfo.LookupObject, 512)} AS [LookupObject],");
        builder.AppendLine($"    {SqlNullableStringLiteral(sideInfo.RelatedObject, 512)} AS [RelatedObject],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.CorpusRelationship)} AS nvarchar(512)) AS [CorpusRelationship],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.CorpusRelationshipPattern)} AS nvarchar(max)) AS [CorpusRelationshipPattern],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.DominantPattern)} AS nvarchar(max)) AS [DominantPattern],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.OutlierPattern)} AS nvarchar(max)) AS [OutlierPattern],");
        builder.AppendLine($"    CAST({SqlStringLiteral(transformViews)} AS nvarchar(max)) AS [TransformViews],");
        builder.AppendLine("    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],");
        builder.AppendLine("    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],");
        builder.AppendLine("    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],");
        builder.AppendLine("    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],");
        builder.AppendLine("    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.Explanation)} AS nvarchar(max)) AS [Explanation],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.Explanation)} AS nvarchar(max)) AS [FindingExplanation],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.EvidenceSummary)} AS nvarchar(max)) AS [EvidenceSummary],");
        builder.AppendLine($"    {SqlBigIntLiteral(metadata.EvidenceOccurrenceCount)} AS [EvidenceOccurrenceCount],");
        builder.AppendLine($"    {SqlBigIntLiteral(metadata.EvidenceOccurrenceCount)} AS [OutlierOccurrenceCount],");
        builder.AppendLine($"    {SqlBigIntLiteral(metadata.EvidenceTransformCount)} AS [EvidenceTransformCount],");
        builder.AppendLine($"    {SqlBigIntLiteral(metadata.EvidenceTransformCount)} AS [OutlierTransformCount],");
        builder.AppendLine($"    {SqlDecimalLiteral(metadata.EvidenceConsensusRatio)} AS [EvidenceConsensusRatio],");
        builder.AppendLine($"    {SqlDecimalLiteral(metadata.EvidenceConsensusRatio)} AS [DominantConsensusRatio],");
        builder.AppendLine($"    {SqlDecimalLiteral(metadata.EvidenceOutlierRatio)} AS [EvidenceOutlierRatio],");
        builder.AppendLine($"    {SqlDecimalLiteral(metadata.EvidenceOutlierRatio)} AS [OutlierRatio],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.EvidenceQuality)} AS nvarchar(16)) AS [EvidenceQuality],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.ConfidenceBand)} AS nvarchar(16)) AS [ConfidenceBand],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.ConfidenceReason)} AS nvarchar(max)) AS [ConfidenceReason],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.EvidenceDiversitySummary)} AS nvarchar(max)) AS [EvidenceDiversitySummary],");
        builder.AppendLine($"    CAST({SqlStringLiteral(metadata.ConfidenceSummary)} AS nvarchar(max)) AS [ConfidenceSummary],");
        builder.AppendLine($"    {SqlBigIntLiteral(metadata.DistinctTransformCount)} AS [DistinctTransformCount],");
        builder.AppendLine($"    {SqlBigIntLiteral(metadata.DistinctSourceTransformCount)} AS [DistinctSourceTransformCount],");
        builder.AppendLine($"    {SqlBigIntLiteral(metadata.DistinctSourceObjectCount)} AS [DistinctSourceObjectCount],");
        builder.AppendLine($"    {SqlBigIntLiteral(metadata.DistinctRelationshipPatternCount)} AS [DistinctRelationshipPatternCount],");
        builder.AppendLine($"    {SqlBigIntLiteral(metadata.EffectiveTransformCount)} AS [EffectiveTransformCount],");
        builder.AppendLine("    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],");
        builder.AppendLine("    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],");
        builder.AppendLine($"    CAST({SqlStringLiteral(generatedView)} AS nvarchar(512)) AS [GeneratedView],");
        builder.AppendLine($"    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN {SqlStringLiteral(reviewQuery)} ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],");
        builder.AppendLine($"    CAST({SqlStringLiteral(reviewQuery)} AS nvarchar(max)) AS [DetailQuery],");
        builder.AppendLine($"    CAST({SqlStringLiteral(transformViewQuery)} AS nvarchar(max)) AS [TransformViewQuery],");
        builder.AppendLine($"    CAST({SqlStringLiteral(transformViewQuery)} AS nvarchar(max)) AS [SupportingTransformQuery]");
        builder.Append($"FROM {QuoteMultipartIdentifier(generatedView)}");
        return builder.ToString();
    }

    private static string RenderSemanticDashboardSelect(
        DataQualityCandidate candidate,
        string candidateType,
        string viewName,
        IReadOnlyList<JoinRenderContext> contexts)
    {
        var generatedView = $"dq.{viewName}";
        var reviewQuery = $"SELECT * FROM {QuoteMultipartIdentifier(generatedView)} ORDER BY [CandidateId], [Relationship], [CorpusRelationshipPattern], [OutlierPattern];";
        var transformViewQuery = BuildTransformViewQuery(contexts);

        var builder = new StringBuilder();
        builder.AppendLine("SELECT");
        builder.AppendLine("    CAST([DQView] AS nvarchar(128)) AS [DQView],");
        builder.AppendLine("    CAST([Issue] AS nvarchar(128)) AS [Issue],");
        builder.AppendLine("    CAST([FindingTitle] AS nvarchar(128)) AS [FindingTitle],");
        builder.AppendLine("    CAST([FindingCategory] AS nvarchar(128)) AS [FindingCategory],");
        builder.AppendLine("    CAST([OutputMode] AS nvarchar(64)) AS [OutputMode],");
        builder.AppendLine("    CAST([CandidateId] AS nvarchar(256)) AS [CandidateId],");
        builder.AppendLine("    CAST([CandidateKind] AS nvarchar(128)) AS [CandidateKind],");
        builder.AppendLine("    CAST([Relationship] AS nvarchar(512)) AS [Relationship],");
        builder.AppendLine("    CAST([RelationshipLabel] AS nvarchar(512)) AS [RelationshipLabel],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [CheckedObject],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [SuspectSide],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [SuspectObject],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [LookupObject],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [RelatedObject],");
        builder.AppendLine("    CAST([CorpusRelationship] AS nvarchar(512)) AS [CorpusRelationship],");
        builder.AppendLine("    CAST([CorpusRelationshipPattern] AS nvarchar(max)) AS [CorpusRelationshipPattern],");
        builder.AppendLine("    CAST([DominantPattern] AS nvarchar(max)) AS [DominantPattern],");
        builder.AppendLine("    CAST([OutlierPattern] AS nvarchar(max)) AS [OutlierPattern],");
        builder.AppendLine("    CAST([TransformViews] AS nvarchar(max)) AS [TransformViews],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [RowsReturned],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [ResultRowCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [FindingGroupCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [TotalSuspectCount],");
        builder.AppendLine("    CAST(NULL AS bigint) AS [SuspectRowCount],");
        builder.AppendLine("    CAST([Explanation] AS nvarchar(max)) AS [Explanation],");
        builder.AppendLine("    CAST([FindingExplanation] AS nvarchar(max)) AS [FindingExplanation],");
        builder.AppendLine("    CAST([EvidenceSummary] AS nvarchar(max)) AS [EvidenceSummary],");
        builder.AppendLine("    CAST([EvidenceOccurrenceCount] AS bigint) AS [EvidenceOccurrenceCount],");
        builder.AppendLine("    CAST([EvidenceOccurrenceCount] AS bigint) AS [OutlierOccurrenceCount],");
        builder.AppendLine("    CAST([EvidenceTransformCount] AS bigint) AS [EvidenceTransformCount],");
        builder.AppendLine("    CAST([EvidenceTransformCount] AS bigint) AS [OutlierTransformCount],");
        builder.AppendLine("    CAST([EvidenceConsensusRatio] AS decimal(18,6)) AS [EvidenceConsensusRatio],");
        builder.AppendLine("    CAST([EvidenceConsensusRatio] AS decimal(18,6)) AS [DominantConsensusRatio],");
        builder.AppendLine("    CAST([EvidenceOutlierRatio] AS decimal(18,6)) AS [EvidenceOutlierRatio],");
        builder.AppendLine("    CAST([EvidenceOutlierRatio] AS decimal(18,6)) AS [OutlierRatio],");
        builder.AppendLine("    CAST([EvidenceQuality] AS nvarchar(16)) AS [EvidenceQuality],");
        builder.AppendLine("    CAST([ConfidenceBand] AS nvarchar(16)) AS [ConfidenceBand],");
        builder.AppendLine("    CAST([ConfidenceReason] AS nvarchar(max)) AS [ConfidenceReason],");
        builder.AppendLine("    CAST([EvidenceDiversitySummary] AS nvarchar(max)) AS [EvidenceDiversitySummary],");
        builder.AppendLine("    CAST([ConfidenceSummary] AS nvarchar(max)) AS [ConfidenceSummary],");
        builder.AppendLine("    CAST([DistinctTransformCount] AS bigint) AS [DistinctTransformCount],");
        builder.AppendLine("    CAST([DistinctSourceTransformCount] AS bigint) AS [DistinctSourceTransformCount],");
        builder.AppendLine("    CAST([DistinctSourceObjectCount] AS bigint) AS [DistinctSourceObjectCount],");
        builder.AppendLine("    CAST([DistinctRelationshipPatternCount] AS bigint) AS [DistinctRelationshipPatternCount],");
        builder.AppendLine("    CAST([EffectiveTransformCount] AS bigint) AS [EffectiveTransformCount],");
        builder.AppendLine("    CAST(N'Review semantic finding' AS nvarchar(128)) AS [RecommendedAction],");
        builder.AppendLine("    CAST(N'Non-runtime (semantic review)' AS nvarchar(64)) AS [RuntimeCountStatus],");
        builder.AppendLine($"    CAST({SqlStringLiteral(generatedView)} AS nvarchar(512)) AS [GeneratedView],");
        builder.AppendLine($"    CAST({SqlStringLiteral(reviewQuery)} AS nvarchar(max)) AS [ReviewQuery],");
        builder.AppendLine($"    CAST({SqlStringLiteral(reviewQuery)} AS nvarchar(max)) AS [DetailQuery],");
        builder.AppendLine($"    CAST({SqlStringLiteral(transformViewQuery)} AS nvarchar(max)) AS [TransformViewQuery],");
        builder.AppendLine($"    CAST({SqlStringLiteral(transformViewQuery)} AS nvarchar(max)) AS [SupportingTransformQuery]");
        builder.Append($"FROM {QuoteMultipartIdentifier(generatedView)}");
        return builder.ToString();
    }

    private static RuntimeSideInfo ResolveRuntimeSideInfo(
        string candidateType,
        IReadOnlyList<JoinRenderContext> contexts)
    {
        if (contexts.Count == 0)
        {
            return RuntimeSideInfo.Empty;
        }

        if (string.Equals(candidateType, CandidateKinds.ImpliedForeignKeyMissingReference, StringComparison.Ordinal))
        {
            var referencing = JoinDistinctObjectNames(contexts.Select(static item => item.RightTable));
            var referenced = JoinDistinctObjectNames(contexts.Select(static item => item.LeftTable));
            return new RuntimeSideInfo(
                BuildDirectedRelationshipPhrase(
                    contexts,
                    static context => $"{context.RightTable} references {context.LeftTable}"),
                referencing,
                referenced,
                referencing,
                referencing,
                referencing,
                referenced,
                referenced);
        }

        if (string.Equals(candidateType, CandidateKinds.ImpliedUniqueKeyViolation, StringComparison.Ordinal))
        {
            var checkedObject = JoinDistinctObjectNames(contexts.Select(static item => item.LeftTable));
            var relatedObject = JoinDistinctObjectNames(contexts.Select(static item => item.RightTable));
            return new RuntimeSideInfo(
                BuildDirectedRelationshipPhrase(
                    contexts,
                    static context => $"{context.LeftTable} expected unique for {context.RightTable} relationship"),
                null,
                null,
                checkedObject,
                checkedObject,
                checkedObject,
                checkedObject,
                relatedObject);
        }

        return RuntimeSideInfo.Empty;
    }

    private static string BuildDirectedRelationshipPhrase(
        IReadOnlyList<JoinRenderContext> contexts,
        Func<JoinRenderContext, string> formatter)
    {
        return string.Join(
            "; ",
            contexts
                .Select(formatter)
                .Where(static value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase));
    }

    private static string JoinDistinctObjectNames(IEnumerable<string> values)
    {
        var names = values
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return names.Length == 0
            ? string.Empty
            : string.Join("; ", names);
    }

    private static string RenderSingleViewSql(
        DataQualityCandidate candidate,
        string candidateType,
        bool includeSchemaGuard,
        IReadOnlyDictionary<string, DataQualityCandidateEvidence[]> candidateEvidenceByCandidateId,
        IReadOnlyDictionary<string, CorpusRelationship> corpusRelationshipById,
        IReadOnlyDictionary<string, CorpusRelationshipPattern> corpusRelationshipPatternById,
        IReadOnlyDictionary<string, PatternComparison> comparisonPatternByCandidateId,
        IReadOnlyDictionary<string, string> optionalityPatternByCandidateId,
        IReadOnlyDictionary<string, string[]> candidateJoinPatternIds,
        IReadOnlyDictionary<string, JoinPattern> joinPatternById,
        IReadOnlyDictionary<string, JoinPatternOccurrence[]> occurrencesByPatternId,
        IReadOnlyDictionary<string, JoinPatternOccurrenceBaseTable[]> baseTablesByOccurrenceId,
        IReadOnlyDictionary<string, JoinPatternKeyPart[]> keyPartsByPatternId)
    {
        var outputMode = ResolveCandidateSqlOutputMode(candidateType);
        if (outputMode == CandidateSqlOutputMode.Unsupported)
        {
            throw new InvalidOperationException(
                $"Unsupported generated view type '{candidateType}' for data-quality-to-sql.");
        }

        var (contexts, warnings) = ResolveJoinContexts(
            candidate,
            candidateJoinPatternIds,
            joinPatternById,
            occurrencesByPatternId,
            baseTablesByOccurrenceId,
            keyPartsByPatternId);
        contexts = FilterContextsForImpliedCandidateTypes(candidateType, contexts);
        var viewName = contexts.Count == 1
            ? BuildViewName(candidateType, contexts[0])
            : BuildViewName(candidate, candidateType);

        var builder = new StringBuilder();
        if (includeSchemaGuard)
        {
            builder.AppendLine("IF SCHEMA_ID(N'dq') IS NULL EXEC(N'CREATE SCHEMA [dq]');");
            builder.AppendLine("GO");
            builder.AppendLine();
        }

        builder.AppendLine($"/* MetaDataQuality: {ToIssueLabel(candidateType)} */");
        foreach (var warning in warnings)
        {
            builder.AppendLine($"/* Warning: {warning} */");
        }

        builder.AppendLine($"CREATE OR ALTER VIEW [dq].[{EscapeSqlIdentifier(viewName)}]");
        builder.AppendLine("AS");
        if (outputMode == CandidateSqlOutputMode.SemanticReviewFinding)
        {
            var semanticFindings = BuildSemanticReviewFindings(
                candidate,
                candidateType,
                contexts,
                candidateEvidenceByCandidateId,
                corpusRelationshipById,
                corpusRelationshipPatternById,
                comparisonPatternByCandidateId,
                optionalityPatternByCandidateId);
            for (var i = 0; i < semanticFindings.Count; i++)
            {
                builder.Append(RenderSemanticReviewFindingSelect(semanticFindings[i]));
                if (i < semanticFindings.Count - 1)
                {
                    builder.AppendLine();
                    builder.AppendLine("UNION ALL");
                }
            }
        }
        else if (contexts.Count == 0)
        {
            builder.AppendLine(RenderEmptySelect(candidateType, "No renderable join relationship was found for this generated view."));
        }
        else
        {
            for (var i = 0; i < contexts.Count; i++)
            {
                builder.Append(RenderCheckSelect(candidateType, contexts[i]));
                if (i < contexts.Count - 1)
                {
                    builder.AppendLine();
                    builder.AppendLine("UNION ALL");
                }
            }
        }

        builder.AppendLine();
        builder.AppendLine("GO");
        return builder.ToString();
    }

    private static (List<JoinRenderContext> Contexts, List<string> Warnings) ResolveJoinContexts(
        DataQualityCandidate candidate,
        IReadOnlyDictionary<string, string[]> candidateJoinPatternIds,
        IReadOnlyDictionary<string, JoinPattern> joinPatternById,
        IReadOnlyDictionary<string, JoinPatternOccurrence[]> occurrencesByPatternId,
        IReadOnlyDictionary<string, JoinPatternOccurrenceBaseTable[]> baseTablesByOccurrenceId,
        IReadOnlyDictionary<string, JoinPatternKeyPart[]> keyPartsByPatternId)
    {
        var contexts = new List<JoinRenderContext>();
        var warnings = new List<string>();
        if (!candidateJoinPatternIds.TryGetValue(candidate.Id, out var patternIds) || patternIds.Length == 0)
        {
            warnings.Add("The generated view is not linked to a join relationship.");
            return (contexts, warnings);
        }

        foreach (var patternId in patternIds)
        {
            if (!joinPatternById.TryGetValue(patternId, out var pattern))
            {
                warnings.Add($"Join relationship '{patternId}' was not found.");
                continue;
            }

            var occurrences = occurrencesByPatternId.TryGetValue(patternId, out var rows)
                ? rows
                : [];
            var anchor = occurrences
                .OrderBy(static item => item.TransformScriptName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(static item => item.QualifiedJoinId, StringComparer.Ordinal)
                .FirstOrDefault();
            if (anchor == null)
            {
                warnings.Add($"Join relationship '{patternId}' has no occurrence to render.");
                continue;
            }

            var leftTables = ResolveSideTableNames(baseTablesByOccurrenceId, anchor.Id, anchor.FirstTableReferenceId);
            var rightTables = ResolveSideTableNames(baseTablesByOccurrenceId, anchor.Id, anchor.SecondTableReferenceId);
            if (leftTables.Length != 1 || rightTables.Length != 1)
            {
                warnings.Add($"Join relationship '{patternId}' resolves to {leftTables.Length} left table(s) and {rightTables.Length} right table(s); exactly one of each is required for SQL generation.");
                continue;
            }

            if (!TryResolveJoinKeyParts(keyPartsByPatternId, patternId, out var keyParts, out var keyError))
            {
                warnings.Add($"Join relationship '{patternId}' cannot be rendered: {keyError}");
                continue;
            }

            contexts.Add(new JoinRenderContext(
                pattern.Id,
                pattern.QualifiedJoinType ?? string.Empty,
                leftTables[0],
                rightTables[0],
                keyParts,
                occurrences.Length,
                occurrences
                    .Select(static item => item.TransformScriptName)
                    .Where(static value => !string.IsNullOrWhiteSpace(value))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
                    .ToArray()));
        }

        return (contexts, warnings);
    }

    private static IReadOnlyDictionary<string, PatternComparison> BuildComparisonPatternByCandidateId(
        MetaDataQualityModel model)
    {
        var map = new Dictionary<string, PatternComparison>(StringComparer.Ordinal);
        foreach (var row in model.MinorityJoinPatternList)
        {
            map[row.DataQualityCandidate.Id] = new PatternComparison(
                row.DominantPattern.Id,
                row.OutlierPattern.Id);
        }

        foreach (var row in model.IncompleteCompositeJoinList)
        {
            map[row.DataQualityCandidate.Id] = new PatternComparison(
                row.DominantPattern.Id,
                row.OutlierPattern.Id);
        }

        foreach (var row in model.SuspiciousExtraJoinPredicateList)
        {
            map[row.DataQualityCandidate.Id] = new PatternComparison(
                row.DominantPattern.Id,
                row.OutlierPattern.Id);
        }

        return map;
    }

    private static IReadOnlyDictionary<string, string> BuildOptionalityPatternByCandidateId(
        MetaDataQualityModel model)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var row in model.InnerJoinAgainstUsuallyOptionalRelationshipList)
        {
            map[row.DataQualityCandidate.Id] = row.CorpusRelationshipPattern.Id;
        }

        foreach (var row in model.LeftJoinAgainstUsuallyMandatoryRelationshipList)
        {
            map[row.DataQualityCandidate.Id] = row.CorpusRelationshipPattern.Id;
        }

        return map;
    }

    private static CandidateMetadata ResolveCandidateMetadata(
        DataQualityCandidate candidate,
        string candidateType,
        IReadOnlyList<JoinRenderContext> contexts,
        IReadOnlyDictionary<string, DataQualityCandidateEvidence[]> candidateEvidenceByCandidateId,
        IReadOnlyDictionary<string, CorpusRelationship> corpusRelationshipById,
        IReadOnlyDictionary<string, CorpusRelationshipPattern> corpusRelationshipPatternById,
        IReadOnlyDictionary<string, PatternComparison> comparisonPatternByCandidateId,
        IReadOnlyDictionary<string, string> optionalityPatternByCandidateId)
    {
        var findings = BuildSemanticReviewFindings(
            candidate,
            candidateType,
            contexts,
            candidateEvidenceByCandidateId,
            corpusRelationshipById,
            corpusRelationshipPatternById,
            comparisonPatternByCandidateId,
            optionalityPatternByCandidateId);
        var first = findings[0];
        var evidenceSummary = BuildEvidenceSummary(
            candidateType,
            first.EvidenceOccurrenceCount,
            first.EvidenceTransformCount,
            first.EvidenceConsensusRatio,
            first.EvidenceOutlierRatio,
            first.ConfidenceBand,
            first.EvidenceDiversitySummary);
        return new CandidateMetadata(
            first.Explanation,
            evidenceSummary,
            first.EvidenceOccurrenceCount,
            first.EvidenceTransformCount,
            first.DistinctTransformCount,
            first.DistinctSourceTransformCount,
            first.DistinctSourceObjectCount,
            first.DistinctRelationshipPatternCount,
            first.EffectiveTransformCount,
            first.EvidenceConsensusRatio,
            first.EvidenceOutlierRatio,
            first.EvidenceQuality,
            first.ConfidenceBand,
            first.ConfidenceReason,
            first.EvidenceDiversitySummary,
            first.ConfidenceSummary,
            first.CorpusRelationship,
            first.CorpusRelationshipPattern,
            first.DominantPattern,
            first.OutlierPattern);
    }

    private static List<SemanticReviewFinding> BuildSemanticReviewFindings(
        DataQualityCandidate candidate,
        string candidateType,
        IReadOnlyList<JoinRenderContext> contexts,
        IReadOnlyDictionary<string, DataQualityCandidateEvidence[]> candidateEvidenceByCandidateId,
        IReadOnlyDictionary<string, CorpusRelationship> corpusRelationshipById,
        IReadOnlyDictionary<string, CorpusRelationshipPattern> corpusRelationshipPatternById,
        IReadOnlyDictionary<string, PatternComparison> comparisonPatternByCandidateId,
        IReadOnlyDictionary<string, string> optionalityPatternByCandidateId)
    {
        var transformViews = contexts.Count == 0
            ? "(unknown transform view)"
            : FormatTransformViewNames(contexts.SelectMany(static item => item.TransformViewNames).ToArray());
        var relationshipFromContexts = contexts.Count == 0
            ? "(unresolved relationship)"
            : string.Join(
                "; ",
                contexts
                    .Select(static context => $"{context.LeftTable} -> {context.RightTable}")
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase));
        var optionalityPatternId = optionalityPatternByCandidateId.TryGetValue(candidate.Id, out var optionalityPattern)
            ? optionalityPattern
            : string.Empty;
        var comparisonPattern = comparisonPatternByCandidateId.TryGetValue(candidate.Id, out var comparison)
            ? comparison
            : PatternComparison.Empty;
        var dominantPatternSignature = ResolvePatternSignature(
            comparisonPattern.DominantPatternId,
            corpusRelationshipPatternById);
        var outlierPatternSignature = ResolvePatternSignature(
            comparisonPattern.OutlierPatternId,
            corpusRelationshipPatternById);

        if (!candidateEvidenceByCandidateId.TryGetValue(candidate.Id, out var evidenceRows) || evidenceRows.Length == 0)
        {
            return
            [
                new SemanticReviewFinding(
                    candidate.Id,
                    candidateType,
                    relationshipFromContexts,
                    string.Empty,
                    ResolvePatternSignature(optionalityPatternId, corpusRelationshipPatternById),
                    dominantPatternSignature,
                    outlierPatternSignature,
                    transformViews,
                    string.IsNullOrWhiteSpace(candidate.Rationale) ? $"Promoted {candidateType} candidate." : candidate.Rationale,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    "Low",
                    "Low",
                    "No corpus evidence row was found for this candidate.",
                    string.Empty,
                    "Confidence Low: no persisted corpus evidence metrics."),
            ];
        }

        var findings = new List<SemanticReviewFinding>(evidenceRows.Length);
        foreach (var evidence in evidenceRows)
        {
            var corpusRelationship = ResolveCorpusRelationshipText(
                evidence.CorpusRelationship.Id,
                corpusRelationshipById);
            var patternId = evidence.CorpusRelationshipPattern is not null
                ? evidence.CorpusRelationshipPattern.Id
                : optionalityPatternId;
            var corpusRelationshipPattern = ResolvePatternSignature(patternId, corpusRelationshipPatternById);
            if (string.IsNullOrWhiteSpace(corpusRelationship)
                && !string.IsNullOrWhiteSpace(patternId)
                && corpusRelationshipPatternById.TryGetValue(patternId, out var pattern)
                && corpusRelationshipById.TryGetValue(pattern.CorpusRelationship.Id, out var relationship))
            {
                corpusRelationship = $"{relationship.CanonicalSideAObjectName} <-> {relationship.CanonicalSideBObjectName}";
            }

            var resolvedRelationship = !string.IsNullOrWhiteSpace(corpusRelationship)
                ? corpusRelationship
                : relationshipFromContexts;
            var explanation = string.IsNullOrWhiteSpace(evidence.Explanation)
                ? (string.IsNullOrWhiteSpace(candidate.Rationale) ? $"Promoted {candidateType} candidate." : candidate.Rationale)
                : evidence.Explanation;
            var confidenceBand = string.IsNullOrWhiteSpace(evidence.ConfidenceBand) ? "Low" : evidence.ConfidenceBand;
            var confidenceReason = string.IsNullOrWhiteSpace(evidence.ConfidenceReason)
                ? "No confidence reason provided."
                : evidence.ConfidenceReason;
            var diversitySummary = string.IsNullOrWhiteSpace(evidence.EvidenceDiversitySummary)
                ? "No diversity summary provided."
                : evidence.EvidenceDiversitySummary;
            var confidenceSummary = BuildConfidenceSummary(confidenceBand, confidenceReason);

            findings.Add(new SemanticReviewFinding(
                candidate.Id,
                candidateType,
                resolvedRelationship,
                corpusRelationship,
                corpusRelationshipPattern,
                dominantPatternSignature,
                outlierPatternSignature,
                transformViews,
                explanation,
                ParseNullableInt64(evidence.OccurrenceCount),
                ParseNullableInt64(evidence.TransformCount),
                ParseNullableInt64(evidence.DistinctTransformCount),
                ParseNullableInt64(evidence.DistinctSourceTransformCount),
                ParseNullableInt64(evidence.DistinctSourceObjectCount),
                ParseNullableInt64(evidence.DistinctRelationshipPatternCount),
                ParseNullableInt64(evidence.EffectiveTransformCount),
                ParseNullableDecimal(evidence.ConsensusRatio),
                ParseNullableDecimal(evidence.OutlierRatio),
                evidence.EvidenceQuality,
                confidenceBand,
                confidenceReason,
                diversitySummary,
                confidenceSummary));
        }

        return findings;
    }

    private static string ResolveCorpusRelationshipText(
        string corpusRelationshipId,
        IReadOnlyDictionary<string, CorpusRelationship> corpusRelationshipById)
    {
        if (string.IsNullOrWhiteSpace(corpusRelationshipId)
            || !corpusRelationshipById.TryGetValue(corpusRelationshipId, out var relationship))
        {
            return string.Empty;
        }

        return $"{relationship.CanonicalSideAObjectName} <-> {relationship.CanonicalSideBObjectName}";
    }

    private static string ResolvePatternSignature(
        string patternId,
        IReadOnlyDictionary<string, CorpusRelationshipPattern> corpusRelationshipPatternById)
    {
        if (string.IsNullOrWhiteSpace(patternId)
            || !corpusRelationshipPatternById.TryGetValue(patternId, out var pattern))
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(pattern.RepresentativeDirectionalSignature))
        {
            return pattern.CanonicalKeyPartSetSignature;
        }

        return $"{pattern.CanonicalKeyPartSetSignature} | {pattern.RepresentativeDirectionalSignature}";
    }

    private static string RenderSemanticReviewFindingSelect(SemanticReviewFinding finding)
    {
        var evidenceSummary = BuildEvidenceSummary(
            finding.CandidateKind,
            finding.EvidenceOccurrenceCount,
            finding.EvidenceTransformCount,
            finding.EvidenceConsensusRatio,
            finding.EvidenceOutlierRatio,
            finding.ConfidenceBand,
            finding.EvidenceDiversitySummary);
        var builder = new StringBuilder();
        builder.AppendLine("SELECT");
        builder.AppendLine($"    CAST({SqlStringLiteral(BuildViewNameLabel(finding.CandidateKind))} AS nvarchar(128)) AS [DQView],");
        builder.AppendLine($"    CAST({SqlStringLiteral(ToIssueLabel(finding.CandidateKind))} AS nvarchar(128)) AS [Issue],");
        builder.AppendLine($"    CAST({SqlStringLiteral(ToFindingTitle(finding.CandidateKind))} AS nvarchar(128)) AS [FindingTitle],");
        builder.AppendLine($"    CAST({SqlStringLiteral(ToFindingCategory(finding.CandidateKind))} AS nvarchar(128)) AS [FindingCategory],");
        builder.AppendLine($"    CAST({SqlStringLiteral(OutputModeSemanticReviewFinding)} AS nvarchar(64)) AS [OutputMode],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.CandidateId)} AS nvarchar(256)) AS [CandidateId],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.CandidateKind)} AS nvarchar(128)) AS [CandidateKind],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.Relationship)} AS nvarchar(512)) AS [Relationship],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.Relationship)} AS nvarchar(512)) AS [RelationshipLabel],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.CorpusRelationship)} AS nvarchar(512)) AS [CorpusRelationship],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.CorpusRelationshipPattern)} AS nvarchar(max)) AS [CorpusRelationshipPattern],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.DominantPattern)} AS nvarchar(max)) AS [DominantPattern],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.OutlierPattern)} AS nvarchar(max)) AS [OutlierPattern],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.TransformViews)} AS nvarchar(max)) AS [TransformViews],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.Explanation)} AS nvarchar(max)) AS [Explanation],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.Explanation)} AS nvarchar(max)) AS [FindingExplanation],");
        builder.AppendLine($"    CAST({SqlStringLiteral(evidenceSummary)} AS nvarchar(max)) AS [EvidenceSummary],");
        builder.AppendLine($"    {SqlBigIntLiteral(finding.EvidenceOccurrenceCount)} AS [EvidenceOccurrenceCount],");
        builder.AppendLine($"    {SqlBigIntLiteral(finding.EvidenceTransformCount)} AS [EvidenceTransformCount],");
        builder.AppendLine($"    {SqlDecimalLiteral(finding.EvidenceConsensusRatio)} AS [EvidenceConsensusRatio],");
        builder.AppendLine($"    {SqlDecimalLiteral(finding.EvidenceOutlierRatio)} AS [EvidenceOutlierRatio],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.EvidenceQuality)} AS nvarchar(16)) AS [EvidenceQuality],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.ConfidenceBand)} AS nvarchar(16)) AS [ConfidenceBand],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.ConfidenceReason)} AS nvarchar(max)) AS [ConfidenceReason],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.EvidenceDiversitySummary)} AS nvarchar(max)) AS [EvidenceDiversitySummary],");
        builder.AppendLine($"    CAST({SqlStringLiteral(finding.ConfidenceSummary)} AS nvarchar(max)) AS [ConfidenceSummary],");
        builder.AppendLine($"    {SqlBigIntLiteral(finding.DistinctTransformCount)} AS [DistinctTransformCount],");
        builder.AppendLine($"    {SqlBigIntLiteral(finding.DistinctSourceTransformCount)} AS [DistinctSourceTransformCount],");
        builder.AppendLine($"    {SqlBigIntLiteral(finding.DistinctSourceObjectCount)} AS [DistinctSourceObjectCount],");
        builder.AppendLine($"    {SqlBigIntLiteral(finding.DistinctRelationshipPatternCount)} AS [DistinctRelationshipPatternCount],");
        builder.Append($"    {SqlBigIntLiteral(finding.EffectiveTransformCount)} AS [EffectiveTransformCount]");
        return builder.ToString();
    }

    private static string RenderCheckSelect(string candidateType, JoinRenderContext context)
    {
        return candidateType switch
        {
            CandidateKinds.JoinOrphan => RenderMissingReferencedRowsSelect(candidateType, context),
            CandidateKinds.OuterJoinNullExpansion => RenderUnexpectedOuterJoinNullSelect(candidateType, context),
            CandidateKinds.JoinMultiplicityExplosion => RenderRowMultiplicationSelect(candidateType, context),
            CandidateKinds.OutputDuplicateRisk => RenderDuplicateOutputRowsSelect(candidateType, context),
            CandidateKinds.ImpliedForeignKeyMissingReference => RenderMissingReferencedRowsSelect(candidateType, context),
            CandidateKinds.ImpliedUniqueKeyViolation => RenderImpliedUniqueKeyViolationSelect(candidateType, context),
            _ => throw new InvalidOperationException(
                $"Unsupported generated view type '{candidateType}' for data-quality-to-sql."),
        };
    }

    private static List<JoinRenderContext> FilterContextsForImpliedCandidateTypes(
        string candidateType,
        IReadOnlyList<JoinRenderContext> contexts)
    {
        if (!string.Equals(candidateType, CandidateKinds.ImpliedForeignKeyMissingReference, StringComparison.Ordinal)
            && !string.Equals(candidateType, CandidateKinds.ImpliedUniqueKeyViolation, StringComparison.Ordinal))
        {
            return contexts.ToList();
        }

        if (contexts.Count <= 1)
        {
            return contexts.ToList();
        }

        var dominantOrientationKey = contexts
            .GroupBy(BuildOrientationKey, StringComparer.Ordinal)
            .Select(group => new
            {
                OrientationKey = group.Key,
                TotalOccurrenceCount = group.Sum(static item => item.OccurrenceCount),
            })
            .OrderByDescending(static item => item.TotalOccurrenceCount)
            .ThenBy(static item => item.OrientationKey, StringComparer.Ordinal)
            .First()
            .OrientationKey;

        return contexts
            .Where(context => string.Equals(BuildOrientationKey(context), dominantOrientationKey, StringComparison.Ordinal))
            .ToList();
    }

    private static string BuildOrientationKey(JoinRenderContext context) =>
        $"{context.LeftTable.Trim().ToLowerInvariant()}|{context.RightTable.Trim().ToLowerInvariant()}";

    private static string RenderMissingReferencedRowsSelect(string candidateType, JoinRenderContext context)
    {
        var builder = new StringBuilder();
        builder.AppendLine("SELECT");
        AppendCommonColumns(builder, candidateType, context, context.RightTable, RenderKeyValues("dq_right", context.KeyParts.Select(static part => part.RightColumn)));
        builder.AppendLine($"FROM {QuoteMultipartIdentifier(context.RightTable)} AS [dq_right]");
        builder.AppendLine("WHERE NOT EXISTS");
        builder.AppendLine("(");
        builder.AppendLine("    SELECT 1");
        builder.AppendLine($"    FROM {QuoteMultipartIdentifier(context.LeftTable)} AS [dq_left]");
        builder.AppendLine($"    WHERE {RenderJoinCondition(context)}");
        builder.Append(")");
        return builder.ToString();
    }

    private static string RenderUnexpectedOuterJoinNullSelect(string candidateType, JoinRenderContext context)
    {
        var builder = new StringBuilder();
        builder.AppendLine("SELECT");
        AppendCommonColumns(builder, candidateType, context, context.LeftTable, RenderKeyValues("dq_left", context.KeyParts.Select(static part => part.LeftColumn)));
        builder.AppendLine($"FROM {QuoteMultipartIdentifier(context.LeftTable)} AS [dq_left]");
        builder.AppendLine("WHERE NOT EXISTS");
        builder.AppendLine("(");
        builder.AppendLine("    SELECT 1");
        builder.AppendLine($"    FROM {QuoteMultipartIdentifier(context.RightTable)} AS [dq_right]");
        builder.AppendLine($"    WHERE {RenderJoinCondition(context)}");
        builder.Append(")");
        return builder.ToString();
    }

    private static string RenderRowMultiplicationSelect(string candidateType, JoinRenderContext context)
    {
        return RenderGroupedJoinSelect(candidateType, context, "COUNT_BIG(*)");
    }

    private static string RenderDuplicateOutputRowsSelect(string candidateType, JoinRenderContext context)
    {
        return RenderGroupedJoinSelect(candidateType, context, "COUNT_BIG(*)");
    }

    private static string RenderImpliedUniqueKeyViolationSelect(string candidateType, JoinRenderContext context)
    {
        var lookupColumns = context.KeyParts
            .Select(static part => part.LeftColumn)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var builder = new StringBuilder();
        builder.AppendLine("SELECT");
        AppendCommonColumns(
            builder,
            candidateType,
            context,
            context.LeftTable,
            RenderKeyValues("dq_lookup", lookupColumns),
            "COUNT_BIG(*)");
        builder.AppendLine($"FROM {QuoteMultipartIdentifier(context.LeftTable)} AS [dq_lookup]");
        builder.AppendLine($"GROUP BY {string.Join(", ", lookupColumns.Select(column => $"[dq_lookup].{QuoteIdentifier(column)}"))}");
        builder.Append("HAVING COUNT_BIG(*) > 1");
        return builder.ToString();
    }

    private static string RenderGroupedJoinSelect(
        string candidateType,
        JoinRenderContext context,
        string countExpression)
    {
        var leftColumns = context.KeyParts.Select(static part => part.LeftColumn).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var builder = new StringBuilder();
        builder.AppendLine("SELECT");
        AppendCommonColumns(builder, candidateType, context, context.LeftTable, RenderKeyValues("dq_left", leftColumns), countExpression);
        builder.AppendLine($"FROM {QuoteMultipartIdentifier(context.LeftTable)} AS [dq_left]");
        builder.AppendLine($"INNER JOIN {QuoteMultipartIdentifier(context.RightTable)} AS [dq_right]");
        builder.AppendLine($"    ON {RenderJoinCondition(context)}");
        builder.AppendLine($"GROUP BY {string.Join(", ", leftColumns.Select(column => $"[dq_left].{QuoteIdentifier(column)}"))}");
        builder.Append("HAVING COUNT_BIG(*) > 1");
        return builder.ToString();
    }

    private static void AppendCommonColumns(
        StringBuilder builder,
        string candidateType,
        JoinRenderContext context,
        string suspectSide,
        string keyValuesExpression,
        string suspectCountExpression = "1")
    {
        builder.AppendLine($"    CAST({SqlStringLiteral(BuildViewNameLabel(candidateType))} AS nvarchar(128)) AS [DQView],");
        builder.AppendLine($"    CAST({SqlStringLiteral(ToIssueLabel(candidateType))} AS nvarchar(128)) AS [Issue],");
        builder.AppendLine($"    CAST({SqlStringLiteral(ResolveDetailRelationshipText(candidateType, context))} AS nvarchar(512)) AS [Relationship],");
        builder.AppendLine($"    CAST({SqlStringLiteral(FormatTransformViewNames(context.TransformViewNames))} AS nvarchar(max)) AS [TransformViews],");
        builder.AppendLine($"    CAST({SqlStringLiteral(suspectSide)} AS nvarchar(512)) AS [SuspectSide],");
        builder.AppendLine($"    {keyValuesExpression} AS [KeyValues],");
        builder.AppendLine($"    CAST({suspectCountExpression} AS bigint) AS [SuspectCount]");
    }

    private static string ResolveDetailRelationshipText(string candidateType, JoinRenderContext context)
    {
        if (string.Equals(candidateType, CandidateKinds.ImpliedForeignKeyMissingReference, StringComparison.Ordinal))
        {
            return $"{context.RightTable} references {context.LeftTable}";
        }

        if (string.Equals(candidateType, CandidateKinds.ImpliedUniqueKeyViolation, StringComparison.Ordinal))
        {
            return $"{context.LeftTable} expected unique for {context.RightTable} relationship";
        }

        return $"{context.LeftTable} -> {context.RightTable}";
    }

    private static string RenderEmptySelect(string candidateType, string reason)
    {
        var builder = new StringBuilder();
        builder.AppendLine("SELECT");
        builder.AppendLine($"    CAST({SqlStringLiteral(BuildViewNameLabel(candidateType))} AS nvarchar(128)) AS [DQView],");
        builder.AppendLine($"    CAST({SqlStringLiteral(ToIssueLabel(candidateType))} AS nvarchar(128)) AS [Issue],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [Relationship],");
        builder.AppendLine("    CAST(NULL AS nvarchar(max)) AS [TransformViews],");
        builder.AppendLine("    CAST(NULL AS nvarchar(512)) AS [SuspectSide],");
        builder.AppendLine($"    CAST({SqlStringLiteral(reason)} AS nvarchar(max)) AS [KeyValues],");
        builder.AppendLine("    CAST(0 AS bigint) AS [SuspectCount]");
        builder.Append("WHERE 1 = 0");
        return builder.ToString();
    }

    private static bool TryResolveJoinKeyParts(
        IReadOnlyDictionary<string, JoinPatternKeyPart[]> keyPartsByPatternId,
        string patternId,
        out IReadOnlyList<JoinKeyPart> keyParts,
        out string error)
    {
        keyParts = [];
        error = string.Empty;
        if (!keyPartsByPatternId.TryGetValue(patternId, out var rows) || rows.Length == 0)
        {
            error = "no equality key parts were captured.";
            return false;
        }

        var resolved = new List<JoinKeyPart>();
        foreach (var row in rows.OrderBy(static item => ParseOrdinalOrMax(item.Ordinal)))
        {
            if (!string.IsNullOrWhiteSpace(row.FirstJoinInputColumnName)
                && !string.IsNullOrWhiteSpace(row.SecondJoinInputColumnName))
            {
                resolved.Add(new JoinKeyPart(row.FirstJoinInputColumnName.Trim(), row.SecondJoinInputColumnName.Trim()));
                continue;
            }

            var firstExpression = string.IsNullOrWhiteSpace(row.FirstExpressionDisplay)
                ? row.FirstExpressionId
                : row.FirstExpressionDisplay;
            var secondExpression = string.IsNullOrWhiteSpace(row.SecondExpressionDisplay)
                ? row.SecondExpressionId
                : row.SecondExpressionDisplay;
            if (!TryParseColumnExpression(firstExpression, out var leftColumn)
                || !TryParseColumnExpression(secondExpression, out var rightColumn))
            {
                error = $"only simple column equality predicates can be rendered; found '{firstExpression}' = '{secondExpression}'.";
                return false;
            }

            resolved.Add(new JoinKeyPart(leftColumn, rightColumn));
        }

        keyParts = resolved;
        return true;
    }

    private static bool TryParseColumnExpression(string expression, out string columnName)
    {
        columnName = string.Empty;
        var parts = SplitMultipartIdentifier(expression);
        if (parts.Length == 0)
        {
            return false;
        }

        var lastPart = parts[^1];
        columnName = UnquoteIdentifier(lastPart);
        if (string.IsNullOrWhiteSpace(columnName))
        {
            return false;
        }

        if (IsBracketQuoted(lastPart))
        {
            return true;
        }

        return columnName.All(static ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '@' || ch == '#' || ch == '$');
    }

    private static string RenderJoinCondition(JoinRenderContext context)
    {
        return string.Join(
            " AND ",
            context.KeyParts.Select(static part =>
                $"[dq_left].{QuoteIdentifier(part.LeftColumn)} = [dq_right].{QuoteIdentifier(part.RightColumn)}"));
    }

    private static string RenderKeyValues(string tableAlias, IEnumerable<string> columns)
    {
        var parts = new List<string>();
        foreach (var column in columns.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (parts.Count > 0)
            {
                parts.Add("N'; '");
            }

            parts.Add(SqlStringLiteral($"{column}="));
            parts.Add($"COALESCE(CONVERT(nvarchar(4000), [{tableAlias}].{QuoteIdentifier(column)}), N'<NULL>')");
        }

        return parts.Count == 0
            ? "CAST(N'(no key values)' AS nvarchar(max))"
            : $"CAST(CONCAT({string.Join(", ", parts)}) AS nvarchar(max))";
    }

    private static long? ParseNullableInt64(string value)
    {
        return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }

    private static decimal? ParseNullableDecimal(string value)
    {
        return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }

    private static string SqlBigIntLiteral(long? value)
    {
        return value.HasValue
            ? value.Value.ToString(CultureInfo.InvariantCulture)
            : "CAST(NULL AS bigint)";
    }

    private static string SqlDecimalLiteral(decimal? value)
    {
        return value.HasValue
            ? $"CAST({value.Value.ToString("0.######", CultureInfo.InvariantCulture)} AS decimal(18,6))"
            : "CAST(NULL AS decimal(18,6))";
    }

    private static string ToIssueLabel(string candidateType)
    {
        return candidateType switch
        {
            CandidateKinds.JoinOrphan => "Missing referenced rows",
            CandidateKinds.OuterJoinNullExpansion => "Unexpected NULLs from outer joins",
            CandidateKinds.JoinMultiplicityExplosion => "Row multiplication",
            CandidateKinds.OutputDuplicateRisk => "Duplicate output rows",
            CandidateKinds.ImpliedForeignKeyMissingReference => "Implied missing referenced rows",
            CandidateKinds.ImpliedUniqueKeyViolation => "Implied unique-key violation",
            CandidateKinds.ImpliedJoinFanoutRisk => "Implied join fanout risk (semantic review)",
            CandidateKinds.ImpliedOutputDuplicateRisk => "Implied output duplicate risk (semantic review)",
            CandidateKinds.MinorityJoinPattern => "Minority join pattern (semantic review)",
            CandidateKinds.IncompleteCompositeJoin => "Incomplete composite join (semantic review)",
            CandidateKinds.SuspiciousExtraJoinPredicate => "Suspicious extra join predicate (semantic review)",
            CandidateKinds.MissingCommonFilter => "Missing common filter (semantic review)",
            CandidateKinds.MinorityColumnEquivalence => "Minority column equivalence (semantic review)",
            CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship => "Inner join against usually optional side (semantic review)",
            CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship => "Left join against usually mandatory side (semantic review)",
            _ => candidateType,
        };
    }

    private static string ToFindingTitle(string candidateType)
    {
        return candidateType switch
        {
            CandidateKinds.JoinOrphan => "Missing referenced rows",
            CandidateKinds.OuterJoinNullExpansion => "Unexpected NULLs from outer joins",
            CandidateKinds.JoinMultiplicityExplosion => "Join fanout risk",
            CandidateKinds.OutputDuplicateRisk => "Duplicate output rows",
            CandidateKinds.ImpliedForeignKeyMissingReference => "Missing referenced rows",
            CandidateKinds.ImpliedUniqueKeyViolation => "Duplicate lookup key",
            CandidateKinds.ImpliedJoinFanoutRisk => "Relationship fanout risk",
            CandidateKinds.ImpliedOutputDuplicateRisk => "Relationship duplicate-output risk",
            CandidateKinds.MinorityJoinPattern => "Minority join pattern",
            CandidateKinds.IncompleteCompositeJoin => "Incomplete composite join",
            CandidateKinds.SuspiciousExtraJoinPredicate => "Suspicious extra join predicate",
            CandidateKinds.MissingCommonFilter => "Missing common filter",
            CandidateKinds.MinorityColumnEquivalence => "Minority column equivalence",
            CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship => "Inner join against usually optional relationship",
            CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship => "Left join against usually mandatory relationship",
            _ => candidateType,
        };
    }

    private static string ToFindingCategory(string candidateType)
    {
        return candidateType switch
        {
            CandidateKinds.JoinOrphan => "Referential integrity",
            CandidateKinds.OuterJoinNullExpansion => "Optionality",
            CandidateKinds.JoinMultiplicityExplosion => "Join cardinality",
            CandidateKinds.OutputDuplicateRisk => "Output uniqueness",
            CandidateKinds.ImpliedForeignKeyMissingReference => "Referential integrity",
            CandidateKinds.ImpliedUniqueKeyViolation => "Uniqueness",
            CandidateKinds.ImpliedJoinFanoutRisk => "Join cardinality",
            CandidateKinds.ImpliedOutputDuplicateRisk => "Output uniqueness",
            CandidateKinds.MinorityJoinPattern => "Join pattern outlier",
            CandidateKinds.IncompleteCompositeJoin => "Composite join outlier",
            CandidateKinds.SuspiciousExtraJoinPredicate => "Join predicate outlier",
            CandidateKinds.MissingCommonFilter => "Filter consensus outlier",
            CandidateKinds.MinorityColumnEquivalence => "Column equivalence outlier",
            CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship => "Optionality drift",
            CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship => "Optionality drift",
            _ => "Data quality",
        };
    }

    private static string BuildEvidenceSummary(
        string candidateType,
        long? evidenceOccurrenceCount,
        long? evidenceTransformCount,
        decimal? evidenceConsensusRatio,
        decimal? evidenceOutlierRatio,
        string confidenceBand,
        string evidenceDiversitySummary)
    {
        var confidenceSummary = BuildConfidenceSummary(confidenceBand, evidenceDiversitySummary);
        if (string.Equals(candidateType, CandidateKinds.ImpliedForeignKeyMissingReference, StringComparison.Ordinal)
            || string.Equals(candidateType, CandidateKinds.ImpliedUniqueKeyViolation, StringComparison.Ordinal))
        {
            return $"Consensus evidence from {FormatCount(evidenceTransformCount, "transform")} and {FormatCount(evidenceOccurrenceCount, "occurrence")} (consensus {FormatRatio(evidenceConsensusRatio)}, outlier {FormatRatio(evidenceOutlierRatio)}). {confidenceSummary}";
        }

        if (string.Equals(candidateType, CandidateKinds.ImpliedJoinFanoutRisk, StringComparison.Ordinal)
            || string.Equals(candidateType, CandidateKinds.ImpliedOutputDuplicateRisk, StringComparison.Ordinal))
        {
            return $"Signal evidence from {FormatCount(evidenceTransformCount, "transform")} and {FormatCount(evidenceOccurrenceCount, "occurrence")} (relationship consensus {FormatRatio(evidenceConsensusRatio)}, unsignaled ratio {FormatRatio(evidenceOutlierRatio)}). {confidenceSummary}";
        }

        return $"Outlier evidence from {FormatCount(evidenceTransformCount, "transform")} and {FormatCount(evidenceOccurrenceCount, "occurrence")} (dominant ratio {FormatRatio(evidenceConsensusRatio)}, outlier ratio {FormatRatio(evidenceOutlierRatio)}). {confidenceSummary}";
    }

    private static string BuildConfidenceSummary(string confidenceBand, string reasonOrDiversity)
    {
        var band = string.IsNullOrWhiteSpace(confidenceBand) ? "Low" : confidenceBand;
        var tail = string.IsNullOrWhiteSpace(reasonOrDiversity) ? "No additional calibration details." : reasonOrDiversity.Trim();
        return $"Confidence {band}: {tail}";
    }

    private static string FormatCount(long? value, string noun)
    {
        if (!value.HasValue)
        {
            return $"unknown {noun}s";
        }

        return value.Value == 1
            ? "1 " + noun
            : value.Value.ToString(CultureInfo.InvariantCulture) + " " + noun + "s";
    }

    private static string FormatRatio(decimal? value)
    {
        return value.HasValue
            ? value.Value.ToString("0.####", CultureInfo.InvariantCulture)
            : "n/a";
    }

    private static string BuildViewNameLabel(string candidateType)
    {
        return candidateType switch
        {
            CandidateKinds.JoinOrphan => "Missing referenced rows",
            CandidateKinds.OuterJoinNullExpansion => "Unexpected NULLs from outer joins",
            CandidateKinds.JoinMultiplicityExplosion => "Row multiplication",
            CandidateKinds.OutputDuplicateRisk => "Duplicate output rows",
            CandidateKinds.ImpliedForeignKeyMissingReference => "Implied missing referenced rows",
            CandidateKinds.ImpliedUniqueKeyViolation => "Implied unique-key violation",
            CandidateKinds.ImpliedJoinFanoutRisk => "Semantic review finding",
            CandidateKinds.ImpliedOutputDuplicateRisk => "Semantic review finding",
            CandidateKinds.MinorityJoinPattern => "Semantic review finding",
            CandidateKinds.IncompleteCompositeJoin => "Semantic review finding",
            CandidateKinds.SuspiciousExtraJoinPredicate => "Semantic review finding",
            CandidateKinds.MissingCommonFilter => "Semantic review finding",
            CandidateKinds.MinorityColumnEquivalence => "Semantic review finding",
            CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship => "Semantic review finding",
            CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship => "Semantic review finding",
            _ => candidateType,
        };
    }

    private static string FormatTransformViewNames(IReadOnlyList<string> transformViewNames)
    {
        var values = transformViewNames
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        return values.Length == 0
            ? "(unknown transform view)"
            : string.Join(", ", values);
    }

    private static string BuildTransformViewQuery(IReadOnlyList<JoinRenderContext> contexts)
    {
        var transformViewNames = contexts
            .SelectMany(static context => context.TransformViewNames)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (transformViewNames.Length == 0)
        {
            return string.Empty;
        }

        return string.Join(
            " ",
            transformViewNames.Select(static viewName => $"SELECT TOP (100) * FROM {QuoteMultipartIdentifier(viewName)};"));
    }

    private static string BuildViewName(DataQualityCandidate candidate, string candidateType)
    {
        var slug = SanitizeSqlIdentifier($"{candidate.Name}_{candidateType}");
        if (string.IsNullOrWhiteSpace(slug))
        {
            slug = SanitizeSqlIdentifier(candidate.Id);
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            slug = "dq_candidate";
        }

        return slug.StartsWith("v_", StringComparison.OrdinalIgnoreCase)
            ? slug
            : $"v_{slug}";
    }

    private static string BuildViewName(string candidateType, JoinRenderContext context)
    {
        var slug = SanitizeSqlIdentifier($"{ToIssueLabel(candidateType)}_{context.LeftTable}_{context.RightTable}");
        if (string.IsNullOrWhiteSpace(slug))
        {
            slug = "data_quality_view";
        }

        return slug.StartsWith("v_", StringComparison.OrdinalIgnoreCase)
            ? slug
            : $"v_{slug}";
    }

    private static string SanitizeSqlIdentifier(string value)
    {
        var chars = value
            .Select(ch => char.IsLetterOrDigit(ch) || ch == '_' ? ch : '_')
            .ToArray();
        var sanitized = new string(chars).Trim('_');
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            return string.Empty;
        }

        if (char.IsDigit(sanitized[0]))
        {
            sanitized = "_" + sanitized;
        }

        if (sanitized.Length > 120)
        {
            sanitized = sanitized[..120].TrimEnd('_');
        }

        return sanitized;
    }

    private static string EscapeSqlIdentifier(string value) =>
        value.Replace("]", "]]", StringComparison.Ordinal);

    private static string QuoteIdentifier(string value) =>
        $"[{EscapeSqlIdentifier(UnquoteIdentifier(value))}]";

    private static string QuoteMultipartIdentifier(string value)
    {
        var parts = SplitMultipartIdentifier(value)
            .Select(QuoteIdentifier)
            .ToArray();
        return parts.Length == 0
            ? QuoteIdentifier(value)
            : string.Join(".", parts);
    }

    private static string[] SplitMultipartIdentifier(string value)
    {
        var parts = new List<string>();
        var start = 0;
        var inBracket = false;
        for (var i = 0; i < value.Length; i++)
        {
            var ch = value[i];
            if (ch == '[')
            {
                inBracket = true;
                continue;
            }

            if (ch == ']')
            {
                inBracket = false;
                continue;
            }

            if (ch == '.' && !inBracket)
            {
                AddIdentifierPart(parts, value[start..i]);
                start = i + 1;
            }
        }

        AddIdentifierPart(parts, value[start..]);
        return parts.ToArray();
    }

    private static void AddIdentifierPart(ICollection<string> parts, string value)
    {
        var trimmed = value.Trim();
        if (!string.IsNullOrWhiteSpace(trimmed))
        {
            parts.Add(trimmed);
        }
    }

    private static string UnquoteIdentifier(string value)
    {
        var trimmed = value.Trim();
        return IsBracketQuoted(trimmed)
            ? trimmed[1..^1].Replace("]]", "]", StringComparison.Ordinal)
            : trimmed;
    }

    private static bool IsBracketQuoted(string value)
    {
        var trimmed = value.Trim();
        return trimmed.Length >= 2 && trimmed[0] == '[' && trimmed[^1] == ']';
    }

    private static string SqlStringLiteral(string value) =>
        $"N'{value.Replace("'", "''", StringComparison.Ordinal)}'";

    private static string SqlNullableStringLiteral(string? value, int maxLength)
    {
        return string.IsNullOrWhiteSpace(value)
            ? $"CAST(NULL AS nvarchar({maxLength}))"
            : $"CAST({SqlStringLiteral(value)} AS nvarchar({maxLength}))";
    }

    private static IReadOnlyDictionary<string, string> ResolveCandidateTypeMap(MetaDataQualityModel model)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        AddTypes(map, model.JoinOrphanList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.JoinOrphan);
        AddTypes(map, model.OuterJoinNullExpansionList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.OuterJoinNullExpansion);
        AddTypes(map, model.JoinMultiplicityExplosionList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.JoinMultiplicityExplosion);
        AddTypes(map, model.OutputDuplicateRiskList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.OutputDuplicateRisk);
        AddTypes(map, model.MinorityJoinPatternList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.MinorityJoinPattern);
        AddTypes(map, model.IncompleteCompositeJoinList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.IncompleteCompositeJoin);
        AddTypes(map, model.SuspiciousExtraJoinPredicateList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.SuspiciousExtraJoinPredicate);
        AddTypes(map, model.MissingCommonFilterList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.MissingCommonFilter);
        AddTypes(map, model.MinorityColumnEquivalenceList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.MinorityColumnEquivalence);
        AddTypes(map, model.InnerJoinAgainstUsuallyOptionalRelationshipList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.InnerJoinAgainstUsuallyOptionalRelationship);
        AddTypes(map, model.LeftJoinAgainstUsuallyMandatoryRelationshipList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.LeftJoinAgainstUsuallyMandatoryRelationship);
        AddTypes(map, model.ImpliedForeignKeyMissingReferenceList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedForeignKeyMissingReference);
        AddTypes(map, model.ImpliedUniqueKeyViolationList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedUniqueKeyViolation);
        AddTypes(map, model.ImpliedJoinFanoutRiskList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedJoinFanoutRisk);
        AddTypes(map, model.ImpliedOutputDuplicateRiskList.Select(static row => row.DataQualityCandidate.Id), CandidateKinds.ImpliedOutputDuplicateRisk);
        return map;
    }

    private static string ResolveCandidateType(IReadOnlyDictionary<string, string> candidateTypes, string candidateId)
    {
        if (!candidateTypes.TryGetValue(candidateId, out var candidateType))
        {
            throw new InvalidOperationException(
                $"DataQualityCandidate '{candidateId}' has no discovered type entity. Expected one of JoinOrphan, OuterJoinNullExpansion, JoinMultiplicityExplosion, OutputDuplicateRisk, MinorityJoinPattern, IncompleteCompositeJoin, SuspiciousExtraJoinPredicate, MissingCommonFilter, MinorityColumnEquivalence, InnerJoinAgainstUsuallyOptionalRelationship, LeftJoinAgainstUsuallyMandatoryRelationship, ImpliedForeignKeyMissingReference, ImpliedUniqueKeyViolation, ImpliedJoinFanoutRisk, ImpliedOutputDuplicateRisk.");
        }

        return candidateType;
    }

    private static void AddTypes(
        IDictionary<string, string> map,
        IEnumerable<string> candidateIds,
        string candidateType)
    {
        foreach (var candidateId in candidateIds.Where(static id => !string.IsNullOrWhiteSpace(id)))
        {
            map[candidateId] = candidateType;
        }
    }

    private static string[] ResolveSideTableNames(
        IReadOnlyDictionary<string, JoinPatternOccurrenceBaseTable[]> baseTablesByOccurrenceId,
        string occurrenceId,
        string? joinInputTableReferenceId)
    {
        if (!baseTablesByOccurrenceId.TryGetValue(occurrenceId, out var rows))
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
        if (!keyPartsByPatternId.TryGetValue(joinPatternId, out var rows) || rows.Length == 0)
        {
            return "(none)";
        }

        var parts = rows
            .OrderBy(static row => ParseOrdinalOrMax(row.Ordinal))
            .Select(static row =>
            {
                var left = string.IsNullOrWhiteSpace(row.FirstExpressionDisplay) ? row.FirstExpressionId : row.FirstExpressionDisplay;
                var right = string.IsNullOrWhiteSpace(row.SecondExpressionDisplay) ? row.SecondExpressionId : row.SecondExpressionDisplay;
                return $"{left}={right}";
            })
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .ToArray();
        return parts.Length == 0
            ? "(none)"
            : string.Join(" AND ", parts);
    }

    private static string JoinCsv(IReadOnlyList<string> values) =>
        values.Count == 0 ? "(none)" : string.Join(",", values);

    private static int ParseOrdinalOrMax(string ordinal)
    {
        return int.TryParse(ordinal, out var parsed)
            ? parsed
            : int.MaxValue;
    }

    private static string SanitizeFileName(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars().ToHashSet();
        var chars = value
            .Select(ch => invalidChars.Contains(ch) ? '_' : ch)
            .ToArray();
        var sanitized = new string(chars).Trim();
        return string.IsNullOrWhiteSpace(sanitized)
            ? "DataQualityCandidate"
            : sanitized;
    }

    private sealed record PatternComparison(
        string DominantPatternId,
        string OutlierPatternId)
    {
        public static PatternComparison Empty { get; } = new(string.Empty, string.Empty);
    }

    private sealed record CandidateMetadata(
        string Explanation,
        string EvidenceSummary,
        long? EvidenceOccurrenceCount,
        long? EvidenceTransformCount,
        long? DistinctTransformCount,
        long? DistinctSourceTransformCount,
        long? DistinctSourceObjectCount,
        long? DistinctRelationshipPatternCount,
        long? EffectiveTransformCount,
        decimal? EvidenceConsensusRatio,
        decimal? EvidenceOutlierRatio,
        string EvidenceQuality,
        string ConfidenceBand,
        string ConfidenceReason,
        string EvidenceDiversitySummary,
        string ConfidenceSummary,
        string CorpusRelationship,
        string CorpusRelationshipPattern,
        string DominantPattern,
        string OutlierPattern);

    private sealed record RuntimeSideInfo(
        string? RelationshipText,
        string? ReferencingObject,
        string? ReferencedObject,
        string? CheckedObject,
        string? SuspectSide,
        string? SuspectObject,
        string? LookupObject,
        string? RelatedObject)
    {
        public static RuntimeSideInfo Empty { get; } = new(null, null, null, null, null, null, null, null);
    }

    private sealed record SemanticReviewFinding(
        string CandidateId,
        string CandidateKind,
        string Relationship,
        string CorpusRelationship,
        string CorpusRelationshipPattern,
        string DominantPattern,
        string OutlierPattern,
        string TransformViews,
        string Explanation,
        long? EvidenceOccurrenceCount,
        long? EvidenceTransformCount,
        long? DistinctTransformCount,
        long? DistinctSourceTransformCount,
        long? DistinctSourceObjectCount,
        long? DistinctRelationshipPatternCount,
        long? EffectiveTransformCount,
        decimal? EvidenceConsensusRatio,
        decimal? EvidenceOutlierRatio,
        string EvidenceQuality,
        string ConfidenceBand,
        string ConfidenceReason,
        string EvidenceDiversitySummary,
        string ConfidenceSummary);

    private sealed record JoinRenderContext(
        string PatternId,
        string JoinType,
        string LeftTable,
        string RightTable,
        IReadOnlyList<JoinKeyPart> KeyParts,
        int OccurrenceCount,
        IReadOnlyList<string> TransformViewNames);

    private sealed record JoinKeyPart(string LeftColumn, string RightColumn);

    private enum CandidateSqlOutputMode
    {
        RuntimeCheck,
        SemanticReviewFinding,
        Unsupported,
    }
}

public sealed class DataQualityToSqlResult
{
    public required string OutputPath { get; init; }

    public required int CandidateViewCount { get; init; }

    public required int DashboardViewCount { get; init; }

    public required int OperationalTableCount { get; init; }

    public required int OperationalProcedureCount { get; init; }

    public int ScriptCount => CandidateViewCount + DashboardViewCount + 1;
}

