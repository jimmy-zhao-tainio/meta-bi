param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path,
    [string]$ExtraCliPath = "",
    [switch]$NoNormalize
)

$ErrorActionPreference = "Stop"

$demoPaths = @(
    (Join-Path $PSScriptRoot "BusinessDataVaultCliIntegration"),
    (Join-Path $PSScriptRoot "MetaPipelineSqlServerCliIntegration"),
    (Join-Path $PSScriptRoot "MetaTransformBindingCliIntegration"),
    (Join-Path $PSScriptRoot "MetaTransformScriptCliIntegration"),
    (Join-Path $PSScriptRoot "MetaTransformScriptReferenceCorpusCliIntegration"),
    (Join-Path $PSScriptRoot "RawDataVaultCliIntegration"),
    (Join-Path $PSScriptRoot "RawDataVaultFromMetaSchemaCliIntegration")
)

foreach ($demoPath in $demoPaths) {
    $runCmdPath = Join-Path $demoPath "run.cmd"
    if (-not (Test-Path $runCmdPath)) {
        throw "Missing run.cmd at '$runCmdPath'."
    }

    $captureCommand = if ([string]::IsNullOrWhiteSpace($ExtraCliPath)) {
        "cd /d $demoPath && run.cmd > run.output 2>&1"
    }
    else {
        "set ""PATH=$ExtraCliPath;%PATH%"" && cd /d $demoPath && run.cmd > run.output 2>&1"
    }

    Write-Host "Capturing output for $demoPath"
    & cmd /c $captureCommand
    if ($LASTEXITCODE -ne 0) {
        throw "run.cmd failed in '$demoPath' with exit code $LASTEXITCODE."
    }

    if (-not $NoNormalize) {
        $outputPath = Join-Path $demoPath "run.output"
        $lines = Get-Content -Path $outputPath
        $normalized = foreach ($line in $lines) {
            if ($line -match '^[A-Za-z]:\\.*?>\s*(.*)$') {
                "> $($matches[1])"
                continue
            }

            if ($line -match '^(call\s+|pushd\s+|popd\s+|meta-[^\s]+\s+|\.\\run\.cmd\s*$)') {
                if ($line -match '^>\s*') {
                    "> $($line -replace '^>\s*', '')"
                }
                else {
                    "> $line"
                }
                continue
            }

            if ($line -match '^>\S') {
                $line -replace '^>(\S)', '> $1'
                continue
            }

            $line
        }

        Set-Content -Path $outputPath -Value $normalized
    }
}

Write-Host "Done. run.output files updated."
