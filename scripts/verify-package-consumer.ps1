param(
    [Parameter(Mandatory = $true)]
    [string]$FoundationFeed
)

$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.IO.Compression.FileSystem

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$feed = [System.IO.Path]::GetFullPath($FoundationFeed)
if (-not (Test-Path -LiteralPath $feed -PathType Container)) {
    throw "Foundation package feed does not exist: $feed"
}

$expectedPackages = @(
    'Meta.Operations',
    'Meta.TypedModels',
    'Meta.Core',
    'Meta.Surfaces',
    'Meta.Surfaces.Xml',
    'Meta.Surfaces.CSharp',
    'Meta.Surfaces.Sql',
    'Meta.Integration',
    'MetaCli.Model',
    'MetaCli.Core',
    'MetaWeave.Model',
    'MetaWeave.Core'
)

$packageFiles = @(Get-ChildItem -LiteralPath $feed -Filter '*.nupkg' -File)
$packageInfo = foreach ($packageFile in $packageFiles) {
    if ($packageFile.Name -notmatch '^(?<id>.+)\.(?<version>\d+\.\d+\.\d+-.+)\.nupkg$') {
        throw "Unexpected package filename: $($packageFile.Name)"
    }

    [pscustomobject]@{
        Id = $Matches.id
        Version = $Matches.version
        Path = $packageFile.FullName
    }
}

$foundIds = @($packageInfo.Id | Sort-Object -Unique)
$sortedExpected = @($expectedPackages | Sort-Object)
if (($foundIds -join '|') -ne ($sortedExpected -join '|')) {
    throw "Foundation feed does not contain exactly the expected package set. Found: $($foundIds -join ', ')"
}

$versions = @($packageInfo.Version | Sort-Object -Unique)
if ($versions.Count -ne 1 -or $versions[0] -eq '0.1.0-internal.9') {
    throw "Foundation packages do not share one current version: $($versions -join ', ')"
}

function Invoke-Dotnet {
    param([Parameter(Mandatory = $true)][string[]]$Arguments)

    Write-Host ('> dotnet ' + ($Arguments -join ' '))
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet command failed with exit code $LASTEXITCODE"
    }
}

function Get-PackageDependencies {
    param([Parameter(Mandatory = $true)][string]$PackagePath)

    $archive = [System.IO.Compression.ZipFile]::OpenRead($PackagePath)
    try {
        $entry = $archive.Entries | Where-Object FullName -like '*.nuspec' | Select-Object -First 1
        if ($null -eq $entry) {
            throw "Package has no nuspec: $PackagePath"
        }

        $reader = [System.IO.StreamReader]::new($entry.Open())
        try {
            [xml]$nuspec = $reader.ReadToEnd()
        }
        finally {
            $reader.Dispose()
        }

        return @($nuspec.package.metadata.dependencies.group.dependency.id | Sort-Object -Unique)
    }
    finally {
        $archive.Dispose()
    }
}

function Assert-PackageDependencies {
    param(
        [Parameter(Mandatory = $true)][string]$PackageId,
        [Parameter(Mandatory = $true)][AllowEmptyCollection()][string[]]$ExpectedDependencies
    )

    $package = $packageInfo | Where-Object Id -eq $PackageId | Select-Object -First 1
    $actual = @(Get-PackageDependencies $package.Path | Sort-Object)
    $expected = @($ExpectedDependencies | Sort-Object)
    if (($actual -join '|') -ne ($expected -join '|')) {
        throw "$PackageId dependencies differ. Expected: $($expected -join ', '). Actual: $($actual -join ', ')"
    }
}

function Get-BuiltCliPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath
    )

    Write-Host "> dotnet msbuild $ProjectPath -getProperty:TargetPath,TargetDir -property:Configuration=Release"
    $json = & dotnet msbuild $ProjectPath '-getProperty:TargetPath,TargetDir' '-property:Configuration=Release' '-nologo' | ConvertFrom-Json
    if ($LASTEXITCODE -ne 0) {
        throw "Could not resolve the CLI TargetPath for $ProjectPath"
    }

    $targetPath = [string]$json.Properties.TargetPath
    if ([string]::IsNullOrWhiteSpace($targetPath)) {
        throw "MSBuild returned no TargetPath for $ProjectPath"
    }

    return $targetPath
}

$consumerRoot = Join-Path $env:TEMP ('meta-bi-package-consumer-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $consumerRoot | Out-Null

robocopy $repoRoot $consumerRoot /E /XD .git bin obj artifacts docs\video-series /NFL /NDL /NJH /NJS /NP | Out-Default
if ($LASTEXITCODE -gt 7) {
    throw "Could not create isolated consumer checkout. robocopy exit code: $LASTEXITCODE"
}

$nugetConfig = Join-Path $consumerRoot 'package-consumer.NuGet.Config'
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="meta-foundation" value="$feed" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@ | Set-Content -LiteralPath $nugetConfig

$packages = Join-Path $env:TEMP ('meta-bi-package-cache-' + [Guid]::NewGuid().ToString('N'))

Assert-PackageDependencies 'Meta.Operations' @()
Assert-PackageDependencies 'Meta.TypedModels' @('Meta.Operations')
Assert-PackageDependencies 'Meta.Core' @('Meta.Operations')
Assert-PackageDependencies 'Meta.Surfaces' @()
Assert-PackageDependencies 'Meta.Surfaces.Xml' @('Meta.Operations', 'Meta.Surfaces', 'Meta.TypedModels')
Assert-PackageDependencies 'Meta.Surfaces.CSharp' @(
    'Meta.Operations', 'Meta.Surfaces', 'Microsoft.CodeAnalysis.CSharp')
Assert-PackageDependencies 'Meta.Surfaces.Sql' @(
    'Meta.Operations', 'Microsoft.Data.SqlClient')
Assert-PackageDependencies 'Meta.Integration' @(
    'Meta.Core', 'Meta.Operations', 'Meta.Surfaces', 'Meta.Surfaces.Xml',
    'Meta.Surfaces.CSharp', 'Meta.Surfaces.Sql', 'Meta.TypedModels', 'Microsoft.Data.SqlClient')
Assert-PackageDependencies 'MetaCli.Model' @()
Assert-PackageDependencies 'MetaCli.Core' @(
    'Meta.Integration', 'Meta.Operations', 'Meta.Surfaces', 'MetaCli.Model')
Assert-PackageDependencies 'MetaWeave.Model' @()
Assert-PackageDependencies 'MetaWeave.Core' @(
    'Meta.Core', 'Meta.Integration', 'Meta.Operations', 'MetaWeave.Model')

$closureCases = @(
    @{ Package = 'Meta.Operations'; Roslyn = $false; SqlClient = $false; Xml = $false },
    @{ Package = 'Meta.TypedModels'; Roslyn = $false; SqlClient = $false; Xml = $false },
    @{ Package = 'Meta.Core'; Roslyn = $false; SqlClient = $false; Xml = $false },
    @{ Package = 'Meta.Surfaces'; Roslyn = $false; SqlClient = $false; Xml = $false },
    @{ Package = 'Meta.Surfaces.Xml'; Roslyn = $false; SqlClient = $false; Xml = $true },
    @{ Package = 'Meta.Surfaces.CSharp'; Roslyn = $true; SqlClient = $false; Xml = $false },
    @{ Package = 'Meta.Surfaces.Sql'; Roslyn = $false; SqlClient = $true; Xml = $false },
    @{ Package = 'Meta.Integration'; Roslyn = $true; SqlClient = $true; Xml = $true },
    @{ Package = 'MetaCli.Model'; Roslyn = $false; SqlClient = $false; Xml = $false },
    @{ Package = 'MetaCli.Core'; Roslyn = $true; SqlClient = $true; Xml = $true },
    @{ Package = 'MetaWeave.Model'; Roslyn = $false; SqlClient = $false; Xml = $false },
    @{ Package = 'MetaWeave.Core'; Roslyn = $true; SqlClient = $true; Xml = $true }
)
foreach ($case in $closureCases) {
    $caseRoot = Join-Path $consumerRoot ('package-closure-' + $case.Package)
    New-Item -ItemType Directory -Path $caseRoot | Out-Null
    $projectPath = Join-Path $caseRoot 'Consumer.csproj'
    @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup>
  <ItemGroup><PackageReference Include="$($case.Package)" Version="$($versions[0])" /></ItemGroup>
</Project>
"@ | Set-Content -LiteralPath $projectPath
    Invoke-Dotnet @('restore', $projectPath, '--configfile', $nugetConfig, '--packages', $packages, '--disable-parallel', '--nologo')

    $assets = Get-Content -LiteralPath (Join-Path $caseRoot 'obj\project.assets.json') -Raw | ConvertFrom-Json
    $libraryNames = @($assets.libraries.PSObject.Properties.Name | ForEach-Object { ($_ -split '/')[0] })
    $hasRoslyn = @($libraryNames | Where-Object {
        $_ -eq 'Microsoft.CodeAnalysis' -or $_ -like 'Microsoft.CodeAnalysis.*'
    }).Count -gt 0
    $hasSqlClient = $libraryNames -contains 'Microsoft.Data.SqlClient'
    $hasXml = $libraryNames -contains 'Meta.Surfaces.Xml'
    if ($hasRoslyn -ne $case.Roslyn -or $hasSqlClient -ne $case.SqlClient -or $hasXml -ne $case.Xml) {
        throw "$($case.Package) closure differs. Roslyn=$hasRoslyn SqlClient=$hasSqlClient Xml=$hasXml"
    }
}

$solutions = @(Get-ChildItem -LiteralPath $consumerRoot -Filter '*.sln' -File | Sort-Object FullName)
foreach ($solution in $solutions) {
    Invoke-Dotnet @('restore', $solution.FullName, '--configfile', $nugetConfig, '--packages', $packages, '--disable-parallel', '--nologo')
}

foreach ($solution in $solutions) {
    Invoke-Dotnet @('build', $solution.FullName, '--configuration', 'Release', '--no-restore', '--nologo', '-m:1', '-nr:false')
}

$assetFiles = @(Get-ChildItem -LiteralPath $consumerRoot -Filter 'project.assets.json' -File -Recurse)
foreach ($assetFile in $assetFiles) {
    $assetText = Get-Content -LiteralPath $assetFile.FullName -Raw
    if ($assetText -match 'Meta\.(Operations|Core|TypedModels|Surfaces(?:\.(?:Xml|CSharp|Sql))?|Integration)\.csproj|MetaCli\.(Model|Core)\.csproj|MetaWeave\.(Model|Core)\.csproj') {
        throw "Foundation dependency was resolved as a project in $($assetFile.FullName)"
    }
}

$testProjects = @(
    'MetaAnalytics\Tests\MetaAnalytics.Tests.csproj',
    'MetaDataQuality\Tests\MetaDataQuality.Tests.csproj',
    'MetaDataType\Tests\MetaDataType.Tests.csproj',
    'MetaDataTypeConversion\Tests\MetaDataTypeConversion.Tests.csproj',
    'MetaDataVault\Tests\MetaDataVault.Tests.csproj',
    'MetaDataWarehouse\Tests\MetaDataWarehouse.Tests.csproj',
    'MetaSchema\Tests\MetaSchema.Tests.csproj',
    'MetaPipeline\Tests\MetaPipeline.Tests.csproj'
)

$testProjects += 'MetaSql\Tests\MetaSql.Tests.csproj'

foreach ($testProject in $testProjects) {
    $testPath = Join-Path $consumerRoot $testProject
    Invoke-Dotnet @('restore', $testPath, '--configfile', $nugetConfig, '--packages', $packages, '--disable-parallel', '--nologo')
    Invoke-Dotnet @('test', $testPath, '--configuration', 'Release', '--no-restore', '--nologo', '-m:1', '-nr:false')
}

foreach ($cli in @(
    @{ Name = 'meta-sql'; Project = 'MetaSql\Cli\MetaSql.Cli.csproj' },
    @{ Name = 'meta-schema'; Project = 'MetaSchema\Cli\MetaSchema.Cli.csproj' },
    @{ Name = 'meta-convert'; Project = 'MetaConvert\Cli\MetaConvert.Cli.csproj' }
)) {
    $cliPath = Get-BuiltCliPath (Join-Path $consumerRoot $cli.Project)
    if (-not (Test-Path -LiteralPath $cliPath -PathType Leaf)) {
        throw "Packaged CLI output was not produced: $($cli.Name)"
    }

    Invoke-Dotnet @($cliPath, '--help')
}

function Assert-PackageEntry {
    param(
        [Parameter(Mandatory = $true)][string]$PackageId,
        [Parameter(Mandatory = $true)][string]$EntryPath
    )

    $package = $packageInfo | Where-Object Id -eq $PackageId | Select-Object -First 1
    $archive = [System.IO.Compression.ZipFile]::OpenRead($package.Path)
    try {
        if ($null -eq $archive.GetEntry($EntryPath)) {
            throw "$PackageId package does not contain $EntryPath"
        }
    }
    finally {
        $archive.Dispose()
    }
}

Assert-PackageEntry 'MetaWeave.Model' 'contentFiles/any/any/MetaWeave/model.xml'
Assert-PackageEntry 'MetaCli.Model' 'contentFiles/any/any/MetaCli/model.xml'

Write-Host "Package-consumer verification passed. Isolated checkout: $consumerRoot"
