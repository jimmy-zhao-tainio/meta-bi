# MetaOrchestration Hairball CLI Demo

This standalone demo generates a deterministic orchestration stress scenario and checks the analyzer against predictions computed before orchestration runs.

The run scripts are intentionally plain:

1. `run-setup.cmd` generates SQL files, schema and MetaSql workspaces, an oracle, and generated setup/execute command scripts
2. `run-setup.cmd` calls `generated-setup.cmd`, which runs the real setup `meta-*` CLI commands with command text and normal output visible
3. `run-setup.cmd` verifies the generated MetaOrchestration workspace against the oracle
4. `run-execute.cmd` calls `generated-execute.cmd`, which runs `meta-orchestration execute` against the generated workspaces

It creates:

- source, staging, core, data warehouse, and mart object names
- SELECT transforms
- stored procedure refresh transforms with ordered reset/append operations
- stored procedure result-rowset transforms feeding InsertRows target writes
- mutation tasks
- many single-task pipelines
- serial hub, bridge, regional mart, and composite pipelines
- expected pipeline and object-level data dependency files
- `CurrentMetaSqlWorkspace`, containing the generated database, schemas, tables, views, and stored procedures
- `generated-setup.cmd` containing the actual `meta-sql deploy-plan`, `meta-sql deploy`, `meta-transform-script`, `meta-transform-binding`, `meta-pipeline`, and setup `meta-orchestration` calls
- `generated-execute.cmd` containing the actual orchestration execution command
- `orchestration-run-plan-graph.txt`, captured from `meta-orchestration inspect-run-plan`
- `orchestration-execute-output.txt`, captured from `meta-orchestration execute`
- `RunArtifacts`, passed as `--run-artifacts-root` for orchestration journals, worker logs, and leases
- real TransformScript, Binding, Pipeline, and Orchestration workspaces created by those commands

The setup script defaults `HAIRBALL_EXECUTION_SQL` and `HAIRBALL_TARGET_SQL` to:

```text
Server=localhost;Database=MetaOrchestrationHairball;Trusted_Connection=True;TrustServerCertificate=True;
```

Set those variables before setup to use a different SQL Server database.

Run setup from this folder:

```cmd
run-setup.cmd
```

Setup deploys the generated `CurrentMetaSqlWorkspace` with:

```cmd
meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-env HAIRBALL_EXECUTION_SQL --out MetaSqlDeployManifest
meta-sql deploy --manifest-workspace MetaSqlDeployManifest --source-workspace CurrentMetaSqlWorkspace --connection-env HAIRBALL_EXECUTION_SQL
meta-sql deploy-plan --source-workspace CurrentMetaSqlWorkspace --connection-env HAIRBALL_EXECUTION_SQL --out MetaSqlVerifyManifest
```

Then run orchestration execution from this folder:

```cmd
run-execute.cmd
```

To review the live user-console progress renderer instead of captured output:

```cmd
cd Runs\hairball-seed-20260530
set HAIRBALL_EXECUTION_SQL=Server=localhost;Database=MetaOrchestrationHairball;Trusted_Connection=True;TrustServerCertificate=True;
set HAIRBALL_TARGET_SQL=Server=localhost;Database=MetaOrchestrationHairball;Trusted_Connection=True;TrustServerCertificate=True;
meta-orchestration.exe execute --workspace OrchestrationWS --pipeline-workspace PipelineWS --transform-workspace TransformWS --binding-workspace BindingWS --max-degree-of-parallelism 12 --run-artifacts-root RunArtifacts
```

The run writes output under `Runs\hairball-seed-20260530`, including:

- `SourceSql`
- `SchemaWS`
- `CurrentMetaSqlWorkspace`
- `MetaSqlDeployManifest`
- `MetaSqlVerifyManifest`
- `generated-setup.cmd`
- `generated-execute.cmd`
- `TransformWS`
- `BindingWS`
- `PipelineWS`
- `OrchestrationWS`
- `RunArtifacts`
- `orchestration-run-plan-graph.txt`
- `orchestration-execute-output.txt`
- `expected-pipeline-edges.tsv`
- `actual-pipeline-edges.tsv`
- `expected-data-edges.tsv`
- `actual-data-edges.tsv`
- `summary.txt`

The demo exits non-zero if the analyzer output does not match the generated oracle.

Clean generated runs:

```cmd
cleanup.cmd
```
