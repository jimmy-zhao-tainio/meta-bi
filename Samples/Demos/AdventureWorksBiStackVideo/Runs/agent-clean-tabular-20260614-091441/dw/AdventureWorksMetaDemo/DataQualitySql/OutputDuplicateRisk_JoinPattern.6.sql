IF SCHEMA_ID(N'dq') IS NULL EXEC(N'CREATE SCHEMA [dq]');
GO

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.6' resolves to 10 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_6_OutputDuplicateRisk]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO
