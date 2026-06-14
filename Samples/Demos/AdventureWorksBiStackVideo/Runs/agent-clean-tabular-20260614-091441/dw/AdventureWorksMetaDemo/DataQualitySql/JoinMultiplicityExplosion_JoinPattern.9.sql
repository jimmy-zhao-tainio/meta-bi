IF SCHEMA_ID(N'dq') IS NULL EXEC(N'CREATE SCHEMA [dq]');
GO

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.9' resolves to 13 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_9_JoinMultiplicityExplosion]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO
