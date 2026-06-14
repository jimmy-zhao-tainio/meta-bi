$ErrorActionPreference = 'Stop'

$rdvTransformRoot = 'rdv\AdventureWorksRawVault\SalesTransforms'
$bdvTransformRoot = 'bdv\AdventureWorksBusinessVault\SalesTransforms'
$rdvWorkspace = Join-Path $rdvTransformRoot 'Workspace'
$bdvWorkspace = Join-Path $bdvTransformRoot 'Workspace'
$rdvBinding = 'rdv\AdventureWorksRawVault\SalesBinding'
$bdvBinding = 'bdv\AdventureWorksBusinessVault\SalesBinding'

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

if (!(Test-Path -LiteralPath 'rdv\AdventureWorksRawVault\Schema\workspace.xml')) {
    throw 'RDV schema workspace is missing. Run the Product RDV/BDV load gate first.'
}

if (!(Test-Path -LiteralPath 'bdv\AdventureWorksBusinessVault\Schema\workspace.xml')) {
    throw 'BDV schema workspace is missing. Run the Product RDV/BDV load gate first.'
}

Reset-Directory $rdvTransformRoot
Reset-Directory $bdvTransformRoot
if (Test-Path -LiteralPath $rdvBinding) { Remove-Item -LiteralPath $rdvBinding -Recurse -Force }
if (Test-Path -LiteralPath $bdvBinding) { Remove-Item -LiteralPath $bdvBinding -Recurse -Force }

$rdvTransforms = @(
    New-Transform '001_load_H_Customer.sql' 'dbo.H_Customer' @'
CREATE VIEW dbo.v_load_H_Customer
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.CustomerID))) AS HashKey,
    CONVERT(nvarchar(256), c.CustomerID) AS CustomerID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Customer') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Customer] AS c;
GO
'@
    New-Transform '002_load_HS_Customer_Customer.sql' 'dbo.HS_Customer_Customer' @'
CREATE VIEW dbo.v_load_HS_Customer_Customer
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.CustomerID))) AS HubHashKey,
    c.AccountNumber,
    c.rowguid,
    c.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        c.AccountNumber,
        CONVERT(nvarchar(36), c.rowguid),
        CONVERT(nvarchar(30), c.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Customer') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Customer] AS c;
GO
'@
    New-Transform '003_load_H_Store.sql' 'dbo.H_Store' @'
CREATE VIEW dbo.v_load_H_Store
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), s.BusinessEntityID))) AS HashKey,
    CONVERT(nvarchar(256), s.BusinessEntityID) AS BusinessEntityID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Store') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Store] AS s;
GO
'@
    New-Transform '004_load_HS_Store_Store.sql' 'dbo.HS_Store_Store' @'
CREATE VIEW dbo.v_load_HS_Store_Store
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), s.BusinessEntityID))) AS HubHashKey,
    s.Name,
    s.Demographics,
    s.rowguid,
    s.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        s.Name,
        CONVERT(nvarchar(max), s.Demographics),
        CONVERT(nvarchar(36), s.rowguid),
        CONVERT(nvarchar(30), s.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Store') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Store] AS s;
GO
'@
    New-Transform '005_load_H_SalesPerson.sql' 'dbo.H_SalesPerson' @'
CREATE VIEW dbo.v_load_H_SalesPerson
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), sp.BusinessEntityID))) AS HashKey,
    CONVERT(nvarchar(256), sp.BusinessEntityID) AS BusinessEntityID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesPerson') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesPerson] AS sp;
GO
'@
    New-Transform '006_load_HS_SalesPerson_SalesPerson.sql' 'dbo.HS_SalesPerson_SalesPerson' @'
CREATE VIEW dbo.v_load_HS_SalesPerson_SalesPerson
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), sp.BusinessEntityID))) AS HubHashKey,
    sp.SalesQuota,
    sp.Bonus,
    sp.CommissionPct,
    sp.SalesYTD,
    sp.SalesLastYear,
    sp.rowguid,
    sp.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        CONVERT(nvarchar(40), sp.SalesQuota),
        CONVERT(nvarchar(40), sp.Bonus),
        CONVERT(nvarchar(40), sp.CommissionPct),
        CONVERT(nvarchar(40), sp.SalesYTD),
        CONVERT(nvarchar(40), sp.SalesLastYear),
        CONVERT(nvarchar(36), sp.rowguid),
        CONVERT(nvarchar(30), sp.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesPerson') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesPerson] AS sp;
GO
'@
    New-Transform '007_load_H_SalesTerritory.sql' 'dbo.H_SalesTerritory' @'
CREATE VIEW dbo.v_load_H_SalesTerritory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), st.TerritoryID))) AS HashKey,
    CONVERT(nvarchar(256), st.TerritoryID) AS TerritoryID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesTerritory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesTerritory] AS st;
GO
'@
    New-Transform '008_load_HS_SalesTerritory_SalesTerritory.sql' 'dbo.HS_SalesTerritory_SalesTerritory' @'
CREATE VIEW dbo.v_load_HS_SalesTerritory_SalesTerritory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), st.TerritoryID))) AS HubHashKey,
    st.Name,
    st.[Group],
    st.SalesYTD,
    st.SalesLastYear,
    st.CostYTD,
    st.CostLastYear,
    st.rowguid,
    st.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        st.Name,
        st.[Group],
        CONVERT(nvarchar(40), st.SalesYTD),
        CONVERT(nvarchar(40), st.SalesLastYear),
        CONVERT(nvarchar(40), st.CostYTD),
        CONVERT(nvarchar(40), st.CostLastYear),
        CONVERT(nvarchar(36), st.rowguid),
        CONVERT(nvarchar(30), st.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesTerritory') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesTerritory] AS st;
GO
'@
    New-Transform '009_load_H_SalesOrderHeader.sql' 'dbo.H_SalesOrderHeader' @'
CREATE VIEW dbo.v_load_H_SalesOrderHeader
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), soh.SalesOrderID))) AS HashKey,
    CONVERT(nvarchar(256), soh.SalesOrderID) AS SalesOrderID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderHeader') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderHeader] AS soh;
GO
'@
    New-Transform '010_load_HS_SalesOrderHeader_SalesOrderHeader.sql' 'dbo.HS_SalesOrderHeader_SalesOrderHeader' @'
CREATE VIEW dbo.v_load_HS_SalesOrderHeader_SalesOrderHeader
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), soh.SalesOrderID))) AS HubHashKey,
    soh.RevisionNumber,
    soh.OrderDate,
    soh.DueDate,
    soh.ShipDate,
    soh.Status,
    soh.OnlineOrderFlag,
    soh.SalesOrderNumber,
    soh.PurchaseOrderNumber,
    soh.AccountNumber,
    soh.CreditCardApprovalCode,
    soh.SubTotal,
    soh.TaxAmt,
    soh.Freight,
    soh.TotalDue,
    soh.Comment,
    soh.rowguid,
    soh.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        CONVERT(nvarchar(20), soh.RevisionNumber),
        CONVERT(nvarchar(30), soh.OrderDate, 126),
        CONVERT(nvarchar(30), soh.DueDate, 126),
        CONVERT(nvarchar(30), soh.ShipDate, 126),
        CONVERT(nvarchar(20), soh.Status),
        CONVERT(nvarchar(1), soh.OnlineOrderFlag),
        soh.SalesOrderNumber,
        soh.PurchaseOrderNumber,
        soh.AccountNumber,
        soh.CreditCardApprovalCode,
        CONVERT(nvarchar(40), soh.SubTotal),
        CONVERT(nvarchar(40), soh.TaxAmt),
        CONVERT(nvarchar(40), soh.Freight),
        CONVERT(nvarchar(40), soh.TotalDue),
        soh.Comment,
        CONVERT(nvarchar(36), soh.rowguid),
        CONVERT(nvarchar(30), soh.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderHeader') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderHeader] AS soh;
GO
'@
    New-Transform '011_load_H_SalesOrderDetail.sql' 'dbo.H_SalesOrderDetail' @'
CREATE VIEW dbo.v_load_H_SalesOrderDetail
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sod.SalesOrderID), N'|', CONVERT(nvarchar(256), sod.SalesOrderDetailID)))) AS HashKey,
    CONVERT(nvarchar(256), sod.SalesOrderID) AS SalesOrderID,
    CONVERT(nvarchar(256), sod.SalesOrderDetailID) AS SalesOrderDetailID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderDetail') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderDetail] AS sod;
GO
'@
    New-Transform '012_load_HS_SalesOrderDetail_SalesOrderDetail.sql' 'dbo.HS_SalesOrderDetail_SalesOrderDetail' @'
CREATE VIEW dbo.v_load_HS_SalesOrderDetail_SalesOrderDetail
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sod.SalesOrderID), N'|', CONVERT(nvarchar(256), sod.SalesOrderDetailID)))) AS HubHashKey,
    sod.CarrierTrackingNumber,
    sod.OrderQty,
    sod.UnitPrice,
    sod.UnitPriceDiscount,
    sod.LineTotal,
    sod.rowguid,
    sod.ModifiedDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        sod.CarrierTrackingNumber,
        CONVERT(nvarchar(20), sod.OrderQty),
        CONVERT(nvarchar(40), sod.UnitPrice),
        CONVERT(nvarchar(40), sod.UnitPriceDiscount),
        CONVERT(nvarchar(40), sod.LineTotal),
        CONVERT(nvarchar(36), sod.rowguid),
        CONVERT(nvarchar(30), sod.ModifiedDate, 126)))) AS HashDiff,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderDetail') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderDetail] AS sod;
GO
'@
    New-Transform '013_load_H_SpecialOfferProduct.sql' 'dbo.H_SpecialOfferProduct' @'
CREATE VIEW dbo.v_load_H_SpecialOfferProduct
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sop.SpecialOfferID), N'|', CONVERT(nvarchar(256), sop.ProductID)))) AS HashKey,
    CONVERT(nvarchar(256), sop.SpecialOfferID) AS SpecialOfferID,
    CONVERT(nvarchar(256), sop.ProductID) AS ProductID,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SpecialOfferProduct') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SpecialOfferProduct] AS sop;
GO
'@
    New-Transform '014_load_L_CustomerStore.sql' 'dbo.L_CustomerStore' @'
CREATE VIEW dbo.v_load_L_CustomerStore
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), c.CustomerID), N'|', CONVERT(nvarchar(256), c.StoreID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.CustomerID))) AS CustomerHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.StoreID))) AS StoreHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Customer') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Customer] AS c
WHERE c.StoreID IS NOT NULL;
GO
'@
    New-Transform '015_load_L_CustomerSalesTerritory.sql' 'dbo.L_CustomerSalesTerritory' @'
CREATE VIEW dbo.v_load_L_CustomerSalesTerritory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), c.CustomerID), N'|', CONVERT(nvarchar(256), c.TerritoryID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.CustomerID))) AS CustomerHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), c.TerritoryID))) AS SalesTerritoryHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.Customer') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[Customer] AS c
WHERE c.TerritoryID IS NOT NULL;
GO
'@
    New-Transform '016_load_L_SalesOrderHeaderCustomer.sql' 'dbo.L_SalesOrderHeaderCustomer' @'
CREATE VIEW dbo.v_load_L_SalesOrderHeaderCustomer
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), soh.SalesOrderID), N'|', CONVERT(nvarchar(256), soh.CustomerID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), soh.SalesOrderID))) AS SalesOrderHeaderHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), soh.CustomerID))) AS CustomerHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderHeader') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderHeader] AS soh;
GO
'@
    New-Transform '017_load_L_SalesOrderHeaderSalesPerson.sql' 'dbo.L_SalesOrderHeaderSalesPerson' @'
CREATE VIEW dbo.v_load_L_SalesOrderHeaderSalesPerson
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), soh.SalesOrderID), N'|', CONVERT(nvarchar(256), soh.SalesPersonID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), soh.SalesOrderID))) AS SalesOrderHeaderHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), soh.SalesPersonID))) AS SalesPersonHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderHeader') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderHeader] AS soh
WHERE soh.SalesPersonID IS NOT NULL;
GO
'@
    New-Transform '018_load_L_SalesOrderHeaderSalesTerritory.sql' 'dbo.L_SalesOrderHeaderSalesTerritory' @'
CREATE VIEW dbo.v_load_L_SalesOrderHeaderSalesTerritory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), soh.SalesOrderID), N'|', CONVERT(nvarchar(256), soh.TerritoryID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), soh.SalesOrderID))) AS SalesOrderHeaderHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), soh.TerritoryID))) AS SalesTerritoryHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderHeader') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderHeader] AS soh
WHERE soh.TerritoryID IS NOT NULL;
GO
'@
    New-Transform '019_load_L_SalesOrderDetailSalesOrderHeader.sql' 'dbo.L_SalesOrderDetailSalesOrderHeader' @'
CREATE VIEW dbo.v_load_L_SalesOrderDetailSalesOrderHeader
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sod.SalesOrderID), N'|', CONVERT(nvarchar(256), sod.SalesOrderDetailID), N'|', CONVERT(nvarchar(256), sod.SalesOrderID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sod.SalesOrderID), N'|', CONVERT(nvarchar(256), sod.SalesOrderDetailID)))) AS SalesOrderDetailHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), sod.SalesOrderID))) AS SalesOrderHeaderHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderDetail') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderDetail] AS sod;
GO
'@
    New-Transform '020_load_L_SalesOrderDetailSpecialOfferProduct.sql' 'dbo.L_SalesOrderDetailSpecialOfferProduct' @'
CREATE VIEW dbo.v_load_L_SalesOrderDetailSpecialOfferProduct
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sod.SalesOrderID), N'|', CONVERT(nvarchar(256), sod.SalesOrderDetailID), N'|', CONVERT(nvarchar(256), sod.SpecialOfferID), N'|', CONVERT(nvarchar(256), sod.ProductID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sod.SalesOrderID), N'|', CONVERT(nvarchar(256), sod.SalesOrderDetailID)))) AS SalesOrderDetailHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sod.SpecialOfferID), N'|', CONVERT(nvarchar(256), sod.ProductID)))) AS SpecialOfferProductHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SalesOrderDetail') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SalesOrderDetail] AS sod;
GO
'@
    New-Transform '021_load_L_SpecialOfferProductProduct.sql' 'dbo.L_SpecialOfferProductProduct' @'
CREATE VIEW dbo.v_load_L_SpecialOfferProductProduct
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sop.SpecialOfferID), N'|', CONVERT(nvarchar(256), sop.ProductID), N'|', CONVERT(nvarchar(256), sop.ProductID)))) AS HashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(256), sop.SpecialOfferID), N'|', CONVERT(nvarchar(256), sop.ProductID)))) AS SpecialOfferProductHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(256), sop.ProductID))) AS ProductHashKey,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorks2022.Sales.SpecialOfferProduct') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorks2022].[Sales].[SpecialOfferProduct] AS sop;
GO
'@
)

$bdvTransforms = @(
    New-Transform '001_load_BH_Customer.sql' 'dbo.BH_Customer' @'
CREATE VIEW dbo.v_load_BH_Customer
AS
SELECT
    h.HashKey,
    h.CustomerID AS CustomerId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_Customer') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Customer] AS h;
GO
'@
    New-Transform '002_load_BHS_Customer_CustomerProfile.sql' 'dbo.BHS_Customer_CustomerProfile' @'
CREATE VIEW dbo.v_load_BHS_Customer_CustomerProfile
AS
SELECT
    h.HashKey AS HubHashKey,
    hs.AccountNumber AS CustomerAccountNumber,
    CONVERT(nvarchar(40), CASE WHEN lcs.CustomerHashKey IS NULL THEN N'Individual' ELSE N'Store' END) AS CustomerType,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        hs.AccountNumber,
        CASE WHEN lcs.CustomerHashKey IS NULL THEN N'Individual' ELSE N'Store' END))) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.CustomerProfile') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Customer] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_Customer_Customer] AS hs
    ON hs.HubHashKey = h.HashKey
LEFT JOIN [AdventureWorksRawVault].[dbo].[L_CustomerStore] AS lcs
    ON lcs.CustomerHashKey = h.HashKey;
GO
'@
    New-Transform '003_load_BH_Store.sql' 'dbo.BH_Store' @'
CREATE VIEW dbo.v_load_BH_Store
AS
SELECT
    h.HashKey,
    h.BusinessEntityID AS StoreId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_Store') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Store] AS h;
GO
'@
    New-Transform '004_load_BHS_Store_StoreProfile.sql' 'dbo.BHS_Store_StoreProfile' @'
CREATE VIEW dbo.v_load_BHS_Store_StoreProfile
AS
SELECT
    h.HashKey AS HubHashKey,
    hs.Name AS StoreName,
    CONVERT(binary(32), HASHBYTES('SHA2_256', hs.Name)) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.StoreProfile') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_Store] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_Store_Store] AS hs
    ON hs.HubHashKey = h.HashKey;
GO
'@
    New-Transform '005_load_BH_SalesPerson.sql' 'dbo.BH_SalesPerson' @'
CREATE VIEW dbo.v_load_BH_SalesPerson
AS
SELECT
    h.HashKey,
    h.BusinessEntityID AS SalesPersonBusinessEntityId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_SalesPerson') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesPerson] AS h;
GO
'@
    New-Transform '006_load_BHS_SalesPerson_SalesPersonProfile.sql' 'dbo.BHS_SalesPerson_SalesPersonProfile' @'
CREATE VIEW dbo.v_load_BHS_SalesPerson_SalesPersonProfile
AS
SELECT
    h.HashKey AS HubHashKey,
    CONVERT(nvarchar(150), CONCAT(N'Salesperson ', h.BusinessEntityID)) AS SalesPersonName,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT(N'Salesperson ', h.BusinessEntityID))) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesPersonProfile') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesPerson] AS h;
GO
'@
    New-Transform '007_load_BH_SalesTerritory.sql' 'dbo.BH_SalesTerritory' @'
CREATE VIEW dbo.v_load_BH_SalesTerritory
AS
SELECT
    h.HashKey,
    h.TerritoryID AS TerritoryId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_SalesTerritory') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesTerritory] AS h;
GO
'@
    New-Transform '008_load_BHS_SalesTerritory_SalesTerritoryProfile.sql' 'dbo.BHS_SalesTerritory_SalesTerritoryProfile' @'
CREATE VIEW dbo.v_load_BHS_SalesTerritory_SalesTerritoryProfile
AS
SELECT
    h.HashKey AS HubHashKey,
    hs.Name AS TerritoryName,
    CONVERT(nvarchar(10), N'Unknown') AS CountryRegionCode,
    hs.[Group] AS TerritoryGroup,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|', hs.Name, N'Unknown', hs.[Group]))) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesTerritoryProfile') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesTerritory] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_SalesTerritory_SalesTerritory] AS hs
    ON hs.HubHashKey = h.HashKey;
GO
'@
    New-Transform '009_load_BH_SalesOrder.sql' 'dbo.BH_SalesOrder' @'
CREATE VIEW dbo.v_load_BH_SalesOrder
AS
SELECT
    h.HashKey,
    h.SalesOrderID AS SalesOrderId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_SalesOrderHeader') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesOrderHeader] AS h;
GO
'@
    New-Transform '010_load_BHS_SalesOrder_SalesOrderProfile.sql' 'dbo.BHS_SalesOrder_SalesOrderProfile' @'
CREATE VIEW dbo.v_load_BHS_SalesOrder_SalesOrderProfile
AS
SELECT
    h.HashKey AS HubHashKey,
    hs.SalesOrderNumber,
    CONVERT(nvarchar(40), hs.Status) AS OrderStatus,
    hs.OnlineOrderFlag,
    CONVERT(date, hs.DueDate) AS DueDate,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        hs.SalesOrderNumber,
        CONVERT(nvarchar(40), hs.Status),
        CONVERT(nvarchar(1), hs.OnlineOrderFlag),
        CONVERT(nvarchar(30), hs.DueDate, 126)))) AS HashDiff,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderProfile') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesOrderHeader] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] AS hs
    ON hs.HubHashKey = h.HashKey;
GO
'@
    New-Transform '011_load_BH_SalesOrderLine.sql' 'dbo.BH_SalesOrderLine' @'
CREATE VIEW dbo.v_load_BH_SalesOrderLine
AS
SELECT
    h.HashKey,
    h.SalesOrderID AS SalesOrderId,
    h.SalesOrderDetailID AS SalesOrderDetailId,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.H_SalesOrderDetail') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesOrderDetail] AS h;
GO
'@
    New-Transform '012_load_BH_OrderDate.sql' 'dbo.BH_OrderDate' @'
CREATE VIEW dbo.v_load_BH_OrderDate
AS
SELECT DISTINCT
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(10), CONVERT(date, hs.OrderDate), 23))) AS HashKey,
    CONVERT(date, hs.OrderDate) AS OrderDate,
    CONVERT(datetime2, SYSUTCDATETIME()) AS LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderHeader.OrderDate') AS RecordSource,
    CONVERT(bigint, 0) AS AuditId
FROM [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] AS hs;
GO
'@
    New-Transform '013_load_BL_SalesOrderCustomer.sql' 'dbo.BL_SalesOrderCustomer' @'
CREATE VIEW dbo.v_load_BL_SalesOrderCustomer
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), l.SalesOrderHeaderHashKey, 2), N'|', CONVERT(nvarchar(32), l.CustomerHashKey, 2)))) AS HashKey,
    l.SalesOrderHeaderHashKey AS SalesOrderHashKey,
    l.CustomerHashKey,
    l.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.L_SalesOrderHeaderCustomer') AS RecordSource,
    l.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderCustomer] AS l;
GO
'@
    New-Transform '014_load_BL_SalesOrderStore.sql' 'dbo.BL_SalesOrderStore' @'
CREATE VIEW dbo.v_load_BL_SalesOrderStore
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), loc.SalesOrderHeaderHashKey, 2), N'|', CONVERT(nvarchar(32), lcs.StoreHashKey, 2)))) AS HashKey,
    loc.SalesOrderHeaderHashKey AS SalesOrderHashKey,
    lcs.StoreHashKey,
    loc.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderStore') AS RecordSource,
    loc.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderCustomer] AS loc
INNER JOIN [AdventureWorksRawVault].[dbo].[L_CustomerStore] AS lcs
    ON lcs.CustomerHashKey = loc.CustomerHashKey;
GO
'@
    New-Transform '015_load_BL_SalesOrderSalesPerson.sql' 'dbo.BL_SalesOrderSalesPerson' @'
CREATE VIEW dbo.v_load_BL_SalesOrderSalesPerson
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), l.SalesOrderHeaderHashKey, 2), N'|', CONVERT(nvarchar(32), l.SalesPersonHashKey, 2)))) AS HashKey,
    l.SalesOrderHeaderHashKey AS SalesOrderHashKey,
    l.SalesPersonHashKey,
    l.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.L_SalesOrderHeaderSalesPerson') AS RecordSource,
    l.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderSalesPerson] AS l;
GO
'@
    New-Transform '016_load_BL_SalesOrderTerritory.sql' 'dbo.BL_SalesOrderTerritory' @'
CREATE VIEW dbo.v_load_BL_SalesOrderTerritory
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), l.SalesOrderHeaderHashKey, 2), N'|', CONVERT(nvarchar(32), l.SalesTerritoryHashKey, 2)))) AS HashKey,
    l.SalesOrderHeaderHashKey AS SalesOrderHashKey,
    l.SalesTerritoryHashKey,
    l.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.L_SalesOrderHeaderSalesTerritory') AS RecordSource,
    l.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderHeaderSalesTerritory] AS l;
GO
'@
    New-Transform '017_load_BL_SalesOrderDate.sql' 'dbo.BL_SalesOrderDate' @'
CREATE VIEW dbo.v_load_BL_SalesOrderDate
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), h.HashKey, 2), N'|', CONVERT(nvarchar(32), CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(10), CONVERT(date, hs.OrderDate), 23))), 2)))) AS HashKey,
    h.HashKey AS SalesOrderHashKey,
    CONVERT(binary(16), HASHBYTES('MD5', CONVERT(nvarchar(10), CONVERT(date, hs.OrderDate), 23))) AS OrderDateHashKey,
    h.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderDate') AS RecordSource,
    h.AuditId
FROM [AdventureWorksRawVault].[dbo].[H_SalesOrderHeader] AS h
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] AS hs
    ON hs.HubHashKey = h.HashKey;
GO
'@
    New-Transform '018_load_BL_SalesOrderLineOrder.sql' 'dbo.BL_SalesOrderLineOrder' @'
CREATE VIEW dbo.v_load_BL_SalesOrderLineOrder
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), l.SalesOrderDetailHashKey, 2), N'|', CONVERT(nvarchar(32), l.SalesOrderHeaderHashKey, 2)))) AS HashKey,
    l.SalesOrderDetailHashKey AS SalesOrderLineHashKey,
    l.SalesOrderHeaderHashKey AS SalesOrderHashKey,
    l.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.L_SalesOrderDetailSalesOrderHeader') AS RecordSource,
    l.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSalesOrderHeader] AS l;
GO
'@
    New-Transform '019_load_BL_SalesOrderLineProduct.sql' 'dbo.BL_SalesOrderLineProduct' @'
CREATE VIEW dbo.v_load_BL_SalesOrderLineProduct
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), lsp.SalesOrderDetailHashKey, 2), N'|', CONVERT(nvarchar(32), spp.ProductHashKey, 2)))) AS HashKey,
    lsp.SalesOrderDetailHashKey AS SalesOrderLineHashKey,
    spp.ProductHashKey,
    lsp.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderLineProduct') AS RecordSource,
    lsp.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSpecialOfferProduct] AS lsp
INNER JOIN [AdventureWorksRawVault].[dbo].[L_SpecialOfferProductProduct] AS spp
    ON spp.SpecialOfferProductHashKey = lsp.SpecialOfferProductHashKey;
GO
'@
    New-Transform '020_load_BLS_SalesOrderLineProduct_SalesOrderLineMeasures.sql' 'dbo.BLS_SalesOrderLineProduct_SalesOrderLineMeasures' @'
CREATE VIEW dbo.v_load_BLS_SalesOrderLineProduct_SalesOrderLineMeasures
AS
SELECT
    CONVERT(binary(16), HASHBYTES('MD5', CONCAT(CONVERT(nvarchar(32), lsp.SalesOrderDetailHashKey, 2), N'|', CONVERT(nvarchar(32), spp.ProductHashKey, 2)))) AS LinkHashKey,
    CONVERT(int, hsod.OrderQty) AS OrderQuantity,
    CONVERT(decimal(19, 4), hsod.UnitPrice) AS UnitPrice,
    CONVERT(decimal(38, 6), hsod.LineTotal) AS LineTotal,
    CONVERT(decimal(19, 4), hsod.UnitPrice * hsod.OrderQty * hsod.UnitPriceDiscount) AS DiscountAmount,
    CONVERT(decimal(19, 4), CASE WHEN hsoh.SubTotal = 0 THEN 0 ELSE hsoh.TaxAmt * (hsod.LineTotal / hsoh.SubTotal) END) AS TaxAmount,
    CONVERT(decimal(19, 4), CASE WHEN hsoh.SubTotal = 0 THEN 0 ELSE hsoh.Freight * (hsod.LineTotal / hsoh.SubTotal) END) AS FreightAmount,
    CONVERT(binary(32), HASHBYTES('SHA2_256', CONCAT_WS(N'|',
        CONVERT(nvarchar(20), hsod.OrderQty),
        CONVERT(nvarchar(40), hsod.UnitPrice),
        CONVERT(nvarchar(40), hsod.LineTotal),
        CONVERT(nvarchar(40), hsod.UnitPrice * hsod.OrderQty * hsod.UnitPriceDiscount),
        CONVERT(nvarchar(40), CASE WHEN hsoh.SubTotal = 0 THEN 0 ELSE hsoh.TaxAmt * (hsod.LineTotal / hsoh.SubTotal) END),
        CONVERT(nvarchar(40), CASE WHEN hsoh.SubTotal = 0 THEN 0 ELSE hsoh.Freight * (hsod.LineTotal / hsoh.SubTotal) END)))) AS HashDiff,
    lsp.LoadTimestamp,
    CONVERT(nvarchar(256), N'AdventureWorksRawVault.SalesOrderLineMeasures') AS RecordSource,
    lsp.AuditId
FROM [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSpecialOfferProduct] AS lsp
INNER JOIN [AdventureWorksRawVault].[dbo].[L_SpecialOfferProductProduct] AS spp
    ON spp.SpecialOfferProductHashKey = lsp.SpecialOfferProductHashKey
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_SalesOrderDetail_SalesOrderDetail] AS hsod
    ON hsod.HubHashKey = lsp.SalesOrderDetailHashKey
INNER JOIN [AdventureWorksRawVault].[dbo].[L_SalesOrderDetailSalesOrderHeader] AS lso
    ON lso.SalesOrderDetailHashKey = lsp.SalesOrderDetailHashKey
INNER JOIN [AdventureWorksRawVault].[dbo].[HS_SalesOrderHeader_SalesOrderHeader] AS hsoh
    ON hsoh.HubHashKey = lso.SalesOrderHeaderHashKey;
GO
'@
)

Write-Transforms $rdvTransformRoot $rdvTransforms
Write-Transforms $bdvTransformRoot $bdvTransforms

Invoke-Product meta-transform-script @('from', 'sql-files', '--manifest', (Join-Path $rdvTransformRoot 'manifest.tsv'), '--new-workspace', $rdvWorkspace, '--report', 'logs\05-rdv-sales-transform-import-report.tsv', '--verbose')
Assert-Workspace $rdvWorkspace

Invoke-Product meta-transform-binding @('bind', '--transform-workspace', $rdvWorkspace, '--source-schema', 'source\AdventureWorks2022\Schema', '--target-schema', 'rdv\AdventureWorksRawVault\Schema', '--execute-system', 'AdventureWorksRawVault', '--new-workspace', $rdvBinding, '--data-type-conversion-workspace', $env:DATA_TYPE_CONVERSION_WORKSPACE)
Assert-Workspace $rdvBinding

$rdvCleanup = @'
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
Write-Host 'Clearing RDV Sales slice tables before load.'
Invoke-Product meta-sql @('execute', '--connection-env', 'AW_RDV_SQL', '--quiet', '--query', $rdvCleanup)

foreach ($transform in $rdvTransforms) {
    $scriptName = 'dbo.v_load_' + ($transform.Target -replace '^dbo\.', '')
    Invoke-Product meta-pipeline @('execute-sqlserver', '--transform-workspace', $rdvWorkspace, '--binding-workspace', $rdvBinding, '--script', $scriptName, '--execution-connection-env', 'AW_RDV_SQL', '--target-connection-env', 'AW_RDV_SQL', '--target', $transform.Target, '--target-data-type-system', 'SqlServer')
}

Write-Host 'RDV Sales slice counts:'
Invoke-Sql 'AW_RDV_SQL' @'
SELECT 'H_Customer' AS TableName, COUNT_BIG(*) AS RowTotal FROM dbo.H_Customer
UNION ALL SELECT 'H_Store', COUNT_BIG(*) FROM dbo.H_Store
UNION ALL SELECT 'H_SalesPerson', COUNT_BIG(*) FROM dbo.H_SalesPerson
UNION ALL SELECT 'H_SalesTerritory', COUNT_BIG(*) FROM dbo.H_SalesTerritory
UNION ALL SELECT 'H_SalesOrderHeader', COUNT_BIG(*) FROM dbo.H_SalesOrderHeader
UNION ALL SELECT 'H_SalesOrderDetail', COUNT_BIG(*) FROM dbo.H_SalesOrderDetail
UNION ALL SELECT 'H_SpecialOfferProduct', COUNT_BIG(*) FROM dbo.H_SpecialOfferProduct
UNION ALL SELECT 'L_SalesOrderDetailSalesOrderHeader', COUNT_BIG(*) FROM dbo.L_SalesOrderDetailSalesOrderHeader
UNION ALL SELECT 'L_SalesOrderDetailSpecialOfferProduct', COUNT_BIG(*) FROM dbo.L_SalesOrderDetailSpecialOfferProduct;
'@

Invoke-Product meta-transform-script @('from', 'sql-files', '--manifest', (Join-Path $bdvTransformRoot 'manifest.tsv'), '--new-workspace', $bdvWorkspace, '--report', 'logs\05-bdv-sales-transform-import-report.tsv', '--verbose')
Assert-Workspace $bdvWorkspace

Invoke-Product meta-transform-binding @('bind', '--transform-workspace', $bdvWorkspace, '--source-schema', 'rdv\AdventureWorksRawVault\Schema', '--target-schema', 'bdv\AdventureWorksBusinessVault\Schema', '--execute-system', 'AdventureWorksBusinessVault', '--new-workspace', $bdvBinding, '--data-type-conversion-workspace', $env:DATA_TYPE_CONVERSION_WORKSPACE)
Assert-Workspace $bdvBinding

$bdvCleanup = @'
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
Write-Host 'Clearing BDV Sales slice tables before load.'
Invoke-Product meta-sql @('execute', '--connection-env', 'AW_BDV_SQL', '--quiet', '--query', $bdvCleanup)

foreach ($transform in $bdvTransforms) {
    $scriptName = 'dbo.v_load_' + ($transform.Target -replace '^dbo\.', '')
    Invoke-Product meta-pipeline @('execute-sqlserver', '--transform-workspace', $bdvWorkspace, '--binding-workspace', $bdvBinding, '--script', $scriptName, '--execution-connection-env', 'AW_BDV_SQL', '--target-connection-env', 'AW_BDV_SQL', '--target', $transform.Target, '--target-data-type-system', 'SqlServer')
}

Write-Host 'BDV Sales slice counts:'
Invoke-Sql 'AW_BDV_SQL' @'
SELECT 'BH_Customer' AS TableName, COUNT_BIG(*) AS RowTotal FROM dbo.BH_Customer
UNION ALL SELECT 'BH_Store', COUNT_BIG(*) FROM dbo.BH_Store
UNION ALL SELECT 'BH_SalesPerson', COUNT_BIG(*) FROM dbo.BH_SalesPerson
UNION ALL SELECT 'BH_SalesTerritory', COUNT_BIG(*) FROM dbo.BH_SalesTerritory
UNION ALL SELECT 'BH_SalesOrder', COUNT_BIG(*) FROM dbo.BH_SalesOrder
UNION ALL SELECT 'BH_SalesOrderLine', COUNT_BIG(*) FROM dbo.BH_SalesOrderLine
UNION ALL SELECT 'BH_OrderDate', COUNT_BIG(*) FROM dbo.BH_OrderDate
UNION ALL SELECT 'BL_SalesOrderLineProduct', COUNT_BIG(*) FROM dbo.BL_SalesOrderLineProduct
UNION ALL SELECT 'BLS_SalesOrderLineProduct_SalesOrderLineMeasures', COUNT_BIG(*) FROM dbo.BLS_SalesOrderLineProduct_SalesOrderLineMeasures;
'@

Write-Host 'Sales vault load slice passed.'
