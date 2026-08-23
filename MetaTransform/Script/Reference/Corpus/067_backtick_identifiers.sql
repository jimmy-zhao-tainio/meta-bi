CREATE VIEW dbo.v_backtick_identifiers AS
SELECT
    count(distinct s.OrderId) AS `order count`,
    sum(s.ShippingCost) AS `total shipping cost`,
    sum(s.NetProfit) AS `total net profit`
FROM dbo.Sales AS s
WHERE s.Score >= 10
  AND s.Rank < 20
  AND s.Age <= 65
  AND s.Status <> 0;
