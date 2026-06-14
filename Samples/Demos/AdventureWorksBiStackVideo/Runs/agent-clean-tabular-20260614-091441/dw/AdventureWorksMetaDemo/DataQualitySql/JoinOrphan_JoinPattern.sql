IF SCHEMA_ID(N'dq') IS NULL EXEC(N'CREATE SCHEMA [dq]');
GO

/* MetaDataQuality: Missing referenced rows */
CREATE OR ALTER VIEW [dq].[v_Missing_referenced_rows_AdventureWorksBusinessVault_dbo_BH_Customer_AdventureWorksBusinessVault_dbo_BHS_Customer_Custome]
AS
SELECT
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [DQView],
    CAST(N'Missing referenced rows' AS nvarchar(128)) AS [Issue],
    CAST(N'AdventureWorksBusinessVault.dbo.BH_Customer -> AdventureWorksBusinessVault.dbo.BHS_Customer_CustomerProfile' AS nvarchar(512)) AS [Relationship],
    CAST(N'awbi.v_load_DimCustomerChannel' AS nvarchar(max)) AS [TransformViews],
    CAST(N'AdventureWorksBusinessVault.dbo.BHS_Customer_CustomerProfile' AS nvarchar(512)) AS [SuspectSide],
    CAST(CONCAT(N'HubHashKey=', COALESCE(CONVERT(nvarchar(4000), [dq_right].[HubHashKey]), N'<NULL>')) AS nvarchar(max)) AS [KeyValues],
    CAST(1 AS bigint) AS [SuspectCount]
FROM [AdventureWorksBusinessVault].[dbo].[BHS_Customer_CustomerProfile] AS [dq_right]
WHERE NOT EXISTS
(
    SELECT 1
    FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS [dq_left]
    WHERE [dq_left].[HashKey] = [dq_right].[HubHashKey]
)
GO
