CREATE VIEW dbo.v_customer_load
AS
SELECT
    CustomerId,
    CustomerName,
    TotalAmount
FROM dbo.SourceCustomer;
GO
