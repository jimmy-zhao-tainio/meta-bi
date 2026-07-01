# Agent task: AdventureWorks modeled BI stack

You are working in `meta-bi`.

You are being recorded. You have a restored AdventureWorks OLTP SQL Server database, the connection environment variables listed below, and the business brief in `BUSINESS-REQUIREMENTS.md`.

Read `agent-meta.md`, then build a BI stack from that live database and the business requirements using `meta` / `meta-bi` CLI commands.

The point of the recording is to see what you can create from:

- a source database connection
- a plain-language analytics requirements document
- the available `meta` / `meta-bi` command-line tools

## Hard rules

- Do not import `.dwproj`, `.bim`, XMLA, TMSL, or Visual Studio generated project artifacts as product truth.
- Use the restored AdventureWorks OLTP SQL Server database as input.
- The first artifact-producing product command must extract source schema from that SQL database with `meta-schema extract sqlserver`.
- You may inspect CLI help, repo docs, and source readiness before that product command.
- Do not start from `.bak`, `.sql`, sample project files, tutorial artifacts, or manually typed source schemas. The backup is only a way to restore the SQL database before the run.
- Do not assume a human has already extracted source schema metadata; create `source\AdventureWorks2022\Schema` or the equivalent source-database-scoped schema workspace as part of the generated run.
- Use `meta` / `meta-bi` CLI commands to create sanctioned workspaces and generated assets.
- Put generated output under `%AW_RUN_ROOT%` in a clear timestamped child folder. If the caller supplies a run folder, use that folder.
- Write `PLAN.md` before artifact-producing product commands. Do not one-shot the whole demo; run it in reviewed phases with evidence gates.
- Do not skip RDV or BDV for this recording. A source-to-DW/mart shortcut is not an accepted full ETL stack. If current tooling blocks RDV or BDV, record the blocker and mark the run partial.
- Include the analytics target in the accepted run. For this recording the target is Tabular unless the environment has no reachable Tabular server; if Tabular deployment or processing is blocked by environment, record that exact blocker and mark the analytics proof partial.
- Name folders by layer, database, and role. Do not create root-level generic folders such as `SourceSchemaWS`, `TransformWS`, `BindingWS`, or `CurrentMetaSqlWS`, and do not append `WS` to every folder. Use names like `source\AdventureWorks2022\Schema`, `rdv\<database>\RawVault`, `bdv\<database>\BusinessVault`, `dw\<database>\Transforms`, `dw\<database>\Binding`, `analytics\Tabular`, and `ops\Orchestration`.
- Write plain `.cmd` files for setup, generation, deployment, DQ, pipeline setup, orchestration setup, and orchestration execution.
- The `.cmd` files must print the command being executed and show normal command output.
- Use environment variables for all connection strings and server names.
- Model the table-load operations as transform-backed `MetaPipeline` work. Use one pipeline per table-producing RDV, BDV, and DW/mart transform unless a specific modeled reason makes a group atomic. Executable pipeline steps may be auxiliary, but they must not replace the transform-backed ETL DAG.
- Let `MetaOrchestration` infer the table-load DAG from modeled pipeline transform/binding evidence. Do not use manifest order, manual dependency rows, or a single executable wrapper as the primary orchestration proof.
- If product models, CLI behavior, environment, or worker execution blocks the run, halt at that phase. The blocker can be fixed outside the accepted run, but the final evidence should come from a fresh clean rerun rather than from a folder full of retries.
- In a failed diagnostic run, record the blocker in `SNAG-LOG.md`. In the accepted clean run, keep `SNAG-LOG.md` focused on blockers that remain relevant to that run.

## Expected run outputs

Create a run folder containing, as the product supports them:

- `PLAN.md`
- source database schema workspace, for example `source\AdventureWorks2022\Schema`
- RDV workspace and realization assets, for example `rdv\<raw-vault-db>\RawVault`, `rdv\<raw-vault-db>\Sql`, and `rdv\<raw-vault-db>\DeployManifest`
- BDV workspace and realization assets, for example `bdv\<business-vault-db>\BusinessVault`, `bdv\<business-vault-db>\Sql`, and `bdv\<business-vault-db>\DeployManifest`
- transform SQL files
- layer-scoped transform workspaces, for example `rdv\<db>\Transforms`, `bdv\<db>\Transforms`, and `dw\<db>\Transforms`
- layer-scoped binding workspaces, for example `dw\<db>\Binding`
- `dw\<db>\Quality`
- derived DQ candidates from the transform model
- generated DQ SQL from promoted candidates
- layer-scoped `MetaSql` workspaces and deploy manifests, for example `dw\<db>\Sql` and `dw\<db>\DeployManifest`
- data warehouse / mart model workspace, for example `dw\<db>\Warehouse`
- `analytics\Analytics`
- `analytics\Tabular`
- `ops\Pipeline`
- `ops\Orchestration`
- `run-artifacts`
- generated command scripts
- a short `summary.txt`

## Recording questions to answer

As you work, make the generated scripts and `summary.txt` answer these questions:

- Did you extract schemas from AdventureWorks?
- Did you create a plan first and execute the run in staged phases?
- Did you model RDV and BDV before the DW/mart, or record the exact product blocker that made the full ETL stack partial?
- Did you model a warehouse, mart, or equivalent analytical structure where the available tooling supports it?
- Did you write SQL transform scripts?
- Did you create target SQL deployment assets?
- Did you create analytics and tabular assets?
- Did you derive DQ candidates from modeled transform structure and binding evidence, including anti-join style missing-reference/orphan checks where joins imply them?
- Did you generate executable DQ SQL from promoted candidates?
- Did you set up one transform-backed pipeline per table-producing RDV, BDV, and DW/mart transform where safe?
- Did orchestration infer/run a dependency DAG from pipeline transform/binding evidence?
- Did the run stay inside the supported modeled surface enough to show that DQ and orchestration are automatic for the normal BI path?
- Did the Tabular database deploy and process after the modeled table-load orchestration?
- If Tabular processing succeeds, what Excel-visible or DAX-queryable measure should be used as the final smoke check?

## Expected command scripts

Generate scripts with simple names and visible commands.

Create one top-level `run.cmd` in the run folder. It should call the stage scripts in order and stop on failure. Stage scripts are still encouraged because they make the CLI flow readable, for example:

- `generated-setup-source-check.cmd`
- `generated-extract-source-schema.cmd`
- `generated-author-rdv.cmd`
- `generated-author-bdv.cmd`
- `generated-author-dw-mart.cmd`
- `generated-bind-layer-transforms.cmd`
- `generated-dq.cmd`
- `generated-deploy-layer-sql.cmd`
- `generated-author-pipeline.cmd`
- `generated-author-orchestration.cmd`
- `generated-execute-orchestration.cmd`
- `generated-author-analytics.cmd`
- `generated-deploy-tabular.cmd`
- `generated-process-tabular.cmd`
- `generated-tabular-proof.cmd`
- `generated-open-excel-check-notes.txt`

Use the actual CLI surface available in this repo. Do not invent commands. If you need to execute CLI commands while discovering or authoring the stack, keep doing that; the final `run.cmd` is the replayable command for the live video run.

## Environment variables

Use these variables when possible:

- `AW_SQL_SERVER`
- `AW_SOURCE_DATABASE`
- `AW_SOURCE_SQL`
- `AW_RDV_DATABASE`
- `AW_RDV_SQL`
- `AW_BDV_DATABASE`
- `AW_BDV_SQL`
- `AW_DW_DATABASE`
- `AW_DW_SQL`
- `AW_TABULAR_SERVER`
- `AW_TABULAR_DATABASE`
- `AW_RUN_ROOT`

Use the layer-specific SQL connection variables for deployment and schema extraction:

- RDV SQL work targets `AW_RDV_SQL`.
- BDV SQL work targets `AW_BDV_SQL`.
- DW/mart SQL work targets `AW_DW_SQL`.

Do not deploy RDV, BDV, and DW/mart into one shared default target database unless the model explicitly calls for a single database and the deploy-plan proves it is clean. A full demo normally uses separate layer databases so full-database deploy planning does not collide with objects owned by another layer.

## First useful slice

If the full stack is too large for one pass, do the smallest honest vertical slice through the required layers. A narrow sales slice is fine; skipping RDV and BDV is not.

1. Plan first in `PLAN.md`, then source schema extraction from the restored SQL database, for example:

   ```cmd
   meta-schema extract sqlserver --new-workspace source\AdventureWorks2022\Schema --connection-env AW_SOURCE_SQL --system AdventureWorks2022 --all-schemas --all-tables
   ```

2. RDV modeling/realization for the chosen source slice.
3. BDV modeling/realization for the chosen business slice.
4. DW/mart modeling and transforms for online/store sales order lines, product, date, geography, customer/store, salesperson, and territory.
5. Strict binding and DQ generation from transform semantics.
6. SQL deployment for the layer targets.
7. Transform-backed pipeline setup: one pipeline per table-producing transform across RDV, BDV, and DW/mart.
8. Orchestration inference/run-plan generation and execution from those modeled pipeline/binding profiles.
9. Analytics and Tabular assets from the mart.
10. Tabular deploy/process and a row-count or measure proof through Excel or a DAX-capable probe.

Use AdventureWorks OLTP source objects where they fit the business request, such as `Sales.SalesOrderHeader`, `Sales.SalesOrderDetail`, `Sales.Customer`, `Sales.Store`, `Sales.SalesPerson`, `Sales.SalesTerritory`, `Sales.SalesPersonQuotaHistory`, `Production.Product`, `Production.ProductSubcategory`, `Production.ProductCategory`, `Person.Person`, and geography-related `Person` tables.

The preferred full ending is: orchestration runs the generated transform-backed table-load pipeline stack, the tabular database is deployed and processed from the mart, then the operator connects from Excel or runs a DAX-capable proof and displays at least one required measure such as sales amount by calendar month or product category.

Stop only for a real blocker. If blocked, leave the diagnostic run folder with the scripts and a clear snag entry, fix the root cause, then create a clean rerun for the accepted demo evidence.
