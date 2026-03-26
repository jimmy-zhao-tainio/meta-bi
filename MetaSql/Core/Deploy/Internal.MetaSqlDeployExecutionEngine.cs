using Meta.Core.Services;
using Meta.Core.Domain;
using MetaSql.Extractors.SqlServer;

namespace MetaSql;

internal sealed class MetaSqlDeployExecutionEngine
{
    private readonly DeployManifestContractValidator manifestContractValidator = new();
    private readonly DeployManifestFingerprintValidator manifestFingerprintValidator = new();
    private readonly DeployStatementPlanBuilder statementPlanBuilder = new();
    private readonly SqlServerDeploySqlRenderer sqlRenderer = new();
    private readonly SqlBatchExecutor sqlBatchExecutor = new();

    public async Task<MetaSqlDeployResult> ExecuteCoreAsync(
        MetaSqlDeployRequest request,
        CancellationToken cancellationToken = default)
    {
        manifestFingerprintValidator.ValidatePreconditions(request);
        var databaseCreated = false;

        var manifestWorkspacePath = Path.GetFullPath(request.ManifestWorkspacePath);
        var sourceWorkspacePath = Path.GetFullPath(request.SourceWorkspacePath);
        var tempRootPath = Path.Combine(Path.GetTempPath(), "MetaSql.Core", "deploy", Guid.NewGuid().ToString("N"));
        var liveWorkspacePath = Path.Combine(tempRootPath, "live-metasql");

        Directory.CreateDirectory(tempRootPath);
        try
        {
            manifestContractValidator.Validate(manifestWorkspacePath);

            var manifestModel = await MetaSqlDeployManifest.MetaSqlDeployManifestModel.LoadFromXmlWorkspaceAsync(
                    manifestWorkspacePath,
                    searchUpward: false,
                    cancellationToken)
                .ConfigureAwait(false);
            var root = manifestContractValidator.RequireSingleManifestRoot(manifestModel);

            var blockCount = statementPlanBuilder.CountBlocks(manifestModel);
            if (blockCount > 0)
            {
                throw new InvalidOperationException(
                    $"Manifest '{root.Name}' is non-deployable. BlockCount={blockCount}.");
            }

            var workspaceService = new WorkspaceService();
            var sourceWorkspace = await workspaceService
                .LoadAsync(sourceWorkspacePath, searchUpward: false, cancellationToken)
                .ConfigureAwait(false);
            MetaSqlDiffService.EnsureMetaSqlWorkspace(sourceWorkspace, nameof(sourceWorkspace));
            manifestFingerprintValidator.ValidateSourceFingerprint(root, sourceWorkspace);

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

            Workspace liveWorkspace;
            if (expectedLiveDatabasePresence == MetaSqlLiveDatabasePresence.Missing)
            {
                liveWorkspace = SqlServerMetaSqlWorkspaceFactory.CreateEmptyWorkspace(
                    liveWorkspacePath,
                    SqlServerDatabaseRuntime.RequireDatabaseName(request.ConnectionString));
            }
            else
            {
                var extractor = new SqlServerMetaSqlExtractor();
                liveWorkspace = extractor.ExtractMetaSqlWorkspace(new SqlServerExtractRequest
                {
                    NewWorkspacePath = liveWorkspacePath,
                    ConnectionString = request.ConnectionString,
                    AllowEmpty = true,
                });
            }
            MetaSqlDiffService.EnsureMetaSqlWorkspace(liveWorkspace, nameof(liveWorkspace));
            manifestFingerprintValidator.ValidateLiveFingerprint(root, liveWorkspace);

            var sourceModel = await MetaSqlModel.LoadFromXmlWorkspaceAsync(
                    sourceWorkspacePath,
                    searchUpward: false,
                    cancellationToken)
                .ConfigureAwait(false);
            var liveModel = await MetaSqlModel.LoadFromXmlWorkspaceAsync(
                    liveWorkspacePath,
                    searchUpward: false,
                    cancellationToken)
                .ConfigureAwait(false);

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
