[CmdletBinding()]
param(
    # Build the test host once beforehand. This probe never builds or changes a binary.
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$probeRoot = Join-Path $repoRoot ('artifacts/custom-conversion-probe/' + [Guid]::NewGuid().ToString('N'))
$testBinary = Join-Path $repoRoot "MetaSchemaAdapter/Tests/bin/$Configuration/net8.0/MetaSchema.Adapter.Tests.dll"
if (!(Test-Path -LiteralPath $testBinary)) {
    throw "Build MetaSchemaAdapter/Tests/MetaSchema.Adapter.Tests.csproj in $Configuration first."
}

function Invoke-Meta {
    & meta @args
    if ($LASTEXITCODE -ne 0) { throw "meta command failed: $args" }
}

$conversions = Join-Path $probeRoot 'ConversionWorkspace'
$source = Join-Path $probeRoot 'SourceSchema'
$target = Join-Path $probeRoot 'TargetSchema'
$aliasTarget = Join-Path $probeRoot 'AliasTargetSchema'
New-Item -ItemType Directory -Path (Join-Path $probeRoot 'Data') -Force | Out-Null
Set-Content -LiteralPath (Join-Path $probeRoot 'Data/Wacky.tsv') -Value "WackyValue`nwx321" -Encoding utf8

# All workspace authoring uses the existing modeled CLI. Sanctioned instances
# seed this private copy; they are not treated as an immutable catalogue.
Invoke-Meta create --source-workspace (Join-Path $repoRoot 'MetaDataTypeConversion/Workspace') --with-instances --xml $conversions
Invoke-Meta insert SqlConversionSnippet probe:snippet:reversed --set 'TransformScript=CAST(REVERSE(SUBSTRING($(value), 3, 8)) AS $(type))' --workspace $conversions
Invoke-Meta insert ConversionImplementation probe:implementation:reversed --set Name=ReversedDigits --set SqlConversionSnippet=probe:snippet:reversed --workspace $conversions
Invoke-Meta insert SqlConversionParameterBinding probe:binding:value --set Name=value --set SqlConversionSnippet=probe:snippet:reversed --set SqlConversionParameter=MetaDataTypeConversion:parameter:source-expression --workspace $conversions
Invoke-Meta insert SqlConversionParameterBinding probe:binding:type --set Name=type --set SqlConversionSnippet=probe:snippet:reversed --set SqlConversionParameter=MetaDataTypeConversion:parameter:destination-type --workspace $conversions
Invoke-Meta insert DataTypeMapping probe:mapping:reversed --set SourceDataTypeId=weird:type:ReversedDigits --set TargetDataTypeId=sqlserver:type:int --set ConversionImplementation=probe:implementation:reversed --workspace $conversions

foreach ($schemaWorkspace in @($source, $target)) {
    Invoke-Meta create --source-workspace (Join-Path $repoRoot 'MetaSchema/Workspaces/MetaSchema') --xml $schemaWorkspace
}
Invoke-Meta insert System source:system --set Name=WackyFiles --workspace $source
Invoke-Meta insert Schema source:schema --set Name=files --set System=source:system --workspace $source
Invoke-Meta insert SchemaObject source:object --set Name=Wacky --set Schema=source:schema --workspace $source
Invoke-Meta insert Table source:table --set SchemaObject=source:object --workspace $source
Invoke-Meta insert Field source:field --set Name=WackyValue --set Ordinal=0 --set MetaDataTypeId=weird:type:ReversedDigits --set IsNullable=false --set SchemaObject=source:object --workspace $source
Invoke-Meta insert System target:system --set Name=SqlTarget --workspace $target
Invoke-Meta insert Schema target:schema --set Name=dbo --set System=target:system --workspace $target
Invoke-Meta insert SchemaObject target:object --set Name=Numbers --set Schema=target:schema --workspace $target
Invoke-Meta insert Table target:table --set SchemaObject=target:object --workspace $target
Invoke-Meta insert Field target:field --set Name=WackyValue --set Ordinal=0 --set MetaDataTypeId=sqlserver:type:int --set IsNullable=false --set SchemaObject=target:object --workspace $target
Invoke-Meta create --source-workspace $target --with-instances --xml $aliasTarget
Invoke-Meta instance update Field target:field --set Name=Value --workspace $aliasTarget

$originalHashes = @{}
Get-ChildItem -LiteralPath (Split-Path $testBinary) -Filter '*.dll' | ForEach-Object {
    $originalHashes[$_.FullName] = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash
}
$previousRoot = $env:META_BI_CUSTOM_CONVERSION_PROBE_ROOT
try {
    $env:META_BI_CUSTOM_CONVERSION_PROBE_ROOT = $probeRoot
    foreach ($expected in @(123, 246)) {
        if ($expected -eq 246) {
            # The only behavior change between runs is instance data.
            Invoke-Meta instance update SqlConversionSnippet probe:snippet:reversed --set 'TransformScript=CAST(REVERSE(SUBSTRING($(value), 3, 8)) AS $(type)) * 2' --workspace $conversions
        }
        Set-Content -LiteralPath (Join-Path $probeRoot 'Expected.txt') -Value $expected -Encoding utf8
        & dotnet test $testBinary --nologo --filter FullyQualifiedName~CustomConversionBindingProbeTests --logger 'console;verbosity=normal'
        if ($LASTEXITCODE -ne 0) { throw "Probe assertions failed for expected value $expected. See $probeRoot." }
        Copy-Item -LiteralPath (Join-Path $probeRoot 'ProbeResult.txt') -Destination (Join-Path $probeRoot "ProbeResult-$expected.txt")
    }
    foreach ($binary in $originalHashes.Keys) {
        if ((Get-FileHash -LiteralPath $binary -Algorithm SHA256).Hash -ne $originalHashes[$binary]) {
            throw "Binary changed during instance-only probe: $binary"
        }
    }
    "All DLL hashes unchanged. Evidence: $probeRoot"
}
finally {
    $env:META_BI_CUSTOM_CONVERSION_PROBE_ROOT = $previousRoot
}
