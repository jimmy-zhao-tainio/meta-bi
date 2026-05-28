CREATE VIEW dbo.v_stage_order
AS
SELECT
    OrderId,
    CustomerId,
    Amount
FROM dbo.RawOrder;
GO
