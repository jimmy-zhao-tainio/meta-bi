$ErrorActionPreference = 'Stop'

$analyticsRoot = 'analytics'
$analyticsWorkspace = Join-Path $analyticsRoot 'Analytics'
$tabularWorkspace = Join-Path $analyticsRoot 'Tabular'
$proofRoot = Join-Path $analyticsRoot 'TabularProof'
$modelId = 'AdventureWorksSales'
$dataSourceId = 'Mart'

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

function Invoke-ProductWithOutput {
    param(
        [Parameter(Mandatory = $true)]
        [string] $FileName,

        [Parameter(Mandatory = $true)]
        [string[]] $Arguments,

        [Parameter(Mandatory = $true)]
        [string] $OutputPath
    )

    Write-Host ($FileName + ' ' + ($Arguments -join ' '))
    & $FileName @Arguments 2>&1 | Tee-Object -FilePath $OutputPath
    if ($LASTEXITCODE -ne 0) {
        throw "$FileName failed with exit code $LASTEXITCODE."
    }
}

function Reset-Directory {
    param([string] $Path)

    if (Test-Path -LiteralPath $Path) {
        Remove-Item -LiteralPath $Path -Recurse -Force
    }

    New-Item -ItemType Directory -Force -Path $Path | Out-Null
}

function Assert-Workspace {
    param([string] $WorkspacePath)

    if (!(Test-Path -LiteralPath (Join-Path $WorkspacePath 'workspace.xml'))) {
        throw "$WorkspacePath\workspace.xml was not created."
    }
}

function Invoke-Sql {
    param([string] $ConnectionEnv, [string] $Query)

    Invoke-Product meta-sql @('execute', '--connection-env', $ConnectionEnv, '--quiet', '--query', $Query)
}

function Escape-SqlLiteral {
    param([string] $Value)

    return $Value.Replace("'", "''")
}

function Quote-SqlIdentifier {
    param([string] $Value)

    return '[' + $Value.Replace(']', ']]') + ']'
}

function Get-TabularServiceLogin {
    $server = $env:AW_TABULAR_SERVER
    if ([string]::IsNullOrWhiteSpace($server)) {
        return $null
    }

    if ($server -match '\\([^\\]+)$') {
        return "NT Service\MSOLAP`$$($Matches[1])"
    }

    return 'NT Service\MSSQLServerOLAPService'
}

function Grant-TabularReadAccess {
    $login = Get-TabularServiceLogin
    if ([string]::IsNullOrWhiteSpace($login)) {
        Write-Host 'No Tabular service login could be inferred from AW_TABULAR_SERVER.'
        return
    }

    $loginLiteral = Escape-SqlLiteral $login
    $loginIdentifier = Quote-SqlIdentifier $login

    Write-Host "Granting mart read access to inferred Tabular service login: $login"

    Invoke-Sql 'AW_MASTER_SQL' @"
BEGIN TRY
    IF SUSER_ID(N'$loginLiteral') IS NULL
        CREATE LOGIN $loginIdentifier FROM WINDOWS;
END TRY
BEGIN CATCH
    PRINT CONCAT('Could not create Tabular service login ${loginLiteral}: ', ERROR_MESSAGE());
END CATCH;
"@

    Invoke-Sql 'AW_TARGET_SQL' @"
IF SUSER_ID(N'$loginLiteral') IS NOT NULL
BEGIN
    IF USER_ID(N'$loginLiteral') IS NULL
        CREATE USER $loginIdentifier FOR LOGIN $loginIdentifier;

    IF ISNULL(IS_ROLEMEMBER(N'db_datareader', N'$loginLiteral'), 0) <> 1
        ALTER ROLE db_datareader ADD MEMBER $loginIdentifier;
END;
"@
}

function Add-AnalyticsTable {
    param([string] $Id, [string] $Name, [string] $Kind, [string] $DataCategory = '')

    $args = @(
        'add-table',
        '--workspace', $analyticsWorkspace,
        '--id', $Id,
        '--model', $modelId,
        '--name', $Name,
        '--kind', $Kind
    )

    if (![string]::IsNullOrWhiteSpace($DataCategory)) {
        $args += @('--data-category', $DataCategory)
    }

    Invoke-Product meta-analytics $args
}

function Add-AnalyticsAttribute {
    param(
        [string] $TableId,
        [string] $Id,
        [string] $Name,
        [string] $SourceName,
        [string] $DataTypeId,
        [int] $Ordinal,
        [bool] $IsKey = $false,
        [bool] $IsHidden = $false,
        [string] $SummarizeBy = ''
    )

    $args = @(
        'add-attribute',
        '--workspace', $analyticsWorkspace,
        '--id', $Id,
        '--table', $TableId,
        '--name', $Name,
        '--source-name', $SourceName,
        '--data-type-id', $DataTypeId,
        '--ordinal', [string]$Ordinal
    )

    if ($IsKey) {
        $args += @('--is-key', 'true')
    }

    if ($IsHidden) {
        $args += @('--is-hidden', 'true')
    }

    if (![string]::IsNullOrWhiteSpace($SummarizeBy)) {
        $args += @('--summarize-by', $SummarizeBy)
    }

    Invoke-Product meta-analytics $args
}

function Add-AnalyticsMeasure {
    param(
        [string] $Id,
        [string] $TableId,
        [string] $SourceAttributeId,
        [string] $Name,
        [string] $DataTypeId,
        [string] $Function,
        [string] $FormatString = ''
    )

    $args = @(
        'add-measure',
        '--workspace', $analyticsWorkspace,
        '--id', $Id,
        '--table', $TableId,
        '--source-attribute', $SourceAttributeId,
        '--name', $Name,
        '--data-type-id', $DataTypeId
    )

    if (![string]::IsNullOrWhiteSpace($FormatString)) {
        $args += @('--format-string', $FormatString)
    }

    Invoke-Product meta-analytics $args
    Invoke-Product meta-analytics @(
        'add-aggregation-behavior',
        '--workspace', $analyticsWorkspace,
        '--id', "Aggregation_$Id",
        '--measure', $Id,
        '--function', $Function)
}

function Add-AnalyticsRelationship {
    param(
        [string] $Id,
        [string] $Name,
        [string] $FromTable,
        [string] $FromAttribute,
        [string] $ToTable,
        [string] $ToAttribute,
        [bool] $IsRequired = $true
    )

    Invoke-Product meta-analytics @(
        'add-relationship',
        '--workspace', $analyticsWorkspace,
        '--id', $Id,
        '--name', $Name,
        '--relationship-kind', 'Regular',
        '--cardinality', 'ManyToOne',
        '--cross-filter-direction', 'Single',
        '--is-active', 'true',
        '--is-required', ([string]$IsRequired).ToLowerInvariant(),
        '--from-table', $FromTable,
        '--from-attribute', $FromAttribute,
        '--to-table', $ToTable,
        '--to-attribute', $ToAttribute)
}

function Add-TabularPartition {
    param(
        [string] $Id,
        [string] $TableId,
        [string] $Name,
        [int] $Ordinal,
        [string] $Expression
    )

    Invoke-Product meta-tabular @(
        'add-tabular-partition',
        '--workspace', $tabularWorkspace,
        '--id', $Id,
        '--tabular-table', $TableId,
        '--tabular-data-source', $dataSourceId,
        '--name', $Name,
        '--ordinal', [string]$Ordinal,
        '--mode', 'Import',
        '--expression', $Expression.Trim())
}

function Write-ProofProject {
    Reset-Directory $proofRoot

    Set-Content -LiteralPath (Join-Path $proofRoot 'TabularProof.csproj') -Encoding ASCII -Value @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AnalysisServices.AdomdClient.NetCore.retail.amd64" Version="19.84.1" />
    <PackageReference Include="Microsoft.Identity.Client" Version="4.84.1" />
  </ItemGroup>
</Project>
'@

    Set-Content -LiteralPath (Join-Path $proofRoot 'Program.cs') -Encoding ASCII -Value @'
using System.Globalization;
using Microsoft.AnalysisServices.AdomdClient;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: TabularProof <server> <database>");
    return 2;
}

var server = args[0];
var database = args[1];
var dax = """
EVALUATE
ROW(
    "FactRows", COUNTROWS('Fact Sales Order Line'),
    "SalesAmount", [Sales Amount],
    "OrderQuantity", [Order Quantity]
)
""";

using var connection = new AdomdConnection($"Data Source={server};Catalog={database};");
connection.Open();

using var command = connection.CreateCommand();
command.CommandText = dax;

using var reader = command.ExecuteReader();
if (!reader.Read())
{
    throw new InvalidOperationException("The Tabular DAX proof returned no rows.");
}

var factRows = Convert.ToInt64(reader.GetValue(0), CultureInfo.InvariantCulture);
var salesAmount = Convert.ToDecimal(reader.GetValue(1), CultureInfo.InvariantCulture);
var orderQuantity = Convert.ToInt64(reader.GetValue(2), CultureInfo.InvariantCulture);

Console.WriteLine($"FactRows={factRows.ToString(CultureInfo.InvariantCulture)}");
Console.WriteLine($"SalesAmount={salesAmount.ToString(CultureInfo.InvariantCulture)}");
Console.WriteLine($"OrderQuantity={orderQuantity.ToString(CultureInfo.InvariantCulture)}");

if (factRows <= 0)
{
    throw new InvalidOperationException("The Tabular fact row count was zero.");
}

if (salesAmount <= 0)
{
    throw new InvalidOperationException("The Tabular sales amount was zero.");
}

return 0;
'@
}

Reset-Directory $analyticsRoot

Invoke-Sql 'AW_TARGET_SQL' 'SELECT COUNT_BIG(*) AS FactRows, SUM(SalesAmount) AS SalesAmount FROM awbi.FactSalesOrderLine;'
Grant-TabularReadAccess

Invoke-Product meta-analytics @('--new-workspace', $analyticsWorkspace)
Invoke-Product meta-analytics @(
    'add-model',
    '--workspace', $analyticsWorkspace,
    '--id', $modelId,
    '--name', 'AdventureWorks Sales',
    '--default-culture', 'en-US',
    '--description', 'Generated sales analytics model over the BDV-backed AdventureWorks mart.')
Invoke-Product meta-analytics @(
    'add-data-source',
    '--workspace', $analyticsWorkspace,
    '--id', $dataSourceId,
    '--model', $modelId,
    '--name', 'AdventureWorks Mart',
    '--provider', 'SqlServer',
    '--connection-reference', 'AW_TARGET_SQL',
    '--source-kind', 'Relational')

Add-AnalyticsTable 'FactSalesOrderLine' 'Fact Sales Order Line' 'Fact' 'Sales'
Add-AnalyticsTable 'DimProduct' 'Product' 'Dimension' 'Products'
Add-AnalyticsTable 'DimOrderDate' 'Order Date' 'Dimension' 'Time'
Add-AnalyticsTable 'DimSalesTerritory' 'Sales Territory' 'Dimension' 'Geography'
Add-AnalyticsTable 'DimCustomerChannel' 'Customer Channel' 'Dimension' 'Customers'

Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_SalesOrderId' 'Sales Order ID' 'SalesOrderId' 'meta:type:Int64' 10
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_SalesOrderDetailId' 'Sales Order Detail ID' 'SalesOrderDetailId' 'meta:type:Int64' 20
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_SalesOrderNumber' 'Sales Order Number' 'SalesOrderNumber' 'meta:type:String' 30
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_OrderDate' 'Order Date' 'OrderDate' 'meta:type:Date' 40
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_ProductId' 'Product ID' 'ProductId' 'meta:type:Int64' 50
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_ProductNumber' 'Product Number' 'ProductNumber' 'meta:type:String' 60
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_ProductName' 'Product Name' 'ProductName' 'meta:type:String' 70
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_ProductCategory' 'Product Category' 'ProductCategory' 'meta:type:String' 80
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_ProductSubcategory' 'Product Subcategory' 'ProductSubcategory' 'meta:type:String' 90
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_CustomerId' 'Customer ID' 'CustomerId' 'meta:type:Int64' 100
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_CustomerType' 'Customer Type' 'CustomerType' 'meta:type:String' 110
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_TerritoryId' 'Territory ID' 'TerritoryId' 'meta:type:Int64' 120
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_TerritoryName' 'Territory Name' 'TerritoryName' 'meta:type:String' 130
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_TerritoryGroup' 'Territory Group' 'TerritoryGroup' 'meta:type:String' 140
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_SalesPersonBusinessEntityId' 'Salesperson Business Entity ID' 'SalesPersonBusinessEntityId' 'meta:type:Int64' 150
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_SalesPersonName' 'Salesperson Name' 'SalesPersonName' 'meta:type:String' 160
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_OnlineOrderFlag' 'Online Order Flag' 'OnlineOrderFlag' 'meta:type:Boolean' 170
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_OrderQuantity' 'Order Quantity Column' 'OrderQuantity' 'meta:type:Int64' 180 $false $true 'Sum'
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_UnitPrice' 'Unit Price' 'UnitPrice' 'meta:type:Decimal' 190
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_SalesAmount' 'Sales Amount Column' 'SalesAmount' 'meta:type:Decimal' 200 $false $true 'Sum'
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_DiscountAmount' 'Discount Amount Column' 'DiscountAmount' 'meta:type:Decimal' 210 $false $true 'Sum'
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_TaxAmount' 'Tax Amount Column' 'TaxAmount' 'meta:type:Decimal' 220 $false $true 'Sum'
Add-AnalyticsAttribute 'FactSalesOrderLine' 'FactSalesOrderLine_FreightAmount' 'Freight Amount Column' 'FreightAmount' 'meta:type:Decimal' 230 $false $true 'Sum'

Add-AnalyticsAttribute 'DimProduct' 'DimProduct_ProductId' 'Product ID' 'ProductId' 'meta:type:Int64' 10 $true $true
Add-AnalyticsAttribute 'DimProduct' 'DimProduct_ProductNumber' 'Product Number' 'ProductNumber' 'meta:type:String' 20
Add-AnalyticsAttribute 'DimProduct' 'DimProduct_ProductName' 'Product Name' 'ProductName' 'meta:type:String' 30
Add-AnalyticsAttribute 'DimProduct' 'DimProduct_ProductCategory' 'Product Category' 'ProductCategory' 'meta:type:String' 40
Add-AnalyticsAttribute 'DimProduct' 'DimProduct_ProductSubcategory' 'Product Subcategory' 'ProductSubcategory' 'meta:type:String' 50

Add-AnalyticsAttribute 'DimOrderDate' 'DimOrderDate_OrderDate' 'Order Date' 'OrderDate' 'meta:type:Date' 10 $true $true
Add-AnalyticsAttribute 'DimOrderDate' 'DimOrderDate_OrderYear' 'Order Year' 'OrderYear' 'meta:type:Int32' 20
Add-AnalyticsAttribute 'DimOrderDate' 'DimOrderDate_OrderMonth' 'Order Month' 'OrderMonth' 'meta:type:Int32' 30
Add-AnalyticsAttribute 'DimOrderDate' 'DimOrderDate_OrderMonthName' 'Order Month Name' 'OrderMonthName' 'meta:type:String' 40

Add-AnalyticsAttribute 'DimSalesTerritory' 'DimSalesTerritory_TerritoryId' 'Territory ID' 'TerritoryId' 'meta:type:Int64' 10 $true $true
Add-AnalyticsAttribute 'DimSalesTerritory' 'DimSalesTerritory_TerritoryName' 'Territory Name' 'TerritoryName' 'meta:type:String' 20
Add-AnalyticsAttribute 'DimSalesTerritory' 'DimSalesTerritory_CountryRegionCode' 'Country Region Code' 'CountryRegionCode' 'meta:type:String' 30
Add-AnalyticsAttribute 'DimSalesTerritory' 'DimSalesTerritory_TerritoryGroup' 'Territory Group' 'TerritoryGroup' 'meta:type:String' 40

Add-AnalyticsAttribute 'DimCustomerChannel' 'DimCustomerChannel_CustomerId' 'Customer ID' 'CustomerId' 'meta:type:Int64' 10 $true $true
Add-AnalyticsAttribute 'DimCustomerChannel' 'DimCustomerChannel_CustomerAccountNumber' 'Customer Account Number' 'CustomerAccountNumber' 'meta:type:String' 20
Add-AnalyticsAttribute 'DimCustomerChannel' 'DimCustomerChannel_CustomerType' 'Customer Type' 'CustomerType' 'meta:type:String' 30

Add-AnalyticsMeasure 'MeasureSalesAmount' 'FactSalesOrderLine' 'FactSalesOrderLine_SalesAmount' 'Sales Amount' 'meta:type:Decimal' 'Sum' '#,0.00'
Add-AnalyticsMeasure 'MeasureOrderQuantity' 'FactSalesOrderLine' 'FactSalesOrderLine_OrderQuantity' 'Order Quantity' 'meta:type:Int64' 'Sum' '#,0'
Add-AnalyticsMeasure 'MeasureDiscountAmount' 'FactSalesOrderLine' 'FactSalesOrderLine_DiscountAmount' 'Discount Amount' 'meta:type:Decimal' 'Sum' '#,0.00'
Add-AnalyticsMeasure 'MeasureTaxAmount' 'FactSalesOrderLine' 'FactSalesOrderLine_TaxAmount' 'Tax Amount' 'meta:type:Decimal' 'Sum' '#,0.00'
Add-AnalyticsMeasure 'MeasureLineCount' 'FactSalesOrderLine' 'FactSalesOrderLine_SalesOrderDetailId' 'Line Count' 'meta:type:Int64' 'Count' '#,0'

Add-AnalyticsRelationship 'RelationshipFactProduct' 'Product' 'FactSalesOrderLine' 'FactSalesOrderLine_ProductId' 'DimProduct' 'DimProduct_ProductId'
Add-AnalyticsRelationship 'RelationshipFactOrderDate' 'Order Date' 'FactSalesOrderLine' 'FactSalesOrderLine_OrderDate' 'DimOrderDate' 'DimOrderDate_OrderDate'
Add-AnalyticsRelationship 'RelationshipFactCustomerChannel' 'Customer Channel' 'FactSalesOrderLine' 'FactSalesOrderLine_CustomerId' 'DimCustomerChannel' 'DimCustomerChannel_CustomerId'
Add-AnalyticsRelationship 'RelationshipFactSalesTerritory' 'Sales Territory' 'FactSalesOrderLine' 'FactSalesOrderLine_TerritoryId' 'DimSalesTerritory' 'DimSalesTerritory_TerritoryId' $false

Assert-Workspace $analyticsWorkspace

Invoke-Product meta-convert @('analytics-to-tabular', '--workspace', $analyticsWorkspace, '--out', $tabularWorkspace)
Assert-Workspace $tabularWorkspace

Add-TabularPartition 'PartitionFactSalesOrderLine' 'FactSalesOrderLine' 'Fact Sales Order Line Partition' 10 @'
SELECT
    SalesOrderId,
    SalesOrderDetailId,
    SalesOrderNumber,
    OrderDate,
    ProductId,
    ProductNumber,
    ProductName,
    ProductCategory,
    ProductSubcategory,
    CustomerId,
    CustomerType,
    TerritoryId,
    TerritoryName,
    TerritoryGroup,
    SalesPersonBusinessEntityId,
    SalesPersonName,
    OnlineOrderFlag,
    OrderQuantity,
    UnitPrice,
    SalesAmount,
    DiscountAmount,
    TaxAmount,
    FreightAmount
FROM awbi.FactSalesOrderLine
'@

Add-TabularPartition 'PartitionDimProduct' 'DimProduct' 'Product Partition' 20 @'
SELECT
    ProductId,
    ProductNumber,
    ProductName,
    ProductCategory,
    ProductSubcategory
FROM awbi.DimProduct
'@

Add-TabularPartition 'PartitionDimOrderDate' 'DimOrderDate' 'Order Date Partition' 30 @'
SELECT
    OrderDate,
    OrderYear,
    OrderMonth,
    OrderMonthName
FROM awbi.DimOrderDate
'@

Add-TabularPartition 'PartitionDimSalesTerritory' 'DimSalesTerritory' 'Sales Territory Partition' 40 @'
SELECT
    TerritoryId,
    TerritoryName,
    CountryRegionCode,
    TerritoryGroup
FROM awbi.DimSalesTerritory
'@

Add-TabularPartition 'PartitionDimCustomerChannel' 'DimCustomerChannel' 'Customer Channel Partition' 50 @'
SELECT
    CustomerId,
    CustomerAccountNumber,
    CustomerType
FROM awbi.DimCustomerChannel
'@

Invoke-Product meta-tabular @(
    'deploy',
    '--workspace', $tabularWorkspace,
    '--server', $env:AW_TABULAR_SERVER,
    '--database-name', $env:AW_TABULAR_DATABASE,
    '--drop-existing',
    '--no-process')

Invoke-Product meta-tabular @(
    'process',
    '--server', $env:AW_TABULAR_SERVER,
    '--database-name', $env:AW_TABULAR_DATABASE,
    '--refresh-type', 'Full')

Write-ProofProject
$proofProject = Join-Path $proofRoot 'TabularProof.csproj'
$proofOutput = Join-Path $proofRoot 'proof-output.txt'
Invoke-Product dotnet @('restore', $proofProject, '--ignore-failed-sources')
Invoke-ProductWithOutput dotnet @('run', '--project', $proofProject, '--no-restore', '--', $env:AW_TABULAR_SERVER, $env:AW_TABULAR_DATABASE) $proofOutput

$pipelineTaskCount = 0
$pipelineTaskPath = 'ops\Pipeline\instances\TransformExecutionTask.xml'
if (Test-Path -LiteralPath $pipelineTaskPath) {
    [xml]$pipelineTaskXml = Get-Content -LiteralPath $pipelineTaskPath
    $pipelineTaskCount = $pipelineTaskXml.SelectNodes('//TransformExecutionTask').Count
}

$plannedTaskCount = 0
$plannedTaskPath = 'ops\Orchestration\instances\PlannedTask.xml'
if (Test-Path -LiteralPath $plannedTaskPath) {
    [xml]$plannedTaskXml = Get-Content -LiteralPath $plannedTaskPath
    $plannedTaskCount = $plannedTaskXml.SelectNodes('//PlannedTask').Count
}

$proofText = Get-Content -LiteralPath $proofOutput -Raw
Set-Content -LiteralPath 'summary.txt' -Encoding ASCII -Value @"
Clean run completed through SourceDBs -> RDV -> BDV -> DW/Mart -> Tabular.

Source schema was extracted from AdventureWorks2022.
RDV and BDV were created and loaded before the DW/mart.
DW/mart persisted target tables were loaded from BDV-backed transforms.
DQ candidates were generated from modeled transform structure and binding evidence, promoted, converted to SQL, and deployed.
MetaPipeline contains $pipelineTaskCount transform-backed table-load tasks.
MetaOrchestration contains $plannedTaskCount planned table-load tasks inferred from modeled pipeline/binding access profiles.
Tabular workspace was generated from MetaAnalytics, deployed to $($env:AW_TABULAR_SERVER), and processed as $($env:AW_TABULAR_DATABASE).

Tabular proof:
$proofText
"@

Write-Host 'Tabular deploy/process/proof completed.'
