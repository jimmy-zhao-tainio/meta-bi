param(
    [Parameter(Mandatory = $true)]
    [string]$WorkspacePath,
    [Parameter(Mandatory = $true)]
    [string]$OutFile
)

$ErrorActionPreference = "Stop"

$instancesPath = Join-Path $WorkspacePath "instances"
if (-not (Test-Path $instancesPath)) {
    throw "Workspace instances folder not found: $instancesPath"
}

$ids = @()

$fkPath = Join-Path $instancesPath "ImpliedForeignKeyMissingReference.xml"
if (Test-Path $fkPath) {
    [xml]$fkXml = Get-Content -Raw $fkPath
    $fkRows = @($fkXml.MetaDataQuality.ImpliedForeignKeyMissingReferenceList.ImpliedForeignKeyMissingReference)
    $ids += $fkRows | ForEach-Object { $_.DataQualityCandidateId }
}

$ukPath = Join-Path $instancesPath "ImpliedUniqueKeyViolation.xml"
if (Test-Path $ukPath) {
    [xml]$ukXml = Get-Content -Raw $ukPath
    $ukRows = @($ukXml.MetaDataQuality.ImpliedUniqueKeyViolationList.ImpliedUniqueKeyViolation)
    $ids += $ukRows | ForEach-Object { $_.DataQualityCandidateId }
}

$ids = @($ids | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Sort-Object -Unique)
if ($ids.Count -eq 0) {
    throw "No implied candidate ids found in workspace '$WorkspacePath'."
}

$ids | Set-Content -Path $OutFile
Write-Host ("Implied candidate ids: " + ($ids -join ", "))
