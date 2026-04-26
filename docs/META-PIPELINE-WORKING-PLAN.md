# MetaPipeline Working Plan

This is the near-term steering note for growing `MetaPipeline` without turning the first SQL Server bulk insert into the whole design.

The broader task-family sketch lives in `META-PIPELINE-MODEL.md`.

## Boundary

`MetaPipeline` executes declared pipeline work.

An orchestrator may later decide which pipeline to execute, when to execute it, and for which target.
That intelligence sits above `MetaPipeline`.

The execution layer should stay plain: validate declared work, read a transform row stream, buffer it, write it, and report what happened.

## Current executable slice

The implemented stage 1 slice is:

- one explicit `TransformScript`
- one matching `TransformBinding`
- one selected target when the binding exposes more than one
- initial `MetaPipeline` XML workspace and instance CLI commands
- SQL Server transform execution as a source row stream
- explicit row-stream shape shared by source, buffers, and writer
- bounded in-memory row buffers
- explicit target write operation seam
- logical insert-rows target write as the first modeled target operation
- SQL Server bulk copy as the current physical realization
- in-memory execution result with row count, batch count, status, failure stage, and failure message

The CLI surface is:

```text
meta-pipeline execute --workspace <path> --pipeline <name> --task <name> --transform-workspace <path> --binding-workspace <path>
meta-pipeline execute-sqlserver --transform-workspace <path> --binding-workspace <path> --transform-script-id <id> --transform-binding-id <id> --source-connection-env <name> --target-connection-env <name> [--target <sql-identifier>] [--batch-size <n>]
```

`execute` is the preferred shape for modeled work.
`execute-sqlserver` remains the direct low-level slice while the modeled path is being filled in.

## Growth axes

Keep these axes separate so one dimension does not accidentally own the others:

- Source transform: execute the selected `TransformScript`; source-side delta logic can live inside the transform script when the user models it there.
- Binding guarantee: confirm the transform result shape matches the selected target shape before writing.
- Row stream: keep shape, read, buffer, and write mechanics reusable across task kinds.
- Target write strategy: the current modeled operation is insert rows; SQL Server currently realizes it with bulk copy. Later operations may include truncate-plus-insert, merge/upsert, SCD handling, Data Vault loads, or other modeled write tasks.
- Task chain: a pipeline can become an ordered list of plain declared tasks, but the first concrete task remains transform execution followed by a target write.
- Runtime result: report completed rows/batches and where a failure occurred without introducing an operational database yet.
- Evidence: keep minimal, non-secret execution evidence now; richer replay, audit, and recovery are later features.

## Near-term moves

- Keep hardening the row stream and target write operation seam as reusable execution primitives.
- Add another target write operation only when a concrete demo or model need forces it.
- Add a simple pre/post task only when a real demo or model need forces it.
- Keep the sanctioned `MetaPipeline` XML model narrow and executable before adding richer task families.
- Keep operation database, resumability, scheduling, and intelligent orchestration out of the core slice for now.

## Anti-goals

- Do not put orchestrator decision-making inside `MetaPipeline`.
- Do not require generic ordering or watermark semantics for arbitrary SQL transforms.
- Do not broaden stage 1 source support beyond database sources supported by the current `MetaSchema` path.
- Do not store connection strings in sanctioned metadata or artifacts.
- Do not encode SCD, delta, or Data Vault taxonomies before there is a concrete modeled task that needs them.
