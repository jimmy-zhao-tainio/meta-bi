MERGE INTO [AdventureWorksAnalytics].[dw].[Dim_Date] AS target
USING (
    SELECT DISTINCT CONVERT(date, source.[DueDate]) AS [CalendarDate]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderHeader] AS source
    WHERE source.[DueDate] IS NOT NULL
) AS source
ON target.[CalendarDate] = source.[CalendarDate]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([DateKey], [CalendarDateKey], [CalendarDate], [CalendarYear], [CalendarQuarter], [MonthNumber], [MonthName], [DayOfMonth], [DayName], [AuditId], [InsertDateTime2])
    VALUES (
        CONVERT(bigint, CONVERT(char(8), source.[CalendarDate], 112)),
        CONVERT(int, CONVERT(char(8), source.[CalendarDate], 112)),
        source.[CalendarDate],
        CONVERT(int, DATEPART(year, source.[CalendarDate])),
        CONVERT(int, DATEPART(quarter, source.[CalendarDate])),
        CONVERT(int, DATEPART(month, source.[CalendarDate])),
        CONVERT(nvarchar(256), CASE DATEPART(month, source.[CalendarDate])
            WHEN 1 THEN N'January' WHEN 2 THEN N'February' WHEN 3 THEN N'March'
            WHEN 4 THEN N'April' WHEN 5 THEN N'May' WHEN 6 THEN N'June'
            WHEN 7 THEN N'July' WHEN 8 THEN N'August' WHEN 9 THEN N'September'
            WHEN 10 THEN N'October' WHEN 11 THEN N'November' WHEN 12 THEN N'December' END),
        CONVERT(int, DATEPART(day, source.[CalendarDate])),
        CONVERT(nvarchar(256), CASE DATEDIFF(day, CONVERT(date, '19000101', 112), source.[CalendarDate]) % 7
            WHEN 0 THEN N'Monday' WHEN 1 THEN N'Tuesday' WHEN 2 THEN N'Wednesday'
            WHEN 3 THEN N'Thursday' WHEN 4 THEN N'Friday' WHEN 5 THEN N'Saturday'
            WHEN 6 THEN N'Sunday' END),
        CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')),
        CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required'))
    );
