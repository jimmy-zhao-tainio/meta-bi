[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string] $MetaRepo,
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Debug",
    [string[]] $Project,
    [switch] $SkipMetaCliBuild,
    [switch] $SkipToolingProjectBuild,
    [switch] $SkipPackLocalMetaPackages,
    [string] $LocalPackageSource,
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

function Assert-PathInside {
    param(
        [Parameter(Mandatory = $true)][string] $Path,
        [Parameter(Mandatory = $true)][string] $Root,
        [Parameter(Mandatory = $true)][string] $Description
    )

    $fullPath = Get-FullPath $Path
    $fullRoot = Get-FullPath $Root
    if (-not $fullRoot.EndsWith([System.IO.Path]::DirectorySeparatorChar)) {
        $fullRoot += [System.IO.Path]::DirectorySeparatorChar
    }

    if (-not $fullPath.StartsWith($fullRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "$Description must stay inside repo root. Path: $fullPath"
    }
}

function Assert-CanonicalToolingOutput {
    param(
        [Parameter(Mandatory = $true)] $Target,
        [Parameter(Mandatory = $true)][string] $OutputPath
    )

    Assert-PathInside -Path $OutputPath -Root $script:RepoRoot -Description "Tooling output for '$($Target.Name)'"

    $leaf = Split-Path $OutputPath -Leaf
    $parentLeaf = Split-Path (Split-Path $OutputPath -Parent) -Leaf

    if ($parentLeaf -ne "Tooling" -or $leaf -ne $Target.Name) {
        throw "Unsafe tooling output for '$($Target.Name)': '$OutputPath'. Expected a canonical nested output path ending in 'Tooling\$($Target.Name)'."
    }
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

$metaRepoRoot = Get-FullPath $MetaRepo
$metaCliProject = Join-Path $metaRepoRoot "Meta\Cli\Meta.Cli.csproj"
$metaCliDll = Join-Path $metaRepoRoot "Meta\Cli\bin\$Configuration\net8.0\meta.dll"
$metaCoreProject = Join-Path $metaRepoRoot "Meta\Core\Meta.Core.csproj"
$metaAdaptersProject = Join-Path $metaRepoRoot "Meta\Adapters\Meta.Adapters.csproj"
$stableBuildArgs = @("-m:1", "-nr:false")

if ([string]::IsNullOrWhiteSpace($LocalPackageSource)) {
    $LocalPackageSource = Join-Path $script:RepoRoot "artifacts\local-meta-packages"
}

$localNuGetPackages = Join-Path $script:RepoRoot "artifacts\local-meta-nuget"
$localNuGetConfig = Join-Path $script:RepoRoot "artifacts\local-meta-nuget.config"

$targets = @(
    [pscustomobject]@{ Name = "MetaSchema"; Workspace = "MetaSchema\Workspaces\MetaSchema"; Output = "MetaSchema\Tooling\MetaSchema"; ToolingProject = "MetaSchema\Tooling\MetaSchema.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaDataType"; Workspace = "MetaDataType\Workspace"; Output = "MetaDataType\Tooling\MetaDataType"; ToolingProject = "MetaDataType\Tooling\MetaDataType.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaDataTypeConversion"; Workspace = "MetaDataTypeConversion\Workspace"; Output = "MetaDataTypeConversion\Tooling\MetaDataTypeConversion"; ToolingProject = "MetaDataTypeConversion\Tooling\MetaDataTypeConversion.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaRawDataVault"; Workspace = "MetaDataVault\Workspaces\MetaRawDataVault"; Output = "MetaDataVault\Tooling\MetaRawDataVault"; ToolingProject = "MetaDataVault\Tooling\MetaDataVault.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaBusinessDataVault"; Workspace = "MetaDataVault\Workspaces\MetaBusinessDataVault"; Output = "MetaDataVault\Tooling\MetaBusinessDataVault"; ToolingProject = "MetaDataVault\Tooling\MetaDataVault.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaDataVaultImplementation"; Workspace = "MetaDataVault\Workspaces\MetaDataVaultImplementation"; Output = "MetaDataVault\Tooling\MetaDataVaultImplementation"; ToolingProject = "MetaDataVault\Tooling\MetaDataVault.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaDataWarehouse"; Workspace = "MetaDataWarehouse\Workspaces\MetaDataWarehouse"; Output = "MetaDataWarehouse\Tooling\MetaDataWarehouse"; ToolingProject = "MetaDataWarehouse\Tooling\MetaDataWarehouse.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaDataWarehouseImplementation"; Workspace = "MetaDataWarehouse\Workspaces\MetaDataWarehouseImplementation"; Output = "MetaDataWarehouse\Tooling\MetaDataWarehouseImplementation"; ToolingProject = "MetaDataWarehouse\Tooling\MetaDataWarehouse.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaAnalytics"; Workspace = "MetaAnalytics\Workspaces\MetaAnalytics"; Output = "MetaAnalytics\Tooling\MetaAnalytics"; ToolingProject = "MetaAnalytics\Tooling\MetaAnalytics.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaTabular"; Workspace = "MetaTabular\Workspaces\MetaTabular"; Output = "MetaTabular\Tooling\MetaTabular"; ToolingProject = "MetaTabular\Tooling\MetaTabular.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaMultiDimensional"; Workspace = "MetaMultiDimensional\Workspaces\MetaMultiDimensional"; Output = "MetaMultiDimensional\Tooling\MetaMultiDimensional"; ToolingProject = "MetaMultiDimensional\Tooling\MetaMultiDimensional.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaSql"; Workspace = "MetaSql\Workspace"; Output = "MetaSql\Tooling\MetaSql"; ToolingProject = "MetaSql\Tooling\MetaSql.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaSqlDeployManifest"; Workspace = "MetaSql\DeployManifest\Workspace"; Output = "MetaSql\DeployManifest\Tooling\MetaSqlDeployManifest"; ToolingProject = "MetaSql\DeployManifest\Tooling\MetaSqlDeployManifest.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaTransformScript"; Workspace = "MetaTransform\Script\Workspaces\MetaTransformScript"; Output = "MetaTransform\Script\Tooling\MetaTransformScript"; ToolingProject = "MetaTransform\Script\Tooling\MetaTransformScript.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaDataQuality"; Workspace = "MetaDataQuality\Workspaces\MetaDataQuality"; Output = "MetaDataQuality\Tooling\MetaDataQuality"; ToolingProject = "MetaDataQuality\Tooling\MetaDataQuality.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaTransformBinding"; Workspace = "MetaTransform\Binding\Workspaces\MetaTransformBinding"; Output = "MetaTransform\Binding\Tooling\MetaTransformBinding"; ToolingProject = "MetaTransform\Binding\Tooling\MetaTransformBinding.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaPipeline"; Workspace = "MetaPipeline\Workspaces\MetaPipeline"; Output = "MetaPipeline\Tooling\MetaPipeline"; ToolingProject = "MetaPipeline\Tooling\MetaPipeline.Tooling.csproj" },
    [pscustomobject]@{ Name = "MetaOrchestration"; Workspace = "MetaOrchestration\Workspaces\MetaOrchestration"; Output = "MetaOrchestration\Tooling\MetaOrchestration"; ToolingProject = "MetaOrchestration\Tooling\MetaOrchestration.Tooling.csproj" }
)

if ($List) {
    $targets |
        Select-Object Name, Workspace, Output, ToolingProject |
        Format-Table -AutoSize
    return
}

$selectedTargets = $targets
if ($Project -and $Project.Count -gt 0) {
    $targetsByName = @{}
    foreach ($target in $targets) {
        $targetsByName[$target.Name.ToLowerInvariant()] = $target
    }

    $selectedTargets = foreach ($projectName in $Project) {
        $key = $projectName.ToLowerInvariant()
        if (-not $targetsByName.ContainsKey($key)) {
            $available = ($targets | ForEach-Object { $_.Name }) -join ", "
            throw "Unknown tooling project '$projectName'. Available projects: $available"
        }

        $targetsByName[$key]
    }
}

foreach ($target in $selectedTargets) {
    $workspacePath = Join-RepoPath $target.Workspace
    $outputPath = Join-RepoPath $target.Output
    $toolingProjectPath = Join-RepoPath $target.ToolingProject

    Assert-PathInside -Path $workspacePath -Root $script:RepoRoot -Description "Workspace path for '$($target.Name)'"
    Assert-CanonicalToolingOutput -Target $target -OutputPath $outputPath

    if (-not (Test-Path (Join-Path $workspacePath "workspace.xml"))) {
        throw "Workspace '$($target.Name)' is missing workspace.xml at '$workspacePath'."
    }

    if (-not (Test-Path (Join-Path $workspacePath "model.xml"))) {
        throw "Workspace '$($target.Name)' is missing model.xml at '$workspacePath'."
    }

    $target | Add-Member -NotePropertyName WorkspacePath -NotePropertyValue $workspacePath -Force
    $target | Add-Member -NotePropertyName OutputPath -NotePropertyValue $outputPath -Force
    $target | Add-Member -NotePropertyName ToolingProjectPath -NotePropertyValue $toolingProjectPath -Force
}

if (-not (Test-Path $metaCliProject)) {
    throw "Could not find upstream meta CLI project at '$metaCliProject'. Use -MetaRepo to point at the core meta repository."
}

if (-not (Test-Path $metaCoreProject)) {
    throw "Could not find upstream Meta.Core project at '$metaCoreProject'. Use -MetaRepo to point at the core meta repository."
}

if (-not (Test-Path $metaAdaptersProject)) {
    throw "Could not find upstream Meta.Adapters project at '$metaAdaptersProject'. Use -MetaRepo to point at the core meta repository."
}

if (-not $SkipMetaCliBuild) {
    if ($PSCmdlet.ShouldProcess($metaCliProject, "Build upstream meta CLI")) {
        Invoke-Checked "Building upstream meta CLI" {
            & dotnet build $metaCliProject -c $Configuration --nologo @stableBuildArgs
        }
    }
}

if (-not (Test-Path $metaCliDll)) {
    throw "Could not find upstream meta CLI at '$metaCliDll'. Build it first or omit -SkipMetaCliBuild."
}

$buildToolingProjects = -not $SkipToolingProjectBuild
$packLocalMetaPackages = -not $SkipPackLocalMetaPackages
$usingLocalMetaPackages = $buildToolingProjects -and $packLocalMetaPackages
if ($usingLocalMetaPackages) {
    $LocalPackageSource = Get-FullPath $LocalPackageSource
    $localNuGetPackages = Get-FullPath $localNuGetPackages
    $localNuGetConfig = Get-FullPath $localNuGetConfig

    Assert-PathInside -Path $LocalPackageSource -Root $script:RepoRoot -Description "Local meta package source"
    Assert-PathInside -Path $localNuGetPackages -Root $script:RepoRoot -Description "Local NuGet package cache"
    Assert-PathInside -Path $localNuGetConfig -Root $script:RepoRoot -Description "Local NuGet config"

    if ($PSCmdlet.ShouldProcess($LocalPackageSource, "Create local meta package source")) {
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
    }

    if ($PSCmdlet.ShouldProcess($metaCoreProject, "Pack local Meta.Core")) {
        Invoke-Checked "Packing local Meta.Core" {
            & dotnet pack $metaCoreProject -c $Configuration --nologo -o $LocalPackageSource @stableBuildArgs
        }
    }

    if ($PSCmdlet.ShouldProcess($metaAdaptersProject, "Pack local Meta.Adapters")) {
        Invoke-Checked "Packing local Meta.Adapters" {
            & dotnet pack $metaAdaptersProject -c $Configuration --nologo -o $LocalPackageSource @stableBuildArgs
        }
    }
}

foreach ($target in $selectedTargets) {
    if ($PSCmdlet.ShouldProcess($target.Name, "Regenerate C# tooling")) {
        Invoke-Checked "Generating $($target.Name) tooling" {
            & dotnet $metaCliDll generate csharp --workspace $target.WorkspacePath --out $target.OutputPath --tooling
        }
    }

    if (-not (Test-Path $target.ToolingProjectPath)) {
        throw "Expected tooling project was not found after generating '$($target.Name)': '$($target.ToolingProjectPath)'."
    }
}

if ($buildToolingProjects) {
    $builtProjects = New-Object 'System.Collections.Generic.HashSet[string]' ([System.StringComparer]::OrdinalIgnoreCase)
    $previousNuGetPackages = $env:NUGET_PACKAGES
    try {
        if ($usingLocalMetaPackages) {
            $env:NUGET_PACKAGES = $localNuGetPackages
        }

        foreach ($target in $selectedTargets) {
            if (-not $builtProjects.Add($target.ToolingProjectPath)) {
                continue
            }

            $buildArgs = @("build", $target.ToolingProjectPath, "--nologo")
            $buildArgs += $stableBuildArgs
            if ($usingLocalMetaPackages) {
                $buildArgs += "/p:RestoreConfigFile=$localNuGetConfig"
                $buildArgs += "/p:RestoreNoCache=true"
            }

            if ($PSCmdlet.ShouldProcess($target.ToolingProjectPath, "Build tooling project")) {
                Invoke-Checked "Building $($target.ToolingProjectPath)" {
                    & dotnet @buildArgs
                }
            }
        }
    }
    finally {
        $env:NUGET_PACKAGES = $previousNuGetPackages
    }
}
