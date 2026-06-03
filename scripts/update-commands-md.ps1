param(
    [string] $OutputPath = "docs/commands.md",
    [string[]] $CliNames = @(
        "meta-schema",
        "meta-data-type",
        "meta-data-type-conversion",
        "meta-sql",
        "meta-datavault-raw",
        "meta-datavault-business",
        "meta-transform-script",
        "meta-transform-binding",
        "meta-data-quality",
        "meta-pipeline",
        "meta-orchestration",
        "meta-data-warehouse",
        "meta-analytics",
        "meta-convert",
        "meta-tabular",
        "meta-multi-dimensional"
    )
)

$ErrorActionPreference = "Stop"

function Resolve-CommandPath {
    param([string] $CommandName)

    $command = Get-Command $CommandName -ErrorAction SilentlyContinue
    if ($null -eq $command) {
        return $null
    }

    return $command.Source
}

function Invoke-CliHelp {
    param(
        [string] $CommandPath,
        [string] $Arguments
    )

    $startInfo = [System.Diagnostics.ProcessStartInfo]::new()
    $startInfo.FileName = $CommandPath
    $startInfo.Arguments = $Arguments
    $startInfo.WorkingDirectory = (Get-Location).Path
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true

    $process = [System.Diagnostics.Process]::Start($startInfo)
    if ($null -eq $process) {
        throw "Could not start '$CommandPath $Arguments'."
    }

    $stdout = $process.StandardOutput.ReadToEnd()
    $stderr = $process.StandardError.ReadToEnd()
    $process.WaitForExit()

    $text = ($stdout + $stderr).TrimEnd()
    return [PSCustomObject]@{
        ExitCode = $process.ExitCode
        Text = $text
    }
}

function Get-CommandNamesFromHelp {
    param([string] $HelpText)

    $names = New-Object System.Collections.Generic.List[string]
    $inCommands = $false

    foreach ($line in ($HelpText -split "`r?`n")) {
        $trimmed = $line.Trim()
        if ($trimmed -eq "Commands:") {
            $inCommands = $true
            continue
        }

        if (-not $inCommands) {
            continue
        }

        if ($trimmed.Length -eq 0) {
            continue
        }

        if ($trimmed -match "^(Usage:|Notes:|Next:|Examples?:|Command:|Commands:)") {
            break
        }

        if ($line -match "^\s{2}((?:[a-z][a-z0-9-]*)|(?:--[a-z][a-z0-9-]*))\b(?:\s{2,}|$)") {
            $name = $Matches[1]
            if ($name -ne "help" -and -not $names.Contains($name)) {
                $names.Add($name)
            }
        }
    }

    return $names
}

function Get-NextHelpArguments {
    param(
        [string] $CliName,
        [string] $HelpText
    )

    $arguments = New-Object System.Collections.Generic.List[string]
    foreach ($line in ($HelpText -split "`r?`n")) {
        $trimmed = $line.Trim()
        if ($trimmed -match ("^Next:\s+" + [regex]::Escape($CliName) + "\s+(.+--help)\s*$")) {
            $args = $Matches[1].Trim()
            if (-not $arguments.Contains($args)) {
                $arguments.Add($args)
            }
        }
    }

    return $arguments
}

function Add-HelpQueueItem {
    param(
        [System.Collections.Generic.Queue[object]] $Queue,
        [System.Collections.Generic.HashSet[string]] $Seen,
        [string] $Arguments
    )

    $key = $Arguments.Trim()
    if ($key.Length -eq 0) {
        $key = "--help"
    }

    if ($Seen.Add($key)) {
        $Queue.Enqueue($key)
    }
}

function Build-CliSection {
    param(
        [string] $CliName,
        [string] $CommandPath
    )

    $section = New-Object System.Collections.Generic.List[string]
    $queue = [System.Collections.Generic.Queue[object]]::new()
    $queuedOrRendered = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)

    Add-HelpQueueItem -Queue $queue -Seen $queuedOrRendered -Arguments "--help"

    $section.Add("## $CliName")
    $section.Add("")

    while ($queue.Count -gt 0) {
        $arguments = [string] $queue.Dequeue()
        $help = Invoke-CliHelp -CommandPath $CommandPath -Arguments $arguments

        $section.Add("### ``$CliName $arguments``")
        $section.Add("")
        $section.Add('```text')
        if ($help.ExitCode -ne 0) {
            $section.Add("Exit code: $($help.ExitCode)")
        }

        if ([string]::IsNullOrWhiteSpace($help.Text)) {
            $section.Add("(no help output)")
        }
        else {
            foreach ($line in ($help.Text -split "`r?`n")) {
                $section.Add($line.TrimEnd())
            }
        }

        $section.Add('```')
        $section.Add("")

        foreach ($commandName in (Get-CommandNamesFromHelp -HelpText $help.Text)) {
            Add-HelpQueueItem -Queue $queue -Seen $queuedOrRendered -Arguments ($commandName + " --help")
        }

        foreach ($nextArgs in (Get-NextHelpArguments -CliName $CliName -HelpText $help.Text)) {
            Add-HelpQueueItem -Queue $queue -Seen $queuedOrRendered -Arguments $nextArgs
        }
    }

    return $section
}

$output = New-Object System.Collections.Generic.List[string]
$output.Add("# Commands")
$output.Add("")
$output.Add("This file is generated from each CLI's own help output.")
$output.Add("Update the CLI help text first, then regenerate with:")
$output.Add("")
$output.Add('```powershell')
$output.Add("powershell -ExecutionPolicy Bypass -File scripts\update-commands-md.ps1")
$output.Add('```')
$output.Add("")
$output.Add("The command sections are intentionally plain console help transcripts so agents and humans see the same interface.")
$output.Add("")

foreach ($cliName in $CliNames) {
    $commandPath = Resolve-CommandPath $cliName
    if ($null -eq $commandPath) {
        $output.Add("## $cliName")
        $output.Add("")
        $output.Add('```text')
        $output.Add("Command not found on PATH.")
        $output.Add('```')
        $output.Add("")
        continue
    }

    foreach ($line in Build-CliSection -CliName $cliName -CommandPath $commandPath) {
        $output.Add($line)
    }
}

while ($output.Count -gt 0 -and [string]::IsNullOrWhiteSpace($output[$output.Count - 1])) {
    $output.RemoveAt($output.Count - 1)
}

$resolvedOutputPath = Resolve-Path -Path "." | ForEach-Object { Join-Path $_ $OutputPath }
$outputDirectory = Split-Path -Parent $resolvedOutputPath
if (-not (Test-Path $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory | Out-Null
}

$text = ($output | ForEach-Object { $_.TrimEnd() }) -join "`r`n"
$text += "`r`n"
[System.IO.File]::WriteAllText($resolvedOutputPath, $text, [System.Text.UTF8Encoding]::new($false))
Write-Host "Wrote $OutputPath"
