[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ConnectionEnv,
    [string]$DataTypeWorkspace,
    [string]$OutputWorkspace,
    [string]$EvidenceDirectory = (Join-Path ([IO.Path]::GetTempPath()) ('MetaBi.TpcDs.Fixture\' + [Guid]::NewGuid().ToString('N'))),
    [switch]$ReplaceExisting
)

$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($DataTypeWorkspace)) { $DataTypeWorkspace = Join-Path $PSScriptRoot '..\..\MetaDataType\Workspace' }
if ([string]::IsNullOrWhiteSpace($OutputWorkspace)) { $OutputWorkspace = Join-Path $PSScriptRoot 'SchemaWS' }
$DataTypeWorkspace = [IO.Path]::GetFullPath($DataTypeWorkspace)
$OutputWorkspace = [IO.Path]::GetFullPath($OutputWorkspace)
$EvidenceDirectory = [IO.Path]::GetFullPath($EvidenceDirectory)
if ($OutputWorkspace -eq [IO.Path]::GetPathRoot($OutputWorkspace) -or
    $EvidenceDirectory.StartsWith($OutputWorkspace + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'The output must be a workspace directory, and evidence must be stored outside it.'
}
if ((Test-Path -LiteralPath $OutputWorkspace) -and -not $ReplaceExisting) {
    throw 'The output workspace exists. Use -ReplaceExisting to preserve it in the evidence directory and replace it.'
}
if ((Test-Path -LiteralPath $OutputWorkspace) -and -not (Test-Path -LiteralPath (Join-Path $OutputWorkspace 'workspace.meta'))) {
    throw 'The existing output directory is not a metadata workspace.'
}
if (Test-Path -LiteralPath $EvidenceDirectory) {
    throw 'Use a new evidence directory for each generation run.'
}
$connectionString = [Environment]::GetEnvironmentVariable($ConnectionEnv)
if ([string]::IsNullOrWhiteSpace($connectionString)) { throw "Connection environment variable '$ConnectionEnv' is empty." }
New-Item -ItemType Directory -Path $EvidenceDirectory | Out-Null

function Invoke-FixtureCli([string]$Step, [string]$Executable, [string[]]$Arguments) {
    & $Executable @Arguments *> (Join-Path $EvidenceDirectory ($Step + '.log'))
    if ($LASTEXITCODE -ne 0) { throw "Fixture step '$Step' failed. See $EvidenceDirectory\$Step.log." }
}

function Invoke-Sql([Data.SqlClient.SqlConnection]$Connection, [string]$Sql) {
    $command = $Connection.CreateCommand()
    try {
        $command.CommandText = $Sql
        $command.CommandTimeout = 120
        [void]$command.ExecuteNonQuery()
    } finally { $command.Dispose() }
}

$databaseName = 'MetaBiTpcDsFixture'
$builder = [Data.SqlClient.SqlConnectionStringBuilder]::new($connectionString)
$builder['Initial Catalog'] = 'master'
$master = [Data.SqlClient.SqlConnection]::new($builder.ConnectionString)
$connection = $null
$created = $false
$queryEnvironment = 'META_TPCDS_FIXTURE_GENERATION_SQL'
$previousEnvironment = [Environment]::GetEnvironmentVariable($queryEnvironment)
try {
    $master.Open()
    # CREATE fails if this named development database already exists. Only our own creation is cleaned up.
    Invoke-Sql $master "CREATE DATABASE [$databaseName];"
    $created = $true
    $builder['Initial Catalog'] = $databaseName
    [Environment]::SetEnvironmentVariable($queryEnvironment, $builder.ConnectionString)
    $connection = [Data.SqlClient.SqlConnection]::new($builder.ConnectionString)
    $connection.Open()
    Invoke-Sql $connection ([IO.File]::ReadAllText((Join-Path $PSScriptRoot 'SourceSchema.sql')))

    $manifest = Import-Csv (Join-Path $PSScriptRoot 'SourceViews.manifest.tsv') -Delimiter "`t"
    if ($manifest.Count -ne 99) { throw 'Expected 99 query entries.' }
    foreach ($entry in $manifest) {
        $queryPath = Join-Path $PSScriptRoot $entry.Path.Replace('\', [IO.Path]::DirectorySeparatorChar)
        try { Invoke-Sql $connection ([IO.File]::ReadAllText($queryPath)) }
        catch { throw "SQL Server rejected $($entry.Path): $($_.Exception.Message)" }
    }

    $command = $connection.CreateCommand()
    $command.CommandText = @'
SELECT s.name AS SchemaName, o.name AS ObjectName, c.name AS ColumnName,
       c.column_id AS Ordinal, t.name AS TypeName,
       CASE WHEN t.name IN ('nchar','nvarchar') AND c.max_length <> -1
            THEN c.max_length / 2 ELSE c.max_length END AS Length,
       c.precision AS Precision, c.scale AS Scale, c.is_nullable AS IsNullable,
       CONVERT(bit, CASE WHEN o.type = 'V' THEN 1 ELSE 0 END) AS IsView
FROM sys.objects o
JOIN sys.schemas s ON s.schema_id = o.schema_id
JOIN sys.columns c ON c.object_id = o.object_id
JOIN sys.types t ON t.user_type_id = c.user_type_id
WHERE o.type IN ('U','V') AND s.name IN ('dbo','tpcds')
ORDER BY s.name, o.name, c.column_id;
'@
    $native = [Data.DataTable]::new()
    $reader = $command.ExecuteReader()
    try { $native.Load($reader) } finally { $reader.Dispose(); $command.Dispose() }
    $native.Rows | Select-Object SchemaName,ObjectName,ColumnName,Ordinal,TypeName,Length,Precision,Scale,IsNullable,IsView |
        Export-Csv (Join-Path $EvidenceDirectory 'sql-server-columns.csv') -NoTypeInformation -Encoding utf8
    if (@($native.Rows | Where-Object IsView | Select-Object ObjectName -Unique).Count -ne 99) {
        throw 'SQL Server did not produce all 99 views.'
    }

    $raw = Join-Path $EvidenceDirectory 'ExtractedSchema'
    $stage = Join-Path $EvidenceDirectory 'ExpectedTableSchema'
    $catalog = Join-Path $EvidenceDirectory 'Catalog'
    Invoke-FixtureCli 'extract' 'meta-schema' @('extract','sqlserver','--connection-env',$queryEnvironment,
        '--data-type-workspace',$DataTypeWorkspace,'--system','TpcDsSnapshot','--all-schemas','--all-tables','--output-xml',$raw)
    Invoke-FixtureCli 'copy-schema' 'meta' @('create','--source-workspace',$raw,'--with-instances','--xml',$stage)
    Invoke-FixtureCli 'copy-catalog' 'meta' @('create','--source-workspace',$DataTypeWorkspace,'--with-instances','--xml',$catalog)

    [xml]$schemaXml = [IO.File]::ReadAllText((Join-Path $stage 'instances\Schema.xml'))
    [xml]$objectXml = [IO.File]::ReadAllText((Join-Path $stage 'instances\SchemaObject.xml'))
    [xml]$fieldXml = [IO.File]::ReadAllText((Join-Path $stage 'instances\Field.xml'))
    [xml]$viewXml = [IO.File]::ReadAllText((Join-Path $stage 'instances\View.xml'))
    [xml]$detailXml = [IO.File]::ReadAllText((Join-Path $stage 'instances\FieldDataTypeDetail.xml'))
    [xml]$typesXml = [IO.File]::ReadAllText((Join-Path $catalog 'instances\DataType.xml'))
    [xml]$systemsXml = [IO.File]::ReadAllText((Join-Path $catalog 'instances\DataTypeSystem.xml'))
    $sqlSystem = @($systemsXml.MetaDataType.DataTypeSystemList.DataTypeSystem | Where-Object Name -eq 'SqlServer')
    if ($sqlSystem.Count -ne 1) { throw 'The catalog must identify one SqlServer type system.' }
    $typeNames = @{}
    foreach ($type in $typesXml.MetaDataType.DataTypeList.DataType) {
        if ($type.DataTypeSystemId -eq $sqlSystem[0].Id) { $typeNames[[string]$type.Id] = [string]$type.Name }
    }
    $schemaNames = @{}
    foreach ($schema in $schemaXml.MetaSchema.SchemaList.Schema) { $schemaNames[[string]$schema.Id] = [string]$schema.Name }
    $objects = @{}
    foreach ($object in $objectXml.MetaSchema.SchemaObjectList.SchemaObject) { $objects[[string]$object.Id] = $object }
    $facets = @{}
    foreach ($detail in $detailXml.MetaSchema.FieldDataTypeDetailList.FieldDataTypeDetail) {
        $facets[[string]$detail.FieldId + '|' + [string]$detail.Name] = [string]$detail.Value
    }
    $fields = @{}
    $fieldsById = @{}
    foreach ($field in $fieldXml.MetaSchema.FieldList.Field) {
        $object = $objects[[string]$field.SchemaObjectId]
        $key = $schemaNames[[string]$object.SchemaId] + '|' + [string]$object.Name + '|' + [string]$field.Name
        $fields[$key] = $field
        $fieldsById[[string]$field.Id] = $field
    }
    if ($fields.Count -ne $native.Rows.Count) { throw 'Extracted column count differs from SQL Server.' }
    $nativeByKey = @{}
    $nullableUpdates = foreach ($column in $native.Rows) {
        $key = $column.SchemaName + '|' + $column.ObjectName + '|' + $column.ColumnName
        $nativeByKey[$key] = $column
        $field = $fields[$key]
        if ($null -eq $field -or $typeNames[[string]$field.MetaDataTypeId] -ne $column.TypeName -or [int]$field.Ordinal -ne $column.Ordinal) {
            throw "Extracted identity, type or ordinal differs from SQL Server for $key."
        }
        if ($column.TypeName -in @('char','varchar','nchar','nvarchar','binary','varbinary') -and
            $facets[[string]$field.Id + '|Length'] -ne [string]$column.Length) { throw "Length mismatch for $key." }
        if ($column.TypeName -in @('decimal','numeric') -and
            ($facets[[string]$field.Id + '|Precision'] -ne [string]$column.Precision -or
             $facets[[string]$field.Id + '|Scale'] -ne [string]$column.Scale)) { throw "Decimal facet mismatch for $key." }
        if ($column.IsView) {
            # Extraction leaves view nullability unspecified. The expected table contract uses SQL Server's result metadata.
            [pscustomobject]@{Id=[string]$field.Id; IsNullable=([string]$column.IsNullable).ToLowerInvariant()}
        } elseif ([string]$field.IsNullable -ne ([string]$column.IsNullable).ToLowerInvariant()) {
            throw "Source nullability mismatch for $key."
        }
    }

    $views = @($viewXml.MetaSchema.ViewList.View)
    if ($views.Count -ne 99) { throw 'Expected 99 extracted View instances.' }
    $tableRows = foreach ($view in $views) {
        Invoke-FixtureCli ('remove-view-' + $objects[[string]$view.SchemaObjectId].Name) 'meta' @('delete','View',[string]$view.Id,'--workspace',$stage)
        [pscustomobject]@{Id=[string]$view.Id; SchemaObjectId=[string]$view.SchemaObjectId}
    }
    $tableFile = Join-Path $EvidenceDirectory 'table-specializations.csv'
    $tableRows | Export-Csv $tableFile -NoTypeInformation -Encoding utf8
    Invoke-FixtureCli 'add-table-specializations' 'meta' @('bulk-insert','Table','--file',$tableFile,'--from','csv','--workspace',$stage)
    $nullableFile = Join-Path $EvidenceDirectory 'output-nullability.csv'
    $nullableUpdates | Export-Csv $nullableFile -NoTypeInformation -Encoding utf8
    Invoke-FixtureCli 'set-output-nullability' 'meta' @('import','csv',$nullableFile,'--entity','Field','--workspace',$stage)

    if ([IO.File]::ReadAllText((Join-Path $raw 'instances\FieldDataTypeDetail.xml')) -cne
        [IO.File]::ReadAllText((Join-Path $stage 'instances\FieldDataTypeDetail.xml'))) { throw 'The table adaptation changed type facets.' }
    [xml]$finalFields = [IO.File]::ReadAllText((Join-Path $stage 'instances\Field.xml'))
    foreach ($field in $finalFields.MetaSchema.FieldList.Field) {
        $before = $fieldsById[[string]$field.Id]
        if ($field.MetaDataTypeId -ne $before.MetaDataTypeId -or $field.SchemaObjectId -ne $before.SchemaObjectId -or
            $field.Name -ne $before.Name -or $field.Ordinal -ne $before.Ordinal) { throw 'The table adaptation changed a column contract.' }
        $object = $objects[[string]$field.SchemaObjectId]
        $key = $schemaNames[[string]$object.SchemaId] + '|' + [string]$object.Name + '|' + [string]$field.Name
        $column = $nativeByKey[$key]
        if ([string]$field.IsNullable -ne ([string]$column.IsNullable).ToLowerInvariant()) { throw "Final nullability mismatch for $key." }
    }
    [xml]$finalTables = [IO.File]::ReadAllText((Join-Path $stage 'instances\Table.xml'))
    if (@($finalTables.MetaSchema.TableList.Table).Count -ne 123) { throw 'Expected 24 source and 99 output table contracts.' }
    $viewPath = Join-Path $stage 'instances\View.xml'
    if (Test-Path -LiteralPath $viewPath) {
        [xml]$finalViews = [IO.File]::ReadAllText($viewPath)
        if (@($finalViews.MetaSchema.ViewList.View | Where-Object { $_ }).Count -ne 0) { throw 'View instances remain after adaptation.' }
    }

    # Publish complete verified artifacts. Keep the previous workspace recoverable.
    $previous = Join-Path $EvidenceDirectory 'PreviousSchema'
    if ($stage -ne (Join-Path $EvidenceDirectory 'ExpectedTableSchema') -or $OutputWorkspace -eq $EvidenceDirectory -or
        $OutputWorkspace.StartsWith($stage + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Unsafe workspace publication paths.'
    }
    if (Test-Path -LiteralPath $OutputWorkspace) { Move-Item -LiteralPath $OutputWorkspace -Destination $previous }
    try { Move-Item -LiteralPath $stage -Destination $OutputWorkspace }
    catch {
        if (Test-Path -LiteralPath $previous) { Move-Item -LiteralPath $previous -Destination $OutputWorkspace }
        throw
    }
    Write-Output "Generated $OutputWorkspace from 24 SQL Server source tables and 99 compiled views."
    Write-Output "Verified $($native.Rows.Count) columns against SQL Server. Evidence: $EvidenceDirectory"
} finally {
    if ($connection) { $connection.Dispose() }
    try {
        if ($created) { Invoke-Sql $master "ALTER DATABASE [$databaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$databaseName];" }
    } finally {
        $master.Dispose()
        [Environment]::SetEnvironmentVariable($queryEnvironment, $previousEnvironment)
    }
}
