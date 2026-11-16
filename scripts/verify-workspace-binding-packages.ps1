param(
    [Parameter(Mandatory = $true)][string]$FoundationFeed,
    [Parameter(Mandatory = $true)][string]$BiFeed,
    [Parameter(Mandatory = $true)][string]$Version,
    [Parameter(Mandatory = $true)][string]$OutputDirectory
)

$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$fixtureRoot = Join-Path $repoRoot 'Tests\PackageConsumer.WorkspaceBindings'
$foundationPath = [IO.Path]::GetFullPath($FoundationFeed)
$biPath = [IO.Path]::GetFullPath($BiFeed)
foreach ($feedPath in @($foundationPath, $biPath)) {
    if (-not (Test-Path -LiteralPath $feedPath -PathType Container)) {
        throw "Package feed does not exist: $feedPath"
    }
}
if ([string]::IsNullOrWhiteSpace($Version)) { throw 'Version is required.' }
if (-not [IO.Path]::IsPathRooted($OutputDirectory)) {
    throw 'OutputDirectory must be an absolute path outside a checkout.'
}
$outputRoot = [IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $outputRoot) { throw "OutputDirectory already exists: $outputRoot" }
$repoPrefix = $repoRoot.TrimEnd([IO.Path]::DirectorySeparatorChar, [IO.Path]::AltDirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
if ($outputRoot.StartsWith($repoPrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'OutputDirectory must be outside the repository.'
}
$ancestor = [IO.DirectoryInfo]::new([IO.Path]::GetDirectoryName($outputRoot))
while ($null -ne $ancestor) {
    if (Test-Path -LiteralPath (Join-Path $ancestor.FullName '.git')) {
        throw "OutputDirectory must be outside a Git checkout: $($ancestor.FullName)"
    }
    $ancestor = $ancestor.Parent
}

$projectName = 'PackageConsumer.WorkspaceBindings.csproj'
[xml]$fixture = Get-Content -LiteralPath (Join-Path $fixtureRoot $projectName) -Raw -Encoding UTF8
$packageReferences = @($fixture.SelectNodes('//PackageReference'))
if ($packageReferences.Count -ne 1 -or $packageReferences[0].Include -ne 'MetaTransform.Binding' -or
    $packageReferences[0].Version -ne '$(MetaBiConsumerVersion)' -or $fixture.SelectNodes('//ProjectReference').Count -ne 0) {
    throw 'The fixture must have exactly one direct MetaTransform.Binding package reference and no project references.'
}

New-Item -ItemType Directory -Path $outputRoot | Out-Null
$projectRoot = Join-Path $outputRoot 'consumer'
$packagesRoot = Join-Path $outputRoot 'packages'
$logsRoot = Join-Path $outputRoot 'logs'
$publishRoot = Join-Path $outputRoot 'publish'
$evidenceRoot = Join-Path $outputRoot 'evidence'
New-Item -ItemType Directory -Path $projectRoot, $packagesRoot, $logsRoot, $evidenceRoot | Out-Null
foreach ($name in @($projectName, 'Program.cs')) {
    Copy-Item -LiteralPath (Join-Path $fixtureRoot $name) -Destination (Join-Path $projectRoot $name)
}
$configPath = Join-Path $outputRoot 'NuGet.Config'
$escapedFoundation = [Security.SecurityElement]::Escape($foundationPath)
$escapedBi = [Security.SecurityElement]::Escape($biPath)
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="foundation" value="$escapedFoundation" />
    <add key="bi" value="$escapedBi" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <fallbackPackageFolders><clear /></fallbackPackageFolders>
</configuration>
"@ | Set-Content -LiteralPath $configPath -Encoding UTF8

function Invoke-ConsumerDotnet {
    param([string]$Stage, [string[]]$Arguments)

    Write-Host ('> dotnet ' + ($Arguments -join ' '))
    & dotnet @Arguments 2>&1 | Tee-Object -FilePath (Join-Path $logsRoot ($Stage + '.txt'))
    if ($LASTEXITCODE -ne 0) { throw "$Stage failed with exit code $LASTEXITCODE; see $logsRoot" }
}

$projectPath = Join-Path $projectRoot $projectName
$properties = @(
    "-p:MetaBiConsumerVersion=$Version",
    '-p:ImportDirectoryBuildProps=false',
    '-p:ImportDirectoryBuildTargets=false',
    '-p:ManagePackageVersionsCentrally=false'
)
Push-Location $projectRoot
try {
    Invoke-ConsumerDotnet 'restore' (@('restore', $projectPath, '--configfile', $configPath,
        '--packages', $packagesRoot, '--disable-parallel', '--nologo') + $properties)
    Invoke-ConsumerDotnet 'build' (@('build', $projectPath, '-c', 'Release', '--no-restore', '--nologo', '-m:1', '-nr:false') + $properties)
    $buildDll = Join-Path $projectRoot 'bin\Release\net8.0\PackageConsumer.WorkspaceBindings.dll'
    Invoke-ConsumerDotnet 'run-build' @($buildDll, (Join-Path $evidenceRoot 'build'))
    Invoke-ConsumerDotnet 'publish' (@('publish', $projectPath, '-c', 'Release', '--no-restore', '--no-build',
        '--nologo', '--output', $publishRoot, '-m:1', '-nr:false') + $properties)
    Invoke-ConsumerDotnet 'run-publish' @((Join-Path $publishRoot 'PackageConsumer.WorkspaceBindings.dll'),
        (Join-Path $evidenceRoot 'publish'))
    @(
        'PASS standalone workspace-binding package consumer: build and published executions.',
        "Direct package: MetaTransform.Binding $Version",
        "Foundation feed: $foundationPath",
        "BI feed: $biPath",
        "Private package cache: $packagesRoot",
        "Build evidence: $(Join-Path $evidenceRoot 'build')",
        "Publish evidence: $(Join-Path $evidenceRoot 'publish')",
        'No repository projects built; no SQL database execution.'
    ) | Set-Content -LiteralPath (Join-Path $outputRoot 'Result.txt') -Encoding UTF8
    Write-Host "PASS: $outputRoot"
}
finally { Pop-Location }
