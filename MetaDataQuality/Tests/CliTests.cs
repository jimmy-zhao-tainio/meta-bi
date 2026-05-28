using System.Globalization;
using MetaConvert.DataQualityToSql;
using MetaBi.Tests.Common;
using MetaDataQuality;
using MetaDataQuality.Core;
using MetaTransformScript.Sql;

namespace MetaDataQuality.Tests;

public sealed class CliTests
{
    [Fact]
    public void Help_ShowsCommands()
    {
        var result = RunCli("help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("from-transform-workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("inspect", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("promote", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("select", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommandHelp_ShowsDescriptorOptions()
    {
        var result = RunCli("from-transform-workspace --help");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("Command: from-transform-workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Options:", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--transform-workspace <path>", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--new-workspace <path>", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FromTransformWorkspace_And_Promote_Works()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");
        var qualityWorkspacePath = Path.Combine(rootPath, "quality");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                "select c.CustomerId, o.OrderId from dbo.Customer c left outer join dbo.[Order] o on c.CustomerId = o.CustomerId",
                "dbo.TargetOrders",
                transformWorkspacePath,
                "dbo.v_customer_orders");

            var generated = RunCli(
                $"from-transform-workspace --transform-workspace \"{transformWorkspacePath}\" --new-workspace \"{qualityWorkspacePath}\"");

            Assert.Equal(0, generated.ExitCode);
            Assert.Contains("Views ready to create:", generated.Output, StringComparison.Ordinal);
            Assert.True(File.Exists(Path.Combine(qualityWorkspacePath, "workspace.xml")));
            Assert.True(File.Exists(Path.Combine(qualityWorkspacePath, "model.xml")));

            var model = MetaDataQualityModel.LoadFromXmlWorkspace(qualityWorkspacePath, searchUpward: false);
            Assert.NotEmpty(model.DataQualityCandidateList);
            Assert.NotEmpty(model.OuterJoinNullExpansionList);
            Assert.NotEmpty(model.CorpusRelationshipList);

            var inspect = RunCli($"inspect --workspace \"{qualityWorkspacePath}\"");
            Assert.Equal(0, inspect.ExitCode);
            Assert.Contains("Corpus Inference:", inspect.Output, StringComparison.Ordinal);

            var firstCandidate = model.DataQualityCandidateList[0];
            var promoted = RunCli($"promote --workspace \"{qualityWorkspacePath}\" --candidate-id \"{firstCandidate.Id}\"");
            Assert.Equal(0, promoted.ExitCode);
            Assert.Contains("Ok", promoted.Output, StringComparison.Ordinal);

            var reloaded = MetaDataQualityModel.LoadFromXmlWorkspace(qualityWorkspacePath, searchUpward: false);
            var promotedRow = Assert.Single(
                reloaded.DataQualityCandidateList,
                item => string.Equals(item.Id, firstCandidate.Id, StringComparison.Ordinal));
            Assert.Equal("Promoted", promotedRow.Status);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public async Task Discovery_CapturesJoinAnchorAcrossCteLayers()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");

        try
        {
            const string sql = """
WITH orders_cte AS
(
    SELECT o.CustomerId, o.OrderId
    FROM dbo.[Order] o
),
joined_cte AS
(
    SELECT c.CustomerId, oc.OrderId
    FROM dbo.Customer c
    LEFT OUTER JOIN orders_cte oc ON c.CustomerId = oc.CustomerId
)
SELECT CustomerId, OrderId
FROM joined_cte;
""";

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                sql,
                "dbo.TargetOrders",
                transformWorkspacePath,
                "dbo.v_customer_orders_cte");

            var result = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);

            Assert.NotEmpty(result.Model.JoinPatternList);
            Assert.NotEmpty(result.Model.DataQualityCandidateJoinPatternLinkList);
            Assert.NotEmpty(result.Model.JoinPatternOccurrenceBaseTableList);
            Assert.NotEmpty(result.Model.JoinPatternKeyPartList);
            Assert.Contains(
                result.Model.JoinPatternOccurrenceList,
                occurrence =>
                    !string.IsNullOrWhiteSpace(occurrence.QualifiedJoinId)
                    && (!string.IsNullOrWhiteSpace(occurrence.CteId)
                        || occurrence.ScopePath.Contains("CTE:", StringComparison.OrdinalIgnoreCase)));
            Assert.Contains(
                result.Model.JoinPatternOccurrenceBaseTableList,
                row => row.BaseObjectName.Contains("Customer", StringComparison.OrdinalIgnoreCase)
                       || row.BaseObjectName.Contains("Order", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(
                result.Model.JoinPatternKeyPartList,
                row =>
                    row.FirstExpressionDisplay.Contains("CustomerId", StringComparison.OrdinalIgnoreCase)
                    || row.SecondExpressionDisplay.Contains("CustomerId", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public async Task Discovery_CapturesCompositeJoinPredicateParts()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");

        try
        {
            const string sql = """
SELECT c.CustomerId, c.RegionId, o.OrderId
FROM dbo.Customer c
INNER JOIN dbo.[Order] o
    ON c.CustomerId = o.CustomerId
   AND c.RegionId = o.RegionId;
""";

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                sql,
                "dbo.TargetOrders",
                transformWorkspacePath,
                "dbo.v_customer_orders_composite");

            var result = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);

            Assert.Contains(
                result.Model.JoinPatternList,
                pattern => string.Equals(pattern.EqualityPredicateCount, "2", StringComparison.Ordinal));

            var firstPattern = result.Model.JoinPatternList.First();
            var predicateParts = result.Model.JoinPatternKeyPartList
                .Where(row => string.Equals(row.JoinPattern.Id, firstPattern.Id, StringComparison.Ordinal))
                .OrderBy(row => int.TryParse(row.Ordinal, out var parsed) ? parsed : int.MaxValue)
                .ToArray();

            Assert.True(predicateParts.Length >= 2);
            Assert.Contains(
                predicateParts,
                row =>
                    row.FirstExpressionDisplay.Contains("CustomerId", StringComparison.OrdinalIgnoreCase)
                    || row.SecondExpressionDisplay.Contains("CustomerId", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(
                predicateParts,
                row =>
                    row.FirstExpressionDisplay.Contains("RegionId", StringComparison.OrdinalIgnoreCase)
                    || row.SecondExpressionDisplay.Contains("RegionId", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public async Task Discovery_CapturesFilterPredicateObservationsFromWhereClause()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");

        try
        {
            const string sql = """
SELECT c.CustomerId, o.OrderId
FROM dbo.Customer c
INNER JOIN dbo.[Order] o
    ON c.CustomerId = o.CustomerId
WHERE c.IsDeleted = 0
  AND c.ValidTo IS NULL;
""";

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                sql,
                "dbo.TargetOrders",
                transformWorkspacePath,
                "dbo.v_customer_orders_filtered");

            var result = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);

            Assert.NotEmpty(result.Model.FilterPredicateObservationList);
            Assert.Contains(
                result.Model.FilterPredicateObservationList,
                row => row.BaseObjectName.Contains("Customer", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(
                result.Model.FilterPredicateObservationList,
                row => row.PredicateDisplay.Contains("IsDeleted", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public async Task Discovery_SuppressesFanoutChecksWhenRightDetailColumnIsProjected()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");

        try
        {
            const string sql = """
SELECT c.CustomerId, c.RegionId, o.OrderId
FROM dbo.Customer c
LEFT OUTER JOIN dbo.[Order] o
    ON c.CustomerId = o.CustomerId
   AND c.RegionId = o.RegionId;
""";

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                sql,
                "dbo.TargetOrders",
                transformWorkspacePath,
                "dbo.v_customer_orders");

            var result = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);

            Assert.NotEmpty(result.Model.JoinOrphanList);
            Assert.NotEmpty(result.Model.OuterJoinNullExpansionList);
            Assert.Empty(result.Model.JoinMultiplicityExplosionList);
            Assert.Empty(result.Model.OutputDuplicateRiskList);
            Assert.DoesNotContain(
                result.Model.JoinPatternOccurrenceSignalList,
                row => string.Equals(row.SignalKind, CandidateKinds.JoinMultiplicityExplosion, StringComparison.Ordinal));
            Assert.DoesNotContain(
                result.Model.JoinPatternOccurrenceSignalList,
                row => string.Equals(row.SignalKind, CandidateKinds.OutputDuplicateRisk, StringComparison.Ordinal));
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public async Task Discovery_SuppressedLocalRisks_DoNotProduceImpliedFanoutOrOutputDuplicateCandidates()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");

        try
        {
            const string sql = """
SELECT c.CustomerId, c.RegionId, o.OrderId
FROM dbo.Customer c
LEFT OUTER JOIN dbo.[Order] o
    ON c.CustomerId = o.CustomerId
   AND c.RegionId = o.RegionId;
""";

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                sql,
                "dbo.TargetOrders",
                transformWorkspacePath,
                "dbo.v_customer_orders");

            var result = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);

            new MetaDataQualityCorpusInferenceService().Apply(
                result.Model,
                new CorpusInferenceOptions
                {
                    MinTablePairOccurrenceCount = 1,
                    MinTablePairTransformCount = 1,
                    DominantPatternMinRatio = 0.5,
                    DominantPatternMinOccurrenceCount = 1,
                    MinorityPatternMaxRatio = 1d,
                    MinRelationshipOccurrenceCount = 1,
                    MinRelationshipTransformCount = 1,
                    MinConsensusRatio = 0.5,
                    MinDominantPatternOccurrenceCount = 1,
                    MinLookupSideOccurrenceCount = int.MaxValue,
                    MinLookupSideTransformCount = int.MaxValue,
                    MinLookupSideConsistencyRatio = 1d,
                    MinKeyPartOccurrenceCount = int.MaxValue,
                    MinPatternOccurrenceCount = int.MaxValue,
                    MinPatternTransformCount = int.MaxValue,
                    DominantOptionalityMinRatio = 1d,
                    DominantOptionalityMinOccurrenceCount = int.MaxValue,
                    OutlierOptionalityMaxRatio = 0d,
                    OutlierOptionalityMinOccurrenceCount = int.MaxValue,
                    MinFanoutSignalOccurrenceCount = 1,
                    MinFanoutSignalTransformCount = 1,
                    MinFanoutSignalRatio = 0.5,
                    MinOutputDuplicateSignalOccurrenceCount = 1,
                    MinOutputDuplicateSignalTransformCount = 1,
                    MinOutputDuplicateSignalRatio = 0.5,
                });

            Assert.Empty(result.Model.JoinMultiplicityExplosionList);
            Assert.Empty(result.Model.OutputDuplicateRiskList);
            Assert.DoesNotContain(
                result.Model.JoinPatternOccurrenceSignalList,
                row => string.Equals(row.SignalKind, CandidateKinds.JoinMultiplicityExplosion, StringComparison.Ordinal));
            Assert.DoesNotContain(
                result.Model.JoinPatternOccurrenceSignalList,
                row => string.Equals(row.SignalKind, CandidateKinds.OutputDuplicateRisk, StringComparison.Ordinal));
            Assert.Empty(result.Model.ImpliedJoinFanoutRiskList);
            Assert.Empty(result.Model.ImpliedOutputDuplicateRiskList);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public async Task DataQualityToSql_GeneratesJoinBasedViews()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");
        var qualityWorkspacePath = Path.Combine(rootPath, "quality");
        var outputPath = Path.Combine(rootPath, "DataQualityViews.sql");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToWorkspaceAsync(
                """
                SELECT c.CustomerId, c.RegionId, o.OrderId
                FROM dbo.Customer c
                LEFT OUTER JOIN dbo.[Order] o
                    ON c.CustomerId = o.CustomerId
                   AND c.RegionId = o.RegionId;
                """,
                "dbo.TargetOrders",
                transformWorkspacePath,
                "dbo.v_customer_orders");

            var discovery = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);
            foreach (var candidate in discovery.Model.DataQualityCandidateList)
            {
                candidate.Status = CandidateStatuses.Promoted;
            }

            discovery.Model.SaveToXmlWorkspace(qualityWorkspacePath);

            var result = new DataQualityToSqlConverter().Convert(qualityWorkspacePath, outputPath);
            var sql = File.ReadAllText(outputPath);

            Assert.True(result.ScriptCount > 0);
            Assert.True(result.CandidateViewCount > 0);
            Assert.Equal(1, result.DashboardViewCount);
            Assert.Equal(2, result.OperationalTableCount);
            Assert.Equal(2, result.OperationalProcedureCount);
            Assert.Contains("CREATE OR ALTER VIEW [dq].[", sql, StringComparison.Ordinal);
            Assert.Contains("CREATE OR ALTER VIEW [dq].[v_DataQualityReview]", sql, StringComparison.Ordinal);
            Assert.Contains("CREATE DATABASE [MetaDQ]", sql, StringComparison.Ordinal);
            Assert.Contains("CREATE OR ALTER PROCEDURE [dbo].[Run]", sql, StringComparison.Ordinal);
            Assert.Contains("CREATE OR ALTER PROCEDURE [dbo].[Findings]", sql, StringComparison.Ordinal);
            Assert.Contains("v_DataQualityReview", sql, StringComparison.Ordinal);
            Assert.Contains("FROM [dbo].[Customer] AS [dq_left]", sql, StringComparison.Ordinal);
            Assert.Contains("FROM [dbo].[Order] AS [dq_right]", sql, StringComparison.Ordinal);
            Assert.Contains("[dq_left].[CustomerId] = [dq_right].[CustomerId]", sql, StringComparison.Ordinal);
            Assert.Contains("NOT EXISTS", sql, StringComparison.Ordinal);
            Assert.Contains("COUNT_BIG(*)", sql, StringComparison.Ordinal);
            Assert.Contains("KeyValues", sql, StringComparison.Ordinal);
            Assert.Contains("TransformViews", sql, StringComparison.Ordinal);
            Assert.Contains("dbo.v_customer_orders", sql, StringComparison.Ordinal);
            Assert.Contains("ReviewQuery", sql, StringComparison.Ordinal);
            Assert.Contains("TransformViewQuery", sql, StringComparison.Ordinal);
            Assert.Contains("DetailQuery", sql, StringComparison.Ordinal);
            Assert.Contains("FindingTitle", sql, StringComparison.Ordinal);
            Assert.Contains("FindingCategory", sql, StringComparison.Ordinal);
            Assert.Contains("FindingGroupCount", sql, StringComparison.Ordinal);
            Assert.Contains("SuspectRowCount", sql, StringComparison.Ordinal);
            Assert.DoesNotContain("WHERE 1 = 0;</SqlTemplate>", sql, StringComparison.Ordinal);
            Assert.DoesNotContain("> 1GO", sql, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public void Inspect_ShowsDirectionalOptionalityWording()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var qualityWorkspacePath = Path.Combine(rootPath, "quality");

        try
        {
            var model = MetaDataQualityModel.CreateEmpty();
            AddJoinPatternForCli(
                model,
                patternId: "JoinPattern.CustomerOrder.Left.Cli",
                keyPairs:
                [
                    ("c.CustomerId", "o.CustomerId"),
                ],
                joinType: "LeftOuter");
            AddJoinPatternForCli(
                model,
                patternId: "JoinPattern.CustomerOrder.Inner.Cli",
                keyPairs:
                [
                    ("c.CustomerId", "o.CustomerId"),
                ],
                joinType: "Inner");

            for (var i = 1; i <= 8; i++)
            {
                AddOccurrenceForCli(
                    model,
                    occurrenceId: $"Occ.CustomerOrder.Left.Cli.{i}",
                    patternId: "JoinPattern.CustomerOrder.Left.Cli",
                    scriptName: $"Script.CustomerOrder.Left.Cli.{i}",
                    leftTable: "sales.Customer",
                    rightTable: "sales.Order");
            }

            AddOccurrenceForCli(
                model,
                occurrenceId: "Occ.CustomerOrder.Inner.Cli.1",
                patternId: "JoinPattern.CustomerOrder.Inner.Cli",
                scriptName: "Script.CustomerOrder.Inner.Cli.1",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");

            new MetaDataQualityCorpusInferenceService().Apply(model, BuildOptionalityOnlyThresholdsForCli());
            model.SaveToXmlWorkspace(qualityWorkspacePath);

            var inspect = RunCli($"inspect --workspace \"{qualityWorkspacePath}\"");
            Assert.Equal(0, inspect.ExitCode);
            Assert.Contains("Optionality drift checks:", inspect.Output, StringComparison.Ordinal);
            Assert.Contains("nullable side is", inspect.Output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    private static (int ExitCode, string Output) RunCli(string arguments) =>
        CliTestRunner.RunStandardCli("MetaDataQuality", "meta-data-quality.exe", arguments);

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }

    private static void AddJoinPatternForCli(
        MetaDataQualityModel model,
        string patternId,
        (string LeftExpression, string RightExpression)[] keyPairs,
        string joinType = "Inner")
    {
        var pattern = new JoinPattern
        {
            Id = patternId,
            CanonicalSignature = patternId,
            QualifiedJoinType = joinType,
            ContainsEqualityPredicate = "true",
            EqualityPredicateCount = keyPairs.Length.ToString(CultureInfo.InvariantCulture),
        };
        model.JoinPatternList.Add(pattern);

        for (var i = 0; i < keyPairs.Length; i++)
        {
            var keyPair = keyPairs[i];
            model.JoinPatternKeyPartList.Add(new JoinPatternKeyPart
            {
                Id = $"{patternId}.KeyPart.{i + 1}",
                JoinPattern = pattern,
                Ordinal = (i + 1).ToString(CultureInfo.InvariantCulture),
                BooleanComparisonExpressionId = $"{patternId}.Comparison.{i + 1}",
                FirstExpressionId = $"{patternId}.First.{i + 1}",
                SecondExpressionId = $"{patternId}.Second.{i + 1}",
                FirstExpressionDisplay = keyPair.LeftExpression,
                SecondExpressionDisplay = keyPair.RightExpression,
            });
        }
    }

    private static void AddOccurrenceForCli(
        MetaDataQualityModel model,
        string occurrenceId,
        string patternId,
        string scriptName,
        string leftTable,
        string rightTable)
    {
        var leftReferenceId = $"{occurrenceId}.Left";
        var rightReferenceId = $"{occurrenceId}.Right";
        var pattern = model.JoinPatternList.Single(row => string.Equals(row.Id, patternId, StringComparison.Ordinal));
        var occurrence = new JoinPatternOccurrence
        {
            Id = occurrenceId,
            JoinPattern = pattern,
            TransformScriptId = $"{scriptName}.Id",
            TransformScriptName = scriptName,
            QueryExpressionId = $"{occurrenceId}.QueryExpression",
            QuerySpecificationId = $"{occurrenceId}.QuerySpecification",
            JoinTableReferenceId = $"{occurrenceId}.JoinRef",
            QualifiedJoinId = $"{occurrenceId}.QualifiedJoin",
            SearchConditionBooleanExpressionId = $"{occurrenceId}.SearchCondition",
            FirstTableReferenceId = leftReferenceId,
            SecondTableReferenceId = rightReferenceId,
            ScopePath = "MainQuery",
            CteId = string.Empty,
            CteName = string.Empty,
        };
        model.JoinPatternOccurrenceList.Add(occurrence);

        model.JoinPatternOccurrenceBaseTableList.Add(new JoinPatternOccurrenceBaseTable
        {
            Id = $"{occurrenceId}.BaseTable.Left",
            JoinPatternOccurrence = occurrence,
            JoinInputTableReferenceId = leftReferenceId,
            BaseTableReferenceId = $"{occurrenceId}.Left.BaseTableRef",
            BaseNamedTableReferenceId = $"{occurrenceId}.Left.NamedTableRef",
            BaseSchemaObjectNameId = $"{occurrenceId}.Left.SchemaObjectRef",
            BaseObjectName = leftTable,
            ResolutionDepth = "0",
            ResolutionPath = string.Empty,
            ResolvedInCteId = string.Empty,
            ResolvedInCteName = string.Empty,
        });
        model.JoinPatternOccurrenceBaseTableList.Add(new JoinPatternOccurrenceBaseTable
        {
            Id = $"{occurrenceId}.BaseTable.Right",
            JoinPatternOccurrence = occurrence,
            JoinInputTableReferenceId = rightReferenceId,
            BaseTableReferenceId = $"{occurrenceId}.Right.BaseTableRef",
            BaseNamedTableReferenceId = $"{occurrenceId}.Right.NamedTableRef",
            BaseSchemaObjectNameId = $"{occurrenceId}.Right.SchemaObjectRef",
            BaseObjectName = rightTable,
            ResolutionDepth = "0",
            ResolutionPath = string.Empty,
            ResolvedInCteId = string.Empty,
            ResolvedInCteName = string.Empty,
        });
    }

    private static CorpusInferenceOptions BuildOptionalityOnlyThresholdsForCli()
    {
        return new CorpusInferenceOptions
        {
            MinTablePairOccurrenceCount = int.MaxValue,
            MinTablePairTransformCount = int.MaxValue,
            DominantPatternMinRatio = 1d,
            DominantPatternMinOccurrenceCount = int.MaxValue,
            MinorityPatternMaxRatio = 0d,
            MinRelationshipOccurrenceCount = int.MaxValue,
            MinRelationshipTransformCount = int.MaxValue,
            MinConsensusRatio = 1d,
            MinDominantPatternOccurrenceCount = int.MaxValue,
            MinLookupSideOccurrenceCount = int.MaxValue,
            MinLookupSideTransformCount = int.MaxValue,
            MinLookupSideConsistencyRatio = 1d,
            MinKeyPartOccurrenceCount = int.MaxValue,
            MinPatternOccurrenceCount = 8,
            MinPatternTransformCount = 4,
            DominantOptionalityMinRatio = 0.85,
            DominantOptionalityMinOccurrenceCount = 6,
            OutlierOptionalityMaxRatio = 0.15,
            OutlierOptionalityMinOccurrenceCount = 1,
        };
    }
}
