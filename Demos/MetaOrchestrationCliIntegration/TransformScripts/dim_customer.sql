CREATE VIEW dbo.v_dim_customer
AS
SELECT
    CustomerId,
    CustomerName
FROM dbo.StageCustomer;
GO
