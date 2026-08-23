[CmdletBinding()]
param(
    [ValidateSet("Unit", "Integration")]
    [string]$Profile = "Unit",

    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [ValidateRange(1, 12)]
    [int]$MaxConcurrency = 4
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))

function ConvertTo-ProcessArgument
{
    param(
        [AllowEmptyString()]
        [Parameter(Mandatory = $true)]
        [string]$Value
    )

    if ($Value.Length -gt 0 -and $Value -notmatch '[\s"]')
    {
        return $Value
    }

    $escaped = [regex]::Replace($Value, '(\\*)"', '$1$1\"')
    $escaped = [regex]::Replace($escaped, '(\\+)$', '$1$1')
    return '"' + $escaped + '"'
}

function New-TestCase
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,

        [Parameter(Mandatory = $true)]
        [string]$Project
    )

    return [pscustomobject]@{
        Name = $Name
        Project = $Project
    }
}

function Start-TestCase
{
    param(
        [Parameter(Mandatory = $true)]
        [psobject]$TestCase
    )

    $projectPath = [IO.Path]::GetFullPath((Join-Path $repoRoot $TestCase.Project))
    $resultDirectoryName = $TestCase.Name -replace '[^A-Za-z0-9.-]', '-'
    $resultDirectory = Join-Path $resultsRoot $resultDirectoryName
    New-Item -ItemType Directory -Path $resultDirectory | Out-Null
    $resultPath = Join-Path $resultDirectory "results.trx"
    $arguments = @(
        "test",
        $projectPath,
        "--configuration",
        $Configuration,
        "--no-build",
        "--no-restore",
        "--nologo",
        "--logger",
        "trx;LogFileName=results.trx",
        "--results-directory",
        $resultDirectory,
        "-m:1",
        "-nr:false"
    )
    $startInfo = New-Object Diagnostics.ProcessStartInfo
    $startInfo.FileName = "dotnet"
    $startInfo.Arguments = ($arguments | ForEach-Object { ConvertTo-ProcessArgument $_ }) -join " "
    $startInfo.WorkingDirectory = $repoRoot
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true

    $process = New-Object Diagnostics.Process
    $process.StartInfo = $startInfo
    if (-not $process.Start())
    {
        $process.Dispose()
        throw "Could not start test project '$($TestCase.Name)'."
    }

    Write-Host "[START] $($TestCase.Name)"
    return [pscustomobject]@{
        TestCase = $TestCase
        Process = $process
        StandardOutput = $process.StandardOutput.ReadToEndAsync()
        StandardError = $process.StandardError.ReadToEndAsync()
        ResultPath = $resultPath
        Stopwatch = [Diagnostics.Stopwatch]::StartNew()
    }
}

function Stop-TestProcessTree
{
    param(
        [Parameter(Mandatory = $true)]
        [Diagnostics.Process]$Process
    )

    try
    {
        if (-not $Process.HasExited)
        {
            & taskkill.exe /PID $Process.Id /T /F 2>&1 | Out-Null
        }
    }
    catch
    {
        if (-not $Process.HasExited)
        {
            $Process.Kill()
        }
    }
}

function Format-Elapsed
{
    param(
        [Parameter(Mandatory = $true)]
        [timespan]$Elapsed
    )

    return "{0:00}:{1:00}.{2:0}" -f [math]::Floor($Elapsed.TotalMinutes), $Elapsed.Seconds, [math]::Floor($Elapsed.Milliseconds / 100)
}

function Get-TestExecutionResult
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$ResultPath
    )

    if (-not (Test-Path -LiteralPath $ResultPath -PathType Leaf))
    {
        throw "Test result file was not produced: $ResultPath"
    }

    [xml]$resultDocument = Get-Content -LiteralPath $ResultPath -Raw
    $testResults = @($resultDocument.TestRun.Results.UnitTestResult)
    if ($testResults.Count -eq 0)
    {
        throw "Test result file contains no unit-test results: $ResultPath"
    }

    $milliseconds = 0.0
    foreach ($testResult in $testResults)
    {
        $milliseconds += [timespan]::Parse(
            [string]$testResult.duration,
            [Globalization.CultureInfo]::InvariantCulture).TotalMilliseconds
    }

    return [pscustomobject]@{
        Count = $testResults.Count
        ExecutionMilliseconds = [long][math]::Ceiling($milliseconds)
    }
}

$unitTests = @(
    (New-TestCase "Meta BI unit" "Tests\Unit\MetaBi.Unit.Tests.csproj")
)

$integrationTests = @(
    (New-TestCase "Analytics integration" "MetaAnalytics\Tests\MetaAnalytics.Tests.csproj"),
    (New-TestCase "Data Quality integration" "MetaDataQuality\Tests\MetaDataQuality.Tests.csproj"),
    (New-TestCase "Data type integration" "MetaDataType\Tests\MetaDataType.Tests.csproj"),
    (New-TestCase "Data type conversion integration" "MetaDataTypeConversion\Tests\MetaDataTypeConversion.Tests.csproj"),
    (New-TestCase "Data Vault integration" "MetaDataVault\Tests\MetaDataVault.Tests.csproj"),
    (New-TestCase "Data warehouse integration" "MetaDataWarehouse\Tests\MetaDataWarehouse.Tests.csproj"),
    (New-TestCase "Multi-dimensional integration" "MetaMultiDimensional\Tests\MetaMultiDimensional.Tests.csproj"),
    (New-TestCase "Orchestration integration" "MetaOrchestration\Tests\MetaOrchestration.Tests.csproj"),
    (New-TestCase "Pipeline integration" "MetaPipeline\Tests\MetaPipeline.Tests.csproj"),
    (New-TestCase "Schema integration" "MetaSchema\Tests\MetaSchema.Tests.csproj"),
    (New-TestCase "Schema adapter integration" "MetaSchemaAdapter\Tests\MetaSchema.Adapter.Tests.csproj"),
    (New-TestCase "SQL integration" "MetaSql\Tests\MetaSql.Tests.csproj"),
    (New-TestCase "Tabular integration" "MetaTabular\Tests\MetaTabular.Tests.csproj"),
    (New-TestCase "TransformScript integration" "MetaTransform\Script\Tests\MetaTransformScript.Tests.csproj"),
    (New-TestCase "Transform surface integration" "Tests\TransformSurfaceContracts\MetaBi.TransformSurfaceContracts.Tests.csproj")
)

$testCases = if ($Profile -eq "Unit") { $unitTests } else { $integrationTests }

foreach ($testCase in $testCases)
{
    $projectPath = [IO.Path]::GetFullPath((Join-Path $repoRoot $testCase.Project))
    if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf))
    {
        throw "Test project does not exist: $projectPath"
    }
}

$tempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
$resultsRoot = [IO.Path]::GetFullPath((Join-Path $tempRoot ("meta-bi-test-results-" + [guid]::NewGuid().ToString("N"))))
if (-not $resultsRoot.StartsWith($tempRoot, [StringComparison]::OrdinalIgnoreCase))
{
    throw "Resolved test-results directory escaped the system temporary directory: $resultsRoot"
}
New-Item -ItemType Directory -Path $resultsRoot | Out-Null

$pending = New-Object 'Collections.Generic.Queue[object]'
foreach ($testCase in $testCases)
{
    $pending.Enqueue($testCase)
}

$active = New-Object Collections.ArrayList
$failures = New-Object Collections.ArrayList
$totalTests = 0
$totalTestExecutionMilliseconds = 0
$unitAssemblyExecutionBudgetMilliseconds = 1000
$overallStopwatch = [Diagnostics.Stopwatch]::StartNew()
try
{
    while ($pending.Count -gt 0 -or $active.Count -gt 0)
    {
        while ($failures.Count -eq 0 -and $pending.Count -gt 0 -and $active.Count -lt $MaxConcurrency)
        {
            [void]$active.Add((Start-TestCase $pending.Dequeue()))
        }

        if ($failures.Count -gt 0 -and $active.Count -eq 0)
        {
            break
        }

        $completed = @($active | Where-Object { $_.Process.HasExited })
        if ($completed.Count -eq 0)
        {
            Start-Sleep -Milliseconds 50
            continue
        }

        foreach ($run in $completed)
        {
            $run.Process.WaitForExit()
            $run.Stopwatch.Stop()
            $standardOutput = $run.StandardOutput.GetAwaiter().GetResult()
            $standardError = $run.StandardError.GetAwaiter().GetResult()
            $exitCode = $run.Process.ExitCode
            $run.Process.Dispose()
            [void]$active.Remove($run)

            $testResult = if ($exitCode -eq 0)
            {
                Get-TestExecutionResult $run.ResultPath
            }
            else
            {
                $null
            }
            if ($exitCode -eq 0 -and $null -ne $testResult)
            {
                $testCount = $testResult.Count
                $testExecutionMilliseconds = $testResult.ExecutionMilliseconds
                $totalTests += $testCount
                $totalTestExecutionMilliseconds += $testExecutionMilliseconds
                Write-Host "[PASS] $($run.TestCase.Name) - $testCount test(s), $testExecutionMilliseconds ms summed test execution, $(Format-Elapsed $run.Stopwatch.Elapsed) process"
                if ($Profile -eq "Unit" -and $testExecutionMilliseconds -ge $unitAssemblyExecutionBudgetMilliseconds)
                {
                    [void]$failures.Add([pscustomobject]@{
                        Name = "$($run.TestCase.Name) test execution budget"
                        StandardOutput = "$testExecutionMilliseconds ms must remain below the $unitAssemblyExecutionBudgetMilliseconds ms unit assembly budget."
                        StandardError = ""
                    })
                }
            }
            else
            {
                Write-Host "[FAIL] $($run.TestCase.Name) - exit $exitCode after $(Format-Elapsed $run.Stopwatch.Elapsed)" -ForegroundColor Red
                [void]$failures.Add([pscustomobject]@{
                    Name = $run.TestCase.Name
                    StandardOutput = $standardOutput
                    StandardError = $standardError
                })
            }
        }
    }
}
finally
{
    foreach ($run in @($active))
    {
        Stop-TestProcessTree $run.Process
        $run.Process.Dispose()
    }

    if (Test-Path -LiteralPath $resultsRoot -PathType Container)
    {
        Remove-Item -LiteralPath $resultsRoot -Recurse -Force
    }
}

$overallStopwatch.Stop()
if ($failures.Count -gt 0)
{
    foreach ($failure in $failures)
    {
        Write-Host ""
        Write-Host "===== $($failure.Name) stdout =====" -ForegroundColor Red
        Write-Host $failure.StandardOutput
        if (-not [string]::IsNullOrWhiteSpace($failure.StandardError))
        {
            Write-Host "===== $($failure.Name) stderr =====" -ForegroundColor Red
            Write-Host $failure.StandardError
        }
    }

    Write-Host "[FAILED] $Profile in $(Format-Elapsed $overallStopwatch.Elapsed)." -ForegroundColor Red
    exit 1
}

Write-Host "[PASSED] $Profile - $totalTests test(s), $totalTestExecutionMilliseconds ms summed test execution, $(Format-Elapsed $overallStopwatch.Elapsed) process."
