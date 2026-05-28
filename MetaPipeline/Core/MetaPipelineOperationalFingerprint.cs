namespace MetaPipeline;

public sealed record MetaPipelineOperationalFingerprint(
    string FingerprintKind,
    string? SubjectId,
    string? SubjectPath,
    string Algorithm,
    string FingerprintValue,
    string? TaskName = null,
    string? TaskKind = null);
