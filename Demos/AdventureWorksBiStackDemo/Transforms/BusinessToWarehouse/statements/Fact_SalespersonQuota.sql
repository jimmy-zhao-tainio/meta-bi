WITH latestQuota AS (
    SELECT source.[HubHashKey], source.[SalesQuota],
        ROW_NUMBER() OVER (PARTITION BY source.[HubHashKey] ORDER BY source.[LoadTimestamp] DESC, source.[AuditId] DESC) AS [VersionRank]
    FROM [AdventureWorksBusinessVault].[dbo].[BHS_SalesPersonQuota_SalesPersonQuotaProfile] AS source
)
MERGE INTO [AdventureWorksAnalytics].[dw].[Fact_SalespersonQuota] AS target
USING (
    SELECT quotaPeriod.[DateKey] AS [QuotaPeriodKey], salesperson.[SalespersonKey],
        CONVERT(decimal(18,4), quotaDetail.[SalesQuota]) AS [QuotaAmount]
    FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesPersonQuota] AS quota
    INNER JOIN latestQuota AS quotaDetail
        ON quotaDetail.[HubHashKey] = quota.[HashKey] AND quotaDetail.[VersionRank] = 1
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesPersonQuotaSalesPerson] AS quotaSalesperson
        ON quotaSalesperson.[SalesPersonQuotaHashKey] = quota.[HashKey]
    INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS businessSalesperson
        ON businessSalesperson.[HashKey] = quotaSalesperson.[SalesPersonHashKey]
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_Salesperson] AS salesperson
        ON salesperson.[BusinessEntityID] = businessSalesperson.[BusinessEntityID]
    INNER JOIN [AdventureWorksAnalytics].[dw].[Dim_Date] AS quotaPeriod
        ON quotaPeriod.[CalendarDate] = CONVERT(date, quota.[QuotaDate])
    WHERE quotaDetail.[SalesQuota] IS NOT NULL
) AS source
ON target.[QuotaPeriodKey] = source.[QuotaPeriodKey] AND target.[SalespersonKey] = source.[SalespersonKey]
WHEN MATCHED THEN UPDATE SET [QuotaAmount] = source.[QuotaAmount]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([QuotaPeriodKey], [SalespersonKey], [QuotaAmount], [AuditId], [InsertDateTime2])
    VALUES (source.[QuotaPeriodKey], source.[SalespersonKey], source.[QuotaAmount],
        CONVERT(bigint, COALESCE(SESSION_CONTEXT(N'MetaPipeline.AuditId'), N'MetaPipeline.AuditId is required')),
        CONVERT(datetime2(7), COALESCE(SESSION_CONTEXT(N'MetaPipeline.TaskStartedAtUtc'), N'MetaPipeline.TaskStartedAtUtc is required'))
    );
