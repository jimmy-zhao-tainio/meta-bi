namespace MetaSql.App;

public sealed record MetaSqlTargetContext(
    MetaSqlRootMode Mode,
    string Name,
    string RootDirectory,
    string ConfigPath,
    string DesiredSqlPath,
    string? TraitsFilePath,
    string ConnectionString,
    string MigrationRootPath,
    string BaselinePath,
    string TargetPath,
    string ArchivePath);
