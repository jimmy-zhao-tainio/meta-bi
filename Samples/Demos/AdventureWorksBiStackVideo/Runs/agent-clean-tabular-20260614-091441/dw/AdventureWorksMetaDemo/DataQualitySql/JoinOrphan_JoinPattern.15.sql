IF SCHEMA_ID(N'dq') IS NULL EXEC(N'CREATE SCHEMA [dq]');
GO

/* MetaDataQuality: Missing referenced rows */
CREATE OR ALTER VIEW [dq].[v_Missing_referenced_rows_AdventureWorksBusinessVault_dbo_BL_SalesOrderLineProduct_AdventureWorksBusinessVault_dbo_BLS_Sal]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BL_SalesOrderLineProduct -> AdventureWorksBusinessVault.dbo.BLS_SalesOrderLineProduct_SalesOrderLineMeasures' AS nvarchar(512)) AS [Relationship],
    CAST(N'awbi.v_load_FactSalesOrderLine' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BLS_SalesOrderLineProduct_SalesOrderLineMeasures' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'LinkHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[LinkHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BLS_SalesOrderLineProduct_SalesOrderLineMeasures] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[LinkHashKey]
)
GO
