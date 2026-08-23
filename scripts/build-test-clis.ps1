[CmdletBinding()]
param(
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Debug",
    [string[]] $Project,
    [string] $FoundationFeed = (Join-Path $PSScriptRoot "..\artifacts\test-cli-meta-packages"),
    [switch] $List
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

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

function Get-BuiltCliPath {
    param([Parameter(Mandatory = $true)][string] $ProjectPath)

    $properties = & dotnet msbuild $ProjectPath '-getProperty:TargetPath,TargetDir' "-property:Configuration=$Configuration" '-nologo' | ConvertFrom-Json
    if ($LASTEXITCODE -ne 0) {
        throw "Could not resolve the CLI output for '$ProjectPath'."
    }

    $targetPath = [string]$properties.Properties.TargetPath
    if ([string]::IsNullOrWhiteSpace($targetPath)) {
        throw "MSBuild returned no TargetPath for '$ProjectPath'."
    }

    return $targetPath
}

$script:RepoRoot = Get-FullPath (Join-Path $PSScriptRoot "..")
$FoundationFeed = Get-FullPath $FoundationFeed
$localNuGetConfig = Join-RepoPath "artifacts\test-cli-nuget.config"
$localNuGetPackages = Join-RepoPath "artifacts\test-cli-nuget"

$targets = @(
    [pscustomobject]@{ Name = "MetaSchema"; Project = "MetaSchema\Cli\MetaSchema.Cli.csproj" },
    [pscustomobject]@{ Name = "MetaDataType"; Project = "MetaDataType\Cli\MetaDataType.Cli.csproj" },
    [pscustomobject]@{ Name = "MetaDataTypeConversion"; Project = "MetaDataTypeConversion\Cli\MetaDataTypeConversion.Cli.csproj" },
    [pscustomobject]@{ Name = "MetaDataQuality"; Project = "MetaDataQuality\Cli\MetaDataQuality.Cli.csproj" },
    [pscustomobject]@{ Name = "MetaPipeline"; Project = "MetaPipeline\Cli\MetaPipeline.Cli.csproj" },
    [pscustomobject]@{ Name = "MetaOrchestration"; Project = "MetaOrchestration\Cli\MetaOrchestration.Cli.csproj" },
    [pscustomobject]@{ Name = "MetaDataWarehouse"; Project = "MetaDataWarehouse\Cli\MetaDataWarehouse.Cli.csproj" },
    [pscustomobject]@{ Name = "MetaAnalytics"; Project = "MetaAnalytics\Cli\MetaAnalytics.Cli.csproj" },
    [pscustomobject]@{ Name = "MetaTabular"; Project = "MetaTabular\Cli\MetaTabular.Cli.csproj" },
    [pscustomobject]@{ Name = "MetaMultiDimensional"; Project = "MetaMultiDimensional\Cli\MetaMultiDimensional.Cli.csproj" },
    [pscustomobject]@{ Name = "MetaTransformPattern"; Project = "MetaTransform\Pattern\Cli\MetaTransformPattern.Cli.csproj" }
)

if ($List) {
    $targets | Select-Object Name, Project | Format-Table -AutoSize
    return
}

if (-not (Test-Path -LiteralPath $FoundationFeed -PathType Container)) {
    throw "Foundation package feed does not exist: $FoundationFeed"
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

if (Test-Path -LiteralPath $localNuGetPackages) {
    Remove-Item -LiteralPath $localNuGetPackages -Recurse -Force
}
New-Item -ItemType Directory -Path $localNuGetPackages -Force | Out-Null

$escapedFoundationFeed = [System.Security.SecurityElement]::Escape($FoundationFeed)
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="meta-foundation" value="$escapedFoundationFeed" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@ | Set-Content -LiteralPath $localNuGetConfig

$restoreArgs = @(
    "/p:RestoreConfigFile=$localNuGetConfig",
    "/p:RestorePackagesPath=$localNuGetPackages",
    "/p:RestoreNoCache=true"
)

foreach ($target in $selectedTargets) {
    $projectPath = Join-RepoPath $target.Project
    if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf)) {
        throw "Could not find CLI project for '$($target.Name)' at '$projectPath'."
    }

    Invoke-Checked "Building $($target.Name) CLI" {
        & dotnet build $projectPath -c $Configuration --nologo -m:1 -nr:false @restoreArgs
    }

    $outputPath = Get-BuiltCliPath $projectPath
    if (-not (Test-Path -LiteralPath $outputPath -PathType Leaf)) {
        throw "Expected CLI assembly for '$($target.Name)' was not found at '$outputPath'."
    }
}

Write-Host "Built $($selectedTargets.Count) standard CLI test assembly(s)."
