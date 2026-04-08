CREATE VIEW dbo.v_cte_column_list AS
WITH CustomerAmounts (CustomerId, TotalAmount) AS
(
    SELECT
        s.CustomerId,
        s.Amount
    FROM dbo.Sales AS s
)
SELECT
    CustomerAmounts.CustomerId,
    CustomerAmounts.TotalAmount
FROM CustomerAmounts;
