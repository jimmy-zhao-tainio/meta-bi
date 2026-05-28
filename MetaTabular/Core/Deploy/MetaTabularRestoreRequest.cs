namespace MetaTabular.Core.Deploy;

public sealed record MetaTabularRestoreRequest
{
    public required string SourceServer { get; init; }

    public required string SourceDatabaseName { get; init; }

    public required string TargetServer { get; init; }

    public required string TargetDatabaseName { get; init; }

    public required string BackupFile { get; init; }

    public bool DropExisting { get; init; }

    public bool OverwriteBackupFile { get; init; }
}
