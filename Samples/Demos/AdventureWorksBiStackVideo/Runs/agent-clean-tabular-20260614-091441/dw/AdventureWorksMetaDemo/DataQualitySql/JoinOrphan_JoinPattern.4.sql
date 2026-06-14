IF SCHEMA_ID(N'dq') IS NULL EXEC(N'CREATE SCHEMA [dq]');
GO

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.4' resolves to 8 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_4_JoinOrphan]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO
