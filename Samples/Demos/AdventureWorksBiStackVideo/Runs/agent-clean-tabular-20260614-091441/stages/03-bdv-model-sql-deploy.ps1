$ErrorActionPreference = 'Stop'

$bdvWorkspace = 'bdv\AdventureWorksBusinessVault\BusinessVault'
$bdvSqlWorkspace = 'bdv\AdventureWorksBusinessVault\Sql'
$bdvDeployManifest = 'bdv\AdventureWorksBusinessVault\DeployManifest'
$bdvVerifyManifest = 'bdv\AdventureWorksBusinessVault\DeployVerifyManifest'

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

function Count-Entity {
    param(
        [string] $WorkspacePath,
        [string] $EntityName
    )

    $path = Join-Path $WorkspacePath "instances\$EntityName.xml"
    if (!(Test-Path -LiteralPath $path)) {
        return 0
    }

    [xml] $xml = Get-Content -Raw -LiteralPath $path
    return @($xml.SelectNodes("//$EntityName")).Count
}

function Assert-NoWorkspace {
    param([string] $WorkspacePath)

    if (Test-Path -LiteralPath (Join-Path $WorkspacePath 'workspace.xml')) {
        throw "$WorkspacePath already contains workspace.xml. This clean run refuses to overwrite accepted evidence."
    }
}

if (!(Test-Path -LiteralPath 'rdv\AdventureWorksRawVault\DeployVerifyManifest\workspace.xml')) {
    throw 'RDV verify manifest is missing. Run the RDV gate before BDV.'
}

if (!(Test-Path -LiteralPath (Join-Path $env:BDV_IMPLEMENTATION_WORKSPACE 'workspace.xml'))) {
    throw "Implementation workspace was not found at $env:BDV_IMPLEMENTATION_WORKSPACE."
}

Assert-NoWorkspace $bdvWorkspace
Assert-NoWorkspace $bdvSqlWorkspace
Assert-NoWorkspace $bdvDeployManifest
Assert-NoWorkspace $bdvVerifyManifest

Invoke-Product meta-datavault-business @('--new-workspace', $bdvWorkspace)

Invoke-Product meta-datavault-business @('add-hub', '--workspace', $bdvWorkspace, '--id', 'Customer', '--name', 'Customer')
Invoke-Product meta-datavault-business @('add-hub-key-part', '--workspace', $bdvWorkspace, '--id', 'CustomerId', '--hub', 'Customer', '--name', 'CustomerId', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '256')
Invoke-Product meta-datavault-business @('add-hub', '--workspace', $bdvWorkspace, '--id', 'Store', '--name', 'Store')
Invoke-Product meta-datavault-business @('add-hub-key-part', '--workspace', $bdvWorkspace, '--id', 'StoreId', '--hub', 'Store', '--name', 'StoreId', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '256')
Invoke-Product meta-datavault-business @('add-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrder', '--name', 'SalesOrder')
Invoke-Product meta-datavault-business @('add-hub-key-part', '--workspace', $bdvWorkspace, '--id', 'SalesOrderId', '--hub', 'SalesOrder', '--name', 'SalesOrderId', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '256')
Invoke-Product meta-datavault-business @('add-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderLine', '--name', 'SalesOrderLine')
Invoke-Product meta-datavault-business @('add-hub-key-part', '--workspace', $bdvWorkspace, '--id', 'SalesOrderLineOrderId', '--hub', 'SalesOrderLine', '--name', 'SalesOrderId', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '256')
Invoke-Product meta-datavault-business @('add-hub-key-part', '--workspace', $bdvWorkspace, '--id', 'SalesOrderLineDetailId', '--hub', 'SalesOrderLine', '--name', 'SalesOrderDetailId', '--data-type-id', 'meta:type:String', '--ordinal', '2', '--length', '256')
Invoke-Product meta-datavault-business @('add-hub', '--workspace', $bdvWorkspace, '--id', 'Product', '--name', 'Product')
Invoke-Product meta-datavault-business @('add-hub-key-part', '--workspace', $bdvWorkspace, '--id', 'ProductId', '--hub', 'Product', '--name', 'ProductId', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '256')
Invoke-Product meta-datavault-business @('add-hub', '--workspace', $bdvWorkspace, '--id', 'SalesPerson', '--name', 'SalesPerson')
Invoke-Product meta-datavault-business @('add-hub-key-part', '--workspace', $bdvWorkspace, '--id', 'SalesPersonBusinessEntityId', '--hub', 'SalesPerson', '--name', 'SalesPersonBusinessEntityId', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '256')
Invoke-Product meta-datavault-business @('add-hub', '--workspace', $bdvWorkspace, '--id', 'SalesTerritory', '--name', 'SalesTerritory')
Invoke-Product meta-datavault-business @('add-hub-key-part', '--workspace', $bdvWorkspace, '--id', 'SalesTerritoryId', '--hub', 'SalesTerritory', '--name', 'TerritoryId', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '256')
Invoke-Product meta-datavault-business @('add-hub', '--workspace', $bdvWorkspace, '--id', 'OrderDate', '--name', 'OrderDate')
Invoke-Product meta-datavault-business @('add-hub-key-part', '--workspace', $bdvWorkspace, '--id', 'OrderDateValue', '--hub', 'OrderDate', '--name', 'OrderDate', '--data-type-id', 'meta:type:DateTime', '--ordinal', '1')

Invoke-Product meta-datavault-business @('add-hub-satellite', '--workspace', $bdvWorkspace, '--id', 'ProductClassification', '--hub', 'Product', '--name', 'ProductClassification')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'ProductNumber', '--hub-satellite', 'ProductClassification', '--name', 'ProductNumber', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '25')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'ProductName', '--hub-satellite', 'ProductClassification', '--name', 'ProductName', '--data-type-id', 'meta:type:String', '--ordinal', '2', '--length', '100')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'ProductCategory', '--hub-satellite', 'ProductClassification', '--name', 'ProductCategory', '--data-type-id', 'meta:type:String', '--ordinal', '3', '--length', '100')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'ProductSubcategory', '--hub-satellite', 'ProductClassification', '--name', 'ProductSubcategory', '--data-type-id', 'meta:type:String', '--ordinal', '4', '--length', '100')
Invoke-Product meta-datavault-business @('add-hub-satellite', '--workspace', $bdvWorkspace, '--id', 'CustomerProfile', '--hub', 'Customer', '--name', 'CustomerProfile')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'CustomerAccountNumber', '--hub-satellite', 'CustomerProfile', '--name', 'CustomerAccountNumber', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '25')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'CustomerType', '--hub-satellite', 'CustomerProfile', '--name', 'CustomerType', '--data-type-id', 'meta:type:String', '--ordinal', '2', '--length', '40')
Invoke-Product meta-datavault-business @('add-hub-satellite', '--workspace', $bdvWorkspace, '--id', 'StoreProfile', '--hub', 'Store', '--name', 'StoreProfile')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'StoreName', '--hub-satellite', 'StoreProfile', '--name', 'StoreName', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '100')
Invoke-Product meta-datavault-business @('add-hub-satellite', '--workspace', $bdvWorkspace, '--id', 'SalesOrderProfile', '--hub', 'SalesOrder', '--name', 'SalesOrderProfile')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'SalesOrderNumber', '--hub-satellite', 'SalesOrderProfile', '--name', 'SalesOrderNumber', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '25')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'OrderStatus', '--hub-satellite', 'SalesOrderProfile', '--name', 'OrderStatus', '--data-type-id', 'meta:type:String', '--ordinal', '2', '--length', '40')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'OnlineOrderFlag', '--hub-satellite', 'SalesOrderProfile', '--name', 'OnlineOrderFlag', '--data-type-id', 'meta:type:Boolean', '--ordinal', '3')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'DueDate', '--hub-satellite', 'SalesOrderProfile', '--name', 'DueDate', '--data-type-id', 'meta:type:DateTime', '--ordinal', '4')
Invoke-Product meta-datavault-business @('add-hub-satellite', '--workspace', $bdvWorkspace, '--id', 'SalesPersonProfile', '--hub', 'SalesPerson', '--name', 'SalesPersonProfile')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'SalesPersonName', '--hub-satellite', 'SalesPersonProfile', '--name', 'SalesPersonName', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '150')
Invoke-Product meta-datavault-business @('add-hub-satellite', '--workspace', $bdvWorkspace, '--id', 'SalesTerritoryProfile', '--hub', 'SalesTerritory', '--name', 'SalesTerritoryProfile')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'TerritoryName', '--hub-satellite', 'SalesTerritoryProfile', '--name', 'TerritoryName', '--data-type-id', 'meta:type:String', '--ordinal', '1', '--length', '100')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'CountryRegionCode', '--hub-satellite', 'SalesTerritoryProfile', '--name', 'CountryRegionCode', '--data-type-id', 'meta:type:String', '--ordinal', '2', '--length', '10')
Invoke-Product meta-datavault-business @('add-hub-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'TerritoryGroup', '--hub-satellite', 'SalesTerritoryProfile', '--name', 'TerritoryGroup', '--data-type-id', 'meta:type:String', '--ordinal', '3', '--length', '50')

Invoke-Product meta-datavault-business @('add-link', '--workspace', $bdvWorkspace, '--id', 'SalesOrderCustomer', '--name', 'SalesOrderCustomer')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderCustomerOrder', '--link', 'SalesOrderCustomer', '--hub', 'SalesOrder', '--ordinal', '1', '--role-name', 'SalesOrder')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderCustomerCustomer', '--link', 'SalesOrderCustomer', '--hub', 'Customer', '--ordinal', '2', '--role-name', 'Customer')
Invoke-Product meta-datavault-business @('add-link', '--workspace', $bdvWorkspace, '--id', 'SalesOrderStore', '--name', 'SalesOrderStore')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderStoreOrder', '--link', 'SalesOrderStore', '--hub', 'SalesOrder', '--ordinal', '1', '--role-name', 'SalesOrder')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderStoreStore', '--link', 'SalesOrderStore', '--hub', 'Store', '--ordinal', '2', '--role-name', 'Store')
Invoke-Product meta-datavault-business @('add-link', '--workspace', $bdvWorkspace, '--id', 'SalesOrderSalesPerson', '--name', 'SalesOrderSalesPerson')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderSalesPersonOrder', '--link', 'SalesOrderSalesPerson', '--hub', 'SalesOrder', '--ordinal', '1', '--role-name', 'SalesOrder')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderSalesPersonPerson', '--link', 'SalesOrderSalesPerson', '--hub', 'SalesPerson', '--ordinal', '2', '--role-name', 'SalesPerson')
Invoke-Product meta-datavault-business @('add-link', '--workspace', $bdvWorkspace, '--id', 'SalesOrderTerritory', '--name', 'SalesOrderTerritory')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderTerritoryOrder', '--link', 'SalesOrderTerritory', '--hub', 'SalesOrder', '--ordinal', '1', '--role-name', 'SalesOrder')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderTerritoryTerritory', '--link', 'SalesOrderTerritory', '--hub', 'SalesTerritory', '--ordinal', '2', '--role-name', 'SalesTerritory')
Invoke-Product meta-datavault-business @('add-link', '--workspace', $bdvWorkspace, '--id', 'SalesOrderDate', '--name', 'SalesOrderDate')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderDateOrder', '--link', 'SalesOrderDate', '--hub', 'SalesOrder', '--ordinal', '1', '--role-name', 'SalesOrder')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderDateDate', '--link', 'SalesOrderDate', '--hub', 'OrderDate', '--ordinal', '2', '--role-name', 'OrderDate')
Invoke-Product meta-datavault-business @('add-link', '--workspace', $bdvWorkspace, '--id', 'SalesOrderLineOrder', '--name', 'SalesOrderLineOrder')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderLineOrderLine', '--link', 'SalesOrderLineOrder', '--hub', 'SalesOrderLine', '--ordinal', '1', '--role-name', 'SalesOrderLine')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderLineOrderOrder', '--link', 'SalesOrderLineOrder', '--hub', 'SalesOrder', '--ordinal', '2', '--role-name', 'SalesOrder')
Invoke-Product meta-datavault-business @('add-link', '--workspace', $bdvWorkspace, '--id', 'SalesOrderLineProduct', '--name', 'SalesOrderLineProduct')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderLineProductLine', '--link', 'SalesOrderLineProduct', '--hub', 'SalesOrderLine', '--ordinal', '1', '--role-name', 'SalesOrderLine')
Invoke-Product meta-datavault-business @('add-link-hub', '--workspace', $bdvWorkspace, '--id', 'SalesOrderLineProductProduct', '--link', 'SalesOrderLineProduct', '--hub', 'Product', '--ordinal', '2', '--role-name', 'Product')

Invoke-Product meta-datavault-business @('add-link-satellite', '--workspace', $bdvWorkspace, '--id', 'SalesOrderLineMeasures', '--link', 'SalesOrderLineProduct', '--name', 'SalesOrderLineMeasures')
Invoke-Product meta-datavault-business @('add-link-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'OrderQuantity', '--link-satellite', 'SalesOrderLineMeasures', '--name', 'OrderQuantity', '--data-type-id', 'meta:type:Int32', '--ordinal', '1')
Invoke-Product meta-datavault-business @('add-link-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'UnitPrice', '--link-satellite', 'SalesOrderLineMeasures', '--name', 'UnitPrice', '--data-type-id', 'meta:type:Decimal', '--ordinal', '2', '--precision', '19', '--scale', '4')
Invoke-Product meta-datavault-business @('add-link-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'LineTotal', '--link-satellite', 'SalesOrderLineMeasures', '--name', 'LineTotal', '--data-type-id', 'meta:type:Decimal', '--ordinal', '3', '--precision', '38', '--scale', '6')
Invoke-Product meta-datavault-business @('add-link-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'DiscountAmount', '--link-satellite', 'SalesOrderLineMeasures', '--name', 'DiscountAmount', '--data-type-id', 'meta:type:Decimal', '--ordinal', '4', '--precision', '19', '--scale', '4')
Invoke-Product meta-datavault-business @('add-link-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'TaxAmount', '--link-satellite', 'SalesOrderLineMeasures', '--name', 'TaxAmount', '--data-type-id', 'meta:type:Decimal', '--ordinal', '5', '--precision', '19', '--scale', '4')
Invoke-Product meta-datavault-business @('add-link-satellite-attribute', '--workspace', $bdvWorkspace, '--id', 'FreightAmount', '--link-satellite', 'SalesOrderLineMeasures', '--name', 'FreightAmount', '--data-type-id', 'meta:type:Decimal', '--ordinal', '6', '--precision', '19', '--scale', '4')

Assert-Workspace $bdvWorkspace
Write-Host ('BDV model counts: hubs={0}, hub-key-parts={1}, hub-satellites={2}, hub-satellite-attributes={3}, links={4}, link-hubs={5}, link-satellites={6}, link-satellite-attributes={7}' -f
    (Count-Entity $bdvWorkspace 'BusinessHub'),
    (Count-Entity $bdvWorkspace 'BusinessHubKeyPart'),
    (Count-Entity $bdvWorkspace 'BusinessHubSatellite'),
    (Count-Entity $bdvWorkspace 'BusinessHubSatelliteAttribute'),
    (Count-Entity $bdvWorkspace 'BusinessLink'),
    (Count-Entity $bdvWorkspace 'BusinessLinkHub'),
    (Count-Entity $bdvWorkspace 'BusinessLinkSatellite'),
    (Count-Entity $bdvWorkspace 'BusinessLinkSatelliteAttribute'))

Invoke-Product meta-convert @('business-datavault-to-sql', '--workspace', $bdvWorkspace, '--implementation-workspace', $env:BDV_IMPLEMENTATION_WORKSPACE, '--database-name', $env:AW_BDV_DATABASE, '--out', $bdvSqlWorkspace)
Assert-Workspace $bdvSqlWorkspace

Write-Host 'BDV SQL identifier scan:'
$identifierFiles = @('Table.xml', 'TableColumn.xml', 'PrimaryKey.xml', 'ForeignKey.xml')
$violations = @()
foreach ($file in $identifierFiles) {
    $path = Join-Path $bdvSqlWorkspace "instances\$file"
    if (!(Test-Path -LiteralPath $path)) {
        throw "Missing $path."
    }

    [xml] $xml = Get-Content -Raw -LiteralPath $path
    foreach ($node in @($xml.SelectNodes('//*[@Id]'))) {
        $nameNode = $node.SelectSingleNode('Name')
        if ($nameNode -and $nameNode.InnerText.Length -gt 128) {
            $violations += [pscustomobject] @{
                File = $file
                Entity = $node.LocalName
                NameLength = $nameNode.InnerText.Length
                Name = $nameNode.InnerText
            }
        }
    }
}

if ($violations.Count -gt 0) {
    $violations | Format-Table -AutoSize | Out-String -Width 220 | Write-Host
    throw 'BDV SQL identifier length scan failed.'
}

Write-Host 'Identifier scan OK.'
Write-Host ('BDV SQL counts: tables={0}, columns={1}, primary-keys={2}, foreign-keys={3}' -f
    (Count-Entity $bdvSqlWorkspace 'Table'),
    (Count-Entity $bdvSqlWorkspace 'TableColumn'),
    (Count-Entity $bdvSqlWorkspace 'PrimaryKey'),
    (Count-Entity $bdvSqlWorkspace 'ForeignKey'))

Invoke-Product meta-sql @('execute', '--connection-env', 'AW_MASTER_SQL', '--quiet', '--query', "IF DB_ID(N'$env:AW_BDV_DATABASE') IS NOT NULL BEGIN ALTER DATABASE [$env:AW_BDV_DATABASE] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$env:AW_BDV_DATABASE]; END")
Invoke-Product meta-sql @('deploy-plan', '--source-workspace', $bdvSqlWorkspace, '--connection-env', 'AW_BDV_SQL', '--out', $bdvDeployManifest)
Assert-Workspace $bdvDeployManifest
Invoke-Product meta-sql @('deploy', '--manifest-workspace', $bdvDeployManifest, '--source-workspace', $bdvSqlWorkspace, '--connection-env', 'AW_BDV_SQL')
Invoke-Product meta-sql @('deploy-plan', '--source-workspace', $bdvSqlWorkspace, '--connection-env', 'AW_BDV_SQL', '--out', $bdvVerifyManifest)
Assert-Workspace $bdvVerifyManifest

Write-Host 'BDV gate passed. RDV-to-BDV load transforms and binding are the next honest gate.'
