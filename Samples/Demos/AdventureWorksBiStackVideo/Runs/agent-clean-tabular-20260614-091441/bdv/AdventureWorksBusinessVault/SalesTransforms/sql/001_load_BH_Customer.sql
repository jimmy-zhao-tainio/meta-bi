CREATE VIEW dbo.v_load_BH_Customer
AS
SELECT
    h.HashKey,
    h.CustomerID AS CustomerId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_Customer') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Customer] AS h;
GO
