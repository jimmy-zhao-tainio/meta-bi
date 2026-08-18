using System.Diagnostics;
using Meta.Integration;
using Meta.Operations.Domain;
using Meta.Surfaces.Xml;
using MetaBi.Tests.Common;
using MetaConvert.DataQualityToSql;
using MetaDataQuality.Core;
using MetaSql;
using MetaWeave.Core;
using MetaWeaveScript.Execution;
using Xunit.Abstractions;

namespace MetaDataQuality.Tests;

public sealed class DataQualityToSqlWeaveWitnessTests(ITestOutputHelper output)
{
    [Fact]
    public async Task DirectWeave_ProducesRuntimeSemanticAndGroupedViews()
    {
        var rootPath = Path.Combine(
            Path.GetTempPath(),
            "MetaDataQuality.Tests",
            Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(rootPath, "source");
        var outputPath = Path.Combine(rootPath, "output");
        var oraclePath = Path.Combine(rootPath, "oracle.sql");

        try
        {
            var source = CreateWitnessModel();
            TypedWorkspaceXmlSerializer.Save(source, sourcePath);

            var repositoryRoot = CliTestRunner.FindRepositoryRoot();
            var probe = await ExecuteDirectWeaveAsync(source);
            var result = probe.Result;

            Assert.True(result.IsSuccess, FormatIssues(result));
            var actual = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
                result.OutputWorkspace!,
                static () => MetaSqlModel.CreateEmpty());
            TypedWorkspaceXmlSerializer.Save(actual, outputPath);

            Assert.Single(actual.DatabaseList);
            Assert.Single(actual.SchemaList);
            Assert.Equal(16, actual.ViewList.Count);

            var runtimeView = Assert.Single(
                actual.ViewList,
                view => view.Name == "v_JoinOrphan.JoinPattern.CustomerOrder.1");
            var semanticView = Assert.Single(
                actual.ViewList,
                view => view.Name == "v_MinorityJoinPattern.CustomerOrder");
            var groupedView = Assert.Single(
                actual.ViewList,
                view => view.Name == "v_JoinMultiplicityExplosion.JoinPattern.ProductOrderLine.1");
            var outerNullView = Assert.Single(
                actual.ViewList,
                view => view.Name == "v_OuterJoinNullExpansion.JoinPattern.OrderShipment.1");
            var duplicateView = Assert.Single(
                actual.ViewList,
                view => view.Name == "v_OutputDuplicateRisk.JoinPattern.SalesOrderCustomer.1");
            var impliedForeignKeyView = Assert.Single(
                actual.ViewList,
                view => view.Name == "v_ImpliedForeignKeyMissingReference.AccountInvoice");
            var impliedUniqueView = Assert.Single(
                actual.ViewList,
                view => view.Name == "v_ImpliedUniqueKeyViolation.AccountInvoice");
            var semanticFamilyWitnesses = SemanticFamilyWitnesses();
            var semanticFamilyViews = semanticFamilyWitnesses.ToDictionary(
                witness => witness.CandidateId,
                witness => Assert.Single(
                    actual.ViewList,
                    view => view.Name == $"v_{witness.CandidateId}"),
                StringComparer.Ordinal);
            var dashboardView = Assert.Single(
                actual.ViewList,
                view => view.Name == "v_DataQualityReview");

            AssertRuntimeSemantics(runtimeView.DefinitionSql);
            AssertSemanticReviewSemantics(semanticView.DefinitionSql);
            AssertGroupedRuntimeSemantics(groupedView.DefinitionSql);
            AssertOuterJoinNullSemantics(outerNullView.DefinitionSql);
            AssertOutputDuplicateSemantics(duplicateView.DefinitionSql);
            AssertImpliedForeignKeySemantics(impliedForeignKeyView.DefinitionSql);
            AssertImpliedUniqueKeySemantics(impliedUniqueView.DefinitionSql);
            foreach (var witness in semanticFamilyWitnesses)
            {
                AssertSemanticFamilySemantics(semanticFamilyViews[witness.CandidateId].DefinitionSql, witness);
            }
            Assert.Equal(1, CountOccurrences(impliedForeignKeyView.DefinitionSql, "UNION ALL"));
            Assert.Equal(1, CountOccurrences(impliedUniqueView.DefinitionSql, "UNION ALL"));
            AssertDashboardSemantics(dashboardView.DefinitionSql);
            Assert.Contains("FROM [dq].[v_JoinOrphan.JoinPattern.CustomerOrder.1]", dashboardView.DefinitionSql, StringComparison.Ordinal);
            Assert.Contains("FROM [dq].[v_JoinMultiplicityExplosion.JoinPattern.ProductOrderLine.1]", dashboardView.DefinitionSql, StringComparison.Ordinal);
            Assert.Contains("FROM [dq].[v_MinorityJoinPattern.CustomerOrder]", dashboardView.DefinitionSql, StringComparison.Ordinal);
            Assert.Contains("FROM [dq].[v_OuterJoinNullExpansion.JoinPattern.OrderShipment.1]", dashboardView.DefinitionSql, StringComparison.Ordinal);
            Assert.Contains("FROM [dq].[v_OutputDuplicateRisk.JoinPattern.SalesOrderCustomer.1]", dashboardView.DefinitionSql, StringComparison.Ordinal);
            Assert.Contains("FROM [dq].[v_ImpliedForeignKeyMissingReference.AccountInvoice]", dashboardView.DefinitionSql, StringComparison.Ordinal);
            Assert.Contains("FROM [dq].[v_ImpliedUniqueKeyViolation.AccountInvoice]", dashboardView.DefinitionSql, StringComparison.Ordinal);
            Assert.DoesNotContain(
                "dbo.Invoice references dbo.Account; dbo.Invoice references dbo.Account",
                dashboardView.DefinitionSql,
                StringComparison.Ordinal);
            Assert.DoesNotContain("dbo.v_invoice_accounts_reversed", dashboardView.DefinitionSql, StringComparison.Ordinal);
            Assert.All(
                semanticFamilyWitnesses,
                witness => Assert.Contains(
                    $"FROM [dq].[v_{witness.CandidateId}]",
                    dashboardView.DefinitionSql,
                    StringComparison.Ordinal));
            Assert.Equal(14, CountOccurrences(dashboardView.DefinitionSql, "UNION ALL"));
            Assert.StartsWith(
                "CREATE OR ALTER VIEW [dq].[v_JoinOrphan.JoinPattern.CustomerOrder.1]",
                runtimeView.DefinitionSql,
                StringComparison.Ordinal);
            Assert.Equal(1, CountOccurrences(semanticView.DefinitionSql, "CREATE OR ALTER VIEW"));
            Assert.Equal(1, CountOccurrences(semanticView.DefinitionSql, "UNION ALL"));
            Assert.Contains(
                "A second transform uses the same minority relationship pattern.",
                semanticView.DefinitionSql,
                StringComparison.Ordinal);
            Assert.All(
                actual.ViewList,
                view => Assert.StartsWith("CREATE OR ALTER VIEW", view.DefinitionSql, StringComparison.Ordinal));

            new DataQualityToSqlConverter().Convert(sourcePath, oraclePath);
            var oracleSql = File.ReadAllText(oraclePath);
            AssertRuntimeSemantics(oracleSql);
            AssertSemanticReviewSemantics(oracleSql);
            AssertGroupedRuntimeSemantics(oracleSql);
            AssertOuterJoinNullSemantics(oracleSql);
            AssertOutputDuplicateSemantics(oracleSql);
            AssertImpliedForeignKeySemantics(oracleSql);
            AssertImpliedUniqueKeySemantics(oracleSql);
            foreach (var witness in semanticFamilyWitnesses)
            {
                AssertSemanticFamilySemantics(oracleSql, witness);
            }
            AssertDashboardSemantics(oracleSql);
            AssertFrozenAdventureWorksContract(
                File.ReadAllText(Path.Combine(
                    repositoryRoot,
                    "Demos",
                    "AdventureWorksBiStackDemo",
                    "Runs",
                    "dq",
                    "DataQuality.sql")));

            var weaveBytes = Directory.EnumerateFiles(probe.WeavePath, "*", SearchOption.AllDirectories)
                .Sum(static path => new FileInfo(path).Length);
            var outputBytes = Directory.EnumerateFiles(outputPath, "*", SearchOption.AllDirectories)
                .Sum(static path => new FileInfo(path).Length);
            output.WriteLine(
                "Direct weave: {0} relations, {1} transformations, {2} requirements, {3} MetaSql objects, {4} output bytes, {5} weave bytes, {6:F1} ms execution.",
                probe.Direction.Relations.Count,
                probe.Direction.Transformations.Count,
                probe.Direction.Requirements.Count,
                actual.DatabaseList.Count + actual.SchemaList.Count + actual.ViewList.Count,
                outputBytes,
                weaveBytes,
                probe.Elapsed.TotalMilliseconds);
        }
        finally
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }

    [Fact]
    public async Task DirectWeave_ProducesTypedEmptyDashboardWhenNoCandidatesExist()
    {
        var probe = await ExecuteDirectWeaveAsync(MetaDataQualityModel.CreateEmpty());

        Assert.True(probe.Result.IsSuccess, FormatIssues(probe.Result));
        var actual = ToMetaSql(probe.Result);
        var dashboard = Assert.Single(actual.ViewList);
        Assert.Equal("v_DataQualityReview", dashboard.Name);
        Assert.Contains("CREATE OR ALTER VIEW [dq].[v_DataQualityReview]", dashboard.DefinitionSql, StringComparison.Ordinal);
        Assert.Contains("CAST(NULL AS nvarchar(128)) AS [DQView]", dashboard.DefinitionSql, StringComparison.Ordinal);
        Assert.Contains("CAST(NULL AS bigint) AS [TotalSuspectCount]", dashboard.DefinitionSql, StringComparison.Ordinal);
        Assert.Contains("WHERE 1 = 0", dashboard.DefinitionSql, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DirectWeave_DashboardUsesCandidateKindWhenRuntimeRationaleIsAbsent()
    {
        var source = CreateWitnessModel();
        source.DataQualityCandidateList
            .Single(candidate => candidate.Id.StartsWith("JoinOrphan.", StringComparison.Ordinal))
            .Rationale = null;

        var probe = await ExecuteDirectWeaveAsync(source);

        Assert.True(probe.Result.IsSuccess, FormatIssues(probe.Result));
        var dashboard = Assert.Single(
            ToMetaSql(probe.Result).ViewList,
            view => view.Name == "v_DataQualityReview");
        Assert.Contains("Promoted JoinOrphan candidate.", dashboard.DefinitionSql, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DirectWeave_RejectsGeneratedViewNamesOverSqlServerLimit()
    {
        var source = CreateWitnessModel();
        source.DataQualityCandidateList
            .Single(candidate => candidate.Id.StartsWith("JoinOrphan.", StringComparison.Ordinal))
            .Id = new string('x', 127);

        var probe = await ExecuteDirectWeaveAsync(source);

        Assert.False(probe.Result.IsSuccess);
        Assert.Contains(
            probe.Result.Issues,
            issue => issue.Code == "DataQualityViewNameTooLong");
    }

    [Fact]
    public async Task DirectWeave_RejectsDuplicateGeneratedViewNames()
    {
        var source = CreateWitnessModel();
        var runtimeCandidate = source.DataQualityCandidateList
            .Single(candidate => candidate.Id.StartsWith("JoinOrphan.", StringComparison.Ordinal));
        var minority = Assert.Single(source.MinorityJoinPatternList);
        var evidence = source.DataQualityCandidateEvidenceList[0];
        source.MinorityJoinPatternList.Add(new MinorityJoinPattern
        {
            Id = "semantic.duplicate-runtime-candidate",
            DataQualityCandidate = runtimeCandidate,
            DominantPattern = minority.DominantPattern,
            OutlierPattern = minority.OutlierPattern,
        });
        source.DataQualityCandidateEvidenceList.Add(CopyEvidence(
            evidence,
            "Evidence.DuplicateRuntimeCandidate",
            runtimeCandidate));

        var probe = await ExecuteDirectWeaveAsync(source);

        Assert.False(probe.Result.IsSuccess);
        Assert.Contains(
            probe.Result.Issues,
            issue => issue.Code == "DataQualityViewNameDuplicate");
    }

    [Fact]
    public async Task DirectWeave_RejectsCandidateViewNameThatCollidesWithDashboard()
    {
        var source = CreateWitnessModel();
        source.DataQualityCandidateList
            .Single(candidate => candidate.Id.StartsWith("JoinOrphan.", StringComparison.Ordinal))
            .Id = "DataQualityReview";

        var probe = await ExecuteDirectWeaveAsync(source);

        Assert.False(probe.Result.IsSuccess);
        Assert.Contains(
            probe.Result.Issues,
            issue => issue.Code == "DataQualityViewNameDuplicate");
    }

    [Fact]
    public async Task DirectWeave_ComposesMultipleContextsIntoOneDeterministicView()
    {
        var firstSource = CreateWitnessModel();
        var reorderedSource = CreateWitnessModel();
        reorderedSource.JoinPatternList.Reverse();
        reorderedSource.JoinPatternKeyPartList.Reverse();
        reorderedSource.JoinPatternKeyPartInputObjectIdentifierPartList.Reverse();
        reorderedSource.JoinPatternOccurrenceList.Reverse();
        reorderedSource.JoinPatternOccurrenceBaseTableList.Reverse();
        reorderedSource.DataQualityCandidateJoinPatternLinkList.Reverse();
        reorderedSource.CorpusRelationshipPatternOccurrenceLinkList.Reverse();
        reorderedSource.DataQualityCandidateEvidenceList.Reverse();

        var firstProbe = await ExecuteDirectWeaveAsync(firstSource);
        var reorderedProbe = await ExecuteDirectWeaveAsync(reorderedSource);

        Assert.True(firstProbe.Result.IsSuccess, FormatIssues(firstProbe.Result));
        Assert.True(reorderedProbe.Result.IsSuccess, FormatIssues(reorderedProbe.Result));
        var first = ToMetaSql(firstProbe.Result);
        var reordered = ToMetaSql(reorderedProbe.Result);
        var firstView = Assert.Single(
            first.ViewList,
            view => view.Name == "v_JoinOrphan.JoinPattern.CustomerOrder.1");
        var reorderedView = Assert.Single(
            reordered.ViewList,
            view => view.Name == firstView.Name);

        Assert.Equal(firstView.DefinitionSql, reorderedView.DefinitionSql);
        Assert.Equal(1, CountOccurrences(firstView.DefinitionSql, "CREATE OR ALTER VIEW"));
        Assert.Equal(1, CountOccurrences(firstView.DefinitionSql, "UNION ALL"));
        Assert.True(
            firstView.DefinitionSql.IndexOf("FROM [dbo].[Order] AS [dq_right]", StringComparison.Ordinal)
            < firstView.DefinitionSql.IndexOf("FROM [dbo].[CustomerRegion] AS [dq_right]", StringComparison.Ordinal));

        var firstSemanticView = Assert.Single(
            first.ViewList,
            view => view.Name == "v_MinorityJoinPattern.CustomerOrder");
        var reorderedSemanticView = Assert.Single(
            reordered.ViewList,
            view => view.Name == firstSemanticView.Name);
        Assert.Equal(firstSemanticView.DefinitionSql, reorderedSemanticView.DefinitionSql);

        var firstDashboard = Assert.Single(
            first.ViewList,
            view => view.Name == "v_DataQualityReview");
        var reorderedDashboard = Assert.Single(
            reordered.ViewList,
            view => view.Name == firstDashboard.Name);
        Assert.Equal(firstDashboard.DefinitionSql, reorderedDashboard.DefinitionSql);

        foreach (var firstCandidateView in first.ViewList.Where(view => view.Name != "v_DataQualityReview"))
        {
            var reorderedCandidateView = Assert.Single(
                reordered.ViewList,
                view => view.Name == firstCandidateView.Name);
            Assert.Equal(firstCandidateView.DefinitionSql, reorderedCandidateView.DefinitionSql);
        }
    }

    [Fact]
    public async Task DirectWeave_RejectsInvalidNumericEvidence()
    {
        var source = CreateWitnessModel();
        source.DataQualityCandidateEvidenceList[0].ConsensusRatio = "not-a-number";

        var probe = await ExecuteDirectWeaveAsync(source);

        Assert.False(probe.Result.IsSuccess);
        var issue = Assert.Single(
            probe.Result.Issues,
            candidate => candidate.Code == "DataQualityNumericEvidenceInvalid");
        Assert.Equal("NumericEvidence", issue.RequirementName);
    }

    [Fact]
    public async Task DirectWeave_RejectsSemanticFamilyCandidateWithoutEvidence()
    {
        var source = CreateWitnessModel();
        source.DataQualityCandidateEvidenceList.RemoveAll(
            evidence => evidence.DataQualityCandidate.Id == "IncompleteCompositeJoin.CustomerOrder");

        var probe = await ExecuteDirectWeaveAsync(source);

        Assert.False(probe.Result.IsSuccess);
        Assert.Contains(
            probe.Result.Issues,
            issue => issue.Code == "DataQualitySemanticEvidenceMissing"
                && issue.RequirementName == "SemanticEvidence");
    }

    private static async Task<ExecutionProbe> ExecuteDirectWeaveAsync(MetaDataQualityModel source)
    {
        var repositoryRoot = CliTestRunner.FindRepositoryRoot();
        var weavePath = Path.Combine(
            repositoryRoot,
            "MetaConvert",
            "Weaves",
            "DataQualityToSql");
        var targetContract = await TypedWorkspaceModelMapper.LoadStateAsync(
            Path.Combine(repositoryRoot, "MetaSql", "Workspace"));
        var emptyTarget = new InMemoryWorkspace(
            targetContract.Model.Clone(),
            new GenericInstance { ModelName = targetContract.Model.Name });
        var direction = new MetaWeaveScriptDirectionLoader().Load(weavePath, "forward");
        var stopwatch = Stopwatch.StartNew();
        var result = new MetaWeaveScriptExecutionService().ExecuteDirection(
            direction,
            new Dictionary<string, InMemoryWorkspace>(StringComparer.OrdinalIgnoreCase)
            {
                ["quality"] = TypedWorkspaceModelMapper.ToInMemoryWorkspace(source),
            },
            emptyTarget,
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["databaseName"] = "DataQuality",
            });
        stopwatch.Stop();
        return new ExecutionProbe(result, direction, weavePath, stopwatch.Elapsed);
    }

    private static MetaSqlModel ToMetaSql(MetaWeaveScriptApplicationResult result) =>
        TypedWorkspaceModelMapper.FromInMemoryWorkspace(
            result.OutputWorkspace!,
            static () => MetaSqlModel.CreateEmpty());

    private static MetaDataQualityModel CreateWitnessModel()
    {
        var model = MetaDataQualityModel.CreateEmpty();

        var runtimeCandidate = new DataQualityCandidate
        {
            Id = "JoinOrphan.JoinPattern.CustomerOrder.1",
            Name = "JoinOrphan:JoinPattern.CustomerOrder",
            Status = CandidateStatuses.Promoted,
            Rationale = "Check for orders whose customer reference is missing.",
        };
        model.DataQualityCandidateList.Add(runtimeCandidate);
        model.JoinOrphanList.Add(new JoinOrphan
        {
            Id = "runtime.join-orphan",
            DataQualityCandidate = runtimeCandidate,
            EqualityPredicateCount = "1",
        });
        AddRuntimeContext(
            model,
            runtimeCandidate,
            "CustomerOrder",
            "dbo.Customer",
            "CustomerId",
            "dbo.Order",
            "CustomerId",
            "dbo.v_customer_orders");
        AddRuntimeContext(
            model,
            runtimeCandidate,
            "CustomerRegion",
            "dbo.Customer",
            "RegionId",
            "dbo.CustomerRegion",
            "RegionId",
            "dbo.v_customer_regions");

        var groupedCandidate = new DataQualityCandidate
        {
            Id = "JoinMultiplicityExplosion.JoinPattern.ProductOrderLine.1",
            Name = "JoinMultiplicityExplosion:JoinPattern.ProductOrderLine",
            Status = CandidateStatuses.Promoted,
            Rationale = "Check for relationship fanout at product grain.",
        };
        model.DataQualityCandidateList.Add(groupedCandidate);
        model.JoinMultiplicityExplosionList.Add(new JoinMultiplicityExplosion
        {
            Id = "runtime.join-multiplicity",
            DataQualityCandidate = groupedCandidate,
            EqualityPredicateCount = "1",
        });
        AddRuntimeContext(
            model,
            groupedCandidate,
            "ProductOrderLine",
            "dbo.Product",
            "ProductId",
            "dbo.OrderLine",
            "ProductId",
            "dbo.v_product_order_lines");

        var outerNullCandidate = new DataQualityCandidate
        {
            Id = "OuterJoinNullExpansion.JoinPattern.OrderShipment.1",
            Name = "OuterJoinNullExpansion:JoinPattern.OrderShipment",
            Status = CandidateStatuses.Promoted,
            Rationale = "Check for orders without a matching shipment.",
        };
        model.DataQualityCandidateList.Add(outerNullCandidate);
        model.OuterJoinNullExpansionList.Add(new OuterJoinNullExpansion
        {
            Id = "runtime.outer-join-null",
            DataQualityCandidate = outerNullCandidate,
            OuterJoinCount = "1",
        });
        AddRuntimeContext(
            model,
            outerNullCandidate,
            "OrderShipment",
            "dbo.Order",
            "OrderId",
            "dbo.Shipment",
            "OrderId",
            "dbo.v_order_shipments");

        var duplicateCandidate = new DataQualityCandidate
        {
            Id = "OutputDuplicateRisk.JoinPattern.SalesOrderCustomer.1",
            Name = "OutputDuplicateRisk:JoinPattern.SalesOrderCustomer",
            Status = CandidateStatuses.Promoted,
            Rationale = "Check for duplicate output at sales-order grain.",
        };
        model.DataQualityCandidateList.Add(duplicateCandidate);
        model.OutputDuplicateRiskList.Add(new OutputDuplicateRisk
        {
            Id = "runtime.output-duplicate",
            DataQualityCandidate = duplicateCandidate,
            HasDistinct = "false",
            HasGroupBy = "false",
            QualifiedJoinCount = "1",
        });
        AddRuntimeContext(
            model,
            duplicateCandidate,
            "SalesOrderCustomer",
            "dbo.SalesOrder",
            "CustomerId",
            "dbo.Customer",
            "CustomerId",
            "dbo.v_sales_order_customer");

        var relationship = new CorpusRelationship
        {
            Id = "Relationship.CustomerOrder",
            CanonicalSideAObjectName = "dbo.Customer",
            CanonicalSideBObjectName = "dbo.Order",
            CanonicalUndirectedSignature = "dbo.customer|dbo.order",
            OccurrenceCount = "9",
            TransformCount = "9",
        };
        var dominantPattern = CreateCorpusPattern(
            "Pattern.CustomerOrder.Dominant",
            relationship,
            "CustomerId=CustomerId",
            "dbo.Customer->dbo.Order",
            "8",
            "0.888889",
            "true");
        var outlierPattern = CreateCorpusPattern(
            "Pattern.CustomerOrder.Outlier",
            relationship,
            "CustomerId=CustomerId|RegionId=RegionId",
            "dbo.Customer->dbo.Order",
            "1",
            "0.111111",
            "false");
        var semanticCandidate = new DataQualityCandidate
        {
            Id = "MinorityJoinPattern.CustomerOrder",
            Name = "Minority join pattern",
            Status = CandidateStatuses.Promoted,
            Rationale = "The observed join differs from the dominant relationship pattern.",
        };

        model.DataQualityCandidateList.Add(semanticCandidate);
        model.CorpusRelationshipList.Add(relationship);
        model.CorpusRelationshipPatternList.AddRange([dominantPattern, outlierPattern]);
        model.MinorityJoinPatternList.Add(new MinorityJoinPattern
        {
            Id = "semantic.minority-join",
            DataQualityCandidate = semanticCandidate,
            DominantPattern = dominantPattern,
            OutlierPattern = outlierPattern,
        });
        model.DataQualityCandidateEvidenceList.Add(new DataQualityCandidateEvidence
        {
            Id = "Evidence.CustomerOrder.Outlier",
            DataQualityCandidate = semanticCandidate,
            CorpusRelationship = relationship,
            CorpusRelationshipPattern = outlierPattern,
            EvidenceType = "CorpusOutlier",
            Explanation = "One transform uses an additional RegionId predicate.",
            OccurrenceCount = "1",
            TransformCount = "1",
            ConsensusRatio = "0.888889",
            OutlierRatio = "0.111111",
            EvidenceQuality = "High",
            ConfidenceBand = "High",
            ConfidenceReason = "Observed across independent transforms.",
            EvidenceDiversitySummary = "9 transforms and 2 relationship patterns.",
            DistinctTransformCount = "9",
            DistinctSourceTransformCount = "9",
            DistinctSourceObjectCount = "2",
            DistinctRelationshipPatternCount = "2",
            EffectiveTransformCount = "9",
        });
        model.DataQualityCandidateEvidenceList.Add(new DataQualityCandidateEvidence
        {
            Id = "Evidence.CustomerOrder.Outlier.Second",
            DataQualityCandidate = semanticCandidate,
            CorpusRelationship = relationship,
            CorpusRelationshipPattern = outlierPattern,
            EvidenceType = "CorpusOutlier",
            Explanation = "A second transform uses the same minority relationship pattern.",
            OccurrenceCount = "1",
            TransformCount = "1",
            ConsensusRatio = "0.888889",
            OutlierRatio = "0.111111",
            EvidenceQuality = "High",
            ConfidenceBand = "High",
            ConfidenceReason = "Observed across independent transforms.",
            EvidenceDiversitySummary = "9 transforms and 2 relationship patterns.",
            DistinctTransformCount = "9",
            DistinctSourceTransformCount = "9",
            DistinctSourceObjectCount = "2",
            DistinctRelationshipPatternCount = "2",
            EffectiveTransformCount = "9",
        });

        model.CorpusRelationshipPatternOccurrenceLinkList.AddRange(
        [
            new CorpusRelationshipPatternOccurrenceLink
            {
                Id = "Pattern.CustomerOrder.Dominant.OccurrenceLink",
                CorpusRelationshipPattern = dominantPattern,
                JoinPatternOccurrence = model.JoinPatternOccurrenceList.Single(row => row.Id == "Occurrence.CustomerOrder.1"),
            },
            new CorpusRelationshipPatternOccurrenceLink
            {
                Id = "Pattern.CustomerOrder.Outlier.OccurrenceLink",
                CorpusRelationshipPattern = outlierPattern,
                JoinPatternOccurrence = model.JoinPatternOccurrenceList.Single(row => row.Id == "Occurrence.CustomerRegion.1"),
            },
        ]);

        var incompleteCandidate = CreateSemanticCandidate(
            model,
            "IncompleteCompositeJoin.CustomerOrder",
            "Incomplete composite relationship key.");
        model.IncompleteCompositeJoinList.Add(new IncompleteCompositeJoin
        {
            Id = "semantic.incomplete-composite",
            DataQualityCandidate = incompleteCandidate,
            DominantPattern = dominantPattern,
            OutlierPattern = outlierPattern,
        });
        AddSemanticEvidence(model, incompleteCandidate, relationship, outlierPattern, "The minority join omits a dominant key part.");

        var suspiciousExtraCandidate = CreateSemanticCandidate(
            model,
            "SuspiciousExtraJoinPredicate.CustomerOrder",
            "Suspicious additional relationship predicate.");
        model.SuspiciousExtraJoinPredicateList.Add(new SuspiciousExtraJoinPredicate
        {
            Id = "semantic.suspicious-extra",
            DataQualityCandidate = suspiciousExtraCandidate,
            DominantPattern = dominantPattern,
            OutlierPattern = outlierPattern,
        });
        AddSemanticEvidence(model, suspiciousExtraCandidate, relationship, outlierPattern, "The minority join adds an unusual predicate.");

        var missingFilterCandidate = CreateSemanticCandidate(
            model,
            "MissingCommonFilter.CustomerOrder",
            "A commonly observed filter is absent.");
        model.MissingCommonFilterList.Add(new MissingCommonFilter
        {
            Id = "semantic.missing-filter",
            DataQualityCandidate = missingFilterCandidate,
            DominantPattern = dominantPattern,
            OutlierPattern = outlierPattern,
            BaseObjectName = "dbo.Order",
            CommonPredicateSignature = "OrderStatus=Open",
            CommonPredicateDisplay = "OrderStatus = 'Open'",
        });
        AddSemanticEvidence(model, missingFilterCandidate, relationship, outlierPattern, "The minority transform omits the common open-order filter.");

        var dominantEquivalence = new CorpusColumnEquivalence
        {
            Id = "Equivalence.CustomerId.CustomerId",
            CanonicalSideAColumnName = "dbo.Customer.CustomerId",
            CanonicalSideBColumnName = "dbo.Order.CustomerId",
            CanonicalUndirectedSignature = "dbo.customer.customerid|dbo.order.customerid",
            OccurrenceCount = "8",
            TransformCount = "8",
        };
        var outlierEquivalence = new CorpusColumnEquivalence
        {
            Id = "Equivalence.CustomerId.RegionId",
            CanonicalSideAColumnName = "dbo.Customer.CustomerId",
            CanonicalSideBColumnName = "dbo.Order.RegionId",
            CanonicalUndirectedSignature = "dbo.customer.customerid|dbo.order.regionid",
            OccurrenceCount = "1",
            TransformCount = "1",
        };
        model.CorpusColumnEquivalenceList.AddRange([dominantEquivalence, outlierEquivalence]);
        var minorityEquivalenceCandidate = CreateSemanticCandidate(
            model,
            "MinorityColumnEquivalence.CustomerOrder",
            "A minority column equivalence differs from the corpus majority.");
        model.MinorityColumnEquivalenceList.Add(new MinorityColumnEquivalence
        {
            Id = "semantic.minority-equivalence",
            DataQualityCandidate = minorityEquivalenceCandidate,
            DominantEquivalence = dominantEquivalence,
            OutlierEquivalence = outlierEquivalence,
        });
        AddSemanticEvidence(model, minorityEquivalenceCandidate, relationship, null, "CustomerId is unusually equated with RegionId.");

        var innerOptionalCandidate = CreateSemanticCandidate(
            model,
            "InnerJoinAgainstUsuallyOptionalRelationship.CustomerOrder",
            "An optional relationship is joined as mandatory.");
        model.InnerJoinAgainstUsuallyOptionalRelationshipList.Add(new InnerJoinAgainstUsuallyOptionalRelationship
        {
            Id = "semantic.inner-against-optional",
            DataQualityCandidate = innerOptionalCandidate,
            CorpusRelationshipPattern = dominantPattern,
        });
        AddSemanticEvidence(model, innerOptionalCandidate, relationship, dominantPattern, "A usually optional relationship is inner joined.");

        var leftMandatoryCandidate = CreateSemanticCandidate(
            model,
            "LeftJoinAgainstUsuallyMandatoryRelationship.CustomerOrder",
            "A mandatory relationship is joined as optional.");
        model.LeftJoinAgainstUsuallyMandatoryRelationshipList.Add(new LeftJoinAgainstUsuallyMandatoryRelationship
        {
            Id = "semantic.left-against-mandatory",
            DataQualityCandidate = leftMandatoryCandidate,
            CorpusRelationshipPattern = dominantPattern,
        });
        AddSemanticEvidence(model, leftMandatoryCandidate, relationship, dominantPattern, "A usually mandatory relationship is left joined.");

        var impliedFanoutCandidate = CreateSemanticCandidate(
            model,
            "ImpliedJoinFanoutRisk.CustomerOrder",
            "The dominant relationship carries fanout evidence.");
        model.ImpliedJoinFanoutRiskList.Add(new ImpliedJoinFanoutRisk
        {
            Id = "semantic.implied-fanout",
            DataQualityCandidate = impliedFanoutCandidate,
            DominantPattern = dominantPattern,
        });
        AddSemanticEvidence(model, impliedFanoutCandidate, relationship, dominantPattern, "The relationship repeatedly carries fanout signals.");

        var impliedDuplicateCandidate = CreateSemanticCandidate(
            model,
            "ImpliedOutputDuplicateRisk.CustomerOrder",
            "The dominant relationship carries duplicate-output evidence.");
        model.ImpliedOutputDuplicateRiskList.Add(new ImpliedOutputDuplicateRisk
        {
            Id = "semantic.implied-output-duplicate",
            DataQualityCandidate = impliedDuplicateCandidate,
            DominantPattern = dominantPattern,
        });
        AddSemanticEvidence(model, impliedDuplicateCandidate, relationship, dominantPattern, "The relationship repeatedly carries duplicate-output signals.");

        var impliedRelationship = new CorpusRelationship
        {
            Id = "Relationship.AccountInvoice",
            CanonicalSideAObjectName = "dbo.Account",
            CanonicalSideBObjectName = "dbo.Invoice",
            CanonicalUndirectedSignature = "dbo.account|dbo.invoice",
            OccurrenceCount = "3",
            TransformCount = "3",
        };
        var impliedDominantPattern = CreateCorpusPattern(
            "Pattern.AccountInvoice.Dominant",
            impliedRelationship,
            "AccountId=AccountId",
            "dbo.Account->dbo.Invoice",
            "3",
            "1.000000",
            "true");
        model.CorpusRelationshipList.Add(impliedRelationship);
        model.CorpusRelationshipPatternList.Add(impliedDominantPattern);

        var impliedForeignKeyCandidate = new DataQualityCandidate
        {
            Id = "ImpliedForeignKeyMissingReference.AccountInvoice",
            Name = "Implied foreign-key missing reference",
            Status = CandidateStatuses.Promoted,
            Rationale = "Check the inferred invoice-to-account reference.",
        };
        var impliedUniqueCandidate = new DataQualityCandidate
        {
            Id = "ImpliedUniqueKeyViolation.AccountInvoice",
            Name = "Implied unique-key violation",
            Status = CandidateStatuses.Promoted,
            Rationale = "Check the inferred account lookup key.",
        };
        model.DataQualityCandidateList.AddRange([impliedForeignKeyCandidate, impliedUniqueCandidate]);
        model.ImpliedForeignKeyMissingReferenceList.Add(new ImpliedForeignKeyMissingReference
        {
            Id = "runtime.implied-foreign-key",
            DataQualityCandidate = impliedForeignKeyCandidate,
            DominantPattern = impliedDominantPattern,
        });
        model.ImpliedUniqueKeyViolationList.Add(new ImpliedUniqueKeyViolation
        {
            Id = "runtime.implied-unique",
            DataQualityCandidate = impliedUniqueCandidate,
            DominantPattern = impliedDominantPattern,
        });
        AddCorpusRuntimeContext(
            model,
            impliedDominantPattern,
            "AccountInvoice.Primary.1",
            "dbo.Account",
            "AccountId",
            "dbo.Invoice",
            "AccountId",
            "dbo.v_account_invoices_a");
        AddCorpusRuntimeContext(
            model,
            impliedDominantPattern,
            "AccountInvoice.Primary.2",
            "dbo.Account",
            "AccountId",
            "dbo.Invoice",
            "AccountId",
            "dbo.v_account_invoices_b");
        AddCorpusRuntimeContext(
            model,
            impliedDominantPattern,
            "AccountInvoice.Reversed",
            "dbo.Invoice",
            "AccountId",
            "dbo.Account",
            "AccountId",
            "dbo.v_invoice_accounts_reversed");

        return model;
    }

    private static void AddRuntimeContext(
        MetaDataQualityModel model,
        DataQualityCandidate candidate,
        string contextId,
        string firstObjectName,
        string firstColumnName,
        string secondObjectName,
        string secondColumnName,
        string transformName)
    {
        var pattern = new JoinPattern
        {
            Id = $"JoinPattern.{contextId}",
            CanonicalSignature = $"left={firstObjectName};right={secondObjectName};key={firstColumnName}={secondColumnName}",
            QualifiedJoinType = "Inner",
            ContainsEqualityPredicate = "true",
            EqualityPredicateCount = "1",
        };
        var keyPart = new JoinPatternKeyPart
        {
            Id = $"JoinPattern.{contextId}.KeyPart.1",
            JoinPattern = pattern,
            Ordinal = "0",
            BooleanComparisonExpressionId = $"comparison.{contextId}",
            FirstExpressionId = $"scalar.{contextId}.first",
            SecondExpressionId = $"scalar.{contextId}.second",
            FirstExpressionDisplay = $"first.{firstColumnName}",
            SecondExpressionDisplay = $"second.{secondColumnName}",
            FirstJoinInputObjectName = firstObjectName,
            SecondJoinInputObjectName = secondObjectName,
            FirstJoinInputColumnName = firstColumnName,
            SecondJoinInputColumnName = secondColumnName,
        };
        var firstTableReferenceId = $"table.{contextId}.first";
        var secondTableReferenceId = $"table.{contextId}.second";
        var occurrence = new JoinPatternOccurrence
        {
            Id = $"Occurrence.{contextId}.1",
            JoinPattern = pattern,
            TransformScriptId = $"Transform.{contextId}",
            TransformScriptName = transformName,
            QueryExpressionId = $"query.{contextId}",
            QuerySpecificationId = $"query-specification.{contextId}",
            JoinTableReferenceId = $"join.{contextId}",
            QualifiedJoinId = $"qualified-join.{contextId}",
            SearchConditionBooleanExpressionId = $"boolean.{contextId}",
            FirstTableReferenceId = firstTableReferenceId,
            SecondTableReferenceId = secondTableReferenceId,
        };

        model.JoinPatternList.Add(pattern);
        model.JoinPatternKeyPartList.Add(keyPart);
        AddObjectParts(model, keyPart, "First", firstObjectName);
        AddObjectParts(model, keyPart, "Second", secondObjectName);
        model.JoinPatternOccurrenceList.Add(occurrence);
        model.JoinPatternOccurrenceBaseTableList.AddRange(
        [
            CreateBaseTable($"base.{contextId}.first", occurrence, firstTableReferenceId, firstObjectName),
            CreateBaseTable($"base.{contextId}.second", occurrence, secondTableReferenceId, secondObjectName),
        ]);
        model.DataQualityCandidateJoinPatternLinkList.Add(new DataQualityCandidateJoinPatternLink
        {
            Id = $"{candidate.Id}.PatternLink.{contextId}",
            DataQualityCandidate = candidate,
            JoinPattern = pattern,
        });
    }

    private static void AddCorpusRuntimeContext(
        MetaDataQualityModel model,
        CorpusRelationshipPattern corpusPattern,
        string contextId,
        string firstObjectName,
        string firstColumnName,
        string secondObjectName,
        string secondColumnName,
        string transformName)
    {
        var pattern = new JoinPattern
        {
            Id = $"JoinPattern.{contextId}",
            CanonicalSignature = $"left={firstObjectName};right={secondObjectName};key={firstColumnName}={secondColumnName}",
            QualifiedJoinType = "Inner",
            ContainsEqualityPredicate = "true",
            EqualityPredicateCount = "1",
        };
        var keyPart = new JoinPatternKeyPart
        {
            Id = $"JoinPattern.{contextId}.KeyPart.1",
            JoinPattern = pattern,
            Ordinal = "0",
            BooleanComparisonExpressionId = $"comparison.{contextId}",
            FirstExpressionId = $"scalar.{contextId}.first",
            SecondExpressionId = $"scalar.{contextId}.second",
            FirstExpressionDisplay = $"first.{firstColumnName}",
            SecondExpressionDisplay = $"second.{secondColumnName}",
            FirstJoinInputObjectName = firstObjectName,
            SecondJoinInputObjectName = secondObjectName,
            FirstJoinInputColumnName = firstColumnName,
            SecondJoinInputColumnName = secondColumnName,
        };
        var firstTableReferenceId = $"table.{contextId}.first";
        var secondTableReferenceId = $"table.{contextId}.second";
        var occurrence = new JoinPatternOccurrence
        {
            Id = $"Occurrence.{contextId}.1",
            JoinPattern = pattern,
            TransformScriptId = $"Transform.{contextId}",
            TransformScriptName = transformName,
            QueryExpressionId = $"query.{contextId}",
            QuerySpecificationId = $"query-specification.{contextId}",
            JoinTableReferenceId = $"join.{contextId}",
            QualifiedJoinId = $"qualified-join.{contextId}",
            SearchConditionBooleanExpressionId = $"boolean.{contextId}",
            FirstTableReferenceId = firstTableReferenceId,
            SecondTableReferenceId = secondTableReferenceId,
        };

        model.JoinPatternList.Add(pattern);
        model.JoinPatternKeyPartList.Add(keyPart);
        AddObjectParts(model, keyPart, "First", firstObjectName);
        AddObjectParts(model, keyPart, "Second", secondObjectName);
        model.JoinPatternOccurrenceList.Add(occurrence);
        model.JoinPatternOccurrenceBaseTableList.AddRange(
        [
            CreateBaseTable($"base.{contextId}.first", occurrence, firstTableReferenceId, firstObjectName),
            CreateBaseTable($"base.{contextId}.second", occurrence, secondTableReferenceId, secondObjectName),
        ]);
        model.CorpusRelationshipPatternOccurrenceLinkList.Add(new CorpusRelationshipPatternOccurrenceLink
        {
            Id = $"{corpusPattern.Id}.OccurrenceLink.{contextId}",
            CorpusRelationshipPattern = corpusPattern,
            JoinPatternOccurrence = occurrence,
        });
    }

    private static void AddObjectParts(
        MetaDataQualityModel model,
        JoinPatternKeyPart keyPart,
        string inputSide,
        string objectName)
    {
        var parts = objectName.Split('.', StringSplitOptions.RemoveEmptyEntries);
        for (var index = 0; index < parts.Length; index++)
        {
            model.JoinPatternKeyPartInputObjectIdentifierPartList.Add(
                CreateObjectPart(
                    $"{keyPart.Id}.{inputSide}.{index}",
                    keyPart,
                    inputSide,
                    index.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    parts[index]));
        }
    }

    private static JoinPatternKeyPartInputObjectIdentifierPart CreateObjectPart(
        string id,
        JoinPatternKeyPart keyPart,
        string inputSide,
        string ordinal,
        string value) =>
        new()
        {
            Id = id,
            JoinPatternKeyPart = keyPart,
            InputSide = inputSide,
            Ordinal = ordinal,
            Value = value,
        };

    private static JoinPatternOccurrenceBaseTable CreateBaseTable(
        string id,
        JoinPatternOccurrence occurrence,
        string tableReferenceId,
        string objectName) =>
        new()
        {
            Id = id,
            JoinPatternOccurrence = occurrence,
            BaseNamedTableReferenceId = $"{tableReferenceId}.named",
            BaseObjectName = objectName,
            BaseSchemaObjectNameId = $"{tableReferenceId}.object",
            BaseTableReferenceId = tableReferenceId,
            JoinInputTableReferenceId = tableReferenceId,
            ResolutionDepth = "0",
        };

    private static CorpusRelationshipPattern CreateCorpusPattern(
        string id,
        CorpusRelationship relationship,
        string keySignature,
        string directionalSignature,
        string occurrenceCount,
        string ratio,
        string isDominant) =>
        new()
        {
            Id = id,
            CorpusRelationship = relationship,
            CanonicalKeyPartSetSignature = keySignature,
            RepresentativeDirectionalSignature = directionalSignature,
            KeyPartCount = "1",
            OccurrenceCount = occurrenceCount,
            TransformCount = occurrenceCount,
            OccurrenceRatio = ratio,
            IsDominant = isDominant,
        };

    private static DataQualityCandidate CreateSemanticCandidate(
        MetaDataQualityModel model,
        string id,
        string rationale)
    {
        var candidate = new DataQualityCandidate
        {
            Id = id,
            Name = id,
            Status = CandidateStatuses.Promoted,
            Rationale = rationale,
        };
        model.DataQualityCandidateList.Add(candidate);
        return candidate;
    }

    private static void AddSemanticEvidence(
        MetaDataQualityModel model,
        DataQualityCandidate candidate,
        CorpusRelationship relationship,
        CorpusRelationshipPattern? pattern,
        string explanation)
    {
        model.DataQualityCandidateEvidenceList.Add(new DataQualityCandidateEvidence
        {
            Id = $"Evidence.{candidate.Id}",
            DataQualityCandidate = candidate,
            CorpusRelationship = relationship,
            CorpusRelationshipPattern = pattern,
            EvidenceType = $"{candidate.Id}.Evidence",
            Explanation = explanation,
            OccurrenceCount = "1",
            TransformCount = "1",
            ConsensusRatio = "0.888889",
            OutlierRatio = "0.111111",
            EvidenceQuality = "High",
            ConfidenceBand = "High",
            ConfidenceReason = "Observed across independent transforms.",
            EvidenceDiversitySummary = "9 transforms and 2 relationship patterns.",
            DistinctTransformCount = "9",
            DistinctSourceTransformCount = "9",
            DistinctSourceObjectCount = "2",
            DistinctRelationshipPatternCount = "2",
            EffectiveTransformCount = "9",
        });
    }

    private static DataQualityCandidateEvidence CopyEvidence(
        DataQualityCandidateEvidence source,
        string id,
        DataQualityCandidate candidate) =>
        new()
        {
            Id = id,
            DataQualityCandidate = candidate,
            CorpusRelationship = source.CorpusRelationship,
            CorpusRelationshipPattern = source.CorpusRelationshipPattern,
            EvidenceType = source.EvidenceType,
            Explanation = source.Explanation,
            OccurrenceCount = source.OccurrenceCount,
            TransformCount = source.TransformCount,
            ConsensusRatio = source.ConsensusRatio,
            OutlierRatio = source.OutlierRatio,
            EvidenceQuality = source.EvidenceQuality,
            ConfidenceBand = source.ConfidenceBand,
            ConfidenceReason = source.ConfidenceReason,
            EvidenceDiversitySummary = source.EvidenceDiversitySummary,
            DistinctTransformCount = source.DistinctTransformCount,
            DistinctSourceTransformCount = source.DistinctSourceTransformCount,
            DistinctSourceObjectCount = source.DistinctSourceObjectCount,
            DistinctRelationshipPatternCount = source.DistinctRelationshipPatternCount,
            EffectiveTransformCount = source.EffectiveTransformCount,
        };

    private static IReadOnlyList<SemanticFamilyWitness> SemanticFamilyWitnesses() =>
    [
        new(
            "IncompleteCompositeJoin.CustomerOrder",
            "IncompleteCompositeJoin",
            "Incomplete composite join (semantic review)",
            "Incomplete composite join",
            "Composite join outlier",
            "The minority join omits a dominant key part.",
            "dbo.v_customer_orders, dbo.v_customer_regions"),
        new(
            "SuspiciousExtraJoinPredicate.CustomerOrder",
            "SuspiciousExtraJoinPredicate",
            "Suspicious extra join predicate (semantic review)",
            "Suspicious extra join predicate",
            "Join predicate outlier",
            "The minority join adds an unusual predicate.",
            "dbo.v_customer_orders, dbo.v_customer_regions"),
        new(
            "MissingCommonFilter.CustomerOrder",
            "MissingCommonFilter",
            "Missing common filter (semantic review)",
            "Missing common filter",
            "Filter consensus outlier",
            "The minority transform omits the common open-order filter.",
            "(unknown transform view)"),
        new(
            "MinorityColumnEquivalence.CustomerOrder",
            "MinorityColumnEquivalence",
            "Minority column equivalence (semantic review)",
            "Minority column equivalence",
            "Column equivalence outlier",
            "CustomerId is unusually equated with RegionId.",
            "(unknown transform view)"),
        new(
            "InnerJoinAgainstUsuallyOptionalRelationship.CustomerOrder",
            "InnerJoinAgainstUsuallyOptionalRelationship",
            "Inner join against usually optional side (semantic review)",
            "Inner join against usually optional relationship",
            "Optionality drift",
            "A usually optional relationship is inner joined.",
            "dbo.v_customer_orders"),
        new(
            "LeftJoinAgainstUsuallyMandatoryRelationship.CustomerOrder",
            "LeftJoinAgainstUsuallyMandatoryRelationship",
            "Left join against usually mandatory side (semantic review)",
            "Left join against usually mandatory relationship",
            "Optionality drift",
            "A usually mandatory relationship is left joined.",
            "dbo.v_customer_orders"),
        new(
            "ImpliedJoinFanoutRisk.CustomerOrder",
            "ImpliedJoinFanoutRisk",
            "Implied join fanout risk (semantic review)",
            "Relationship fanout risk",
            "Join cardinality",
            "The relationship repeatedly carries fanout signals.",
            "dbo.v_customer_orders"),
        new(
            "ImpliedOutputDuplicateRisk.CustomerOrder",
            "ImpliedOutputDuplicateRisk",
            "Implied output duplicate risk (semantic review)",
            "Relationship duplicate-output risk",
            "Output uniqueness",
            "The relationship repeatedly carries duplicate-output signals.",
            "dbo.v_customer_orders"),
    ];

    private static void AssertSemanticFamilySemantics(string sql, SemanticFamilyWitness witness)
    {
        Assert.Contains(witness.CandidateId, sql, StringComparison.Ordinal);
        Assert.Contains($"N'{witness.CandidateKind}'", sql, StringComparison.Ordinal);
        Assert.Contains(witness.Issue, sql, StringComparison.Ordinal);
        Assert.Contains(witness.FindingTitle, sql, StringComparison.Ordinal);
        Assert.Contains(witness.FindingCategory, sql, StringComparison.Ordinal);
        Assert.Contains(witness.Explanation, sql, StringComparison.Ordinal);
        Assert.Contains(witness.TransformViews, sql, StringComparison.Ordinal);
        Assert.Contains("SemanticReviewFinding", sql, StringComparison.Ordinal);
    }

    private static void AssertRuntimeSemantics(string sql)
    {
        Assert.Contains("Missing referenced rows", sql, StringComparison.Ordinal);
        Assert.Contains("FROM [dbo].[Order] AS [dq_right]", sql, StringComparison.Ordinal);
        Assert.Contains("WHERE NOT EXISTS", sql, StringComparison.Ordinal);
        Assert.Contains("FROM [dbo].[Customer] AS [dq_left]", sql, StringComparison.Ordinal);
        Assert.Contains("[dq_left].[CustomerId] = [dq_right].[CustomerId]", sql, StringComparison.Ordinal);
        Assert.Contains("CustomerId=", sql, StringComparison.Ordinal);
        Assert.Contains("AS [KeyValues]", sql, StringComparison.Ordinal);
        Assert.Contains("AS [SuspectCount]", sql, StringComparison.Ordinal);
    }

    private static void AssertSemanticReviewSemantics(string sql)
    {
        Assert.Contains("SemanticReviewFinding", sql, StringComparison.Ordinal);
        Assert.Contains("Minority join pattern (semantic review)", sql, StringComparison.Ordinal);
        Assert.Contains("MinorityJoinPattern.CustomerOrder", sql, StringComparison.Ordinal);
        Assert.Contains("dbo.Customer <-> dbo.Order", sql, StringComparison.Ordinal);
        Assert.Contains("CustomerId=CustomerId | dbo.Customer->dbo.Order", sql, StringComparison.Ordinal);
        Assert.Contains("CustomerId=CustomerId|RegionId=RegionId | dbo.Customer->dbo.Order", sql, StringComparison.Ordinal);
        Assert.Contains("CAST(0.888889 AS decimal(18,6))", sql, StringComparison.Ordinal);
        Assert.Contains("CAST(0.111111 AS decimal(18,6))", sql, StringComparison.Ordinal);
        Assert.Contains("One transform uses an additional RegionId predicate.", sql, StringComparison.Ordinal);
    }

    private static void AssertGroupedRuntimeSemantics(string sql)
    {
        Assert.Contains("Row multiplication", sql, StringComparison.Ordinal);
        Assert.Contains("FROM [dbo].[Product] AS [dq_left]", sql, StringComparison.Ordinal);
        Assert.Contains("INNER JOIN [dbo].[OrderLine] AS [dq_right]", sql, StringComparison.Ordinal);
        Assert.Contains("[dq_left].[ProductId] = [dq_right].[ProductId]", sql, StringComparison.Ordinal);
        Assert.Contains("CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]", sql, StringComparison.Ordinal);
        Assert.Contains("GROUP BY [dq_left].[ProductId]", sql, StringComparison.Ordinal);
        Assert.Contains("HAVING COUNT_BIG(*) > 1", sql, StringComparison.Ordinal);
    }

    private static void AssertOuterJoinNullSemantics(string sql)
    {
        Assert.Contains("Unexpected NULLs from outer joins", sql, StringComparison.Ordinal);
        Assert.Contains("FROM [dbo].[Order] AS [dq_left]", sql, StringComparison.Ordinal);
        Assert.Contains("WHERE NOT EXISTS", sql, StringComparison.Ordinal);
        Assert.Contains("FROM [dbo].[Shipment] AS [dq_right]", sql, StringComparison.Ordinal);
        Assert.Contains("[dq_left].[OrderId] = [dq_right].[OrderId]", sql, StringComparison.Ordinal);
        Assert.Contains("OrderId=", sql, StringComparison.Ordinal);
    }

    private static void AssertOutputDuplicateSemantics(string sql)
    {
        Assert.Contains("Duplicate output rows", sql, StringComparison.Ordinal);
        Assert.Contains("FROM [dbo].[SalesOrder] AS [dq_left]", sql, StringComparison.Ordinal);
        Assert.Contains("INNER JOIN [dbo].[Customer] AS [dq_right]", sql, StringComparison.Ordinal);
        Assert.Contains("[dq_left].[CustomerId] = [dq_right].[CustomerId]", sql, StringComparison.Ordinal);
        Assert.Contains("GROUP BY [dq_left].[CustomerId]", sql, StringComparison.Ordinal);
        Assert.Contains("HAVING COUNT_BIG(*) > 1", sql, StringComparison.Ordinal);
    }

    private static void AssertImpliedForeignKeySemantics(string sql)
    {
        Assert.Contains("Implied missing referenced rows", sql, StringComparison.Ordinal);
        Assert.Contains("dbo.Invoice references dbo.Account", sql, StringComparison.Ordinal);
        Assert.Contains("FROM [dbo].[Invoice] AS [dq_right]", sql, StringComparison.Ordinal);
        Assert.Contains("FROM [dbo].[Account] AS [dq_left]", sql, StringComparison.Ordinal);
        Assert.Contains("[dq_left].[AccountId] = [dq_right].[AccountId]", sql, StringComparison.Ordinal);
        Assert.DoesNotContain("dbo.v_invoice_accounts_reversed", sql, StringComparison.Ordinal);
    }

    private static void AssertImpliedUniqueKeySemantics(string sql)
    {
        Assert.Contains("Implied unique-key violation", sql, StringComparison.Ordinal);
        Assert.Contains("dbo.Account expected unique for dbo.Invoice relationship", sql, StringComparison.Ordinal);
        Assert.Contains("FROM [dbo].[Account] AS [dq_lookup]", sql, StringComparison.Ordinal);
        Assert.Contains("CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]", sql, StringComparison.Ordinal);
        Assert.Contains("GROUP BY [dq_lookup].[AccountId]", sql, StringComparison.Ordinal);
        Assert.Contains("HAVING COUNT_BIG(*) > 1", sql, StringComparison.Ordinal);
    }

    private static void AssertDashboardSemantics(string sql)
    {
        string[] columns =
        [
            "DQView", "Issue", "FindingTitle", "FindingCategory", "OutputMode", "CandidateId",
            "CandidateKind", "Relationship", "RelationshipLabel", "ReferencingObject", "ReferencedObject",
            "CheckedObject", "SuspectSide", "SuspectObject", "LookupObject", "RelatedObject",
            "CorpusRelationship", "CorpusRelationshipPattern", "DominantPattern", "OutlierPattern",
            "TransformViews", "RowsReturned", "ResultRowCount", "FindingGroupCount", "TotalSuspectCount",
            "SuspectRowCount", "Explanation", "FindingExplanation", "EvidenceSummary",
            "EvidenceOccurrenceCount", "OutlierOccurrenceCount", "EvidenceTransformCount",
            "OutlierTransformCount", "EvidenceConsensusRatio", "DominantConsensusRatio",
            "EvidenceOutlierRatio", "OutlierRatio", "EvidenceQuality", "ConfidenceBand", "ConfidenceReason",
            "EvidenceDiversitySummary", "ConfidenceSummary", "DistinctTransformCount",
            "DistinctSourceTransformCount", "DistinctSourceObjectCount", "DistinctRelationshipPatternCount",
            "EffectiveTransformCount", "RecommendedAction", "RuntimeCountStatus", "GeneratedView", "ReviewQuery",
            "DetailQuery", "TransformViewQuery", "SupportingTransformQuery",
        ];

        Assert.Contains("CREATE OR ALTER VIEW [dq].[v_DataQualityReview]", sql, StringComparison.Ordinal);
        Assert.All(columns, column => Assert.Contains($"AS [{column}]", sql, StringComparison.Ordinal));
        Assert.Contains("CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned]", sql, StringComparison.Ordinal);
        Assert.Contains("CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount]", sql, StringComparison.Ordinal);
        Assert.Contains("CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus]", sql, StringComparison.Ordinal);
        Assert.Contains("CAST(N'Non-runtime (semantic review)' AS nvarchar(64)) AS [RuntimeCountStatus]", sql, StringComparison.Ordinal);
        Assert.Contains("dbo.Customer -> dbo.CustomerRegion; dbo.Customer -> dbo.Order", sql, StringComparison.Ordinal);
        Assert.Contains("dbo.Product -> dbo.OrderLine", sql, StringComparison.Ordinal);
        Assert.Contains("AS [RecommendedAction]", sql, StringComparison.Ordinal);
        Assert.Contains("AS [ReviewQuery]", sql, StringComparison.Ordinal);
    }

    private static void AssertFrozenAdventureWorksContract(string sql)
    {
        Assert.Contains("MetaDataQuality: Missing referenced rows", sql, StringComparison.Ordinal);
        Assert.Contains("WHERE NOT EXISTS", sql, StringComparison.Ordinal);
        Assert.Contains("AS [KeyValues]", sql, StringComparison.Ordinal);
        Assert.Contains("AS [SuspectCount]", sql, StringComparison.Ordinal);
        Assert.Contains("AS [DominantPattern]", sql, StringComparison.Ordinal);
        Assert.Contains("AS [OutlierPattern]", sql, StringComparison.Ordinal);
        Assert.Contains("AS [EvidenceConsensusRatio]", sql, StringComparison.Ordinal);
        Assert.Contains("AS [EvidenceOutlierRatio]", sql, StringComparison.Ordinal);
    }

    private static string FormatIssues(MetaWeaveScriptApplicationResult result) =>
        string.Join(
            Environment.NewLine,
            result.Issues.Select(issue => $"{issue.Code}: {issue.Message}"));

    private static int CountOccurrences(string value, string token)
    {
        var count = 0;
        var offset = 0;
        while ((offset = value.IndexOf(token, offset, StringComparison.Ordinal)) >= 0)
        {
            count++;
            offset += token.Length;
        }

        return count;
    }

    private sealed record ExecutionProbe(
        MetaWeaveScriptApplicationResult Result,
        MetaWeaveScriptDirection Direction,
        string WeavePath,
        TimeSpan Elapsed);

    private sealed record SemanticFamilyWitness(
        string CandidateId,
        string CandidateKind,
        string Issue,
        string FindingTitle,
        string FindingCategory,
        string Explanation,
        string TransformViews);
}
