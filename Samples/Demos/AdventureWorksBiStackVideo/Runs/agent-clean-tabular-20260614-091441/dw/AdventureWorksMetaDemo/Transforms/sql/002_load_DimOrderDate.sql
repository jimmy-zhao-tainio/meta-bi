CREATE OR ALTER VIEW awbi.v_load_DimOrderDate
AS
SELECT
    od.OrderDate,
    DATEPART(year, od.OrderDate) AS OrderYear,
    DATEPART(month, od.OrderDate) AS OrderMonth,
    CONVERT(nvarchar(7), od.OrderDate, 126) AS OrderMonthName
FROM [AdventureWorksBusinessVault].[dbo].[BH_OrderDate] AS od;
