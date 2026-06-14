CREATE OR ALTER VIEW awbi.v_load_DimCustomerChannel
AS
SELECT
    c.CustomerId,
    cp.CustomerAccountNumber,
    cp.CustomerType
FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS c
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_Customer_CustomerProfile] AS cp
    ON cp.HubHashKey = c.HashKey;
