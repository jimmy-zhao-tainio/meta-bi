CREATE VIEW dbo.v_shared_writer_b
AS
SELECT
    CustomerId,
    CustomerName
FROM dbo.RawCustomerDelta;
GO
