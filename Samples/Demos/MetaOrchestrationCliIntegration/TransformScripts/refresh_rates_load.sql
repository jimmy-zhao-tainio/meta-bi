CREATE VIEW dbo.v_work_exchange_rate
AS
SELECT
    CurrencyCode,
    Rate
FROM dbo.RawExchangeRate;
GO
