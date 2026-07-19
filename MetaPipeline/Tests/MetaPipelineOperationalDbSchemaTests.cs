namespace MetaPipeline.Tests;

public sealed class MetaPipelineOperationalDbSchemaTests
{
    [Fact]
    public void BootstrapSql_IsOperationalEvidenceOnly()
    {
        var sql = global::MetaPipeline.MetaPipelineOperationalDbSchema.BootstrapSql;

        Assert.Contains("[MetaPipeline].[PipelineRun]", sql, StringComparison.Ordinal);
        Assert.Contains("[MetaPipeline].[TaskRun]", sql, StringComparison.Ordinal);
        Assert.Contains("[MetaPipeline].[RunMetric]", sql, StringComparison.Ordinal);
        Assert.Contains("[MetaPipeline].[RunLog]", sql, StringComparison.Ordinal);
        Assert.Contains("[MetaPipeline].[RunDiagnosticsLog]", sql, StringComparison.Ordinal);
        Assert.Contains("[MetaPipeline].[RunFailure]", sql, StringComparison.Ordinal);
        Assert.Contains("[MetaPipeline].[RunFingerprint]", sql, StringComparison.Ordinal);
        Assert.Contains("[OccurredAtUtc]", sql, StringComparison.Ordinal);
        Assert.Contains("[MetaPipeline].[AuditIdSequence]", sql, StringComparison.Ordinal);
        Assert.Contains("[AuditId] bigint", sql, StringComparison.Ordinal);
        Assert.DoesNotContain("CREATE DATABASE", sql, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Watermark", sql, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Checkpoint", sql, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Schedule", sql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BootstrapSql_DoesNotPersistConnectionStrings()
    {
        var sql = global::MetaPipeline.MetaPipelineOperationalDbSchema.BootstrapSql;

        Assert.Contains("[ExecutionConnectionEnvironmentVariableName]", sql, StringComparison.Ordinal);
        Assert.Contains("[TargetConnectionEnvironmentVariableName]", sql, StringComparison.Ordinal);
        Assert.DoesNotContain("ConnectionString", sql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BootstrapSql_AddsDateIndexesForOperationalQueries()
    {
        var sql = global::MetaPipeline.MetaPipelineOperationalDbSchema.BootstrapSql;

        Assert.Contains("IX_MetaPipeline_PipelineRun_StartedAtUtc", sql, StringComparison.Ordinal);
        Assert.Contains("IX_MetaPipeline_PipelineRun_CompletedAtUtc", sql, StringComparison.Ordinal);
        Assert.Contains("IX_MetaPipeline_PipelineRun_Status_StartedAtUtc", sql, StringComparison.Ordinal);
        Assert.Contains("IX_MetaPipeline_TaskRun_StartedAtUtc", sql, StringComparison.Ordinal);
        Assert.Contains("IX_MetaPipeline_TaskRun_CompletedAtUtc", sql, StringComparison.Ordinal);
        Assert.Contains("UX_MetaPipeline_TaskRun_AuditId", sql, StringComparison.Ordinal);
        Assert.Contains("IX_MetaPipeline_RunLog_LoggedAtUtc", sql, StringComparison.Ordinal);
        Assert.Contains("IX_MetaPipeline_RunDiagnosticsLog_LoggedAtUtc", sql, StringComparison.Ordinal);
        Assert.Contains("IX_MetaPipeline_RunFailure_OccurredAtUtc", sql, StringComparison.Ordinal);
        Assert.Contains("IX_MetaPipeline_RunFingerprint_PipelineRunId_Kind", sql, StringComparison.Ordinal);
        Assert.Contains("IX_MetaPipeline_RunFingerprint_TaskRunId_Kind", sql, StringComparison.Ordinal);
    }

    [Fact]
    public void BootstrapSql_MigratesInformationalLogsToDiagnosticsTable()
    {
        var sql = global::MetaPipeline.MetaPipelineOperationalDbSchema.BootstrapSql;

        Assert.Equal(7, global::MetaPipeline.MetaPipelineOperationalDbSchema.CurrentVersion);
        Assert.Contains("INSERT INTO [MetaPipeline].[RunDiagnosticsLog]", sql, StringComparison.Ordinal);
        Assert.Contains("FROM [MetaPipeline].[RunLog]", sql, StringComparison.Ordinal);
        Assert.Contains("[Level]", sql, StringComparison.Ordinal);
        Assert.Contains("WHERE [Level] = N'Information'", sql, StringComparison.Ordinal);
        Assert.Contains("DELETE FROM [MetaPipeline].[RunLog]", sql, StringComparison.Ordinal);
        Assert.Contains("[Version] = 3", sql, StringComparison.Ordinal);
    }

    [Fact]
    public void BootstrapSql_MigratesSourceConnectionEvidenceColumnsToExecutionConnection()
    {
        var sql = global::MetaPipeline.MetaPipelineOperationalDbSchema.BootstrapSql;

        Assert.Contains("[ExecutionConnectionReferenceName]", sql, StringComparison.Ordinal);
        Assert.Contains("[ExecutionConnectionEnvironmentVariableName]", sql, StringComparison.Ordinal);
        Assert.Contains("SourceConnectionReferenceName", sql, StringComparison.Ordinal);
        Assert.Contains("SourceConnectionEnvironmentVariableName", sql, StringComparison.Ordinal);
        Assert.Contains("sp_rename", sql, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("[Version] = 4", sql, StringComparison.Ordinal);
    }

    [Fact]
    public void BootstrapSql_AddsTaskTimeoutAndFingerprintEvidence()
    {
        var sql = global::MetaPipeline.MetaPipelineOperationalDbSchema.BootstrapSql;

        Assert.Contains("[TimeoutSeconds] int NULL", sql, StringComparison.Ordinal);
        Assert.Contains("COL_LENGTH(N'MetaPipeline.TaskRun', N'TimeoutSeconds')", sql, StringComparison.Ordinal);
        Assert.Contains("[MetaPipeline].[RunFingerprint]", sql, StringComparison.Ordinal);
        Assert.Contains("[FingerprintKind]", sql, StringComparison.Ordinal);
        Assert.Contains("[FingerprintValue]", sql, StringComparison.Ordinal);
        Assert.Contains("[Algorithm]", sql, StringComparison.Ordinal);
        Assert.Contains("[Version] = 5", sql, StringComparison.Ordinal);
        Assert.Contains("[Version] = 6", sql, StringComparison.Ordinal);
    }

    [Fact]
    public void BootstrapSql_StoresTransformIdentityOnTaskRuns()
    {
        var sql = global::MetaPipeline.MetaPipelineOperationalDbSchema.BootstrapSql;

        Assert.Equal(7, global::MetaPipeline.MetaPipelineOperationalDbSchema.CurrentVersion);
        Assert.Contains("COL_LENGTH(N'MetaPipeline.TaskRun', N'TransformScriptId')", sql, StringComparison.Ordinal);
        Assert.Contains("[TransformScriptId] nvarchar(512) NULL", sql, StringComparison.Ordinal);
        Assert.Contains("COL_LENGTH(N'MetaPipeline.TaskRun', N'TransformScriptName')", sql, StringComparison.Ordinal);
        Assert.Contains("[TransformScriptName] nvarchar(512) NULL", sql, StringComparison.Ordinal);
        Assert.Contains("[Version] = 7", sql, StringComparison.Ordinal);
    }
}
