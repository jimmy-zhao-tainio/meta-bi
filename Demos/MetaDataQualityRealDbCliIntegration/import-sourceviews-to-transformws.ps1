param(
    [Parameter(Mandatory = $true)]
    [string]$SourceViewsPath,
    [Parameter(Mandatory = $true)]
    [string]$NewWorkspacePath
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $SourceViewsPath)) {
    throw "Source views path not found: $SourceViewsPath"
}

$files = Get-ChildItem -Path $SourceViewsPath -Filter *.sql -File | Sort-Object Name
if ($files.Count -eq 0) {
    throw "No .sql files found under '$SourceViewsPath'."
}

$workspaceInitialized = $false
foreach ($file in $files) {
    $sql = Get-Content -Raw $file.FullName
    $match = [regex]::Match(
        $sql,
        '(?is)\bCREATE\s+VIEW\s+\[\s*(?<schema>[^\]]+)\s*\]\s*\.\s*\[\s*(?<name>[^\]]+)\s*\]'
    )

    if (-not $match.Success) {
        throw "Could not parse CREATE VIEW schema/name from '$($file.FullName)'."
    }

    $target = "$($match.Groups['schema'].Value).$($match.Groups['name'].Value)"

    if (-not $workspaceInitialized) {
        & meta-transform-script from sql-file --path $file.FullName --target $target --new-workspace $NewWorkspacePath
        if ($LASTEXITCODE -ne 0) {
            exit $LASTEXITCODE
        }

        $workspaceInitialized = $true
        continue
    }

    & meta-transform-script from sql-file --path $file.FullName --target $target --workspace $NewWorkspacePath
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}

Write-Host "Imported $($files.Count) source views into MetaTransformScript workspace."
