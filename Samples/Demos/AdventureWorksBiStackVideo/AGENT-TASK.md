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
- The first product command must extract source schema from that SQL database with `meta-schema extract sqlserver`.
- Do not start from `.bak`, `.sql`, sample project files, tutorial artifacts, or manually typed source schemas. The backup is only a way to restore the SQL database before the run.
- Do not assume a human has already extracted `SourceSchemaWS`; create it as part of the generated run.
- Use `meta` / `meta-bi` CLI commands to create sanctioned workspaces and generated assets.
- Put generated output under `Runs\adventureworks-bi-stack-<timestamp>`.
- Write plain `.cmd` files for setup, generation, deployment, DQ, pipeline setup, orchestration setup, and orchestration execution.
- The `.cmd` files must print the command being executed and show normal command output.
- Use environment variables for all connection strings and server names.
- Record product snags in `SNAG-LOG.md` before fixing or working around them.

## Expected run outputs

Create a run folder containing, as the product supports them:

- `SourceSchemaWS`
- transform SQL files
- `TransformWS`
- `BindingWS`
- `DataQualityWS`
- generated DQ SQL
- current target `MetaSql` workspace
- `MetaSqlDeployManifest`
- modeled BDV / warehouse / mart workspaces if the current CLI surface supports them
- data warehouse / mart model workspace if authored
- `MetaAnalyticsWS`
- `MetaTabularWS`
- `MetaPipelineWS`
- `MetaOrchestrationWS`
- `RunArtifacts`
- generated command scripts
- a short `summary.txt`

## Recording questions to answer

As you work, make the generated scripts and `summary.txt` answer these questions:

- Did you extract schemas from AdventureWorks?
- Did you model a BDV, warehouse, mart, or equivalent analytical structure where the available tooling supports it?
- Did you write SQL transform scripts?
- Did you create target SQL deployment assets?
- Did you create analytics and tabular assets?
- Did you create DQ checks from modeled structure?
- Did you set up pipelines and orchestration?
- Did orchestration process the tabular database at the end?
- If tabular processing succeeds, what Excel-visible measure should be used as the final smoke check?

## Expected command scripts

Generate scripts with simple names and visible commands.

Create one top-level `run.cmd` in the run folder. It should call the stage scripts in order and stop on failure. Stage scripts are still encouraged because they make the CLI flow readable, for example:

- `generated-setup-source-check.cmd`
- `generated-extract-source-schema.cmd`
- `generated-author-transforms.cmd`
- `generated-bind.cmd`
- `generated-dq.cmd`
- `generated-deploy-sql.cmd`
- `generated-author-analytics.cmd`
- `generated-deploy-tabular.cmd`
- `generated-author-pipeline.cmd`
- `generated-author-orchestration.cmd`
- `generated-execute-orchestration.cmd`
- `generated-open-excel-check-notes.txt`

Use the actual CLI surface available in this repo. Do not invent commands. If you need to execute CLI commands while discovering or authoring the stack, keep doing that; the final `run.cmd` is the replayable command for the live video run.

## Environment variables

Use these variables when possible:

- `AW_SQL_SERVER`
- `AW_SOURCE_DATABASE`
- `AW_SOURCE_SQL`
- `AW_TARGET_DATABASE`
- `AW_TARGET_SQL`
- `AW_TABULAR_SERVER`
- `AW_TABULAR_DATABASE`
- `AW_RUN_ROOT`

## First useful slice

If the full stack is too large for one pass, do the smallest honest vertical slice:

1. Source schema extraction from the restored SQL database, for example:

   ```cmd
   meta-schema extract sqlserver --new-workspace SourceSchemaWS --connection-env AW_SOURCE_SQL --system AdventureWorks --all-schemas --all-tables
   ```

2. A few transform scripts for internet sales, reseller sales, product, date, geography, and customer/reseller dimensions.
3. Binding and DQ generation.
4. SQL deployment for the target objects.
5. Pipeline and orchestration setup.
6. One executable pipeline step for a target command such as a tabular deploy/process or a smoke check.

The preferred full ending is: orchestration runs the generated pipeline stack, processes the tabular database, then the operator connects from Excel and displays at least one required measure such as sales amount by calendar month or product category.

Stop only for a real blocker. If blocked, leave the run folder with the scripts and a clear snag entry.
