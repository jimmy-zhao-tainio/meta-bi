INSERT INTO [AdventureWorksAnalytics].[dw].[Dim_SalesChannel] ([SalesChannelKey], [ChannelCode], [ChannelName], [AuditId], [InsertDateTime2])
SELECT
    CONVERT(bigint, 1),
    CONVERT(nvarchar(256), N'Online'),
    CONVERT(nvarchar(256), N'Online'),
    CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')),
    CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required'))
WHERE NOT EXISTS (
    SELECT 1 FROM [AdventureWorksAnalytics].[dw].[Dim_SalesChannel] AS existing WHERE existing.[ChannelCode] = N'Online'
)
