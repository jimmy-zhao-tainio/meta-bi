param(
    [Parameter(Mandatory = $true)][string]$FoundationFeed,
    [Parameter(Mandatory = $true)][string]$BiFeed,
    [Parameter(Mandatory = $true)][string]$Version,
    [Parameter(Mandatory = $true)][string]$OutputDirectory
)

$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$fixtureRoot = Join-Path $repoRoot 'Tests\PackageConsumer.MigratedOwners'
$foundationPath = [IO.Path]::GetFullPath($FoundationFeed)
$biPath = [IO.Path]::GetFullPath($BiFeed)
foreach ($feedPath in @($foundationPath, $biPath)) {
    if (-not (Test-Path -LiteralPath $feedPath -PathType Container)) { throw "Package feed does not exist: $feedPath" }
}
if ([string]::IsNullOrWhiteSpace($Version)) { throw 'Version is required.' }
if (-not [IO.Path]::IsPathRooted($OutputDirectory)) { throw 'OutputDirectory must be an absolute path outside a checkout.' }
$outputRoot = [IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $outputRoot) { throw "OutputDirectory already exists: $outputRoot" }
$repoPrefix = $repoRoot.TrimEnd([IO.Path]::DirectorySeparatorChar, [IO.Path]::AltDirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
if ($outputRoot.StartsWith($repoPrefix, [StringComparison]::OrdinalIgnoreCase)) { throw 'OutputDirectory must be outside the repository.' }
$ancestor = [IO.DirectoryInfo]::new([IO.Path]::GetDirectoryName($outputRoot))
while ($null -ne $ancestor) {
    if (Test-Path -LiteralPath (Join-Path $ancestor.FullName '.git')) { throw "OutputDirectory must be outside a Git checkout: $($ancestor.FullName)" }
    $ancestor = $ancestor.Parent
}

$consumerProjectName = 'PackageConsumer.MigratedOwners.csproj'
[xml]$consumerXml = Get-Content -LiteralPath (Join-Path $fixtureRoot "Consumer\$consumerProjectName") -Encoding UTF8 -Raw
$consumerReferences = @($consumerXml.SelectNodes('//PackageReference'))
if ($consumerReferences.Count -ne 1 -or $consumerReferences[0].Include -ne 'MetaBi.PackageConsumer.MigratedOwnerReferences' -or
    $consumerReferences[0].Version -ne '$(MetaBiConsumerVersion)' -or $consumerXml.SelectNodes('//ProjectReference').Count) {
    throw 'The executable must consume only the test bridge package and no project references.'
}
[xml]$bridgeXml = Get-Content -LiteralPath (Join-Path $fixtureRoot 'OwnerReferences\OwnerReferences.csproj') -Encoding UTF8 -Raw
$ownerIds = @($bridgeXml.SelectNodes('//PackageReference') | ForEach-Object {
    if ($_.Version -ne '$(MetaBiConsumerVersion)') { throw 'Every owner package must use MetaBiConsumerVersion.' }
    $_.Include
})
if ($ownerIds.Count -ne 17 -or $bridgeXml.SelectNodes('//ProjectReference').Count) { throw 'The test bridge must reference the 17 migrated owner packages only.' }

New-Item -ItemType Directory -Path $outputRoot | Out-Null
$consumerRoot = Join-Path $outputRoot 'consumer'
$bridgeRoot = Join-Path $outputRoot 'bridge'
$bridgeFeed = Join-Path $outputRoot 'bridge-feed'
$packagesRoot = Join-Path $outputRoot 'packages'
$logsRoot = Join-Path $outputRoot 'logs'
$publishRoot = Join-Path $outputRoot 'publish'
$evidenceRoot = Join-Path $outputRoot 'evidence'
New-Item -ItemType Directory -Path $consumerRoot, $bridgeRoot, $bridgeFeed, $packagesRoot, $logsRoot, $evidenceRoot | Out-Null
Copy-Item -LiteralPath (Join-Path $fixtureRoot 'OwnerReferences\OwnerReferences.csproj') -Destination $bridgeRoot
foreach ($name in @($consumerProjectName, 'Program.cs')) {
    Copy-Item -LiteralPath (Join-Path $fixtureRoot "Consumer\$name") -Destination $consumerRoot
}
$configPath = Join-Path $outputRoot 'NuGet.Config'
$escapedFoundation = [Security.SecurityElement]::Escape($foundationPath)
$escapedBi = [Security.SecurityElement]::Escape($biPath)
$escapedBridge = [Security.SecurityElement]::Escape($bridgeFeed)
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="foundation" value="$escapedFoundation" />
    <add key="bi" value="$escapedBi" />
    <add key="test-bridge" value="$escapedBridge" />
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
$properties = @(
    "-p:MetaBiConsumerVersion=$Version",
    '-p:ImportDirectoryBuildProps=false',
    '-p:ImportDirectoryBuildTargets=false',
    '-p:ManagePackageVersionsCentrally=false'
)
$bridgeProject = Join-Path $bridgeRoot 'OwnerReferences.csproj'
$consumerProject = Join-Path $consumerRoot $consumerProjectName
Push-Location $outputRoot
try {
    Invoke-ConsumerDotnet 'restore-bridge' (@('restore', $bridgeProject, '--configfile', $configPath,
        '--packages', $packagesRoot, '--disable-parallel', '--nologo') + $properties)
    Invoke-ConsumerDotnet 'build-bridge' (@('build', $bridgeProject, '-c', 'Release', '--no-restore', '--nologo', '-m:1', '-nr:false') + $properties)
    Invoke-ConsumerDotnet 'pack-bridge' (@('pack', $bridgeProject, '-c', 'Release', '--no-build', '--no-restore',
        '--nologo', '--output', $bridgeFeed, "-p:PackageVersion=$Version", '-m:1', '-nr:false') + $properties)
    Invoke-ConsumerDotnet 'restore-consumer' (@('restore', $consumerProject, '--configfile', $configPath,
        '--packages', $packagesRoot, '--disable-parallel', '--nologo') + $properties)
    $assets = Get-Content -LiteralPath (Join-Path $consumerRoot 'obj\project.assets.json') -Encoding UTF8 -Raw | ConvertFrom-Json
    $resolved = @($assets.libraries.PSObject.Properties.Name)
    foreach ($id in $ownerIds + @('MetaSql.CSharp', 'MetaSql.Extractors.SqlServer', 'MetaOrchestration.WorkerProtocol')) {
        if ($resolved -notcontains "$id/$Version") { throw "Expected migrated owner package missing from restored consumer: $id/$Version" }
    }
    $resolved | Sort-Object | Set-Content -LiteralPath (Join-Path $logsRoot 'ResolvedPackages.txt') -Encoding UTF8
    Invoke-ConsumerDotnet 'build-consumer' (@('build', $consumerProject, '-c', 'Release', '--no-restore', '--nologo', '-m:1', '-nr:false') + $properties)
    $dllName = 'PackageConsumer.MigratedOwners.dll'
    Invoke-ConsumerDotnet 'run-build' @((Join-Path $consumerRoot "bin\Release\net8.0\$dllName"), (Join-Path $evidenceRoot 'build'))
    Invoke-ConsumerDotnet 'publish-consumer' (@('publish', $consumerProject, '-c', 'Release', '--no-restore', '--no-build',
        '--nologo', '--output', $publishRoot, '-m:1', '-nr:false') + $properties)
    Invoke-ConsumerDotnet 'run-publish' @((Join-Path $publishRoot $dllName), (Join-Path $evidenceRoot 'publish'))
    @(
        'PASS migrated owner package consumer: build and published executions.',
        "BI version: $Version",
        'Executable directly references only the external test bridge NuGet package.',
        'Bridge references 17 owner packages; 20 BI packages resolve transitively; 21 actual authored model workspaces are exercised.',
        "Foundation feed: $foundationPath",
        "BI feed: $biPath",
        "Private package cache: $packagesRoot",
        "Build evidence: $(Join-Path $evidenceRoot 'build')",
        "Publish evidence: $(Join-Path $evidenceRoot 'publish')",
        'No repository ProjectReferences or repository builds; no SQL database execution.'
    ) | Set-Content -LiteralPath (Join-Path $outputRoot 'Result.txt') -Encoding UTF8
    Write-Host "PASS: $outputRoot"
}
finally { Pop-Location }
