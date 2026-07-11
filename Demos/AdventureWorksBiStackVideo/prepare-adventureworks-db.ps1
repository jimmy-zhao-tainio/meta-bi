$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

trap {
    Write-Host $_.Exception.Message
    exit 1
}

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$server = if ($env:AW_SQL_SERVER) { $env:AW_SQL_SERVER } else { 'localhost' }
$database = if ($env:AW_SOURCE_DATABASE) { $env:AW_SOURCE_DATABASE } else { 'AdventureWorks2022' }
$runRoot = if ($env:AW_RUN_ROOT) { $env:AW_RUN_ROOT } else { 'Runs' }
$backupUrl = if ($env:AW_BACKUP_URL) { $env:AW_BACKUP_URL } else { 'https://github.com/Microsoft/sql-server-samples/releases/download/adventureworks/AdventureWorks2022.bak' }
$replaceDatabase = $env:AW_RESTORE_REPLACE -eq '1'

if (-not [System.IO.Path]::IsPathRooted($runRoot)) {
    $runRoot = Join-Path $scriptRoot $runRoot
}

$downloadDir = Join-Path $runRoot '_downloads'
$backupFileName = Split-Path -Leaf ([Uri]$backupUrl).AbsolutePath
if ([string]::IsNullOrWhiteSpace($backupFileName)) {
    $backupFileName = 'AdventureWorks2022.bak'
}

$localBackupPath = Join-Path $downloadDir $backupFileName

function Invoke-SqlLines {
    param(
        [Parameter(Mandatory = $true)]
        [string] $Query
    )

    $output = & sqlcmd -S $server -E -b -W -h -1 -s '|' -Q $Query 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw ($output -join [Environment]::NewLine)
    }

    return @($output | Where-Object { $_ -and $_.Trim().Length -gt 0 })
}

function Invoke-SqlScript {
    param(
        [Parameter(Mandatory = $true)]
        [string] $Script
    )

    $path = [System.IO.Path]::GetTempFileName()
    try {
        Set-Content -LiteralPath $path -Value $Script -Encoding UTF8
        & sqlcmd -S $server -E -b -i $path
        if ($LASTEXITCODE -ne 0) {
            throw "sqlcmd failed with exit code $LASTEXITCODE."
        }
    }
    finally {
        Remove-Item -LiteralPath $path -ErrorAction SilentlyContinue
    }
}

function SqlString {
    param([string] $Value)
    return $Value.Replace("'", "''")
}

function SqlIdentifier {
    param([string] $Value)
    return '[' + $Value.Replace(']', ']]') + ']'
}

function Grant-SqlServiceRead {
    param([string] $Path)

    if ($server -notmatch '^(localhost|\.|\(local\))($|\\)') {
        return
    }

    $serviceName = 'MSSQLSERVER'
    if ($server -match '\\([^\\]+)$') {
        $serviceName = 'MSSQL$' + $Matches[1]
    }

    try {
        $service = Get-CimInstance Win32_Service -Filter "Name='$serviceName'" -ErrorAction Stop
        if (-not $service -or [string]::IsNullOrWhiteSpace($service.StartName)) {
            return
        }

        $account = $service.StartName
        if ($account -eq 'LocalSystem' -or $account -eq 'NT AUTHORITY\SYSTEM') {
            return
        }

        Write-Host "Granting SQL Server service read access to backup cache..."
        $grant = "${account}:(OI)(CI)RX"
        & icacls $Path /grant $grant | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Could not update ACL with icacls; restore will still be attempted."
        }
    }
    catch {
        Write-Host "Could not determine SQL Server service account; restore will still be attempted."
    }
}

function FirstValue {
    param([string] $Query)
    $lines = @(Invoke-SqlLines -Query $Query)
    if ($lines.Count -eq 0) {
        return ''
    }

    return $lines[0].Trim()
}

function Invoke-SourceValidation {
    if ([string]::IsNullOrWhiteSpace($env:AW_SOURCE_SQL)) {
        $env:AW_SOURCE_SQL = "Server=$server;Database=$database;Trusted_Connection=True;TrustServerCertificate=True;"
    }

    Push-Location (Join-Path $scriptRoot 'AdventureWorksBiStackVideo.MetaMesh')
    try {
        & meta-mesh run --operation validate-source
        if ($LASTEXITCODE -ne 0) {
            throw "AdventureWorks source validation failed with exit code $LASTEXITCODE."
        }
    }
    finally {
        Pop-Location
    }
}

Write-Host "AdventureWorks OLTP database preparation"
Write-Host "  Server:   $server"
Write-Host "  Database: $database"
Write-Host "  Source:   $backupUrl"
Write-Host "  Cache:    $localBackupPath"

$dbExists = FirstValue -Query "SET NOCOUNT ON; SELECT CASE WHEN DB_ID(N'$(SqlString $database)') IS NULL THEN 0 ELSE 1 END;"
if ($dbExists -eq '1' -and -not $replaceDatabase) {
    Write-Host "Database already exists. Verifying data instead of restoring. Set AW_RESTORE_REPLACE=1 to replace it."
    Invoke-SourceValidation

    exit 0
}

New-Item -ItemType Directory -Force -Path $downloadDir | Out-Null

$expectedLength = 0L
try {
    $head = Invoke-WebRequest -Uri $backupUrl -Method Head -MaximumRedirection 5 -UseBasicParsing
    $lengthHeader = $head.Headers['Content-Length']
    if ($lengthHeader) {
        [void][long]::TryParse($lengthHeader, [ref]$expectedLength)
    }
}
catch {
    Write-Host "Could not read remote length; download will proceed if the local file is missing."
}

$downloadNeeded = -not (Test-Path -LiteralPath $localBackupPath)
if (-not $downloadNeeded -and $expectedLength -gt 0) {
    $existingLength = (Get-Item -LiteralPath $localBackupPath).Length
    $downloadNeeded = $existingLength -ne $expectedLength
}

if ($downloadNeeded) {
    Write-Host "Downloading AdventureWorks backup..."
    Invoke-WebRequest -Uri $backupUrl -OutFile $localBackupPath -MaximumRedirection 5 -UseBasicParsing
}
else {
    Write-Host "Backup already downloaded."
}

Grant-SqlServiceRead -Path $downloadDir

$paths = FirstValue -Query "SET NOCOUNT ON; SELECT COALESCE(CONVERT(nvarchar(4000), SERVERPROPERTY('InstanceDefaultBackupPath')), N'') + N'|' + COALESCE(CONVERT(nvarchar(4000), SERVERPROPERTY('InstanceDefaultDataPath')), N'') + N'|' + COALESCE(CONVERT(nvarchar(4000), SERVERPROPERTY('InstanceDefaultLogPath')), N'');"
$parts = $paths.Split('|')
$serverBackupDir = if ($parts.Count -gt 0) { $parts[0].Trim() } else { '' }
$serverDataDir = if ($parts.Count -gt 1) { $parts[1].Trim() } else { '' }
$serverLogDir = if ($parts.Count -gt 2) { $parts[2].Trim() } else { '' }

if ([string]::IsNullOrWhiteSpace($serverDataDir)) {
    throw "SQL Server did not report an InstanceDefaultDataPath."
}

if ([string]::IsNullOrWhiteSpace($serverLogDir)) {
    $serverLogDir = $serverDataDir
}

$restoreBackupPath = $localBackupPath
if (-not [string]::IsNullOrWhiteSpace($serverBackupDir)) {
    $serverBackupPath = Join-Path $serverBackupDir $backupFileName
    try {
        if (-not (Test-Path -LiteralPath $serverBackupPath) -or ((Get-Item -LiteralPath $serverBackupPath).Length -ne (Get-Item -LiteralPath $localBackupPath).Length)) {
            Write-Host "Copying backup to SQL Server backup folder..."
            Copy-Item -LiteralPath $localBackupPath -Destination $serverBackupPath -Force
        }

        $restoreBackupPath = $serverBackupPath
    }
    catch {
        Write-Host "Could not copy to SQL Server backup folder; trying local cache path."
        Write-Host "  $($_.Exception.Message)"
    }
}

Write-Host "Reading backup file list..."
$fileListLines = Invoke-SqlLines -Query "RESTORE FILELISTONLY FROM DISK = N'$(SqlString $restoreBackupPath)';"
$dataLogicalName = $null
$logLogicalName = $null

foreach ($line in $fileListLines) {
    $columns = $line -split '\|'
    if ($columns.Count -lt 3) {
        continue
    }

    $logicalName = $columns[0].Trim()
    $fileType = $columns[2].Trim()

    if ($fileType -eq 'D' -and -not $dataLogicalName) {
        $dataLogicalName = $logicalName
    }
    elseif ($fileType -eq 'L' -and -not $logLogicalName) {
        $logLogicalName = $logicalName
    }
}

if (-not $dataLogicalName -or -not $logLogicalName) {
    throw "Could not determine logical data/log file names from the backup."
}

$dataFilePath = Join-Path $serverDataDir "$database.mdf"
$logFilePath = Join-Path $serverLogDir "$($database)_log.ldf"

Write-Host "Restoring database..."
$restoreScript = @"
USE [master];
IF DB_ID(N'$(SqlString $database)') IS NOT NULL
BEGIN
    ALTER DATABASE $(SqlIdentifier $database) SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
END;
RESTORE DATABASE $(SqlIdentifier $database)
FROM DISK = N'$(SqlString $restoreBackupPath)'
WITH
    FILE = 1,
    MOVE N'$(SqlString $dataLogicalName)' TO N'$(SqlString $dataFilePath)',
    MOVE N'$(SqlString $logLogicalName)' TO N'$(SqlString $logFilePath)',
    REPLACE,
    RECOVERY,
    STATS = 10;
ALTER DATABASE $(SqlIdentifier $database) SET MULTI_USER;
"@

Invoke-SqlScript -Script $restoreScript

Write-Host "Verifying restored data..."
Invoke-SourceValidation

Write-Host "AdventureWorks is ready."
