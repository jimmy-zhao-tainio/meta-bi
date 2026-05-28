CREATE VIEW dbo.v_stage_customer
AS
SELECT
    CustomerId,
    CustomerName
FROM dbo.RawCustomer;
GO
