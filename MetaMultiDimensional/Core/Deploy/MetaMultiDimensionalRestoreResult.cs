namespace MetaMultiDimensional.Core.Deploy;

public sealed record MetaMultiDimensionalRestoreResult
{
    public required string SourceServer { get; init; }

    public required string SourceDatabaseName { get; init; }

    public required string TargetServer { get; init; }

    public required string TargetDatabaseName { get; init; }

    public required string BackupFile { get; init; }

    public required bool DroppedExisting { get; init; }

    public required bool OverwriteBackupFile { get; init; }
}
