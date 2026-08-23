using Tom = Microsoft.AnalysisServices.Tabular;

namespace MetaTabular.Core.Deploy;

public sealed class MetaTabularRestoreService
{
    public Task<MetaTabularRestoreResult> RestoreAsync(MetaTabularRestoreRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.SourceServer);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.SourceDatabaseName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TargetServer);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TargetDatabaseName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.BackupFile);

        if (string.Equals(request.SourceServer.Trim(), request.TargetServer.Trim(), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(request.SourceDatabaseName.Trim(), request.TargetDatabaseName.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Tabular restore requires different source and target database names when source and target server are the same.");
        }

        using var sourceServer = new Tom.Server();
        sourceServer.Connect($"Data Source={request.SourceServer}");
        var sourceDatabase = FindDatabase(sourceServer, request.SourceDatabaseName)
            ?? throw new InvalidOperationException($"Source tabular database '{request.SourceDatabaseName}' does not exist on '{request.SourceServer}'.");

        sourceDatabase.Backup(request.BackupFile, request.OverwriteBackupFile);

        using var targetServer = new Tom.Server();
        targetServer.Connect($"Data Source={request.TargetServer}");
        var existingTarget = FindDatabase(targetServer, request.TargetDatabaseName);
        var droppedExisting = false;
        if (existingTarget != null)
        {
            if (!request.DropExisting)
            {
                throw new InvalidOperationException($"Target tabular database '{request.TargetDatabaseName}' already exists on '{request.TargetServer}'. Pass --drop-existing to drop it before restore.");
            }

            existingTarget.Drop();
            targetServer.Refresh();
            droppedExisting = true;
        }

        targetServer.Restore(request.BackupFile, request.TargetDatabaseName, allowOverwrite: false);
        targetServer.Refresh();

        return Task.FromResult(new MetaTabularRestoreResult
        {
            SourceServer = request.SourceServer,
            SourceDatabaseName = request.SourceDatabaseName,
            TargetServer = request.TargetServer,
            TargetDatabaseName = request.TargetDatabaseName,
            BackupFile = request.BackupFile,
            DroppedExisting = droppedExisting,
            OverwriteBackupFile = request.OverwriteBackupFile,
        });
    }

    private static Tom.Database? FindDatabase(Tom.Server server, string databaseName)
    {
        return server.Databases
            .OfType<Tom.Database>()
            .FirstOrDefault(database =>
                string.Equals(database.ID, databaseName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(database.Name, databaseName, StringComparison.OrdinalIgnoreCase));
    }
}
