INSERT INTO dbo.InsertTargetCustomer
(
    CustomerId,
    CustomerName,
    TotalAmount
)
SELECT
    CustomerId,
    CustomerName,
    TotalAmount
FROM dbo.InsertSourceCustomer;
