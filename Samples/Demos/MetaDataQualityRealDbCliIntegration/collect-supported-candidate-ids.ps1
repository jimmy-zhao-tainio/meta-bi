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

$families = @(
    @{ File = "ImpliedForeignKeyMissingReference.xml"; XPath = "/MetaDataQuality/ImpliedForeignKeyMissingReferenceList/ImpliedForeignKeyMissingReference" },
    @{ File = "ImpliedUniqueKeyViolation.xml"; XPath = "/MetaDataQuality/ImpliedUniqueKeyViolationList/ImpliedUniqueKeyViolation" },
    @{ File = "MinorityJoinPattern.xml"; XPath = "/MetaDataQuality/MinorityJoinPatternList/MinorityJoinPattern" },
    @{ File = "IncompleteCompositeJoin.xml"; XPath = "/MetaDataQuality/IncompleteCompositeJoinList/IncompleteCompositeJoin" },
    @{ File = "SuspiciousExtraJoinPredicate.xml"; XPath = "/MetaDataQuality/SuspiciousExtraJoinPredicateList/SuspiciousExtraJoinPredicate" },
    @{ File = "InnerJoinAgainstUsuallyOptionalRelationship.xml"; XPath = "/MetaDataQuality/InnerJoinAgainstUsuallyOptionalRelationshipList/InnerJoinAgainstUsuallyOptionalRelationship" },
    @{ File = "LeftJoinAgainstUsuallyMandatoryRelationship.xml"; XPath = "/MetaDataQuality/LeftJoinAgainstUsuallyMandatoryRelationshipList/LeftJoinAgainstUsuallyMandatoryRelationship" }
)

foreach ($family in $families) {
    $path = Join-Path $instancesPath $family.File
    if (-not (Test-Path $path)) {
        continue
    }

    [xml]$xml = Get-Content -Raw $path
    $rows = @($xml.SelectNodes($family.XPath))
    foreach ($row in $rows) {
        $candidateId = [string]$row.DataQualityCandidateId
        if (-not [string]::IsNullOrWhiteSpace($candidateId)) {
            $ids += $candidateId
        }
    }
}

$ids = @($ids | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Sort-Object -Unique)
if ($ids.Count -eq 0) {
    throw "No SQL-output-capable candidate ids found in workspace '$WorkspacePath'."
}

$ids | Set-Content -Path $OutFile
Write-Host ("SQL-output candidate ids: " + ($ids -join ", "))
