using Meta.Operations.Domain;
using Meta.Integration;
using MetaSql.Extractors.SqlServer;
using MetaDataType.Core;

namespace MetaSql;

internal sealed class MetaSqlDeployExecutionEngine
{
    private readonly DeployManifestContractValidator manifestContractValidator = new();
    private readonly DeployManifestFingerprintValidator manifestFingerprintValidator = new();
    private readonly DeployStatementPlanBuilder statementPlanBuilder = new();
    private readonly SqlBatchExecutor sqlBatchExecutor = new();

    public async Task<MetaSqlDeployResult> ExecuteCoreAsync(
        MetaSqlDeployRequest request,
        CancellationToken cancellationToken = default)
    {
        manifestFingerprintValidator.ValidatePreconditions(request);
        var catalog = MetaDataTypeWorkspaceProvider.LoadOrDefault(request.DataTypeWorkspacePath);
        var types = new SqlServerDataTypeCatalog(catalog);
        var sqlRenderer = new SqlServerDeploySqlRenderer(types);
        var databaseCreated = false;

        var manifestWorkspacePath = Path.GetFullPath(request.ManifestWorkspacePath);
        var sourceWorkspacePath = Path.GetFullPath(request.SourceWorkspacePath);
        var tempRootPath = Path.Combine(Path.GetTempPath(), "MetaSql.Core", "deploy", Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(tempRootPath);
        try
        {
            var manifestWorkspace = await TypedWorkspaceModelMapper.LoadStateAsync(
                    manifestWorkspacePath,
                    cancellationToken)
                .ConfigureAwait(false);
            manifestContractValidator.Validate(manifestWorkspace);

            var manifestModel = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
                manifestWorkspace,
                new global::MetaSqlDeployManifest.MetaSqlDeployManifestWorkspaceBinding());
            var root = manifestContractValidator.RequireSingleManifestRoot(manifestModel);

            var blockCount = statementPlanBuilder.CountBlocks(manifestModel);
            if (blockCount > 0)
            {
                throw new InvalidOperationException(
                    $"Manifest '{root.Name}' is not deployable because it contains {blockCount} block {(blockCount == 1 ? "entry" : "entries")}.");
            }

            var sourceWorkspace = await TypedWorkspaceModelMapper.LoadStateAsync(
                    sourceWorkspacePath,
                    cancellationToken)
                .ConfigureAwait(false);
            MetaSqlDiffService.EnsureMetaSqlWorkspace(sourceWorkspace, nameof(sourceWorkspace));
            var sourceModel = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
                sourceWorkspace,
                new global::MetaSql.MetaSqlWorkspaceBinding());
            types.ValidateColumns(sourceWorkspace, "Source workspace");
            manifestFingerprintValidator.ValidateSourceFingerprint(root, sourceWorkspace, types);

            var expectedLiveDatabasePresence = ParseExpectedLiveDatabasePresence(root.ExpectedLiveDatabasePresence);
            var actualLiveDatabasePresence = await SqlServerDatabaseRuntime
                .GetPresenceAsync(request.ConnectionString, cancellationToken)
                .ConfigureAwait(false);
            if (expectedLiveDatabasePresence == MetaSqlLiveDatabasePresence.Missing)
            {
                if (actualLiveDatabasePresence == MetaSqlLiveDatabasePresence.Present)
                {
                    throw new InvalidOperationException(
                        "Manifest expects the live database to be missing so deploy can create it, but the database already exists.");
                }

                await SqlServerDatabaseRuntime.CreateDatabaseAsync(request.ConnectionString, cancellationToken)
                    .ConfigureAwait(false);
                databaseCreated = true;
            }
            else if (actualLiveDatabasePresence == MetaSqlLiveDatabasePresence.Missing)
            {
                throw new InvalidOperationException(
                    "Manifest expects an existing live database, but the database does not exist.");
            }

            InMemoryWorkspace liveWorkspace;
            if (expectedLiveDatabasePresence == MetaSqlLiveDatabasePresence.Missing)
            {
                liveWorkspace = SqlServerMetaSqlWorkspaceFactory.CreateEmptyWorkspace(
                    SqlServerDatabaseRuntime.RequireDatabaseName(request.ConnectionString));
            }
            else
            {
                var extractor = new SqlServerMetaSqlExtractor();
                liveWorkspace = extractor.ExtractMetaSqlWorkspace(new SqlServerExtractRequest
                {
                    ConnectionString = request.ConnectionString,
                    AllowEmpty = true,
                }, catalog);
            }
            MetaSqlDiffService.EnsureMetaSqlWorkspace(liveWorkspace, nameof(liveWorkspace));
            types.ValidateColumns(liveWorkspace, "Live workspace");
            manifestFingerprintValidator.ValidateLiveFingerprint(root, liveWorkspace, types);

            var liveModel = TypedWorkspaceModelMapper.FromInMemoryWorkspace(
                liveWorkspace,
                new global::MetaSql.MetaSqlWorkspaceBinding());

            var statementPlan = statementPlanBuilder.Build(manifestModel, sourceModel, liveModel);
            var statements = sqlRenderer.Render(statementPlan);
            await sqlBatchExecutor.ExecuteAsync(request.ConnectionString, statements, cancellationToken).ConfigureAwait(false);

            return new MetaSqlDeployResult
            {
                DatabaseCreated = databaseCreated,
                AppliedAddCount = statementPlan.AppliedAddCount,
                AppliedDropCount = statementPlan.AppliedDropCount,
                AppliedAlterCount = statementPlan.AppliedAlterCount,
                AppliedTruncateCount = statementPlan.AppliedTruncateCount,
                AppliedReplaceCount = statementPlan.AppliedReplaceCount,
                ExecutedStatementCount = statements.Count,
            };
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            if (databaseCreated)
            {
                throw new InvalidOperationException(
                    "Deploy failed after creating the database. Database created before failure.",
                    ex);
            }

            throw;
        }
        finally
        {
            if (Directory.Exists(tempRootPath))
            {
                Directory.Delete(tempRootPath, recursive: true);
            }
        }
    }

    private static MetaSqlLiveDatabasePresence ParseExpectedLiveDatabasePresence(string value)
    {
        return Enum.TryParse<MetaSqlLiveDatabasePresence>(value, ignoreCase: false, out var parsed)
            ? parsed
            : throw new InvalidOperationException(
                $"Manifest contains unknown ExpectedLiveDatabasePresence '{value}'.");
    }
}
