param(
    [switch] $ReadyToRun,
    [switch] $SelfContained,
    [switch] $SingleFile,
    [string] $MetaRepo
)

$ErrorActionPreference = 'Stop'

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = (Resolve-Path (Join-Path $scriptDir '..\..')).Path
$publishRoot = Join-Path $repoRoot 'MetaInstaller\Installer\bin\publish'
$packageFlavor = if ($SelfContained) {
    if ($SingleFile) { 'self-contained-singlefile' } else { 'self-contained-shared' }
}
else {
    if ($SingleFile) { 'framework-dependent-singlefile' } else { 'framework-dependent-shared' }
}
$outDir = Join-Path $publishRoot "win-x64-$packageFlavor"
$payloadDir = Join-Path $outDir 'payload\meta-bi\bin'
$publishStagingRoot = Join-Path $publishRoot "staging-$packageFlavor"
$packageDate = Get-Date -Format 'yyyy-MM-dd'
$zipPath = Join-Path $publishRoot "meta-bi-offline-win-x64-$packageDate-$packageFlavor.zip"
$publishReadyToRun = if ($ReadyToRun) { 'true' } else { 'false' }
$publishSelfContained = if ($SelfContained) { 'true' } else { 'false' }
$publishSingleFile = if ($SingleFile) { 'true' } else { 'false' }
$installerPublishSingleFile = 'true'
$payloadUseAppHost = 'true'
$includeNativeLibrariesForSelfExtract = if ($SingleFile) { 'true' } else { 'false' }
$metaRepoRoot = if ([string]::IsNullOrWhiteSpace($MetaRepo)) {
    (Resolve-Path (Join-Path $repoRoot '..\meta')).Path
}
else {
    (Resolve-Path $MetaRepo).Path
}
$localPackageSource = Join-Path $repoRoot 'artifacts\release-meta-packages'
$localNuGetCache = Join-Path $repoRoot 'artifacts\release-nuget'
$localNuGetConfig = Join-Path $repoRoot 'artifacts\release-nuget.config'
$publishRestoreArgs = @()

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

function Add-ZipFile {
    param(
        [Parameter(Mandatory = $true)][System.IO.Compression.ZipArchive] $Archive,
        [Parameter(Mandatory = $true)][string] $SourcePath,
        [Parameter(Mandatory = $true)][string] $EntryName
    )

    $normalizedEntryName = $EntryName.Replace('\', '/')
    [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile(
        $Archive,
        $SourcePath,
        $normalizedEntryName,
        [System.IO.Compression.CompressionLevel]::Fastest) | Out-Null
}

function Add-ZipDirectory {
    param(
        [Parameter(Mandatory = $true)][System.IO.Compression.ZipArchive] $Archive,
        [Parameter(Mandatory = $true)][string] $SourceDirectory,
        [Parameter(Mandatory = $true)][string] $EntryPrefix
    )

    $root = (Resolve-Path $SourceDirectory).Path.TrimEnd('\', '/')
    foreach ($file in [System.IO.Directory]::EnumerateFiles($root, '*', [System.IO.SearchOption]::AllDirectories)) {
        $relativePath = $file.Substring($root.Length).TrimStart('\', '/')
        Add-ZipFile -Archive $Archive -SourcePath $file -EntryName (Join-Path $EntryPrefix $relativePath)
    }
}

function New-OfflineZip {
    param(
        [Parameter(Mandatory = $true)][string] $DestinationPath,
        [Parameter(Mandatory = $true)][string] $InstallerPath,
        [Parameter(Mandatory = $true)][string] $PayloadPath
    )

    if (Test-Path -LiteralPath $DestinationPath) {
        Remove-Item -LiteralPath $DestinationPath -Force
    }

    $archive = [System.IO.Compression.ZipFile]::Open($DestinationPath, [System.IO.Compression.ZipArchiveMode]::Create)
    try {
        Add-ZipFile -Archive $archive -SourcePath $InstallerPath -EntryName (Split-Path $InstallerPath -Leaf)
        Add-ZipDirectory -Archive $archive -SourceDirectory $PayloadPath -EntryPrefix 'payload'
    }
    finally {
        $archive.Dispose()
    }
}

function Remove-DirectoryUnderRoot {
    param(
        [Parameter(Mandatory = $true)][string] $Path,
        [Parameter(Mandatory = $true)][string] $Root
    )

    $fullPath = [System.IO.Path]::GetFullPath($Path)
    $fullRoot = [System.IO.Path]::GetFullPath($Root).TrimEnd('\', '/')
    $rootPrefix = $fullRoot + [System.IO.Path]::DirectorySeparatorChar
    if (-not $fullPath.StartsWith($rootPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to remove path outside expected root. Path: $fullPath Root: $fullRoot"
    }

    if (Test-Path -LiteralPath $fullPath) {
        Remove-Item -LiteralPath $fullPath -Recurse -Force
    }
}

function ConvertTo-SafePathSegment {
    param([Parameter(Mandatory = $true)][string] $Value)

    $safe = [Regex]::Replace($Value, '[<>:"/\\|?*]', '_').Trim()
    if ([string]::IsNullOrWhiteSpace($safe)) {
        return 'unnamed'
    }

    return $safe.TrimEnd([char[]]@('.', ' '))
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

function Initialize-ReleaseNuGet {
    $metaOperationsProject = Join-Path $metaRepoRoot 'Meta\Operations\Meta.Operations.csproj'
    $metaCoreProject = Join-Path $metaRepoRoot 'Meta\Core\Meta.Core.csproj'
    $metaAdaptersProject = Join-Path $metaRepoRoot 'Meta\Adapters\Meta.Adapters.csproj'
    if (-not (Test-Path -LiteralPath $metaOperationsProject)) {
        throw "Could not find upstream Meta.Operations project at '$metaOperationsProject'. Use -MetaRepo to point at the core meta repository."
    }

    if (-not (Test-Path -LiteralPath $metaCoreProject)) {
        throw "Could not find upstream Meta.Core project at '$metaCoreProject'. Use -MetaRepo to point at the core meta repository."
    }

    if (-not (Test-Path -LiteralPath $metaAdaptersProject)) {
        throw "Could not find upstream Meta.Adapters project at '$metaAdaptersProject'. Use -MetaRepo to point at the core meta repository."
    }

    Remove-DirectoryUnderRoot -Path $localPackageSource -Root $repoRoot
    New-Item -ItemType Directory -Path $localPackageSource -Force | Out-Null
    New-Item -ItemType Directory -Path $localNuGetCache -Force | Out-Null
    Remove-DirectoryUnderRoot -Path (Join-Path $localNuGetCache 'meta.core') -Root $localNuGetCache
    Remove-DirectoryUnderRoot -Path (Join-Path $localNuGetCache 'meta.adapters') -Root $localNuGetCache

    $escapedLocalPackageSource = [System.Security.SecurityElement]::Escape($localPackageSource)
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

    Invoke-Checked "Packing local Meta.Operations from: $metaOperationsProject" {
        dotnet pack $metaOperationsProject -c Release --nologo -o $localPackageSource
    }

    Invoke-Checked "Packing local Meta.Core from: $metaCoreProject" {
        dotnet pack $metaCoreProject -c Release --nologo -o $localPackageSource
    }

    Invoke-Checked "Packing local Meta.Adapters from: $metaAdaptersProject" {
        dotnet pack $metaAdaptersProject -c Release --nologo -o $localPackageSource
    }

    $env:NUGET_PACKAGES = $localNuGetCache
    $script:publishRestoreArgs = @("/p:RestoreConfigFile=$localNuGetConfig")
}

$cliProjects = @(
    'MetaSchema\Cli\MetaSchema.Cli.csproj',
    'MetaDataType\Cli\MetaDataType.Cli.csproj',
    'MetaDataTypeConversion\Cli\MetaDataTypeConversion.Cli.csproj',
    'MetaDataQuality\Cli\MetaDataQuality.Cli.csproj',
    'MetaDataWarehouse\Cli\MetaDataWarehouse.Cli.csproj',
    'MetaAnalytics\Cli\MetaAnalytics.Cli.csproj',
    'MetaTabular\Cli\MetaTabular.Cli.csproj',
    'MetaMultiDimensional\Cli\MetaMultiDimensional.Cli.csproj',
    'MetaSql\Cli\MetaSql.Cli.csproj',
    'MetaConvert\Cli\MetaConvert.Cli.csproj',
    'MetaDataVault\Cli\Raw\MetaDataVault.Raw.Cli.csproj',
    'MetaDataVault\Cli\Business\MetaDataVault.Business.Cli.csproj',
    'MetaPipeline\Cli\MetaPipeline.Cli.csproj',
    'MetaOrchestration\Cli\MetaOrchestration.Cli.csproj',
    'MetaTransform\Script\Cli\MetaTransformScript.Cli.csproj',
    'MetaTransform\Binding\Cli\MetaTransformBinding.Cli.csproj'
)

$workspaces = @(
    @{ Source = 'MetaSchema\Workspaces\MetaSchema'; Target = 'MetaSchema' },
    @{ Source = 'MetaDataType\Workspace'; Target = 'MetaDataType' },
    @{ Source = 'MetaDataTypeConversion\Workspace'; Target = 'MetaDataTypeConversion' },
    @{ Source = 'MetaDataVault\Workspaces\MetaRawDataVault'; Target = 'MetaRawDataVault' },
    @{ Source = 'MetaDataVault\Workspaces\MetaBusinessDataVault'; Target = 'MetaBusinessDataVault' },
    @{ Source = 'MetaDataVault\Workspaces\MetaDataVaultImplementation'; Target = 'MetaDataVaultImplementation' },
    @{ Source = 'MetaDataWarehouse\Workspaces\MetaDataWarehouse'; Target = 'MetaDataWarehouse' },
    @{ Source = 'MetaDataWarehouse\Workspaces\MetaDataWarehouseImplementation'; Target = 'MetaDataWarehouseImplementation' },
    @{ Source = 'MetaAnalytics\Workspaces\MetaAnalytics'; Target = 'MetaAnalytics' },
    @{ Source = 'MetaTabular\Workspaces\MetaTabular'; Target = 'MetaTabular' },
    @{ Source = 'MetaMultiDimensional\Workspaces\MetaMultiDimensional'; Target = 'MetaMultiDimensional' },
    @{ Source = 'MetaSql\Workspace'; Target = 'MetaSql' },
    @{ Source = 'MetaSql\DeployManifest\Workspace'; Target = 'MetaSqlDeployManifest' },
    @{ Source = 'MetaTransform\Script\Workspaces\MetaTransformScript'; Target = 'MetaTransformScript' },
    @{ Source = 'MetaTransform\Binding\Workspaces\MetaTransformBinding'; Target = 'MetaTransformBinding' },
    @{ Source = 'MetaPipeline\Workspaces\MetaPipeline'; Target = 'MetaPipeline' },
    @{ Source = 'MetaOrchestration\Workspaces\MetaOrchestration'; Target = 'MetaOrchestration' },
    @{ Source = 'MetaDataQuality\Workspaces\MetaDataQuality'; Target = 'MetaDataQuality' }
)

function Publish-CliProject {
    param([string] $Project)

    $projectPath = Join-Path $repoRoot $Project
    $projectStageName = ConvertTo-SafePathSegment -Value ([System.IO.Path]::ChangeExtension($Project, $null))
    $projectPublishDir = Join-Path $publishStagingRoot $projectStageName
    Remove-DirectoryUnderRoot -Path $projectPublishDir -Root $publishStagingRoot
    New-Item -ItemType Directory -Path $projectPublishDir -Force | Out-Null

    Write-Host "Publishing payload from: $projectPath"
    dotnet publish $projectPath -c Release -r win-x64 --self-contained $publishSelfContained -p:UseAppHost=$payloadUseAppHost -p:PublishSingleFile=$publishSingleFile -p:IncludeNativeLibrariesForSelfExtract=$includeNativeLibrariesForSelfExtract -p:PublishReadyToRun=$publishReadyToRun -o $projectPublishDir @publishRestoreArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Publishing payload failed for '$Project' with exit code $LASTEXITCODE."
    }

    Merge-PublishedDirectory -SourceDirectory $projectPublishDir -DestinationDirectory $payloadDir -SourceDescription $Project
}

function Merge-PublishedDirectory {
    param(
        [Parameter(Mandatory = $true)][string] $SourceDirectory,
        [Parameter(Mandatory = $true)][string] $DestinationDirectory,
        [Parameter(Mandatory = $true)][string] $SourceDescription
    )

    $sourceRoot = (Resolve-Path $SourceDirectory).Path.TrimEnd('\', '/')
    New-Item -ItemType Directory -Path $DestinationDirectory -Force | Out-Null
    foreach ($file in [System.IO.Directory]::EnumerateFiles($sourceRoot, '*', [System.IO.SearchOption]::AllDirectories)) {
        $relativePath = $file.Substring($sourceRoot.Length).TrimStart('\', '/')
        $targetPath = Join-Path $DestinationDirectory $relativePath
        $targetDirectory = Split-Path -Parent $targetPath
        if (-not (Test-Path -LiteralPath $targetDirectory)) {
            New-Item -ItemType Directory -Path $targetDirectory -Force | Out-Null
        }

        if (Test-Path -LiteralPath $targetPath -PathType Leaf) {
            $sourceHash = (Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash
            $targetHash = (Get-FileHash -LiteralPath $targetPath -Algorithm SHA256).Hash
            if ($sourceHash -ne $targetHash) {
                throw "Publish collision while merging '$SourceDescription'. File '$relativePath' already exists in the shared payload with different content."
            }

            continue
        }

        Copy-Item -LiteralPath $file -Destination $targetPath -Force
    }
}

function Copy-SanctionedWorkspace {
    param(
        [string] $Source,
        [string] $Target
    )

    $sourceWorkspace = Join-Path $repoRoot $Source
    $targetWorkspace = Join-Path $payloadDir $Target
    $workspaceMeta = Join-Path $sourceWorkspace 'workspace.meta'
    $modelXml = Join-Path $sourceWorkspace 'model.xml'
    if (-not (Test-Path -LiteralPath $workspaceMeta)) {
        throw "Missing sanctioned workspace.meta: $sourceWorkspace"
    }
    if (-not (Test-Path -LiteralPath $modelXml)) {
        throw "Missing sanctioned model.xml: $sourceWorkspace"
    }

    Write-Host "Copying sanctioned workspace: $Target"
    New-Item -ItemType Directory -Path $targetWorkspace -Force | Out-Null
    Copy-Item -LiteralPath $workspaceMeta -Destination (Join-Path $targetWorkspace 'workspace.meta') -Force
    Copy-Item -LiteralPath $modelXml -Destination (Join-Path $targetWorkspace 'model.xml') -Force

    $sourceInstances = Join-Path $sourceWorkspace 'instances'
    if (Test-Path -LiteralPath $sourceInstances) {
        Copy-Item -LiteralPath $sourceInstances -Destination (Join-Path $targetWorkspace 'instances') -Recurse -Force
    }
}

$previousNuGetPackages = $env:NUGET_PACKAGES
try {
Initialize-ReleaseNuGet

Remove-DirectoryUnderRoot -Path $outDir -Root $publishRoot
Remove-DirectoryUnderRoot -Path $publishStagingRoot -Root $publishRoot
New-Item -ItemType Directory -Path $payloadDir -Force | Out-Null
New-Item -ItemType Directory -Path $publishStagingRoot -Force | Out-Null

Write-Host "PackageFlavor: $packageFlavor"
Write-Host "PublishSelfContained: $publishSelfContained"
Write-Host "PublishSingleFile payload: $publishSingleFile"
Write-Host "PublishReadyToRun: $publishReadyToRun"

Write-Host 'Publishing install-meta-bi.exe...'
dotnet publish (Join-Path $repoRoot 'MetaInstaller\Installer\MetaBi.Installer.csproj') -c Release -r win-x64 --self-contained $publishSelfContained -p:UseAppHost=true -p:PublishSingleFile=$installerPublishSingleFile -p:IncludeNativeLibrariesForSelfExtract=$includeNativeLibrariesForSelfExtract -p:PublishReadyToRun=$publishReadyToRun -p:UpdateInstallMetaBiPublishDir=false -o $outDir @publishRestoreArgs
if ($LASTEXITCODE -ne 0) {
    throw "Publishing install-meta-bi.exe failed with exit code $LASTEXITCODE."
}

foreach ($project in $cliProjects) {
    Publish-CliProject $project
}

foreach ($workspace in $workspaces) {
    Copy-SanctionedWorkspace -Source $workspace.Source -Target $workspace.Target
}

Write-Host 'Removing debug symbol files (*.pdb) from release payload...'
Get-ChildItem -LiteralPath $outDir -Recurse -Filter '*.pdb' -File | Remove-Item -Force

Write-Host 'Removing old local zip packages...'
Get-ChildItem -LiteralPath $publishRoot -Filter 'meta-bi-offline-win-x64-*.zip' -File -ErrorAction SilentlyContinue | Remove-Item -Force

Write-Host 'Creating zipped offline package...'
$items = @(
    (Join-Path $outDir 'install-meta-bi.exe'),
    (Join-Path $outDir 'payload')
)
foreach ($item in $items) {
    if (-not (Test-Path -LiteralPath $item)) {
        throw "Missing release item: $item"
    }
}
New-OfflineZip `
    -DestinationPath $zipPath `
    -InstallerPath (Join-Path $outDir 'install-meta-bi.exe') `
    -PayloadPath (Join-Path $outDir 'payload')

Write-Host ''
Write-Host 'Offline package ready:'
Write-Host "  $outDir"
Write-Host 'Zipped release:'
Write-Host "  $zipPath"
Write-Host ''
Write-Host 'Required layout:'
Write-Host '  install-meta-bi.exe'
Write-Host '  payload\meta-bi\bin\...'
}
finally {
    $env:NUGET_PACKAGES = $previousNuGetPackages
}
