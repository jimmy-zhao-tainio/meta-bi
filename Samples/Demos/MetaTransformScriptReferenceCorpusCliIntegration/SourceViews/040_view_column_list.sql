CREATE VIEW dbo.v_view_column_list
(
    OutputCustomerId,
    OutputCustomerName
)
AS
SELECT
    s.CustomerId,
    s.CustomerName
FROM dbo.Source AS s;
