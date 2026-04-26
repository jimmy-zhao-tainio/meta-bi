# MetaPipeline Model Sketch

This is the broader model sketch behind the initial sanctioned `MetaPipeline` XML model.

The goal is to name the broad shape early enough that the runtime work does not accidentally turn the first SQL Server bulk insert into the whole product.

## Identity

`MetaPipeline` is the executable data-moving model for `meta-bi`.

It should describe declared pipeline work.
It should not become the intelligent orchestrator.

The orchestrator may later decide which pipeline to run, when to run it, which target to run it for, which partitions or tenants to include, and whether approvals or dependency gates are satisfied.
`MetaPipeline` should execute the declared work honestly and report what happened.

## Core Shape

The model center is:

| Concept | Meaning |
| --- | --- |
| `Pipeline` | A named executable definition made of declared tasks. |
| `PipelineTask` | The ordered task backbone. Concrete task meaning comes from related task-detail entities. |
| `TaskDependency` | An explicit ordering or data dependency between tasks inside the pipeline. |
| `RowStream` | The main in-memory/batched data carrier between read/transform and write tasks. |
| `RowStreamShape` | The column contract for a row stream. |
| `ConnectionReference` | A shell environment variable name that resolves to a connection string at runtime. |

The first implemented slice already proves part of this:

| Current runtime piece | Model concept |
| --- | --- |
| `TransformScript` + `TransformBinding` selection | Transform execution task nucleus |
| `PipelineRowStreamShape` | `RowStreamShape` |
| `PipelineDataBatch` | Bounded row-stream buffer |
| `IPipelineTargetWriteOperation` | Target write operation |
| `InsertRowsTargetWriteTask` | Logical insert-rows target write task. |
| `SqlServerBulkInsertTargetWriteOperation` | Current SQL Server physical realization of insert rows. |
| `MetaPipelineExecutionResult` | Minimal `PipelineRun` evidence |

## Pipeline

A `Pipeline` should be a plain executable definition:

| Field | Intent |
| --- | --- |
| `Name` | Stable pipeline identity. |
| `Description` | Human explanation, not runtime logic. |
| `TaskList` | Ordered task declarations. Ordered collections must remain explicit. |
| `TaskDependencyList` | Optional explicit dependencies when simple order is not enough. |
| `ConnectionReferenceList` | Named environment-variable references used by tasks. |
| `ParameterList` | Runtime values the orchestrator or caller must provide, when needed. |
| `EvidenceContract` | The minimum runtime facts the pipeline promises to report. |

The pipeline can declare structure.
It should not decide scheduling, dependency freshness, approval workflow, tenant fan-out, or environment promotion by itself.

## Task

Every task should have the same backbone:

| Field | Intent |
| --- | --- |
| `Name` | Stable task identity inside the pipeline. |
| Task detail entity | The concrete task family, for example `TransformExecutionTask` or `TargetWriteTask`. |
| `InputList` | Declared inputs such as row streams, target objects, source objects, or run values. |
| `OutputList` | Declared outputs such as row streams, target effects, evidence, or watermarks. |
| `DependsOnTaskList` | Explicit dependency references. |
| Failure behavior | A concrete policy entity when non-default behavior is needed. |
| `EvidenceContract` | Task-level counters, timings, fingerprints, and failure classification. |

This keeps task execution boring.
Different ETL semantics live in concrete task and operation entities, not in a string-valued `Kind` field or a single clever executor.

## Data Carriers

The first data carrier should remain `RowStream`.

| Carrier | Meaning |
| --- | --- |
| `RowStream` | Rows with a declared `RowStreamShape`, read in bounded buffers. |
| `DataArtifact` | A named external/staged artifact such as a file, object-store object, or staging table. |
| `ControlValue` | A scalar runtime value such as a watermark, source artifact identity, or row count. |
| `Evidence` | Runtime facts emitted by a task or pipeline run. |

Stage 1 is database-first, so `RowStream` is the important carrier now.
Other carriers can be modeled later when file, API, message, or object-store sources become real.

## Task Families

These families map the broad `META-LOAD.md` concern list into model space.
They are not all stage 1 work.

| Family | What it covers | Example task kinds |
| --- | --- | --- |
| Source acquisition | Making source data available. | database read, file read, API page read, message consume, manifest wait, decompression, decryption |
| Change detection | Deciding what changed. | watermark select, CDC token read, full compare, hash compare, dedupe, late-arrival overlap |
| Transform execution | Producing a row stream from sanctioned transform logic. | execute `TransformScript`, normalize, canonicalize, derive columns |
| Reference resolution | Resolving keys and reference data. | dimension lookup, inferred member create, unknown-key stamp, foreign-key repair |
| Validation | Checking shape, data quality, and business contract. | schema conformance, nullability check, duplicate check, control total check, reconciliation |
| Target write | Applying a row stream or artifact to a target. | bulk insert, append, truncate-reload, merge/upsert, update-only, delete-insert, partition overwrite |
| Temporal write | Applying history-aware target semantics. | SCD Type 1, SCD Type 2, close current row, reopen history, snapshot fact load |
| Data Vault write | Applying Data Vault loading patterns. | hub load, link load, satellite load, PIT/bridge refresh |
| Maintenance | Preparing or repairing platform objects. | disable indexes, rebuild indexes, update stats, refresh aggregate, re-enable constraints |
| Evidence/state | Recording runtime facts. | log run start/end, record counters, write watermark, checkpoint, audit stamp |
| Error routing | Handling rejected or unresolved data. | quarantine rows, write reject table, emit warning, poison-batch mark |
| Custom extension | Approved custom behavior. | pre-read hook, post-write hook, custom validation, custom conflict resolution |

The model can know these families exist without implementing them all now.
The first concrete family pair is transform execution plus target write.

## Target Write Operations

Target writes should be modeled as concrete operation entities under a target write task.

| Operation | Meaning |
| --- | --- |
| `InsertRowsTargetWriteTask` | Insert the incoming row stream into the target. Current modeled operation. |
| `Append` | Append without replacing existing rows, with explicit duplicate policy. |
| `TruncateReload` | Clear target or partition, then insert. This is a composed operation, not hidden behavior. |
| `FullReplace` | Replace an entire target using staging or atomic publish where supported. |
| `MergeUpsert` | Match existing rows and insert/update according to explicit keys and update rules. |
| `UpdateOnly` | Update existing rows only. |
| `DeleteInsert` | Delete matching target rows and insert replacements. |
| `SoftDelete` | Mark target rows inactive or deleted. |
| `SCD` | Apply history-aware dimension behavior. |
| `PartitionWrite` | Append, overwrite, exchange, or publish partition-scoped data. |
| `DataVaultLoad` | Apply hub/link/satellite style insert semantics. |

Each operation entity should declare its match keys, allowed side effects, transaction scope, idempotency claim, and required evidence when those concepts matter.
It should not silently choose destructive behavior.

## Change and Temporal Semantics

Change detection and temporal behavior should be explicit task semantics, not accidental SQL side effects.

Some source-side delta logic can live inside a `TransformScript`.
That is acceptable when the transform owns the query semantics.

If `MetaPipeline` itself owns a watermark, CDC token, replay window, SCD rule, or historical correction policy, that ownership must be modeled as a task or operation.

No generic stable ordering or watermark requirement should be imposed on arbitrary SQL transforms.
Ordering and watermarks matter only when a task kind claims restartability, replay, delta loading, or temporal correctness that depends on them.

## Validation and Quality

Validation should be a task family, not a side effect sprinkled through writers.

The model should support:

| Validation level | Examples |
| --- | --- |
| Structural | required columns, type compatibility, length/precision/scale fit, nullability |
| Contract | unique key, reference readiness, duplicate detection, source schema version |
| Business | domain checks, value dependencies, reconciliation, control totals |
| Runtime | row count thresholds, reject thresholds, warning vs error severity |

Stage 1 already performs minimal structural shape checks.
Broader validation should become declared validation tasks.

## Runtime State and Evidence

Runtime state is evidence first, operational database later.

The model should distinguish:

| Concept | Meaning |
| --- | --- |
| `PipelineRun` | One execution attempt. |
| `TaskRun` | One task execution attempt. |
| `RunInput` | Parameter values, connection reference names, source artifacts, target selection. |
| `RunEvidence` | counts, duration, fingerprints, failure stage, warnings, target effects. |
| `Checkpoint` | Durable restart marker, only when a task kind supports it. |
| `Watermark` | Durable change marker, only when change detection owns it. |

The current runtime should remain in-memory.
An operational database can be added later without changing the model premise.

## Connection Handling

Connection strings are not stored in sanctioned metadata or artifacts.

`ConnectionReference` values name shell-visible environment variables.
Runtime resolves those names to connection strings immediately before execution.
Errors must name the missing or empty variable, not print the secret value.

## Orchestration Boundary

`MetaPipeline` can declare internal task order.

The orchestrator owns:

| Orchestrator concern | Reason |
| --- | --- |
| Schedules and triggers | These decide when work should happen. |
| Cross-pipeline dependency freshness | This needs global state and policy. |
| Tenant/partition fan-out decisions | These choose many executions from one definition. |
| Approval workflow | This is governance and operator interaction. |
| Resource queues and priorities | These are platform scheduling concerns. |
| Environment promotion | This is release/deployment concern. |

The pipeline may expose metadata the orchestrator can use.
It should not become the scheduler brain.

## Stage 1 Model Nucleus

The smallest honest model nucleus is:

| Model piece | Current status |
| --- | --- |
| `Pipeline` | Modeled as an explicit row added to a `MetaPipeline` workspace. |
| `TransformExecutionTask` | Modeled; runtime still executes through explicit `TransformScript` and `TransformBinding` selection. |
| `RowStreamShape` | Implemented in core runtime. |
| `RowStreamBatch` | Implemented as `PipelineDataBatch`. |
| `TargetWriteTask` | Modeled and backed by `IPipelineTargetWriteOperation`. |
| `InsertRowsTargetWriteTask` | Modeled as the first concrete target write operation. SQL Server currently realizes it through bulk copy. |
| Run evidence | Minimal in-memory result exists; sanctioned run-state XML is not modeled yet. |

The initial instance CLI supports:

```text
meta-pipeline init --new-workspace <path>
meta-pipeline add-pipeline --workspace <path> --name <name> [--description <text>]
meta-pipeline add-transform --workspace <path> --pipeline <name> --task <name> --transform-workspace <path> --binding-workspace <path> --transform-script-id <id> --transform-binding-id <id> --source-connection-ref <name> --source-connection-env <name> --target-connection-ref <name> --target-connection-env <name> [--target <sql-identifier>]
meta-pipeline inspect --workspace <path>
meta-pipeline execute --workspace <path> --pipeline <name> --task <name> --transform-workspace <path> --binding-workspace <path>
```

`execute` reads the declared transform task, resolves the modeled connection references through environment variables, and executes the first supported target write task: `InsertRowsTargetWriteTask`.

## Anti-Patterns

Do not encode broad ETL concerns as one giant `PipelineOptions` blob.

Do not introduce ad-hoc JSON or blob artifacts as product truth.

Do not hide destructive target behavior inside a friendly operation name.

Do not infer lineage or dependencies from SQL text as product truth.

Do not store connection strings in pipeline metadata.

Do not make orchestration intelligence part of the plain executor.

Do not model every `META-LOAD.md` concern as a first-class class before there is a concrete task that needs it.
