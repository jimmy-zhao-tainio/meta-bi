# MetaPipeline Working Plan

This is the near-term steering note for growing `MetaPipeline` without turning the first SQL Server bulk insert into the whole design.

The broader task-family sketch lives in [`META-PIPELINE-MODEL.md`](META-PIPELINE-MODEL.md).

## Boundary

`MetaPipeline` executes declared pipeline work.

An orchestrator may later decide which pipeline to execute, when to execute it, and for which target.
That intelligence sits above `MetaPipeline`.

The execution layer should stay plain: validate declared work, read a transform row stream, buffer it, write it, and report what happened.

## Current executable slice

The implemented stage 1 slice is:

- one explicit `TransformScript` per transform-backed task
- one matching `TransformBinding` per transform-backed task
- one selected target when a SELECT binding exposes more than one target
- initial `MetaPipeline` XML workspace and instance CLI commands
- a serial `PipelineTask` chain with one bound transform script per transform-backed task
- SQL Server transform execution through an explicit execution connection
- explicit row-stream shape shared by source, buffers, and writer
- bounded in-memory row buffers
- explicit target write operation seam
- logical insert-rows target write as the modeled target operation
- SQL Server bulk copy as the current physical realization
- pre-execute modeled validation and CLI guardrails for supported task/detail shape
- execute-time row-stream shape drift validation between modeled pipeline row stream and resolved binding output shape
- in-memory execution result with row count, batch count, status, failure stage, and failure message
- optional SQL Server operational DB recording for separated diagnostic logs, audit-relevant run logs, metrics, task runs, and failures through an explicitly initialized operational DB
- explicit pruning for old `RunDiagnosticsLog` rows only; run lineage, audit ids, metrics, fingerprints, audit logs, and failures stay audit-preserved

The CLI surface is:

```text
meta-pipeline create-pipeline-db --pipeline-db-connection-env <name> [--pipeline-db-name <name>]
meta-pipeline prune-pipeline-db --pipeline-db-connection-env <name> --retention-days <days> [--dry-run]
meta-pipeline execute --workspace <path> --pipeline <name> --transform-workspace <path> --binding-workspace <path> [--pipeline-db-connection-env <name>]
meta-pipeline execute-sqlserver --transform-workspace <path> --binding-workspace <path> --script <name-or-id> [--binding <id>] --execution-connection-env <name> [--target-connection-env <name>] [--target <sql-identifier>] [--batch-size <n>] [--timeout-seconds <n>] [--pipeline-db-connection-env <name>]
```

`execute` is the preferred shape for modeled work.
`execute-sqlserver` remains the direct low-level slice while the modeled path is being filled in.
Target connection options are required only for SELECT-kind scripts that materialize through `InsertRows`; mutation scripts execute through the explicit execution connection.
Timeouts are explicit: omitted means no SQL command timeout, and configured timeouts apply to SQL commands and SQL Server bulk copy.

## Growth axes

Keep these axes separate so one dimension does not accidentally own the others:

- Source transform: execute the selected `TransformScript`; source-side delta logic can live inside the transform script when the user models it there.
- Binding guarantee: confirm the transform result shape matches the selected target shape before writing.
- Row stream: keep shape, read, buffer, and write mechanics reusable across task kinds.
- Target write strategy: modeled target movement is explicit. SQL Server realization currently covers insert rows for SELECT-kind output materialization.
- SQL-shaped mutation boundary: mutation statements such as `MERGE`, `UPDATE`, `DELETE`, `TRUNCATE`, and `INSERT` belong in `MetaTransformScript`, not in pipeline-owned mini-DML task kinds.
- Task chain: a pipeline is an ordered list of plain declared tasks. Each transform task executes one bound `TransformScript`; SELECT-kind transform execution and target write stay adjacent because the row stream is in-memory.
- Runtime result: report completed rows/batches and where a failure occurred.
- Failure policy: `StopOnFirstFailure` for the serial chain; later tasks are recorded as skipped.
- Transaction boundary: no pipeline-owned begin/end transaction tasks; SQL-shaped transaction atomicity belongs inside transform scripts or generated SQL patterns.
- Evidence: runtime result is always returned in-memory; optional SQL Server operational DB persistence records runtime evidence without writing run telemetry to XML.

## Near-term moves

- Keep hardening the row stream and target write operation seam as reusable execution primitives.
- Add new task families only when ownership is clear and the behavior is not SQL statement syntax that belongs in `MetaTransformScript`.
- Keep the sanctioned `MetaPipeline` XML model narrow and executable before adding richer task families.
- Keep the operation database limited to evidence. Resumability, scheduling, and intelligent orchestration remain out of the core slice for now.

## Anti-goals

- Do not put orchestrator decision-making inside `MetaPipeline`.
- Do not require generic ordering or watermark semantics for arbitrary SQL transforms.
- Do not broaden stage 1 source support beyond database sources supported by the current `MetaSchema` path.
- Do not store connection strings in sanctioned metadata or artifacts.
- Do not encode SCD, delta, or Data Vault taxonomies before there is a concrete modeled task that needs them.
