namespace MetaPipeline;

public sealed record MetaPipelineOperationalPruneResult(
    DateTimeOffset CutoffUtc,
    bool DryRun,
    long EligibleCompletedRuns,
    long RunDiagnosticsLogs);
