$ErrorActionPreference = 'Stop'

$martRoot = 'dw\AdventureWorksMetaDemo'
$transformRoot = Join-Path $martRoot 'Transforms'
$transformWorkspace = Join-Path $transformRoot 'Workspace'
$targetSchema = Join-Path $martRoot 'Schema'
$bindingWorkspace = Join-Path $martRoot 'Binding'
$dqWorkspace = Join-Path $martRoot 'DataQuality'
$dqSqlRoot = Join-Path $martRoot 'DataQualitySql'
$pipelineWorkspace = 'ops\Pipeline'
$orchestrationWorkspace = 'ops\Orchestration'
$runArtifactsRoot = 'ops\RunArtifacts'

function Invoke-Product {
    param(
        [Parameter(Mandatory = $true)]
        [string] $FileName,

        [Parameter(Mandatory = $true)]
        [string[]] $Arguments
    )

    Write-Host ($FileName + ' ' + ($Arguments -join ' '))
    & $FileName @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "$FileName failed with exit code $LASTEXITCODE."
    }
}

function Assert-Workspace {
    param([string] $WorkspacePath)

    if (!(Test-Path -LiteralPath (Join-Path $WorkspacePath 'workspace.xml'))) {
        throw "$WorkspacePath\workspace.xml was not created."
    }
}

function Reset-Directory {
    param([string] $Path)

    if (Test-Path -LiteralPath $Path) {
        Remove-Item -LiteralPath $Path -Recurse -Force
    }

    New-Item -ItemType Directory -Force -Path $Path | Out-Null
}

function Convert-ToIdentifierToken {
    param([string] $Value)

    $name = ($Value -replace '[^A-Za-z0-9_]+', '_').Trim('_')
    if ([string]::IsNullOrWhiteSpace($name)) {
        return 'Item'
    }

    return $name
}

function Convert-ToPipelineName {
    param([string] $Layer, [string] $SqlIdentifier)

    return ((Convert-ToIdentifierToken $Layer) + '_' + (Convert-ToIdentifierToken $SqlIdentifier))
}

function Convert-ToStepName {
    param([string] $SqlIdentifier)

    return ('Load_' + (Convert-ToIdentifierToken $SqlIdentifier))
}

function Split-SqlIdentifier {
    param([string] $SqlIdentifier)

    $parts = $SqlIdentifier.Split('.', [System.StringSplitOptions]::RemoveEmptyEntries)
    if ($parts.Count -lt 2) {
        throw "Expected schema.object SQL identifier, got '$SqlIdentifier'."
    }

    [pscustomobject]@{
        Schema = $parts[$parts.Count - 2].Trim([char[]]'[]')
        Object = $parts[$parts.Count - 1].Trim([char[]]'[]')
    }
}

function New-Transform {
    param([string] $File, [string] $Target, [string] $SelectSql)

    $targetParts = Split-SqlIdentifier $Target
    [pscustomobject]@{
        File = $File
        Target = $Target
        ViewName = ('{0}.v_load_{1}' -f $targetParts.Schema, $targetParts.Object)
        SelectSql = $SelectSql.Trim().TrimEnd(';')
    }
}

function Write-Transforms {
    param(
        [string] $Root,
        [object[]] $Transforms
    )

    $sqlRoot = Join-Path $Root 'sql'
    Reset-Directory $sqlRoot
    $manifestRows = New-Object System.Collections.Generic.List[string]
    $manifestRows.Add("Path`tTarget")

    foreach ($transform in $Transforms) {
        $path = Join-Path $sqlRoot $transform.File
        $sql = @"
CREATE OR ALTER VIEW $($transform.ViewName)
AS
$($transform.SelectSql);
"@
        Set-Content -LiteralPath $path -Value $sql.Trim() -Encoding ASCII
        $manifestRows.Add(("sql\{0}`t{1}" -f $transform.File, $transform.Target))
    }

    Set-Content -LiteralPath (Join-Path $Root 'manifest.tsv') -Value $manifestRows -Encoding ASCII
}

function Read-TransformManifest {
    param(
        [string] $Root,
        [string] $Layer,
        [string] $TransformWorkspace,
        [string] $BindingWorkspace,
        [string] $ConnectionEnv
    )

    $manifestPath = Join-Path $Root 'manifest.tsv'
    if (!(Test-Path -LiteralPath $manifestPath)) {
        throw "Transform manifest was not found: $manifestPath"
    }

    Import-Csv -LiteralPath $manifestPath -Delimiter "`t" | ForEach-Object {
        $relativePath = $_.Path.Trim()
        $target = $_.Target.Trim()
        if ([string]::IsNullOrWhiteSpace($relativePath) -or [string]::IsNullOrWhiteSpace($target)) {
            throw "Manifest $manifestPath contains an empty path or target."
        }

        $filePath = Join-Path $Root $relativePath
        if (!(Test-Path -LiteralPath $filePath)) {
            throw "Transform SQL file was not found: $filePath"
        }

        $targetParts = Split-SqlIdentifier $target
        [pscustomobject]@{
            Layer = $Layer
            Target = $target
            TransformWorkspace = $TransformWorkspace
            BindingWorkspace = $BindingWorkspace
            ConnectionEnv = $ConnectionEnv
            ScriptName = ('{0}.v_load_{1}' -f $targetParts.Schema, $targetParts.Object)
        }
    }
}

function Add-ModeledPipeline {
    param([pscustomobject] $Transform)

    $pipelineName = Convert-ToPipelineName $Transform.Layer $Transform.Target
    $stepName = Convert-ToStepName $Transform.Target

    Invoke-Product meta-pipeline @(
        'add-pipeline',
        '--workspace', $pipelineWorkspace,
        '--name', $pipelineName,
        '--description', "Modeled $($Transform.Layer) table-load pipeline for $($Transform.Target).") | Out-Host

    Invoke-Product meta-pipeline @(
        'add-step',
        '--workspace', $pipelineWorkspace,
        '--pipeline', $pipelineName,
        '--step-name', $stepName,
        '--script', $Transform.ScriptName,
        '--transform-workspace', $Transform.TransformWorkspace,
        '--binding-workspace', $Transform.BindingWorkspace,
        '--execution-connection-env', $Transform.ConnectionEnv,
        '--target-connection-env', $Transform.ConnectionEnv,
        '--target', $Transform.Target,
        '--target-write', 'insert-rows',
        '--target-data-type-system', 'SqlServer') | Out-Host

    [pscustomobject]@{
        PipelineName = $pipelineName
        StepName = $stepName
        Target = $Transform.Target
        Layer = $Transform.Layer
    }
}

function Invoke-Sql {
    param([string] $ConnectionEnv, [string] $Query)

    Invoke-Product meta-sql @('execute', '--connection-env', $ConnectionEnv, '--quiet', '--query', $Query)
}

function Reset-TargetDatabase {
    $databaseName = $env:AW_TARGET_DATABASE
    Invoke-Sql 'AW_MASTER_SQL' @"
IF DB_ID(N'$databaseName') IS NOT NULL
BEGIN
    ALTER DATABASE [$databaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$databaseName];
END;
CREATE DATABASE [$databaseName];
"@
    Invoke-Sql 'AW_TARGET_SQL' 'CREATE SCHEMA awbi;'
}

function Initialize-MartTables {
    param([object[]] $Transforms)

    foreach ($transform in $Transforms) {
        $targetParts = Split-SqlIdentifier $transform.Target
        $query = @"
IF OBJECT_ID(N'$($transform.Target)', N'U') IS NOT NULL
    DROP TABLE $($transform.Target);

SELECT TOP (0)
    src.*
INTO $($transform.Target)
FROM
(
$($transform.SelectSql)
) AS src;
"@
        Write-Host ("Creating empty mart table {0}.{1} from modeled select shape." -f $targetParts.Schema, $targetParts.Object)
        Invoke-Sql 'AW_TARGET_SQL' $query
    }
}

function Clear-LoadedTablesForPipelineReplay {
    $martCleanup = @'
DELETE FROM awbi.FactSalesOrderLine;
DELETE FROM awbi.DimCustomerChannel;
DELETE FROM awbi.DimSalesTerritory;
DELETE FROM awbi.DimOrderDate;
DELETE FROM awbi.DimProduct;
'@
    $bdvSalesCleanup = @'
DELETE FROM dbo.BLS_SalesOrderLineProduct_SalesOrderLineMeasures;
DELETE FROM dbo.BL_SalesOrderLineProduct;
DELETE FROM dbo.BL_SalesOrderLineOrder;
DELETE FROM dbo.BL_SalesOrderDate;
DELETE FROM dbo.BL_SalesOrderTerritory;
DELETE FROM dbo.BL_SalesOrderSalesPerson;
DELETE FROM dbo.BL_SalesOrderStore;
DELETE FROM dbo.BL_SalesOrderCustomer;
DELETE FROM dbo.BHS_SalesOrder_SalesOrderProfile;
DELETE FROM dbo.BH_SalesOrder;
DELETE FROM dbo.BH_SalesOrderLine;
DELETE FROM dbo.BH_OrderDate;
DELETE FROM dbo.BHS_Customer_CustomerProfile;
DELETE FROM dbo.BH_Customer;
DELETE FROM dbo.BHS_Store_StoreProfile;
DELETE FROM dbo.BH_Store;
DELETE FROM dbo.BHS_SalesPerson_SalesPersonProfile;
DELETE FROM dbo.BH_SalesPerson;
DELETE FROM dbo.BHS_SalesTerritory_SalesTerritoryProfile;
DELETE FROM dbo.BH_SalesTerritory;
'@
    $bdvProductCleanup = @'
DELETE FROM dbo.BHS_Product_ProductClassification;
DELETE FROM dbo.BH_Product;
'@
    $rdvSalesCleanup = @'
DELETE FROM dbo.L_SalesOrderDetailSpecialOfferProduct;
DELETE FROM dbo.L_SpecialOfferProductProduct;
DELETE FROM dbo.L_SalesOrderDetailSalesOrderHeader;
DELETE FROM dbo.L_SalesOrderHeaderSalesTerritory;
DELETE FROM dbo.L_SalesOrderHeaderSalesPerson;
DELETE FROM dbo.L_SalesOrderHeaderCustomer;
DELETE FROM dbo.L_CustomerSalesTerritory;
DELETE FROM dbo.L_CustomerStore;
DELETE FROM dbo.HS_SalesOrderDetail_SalesOrderDetail;
DELETE FROM dbo.H_SalesOrderDetail;
DELETE FROM dbo.HS_SalesOrderHeader_SalesOrderHeader;
DELETE FROM dbo.H_SalesOrderHeader;
DELETE FROM dbo.HS_SpecialOfferProduct_SpecialOfferProduct;
DELETE FROM dbo.H_SpecialOfferProduct;
DELETE FROM dbo.HS_SalesPerson_SalesPerson;
DELETE FROM dbo.H_SalesPerson;
DELETE FROM dbo.HS_SalesTerritory_SalesTerritory;
DELETE FROM dbo.H_SalesTerritory;
DELETE FROM dbo.HS_Store_Store;
DELETE FROM dbo.H_Store;
DELETE FROM dbo.HS_Customer_Customer;
DELETE FROM dbo.H_Customer;
'@
    $rdvProductCleanup = @'
DELETE FROM dbo.L_ProductProductSubcategory;
DELETE FROM dbo.L_ProductSubcategoryProductCategory;
DELETE FROM dbo.HS_Product_Product;
DELETE FROM dbo.H_Product;
DELETE FROM dbo.HS_ProductSubcategory_ProductSubcategory;
DELETE FROM dbo.H_ProductSubcategory;
DELETE FROM dbo.HS_ProductCategory_ProductCategory;
DELETE FROM dbo.H_ProductCategory;
'@

    Write-Host 'Clearing mart tables before orchestrated reload.'
    Invoke-Sql 'AW_TARGET_SQL' $martCleanup
    Write-Host 'Clearing BDV Sales slice tables before orchestrated reload.'
    Invoke-Sql 'AW_BDV_SQL' $bdvSalesCleanup
    Write-Host 'Clearing BDV Product slice tables before orchestrated reload.'
    Invoke-Sql 'AW_BDV_SQL' $bdvProductCleanup
    Write-Host 'Clearing RDV Sales slice tables before orchestrated reload.'
    Invoke-Sql 'AW_RDV_SQL' $rdvSalesCleanup
    Write-Host 'Clearing RDV Product slice tables before orchestrated reload.'
    Invoke-Sql 'AW_RDV_SQL' $rdvProductCleanup
}

if (!(Test-Path -LiteralPath 'bdv\AdventureWorksBusinessVault\SalesBinding\workspace.xml')) {
    throw 'BDV sales binding workspace is missing. Run the sales vault load gate first.'
}

Reset-Directory $martRoot
Reset-Directory 'ops'

$martTransforms = @(
    New-Transform '001_load_DimProduct.sql' 'awbi.DimProduct' @'
SELECT
    hp.ProductId,
    hpc.ProductNumber,
    hpc.ProductName,
    hpc.ProductCategory,
    hpc.ProductSubcategory
FROM [AdventureWorksBusinessVault].[dbo].[BH_Product] AS hp
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_Product_ProductClassification] AS hpc
    ON hpc.HubHashKey = hp.HashKey
'@
    New-Transform '002_load_DimOrderDate.sql' 'awbi.DimOrderDate' @'
SELECT
    od.OrderDate,
    DATEPART(year, od.OrderDate) AS OrderYear,
    DATEPART(month, od.OrderDate) AS OrderMonth,
    CONVERT(nvarchar(7), od.OrderDate, 126) AS OrderMonthName
FROM [AdventureWorksBusinessVault].[dbo].[BH_OrderDate] AS od
'@
    New-Transform '003_load_DimSalesTerritory.sql' 'awbi.DimSalesTerritory' @'
SELECT
    st.TerritoryId,
    stp.TerritoryName,
    stp.CountryRegionCode,
    stp.TerritoryGroup
FROM [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS st
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesTerritory_SalesTerritoryProfile] AS stp
    ON stp.HubHashKey = st.HashKey
'@
    New-Transform '004_load_DimCustomerChannel.sql' 'awbi.DimCustomerChannel' @'
SELECT
    c.CustomerId,
    cp.CustomerAccountNumber,
    cp.CustomerType
FROM [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS c
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_Customer_CustomerProfile] AS cp
    ON cp.HubHashKey = c.HashKey
'@
    New-Transform '005_load_FactSalesOrderLine.sql' 'awbi.FactSalesOrderLine' @'
SELECT
    sol.SalesOrderId,
    sol.SalesOrderDetailId,
    so.SalesOrderNumber,
    od.OrderDate,
    p.ProductId,
    pc.ProductNumber,
    pc.ProductName,
    pc.ProductCategory,
    pc.ProductSubcategory,
    c.CustomerId,
    cp.CustomerType,
    st.TerritoryId,
    stp.TerritoryName,
    stp.TerritoryGroup,
    sp.SalesPersonBusinessEntityId,
    spp.SalesPersonName,
    so.OnlineOrderFlag,
    m.OrderQuantity,
    m.UnitPrice,
    m.LineTotal AS SalesAmount,
    m.DiscountAmount,
    m.TaxAmount,
    m.FreightAmount
FROM [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineProduct] AS lop
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BLS_SalesOrderLineProduct_SalesOrderLineMeasures] AS m
    ON m.LinkHashKey = lop.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrderLine] AS sol
    ON sol.HashKey = lop.SalesOrderLineHashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Product] AS p
    ON p.HashKey = lop.ProductHashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_Product_ProductClassification] AS pc
    ON pc.HubHashKey = p.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderLineOrder] AS loo
    ON loo.SalesOrderLineHashKey = sol.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesOrder] AS soHub
    ON soHub.HashKey = loo.SalesOrderHashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesOrder_SalesOrderProfile] AS so
    ON so.HubHashKey = soHub.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderDate] AS sod
    ON sod.SalesOrderHashKey = soHub.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_OrderDate] AS od
    ON od.HashKey = sod.OrderDateHashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderCustomer] AS soc
    ON soc.SalesOrderHashKey = soHub.HashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BH_Customer] AS c
    ON c.HashKey = soc.CustomerHashKey
INNER JOIN [AdventureWorksBusinessVault].[dbo].[BHS_Customer_CustomerProfile] AS cp
    ON cp.HubHashKey = c.HashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderTerritory] AS sot
    ON sot.SalesOrderHashKey = soHub.HashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesTerritory] AS st
    ON st.HashKey = sot.SalesTerritoryHashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesTerritory_SalesTerritoryProfile] AS stp
    ON stp.HubHashKey = st.HashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BL_SalesOrderSalesPerson] AS sosp
    ON sosp.SalesOrderHashKey = soHub.HashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BH_SalesPerson] AS sp
    ON sp.HashKey = sosp.SalesPersonHashKey
LEFT JOIN [AdventureWorksBusinessVault].[dbo].[BHS_SalesPerson_SalesPersonProfile] AS spp
    ON spp.HubHashKey = sp.HashKey
'@
)

Write-Transforms $transformRoot $martTransforms
Reset-TargetDatabase
Initialize-MartTables $martTransforms

Invoke-Product meta-transform-script @('from', 'sql-files', '--manifest', (Join-Path $transformRoot 'manifest.tsv'), '--new-workspace', $transformWorkspace, '--report', 'logs\06-mart-transform-import-report.tsv', '--verbose')
Assert-Workspace $transformWorkspace

Invoke-Product meta-schema @('extract', 'sqlserver', '--new-workspace', $targetSchema, '--connection-env', 'AW_TARGET_SQL', '--system', $env:AW_TARGET_DATABASE, '--all-schemas', '--all-tables')
Assert-Workspace $targetSchema

Invoke-Product meta-transform-binding @('bind', '--transform-workspace', $transformWorkspace, '--source-schema', 'bdv\AdventureWorksBusinessVault\Schema', '--target-schema', $targetSchema, '--execute-system', $env:AW_TARGET_DATABASE, '--new-workspace', $bindingWorkspace, '--data-type-conversion-workspace', $env:DATA_TYPE_CONVERSION_WORKSPACE)
Assert-Workspace $bindingWorkspace

Invoke-Product meta-data-quality @('from-transform-workspace', '--transform-workspace', $transformWorkspace, '--binding-workspace', $bindingWorkspace, '--new-workspace', $dqWorkspace)
Assert-Workspace $dqWorkspace
Invoke-Product meta-data-quality @('inspect', '--workspace', $dqWorkspace)
Invoke-Product meta-data-quality @('promote', '--workspace', $dqWorkspace, '--all')
Invoke-Product meta-convert @('data-quality-to-sql', '--workspace', $dqWorkspace, '--out', $dqSqlRoot)

$dqScripts = @(Get-ChildItem -LiteralPath $dqSqlRoot -Filter '*.sql' -File -Recurse | Sort-Object FullName)
if ($dqScripts.Count -eq 0) {
    throw "No DQ SQL files were generated under $dqSqlRoot."
}

Write-Host ("DQ SQL pack generated: {0} files under {1}." -f $dqScripts.Count, $dqSqlRoot)

$dqOperationalScript = Join-Path $dqSqlRoot 'MetaDQ.Operational.sql'
$dqDashboardScript = Join-Path $dqSqlRoot 'v_DataQualityReview.sql'
$dqViewScripts = @($dqScripts | Where-Object {
        $_.Name -ne 'MetaDQ.Operational.sql' -and $_.Name -ne 'v_DataQualityReview.sql'
    })
foreach ($dqScript in $dqViewScripts) {
    Invoke-Product meta-sql @('execute', '--connection-env', 'AW_TARGET_SQL', '--file', $dqScript.FullName, '--quiet')
}

Invoke-Product meta-sql @('execute', '--connection-env', 'AW_TARGET_SQL', '--file', $dqDashboardScript, '--quiet')
Invoke-Product meta-sql @('execute', '--connection-env', 'AW_MASTER_SQL', '--file', $dqOperationalScript, '--quiet')

$rdvProductTransforms = @(Read-TransformManifest `
    -Root 'rdv\AdventureWorksRawVault\Transforms' `
    -Layer 'RDV_Product' `
    -TransformWorkspace 'rdv\AdventureWorksRawVault\Transforms\Workspace' `
    -BindingWorkspace 'rdv\AdventureWorksRawVault\Binding' `
    -ConnectionEnv 'AW_RDV_SQL')
$rdvSalesTransforms = @(Read-TransformManifest `
    -Root 'rdv\AdventureWorksRawVault\SalesTransforms' `
    -Layer 'RDV_Sales' `
    -TransformWorkspace 'rdv\AdventureWorksRawVault\SalesTransforms\Workspace' `
    -BindingWorkspace 'rdv\AdventureWorksRawVault\SalesBinding' `
    -ConnectionEnv 'AW_RDV_SQL')
$bdvProductTransforms = @(Read-TransformManifest `
    -Root 'bdv\AdventureWorksBusinessVault\Transforms' `
    -Layer 'BDV_Product' `
    -TransformWorkspace 'bdv\AdventureWorksBusinessVault\Transforms\Workspace' `
    -BindingWorkspace 'bdv\AdventureWorksBusinessVault\Binding' `
    -ConnectionEnv 'AW_BDV_SQL')
$bdvSalesTransforms = @(Read-TransformManifest `
    -Root 'bdv\AdventureWorksBusinessVault\SalesTransforms' `
    -Layer 'BDV_Sales' `
    -TransformWorkspace 'bdv\AdventureWorksBusinessVault\SalesTransforms\Workspace' `
    -BindingWorkspace 'bdv\AdventureWorksBusinessVault\SalesBinding' `
    -ConnectionEnv 'AW_BDV_SQL')
$dwTransforms = @(Read-TransformManifest `
    -Root $transformRoot `
    -Layer 'DW_Mart' `
    -TransformWorkspace $transformWorkspace `
    -BindingWorkspace $bindingWorkspace `
    -ConnectionEnv 'AW_TARGET_SQL')

Write-Host ("Pipeline build plan: RDV Product {0}, RDV Sales {1}, BDV Product {2}, BDV Sales {3}, DW/Mart {4}, total {5}." -f `
    $rdvProductTransforms.Count,
    $rdvSalesTransforms.Count,
    $bdvProductTransforms.Count,
    $bdvSalesTransforms.Count,
    $dwTransforms.Count,
    ($rdvProductTransforms.Count + $rdvSalesTransforms.Count + $bdvProductTransforms.Count + $bdvSalesTransforms.Count + $dwTransforms.Count))

Clear-LoadedTablesForPipelineReplay

Invoke-Product meta-pipeline @('--new-workspace', $pipelineWorkspace)
$allModeledPipelines = @()
$allModeledPipelines += @($rdvProductTransforms | ForEach-Object { Add-ModeledPipeline $_ })
$allModeledPipelines += @($rdvSalesTransforms | ForEach-Object { Add-ModeledPipeline $_ })
$allModeledPipelines += @($bdvProductTransforms | ForEach-Object { Add-ModeledPipeline $_ })
$allModeledPipelines += @($bdvSalesTransforms | ForEach-Object { Add-ModeledPipeline $_ })
$allModeledPipelines += @($dwTransforms | ForEach-Object { Add-ModeledPipeline $_ })
Assert-Workspace $pipelineWorkspace

Invoke-Product meta-orchestration @('--pipeline-workspace', $pipelineWorkspace, '--new-workspace', $orchestrationWorkspace, '--description', 'Modeled orchestration for one transform-backed table-producing pipeline per authored RDV, BDV, and DW/Mart transform.')
Assert-Workspace $orchestrationWorkspace

Invoke-Product meta-orchestration @('refresh-run-plan', '--workspace', $orchestrationWorkspace)
Invoke-Product meta-orchestration @('inspect-run-plan', '--workspace', $orchestrationWorkspace)
Invoke-Product meta-orchestration @('execute', '--workspace', $orchestrationWorkspace, '--pipeline-workspace', $pipelineWorkspace, '--max-degree-of-parallelism', '4', '--run-artifacts-root', $runArtifactsRoot)

Write-Host 'Post-orchestration mart proof from BDV-backed persisted tables:'
Invoke-Product meta-sql @('execute', '--connection-env', 'AW_TARGET_SQL', '--query', @'
SELECT
    COUNT_BIG(*) AS FactRows,
    SUM(SalesAmount) AS SalesAmount,
    SUM(TaxAmount) AS TaxAmount,
    SUM(FreightAmount) AS FreightAmount
FROM awbi.FactSalesOrderLine;

SELECT TOP (5)
    ProductCategory,
    COUNT_BIG(*) AS LineRows,
    SUM(SalesAmount) AS SalesAmount
FROM awbi.FactSalesOrderLine
GROUP BY ProductCategory
ORDER BY SalesAmount DESC;

SELECT
    Issue,
    COUNT_BIG(*) AS ReviewRows,
    SUM(TotalSuspectCount) AS TotalSuspectCount
FROM dq.v_DataQualityReview
GROUP BY Issue
ORDER BY Issue;
'@)

Write-Host ("Modeled pipeline/orchestration gate passed for {0} transform-backed table-load pipelines." -f $allModeledPipelines.Count)
