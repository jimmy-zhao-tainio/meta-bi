using System.Globalization;
using Meta.Surfaces.Xml;
using MetaConvert.DataQualityToSql;
using MetaBi.Tests.Common;
using MetaCli.Core;
using MetaDataQuality;
using MetaDataQuality.Core;
using MetaTransformBinding;
using MetaTransformScript;
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
        Assert.Contains("Usage:", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("meta-data-quality from-transform-workspace", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Options:", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--transform-workspace <path>", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--output-xml <path>", result.Output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("--binding-workspace <path>", result.Output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FromTransformWorkspace_And_Promote_Works()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");
        var qualityWorkspacePath = Path.Combine(rootPath, "quality");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                "select c.CustomerId, o.OrderId from dbo.Customer c left outer join dbo.[Order] o on c.CustomerId = o.CustomerId",
                "dbo.TargetOrders",
                transformWorkspacePath,
                "dbo.v_customer_orders");

            var generated = RunCli(
                $"from-transform-workspace --transform-workspace \"{transformWorkspacePath}\" --output-xml \"{qualityWorkspacePath}\"");

            Assert.Equal(0, generated.ExitCode);
            Assert.Contains("Views ready to create:", generated.Output, StringComparison.Ordinal);
            Assert.Contains("Relationships captured:", generated.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("Ok", generated.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("Run This First:", generated.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("Review The Results:", generated.Output, StringComparison.Ordinal);
            Assert.True(File.Exists(Path.Combine(qualityWorkspacePath, "workspace.meta")));
            Assert.True(File.Exists(Path.Combine(qualityWorkspacePath, "model.xml")));

            var model = TypedWorkspaceXmlSerializer.Load<MetaDataQualityModel>(qualityWorkspacePath, searchUpward: false);
            Assert.NotEmpty(model.DataQualityCandidateList);
            Assert.NotEmpty(model.OuterJoinNullExpansionList);
            Assert.NotEmpty(model.CorpusRelationshipList);

            var inspect = RunCli($"inspect --workspace \"{qualityWorkspacePath}\"");
            Assert.Equal(0, inspect.ExitCode);
            Assert.DoesNotContain("Loaded MetaDataQuality workspace", inspect.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("Why These Views Exist:", inspect.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("Run This First:", inspect.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("Review The Results:", inspect.Output, StringComparison.Ordinal);
            Assert.Contains("Corpus Inference:", inspect.Output, StringComparison.Ordinal);

            var inspectFromCurrentDirectory = RunCli("inspect", workingDirectory: qualityWorkspacePath);
            Assert.Equal(0, inspectFromCurrentDirectory.ExitCode);
            Assert.Contains("Corpus Inference:", inspectFromCurrentDirectory.Output, StringComparison.Ordinal);

            var firstCandidate = model.DataQualityCandidateList[0];
            var promoted = RunCli($"promote --candidate-id \"{firstCandidate.Id}\"", workingDirectory: qualityWorkspacePath);
            Assert.True(promoted.ExitCode == 0, promoted.Output);
            Assert.Contains("Candidates promoted this run: 1", promoted.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("Ok", promoted.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("Next:", promoted.Output, StringComparison.Ordinal);

            var reloaded = TypedWorkspaceXmlSerializer.Load<MetaDataQualityModel>(qualityWorkspacePath, searchUpward: false);
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
    public void Promote_ByCandidateKind_PromotesMatchingFamilies()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var qualityWorkspacePath = Path.Combine(rootPath, "quality");

        try
        {
            var model = MetaDataQualityModel.CreateEmpty();
            var plainCandidate = AddCandidate(model, "candidate-local", "Local candidate");
            model.JoinOrphanList.Add(new JoinOrphan
            {
                Id = "join-orphan-local",
                DataQualityCandidate = plainCandidate,
                EqualityPredicateCount = "1",
            });

            var relationship = new CorpusRelationship
            {
                Id = "relationship-customer-order",
                CanonicalSideAObjectName = "sales.Customer",
                CanonicalSideBObjectName = "sales.Order",
                CanonicalUndirectedSignature = "sales.Customer|sales.Order",
                OccurrenceCount = "8",
                TransformCount = "8",
            };
            model.CorpusRelationshipList.Add(relationship);
            var dominantPattern = new CorpusRelationshipPattern
            {
                Id = "pattern-customer-order",
                CorpusRelationship = relationship,
                CanonicalKeyPartSetSignature = "CustomerId=CustomerId",
                IsDominant = "true",
                KeyPartCount = "1",
                OccurrenceCount = "8",
                OccurrenceRatio = "1",
                RepresentativeDirectionalSignature = "sales.Customer -> sales.Order",
                TransformCount = "8",
            };
            model.CorpusRelationshipPatternList.Add(dominantPattern);

            var impliedForeignKeyCandidate = AddCandidate(model, "candidate-implied-fk", "Implied FK candidate");
            model.ImpliedForeignKeyMissingReferenceList.Add(new ImpliedForeignKeyMissingReference
            {
                Id = "implied-fk-candidate",
                DataQualityCandidate = impliedForeignKeyCandidate,
                DominantPattern = dominantPattern,
            });
            var impliedUniqueCandidate = AddCandidate(model, "candidate-implied-unique", "Implied unique candidate");
            model.ImpliedUniqueKeyViolationList.Add(new ImpliedUniqueKeyViolation
            {
                Id = "implied-unique-candidate",
                DataQualityCandidate = impliedUniqueCandidate,
                DominantPattern = dominantPattern,
            });

            TypedWorkspaceXmlSerializer.Save(model, qualityWorkspacePath);
            MetaCliWorkspace.DescribeXml(qualityWorkspacePath);

            var promoted = RunCli(
                "promote --candidate-kind ImpliedForeignKeyMissingReference --candidate-kind ImpliedUniqueKeyViolation",
                workingDirectory: qualityWorkspacePath);

            Assert.True(promoted.ExitCode == 0, promoted.Output);
            Assert.Contains("Candidates promoted this run: 2", promoted.Output, StringComparison.Ordinal);

            var reloaded = TypedWorkspaceXmlSerializer.Load<MetaDataQualityModel>(qualityWorkspacePath, searchUpward: false);
            Assert.Equal(
                CandidateStatuses.Discovered,
                reloaded.DataQualityCandidateList.Single(row => string.Equals(row.Id, plainCandidate.Id, StringComparison.Ordinal)).Status);
            Assert.Equal(
                CandidateStatuses.Promoted,
                reloaded.DataQualityCandidateList.Single(row => string.Equals(row.Id, impliedForeignKeyCandidate.Id, StringComparison.Ordinal)).Status);
            Assert.Equal(
                CandidateStatuses.Promoted,
                reloaded.DataQualityCandidateList.Single(row => string.Equals(row.Id, impliedUniqueCandidate.Id, StringComparison.Ordinal)).Status);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public async Task FromTransformWorkspace_WithBindingWorkspace_ScansOnlyBoundScripts()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");
        var bindingWorkspacePath = Path.Combine(rootPath, "binding");
        var qualityWorkspacePath = Path.Combine(rootPath, "quality");

        try
        {
            var sqlService = new MetaTransformScriptSqlService();
            await sqlService.ImportFromSqlCodeToXmlWorkspaceAsync(
                "select c.CustomerId, o.OrderId from dbo.Customer c left outer join dbo.[Order] o on c.CustomerId = o.CustomerId",
                "dbo.TargetOrders",
                transformWorkspacePath,
                "dbo.v_customer_orders");
            await sqlService.AddSqlCodeToWorkspaceAsync(
                "select m.CustomerId, p.PaymentId from dbo.MissingCustomer m left outer join dbo.Payment p on m.CustomerId = p.CustomerId",
                "dbo.TargetPayments",
                transformWorkspacePath,
                "dbo.v_missing_payments");

            var transformModel = TypedWorkspaceXmlSerializer.Load<MetaTransformScriptModel>(transformWorkspacePath, searchUpward: false);
            var boundScript = transformModel.TransformScriptList.Single(item =>
                string.Equals(item.Name, "dbo.v_customer_orders", StringComparison.OrdinalIgnoreCase));
            var bindingModel = MetaTransformBindingModel.CreateEmpty();
            var binding = new TransformBinding
            {
                Id = $"{boundScript.Id}:binding",
                MetaTransformScriptTransformScriptId = boundScript.Id,
                TransformScriptName = boundScript.Name
            };
            bindingModel.TransformBindingList.Add(binding);
            bindingModel.ValidationList.Add(new Validation
            {
                Id = $"{binding.Id}:validation",
                TransformBinding = binding
            });
            TypedWorkspaceXmlSerializer.Save(bindingModel, bindingWorkspacePath);

            var generated = RunCli(
                $"from-transform-workspace --transform-workspace \"{transformWorkspacePath}\" --binding-workspace \"{bindingWorkspacePath}\" --output-xml \"{qualityWorkspacePath}\"");

            Assert.Equal(0, generated.ExitCode);
            Assert.Contains("Transform scripts scanned: 1/2", generated.Output, StringComparison.Ordinal);

            var model = TypedWorkspaceXmlSerializer.Load<MetaDataQualityModel>(qualityWorkspacePath, searchUpward: false);
            Assert.NotEmpty(model.DataQualityCandidateList);
            Assert.Contains(model.JoinPatternOccurrenceList, item =>
                string.Equals(item.TransformScriptName, "dbo.v_customer_orders", StringComparison.OrdinalIgnoreCase));
            Assert.DoesNotContain(model.JoinPatternOccurrenceList, item =>
                string.Equals(item.TransformScriptName, "dbo.v_missing_payments", StringComparison.OrdinalIgnoreCase));
            Assert.DoesNotContain(model.JoinPatternOccurrenceBaseTableList, item =>
                item.BaseObjectName.Contains("MissingCustomer", StringComparison.OrdinalIgnoreCase) ||
                item.BaseObjectName.Contains("Payment", StringComparison.OrdinalIgnoreCase));
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

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
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
                        || (occurrence.ScopePath ?? string.Empty).Contains("CTE:", StringComparison.OrdinalIgnoreCase)));
            Assert.Contains(
                result.Model.JoinPatternOccurrenceBaseTableList,
                row => (row.BaseObjectName ?? string.Empty).Contains("Customer", StringComparison.OrdinalIgnoreCase)
                       || (row.BaseObjectName ?? string.Empty).Contains("Order", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(
                result.Model.JoinPatternKeyPartList,
                row =>
                    (row.FirstExpressionDisplay ?? string.Empty).Contains("CustomerId", StringComparison.OrdinalIgnoreCase)
                    || (row.SecondExpressionDisplay ?? string.Empty).Contains("CustomerId", StringComparison.OrdinalIgnoreCase));
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

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
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
                    (row.FirstExpressionDisplay ?? string.Empty).Contains("CustomerId", StringComparison.OrdinalIgnoreCase)
                    || (row.SecondExpressionDisplay ?? string.Empty).Contains("CustomerId", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(
                predicateParts,
                row =>
                    (row.FirstExpressionDisplay ?? string.Empty).Contains("RegionId", StringComparison.OrdinalIgnoreCase)
                    || (row.SecondExpressionDisplay ?? string.Empty).Contains("RegionId", StringComparison.OrdinalIgnoreCase));

            Assert.All(
                predicateParts,
                row =>
                {
                    Assert.Equal("dbo.Customer", row.FirstJoinInputObjectName);
                    Assert.Equal("dbo.Order", row.SecondJoinInputObjectName);
                    Assert.False(string.IsNullOrWhiteSpace(row.FirstJoinInputColumnName));
                    Assert.False(string.IsNullOrWhiteSpace(row.SecondJoinInputColumnName));
                });
            Assert.All(
                predicateParts,
                keyPart =>
                {
                    var firstObjectParts = result.Model.JoinPatternKeyPartInputObjectIdentifierPartList
                        .Where(row => string.Equals(row.JoinPatternKeyPart.Id, keyPart.Id, StringComparison.Ordinal)
                                      && string.Equals(row.InputSide, "First", StringComparison.Ordinal))
                        .OrderBy(row => int.Parse(row.Ordinal, CultureInfo.InvariantCulture))
                        .Select(static row => row.Value)
                        .ToArray();
                    var secondObjectParts = result.Model.JoinPatternKeyPartInputObjectIdentifierPartList
                        .Where(row => string.Equals(row.JoinPatternKeyPart.Id, keyPart.Id, StringComparison.Ordinal)
                                      && string.Equals(row.InputSide, "Second", StringComparison.Ordinal))
                        .OrderBy(row => int.Parse(row.Ordinal, CultureInfo.InvariantCulture))
                        .Select(static row => row.Value)
                        .ToArray();

                    Assert.Equal(["dbo", "Customer"], firstObjectParts);
                    Assert.Equal(["dbo", "Order"], secondObjectParts);
                });
            Assert.NotEmpty(result.Model.JoinOrphanList);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public async Task Discovery_DoesNotCreateRuntimeChecksWithoutJoinColumnEvidence()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");

        try
        {
            const string sql = """
SELECT c.CustomerId, o.OrderId
FROM dbo.Customer c
INNER JOIN dbo.[Order] o ON 1 = 1;
""";

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                sql,
                "dbo.TargetOrders",
                transformWorkspacePath,
                "dbo.v_customer_orders_missing_key");

            var result = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);

            var keyPart = Assert.Single(result.Model.JoinPatternKeyPartList);
            Assert.True(string.IsNullOrWhiteSpace(keyPart.FirstJoinInputObjectName));
            Assert.True(string.IsNullOrWhiteSpace(keyPart.FirstJoinInputColumnName));
            Assert.True(string.IsNullOrWhiteSpace(keyPart.SecondJoinInputObjectName));
            Assert.True(string.IsNullOrWhiteSpace(keyPart.SecondJoinInputColumnName));
            Assert.Empty(result.Model.JoinPatternKeyPartInputObjectIdentifierPartList);
            Assert.Empty(result.Model.JoinOrphanList);
            Assert.Empty(result.Model.JoinMultiplicityExplosionList);
            Assert.Empty(result.Model.OuterJoinNullExpansionList);
            Assert.Empty(result.Model.OutputDuplicateRiskList);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public async Task Discovery_DoesNotCreateRuntimeChecksFromIncompleteJoinColumnEvidence()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");

        try
        {
            const string sql = """
SELECT c.CustomerId, o.OrderId
FROM dbo.Customer c
LEFT OUTER JOIN dbo.[Order] o ON c.CustomerId = CustomerId;
""";

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                sql,
                "dbo.TargetOrders",
                transformWorkspacePath,
                "dbo.v_customer_orders_incomplete_key");

            var result = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);

            var keyPart = Assert.Single(result.Model.JoinPatternKeyPartList);
            Assert.Contains("c.CustomerId", keyPart.FirstExpressionDisplay, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("CustomerId", keyPart.SecondExpressionDisplay, StringComparison.OrdinalIgnoreCase);
            Assert.True(string.IsNullOrWhiteSpace(keyPart.FirstJoinInputObjectName));
            Assert.True(string.IsNullOrWhiteSpace(keyPart.FirstJoinInputColumnName));
            Assert.True(string.IsNullOrWhiteSpace(keyPart.SecondJoinInputObjectName));
            Assert.True(string.IsNullOrWhiteSpace(keyPart.SecondJoinInputColumnName));
            Assert.Empty(result.Model.JoinPatternKeyPartInputObjectIdentifierPartList);
            Assert.Empty(result.Model.JoinOrphanList);
            Assert.Empty(result.Model.JoinMultiplicityExplosionList);
            Assert.Empty(result.Model.OuterJoinNullExpansionList);
            Assert.Empty(result.Model.OutputDuplicateRiskList);
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

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
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
    public async Task Discovery_PreservesQuotedDotIdentityForJoinProjectionAndFilter()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                SELECT [c.x].[Customer.Id], [o.x].[Detail.Code]
                FROM [Db.One].dbo.[Customer.Table] AS [c.x]
                LEFT OUTER JOIN [Db.One].dbo.[Order.Table] AS [o.x]
                    ON [c.x].[Customer.Id] = [o.x].[Customer.Id]
                WHERE [c.x].[Status.Code] = 0;
                """,
                "dbo.TargetQuoted",
                transformWorkspacePath,
                "dbo.v_quoted_dot_witness");

            var result = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);

            var keyPart = Assert.Single(result.Model.JoinPatternKeyPartList);
            Assert.Equal("Customer.Id", keyPart.FirstJoinInputColumnName);
            Assert.Equal("Customer.Id", keyPart.SecondJoinInputColumnName);
            Assert.Equal("[Db.One].dbo.[Customer.Table]", keyPart.FirstJoinInputObjectName);
            Assert.Equal("[Db.One].dbo.[Order.Table]", keyPart.SecondJoinInputObjectName);

            var filter = Assert.Single(result.Model.FilterPredicateObservationList);
            Assert.Equal("[Db.One].dbo.[Customer.Table]", filter.BaseObjectName);
            Assert.Contains("Status.Code", filter.PredicateDisplay, StringComparison.Ordinal);

            Assert.Empty(result.Model.JoinMultiplicityExplosionList);
            Assert.Empty(result.Model.OutputDuplicateRiskList);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public async Task Discovery_DistinguishesSameDisplayDifferentMultipartObjects()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");

        try
        {
            var sqlService = new MetaTransformScriptSqlService();
            await sqlService.ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                SELECT l.CustomerId, r.OrderId
                FROM [sales.eu].Customer l
                INNER JOIN [sales.eu].[Order] r ON l.CustomerId = r.CustomerId;
                """,
                "dbo.TargetTwoPart",
                transformWorkspacePath,
                "dbo.v_two_part");
            await sqlService.AddSqlCodeToWorkspaceAsync(
                """
                SELECT l.CustomerId, r.OrderId
                FROM sales.eu.Customer l
                INNER JOIN sales.eu.[Order] r ON l.CustomerId = r.CustomerId;
                """,
                "dbo.TargetThreePart",
                transformWorkspacePath,
                "dbo.v_three_part");

            var result = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);

            Assert.Equal(2, result.Model.JoinPatternOccurrenceList.Count);
            Assert.Equal(2, result.Model.JoinPatternList.Count);
            Assert.Contains(
                result.Model.JoinPatternList,
                item => item.CanonicalSignature.Contains("[sales.eu].Customer", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(
                result.Model.JoinPatternList,
                item => item.CanonicalSignature.Contains("sales.eu.Customer", StringComparison.OrdinalIgnoreCase)
                        && !item.CanonicalSignature.Contains("[sales.eu].Customer", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(
                result.Model.JoinPatternKeyPartInputObjectIdentifierPartList,
                item => string.Equals(item.Value, "sales.eu", StringComparison.Ordinal));
            Assert.Contains(
                result.Model.JoinPatternKeyPartInputObjectIdentifierPartList,
                item => string.Equals(item.Value, "sales", StringComparison.Ordinal));
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

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
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

            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
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
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
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

            TypedWorkspaceXmlSerializer.Save(discovery.Model, qualityWorkspacePath);

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
    public async Task DataQualityToSql_UsesJoinInputColumnsWhenPredicateExpressionOrderIsReversed()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var transformWorkspacePath = Path.Combine(rootPath, "transform");
        var qualityWorkspacePath = Path.Combine(rootPath, "quality");
        var outputPath = Path.Combine(rootPath, "DataQualityViews.sql");

        try
        {
            await new MetaTransformScriptSqlService().ImportFromSqlCodeToXmlWorkspaceAsync(
                """
                SELECT c.CustomerId, cp.ProfileName
                FROM dbo.Customer c
                INNER JOIN dbo.CustomerProfile cp
                    ON cp.CustomerHubId = c.CustomerId;
                """,
                "dbo.TargetCustomerProfile",
                transformWorkspacePath,
                "dbo.v_customer_profile");

            var discovery = new MetaDataQualityCandidateDiscoveryService()
                .DiscoverFromTransformWorkspace(transformWorkspacePath);
            foreach (var candidate in discovery.Model.DataQualityCandidateList)
            {
                candidate.Status = CandidateStatuses.Promoted;
            }

            var keyPart = Assert.Single(discovery.Model.JoinPatternKeyPartList);
            Assert.Equal("CustomerId", keyPart.FirstJoinInputColumnName);
            Assert.Equal("CustomerHubId", keyPart.SecondJoinInputColumnName);

            TypedWorkspaceXmlSerializer.Save(discovery.Model, qualityWorkspacePath);

            new DataQualityToSqlConverter().Convert(qualityWorkspacePath, outputPath);
            var sql = File.ReadAllText(outputPath);

            Assert.Contains("[dq_left].[CustomerId] = [dq_right].[CustomerHubId]", sql, StringComparison.Ordinal);
            Assert.Contains("FROM [dbo].[CustomerProfile] AS [dq_right]", sql, StringComparison.Ordinal);
            Assert.Contains("CustomerHubId=", sql, StringComparison.Ordinal);
            Assert.DoesNotContain("[dq_left].[CustomerHubId] = [dq_right].[CustomerId]", sql, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public void DataQualityToSql_GeneratesDeployableSqlForEmptyCandidateSet()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var qualityWorkspacePath = Path.Combine(rootPath, "quality");
        var outputPath = Path.Combine(rootPath, "DataQualityViews.sql");

        try
        {
            Meta.Surfaces.Xml.TypedWorkspaceXmlSerializer.Save(
                MetaDataQualityModel.CreateEmpty(),
                qualityWorkspacePath);

            var result = new DataQualityToSqlConverter().Convert(qualityWorkspacePath, outputPath);
            var sql = File.ReadAllText(outputPath);

            Assert.Equal(0, result.CandidateViewCount);
            Assert.Equal(1, result.DashboardViewCount);
            Assert.Equal(2, result.OperationalTableCount);
            Assert.Equal(2, result.OperationalProcedureCount);
            Assert.Equal(2, result.ScriptCount);
            Assert.Contains("CREATE OR ALTER VIEW [dq].[v_DataQualityReview]", sql, StringComparison.Ordinal);
            Assert.Contains("WHERE 1 = 0;", sql, StringComparison.Ordinal);
            Assert.Contains("CREATE DATABASE [MetaDQ]", sql, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public void Inspect_SummarizesDirectionalOptionality()
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
            TypedWorkspaceXmlSerializer.Save(model, qualityWorkspacePath);
            MetaCliWorkspace.DescribeXml(qualityWorkspacePath);

            var inspect = RunCli($"inspect --workspace \"{qualityWorkspacePath}\"");
            Assert.True(inspect.ExitCode == 0, inspect.Output);
            Assert.Contains("Optionality-drift (inner vs usually optional):", inspect.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("Diversity:", inspect.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("nullable side is", inspect.Output, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    [Fact]
    public void Inspect_ShowCases_HidesInternalScalarExpressionIds()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "MetaDataQuality.Tests", Guid.NewGuid().ToString("N"));
        var qualityWorkspacePath = Path.Combine(rootPath, "quality");

        try
        {
            var model = MetaDataQualityModel.CreateEmpty();
            AddJoinPatternForCli(
                model,
                patternId: "JoinPattern.Scalar.Cli",
                keyPairs:
                [
                    ("ScalarExpression:5840", "ScalarExpression:5844"),
                ],
                joinType: "LeftOuter");
            AddOccurrenceForCli(
                model,
                occurrenceId: "Occ.Scalar.Cli",
                patternId: "JoinPattern.Scalar.Cli",
                scriptName: "Script.Scalar.Cli",
                leftTable: "sales.ExpressionSource",
                rightTable: "sales.CalendarException");
            AddDiscoveredJoinOrphanCandidateForCli(model, "JoinPattern.Scalar.Cli");
            TypedWorkspaceXmlSerializer.Save(model, qualityWorkspacePath);
            MetaCliWorkspace.DescribeXml(qualityWorkspacePath);

            var inspect = RunCli($"inspect --workspace \"{qualityWorkspacePath}\" --show-cases --top-cases 1");

            Assert.True(inspect.ExitCode == 0, inspect.Output);
            Assert.Contains("Keys: (scalar expression) = (scalar expression)", inspect.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("ScalarExpression:", inspect.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryIfExists(rootPath);
        }
    }

    private static (int ExitCode, string Output) RunCli(string arguments, string? workingDirectory = null) =>
        CliTestRunner.RunStandardCli("meta-data-quality", arguments, workingDirectory: workingDirectory);

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }

    private static DataQualityCandidate AddCandidate(
        MetaDataQualityModel model,
        string id,
        string name)
    {
        var candidate = new DataQualityCandidate
        {
            Id = id,
            Name = name,
            Status = CandidateStatuses.Discovered,
            Rationale = "Test candidate.",
            Assumptions = string.Empty,
            SqlTemplate = "SELECT 1;",
        };
        model.DataQualityCandidateList.Add(candidate);
        return candidate;
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

    private static void AddDiscoveredJoinOrphanCandidateForCli(
        MetaDataQualityModel model,
        string patternId)
    {
        var pattern = model.JoinPatternList.Single(row => string.Equals(row.Id, patternId, StringComparison.Ordinal));
        var candidate = new DataQualityCandidate
        {
            Id = $"{patternId}.Candidate.JoinOrphan",
            Name = $"JoinOrphan:{patternId}",
            Status = CandidateStatuses.Discovered,
            Rationale = "Test candidate.",
            Assumptions = string.Empty,
            SqlTemplate = "SELECT 1;",
        };
        model.DataQualityCandidateList.Add(candidate);
        model.JoinOrphanList.Add(new JoinOrphan
        {
            Id = $"{candidate.Id}.JoinOrphan",
            DataQualityCandidate = candidate,
            EqualityPredicateCount = "1",
        });
        model.DataQualityCandidateJoinPatternLinkList.Add(new DataQualityCandidateJoinPatternLink
        {
            Id = $"{candidate.Id}.JoinPatternLink",
            DataQualityCandidate = candidate,
            JoinPattern = pattern,
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
