# meta-bi

`meta-bi` is the BI stack that sits on top of the generic `meta` foundation.

## CLI Guide

`meta-bi` ships these operator-facing CLIs:

- `meta-schema`
- `meta-data-type`
- `meta-data-type-conversion`
- `meta-convert`
- `meta-datavault-raw`
- `meta-datavault-business`
- `meta-data-warehouse`
- `meta-analytics`
- `meta-tabular`
- `meta-multi-dimensional`
- `meta-pipeline`
- `meta-orchestration`
- `meta-sql`
- `meta-transform-script`
- `meta-transform-binding`
- `meta-data-quality`

For exhaustive current command and switch syntax, use [`docs/commands.md`](docs/commands.md). That file is generated from each CLI's own help output by `scripts\update-commands-md.ps1`; this README keeps the higher-level workflows and representative command surfaces.

### meta-schema

Purpose:
- extract a sanctioned `MetaSchema` workspace from a live SQL Server schema

Command surface:
- `meta-schema help`
- `meta-schema extract sqlserver --new-workspace <path> --connection-env <name> --system <name> (--schema <name> | --all-schemas) (--table <name> | --all-tables)`

Example:

`--connection-env` names the shell-visible environment variable that contains the SQL Server connection string.

```cmd
meta-schema extract sqlserver --new-workspace .\MetaSchema.Workspace --connection-env META_SQL_DEV --system MySystem --schema dbo --all-tables
```

### meta-data-type

Purpose:
- bootstrap sanctioned `MetaDataType` workspaces

Command surface:
- `meta-data-type help`
- `meta-data-type --new-workspace <path>`

Example:

```cmd
meta-data-type --new-workspace .\MetaDataType.Workspace
```

### meta-data-type-conversion

Purpose:
- author and validate sanctioned type-conversion workspaces
- resolve one source data type through the sanctioned conversion graph

Command surface:
- `meta-data-type-conversion help`
- `meta-data-type-conversion --new-workspace <path>`
- `meta-data-type-conversion check --workspace <path>`
- `meta-data-type-conversion resolve --workspace <path> --source-data-type <id> [--target-data-type-system <name>]`

Behavior summary:
- `check` requires each source data type to map deterministically per target data type system
- `resolve` without `--target-data-type-system` requires the source type to have one unambiguous mapping
- use `--target-data-type-system` when a source type has mappings for multiple runtime targets, such as `SqlServer` and `Meta`

Examples:

```cmd
meta-data-type-conversion --new-workspace .\MetaDataTypeConversion.Workspace
meta-data-type-conversion check --workspace .\MetaDataTypeConversion.Workspace
meta-data-type-conversion resolve --workspace .\MetaDataTypeConversion.Workspace --source-data-type meta:type:String
meta-data-type-conversion resolve --workspace .\MetaDataTypeConversion.Workspace --source-data-type meta:type:String --target-data-type-system SqlServer
```

### meta-convert

Purpose:
- perform cross-model conversions handled by conversion glue code

Command surface:
- `meta-convert help`
- `meta-convert schema-to-raw-datavault --source-workspace <path> --new-workspace <path> [--ignore-field-name <name>]... [--ignore-field-suffix <suffix>]... [--include-views] [--verbose]`
- `meta-convert raw-datavault-to-sql [--workspace <path>] --implementation-workspace <path> --database-name <name> --out <path>`
- `meta-convert business-datavault-to-sql [--workspace <path>] --implementation-workspace <path> --database-name <name> --out <path>`
- `meta-convert data-warehouse-to-sql [--workspace <path>] --implementation-workspace <path> --database-name <name> --out <path>`
- `meta-convert data-quality-to-sql [--workspace <path>] --out <path>`

Projection note:
- `raw-datavault-to-sql` and `business-datavault-to-sql` take physical schema ownership from the sanctioned `MetaDataVaultImplementation` workspace and do not accept a schema override.
- `data-warehouse-to-sql` takes physical table/column/key policy from the sanctioned `MetaDataWarehouseImplementation` workspace and does not query a live database.

Example:

```cmd
meta-convert schema-to-raw-datavault --source-workspace .\MetaSchema.Workspace --new-workspace .\MetaRawDataVault.Workspace
meta-convert raw-datavault-to-sql --workspace .\MetaRawDataVault.Workspace --implementation-workspace .\MetaDataVault\Workspaces\MetaDataVaultImplementation --database-name MyVault --out .\out\CurrentMetaSql.Workspace
meta-convert business-datavault-to-sql --workspace .\MetaBusinessDataVault.Workspace --implementation-workspace .\MetaDataVault\Workspaces\MetaDataVaultImplementation --database-name MyBusinessVault --out .\out\CurrentMetaSql.Workspace
meta-convert data-warehouse-to-sql --workspace .\MetaDataWarehouse.Workspace --implementation-workspace .\MetaDataWarehouse\Workspaces\MetaDataWarehouseImplementation --database-name MyWarehouse --out .\out\CurrentMetaSql.Workspace
meta-convert data-quality-to-sql --workspace .\MetaDataQuality.Workspace --out .\out\DataQualityViews.sql
```

### meta-datavault-raw

Purpose:
- author sanctioned raw Data Vault workspaces

Command surface:
- `meta-datavault-raw --new-workspace <path>`
- `meta-datavault-raw add-*`

`add-*` commands:
- `add-source-system`
- `add-source-schema`
- `add-source-table`
- `add-source-field`
- `add-source-field-data-type-detail`
- `add-source-table-relationship`
- `add-source-table-relationship-field`
- `add-hub`
- `add-hub-key-part`
- `add-hub-satellite`
- `add-hub-satellite-attribute`
- `add-link`
- `add-link-hub`
- `add-link-satellite`
- `add-link-satellite-attribute`

Examples:

```cmd
meta-datavault-raw --new-workspace .\MetaRawDataVault.Workspace
```

### meta-datavault-business

Purpose:
- author sanctioned business Data Vault workspaces

Command surface:
- `meta-datavault-business --new-workspace <path>`
- `meta-datavault-business add-*`
- typed business authoring commands take optional datatype facets inline via `--length`, `--precision`, and `--scale`; the CLI persists the underlying detail rows for you

Representative `add-*` families:
- `add-bridge*`
- `add-hub*`
- `add-link*`
- `add-hierarchical-link*`
- `add-reference*`
- `add-same-as-link*`
- `add-point-in-time*`

Example:

```cmd
meta-datavault-business --new-workspace .\MetaBusinessDataVault.Workspace
```

### meta-data-warehouse

Purpose:
- author sanctioned dimensional warehouse workspaces through logical concept commands

Command surface:
- `meta-data-warehouse help`
- `meta-data-warehouse --new-workspace <path>`
- `meta-data-warehouse add-warehouse --id <id> --name <name>`
- `meta-data-warehouse add-dimension --id <id> --warehouse <id> --name <name>`
- `meta-data-warehouse add-dimension-attribute --id <id> --dimension <id> --name <name> --data-type-id meta:type:<type>`
- `meta-data-warehouse add-dimension-business-key --id <id> --dimension <id> --name <name>`
- `meta-data-warehouse add-dimension-business-key-part --id <id> --business-key <id> --attribute <id>`
- `meta-data-warehouse add-slowly-changing-dimension --id <id> --dimension <id> [--name <name>]`
- `meta-data-warehouse add-fact --id <id> --warehouse <id> --name <name>`
- `meta-data-warehouse add-fact-dimension --id <id> --fact <id> --dimension <id> --role-name <name>`
- `meta-data-warehouse add-fact-measure --id <id> --fact <id> --name <name> --data-type-id meta:type:<type>`

Behavior summary:
- logical `MetaDataWarehouse` owns Kimball-style dimensional intent: dimensions, facts, grain, roles, typed measures, snapshots, bridges, and SCD declarations
- measure additivity and aggregation functions belong to the analytics/semantic layer, not the logical warehouse table model
- `MetaDataWarehouseImplementation` owns SQL realization policy such as table naming, key naming, platform columns, defaults, and indexes
- logical attribute and measure `DataTypeId` values are Meta-system semantic types; SQL Server types come from `MetaDataTypeConversion` and implementation policy during conversion
- surrogate keys, SCD effective/current/hash columns, platform columns, and SQL type facets are not authored as logical DW rows
- fact-to-dimension roles are modeled on fact participation, not as separate role-playing dimension tables

Examples:

```cmd
meta-data-warehouse --new-workspace .\MetaDataWarehouse.Workspace
meta-data-warehouse add-warehouse --workspace .\MetaDataWarehouse.Workspace --id Commerce --name Commerce
meta-data-warehouse add-dimension --workspace .\MetaDataWarehouse.Workspace --id Customer --warehouse Commerce --name Customer
meta-data-warehouse add-dimension-attribute --workspace .\MetaDataWarehouse.Workspace --id CustomerNumber --dimension Customer --name CustomerNumber --data-type-id meta:type:String
meta-data-warehouse add-dimension-business-key --workspace .\MetaDataWarehouse.Workspace --id CustomerBusinessKey --dimension Customer --name CustomerBusinessKey
meta-data-warehouse add-dimension-business-key-part --workspace .\MetaDataWarehouse.Workspace --id CustomerBusinessKeyPart --business-key CustomerBusinessKey --attribute CustomerNumber
meta-data-warehouse add-slowly-changing-dimension --workspace .\MetaDataWarehouse.Workspace --id CustomerHistory --dimension Customer --name CustomerHistory
meta-convert data-warehouse-to-sql --workspace .\MetaDataWarehouse.Workspace --implementation-workspace .\MetaDataWarehouse\Workspaces\MetaDataWarehouseImplementation --database-name MyWarehouse --out .\CurrentMetaSql.Workspace
```

### meta-analytics

Purpose:
- author sanctioned conceptual analytics workspaces as portable analytical intent
- describe the shared analytical model before choosing a tabular or multidimensional implementation target

Command surface:
- `meta-analytics help`
- `meta-analytics --new-workspace <path>`
- `meta-analytics add-model --id <id> --name <name>`
- `meta-analytics add-data-source --id <id> --model <id> --name <name>`
- `meta-analytics add-table --id <id> --model <id> --name <name> --kind <kind>`
- `meta-analytics add-attribute --id <id> --table <id> --name <name> --data-type-id meta:type:<type>`
- `meta-analytics add-measure --id <id> --table <id> --source-attribute <id> --name <name>`
- `meta-analytics add-aggregation-behavior --id <id> --measure <id> --function <aggregate>`
- `meta-analytics add-role-filter --id <id> --role <id> --table <id> --expression-language <language> --expression <expr>`
- `meta-analytics add-perspective --id <id> --model <id> --name <name>`
- `meta-analytics add-culture --id <id> --model <id> --name <culture>`
- `meta-analytics add-*-translation ...`

Behavior summary:
- `MetaAnalytics` models analytical product truth, not `.smproj`, `.dwproj`, XMLA, or TMSL payload syntax
- common analytical concepts are represented with clean names: tables, attributes, hierarchies, relationships, source-backed base measures, aggregation behavior, perspectives, roles, row/object security intent, cultures, and translations
- conceptual measures are base measurements over a source attribute plus an aggregate function; calculated measures, KPIs, and target-language patterns belong in `MetaTabular` or `MetaMultiDimensional`
- engine-specific implementation concepts such as tabular calculation groups/partitions and multidimensional cubes/measure groups/named sets/actions/cell security live in `MetaTabular` or `MetaMultiDimensional`
- perspective membership and translation targets are typed rows, not `ObjectKind`/`ObjectId` blobs
- target converters project base measures deterministically; target-specific calculated measures, KPIs, and DAX/MDX script surfaces are patched in the target model after conversion

Example:

```cmd
meta-analytics --new-workspace .\MetaAnalytics.Workspace
meta-analytics add-model --workspace .\MetaAnalytics.Workspace --id Commerce --name Commerce --default-culture en-US
meta-analytics add-data-source --workspace .\MetaAnalytics.Workspace --id Warehouse --model Commerce --name Warehouse --provider SqlServer --connection-reference COMMERCE_DW
meta-analytics add-table --workspace .\MetaAnalytics.Workspace --id Sales --model Commerce --name Sales --kind Fact
meta-analytics add-attribute --workspace .\MetaAnalytics.Workspace --id SalesAmountColumn --table Sales --name SalesAmount --data-type-id meta:type:Decimal
meta-analytics add-measure --workspace .\MetaAnalytics.Workspace --id SalesAmount --table Sales --source-attribute SalesAmountColumn --name "Sales Amount" --data-type-id meta:type:Decimal
meta-analytics add-aggregation-behavior --workspace .\MetaAnalytics.Workspace --id SalesAmountAggregation --measure SalesAmount --function Sum
```

Convert to a target implementation workspace:

```cmd
meta-convert analytics-to-tabular --workspace .\MetaAnalytics.Workspace --out .\MetaTabular.Workspace
meta-convert analytics-to-multi-dimensional --workspace .\MetaAnalytics.Workspace --out .\MetaMultiDimensional.Workspace
```

### meta-tabular

Purpose:
- author target-specific tabular implementation workspaces after `MetaAnalytics` conversion
- patch tabular-only implementation details such as DAX calculated measures, calculation groups, partitions, tabular KPIs, perspectives, and tabular security

Command surface:
- `meta-tabular help`
- `meta-tabular --new-workspace <path>`
- `meta-tabular add-tabular-model --id <id> --name <name>`
- `meta-tabular add-tabular-table --id <id> --tabular-model <id> --name <name>`
- `meta-tabular add-tabular-column --id <id> --tabular-table <id> --name <name> --data-type-id meta:type:<type>`
- `meta-tabular add-tabular-measure --id <id> --tabular-table <id> --name <name> [--expression <dax>]`
- `meta-tabular add-tabular-calculation-group --id <id> --tabular-model <id> --name <name> --precedence <number>`
- `meta-tabular add-tabular-calculation-item --id <id> --tabular-calculation-group <id> --name <name> --expression <dax>`
- `meta-tabular add-tabular-partition --id <id> --tabular-table <id> --name <name>`
- `meta-tabular add-tabular-kpi --id <id> --base-measure <id> --name <name>`
- `meta-tabular add-tabular-role-filter --id <id> --tabular-security-role <id> --tabular-table <id> --expression <dax>`
- `meta-tabular deploy [--workspace <path>] --server <server> [--database-name <name>] [--drop-existing] [--no-process]`
- `meta-tabular restore --source-server <server> --source-database-name <name> --target-server <server> --target-database-name <name> --backup-file <path> [--drop-existing] [--overwrite-backup-file]`
- `meta-tabular drop --server <server> --database-name <name>`

Behavior summary:
- `MetaTabular` is a target implementation model, not the portable conceptual model
- base measures converted from `MetaAnalytics` are emitted as tabular measures from source attribute plus aggregate function
- DAX calculated measures, calculation groups, calculation items, KPIs, partitions, row filters, object-level security, and tabular-only perspectives belong here
- `deploy` creates the Analysis Services tabular database and modeled objects from the target workspace root model; `--drop-existing` explicitly drops an existing database first
- `deploy` processes by default and fails if processing fails; for existing databases, use `--drop-existing` so the safe sequence is drop, create, full process
- use `--no-process` for metadata-only deployment
- `restore` promotes a processed pre-prod tabular database through SSAS backup/restore; if the target exists, `--drop-existing` is required before restore
- `restore` does not process and does not do partial/object-level processing; that belongs in a separate target-owned command
- the restore backup file path must be accessible to the Analysis Services service accounts on both source and target servers
- `drop` directly deletes the named tabular database and has no confirmation prompt
- current deploy realization covers modeled data sources, tables, columns, partitions, measures, relationships, calculation groups/items, and role filters
- processing, credential policy, and production scale knobs remain target-specific follow-up work

Example:

```cmd
meta-convert analytics-to-tabular --workspace .\MetaAnalytics.Workspace --out .\MetaTabular.Workspace
meta-tabular add-tabular-calculation-group --workspace .\MetaTabular.Workspace --id TimeIntelligence --tabular-model Commerce --name "Time Intelligence" --precedence 10
meta-tabular add-tabular-calculation-item --workspace .\MetaTabular.Workspace --id TimeYtd --tabular-calculation-group TimeIntelligence --name YTD --expression "CALCULATE(SELECTEDMEASURE(), DATESYTD('Date'[Date]))"
meta-tabular add-tabular-partition --workspace .\MetaTabular.Workspace --id SalesCurrent --tabular-table Sales --tabular-data-source Warehouse --name "Sales Current" --mode Import --expression "SELECT * FROM mart.FactSales"
meta-tabular deploy --workspace .\MetaTabular.Workspace --server localhost\TABULAR --database-name Commerce --drop-existing
meta-tabular restore --source-server preprod-tabular --source-database-name Commerce --target-server prod-tabular --target-database-name Commerce --backup-file \\fileserver\ssas-backups\Commerce.abf --drop-existing --overwrite-backup-file
meta-tabular drop --server localhost\TABULAR --database-name Commerce
```

Runnable deploy demo:

```cmd
Samples\Demos\MetaTabularDeployCliIntegration\run.cmd
```

### meta-multi-dimensional

Purpose:
- author target-specific multidimensional implementation workspaces after `MetaAnalytics` conversion
- patch multidimensional-only implementation details such as cubes, measure groups, dimension usage, MDX calculations, named sets, actions, KPIs, partitions, and cell/dimension security

Command surface:
- `meta-multi-dimensional help`
- `meta-multi-dimensional --new-workspace <path>`
- `meta-multi-dimensional add-multi-dimensional-database --id <id> --name <name>`
- `meta-multi-dimensional add-cube --id <id> --multi-dimensional-database <id> --name <name>`
- `meta-multi-dimensional add-dimension --id <id> --multi-dimensional-database <id> --name <name>`
- `meta-multi-dimensional add-measure-group --id <id> --cube <id> --name <name>`
- `meta-multi-dimensional add-measure --id <id> --measure-group <id> --name <name> --aggregate-function <function>`
- `meta-multi-dimensional add-dimension-usage --id <id> --cube-dimension <id> --measure-group <id> --usage-kind <kind>`
- `meta-multi-dimensional add-mdx-calculation --id <id> --cube <id> --name <name> --calculation-kind <kind> --expression <mdx>`
- `meta-multi-dimensional add-named-set --id <id> --cube <id> --name <name> --expression <mdx>`
- `meta-multi-dimensional add-kpi --id <id> --cube <id> --name <name>`
- `meta-multi-dimensional add-cell-permission --id <id> --security-role <id> --cube <id> --expression <mdx>`
- `meta-multi-dimensional deploy [--workspace <path>] --server <server> [--database-name <name>] [--drop-existing] [--no-process]`
- `meta-multi-dimensional restore --source-server <server> --source-database-name <name> --target-server <server> --target-database-name <name> --backup-file <path> [--drop-existing] [--overwrite-backup-file]`
- `meta-multi-dimensional drop --server <server> --database-name <name>`

Behavior summary:
- `MetaMultiDimensional` is a target implementation model for cube-shaped Analysis Services metadata
- converted base measures become measure group measures using the source attribute and aggregate function from `MetaAnalytics`
- MDX calculations, named sets, actions, KPIs, cell security, dimension security, partitions, and detailed dimension usage belong here
- `deploy` creates the Analysis Services multidimensional database and modeled objects from the target workspace root database; `--drop-existing` explicitly drops an existing database first
- `deploy` processes by default and fails if processing fails; for existing databases, use `--drop-existing` so the safe sequence is drop, create, full process
- use `--no-process` for metadata-only deployment
- `restore` promotes a processed pre-prod multidimensional database through SSAS backup/restore; if the target exists, `--drop-existing` is required before restore
- `restore` does not process and does not do partial/object-level processing; that belongs in a separate target-owned command
- the restore backup file path must be accessible to the Analysis Services service accounts on both source and target servers
- `drop` directly deletes the named multidimensional database and has no confirmation prompt
- current deploy realization covers modeled data sources, data source views, dimensions/attributes, cubes, measure groups, measures, partitions, MDX scripts/named sets, actions, roles, and cell permissions
- processing, full source-column binding policy, aggregation design, and production scale knobs remain target-specific follow-up work

Example:

```cmd
meta-convert analytics-to-multi-dimensional --workspace .\MetaAnalytics.Workspace --out .\MetaMultiDimensional.Workspace
meta-multi-dimensional add-named-set --workspace .\MetaMultiDimensional.Workspace --id TopDates --cube Commerce:cube --name TopDates --expression "TOPCOUNT([Date].[DateKey].MEMBERS, 10, [Measures].[Sales Amount])"
meta-multi-dimensional add-cube-action --workspace .\MetaMultiDimensional.Workspace --id SalesDrillthrough --cube Commerce:cube --name "Sales Drillthrough" --action-type DrillThrough --target-kind Cells --expression DRILLTHROUGH
meta-multi-dimensional add-cell-permission --workspace .\MetaMultiDimensional.Workspace --id ReaderSalesCells --security-role Readers --cube Commerce:cube --expression "Measures.CurrentMember IS [Measures].[Sales Amount]"
meta-multi-dimensional deploy --workspace .\MetaMultiDimensional.Workspace --server localhost\MULTI --database-name Commerce --drop-existing
meta-multi-dimensional restore --source-server preprod-multi --source-database-name Commerce --target-server prod-multi --target-database-name Commerce --backup-file \\fileserver\ssas-backups\Commerce.abf --drop-existing --overwrite-backup-file
meta-multi-dimensional drop --server localhost\MULTI --database-name Commerce
```

Runnable deploy demo:

```cmd
Samples\Demos\MetaMultiDimensionalDeployCliIntegration\run.cmd
```

### meta-convert analytics targets

Purpose:
- project a portable `MetaAnalytics` workspace into one target implementation workspace

Command surface:
- `meta-convert analytics-to-tabular [--workspace <path>] --out <path>`
- `meta-convert analytics-to-multi-dimensional [--workspace <path>] --out <path>`

Behavior summary:
- `MetaAnalytics` is the common authoring surface
- `MetaTabular` and `MetaMultiDimensional` are target implementation surfaces where users patch engine-specific details before target-owned realization
- conversion is deterministic and fails clearly when a conceptual row cannot be represented in the selected target
- the converter does not try to make tabular-only or multidimensional-only concepts portable by hiding them in `MetaAnalytics`

### meta-sql

Purpose:
- plan and apply manifest-driven SQL Server deployment from sanctioned `MetaSql` workspaces

Command surface:
- `meta-sql deploy-plan --source-workspace <path> --connection-env <name> --out <path> [--approve-drop-table <schema.table>] [--approve-drop-column <schema.table.column>] [--approve-truncate-column <schema.table.column>] [--approval-file <path>]`
- `meta-sql deploy --manifest-workspace <path> --source-workspace <path> --connection-env <name>`

Behavior summary:
- `deploy-plan` extracts live schema when the target database exists, otherwise treats live as truly empty and writes a deploy manifest against that empty live state
- `deploy-plan` and `deploy` always operate on the full source workspace and full live database; filtered subset deploy is not supported
- destructive actions require exact object-scoped approvals
- `deploy` executes only the manifest after source/live fingerprint validation
- when the manifest expects a missing target database, `deploy` creates it first and refuses if it already exists
- schema creation is explicit in the manifest (`AddSchema`), not inferred while rendering table DDL
- ad-hoc SQL script execution is not a `MetaSql` product surface; use setup-specific tooling such as `sqlcmd` for demo/bootstrap scripts

Examples:

`--connection-env` names the shell-visible environment variable that contains the target SQL Server connection string.

```cmd
meta-sql deploy-plan --source-workspace .\CurrentMetaSql.Workspace --connection-env META_SQL_DEV --out .\out\deploy-manifest
meta-sql deploy --manifest-workspace .\out\deploy-manifest --source-workspace .\CurrentMetaSql.Workspace --connection-env META_SQL_DEV
```

### meta-pipeline

Purpose:
- execute one modeled `MetaPipeline` unit as a declared serial task chain
- run one bound `MetaTransformScript.TransformScript` per transform-backed task
- materialize SELECT-kind scripts through `InsertRows`
- execute SQL-shaped mutation scripts directly through their modeled SQL statements
- keep stage 1 centered on SQL Server execution, bounded row buffering, honest failures, and operational evidence

Command surface:
- `meta-pipeline help`
- `meta-pipeline --new-workspace <path>`
- `meta-pipeline add-pipeline --workspace <path> --name <name> [--description <text>]`
- `meta-pipeline add-step --workspace <path> --pipeline <name> --script <name-or-id> --transform-workspace <path> --binding-workspace <path> --execution-connection-env <name> [--step-name <name>] [--binding <id>] [--target-connection-env <name>] [--target <sql-identifier>] [--target-write <insert-rows>] [--batch-size <n>] [--timeout-seconds <n>] [--target-data-type-system <name>]`
- `meta-pipeline inspect --workspace <path>`
- `meta-pipeline execute --workspace <path> --pipeline <name> --transform-workspace <path> --binding-workspace <path> [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env <name>]`
- `meta-pipeline execute-step --workspace <path> --pipeline <name> --step-name <name-or-id> --transform-workspace <path> --binding-workspace <path> [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env <name>]`
- `meta-pipeline execute-sqlserver --transform-workspace <path> --binding-workspace <path> --script <name-or-id> [--binding <id>] --execution-connection-env <name> [--target-connection-env <name>] [--target <sql-identifier>] [--batch-size <n>] [--timeout-seconds <n>] [--target-data-type-system <name>] [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env <name>]`
- `meta-pipeline create-pipeline-db --pipeline-db-connection-env <name> [--pipeline-db-name <name>]`
- `meta-pipeline prune-pipeline-db --pipeline-db-connection-env <name> --retention-days <days> [--dry-run]`

Behavior summary:
- `MetaTransformScript` owns SQL statement semantics, including SQL-shaped mutation statements such as `MERGE`
- `MetaPipeline` executes modeled transform scripts and handles buffering/row movement where rowsets are materialized
- `execute` is the modeled path and resolves one connected serial `PipelineTask` chain
- `execute-step` executes one transform-backed task and its paired `InsertRows` target write when the selected script is SELECT-kind; it is the task-grain worker surface used by `MetaOrchestration`
- every transform-backed task persists the resolved transform script id and transform binding id
- CLI authoring selects scripts through `--script` by exact `TransformScript.Name` first, with exact id fallback; `--binding` is only needed when multiple bindings reference the selected script
- SELECT-kind scripts must feed exactly one adjacent `InsertRows` target-write task
- non-SELECT scripts execute directly and must not feed an `InsertRows` target write
- `add-step` appends to the current terminal task in the serial chain
- `InsertRows` is the only supported modeled target write in stage 1
- `InsertRowsTargetWriteTask.TargetDataTypeSystemName` records the runtime target type family; omitted defaults to `SqlServer`
- `--data-type-conversion-workspace` selects the type-conversion policy workspace; omitted uses the built-in sanctioned defaults
- the modeled path resolves execution/target connection references through environment-variable names stored in `ConnectionReference` rows
- if the selected binding contains multiple targets, `--target` is required
- stage 1 execution supports parameterless transform scripts and one selected target per run
- modeled transform executions can declare `TimeoutSeconds`; direct `execute-sqlserver` can use `--timeout-seconds`; omitted means no SQL command timeout
- mutation task row-count evidence uses SQL Server rows affected where SQL Server reports it
- attached-console execution shows compact live operator progress: step count, elapsed time, rows, batches, and automatic B/KB/MB/GB rate; redirected/headless runs stay quiet
- `--pipeline-db-connection-env` records diagnostic logs separately from audit-relevant run logs, task runs, metrics, audit ids, workspace fingerprints, and failures in an initialized operational DB
- timeout evidence is task-level (`TaskRun.TimeoutSeconds`), while workspace fingerprints are recorded in `RunFingerprint` using SHA-256 over the executed XML workspaces
- `create-pipeline-db` creates or updates the SQL Server operational DB schema; `--pipeline-db-name` defaults to `MetaPipeline`
- `prune-pipeline-db` prunes old `RunDiagnosticsLog` rows while preserving run lineage, audit logs, audit ids, metrics, fingerprints, and failures
- the operational DB is runtime evidence only; it is not model truth, run-plan state, watermarks, checkpoints, or orchestration policy

Example:

```cmd
meta-pipeline create-pipeline-db --pipeline-db-connection-env META_PIPELINE_ADMIN_SQL --pipeline-db-name MetaPipeline

meta-pipeline --new-workspace .\PipelineWS
meta-pipeline add-pipeline --workspace .\PipelineWS --name CustomerLoad
meta-pipeline add-step --workspace .\PipelineWS --pipeline CustomerLoad --step-name load-customers --script dbo.v_customer_load --transform-workspace .\TransformWS --binding-workspace .\BindingWS --execution-connection-env META_PIPELINE_EXECUTION --target-connection-env META_PIPELINE_TARGET --target dbo.TargetCustomer --timeout-seconds 300 --target-data-type-system SqlServer
meta-pipeline execute --workspace .\PipelineWS --pipeline CustomerLoad --transform-workspace .\TransformWS --binding-workspace .\BindingWS
meta-pipeline execute-sqlserver --transform-workspace .\TransformWS --binding-workspace .\BindingWS --script dbo.v_customer_load --execution-connection-env META_PIPELINE_EXECUTION --target-connection-env META_PIPELINE_TARGET --timeout-seconds 300 --target-data-type-system SqlServer
```

### meta-orchestration

Purpose:
- infer a task-level dependency graph from bound `MetaPipeline` transform steps
- use `MetaTransformBinding` rowset/target profiles as the minimum dependency input
- keep orchestration state in a sanctioned workspace
- separate data dependency, write determinism, and runtime synchronization concerns

Command surface:
- `meta-orchestration help`
- `meta-orchestration --pipeline-workspace <path> --transform-workspace <path> --binding-workspace <path> --new-workspace <path> [--description <text>]`
- `meta-orchestration inspect --workspace <path>`
- `meta-orchestration list-issues --workspace <path>`
- `meta-orchestration explain-issue --workspace <path> --issue <id-or-unique-code>`
- `meta-orchestration add-dependency --workspace <path> --from-task <task> --to-task <task> --condition <success|failure> [--object <sql-identifier>] [--reason <text>]`
- `meta-orchestration add-order --workspace <path> --from-task <task> --to-task <task> [--condition <success|failure>] [--object <sql-identifier>] [--reason <text>]`
- `meta-orchestration allow-concurrent-append --workspace <path> --object <sql-identifier> [--reason <text>]`
- `meta-orchestration set-lock-policy --workspace <path> --object <sql-identifier> --left-effect <effect> --right-effect <effect> --behavior <serialize|allow> [--reason <text>]`
- `meta-orchestration refresh-run-plan --workspace <path>`
- `meta-orchestration inspect-run-plan --workspace <path>`
- `meta-orchestration execute --workspace <path> --pipeline-workspace <path> --transform-workspace <path> --binding-workspace <path> [--data-type-conversion-workspace <path>] [--pipeline-db-connection-env <name>] [--max-degree-of-parallelism <n>]`

Behavior summary:
- orchestration does not parse or bind SQL; it consumes already-bound transform metadata
- scalar function definitions in a transform workspace are treated as helper objects; if a pipeline task references one directly, orchestration records a blocking `NonExecutableTransformScript` issue instead of treating it as an unknown SQL statement
- source reads surfaced from same-workspace scalar function return-expression bodies participate in normal dependency inference for executable transforms that call those functions
- there is no empty `init` surface today; root `--new-workspace` creates the orchestration workspace by inference, and `refresh-run-plan --workspace` writes run-plan rows into that same workspace
- each modeled pipeline becomes a `PipelineReference`
- ordered transform steps become `TaskAccessProfile` rows
- object reads/writes become `ObjectAccess` and `PipelineObjectAccess` rows
- derived `TaskObjectEffect` rows classify producer/consumer, write effect, purpose, synchronization, and lock intent
- data dependencies become `TaskDependency` rows first, then cross-pipeline summaries become `PipelineDependency` rows
- task dependencies carry a condition: normal inferred flow is `OnSuccess`, and authored failure branches use `OnFailure`
- a lone destructive access is allowed when no other pipeline touches the object
- same-pipeline truncate plus write is treated as an isolated replace sequence
- same-target append writers can keep `DagStatus=Complete` while recording synchronization constraints
- replacement mixed with append or same-table mutations can keep `DagStatus=Complete` while setting `DeterminismStatus=RequiresExplicitOrdering`
- unsafe shared reset and true dependency cycles make `DagStatus=Invalid`
- explicit task-ordering resolutions and scoped lock compatibility policies are workspace rows
- `refresh-run-plan` writes `RunPlan`, dependency-ordered `PlannedTask`, and `PlannedTaskLock` rows using task dependencies plus conservative lock policy
- `execute` refreshes current run-plan rows from the orchestration workspace and starts ready `meta-pipeline execute-step` child processes by traversing the dependency graph
- attached-console `execute` renders one compact live progress line with the current load/plan/save phase or task count and running task names; redirected/headless runs stay quiet
- execution continues viable DAG paths by default; failed tasks block only downstream dependents, while unrelated paths continue
- blocked dependent success branches are reported as skipped, not failed
- failure branches are ordinary planned tasks reached through `OnFailure` dependency edges, not post-run action hooks
- an unsatisfied failure branch after a successful predecessor is skipped as a normal unchosen branch
- `allow` lock behavior is currently accepted only for `Append`/`Append`; other effect pairs can be explicitly serialized until stronger safety contracts are modeled

Example:

```cmd
meta-orchestration --pipeline-workspace .\PipelineWS --transform-workspace .\TransformWS --binding-workspace .\BindingWS --new-workspace .\OrchestrationWS
meta-orchestration list-issues --workspace .\OrchestrationWS
meta-orchestration add-order --workspace .\OrchestrationWS --from-task Stage.Refresh --to-task Stage.Append --object dbo.Stage
meta-orchestration add-dependency --workspace .\OrchestrationWS --from-task Stage.Load --to-task FailureHandler.Record --condition failure
meta-orchestration allow-concurrent-append --workspace .\OrchestrationWS --object dbo.Stage
meta-orchestration set-lock-policy --workspace .\OrchestrationWS --object dbo.Stage --left-effect Mutation --right-effect Mutation --behavior serialize
meta-orchestration refresh-run-plan --workspace .\OrchestrationWS
meta-orchestration inspect-run-plan --workspace .\OrchestrationWS
meta-orchestration execute --workspace .\OrchestrationWS --pipeline-workspace .\PipelineWS --transform-workspace .\TransformWS --binding-workspace .\BindingWS --max-degree-of-parallelism 4
```

Demo:
- `Samples\Demos\MetaOrchestrationCliIntegration\run.cmd` builds the focused complete DAG/run-plan/execution scenario. The same folder also provides `run-all.cmd` plus named scenario scripts for write-order policy, invalid DAG evidence, and modeled failure dependency execution.

### meta-transform-script

`MetaTransformScript` provides a semantically round-trippable SQL statement syntax model for a supported bounded SQL surface. It imports supported SQL into modeled workspace rows, emits semantically equivalent SQL back out, and proves stability through `SQL -> workspace -> SQL -> workspace` plus `meta instance diff`.

Purpose:
- author and maintain a sanctioned `MetaTransformScript` workspace for the supported SQL statement subset
- import supported SQL from files or inline code into modeled workspace rows
- emit semantically equivalent SQL back out of that workspace
- prove the core invariant `SQL -> workspace -> SQL -> workspace` with `meta instance diff`
- serve as the authored syntax substrate for later binding, type inference, and validation layers

Command surface:
- `meta-transform-script help`
- `meta-transform-script from sql-file --path <file.sql> [--target <sql-identifier>] (--new-workspace <path> | --workspace <path>)`
- `meta-transform-script from sql-files --manifest <manifest.tsv> (--new-workspace <path> | --workspace <path>) [--report <report.tsv>] [--verbose]`
- `meta-transform-script from sql-code --code <sql> [--target <sql-identifier>] (--new-workspace <path> | --workspace <path>) [--name <name>]`
- `meta-transform-script to sql-path [--workspace <path>] --out <path>`
- `meta-transform-script to sql-code [--workspace <path>] [--name <name>]`

What the model is:
- this is a typed syntax model with deterministic workspace rows for supported SQL structure
- the modeled truth is the supported SQL statement, rooted in the `TSqlStatement` family
- `CREATE VIEW` and inline table-valued-function wrapper syntax are treated as import/export envelopes around SELECT-kind scripts, not as the primary modeled truth
- supported scalar `CREATE FUNCTION` wrappers are modeled as scalar script objects with parameters, return type, and one return expression
- wrapper details captured in the model are:
  - view schema identifier
  - view object identifier
  - explicit view column list
  - function schema identifier
  - function object identifier
  - function parameters and parameter data types
  - scalar function return type
- round-trip is semantic, not trivia-preserving:
  - original whitespace
  - comments
  - token offsets
  - exact file formatting
  are not part of the contract
- binding, type inference, target validation, and lineage are follow-on layers built on top of this syntax model; they do not replace it

Model entity list:
- script and statement spine:
  - `TransformScript`
  - `TransformScriptStatementLink`
  - `ScriptObjectView`
  - `ScriptObjectTVF`
  - `ScriptObjectScalarFunction`
  - `TSqlStatement`
  - `SelectStatement`
  - `InsertStatement`
  - `UpdateStatement`
  - `DeleteStatement`
  - `TruncateStatement`
  - `MergeStatement`
  - `StatementWithCtesAndXmlNamespaces`
  - `QueryExpression`
  - `QuerySpecification`
  - `CommonTableExpression`
- source/table side:
  - `FromClause`
  - `TableReference`
  - `NamedTableReference`
  - `QualifiedJoin`
  - `QueryDerivedTable`
  - `PivotedTableReference`
  - `UnpivotedTableReference`
  - `FullTextTableReference`
- expression side:
  - `SelectElement`
  - `ScalarExpression`
  - `BooleanExpression`
  - `FunctionCall`
  - `CaseExpression`
  - `ColumnReferenceExpression`
  - `ScalarSubquery`
  - `OverClause`
  - `WindowDefinition`
- support entities:
  - `Identifier`
  - `MultiPartIdentifier`
  - `SchemaObjectName`
  - `Literal`
  - `DataTypeReference`
- structural helper entities:
  - `*Link` rows carry optional or structured relationships explicitly
  - `*Item` rows preserve ordered collections explicitly

These entities and helper rows are what the workspace persists for the supported SQL structure.

Import behavior:
- `from sql-file` accepts one `.sql` file per command invocation
- `from sql-files` accepts an explicit TSV manifest and imports many `.sql` files in one process, loading the target workspace once and saving successful imports once
- manifest rows use a required `Path` column and an optional `Target` column; relative paths resolve from the manifest file, `CREATE VIEW` and bare `SELECT` rows require `Target`, and inline TVF/scalar UDF rows leave `Target` blank
- bulk import continues after per-file failures, writes an optional per-file report through `--report`, and exits nonzero when any file failed so downstream binding/DQ cannot treat an incomplete corpus as clean
- report failure rows include `FailureKind`, `ErrorSummary`, `Line`, `Column`, and the full diagnostic `Message`, so corpus harnesses can group failures without scraping long console text
- imports are single-transform only; the file/input must produce exactly one transform script
- `CREATE VIEW` and bare `SELECT` imports require `--target <sql-identifier>`
- inline `CREATE FUNCTION` (inline TVF) and scalar `CREATE FUNCTION` imports must not specify `--target`
- bare mutation imports must not specify `--target`; `sql-code` requires `--name`, while `sql-file` uses the file stem when no wrapper name exists
- use `--new-workspace` for the first script and `--workspace` to append subsequent scripts one-by-one
- a `.sql` file may contain:
  - one supported bare `SELECT`
  - one supported bare `INSERT`, `UPDATE`, `DELETE`, `TRUNCATE`, or `MERGE`
  - one supported `CREATE VIEW ... AS ... GO` statement
  - one supported inline `CREATE FUNCTION ... RETURNS TABLE ...` statement
  - one supported scalar `CREATE FUNCTION ... RETURNS <scalar type> ... RETURN <scalar expression>` statement
  - batches with leading `SET ...` statements
- explicit view column lists in `CREATE VIEW` are captured and emitted back out
- `from sql-code` imports SQL text directly, and takes `--name` when the input is a bare statement without a wrapper object name

Export behavior:
- `to sql-code` emits the modeled statement for statement-backed scripts; scalar function scripts emit the function wrapper because their body is owned by `ScriptObjectScalarFunction`
- `to sql-path` emits `CREATE VIEW ... AS ... GO`, inline TVF wrappers, or scalar function wrappers where modeled; mutation scripts emit as statements
- if `--out` ends with `.sql`, all scripts are emitted into one combined file
- otherwise `--out` is treated as a target folder
- folder export preserves the original `SourcePath` file names when possible so a re-import can round-trip deterministically

Supported SQL surface today:
- statement roots:
  - `SELECT`
  - bounded `INSERT`
  - bounded `UPDATE`
  - bounded `DELETE`
  - bounded `TRUNCATE TABLE`
  - bounded `MERGE` for DW/ETL patterns, including CTE prefix, `TOP`, currently modeled target hints, `WHEN ... AND`, SQL Server-constrained repeated matched/source clauses, `OUTPUT`, `OUTPUT INTO`, currently modeled `OPTION` hints, and required SQL Server semicolon
- query roots and composition:
  - `SELECT`
  - parenthesized query expressions
  - common table expressions (`WITH ... AS (...)`)
  - `WITH XMLNAMESPACES`
  - `UNION`
  - `UNION ALL`
  - `EXCEPT`
  - `INTERSECT`
- projection:
  - named select items
  - aliases
  - `*`
  - qualified `alias.*`
- table sources:
  - named table references
  - aliases
  - qualified joins
  - unqualified joins
  - `CROSS APPLY`
  - `OUTER APPLY`
  - query-derived tables
  - inline `VALUES`
  - parenthesized joins
  - schema-object function table references
  - built-in table functions covered by the reference corpus
  - `PIVOT`
  - `UNPIVOT`
  - `TABLESAMPLE`
  - full-text table references such as `CONTAINSTABLE` / `FREETEXTTABLE`
- filtering and predicate forms:
  - boolean `AND` / `OR`
  - `NOT`
  - parenthesized boolean expressions
  - scalar comparison predicates
  - `IS NULL` / `IS NOT NULL`
  - `BETWEEN`
  - `IN (...)`
  - `IN (subquery)`
  - `EXISTS`
  - subquery comparison predicates with `ANY` / `ALL`
  - `LIKE`
  - `IS DISTINCT FROM`
  - full-text predicates such as `CONTAINS` / `FREETEXT`
- grouping and aggregation:
  - `GROUP BY`
  - `HAVING`
  - aggregate function calls in projection and other supported expression positions
  - `GROUPING SETS`
  - `ROLLUP`
  - `CUBE`
  - composite and grand-total grouping shapes covered by the corpus
- ordering and row limiting:
  - query-level `ORDER BY`
  - `TOP`
  - `OFFSET ... FETCH`
  - sort order per element
- windowing:
  - `OVER`
  - `PARTITION BY`
  - window `ORDER BY`
  - window frames
  - named `WINDOW` definitions
  - windowed aggregate and analytic functions covered by the corpus
  - percentile analytic/window functions with `WITHIN GROUP` such as `PERCENTILE_CONT` and `PERCENTILE_DISC`
- scalar/value expression families:
  - column references
  - multipart identifiers
  - string, integer, numeric, money, binary, `NULL`, and `MAX` literal families covered by the corpus
  - arithmetic binary expressions
  - unary expressions
  - parenthesized expressions
  - `CASE`
  - `COALESCE`
  - `NULLIF`
  - `IIF`
  - ordinary function calls
  - parameterless/system-call style expressions covered by the corpus
  - `CAST`
  - `TRY_CAST`
  - `CONVERT`
  - `TRY_CONVERT`
  - `PARSE`
  - `TRY_PARSE`
  - parameterized data type references
  - SQL Server national string literals such as `N'...'`
  - primary-expression collation
  - `AT TIME ZONE`
  - sequence/global expression cases covered by the corpus
- subqueries:
  - scalar subqueries
  - correlated subqueries in the supported expression and predicate forms exercised by the corpus
- XML-in-view support:
  - `WITH XMLNAMESPACES`
  - XML method-style calls as exercised in the reference corpus, for example `.value(...)`, `.query(...)`, and `.exist(...)`
  - XML `nodes(...)` table sources

Data type alias note:
- import accepts selected SQL Server type aliases in supported type-reference positions
- aliases are normalized to sanctioned canonical SQL type names in the model and emitter output
- examples: `integer -> int`, `sysname -> nvarchar(128)`, `character varying -> varchar`, `double precision -> float`, `national character varying -> nvarchar`

Detailed parser/emitter checklist:
- the exact implemented and verified surface is tracked in [docs/meta-transform-script/META-TRANSFORM-SCRIPT-PARSER-STATUS.md](docs/meta-transform-script/META-TRANSFORM-SCRIPT-PARSER-STATUS.md)
- open items there are ordinary parser/emitter/model/import-shaping gaps

Unsupported or Excluded Surface:
- `OPENJSON`
- `OPENROWSET`
- `OPENQUERY`
- provider/ad-hoc external-source wrapper forms such as `OPENROWSET` provider and `OPENDATASOURCE` reference cases
- `CHANGETABLE`
- the ODBC escape-surface reference case
- `CREATE VIEW` wrapper options
- `WITH CHECK OPTION`
- materialized view syntax
- multistatement table-valued functions such as `RETURNS @Output TABLE`
- procedural scalar UDF bodies with local `DECLARE` / `SET` / `IF` statement flow

Scalar UDF support note:
- current scalar UDF support is deliberately small and structural: parameters, scalar return type, and a body reducible to a single return expression
- `dbo.fnTidBK`-style bodies are plausible as a generic expression-lowering pass, but are not yet accepted
- details are tracked in [docs/meta-transform-script/META-TRANSFORM-SCRIPT-SCALAR-UDFS.md](docs/meta-transform-script/META-TRANSFORM-SCRIPT-SCALAR-UDFS.md)

Reference corpus status:
- `MetaTransform\Script\Reference\Corpus` contains the broader SQL corpus used to pressure the importer/emitter
- the reference-corpus round-trip demo uses the supported subset of that corpus and excludes the unsupported surfaces listed above
- the exact supported parser/emitter surface and proof cases are tracked in `docs/meta-transform-script/META-TRANSFORM-SCRIPT-PARSER-STATUS.md`
- the proof point is `meta instance diff` reporting no differences between the original and round-tripped workspaces

Reference corpus demo commands:

```cmd
cd Samples\Demos\MetaTransformScriptReferenceCorpusCliIntegration
call cleanup.cmd

meta-transform-script from sql-file --path SourceViews\001_basic_select\view.sql --target dbo.v_basic --new-workspace MetaTransformScriptReferenceCorpusWorkspace

pushd MetaTransformScriptReferenceCorpusWorkspace
meta-transform-script to sql-path --out ..\RoundTrippedViews
meta-transform-script to sql-path --out ..\RoundTrippedViews.sql
meta-transform-script to sql-code --name dbo.v_window_functions
popd

meta-transform-script from sql-file --path RoundTrippedViews\view.sql --target dbo.v_basic --new-workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta instance diff MetaTransformScriptReferenceCorpusWorkspace MetaTransformScriptReferenceCorpusRoundTripWorkspace

pushd MetaTransformScriptReferenceCorpusRoundTripWorkspace
meta-transform-script to sql-code --name dbo.v_xml_namespaces_and_methods
popd
```

Captured output excerpt from `Samples\Demos\MetaTransformScriptReferenceCorpusCliIntegration\run.output`:

```text
> meta-transform-script from sql-file --path SourceViews\001_basic_select\view.sql --target dbo.v_basic --new-workspace MetaTransformScriptReferenceCorpusWorkspace
Ok

> meta-transform-script from sql-file --path RoundTrippedViews\view.sql --target dbo.v_basic --new-workspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
Ok

> meta instance diff MetaTransformScriptReferenceCorpusWorkspace MetaTransformScriptReferenceCorpusRoundTripWorkspace
Instance diff: no differences.
Rows: left=4996, right=4996  Properties: left=8348, right=8348
NotIn: left-not-in-right=0, right-not-in-left=0
```

### meta-transform-binding

`MetaTransformBinding` is the binding contract layer on top of `MetaTransformScript`.

Purpose:
- bind all transform scripts in a transform workspace into an explicit binding workspace (`rowsets`, `columns`, source/target SQL identifiers)
- validate source and target contracts against explicit schema workspaces in the same command
- fail hard on contract mismatches and persist explicit validation link rows in the resulting workspace

Command surface:
- `meta-transform-binding help`
- `meta-transform-binding bind --transform-workspace <path> --source-schema <path> [--source-schema <path> ...] --target-schema <path> --execute-system <name> --new-workspace <path> [--execute-system-default-schema-name <schema>] [--ignore-target-columns <col[,col...]>] [--data-type-conversion-workspace <path>]`

Behavior summary:
- `bind` reads the target SQL identifier from view/mutation transform metadata; inline TVF and scalar function definitions are targetless helper scripts
- scalar function definitions materialize as binding rows without rowsets or targets, while scalar function call sites inside views/statements still bind their argument expressions and same-workspace scalar-function return-expression body sources
- source identifiers resolve against source schema workspaces; target identifiers resolve against the target schema workspace
- source identifier resolution is explicit:
  - `system.schema.table` resolves directly
  - `schema.table` resolves as `<execute-system>.<schema>.<table>`
  - `table` resolves as `<execute-system>.<execute-system-default-schema-name>.<table>`
- `--execute-system` is required and must be represented in provided source schema workspaces when one/two-part source identifiers exist
- if any one-part source identifier exists, `--execute-system-default-schema-name` is required
- each source/target schema workspace must contain exactly one system
- `bind` enforces target write-contract shape using non-identity target fields
- `bind` processes all transform scripts in the transform workspace
- `--ignore-target-columns` excludes named non-identity target columns from target conformance checks; unknown names fail explicitly
- `--data-type-conversion-workspace` selects the sanctioned conversion policy workspace used for type compatibility checks; omitted uses the built-in defaults
- source-to-target data type conformance is checked as exact or sanctioned conversion path, not hardcoded widening logic
- bind is atomic: if binding or validation fails, no output workspace is created
- scale proof is included in `Samples\Demos\MetaTransformScriptTpcDsCliIntegration\run.cmd`, which imports and binds TPC-DS `q01`-`q99` in one workspace run

Examples:

```cmd
meta-transform-binding bind --transform-workspace .\TransformWS --source-schema .\SourceSchemaWS --target-schema .\TargetSchemaWS --execute-system WarehouseDb --new-workspace .\BindingWS

meta-transform-binding bind --transform-workspace .\TransformWS --source-schema .\SalesSchemaWS --source-schema .\ReferenceSchemaWS --target-schema .\WarehouseSchemaWS --execute-system WarehouseDb --execute-system-default-schema-name dbo --new-workspace .\BindingWS --ignore-target-columns LoadUtc,RunId --data-type-conversion-workspace .\MetaDataTypeConversion.Workspace
```

See also:
- `Samples\Demos\MetaTransformBindingCliIntegration\run.cmd`
- `Samples\Demos\MetaTransformBindingCliIntegration\README.md`
- `Samples\Demos\MetaTransformScriptTpcDsCliIntegration\run.cmd`

### meta-data-quality

`meta-data-quality` derives sanctioned `MetaDataQuality` candidates from a `MetaTransformScript` workspace.

It traverses the typed semantic `MetaTransformScript` graph (not raw SQL text): `TransformScript` -> `SelectStatement` -> `QueryExpression` -> `QuerySpecification` -> `FromClause` -> `TableReference` / `QualifiedJoin`, including CTE scopes, derived tables, and modeled boolean expressions.

Purpose:
- discover reviewable DQ candidates from transform semantics
- persist candidates in a sanctioned `MetaDataQuality` workspace
- convert promoted candidates into executable SQL DQ assets

Command surface:
- `meta-data-quality help`
- `meta-data-quality from-transform-workspace --transform-workspace <path> --new-workspace <path>`
- `meta-data-quality inspect --workspace <path> [--show-cases] [--show-candidate-ids]`
- `meta-data-quality promote --workspace <path> (--all | --candidate-id <id> [--candidate-id <id> ...])`

Workflow:

```cmd
meta-data-quality from-transform-workspace --transform-workspace .\MetaTransformScript.Workspace --new-workspace .\MetaDataQuality.Workspace
meta-data-quality inspect --workspace .\MetaDataQuality.Workspace
meta-data-quality promote --workspace .\MetaDataQuality.Workspace --all
meta-convert data-quality-to-sql --workspace .\MetaDataQuality.Workspace --out .\DataQualityViews.sql
```

Analysis scopes:
1. Transform-scope analysis (single script): detects missing referenced rows, unexpected outer-join nulls, row multiplication risk, duplicate output risk, and records modeled join evidence (`JoinPattern`, `JoinPatternOccurrence`, key parts).
2. Corpus-scope analysis (workspace-wide): aggregates repeated relationship evidence and infers dominant vs outlier behavior across the transform corpus.

Completed corpus capabilities:
- relationship consensus:
`MinorityJoinPattern`, `IncompleteCompositeJoin`, `SuspiciousExtraJoinPredicate`
- implied relationship-level integrity:
`ImpliedForeignKeyMissingReference`, `ImpliedUniqueKeyViolation`
- optionality drift:
`InnerJoinAgainstUsuallyOptionalRelationship`, `LeftJoinAgainstUsuallyMandatoryRelationship`
- column equivalence (first slice):
`MinorityColumnEquivalence`
- implied cardinality-risk signals (from repeated unsuppressed transform-scope risk signals):
`ImpliedJoinFanoutRisk`, `ImpliedOutputDuplicateRisk`
- filter consensus (first slice):
`MissingCommonFilter`

Promotion and SQL generation:
- discovery never auto-promotes; promotion is explicit and persisted in `MetaDataQuality`
- SQL generation only uses promoted candidates
- SQL output modes:
- `RuntimeCheck`: promoted runtime-checkable candidates generate executable SQL checks against source data (`ImpliedForeignKeyMissingReference`, `ImpliedUniqueKeyViolation`)
- `SemanticReviewFinding`: promoted semantic candidates generate informational review findings (`MinorityJoinPattern`, `IncompleteCompositeJoin`, `SuspiciousExtraJoinPredicate`, `InnerJoinAgainstUsuallyOptionalRelationship`, `LeftJoinAgainstUsuallyMandatoryRelationship`, `MinorityColumnEquivalence`, `MissingCommonFilter`, `ImpliedJoinFanoutRisk`, `ImpliedOutputDuplicateRisk`)
- promoted unsupported families fail fast with a clear error (no silent skip)

Generated SQL assets:
- one DQ view per promoted candidate
- `dq.v_DataQualityReview` dashboard with both runtime and semantic findings
- `MetaDQ` operational objects:
- tables `dbo.RunLog`, `dbo.FindingLog`
- procedures `dbo.Run`, `dbo.Findings`
- `dbo.Run` executes review checks and persists run evidence; `dbo.Findings` returns actionable findings for a selected run

Reviewer-facing dashboard semantics:
- `FindingGroupCount`: number of result rows/groups returned by a runtime finding view
- `SuspectRowCount`: sum of suspect rows represented by those groups
- runtime findings expose explicit direction columns (`ReferencingObject`, `ReferencedObject`, `CheckedObject`, `SuspectSide`)
- semantic findings are explicit (`OutputMode=SemanticReviewFinding`) and keep runtime count columns `NULL`

Real SQL Server end-to-end demo:
- `Samples\Demos\MetaDataQualityRealDbCliIntegration\run.cmd`
- proves mixed promoted runtime + semantic conversion, dashboard output, and `MetaDQ` run persistence on real data



