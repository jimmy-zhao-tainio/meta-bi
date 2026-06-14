$ErrorActionPreference = 'Stop'

$rdvTransformRoot = 'rdv\AdventureWorksRawVault\Transforms'
$bdvTransformRoot = 'bdv\AdventureWorksBusinessVault\Transforms'
$rdvWorkspace = Join-Path $rdvTransformRoot 'Workspace'
$bdvWorkspace = Join-Path $bdvTransformRoot 'Workspace'
$rdvBinding = 'rdv\AdventureWorksRawVault\Binding'
$bdvBinding = 'bdv\AdventureWorksBusinessVault\Binding'

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

function Invoke-Sql {
    param([string] $ConnectionEnv, [string] $Sql)

    Invoke-Product meta-sql @('execute', '--connection-env', $ConnectionEnv, '--query', $Sql)
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
        Set-Content -LiteralPath $path -Value $transform.Sql.Trim() -Encoding ASCII
        $manifestRows.Add(("sql\{0}`t{1}" -f $transform.File, $transform.Target))
    }

    Set-Content -LiteralPath (Join-Path $Root 'manifest.tsv') -Value $manifestRows -Encoding ASCII
}

function New-Transform {
    param([string] $File, [string] $Target, [string] $Sql)

    [pscustomobject]@{
        File = $File
        Target = $Target
        Sql = $Sql
    }
}

if (!(Test-Path -LiteralPath 'source\AdventureWorks2022\Schema\workspace.xml')) {
    throw 'source schema workspace is missing.'
}

if (!(Test-Path -LiteralPath 'rdv\AdventureWorksRawVault\DeployVerifyManifest\workspace.xml')) {
    throw 'RDV verify manifest is missing.'
}

if (!(Test-Path -LiteralPath 'bdv\AdventureWorksBusinessVault\DeployVerifyManifest\workspace.xml')) {
    throw 'BDV verify manifest is missing.'
}

Write-Host 'Extracting current RDV and BDV schemas for strict transform binding.'
Reset-Directory 'rdv\AdventureWorksRawVault\Schema'
Invoke-Product meta-schema @('extract', 'sqlserver', '--new-workspace', 'rdv\AdventureWorksRawVault\Schema', '--connection-env', 'AW_RDV_SQL', '--system', 'AdventureWorksRawVault', '--all-schemas', '--all-tables')
Assert-Workspace 'rdv\AdventureWorksRawVault\Schema'

Reset-Directory 'bdv\AdventureWorksBusinessVault\Schema'
Invoke-Product meta-schema @('extract', 'sqlserver', '--new-workspace', 'bdv\AdventureWorksBusinessVault\Schema', '--connection-env', 'AW_BDV_SQL', '--system', 'AdventureWorksBusinessVault', '--all-schemas', '--all-tables')
Assert-Workspace 'bdv\AdventureWorksBusinessVault\Schema'

Reset-Directory $rdvTransformRoot
Reset-Directory $bdvTransformRoot
if (Test-Path -LiteralPath $rdvBinding) { Remove-Item -LiteralPath $rdvBinding -Recurse -Force }
if (Test-Path -LiteralPath $bdvBinding) { Remove-Item -LiteralPath $bdvBinding -Recurse -Force }

$rdvTransforms = @(
    New-Transform '001_load_H_ProductCategory.sql' 'dbo.H_ProductCategory' @'
CREATE VIEW dbo.v_load_H_ProductCategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), pc.ProductCategoryID))) AS HashKey,
    CONVERT(nvarchar(256), pc.ProductCategoryID) AS ProductCategoryID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductCategory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductCategory] AS pc;
GO
'@
    New-Transform '002_load_HS_ProductCategory_ProductCategory.sql' 'dbo.HS_ProductCategory_ProductCategory' @'
CREATE VIEW dbo.v_load_HS_ProductCategory_ProductCategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), pc.ProductCategoryID))) AS HubHashKey,
    pc.Name,
    pc.rowguid,
    pc.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        pc.Name,
        CONVERT(nvarchar(36), pc.rowguid),
        CONVERT(nvarchar(30), pc.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductCategory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductCategory] AS pc;
GO
'@
    New-Transform '003_load_H_ProductSubcategory.sql' 'dbo.H_ProductSubcategory' @'
CREATE VIEW dbo.v_load_H_ProductSubcategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), ps.ProductSubcategoryID))) AS HashKey,
    CONVERT(nvarchar(256), ps.ProductSubcategoryID) AS ProductSubcategoryID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductSubcategory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductSubcategory] AS ps;
GO
'@
    New-Transform '004_load_HS_ProductSubcategory_ProductSubcategory.sql' 'dbo.HS_ProductSubcategory_ProductSubcategory' @'
CREATE VIEW dbo.v_load_HS_ProductSubcategory_ProductSubcategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), ps.ProductSubcategoryID))) AS HubHashKey,
    ps.Name,
    ps.rowguid,
    ps.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        ps.Name,
        CONVERT(nvarchar(36), ps.rowguid),
        CONVERT(nvarchar(30), ps.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductSubcategory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductSubcategory] AS ps;
GO
'@
    New-Transform '005_load_H_Product.sql' 'dbo.H_Product' @'
CREATE VIEW dbo.v_load_H_Product
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), p.ProductID))) AS HashKey,
    CONVERT(nvarchar(256), p.ProductID) AS ProductID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.Product') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[Product] AS p;
GO
'@
    New-Transform '006_load_HS_Product_Product.sql' 'dbo.HS_Product_Product' @'
CREATE VIEW dbo.v_load_HS_Product_Product
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), p.ProductID))) AS HubHashKey,
    p.Name,
    p.ProductNumber,
    p.MakeFlag,
    p.FinishedGoodsFlag,
    p.Color,
    p.SafetyStockLevel,
    p.ReorderPoint,
    p.StandardCost,
    p.ListPrice,
    p.Size,
    p.Weight,
    p.DaysToManufacture,
    p.ProductLine,
    p.Class,
    p.Style,
    p.SellStartDate,
    p.SellEndDate,
    p.DiscontinuedDate,
    p.rowguid,
    p.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        p.Name,
        p.ProductNumber,
        CONVERT(nvarchar(1), p.MakeFlag),
        CONVERT(nvarchar(1), p.FinishedGoodsFlag),
        p.Color,
        CONVERT(nvarchar(30), p.SafetyStockLevel),
        CONVERT(nvarchar(30), p.ReorderPoint),
        CONVERT(nvarchar(50), p.StandardCost),
        CONVERT(nvarchar(50), p.ListPrice),
        p.Size,
        CONVERT(nvarchar(50), p.Weight),
        CONVERT(nvarchar(30), p.DaysToManufacture),
        p.ProductLine,
        p.Class,
        p.Style,
        CONVERT(nvarchar(30), p.SellStartDate, 126),
        CONVERT(nvarchar(30), p.SellEndDate, 126),
        CONVERT(nvarchar(30), p.DiscontinuedDate, 126),
        CONVERT(nvarchar(36), p.rowguid),
        CONVERT(nvarchar(30), p.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.Product') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[Product] AS p;
GO
'@
    New-Transform '007_load_L_ProductSubcategoryProductCategory.sql' 'dbo.L_ProductSubcategoryProductCategory' @'
CREATE VIEW dbo.v_load_L_ProductSubcategoryProductCategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT_WS(N'|',
        CONVERT(nvarchar(256), ps.ProductSubcategoryID),
        CONVERT(nvarchar(256), ps.ProductCategoryID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), ps.ProductSubcategoryID))) AS ProductSubcategoryHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), ps.ProductCategoryID))) AS ProductCategoryHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.ProductSubcategory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[ProductSubcategory] AS ps;
GO
'@
    New-Transform '008_load_L_ProductProductSubcategory.sql' 'dbo.L_ProductProductSubcategory' @'
CREATE VIEW dbo.v_load_L_ProductProductSubcategory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT_WS(N'|',
        CONVERT(nvarchar(256), p.ProductID),
        CONVERT(nvarchar(256), p.ProductSubcategoryID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), p.ProductID))) AS ProductHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), p.ProductSubcategoryID))) AS ProductSubcategoryHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Production.Product') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Production].[Product] AS p
WHERE p.ProductSubcategoryID IS NOT NULL;
GO
'@
)

$bdvTransforms = @(
    New-Transform '001_load_BH_Product.sql' 'dbo.BH_Product' @'
CREATE VIEW dbo.v_load_BH_Product
AS
SELECT
    h.HashKey,
    h.ProductID AS ProductId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_Product') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Product] AS h;
GO
'@
    New-Transform '002_load_BHS_Product_ProductClassification.sql' 'dbo.BHS_Product_ProductClassification' @'
CREATE VIEW dbo.v_load_BHS_Product_ProductClassification
AS
SELECT
    h.HashKey AS HubHashKey,
    CONVERT(nvarchar(25), hs.ProductNumber) AS ProductNumber,
    CONVERT(nvarchar(100), hs.Name) AS ProductName,
    CONVERT(nvarchar(100), COALESCE(pc.Name, N'Unclassified')) AS ProductCategory,
    CONVERT(nvarchar(100), COALESCE(ps.Name, N'Unclassified')) AS ProductSubcategory,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        hs.ProductNumber,
        hs.Name,
        COALESCE(pc.Name, N'Unclassified'),
        COALESCE(ps.Name, N'Unclassified')))) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.ProductClassification') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Product] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_Product_Product] AS hs
    ON hs.HubHashKey = h.HashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[L_ProductProductSubcategory] AS lps
    ON lps.ProductHashKey = h.HashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[H_ProductSubcategory] AS hps
    ON hps.HashKey = lps.ProductSubcategoryHashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[HS_ProductSubcategory_ProductSubcategory] AS ps
    ON ps.HubHashKey = hps.HashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[L_ProductSubcategoryProductCategory] AS lpc
    ON lpc.ProductSubcategoryHashKey = hps.HashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[H_ProductCategory] AS hpc
    ON hpc.HashKey = lpc.ProductCategoryHashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[HS_ProductCategory_ProductCategory] AS pc
    ON pc.HubHashKey = hpc.HashKey;
GO
'@
)

Write-Transforms $rdvTransformRoot $rdvTransforms
Write-Transforms $bdvTransformRoot $bdvTransforms

Invoke-Product meta-transform-script @('from', 'sql-files', '--manifest', (Join-Path $rdvTransformRoot 'manifest.tsv'), '--new-workspace', $rdvWorkspace, '--report', 'logs\04-rdv-transform-import-report.tsv', '--verbose')
Assert-Workspace $rdvWorkspace

Invoke-Product meta-transform-binding @('bind', '--transform-workspace', $rdvWorkspace, '--source-schema', 'source\AdventureWorks2022\Schema', '--target-schema', 'rdv\AdventureWorksRawVault\Schema', '--execute-system', 'AdventureWorksRawVault', '--new-workspace', $rdvBinding, '--data-type-conversion-workspace', $env:DATA_TYPE_CONVERSION_WORKSPACE)
Assert-Workspace $rdvBinding

Write-Host 'Clearing RDV Product slice tables before load.'
Invoke-Product meta-sql @('execute', '--connection-env', 'AW_RDV_SQL', '--quiet', '--query', @'
DELETE FROM dbo.L_ProductProductSubcategory;
DELETE FROM dbo.L_ProductSubcategoryProductCategory;
DELETE FROM dbo.HS_Product_Product;
DELETE FROM dbo.H_Product;
DELETE FROM dbo.HS_ProductSubcategory_ProductSubcategory;
DELETE FROM dbo.H_ProductSubcategory;
DELETE FROM dbo.HS_ProductCategory_ProductCategory;
DELETE FROM dbo.H_ProductCategory;
'@)

foreach ($transform in $rdvTransforms) {
    $scriptName = 'dbo.v_load_' + ($transform.Target -replace '^dbo\.', '')
    Invoke-Product meta-pipeline @('execute-sqlserver', '--transform-workspace', $rdvWorkspace, '--binding-workspace', $rdvBinding, '--script', $scriptName, '--execution-connection-env', 'AW_RDV_SQL', '--target-connection-env', 'AW_RDV_SQL', '--target', $transform.Target, '--target-data-type-system', 'SqlServer')
}

Write-Host 'RDV Product slice counts:'
Invoke-Sql 'AW_RDV_SQL' @'
SELECT 'H_ProductCategory' AS TableName, COUNT_BIG(*) AS RowTotal FROM dbo.H_ProductCategory
UNION ALL SELECT 'HS_ProductCategory_ProductCategory', COUNT_BIG(*) FROM dbo.HS_ProductCategory_ProductCategory
UNION ALL SELECT 'H_ProductSubcategory', COUNT_BIG(*) FROM dbo.H_ProductSubcategory
UNION ALL SELECT 'HS_ProductSubcategory_ProductSubcategory', COUNT_BIG(*) FROM dbo.HS_ProductSubcategory_ProductSubcategory
UNION ALL SELECT 'H_Product', COUNT_BIG(*) FROM dbo.H_Product
UNION ALL SELECT 'HS_Product_Product', COUNT_BIG(*) FROM dbo.HS_Product_Product
UNION ALL SELECT 'L_ProductSubcategoryProductCategory', COUNT_BIG(*) FROM dbo.L_ProductSubcategoryProductCategory
UNION ALL SELECT 'L_ProductProductSubcategory', COUNT_BIG(*) FROM dbo.L_ProductProductSubcategory;
'@

Invoke-Product meta-transform-script @('from', 'sql-files', '--manifest', (Join-Path $bdvTransformRoot 'manifest.tsv'), '--new-workspace', $bdvWorkspace, '--report', 'logs\04-bdv-transform-import-report.tsv', '--verbose')
Assert-Workspace $bdvWorkspace

Invoke-Product meta-transform-binding @('bind', '--transform-workspace', $bdvWorkspace, '--source-schema', 'rdv\AdventureWorksRawVault\Schema', '--target-schema', 'bdv\AdventureWorksBusinessVault\Schema', '--execute-system', 'AdventureWorksBusinessVault', '--new-workspace', $bdvBinding, '--data-type-conversion-workspace', $env:DATA_TYPE_CONVERSION_WORKSPACE)
Assert-Workspace $bdvBinding

Write-Host 'Clearing BDV Product slice tables before load.'
Invoke-Product meta-sql @('execute', '--connection-env', 'AW_BDV_SQL', '--quiet', '--query', @'
DELETE FROM dbo.BHS_Product_ProductClassification;
DELETE FROM dbo.BH_Product;
'@)

foreach ($transform in $bdvTransforms) {
    $scriptName = 'dbo.v_load_' + ($transform.Target -replace '^dbo\.', '')
    Invoke-Product meta-pipeline @('execute-sqlserver', '--transform-workspace', $bdvWorkspace, '--binding-workspace', $bdvBinding, '--script', $scriptName, '--execution-connection-env', 'AW_BDV_SQL', '--target-connection-env', 'AW_BDV_SQL', '--target', $transform.Target, '--target-data-type-system', 'SqlServer')
}

Write-Host 'BDV Product slice counts:'
Invoke-Sql 'AW_BDV_SQL' @'
SELECT 'BH_Product' AS TableName, COUNT_BIG(*) AS RowTotal FROM dbo.BH_Product
UNION ALL SELECT 'BHS_Product_ProductClassification', COUNT_BIG(*) FROM dbo.BHS_Product_ProductClassification;
'@

Write-Host 'Product vault load slice passed.'
