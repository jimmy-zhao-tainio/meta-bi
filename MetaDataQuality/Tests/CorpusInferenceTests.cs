using System.Globalization;
using MetaConvert.DataQualityToSql;
using MetaDataQuality;
using MetaDataQuality.Core;

namespace MetaDataQuality.Tests;

public sealed class CorpusInferenceTests
{
    [Fact]
    public void CorpusInference_CanonicalUndirectedSides_AreDeterministic_AndDirectionalEvidenceIsPreserved()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.OrderCustomer",
            keyPairs:
            [
                ("o.CustomerId", "c.CustomerId"),
            ]);
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ]);

        AddOccurrence(
            model,
            occurrenceId: "Occ.1",
            patternId: "JoinPattern.OrderCustomer",
            scriptName: "Script.OrderCustomer",
            leftTable: "sales.Order",
            rightTable: "sales.Customer");
        AddOccurrence(
            model,
            occurrenceId: "Occ.2",
            patternId: "JoinPattern.CustomerOrder",
            scriptName: "Script.CustomerOrder",
            leftTable: "sales.Customer",
            rightTable: "sales.Order");

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildTinyThresholds());

        var relationship = Assert.Single(model.CorpusRelationshipList);
        Assert.Equal("sales.Customer", relationship.CanonicalSideAObjectName);
        Assert.Equal("sales.Order", relationship.CanonicalSideBObjectName);

        var pattern = Assert.Single(model.CorpusRelationshipPatternList);
        Assert.Equal(2, model.CorpusRelationshipPatternOccurrenceLinkList.Count);
        Assert.Contains(
            model.CorpusRelationshipPatternOccurrenceLinkList,
            row => string.Equals(row.JoinPatternOccurrence.Id, "Occ.1", StringComparison.Ordinal));
        Assert.Contains(
            model.CorpusRelationshipPatternOccurrenceLinkList,
            row => string.Equals(row.JoinPatternOccurrence.Id, "Occ.2", StringComparison.Ordinal));
        Assert.Contains("->", pattern.RepresentativeDirectionalSignature, StringComparison.Ordinal);
    }

    [Fact]
    public void CorpusInference_SelfJoinCanonicalKeyPartSetSignature_IsOrientationInvariant()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.Self.LeftRight",
            keyPairs:
            [
                ("a.CustomerId", "b.ParentCustomerId"),
            ]);
        AddJoinPattern(
            model,
            patternId: "JoinPattern.Self.RightLeft",
            keyPairs:
            [
                ("b.ParentCustomerId", "a.CustomerId"),
            ]);

        AddOccurrence(
            model,
            occurrenceId: "Occ.Self.1",
            patternId: "JoinPattern.Self.LeftRight",
            scriptName: "Script.Self.1",
            leftTable: "sales.Customer",
            rightTable: "sales.Customer");
        AddOccurrence(
            model,
            occurrenceId: "Occ.Self.2",
            patternId: "JoinPattern.Self.RightLeft",
            scriptName: "Script.Self.2",
            leftTable: "sales.Customer",
            rightTable: "sales.Customer");

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildTinyThresholds());

        Assert.Single(model.CorpusRelationshipList);
        Assert.Single(model.CorpusRelationshipPatternList);
    }

    [Fact]
    public void CorpusInference_GeneratesMinorityAndIncompleteComposite_WithRatios()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.Composite",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
                ("c.RegionId", "o.RegionId"),
            ]);
        AddJoinPattern(
            model,
            patternId: "JoinPattern.Subset",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ]);

        for (var i = 1; i <= 8; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.Composite.{i}",
                patternId: "JoinPattern.Composite",
                scriptName: $"Script.Composite.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");
        }

        for (var i = 1; i <= 2; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.Subset.{i}",
                patternId: "JoinPattern.Subset",
                scriptName: $"Script.Subset.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");
        }

        new MetaDataQualityCorpusInferenceService().Apply(model);

        var minority = Assert.Single(model.MinorityJoinPatternList);
        var incomplete = Assert.Single(model.IncompleteCompositeJoinList);
        Assert.Empty(model.SuspiciousExtraJoinPredicateList);
        Assert.Equal(minority.DominantPattern.Id, incomplete.DominantPattern.Id);
        Assert.Equal(minority.OutlierPattern.Id, incomplete.OutlierPattern.Id);

        var evidence = model.DataQualityCandidateEvidenceList
            .Single(row => string.Equals(row.DataQualityCandidate.Id, minority.DataQualityCandidate.Id, StringComparison.Ordinal));
        Assert.Equal(0.8d, ParseRatio(evidence.ConsensusRatio), 4);
        Assert.Equal(0.2d, ParseRatio(evidence.OutlierRatio), 4);
    }

    [Fact]
    public void CorpusInference_GeneratesSuspiciousExtra_AndGatesImpliedCandidatesByCounts()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.InvoiceDominant",
            keyPairs:
            [
                ("c.CustomerId", "i.CustomerId"),
            ]);
        AddJoinPattern(
            model,
            patternId: "JoinPattern.InvoiceOutlierExtra",
            keyPairs:
            [
                ("c.CustomerId", "i.CustomerId"),
                ("c.RegionId", "i.RegionId"),
            ]);
        AddJoinPattern(
            model,
            patternId: "JoinPattern.LowCount",
            keyPairs:
            [
                ("r.RegionId", "d.RegionId"),
            ]);

        for (var i = 1; i <= 8; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.Invoice.Dom.{i}",
                patternId: "JoinPattern.InvoiceDominant",
                scriptName: $"Script.Invoice.Dom.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Invoice");
        }

        AddOccurrence(
            model,
            occurrenceId: "Occ.Invoice.Outlier.1",
            patternId: "JoinPattern.InvoiceOutlierExtra",
            scriptName: "Script.Invoice.Outlier.1",
            leftTable: "sales.Customer",
            rightTable: "sales.Invoice");

        for (var i = 1; i <= 4; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.Low.{i}",
                patternId: "JoinPattern.LowCount",
                scriptName: $"Script.Low.{i}",
                leftTable: "sales.Region",
                rightTable: "sales.RegionDim");
        }

        new MetaDataQualityCorpusInferenceService().Apply(model);

        Assert.Single(model.SuspiciousExtraJoinPredicateList);
        Assert.Single(model.ImpliedForeignKeyMissingReferenceList);
        Assert.Single(model.ImpliedUniqueKeyViolationList);

        var impliedUnique = model.ImpliedUniqueKeyViolationList[0];
        var uniqueEvidence = model.DataQualityCandidateEvidenceList
            .Single(row => string.Equals(row.DataQualityCandidate.Id, impliedUnique.DataQualityCandidate.Id, StringComparison.Ordinal));
        Assert.Equal(0d, ParseRatio(uniqueEvidence.OutlierRatio), 4);

        var lowRelationship = model.CorpusRelationshipList.Single(
            row => row.CanonicalSideAObjectName.Contains("Region", StringComparison.OrdinalIgnoreCase)
                   && row.CanonicalSideBObjectName.Contains("RegionDim", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(
            model.DataQualityCandidateEvidenceList,
            row => string.Equals(row.CorpusRelationship.Id, lowRelationship.Id, StringComparison.Ordinal)
                   && string.Equals(row.EvidenceType, "ImpliedForeignKeyConsensus", StringComparison.Ordinal));
    }

    [Fact]
    public void CorpusInference_OptionalityDrift_DominantLeftWithMinorityInner_EmitsInnerAgainstUsuallyOptional()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Left",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ],
            joinType: "LeftOuter");
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Inner",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ],
            joinType: "Inner");

        for (var i = 1; i <= 8; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.CustomerOrder.Left.{i}",
                patternId: "JoinPattern.CustomerOrder.Left",
                scriptName: $"Script.CustomerOrder.Left.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");
        }

        AddOccurrence(
            model,
            occurrenceId: "Occ.CustomerOrder.Inner.1",
            patternId: "JoinPattern.CustomerOrder.Inner",
            scriptName: "Script.CustomerOrder.Inner.1",
            leftTable: "sales.Customer",
            rightTable: "sales.Order");

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildOptionalityOnlyThresholds());

        var detail = Assert.Single(model.InnerJoinAgainstUsuallyOptionalRelationshipList);
        Assert.Empty(model.LeftJoinAgainstUsuallyMandatoryRelationshipList);

        var candidate = model.DataQualityCandidateList.Single(
            row => string.Equals(row.Id, detail.DataQualityCandidate.Id, StringComparison.Ordinal));
        Assert.Equal(CandidateStatuses.Discovered, candidate.Status);

        var evidence = model.DataQualityCandidateEvidenceList.Single(
            row => string.Equals(row.DataQualityCandidate.Id, candidate.Id, StringComparison.Ordinal));
        Assert.Equal("OptionalityDriftInnerAgainstUsuallyOptional", evidence.EvidenceType);
        Assert.Equal(0.8889d, ParseRatio(evidence.ConsensusRatio), 4);
        Assert.Equal(0.1111d, ParseRatio(evidence.OutlierRatio), 4);
        Assert.Equal("1", evidence.OccurrenceCount);
    }

    [Fact]
    public void CorpusInference_OptionalityDrift_DominantInnerWithMinorityLeft_EmitsLeftAgainstUsuallyMandatory()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerInvoice.Inner",
            keyPairs:
            [
                ("c.CustomerId", "i.CustomerId"),
            ],
            joinType: "Inner");
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerInvoice.Left",
            keyPairs:
            [
                ("c.CustomerId", "i.CustomerId"),
            ],
            joinType: "LeftOuter");

        for (var i = 1; i <= 8; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.CustomerInvoice.Inner.{i}",
                patternId: "JoinPattern.CustomerInvoice.Inner",
                scriptName: $"Script.CustomerInvoice.Inner.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Invoice");
        }

        AddOccurrence(
            model,
            occurrenceId: "Occ.CustomerInvoice.Left.1",
            patternId: "JoinPattern.CustomerInvoice.Left",
            scriptName: "Script.CustomerInvoice.Left.1",
            leftTable: "sales.Customer",
            rightTable: "sales.Invoice");

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildOptionalityOnlyThresholds());

        var detail = Assert.Single(model.LeftJoinAgainstUsuallyMandatoryRelationshipList);
        Assert.Empty(model.InnerJoinAgainstUsuallyOptionalRelationshipList);

        var candidate = model.DataQualityCandidateList.Single(
            row => string.Equals(row.Id, detail.DataQualityCandidate.Id, StringComparison.Ordinal));
        Assert.Equal(CandidateStatuses.Discovered, candidate.Status);

        var evidence = model.DataQualityCandidateEvidenceList.Single(
            row => string.Equals(row.DataQualityCandidate.Id, candidate.Id, StringComparison.Ordinal));
        Assert.Equal("OptionalityDriftLeftAgainstUsuallyMandatory", evidence.EvidenceType);
        Assert.Equal(0.8889d, ParseRatio(evidence.ConsensusRatio), 4);
        Assert.Equal(0.1111d, ParseRatio(evidence.OutlierRatio), 4);
        Assert.Equal("1", evidence.OccurrenceCount);
    }

    [Fact]
    public void CorpusInference_OptionalityDrift_LowCountPattern_DoesNotEmitCandidates()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerInvoice.Inner.Low",
            keyPairs:
            [
                ("c.CustomerId", "i.CustomerId"),
            ],
            joinType: "Inner");
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerInvoice.Left.Low",
            keyPairs:
            [
                ("c.CustomerId", "i.CustomerId"),
            ],
            joinType: "LeftOuter");

        for (var i = 1; i <= 6; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.CustomerInvoice.Inner.Low.{i}",
                patternId: "JoinPattern.CustomerInvoice.Inner.Low",
                scriptName: $"Script.CustomerInvoice.Inner.Low.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Invoice");
        }

        AddOccurrence(
            model,
            occurrenceId: "Occ.CustomerInvoice.Left.Low.1",
            patternId: "JoinPattern.CustomerInvoice.Left.Low",
            scriptName: "Script.CustomerInvoice.Left.Low.1",
            leftTable: "sales.Customer",
            rightTable: "sales.Invoice");

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildOptionalityOnlyThresholds());

        Assert.Empty(model.InnerJoinAgainstUsuallyOptionalRelationshipList);
        Assert.Empty(model.LeftJoinAgainstUsuallyMandatoryRelationshipList);
    }

    [Fact]
    public void CorpusInference_OptionalityDrift_UnknownJoinKinds_DoNotCreateFalseCandidates()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Left.UnknownMix",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ],
            joinType: "LeftOuter");
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Cross.UnknownMix",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ],
            joinType: "Cross");

        for (var i = 1; i <= 8; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.CustomerOrder.Left.UnknownMix.{i}",
                patternId: "JoinPattern.CustomerOrder.Left.UnknownMix",
                scriptName: $"Script.CustomerOrder.Left.UnknownMix.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");
        }

        AddOccurrence(
            model,
            occurrenceId: "Occ.CustomerOrder.Cross.UnknownMix.1",
            patternId: "JoinPattern.CustomerOrder.Cross.UnknownMix",
            scriptName: "Script.CustomerOrder.Cross.UnknownMix.1",
            leftTable: "sales.Customer",
            rightTable: "sales.Order");

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildOptionalityOnlyThresholds());

        Assert.Empty(model.InnerJoinAgainstUsuallyOptionalRelationshipList);
        Assert.Empty(model.LeftJoinAgainstUsuallyMandatoryRelationshipList);
    }

    [Fact]
    public void CorpusInference_OptionalityDrift_MixedDirectionLeftJoinEvidence_DoesNotTreatAsSingleOptionality()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Left.CanonicalDirection",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ],
            joinType: "LeftOuter");
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Left.ReversedDirection",
            keyPairs:
            [
                ("o.CustomerId", "c.CustomerId"),
            ],
            joinType: "LeftOuter");
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Inner.MixedDirectionControl",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ],
            joinType: "Inner");

        for (var i = 1; i <= 8; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.CustomerOrder.Left.CanonicalDirection.{i}",
                patternId: "JoinPattern.CustomerOrder.Left.CanonicalDirection",
                scriptName: $"Script.CustomerOrder.Left.CanonicalDirection.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");
        }

        AddOccurrence(
            model,
            occurrenceId: "Occ.CustomerOrder.Left.ReversedDirection.1",
            patternId: "JoinPattern.CustomerOrder.Left.ReversedDirection",
            scriptName: "Script.CustomerOrder.Left.ReversedDirection.1",
            leftTable: "sales.Order",
            rightTable: "sales.Customer");

        AddOccurrence(
            model,
            occurrenceId: "Occ.CustomerOrder.Inner.MixedDirectionControl.1",
            patternId: "JoinPattern.CustomerOrder.Inner.MixedDirectionControl",
            scriptName: "Script.CustomerOrder.Inner.MixedDirectionControl.1",
            leftTable: "sales.Customer",
            rightTable: "sales.Order");

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildOptionalityOnlyThresholds());

        Assert.Empty(model.InnerJoinAgainstUsuallyOptionalRelationshipList);
    }

    [Fact]
    public void DataQualityToSql_SupportsPromotedSemanticAndImpliedFamilies()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(rootPath, "DataQualityWS");
        var outputPath = Path.Combine(rootPath, "DataQualityViews.sql");

        try
        {
            var model = MetaDataQualityModel.CreateEmpty();
            AddJoinPattern(
                model,
                patternId: "JoinPattern.Main",
                keyPairs:
                [
                    ("c.CustomerId", "o.CustomerId"),
                ]);
            AddJoinPattern(
                model,
                patternId: "JoinPattern.MainOutlier",
                keyPairs:
                [
                    ("c.CustomerId", "o.CustomerId"),
                    ("c.RegionId", "o.RegionId"),
                ]);

            for (var i = 1; i <= 8; i++)
            {
                AddOccurrence(
                    model,
                    occurrenceId: $"Occ.Main.Dom.{i}",
                    patternId: "JoinPattern.Main",
                    scriptName: $"Script.Main.Dom.{i}",
                    leftTable: "sales.Customer",
                    rightTable: "sales.Order");
            }

            AddOccurrence(
                model,
                occurrenceId: "Occ.Main.Outlier.1",
                patternId: "JoinPattern.MainOutlier",
                scriptName: "Script.Main.Outlier.1",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");

            new MetaDataQualityCorpusInferenceService().Apply(model);
            Assert.NotEmpty(model.MinorityJoinPatternList);
            Assert.NotEmpty(model.ImpliedForeignKeyMissingReferenceList);
            Assert.NotEmpty(model.ImpliedUniqueKeyViolationList);

            foreach (var candidate in model.DataQualityCandidateList)
            {
                candidate.Status = CandidateStatuses.Promoted;
            }

            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(model, workspacePath);

            var result = new DataQualityToSqlConverter().Convert(workspacePath, outputPath);
            var sql = File.ReadAllText(outputPath);
            Assert.True(result.CandidateViewCount >= 2);
            Assert.Contains("SemanticReviewFinding", sql, StringComparison.Ordinal);
            Assert.Contains("RuntimeCheck", sql, StringComparison.Ordinal);
            Assert.Contains("EvidenceOccurrenceCount", sql, StringComparison.Ordinal);
            Assert.Contains("OutlierOccurrenceCount", sql, StringComparison.Ordinal);
            Assert.Contains("DominantConsensusRatio", sql, StringComparison.Ordinal);
            Assert.Contains("SuspectRowCount", sql, StringComparison.Ordinal);
            Assert.Contains("FindingGroupCount", sql, StringComparison.Ordinal);
            Assert.Contains("FindingTitle", sql, StringComparison.Ordinal);
            Assert.Contains("FindingCategory", sql, StringComparison.Ordinal);
            Assert.Contains("FindingExplanation", sql, StringComparison.Ordinal);
            Assert.Contains("EvidenceSummary", sql, StringComparison.Ordinal);
            Assert.Contains("EvidenceQuality", sql, StringComparison.Ordinal);
            Assert.Contains("ConfidenceBand", sql, StringComparison.Ordinal);
            Assert.Contains("ConfidenceReason", sql, StringComparison.Ordinal);
            Assert.Contains("EvidenceDiversitySummary", sql, StringComparison.Ordinal);
            Assert.Contains("ConfidenceSummary", sql, StringComparison.Ordinal);
            Assert.Contains("DistinctTransformCount", sql, StringComparison.Ordinal);
            Assert.Contains("DistinctSourceTransformCount", sql, StringComparison.Ordinal);
            Assert.Contains("DistinctSourceObjectCount", sql, StringComparison.Ordinal);
            Assert.Contains("DistinctRelationshipPatternCount", sql, StringComparison.Ordinal);
            Assert.Contains("EffectiveTransformCount", sql, StringComparison.Ordinal);
            Assert.Contains("RelationshipLabel", sql, StringComparison.Ordinal);
            Assert.Contains("RuntimeCountStatus", sql, StringComparison.Ordinal);
            Assert.Contains("DetailQuery", sql, StringComparison.Ordinal);
            Assert.Contains("SupportingTransformQuery", sql, StringComparison.Ordinal);
            Assert.Contains("ReferencingObject", sql, StringComparison.Ordinal);
            Assert.Contains("ReferencedObject", sql, StringComparison.Ordinal);
            Assert.Contains("CheckedObject", sql, StringComparison.Ordinal);
            Assert.Contains("SuspectObject", sql, StringComparison.Ordinal);
            Assert.Contains("LookupObject", sql, StringComparison.Ordinal);
            Assert.Contains("RelatedObject", sql, StringComparison.Ordinal);
            Assert.Contains("references", sql, StringComparison.Ordinal);
            Assert.Contains("expected unique for", sql, StringComparison.Ordinal);
            Assert.Contains("Implied missing referenced rows", sql, StringComparison.Ordinal);
            Assert.Contains("Implied unique-key violation", sql, StringComparison.Ordinal);
            Assert.Contains("Minority join pattern (semantic review)", sql, StringComparison.Ordinal);
            Assert.Contains("Review semantic finding", sql, StringComparison.Ordinal);
            Assert.Contains("Non-runtime (semantic review)", sql, StringComparison.Ordinal);
            Assert.Contains("SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END)", sql, StringComparison.Ordinal);
            Assert.Contains("SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END)", sql, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public void DataQualityToSql_ImpliedCandidates_UseDominantDirectionalOrientationOnly()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(rootPath, "DataQualityWS");
        var outputPath = Path.Combine(rootPath, "DataQualityViews.sql");

        try
        {
            var model = MetaDataQualityModel.CreateEmpty();
            AddJoinPattern(
                model,
                patternId: "JoinPattern.CustomerInvoice.Dominant",
                keyPairs:
                [
                    ("c.CustomerId", "i.CustomerId"),
                ]);
            AddJoinPattern(
                model,
                patternId: "JoinPattern.CustomerInvoice.ReverseMinority",
                keyPairs:
                [
                    ("i.CustomerId", "c.CustomerId"),
                ]);

            for (var i = 1; i <= 8; i++)
            {
                AddOccurrence(
                    model,
                    occurrenceId: $"Occ.CustInv.Dom.{i}",
                    patternId: "JoinPattern.CustomerInvoice.Dominant",
                    scriptName: $"Script.CustInv.Dom.{i}",
                    leftTable: "sales.Customer",
                    rightTable: "sales.Invoice");
            }

            AddOccurrence(
                model,
                occurrenceId: "Occ.CustInv.Rev.1",
                patternId: "JoinPattern.CustomerInvoice.ReverseMinority",
                scriptName: "Script.CustInv.Rev.1",
                leftTable: "sales.Invoice",
                rightTable: "sales.Customer");

            new MetaDataQualityCorpusInferenceService().Apply(model);
            Assert.Single(model.ImpliedUniqueKeyViolationList);

            foreach (var candidate in model.DataQualityCandidateList)
            {
                candidate.Status = CandidateStatuses.Discovered;
            }

            var impliedUniqueCandidateId = model.ImpliedUniqueKeyViolationList[0].DataQualityCandidate.Id;
            var impliedUniqueCandidate = model.DataQualityCandidateList.Single(
                row => string.Equals(row.Id, impliedUniqueCandidateId, StringComparison.Ordinal));
            impliedUniqueCandidate.Status = CandidateStatuses.Promoted;

            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(model, workspacePath);

            var result = new DataQualityToSqlConverter().Convert(workspacePath, outputPath);
            var sql = File.ReadAllText(outputPath);
            Assert.True(result.CandidateViewCount >= 1);
            Assert.Contains("FROM [sales].[Customer] AS [dq_lookup]", sql, StringComparison.Ordinal);
            Assert.DoesNotContain("FROM [sales].[Invoice] AS [dq_lookup]", sql, StringComparison.Ordinal);
            Assert.Contains("expected unique for", sql, StringComparison.Ordinal);
            Assert.Contains("AS [CheckedObject]", sql, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public void DataQualityToSql_PromotedOptionalityDriftCandidates_RenderSemanticReviewFindings()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(rootPath, "DataQualityWS");
        var outputPath = Path.Combine(rootPath, "DataQualityViews.sql");

        try
        {
            var model = MetaDataQualityModel.CreateEmpty();
            AddJoinPattern(
                model,
                patternId: "JoinPattern.CustomerOrder.Left.ForSql",
                keyPairs:
                [
                    ("c.CustomerId", "o.CustomerId"),
                ],
                joinType: "LeftOuter");
            AddJoinPattern(
                model,
                patternId: "JoinPattern.CustomerOrder.Inner.ForSql",
                keyPairs:
                [
                    ("c.CustomerId", "o.CustomerId"),
                ],
                joinType: "Inner");

            for (var i = 1; i <= 8; i++)
            {
                AddOccurrence(
                    model,
                    occurrenceId: $"Occ.CustomerOrder.Left.ForSql.{i}",
                    patternId: "JoinPattern.CustomerOrder.Left.ForSql",
                    scriptName: $"Script.CustomerOrder.Left.ForSql.{i}",
                    leftTable: "sales.Customer",
                    rightTable: "sales.Order");
            }

            AddOccurrence(
                model,
                occurrenceId: "Occ.CustomerOrder.Inner.ForSql.1",
                patternId: "JoinPattern.CustomerOrder.Inner.ForSql",
                scriptName: "Script.CustomerOrder.Inner.ForSql.1",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");

            new MetaDataQualityCorpusInferenceService().Apply(model, BuildOptionalityOnlyThresholds());
            var detail = Assert.Single(model.InnerJoinAgainstUsuallyOptionalRelationshipList);

            foreach (var candidate in model.DataQualityCandidateList)
            {
                candidate.Status = CandidateStatuses.Discovered;
            }

            var promotedCandidate = model.DataQualityCandidateList.Single(
                row => string.Equals(row.Id, detail.DataQualityCandidate.Id, StringComparison.Ordinal));
            promotedCandidate.Status = CandidateStatuses.Promoted;
            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(model, workspacePath);

            var result = new DataQualityToSqlConverter().Convert(workspacePath, outputPath);
            var sql = File.ReadAllText(outputPath);
            Assert.True(result.CandidateViewCount >= 1);
            Assert.Contains("SemanticReviewFinding", sql, StringComparison.Ordinal);
            Assert.Contains("Inner join against usually optional side (semantic review)", sql, StringComparison.Ordinal);
            Assert.Contains("CAST(NULL AS bigint) AS [RowsReturned]", sql, StringComparison.Ordinal);
            Assert.Contains("CAST(NULL AS bigint) AS [TotalSuspectCount]", sql, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public void CorpusInference_EmitsImpliedFanoutAndOutputDuplicateRisk_WhenDominantPatternCarriesSignals()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Dominant",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ]);

        for (var i = 1; i <= 8; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.CustomerOrder.Dominant.{i}",
                patternId: "JoinPattern.CustomerOrder.Dominant",
                scriptName: $"Script.CustomerOrder.Dominant.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");
        }

        AddOccurrenceSignalsForPattern(model, "JoinPattern.CustomerOrder.Dominant", CandidateKinds.JoinMultiplicityExplosion);
        AddOccurrenceSignalsForPattern(model, "JoinPattern.CustomerOrder.Dominant", CandidateKinds.OutputDuplicateRisk);

        new MetaDataQualityCorpusInferenceService().Apply(model);

        var fanout = Assert.Single(model.ImpliedJoinFanoutRiskList);
        var outputDuplicate = Assert.Single(model.ImpliedOutputDuplicateRiskList);
        var fanoutEvidence = model.DataQualityCandidateEvidenceList.Single(
            row => string.Equals(row.DataQualityCandidate.Id, fanout.DataQualityCandidate.Id, StringComparison.Ordinal));
        var outputDuplicateEvidence = model.DataQualityCandidateEvidenceList.Single(
            row => string.Equals(row.DataQualityCandidate.Id, outputDuplicate.DataQualityCandidate.Id, StringComparison.Ordinal));

        Assert.Equal("ImpliedJoinFanoutRiskConsensus", fanoutEvidence.EvidenceType);
        Assert.Equal("ImpliedOutputDuplicateRiskConsensus", outputDuplicateEvidence.EvidenceType);
        Assert.Equal("8", fanoutEvidence.OccurrenceCount);
        Assert.Equal("8", outputDuplicateEvidence.OccurrenceCount);
        Assert.Equal("8", fanoutEvidence.DistinctTransformCount);
        Assert.Equal("8", outputDuplicateEvidence.DistinctTransformCount);
        Assert.Equal("8", fanoutEvidence.EffectiveTransformCount);
        Assert.Equal("8", outputDuplicateEvidence.EffectiveTransformCount);
        Assert.Equal("High", fanoutEvidence.ConfidenceBand);
        Assert.Equal("High", outputDuplicateEvidence.ConfidenceBand);
        Assert.Equal("High", fanoutEvidence.EvidenceQuality);
        Assert.Equal("High", outputDuplicateEvidence.EvidenceQuality);
        Assert.Contains("Distinct transforms: 8", fanoutEvidence.EvidenceDiversitySummary, StringComparison.Ordinal);
        Assert.Contains("Distinct transforms: 8", outputDuplicateEvidence.EvidenceDiversitySummary, StringComparison.Ordinal);
        Assert.Equal(1d, ParseRatio(fanoutEvidence.ConsensusRatio), 4);
        Assert.Equal(1d, ParseRatio(outputDuplicateEvidence.ConsensusRatio), 4);
        Assert.Contains("Occ.CustomerOrder.Dominant.1", fanoutEvidence.Explanation, StringComparison.Ordinal);
        Assert.Contains("Occ.CustomerOrder.Dominant.1", outputDuplicateEvidence.Explanation, StringComparison.Ordinal);
    }

    [Fact]
    public void CorpusInference_DoesNotEmitImpliedFanoutOrOutputDuplicateRisk_WhenSignalsAreNotOnDominantPattern()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Dominant.NoSignal",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ]);
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Outlier.WithSignal",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
                ("c.RegionId", "o.RegionId"),
            ]);

        for (var i = 1; i <= 8; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.CustomerOrder.Dominant.NoSignal.{i}",
                patternId: "JoinPattern.CustomerOrder.Dominant.NoSignal",
                scriptName: $"Script.CustomerOrder.Dominant.NoSignal.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");
        }

        AddOccurrence(
            model,
            occurrenceId: "Occ.CustomerOrder.Outlier.WithSignal.1",
            patternId: "JoinPattern.CustomerOrder.Outlier.WithSignal",
            scriptName: "Script.CustomerOrder.Outlier.WithSignal.1",
            leftTable: "sales.Customer",
            rightTable: "sales.Order");

        AddOccurrenceSignalsForPattern(model, "JoinPattern.CustomerOrder.Outlier.WithSignal", CandidateKinds.JoinMultiplicityExplosion);
        AddOccurrenceSignalsForPattern(model, "JoinPattern.CustomerOrder.Outlier.WithSignal", CandidateKinds.OutputDuplicateRisk);

        new MetaDataQualityCorpusInferenceService().Apply(model);

        Assert.Empty(model.ImpliedJoinFanoutRiskList);
        Assert.Empty(model.ImpliedOutputDuplicateRiskList);
    }

    [Fact]
    public void DataQualityToSql_PromotedImpliedFanoutAndOutputDuplicateRisk_RenderSemanticReviewFindings()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(rootPath, "DataQualityWS");
        var outputPath = Path.Combine(rootPath, "DataQualityViews.sql");

        try
        {
            var model = MetaDataQualityModel.CreateEmpty();
            AddJoinPattern(
                model,
                patternId: "JoinPattern.CustomerOrder.Dominant.ForSql",
                keyPairs:
                [
                    ("c.CustomerId", "o.CustomerId"),
                ]);

            for (var i = 1; i <= 8; i++)
            {
                AddOccurrence(
                    model,
                    occurrenceId: $"Occ.CustomerOrder.Dominant.ForSql.{i}",
                    patternId: "JoinPattern.CustomerOrder.Dominant.ForSql",
                    scriptName: $"Script.CustomerOrder.Dominant.ForSql.{i}",
                    leftTable: "sales.Customer",
                    rightTable: "sales.Order");
            }

            AddOccurrenceSignalsForPattern(model, "JoinPattern.CustomerOrder.Dominant.ForSql", CandidateKinds.JoinMultiplicityExplosion);
            AddOccurrenceSignalsForPattern(model, "JoinPattern.CustomerOrder.Dominant.ForSql", CandidateKinds.OutputDuplicateRisk);

            new MetaDataQualityCorpusInferenceService().Apply(model);
            Assert.Single(model.ImpliedJoinFanoutRiskList);
            Assert.Single(model.ImpliedOutputDuplicateRiskList);

            foreach (var candidate in model.DataQualityCandidateList)
            {
                candidate.Status = CandidateStatuses.Discovered;
            }

            var promotedIds = model.ImpliedJoinFanoutRiskList
                .Select(static row => row.DataQualityCandidate.Id)
                .Concat(model.ImpliedOutputDuplicateRiskList.Select(static row => row.DataQualityCandidate.Id))
                .ToHashSet(StringComparer.Ordinal);
            foreach (var candidate in model.DataQualityCandidateList.Where(candidate => promotedIds.Contains(candidate.Id)))
            {
                candidate.Status = CandidateStatuses.Promoted;
            }

            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(model, workspacePath);

            var result = new DataQualityToSqlConverter().Convert(workspacePath, outputPath);
            var sql = File.ReadAllText(outputPath);
            Assert.True(result.CandidateViewCount >= 2);
            Assert.Contains("Implied join fanout risk (semantic review)", sql, StringComparison.Ordinal);
            Assert.Contains("Implied output duplicate risk (semantic review)", sql, StringComparison.Ordinal);
            Assert.Contains("Signal evidence from", sql, StringComparison.Ordinal);
            Assert.Contains("[ConfidenceBand]", sql, StringComparison.Ordinal);
            Assert.Contains("[ConfidenceSummary]", sql, StringComparison.Ordinal);
            Assert.Contains("SemanticReviewFinding", sql, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public void CorpusInference_BuildsColumnEquivalenceGraph_AndEmitsMinorityColumnEquivalence()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.OrderCustomer.CustomerId",
            keyPairs:
            [
                ("o.CustomerId", "c.CustomerId"),
            ]);
        AddJoinPattern(
            model,
            patternId: "JoinPattern.OrderCustomer.CustomerNo",
            keyPairs:
            [
                ("o.CustomerId", "c.CustomerNo"),
            ]);

        for (var i = 1; i <= 8; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.OrderCustomer.CustomerId.{i}",
                patternId: "JoinPattern.OrderCustomer.CustomerId",
                scriptName: $"Script.OrderCustomer.CustomerId.{i}",
                leftTable: "sales.Order",
                rightTable: "sales.Customer");
        }

        for (var i = 1; i <= 2; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.OrderCustomer.CustomerNo.{i}",
                patternId: "JoinPattern.OrderCustomer.CustomerNo",
                scriptName: $"Script.OrderCustomer.CustomerNo.{i}",
                leftTable: "sales.Order",
                rightTable: "sales.Customer");
        }

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildColumnEquivalenceOnlyThresholds());

        Assert.Equal(2, model.CorpusColumnEquivalenceList.Count);
        Assert.Equal(10, model.CorpusColumnEquivalenceOccurrenceLinkList.Count);

        var minority = Assert.Single(model.MinorityColumnEquivalenceList);
        var evidence = Assert.Single(
            model.DataQualityCandidateEvidenceList,
            row => string.Equals(row.DataQualityCandidate.Id, minority.DataQualityCandidate.Id, StringComparison.Ordinal));
        Assert.Equal("2", evidence.OccurrenceCount);
        Assert.Equal("2", evidence.TransformCount);
        Assert.Equal(0.8d, ParseRatio(evidence.ConsensusRatio), 4);
        Assert.Equal(0.2d, ParseRatio(evidence.OutlierRatio), 4);
        Assert.Contains("usually equated", evidence.Explanation, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CorpusInference_LowCountColumnEquivalenceEvidence_DoesNotEmitMinorityColumnEquivalence()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.OrderCustomer.CustomerId.Low",
            keyPairs:
            [
                ("o.CustomerId", "c.CustomerId"),
            ]);
        AddJoinPattern(
            model,
            patternId: "JoinPattern.OrderCustomer.CustomerNo.Low",
            keyPairs:
            [
                ("o.CustomerId", "c.CustomerNo"),
            ]);

        for (var i = 1; i <= 6; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.OrderCustomer.CustomerId.Low.{i}",
                patternId: "JoinPattern.OrderCustomer.CustomerId.Low",
                scriptName: $"Script.OrderCustomer.CustomerId.Low.{i}",
                leftTable: "sales.Order",
                rightTable: "sales.Customer");
        }

        AddOccurrence(
            model,
            occurrenceId: "Occ.OrderCustomer.CustomerNo.Low.1",
            patternId: "JoinPattern.OrderCustomer.CustomerNo.Low",
            scriptName: "Script.OrderCustomer.CustomerNo.Low.1",
            leftTable: "sales.Order",
            rightTable: "sales.Customer");

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildColumnEquivalenceOnlyThresholds());

        Assert.Empty(model.MinorityColumnEquivalenceList);
    }

    [Fact]
    public void DataQualityToSql_PromotedMinorityColumnEquivalence_RendersSemanticReviewFinding()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(rootPath, "DataQualityWS");
        var outputPath = Path.Combine(rootPath, "DataQualityViews.sql");

        try
        {
            var model = MetaDataQualityModel.CreateEmpty();
            AddJoinPattern(
                model,
                patternId: "JoinPattern.OrderCustomer.CustomerId.ForSql",
                keyPairs:
                [
                    ("o.CustomerId", "c.CustomerId"),
                ]);
            AddJoinPattern(
                model,
                patternId: "JoinPattern.OrderCustomer.CustomerNo.ForSql",
                keyPairs:
                [
                    ("o.CustomerId", "c.CustomerNo"),
                ]);

            for (var i = 1; i <= 8; i++)
            {
                AddOccurrence(
                    model,
                    occurrenceId: $"Occ.OrderCustomer.CustomerId.ForSql.{i}",
                    patternId: "JoinPattern.OrderCustomer.CustomerId.ForSql",
                    scriptName: $"Script.OrderCustomer.CustomerId.ForSql.{i}",
                    leftTable: "sales.Order",
                    rightTable: "sales.Customer");
            }

            for (var i = 1; i <= 2; i++)
            {
                AddOccurrence(
                    model,
                    occurrenceId: $"Occ.OrderCustomer.CustomerNo.ForSql.{i}",
                    patternId: "JoinPattern.OrderCustomer.CustomerNo.ForSql",
                    scriptName: $"Script.OrderCustomer.CustomerNo.ForSql.{i}",
                    leftTable: "sales.Order",
                    rightTable: "sales.Customer");
            }

            new MetaDataQualityCorpusInferenceService().Apply(model, BuildColumnEquivalenceOnlyThresholds());
            var minority = Assert.Single(model.MinorityColumnEquivalenceList);

            foreach (var candidate in model.DataQualityCandidateList)
            {
                candidate.Status = CandidateStatuses.Discovered;
            }

            model.DataQualityCandidateList
                .Single(candidate => string.Equals(candidate.Id, minority.DataQualityCandidate.Id, StringComparison.Ordinal))
                .Status = CandidateStatuses.Promoted;
            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(model, workspacePath);

            var result = new DataQualityToSqlConverter().Convert(workspacePath, outputPath);
            var sql = File.ReadAllText(outputPath);
            Assert.True(result.CandidateViewCount >= 1);
            Assert.Contains("Minority column equivalence (semantic review)", sql, StringComparison.Ordinal);
            Assert.Contains("SemanticReviewFinding", sql, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public void CorpusInference_EmitsMissingCommonFilter_WhenDominantUsesFilterAndOutlierOmitsIt()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.DominantFilter",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ]);
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.OutlierFilter",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
                ("c.RegionId", "o.RegionId"),
            ]);

        for (var i = 1; i <= 8; i++)
        {
            var occurrenceId = $"Occ.CustomerOrder.DominantFilter.{i}";
            AddOccurrence(
                model,
                occurrenceId: occurrenceId,
                patternId: "JoinPattern.CustomerOrder.DominantFilter",
                scriptName: $"Script.CustomerOrder.DominantFilter.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");
            AddFilterPredicateObservation(
                model,
                occurrenceId,
                "sales.Customer",
                "isdeleted equals 0",
                "c.IsDeleted = 0");
        }

        for (var i = 1; i <= 2; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.CustomerOrder.OutlierFilter.{i}",
                patternId: "JoinPattern.CustomerOrder.OutlierFilter",
                scriptName: $"Script.CustomerOrder.OutlierFilter.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");
        }

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildCommonFilterOnlyThresholds());

        var missingCommonFilter = Assert.Single(model.MissingCommonFilterList);
        Assert.Equal("sales.Customer", missingCommonFilter.BaseObjectName);
        Assert.Equal("isdeleted equals 0", missingCommonFilter.CommonPredicateSignature);
        Assert.Equal("c.IsDeleted = 0", missingCommonFilter.CommonPredicateDisplay);

        var evidence = Assert.Single(
            model.DataQualityCandidateEvidenceList,
            row => string.Equals(row.DataQualityCandidate.Id, missingCommonFilter.DataQualityCandidate.Id, StringComparison.Ordinal));
        Assert.Equal("0", evidence.OccurrenceCount);
        Assert.Equal(1d, ParseRatio(evidence.ConsensusRatio), 4);
        Assert.Equal(0d, ParseRatio(evidence.OutlierRatio), 4);
    }

    [Fact]
    public void DataQualityToSql_PromotedMissingCommonFilter_RendersSemanticReviewFinding()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var workspacePath = Path.Combine(rootPath, "DataQualityWS");
        var outputPath = Path.Combine(rootPath, "DataQualityViews.sql");

        try
        {
            var model = MetaDataQualityModel.CreateEmpty();
            AddJoinPattern(
                model,
                patternId: "JoinPattern.CustomerOrder.DominantFilter.ForSql",
                keyPairs:
                [
                    ("c.CustomerId", "o.CustomerId"),
                ]);
            AddJoinPattern(
                model,
                patternId: "JoinPattern.CustomerOrder.OutlierFilter.ForSql",
                keyPairs:
                [
                    ("c.CustomerId", "o.CustomerId"),
                    ("c.RegionId", "o.RegionId"),
                ]);

            for (var i = 1; i <= 8; i++)
            {
                var occurrenceId = $"Occ.CustomerOrder.DominantFilter.ForSql.{i}";
                AddOccurrence(
                    model,
                    occurrenceId: occurrenceId,
                    patternId: "JoinPattern.CustomerOrder.DominantFilter.ForSql",
                    scriptName: $"Script.CustomerOrder.DominantFilter.ForSql.{i}",
                    leftTable: "sales.Customer",
                    rightTable: "sales.Order");
                AddFilterPredicateObservation(
                    model,
                    occurrenceId,
                    "sales.Customer",
                    "isdeleted equals 0",
                    "c.IsDeleted = 0");
            }

            for (var i = 1; i <= 2; i++)
            {
                AddOccurrence(
                    model,
                    occurrenceId: $"Occ.CustomerOrder.OutlierFilter.ForSql.{i}",
                    patternId: "JoinPattern.CustomerOrder.OutlierFilter.ForSql",
                    scriptName: $"Script.CustomerOrder.OutlierFilter.ForSql.{i}",
                    leftTable: "sales.Customer",
                    rightTable: "sales.Order");
            }

            new MetaDataQualityCorpusInferenceService().Apply(model, BuildCommonFilterOnlyThresholds());
            var missingCommonFilter = Assert.Single(model.MissingCommonFilterList);

            foreach (var candidate in model.DataQualityCandidateList)
            {
                candidate.Status = CandidateStatuses.Discovered;
            }

            model.DataQualityCandidateList
                .Single(candidate => string.Equals(candidate.Id, missingCommonFilter.DataQualityCandidate.Id, StringComparison.Ordinal))
                .Status = CandidateStatuses.Promoted;
            Meta.Core.Serialization.TypedWorkspaceXmlSerializer.Save(model, workspacePath);

            var result = new DataQualityToSqlConverter().Convert(workspacePath, outputPath);
            var sql = File.ReadAllText(outputPath);
            Assert.True(result.CandidateViewCount >= 1);
            Assert.Contains("Missing common filter (semantic review)", sql, StringComparison.Ordinal);
            Assert.Contains("SemanticReviewFinding", sql, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public void CorpusInference_LowCountSignalEvidence_DoesNotEmitImpliedFanoutOrOutputDuplicateRisk()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.LowCount",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ]);

        for (var i = 1; i <= 7; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.CustomerOrder.LowCount.{i}",
                patternId: "JoinPattern.CustomerOrder.LowCount",
                scriptName: $"Script.CustomerOrder.LowCount.{i}",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");
        }

        AddOccurrenceSignalsForPattern(model, "JoinPattern.CustomerOrder.LowCount", CandidateKinds.JoinMultiplicityExplosion);
        AddOccurrenceSignalsForPattern(model, "JoinPattern.CustomerOrder.LowCount", CandidateKinds.OutputDuplicateRisk);

        new MetaDataQualityCorpusInferenceService().Apply(model);

        Assert.Empty(model.ImpliedJoinFanoutRiskList);
        Assert.Empty(model.ImpliedOutputDuplicateRiskList);
    }

    [Fact]
    public void CorpusInference_RepeatedSignalsInsideOneTransform_DoNotPassSignalTransformCountThreshold()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.SingleTransform",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ]);

        for (var i = 1; i <= 8; i++)
        {
            AddOccurrence(
                model,
                occurrenceId: $"Occ.CustomerOrder.SingleTransform.{i}",
                patternId: "JoinPattern.CustomerOrder.SingleTransform",
                scriptName: "Script.CustomerOrder.SingleTransform",
                leftTable: "sales.Customer",
                rightTable: "sales.Order");
        }

        AddOccurrenceSignalsForPattern(model, "JoinPattern.CustomerOrder.SingleTransform", CandidateKinds.JoinMultiplicityExplosion);
        AddOccurrenceSignalsForPattern(model, "JoinPattern.CustomerOrder.SingleTransform", CandidateKinds.OutputDuplicateRisk);

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildSignalThresholdIsolationOptions());

        Assert.Empty(model.ImpliedJoinFanoutRiskList);
        Assert.Empty(model.ImpliedOutputDuplicateRiskList);
    }

    [Fact]
    public void CorpusInference_CalibrationDistinctTransformCount_DoesNotInflateFromRepeatedOccurrences()
    {
        var model = MetaDataQualityModel.CreateEmpty();
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Dominant.Calibration",
            keyPairs:
            [
                ("c.CustomerId", "o.CustomerId"),
            ]);
        AddJoinPattern(
            model,
            patternId: "JoinPattern.CustomerOrder.Outlier.Calibration",
            keyPairs:
            [
                ("c.CustomerNo", "o.CustomerNo"),
            ]);

        AddOccurrence(
            model,
            occurrenceId: "Occ.Calibration.Dom.1",
            patternId: "JoinPattern.CustomerOrder.Dominant.Calibration",
            scriptName: "Script.Calibration.Dom.1",
            leftTable: "sales.Customer",
            rightTable: "sales.Order");
        AddOccurrence(
            model,
            occurrenceId: "Occ.Calibration.Dom.2",
            patternId: "JoinPattern.CustomerOrder.Dominant.Calibration",
            scriptName: "Script.Calibration.Dom.2",
            leftTable: "sales.Customer",
            rightTable: "sales.Order");

        AddOccurrence(
            model,
            occurrenceId: "Occ.Calibration.Outlier.1",
            patternId: "JoinPattern.CustomerOrder.Outlier.Calibration",
            scriptName: "Script.Calibration.Outlier",
            leftTable: "sales.Customer",
            rightTable: "sales.Order");
        AddOccurrence(
            model,
            occurrenceId: "Occ.Calibration.Outlier.2",
            patternId: "JoinPattern.CustomerOrder.Outlier.Calibration",
            scriptName: "Script.Calibration.Outlier",
            leftTable: "sales.Customer",
            rightTable: "sales.Order");

        new MetaDataQualityCorpusInferenceService().Apply(model, BuildTinyThresholds());

        var minority = Assert.Single(model.MinorityJoinPatternList);
        var evidence = Assert.Single(
            model.DataQualityCandidateEvidenceList,
            row => string.Equals(row.DataQualityCandidate.Id, minority.DataQualityCandidate.Id, StringComparison.Ordinal));
        Assert.Equal("2", evidence.OccurrenceCount);
        Assert.Equal("1", evidence.TransformCount);
        Assert.Equal("1", evidence.DistinctTransformCount);
        Assert.Equal("1", evidence.EffectiveTransformCount);
        Assert.Equal("Low", evidence.ConfidenceBand);
    }

    [Fact]
    public void CorpusInference_CalibrationConfidenceMetadata_IsDeterministicAcrossRuns()
    {
        static MetaDataQualityModel BuildModel()
        {
            var model = MetaDataQualityModel.CreateEmpty();
            AddJoinPattern(
                model,
                patternId: "JoinPattern.CustomerOrder.Deterministic",
                keyPairs:
                [
                    ("c.CustomerId", "o.CustomerId"),
                ]);

            for (var i = 1; i <= 8; i++)
            {
                AddOccurrence(
                    model,
                    occurrenceId: $"Occ.Deterministic.{i}",
                    patternId: "JoinPattern.CustomerOrder.Deterministic",
                    scriptName: $"Script.Deterministic.{i}",
                    leftTable: "sales.Customer",
                    rightTable: "sales.Order");
            }

            AddOccurrenceSignalsForPattern(model, "JoinPattern.CustomerOrder.Deterministic", CandidateKinds.JoinMultiplicityExplosion);
            return model;
        }

        var first = BuildModel();
        var second = BuildModel();

        new MetaDataQualityCorpusInferenceService().Apply(first);
        new MetaDataQualityCorpusInferenceService().Apply(second);

        var firstEvidence = first.DataQualityCandidateEvidenceList
            .OrderBy(row => row.EvidenceType, StringComparer.Ordinal)
            .Select(row => $"{row.EvidenceType}|{row.ConfidenceBand}|{row.ConfidenceReason}|{row.EvidenceDiversitySummary}|{row.DistinctTransformCount}")
            .ToArray();
        var secondEvidence = second.DataQualityCandidateEvidenceList
            .OrderBy(row => row.EvidenceType, StringComparer.Ordinal)
            .Select(row => $"{row.EvidenceType}|{row.ConfidenceBand}|{row.ConfidenceReason}|{row.EvidenceDiversitySummary}|{row.DistinctTransformCount}")
            .ToArray();

        Assert.Equal(firstEvidence, secondEvidence);
    }

    private static void AddJoinPattern(
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

    private static void AddOccurrence(
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

    private static void AddOccurrenceSignalsForPattern(
        MetaDataQualityModel model,
        string patternId,
        string signalKind)
    {
        var occurrences = model.JoinPatternOccurrenceList
            .Where(row => string.Equals(row.JoinPattern.Id, patternId, StringComparison.Ordinal))
            .OrderBy(row => row.Id, StringComparer.Ordinal)
            .ToArray();
        foreach (var occurrence in occurrences)
        {
            AddOccurrenceSignal(
                model,
                occurrence.Id,
                signalKind);
        }
    }

    private static void AddOccurrenceSignal(
        MetaDataQualityModel model,
        string occurrenceId,
        string signalKind)
    {
        var id = $"{occurrenceId}.Signal.{signalKind}";
        if (model.JoinPatternOccurrenceSignalList.Any(row => string.Equals(row.Id, id, StringComparison.Ordinal)))
        {
            return;
        }

        model.JoinPatternOccurrenceSignalList.Add(new JoinPatternOccurrenceSignal
        {
            Id = id,
            JoinPatternOccurrence = model.JoinPatternOccurrenceList.Single(row => string.Equals(row.Id, occurrenceId, StringComparison.Ordinal)),
            SignalKind = signalKind,
            SourceCandidateKind = signalKind,
            Explanation = "Test signal.",
        });
    }

    private static void AddFilterPredicateObservation(
        MetaDataQualityModel model,
        string occurrenceId,
        string baseObjectName,
        string predicateSignature,
        string predicateDisplay)
    {
        var id = $"{occurrenceId}.Filter.{predicateSignature}";
        if (model.FilterPredicateObservationList.Any(row => string.Equals(row.Id, id, StringComparison.Ordinal)))
        {
            return;
        }

        model.FilterPredicateObservationList.Add(new FilterPredicateObservation
        {
            Id = id,
            JoinPatternOccurrence = model.JoinPatternOccurrenceList.Single(row => string.Equals(row.Id, occurrenceId, StringComparison.Ordinal)),
            BaseObjectName = baseObjectName,
            PredicateSignature = predicateSignature,
            PredicateDisplay = predicateDisplay,
        });
    }

    private static CorpusInferenceOptions BuildTinyThresholds()
    {
        return new CorpusInferenceOptions
        {
            MinTablePairOccurrenceCount = 1,
            MinTablePairTransformCount = 1,
            DominantPatternMinRatio = 0.5,
            DominantPatternMinOccurrenceCount = 1,
            MinorityPatternMaxRatio = 0.5,
            MinRelationshipOccurrenceCount = 1,
            MinRelationshipTransformCount = 1,
            MinConsensusRatio = 0.5,
            MinDominantPatternOccurrenceCount = 1,
            MinLookupSideOccurrenceCount = 1,
            MinLookupSideTransformCount = 1,
            MinLookupSideConsistencyRatio = 0.5,
            MinKeyPartOccurrenceCount = 1,
            MinPatternOccurrenceCount = 1,
            MinPatternTransformCount = 1,
            DominantOptionalityMinRatio = 0.5,
            DominantOptionalityMinOccurrenceCount = 1,
            OutlierOptionalityMaxRatio = 0.5,
            OutlierOptionalityMinOccurrenceCount = 1,
            MinFanoutSignalOccurrenceCount = 1,
            MinFanoutSignalTransformCount = 1,
            MinFanoutSignalRatio = 0.5,
            MinOutputDuplicateSignalOccurrenceCount = 1,
            MinOutputDuplicateSignalTransformCount = 1,
            MinOutputDuplicateSignalRatio = 0.5,
        };
    }

    private static CorpusInferenceOptions BuildOptionalityOnlyThresholds()
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
            MinFanoutSignalOccurrenceCount = int.MaxValue,
            MinFanoutSignalTransformCount = int.MaxValue,
            MinFanoutSignalRatio = 1d,
            MinOutputDuplicateSignalOccurrenceCount = int.MaxValue,
            MinOutputDuplicateSignalTransformCount = int.MaxValue,
            MinOutputDuplicateSignalRatio = 1d,
        };
    }

    private static CorpusInferenceOptions BuildSignalThresholdIsolationOptions()
    {
        return new CorpusInferenceOptions
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
            MinFanoutSignalOccurrenceCount = 8,
            MinFanoutSignalTransformCount = 4,
            MinFanoutSignalRatio = 0.7,
            MinOutputDuplicateSignalOccurrenceCount = 8,
            MinOutputDuplicateSignalTransformCount = 4,
            MinOutputDuplicateSignalRatio = 0.7,
        };
    }

    private static CorpusInferenceOptions BuildCommonFilterOnlyThresholds()
    {
        return new CorpusInferenceOptions
        {
            MinTablePairOccurrenceCount = 5,
            MinTablePairTransformCount = 3,
            DominantPatternMinRatio = 0.8,
            DominantPatternMinOccurrenceCount = 4,
            MinorityPatternMaxRatio = 0.2,
            MinRelationshipOccurrenceCount = int.MaxValue,
            MinRelationshipTransformCount = int.MaxValue,
            MinConsensusRatio = 1d,
            MinDominantPatternOccurrenceCount = int.MaxValue,
            MinLookupSideOccurrenceCount = int.MaxValue,
            MinLookupSideTransformCount = int.MaxValue,
            MinLookupSideConsistencyRatio = 1d,
            MinKeyPartOccurrenceCount = int.MaxValue,
            MinColumnAnchorOccurrenceCount = int.MaxValue,
            MinColumnAnchorTransformCount = int.MaxValue,
            DominantColumnEquivalenceMinRatio = 1d,
            DominantColumnEquivalenceMinOccurrenceCount = int.MaxValue,
            MinorityColumnEquivalenceMaxRatio = 0d,
            MinCommonFilterOccurrenceCount = 6,
            MinCommonFilterTransformCount = 4,
            MinCommonFilterConsensusRatio = 0.85,
            MissingCommonFilterOutlierMaxRatio = 0.15,
            MinPatternOccurrenceCount = int.MaxValue,
            MinPatternTransformCount = int.MaxValue,
            DominantOptionalityMinRatio = 1d,
            DominantOptionalityMinOccurrenceCount = int.MaxValue,
            OutlierOptionalityMaxRatio = 0d,
            OutlierOptionalityMinOccurrenceCount = int.MaxValue,
            MinFanoutSignalOccurrenceCount = int.MaxValue,
            MinFanoutSignalTransformCount = int.MaxValue,
            MinFanoutSignalRatio = 1d,
            MinOutputDuplicateSignalOccurrenceCount = int.MaxValue,
            MinOutputDuplicateSignalTransformCount = int.MaxValue,
            MinOutputDuplicateSignalRatio = 1d,
        };
    }

    private static CorpusInferenceOptions BuildColumnEquivalenceOnlyThresholds()
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
            MinColumnAnchorOccurrenceCount = 8,
            MinColumnAnchorTransformCount = 4,
            DominantColumnEquivalenceMinRatio = 0.8,
            DominantColumnEquivalenceMinOccurrenceCount = 6,
            MinorityColumnEquivalenceMaxRatio = 0.2,
            MinPatternOccurrenceCount = int.MaxValue,
            MinPatternTransformCount = int.MaxValue,
            DominantOptionalityMinRatio = 1d,
            DominantOptionalityMinOccurrenceCount = int.MaxValue,
            OutlierOptionalityMaxRatio = 0d,
            OutlierOptionalityMinOccurrenceCount = int.MaxValue,
            MinFanoutSignalOccurrenceCount = int.MaxValue,
            MinFanoutSignalTransformCount = int.MaxValue,
            MinFanoutSignalRatio = 1d,
            MinOutputDuplicateSignalOccurrenceCount = int.MaxValue,
            MinOutputDuplicateSignalTransformCount = int.MaxValue,
            MinOutputDuplicateSignalRatio = 1d,
        };
    }

    private static double ParseRatio(string value)
    {
        return double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
