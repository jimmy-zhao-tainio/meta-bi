[CmdletBinding()]
param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Debug",
    [string[]] $Project,
    [string] $MetaRepo,
    [switch] $SkipPackLocalMetaPackages,
    [string] $LocalPackageSource,
    [switch] $List
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Standard CLI tests consume already-built executables. This script builds those
# executables once and, by default, restores against freshly packed local meta
# packages so same-version global package cache drift cannot hide or create test
# failures.

function Get-FullPath {
    param([Parameter(Mandatory = $true)][string] $Path)

    return [System.IO.Path]::GetFullPath($Path)
}

function Join-RepoPath {
    param([Parameter(Mandatory = $true)][string] $RelativePath)

    return Get-FullPath (Join-Path $script:RepoRoot $RelativePath)
}

function Invoke-Checked {
    param(
        [Parameter(Mandatory = $true)][string] $Description,
        [Parameter(Mandatory = $true)][scriptblock] $Command
    )

    Write-Host $Description
    & $Command
    if ($LASTEXITCODE -ne 0) {
        throw "$Description failed with exit code $LASTEXITCODE."
    }
}

$script:RepoRoot = Get-FullPath (Join-Path $PSScriptRoot "..")
if ([string]::IsNullOrWhiteSpace($MetaRepo)) {
    $MetaRepo = Join-Path $script:RepoRoot "..\meta"
}

$MetaRepo = Get-FullPath $MetaRepo
$metaOperationsProject = Join-Path $MetaRepo "Meta\Operations\Meta.Operations.csproj"
$metaCoreProject = Join-Path $MetaRepo "Meta\Core\Meta.Core.csproj"
$metaAdaptersProject = Join-Path $MetaRepo "Meta\Adapters\Meta.Adapters.csproj"
if ([string]::IsNullOrWhiteSpace($LocalPackageSource)) {
    $LocalPackageSource = Join-Path $script:RepoRoot "artifacts\test-cli-meta-packages"
}

$LocalPackageSource = Get-FullPath $LocalPackageSource
$localNuGetPackages = Join-RepoPath "artifacts\test-cli-nuget"
$localNuGetConfig = Join-RepoPath "artifacts\test-cli-nuget.config"

$targets = @(
    [pscustomobject]@{ Name = "MetaSchema"; Project = "MetaSchema\Cli\MetaSchema.Cli.csproj"; Executable = "MetaSchema\Cli\bin\$Configuration\net8.0\meta-schema.exe" },
    [pscustomobject]@{ Name = "MetaDataType"; Project = "MetaDataType\Cli\MetaDataType.Cli.csproj"; Executable = "MetaDataType\Cli\bin\$Configuration\net8.0\meta-data-type.exe" },
    [pscustomobject]@{ Name = "MetaDataTypeConversion"; Project = "MetaDataTypeConversion\Cli\MetaDataTypeConversion.Cli.csproj"; Executable = "MetaDataTypeConversion\Cli\bin\$Configuration\net8.0\meta-data-type-conversion.exe" },
    [pscustomobject]@{ Name = "MetaDataQuality"; Project = "MetaDataQuality\Cli\MetaDataQuality.Cli.csproj"; Executable = "MetaDataQuality\Cli\bin\$Configuration\net8.0\meta-data-quality.exe" },
    [pscustomobject]@{ Name = "MetaPipeline"; Project = "MetaPipeline\Cli\MetaPipeline.Cli.csproj"; Executable = "MetaPipeline\Cli\bin\$Configuration\net8.0\meta-pipeline.exe" },
    [pscustomobject]@{ Name = "MetaOrchestration"; Project = "MetaOrchestration\Cli\MetaOrchestration.Cli.csproj"; Executable = "MetaOrchestration\Cli\bin\$Configuration\net8.0\meta-orchestration.exe" },
    [pscustomobject]@{ Name = "MetaDataWarehouse"; Project = "MetaDataWarehouse\Cli\MetaDataWarehouse.Cli.csproj"; Executable = "MetaDataWarehouse\Cli\bin\$Configuration\net8.0\meta-data-warehouse.exe" },
    [pscustomobject]@{ Name = "MetaAnalytics"; Project = "MetaAnalytics\Cli\MetaAnalytics.Cli.csproj"; Executable = "MetaAnalytics\Cli\bin\$Configuration\net8.0\meta-analytics.exe" },
    [pscustomobject]@{ Name = "MetaTabular"; Project = "MetaTabular\Cli\MetaTabular.Cli.csproj"; Executable = "MetaTabular\Cli\bin\$Configuration\net8.0\meta-tabular.exe" },
    [pscustomobject]@{ Name = "MetaMultiDimensional"; Project = "MetaMultiDimensional\Cli\MetaMultiDimensional.Cli.csproj"; Executable = "MetaMultiDimensional\Cli\bin\$Configuration\net8.0\meta-multi-dimensional.exe" }
)

if ($List) {
    $targets |
        Select-Object Name, Project, Executable |
        Format-Table -AutoSize
    return
}

$selectedTargets = $targets
if ($Project -and $Project.Count -gt 0) {
    $targetsByName = @{}
    foreach ($target in $targets) {
        $targetsByName[$target.Name.ToLowerInvariant()] = $target
    }

    $selectedTargets = @(
        foreach ($projectName in $Project) {
            $key = $projectName.ToLowerInvariant()
            if (-not $targetsByName.ContainsKey($key)) {
                $available = ($targets | ForEach-Object { $_.Name }) -join ", "
                throw "Unknown CLI test project '$projectName'. Available projects: $available"
            }

            $targetsByName[$key]
        }
    )
}

$buildArgsSuffix = @()
$stableBuildArgs = @("-m:1", "-nr:false")
$previousNuGetPackages = $env:NUGET_PACKAGES
$packLocalMetaPackages = -not $SkipPackLocalMetaPackages
try {
    if ($packLocalMetaPackages) {
        if (-not (Test-Path $metaCoreProject)) {
            throw "Could not find upstream Meta.Core project at '$metaCoreProject'. Use -MetaRepo to point at the core meta repository."
        }

        if (-not (Test-Path $metaAdaptersProject)) {
            throw "Could not find upstream Meta.Adapters project at '$metaAdaptersProject'. Use -MetaRepo to point at the core meta repository."
        }

        if (Test-Path $LocalPackageSource) {
            Remove-Item -LiteralPath $LocalPackageSource -Recurse -Force
        }

        if (Test-Path $localNuGetPackages) {
            Remove-Item -LiteralPath $localNuGetPackages -Recurse -Force
        }

        New-Item -ItemType Directory -Path $LocalPackageSource -Force | Out-Null
        New-Item -ItemType Directory -Path $localNuGetPackages -Force | Out-Null
        $escapedLocalPackageSource = [System.Security.SecurityElement]::Escape($LocalPackageSource)
        $nugetConfigContents = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local-meta" value="$escapedLocalPackageSource" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@
        Set-Content -LiteralPath $localNuGetConfig -Value $nugetConfigContents -Encoding UTF8

        Invoke-Checked "Packing local Meta.Operations" {
            & dotnet pack $metaOperationsProject -c $Configuration --nologo -o $LocalPackageSource @stableBuildArgs
        }

        Invoke-Checked "Packing local Meta.Core" {
            & dotnet pack $metaCoreProject -c $Configuration --nologo -o $LocalPackageSource @stableBuildArgs
        }

        Invoke-Checked "Packing local Meta.Adapters" {
            & dotnet pack $metaAdaptersProject -c $Configuration --nologo -o $LocalPackageSource @stableBuildArgs
        }

        $env:NUGET_PACKAGES = $localNuGetPackages
        $buildArgsSuffix = @("/p:RestoreConfigFile=$localNuGetConfig", "/p:RestoreNoCache=true")
    }

    foreach ($target in $selectedTargets) {
        $projectPath = Join-RepoPath $target.Project
        if (-not (Test-Path $projectPath)) {
            throw "Could not find CLI project for '$($target.Name)' at '$projectPath'."
        }

        Invoke-Checked "Building $($target.Name) CLI" {
            & dotnet build $projectPath -c $Configuration --nologo @stableBuildArgs @buildArgsSuffix
        }

        $executablePath = Join-RepoPath $target.Executable
        if (-not (Test-Path $executablePath)) {
            throw "Expected CLI executable for '$($target.Name)' was not found at '$executablePath'."
        }
    }
}
finally {
    $env:NUGET_PACKAGES = $previousNuGetPackages
}

Write-Host "Built $($selectedTargets.Count) standard CLI test executable(s)."
