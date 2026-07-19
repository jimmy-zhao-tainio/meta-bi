INSERT INTO [AdventureWorksAnalytics].[dw].[Dim_OrderStatus] ([OrderStatusKey], [StatusCode], [StatusName], [AuditId], [InsertDateTime2])
SELECT CONVERT(bigint, 1), CONVERT(int, 1), CONVERT(nvarchar(256), N'In process'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')),
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required'))
WHERE NOT EXISTS (SELECT 1 FROM [AdventureWorksAnalytics].[dw].[Dim_OrderStatus] AS existing WHERE existing.[StatusCode] = 1)
