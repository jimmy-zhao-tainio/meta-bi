IF SCHEMA_ID(N'dq') IS NULL EXEC(N'CREATE SCHEMA [dq]');
GO

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.1' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_1_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.10' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_10_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.12' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_12_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.14' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_14_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
CREATE OR ALTER VIEW [dq].[v_Row_multiplication_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPerson]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesPersonHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.19' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_19_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.2' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_2_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.21' cannot be rendered: only simple column equality predicates can be rendered; found 'lineDetail.VersionRank' = 'ScalarExpression:1119'. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_21_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.22' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_22_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.23' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_23_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.24' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_24_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.25' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_25_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.26' resolves to 6 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_26_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.28' resolves to 8 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_28_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.29' resolves to 9 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_29_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.3' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_3_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.31' resolves to 11 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_31_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.32' resolves to 12 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_32_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.34' resolves to 14 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_34_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.35' resolves to 15 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_35_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.37' resolves to 17 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_37_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.38' resolves to 18 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_38_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.42' cannot be rendered: only simple column equality predicates can be rendered; found 'detail.VersionRank' = 'ScalarExpression:955'. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_42_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.43' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_43_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.44' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_44_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.46' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_46_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.47' resolves to 6 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_47_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.49' resolves to 8 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_49_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.50' resolves to 9 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_50_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.52' resolves to 11 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_52_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.53' resolves to 12 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_53_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.55' resolves to 14 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_55_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.56' resolves to 15 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_56_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.63' cannot be rendered: only simple column equality predicates can be rendered; found 'quotaDetail.VersionRank' = 'ScalarExpression:1238'. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_63_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.64' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_64_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.65' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_65_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Row multiplication */
/* Warning: Join relationship 'JoinPattern.8' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinMultiplicityExplosion_JoinPattern_8_JoinMultiplicityExplosion]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern' cannot be rendered: only simple column equality predicates can be rendered; found 'customerProfile.VersionRank' = 'ScalarExpression:777'. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.1' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_1_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.10' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_10_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.11' cannot be rendered: only simple column equality predicates can be rendered; found 'profile.VersionRank' = 'ScalarExpression:600'. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_11_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.12' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_12_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.13' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_13_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.14' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_14_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.15' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_15_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
CREATE OR ALTER VIEW [dq].[v_Missing_referenced_rows_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPer]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'SalesPersonHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[SalesPersonHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesPersonHashKey]
)
GO

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.17' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_17_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.18' cannot be rendered: only simple column equality predicates can be rendered; found 'profile.VersionRank' = 'ScalarExpression:879'. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_18_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.19' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_19_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.2' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_2_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.20' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_20_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.21' cannot be rendered: only simple column equality predicates can be rendered; found 'lineDetail.VersionRank' = 'ScalarExpression:1119'. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_21_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.22' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_22_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.23' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_23_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.24' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_24_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.25' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_25_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.26' resolves to 6 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_26_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.27' resolves to 7 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_27_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.28' resolves to 8 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_28_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.29' resolves to 9 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_29_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.3' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_3_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.30' resolves to 10 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_30_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.31' resolves to 11 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_31_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.32' resolves to 12 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_32_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.33' resolves to 13 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_33_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.34' resolves to 14 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_34_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.35' resolves to 15 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_35_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.36' resolves to 16 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_36_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.37' resolves to 17 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_37_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.38' resolves to 18 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_38_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.39' resolves to 19 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_39_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.4' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.40' resolves to 20 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_40_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.41' resolves to 21 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_41_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.42' cannot be rendered: only simple column equality predicates can be rendered; found 'detail.VersionRank' = 'ScalarExpression:955'. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_42_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.43' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_43_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.44' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_44_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.45' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_45_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.46' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_46_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.47' resolves to 6 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_47_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.48' resolves to 7 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_48_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.49' resolves to 8 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_49_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.5' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_5_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.50' resolves to 9 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_50_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.51' resolves to 10 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_51_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.52' resolves to 11 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_52_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.53' resolves to 12 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_53_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.54' resolves to 13 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_54_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.55' resolves to 14 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_55_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.56' resolves to 15 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_56_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.57' resolves to 15 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_57_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.58' resolves to 15 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_58_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.59' resolves to 16 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_59_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.6' resolves to 6 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_6_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.60' resolves to 16 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_60_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.61' resolves to 16 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_61_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.62' resolves to 17 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_62_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.63' cannot be rendered: only simple column equality predicates can be rendered; found 'quotaDetail.VersionRank' = 'ScalarExpression:1238'. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_63_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.64' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_64_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.65' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_65_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.66' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_66_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.67' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_67_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.7' cannot be rendered: only simple column equality predicates can be rendered; found 'addressProfile.VersionRank' = 'ScalarExpression:678'. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_7_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.8' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_8_JoinOrphan]
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

/* MetaDataQuality: Missing referenced rows */
/* Warning: Join relationship 'JoinPattern.9' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_JoinOrphan_JoinPattern_9_JoinOrphan]
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

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern' cannot be rendered: only simple column equality predicates can be rendered; found 'customerProfile.VersionRank' = 'ScalarExpression:777'. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.1' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_1_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.10' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_10_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.11' cannot be rendered: only simple column equality predicates can be rendered; found 'profile.VersionRank' = 'ScalarExpression:600'. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_11_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.12' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_12_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.13' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_13_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.14' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_14_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.15' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_15_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
CREATE OR ALTER VIEW [dq].[v_Unexpected_NULLs_from_outer_joins_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_Sale]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS [dq_right]
    WHERE [dq_left].[HashKey] = [dq_right].[SalesPersonHashKey]
)
GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.17' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_17_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.18' cannot be rendered: only simple column equality predicates can be rendered; found 'profile.VersionRank' = 'ScalarExpression:879'. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_18_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.19' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_19_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.2' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_2_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.20' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_20_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.3' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_3_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.31' resolves to 11 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_31_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.32' resolves to 12 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_32_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.33' resolves to 13 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_33_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.34' resolves to 14 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_34_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.35' resolves to 15 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_35_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.36' resolves to 16 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_36_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.4' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_4_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.46' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_46_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.47' resolves to 6 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_47_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.48' resolves to 7 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_48_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.49' resolves to 8 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_49_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.5' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_5_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.50' resolves to 9 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_50_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.51' resolves to 10 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_51_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.6' resolves to 6 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_6_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.60' resolves to 16 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_60_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.7' cannot be rendered: only simple column equality predicates can be rendered; found 'addressProfile.VersionRank' = 'ScalarExpression:678'. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_7_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.8' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_8_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Unexpected NULLs from outer joins */
/* Warning: Join relationship 'JoinPattern.9' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OuterJoinNullExpansion_JoinPattern_9_OuterJoinNullExpansion]
AS
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(NULL AS nvarchar(512)) AS [Relationship],
    CAST(NULL AS nvarchar(max)) AS [TransformViews],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(N'No renderable join relationship was found for this generated view.' AS nvarchar(max)) AS [KeyValues],
    CAST(0 AS bigint) AS [SuspectCount]
WHERE 1 = 0

GO

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.1' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_1_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.10' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_10_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.12' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_12_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.14' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_14_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
CREATE OR ALTER VIEW [dq].[v_Duplicate_output_rows_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPerso]
AS
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_left].[HashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(COUNT_BIG(*) AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS [dq_left]
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonPerson] AS [dq_right]
    ON [dq_left].[HashKey] = [dq_right].[SalesPersonHashKey]
GROUP BY [dq_left].[HashKey]
HAVING COUNT_BIG(*) > 1
GO

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.19' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_19_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.2' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_2_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.21' cannot be rendered: only simple column equality predicates can be rendered; found 'lineDetail.VersionRank' = 'ScalarExpression:1119'. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_21_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.22' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_22_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.23' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_23_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.24' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_24_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.25' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_25_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.26' resolves to 6 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_26_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.28' resolves to 8 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_28_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.29' resolves to 9 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_29_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.3' resolves to 4 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_3_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.31' resolves to 11 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_31_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.32' resolves to 12 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_32_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.34' resolves to 14 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_34_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.35' resolves to 15 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_35_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.37' resolves to 17 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_37_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.38' resolves to 18 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_38_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.42' cannot be rendered: only simple column equality predicates can be rendered; found 'detail.VersionRank' = 'ScalarExpression:955'. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_42_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.43' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_43_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.44' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_44_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.46' resolves to 5 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_46_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.47' resolves to 6 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_47_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.49' resolves to 8 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_49_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.50' resolves to 9 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_50_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.52' resolves to 11 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_52_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.53' resolves to 12 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_53_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.55' resolves to 14 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_55_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.56' resolves to 15 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_56_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.63' cannot be rendered: only simple column equality predicates can be rendered; found 'quotaDetail.VersionRank' = 'ScalarExpression:1238'. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_63_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.64' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_64_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.65' resolves to 3 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_65_OutputDuplicateRisk]
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

/* MetaDataQuality: Duplicate output rows */
/* Warning: Join relationship 'JoinPattern.8' resolves to 2 left table(s) and 1 right table(s); exactly one of each is required for SQL generation. */
CREATE OR ALTER VIEW [dq].[v_OutputDuplicateRisk_JoinPattern_8_OutputDuplicateRisk]
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

/* MetaDataQuality: Review dashboard */
CREATE OR ALTER VIEW [dq].[v_DataQualityReview]
AS
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.1.144' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_1_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_1_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_1_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_1_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.10.122' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_10_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_10_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_10_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_10_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.12.158' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_12_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_12_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_12_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_12_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.14.152' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_14_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_14_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_14_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_14_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.16.166' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_Row_multiplication_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPerson' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_Row_multiplication_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPerson] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_Row_multiplication_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPerson] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'SELECT TOP (100) * FROM [Dim_Salesperson];' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'SELECT TOP (100) * FROM [Dim_Salesperson];' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_Row_multiplication_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPerson]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.19.172' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_19_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_19_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_19_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_19_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.2.140' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_2_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_2_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_2_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_2_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.21.63' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_21_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_21_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_21_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_21_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.22.60' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_22_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_22_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_22_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_22_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.23.57' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_23_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_23_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_23_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_23_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.24.45' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_24_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_24_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_24_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_24_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.25.42' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_25_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_25_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_25_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_25_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.26.39' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_26_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_26_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_26_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_26_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.28.33' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_28_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_28_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_28_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_28_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.29.30' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_29_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_29_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_29_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_29_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.3.136' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_3_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_3_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_3_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_3_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.31.94' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_31_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_31_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_31_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_31_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.32.90' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_32_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_32_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_32_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_32_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.34.84' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_34_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_34_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_34_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_34_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.35.80' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_35_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_35_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_35_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_35_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.37.19' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_37_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_37_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_37_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_37_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.38.16' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_38_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_38_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_38_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_38_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.42.54' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_42_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_42_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_42_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_42_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.43.51' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_43_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_43_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_43_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_43_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.44.48' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_44_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_44_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_44_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_44_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.46.114' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_46_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_46_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_46_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_46_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.47.110' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_47_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_47_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_47_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_47_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.49.104' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_49_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_49_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_49_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_49_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.50.100' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_50_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_50_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_50_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_50_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.52.26' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_52_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_52_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_52_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_52_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.53.23' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_53_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_53_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_53_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_53_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.55.12' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_55_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_55_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_55_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_55_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.56.9' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_56_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_56_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_56_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_56_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.63.73' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_63_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_63_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_63_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_63_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.64.70' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_64_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_64_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_64_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_64_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.65.67' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_65_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_65_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_65_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_65_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Row multiplication' AS nvarchar(128)) AS [DQView],
    CAST(N'Row multiplication' AS nvarchar(128)) AS [Issue],
    CAST(N'Join fanout risk' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Join cardinality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinMultiplicityExplosion.JoinPattern.8.128' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinMultiplicityExplosion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found without an obvious detail-grain projection; fanout checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinMultiplicityExplosion_JoinPattern_8_JoinMultiplicityExplosion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_8_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_8_JoinMultiplicityExplosion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinMultiplicityExplosion_JoinPattern_8_JoinMultiplicityExplosion]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.147' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.1.143' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_1_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_1_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_1_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_1_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.10.121' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_10_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_10_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_10_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_10_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.11.161' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_11_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_11_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_11_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_11_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.12.157' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_12_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_12_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_12_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_12_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.13.155' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_13_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_13_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_13_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_13_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.14.151' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_14_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_14_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_14_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_14_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.15.149' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_15_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_15_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_15_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_15_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.16.165' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_Missing_referenced_rows_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPer' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_Missing_referenced_rows_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPer] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_Missing_referenced_rows_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPer] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'SELECT TOP (100) * FROM [Dim_Salesperson];' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'SELECT TOP (100) * FROM [Dim_Salesperson];' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_Missing_referenced_rows_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPer]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.17.163' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_17_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_17_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_17_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_17_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.18.175' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_18_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_18_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_18_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_18_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.19.171' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_19_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_19_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_19_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_19_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.2.139' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_2_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_2_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_2_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_2_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.20.169' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_20_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_20_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_20_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_20_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.21.62' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_21_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_21_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_21_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_21_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.22.59' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_22_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_22_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_22_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_22_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.23.56' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_23_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_23_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_23_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_23_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.24.44' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_24_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_24_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_24_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_24_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.25.41' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_25_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_25_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_25_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_25_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.26.38' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_26_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_26_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_26_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_26_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.27.37' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_27_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_27_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_27_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_27_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.28.32' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_28_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_28_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_28_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_28_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.29.29' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_29_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_29_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_29_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_29_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.3.135' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_3_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_3_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_3_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_3_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.30.28' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_30_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_30_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_30_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_30_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.31.93' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_31_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_31_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_31_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_31_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.32.89' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_32_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_32_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_32_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_32_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.33.87' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_33_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_33_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_33_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_33_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.34.83' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_34_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_34_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_34_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_34_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.35.79' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_35_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_35_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_35_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_35_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.36.77' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_36_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_36_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_36_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_36_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.37.18' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_37_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_37_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_37_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_37_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.38.15' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_38_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_38_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_38_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_38_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.39.14' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_39_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_39_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_39_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_39_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.4.133' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_4_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_4_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_4_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_4_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.40.5' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_40_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_40_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_40_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_40_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.41.1' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_41_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_41_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_41_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_41_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.42.53' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_42_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_42_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_42_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_42_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.43.50' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_43_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_43_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_43_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_43_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.44.47' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_44_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_44_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_44_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_44_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.45.36' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_45_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_45_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_45_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_45_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.46.113' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_46_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_46_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_46_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_46_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.47.109' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_47_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_47_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_47_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_47_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.48.107' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_48_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_48_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_48_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_48_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.49.103' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_49_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_49_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_49_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_49_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.5.119' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_5_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_5_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_5_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_5_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.50.99' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_50_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_50_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_50_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_50_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.51.97' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_51_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_51_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_51_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_51_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.52.25' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_52_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_52_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_52_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_52_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.53.22' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_53_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_53_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_53_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_53_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.54.21' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_54_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_54_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_54_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_54_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.55.11' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_55_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_55_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_55_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_55_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.56.8' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_56_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_56_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_56_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_56_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.57.7' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_57_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_57_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_57_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_57_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.58.6' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_58_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_58_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_58_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_58_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.59.3' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_59_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_59_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_59_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_59_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.6.117' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_6_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_6_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_6_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_6_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.60.75' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_60_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_60_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_60_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_60_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.61.4' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_61_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_61_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_61_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_61_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.62.2' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_62_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_62_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_62_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_62_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.63.72' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_63_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_63_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_63_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_63_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.64.69' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_64_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_64_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_64_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_64_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.65.66' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_65_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_65_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_65_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_65_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.66.65' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_66_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_66_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_66_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_66_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.67.35' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_67_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_67_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_67_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_67_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.7.131' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_7_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_7_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_7_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_7_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.8.127' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_8_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_8_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_8_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_8_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Referential integrity' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'JoinOrphan.JoinPattern.9.125' AS nvarchar(256)) AS [CandidateId],
    CAST(N'JoinOrphan' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Qualified joins with equality predicates were found; orphan anti-join checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_JoinOrphan_JoinPattern_9_JoinOrphan' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_9_JoinOrphan] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_JoinOrphan_JoinPattern_9_JoinOrphan] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_JoinOrphan_JoinPattern_9_JoinOrphan]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.148' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.1.145' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_1_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_1_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_1_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_1_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.10.123' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_10_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_10_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_10_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_10_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.11.162' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_11_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_11_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_11_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_11_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.12.159' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_12_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_12_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_12_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_12_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.13.156' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_13_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_13_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_13_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_13_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.14.153' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_14_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_14_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_14_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_14_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.15.150' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_15_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_15_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_15_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_15_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.16.167' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_Unexpected_NULLs_from_outer_joins_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_Sale' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_Unexpected_NULLs_from_outer_joins_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_Sale] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_Unexpected_NULLs_from_outer_joins_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_Sale] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'SELECT TOP (100) * FROM [Dim_Salesperson];' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'SELECT TOP (100) * FROM [Dim_Salesperson];' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_Unexpected_NULLs_from_outer_joins_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_Sale]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.17.164' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_17_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_17_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_17_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_17_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.18.176' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_18_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_18_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_18_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_18_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.19.173' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_19_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_19_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_19_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_19_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.2.141' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_2_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_2_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_2_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_2_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.20.170' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_20_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_20_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_20_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_20_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.3.137' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_3_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_3_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_3_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_3_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.31.95' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_31_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_31_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_31_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_31_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.32.91' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_32_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_32_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_32_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_32_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.33.88' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_33_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_33_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_33_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_33_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.34.85' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_34_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_34_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_34_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_34_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.35.81' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_35_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_35_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_35_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_35_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.36.78' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_36_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_36_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_36_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_36_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.4.134' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_4_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_4_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_4_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_4_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.46.115' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_46_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_46_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_46_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_46_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.47.111' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_47_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_47_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_47_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_47_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.48.108' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_48_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_48_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_48_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_48_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.49.105' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_49_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_49_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_49_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_49_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.5.120' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_5_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_5_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_5_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_5_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.50.101' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_50_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_50_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_50_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_50_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.51.98' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_51_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_51_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_51_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_51_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.6.118' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_6_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_6_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_6_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_6_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.60.76' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_60_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_60_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_60_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_60_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.7.132' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_7_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_7_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_7_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_7_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.8.129' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_8_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_8_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_8_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_8_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [DQView],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [Issue],
    CAST(N'Unexpected NULLs from outer joins' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Optionality' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OuterJoinNullExpansion.JoinPattern.9.126' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OuterJoinNullExpansion' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Outer join usage was found; null-expansion checks are likely relevant.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OuterJoinNullExpansion_JoinPattern_9_OuterJoinNullExpansion' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_9_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_9_OuterJoinNullExpansion] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OuterJoinNullExpansion_JoinPattern_9_OuterJoinNullExpansion]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.1.146' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_1_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_1_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_1_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_1_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.10.124' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_10_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_10_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_10_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_10_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.12.160' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_12_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_12_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_12_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_12_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.14.154' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_14_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_14_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_14_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_14_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.16.168' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [Relationship],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_SalesPerson -> AdventureWorksBusinessVault.dbo.BL_SalesPersonPerson' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'Dim_Salesperson' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_Duplicate_output_rows_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPerso' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_Duplicate_output_rows_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPerso] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_Duplicate_output_rows_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPerso] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'SELECT TOP (100) * FROM [Dim_Salesperson];' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'SELECT TOP (100) * FROM [Dim_Salesperson];' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_Duplicate_output_rows_AdventureWorksBusinessVault_dbo_BH_SalesPerson_AdventureWorksBusinessVault_dbo_BL_SalesPersonPerso]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.19.174' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_19_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_19_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_19_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_19_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.2.142' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_2_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_2_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_2_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_2_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.21.64' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_21_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_21_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_21_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_21_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.22.61' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_22_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_22_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_22_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_22_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.23.58' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_23_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_23_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_23_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_23_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.24.46' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_24_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_24_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_24_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_24_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.25.43' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_25_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_25_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_25_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_25_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.26.40' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_26_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_26_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_26_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_26_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.28.34' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_28_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_28_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_28_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_28_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.29.31' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_29_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_29_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_29_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_29_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.3.138' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_3_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_3_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_3_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_3_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.31.96' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_31_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_31_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_31_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_31_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.32.92' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_32_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_32_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_32_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_32_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.34.86' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_34_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_34_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_34_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_34_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.35.82' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_35_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_35_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_35_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_35_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.37.20' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_37_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_37_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_37_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_37_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.38.17' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_38_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_38_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_38_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_38_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.42.55' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_42_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_42_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_42_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_42_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.43.52' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_43_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_43_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_43_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_43_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.44.49' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_44_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_44_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_44_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_44_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.46.116' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_46_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_46_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_46_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_46_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.47.112' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_47_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_47_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_47_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_47_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.49.106' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_49_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_49_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_49_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_49_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.50.102' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_50_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_50_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_50_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_50_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.52.27' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_52_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_52_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_52_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_52_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.53.24' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_53_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_53_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_53_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_53_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.55.13' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_55_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_55_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_55_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_55_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.56.10' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_56_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_56_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_56_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_56_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.63.74' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_63_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_63_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_63_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_63_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.64.71' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_64_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_64_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_64_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_64_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.65.68' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_65_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_65_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_65_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_65_OutputDuplicateRisk]
UNION ALL
SELECT
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [Issue],
    CAST(N'Duplicate output rows' AS nvarchar(128)) AS [FindingTitle],
    CAST(N'Output uniqueness' AS nvarchar(128)) AS [FindingCategory],
    CAST(N'RuntimeCheck' AS nvarchar(64)) AS [OutputMode],
    CAST(N'OutputDuplicateRisk.JoinPattern.8.130' AS nvarchar(256)) AS [CandidateId],
    CAST(N'OutputDuplicateRisk' AS nvarchar(128)) AS [CandidateKind],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [Relationship],
    CAST(N'(unresolved relationship)' AS nvarchar(512)) AS [RelationshipLabel],
    CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
    CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
    CAST(NULL AS nvarchar(512)) AS [CheckedObject],
    CAST(NULL AS nvarchar(512)) AS [SuspectSide],
    CAST(NULL AS nvarchar(512)) AS [SuspectObject],
    CAST(NULL AS nvarchar(512)) AS [LookupObject],
    CAST(NULL AS nvarchar(512)) AS [RelatedObject],
    CAST(N'' AS nvarchar(512)) AS [CorpusRelationship],
    CAST(N'' AS nvarchar(max)) AS [CorpusRelationshipPattern],
    CAST(N'' AS nvarchar(max)) AS [DominantPattern],
    CAST(N'' AS nvarchar(max)) AS [OutlierPattern],
    CAST(N'(unknown transform view)' AS nvarchar(max)) AS [TransformViews],
    CAST(COUNT_BIG(*) AS bigint) AS [RowsReturned],
    CAST(COUNT_BIG(*) AS bigint) AS [ResultRowCount],
    CAST(COUNT_BIG(*) AS bigint) AS [FindingGroupCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [TotalSuspectCount],
    CAST(COALESCE(SUM([SuspectCount]), 0) AS bigint) AS [SuspectRowCount],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [Explanation],
    CAST(N'Join usage without DISTINCT/GROUP BY indicates possible duplicate amplification.' AS nvarchar(max)) AS [FindingExplanation],
    CAST(N'Outlier evidence from unknown transforms and unknown occurrences (dominant ratio n/a, outlier ratio n/a). Confidence Low: No additional calibration details.' AS nvarchar(max)) AS [EvidenceSummary],
    CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
    CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
    CAST(NULL AS bigint) AS [EvidenceTransformCount],
    CAST(NULL AS bigint) AS [OutlierTransformCount],
    CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
    CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
    CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
    CAST(N'Low' AS nvarchar(16)) AS [EvidenceQuality],
    CAST(N'Low' AS nvarchar(16)) AS [ConfidenceBand],
    CAST(N'No corpus evidence row was found for this candidate.' AS nvarchar(max)) AS [ConfidenceReason],
    CAST(N'' AS nvarchar(max)) AS [EvidenceDiversitySummary],
    CAST(N'Confidence Low: no persisted corpus evidence metrics.' AS nvarchar(max)) AS [ConfidenceSummary],
    CAST(NULL AS bigint) AS [DistinctTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
    CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
    CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
    CAST(NULL AS bigint) AS [EffectiveTransformCount],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'Review returned rows' ELSE N'No rows returned' END AS nvarchar(128)) AS [RecommendedAction],
    CAST(N'Runtime counts' AS nvarchar(64)) AS [RuntimeCountStatus],
    CAST(N'dq.v_OutputDuplicateRisk_JoinPattern_8_OutputDuplicateRisk' AS nvarchar(512)) AS [GeneratedView],
    CAST(CASE WHEN COUNT_BIG(*) > 0 THEN N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_8_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' ELSE NULL END AS nvarchar(max)) AS [ReviewQuery],
    CAST(N'SELECT * FROM [dq].[v_OutputDuplicateRisk_JoinPattern_8_OutputDuplicateRisk] ORDER BY [Relationship], [KeyValues];' AS nvarchar(max)) AS [DetailQuery],
    CAST(N'' AS nvarchar(max)) AS [TransformViewQuery],
    CAST(N'' AS nvarchar(max)) AS [SupportingTransformQuery]
FROM [dq].[v_OutputDuplicateRisk_JoinPattern_8_OutputDuplicateRisk]
GO

/* MetaDataQuality operational store */
IF DB_ID(N'MetaDQ') IS NULL
BEGIN
    CREATE DATABASE [MetaDQ];
END
GO

EXEC [MetaDQ].sys.sp_executesql N'
IF OBJECT_ID(N''[dbo].[RunLog]'', N''U'') IS NULL
BEGIN
    CREATE TABLE [dbo].[RunLog]
    (
        [RunId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_dbo_RunLog] PRIMARY KEY,
        [StartedAtUtc] datetime2(3) NOT NULL CONSTRAINT [DF_dbo_RunLog_StartedAtUtc] DEFAULT (SYSUTCDATETIME()),
        [CompletedAtUtc] datetime2(3) NULL,
        [SourceDatabaseName] sysname NOT NULL,
        [Status] nvarchar(32) NOT NULL,
        [ErrorMessage] nvarchar(4000) NULL
    );
END;

IF COL_LENGTH(N''dbo.RunLog'', N''SourceDatabaseName'') IS NULL
BEGIN
    ALTER TABLE [dbo].[RunLog] ADD [SourceDatabaseName] sysname NULL;
END;
IF COL_LENGTH(N''dbo.RunLog'', N''Status'') IS NULL
BEGIN
    ALTER TABLE [dbo].[RunLog] ADD [Status] nvarchar(32) NULL;
END;
IF COL_LENGTH(N''dbo.RunLog'', N''ErrorMessage'') IS NULL
BEGIN
    ALTER TABLE [dbo].[RunLog] ADD [ErrorMessage] nvarchar(4000) NULL;
END;

IF OBJECT_ID(N''[dbo].[FindingLog]'', N''U'') IS NULL
BEGIN
    CREATE TABLE [dbo].[FindingLog]
    (
        [FindingId] bigint IDENTITY(1,1) NOT NULL CONSTRAINT [PK_dbo_FindingLog] PRIMARY KEY,
        [RunId] bigint NOT NULL,
        [DQView] nvarchar(128) NOT NULL,
        [Issue] nvarchar(128) NOT NULL,
        [FindingTitle] nvarchar(128) NULL,
        [FindingCategory] nvarchar(128) NULL,
        [OutputMode] nvarchar(64) NULL,
        [CandidateId] nvarchar(256) NULL,
        [CandidateKind] nvarchar(128) NULL,
        [Relationship] nvarchar(512) NOT NULL,
        [RelationshipLabel] nvarchar(512) NULL,
        [ReferencingObject] nvarchar(512) NULL,
        [ReferencedObject] nvarchar(512) NULL,
        [CheckedObject] nvarchar(512) NULL,
        [SuspectSide] nvarchar(512) NULL,
        [SuspectObject] nvarchar(512) NULL,
        [LookupObject] nvarchar(512) NULL,
        [RelatedObject] nvarchar(512) NULL,
        [CorpusRelationship] nvarchar(512) NULL,
        [CorpusRelationshipPattern] nvarchar(max) NULL,
        [DominantPattern] nvarchar(max) NULL,
        [OutlierPattern] nvarchar(max) NULL,
        [TransformViews] nvarchar(max) NOT NULL,
        [GeneratedView] nvarchar(512) NOT NULL,
        [RowsReturned] bigint NULL,
        [ResultRowCount] bigint NULL,
        [FindingGroupCount] bigint NULL,
        [TotalSuspectCount] bigint NULL,
        [SuspectRowCount] bigint NULL,
        [Explanation] nvarchar(max) NULL,
        [FindingExplanation] nvarchar(max) NULL,
        [EvidenceSummary] nvarchar(max) NULL,
        [EvidenceOccurrenceCount] bigint NULL,
        [OutlierOccurrenceCount] bigint NULL,
        [EvidenceTransformCount] bigint NULL,
        [OutlierTransformCount] bigint NULL,
        [EvidenceConsensusRatio] decimal(18,6) NULL,
        [DominantConsensusRatio] decimal(18,6) NULL,
        [EvidenceOutlierRatio] decimal(18,6) NULL,
        [OutlierRatio] decimal(18,6) NULL,
        [EvidenceQuality] nvarchar(16) NULL,
        [ConfidenceBand] nvarchar(16) NULL,
        [ConfidenceReason] nvarchar(max) NULL,
        [EvidenceDiversitySummary] nvarchar(max) NULL,
        [ConfidenceSummary] nvarchar(max) NULL,
        [DistinctTransformCount] bigint NULL,
        [DistinctSourceTransformCount] bigint NULL,
        [DistinctSourceObjectCount] bigint NULL,
        [DistinctRelationshipPatternCount] bigint NULL,
        [EffectiveTransformCount] bigint NULL,
        [RecommendedAction] nvarchar(128) NOT NULL,
        [RuntimeCountStatus] nvarchar(64) NULL,
        [ReviewQuery] nvarchar(max) NULL,
        [DetailQuery] nvarchar(max) NULL,
        [TransformViewQuery] nvarchar(max) NULL,
        [SupportingTransformQuery] nvarchar(max) NULL,
        [CapturedAtUtc] datetime2(3) NOT NULL CONSTRAINT [DF_dbo_FindingLog_CapturedAtUtc] DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT [FK_dbo_FindingLog_RunId] FOREIGN KEY ([RunId]) REFERENCES [dbo].[RunLog]([RunId])
    );
END;

IF COL_LENGTH(N''dbo.FindingLog'', N''Issue'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [Issue] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingTitle'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingTitle] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingCategory'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingCategory] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ReviewQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ReviewQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DetailQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DetailQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''TransformViewQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [TransformViewQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SupportingTransformQuery'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SupportingTransformQuery] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutputMode'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutputMode] nvarchar(64) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CandidateId'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CandidateId] nvarchar(256) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CandidateKind'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CandidateKind] nvarchar(128) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RelationshipLabel'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [RelationshipLabel] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ReferencingObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ReferencingObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ReferencedObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ReferencedObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CheckedObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CheckedObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectSide'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SuspectSide] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SuspectObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''LookupObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [LookupObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RelatedObject'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [RelatedObject] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CorpusRelationship'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CorpusRelationship] nvarchar(512) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''CorpusRelationshipPattern'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [CorpusRelationshipPattern] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DominantPattern'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DominantPattern] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierPattern'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierPattern] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ResultRowCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ResultRowCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingGroupCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingGroupCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''SuspectRowCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [SuspectRowCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''Explanation'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [Explanation] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''FindingExplanation'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [FindingExplanation] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceSummary'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceSummary] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceOccurrenceCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceOccurrenceCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierOccurrenceCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierOccurrenceCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceConsensusRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceConsensusRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DominantConsensusRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DominantConsensusRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceOutlierRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceOutlierRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''OutlierRatio'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [OutlierRatio] decimal(18,6) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceQuality'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceQuality] nvarchar(16) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceBand'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceBand] nvarchar(16) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceReason'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceReason] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EvidenceDiversitySummary'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EvidenceDiversitySummary] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''ConfidenceSummary'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [ConfidenceSummary] nvarchar(max) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctSourceTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctSourceTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctSourceObjectCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctSourceObjectCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''DistinctRelationshipPatternCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [DistinctRelationshipPatternCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''EffectiveTransformCount'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [EffectiveTransformCount] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RuntimeCountStatus'') IS NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ADD [RuntimeCountStatus] nvarchar(64) NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''RowsReturned'') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ALTER COLUMN [RowsReturned] bigint NULL;
END;
IF COL_LENGTH(N''dbo.FindingLog'', N''TotalSuspectCount'') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[FindingLog] ALTER COLUMN [TotalSuspectCount] bigint NULL;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N''IX_dbo_FindingLog_RunId''
      AND [object_id] = OBJECT_ID(N''[dbo].[FindingLog]'')
)
BEGIN
    CREATE INDEX [IX_dbo_FindingLog_RunId] ON [dbo].[FindingLog]([RunId]);
END;
';
GO

EXEC [MetaDQ].sys.sp_executesql N'
CREATE OR ALTER PROCEDURE [dbo].[Run]
    @SourceDatabaseName sysname,
    @RunId bigint OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NULLIF(LTRIM(RTRIM(@SourceDatabaseName)), N'''') IS NULL
    BEGIN
        THROW 51000, N''@SourceDatabaseName is required.'', 1;
    END;

    IF DB_ID(@SourceDatabaseName) IS NULL
    BEGIN
        THROW 51000, N''Source database was not found.'', 1;
    END;

    DECLARE @runIdLocal bigint;
    INSERT INTO [dbo].[RunLog] ([SourceDatabaseName], [Status])
    VALUES (@SourceDatabaseName, N''Running'');
    SET @runIdLocal = SCOPE_IDENTITY();
    SET @RunId = @runIdLocal;

    BEGIN TRY
        DECLARE @sql nvarchar(max) =
            N''INSERT INTO [dbo].[FindingLog] ([RunId], [DQView], [Issue], [FindingTitle], [FindingCategory], [OutputMode], [CandidateId], [CandidateKind], [Relationship], [RelationshipLabel], [ReferencingObject], [ReferencedObject], [CheckedObject], [SuspectSide], [SuspectObject], [LookupObject], [RelatedObject], [CorpusRelationship], [CorpusRelationshipPattern], [DominantPattern], [OutlierPattern], [TransformViews], [GeneratedView], [RowsReturned], [ResultRowCount], [FindingGroupCount], [TotalSuspectCount], [SuspectRowCount], [Explanation], [FindingExplanation], [EvidenceSummary], [EvidenceOccurrenceCount], [OutlierOccurrenceCount], [EvidenceTransformCount], [OutlierTransformCount], [EvidenceConsensusRatio], [DominantConsensusRatio], [EvidenceOutlierRatio], [OutlierRatio], [EvidenceQuality], [ConfidenceBand], [ConfidenceReason], [EvidenceDiversitySummary], [ConfidenceSummary], [DistinctTransformCount], [DistinctSourceTransformCount], [DistinctSourceObjectCount], [DistinctRelationshipPatternCount], [EffectiveTransformCount], [RecommendedAction], [RuntimeCountStatus], [ReviewQuery], [DetailQuery], [TransformViewQuery], [SupportingTransformQuery]) '' +
            N''SELECT @RunId, [DQView], [Issue], [FindingTitle], [FindingCategory], [OutputMode], [CandidateId], [CandidateKind], [Relationship], [RelationshipLabel], [ReferencingObject], [ReferencedObject], [CheckedObject], [SuspectSide], [SuspectObject], [LookupObject], [RelatedObject], [CorpusRelationship], [CorpusRelationshipPattern], [DominantPattern], [OutlierPattern], [TransformViews], [GeneratedView], [RowsReturned], [ResultRowCount], [FindingGroupCount], [TotalSuspectCount], [SuspectRowCount], [Explanation], [FindingExplanation], [EvidenceSummary], [EvidenceOccurrenceCount], [OutlierOccurrenceCount], [EvidenceTransformCount], [OutlierTransformCount], [EvidenceConsensusRatio], [DominantConsensusRatio], [EvidenceOutlierRatio], [OutlierRatio], [EvidenceQuality], [ConfidenceBand], [ConfidenceReason], [EvidenceDiversitySummary], [ConfidenceSummary], [DistinctTransformCount], [DistinctSourceTransformCount], [DistinctSourceObjectCount], [DistinctRelationshipPatternCount], [EffectiveTransformCount], [RecommendedAction], [RuntimeCountStatus], [ReviewQuery], [DetailQuery], [TransformViewQuery], [SupportingTransformQuery] '' +
            N''FROM '' + QUOTENAME(@SourceDatabaseName) + N''.[dq].[v_DataQualityReview];'';

        EXEC sys.sp_executesql
            @sql,
            N''@RunId bigint'',
            @RunId = @runIdLocal;

        UPDATE [dbo].[RunLog]
        SET [CompletedAtUtc] = SYSUTCDATETIME(),
            [Status] = N''Completed''
        WHERE [RunId] = @runIdLocal;
    END TRY
    BEGIN CATCH
        UPDATE [dbo].[RunLog]
        SET [CompletedAtUtc] = SYSUTCDATETIME(),
            [Status] = N''Failed'',
            [ErrorMessage] = ERROR_MESSAGE()
        WHERE [RunId] = @runIdLocal;
        THROW;
    END CATCH;

    SELECT
        r.[RunId],
        r.[SourceDatabaseName],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [RowsReturned],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [ResultRowCount],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [FindingGroupCount],
        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [TotalSuspectCount],
        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [SuspectRowCount],
        COUNT(f.[FindingId]) AS [ChecksExecuted],
        COUNT(f.[FindingId]) AS [FindingsExecuted],
        SUM(CASE WHEN f.[RowsReturned] IS NULL THEN 0 ELSE f.[RowsReturned] END) AS [RuntimeFindingGroupCount],
        SUM(CASE WHEN f.[TotalSuspectCount] IS NULL THEN 0 ELSE f.[TotalSuspectCount] END) AS [RuntimeSuspectRowCount]
    FROM [dbo].[RunLog] AS r
    LEFT JOIN [dbo].[FindingLog] AS f
      ON f.[RunId] = r.[RunId]
    WHERE r.[RunId] = @runIdLocal
    GROUP BY r.[RunId], r.[SourceDatabaseName];

    SELECT
        [DQView],
        [Issue],
        [FindingTitle],
        [FindingCategory],
        [OutputMode],
        [CandidateId],
        [CandidateKind],
        [Relationship],
        [RelationshipLabel],
        [ReferencingObject],
        [ReferencedObject],
        [CheckedObject],
        [SuspectSide],
        [SuspectObject],
        [LookupObject],
        [RelatedObject],
        [CorpusRelationship],
        [CorpusRelationshipPattern],
        [DominantPattern],
        [OutlierPattern],
        [RowsReturned],
        [ResultRowCount],
        [FindingGroupCount],
        [TotalSuspectCount],
        [SuspectRowCount],
        [Explanation],
        [FindingExplanation],
        [EvidenceSummary],
        [EvidenceOccurrenceCount],
        [OutlierOccurrenceCount],
        [EvidenceTransformCount],
        [OutlierTransformCount],
        [EvidenceConsensusRatio],
        [DominantConsensusRatio],
        [EvidenceOutlierRatio],
        [OutlierRatio],
        [EvidenceQuality],
        [ConfidenceBand],
        [ConfidenceReason],
        [EvidenceDiversitySummary],
        [ConfidenceSummary],
        [DistinctTransformCount],
        [DistinctSourceTransformCount],
        [DistinctSourceObjectCount],
        [DistinctRelationshipPatternCount],
        [EffectiveTransformCount],
        [RecommendedAction],
        [RuntimeCountStatus],
        [GeneratedView],
        [ReviewQuery],
        [DetailQuery],
        [TransformViewQuery],
        [SupportingTransformQuery]
    FROM [dbo].[FindingLog]
    WHERE [RunId] = @runIdLocal
      AND ([RowsReturned] > 0 OR [RowsReturned] IS NULL)
    ORDER BY CASE WHEN [RowsReturned] IS NULL THEN 1 ELSE 0 END, [RowsReturned] DESC, [DQView] ASC;
END;
';
GO


EXEC [MetaDQ].sys.sp_executesql N'
CREATE OR ALTER PROCEDURE [dbo].[Findings]
    @RunId bigint = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @RunId IS NULL
    BEGIN
        SELECT TOP (1) @RunId = [RunId]
        FROM [dbo].[RunLog]
        ORDER BY [RunId] DESC;
    END;

    IF @RunId IS NULL
    BEGIN
        SELECT
            CAST(NULL AS bigint) AS [RunId],
            CAST(NULL AS nvarchar(128)) AS [DQView],
            CAST(NULL AS nvarchar(128)) AS [Issue],
            CAST(NULL AS nvarchar(128)) AS [FindingTitle],
            CAST(NULL AS nvarchar(128)) AS [FindingCategory],
            CAST(NULL AS nvarchar(64)) AS [OutputMode],
            CAST(NULL AS nvarchar(256)) AS [CandidateId],
            CAST(NULL AS nvarchar(128)) AS [CandidateKind],
            CAST(NULL AS nvarchar(512)) AS [Relationship],
            CAST(NULL AS nvarchar(512)) AS [RelationshipLabel],
            CAST(NULL AS nvarchar(512)) AS [ReferencingObject],
            CAST(NULL AS nvarchar(512)) AS [ReferencedObject],
            CAST(NULL AS nvarchar(512)) AS [CheckedObject],
            CAST(NULL AS nvarchar(512)) AS [SuspectSide],
            CAST(NULL AS nvarchar(512)) AS [SuspectObject],
            CAST(NULL AS nvarchar(512)) AS [LookupObject],
            CAST(NULL AS nvarchar(512)) AS [RelatedObject],
            CAST(NULL AS nvarchar(512)) AS [CorpusRelationship],
            CAST(NULL AS nvarchar(max)) AS [CorpusRelationshipPattern],
            CAST(NULL AS nvarchar(max)) AS [DominantPattern],
            CAST(NULL AS nvarchar(max)) AS [OutlierPattern],
            CAST(NULL AS bigint) AS [RowsReturned],
            CAST(NULL AS bigint) AS [ResultRowCount],
            CAST(NULL AS bigint) AS [FindingGroupCount],
            CAST(NULL AS bigint) AS [TotalSuspectCount],
            CAST(NULL AS bigint) AS [SuspectRowCount],
            CAST(NULL AS nvarchar(max)) AS [Explanation],
            CAST(NULL AS nvarchar(max)) AS [FindingExplanation],
            CAST(NULL AS nvarchar(max)) AS [EvidenceSummary],
            CAST(NULL AS bigint) AS [EvidenceOccurrenceCount],
            CAST(NULL AS bigint) AS [OutlierOccurrenceCount],
            CAST(NULL AS bigint) AS [EvidenceTransformCount],
            CAST(NULL AS bigint) AS [OutlierTransformCount],
            CAST(NULL AS decimal(18,6)) AS [EvidenceConsensusRatio],
            CAST(NULL AS decimal(18,6)) AS [DominantConsensusRatio],
            CAST(NULL AS decimal(18,6)) AS [EvidenceOutlierRatio],
            CAST(NULL AS decimal(18,6)) AS [OutlierRatio],
            CAST(NULL AS nvarchar(16)) AS [EvidenceQuality],
            CAST(NULL AS nvarchar(16)) AS [ConfidenceBand],
            CAST(NULL AS nvarchar(max)) AS [ConfidenceReason],
            CAST(NULL AS nvarchar(max)) AS [EvidenceDiversitySummary],
            CAST(NULL AS nvarchar(max)) AS [ConfidenceSummary],
            CAST(NULL AS bigint) AS [DistinctTransformCount],
            CAST(NULL AS bigint) AS [DistinctSourceTransformCount],
            CAST(NULL AS bigint) AS [DistinctSourceObjectCount],
            CAST(NULL AS bigint) AS [DistinctRelationshipPatternCount],
            CAST(NULL AS bigint) AS [EffectiveTransformCount],
            CAST(NULL AS nvarchar(128)) AS [RecommendedAction],
            CAST(NULL AS nvarchar(64)) AS [RuntimeCountStatus],
            CAST(NULL AS nvarchar(512)) AS [GeneratedView],
            CAST(NULL AS nvarchar(max)) AS [ReviewQuery],
            CAST(NULL AS nvarchar(max)) AS [DetailQuery],
            CAST(NULL AS nvarchar(max)) AS [TransformViewQuery],
            CAST(NULL AS nvarchar(max)) AS [SupportingTransformQuery]
        WHERE 1 = 0;
        RETURN;
    END;

    SELECT
        [RunId],
        [DQView],
        [Issue],
        [FindingTitle],
        [FindingCategory],
        [OutputMode],
        [CandidateId],
        [CandidateKind],
        [Relationship],
        [RelationshipLabel],
        [ReferencingObject],
        [ReferencedObject],
        [CheckedObject],
        [SuspectSide],
        [SuspectObject],
        [LookupObject],
        [RelatedObject],
        [CorpusRelationship],
        [CorpusRelationshipPattern],
        [DominantPattern],
        [OutlierPattern],
        [RowsReturned],
        [ResultRowCount],
        [FindingGroupCount],
        [TotalSuspectCount],
        [SuspectRowCount],
        [Explanation],
        [FindingExplanation],
        [EvidenceSummary],
        [EvidenceOccurrenceCount],
        [OutlierOccurrenceCount],
        [EvidenceTransformCount],
        [OutlierTransformCount],
        [EvidenceConsensusRatio],
        [DominantConsensusRatio],
        [EvidenceOutlierRatio],
        [OutlierRatio],
        [EvidenceQuality],
        [ConfidenceBand],
        [ConfidenceReason],
        [EvidenceDiversitySummary],
        [ConfidenceSummary],
        [DistinctTransformCount],
        [DistinctSourceTransformCount],
        [DistinctSourceObjectCount],
        [DistinctRelationshipPatternCount],
        [EffectiveTransformCount],
        [RecommendedAction],
        [RuntimeCountStatus],
        [GeneratedView],
        [ReviewQuery],
        [DetailQuery],
        [TransformViewQuery],
        [SupportingTransformQuery]
    FROM [dbo].[FindingLog]
    WHERE [RunId] = @RunId
      AND ([RowsReturned] > 0 OR [RowsReturned] IS NULL)
    ORDER BY CASE WHEN [RowsReturned] IS NULL THEN 1 ELSE 0 END, [RowsReturned] DESC, [DQView] ASC;
END;
';
GO

