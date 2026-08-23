namespace MetaPipeline;

public static class MetaPipelineOperationalDbSchema
{
    public const int CurrentVersion = 7;
    public const string DefaultDatabaseName = "MetaPipeline";

    public static string BootstrapSql { get; } = """
IF SCHEMA_ID(N'MetaPipeline') IS NULL
    EXEC(N'CREATE SCHEMA [MetaPipeline]');

IF OBJECT_ID(N'[MetaPipeline].[SchemaVersion]', N'U') IS NULL
BEGIN
    CREATE TABLE [MetaPipeline].[SchemaVersion]
    (
        [Version] int NOT NULL CONSTRAINT [PK_MetaPipeline_SchemaVersion] PRIMARY KEY,
        [AppliedAtUtc] datetimeoffset(7) NOT NULL CONSTRAINT [DF_MetaPipeline_SchemaVersion_AppliedAtUtc] DEFAULT SYSUTCDATETIME()
    );
END;

IF OBJECT_ID(N'[MetaPipeline].[PipelineRun]', N'U') IS NULL
BEGIN
    CREATE TABLE [MetaPipeline].[PipelineRun]
    (
        [PipelineRunId] uniqueidentifier NOT NULL CONSTRAINT [PK_MetaPipeline_PipelineRun] PRIMARY KEY,
        [StartedAtUtc] datetimeoffset(7) NOT NULL,
        [CompletedAtUtc] datetimeoffset(7) NULL,
        [Status] nvarchar(32) NOT NULL,
        [PipelineWorkspacePath] nvarchar(1024) NULL,
        [PipelineId] nvarchar(512) NULL,
        [PipelineName] nvarchar(512) NULL,
        [TransformTaskId] nvarchar(512) NULL,
        [TransformTaskName] nvarchar(512) NULL,
        [TargetWriteTaskId] nvarchar(512) NULL,
        [TargetWriteTaskName] nvarchar(512) NULL,
        [TransformWorkspacePath] nvarchar(1024) NULL,
        [BindingWorkspacePath] nvarchar(1024) NULL,
        [TransformScriptId] nvarchar(512) NULL,
        [TransformBindingId] nvarchar(512) NULL,
        [TransformScriptName] nvarchar(512) NULL,
        [ExecutionConnectionReferenceName] nvarchar(256) NULL,
        [ExecutionConnectionEnvironmentVariableName] nvarchar(256) NULL,
        [TargetConnectionReferenceName] nvarchar(256) NULL,
        [TargetConnectionEnvironmentVariableName] nvarchar(256) NULL,
        [TargetSqlIdentifier] nvarchar(512) NULL,
        [TargetWriteModelName] nvarchar(128) NULL,
        [BatchSize] int NULL,
        [FailureStage] nvarchar(64) NULL,
        [FailureKind] nvarchar(64) NULL,
        [FailureMessage] nvarchar(max) NULL
    );
END;

IF OBJECT_ID(N'[MetaPipeline].[TaskRun]', N'U') IS NULL
BEGIN
    CREATE TABLE [MetaPipeline].[TaskRun]
    (
        [TaskRunId] uniqueidentifier NOT NULL CONSTRAINT [PK_MetaPipeline_TaskRun] PRIMARY KEY,
        [PipelineRunId] uniqueidentifier NOT NULL,
        [AuditId] bigint NULL,
        [TaskName] nvarchar(512) NOT NULL,
        [TaskKind] nvarchar(128) NOT NULL,
        [TransformScriptId] nvarchar(512) NULL,
        [TransformScriptName] nvarchar(512) NULL,
        [StartedAtUtc] datetimeoffset(7) NOT NULL,
        [CompletedAtUtc] datetimeoffset(7) NOT NULL,
        [Status] nvarchar(32) NOT NULL,
        [TimeoutSeconds] int NULL,
        [FailureStage] nvarchar(64) NULL,
        [FailureMessage] nvarchar(max) NULL,
        CONSTRAINT [FK_MetaPipeline_TaskRun_PipelineRun] FOREIGN KEY ([PipelineRunId])
            REFERENCES [MetaPipeline].[PipelineRun] ([PipelineRunId])
    );
END;

IF OBJECT_ID(N'[MetaPipeline].[AuditIdSequence]', N'SO') IS NULL
BEGIN
    CREATE SEQUENCE [MetaPipeline].[AuditIdSequence]
        AS bigint
        START WITH 1
        INCREMENT BY 1;
END;

IF COL_LENGTH(N'MetaPipeline.TaskRun', N'AuditId') IS NULL
BEGIN
    ALTER TABLE [MetaPipeline].[TaskRun]
        ADD [AuditId] bigint NULL;
END;

IF OBJECT_ID(N'[MetaPipeline].[RunMetric]', N'U') IS NULL
BEGIN
    CREATE TABLE [MetaPipeline].[RunMetric]
    (
        [RunMetricId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MetaPipeline_RunMetric] PRIMARY KEY,
        [PipelineRunId] uniqueidentifier NOT NULL,
        [TaskRunId] uniqueidentifier NULL,
        [MetricName] nvarchar(128) NOT NULL,
        [MetricValue] decimal(38, 6) NOT NULL,
        [MetricUnit] nvarchar(64) NULL,
        CONSTRAINT [FK_MetaPipeline_RunMetric_PipelineRun] FOREIGN KEY ([PipelineRunId])
            REFERENCES [MetaPipeline].[PipelineRun] ([PipelineRunId]),
        CONSTRAINT [FK_MetaPipeline_RunMetric_TaskRun] FOREIGN KEY ([TaskRunId])
            REFERENCES [MetaPipeline].[TaskRun] ([TaskRunId])
    );
END;

IF OBJECT_ID(N'[MetaPipeline].[RunLog]', N'U') IS NULL
BEGIN
    CREATE TABLE [MetaPipeline].[RunLog]
    (
        [RunLogId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MetaPipeline_RunLog] PRIMARY KEY,
        [PipelineRunId] uniqueidentifier NOT NULL,
        [TaskRunId] uniqueidentifier NULL,
        [LoggedAtUtc] datetimeoffset(7) NOT NULL,
        [Level] nvarchar(32) NOT NULL,
        [Category] nvarchar(128) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        CONSTRAINT [FK_MetaPipeline_RunLog_PipelineRun] FOREIGN KEY ([PipelineRunId])
            REFERENCES [MetaPipeline].[PipelineRun] ([PipelineRunId]),
        CONSTRAINT [FK_MetaPipeline_RunLog_TaskRun] FOREIGN KEY ([TaskRunId])
            REFERENCES [MetaPipeline].[TaskRun] ([TaskRunId])
    );
END;

IF OBJECT_ID(N'[MetaPipeline].[RunDiagnosticsLog]', N'U') IS NULL
BEGIN
    CREATE TABLE [MetaPipeline].[RunDiagnosticsLog]
    (
        [RunDiagnosticsLogId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MetaPipeline_RunDiagnosticsLog] PRIMARY KEY,
        [PipelineRunId] uniqueidentifier NOT NULL,
        [TaskRunId] uniqueidentifier NULL,
        [LoggedAtUtc] datetimeoffset(7) NOT NULL,
        [Level] nvarchar(32) NOT NULL,
        [Category] nvarchar(128) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        CONSTRAINT [FK_MetaPipeline_RunDiagnosticsLog_PipelineRun] FOREIGN KEY ([PipelineRunId])
            REFERENCES [MetaPipeline].[PipelineRun] ([PipelineRunId]),
        CONSTRAINT [FK_MetaPipeline_RunDiagnosticsLog_TaskRun] FOREIGN KEY ([TaskRunId])
            REFERENCES [MetaPipeline].[TaskRun] ([TaskRunId])
    );
END;

IF OBJECT_ID(N'[MetaPipeline].[RunFailure]', N'U') IS NULL
BEGIN
    CREATE TABLE [MetaPipeline].[RunFailure]
    (
        [RunFailureId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_MetaPipeline_RunFailure] PRIMARY KEY,
        [PipelineRunId] uniqueidentifier NOT NULL,
        [TaskRunId] uniqueidentifier NULL,
        [FailureKind] nvarchar(64) NOT NULL,
        [FailureStage] nvarchar(64) NOT NULL,
        [OccurredAtUtc] datetimeoffset(7) NOT NULL CONSTRAINT [DF_MetaPipeline_RunFailure_OccurredAtUtc] DEFAULT SYSUTCDATETIME(),
        [ExceptionType] nvarchar(512) NULL,
        [Message] nvarchar(max) NOT NULL,
        CONSTRAINT [FK_MetaPipeline_RunFailure_PipelineRun] FOREIGN KEY ([PipelineRunId])
            REFERENCES [MetaPipeline].[PipelineRun] ([PipelineRunId]),
        CONSTRAINT [FK_MetaPipeline_RunFailure_TaskRun] FOREIGN KEY ([TaskRunId])
            REFERENCES [MetaPipeline].[TaskRun] ([TaskRunId])
    );
END;

IF COL_LENGTH(N'MetaPipeline.RunFailure', N'OccurredAtUtc') IS NULL
BEGIN
    ALTER TABLE [MetaPipeline].[RunFailure]
        ADD [OccurredAtUtc] datetimeoffset(7) NOT NULL
            CONSTRAINT [DF_MetaPipeline_RunFailure_OccurredAtUtc] DEFAULT SYSUTCDATETIME();
END;

IF NOT EXISTS (SELECT 1 FROM [MetaPipeline].[SchemaVersion] WHERE [Version] = 1)
BEGIN
    INSERT INTO [MetaPipeline].[SchemaVersion] ([Version]) VALUES (1);
END;

IF NOT EXISTS (SELECT 1 FROM [MetaPipeline].[SchemaVersion] WHERE [Version] = 2)
BEGIN
    INSERT INTO [MetaPipeline].[SchemaVersion] ([Version]) VALUES (2);
END;

IF NOT EXISTS (SELECT 1 FROM [MetaPipeline].[SchemaVersion] WHERE [Version] = 3)
BEGIN
    INSERT INTO [MetaPipeline].[RunDiagnosticsLog]
    (
        [PipelineRunId],
        [TaskRunId],
        [LoggedAtUtc],
        [Level],
        [Category],
        [Message]
    )
    SELECT
        [PipelineRunId],
        [TaskRunId],
        [LoggedAtUtc],
        [Level],
        [Category],
        [Message]
    FROM [MetaPipeline].[RunLog]
    WHERE [Level] = N'Information';

    DELETE FROM [MetaPipeline].[RunLog]
    WHERE [Level] = N'Information';

    INSERT INTO [MetaPipeline].[SchemaVersion] ([Version]) VALUES (3);
END;

IF COL_LENGTH(N'MetaPipeline.PipelineRun', N'ExecutionConnectionReferenceName') IS NULL
   AND COL_LENGTH(N'MetaPipeline.PipelineRun', N'SourceConnectionReferenceName') IS NOT NULL
BEGIN
    EXEC sp_rename
        N'MetaPipeline.PipelineRun.SourceConnectionReferenceName',
        N'ExecutionConnectionReferenceName',
        N'COLUMN';
END;

IF COL_LENGTH(N'MetaPipeline.PipelineRun', N'ExecutionConnectionEnvironmentVariableName') IS NULL
   AND COL_LENGTH(N'MetaPipeline.PipelineRun', N'SourceConnectionEnvironmentVariableName') IS NOT NULL
BEGIN
    EXEC sp_rename
        N'MetaPipeline.PipelineRun.SourceConnectionEnvironmentVariableName',
        N'ExecutionConnectionEnvironmentVariableName',
        N'COLUMN';
END;

IF NOT EXISTS (SELECT 1 FROM [MetaPipeline].[SchemaVersion] WHERE [Version] = 4)
BEGIN
    INSERT INTO [MetaPipeline].[SchemaVersion] ([Version]) VALUES (4);
END;

IF COL_LENGTH(N'MetaPipeline.TaskRun', N'TimeoutSeconds') IS NULL
BEGIN
    ALTER TABLE [MetaPipeline].[TaskRun]
        ADD [TimeoutSeconds] int NULL;
END;

IF NOT EXISTS (SELECT 1 FROM [MetaPipeline].[SchemaVersion] WHERE [Version] = 5)
BEGIN
    INSERT INTO [MetaPipeline].[SchemaVersion] ([Version]) VALUES (5);
END;

IF NOT EXISTS (SELECT 1 FROM [MetaPipeline].[SchemaVersion] WHERE [Version] = 6)
BEGIN
    INSERT INTO [MetaPipeline].[SchemaVersion] ([Version]) VALUES (6);
END;

IF COL_LENGTH(N'MetaPipeline.TaskRun', N'TransformScriptId') IS NULL
BEGIN
    ALTER TABLE [MetaPipeline].[TaskRun]
        ADD [TransformScriptId] nvarchar(512) NULL;
END;

IF COL_LENGTH(N'MetaPipeline.TaskRun', N'TransformScriptName') IS NULL
BEGIN
    ALTER TABLE [MetaPipeline].[TaskRun]
        ADD [TransformScriptName] nvarchar(512) NULL;
END;

IF NOT EXISTS (SELECT 1 FROM [MetaPipeline].[SchemaVersion] WHERE [Version] = 7)
BEGIN
    INSERT INTO [MetaPipeline].[SchemaVersion] ([Version]) VALUES (7);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_PipelineRun_StartedAtUtc'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[PipelineRun]', N'U'))
BEGIN
    CREATE INDEX [IX_MetaPipeline_PipelineRun_StartedAtUtc]
        ON [MetaPipeline].[PipelineRun] ([StartedAtUtc] DESC);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_PipelineRun_CompletedAtUtc'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[PipelineRun]', N'U'))
BEGIN
    CREATE INDEX [IX_MetaPipeline_PipelineRun_CompletedAtUtc]
        ON [MetaPipeline].[PipelineRun] ([CompletedAtUtc] DESC)
        WHERE [CompletedAtUtc] IS NOT NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_PipelineRun_Status_StartedAtUtc'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[PipelineRun]', N'U'))
BEGIN
    CREATE INDEX [IX_MetaPipeline_PipelineRun_Status_StartedAtUtc]
        ON [MetaPipeline].[PipelineRun] ([Status], [StartedAtUtc] DESC);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_TaskRun_PipelineRunId'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[TaskRun]', N'U'))
BEGIN
    CREATE INDEX [IX_MetaPipeline_TaskRun_PipelineRunId]
        ON [MetaPipeline].[TaskRun] ([PipelineRunId]);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'UX_MetaPipeline_TaskRun_AuditId'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[TaskRun]', N'U'))
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_MetaPipeline_TaskRun_AuditId]
        ON [MetaPipeline].[TaskRun] ([AuditId])
        WHERE [AuditId] IS NOT NULL;');
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_TaskRun_StartedAtUtc'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[TaskRun]', N'U'))
BEGIN
    CREATE INDEX [IX_MetaPipeline_TaskRun_StartedAtUtc]
        ON [MetaPipeline].[TaskRun] ([StartedAtUtc] DESC);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_TaskRun_CompletedAtUtc'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[TaskRun]', N'U'))
BEGIN
    CREATE INDEX [IX_MetaPipeline_TaskRun_CompletedAtUtc]
        ON [MetaPipeline].[TaskRun] ([CompletedAtUtc] DESC);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_RunMetric_PipelineRunId'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[RunMetric]', N'U'))
BEGIN
    CREATE INDEX [IX_MetaPipeline_RunMetric_PipelineRunId]
        ON [MetaPipeline].[RunMetric] ([PipelineRunId], [MetricName]);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_RunLog_LoggedAtUtc'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[RunLog]', N'U'))
BEGIN
    CREATE INDEX [IX_MetaPipeline_RunLog_LoggedAtUtc]
        ON [MetaPipeline].[RunLog] ([LoggedAtUtc] DESC);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_RunLog_PipelineRunId_LoggedAtUtc'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[RunLog]', N'U'))
BEGIN
    CREATE INDEX [IX_MetaPipeline_RunLog_PipelineRunId_LoggedAtUtc]
        ON [MetaPipeline].[RunLog] ([PipelineRunId], [LoggedAtUtc] DESC);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_RunDiagnosticsLog_LoggedAtUtc'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[RunDiagnosticsLog]', N'U'))
BEGIN
    CREATE INDEX [IX_MetaPipeline_RunDiagnosticsLog_LoggedAtUtc]
        ON [MetaPipeline].[RunDiagnosticsLog] ([LoggedAtUtc] DESC);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_RunDiagnosticsLog_PipelineRunId_LoggedAtUtc'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[RunDiagnosticsLog]', N'U'))
BEGIN
    CREATE INDEX [IX_MetaPipeline_RunDiagnosticsLog_PipelineRunId_LoggedAtUtc]
        ON [MetaPipeline].[RunDiagnosticsLog] ([PipelineRunId], [LoggedAtUtc] DESC);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_RunFailure_OccurredAtUtc'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[RunFailure]', N'U'))
BEGIN
    EXEC(N'CREATE INDEX [IX_MetaPipeline_RunFailure_OccurredAtUtc]
        ON [MetaPipeline].[RunFailure] ([OccurredAtUtc] DESC);');
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_MetaPipeline_RunFailure_PipelineRunId_OccurredAtUtc'
      AND [object_id] = OBJECT_ID(N'[MetaPipeline].[RunFailure]', N'U'))
BEGIN
    EXEC(N'CREATE INDEX [IX_MetaPipeline_RunFailure_PipelineRunId_OccurredAtUtc]
        ON [MetaPipeline].[RunFailure] ([PipelineRunId], [OccurredAtUtc] DESC);');
END;

""";
}
