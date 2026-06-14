# Agent guide: using meta-bi CLIs

This guide is generic. It explains how agents should approach BI work with `meta-bi` command-line tools. Pair it with a task prompt that names the actual source system, business requirements, output folder, and connection variables.

## Core stance

Use modeled metadata as the durable product truth.

Source systems, SQL scripts, business requirements, and operational logs are inputs or evidence. The work should move structure into sanctioned workspaces so it can be inspected, converted, validated, deployed, and operated.

Do not replace source evidence with guesses. Extract it, inspect it, model it, and carry it forward.

When a CLI surface is missing or insufficient, record the gap clearly. Do not invent commands, fake generated outputs, or silently switch to an unrelated artifact type.

## Agent operating loop

Work like this:

1. Read the business requirements.
2. Read this guide.
3. Create a clean run folder.
4. Discover the available CLI surface with `where`, `--help`, `help`, README files, and `docs/commands.md`.
5. Write `PLAN.md` before running artifact-producing commands.
6. Extract source schema from the live source before creating product artifacts.
7. Inspect the extracted source schema enough to choose a bounded analytical slice.
8. Decide the stack path and record the decision.
9. Generate visible `.cmd` stage scripts.
10. Execute stages as far as the local tooling and environment allow.
11. Record outputs, snags, and the next honest step.

The agent should keep moving, but not pretend. If a command does not exist, a required workspace cannot be created, or a deployment target is unavailable, record the exact blocker and continue with the nearest truthful slice.

For recorded demo work, treat failed or diagnostic run folders as disposable. If product code, model shape, environment, or worker execution blocks the run, halt at that point, name the blocker, apply the correction outside the accepted run, then rerun the demo from a fresh clean run folder. The accepted evidence should be the clean rerun, not a folder full of patched-over attempts.

## Plan-first execution

Do not try to one-shot a full BI demo. A full stack has too many semantic and operational gates to compress into one unreviewed jump.

Before running the first product command, create `PLAN.md` with:

- source databases and connection variables
- target databases and analytical server/database names
- layer plan: SourceDBs, RDV, BDV, DW/Mart, Analytics, Tabular or MultiDimensional
- planned folder names for every layer and database
- stage scripts to create
- expected evidence after each stage
- known unknowns or CLI surfaces to inspect

Run the work in phases. Each phase should leave scripts, logs, and a short status note before the next phase starts:

1. Plan and CLI discovery.
2. Source readiness and source schema extraction.
3. RDV modeling/realization evidence.
4. BDV modeling/realization evidence.
5. DW/mart modeling, transforms, target contracts, binding, and DQ.
6. SQL deployment and smoke checks.
7. Pipeline and orchestration modeling for the table-load transforms.
8. Orchestration execution and DQ/mart proof.
9. Analytics/Tabular or MultiDimensional deploy/process proof from the mart.

If coordinating with a human or another agent, stop at phase boundaries for review. If running unattended, still write the gate evidence into `journal.md` and `summary.txt` before continuing.

If a phase fails during recording preparation, do not keep layering retries into the same accepted run folder. Use the failed folder only as temporary diagnostic evidence, fix the root cause, and start a new clean run from the beginning or from the agreed replay boundary.

## Expected stack shape

A useful full `meta-bi` BI stack usually has this business intelligence shape:

```text
SourceDBs -> RDV -> BDV -> DW/Mart -> Tabular or MultiDimensional
```

The supporting operating structure is:

```text
ETL transforms + binding + DQ + SQL realization + pipelines + orchestration
```

Layer responsibilities:

- Source database contracts: `MetaSchema` workspaces extracted from the real source databases.
- Raw Data Vault: RDV preserves source-grain, source-shaped, historized evidence so audits can trace analytical results back to original operational records.
- Business Data Vault: BDV for business-oriented integration, relationships, and business keys.
- Data Warehouse or mart: dimensional structures suitable for analytical engines.
- Analytical model: `MetaTabular` or `MetaMultiDimensional` for the target analytical surface.
- ETL transformations: SQL files and/or `MetaTransformScript` workspace data connecting layers.
- Binding evidence: `MetaTransformBinding` validation connecting transforms to source and target contracts.
- Data quality: `MetaDataQuality` candidates automatically derived from modeled transform structure and binding evidence where available.
- SQL realization: `MetaSql` deployable SQL assets and deploy manifests.
- Analytical intent: `MetaAnalytics` portable analytical concepts where useful before target-specific realization.
- Pipelines: `MetaPipeline` modeled execution steps for transforms first; deployment, processing, checks, and external commands are auxiliary only.
- Orchestration: `MetaOrchestration` dependency/run-plan structure inferred from modeled pipeline transform/binding profiles where safe, with explicit policy rows for cases the model cannot prove.
- Run artifacts: logs, reports, and a concise summary.

For a full BI stack demo, RDV and BDV are required layers. Do not replace them with a direct source-to-DW/mart shortcut and still call the result a full ETL system.

The reason is structural, not ceremonial:

- RDV keeps raw source evidence close to the source grain and preserves lineage/history before business reinterpretation.
- BDV integrates source evidence into business concepts without destroying the raw audit trail.
- DW/mart structures often reshape, aggregate, denormalize, or retain only current analytical state. They are delivery surfaces, not substitutes for source-grain historical evidence.

For demo work, the accepted path is:

```text
SourceDBs -> RDV -> BDV -> DW/Mart -> Tabular or MultiDimensional
```

A DW or mart layer is still required before tabular or multidimensional delivery. Analytical engines should consume a shaped analytical warehouse/mart surface, not raw source tables or vault internals.

Not every task outside a demo needs every layer. Use the smallest honest slice that proves the requested outcome, but keep the shape coherent. If a requested full-stack demo cannot build RDV or BDV because the current CLI surface is missing a needed command, record the product gap and mark the run partial. Do not silently skip the data-vault layers.

## Run folder shape

Use a task-specific run folder. Keep generated work out of source-controlled product folders unless the task explicitly asks for committed sample assets.

The accepted recording run should be clean: one plan, one ordered set of stage scripts, one `run.cmd`, and final logs from the successful path. Failed exploratory folders may be deleted once the blocker has been understood and corrected.

A practical run folder uses layer and database/system names. Avoid flat generic workspace names such as `SourceSchemaWS`, `TransformWS`, `BindingWS`, or `CurrentMetaSqlWS` in a full or multi-database run; those names collapse ownership and become ambiguous as soon as more than one database or layer exists. Also avoid appending `WS` to every folder. A folder with `workspace.xml` inside is already a workspace; name the folder by its role, such as `Schema`, `Transforms`, `Binding`, `Quality`, `Sql`, `Pipeline`, or `Orchestration`.

Use a shape like this:

```text
run.cmd
stages/
  00-source-readiness.cmd
  01-extract-source-schema.cmd
  02-author-rdv.cmd
  03-author-bdv.cmd
  04-author-dw-mart.cmd
  05-bind-layer-transforms.cmd
  06-data-quality.cmd
  07-realize-or-deploy-sql.cmd
  08-author-pipeline.cmd
  09-author-orchestration.cmd
  10-execute-orchestration.cmd
  11-author-analytics.cmd
  12-deploy-process-tabular.cmd
  13-final-analytics-proof.cmd
SNAG-LOG.md
PLAN.md
journal.md
summary.txt
logs/
source/
  SourceDatabase/
    Schema/
rdv/
  RawVaultDatabase/
    RawVault/
    Transforms/
    Schema/
    Binding/
    Sql/
    DeployManifest/
bdv/
  BusinessVaultDatabase/
    BusinessVault/
    Transforms/
    Schema/
    Binding/
    Sql/
    DeployManifest/
dw/
  MartDatabase/
    Warehouse/
    Transforms/
    Schema/
    Binding/
    Quality/
    Sql/
    DeployManifest/
shared/
  DataTypeConversion/
analytics/
  Analytics/
  Tabular/ or MultiDimensional/
ops/
  Pipeline/
  Orchestration/
run-artifacts/
```

The top-level `run.cmd` should call stage scripts in order and stop on the first failure. Each stage script should echo the command it is about to run and leave normal CLI output visible.

The exact database folder names should come from the task and environment. For example, if the source is `AdventureWorks2022`, use `source\AdventureWorks2022\Schema`, not a root-level `SourceSchemaWS`.

## Source schema extraction

Start from source evidence. Do not hand-author source schema metadata when a live source can be extracted.

Typical shape:

```cmd
meta-schema extract sqlserver --new-workspace source\SourceSystem\Schema --connection-env SOURCE_SQL --system SourceSystem --all-schemas --all-tables
```

Choose the `--system` value deliberately. For bindable SQL transforms, the leading database/system identifier used in the SQL must match the `MetaSchema` system name for that source contract. If the SQL will reference a source database as `[SourceDatabase].[schema].[table]`, extract that source schema with `--system SourceDatabase`.

Good binding-aligned pattern:

```cmd
meta-schema extract sqlserver --new-workspace source\SourceDatabase\Schema --connection-env SOURCE_SQL --system SourceDatabase --all-schemas --all-tables
```

```sql
CREATE VIEW mart.vFactSales
AS
SELECT ...
FROM [SourceDatabase].[Sales].[SalesOrderHeader] AS h
JOIN [SourceDatabase].[Sales].[SalesOrderDetail] AS d
    ON d.SalesOrderID = h.SalesOrderID;
```

Do not confuse the source system name with `--execute-system`.

`--execute-system` is the database/context where the transform script is executed. It is used to resolve one-part and two-part source identifiers in the script. It is not a blanket replacement for the source database name used in three-part references.

For Extract (E), transform scripts are usually plain source-side `SELECT` statements. They are generally not stored in a database. They execute on the source system, so `--execute-system` is usually the source database/system:

```cmd
meta-transform-binding bind --transform-workspace source\SourceDatabase\ExtractTransforms --source-schema source\SourceDatabase\Schema --target-schema source\SourceDatabase\ExtractTargetSchema --execute-system SourceDatabase --new-workspace source\SourceDatabase\ExtractBinding --allow-partial --partial-report logs\extract-binding-partial.tsv
```

For Transform (T) and Load (L), transform views are usually kept in the same database as the layer they populate or expose:

- RDV transforms in the RDV database.
- BDV transforms in the BDV database.
- DW/mart transforms in the DW/mart database.

For a DW view that lives in the DW database but reads source tables through three-part names, `--execute-system` should be the DW execution database, while the three-part source identifiers still match the source schema `System.Name`:

```cmd
meta-transform-binding bind --transform-workspace dw\MartDatabase\Transforms --source-schema source\SourceDatabase\Schema --target-schema dw\MartDatabase\Schema --execute-system MartDatabase --new-workspace dw\MartDatabase\Binding --allow-partial --partial-report logs\binding-partial.tsv
```

Three-part source references remain source-qualified:

```sql
CREATE VIEW mart.vFactSales
AS
SELECT ...
FROM [SourceDatabase].[Sales].[SalesOrderHeader] AS h
JOIN [SourceDatabase].[Sales].[SalesOrderDetail] AS d
    ON d.SalesOrderID = h.SalesOrderID;
```

One-part or two-part references are interpreted relative to `--execute-system`, so only use them when the referenced objects are genuinely in the execution database/context. Avoid mixing a logical `--system` name with SQL that uses the physical database name. That makes binding look like missing columns or missing tables even though the source schema extraction was good.

For a smaller source slice:

```cmd
meta-schema extract sqlserver --new-workspace source\SourceSystem\Schema --connection-env SOURCE_SQL --system SourceSystem --schema dbo --all-tables
```

After extraction, inspect the workspace:

- which schemas and tables exist
- which tables answer the business questions
- which keys and relationships exist
- which date, product, customer, geography, employee, account, or measure-like fields exist
- which source objects are not useful for the requested slice

Record the selected source objects in `summary.txt`.

## RDV and BDV decisions

For a full-stack demo, do not decide whether RDV and BDV are worth modeling. They are part of the required stack shape.

Use the phase gates this way:

- RDV gate: source schema has been extracted, a Raw Data Vault model exists, SQL realization/deployment is planned or blocked with evidence, and the layer preserves source-grain audit evidence.
- BDV gate: business vault concepts exist, business keys/relationships are modeled where supported, and the layer integrates RDV evidence without destroying the raw trail.
- DW/Mart gate: dimensions/facts are shaped from BDV-facing evidence, not by skipping straight from source tables to presentation views.

A common RDV path is:

```cmd
meta-convert schema-to-raw-datavault --source-workspace source\SourceDatabase\Schema --new-workspace rdv\RawVaultDatabase\RawVault
meta-convert raw-datavault-to-sql --workspace rdv\RawVaultDatabase\RawVault --implementation-workspace <MetaDataVaultImplementation> --database-name <RawVaultDatabase> --out rdv\RawVaultDatabase\Sql
```

Use `meta-datavault-business` commands for BDV where the available CLI surface supports the concepts needed by the business slice. Discover exact commands with:

```cmd
meta-datavault-business help
```

If RDV or BDV cannot be meaningfully modeled with the available CLI surface, record the exact missing command or model gap in `SNAG-LOG.md` and mark the run partial. You may continue to produce a nearest truthful DW/mart slice for investigation, but the summary must say it is not the accepted full ETL stack.

## DW and mart modeling

A DW or mart layer is the analytical delivery surface. Model dimensions, facts, business keys, relationships, and measures around the business questions.

Typical command shapes:

```cmd
meta-data-warehouse --new-workspace dw\MartDatabase\Warehouse
meta-data-warehouse add-warehouse --workspace dw\MartDatabase\Warehouse --id Sales --name Sales
meta-data-warehouse add-dimension --workspace dw\MartDatabase\Warehouse --id Date --warehouse Sales --name Date
meta-data-warehouse add-dimension-attribute --workspace dw\MartDatabase\Warehouse --id DateKey --dimension Date --name DateKey --data-type-id meta:type:Int32
meta-data-warehouse add-dimension-business-key --workspace dw\MartDatabase\Warehouse --id DateBusinessKey --dimension Date --name DateBusinessKey
meta-data-warehouse add-dimension-business-key-part --workspace dw\MartDatabase\Warehouse --id DateBusinessKeyPart --business-key DateBusinessKey --attribute DateKey
meta-data-warehouse add-fact --workspace dw\MartDatabase\Warehouse --id SalesOrderLine --warehouse Sales --name SalesOrderLine
meta-data-warehouse add-fact-measure --workspace dw\MartDatabase\Warehouse --id SalesAmount --fact SalesOrderLine --name SalesAmount --data-type-id meta:type:Decimal
meta-data-warehouse add-fact-dimension --workspace dw\MartDatabase\Warehouse --id SalesDate --fact SalesOrderLine --dimension Date --role-name OrderDate
```

Convert DW model structure to SQL where supported:

```cmd
meta-convert data-warehouse-to-sql --workspace dw\MartDatabase\Warehouse --implementation-workspace <MetaDataWarehouseImplementation> --database-name <MartDatabase> --out dw\MartDatabase\Sql
```

If a DW conversion is not enough to produce all target transform views/tables, author transform SQL for the missing delivery objects and import it into `MetaTransformScript`.

## Transform SQL and MetaTransformScript

Transforms should be explicit SQL files or modeled `MetaTransformScript` data. They connect source, vault, warehouse, mart, DQ, and analytical processing surfaces.

Keep the transform layer honest:

- Use supported SQL surfaces, especially bounded `CREATE VIEW ... AS SELECT ...` shapes, when you need transform binding, DQ, pipeline, or orchestration evidence.
- Do not hide business logic in dynamic SQL or opaque stored procedures and then expect automatic DQ or orchestration to know what happened.
- If a transform selects from a physical database view, include matching `MetaTransformScript` for that view before depending on it for orchestration. Until the model can prove a DB view corresponds to transform-script truth, treat the DB view as opaque.
- Keep each table load transform identifiable. The pipeline/orchestration layer should be able to see which modeled transform reads and writes which objects.

Use clear SQL object identifiers:

```sql
CREATE VIEW mart.vFactSales
AS
SELECT ...
```

Import one file:

```cmd
meta-transform-script from sql-file --path transforms\mart_vFactSales.sql --target mart.vFactSales --new-workspace dw\MartDatabase\Transforms
```

Import many files with a manifest:

```cmd
meta-transform-script from sql-files --manifest transform-manifest.tsv --new-workspace dw\MartDatabase\Transforms --report logs\transform-import-report.tsv --verbose
```

Typical manifest:

```text
Path
transforms/mart_vFactSales.sql
transforms/mart_vDimDate.sql
transforms/mart_vDimProduct.sql
```

Emit SQL back out when needed:

```cmd
meta-transform-script to sql-path --workspace dw\MartDatabase\Transforms --out emitted-sql
```

## Target contracts and binding

Binding needs source and target schema contracts. Source contracts usually come from `meta-schema extract sqlserver`. Target contracts can come from modeled SQL assets, deployed target structures, or an extracted target database after target objects have been created.

Before binding, create a data-type conversion workspace unless the task already supplies one:

```cmd
meta-data-type-conversion --new-workspace shared\DataTypeConversion
meta-data-type-conversion check --workspace shared\DataTypeConversion
```

Pass it to binding and later pipeline/orchestration execution. Use the execution database for `--execute-system`, not automatically the source database:

```cmd
meta-transform-binding bind --transform-workspace dw\MartDatabase\Transforms --source-schema bdv\BusinessVaultDatabase\Schema --target-schema dw\MartDatabase\Schema --execute-system MartDatabase --new-workspace dw\MartDatabase\Binding --data-type-conversion-workspace shared\DataTypeConversion --allow-partial --partial-report logs\binding-partial.tsv
```

If binding reports `SourceSchemaFieldMetaDataTypeNotSanctioned` or `TargetSchemaFieldMetaDataTypeNotSanctioned`, do not treat that as a SQL transform failure. It means the conversion policy does not cover a data type found in the source or target schema. Record the specific `MetaDataTypeId` values, for example SQL Server alias/user-defined types such as `sqlserver:type:Name` or flags, then either extend/select the conversion policy if the CLI supports it or continue with binding-free DQ and executable SQL deployment as the nearest truthful slice.

Bind transforms when both contracts are available:

Use repeated `--source-schema` inputs when transforms read from multiple source databases. Each extracted source workspace should have a `System.Name` matching the source qualifier used in the SQL.

Treat binding as a hard gate for transform-backed downstream work.

`--allow-partial` is for diagnosis and corpus discovery. It can produce useful evidence, but it is not a green light for the stack. If the intended downstream pipeline, orchestration, binding-aware DQ, or transform execution depends on those transform scripts, the relevant transforms must bind and validate successfully. A result such as `1/9 bound` means the run should stop at binding, record the partial report, and fix binding inputs or policy before continuing.

Only continue past partial binding when the later stages are deliberately independent of the failed bindings, for example an executable-only smoke check over already deployed SQL artifacts. In that case, state clearly that the operational slice is not transform-binding-certified.

Use binding evidence to find:

- missing source objects
- missing target objects
- unresolved columns
- rowset shape mismatches
- nullable/non-nullable mismatch risks
- transforms that cannot be safely executed as modeled

Read partial binding reports in order. Common diagnosis:

- `SourceSchemaTableNotFound`: the SQL source identifier does not match the extracted `MetaSchema` system/schema/table names, or the table was not extracted.
- `QualifiedColumnReferenceNotFound` with each table exposing only one or very few columns: first suspect source identifier mismatch; verify the SQL leading source part and source `System.Name`. Then verify that `--execute-system` is the actual execution database/context for one-part and two-part references.
- `--execute-system` set to the source database for a DW/RDV/BDV view: usually wrong unless the transform truly executes in the source database. A view stored in the DW database should bind with the DW database as execution system and use three-part references for external source databases.
- `SourceSchemaFieldMetaDataTypeNotSanctioned`: identifier matching worked, but data-type conversion policy coverage is incomplete.
- target rowset/column count or nullability failures: target contracts and final projected columns disagree; fix the target model, transform projection, ignored platform columns, or documented load convention.

If target contracts are not available yet, record that binding is blocked and continue with the next truthful artifact, usually SQL realization or target deployment preparation.

## Data quality

Generate DQ candidates automatically from transform structure. Prefer binding-aware DQ when binding exists; use binding-free DQ only when binding is unavailable and the command supports it.

DQ is not looking for hand-written test scripts. It reads modeled transform joins, predicates, projections, and binding evidence, then proposes operational checks such as missing-reference/orphan anti-joins, uniqueness and duplicate risks, join fanout risks, incomplete joins, suspicious extra predicates, and optional/mandatory relationship mismatches where the model evidence supports them.

```cmd
meta-data-quality from-transform-workspace --transform-workspace dw\MartDatabase\Transforms --binding-workspace dw\MartDatabase\Binding --new-workspace dw\MartDatabase\Quality
```

or:

```cmd
meta-data-quality from-transform-workspace --transform-workspace dw\MartDatabase\Transforms --new-workspace dw\MartDatabase\Quality
```

Inspect and promote candidates:

```cmd
meta-data-quality inspect --workspace dw\MartDatabase\Quality
meta-data-quality promote --workspace dw\MartDatabase\Quality --all
```

Convert promoted candidates to SQL:

```cmd
meta-convert data-quality-to-sql --workspace dw\MartDatabase\Quality --out dw\MartDatabase\quality-sql
```

If no candidates exist, do not treat that as success by itself. Record why: no joins, no promoted candidates, binding skipped too much, or command returned no useful findings.

## SQL realization and deployment

Use `MetaSql` for deployable SQL assets. Do not hide deployment in an opaque script when a modeled SQL workspace can be produced.

Create deploy plan:

```cmd
meta-sql deploy-plan --source-workspace dw\MartDatabase\Sql --connection-env TARGET_SQL --out dw\MartDatabase\DeployManifest
```

Apply deployment:

```cmd
meta-sql deploy --manifest-workspace dw\MartDatabase\DeployManifest --source-workspace dw\MartDatabase\Sql --connection-env TARGET_SQL
```

If the current CLI cannot convert a required model to `MetaSql`, record the missing conversion and keep the authored SQL files as evidence. Do not call raw SQL deployment "MetaSql deployment" unless `MetaSql` is actually used.

## Analytics and target analytical models

Use `MetaAnalytics` to model portable analytical intent when possible. Then convert to the target engine.

Typical analytical model shape:

```cmd
meta-analytics --new-workspace analytics\Analytics
meta-analytics add-model --workspace analytics\Analytics --id Sales --name Sales --default-culture en-US
meta-analytics add-data-source --workspace analytics\Analytics --id Warehouse --model Sales --name Warehouse --provider SqlServer --connection-reference TARGET_SQL
meta-analytics add-table --workspace analytics\Analytics --id SalesOrderLine --model Sales --name SalesOrderLine --kind Fact
meta-analytics add-attribute --workspace analytics\Analytics --id SalesAmountColumn --table SalesOrderLine --name SalesAmount --data-type-id meta:type:Decimal
meta-analytics add-measure --workspace analytics\Analytics --id SalesAmount --table SalesOrderLine --source-attribute SalesAmountColumn --name "Sales Amount" --data-type-id meta:type:Decimal
meta-analytics add-aggregation-behavior --workspace analytics\Analytics --id SalesAmountAggregation --measure SalesAmount --function Sum
```

Convert to tabular:

```cmd
meta-convert analytics-to-tabular --workspace analytics\Analytics --out analytics\Tabular
```

Deploy/process tabular:

```cmd
meta-tabular deploy --workspace analytics\Tabular --server <server> --database-name <database> --drop-existing --no-process
meta-tabular process --server <server> --database-name <database>
```

Use the server value supplied by the task or environment exactly. Do not assume plain `localhost` for Analysis Services. Local analytical servers are often named instances such as `.\TABULAR` or `localhost\TABULAR`.

Convert to multidimensional when that target is requested:

```cmd
meta-convert analytics-to-multi-dimensional --workspace analytics\Analytics --out analytics\MultiDimensional
meta-multi-dimensional deploy --workspace analytics\MultiDimensional --server <server> --database-name <database> --drop-existing
```

If no analytical server is available, still produce the analytical workspace and target model workspace if supported, then record deployment as blocked by environment.

## Pipelines

Use `MetaPipeline` for modeled table-load units of work first. For a full BI stack demo, the accepted operational path is one transform-backed pipeline per table-producing transform unless there is a specific modeled reason that multiple transforms are one atomic table load.

Do not collapse the RDV, BDV, and DW/mart table loads into one generic executable wrapper. Executable pipeline steps can still be useful for auxiliary deployment, processing, smoke checks, or helper scripts after the modeled load DAG exists, but they are not a substitute for transform-backed ETL truth.

Create a pipeline workspace and a pipeline for the table load:

```cmd
meta-pipeline --new-workspace ops\Pipeline
meta-pipeline add-pipeline --workspace ops\Pipeline --name LoadFactSales
```

Add a transform-backed step when a transform and binding are available:

```cmd
meta-pipeline add-step --workspace ops\Pipeline --pipeline LoadFactSales --script mart.vFactSales --transform-workspace dw\MartDatabase\Transforms --binding-workspace dw\MartDatabase\Binding --execution-connection-env TARGET_SQL --step-name LoadFactSales --target mart.FactSales
```

Add auxiliary executable steps only for commands such as deploy, analytical process, smoke checks, or helper scripts:

```cmd
meta-pipeline add-pipeline --workspace ops\Pipeline --name ProcessAnalytics
meta-pipeline add-executable-step --workspace ops\Pipeline --pipeline ProcessAnalytics --step-name ProcessTabular --executable cmd.exe --arguments "/c stages\\12-deploy-process-tabular.cmd" --working-directory .
```

Inspect or execute:

```cmd
meta-pipeline inspect --workspace ops\Pipeline
meta-pipeline execute --workspace ops\Pipeline --pipeline LoadFactSales --transform-workspace dw\MartDatabase\Transforms --binding-workspace dw\MartDatabase\Binding
```

If a pipeline is executable-only, transform and binding workspaces may not be required by the current CLI. For this demo, executable-only pipelines are acceptable only as clearly named auxiliary steps after the transform-backed table-load pipelines exist.

## Orchestration

Use `MetaOrchestration` to infer and coordinate pipeline execution where the modeled evidence is sufficient. It should not replace pipeline steps; it derives safe dependencies/run plans from pipeline transform/binding access profiles, then records explicit policy issues where write order or synchronization cannot be proven.

Correct table-load ordering is expected to be automatic for the supported modeled path. Orchestration should inspect transform-backed pipeline tasks, build the dependency DAG from modeled read/write evidence, and execute that run plan. Do not hand-author dependency order, rely on manifest order, or use a single executable script as the primary ordering mechanism.

Discover the exact local creation command:

```cmd
meta-orchestration help
meta-orchestration --help
```

Typical execution shape:

```cmd
meta-orchestration execute --workspace ops\Orchestration --pipeline-workspace ops\Pipeline --run-artifacts-root run-artifacts
```

If the orchestration workspace cannot be created from the available CLI surface, record that as a product gap and leave the transform-backed pipelines as the nearest truthful operational slice. Do not downgrade the accepted full-stack claim to an executable-only orchestration shortcut.

## Command discovery

Prefer current local help over remembered syntax:

```cmd
where meta-schema
where meta-convert
meta-schema help
meta-convert help
meta-data-warehouse help
meta-analytics help
meta-tabular help
meta-pipeline help
meta-orchestration help
```

Use `docs/commands.md` and README files as supporting references, but do not rely on stale examples when command help disagrees.

## Evidence and summaries

Leave a concise run summary:

- inputs used
- stack path chosen, including RDV and BDV gate status
- analytical slice chosen
- source objects used
- transforms authored or imported
- workspaces created
- commands generated or executed
- deployment targets touched
- checks passed
- gaps or snags
- next manual action, if any

For failures, preserve the real failing command, exit code, and relevant output. Do not overwrite evidence with a polished story.

When the requested BI stack is large, state the slice boundary explicitly. For example, "this run covers sales order lines, product, customer/store, salesperson, territory, and date; it does not yet cover purchasing or inventory." That makes partial progress inspectable instead of vague.

## What not to do

Do not import external generated project artifacts as product truth unless the task explicitly asks for that surface and the repo supports it.

Do not hand-author source schema metadata when a live source can be extracted.

Do not hide important model/conversion/deployment work inside opaque scripts.

Do not invent successful results for unsupported slices. Record the product gap and continue with the nearest truthful path.
