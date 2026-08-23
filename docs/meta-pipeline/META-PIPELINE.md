# MetaPipeline

## Purpose

`MetaPipeline` is the core data-moving model and CLI for `meta-bi`.

The model is `MetaPipeline`.
The CLI is `meta-pipeline`.

Stage 1 is not trying to define the full orchestration story.
It is trying to close the small kernel that must be true for one sanctioned pipeline unit to run honestly.
Internal preparation or compilation may exist, but visible UX should not be centered on `plan` / `run` verbs or on artifact generation.
The optional operational DB stores runtime evidence for this kernel; it is not model truth or orchestration state.

## Pipeline unit

The stage 1 pipeline unit is centered on a serial chain of executable tasks.
Each transform-backed task is centered on:

- one `TransformScript` instance
- one `TransformBinding` instance
- the `MetaSchema` workspace context that the binding depends on
- the runtime machinery needed to read, buffer, and write that combination honestly

The transform is the core.
The binding is the guarantee.

Executable process tasks are also modeled pipeline tasks.
They carry the executable path, arguments, working directory, expected success exit code, and optional timeout in the workspace.
They do not use transform or binding workspaces.
At runtime, success/failure is based on the real process exit code.

Every transform execution task names the transform script and transform binding explicitly by id.
If a SELECT-kind binding exposes multiple targets, the task also names the target explicitly.
In the modeled CLI path, `meta-pipeline execute` runs the pipeline's declared serial `PipelineTask` chain.

A pipeline may contain multiple local tasks, such as preparation, mutation statements, SELECT materialization, and cleanup.
Those tasks are not orchestration intelligence.
External orchestration decides which pipeline to run; the pipeline executes its declared local serial unit.

This is a narrower center than generic ETL or orchestration language.
Stage 1 is about making this unit real before widening the support claim.

## Transform vs Effects

`MetaTransformScript` and `MetaPipeline` own different kinds of truth:

- `MetaTransformScript` owns SQL statement semantics. SQL-shaped mutations such as `MERGE`, `UPDATE`, `DELETE`, `TRUNCATE`, and `INSERT` belong there as modeled SQL.
- `MetaPipeline` executes modeled transform scripts and handles buffered row movement when a SELECT-kind script is materialized.

Do not introduce pipeline-owned partial DML abstractions for SQL statement behavior.
If mutation logic is SQL-shaped, model it in `MetaTransformScript` or reject it clearly until that SQL surface is supported.

Sanctioned modeled target movement currently includes:

- `InsertRows` through pipeline row buffers

`InsertRows` records the runtime target data type system through
`InsertRowsTargetWriteTask.TargetDataTypeSystemName`.
If omitted, SQL Server execution defaults it to `SqlServer`.
Binding and execution can also be supplied a `MetaDataTypeConversion` workspace so source/target
type normalization remains a sanctioned policy workspace, not a pipeline-owned type table.

## Stage 1 nucleus

The early nucleus is small:

- pipeline unit shape
- source read realization
- target write realization
- bounded row buffering and movement
- failure honesty
- minimal validation

This is the irreducible base layer even if later orchestration or richer semantics impose additional constraints downward.

## Stage 1 source grounding

Stage 1 is grounded on database sources already supported by `MetaSchema`.

That grounding is deliberate.
Files, APIs, object stores, and messages are not the stage 1 source family for this document.

## Connection references

Connection strings are not stored in sanctioned metadata or artifacts.

Sanctioned metadata may name connection references explicitly.
At runtime those reference names are resolved through shell-visible environment variables.
Users and devops systems are responsible for populating those variables before invoking `meta-pipeline`.

## Operational DB

`meta-pipeline create-pipeline-db --pipeline-db-connection-env <name> [--pipeline-db-name <name>]` creates the SQL Server operational evidence database and schema.
The database name defaults to `MetaPipeline`.
Execution commands may opt into recording with `--pipeline-db-connection-env <name>`.
They require the operational DB to be initialized first and fail with a `Next:` helper if it is unavailable.
`meta-pipeline prune-pipeline-db --pipeline-db-connection-env <name> --retention-days <days>` performs explicit retention maintenance for old `RunDiagnosticsLog` rows while preserving run lineage.

When `execute` or `execute-sqlserver` runs in an attached console, the CLI prints one compact live operator progress line with step count, elapsed time, rows, batches, and estimated payload rate. Rate units switch between `B/s`, `KB/s`, `MB/s`, and `GB/s` as the observed rate grows. Redirected/headless executions do not print the live progress line.

Executable process tasks can be authored with `meta-pipeline add-executable-step`.
`execute`, `execute-step`, and `execute-worker` can run executable-only tasks without transform or binding workspace arguments.

The operational DB stores run diagnostic logs separately from audit-relevant run logs, metrics, task runs, and failures.
It does not store XML metadata, connection strings, scheduling policy, watermarks, checkpoints, or orchestration decisions.

## Relationship to other docs

- `META-PIPELINE.md` is the compact grounding note for the first `MetaPipeline` slice.
- [`META-PIPELINE-MODEL.md`](META-PIPELINE-MODEL.md) is the overarching model sketch for task families and boundaries.
- [`META-PIPELINE-DB.md`](META-PIPELINE-DB.md) is the operational evidence DB note.

If later stages widen into orchestration or richer semantics, they should build on this nucleus rather than replace it.
