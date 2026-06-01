# MetaOrchestration Hairball CLI Demo

This standalone demo generates a deterministic orchestration stress scenario and checks the analyzer against predictions computed before orchestration runs.

The run scripts are intentionally plain:

1. `run-setup.cmd` generates SQL files, a schema workspace, an oracle, and generated setup/execute command scripts
2. `run-setup.cmd` calls `generated-setup.cmd`, which runs the real setup `meta-*` CLI commands with command text and normal output visible
3. `run-setup.cmd` verifies the generated MetaOrchestration workspace against the oracle
4. `run-execute.cmd` calls `generated-execute.cmd`, which runs `meta-orchestration execute` against the generated workspaces

It creates:

- source, staging, core, data warehouse, and mart object names
- SELECT transforms
- stored procedure refresh transforms with ordered reset/append operations
- stored procedure result-rowset transforms feeding InsertRows target writes
- a mutation task
- a serial multi-task pipeline
- expected pipeline and object-level data dependency files
- `generated-setup.cmd` containing the actual `meta-transform-script`, `meta-transform-binding`, `meta-pipeline`, and setup `meta-orchestration` calls
- `generated-execute.cmd` containing the actual orchestration execution command
- real TransformScript, Binding, Pipeline, and Orchestration workspaces created by those commands

Run setup from this folder:

```cmd
run-setup.cmd
```

Then run orchestration execution from this folder:

```cmd
run-execute.cmd
```

The run writes output under `Runs\hairball-seed-20260530`, including:

- `SourceSql`
- `SchemaWS`
- `generated-setup.cmd`
- `generated-execute.cmd`
- `TransformWS`
- `BindingWS`
- `PipelineWS`
- `OrchestrationWS`
- `expected-pipeline-edges.tsv`
- `actual-pipeline-edges.tsv`
- `expected-data-edges.tsv`
- `actual-data-edges.tsv`
- `summary.txt`

The demo exits non-zero if the analyzer output does not match the generated oracle.
The execution scripts default `HAIRBALL_EXECUTION_SQL` and `HAIRBALL_TARGET_SQL` to:

```text
Server=localhost;Database=MetaOrchestrationHairball;Trusted_Connection=True;TrustServerCertificate=True;
```

Set those variables before running the script to use a different SQL Server database.

Clean generated runs:

```cmd
cleanup.cmd
```
