CREATE VIEW dbo.v_shared_writer_a
AS
SELECT
    CustomerId,
    CustomerName
FROM dbo.RawCustomer;
GO
