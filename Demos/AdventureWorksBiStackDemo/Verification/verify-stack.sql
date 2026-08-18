SET NOCOUNT ON;

IF DB_ID(N'AdventureWorks2022') IS NULL
    THROW 51000, N'AdventureWorks2022 is not available.', 1;

IF DB_ID(N'AdventureWorksRawVault') IS NULL
    THROW 51000, N'AdventureWorksRawVault is not available.', 1;

IF DB_ID(N'AdventureWorksBusinessVault') IS NULL
    THROW 51000, N'AdventureWorksBusinessVault is not available.', 1;

IF DB_ID(N'AdventureWorksAnalytics') IS NULL
    THROW 51000, N'AdventureWorksAnalytics is not available.', 1;

IF DB_ID(N'AdventureWorksMetaPipeline') IS NULL
    THROW 51000, N'AdventureWorksMetaPipeline is not available.', 1;

DECLARE @SourceSalesOrderRows bigint =
(
    SELECT COUNT_BIG(*)
    FROM AdventureWorks2022.Sales.SalesOrderHeader
);

DECLARE @SourceSalesLineRows bigint =
(
    SELECT COUNT_BIG(*)
    FROM AdventureWorks2022.Sales.SalesOrderDetail
);

DECLARE @SourceQuotaRows bigint =
(
    SELECT COUNT_BIG(*)
    FROM AdventureWorks2022.Sales.SalesPersonQuotaHistory
);

IF (SELECT COUNT_BIG(*) FROM AdventureWorksRawVault.dbo.H_SalesOrderHeader) <> @SourceSalesOrderRows
    THROW 51000, N'Raw Data Vault sales-order count does not match the source.', 1;

IF (SELECT COUNT_BIG(*) FROM AdventureWorksRawVault.dbo.H_SalesOrderDetail) <> @SourceSalesLineRows
    THROW 51000, N'Raw Data Vault sales-line count does not match the source.', 1;

IF (SELECT COUNT_BIG(*) FROM AdventureWorksBusinessVault.dbo.BH_SalesOrder) <> @SourceSalesOrderRows
    THROW 51000, N'Business Data Vault sales-order count does not match the source.', 1;

IF (SELECT COUNT_BIG(*) FROM AdventureWorksBusinessVault.dbo.BH_SalesOrderLine) <> @SourceSalesLineRows
    THROW 51000, N'Business Data Vault sales-line count does not match the source.', 1;

IF (SELECT COUNT_BIG(*) FROM AdventureWorksAnalytics.dw.Fact_SalesOrder) <> @SourceSalesOrderRows
    THROW 51000, N'Warehouse sales-order count does not match the source.', 1;

IF (SELECT COUNT_BIG(*) FROM AdventureWorksAnalytics.dw.Fact_SalesLine) <> @SourceSalesLineRows
    THROW 51000, N'Warehouse sales-line count does not match the source.', 1;

IF (SELECT COUNT_BIG(*) FROM AdventureWorksAnalytics.dw.Fact_SalespersonQuota) <> @SourceQuotaRows
    THROW 51000, N'Warehouse salesperson-quota count does not match the source.', 1;

IF EXISTS
(
    SELECT SalesOrderID
    FROM AdventureWorksAnalytics.dw.Fact_SalesOrder
    GROUP BY SalesOrderID
    HAVING COUNT_BIG(*) <> 1
)
    THROW 51000, N'Warehouse sales-order grain is not unique.', 1;

IF EXISTS
(
    SELECT SalesOrderID, SalesOrderDetailID
    FROM AdventureWorksAnalytics.dw.Fact_SalesLine
    GROUP BY SalesOrderID, SalesOrderDetailID
    HAVING COUNT_BIG(*) <> 1
)
    THROW 51000, N'Warehouse sales-line grain is not unique.', 1;

IF EXISTS
(
    SELECT QuotaPeriodKey, SalespersonKey
    FROM AdventureWorksAnalytics.dw.Fact_SalespersonQuota
    GROUP BY QuotaPeriodKey, SalespersonKey
    HAVING COUNT_BIG(*) <> 1
)
    THROW 51000, N'Warehouse salesperson-quota grain is not unique.', 1;

DECLARE @SourceTotalDue decimal(38, 4) =
(
    SELECT SUM(CONVERT(decimal(38, 4), TotalDue))
    FROM AdventureWorks2022.Sales.SalesOrderHeader
);

DECLARE @WarehouseTotalDue decimal(38, 4) =
(
    SELECT SUM(CONVERT(decimal(38, 4), TotalDue))
    FROM AdventureWorksAnalytics.dw.Fact_SalesOrder
);

IF @SourceTotalDue <> @WarehouseTotalDue
    THROW 51000, N'Warehouse total due does not reconcile to the source.', 1;

DECLARE @SourceLineSales decimal(38, 4) =
(
    SELECT SUM(CONVERT(decimal(38, 4), LineTotal))
    FROM AdventureWorks2022.Sales.SalesOrderDetail
);

DECLARE @WarehouseLineSales decimal(38, 4) =
(
    SELECT SUM(CONVERT(decimal(38, 4), LineSalesAmount))
    FROM AdventureWorksAnalytics.dw.Fact_SalesLine
);

IF @SourceLineSales <> @WarehouseLineSales
    THROW 51000, N'Warehouse line sales do not reconcile to the source.', 1;

DECLARE @SourceQuotaAmount decimal(38, 4) =
(
    SELECT SUM(CONVERT(decimal(38, 4), SalesQuota))
    FROM AdventureWorks2022.Sales.SalesPersonQuotaHistory
);

DECLARE @WarehouseQuotaAmount decimal(38, 4) =
(
    SELECT SUM(CONVERT(decimal(38, 4), QuotaAmount))
    FROM AdventureWorksAnalytics.dw.Fact_SalespersonQuota
);

IF @SourceQuotaAmount <> @WarehouseQuotaAmount
    THROW 51000, N'Warehouse quota amount does not reconcile to the source.', 1;

DECLARE @DataQualityChecks bigint =
(
    SELECT COUNT_BIG(*)
    FROM AdventureWorksAnalytics.dq.v_DataQualityReview
);

IF @DataQualityChecks <> 167
    THROW 51000, N'The deployed Data Quality review does not contain the 167 promoted checks.', 1;

IF EXISTS
(
    SELECT 1
    FROM AdventureWorksAnalytics.dq.v_DataQualityReview
    WHERE COALESCE(ResultRowCount, RowsReturned, 0) <> 0
)
    THROW 51000, N'Data Quality checks returned findings.', 1;

IF NOT EXISTS
(
    SELECT 1
    FROM AdventureWorksMetaPipeline.MetaPipeline.SchemaVersion
    WHERE Version = 7
)
    THROW 51000, N'The MetaPipeline operational schema is not at version 7.', 1;

DECLARE @LatestPipelineRun TABLE
(
    PipelineId nvarchar(128) NOT NULL PRIMARY KEY,
    PipelineRunId uniqueidentifier NOT NULL,
    Status nvarchar(32) NOT NULL,
    TransformScriptName nvarchar(512) NULL
);

WITH RankedPipelineRun AS
(
    SELECT
        PipelineId,
        PipelineRunId,
        Status,
        TransformScriptName,
        ROW_NUMBER() OVER
        (
            PARTITION BY PipelineId
            ORDER BY StartedAtUtc DESC, PipelineRunId DESC
        ) AS Recency
    FROM AdventureWorksMetaPipeline.MetaPipeline.PipelineRun
    WHERE PipelineId IN (N'SourceToRaw', N'RawToBusiness', N'BusinessToWarehouse')
)
INSERT @LatestPipelineRun (PipelineId, PipelineRunId, Status, TransformScriptName)
SELECT PipelineId, PipelineRunId, Status, TransformScriptName
FROM RankedPipelineRun
WHERE Recency = 1;

IF (SELECT COUNT_BIG(*) FROM @LatestPipelineRun) <> 3
    THROW 51000, N'Latest ETL evidence does not include all three pipelines.', 1;

IF EXISTS (SELECT 1 FROM @LatestPipelineRun WHERE Status <> N'Succeeded')
    THROW 51000, N'At least one latest ETL pipeline run did not succeed.', 1;

IF EXISTS (SELECT 1 FROM @LatestPipelineRun WHERE TransformScriptName IS NOT NULL)
    THROW 51000, N'Pipeline-level audit evidence incorrectly contains aggregate transform identity.', 1;

DECLARE @ExpectedTaskCount TABLE
(
    PipelineId nvarchar(128) NOT NULL PRIMARY KEY,
    TaskCount bigint NOT NULL
);

INSERT @ExpectedTaskCount (PipelineId, TaskCount)
VALUES
    (N'SourceToRaw', 53),
    (N'RawToBusiness', 47),
    (N'BusinessToWarehouse', 20);

IF EXISTS
(
    SELECT 1
    FROM @LatestPipelineRun AS pipeline
    INNER JOIN @ExpectedTaskCount AS expected
        ON expected.PipelineId = pipeline.PipelineId
    OUTER APPLY
    (
        SELECT
            COUNT_BIG(*) AS TaskCount,
            COUNT_BIG(taskRun.TransformScriptId) AS ScriptIdentityCount,
            COUNT_BIG(DISTINCT taskRun.TransformScriptId) AS DistinctScriptCount
        FROM AdventureWorksMetaPipeline.MetaPipeline.TaskRun AS taskRun
        WHERE taskRun.PipelineRunId = pipeline.PipelineRunId
    ) AS actual
    WHERE actual.TaskCount <> expected.TaskCount
       OR actual.ScriptIdentityCount <> expected.TaskCount
       OR actual.DistinctScriptCount <> expected.TaskCount
)
    THROW 51000, N'Latest ETL task evidence is incomplete or has duplicate transform identities.', 1;

SELECT
    N'AdventureWorks full stack verified' AS Status,
    @SourceSalesOrderRows AS SalesOrders,
    @SourceSalesLineRows AS SalesLines,
    @SourceQuotaRows AS SalespersonQuotas,
    @WarehouseTotalDue AS TotalDue,
    @WarehouseLineSales AS LineSales,
    @WarehouseQuotaAmount AS QuotaAmount,
    @DataQualityChecks AS DataQualityChecks;
